using AA98_AvlnCodeStudio.Base.OS.Models;
using AA98_AvlnCodeStudio.Base.OS.Services;
using Avalonia.Threading;
using Avln_TestConsole;
using Avln_TestConsole.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AA98.Terminal.Host.ViewModels;

/// <summary>
/// Coordinates the terminal host UI and the hosted AA98 terminal session.
/// </summary>
public partial class MainWindowViewModel : ViewModelBase, IAsyncDisposable
{
    private readonly ITerminalSessionService _terminalSessionService;
    private readonly StringBuilder _interactiveInputBuffer = new();
    private ITerminalSession? _terminalSession;

    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
    /// </summary>
    /// <param name="terminalSessionService">The terminal session service used to start shells.</param>
    public MainWindowViewModel(ITerminalSessionService terminalSessionService)
    {
        _terminalSessionService = terminalSessionService;
        Console = new AvaloniaTestConsole
        {
            WindowWidth = 120,
            WindowHeight = 40,
        };
        SessionDescription = "No active terminal session.";
    }

    /// <summary>
    /// Gets the reusable console instance displayed by the UI.
    /// </summary>
    public IAvaloniaConsole Console { get; }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(StartCommand))]
    [NotifyCanExecuteChangedFor(nameof(SendInputCommand))]
    [NotifyCanExecuteChangedFor(nameof(StopCommand))]
    private bool _isRunning;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SendInputCommand))]
    private string _inputText = string.Empty;

    [ObservableProperty]
    private string _sessionDescription;

    /// <summary>
    /// Starts the terminal process if it is not already running.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanStart))]
    private async Task StartAsync()
    {
        if (_terminalSession is not null)
        {
            return;
        }

        Console.Clear();
        Console.Title = "AA98 Terminal Host";
        _terminalSession = await _terminalSessionService.StartSessionAsync(new TerminalSessionStartRequest(), CancellationToken.None);
        _terminalSession.OutputReceived += OnOutputReceived;
        _terminalSession.OutputPartialReceived += OnOutputPartialReceived;
        _terminalSession.ErrorReceived += OnErrorReceived;
        _terminalSession.ErrorPartialReceived += OnErrorPartialReceived;
        _terminalSession.Exited += OnExited;
        _interactiveInputBuffer.Clear();
        InputText = string.Empty;
        IsRunning = true;
        SessionDescription = $"Running shell: {_terminalSession.Shell.DisplayName ?? _terminalSession.Shell.ExecutablePath ?? "unknown"}";
        Console.WriteLine("Started terminal session.", ConsoleColor.Gray, ConsoleColor.Black);
    }

    /// <summary>
    /// Processes direct text input coming from the terminal control.
    /// </summary>
    /// <param name="text">The entered text.</param>
    public Task HandleConsoleTextInputAsync(string? text)
    {
        if (!IsRunning || _terminalSession is null || string.IsNullOrEmpty(text))
        {
            return Task.CompletedTask;
        }

        foreach (var character in text)
        {
            if (character == '\r' || character == '\n')
            {
                continue;
            }

            _interactiveInputBuffer.Append(character);
            Console.Write(character);
        }

        InputText = _interactiveInputBuffer.ToString();
        return Task.CompletedTask;
    }

    /// <summary>
    /// Processes direct special-key input coming from the terminal control.
    /// </summary>
    /// <param name="key">The pressed console key.</param>
    public async Task HandleConsoleSpecialKeyAsync(ConsoleKey key)
    {
        if (!IsRunning || _terminalSession is null)
        {
            return;
        }

        switch (key)
        {
            case ConsoleKey.Backspace:
                if (_interactiveInputBuffer.Length > 0)
                {
                    _interactiveInputBuffer.Length -= 1;
                    Console.Write("\b \b");
                    InputText = _interactiveInputBuffer.ToString();
                }

                break;

            case ConsoleKey.Tab:
                _interactiveInputBuffer.Append('\t');
                Console.Write('\t');
                InputText = _interactiveInputBuffer.ToString();
                break;

            case ConsoleKey.Enter:
                var currentInput = _interactiveInputBuffer.ToString();
                await _terminalSession.WriteInputAsync(currentInput, CancellationToken.None);
                _interactiveInputBuffer.Clear();
                InputText = string.Empty;
                Console.WriteLine();
                break;
        }
    }

    /// <summary>
    /// Sends the current input line to the hosted process.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanSendInput))]
    private async Task SendInputAsync()
    {
        if (_terminalSession is null)
        {
            return;
        }

        var currentInput = InputText;
        if (string.IsNullOrWhiteSpace(currentInput))
        {
            return;
        }

        await _terminalSession.WriteInputAsync(currentInput, CancellationToken.None);
        Console.WriteLine($"> {currentInput}", ConsoleColor.Green, ConsoleColor.Black);
        _interactiveInputBuffer.Clear();
        InputText = string.Empty;
    }

    /// <summary>
    /// Stops the running terminal session.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanStop))]
    private async Task StopAsync()
    {
        if (_terminalSession is null)
        {
            return;
        }

        await _terminalSession.StopAsync(CancellationToken.None);
        Console.WriteLine("Stopping terminal session.", ConsoleColor.Yellow, ConsoleColor.Black);
    }

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        if (_terminalSession is not null)
        {
            await DetachAndDisposeSessionAsync(_terminalSession);
            _terminalSession = null;
        }

        _interactiveInputBuffer.Clear();
        InputText = string.Empty;
        SessionDescription = "No active terminal session.";
    }

    private bool CanStart() => !IsRunning;

    private bool CanSendInput() => IsRunning && !string.IsNullOrWhiteSpace(InputText);

    private bool CanStop() => IsRunning;

    private void OnOutputReceived(object? sender, string text)
        => Dispatcher.UIThread.Post(() => Console.WriteLine(text, ConsoleColor.Gray, ConsoleColor.Black));

    private void OnOutputPartialReceived(object? sender, string text)
        => Dispatcher.UIThread.Post(() => Console.Write(text));

    private void OnErrorReceived(object? sender, string text)
        => Dispatcher.UIThread.Post(() => Console.WriteLine(text, ConsoleColor.White, ConsoleColor.DarkRed));

    private void OnErrorPartialReceived(object? sender, string text)
        => Dispatcher.UIThread.Post(() =>
        {
            var previousForegroundColor = Console.ForegroundColor;
            var previousBackgroundColor = Console.BackgroundColor;
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.DarkRed;
            Console.Write(text);
            Console.ForegroundColor = previousForegroundColor;
            Console.BackgroundColor = previousBackgroundColor;
        });

    private async void OnExited(object? sender, int exitCode)
    {
        await Dispatcher.UIThread.InvokeAsync(async () =>
        {
            Console.WriteLine($"Terminal session exited with code {exitCode}.", ConsoleColor.Yellow, ConsoleColor.Black);
            IsRunning = false;
            SessionDescription = "No active terminal session.";

            if (_terminalSession is not null)
            {
                await DetachAndDisposeSessionAsync(_terminalSession);
                _terminalSession = null;
            }

            _interactiveInputBuffer.Clear();
            InputText = string.Empty;
        });
    }

    private async Task DetachAndDisposeSessionAsync(ITerminalSession terminalSession)
    {
        terminalSession.OutputReceived -= OnOutputReceived;
        terminalSession.OutputPartialReceived -= OnOutputPartialReceived;
        terminalSession.ErrorReceived -= OnErrorReceived;
        terminalSession.ErrorPartialReceived -= OnErrorPartialReceived;
        terminalSession.Exited -= OnExited;
        await terminalSession.DisposeAsync();
    }
}