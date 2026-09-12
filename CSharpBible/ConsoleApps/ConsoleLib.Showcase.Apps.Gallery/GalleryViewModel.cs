using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ConsoleLib.Showcase.Apps.Gallery;

/// <summary>Interactive state shared by the controls gallery page.</summary>
public sealed partial class GalleryViewModel : ObservableObject
{
    [ObservableProperty]
    private string title = "ConsoleLib Controls Gallery";

    [ObservableProperty]
    private string status = "Ready — use Advance to update the sample progress.";

    [ObservableProperty]
    private int progress = 45;

    [RelayCommand]
    private void Advance()
    {
        Progress = Progress >= 90 ? 10 : Progress + 15;
        Status = $"Progress advanced to {Progress}%.";
    }

    [RelayCommand]
    private void Reset()
    {
        Progress = 45;
        Status = "Gallery samples reset.";
    }
}
