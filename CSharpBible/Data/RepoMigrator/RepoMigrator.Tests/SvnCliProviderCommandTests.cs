using RepoMigrator.Core;
using RepoMigrator.Providers.SvnCli;
using System.Reflection;

namespace RepoMigrator.Tests;

[TestClass]
[DoNotParallelize]
public sealed class SvnCliProviderCommandTests
{
    [TestMethod]
    public async Task GetSelectionDataAsync_LoadsAndSortsRevisionInfosFromXmlLog()
    {
        var endpoint = new RepositoryEndpoint
        {
            Type = RepoType.Svn,
            UrlOrPath = "https://example.org/svn/project"
        };

        await WithSvnCommandRunnerAsync((ep, arguments, _workingDir, _ct) =>
        {
            Assert.IsNotNull(ep);

            if (arguments.StartsWith("log -v -r 0:HEAD --xml", StringComparison.Ordinal))
            {
                const string logXml = "<log>" +
                                      "<logentry revision=\"5\"><author>alice</author><date>2024-01-05T12:00:00Z</date><msg>r5</msg><paths><path action=\"M\" kind=\"file\">/trunk/readme.txt</path></paths></logentry>" +
                                      "<logentry revision=\"3\"><author>bob</author><date>2024-01-03T12:00:00Z</date><msg>r3</msg><paths><path action=\"A\" kind=\"file\">/trunk/new.txt</path><path action=\"D\" kind=\"file\">/trunk/old.txt</path></paths></logentry>" +
                                      "</log>";
                return Task.FromResult(logXml);
            }

            return Task.FromResult(string.Empty);
        }, async () =>
        {
            await using var provider = new SvnCliProvider();

            var selectionData = await provider.GetSelectionDataAsync(endpoint, CancellationToken.None);

            Assert.AreEqual(2, selectionData.Revisions.Count);
            Assert.AreEqual("3", selectionData.Revisions[0].Id);
            Assert.AreEqual("5", selectionData.Revisions[1].Id);
            Assert.AreEqual(2, selectionData.Revisions[0].ChangedPaths.Count);
            Assert.AreEqual("A", selectionData.Revisions[0].ChangedPaths[0].Action);
            Assert.AreEqual("/trunk/new.txt", selectionData.Revisions[0].ChangedPaths[0].Path);
            Assert.AreEqual(1, selectionData.Revisions[1].ChangedPaths.Count);
            Assert.AreEqual("3", selectionData.SuggestedFromRevisionId);
        });
    }

    [TestMethod]
    public async Task InitializeTargetAsync_ForRemote_ExecutesCheckoutWhenWorkingCopyMissing()
    {
        var calls = new List<string>();
        var endpoint = new RepositoryEndpoint
        {
            Type = RepoType.Svn,
            UrlOrPath = "https://example.org/svn/project",
            BranchOrTrunk = "trunk"
        };

        await WithSvnCommandRunnerAsync((_ep, arguments, _workingDir, _ct) =>
        {
            calls.Add(arguments);

            if (arguments.StartsWith("info --show-item wc-root", StringComparison.Ordinal))
                throw new InvalidOperationException("not a working copy yet");

            return Task.FromResult(string.Empty);
        }, async () =>
        {
            await using var provider = new SvnCliProvider();

            await provider.InitializeTargetAsync(endpoint, emptyInit: true, CancellationToken.None);
        });

        Assert.IsTrue(calls.Any(call => call.StartsWith("info --show-item wc-root", StringComparison.Ordinal)));
        Assert.IsTrue(calls.Any(call => call.StartsWith("checkout \"https://example.org/svn/project/trunk\"", StringComparison.Ordinal)));
    }

