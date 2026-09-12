using CommunityToolkit.Mvvm.ComponentModel;

namespace ConsoleLib.Showcase.Apps.Dialogs;

/// <summary>Editable settings state shared by the settings dialog resource.</summary>
public partial class SettingsViewModel : ObservableObject
{
    [ObservableProperty]
    private string title = "Settings";

    [ObservableProperty]
    private string theme = "Default";

    [ObservableProperty]
    private bool animationsEnabled = true;

    [ObservableProperty]
    private bool statusBarVisible = true;

    [ObservableProperty]
    private string status = "Edit settings and choose Apply or Cancel.";

    public SettingsSnapshot Snapshot() =>
        new(Theme, AnimationsEnabled, StatusBarVisible);

    public void Apply(SettingsSnapshot snapshot)
    {
        Theme = snapshot.Theme;
        AnimationsEnabled = snapshot.AnimationsEnabled;
        StatusBarVisible = snapshot.StatusBarVisible;
        Status = "Settings applied.";
    }

    public void Cancel() => Status = "Settings changes cancelled.";
}

public readonly record struct SettingsSnapshot(
    string Theme,
    bool AnimationsEnabled,
    bool StatusBarVisible);
