using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml.Linq;
using Project.Explorer;

namespace Project.Explorer.FileSystem;

internal static class SolutionProjectExplorerBuilder
{
    private static readonly string[] s_ignoredDirectories = ["bin", "obj", ".git", ".vs"];
    private static readonly Regex s_slnProject = new(
        "^Project\\(\"(?<type>[^\"]+)\"\\)\\s*=\\s*\"(?<name>[^\"]+)\",\\s*\"(?<path>[^\"]+)\",\\s*\"(?<id>[^\"]+)\"",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
    private const string SolutionFolderType = "{66A26720-8FB5-11D2-AA7E-00C04F688DDE}";

    public static bool Supports(string path) =>
        new[] { ".slnx", ".sln", ".csproj" }.Contains(Path.GetExtension(path), StringComparer.OrdinalIgnoreCase);

    public static ProjectExplorerItem Build(string path, ISet<string>? extensions, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        if (Path.GetExtension(path).Equals(".csproj", StringComparison.OrdinalIgnoreCase))
        {
            return BuildProject(path, $"Project:{path}", extensions, token);
        }

        Node root = new($"Solution:{path}", Path.GetFileName(path), path, ProjectExplorerItemKind.Folder);
        if (!File.Exists(path))
        {
            root.Available = false;
        }
        else if (Path.GetExtension(path).Equals(".slnx", StringComparison.OrdinalIgnoreCase))
        {
            ReadXmlSolution(root, path, extensions, token);
        }
        else
        {
            ReadSln(root, path, extensions, token);
        }

        return root.ToItem();
    }

    private static void ReadXmlSolution(Node root, string path, ISet<string>? extensions, CancellationToken token)
    {
        XDocument document = XDocument.Load(path);
        string directory = Path.GetDirectoryName(path)!;
        foreach (XElement element in document.Descendants())
        {
            token.ThrowIfCancellationRequested();
            if (element.Name.LocalName == "Folder")
            {
                string? name = (string?)element.Attribute("Name");
                if (!string.IsNullOrWhiteSpace(name))
                {
                    FindFolder(root, name!);
                }
            }
            else if (element.Name.LocalName == "Project" || element.Name.LocalName == "File")
            {
                string? relativePath = (string?)element.Attribute("Path");
                if (string.IsNullOrWhiteSpace(relativePath))
                {
                    continue;
                }

                XElement? folderElement = element.Ancestors().FirstOrDefault(e => e.Name.LocalName == "Folder");
                string? folderName = (string?)folderElement?.Attribute("Name");
                Node parent = string.IsNullOrWhiteSpace(folderName) ? root : FindFolder(root, folderName!);
                string itemPath = Resolve(directory, relativePath!);
                if (element.Name.LocalName == "Project")
                {
                    parent.Items.Add(BuildProject(itemPath, $"{parent.Id}/Project:{itemPath}", extensions, token));
                }
                else
                {
                    parent.Items.Add(FileItem(itemPath, $"{parent.Id}/File:{itemPath}"));
                }
            }
        }
    }

    private static Node FindFolder(Node root, string name)
    {
        Node current = root;
        foreach (string part in name.Split(['/', '\\'], StringSplitOptions.RemoveEmptyEntries))
        {
            Node? folder = current.Folders.FirstOrDefault(f => string.Equals(f.Name, part, StringComparison.OrdinalIgnoreCase));
            if (folder is null)
            {
                folder = new Node($"{current.Id}/Folder:{part}", part, root.Path, ProjectExplorerItemKind.Folder);
                current.Folders.Add(folder);
            }
            current = folder;
        }
        return current;
    }

    private static void ReadSln(Node root, string path, ISet<string>? extensions, CancellationToken token)
    {
        string directory = Path.GetDirectoryName(path)!;
        Dictionary<string, Node> folders = new(StringComparer.OrdinalIgnoreCase);
        Dictionary<string, ProjectExplorerItem> projects = new(StringComparer.OrdinalIgnoreCase);
        Dictionary<string, string> parents = new(StringComparer.OrdinalIgnoreCase);
        string? activeFolder = null;
        bool solutionItems = false;
        bool nested = false;
        foreach (string line in File.ReadLines(path))
        {
            token.ThrowIfCancellationRequested();
            Match match = s_slnProject.Match(line);
            if (match.Success)
            {
                string id = match.Groups["id"].Value;
                if (match.Groups["type"].Value.Equals(SolutionFolderType, StringComparison.OrdinalIgnoreCase))
                {
                    folders[id] = new Node($"{root.Id}/FolderId:{id}", match.Groups["name"].Value, path, ProjectExplorerItemKind.Folder);
                    activeFolder = id;
                }
                else if (match.Groups["path"].Value.EndsWith(".csproj", StringComparison.OrdinalIgnoreCase))
                {
                    string projectPath = Resolve(directory, match.Groups["path"].Value);
                    projects[id] = BuildProject(projectPath, $"{root.Id}/ProjectId:{id}", extensions, token);
                    activeFolder = null;
                }
                continue;
            }

            string trimmed = line.Trim();
            if (trimmed.StartsWith("ProjectSection(SolutionItems)", StringComparison.OrdinalIgnoreCase)) solutionItems = true;
            else if (trimmed.StartsWith("EndProjectSection", StringComparison.OrdinalIgnoreCase)) solutionItems = false;
            else if (trimmed.StartsWith("GlobalSection(NestedProjects)", StringComparison.OrdinalIgnoreCase)) nested = true;
            else if (trimmed.StartsWith("EndGlobalSection", StringComparison.OrdinalIgnoreCase)) nested = false;
            else if (trimmed.StartsWith("EndProject", StringComparison.OrdinalIgnoreCase)) activeFolder = null;
            else if (trimmed.Contains('='))
            {
                string[] pair = trimmed.Split('=', 2, StringSplitOptions.TrimEntries);
                if (nested) parents[pair[0]] = pair[1];
                else if (solutionItems && activeFolder is not null)
                {
                    string itemPath = Resolve(directory, pair[0]);
                    Node folder = folders[activeFolder];
                    folder.Items.Add(FileItem(itemPath, $"{folder.Id}/File:{itemPath}"));
                }
            }
        }
        foreach ((string id, Node folder) in folders)
        {
            // Invalid or cyclic parent links must not make nodes disappear.
            Node parent = root;
            if (parents.TryGetValue(id, out string? parentId) && folders.TryGetValue(parentId, out Node? candidate))
            {
                HashSet<string> visited = new(StringComparer.OrdinalIgnoreCase) { id };
                string? cursor = parentId;
                while (cursor is not null && visited.Add(cursor))
                    cursor = parents.TryGetValue(cursor, out string? next) ? next : null;
                if (cursor is null) parent = candidate;
            }
            parent.Folders.Add(folder);
        }
        foreach ((string id, ProjectExplorerItem project) in projects)
        {
            Node parent = parents.TryGetValue(id, out string? parentId) && folders.TryGetValue(parentId, out Node? folder)
                ? folder : root;
            parent.Items.Add(project);
        }
    }

    private static ProjectExplorerItem BuildProject(string path, string id, ISet<string>? extensions, CancellationToken token)
    {
        if (!File.Exists(path))
            return new ProjectExplorerItem(id, Path.GetFileNameWithoutExtension(path), path, ProjectExplorerItemKind.Project, false);

        string directory = Path.GetDirectoryName(path)!;
        List<ProjectExplorerItem> children = ScanDirectory(directory, id, extensions, token);
        XDocument project = XDocument.Load(path);
        List<ProjectExplorerItem> references = [];
        foreach (XElement element in project.Descendants().Where(e =>
                     e.Name.LocalName is "PackageReference" or "ProjectReference" or "Reference"))
        {
            token.ThrowIfCancellationRequested();
            string? include = (string?)element.Attribute("Include");
            if (string.IsNullOrWhiteSpace(include)) continue;
            string name = include!.Replace('\\', '/').Split('/').Last();
            references.Add(new ProjectExplorerItem($"{id}/Reference:{element.Name.LocalName}:{include}", name, path,
                ProjectExplorerItemKind.Folder));
        }
        if (references.Count > 0)
        {
            children.Insert(0, new ProjectExplorerItem($"{id}/References", "References", path,
                ProjectExplorerItemKind.Folder, children: references.OrderBy(i => i.Name, StringComparer.OrdinalIgnoreCase).ToArray()));
        }
        return new ProjectExplorerItem(id, Path.GetFileNameWithoutExtension(path), path, ProjectExplorerItemKind.Project, children: children);
    }

    private static List<ProjectExplorerItem> ScanDirectory(string directory, string parentId, ISet<string>? extensions, CancellationToken token)
    {
        List<ProjectExplorerItem> result = [];
        try
        {
            foreach (string subdirectory in Directory.EnumerateDirectories(directory).OrderBy(Path.GetFileName, StringComparer.OrdinalIgnoreCase))
            {
                token.ThrowIfCancellationRequested();
                if (s_ignoredDirectories.Contains(Path.GetFileName(subdirectory), StringComparer.OrdinalIgnoreCase)) continue;
                string id = $"{parentId}/Folder:{subdirectory}";
                bool reparse = (File.GetAttributes(subdirectory) & FileAttributes.ReparsePoint) != 0;
                result.Add(new ProjectExplorerItem(id, Path.GetFileName(subdirectory), subdirectory, ProjectExplorerItemKind.Folder,
                    children: reparse ? [] : ScanDirectory(subdirectory, id, extensions, token)));
            }
            string[] files = Directory.EnumerateFiles(directory)
                .Where(f => extensions is null || extensions.Contains(Path.GetExtension(f)))
                .OrderBy(Path.GetFileName, StringComparer.OrdinalIgnoreCase).ToArray();
            HashSet<string> nested = new(StringComparer.OrdinalIgnoreCase);
            foreach (string file in files)
            {
                if (nested.Contains(file)) continue;
                string id = $"{parentId}/File:{file}";
                string codeBehind = file + ".cs";
                ProjectExplorerItem[] children = files.Contains(codeBehind, StringComparer.OrdinalIgnoreCase)
                    ? [FileItem(codeBehind, $"{id}/File:{codeBehind}")] : [];
                if (children.Length > 0) nested.Add(codeBehind);
                result.Add(new ProjectExplorerItem(id, Path.GetFileName(file), file, ProjectExplorerItemKind.File, children: children));
            }
        }
        catch (UnauthorizedAccessException) { }
        catch (IOException) { }
        return result;
    }

    private static ProjectExplorerItem FileItem(string path, string id) =>
        new(id, Path.GetFileName(path), path, ProjectExplorerItemKind.File, File.Exists(path));

    private static string Resolve(string directory, string path) =>
        Path.GetFullPath(Path.Combine(directory, path.Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar)));

    private sealed class Node(string id, string name, string path, ProjectExplorerItemKind kind)
    {
        public string Id { get; } = id;
        public string Name { get; } = name;
        public string Path { get; } = path;
        public ProjectExplorerItemKind Kind { get; } = kind;
        public bool Available { get; set; } = true;
        public List<Node> Folders { get; } = [];
        public List<ProjectExplorerItem> Items { get; } = [];
        public ProjectExplorerItem ToItem() => new(Id, Name, Path, Kind, Available,
            Folders.OrderBy(f => f.Name, StringComparer.OrdinalIgnoreCase).Select(f => f.ToItem())
                .Concat(Items.OrderBy(i => i.Name, StringComparer.OrdinalIgnoreCase)).ToArray());
    }
}
