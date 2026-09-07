using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ConsoleLib.Showcase.Desktop.Capabilities;

namespace ConsoleLib.Showcase.Desktop.ViewModels;

/// <summary>Portable terminal capability status until a host supplies a session.</summary>
public partial class TerminalViewModel : ObservableObject, IDisposable
{
    private readonly IShowcaseTerminalCapability? _terminal;

    [ObservableProperty]
    private string status = "Terminal capability unavailable";

    [ObservableProperty]
    private string output = "The selected host can provide a Terminal.Core session here.";

    public TerminalViewModel(IShowcaseTerminalCapability? terminal = null)
    {
        _terminal = terminal;
        if (_terminal?.IsAvailable == true)
        {
            Status = "Terminal capability available";
            Output = "Start a host-provided Terminal.Core session.";
            _terminal.OutputChanged += Terminal_OutputChanged;
        }
    }

    [RelayCommand]
    private async Task StartAsync()
    {
        if (_terminal?.IsAvailable != true)
        {
            Status = "Terminal requires a host-provided session.";
            return;
        }

        Status = "Starting terminal session...";
        Output = "Starting the host terminal...";
        try
        {
            await _terminal.StartAsync(new Size(72, 14)).ConfigureAwait(false);
            Status = "Terminal session running.";
            if (string.IsNullOrWhiteSpace(Output) || Output == "Starting the host terminal...")
                Output = "Terminal session started; waiting for shell output...";
        }
        catch (Exception exception)
        {
            Status = $"Terminal start failed: {exception.Message}";
            Output = "The host terminal could not be started.";
        }
    }

    [RelayCommand]
    private async Task StopAsync()
    {
        if (_terminal is not null)
            await _terminal.StopAsync().ConfigureAwait(false);
        Status = "Terminal stopped.";
        Output = "Terminal session stopped.";
    }

    public async ValueTask DisposeAsync()
    {
        if (_terminal is not null)
        {
            _terminal.OutputChanged -= Terminal_OutputChanged;
            await _terminal.DisposeAsync().ConfigureAwait(false);
        }
    }

    public void Dispose()
    {
        if (_terminal is not null)
        {
            _terminal.OutputChanged -= Terminal_OutputChanged;
            try
            {
                _terminal.DisposeAsync().AsTask().GetAwaiter().GetResult();
            }
            catch (ObjectDisposedException)
            {
            }
            catch (InvalidOperationException)
            {
            }
        }
    }

    private void Terminal_OutputChanged(object? sender, string output) =>
        Output = output;
}
