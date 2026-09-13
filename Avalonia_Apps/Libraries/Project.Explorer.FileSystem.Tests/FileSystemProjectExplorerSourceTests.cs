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
        _rootPath = Path.Combine(Path.GetTempPath(), $"ProjectExplorer-{Guid.NewGuid():N}");
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
}
