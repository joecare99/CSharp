using AA98_AvlnCodeStudio.Planning.Core.Models;
using AppKomponentBaseLib.Diagnostics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.Planning.Core.Services;

/// <summary>
/// Provides deterministic loading of local markdown planning items from the repository planning tree.
/// </summary>
public sealed class MarkdownPlanningReader : IPlanningProvider
{
    private static readonly (string FolderName, PlanningItemKind Kind)[] s_supportedFolders =
    [
        ("Epics", PlanningItemKind.Epic),
        ("Features", PlanningItemKind.Feature),
        ("BacklogItems", PlanningItemKind.BacklogItem),
        ("Tasks", PlanningItemKind.Task),
        ("Bugs", PlanningItemKind.Bug),
        ("TestCases", PlanningItemKind.TestCase),
        ("Impediments", PlanningItemKind.Impediment),
    ];

    /// <inheritdoc/>
    public Task<PlanningReadResult> ReadAsync(PlanningReadRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        cancellationToken.ThrowIfCancellationRequested();

        string repositoryRootPath = string.IsNullOrWhiteSpace(request.RepositoryRootPath)
            ? Directory.GetCurrentDirectory()
            : Path.GetFullPath(request.RepositoryRootPath);
        string planningRootPath = Path.GetFullPath(Path.Combine(repositoryRootPath, request.PlanningRootPath));

        PlanningReadResult result = new()
        {
            RepositoryRootPath = repositoryRootPath,
            PlanningRootPath = planningRootPath,
        };

        if (!Directory.Exists(planningRootPath))
        {
            result.Diagnostics.Add(new Diagnostic
            {
                Code = "PLN001",
                Message = "The planning root directory does not exist.",
                Severity = DiagnosticSeverity.Error,
                SourcePath = request.PlanningRootPath,
            });

            return Task.FromResult(result);
        }

        foreach ((string folderName, PlanningItemKind kind) in s_supportedFolders)
        {
            cancellationToken.ThrowIfCancellationRequested();

            string folderPath = Path.Combine(planningRootPath, folderName);
            if (!Directory.Exists(folderPath))
            {
                continue;
            }

            foreach (string filePath in Directory.EnumerateFiles(folderPath, "*.md", SearchOption.TopDirectoryOnly))
            {
                cancellationToken.ThrowIfCancellationRequested();
                result.Items.Add(ReadItem(repositoryRootPath, filePath, kind));
            }
        }

        ApplyCrossItemDiagnostics(result);
        return Task.FromResult(result);
    }

    private static PlanningItem ReadItem(string repositoryRootPath, string filePath, PlanningItemKind kind)
    {
        string[] lines = File.ReadAllLines(filePath);
        string sourcePath = Path.GetRelativePath(repositoryRootPath, filePath);
        PlanningItem item = new()
        {
            Kind = kind,
            SourcePath = sourcePath,
        };

        ParseHeading(item, lines);
        ParseParent(item, lines);
        ParseStatus(item, lines);
        return item;
    }

    private static void ParseHeading(PlanningItem item, IReadOnlyList<string> lines)
    {
        string? headingLine = lines.FirstOrDefault(static line => line.StartsWith("# ", StringComparison.Ordinal));
        if (string.IsNullOrWhiteSpace(headingLine))
        {
            item.Diagnostics.Add(new Diagnostic
            {
                Code = "PLN010",
                Message = "The planning item is missing a top-level heading.",
                Severity = DiagnosticSeverity.Error,
                SourcePath = item.SourcePath,
            });

            return;
        }

        string headingContent = headingLine[2..].Trim();
        int separatorIndex = headingContent.IndexOf(' ');
        if (separatorIndex <= 0 || separatorIndex == headingContent.Length - 1)
        {
            item.Diagnostics.Add(new Diagnostic
            {
                Code = "PLN011",
                Message = "The planning item heading must contain an ID and title.",
                Severity = DiagnosticSeverity.Error,
                SourcePath = item.SourcePath,
            });

            item.Id = headingContent;
            return;
        }

        item.Id = headingContent[..separatorIndex].Trim();
        item.Title = headingContent[(separatorIndex + 1)..].Trim();
    }

