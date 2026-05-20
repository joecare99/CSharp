using LibGit2Sharp;
using RepoMigrator.Tools.GitBranchSplitter;

namespace RepoMigrator.Tests;

[TestClass]
public sealed class GitBranchSplitServiceTests
{
    [TestMethod]
    public async Task SplitAsync_ThrowsForInvalidRepositoryPath()
    {
        var service = new GitBranchSplitService();
        var options = new GitBranchSplitOptions
        {
            RepositoryPath = Path.Combine(Path.GetTempPath(), "RepoMigrator.Tests", Guid.NewGuid().ToString("N"), "missing-repo"),
            SourceBranch = "main"
        };

        try
        {
            await service.SplitAsync(options, CancellationToken.None);
            Assert.Fail("Expected DirectoryNotFoundException.");
        }
        catch (DirectoryNotFoundException ex)
        {
            StringAssert.Contains(ex.Message, "is not a valid Git repository");
        }
    }

    [TestMethod]
    public async Task SplitAsync_ThrowsWhenRepositoryIsDirty()
    {
        var sRepoPath = CreateRepository();

        try
        {
            File.WriteAllText(Path.Combine(sRepoPath, "dirty.txt"), "dirty");

            var service = new GitBranchSplitService();
            var options = new GitBranchSplitOptions
            {
                RepositoryPath = sRepoPath,
                SourceBranch = "main"
            };

            try
            {
                await service.SplitAsync(options, CancellationToken.None);
                Assert.Fail("Expected InvalidOperationException.");
            }
            catch (InvalidOperationException ex)
            {
                StringAssert.Contains(ex.Message, "contains local changes");
            }
        }
        finally
        {
            DeleteDirectoryIfExists(sRepoPath);
        }
    }

    [TestMethod]
    public async Task SplitAsync_ThrowsWhenSourceBranchIsMissing()
    {
        var sRepoPath = CreateRepository();

        try
        {
            var service = new GitBranchSplitService();
            var options = new GitBranchSplitOptions
            {
                RepositoryPath = sRepoPath,
                SourceBranch = "missing-branch"
            };

            try
            {
                await service.SplitAsync(options, CancellationToken.None);
                Assert.Fail("Expected InvalidOperationException.");
            }
            catch (InvalidOperationException ex)
            {
                StringAssert.Contains(ex.Message, "was not found");
            }
        }
        finally
        {
            DeleteDirectoryIfExists(sRepoPath);
        }
    }

    [TestMethod]
    public async Task SplitAsync_CreatesExpectedSplitBranches_WithFilteredContent()
    {
        var sRepoPath = CreateRepositoryWithGroupedPaths();

        try
        {
            var service = new GitBranchSplitService();
            var options = new GitBranchSplitOptions
            {
                RepositoryPath = sRepoPath,
                SourceBranch = "main",
                BranchPrefix = "split",
                OverwriteExistingBranches = true
            };

            await service.SplitAsync(options, CancellationToken.None);

            using var repository = new Repository(sRepoPath);

            Assert.IsNotNull(repository.Branches["split/_root"]);
            Assert.IsNotNull(repository.Branches["split/AreaA"]);
            Assert.IsNotNull(repository.Branches["split/AreaB"]);
            Assert.IsNotNull(repository.Branches["split/AreaC"]);

            AssertBranchContains(repository, "split/_root", new[] { "README.md" });
            AssertBranchContains(repository, "split/AreaA", new[] { "AreaA/summary.txt" });
            AssertBranchContains(repository, "split/AreaB", new[] { "AreaB/info.txt" });
            AssertBranchContains(repository, "split/AreaC", new[] { "AreaC/data.txt" });
        }
        finally
        {
            DeleteDirectoryIfExists(sRepoPath);
        }
    }

    [TestMethod]
    public async Task SplitAsync_ThrowsWhenSplitBranchExists_AndOverwriteDisabled()
    {
        var sRepoPath = CreateRepositoryWithGroupedPaths();

        try
        {
            var service = new GitBranchSplitService();

            await service.SplitAsync(new GitBranchSplitOptions
            {
                RepositoryPath = sRepoPath,
                SourceBranch = "main",
                BranchPrefix = "split",
                OverwriteExistingBranches = true
            }, CancellationToken.None);

            try
            {
                await service.SplitAsync(new GitBranchSplitOptions
                {
                    RepositoryPath = sRepoPath,
                    SourceBranch = "main",
                    BranchPrefix = "split",
                    OverwriteExistingBranches = false
                }, CancellationToken.None);
                Assert.Fail("Expected InvalidOperationException.");
            }
            catch (InvalidOperationException ex)
            {
                StringAssert.Contains(ex.Message, "already exists");
            }
        }
        finally
        {
            DeleteDirectoryIfExists(sRepoPath);
        }
    }

