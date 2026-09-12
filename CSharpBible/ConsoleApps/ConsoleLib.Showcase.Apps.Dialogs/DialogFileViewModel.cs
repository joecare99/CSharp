using CommunityToolkit.Mvvm.ComponentModel;

namespace ConsoleLib.Showcase.Apps.Dialogs;

/// <summary>Data context used by the reusable file-dialog resources.</summary>
public partial class DialogFileViewModel : ObservableObject
{
    public DialogFileViewModel(string title, string? initialPath = null)
    {
        Title = title;
        Path = initialPath ?? string.Empty;
    }

    public string Title { get; }

    [ObservableProperty]
    private string path;
}
