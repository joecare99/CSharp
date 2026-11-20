using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Project.Explorer;

namespace AA98_AvlnCodeStudio.UI.Services;

/// <summary>Applies literal resource dependencies to a filesystem explorer snapshot.</summary>
public sealed class ResourceDependencyProjectExplorerSource(IProjectExplorerSource source) : IProjectExplorerSource
{
    private static readonly StringComparer s_pathComparer = OperatingSystem.IsWindows()
        ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal;

    public async Task<ProjectExplorerSnapshot> RefreshAsync(string rootPath, CancellationToken cancellationToken = default)
    {
        ProjectExplorerSnapshot snapshot = await source.RefreshAsync(rootPath, cancellationToken);
        cancellationToken.ThrowIfCancellationRequested();
        return new ProjectExplorerSnapshot(snapshot.RootPath,
            snapshot.RootItems.Select(item => Project(item, cancellationToken)).ToArray());
    }

    private static ProjectExplorerItem Project(ProjectExplorerItem item, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        if (item.Kind == ProjectExplorerItemKind.Project && File.Exists(item.Path))
        {
            return Nest(item, ReadDependencies(item.Path, token), token);
        }

        return Copy(item, item.Children.Select(child => Project(child, token)).ToArray());
    }

    private static Dictionary<string, string> ReadDependencies(string projectPath, CancellationToken token)
    {
        Dictionary<string, string> dependencies = new(s_pathComparer);
        HashSet<string> ambiguous = new(s_pathComparer);
        try
        {
            using XmlReader reader = XmlReader.Create(projectPath, new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Prohibit,
                XmlResolver = null,
                MaxCharactersInDocument = 1_000_000,
            });
            XDocument document = XDocument.Load(reader);
            string directory = Path.GetDirectoryName(projectPath)!;
            foreach (XElement element in document.Descendants().Where(e => e.Name.LocalName == "Compile"))
            {
                token.ThrowIfCancellationRequested();
                string? include = (string?)element.Attribute("Update") ?? (string?)element.Attribute("Include");
                XElement[] metadata = element.Elements().Where(e => e.Name.LocalName == "DependentUpon").ToArray();
                XAttribute? metadataAttribute = element.Attribute("DependentUpon");
                if (!IsLiteral(include) || (metadata.Length == 0 && metadataAttribute is null))
                    continue;

                string child = Path.GetFullPath(Path.Combine(directory, include!.Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar)));
                if (!child.EndsWith(".cs", StringComparison.OrdinalIgnoreCase))
                    continue;
                if (element.AncestorsAndSelf().Any(e => e.Attribute("Condition") is not null ||
                    e.Name.LocalName is "Choose" or "Target") ||
                    (element.Attribute("Update") is not null && element.Attribute("Include") is not null) ||
                    (metadata.Length + (metadataAttribute is null ? 0 : 1)) != 1 ||
                    (metadata.Length == 1 && (metadata[0].Attribute("Condition") is not null || metadata[0].HasElements)))
                {
                    ambiguous.Add(child);
                    continue;
                }

                string parentName = (metadataAttribute?.Value ?? metadata[0].Value).Trim();
                if (!IsLiteral(parentName) ||
                    !parentName.EndsWith(".resx", StringComparison.OrdinalIgnoreCase) ||
                    parentName.IndexOfAny(['/', '\\']) >= 0)
                {
                    ambiguous.Add(child);
                    continue;
                }
                string parent = Path.Combine(Path.GetDirectoryName(child)!, parentName);
                if (!dependencies.TryAdd(child, parent))
                    ambiguous.Add(child);
            }
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or XmlException or ArgumentException or NotSupportedException)
        {
            dependencies.Clear();
        }

        foreach (string child in ambiguous)
            dependencies.Remove(child);
        return dependencies;
    }

    private static bool IsLiteral(string? value) =>
        !string.IsNullOrWhiteSpace(value) && value.IndexOfAny(['$', '@', '%', '*', '?', ';']) < 0;

    private static ProjectExplorerItem Nest(ProjectExplorerItem item, Dictionary<string, string> dependencies, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        ProjectExplorerItem[] children = item.Children.Select(child => Nest(child, dependencies, token)).ToArray();
        Dictionary<string, ProjectExplorerItem> files = new(s_pathComparer);
        foreach (ProjectExplorerItem child in children.Where(c => c.Kind == ProjectExplorerItemKind.File))
            files.TryAdd(child.Path, child);

        Dictionary<string, List<ProjectExplorerItem>> nested = new(s_pathComparer);
        HashSet<string> moved = new(StringComparer.Ordinal);
        foreach (ProjectExplorerItem child in children)
        {
            if (child.Kind != ProjectExplorerItemKind.File ||
                !dependencies.TryGetValue(child.Path, out string? parentPath) ||
                !files.TryGetValue(parentPath, out ProjectExplorerItem? parent) ||
                !child.IsAvailable || !parent.IsAvailable)
                continue;
            if (!nested.TryGetValue(parentPath, out List<ProjectExplorerItem>? list))
                nested[parentPath] = list = [];
            list.Add(child);
            moved.Add(child.Id);
        }

        return Copy(item, children.Where(child => !moved.Contains(child.Id)).Select(child =>
            nested.TryGetValue(child.Path, out List<ProjectExplorerItem>? additions)
                ? Copy(child, child.Children.Concat(additions).ToArray())
                : child).ToArray());
    }

    private static ProjectExplorerItem Copy(ProjectExplorerItem item, IReadOnlyList<ProjectExplorerItem> children) =>
        new(item.Id, item.Name, item.Path, item.Kind, item.IsAvailable, children);
}
