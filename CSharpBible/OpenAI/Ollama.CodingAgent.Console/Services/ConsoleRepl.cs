using Ollama.CodingAgent.Console.Configuration;
using Ollama.CodingAgent.Console.Commands;
using Ollama.CodingAgent.Console.Interfaces;
using Ollama.CodingAgent.Console.Infrastructure;
using Ollama.CodingAgent.Console.Presentation;
using Ollama.CodingAgent.Console.Services;
using Ollama.CodingAgent.Models;
using Ollama.CodingAgent.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Interfaces;
using Ollama.CodingAgent.Application.Diagnostics;
using Ollama.CodingAgent.Application.Extensions;
using Ollama.CodingAgent.Application.Interfaces;
using Ollama.CodingAgent.Application.Models;
using Ollama.CodingAgent.Application.ViewModels;
using Ollama.CodingAgent;

namespace Ollama.CodingAgent.Console.Services;

/// <summary>
/// Hosts the line-oriented terminal experience over the shared application view model.
/// </summary>
public sealed class ConsoleRepl
{
    private readonly AgentSessionViewModel _session;
    private readonly IAgentApprovalService _approvalService;
    private readonly IConsole _console;
    private readonly Func<string?, ConsoleCommandParseResult> _commandParser;
    private readonly Func<bool> _isInputRedirected;
    private readonly ConsoleRuntimeConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConsoleRepl"/> class.
    /// </summary>
    public ConsoleRepl(
        AgentSessionViewModel session,
        IAgentApprovalService approvalService,
        IConsole console,
        Func<string?, ConsoleCommandParseResult>? commandParser = null)
        : this(session, approvalService, console, commandParser, null, null, null)
    {
    }

    /// <summary>
    /// Initializes a new instance with a live diagnostics channel.
    /// </summary>
    public ConsoleRepl(
        AgentSessionViewModel session,
        IAgentApprovalService approvalService,
        IConsole console,
        Func<string?, ConsoleCommandParseResult>? commandParser,
        AgentDiagnosticsChannel? diagnosticsChannel,
        Func<bool>? isInputRedirected = null,
        ConsoleRuntimeConfiguration? configuration = null)
    {
        _session = session ?? throw new ArgumentNullException(nameof(session));
        _approvalService = approvalService ?? throw new ArgumentNullException(nameof(approvalService));
        _console = console ?? throw new ArgumentNullException(nameof(console));
        _commandParser = commandParser ?? ConsoleCommandParser.Parse;
        _isInputRedirected = isInputRedirected ?? (() => System.Console.IsInputRedirected || System.Console.IsOutputRedirected);
        _configuration = configuration ?? new ConsoleRuntimeConfiguration(_session.WorkspacePath, string.Empty, string.Empty);
        if (diagnosticsChannel is not null)
        {
            diagnosticsChannel.EventRecorded += OnDiagnosticEventRecorded;
        }
    }

    private void OnDiagnosticEventRecorded(object? sender, AgentDiagnosticEvent diagnosticEvent)
    {
        string detail = string.IsNullOrWhiteSpace(diagnosticEvent.Detail)
            ? string.Empty
            : $" {diagnosticEvent.Detail}";
        _console.WriteLine($"[agent:{diagnosticEvent.EventName}] {detail}".TrimEnd());
    }

    /// <summary>
    /// Runs the persistent terminal session until the operator exits or input closes.
    /// </summary>
    public async Task RunAsync()
    {
        ConsoleCancelEventHandler cancelHandler = OnCancelKeyPress;
        System.Console.CancelKeyPress += cancelHandler;
        try
        {
            _console.WriteLine("Ollama Coding Agent terminal session. Type :help for commands.");
            _console.WriteLine($"Session '{_session.SessionId}' in '{_session.WorkspacePath}'.");
            while (true)
            {
                _console.Write("agent> ");
                string line = _console.ReadLine();
                if (line is null || (string.IsNullOrEmpty(line) && _isInputRedirected()))
                {
                    return;
                }

                ConsoleCommandParseResult parsedResult = _commandParser(line);
                if (!parsedResult.Success)
                {
                    _console.WriteLine($"Input error: {parsedResult.Error}");
                    continue;
                }

                if (await ExecuteAsync(parsedResult.Command))
                {
                    return;
                }
            }
        }
        finally
        {
            System.Console.CancelKeyPress -= cancelHandler;
        }
    }

