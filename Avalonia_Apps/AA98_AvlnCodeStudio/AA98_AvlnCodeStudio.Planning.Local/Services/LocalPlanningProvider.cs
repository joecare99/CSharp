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

        PlanningWriteResult result = new();
        string repositoryRootPath;
        try
        {
            repositoryRootPath = string.IsNullOrWhiteSpace(request.RepositoryRootPath)
                ? Directory.GetCurrentDirectory()
                : Path.GetFullPath(request.RepositoryRootPath);
            repositoryRootPath = ResolveExistingLinks(repositoryRootPath);
        }
        catch (Exception exception) when (IsPathResolutionException(exception))
        {
            AddPathDiagnostic(result, request.RepositoryRootPath, $"The repository root could not be resolved safely: {exception.Message}");
            return result;
        }

        if (!TryNormalizeRelativePath(request.PlanningRootPath, allowCurrentDirectory: true, out string normalizedPlanningRoot, out string planningRootError)
            || !TryResolvePathWithinRoot(
                repositoryRootPath,
                Path.Combine(repositoryRootPath, normalizedPlanningRoot),
                out string planningRootPath,
                out planningRootError))
        {
            AddPathDiagnostic(result, request.PlanningRootPath, planningRootError);
            return result;
        }

        try
        {
            Directory.CreateDirectory(planningRootPath);
        }
        catch (Exception exception) when (IsPathResolutionException(exception))
        {
            AddPathDiagnostic(result, request.PlanningRootPath, $"The planning root could not be created safely: {exception.Message}");
            return result;
        }

        if (!TryResolvePathWithinRoot(repositoryRootPath, planningRootPath, out planningRootPath, out planningRootError))
        {
            AddPathDiagnostic(result, request.PlanningRootPath, planningRootError);
            return result;
        }

        IReadOnlyList<PlanningDocumentTemplate> templates = await GetTemplatesAsync(cancellationToken).ConfigureAwait(false);
        Dictionary<PlanningItemKind, PlanningDocumentTemplate> templatesByKind = templates.ToDictionary(static template => template.Kind);

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

            string requestedSourcePath = ResolveSourcePath(item, normalizedPlanningRoot);
            if (!TryNormalizeRelativePath(requestedSourcePath, allowCurrentDirectory: false, out string relativeSourcePath, out string sourcePathError))
            {
                AddPathDiagnostic(result, item.SourcePath, sourcePathError);
                continue;
            }

            if (!TryResolvePathWithinRoot(
                planningRootPath,
                Path.Combine(repositoryRootPath, relativeSourcePath),
                out string fullSourcePath,
                out sourcePathError))
            {
                AddPathDiagnostic(result, item.SourcePath, sourcePathError);
                continue;
            }

            string? directoryPath = Path.GetDirectoryName(fullSourcePath);
            if (!string.IsNullOrWhiteSpace(directoryPath))
            {
                if (!TryResolvePathWithinRoot(planningRootPath, directoryPath, out directoryPath, out sourcePathError))
                {
                    AddPathDiagnostic(result, item.SourcePath, sourcePathError);
                    continue;
                }

                try
                {
                    Directory.CreateDirectory(directoryPath);
                }
                catch (Exception exception) when (IsPathResolutionException(exception))
                {
                    AddPathDiagnostic(result, item.SourcePath, $"The planning document directory could not be created safely: {exception.Message}");
                    continue;
                }

                if (!TryResolvePathWithinRoot(
                    planningRootPath,
                    Path.Combine(directoryPath, Path.GetFileName(fullSourcePath)),
                    out fullSourcePath,
                    out sourcePathError))
                {
                    AddPathDiagnostic(result, item.SourcePath, sourcePathError);
                    continue;
                }
            }

            if (HasDocumentConflict(request, item, relativeSourcePath, fullSourcePath, out Diagnostic? conflictDiagnostic))
            {
                result.Diagnostics.Add(conflictDiagnostic);
                continue;
            }

            string content = File.Exists(fullSourcePath)
                ? UpdateExistingDocument(ResolveDocumentText(fullSourcePath, item), item)
                : RenderTemplate(template.Content, item);

            if (!TryResolvePathWithinRoot(planningRootPath, fullSourcePath, out fullSourcePath, out sourcePathError))
            {
                AddPathDiagnostic(result, item.SourcePath, sourcePathError);
                continue;
            }

            await File.WriteAllTextAsync(fullSourcePath, content, cancellationToken).ConfigureAwait(false);
            result.WrittenSourcePaths.Add(relativeSourcePath);
        }

        return result;
    }

    private static bool TryNormalizeRelativePath(
        string? path,
        bool allowCurrentDirectory,
        out string normalizedPath,
        out string errorMessage)
    {
        normalizedPath = string.Empty;
        errorMessage = string.Empty;
        if (string.IsNullOrWhiteSpace(path))
        {
            if (allowCurrentDirectory)
            {
                normalizedPath = ".";
                return true;
            }

            errorMessage = "The planning document path must be a non-empty repository-relative path.";
            return false;
        }

        if (Path.IsPathRooted(path)
            || path.StartsWith("\\\\", StringComparison.Ordinal)
            || path.StartsWith("//", StringComparison.Ordinal)
            || (path.Length >= 2 && char.IsAsciiLetter(path[0]) && path[1] == ':'))
        {
            errorMessage = "Rooted planning paths are not allowed.";
            return false;
        }

        string platformPath = path
            .Replace('\\', Path.DirectorySeparatorChar)
            .Replace('/', Path.DirectorySeparatorChar);
        if (Path.IsPathRooted(platformPath))
        {
            errorMessage = "Rooted planning paths are not allowed.";
            return false;
        }

        string[] segments = platformPath.Split(
            [Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar],
            StringSplitOptions.RemoveEmptyEntries);
        if (segments.Any(static segment => segment == ".."))
        {
            errorMessage = "Parent-directory traversal is not allowed in planning paths.";
            return false;
        }

        if (!allowCurrentDirectory && (segments.Length == 0 || segments.All(static segment => segment == ".")))
        {
            errorMessage = "The planning document path must identify a file inside the planning root.";
            return false;
        }

        normalizedPath = segments.Length == 0 ? "." : Path.Combine(segments);
        return true;
    }

    private static bool TryResolvePathWithinRoot(
        string rootPath,
        string candidatePath,
        out string resolvedPath,
        out string errorMessage)
    {
        resolvedPath = string.Empty;
        errorMessage = string.Empty;
        try
        {
            string fullCandidatePath = Path.GetFullPath(candidatePath);
            if (!IsPathWithin(rootPath, fullCandidatePath))
            {
                errorMessage = "The planning path resolves outside the allowed planning root.";
                return false;
            }

            resolvedPath = ResolveExistingLinks(fullCandidatePath);
            if (!IsPathWithin(rootPath, resolvedPath))
            {
                errorMessage = "A symbolic link redirects the planning path outside the allowed planning root.";
                resolvedPath = string.Empty;
                return false;
            }

            return true;
        }
        catch (Exception exception) when (IsPathResolutionException(exception))
        {
            errorMessage = $"The planning path could not be resolved safely: {exception.Message}";
            resolvedPath = string.Empty;
            return false;
        }
    }

    private static string ResolveExistingLinks(string path)
    {
        string fullPath = Path.GetFullPath(path);
        string pathRoot = Path.GetPathRoot(fullPath)
            ?? throw new IOException("The path has no filesystem root.");
        string resolvedPath = pathRoot;
        string[] segments = fullPath[pathRoot.Length..].Split(
            [Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar],
            StringSplitOptions.RemoveEmptyEntries);

        foreach (string segment in segments)
        {
            string candidatePath = Path.Combine(resolvedPath, segment);
            FileSystemInfo pathInfo = Directory.Exists(candidatePath)
                ? new DirectoryInfo(candidatePath)
                : new FileInfo(candidatePath);
            if (pathInfo.LinkTarget is not null)
            {
                FileSystemInfo target = pathInfo.ResolveLinkTarget(returnFinalTarget: true)
                    ?? throw new IOException($"The symbolic link '{candidatePath}' could not be resolved.");
                resolvedPath = Path.GetFullPath(target.FullName);
            }
            else
            {
                resolvedPath = candidatePath;
            }
        }

        return Path.GetFullPath(resolvedPath);
    }

    private static bool IsPathWithin(string rootPath, string candidatePath)
    {
        string relativePath = Path.GetRelativePath(rootPath, candidatePath);
        return !Path.IsPathRooted(relativePath)
            && relativePath != ".."
            && !relativePath.StartsWith($"..{Path.DirectorySeparatorChar}", StringComparison.Ordinal);
    }

    private static bool IsPathResolutionException(Exception exception)
        => exception is ArgumentException
            or IOException
            or UnauthorizedAccessException
            or NotSupportedException;

    private static void AddPathDiagnostic(PlanningWriteResult result, string? sourcePath, string message)
    {
        result.Diagnostics.Add(new Diagnostic
        {
            Code = "PLW003",
            Message = message,
            Severity = DiagnosticSeverity.Error,
            SourcePath = sourcePath,
        });
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
