using AppKomponentBaseLib.Diagnostics;
using AA98_AvlnCodeStudio.Planning.Core.Models;
using AA98_AvlnCodeStudio.Planning.Core.Services;
using BaseReader = AA98_AvlnCodeStudio.Planning.Core.Services.MarkdownPlanningReader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.Planning.Local.Services;

/// <summary>
/// Implements the first local markdown-backed planning provider with embedded templates.
/// </summary>
public sealed class LocalPlanningProvider : IPlanningProvider
{
    private static readonly Dictionary<PlanningItemKind, string> s_templateNames = new()
    {
        [PlanningItemKind.Epic] = "Epic.md",
        [PlanningItemKind.Feature] = "Feature.md",
        [PlanningItemKind.BacklogItem] = "BacklogItem.md",
        [PlanningItemKind.Task] = "Task.md",
        [PlanningItemKind.Bug] = "Bug.md",
        [PlanningItemKind.TestCase] = "TestCase.md",
        [PlanningItemKind.Impediment] = "Impediment.md",
    };

    private static readonly Dictionary<PlanningItemKind, string> s_folderNames = new()
    {
        [PlanningItemKind.Epic] = "Epics",
        [PlanningItemKind.Feature] = "Features",
        [PlanningItemKind.BacklogItem] = "BacklogItems",
        [PlanningItemKind.Task] = "Tasks",
        [PlanningItemKind.Bug] = "Bugs",
        [PlanningItemKind.TestCase] = "TestCases",
        [PlanningItemKind.Impediment] = "Impediments",
    };

    public async Task<PlanningReadResult> ReadAsync(PlanningReadRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        BaseReader reader = new();
        PlanningReadResult result = await reader.ReadAsync(new PlanningReadRequest
        {
            RepositoryRootPath = request.RepositoryRootPath,
            PlanningRootPath = request.PlanningRootPath,
        }, cancellationToken).ConfigureAwait(false);

        return result;
    }

    public async Task<PlanningWriteResult> WriteAsync(PlanningWriteRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        cancellationToken.ThrowIfCancellationRequested();

        string repositoryRootPath = string.IsNullOrWhiteSpace(request.RepositoryRootPath)
            ? Directory.GetCurrentDirectory()
            : Path.GetFullPath(request.RepositoryRootPath);
        string planningRootPath = Path.GetFullPath(Path.Combine(repositoryRootPath, request.PlanningRootPath));
        Directory.CreateDirectory(planningRootPath);

        IReadOnlyList<PlanningDocumentTemplate> templates = await GetTemplatesAsync(cancellationToken).ConfigureAwait(false);
        Dictionary<PlanningItemKind, PlanningDocumentTemplate> templatesByKind = templates.ToDictionary(static template => template.Kind);

        PlanningWriteResult result = new();
        foreach (PlanningItem item in request.Items)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!templatesByKind.TryGetValue(item.Kind, out PlanningDocumentTemplate? template))
            {
                result.Diagnostics.Add(new Diagnostic
                {
                    Code = "PLW001",
                    Message = $"No local template is registered for planning kind '{item.Kind}'.",
                    Severity = DiagnosticSeverity.Error,
                    SourcePath = item.SourcePath,
                });
                continue;
            }

            string relativeSourcePath = ResolveSourcePath(item, request.PlanningRootPath);
            string fullSourcePath = Path.GetFullPath(Path.Combine(repositoryRootPath, relativeSourcePath));
            string? directoryPath = Path.GetDirectoryName(fullSourcePath);
            if (!string.IsNullOrWhiteSpace(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            if (HasDocumentConflict(request, item, relativeSourcePath, fullSourcePath, out Diagnostic? conflictDiagnostic))
            {
                result.Diagnostics.Add(conflictDiagnostic);
                continue;
            }

            string content = File.Exists(fullSourcePath)
                ? UpdateExistingDocument(ResolveDocumentText(fullSourcePath, item), item)
                : RenderTemplate(template.Content, item);
            await File.WriteAllTextAsync(fullSourcePath, content, cancellationToken).ConfigureAwait(false);
            result.WrittenSourcePaths.Add(relativeSourcePath);
        }

        return result;
    }

    public Task<IReadOnlyList<PlanningDocumentTemplate>> GetTemplatesAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Assembly assembly = typeof(LocalPlanningProvider).Assembly;
        List<PlanningDocumentTemplate> templates = [];
        foreach (KeyValuePair<PlanningItemKind, string> pair in s_templateNames)
        {
            string resourceName = $"AA98_AvlnCodeStudio.Planning.Local.Templates.{pair.Value}";
            using Stream? stream = assembly.GetManifestResourceStream(resourceName);
            if (stream is null)
            {
                continue;
            }

            using StreamReader reader = new(stream);
            string content = reader.ReadToEnd();
            templates.Add(new PlanningDocumentTemplate
            {
                TemplateId = pair.Key.ToString(),
                Kind = pair.Key,
                DisplayName = pair.Key.ToString(),
                SuggestedFileNamePattern = $"{{ItemId}}-{{Title}}.md",
                Content = content,
            });
        }

