using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ConsoleLib.Showcase.Models;
using ConsoleLib.Showcase.Services;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Terminal.Core;

namespace ConsoleLib.Showcase.ViewModels;

/// <summary>Coordinates gallery selection, commands, status, and the visual-effects demo.</summary>
public partial class ShowcaseViewModel : ObservableObject
{
    private readonly IShowcaseTerminalService _terminalService;

    public ShowcaseViewModel(IShowcaseTerminalService terminalService)
    {
        _terminalService = terminalService ?? throw new ArgumentNullException(nameof(terminalService));
        Sections = new ObservableCollection<ShowcaseSection>
        {
            new("Controls", "Buttons, labels, list boxes, progress, and text editing."),
            new("Layout", "Panels, borders, focus, and keyboard navigation."),
            new("Effects", "Animated glyph ramps and progress rendering."),
            new("Terminal", "A showcase-owned Windows ConPTY bridge.")
        };
        SelectedSection = Sections[0];
        Status = "Ready. Select a gallery area or start the visual effects.";
    }

    public IShowcaseTerminalService TerminalService => _terminalService;

    public ObservableCollection<ShowcaseSection> Sections { get; }

    [ObservableProperty]
    private ShowcaseSection? selectedSection;

    [ObservableProperty]
    private string status = string.Empty;

    [ObservableProperty]
    private double progress;

    [ObservableProperty]
    private bool effectsRunning;

    public event EventHandler? RequestClose;
    public event EventHandler? RequestAbout;
    public event EventHandler? RequestCloseAbout;

    [RelayCommand]
    private void ToggleEffects() => EffectsRunning = !EffectsRunning;

    [RelayCommand]
    private void AdvanceProgress() => Progress = Progress >= 100 ? 0 : Progress + 10;

    [RelayCommand]
    private async Task LaunchTerminalAsync()
    {
        Status = "Starting the showcase ConPTY probe...";
        try
        {
            Status = await _terminalService.RunProbeAsync().ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            Status = $"Terminal probe unavailable: {exception.Message}";
        }
    }

    [RelayCommand]
    private async Task StartTerminalAsync()
    {
        Status = "Starting the live terminal workspace...";
        try
        {
            await _terminalService.StartAsync(new TerminalSize(80, 24)).ConfigureAwait(false);
            Status = "Live terminal ready. Click inside it to focus input.";
        }
        catch (Exception exception)
        {
            Status = $"Terminal unavailable: {exception.Message}";
        }
    }

    [RelayCommand]
    private async Task StopTerminalAsync()
    {
        await _terminalService.StopAsync().ConfigureAwait(false);
        Status = "Live terminal stopped.";
    }

    [RelayCommand]
    private void Close() => RequestClose?.Invoke(this, EventArgs.Empty);

    [RelayCommand]
    private void ShowAbout() => RequestAbout?.Invoke(this, EventArgs.Empty);

    [RelayCommand]
    private void CloseAbout() => RequestCloseAbout?.Invoke(this, EventArgs.Empty);
}
