using RepoMigrator.Core;

namespace RepoMigrator.App.State.Settings;

/// <summary>
/// Represents the persisted user input state for the RepoMigrator app.
/// </summary>
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
    public List<RepoTypeRecentValues> RecentSourceUrlsByType { get; set; } = [];
    public List<RepoTypeRecentValues> RecentTargetUrlsByType { get; set; } = [];
    public List<string> RecentSourceUrls { get; set; } = [];
    public List<string> RecentSourceBranches { get; set; } = [];
    public List<string> RecentSourceUsers { get; set; } = [];
    public List<string> RecentTargetUrls { get; set; } = [];
    public List<string> RecentTargetBranches { get; set; } = [];
    public List<string> RecentTargetUsers { get; set; } = [];
    public string? SelectedSvnFromRevisionId { get; set; }
    public string? SelectedSvnToRevisionId { get; set; }

    public string? FromId { get; set; }
    public string? ToId { get; set; }
    public int? MaxCount { get; set; }
    public bool OldestFirst { get; set; } = true;
    public bool UsePipelinedMigration { get; set; }
    public int PipelinePrefetchCount { get; set; } = 3;
    public int PipelineExportWorkerCount { get; set; } = 2;
    public bool SplitIntoSubdirectoryBranches { get; set; }
    public int BranchSplitDepth { get; set; } = 1;
}