        return Task.FromResult<IReadOnlyList<PlanningDocumentTemplate>>(templates);
    }

    private static string ResolveSourcePath(PlanningItem item, string planningRootPath)
    {
        if (!string.IsNullOrWhiteSpace(item.SourcePath))
        {
            return item.SourcePath;
        }

        string folderName = s_folderNames.TryGetValue(item.Kind, out string? mappedFolderName)
            ? mappedFolderName
            : "Documents";
        string safeTitle = item.Title
            .Replace(' ', '-')
            .Replace("/", "-")
            .Replace("\\", "-");
        return Path.Combine(planningRootPath, folderName, $"{item.Id}-{safeTitle}.md");
    }

    private static string RenderTemplate(string templateContent, PlanningItem item)
    {
        string parentSection = BuildParentSection(item);
        return templateContent
            .Replace("{{ItemId}}", item.Id, StringComparison.Ordinal)
            .Replace("{{Title}}", item.Title, StringComparison.Ordinal)
            .Replace("{{Status}}", item.Status switch
            {
                PlanningItemStatus.InProgress => "In Progress",
                PlanningItemStatus.Blocked => "Blocked",
                PlanningItemStatus.Completed => "Done",
                PlanningItemStatus.Cancelled => "Cancelled",
                _ => "Proposed",
            }, StringComparison.Ordinal)
            .Replace("{{ParentSection}}", parentSection, StringComparison.Ordinal)
            .ReplaceLineEndings(Environment.NewLine);
    }

    private static string ResolveDocumentText(string fullSourcePath, PlanningItem item)
        => string.IsNullOrEmpty(item.DocumentText)
            ? File.ReadAllText(fullSourcePath)
            : item.DocumentText;

    private static bool HasDocumentConflict(
        PlanningWriteRequest request,
        PlanningItem item,
        string relativeSourcePath,
        string fullSourcePath,
        out Diagnostic? diagnostic)
    {
        diagnostic = null;
        if (!File.Exists(fullSourcePath)
            || !TryGetExpectedDocumentText(request, item.SourcePath, relativeSourcePath, out string? expectedDocumentText))
        {
            return false;
        }

        string currentDocumentText = File.ReadAllText(fullSourcePath);
        if (string.Equals(currentDocumentText, expectedDocumentText, StringComparison.Ordinal))
        {
            return false;
        }

        diagnostic = new Diagnostic
        {
            Code = "PLW002",
            Message = "The planning document changed after it was loaded. Reload it before saving.",
            Severity = DiagnosticSeverity.Error,
            SourcePath = relativeSourcePath,
        };
        return true;
    }

    private static bool TryGetExpectedDocumentText(
        PlanningWriteRequest request,
        string sourcePath,
        string relativeSourcePath,
        out string? expectedDocumentText)
    {
        if (!string.IsNullOrWhiteSpace(sourcePath)
            && request.ExpectedDocumentTexts.TryGetValue(sourcePath, out expectedDocumentText))
        {
            return true;
        }

        return request.ExpectedDocumentTexts.TryGetValue(relativeSourcePath, out expectedDocumentText);
    }

    private static string UpdateExistingDocument(string documentText, PlanningItem item)
    {
        string updatedDocument = Regex.Replace(
            documentText,
            "^# .*$",
            $"# {item.Id} {item.Title}",
            RegexOptions.Multiline);

        string statusText = item.Status switch
        {
            PlanningItemStatus.InProgress => "In Progress",
            PlanningItemStatus.Blocked => "Blocked",
            PlanningItemStatus.Completed => "Done",
            PlanningItemStatus.Cancelled => "Cancelled",
            _ => "Proposed",
        };

        return Regex.Replace(
            updatedDocument,
            "(?ms)(^## Status\\r?\\n)(.*?)(?=^## |\\z)",
            match => $"{match.Groups[1].Value}- {statusText}{Environment.NewLine}");
    }

    private static string BuildParentSection(PlanningItem item)
    {
        if (item.Parent is null && item.RelatedParents.Count == 0)
        {
            return string.Empty;
        }

        List<string> lines = ["## Parent"];
        if (item.Parent is not null)
        {
            lines.Add($"- {MapLabel(item.Parent.Kind)}: `{item.Parent.SourcePath ?? item.Parent.ItemId}`");
        }

        foreach (PlanningItemLink relatedParent in item.RelatedParents)
        {
            lines.Add($"- {MapLabel(relatedParent.Kind)}: `{relatedParent.SourcePath ?? relatedParent.ItemId}`");
        }

        lines.Add(string.Empty);
        return string.Join(Environment.NewLine, lines) + Environment.NewLine;
    }

    private static string MapLabel(PlanningItemKind kind)
        => kind switch
        {
            PlanningItemKind.Epic => "Epic",
            PlanningItemKind.Feature => "Feature",
            PlanningItemKind.BacklogItem => "Backlog Item",
            PlanningItemKind.Task => "Task",
            PlanningItemKind.Bug => "Bug",
            PlanningItemKind.Impediment => "Impediment",
            PlanningItemKind.TestCase => "Test Case",
            _ => "Related",
        };
}
