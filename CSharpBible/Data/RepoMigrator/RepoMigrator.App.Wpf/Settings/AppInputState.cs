using RepoMigrator.Core;

namespace RepoMigrator.App.Wpf.Settings;

public sealed class AppInputState
{
    public RepoType SourceType { get; set; } = RepoType.Git;
    public string SourceUrl { get; set; } = "";
    public string? SourceBranch { get; set; }
    public string? SourceUser { get; set; }
    public string? SourcePassword { get; set; }

    public RepoType TargetType { get; set; } = RepoType.Git;
    public string TargetUrl { get; set; } = "";
    public string? TargetBranch { get; set; }
    public string? TargetUser { get; set; }
    public string? TargetPassword { get; set; }
    public bool TransferGitBranches { get; set; }
    public bool TransferGitTags { get; set; }
    public List<string> SelectedGitBranches { get; set; } = [];
    public List<string> SelectedGitTags { get; set; } = [];
    public string? SelectedSvnFromRevisionId { get; set; }
    public string? SelectedSvnToRevisionId { get; set; }

    public string? FromId { get; set; }
    public string? ToId { get; set; }
    public int? MaxCount { get; set; }
    public bool OldestFirst { get; set; } = true;
}