    [TestMethod]
    public async Task SplitAsync_ReplacesExistingSplitBranch_WhenOverwriteEnabled()
    {
        var sRepoPath = CreateRepositoryWithGroupedPaths();

        try
        {
            var service = new GitBranchSplitService();

            await service.SplitAsync(new GitBranchSplitOptions
            {
                RepositoryPath = sRepoPath,
                SourceBranch = "main",
                BranchPrefix = "split",
                OverwriteExistingBranches = true
            }, CancellationToken.None);

            using (var repository = new Repository(sRepoPath))
            {
                Commands.Checkout(repository, repository.Branches["split/_root"]);
                File.AppendAllText(Path.Combine(sRepoPath, "README.md"), "\nchanged");
                Commands.Stage(repository, "README.md");
                var signature = new Signature("tester", "tester@example.local", DateTimeOffset.UtcNow);
                repository.Commit("Mutate split branch", signature, signature);
            }

            await service.SplitAsync(new GitBranchSplitOptions
            {
                RepositoryPath = sRepoPath,
                SourceBranch = "main",
                BranchPrefix = "split",
                OverwriteExistingBranches = true
            }, CancellationToken.None);

            using var reloadedRepository = new Repository(sRepoPath);
            var rootBranch = reloadedRepository.Branches["split/_root"];
            Assert.IsNotNull(rootBranch);
            Commands.Checkout(reloadedRepository, rootBranch);

            var readmeContent = File.ReadAllText(Path.Combine(sRepoPath, "README.md"));
            Assert.AreEqual("root", readmeContent);
            Assert.IsFalse(readmeContent.Contains("changed", StringComparison.Ordinal));
        }
        finally
        {
            DeleteDirectoryIfExists(sRepoPath);
        }
    }

    [TestMethod]
    public async Task SplitAsync_TreatsUnbornOrphanBranchAsMissingBranch()
    {
        var sRepoPath = CreateRepositoryWithOrphanBranchWithoutCommit();

        try
        {
            var service = new GitBranchSplitService();
            var options = new GitBranchSplitOptions
            {
                RepositoryPath = sRepoPath,
                SourceBranch = "orphan"
            };

            try
            {
                await service.SplitAsync(options, CancellationToken.None);
                Assert.Fail("Expected InvalidOperationException.");
            }
            catch (InvalidOperationException ex)
            {
                StringAssert.Contains(ex.Message, "was not found");
            }
        }
        finally
        {
            DeleteDirectoryIfExists(sRepoPath);
        }
    }

    [TestMethod]
    public async Task SplitAsync_DoesNotCreateExtraCommit_WhenBranchFilterKeepsAllFiles()
    {
        var sRepoPath = CreateRepositoryWithSingleRootFile();

        try
        {
            var service = new GitBranchSplitService();
            var options = new GitBranchSplitOptions
            {
                RepositoryPath = sRepoPath,
                SourceBranch = "main",
                BranchPrefix = "split",
                OverwriteExistingBranches = true
            };

            await service.SplitAsync(options, CancellationToken.None);

            using var repository = new Repository(sRepoPath);
            var sourceBranch = repository.Branches["main"];
            var rootBranch = repository.Branches["split/_root"];

            Assert.IsNotNull(sourceBranch);
            Assert.IsNotNull(rootBranch);
            Assert.AreEqual(sourceBranch.Tip.Sha, rootBranch.Tip.Sha);
        }
        finally
        {
            DeleteDirectoryIfExists(sRepoPath);
        }
    }

