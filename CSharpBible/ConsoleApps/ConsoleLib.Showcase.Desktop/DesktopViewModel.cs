using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ConsoleLib.Showcase.Desktop;

/// <summary>Portable state and commands exposed by the desktop CXAML page.</summary>
public partial class DesktopViewModel : ObservableObject
{
    [ObservableProperty]
    private string status = "Ready. Choose an application.";

    [RelayCommand]
    private void OpenCalendar() => Status = "Calendar requested.";

    [RelayCommand]
    private void OpenCalculator() => Status = "Calculator requested.";

    [RelayCommand]
    private void OpenNotepad() => Status = "Notepad requested.";

    [RelayCommand]
    private void OpenCharacters() => Status = "Character table requested.";

    [RelayCommand]
    private void OpenClock() => Status = "Analog clock requested.";

    [RelayCommand]
    private void OpenTerminal() => Status = "Terminal requested.";

    [RelayCommand]
    private void OpenAbout() => Status = "About requested.";
}
