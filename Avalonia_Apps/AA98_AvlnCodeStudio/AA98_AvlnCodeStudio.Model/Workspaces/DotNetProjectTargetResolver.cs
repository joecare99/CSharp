using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.Model.Workspaces;

/// <summary>
/// Inspects evaluated MSBuild properties without building or changing the project.
/// Project evaluation may execute imports and property functions, so callers must obtain explicit consent.
/// </summary>
public sealed class DotNetProjectTargetResolver : IProjectTargetResolver
{
    private const string PropertyNames =
        "Configuration,Configurations,TargetFramework,TargetFrameworks,TargetPath,OutputType,TargetFrameworkIdentifier,IsTestProject";

    public async Task<ProjectTargetInspection> InspectAsync(
        ProjectWorkflowContext context,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);
        cancellationToken.ThrowIfCancellationRequested();
        if (!File.Exists(context.Project.ProjectPath))
        {
            throw new FileNotFoundException("The selected project does not exist.", context.Project.ProjectPath);
        }

        Dictionary<string, string> outer = await EvaluateAsync(context, null, cancellationToken).ConfigureAwait(false);
        string[] configurations = SplitValues(outer["Configurations"], "configuration");
        if (configurations.Length > 0
            && !configurations.Contains(context.Configuration, StringComparer.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                $"Configuration '{context.Configuration}' is not declared by the project. Available: {string.Join(", ", configurations)}.");
        }

        string[] frameworks = SplitValues(outer["TargetFrameworks"], "target framework");
        if (frameworks.Length == 0 && !string.IsNullOrWhiteSpace(outer["TargetFramework"]))
        {
            frameworks = SplitValues(outer["TargetFramework"], "target framework");
        }

        string? framework = context.TargetFramework;
        if (framework is not null && !frameworks.Contains(framework, StringComparer.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                $"Target framework '{framework}' is not declared by the project. Available: {string.Join(", ", frameworks)}.");
        }

        framework ??= frameworks.Length == 1 ? frameworks[0] : null;
        if (framework is null)
        {
            return new ProjectTargetInspection
            {
                Context = context,
                Configurations = configurations,
                TargetFrameworks = frameworks,
                DebugUnavailableReason = frameworks.Length == 0
                    ? "The project has no supported target framework."
                    : "Select a target framework and inspect the project again.",
            };
        }

        Dictionary<string, string> inner = await EvaluateAsync(context, framework, cancellationToken).ConfigureAwait(false);
        string outputType = inner["OutputType"];
        string frameworkIdentifier = inner["TargetFrameworkIdentifier"];
        string targetPath = inner["TargetPath"];
        string? reason = GetDebugUnavailableReason(outputType, frameworkIdentifier, targetPath);
        return new ProjectTargetInspection
        {
            Context = context,
            Configurations = configurations,
            TargetFrameworks = frameworks,
            TargetFramework = framework,
            TargetPath = string.IsNullOrWhiteSpace(targetPath) ? null : targetPath,
            OutputType = outputType,
            FrameworkIdentifier = frameworkIdentifier,
            IsTestProject = inner["IsTestProject"],
            DebugUnavailableReason = reason,
        };
    }

    private static string? GetDebugUnavailableReason(string outputType, string frameworkIdentifier, string targetPath)
    {
        if (!string.Equals(outputType, "Exe", StringComparison.OrdinalIgnoreCase)
            && !string.Equals(outputType, "WinExe", StringComparison.OrdinalIgnoreCase))
        {
            return $"The project output type '{outputType}' is not executable.";
        }

        if (!string.Equals(frameworkIdentifier, ".NETCoreApp", StringComparison.OrdinalIgnoreCase))
        {
            return $"The target framework type '{frameworkIdentifier}' is not supported by this debugger.";
        }

        if (string.IsNullOrWhiteSpace(targetPath)
            || !Path.IsPathFullyQualified(targetPath)
            || !string.Equals(Path.GetExtension(targetPath), ".dll", StringComparison.OrdinalIgnoreCase))
        {
            return "The project did not resolve an absolute .NET assembly target path.";
        }

        return null;
    }

    private static string[] SplitValues(string value, string description)
    {
        return value.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(item => ProjectWorkflowContext.ValidateValue(item, description))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private static async Task<Dictionary<string, string>> EvaluateAsync(
        ProjectWorkflowContext context,
        string? framework,
        CancellationToken cancellationToken)
    {
        ProcessStartInfo startInfo = new("dotnet")
        {
            WorkingDirectory = Path.GetDirectoryName(context.Project.ProjectPath)!,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };
        startInfo.Environment.Remove("MSBuildSDKsPath");
        startInfo.Environment.Remove("MSBuildExtensionsPath");
        startInfo.Environment.Remove("MSBuildExtensionsPath32");
        startInfo.Environment.Remove("MSBuildToolsPath");
        startInfo.Environment.Remove("DOTNET_MSBUILD_SDK_RESOLVER_SDKS_DIR");
        startInfo.Environment.Remove("DOTNET_MSBUILD_SDK_RESOLVER_CLI_DIR");
        startInfo.ArgumentList.Add("msbuild");
        startInfo.ArgumentList.Add(context.Project.ProjectPath);
        startInfo.ArgumentList.Add("-nologo");
        startInfo.ArgumentList.Add($"-getProperty:{PropertyNames}");
        startInfo.ArgumentList.Add($"-property:Configuration={context.Configuration}");
        if (framework is not null)
        {
            startInfo.ArgumentList.Add($"-property:TargetFramework={framework}");
        }

        cancellationToken.ThrowIfCancellationRequested();
        using Process process = Process.Start(startInfo)
            ?? throw new InvalidOperationException("The dotnet project inspection process could not be started.");
        Task<string> outputTask = process.StandardOutput.ReadToEndAsync();
        Task<string> errorTask = process.StandardError.ReadToEndAsync();
        try
        {
            await process.WaitForExitAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            try
            {
                if (!process.HasExited)
                {
                    process.Kill(entireProcessTree: true);
                }
            }
            catch (Exception exception) when (exception is InvalidOperationException or Win32Exception)
            {
            }

            await process.WaitForExitAsync(CancellationToken.None).ConfigureAwait(false);
            throw;
        }

        string output = await outputTask.ConfigureAwait(false);
        string error = await errorTask.ConfigureAwait(false);
        cancellationToken.ThrowIfCancellationRequested();
        if (process.ExitCode != 0)
        {
            string detail = string.IsNullOrWhiteSpace(error) ? output : error;
            detail = detail.Trim();
            if (detail.Length > 4096)
            {
                detail = detail[..4096];
            }

            throw new InvalidOperationException($"Project inspection failed (exit code {process.ExitCode}): {detail}");
        }

        using JsonDocument document = JsonDocument.Parse(output);
        JsonElement properties = document.RootElement.GetProperty("Properties");
        return PropertyNames.Split(',').ToDictionary(
            name => name,
            name => properties.TryGetProperty(name, out JsonElement value) ? value.GetString() ?? string.Empty : string.Empty,
            StringComparer.Ordinal);
    }
}
