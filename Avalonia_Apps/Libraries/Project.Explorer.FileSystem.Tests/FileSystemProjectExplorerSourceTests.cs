using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Project.Explorer;
using Project.Explorer.FileSystem;

namespace Project.Explorer.FileSystem.Tests;

/// <summary>Tests isolated filesystem hierarchy projection and refresh behavior.</summary>
[TestClass]
public sealed class FileSystemProjectExplorerSourceTests
{
    private string? _rootPath;

    [TestInitialize]
    public void Initialize()
    {
        _rootPath = Path.Combine(Directory.GetCurrentDirectory(), $"ProjectExplorer-{Guid.NewGuid():N}");
        Directory.CreateDirectory(_rootPath);
    }

    [TestCleanup]
    public void Cleanup()
    {
        if (_rootPath is not null && Directory.Exists(_rootPath))
        {
            Directory.Delete(_rootPath, recursive: true);
        }
    }

    [TestMethod]
    public async Task RefreshAsync_OrdersFoldersBeforeSupportedFilesAndUpdatesChanges()
    {
        Directory.CreateDirectory(Path.Combine(_rootPath!, "zeta"));
        Directory.CreateDirectory(Path.Combine(_rootPath!, "Alpha"));
        await File.WriteAllTextAsync(Path.Combine(_rootPath!, "z.cs"), string.Empty);
        await File.WriteAllTextAsync(Path.Combine(_rootPath!, "a.md"), string.Empty);
        await File.WriteAllTextAsync(Path.Combine(_rootPath!, "skip.txt"), string.Empty);
        FileSystemProjectExplorerSource source = new([".cs", ".md"]);

        ProjectExplorerSnapshot first = await source.RefreshAsync(_rootPath!);
        await File.WriteAllTextAsync(Path.Combine(_rootPath!, "new.cs"), string.Empty);
        File.Delete(Path.Combine(_rootPath!, "a.md"));
        ProjectExplorerSnapshot refreshed = await source.RefreshAsync(_rootPath!);

        CollectionAssert.AreEqual(
            new[] { "Alpha", "zeta", "a.md", "z.cs" },
            first.RootItems[0].Children.Select(static item => item.Name).ToArray());
        CollectionAssert.AreEqual(
            new[] { "Alpha", "zeta", "new.cs", "z.cs" },
            refreshed.RootItems[0].Children.Select(static item => item.Name).ToArray());
    }

    [TestMethod]
    public async Task RefreshAsync_RepresentsMissingRootAsUnavailableProject()
    {
        Directory.Delete(_rootPath!, recursive: true);
        FileSystemProjectExplorerSource source = new();

        ProjectExplorerSnapshot snapshot = await source.RefreshAsync(_rootPath!);

        Assert.AreEqual(ProjectExplorerItemKind.Project, snapshot.RootItems[0].Kind);
        Assert.IsFalse(snapshot.RootItems[0].IsAvailable);
        Assert.AreEqual(0, snapshot.RootItems[0].Children.Count);
    }

