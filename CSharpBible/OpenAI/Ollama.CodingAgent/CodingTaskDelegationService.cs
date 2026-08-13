using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Ollama.Client.Models;
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

    /// <summary>
    /// Initializes a new instance of the <see cref="CodingTaskDelegationService"/> class.
    /// </summary>
    public CodingTaskDelegationService(
        IAgentModelClient agentModelClient,
        OllamaToolChatRunnerAdapter toolChatRunnerAdapter,
        IOllamaToolRegistry toolRegistry,
        OllamaToolOrchestrator toolOrchestrator,
        OllamaAgentRuntimeSettings runtimeSettings)
    {
        _agentModelClient = agentModelClient ?? throw new ArgumentNullException(nameof(agentModelClient));
        _toolChatRunnerAdapter = toolChatRunnerAdapter ?? throw new ArgumentNullException(nameof(toolChatRunnerAdapter));
        _toolRegistry = toolRegistry ?? throw new ArgumentNullException(nameof(toolRegistry));
        _toolOrchestrator = toolOrchestrator ?? throw new ArgumentNullException(nameof(toolOrchestrator));
        _runtimeSettings = runtimeSettings ?? throw new ArgumentNullException(nameof(runtimeSettings));
    }

    /// <summary>
    /// Runs delegated coding-task mode.
    /// </summary>
    /// <param name="userPrompt">The user coding request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The run result.</returns>
    public async Task<AgentRunResult> RunDelegatedAsync(string userPrompt, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userPrompt);

        List<DelegatedToolStep> steps = await RunDelegatedStepsAsync(userPrompt, cancellationToken);
        AgentRunResult result;
        try
        {
            string summaryPrompt = BuildSummaryPrompt(userPrompt, steps);
            AgentRunner agentRunner = new(_agentModelClient, _runtimeSettings);
            result = await agentRunner.RunAsync(new AgentRunRequest
            {
                Prompt = summaryPrompt,
                SystemPrompt = "You are a coding agent. Provide a concise final answer with [[FINAL]].",
            }, cancellationToken);
        }
        catch
        {
            result = CreateFallbackSummaryResult(steps);
        }

        string enriched = BuildDelegationReport(steps) + "\n\n" + $"Agent summary:\n{result.FinalResponse}";
        return new AgentRunResult
        {
            FinalResponse = enriched,
            IterationsUsed = result.IterationsUsed,
            RetryAttemptsUsed = result.RetryAttemptsUsed,
            FinalizedWithMarker = result.FinalizedWithMarker,
        };
    }

    private async Task<List<DelegatedToolStep>> RunDelegatedStepsAsync(string userPrompt, CancellationToken cancellationToken)
    {
        List<DelegatedToolStep> steps = [];
        for (int stepIndex = 1; stepIndex <= MaxDelegatedToolSteps; stepIndex++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            OllamaToolInvocationResult toolResult = await SelectAndExecuteToolAsync(userPrompt, steps, cancellationToken);
            steps.Add(new DelegatedToolStep
            {
                StepIndex = stepIndex,
                ToolName = toolResult.ToolName,
                Success = toolResult.Success,
                Output = NormalizeToolOutput(toolResult),
            });

            if (string.Equals(toolResult.ToolName, "none", StringComparison.OrdinalIgnoreCase))
            {
                break;
            }
        }

        return steps;
    }

    private async Task<OllamaToolInvocationResult> SelectAndExecuteToolAsync(
        string userPrompt,
        IReadOnlyList<DelegatedToolStep> previousSteps,
        CancellationToken cancellationToken)
    {
        string instructions = OllamaToolPromptBuilder.BuildToolInstructions(_toolRegistry);
        string selectionPrompt = BuildSelectionPrompt(userPrompt, previousSteps);

        OllamaChatCompletion completion;
        try
        {
            using CancellationTokenSource delegationStepTimeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            delegationStepTimeoutCts.CancelAfter(_runtimeSettings.StepTimeout);
            completion = await _toolChatRunnerAdapter.CompleteChatAsync(new Ollama.Client.ChatCompletionOptions
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
            }, delegationStepTimeoutCts.Token);
        }
        catch (Exception ex)
        {
            return new OllamaToolInvocationResult
            {
                ToolName = "none",
                Success = false,
                Error = $"Delegation selection failed: {ex.Message}",
                Output = string.Empty,
            };
        }

        try
        {
            OllamaToolCall parsedToolCall = ToolCallParser.Parse(completion.Content);
            if (string.Equals(parsedToolCall.ToolName, "none", StringComparison.OrdinalIgnoreCase))
            {
                return new OllamaToolInvocationResult
                {
                    ToolName = "none",
                    Success = true,
                    Output = "No further delegated tool step requested by model.",
                };
            }

            return await _toolOrchestrator.ExecuteAsync(parsedToolCall, cancellationToken);
        }
        catch (Exception ex)
        {
            return new OllamaToolInvocationResult
            {
                ToolName = "none",
                Success = false,
                Error = $"Delegation fallback: {ex.Message}",
                Output = completion.Content ?? string.Empty,
            };
        }
    }

    private static AgentRunResult CreateFallbackSummaryResult(IReadOnlyList<DelegatedToolStep> steps)
    {
        string nextStep = steps
            .FirstOrDefault(static step => step.Success && !string.Equals(step.ToolName, "none", StringComparison.OrdinalIgnoreCase))
            ?.ToolName ?? "none";
        string summary = nextStep == "none"
            ? "Delegation completed without a successful tool step. Next implementation step: run `list_workspace_files` first, then `read_workspace_file` on the target file and continue with a focused code change."
            : $"Delegation completed. Next implementation step: continue from tool `{nextStep}` output and apply the smallest focused C# code change required by the task.";

        return new AgentRunResult
        {
            FinalResponse = summary,
            IterationsUsed = 0,
            RetryAttemptsUsed = 0,
            FinalizedWithMarker = false,
        };
    }

    private static string BuildSelectionPrompt(string userPrompt, IReadOnlyList<DelegatedToolStep> previousSteps)
    {
        List<string> lines =
        [
            "Coding task:",
            userPrompt,
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
            BuildDelegationReport(steps),
            string.Empty,
            "Now provide a short implementation-oriented answer for the user.",
        ];

        return string.Join("\n", lines);
    }

    private static string BuildDelegationReport(IReadOnlyList<DelegatedToolStep> steps)
    {
        if (steps.Count == 0)
        {
            return "No delegated tool steps were executed.";
        }

        StringBuilder builder = new();
        foreach (DelegatedToolStep step in steps.OrderBy(static step => step.StepIndex))
        {
            builder.AppendLine($"Step {step.StepIndex}: {step.ToolName} | success={step.Success}");
            if (!string.IsNullOrWhiteSpace(step.Output))
            {
                builder.AppendLine(step.Output);
            }

            builder.AppendLine();
        }

        return builder.ToString().Trim();
    }

    private static string NormalizeToolOutput(OllamaToolInvocationResult toolResult)
    {
        if (toolResult.Success)
        {
            return toolResult.Output;
        }

        if (!string.IsNullOrWhiteSpace(toolResult.Error))
        {
            return toolResult.Error;
        }

        return toolResult.Output;
    }
}
