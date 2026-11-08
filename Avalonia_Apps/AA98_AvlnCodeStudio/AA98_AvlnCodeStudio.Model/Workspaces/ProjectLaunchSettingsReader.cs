using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.Model.Workspaces;

/// <summary>Describes a project launch profile without exposing arguments or environment values.</summary>
/// <param name="Name">The profile name as written in the launch settings file.</param>
/// <param name="CommandName">The simple command name, or null when it is missing or invalid.</param>
/// <param name="HasCommandLineArguments">Whether the argument field is present, even if its value is null or invalid.</param>
/// <param name="HasEnvironmentVariables">Whether the environment field is present, even if its value is null or invalid.</param>
/// <param name="IsAmbiguous">Whether another profile has the same name ignoring case.</param>
/// <param name="Diagnostic">A description of any unsupported or ambiguous profile metadata.</param>
public sealed record ProjectLaunchProfile(
    string Name,
    string? CommandName,
    bool HasCommandLineArguments,
    bool HasEnvironmentVariables,
    bool IsAmbiguous,
    string? Diagnostic);

/// <summary>Contains a read-only snapshot of one project's launch profile metadata.</summary>
public sealed class ProjectLaunchSettingsSnapshot
{
    internal ProjectLaunchSettingsSnapshot(
        string filePath,
        bool fileExists,
        bool hasProfilesObject,
        IEnumerable<ProjectLaunchProfile> profiles)
    {
        FilePath = filePath;
        FileExists = fileExists;
        HasProfilesObject = hasProfilesObject;
        Profiles = Array.AsReadOnly(profiles.ToArray());
    }

    public string FilePath { get; }
    public bool FileExists { get; }
    public bool HasProfilesObject { get; }
    public IReadOnlyList<ProjectLaunchProfile> Profiles { get; }
}

/// <summary>Reads project launch profile metadata without selecting, executing, or changing profiles.</summary>
/// <remarks>
/// The reader accepts UTF-8 BOM, JSON comments and trailing commas. It limits input size,
/// profile count and names, and reports malformed data instead of inventing defaults.
/// Arguments and environment values are not returned; their presence alone is reported.
/// The snapshot is informational and does not guarantee that any debugger applies a profile.
/// </remarks>
public sealed class ProjectLaunchSettingsReader
{
    public const int MaximumFileSizeBytes = 1024 * 1024;
    private const int MaximumProfiles = 256;
    private const int MaximumNameLength = 256;

