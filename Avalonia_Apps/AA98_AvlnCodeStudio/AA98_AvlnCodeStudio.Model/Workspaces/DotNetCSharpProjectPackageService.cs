using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.Model.Workspaces;

/// <summary>
/// Uses the .NET CLI to manage NuGet references in C# projects.
/// </summary>
public sealed class DotNetCSharpProjectPackageService : ICSharpProjectPackageService
{
    private static readonly Regex s_packageIdPattern = new(
        "^[A-Za-z0-9][A-Za-z0-9._-]*$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);
    private static readonly Regex s_packageVersionPattern = new(
        "^[A-Za-z0-9][A-Za-z0-9.+*-]*$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);
    private static readonly HttpClient s_defaultHttpClient = new()
    {
        Timeout = TimeSpan.FromSeconds(20),
    };
    private readonly string _dotnetExecutable;
    private readonly HttpClient _httpClient;

    public DotNetCSharpProjectPackageService(string dotnetExecutable = "dotnet", HttpClient? httpClient = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(dotnetExecutable);
        _dotnetExecutable = dotnetExecutable;
        _httpClient = httpClient ?? s_defaultHttpClient;
    }

    public async Task<IReadOnlyList<CSharpProjectPackageReference>> ListPackagesAsync(
        string projectPath,
        CancellationToken cancellationToken = default)
    {
        ValidateProjectPath(projectPath);
        string output = await RunDotNetAsync(
            projectPath,
            ["list", Path.GetFullPath(projectPath), "package", "--format", "json"],
            cancellationToken,
            includeStandardErrorInOutput: false).ConfigureAwait(false);
        return ParsePackageListJson(output);
    }

    internal static IReadOnlyList<CSharpProjectPackageReference> ParsePackageListJson(string output)
    {
        ArgumentNullException.ThrowIfNull(output);
        using JsonDocument document = JsonDocument.Parse(output);
        if (!document.RootElement.TryGetProperty("projects", out JsonElement projects)
            || projects.ValueKind != JsonValueKind.Array)
        {
            throw new InvalidDataException("The .NET package list response does not contain a projects array.");
        }

        List<CSharpProjectPackageReference> packages = [];
        foreach (JsonElement project in projects.EnumerateArray())
        {
            if (!project.TryGetProperty("frameworks", out JsonElement frameworks)
                || frameworks.ValueKind != JsonValueKind.Array)
            {
                continue;
            }

            foreach (JsonElement framework in frameworks.EnumerateArray())
            {
                string targetFramework = GetStringProperty(framework, "framework");
                if (!framework.TryGetProperty("topLevelPackages", out JsonElement references)
                    || references.ValueKind != JsonValueKind.Array)
                {
                    continue;
                }

                foreach (JsonElement reference in references.EnumerateArray())
                {
                    string id = GetStringProperty(reference, "id");
                    string requested = GetOptionalStringProperty(reference, "requestedVersion");
                    string resolved = GetOptionalStringProperty(reference, "resolvedVersion");
                    packages.Add(new CSharpProjectPackageReference(
                        id,
                        requested,
                        resolved,
                        targetFramework));
                }
            }
        }

        return packages
            .OrderBy(static package => package.Id, StringComparer.OrdinalIgnoreCase)
            .ThenBy(static package => package.TargetFramework, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    public async Task<IReadOnlyList<NuGetPackageSearchResult>> SearchPackagesAsync(
        string searchText,
        int take = 20,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(searchText);
        if (take is < 1 or > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(take), "NuGet search result count must be between 1 and 100.");
        }

        string requestUri = $"https://azuresearch-usnc.nuget.org/query?q={Uri.EscapeDataString(searchText.Trim())}&take={take}&prerelease=false";
        using HttpResponseMessage response = await _httpClient.GetAsync(
            requestUri,
            HttpCompletionOption.ResponseHeadersRead,
            cancellationToken).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        await using Stream content = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
        using JsonDocument document = await JsonDocument.ParseAsync(content, cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        if (!document.RootElement.TryGetProperty("data", out JsonElement data)
            || data.ValueKind != JsonValueKind.Array)
        {
            throw new InvalidDataException("The NuGet search response does not contain a data array.");
        }

        List<NuGetPackageSearchResult> results = [];
        foreach (JsonElement package in data.EnumerateArray())
        {
            string id = GetStringProperty(package, "id");
            string version = GetOptionalStringProperty(package, "version");
            string description = GetOptionalStringProperty(package, "description");
            results.Add(new NuGetPackageSearchResult(id, version, description));
        }

        return results;
    }

    public Task<string> AddPackageAsync(
        string projectPath,
        string packageId,
        string? version = null,
        CancellationToken cancellationToken = default)
    {
        ValidateProjectPath(projectPath);
        ValidatePackageId(packageId);
        if (!string.IsNullOrWhiteSpace(version) && !s_packageVersionPattern.IsMatch(version))
        {
            throw new ArgumentException("The NuGet package version contains unsupported characters.", nameof(version));
        }

        return RunDotNetAsync(
            projectPath,
            ["add", Path.GetFullPath(projectPath), "package", packageId, .. GetVersionArguments(version)],
            cancellationToken);
    }

    public Task<string> RemovePackageAsync(
        string projectPath,
        string packageId,
        CancellationToken cancellationToken = default)
    {
        ValidateProjectPath(projectPath);
        ValidatePackageId(packageId);
        return RunDotNetAsync(
            projectPath,
            ["remove", Path.GetFullPath(projectPath), "package", packageId],
            cancellationToken);
    }

    private static void ValidateProjectPath(string projectPath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(projectPath);
        if (!string.Equals(Path.GetExtension(projectPath), ".csproj", StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException("A C# project file path ending in .csproj is required.", nameof(projectPath));
        }

        if (!File.Exists(projectPath))
        {
            throw new FileNotFoundException("The C# project file does not exist.", projectPath);
        }
    }

    private static string GetStringProperty(JsonElement element, string propertyName)
    {
        if (element.TryGetProperty(propertyName, out JsonElement property)
            && property.ValueKind == JsonValueKind.String
            && !string.IsNullOrWhiteSpace(property.GetString()))
        {
            return property.GetString()!;
        }

        throw new InvalidDataException($"The package response is missing a valid '{propertyName}' value.");
    }

    private static string GetOptionalStringProperty(JsonElement element, string propertyName)
    {
        return element.TryGetProperty(propertyName, out JsonElement property)
            && property.ValueKind == JsonValueKind.String
                ? property.GetString() ?? string.Empty
                : string.Empty;
    }

    private static void ValidatePackageId(string packageId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(packageId);
        if (!s_packageIdPattern.IsMatch(packageId))
        {
            throw new ArgumentException("The NuGet package ID contains unsupported characters.", nameof(packageId));
        }
    }

    private static string[] GetVersionArguments(string? version)
    {
        return string.IsNullOrWhiteSpace(version) ? [] : ["--version", version];
    }

    private async Task<string> RunDotNetAsync(
        string projectPath,
        string[] arguments,
        CancellationToken cancellationToken,
        bool includeStandardErrorInOutput = true)
    {
        ProcessStartInfo startInfo = new(_dotnetExecutable)
        {
            WorkingDirectory = Path.GetDirectoryName(Path.GetFullPath(projectPath))
                ?? throw new ArgumentException("The project file must have a parent directory.", nameof(projectPath)),
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };
        foreach (string argument in arguments)
        {
            startInfo.ArgumentList.Add(argument);
        }

        using Process process = Process.Start(startInfo)
            ?? throw new InvalidOperationException("The .NET package command could not be started.");
        using CancellationTokenRegistration registration = cancellationToken.Register(() =>
        {
            try
            {
                if (!process.HasExited)
                {
                    process.Kill(entireProcessTree: true);
                }
            }
            catch (InvalidOperationException)
            {
            }
            catch (Win32Exception)
            {
            }
        });

        Task<string> outputTask = process.StandardOutput.ReadToEndAsync(cancellationToken);
        Task<string> errorTask = process.StandardError.ReadToEndAsync(cancellationToken);
        await process.WaitForExitAsync(cancellationToken).ConfigureAwait(false);
        string output = await outputTask.ConfigureAwait(false);
        string error = await errorTask.ConfigureAwait(false);
        string combinedOutput = string.IsNullOrWhiteSpace(error)
            ? output.Trim()
            : $"{output.Trim()}{Environment.NewLine}{error.Trim()}".Trim();
        if (process.ExitCode != 0)
        {
            throw new InvalidOperationException(
                $"dotnet {string.Join(' ', arguments)} failed with exit code {process.ExitCode}: {combinedOutput}");
        }

        return includeStandardErrorInOutput ? combinedOutput : output.Trim();
    }
}