    private static void ParseParent(PlanningItem item, IReadOnlyList<string> lines)
    {
        int parentSectionIndex = FindSectionIndex(lines, "## Parent");
        if (parentSectionIndex < 0)
        {
            return;
        }

        List<(PlanningItemLink Link, string ParentContent, int LineNumber)> parentEntries = [];

        for (int i = parentSectionIndex + 1; i < lines.Count; i++)
        {
            string line = lines[i];
            if (line.StartsWith("## ", StringComparison.Ordinal))
            {
                break;
            }

            string trimmed = line.Trim();
            if (!trimmed.StartsWith("- ", StringComparison.Ordinal))
            {
                continue;
            }

            string parentContent = trimmed[2..].Trim();
            if (string.IsNullOrWhiteSpace(parentContent))
            {
                continue;
            }

            PlanningItemLink parentLink = ParseParentLink(parentContent);
            parentEntries.Add((parentLink, parentContent, i + 1));
        }

        if (parentEntries.Count == 0)
        {
            return;
        }

        List<(PlanningItemLink Link, string ParentContent, int LineNumber)> hierarchyEntries = parentEntries
            .Where(static entry => entry.Link.Kind != PlanningItemKind.Unknown || IsLikelyPlanningId(entry.Link.ItemId))
            .ToList();

        item.Parent = hierarchyEntries.Count > 0
            ? hierarchyEntries[0].Link
            : parentEntries[0].Link;

        IEnumerable<PlanningItemLink> relatedParents = parentEntries
            .Skip(1)
            .Select(static entry => entry.Link);
        foreach (PlanningItemLink relatedParent in relatedParents)
        {
            item.RelatedParents.Add(relatedParent);
        }
    }

    private static PlanningItemLink ParseParentLink(string parentContent)
    {
        Match match = Regex.Match(parentContent, "^(?<label>[^:]+):\\s*`(?<id>[^`]+)`(?:\\s+(?<title>.+))?$", RegexOptions.CultureInvariant);
        if (!match.Success)
        {
            return new PlanningItemLink
            {
                ItemId = parentContent,
            };
        }

        string label = match.Groups["label"].Value.Trim();
        string title = match.Groups["title"].Value.Trim();

        return new PlanningItemLink
        {
            ItemId = match.Groups["id"].Value.Trim(),
            Title = string.IsNullOrWhiteSpace(title) ? null : title,
            Kind = MapParentLabel(label),
        };
    }

    private static void ParseStatus(PlanningItem item, IReadOnlyList<string> lines)
    {
        int statusSectionIndex = FindSectionIndex(lines, "## Status");
        if (statusSectionIndex < 0)
        {
            item.Diagnostics.Add(new Diagnostic
            {
                Code = "PLN020",
                Message = "The planning item is missing a status section.",
                Severity = DiagnosticSeverity.Warning,
                SourcePath = item.SourcePath,
            });

            return;
        }

        for (int i = statusSectionIndex + 1; i < lines.Count; i++)
        {
            string line = lines[i];
            if (line.StartsWith("## ", StringComparison.Ordinal))
            {
                break;
            }

            string trimmed = line.Trim();
            if (!trimmed.StartsWith("- ", StringComparison.Ordinal))
            {
                continue;
            }

            item.Status = MapStatus(trimmed[2..].Trim());
            if (item.Status == PlanningItemStatus.Unknown)
            {
                item.Diagnostics.Add(new Diagnostic
                {
                    Code = "PLN021",
                    Message = "The planning item status could not be normalized.",
                    Severity = DiagnosticSeverity.Warning,
                    SourcePath = item.SourcePath,
                    LineNumber = i + 1,
                });
            }

            return;
        }

        item.Diagnostics.Add(new Diagnostic
        {
            Code = "PLN022",
            Message = "The planning item status section does not contain a status value.",
            Severity = DiagnosticSeverity.Warning,
            SourcePath = item.SourcePath,
        });
    }