    public static string GetSettingsPath(string projectPath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(projectPath);
        string fullProjectPath = Path.GetFullPath(projectPath);
        if (!string.Equals(Path.GetExtension(fullProjectPath), ".csproj", StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException("A C# project file is required.", nameof(projectPath));
        }

        return Path.Combine(Path.GetDirectoryName(fullProjectPath)!, "Properties", "launchSettings.json");
    }

    public async Task<ProjectLaunchSettingsSnapshot> ReadAsync(
        string projectPath,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string settingsPath = GetSettingsPath(projectPath);
        string fullProjectPath = Path.GetFullPath(projectPath);
        FileAttributes projectAttributes = File.GetAttributes(fullProjectPath);
        if ((projectAttributes & FileAttributes.Directory) != 0)
        {
            throw new InvalidDataException("The selected project path is a directory, not a C# project file.");
        }

        FileStream stream;
        try
        {
            stream = new FileStream(
                settingsPath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.ReadWrite | FileShare.Delete,
                4096,
                FileOptions.Asynchronous | FileOptions.SequentialScan);
        }
        catch (FileNotFoundException)
        {
            return new ProjectLaunchSettingsSnapshot(settingsPath, false, false, []);
        }
        catch (DirectoryNotFoundException)
        {
            return new ProjectLaunchSettingsSnapshot(settingsPath, false, false, []);
        }

        await using (stream)
        {
            byte[] buffer = new byte[MaximumFileSizeBytes + 1];
            try
            {
                int length = 0;
                while (length < buffer.Length)
                {
                    int count = await stream.ReadAsync(buffer.AsMemory(length), cancellationToken);
                    if (count == 0)
                    {
                        break;
                    }

                    length += count;
                }

                if (length > MaximumFileSizeBytes)
                {
                    throw new InvalidDataException("The project launch settings file exceeds the 1 MiB inspection limit.");
                }

                int offset = length >= 3 && buffer[0] == 0xef && buffer[1] == 0xbb && buffer[2] == 0xbf
                    ? 3
                    : 0;
                try
                {
                    using JsonDocument document = JsonDocument.Parse(
                        buffer.AsMemory(offset, length - offset),
                        new JsonDocumentOptions
                        {
                            AllowTrailingCommas = true,
                            CommentHandling = JsonCommentHandling.Skip,
                            MaxDepth = 64,
                        });
                    return Inspect(document.RootElement, settingsPath);
                }
                catch (JsonException exception)
                {
                    throw new InvalidDataException(
                        $"Invalid project launch settings JSON at line {(exception.LineNumber ?? 0) + 1}, byte {(exception.BytePositionInLine ?? 0) + 1}.",
                        exception);
                }
            }
            finally
            {
                CryptographicOperations.ZeroMemory(buffer);
            }
        }
    }

    private static ProjectLaunchSettingsSnapshot Inspect(JsonElement root, string settingsPath)
    {
        if (root.ValueKind != JsonValueKind.Object)
        {
            throw new InvalidDataException("Project launch settings must contain a JSON object.");
        }

        JsonProperty[] profileSections = root.EnumerateObject()
            .Where(static property => property.Name.Equals("profiles", StringComparison.OrdinalIgnoreCase))
            .ToArray();
        if (profileSections.Length == 0)
        {
            return new ProjectLaunchSettingsSnapshot(settingsPath, true, false, []);
        }

        if (profileSections.Length != 1 || profileSections[0].Value.ValueKind != JsonValueKind.Object)
        {
            throw new InvalidDataException("Project launch settings must contain at most one profiles object.");
        }

        JsonProperty[] profiles = profileSections[0].Value.EnumerateObject().ToArray();
        if (profiles.Length > MaximumProfiles)
        {
            throw new InvalidDataException($"The project launch settings contain more than {MaximumProfiles} profiles.");
        }

        if (profiles.Any(static profile => !IsSimpleName(profile.Name)))
        {
            throw new InvalidDataException("A launch profile name is empty, contains control characters, or is too long.");
        }

        HashSet<string> ambiguousNames = profiles
            .GroupBy(static profile => profile.Name, StringComparer.OrdinalIgnoreCase)
            .Where(static group => group.Count() > 1)
            .Select(static group => group.Key)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        ProjectLaunchProfile[] result = profiles
            .Select(profile => InspectProfile(profile, ambiguousNames.Contains(profile.Name)))
            .ToArray();
        return new ProjectLaunchSettingsSnapshot(settingsPath, true, true, result);
    }

    private static ProjectLaunchProfile InspectProfile(JsonProperty profile, bool ambiguous)
    {
        string? commandName = null;
        string? diagnostic = null;
        bool hasArguments = false;
        bool hasEnvironment = false;
        if (profile.Value.ValueKind != JsonValueKind.Object)
        {
            diagnostic = "The profile is not a JSON object.";
        }
        else
        {
            JsonProperty[] properties = profile.Value.EnumerateObject().ToArray();
            JsonProperty[] commands = properties
                .Where(static property => property.Name.Equals("commandName", StringComparison.OrdinalIgnoreCase))
                .ToArray();
            if (commands.Length == 1 && commands[0].Value.ValueKind == JsonValueKind.String)
            {
                string? value = commands[0].Value.GetString();
                if (value is not null && IsSimpleName(value))
                {
                    commandName = value;
                }
                else
                {
                    diagnostic = "The commandName is empty, contains control characters, or is too long.";
                }
            }
            else
            {
                diagnostic = commands.Length == 0
                    ? "The commandName is missing."
                    : "The commandName is not a single string value.";
            }

            hasArguments = properties.Any(static property =>
                property.Name.Equals("commandLineArgs", StringComparison.OrdinalIgnoreCase));
            hasEnvironment = properties.Any(static property =>
                property.Name.Equals("environmentVariables", StringComparison.OrdinalIgnoreCase));
        }

        if (ambiguous)
        {
            diagnostic = string.IsNullOrEmpty(diagnostic)
                ? "The profile name is ambiguous ignoring case."
                : $"{diagnostic} The profile name is ambiguous ignoring case.";
        }

        return new ProjectLaunchProfile(profile.Name, commandName, hasArguments, hasEnvironment, ambiguous, diagnostic);
    }

    private static bool IsSimpleName(string value) =>
        !string.IsNullOrWhiteSpace(value)
        && value.Length <= MaximumNameLength
        && !value.Any(char.IsControl);
}