    private static string CreateRepository()
    {
        var sRepoPath = Path.Combine(Path.GetTempPath(), "RepoMigrator.Tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(sRepoPath);
        Repository.Init(sRepoPath);

        using var repository = new Repository(sRepoPath);
        File.WriteAllText(Path.Combine(sRepoPath, "README.md"), "repo");
        Commands.Stage(repository, "README.md");
        var signature = new Signature("tester", "tester@example.local", DateTimeOffset.UtcNow);
        repository.Commit("Initial commit", signature, signature);

        return sRepoPath;
    }

    private static string CreateRepositoryWithGroupedPaths()
    {
        var sRepoPath = Path.Combine(Path.GetTempPath(), "RepoMigrator.Tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(sRepoPath);
        Repository.Init(sRepoPath);

        using var repository = new Repository(sRepoPath);

        WriteTrackedFile(sRepoPath, "README.md", "root");
        WriteTrackedFile(sRepoPath, Path.Combine("AreaA", "summary.txt"), "A-summary");
        WriteTrackedFile(sRepoPath, Path.Combine("AreaB", "info.txt"), "B-info");
        WriteTrackedFile(sRepoPath, Path.Combine("AreaC", "data.txt"), "C-data");

        Commands.Stage(repository, "*");
        var signature = new Signature("tester", "tester@example.local", DateTimeOffset.UtcNow);
        repository.Commit("Initial grouped content", signature, signature);

        return sRepoPath;
    }

    private static string CreateRepositoryWithSingleRootFile()
    {
        var sRepoPath = Path.Combine(Path.GetTempPath(), "RepoMigrator.Tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(sRepoPath);
        Repository.Init(sRepoPath);

        using var repository = new Repository(sRepoPath);
        File.WriteAllText(Path.Combine(sRepoPath, "README.md"), "root-only");
        Commands.Stage(repository, "README.md");
        var signature = new Signature("tester", "tester@example.local", DateTimeOffset.UtcNow);
        repository.Commit("Initial root-only content", signature, signature);

        return sRepoPath;
    }

    private static string CreateRepositoryWithOrphanBranchWithoutCommit()
    {
        var sRepoPath = CreateRepository();

        using var repository = new Repository(sRepoPath);
        Commands.Checkout(repository, "main");

        RunGitInRepository(sRepoPath, "checkout --orphan orphan");
        RunGitInRepository(sRepoPath, "rm -rf --cached .");

        var readmePath = Path.Combine(sRepoPath, "README.md");
        if (File.Exists(readmePath))
            File.Delete(readmePath);

        return sRepoPath;
    }

    private static void RunGitInRepository(string sRepositoryPath, string sArguments)
    {
        var startInfo = new System.Diagnostics.ProcessStartInfo
        {
            FileName = "git",
            Arguments = sArguments,
            WorkingDirectory = sRepositoryPath,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = System.Diagnostics.Process.Start(startInfo);
        Assert.IsNotNull(process);
        process.WaitForExit();

        if (process.ExitCode != 0)
        {
            var error = process.StandardError.ReadToEnd();
            Assert.Fail($"git {sArguments} failed: {error}");
        }
    }

    private static void WriteTrackedFile(string sRepoPath, string sRelativePath, string sContent)
    {
        var sAbsolutePath = Path.Combine(sRepoPath, sRelativePath);
        var sDirectory = Path.GetDirectoryName(sAbsolutePath);
        if (!string.IsNullOrEmpty(sDirectory))
            Directory.CreateDirectory(sDirectory);
        File.WriteAllText(sAbsolutePath, sContent);
    }

    private static void DeleteDirectoryIfExists(string sPath)
    {
        if (!Directory.Exists(sPath))
            return;

        foreach (var file in Directory.EnumerateFiles(sPath, "*", SearchOption.AllDirectories))
            File.SetAttributes(file, FileAttributes.Normal);

        Directory.Delete(sPath, recursive: true);
    }

    private static void AssertBranchContains(Repository repository, string sBranchName, string[] arrExpectedPaths)
    {
        var branch = repository.Branches[sBranchName];
        Assert.IsNotNull(branch);
        Commands.Checkout(repository, branch);

        var arrTrackedPaths = repository.Index
            .Select(entry => entry.Path.Replace('\\', '/'))
            .OrderBy(path => path, StringComparer.Ordinal)
            .ToArray();

        var arrExpectedNormalized = arrExpectedPaths
            .Select(path => path.Replace('\\', '/'))
            .OrderBy(path => path, StringComparer.Ordinal)
            .ToArray();

        CollectionAssert.AreEqual(arrExpectedNormalized, arrTrackedPaths);
    }
}