    [TestMethod]
    public async Task RefreshAsync_ProjectsSolutionFoldersAndOnlyExplicitSolutionFiles()
    {
        string root = _rootPath!;
        string solutionDirectory = Path.Combine(root, "solution");
        string projectDirectory = Path.Combine(root, "external", "app");
        Directory.CreateDirectory(solutionDirectory);
        Directory.CreateDirectory(projectDirectory);
        Directory.CreateDirectory(Path.Combine(projectDirectory, "Views"));
        Directory.CreateDirectory(Path.Combine(projectDirectory, "obj"));
        await File.WriteAllTextAsync(Path.Combine(root, "shared.props"), "");
        await File.WriteAllTextAsync(Path.Combine(root, "unrelated.txt"), "");
        await File.WriteAllTextAsync(Path.Combine(solutionDirectory, "Application.props"), "");
        await File.WriteAllTextAsync(Path.Combine(projectDirectory, "App.csproj"),
            "<Project><ItemGroup><PackageReference Include=\"Example\" /></ItemGroup></Project>");
        await File.WriteAllTextAsync(Path.Combine(projectDirectory, "Views", "Main.axaml"), "");
        await File.WriteAllTextAsync(Path.Combine(projectDirectory, "Views", "Main.axaml.cs"), "");
        await File.WriteAllTextAsync(Path.Combine(projectDirectory, "obj", "generated.cs"), "");
        string solution = Path.Combine(solutionDirectory, "App.slnx");
        await File.WriteAllTextAsync(solution,
            "<Solution><Folder Name=\"/Items/\"><File Path=\"Application.props\" /></Folder>" +
            "<Folder Name=\"/UpDir/\"><File Path=\"../shared.props\" /><File Path=\"../missing.props\" /></Folder>" +
            "<Folder Name=\"/Group/Nested/\"><Project Path=\"../external/app/App.csproj\" /></Folder></Solution>");

        FileSystemProjectExplorerSource source = new();
        ProjectExplorerItem tree = (await source.RefreshAsync(solution)).RootItems.Single();
        Assert.AreEqual("App.slnx", tree.Name);
        ProjectExplorerItem upDir = tree.Children.Single(i => i.Name == "UpDir");
        Assert.AreEqual(Path.Combine(root, "shared.props"), upDir.Children.Single(i => i.Name == "shared.props").Path);
        Assert.IsFalse(upDir.Children.Single(i => i.Name == "missing.props").IsAvailable);
        Assert.IsFalse(tree.Children.Any(i => i.Name == "unrelated.txt"));
        ProjectExplorerItem project = tree.Children.Single(i => i.Name == "Group").Children.Single().Children.Single();
        Assert.AreEqual(ProjectExplorerItemKind.Project, project.Kind);
        Assert.IsTrue(project.Children.Any(i => i.Name == "References" && i.Children.Single().Name == "Example"));
        Assert.IsFalse(project.Children.Any(i => i.Name == "obj"));
        ProjectExplorerItem axaml = project.Children.Single(i => i.Name == "Views").Children.Single();
        Assert.AreEqual("Main.axaml", axaml.Name);
        Assert.AreEqual("Main.axaml.cs", axaml.Children.Single().Name);

        ProjectExplorerItem refreshed = (await source.RefreshAsync(solution)).RootItems.Single();
        Assert.AreEqual(project.Id, refreshed.Children.Single(i => i.Name == "Group").Children.Single().Children.Single().Id);
    }

    [TestMethod]
    public async Task RefreshAsync_SlnSolutionFoldersAndNestedProjects()
    {
        string root = _rootPath!;
        await File.WriteAllTextAsync(Path.Combine(root, "App.csproj"), "<Project />");
        await File.WriteAllTextAsync(Path.Combine(root, "notes.txt"), "");
        string solution = Path.Combine(root, "App.sln");
        await File.WriteAllTextAsync(solution,
            "Project(\"{66A26720-8FB5-11D2-AA7E-00C04F688DDE}\") = \"Items\", \"Items\", \"{FOLDER}\"\n" +
            "\tProjectSection(SolutionItems) = preProject\n\t\tnotes.txt = notes.txt\n\tEndProjectSection\nEndProject\n" +
            "Project(\"{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}\") = \"App\", \"App.csproj\", \"{PROJECT}\"\nEndProject\n" +
            "Global\n\tGlobalSection(NestedProjects) = preSolution\n\t\t{PROJECT} = {FOLDER}\n\tEndGlobalSection\nEndGlobal\n");

        ProjectExplorerItem folder = (await new FileSystemProjectExplorerSource().RefreshAsync(solution)).RootItems.Single().Children.Single();
        Assert.AreEqual("Items", folder.Name);
        Assert.IsTrue(folder.Children.Any(i => i.Kind == ProjectExplorerItemKind.Project && i.Name == "App"));
        Assert.AreEqual(Path.Combine(root, "notes.txt"), folder.Children.Single(i => i.Kind == ProjectExplorerItemKind.File).Path);
    }
}
