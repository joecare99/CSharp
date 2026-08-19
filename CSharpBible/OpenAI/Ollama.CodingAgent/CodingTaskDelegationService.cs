using Ollama.CodingAgent.Models;
using Ollama.CodingAgent.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Ollama.Client.Models;
using Ollama.Tools.Abstractions;
using Ollama.Tools;

namespace Ollama.CodingAgent;

/// <summary>
/// Executes delegated coding subtasks through a bounded multi-step tool loop and summarizes the outcome.
/// </summary>
public sealed class CodingTaskDelegationService
{
    private const int MaxDelegatedToolSteps = 3;

    private readonly IAgentModelClient _agentModelClient;
    private readonly OllamaToolChatRunnerAdapter _toolChatRunnerAdapter;
    private readonly IOllamaToolRegistry _toolRegistry;
    private readonly OllamaToolOrchestrator _toolOrchestrator;
    private readonly OllamaAgentRuntimeSettings _runtimeSettings;
    private readonly IAgentDiagnosticsSink? _diagnosticsSink;

    /// <summary>
    /// Initializes a new instance of the <see cref="CodingTaskDelegationService"/> class.
    /// </summary>
    public CodingTaskDelegationService(
        IAgentModelClient agentModelClient,
        OllamaToolChatRunnerAdapter toolChatRunnerAdapter,
        IOllamaToolRegistry toolRegistry,
        OllamaToolOrchestrator toolOrchestrator,
        OllamaAgentRuntimeSettings runtimeSettings)
        : this(agentModelClient, toolChatRunnerAdapter, toolRegistry, toolOrchestrator, runtimeSettings, null)
    {
    }

    /// <summary>
    /// Initializes a new instance with a diagnostic event sink.
    /// </summary>
    public CodingTaskDelegationService(
        IAgentModelClient agentModelClient,
        OllamaToolChatRunnerAdapter toolChatRunnerAdapter,
        IOllamaToolRegistry toolRegistry,
        OllamaToolOrchestrator toolOrchestrator,
        OllamaAgentRuntimeSettings runtimeSettings,
        IAgentDiagnosticsSink? diagnosticsSink)
    {
        _agentModelClient = agentModelClient ?? throw new ArgumentNullException(nameof(agentModelClient));
        _toolChatRunnerAdapter = toolChatRunnerAdapter ?? throw new ArgumentNullException(nameof(toolChatRunnerAdapter));
        _toolRegistry = toolRegistry ?? throw new ArgumentNullException(nameof(toolRegistry));
        _toolOrchestrator = toolOrchestrator ?? throw new ArgumentNullException(nameof(toolOrchestrator));
        _runtimeSettings = runtimeSettings ?? throw new ArgumentNullException(nameof(runtimeSettings));
        _diagnosticsSink = diagnosticsSink;
    }

    /// <summary>
    /// Runs delegated coding-task mode.
    /// </summary>
    /// <param name="userPrompt">The user coding request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The run result.</returns>
    public async Task<AgentRunResult> RunDelegatedAsync(string userPrompt, CancellationToken cancellationToken = default)
        => await RunDelegatedAsync(userPrompt, null, cancellationToken);

    /// <summary>
    /// Runs delegated coding-task mode and reports live runtime updates.
    /// </summary>
    public async Task<AgentRunResult> RunDelegatedAsync(
        string userPrompt,
        Action<AgentRuntimeUpdate>? onUpdate,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userPrompt);

        PlanState planState = SubtaskPlanner.CreateInitialPlan(userPrompt);
        string correlationId = Guid.NewGuid().ToString("N");
        _diagnosticsSink?.Record(new AgentDiagnosticEvent
        {
            CorrelationId = correlationId,
            EventName = "delegation.started",
            Detail = "Delegated tool execution started.",
        });
        List<DelegatedToolStep> steps = await RunDelegatedStepsWithCorrelationAsync(planState, correlationId, cancellationToken, onUpdate);
        AgentRunResult result;
        try
        {
            string summaryPrompt = BuildSummaryPrompt(userPrompt, steps);
            AgentRunner agentRunner = new(_agentModelClient, _runtimeSettings);
            result = await agentRunner.RunAsync(new AgentRunRequest
            {
                Prompt = summaryPrompt,
                SystemPrompt = "You are a coding agent. Provide a concise final answer with [[FINAL]].",
            }, cancellationToken, onUpdate);
        }
        catch
        {
            result = CreateFallbackSummaryResult(steps);
        }