    [TestMethod]
    public async Task MaterializeSnapshotAsync_UsesExportCommandWithSelectedRevision()
    {
        var calls = new List<string>();
        var endpoint = new RepositoryEndpoint
        {
            Type = RepoType.Svn,
            UrlOrPath = "https://example.org/svn/project",
            BranchOrTrunk = "branches/release"
        };

        await WithSvnCommandRunnerAsync((_ep, arguments, _workingDir, _ct) =>
        {
            calls.Add(arguments);
            return Task.FromResult("ok");
        }, async () =>
        {
            var sWorkDir = Path.Combine(Path.GetTempPath(), "RepoMigrator.Tests", Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(sWorkDir);
            try
            {
                await using var provider = new SvnCliProvider();
                await provider.OpenAsync(endpoint, CancellationToken.None);

                await provider.MaterializeSnapshotAsync(sWorkDir, "42", CancellationToken.None);
            }
            finally
            {
                if (Directory.Exists(sWorkDir))
                    Directory.Delete(sWorkDir, recursive: true);
            }
        });

        Assert.IsTrue(calls.Any(call => call.StartsWith("export -r 42 \"https://example.org/svn/project/branches/release\"", StringComparison.Ordinal)));
    }

    [TestMethod]
    public async Task CommitSnapshotAsync_WhenMissingFilesDetected_ExecutesDeleteCommitRevpropsAndUpdate()
    {
        var calls = new List<string>();
        var endpoint = new RepositoryEndpoint
        {
            Type = RepoType.Svn,
            UrlOrPath = "https://example.org/svn/project",
            BranchOrTrunk = "trunk"
        };

        await WithSvnCommandRunnerAsync((_ep, arguments, _workingDir, _ct) =>
        {
            calls.Add(arguments);

            if (arguments.StartsWith("info --show-item wc-root", StringComparison.Ordinal))
                return Task.FromResult("C:/wc");

            if (arguments.StartsWith("status --xml", StringComparison.Ordinal))
                return Task.FromResult("<status><target path=\".\"><entry path=\"removed.txt\"><wc-status item=\"missing\" /></entry></target></status>");

            if (arguments.StartsWith("commit ", StringComparison.Ordinal))
                return Task.FromResult("Committed revision 123.\n");

            if (arguments.StartsWith("info --show-item url", StringComparison.Ordinal))
                return Task.FromResult("https://example.org/svn/project/trunk\n");

            return Task.FromResult(string.Empty);
        }, async () =>
        {
            var sRootPath = Path.Combine(Path.GetTempPath(), "RepoMigrator.Tests", Guid.NewGuid().ToString("N"));
            var sSnapshotPath = Path.Combine(sRootPath, "snapshot");
            var sWorkingCopyPath = Path.Combine(sRootPath, "wc");
            Directory.CreateDirectory(sSnapshotPath);
            Directory.CreateDirectory(sWorkingCopyPath);
            File.WriteAllText(Path.Combine(sSnapshotPath, "README.md"), "content");

            try
            {
                await using var provider = new SvnCliProvider();
                await provider.InitializeTargetAsync(new RepositoryEndpoint
                {
                    Type = RepoType.Svn,
                    UrlOrPath = sWorkingCopyPath
                }, emptyInit: true, CancellationToken.None);

                await provider.OpenAsync(endpoint, CancellationToken.None);
                await provider.CommitSnapshotAsync(sSnapshotPath, new CommitMetadata
                {
                    Message = "import",
                    AuthorName = "alice",
                    Timestamp = new DateTimeOffset(2024, 1, 1, 8, 0, 0, TimeSpan.Zero)
                }, CancellationToken.None);
            }
            finally
            {
                if (Directory.Exists(sRootPath))
                    Directory.Delete(sRootPath, recursive: true);
            }
        });

        Assert.IsTrue(calls.Any(call => call.StartsWith("add --force --parents .", StringComparison.Ordinal)));
        Assert.IsTrue(calls.Any(call => call.StartsWith("status", StringComparison.Ordinal)));
        Assert.IsTrue(calls.Any(call => call.StartsWith("delete --force --targets", StringComparison.Ordinal)));
        Assert.IsTrue(calls.Any(call => call.StartsWith("commit ", StringComparison.Ordinal)));
        Assert.IsTrue(calls.Any(call => call.StartsWith("propset --revprop -r 123 svn:author", StringComparison.Ordinal)));
        Assert.IsTrue(calls.Any(call => call.StartsWith("propset --revprop -r 123 svn:date", StringComparison.Ordinal)));
        Assert.IsTrue(calls.Any(call => call.StartsWith("update", StringComparison.Ordinal)));
    }

    private static async Task WithSvnCommandRunnerAsync(
        Func<RepositoryEndpoint?, string, string?, CancellationToken, Task<string>> runner,
        Func<Task> testAction)
    {
        var field = typeof(SvnCliProvider).GetField("s_runSvnCommandAsync", BindingFlags.NonPublic | BindingFlags.Static);
        Assert.IsNotNull(field);

        var previousRunner = (Func<RepositoryEndpoint?, string, string?, CancellationToken, Task<string>>)field.GetValue(null)!;
        field.SetValue(null, runner);
        try
        {
            await testAction();
        }
        finally
        {
            field.SetValue(null, previousRunner);
        }
    }
}
