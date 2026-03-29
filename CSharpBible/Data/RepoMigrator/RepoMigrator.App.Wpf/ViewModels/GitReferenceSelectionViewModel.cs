using CommunityToolkit.Mvvm.ComponentModel;

namespace RepoMigrator.App.Wpf.ViewModels;

/// <summary>
/// Represents a selectable Git branch or tag inside the WPF migration UI.
/// </summary>
public sealed class GitReferenceSelectionViewModel : ObservableObject
{
    private bool _isSelected;

    public GitReferenceSelectionViewModel(string sName, string? sCommitId, bool xIsSelected = false)
    {
        Name = sName;
        CommitId = sCommitId;
        _isSelected = xIsSelected;
    }

    public string Name { get; }

    public string? CommitId { get; }

    public bool IsSelected
    {
        get => _isSelected;
        set => SetProperty(ref _isSelected, value);
    }
}