    private static void ApplyCrossItemDiagnostics(PlanningReadResult result)
    {
        Dictionary<string, List<PlanningItem>> itemsById = result.Items
            .Where(static item => !string.IsNullOrWhiteSpace(item.Id))
            .GroupBy(static item => item.Id, StringComparer.Ordinal)
            .ToDictionary(static group => group.Key, static group => group.ToList(), StringComparer.Ordinal);

        Dictionary<string, PlanningItem> itemsBySourcePath = result.Items
            .GroupBy(static item => NormalizeRelativePath(item.SourcePath), StringComparer.OrdinalIgnoreCase)
            .ToDictionary(static group => group.Key, static group => group.First(), StringComparer.OrdinalIgnoreCase);

        Dictionary<string, List<PlanningItem>> itemsByFileName = result.Items
            .GroupBy(static item => Path.GetFileName(item.SourcePath), StringComparer.OrdinalIgnoreCase)
            .ToDictionary(static group => group.Key, static group => group.ToList(), StringComparer.OrdinalIgnoreCase);

        foreach (KeyValuePair<string, List<PlanningItem>> pair in itemsById)
        {
            if (pair.Value.Count < 2)
            {
                continue;
            }

            foreach (PlanningItem item in pair.Value)
            {
                item.Diagnostics.Add(new Diagnostic
                {
                    Code = "PLN030",
                    Message = "The planning item ID is duplicated within the local planning tree.",
                    Severity = DiagnosticSeverity.Error,
                    SourcePath = item.SourcePath,
                });
            }
        }

        foreach (PlanningItem item in result.Items)
        {
            if (item.Parent is null || string.IsNullOrWhiteSpace(item.Parent.ItemId))
            {
                continue;
            }

            string parentReference = item.Parent.ItemId;
            PlanningItem? parent = ResolveParent(item, parentReference, itemsById, itemsBySourcePath, itemsByFileName);
            if (parent is null)
            {
                if (TryResolveExistingParentFilePath(item, parentReference, result.RepositoryRootPath, out string? existingParentSourcePath))
                {
                    item.Parent.SourcePath = existingParentSourcePath;
                    continue;
                }

                item.Diagnostics.Add(new Diagnostic
                {
                    Code = "PLN031",
                    Message = $"The referenced parent '{parentReference}' could not be resolved to a planning item or existing file.",
                    Severity = DiagnosticSeverity.Warning,
                    SourcePath = item.SourcePath,
                });

                continue;
            }

            item.Parent.ItemId = parent.Id;
            item.Parent.SourcePath = parent.SourcePath;
            if (item.Parent.Kind == PlanningItemKind.Unknown)
            {
                item.Parent.Kind = parent.Kind;
            }

            parent.Children.Add(new PlanningItemLink
            {
                ItemId = item.Id,
                Title = item.Title,
                Kind = item.Kind,
                SourcePath = item.SourcePath,
            });
        }
    }

    private static PlanningItem? ResolveParent(
        PlanningItem item,
        string parentReference,
        IReadOnlyDictionary<string, List<PlanningItem>> itemsById,
        IReadOnlyDictionary<string, PlanningItem> itemsBySourcePath,
        IReadOnlyDictionary<string, List<PlanningItem>> itemsByFileName)
    {
        foreach (string candidatePath in BuildParentPathCandidates(item, parentReference))
        {
            if (itemsBySourcePath.TryGetValue(candidatePath, out PlanningItem? parentByPath))
            {
                return parentByPath;
            }
        }

        string referenceFileName = Path.GetFileName(parentReference.Replace('/', Path.DirectorySeparatorChar));
        if (!string.IsNullOrWhiteSpace(referenceFileName) &&
            itemsByFileName.TryGetValue(referenceFileName, out List<PlanningItem>? parentsByFileName) &&
            parentsByFileName.Count == 1)
        {
            return parentsByFileName[0];
        }

        if (itemsById.TryGetValue(parentReference, out List<PlanningItem>? parentsById) && parentsById.Count > 0)
        {
            return parentsById[0];
        }

        if (TryExtractPlanningId(parentReference, out string? extractedPlanningId) &&
            !string.IsNullOrWhiteSpace(extractedPlanningId) &&
            itemsById.TryGetValue(extractedPlanningId, out List<PlanningItem>? parentsByExtractedId) &&
            parentsByExtractedId.Count > 0)
        {
            return parentsByExtractedId[0];
        }

        return null;
    }

    private static IReadOnlyList<string> BuildParentPathCandidates(PlanningItem item, string parentReference)
    {
        HashSet<string> candidates = new(StringComparer.OrdinalIgnoreCase);

        string normalizedReference = NormalizeRelativePath(parentReference);
        if (!string.IsNullOrWhiteSpace(normalizedReference))
        {
            candidates.Add(normalizedReference);
        }

        string itemDirectory = NormalizeRelativePath(Path.GetDirectoryName(item.SourcePath) ?? string.Empty);
        if (!string.IsNullOrWhiteSpace(itemDirectory))
        {
            candidates.Add(NormalizeRelativePath(Path.Combine(itemDirectory, parentReference)));
        }

        if (!normalizedReference.StartsWith("DevOps/", StringComparison.OrdinalIgnoreCase))
        {
            candidates.Add(NormalizeRelativePath(Path.Combine("DevOps", parentReference)));
        }

        string fileNameOnly = Path.GetFileName(parentReference.Replace('/', Path.DirectorySeparatorChar));
        if (!string.IsNullOrWhiteSpace(fileNameOnly))
        {
            candidates.Add(NormalizeRelativePath(Path.Combine("DevOps", fileNameOnly)));
        }

        candidates.RemoveWhere(static candidate => string.IsNullOrWhiteSpace(candidate));
        return candidates.ToArray();
    }

