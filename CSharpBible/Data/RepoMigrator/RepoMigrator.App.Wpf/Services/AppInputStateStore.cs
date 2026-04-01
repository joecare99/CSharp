using RepoMigrator.App.Wpf.Settings;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace RepoMigrator.App.Wpf.Services;

public sealed class AppInputStateStore
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true
    };

    private readonly string _filePath;

    public AppInputStateStore()
        : this(System.IO.Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "RepoMigrator",
            "inputs.json"))
    {
    }

    internal AppInputStateStore(string filePath)
    {
        _filePath = filePath;
    }

    public AppInputState Load()
    {
        if (!System.IO.File.Exists(_filePath))
            return new AppInputState();

        try
        {
            var json = System.IO.File.ReadAllText(_filePath);
            var persisted = JsonSerializer.Deserialize<PersistedAppInputState>(json, SerializerOptions);
            if (persisted is null)
                return new AppInputState();

            return new AppInputState
            {
                SourceType = persisted.SourceType,
                SourceUrl = persisted.SourceUrl ?? "",
                SourceBranch = persisted.SourceBranch,
                SourceUser = persisted.SourceUser,
                SourcePassword = Unprotect(persisted.SourcePasswordProtected),
                TargetType = persisted.TargetType,
                TargetUrl = persisted.TargetUrl ?? "",
                TargetBranch = persisted.TargetBranch,
                TargetUser = persisted.TargetUser,
                TargetPassword = Unprotect(persisted.TargetPasswordProtected),
                TransferGitBranches = persisted.TransferGitBranches,
                TransferGitTags = persisted.TransferGitTags,
                SelectedGitBranches = persisted.SelectedGitBranches ?? [],
                SelectedGitTags = persisted.SelectedGitTags ?? [],
                RecentSourceUrls = persisted.RecentSourceUrls ?? [],
                RecentSourceBranches = persisted.RecentSourceBranches ?? [],
                RecentSourceUsers = persisted.RecentSourceUsers ?? [],
                RecentTargetUrls = persisted.RecentTargetUrls ?? [],
                RecentTargetBranches = persisted.RecentTargetBranches ?? [],
                RecentTargetUsers = persisted.RecentTargetUsers ?? [],
                SelectedSvnFromRevisionId = persisted.SelectedSvnFromRevisionId,
                SelectedSvnToRevisionId = persisted.SelectedSvnToRevisionId,
                FromId = persisted.FromId,
                ToId = persisted.ToId,
                MaxCount = persisted.MaxCount,
                OldestFirst = persisted.OldestFirst
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
            System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(_filePath)!);

            var persisted = new PersistedAppInputState
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
                OldestFirst = state.OldestFirst
            };

            var json = JsonSerializer.Serialize(persisted, SerializerOptions);
            System.IO.File.WriteAllText(_filePath, json);
        }
        catch
        {
        }
    }

    private static string? Protect(string? value)
    {
        if (string.IsNullOrEmpty(value))
            return null;

        var bytes = Encoding.UTF8.GetBytes(value);
        var protectedBytes = ProtectedData.Protect(bytes, null, DataProtectionScope.CurrentUser);
        return Convert.ToBase64String(protectedBytes);
    }

    private static string? Unprotect(string? value)
    {
        if (string.IsNullOrEmpty(value))
            return null;

        try
        {
            var protectedBytes = Convert.FromBase64String(value);
            var bytes = ProtectedData.Unprotect(protectedBytes, null, DataProtectionScope.CurrentUser);
            return Encoding.UTF8.GetString(bytes);
        }
        catch
        {
            return null;
        }
    }

    private sealed class PersistedAppInputState
    {
        public Core.RepoType SourceType { get; set; } = Core.RepoType.Git;
        public string? SourceUrl { get; set; }
        public string? SourceBranch { get; set; }
        public string? SourceUser { get; set; }
        public string? SourcePasswordProtected { get; set; }
        public Core.RepoType TargetType { get; set; } = Core.RepoType.Git;
        public string? TargetUrl { get; set; }
        public string? TargetBranch { get; set; }
        public string? TargetUser { get; set; }
        public string? TargetPasswordProtected { get; set; }
        public bool TransferGitBranches { get; set; }
        public bool TransferGitTags { get; set; }
        public List<string>? SelectedGitBranches { get; set; }
        public List<string>? SelectedGitTags { get; set; }
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
    }
}
