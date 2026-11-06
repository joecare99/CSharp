using System;
using System.IO;

namespace Config.Service;

/// <summary>
/// Resolves conventional file locations for configuration layers without creating directories
/// or asserting that system/group locations are protected.
/// </summary>
public sealed class ConfigLayerStorePathResolver
{
    private readonly string _vendorName;
    private readonly string _applicationName;
    private readonly string _userRoot;
    private readonly string _systemRoot;
    private readonly string? _groupRoot;

    public ConfigLayerStorePathResolver(
        string vendorName,
        string applicationName,
        string? userRoot = null,
        string? systemRoot = null,
        string? groupRoot = null)
    {
        _vendorName = ValidateSegment(vendorName, nameof(vendorName));
        _applicationName = ValidateSegment(applicationName, nameof(applicationName));
        _userRoot = ResolveRoot(
            userRoot ?? Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            nameof(userRoot));
        _systemRoot = ResolveRoot(
            systemRoot ?? Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
            nameof(systemRoot));
        _groupRoot = groupRoot is null
            ? null
            : ResolveRoot(groupRoot, nameof(groupRoot));
    }

    /// <summary>Resolves the per-user layer under the current user's local application data.</summary>
    public string ResolveUserPath() => Path.Combine(
        _userRoot,
        _vendorName,
        _applicationName,
        "config",
        "layers",
        "user.json");

    /// <summary>
    /// Creates the per-user layer directory and returns its JSON store.
    /// This convenience does not change any system/group directory or permission.
    /// </summary>
    public JsonConfigLayerStore CreateUserStore()
    {
        string path = ResolveUserPath();
        string directory = Path.GetDirectoryName(path)
            ?? throw new InvalidOperationException("The user configuration layer path has no parent directory.");
        Directory.CreateDirectory(directory);
        return new JsonConfigLayerStore(ConfigLayer.User, path);
    }

    /// <summary>Resolves the machine-wide system layer under common application data.</summary>
    public string ResolveSystemPath() => Path.Combine(
        _systemRoot,
        _vendorName,
        _applicationName,
        "config",
        "layers",
        "system.json");

    /// <summary>
    /// Resolves an explicitly identified group layer below an administratively configured root.
    /// The caller remains responsible for creating/protecting the directory and validating membership.
    /// </summary>
    public string ResolveGroupPath(string groupId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(groupId);
        if (_groupRoot is null)
        {
            throw new InvalidOperationException("A group configuration root must be explicitly configured.");
        }

        string safeGroupId = ValidateSegment(groupId, nameof(groupId));
        return Path.Combine(
            _groupRoot,
            _vendorName,
            _applicationName,
            "config",
            "layers",
            safeGroupId + ".json");
    }

    private static string ValidateSegment(string value, string parameterName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        string trimmed = value.Trim();
        if (trimmed is "." or ".."
            || trimmed.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0
            || trimmed.Contains('/')
            || trimmed.Contains('\\')
            || trimmed.Contains(':'))
        {
            throw new ArgumentException("The value must be one safe path segment.", parameterName);
        }

        return trimmed;
    }

    private static string ResolveRoot(string path, string parameterName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path, parameterName);
        return Path.GetFullPath(path);
    }
}
