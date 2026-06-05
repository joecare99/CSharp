using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Avln_TestConsole;
using Avln_TestConsole.Interfaces;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Avln_TerminalHost.Services;

namespace Avln_TerminalHost.ViewModels;

/// <summary>
/// Coordinates the terminal host UI and the hosted shell process.
/// </summary>
public partial class MainWindowViewModel : ViewModelBase, IAsyncDisposable
{
    private readonly IProcessRunner _processRunner;
    private readonly StringBuilder _interactiveInputBuffer = new();
    private IHostedProcess? _hostedProcess;

    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
    /// </summary>
    /// <param name="processRunner">The process runner used to start the shell.</param>
    public MainWindowViewModel(IProcessRunner processRunner)
    {
        _processRunner = processRunner;
        Console = new AvaloniaTestConsole
        {
            WindowWidth = 120,
            WindowHeight = 40,
        };
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

    /// <summary>
    /// Starts the terminal process if it is not already running.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanStart))]
    private async Task StartAsync()
    {
        if (_hostedProcess is not null)
        {
            return;
        }

        Console.Clear();
        Console.Title = "Terminal Host";
        _hostedProcess = await _processRunner.StartAsync();
        _hostedProcess.StandardOutputReceived += OnStandardOutputReceived;
        _hostedProcess.StandardOutputPartialReceived += OnStandardOutputPartialReceived;
        _hostedProcess.StandardErrorReceived += OnStandardErrorReceived;
        _hostedProcess.StandardErrorPartialReceived += OnStandardErrorPartialReceived;
        _hostedProcess.Exited += OnExited;
        _interactiveInputBuffer.Clear();
        InputText = string.Empty;
        IsRunning = true;
        Console.WriteLine("Started shell process.", ConsoleColor.Gray, ConsoleColor.Black);
    }

    /// <summary>
    /// Processes direct text input coming from the terminal control.
    /// </summary>
    /// <param name="text">The entered text.</param>
    public Task HandleConsoleTextInputAsync(string? text)
    {
        if (!IsRunning || _hostedProcess is null || string.IsNullOrEmpty(text))
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
        if (!IsRunning || _hostedProcess is null)
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
                await _hostedProcess.WriteLineAsync(currentInput, CancellationToken.None);
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
        if (_hostedProcess is null)
        {
            return;
        }

        var currentInput = InputText;
        if (string.IsNullOrWhiteSpace(currentInput))
        {
            return;
        }

        await _hostedProcess.WriteLineAsync(currentInput, CancellationToken.None);
        Console.WriteLine($"> {currentInput}", ConsoleColor.Green, ConsoleColor.Black);
        _interactiveInputBuffer.Clear();
        InputText = string.Empty;
    }

    /// <summary>
    /// Stops the running shell process.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanStop))]
    private async Task StopAsync()
    {
        if (_hostedProcess is null)
        {
            return;
        }

        await _hostedProcess.StopAsync(CancellationToken.None);
        Console.WriteLine("Stopping shell process.", ConsoleColor.Yellow, ConsoleColor.Black);
    }

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        if (_hostedProcess is not null)
        {
            await DetachAndDisposeProcessAsync(_hostedProcess);
            _hostedProcess = null;
        }

        _interactiveInputBuffer.Clear();
        InputText = string.Empty;
    }

    private bool CanStart() => !IsRunning;

    private bool CanSendInput() => IsRunning && !string.IsNullOrWhiteSpace(InputText);

    private bool CanStop() => IsRunning;

    private void OnStandardOutputReceived(object? sender, string text)
        => Dispatcher.UIThread.Post(() => Console.WriteLine(text, ConsoleColor.Gray, ConsoleColor.Black));

    private void OnStandardOutputPartialReceived(object? sender, string text)
        => Dispatcher.UIThread.Post(() => Console.Write(text));

    private void OnStandardErrorReceived(object? sender, string text)
        => Dispatcher.UIThread.Post(() => Console.WriteLine(text, ConsoleColor.White, ConsoleColor.DarkRed));

    private void OnStandardErrorPartialReceived(object? sender, string text)
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
            Console.WriteLine($"Process exited with code {exitCode}.", ConsoleColor.Yellow, ConsoleColor.Black);
            IsRunning = false;

            if (_hostedProcess is not null)
            {
                await DetachAndDisposeProcessAsync(_hostedProcess);
                _hostedProcess = null;
            }

            _interactiveInputBuffer.Clear();
            InputText = string.Empty;
        });
    }

    private async Task DetachAndDisposeProcessAsync(IHostedProcess hostedProcess)
    {
        hostedProcess.StandardOutputReceived -= OnStandardOutputReceived;
        hostedProcess.StandardOutputPartialReceived -= OnStandardOutputPartialReceived;
        hostedProcess.StandardErrorReceived -= OnStandardErrorReceived;
        hostedProcess.StandardErrorPartialReceived -= OnStandardErrorPartialReceived;
        hostedProcess.Exited -= OnExited;
        await hostedProcess.DisposeAsync();
    }
}