        List<string> thinking = result.Thinking
            .Concat(steps.SelectMany(static step => step.Thinking))
            .ToList();
        string enriched = _runtimeSettings.Verbosity == AgentVerbosity.Quiet
            ? result.FinalResponse
            : PlanStateRenderer.Render(planState)
                + "\n\n"
                + BuildDelegationReport(steps, _runtimeSettings.Verbosity == AgentVerbosity.Verbose)
                + "\n\n"
                + $"Agent summary:\n{result.FinalResponse}";
        return new AgentRunResult
        {
            FinalResponse = enriched,
            IterationsUsed = result.IterationsUsed,
            RetryAttemptsUsed = result.RetryAttemptsUsed,
            FinalizedWithMarker = result.FinalizedWithMarker,
            Thinking = thinking,
        };
    }

    private async Task<List<DelegatedToolStep>> RunDelegatedStepsWithCorrelationAsync(PlanState planState, string correlationId, CancellationToken cancellationToken, Action<AgentRuntimeUpdate>? onUpdate = null)
    {
        List<DelegatedToolStep> steps = [];
        for (int stepIndex = 1; stepIndex <= MaxDelegatedToolSteps; stepIndex++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            PlannedSubtask? plannedSubtask = planState.GetReadySubtasks().FirstOrDefault();
            if (plannedSubtask is null)
            {
                break;
            }

            plannedSubtask.Status = PlannedSubtaskStatus.InProgress;
            Stopwatch stopwatch = Stopwatch.StartNew();
            (OllamaToolInvocationResult toolResult, IReadOnlyList<string> selectionThinking) = await SelectAndExecuteToolAsync(
                planState.GoalContract.Objective,
                plannedSubtask,
                steps,
                correlationId,
                cancellationToken,
                onUpdate);
            stopwatch.Stop();
            bool driftDetected = GoalDriftAnalyzer.IsDriftDetected(planState.GoalContract, plannedSubtask, NormalizeToolOutput(toolResult));
            plannedSubtask.Status = toolResult.Success && !driftDetected ? PlannedSubtaskStatus.Done : PlannedSubtaskStatus.Blocked;
            steps.Add(new DelegatedToolStep
            {
                StepIndex = stepIndex,
                ToolName = toolResult.ToolName,
                Success = toolResult.Success,
                Output = NormalizeToolOutput(toolResult),
                Input = toolResult.Input,
                Duration = stopwatch.Elapsed,
                Thinking = selectionThinking,
            });

            if (string.Equals(toolResult.ToolName, "none", StringComparison.OrdinalIgnoreCase) || !toolResult.Success || driftDetected)
            {
                break;
            }
        }

        return steps;
    }

    private Task<List<DelegatedToolStep>> RunDelegatedStepsAsync(PlanState planState, CancellationToken cancellationToken)
        => RunDelegatedStepsWithCorrelationAsync(planState, Guid.NewGuid().ToString("N"), cancellationToken);

    private async Task<(OllamaToolInvocationResult Result, IReadOnlyList<string> Thinking)> SelectAndExecuteToolAsync(
        string userPrompt,
        PlannedSubtask plannedSubtask,
        IReadOnlyList<DelegatedToolStep> previousSteps,
        string correlationId,
        CancellationToken cancellationToken,
        Action<AgentRuntimeUpdate>? onUpdate = null)
    {
        string instructions = OllamaToolPromptBuilder.BuildToolInstructions(_toolRegistry);
        string selectionPrompt = BuildSelectionPrompt(userPrompt, plannedSubtask, previousSteps);
        IReadOnlyList<OllamaChatTool> chatTools = OllamaToolChatDefinitionBuilder.Build(_toolRegistry);
        _diagnosticsSink?.Record(new AgentDiagnosticEvent
        {
            CorrelationId = correlationId,
            EventName = "tool.selection.requested",
            Detail = "Tool definitions were included in the model request.",
            Data = new Dictionary<string, string>
            {
                ["toolNames"] = string.Join(",", chatTools.Select(static tool => tool.Name)),
            },
        });

        OllamaChatCompletion completion;
        try
        {
            using CancellationTokenSource delegationStepTimeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            delegationStepTimeoutCts.CancelAfter(_runtimeSettings.StepTimeout);
            Ollama.Client.Models.ChatCompletionOptions chatOptions = new()
            {
                Messages =
                [
                    new Ollama.Client.Models.OllamaClientChatMessage
                    {
                        Role = "system",
                        Content = instructions + "\nReturn ONLY a JSON object for one tool call. If no further tool is needed, use {\"toolName\":\"none\",\"input\":\"\"}.",
                    },
                    new Ollama.Client.Models.OllamaClientChatMessage
                    {
                        Role = "user",
                        Content = selectionPrompt,
                    },
                ],
                Tools = chatTools,
            };
            completion = _toolChatRunnerAdapter is IStreamingOllamaToolChatRunner streamingToolRunner && onUpdate is not null
                ? await streamingToolRunner.CompleteChatAsync(chatOptions, fragment => onUpdate(new AgentRuntimeUpdate
                {
                    Kind = AgentRuntimeUpdateKind.Thinking,
                    Content = fragment,
                }), delegationStepTimeoutCts.Token)
                : await _toolChatRunnerAdapter.CompleteChatAsync(chatOptions, delegationStepTimeoutCts.Token);
            foreach (string fragment in completion.Thinking.Where(static fragment => !string.IsNullOrWhiteSpace(fragment)))
            {
                onUpdate?.Invoke(new AgentRuntimeUpdate
                {
                    Kind = AgentRuntimeUpdateKind.Thinking,
                    Content = fragment,
                });
            }
            _diagnosticsSink?.Record(new AgentDiagnosticEvent
            {
                CorrelationId = correlationId,
                EventName = "tool.selection.completed",
                Detail = "The model returned a tool selection response.",
                Data = new Dictionary<string, string>
                {
                    ["nativeToolCalls"] = completion.ToolCalls.Count.ToString(),
                },
            });
        }
        catch (OperationCanceledException)
        {
            _diagnosticsSink?.Record(new AgentDiagnosticEvent
            {
                CorrelationId = correlationId,
                EventName = "tool.selection.cancelled",
                Detail = "Tool selection was cancelled.",
            });
            throw;
        }
        catch (Exception ex)
        {
            OllamaToolCall fallbackCall = DelegationFallbackToolPlanner.CreateFallbackToolCall(userPrompt);
            _diagnosticsSink?.Record(new AgentDiagnosticEvent
            {
                CorrelationId = correlationId,
                EventName = "tool.selection.fallback",
                Detail = "Model tool selection failed; deterministic fallback selected.",
                Error = ex.Message,
                Data = new Dictionary<string, string>
                {
                    ["toolName"] = fallbackCall.ToolName,
                },
            });
            OllamaToolInvocationResult fallbackResult = await _toolOrchestrator.ExecuteAsync(fallbackCall, cancellationToken);
            string fallbackOutput = NormalizeToolOutput(fallbackResult);
            return (new OllamaToolInvocationResult
            {
                ToolName = fallbackCall.ToolName,
                Input = fallbackCall.Input,
                Success = fallbackResult.Success,
                Error = fallbackResult.Success ? null : $"Delegation selection failed: {ex.Message}",
                Output = fallbackResult.Success
                    ? $"Fallback tool execution after selection failure: {fallbackOutput}"
                    : $"Delegation selection failed: {ex.Message}\nFallback tool output: {fallbackOutput}",
            }, Array.Empty<string>());
        }

        try
        {
            OllamaToolCall parsedToolCall = TryGetNativeToolCall(completion.ToolCalls)
                ?? ToolCallParser.Parse(completion.Content);
            if (string.Equals(parsedToolCall.ToolName, "none", StringComparison.OrdinalIgnoreCase))
            {
                return (new OllamaToolInvocationResult
                {
                    ToolName = "none",
                    Success = true,
                    Output = "No further delegated tool step requested by model.",
                }, completion.Thinking);
            }

            _diagnosticsSink?.Record(new AgentDiagnosticEvent
            {
                CorrelationId = correlationId,
                EventName = "tool.call.requested",
                Detail = "The model requested a registered tool.",
                Data = new Dictionary<string, string>
                {
                    ["toolName"] = parsedToolCall.ToolName,
                    ["input"] = parsedToolCall.Input,
                },
            });
            onUpdate?.Invoke(new AgentRuntimeUpdate
            {
                Kind = AgentRuntimeUpdateKind.Tool,
                Content = $"Tool requested: {parsedToolCall.ToolName}",
            });
            OllamaToolInvocationResult invocation = await _toolOrchestrator.ExecuteAsync(parsedToolCall, cancellationToken);
            _diagnosticsSink?.Record(new AgentDiagnosticEvent
            {
                CorrelationId = correlationId,
                EventName = "tool.call.completed",
                Detail = invocation.Success ? "Tool execution completed." : "Tool execution failed.",
                Error = invocation.Success ? null : invocation.Error,
                Data = new Dictionary<string, string>
                {
                    ["toolName"] = invocation.ToolName,
                    ["success"] = invocation.Success.ToString(),
                },
            });
            onUpdate?.Invoke(new AgentRuntimeUpdate
            {
                Kind = AgentRuntimeUpdateKind.Tool,
                Content = invocation.Success
                    ? $"Tool completed: {invocation.ToolName}"
                    : $"Tool failed: {invocation.ToolName} - {invocation.Error}",
            });
            return (invocation, completion.Thinking);
        }

        catch (Exception ex)
        {
            return (new OllamaToolInvocationResult
            {
                ToolName = "none",
                Success = false,
                Error = $"Delegation fallback: {ex.Message}",
                Output = completion.Content ?? string.Empty,
            }, completion.Thinking);
        }
    }

    private static OllamaToolCall? TryGetNativeToolCall(IReadOnlyList<OllamaChatToolCall> toolCalls)
    {
        OllamaChatToolCall? call = toolCalls.FirstOrDefault();
        return call is null
            ? null
            : new OllamaToolCall
            {
                ToolName = call.Name,
                Input = call.Arguments,
            };
    }

    private static AgentRunResult CreateFallbackSummaryResult(IReadOnlyList<DelegatedToolStep> steps)
    {
        string nextStep = steps
            .FirstOrDefault(static step => step.Success && !string.Equals(step.ToolName, "none", StringComparison.OrdinalIgnoreCase))
            ?.ToolName ?? "none";
        string summary = nextStep == "none"
            ? "Delegation completed without a successful tool step. Next implementation step: run list_workspace_files first, then read_workspace_file on the target file and continue with a focused code change."
            : $"Delegation completed. Next implementation step: continue from tool {nextStep} output and apply the smallest focused C# code change required by the task.";

        return new AgentRunResult
        {
            FinalResponse = summary,
            IterationsUsed = 0,
            RetryAttemptsUsed = 0,
            FinalizedWithMarker = false,
        };
    }

    private static string BuildSelectionPrompt(string userPrompt, PlannedSubtask plannedSubtask, IReadOnlyList<DelegatedToolStep> previousSteps)
    {
        List<string> lines =
        [
            "Coding task:",
            userPrompt,
            string.Empty,
            "Current planned subtask:",
            plannedSubtask.Title,
            $"Rationale: {plannedSubtask.Rationale}",
            string.Empty,
            "Previous delegated steps:",
        ];

        if (previousSteps.Count == 0)
        {
            lines.Add("- none");
        }
        else
        {
            foreach (DelegatedToolStep step in previousSteps)
            {
                lines.Add($"- step {step.StepIndex}: tool={step.ToolName}, success={step.Success}");
            }
        }

        lines.Add(string.Empty);
        lines.Add("Select the most useful next delegated step.");
        return string.Join("\n", lines);
    }

    private static string BuildSummaryPrompt(string userPrompt, IReadOnlyList<DelegatedToolStep> steps)
    {
        List<string> lines =
        [
            "Original coding task:",
            userPrompt,
            string.Empty,
            "Delegated tool execution history:",
            BuildDelegationReport(steps, detailed: false),
            string.Empty,
            "Now provide a short implementation-oriented answer for the user.",
        ];

        return string.Join("\n", lines);
    }

    private static string BuildDelegationReport(IReadOnlyList<DelegatedToolStep> steps, bool detailed)
    {
        if (steps.Count == 0)
        {
            return "No delegated tool steps were executed.";
        }

        StringBuilder builder = new();
        foreach (DelegatedToolStep step in steps.OrderBy(static step => step.StepIndex))
        {
            builder.AppendLine($"Step {step.StepIndex}: {step.ToolName} | success={step.Success}");
            if (detailed)
            {
                builder.AppendLine($"  Duration: {step.Duration.TotalMilliseconds:F0} ms");
                builder.AppendLine($"  Validated input: {Truncate(step.Input, 1200)}");
            }

            if (!string.IsNullOrWhiteSpace(step.Output))
            {
                builder.AppendLine(detailed ? Truncate(step.Output, 4000) : step.Output);
            }

            builder.AppendLine();
        }

        return builder.ToString().Trim();
    }

    private static string Truncate(string value, int maximumLength)
    {
        if (value.Length <= maximumLength)
        {
            return value;
        }

        return value[..maximumLength] + "...";
    }

    private static string NormalizeToolOutput(OllamaToolInvocationResult toolResult)
    {
        if (toolResult.Success && string.IsNullOrWhiteSpace(toolResult.Error))
        {
            return toolResult.Output;
        }

        if (!string.IsNullOrWhiteSpace(toolResult.Error) && !string.IsNullOrWhiteSpace(toolResult.Output))
        {
            return $"{toolResult.Error}\n{toolResult.Output}";
        }

        if (!string.IsNullOrWhiteSpace(toolResult.Error))
        {
            return toolResult.Error;
        }

        return toolResult.Output;
    }
}
