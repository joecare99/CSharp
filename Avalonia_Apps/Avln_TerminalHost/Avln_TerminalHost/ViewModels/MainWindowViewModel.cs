using System;
using System.Threading;
using System.Threading.Tasks;
using Avln_TestConsole;
using Avln_TestConsole.Interfaces;
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
        _hostedProcess = await _processRunner.StartAsync().ConfigureAwait(false);
        _hostedProcess.StandardOutputReceived += OnStandardOutputReceived;
        _hostedProcess.StandardErrorReceived += OnStandardErrorReceived;
        _hostedProcess.Exited += OnExited;
        IsRunning = true;
        Console.WriteLine("Started shell process.", ConsoleColor.Gray, ConsoleColor.Black);
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

        await _hostedProcess.WriteLineAsync(currentInput, CancellationToken.None).ConfigureAwait(false);
        Console.WriteLine($"> {currentInput}", ConsoleColor.Green, ConsoleColor.Black);
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

        await _hostedProcess.StopAsync(CancellationToken.None).ConfigureAwait(false);
        Console.WriteLine("Stopping shell process.", ConsoleColor.Yellow, ConsoleColor.Black);
    }

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        if (_hostedProcess is not null)
        {
            await DetachAndDisposeProcessAsync(_hostedProcess).ConfigureAwait(false);
            _hostedProcess = null;
        }
    }

    private bool CanStart() => !IsRunning;

    private bool CanSendInput() => IsRunning && !string.IsNullOrWhiteSpace(InputText);

    private bool CanStop() => IsRunning;

    private void OnStandardOutputReceived(object? sender, string text)
        => Console.WriteLine(text, ConsoleColor.Gray, ConsoleColor.Black);

    private void OnStandardErrorReceived(object? sender, string text)
        => Console.WriteLine(text, ConsoleColor.White, ConsoleColor.DarkRed);

    private async void OnExited(object? sender, int exitCode)
    {
        Console.WriteLine($"Process exited with code {exitCode}.", ConsoleColor.Yellow, ConsoleColor.Black);
        IsRunning = false;

        if (_hostedProcess is not null)
        {
            await DetachAndDisposeProcessAsync(_hostedProcess).ConfigureAwait(false);
            _hostedProcess = null;
        }
    }

    private async Task DetachAndDisposeProcessAsync(IHostedProcess hostedProcess)
    {
        hostedProcess.StandardOutputReceived -= OnStandardOutputReceived;
        hostedProcess.StandardErrorReceived -= OnStandardErrorReceived;
        hostedProcess.Exited -= OnExited;
        await hostedProcess.DisposeAsync().ConfigureAwait(false);
    }
}