    private static bool TryResolveExistingParentFilePath(
        PlanningItem item,
        string parentReference,
        string repositoryRootPath,
        out string? existingParentSourcePath)
    {
        if (Path.IsPathRooted(parentReference) && File.Exists(parentReference))
        {
            existingParentSourcePath = NormalizeRelativePath(Path.GetRelativePath(repositoryRootPath, parentReference));
            return true;
        }

        foreach (string candidatePath in BuildParentPathCandidates(item, parentReference))
        {
            string fullPath = Path.GetFullPath(Path.Combine(repositoryRootPath, candidatePath));
            if (File.Exists(fullPath))
            {
                existingParentSourcePath = candidatePath;
                return true;
            }
        }

        existingParentSourcePath = null;
        return false;
    }

    private static bool IsLikelyPlanningId(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        return Regex.IsMatch(value.Trim(), "^[A-Za-z0-9]+-[A-Za-z]{1,3}[0-9]+[A-Za-z]?", RegexOptions.CultureInvariant);
    }

    private static bool TryExtractPlanningId(string value, out string? planningId)
    {
        Match match = Regex.Match(
            value ?? string.Empty,
            "(?<id>[A-Za-z0-9]+-[A-Za-z]{1,3}[0-9]+[A-Za-z]?)",
            RegexOptions.CultureInvariant);

        if (match.Success)
        {
            planningId = match.Groups["id"].Value;
            return true;
        }

        planningId = null;
        return false;
    }

    private static string NormalizeRelativePath(string value)
    {
        string[] segments = value
            .Replace('\\', '/')
            .Split('/', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        List<string> normalized = [];
        foreach (string segment in segments)
        {
            if (string.Equals(segment, ".", StringComparison.Ordinal))
            {
                continue;
            }

            if (string.Equals(segment, "..", StringComparison.Ordinal))
            {
                if (normalized.Count > 0)
                {
                    normalized.RemoveAt(normalized.Count - 1);
                }

                continue;
            }

            normalized.Add(segment);
        }

        return string.Join('/', normalized);
    }

    private static int FindSectionIndex(IReadOnlyList<string> lines, string heading)
    {
        for (int i = 0; i < lines.Count; i++)
        {
            if (string.Equals(lines[i].Trim(), heading, StringComparison.Ordinal))
            {
                return i;
            }
        }

        return -1;
    }

    private static PlanningItemStatus MapStatus(string? value)
    {
        string normalizedStatus = NormalizeStatusValue(value);
        return normalizedStatus switch
        {
            "PROPOSED" => PlanningItemStatus.Proposed,
            "IN PROGRESS" => PlanningItemStatus.InProgress,
            "ACTIVE" => PlanningItemStatus.InProgress,
            "BLOCKED" => PlanningItemStatus.Blocked,
            "COMPLETED" => PlanningItemStatus.Completed,
            "DONE" => PlanningItemStatus.Completed,
            "CANCELLED" => PlanningItemStatus.Cancelled,
            "CANCELED" => PlanningItemStatus.Cancelled,
            _ => PlanningItemStatus.Unknown,
        };
    }

    private static string NormalizeStatusValue(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        string normalized = value.Trim();
        if (normalized.StartsWith("[x]", StringComparison.OrdinalIgnoreCase) ||
            normalized.StartsWith("[ ]", StringComparison.OrdinalIgnoreCase))
        {
            normalized = normalized[3..].Trim();
        }

        normalized = normalized.TrimEnd('.', ':', ';').Trim();
        return normalized.ToUpperInvariant();
    }

    private static PlanningItemKind MapParentLabel(string value)
        => value.Trim().ToUpperInvariant() switch
        {
            "EPIC" => PlanningItemKind.Epic,
            "FEATURE" => PlanningItemKind.Feature,
            "BACKLOG ITEM" => PlanningItemKind.BacklogItem,
            "TASK" => PlanningItemKind.Task,
            "VISION" => PlanningItemKind.Vision,
            "BUG" => PlanningItemKind.Bug,
            "TEST" => PlanningItemKind.TestCase,
            _ => PlanningItemKind.Unknown,
        };

    public Task<PlanningWriteResult> WriteAsync(PlanningWriteRequest request, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyList<PlanningDocumentTemplate>> GetTemplatesAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}