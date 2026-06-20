using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Workbench.Builder.Core.Abstractions;
using Workbench.Builder.Core.Models.Diagnostics;
using Workbench.Builder.Core.Models.Inspection;
using Workbench.Builder.Core.Models.Projects;
using Workbench.Builder.Core.Models.References;

namespace Workbench.Builder.Core.Services.Formatting;

/// <summary>
/// Formats a <see cref="ProjectInspectionResult"/> for plain-text or JSON host output.
/// </summary>
public sealed class ProjectInspectionFormatter : IProjectInspectionFormatter
{
    private static readonly JsonSerializerOptions JsonOptions = CreateJsonOptions();

    /// <inheritdoc/>
    public string Format(ProjectInspectionResult result, ProjectInspectionOutputFormat format)
    {
        ArgumentNullException.ThrowIfNull(result);

        return format switch
        {
            ProjectInspectionOutputFormat.PlainText => FormatPlainText(result),
            ProjectInspectionOutputFormat.Json => JsonSerializer.Serialize(result, JsonOptions),
            _ => throw new ArgumentOutOfRangeException(nameof(format), format, "The requested project inspection output format is not supported."),
        };
    }

    private static JsonSerializerOptions CreateJsonOptions()
    {
        JsonSerializerOptions options = new()
        {
            WriteIndented = true,
        };

        options.Converters.Add(new JsonStringEnumConverter());
        return options;
    }

    private static string FormatPlainText(ProjectInspectionResult result)
    {
        StringBuilder builder = new();

        builder.AppendLine("Project Inspection");
        builder.AppendLine($"Project File: {result.Project.ProjectFilePath}");
        builder.AppendLine($"Assembly Name: {result.Project.AssemblyName}");
        builder.AppendLine($"Root Namespace: {result.Project.RootNamespace}");
        builder.AppendLine($"Target Framework: {result.Project.TargetFramework}");
        builder.AppendLine($"Output Type: {FormatValue(result.Project.OutputType)}");
        builder.AppendLine($"Configuration: {FormatValue(result.Project.Configuration)}");
        builder.AppendLine($"Runtime Identifier: {FormatValue(result.Project.RuntimeIdentifier)}");
        builder.AppendLine($"SDK Style: {FormatBoolean(result.Project.IsSdkStyle)}");
        builder.AppendLine($"Packable: {FormatNullableBoolean(result.Project.IsPackable)}");
        builder.AppendLine($"Test Project: {FormatBoolean(result.IsTestProject)}");
        builder.AppendLine();

        AppendCompileItems(builder, result.CompileItems);
        builder.AppendLine();
        AppendProjectReferences(builder, result.ProjectReferences);
        builder.AppendLine();
        AppendPackageReferences(builder, result.PackageReferences);
        builder.AppendLine();
        AppendResolvedReferences(builder, result.ResolvedReferences);
        builder.AppendLine();
        AppendDiagnostics(builder, result.Diagnostics);

        return builder.ToString().TrimEnd();
    }

    private static void AppendCompileItems(StringBuilder builder, IReadOnlyList<CompileItemInfo> compileItems)
    {
        builder.AppendLine($"Compile Items ({compileItems.Count})");

        if (compileItems.Count == 0)
        {
            builder.AppendLine("- none");
            return;
        }

        foreach (CompileItemInfo compileItem in compileItems)
        {
            builder.AppendLine($"- {compileItem.Include} -> {compileItem.FilePath} [{FormatExists(compileItem.Exists)}]");
        }
    }

    private static void AppendProjectReferences(StringBuilder builder, IReadOnlyList<ProjectReferenceInfo> projectReferences)
    {
        builder.AppendLine($"Project References ({projectReferences.Count})");

        if (projectReferences.Count == 0)
        {
            builder.AppendLine("- none");
            return;
        }

        foreach (ProjectReferenceInfo projectReference in projectReferences)
        {
            builder.AppendLine($"- {projectReference.Include} -> {projectReference.ProjectFilePath} [{FormatExists(projectReference.Exists)}]");
        }
    }

    private static void AppendPackageReferences(StringBuilder builder, IReadOnlyList<PackageReferenceInfo> packageReferences)
    {
        builder.AppendLine($"Package References ({packageReferences.Count})");

        if (packageReferences.Count == 0)
        {
            builder.AppendLine("- none");
            return;
        }

        foreach (PackageReferenceInfo packageReference in packageReferences)
        {
            builder.AppendLine($"- {packageReference.PackageId} ({FormatValue(packageReference.Version)}) PrivateAssets={FormatValue(packageReference.PrivateAssets)}");
        }
    }

    private static void AppendResolvedReferences(StringBuilder builder, IReadOnlyList<ResolvedReferenceInfo> resolvedReferences)
    {
        builder.AppendLine($"Resolved References ({resolvedReferences.Count})");

        if (resolvedReferences.Count == 0)
        {
            builder.AppendLine("- none");
            return;
        }

        foreach (ResolvedReferenceInfo resolvedReference in resolvedReferences)
        {
            builder.AppendLine($"- {resolvedReference.Kind}: {resolvedReference.DisplayName} | Source={FormatValue(resolvedReference.Source)} | Path={FormatValue(resolvedReference.ResolvedPath)} | Exists={FormatBoolean(resolvedReference.Exists)}");
        }
    }

    private static void AppendDiagnostics(StringBuilder builder, IReadOnlyList<BuildDiagnostic> diagnostics)
    {
        builder.AppendLine($"Diagnostics ({diagnostics.Count})");

        if (diagnostics.Count == 0)
        {
            builder.AppendLine("- none");
            return;
        }

        foreach (BuildDiagnostic diagnostic in diagnostics)
        {
            builder.AppendLine($"- {BuildDiagnosticTextFormatter.Format(diagnostic)}");
        }
    }

    private static string FormatValue(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? "(none)" : value;
    }

    private static string FormatExists(bool exists)
    {
        return exists ? "exists" : "missing";
    }

    private static string FormatBoolean(bool value)
    {
        return value ? "true" : "false";
    }

    private static string FormatNullableBoolean(bool? value)
    {
        return value.HasValue ? FormatBoolean(value.Value) : "(unknown)";
    }
}
