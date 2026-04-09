using RepoMigrator.App.State.Settings;
using RepoMigrator.Core;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace RepoMigrator.App.State.Services;

/// <summary>
/// Persists and restores the RepoMigrator app input state outside the UI layer.
/// </summary>
public sealed class AppInputStateStore
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true
    };

    private readonly string _filePath;

    public AppInputStateStore()
        : this(Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "RepoMigrator",
            "inputs.json"))
    {
    }

    internal AppInputStateStore(string sFilePath)
    {
        _filePath = sFilePath;
    }

    public AppInputState Load()
    {
        if (!File.Exists(_filePath))
            return new AppInputState();

        try
        {
            var sJson = File.ReadAllText(_filePath);
            var persistedState = JsonSerializer.Deserialize<PersistedAppInputState>(sJson, SerializerOptions);
            if (persistedState is null)
                return new AppInputState();

            return new AppInputState
            {
                SourceType = persistedState.SourceType,
                SourceUrl = persistedState.SourceUrl ?? "",
                SourceBranch = persistedState.SourceBranch,
                SourceUser = persistedState.SourceUser,
                SourcePassword = Unprotect(persistedState.SourcePasswordProtected),
                TargetType = persistedState.TargetType,
                TargetUrl = persistedState.TargetUrl ?? "",
                TargetBranch = persistedState.TargetBranch,
                TargetUser = persistedState.TargetUser,
                TargetPassword = Unprotect(persistedState.TargetPasswordProtected),
                TransferGitBranches = persistedState.TransferGitBranches,
                TransferGitTags = persistedState.TransferGitTags,
                SelectedGitBranches = persistedState.SelectedGitBranches ?? [],
                SelectedGitTags = persistedState.SelectedGitTags ?? [],
                RecentSourceUrlsByType = persistedState.RecentSourceUrlsByType ?? CreateLegacyRepoTypeHistory(persistedState.SourceType, persistedState.RecentSourceUrls),
                RecentTargetUrlsByType = persistedState.RecentTargetUrlsByType ?? CreateLegacyRepoTypeHistory(persistedState.TargetType, persistedState.RecentTargetUrls),
                RecentSourceUrls = persistedState.RecentSourceUrls ?? [],
                RecentSourceBranches = persistedState.RecentSourceBranches ?? [],
                RecentSourceUsers = persistedState.RecentSourceUsers ?? [],
                RecentTargetUrls = persistedState.RecentTargetUrls ?? [],
                RecentTargetBranches = persistedState.RecentTargetBranches ?? [],
                RecentTargetUsers = persistedState.RecentTargetUsers ?? [],
                SelectedSvnFromRevisionId = persistedState.SelectedSvnFromRevisionId,
                SelectedSvnToRevisionId = persistedState.SelectedSvnToRevisionId,
                FromId = persistedState.FromId,
                ToId = persistedState.ToId,
                MaxCount = persistedState.MaxCount,
                OldestFirst = persistedState.OldestFirst,
                UsePipelinedMigration = persistedState.UsePipelinedMigration,
                PipelinePrefetchCount = persistedState.PipelinePrefetchCount,
                PipelineExportWorkerCount = persistedState.PipelineExportWorkerCount,
                SplitIntoSubdirectoryBranches = persistedState.SplitIntoSubdirectoryBranches,
                BranchSplitDepth = persistedState.BranchSplitDepth
            };
        }
        catch
        {
            return new AppInputState();
        }
    }

    public void Save(AppInputState state)
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_filePath)!);

            var persistedState = new PersistedAppInputState
            {
                SourceType = state.SourceType,
                SourceUrl = state.SourceUrl,
                SourceBranch = state.SourceBranch,
                SourceUser = state.SourceUser,
                SourcePasswordProtected = Protect(state.SourcePassword),
                TargetType = state.TargetType,
                TargetUrl = state.TargetUrl,
                TargetBranch = state.TargetBranch,
                TargetUser = state.TargetUser,
                TargetPasswordProtected = Protect(state.TargetPassword),
                TransferGitBranches = state.TransferGitBranches,
                TransferGitTags = state.TransferGitTags,
                SelectedGitBranches = state.SelectedGitBranches,
                SelectedGitTags = state.SelectedGitTags,
                RecentSourceUrlsByType = state.RecentSourceUrlsByType,
                RecentTargetUrlsByType = state.RecentTargetUrlsByType,
                RecentSourceUrls = state.RecentSourceUrls,
                RecentSourceBranches = state.RecentSourceBranches,
                RecentSourceUsers = state.RecentSourceUsers,
                RecentTargetUrls = state.RecentTargetUrls,
                RecentTargetBranches = state.RecentTargetBranches,
                RecentTargetUsers = state.RecentTargetUsers,
                SelectedSvnFromRevisionId = state.SelectedSvnFromRevisionId,
                SelectedSvnToRevisionId = state.SelectedSvnToRevisionId,
                FromId = state.FromId,
                ToId = state.ToId,
                MaxCount = state.MaxCount,
                OldestFirst = state.OldestFirst,
                UsePipelinedMigration = state.UsePipelinedMigration,
                PipelinePrefetchCount = state.PipelinePrefetchCount,
                PipelineExportWorkerCount = state.PipelineExportWorkerCount,
                SplitIntoSubdirectoryBranches = state.SplitIntoSubdirectoryBranches,
                BranchSplitDepth = state.BranchSplitDepth
            };

            var sJson = JsonSerializer.Serialize(persistedState, SerializerOptions);
            File.WriteAllText(_filePath, sJson);
        }
        catch
        {
        }
    }

    private static string? Protect(string? sValue)
    {
        if (string.IsNullOrEmpty(sValue))
            return null;

        var arrBytes = Encoding.UTF8.GetBytes(sValue);
        var arrProtectedBytes = ProtectedData.Protect(arrBytes, null, DataProtectionScope.CurrentUser);
        return Convert.ToBase64String(arrProtectedBytes);
    }

    private static string? Unprotect(string? sValue)
    {
        if (string.IsNullOrEmpty(sValue))
            return null;

        try
        {
            var arrProtectedBytes = Convert.FromBase64String(sValue);
            var arrBytes = ProtectedData.Unprotect(arrProtectedBytes, null, DataProtectionScope.CurrentUser);
            return Encoding.UTF8.GetString(arrBytes);
        }
        catch
        {
            return null;
        }
    }

    private static List<RepoTypeRecentValues> CreateLegacyRepoTypeHistory(RepoType repoType, List<string>? lstValues)
        => lstValues is { Count: > 0 }
            ? [new RepoTypeRecentValues { RepoType = repoType, Values = [.. lstValues] }]
            : [];

    private sealed class PersistedAppInputState
    {
        public RepoType SourceType { get; set; } = RepoType.Git;
        public string? SourceUrl { get; set; }
        public string? SourceBranch { get; set; }
        public string? SourceUser { get; set; }
        public string? SourcePasswordProtected { get; set; }
        public RepoType TargetType { get; set; } = RepoType.Git;
        public string? TargetUrl { get; set; }
        public string? TargetBranch { get; set; }
        public string? TargetUser { get; set; }
        public string? TargetPasswordProtected { get; set; }
        public bool TransferGitBranches { get; set; }
        public bool TransferGitTags { get; set; }
        public List<string>? SelectedGitBranches { get; set; }
        public List<string>? SelectedGitTags { get; set; }
        public List<RepoTypeRecentValues>? RecentSourceUrlsByType { get; set; }
        public List<RepoTypeRecentValues>? RecentTargetUrlsByType { get; set; }
        public List<string>? RecentSourceUrls { get; set; }
        public List<string>? RecentSourceBranches { get; set; }
        public List<string>? RecentSourceUsers { get; set; }
        public List<string>? RecentTargetUrls { get; set; }
        public List<string>? RecentTargetBranches { get; set; }
        public List<string>? RecentTargetUsers { get; set; }
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
}