    private async Task<bool> ExecuteAsync(ConsoleCommand command)
    {
        switch (command.Kind)
        {
            case ConsoleCommandKind.Empty:
                return false;
            case ConsoleCommandKind.Prompt:
                await SubmitPromptAsync(command.Argument);
                return false;
            case ConsoleCommandKind.Help:
                PrintHelp();
                return false;
            case ConsoleCommandKind.Config:
                Configure(command.Argument);
                return false;
            case ConsoleCommandKind.Status:
                _console.WriteLine(AgentSessionProjection.FormatStatus(_session));
                return false;
            case ConsoleCommandKind.Transcript:
                PrintTranscript();
                return false;
            case ConsoleCommandKind.Reload:
                await ExecuteViewModelCommandAsync(_session.ReloadCommand.ExecuteAsync(null));
                return false;
            case ConsoleCommandKind.Clear:
                await ExecuteViewModelCommandAsync(_session.ClearCommand.ExecuteAsync(null));
                return false;
            case ConsoleCommandKind.Cancel:
                if (_session.IsRunning)
                {
                    _session.CancelCommand.Execute(null);
                    _console.WriteLine("Cancellation requested.");
                }

                else
                {
                    _console.WriteLine("No agent request is active.");
                }

                return false;
            case ConsoleCommandKind.Approvals:
                _console.WriteLine(AgentSessionProjection.FormatApprovals(_approvalService.PendingRequests));
                return false;
            case ConsoleCommandKind.Approve:
                ResolveApproval(command.Argument, approved: true);
                return false;
            case ConsoleCommandKind.Reject:
                ResolveApproval(command.Argument, approved: false);
                return false;
            case ConsoleCommandKind.Exit:
                return true;
            default:
                _console.WriteLine("Unsupported command. Use :help.");
                return false;
        }
    }

    private void Configure(string? argument)
    {
        string[] values = (argument ?? string.Empty).Split('|', StringSplitOptions.TrimEntries);
        if (values.Length != 3 || values.Any(string.IsNullOrWhiteSpace))
        {
            _console.WriteLine("Usage: /config <endpoint> | <model> | <workspace>");
            return;
        }

        try
        {
            _configuration.Set(values[0], values[1], values[2]);
            _session.WorkspacePath = _configuration.WorkspacePath;
            _console.WriteLine($"Configuration applied for future prompts: {_configuration.Endpoint}, {_configuration.Model}, {_configuration.WorkspacePath}");
        }
        catch (ArgumentException exception)
        {
            _console.WriteLine($"Configuration invalid: {exception.Message}");
        }
    }

    private async Task SubmitPromptAsync(string? prompt)
    {
        if (string.IsNullOrWhiteSpace(prompt))
        {
            _console.WriteLine("Prompt text is required.");
            return;
        }

        int existingTurnCount = _session.Conversation.Count;
        _session.Prompt = prompt;
        await ExecuteViewModelCommandAsync(_session.SubmitCommand.ExecuteAsync(null));
        AgentConversationTurn? latestResponse = _session.Conversation.Skip(existingTurnCount).LastOrDefault(
            turn => turn.Role is AgentConversationRole.Assistant or AgentConversationRole.System);
        if (latestResponse is not null)
        {
            _console.WriteLine($"{(latestResponse.Role == AgentConversationRole.Assistant ? "assistant" : "system")}> {latestResponse.Content}");
        }
    }

    private async Task ExecuteViewModelCommandAsync(Task commandTask)
    {
        try
        {
            await commandTask;
            _console.WriteLine(_session.Status);
        }
        catch (Exception ex)
        {
            _console.WriteLine($"Operation failed: {ex.Message}");
        }
    }

    private void ResolveApproval(string? requestId, bool approved)
    {
        if (string.IsNullOrWhiteSpace(requestId) || !_session.ResolveApproval(requestId, approved))
        {
            _console.WriteLine($"No pending approval with id '{requestId}'.");
            return;
        }

        _console.WriteLine(approved ? "Operation approved." : "Operation rejected.");
    }

    private void PrintTranscript()
    {
        string transcript = AgentSessionProjection.FormatTranscript(_session.Conversation);
        _console.WriteLine(string.IsNullOrEmpty(transcript) ? "The transcript is empty." : transcript);
    }

    private void OnCancelKeyPress(object? sender, ConsoleCancelEventArgs eventArgs)
    {
        if (!_session.IsRunning)
        {
            return;
        }

        eventArgs.Cancel = true;
        _session.CancelCommand.Execute(null);
        _console.WriteLine("Cancellation requested.");
    }

    private void PrintHelp()
    {
        _console.WriteLine("""
            Enter text to submit an agent prompt.
            :help                 Show this help.
             /config <endpoint> | <model> | <workspace>
                                   Apply configuration for future prompts.
            :status               Show session status.
            :transcript           Show visible conversation history.
            :reload               Reload the persisted session.
            :clear                Clear and persist the session.
            :cancel               Request cancellation of an active prompt.
            :approvals            List pending mutation approvals.
            :approve <id>         Approve one pending operation.
            :reject <id>          Reject one pending operation.
            :exit                 End this terminal session.

            Ctrl+C requests cancellation while a prompt is running. Model thinking is displayed in the transcript.
            """);
    }
}
