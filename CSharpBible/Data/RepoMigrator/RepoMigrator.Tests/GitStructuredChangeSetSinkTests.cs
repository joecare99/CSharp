using NSubstitute;
using RepoMigrator.Core;
using RepoMigrator.Core.Abstractions;
using RepoMigrator.Providers.Git;

namespace RepoMigrator.Tests;

[TestClass]
public sealed class GitStructuredChangeSetSinkTests
{
    [TestMethod]
    public async Task ApplyChangeSetAsync_WhenModifyHunkMatchesExistingContent_WritesPatchedTextToCommitSnapshot()
    {
        var provider = CreateProvider(out var committedWorkDirs);
        var workspaceRootPath = CreateWorkspaceRootPath();
        Directory.CreateDirectory(workspaceRootPath);
        var sink = new GitStructuredChangeSetSink(provider, workspaceRootPath);
        var destination = CreateGitDestination(workspaceRootPath);

        try
        {
            await sink.InitializeAsync(destination, CancellationToken.None);
            await sink.ApplyChangeSetAsync(CreateAddChangeSet("README.md", "line1\nline2\n"), NullMigrationProgress.Instance, CancellationToken.None);
            await sink.ApplyChangeSetAsync(
                new MigrationChangeSet
                {
                    ChangeSetId = "change-2",
                    Message = "modify",
                    AuthorName = "alice",
                    Timestamp = new DateTimeOffset(2024, 1, 2, 3, 5, 5, TimeSpan.Zero),
                    FileChanges =
                    [
                        new MigrationFileChange
                        {
                            Kind = MigrationFileChangeKind.Modify,
                            PathAfter = "README.md",
                            TextChange = new MigrationTextChange
                            {
                                LineEnding = "\n",
                                Hunks =
                                [
                                    new MigrationTextHunk
                                    {
                                        OriginalStartLine = 2,
                                        ModifiedStartLine = 2,
                                        RemovedText = "line2\n",
                                        AddedText = "line2 updated\n"
                                    }
                                ]
                            }
                        }
                    ]
                },
                NullMigrationProgress.Instance,
                CancellationToken.None);

            var committedFilePath = Path.Combine(committedWorkDirs.Last(), "README.md");
            Assert.AreEqual("line1\nline2 updated\n", File.ReadAllText(committedFilePath));
        }
        finally
        {
            await sink.DisposeAsync();
            DeleteDirectoryIfExists(workspaceRootPath);
        }
    }

    [TestMethod]
    public async Task ApplyChangeSetAsync_WhenModifyHunkDoesNotMatchExistingContent_FallsBackToAddedText()
    {
        var provider = CreateProvider(out var committedWorkDirs);
        var workspaceRootPath = CreateWorkspaceRootPath();
        Directory.CreateDirectory(workspaceRootPath);
        var sink = new GitStructuredChangeSetSink(provider, workspaceRootPath);
        var destination = CreateGitDestination(workspaceRootPath);

        try
        {
            await sink.InitializeAsync(destination, CancellationToken.None);
            await sink.ApplyChangeSetAsync(CreateAddChangeSet("README.md", "line1\nline2\n"), NullMigrationProgress.Instance, CancellationToken.None);
            await sink.ApplyChangeSetAsync(
                new MigrationChangeSet
                {
                    ChangeSetId = "change-2",
                    Message = "fallback",
                    AuthorName = "alice",
                    Timestamp = new DateTimeOffset(2024, 1, 2, 3, 5, 5, TimeSpan.Zero),
                    FileChanges =
                    [
                        new MigrationFileChange
                        {
                            Kind = MigrationFileChangeKind.Modify,
                            PathAfter = "README.md",
                            TextChange = new MigrationTextChange
                            {
                                LineEnding = "\n",
                                Hunks =
                                [
                                    new MigrationTextHunk
                                    {
                                        OriginalStartLine = 1,
                                        ModifiedStartLine = 1,
                                        RemovedText = "different\n",
                                        AddedText = "fallback only\n"
                                    }
                                ]
                            }
                        }
                    ]
                },
                NullMigrationProgress.Instance,
                CancellationToken.None);

            var committedFilePath = Path.Combine(committedWorkDirs.Last(), "README.md");
            Assert.AreEqual("fallback only\n", File.ReadAllText(committedFilePath));
        }
        finally
        {
            await sink.DisposeAsync();
            DeleteDirectoryIfExists(workspaceRootPath);
        }
    }

    [TestMethod]
    public async Task ApplyChangeSetAsync_WhenDeleteTargetsExistingFile_RemovesFileFromCommittedSnapshot()
    {
        var provider = CreateProvider(out var committedWorkDirs);
        var workspaceRootPath = CreateWorkspaceRootPath();
        Directory.CreateDirectory(workspaceRootPath);
        var sink = new GitStructuredChangeSetSink(provider, workspaceRootPath);
        var destination = CreateGitDestination(workspaceRootPath);

        try
        {
            await sink.InitializeAsync(destination, CancellationToken.None);
            await sink.ApplyChangeSetAsync(CreateAddChangeSet("README.md", "seed\n"), NullMigrationProgress.Instance, CancellationToken.None);
            await sink.ApplyChangeSetAsync(
                new MigrationChangeSet
                {
                    ChangeSetId = "delete-1",
                    Message = "delete",
                    AuthorName = "alice",
                    Timestamp = new DateTimeOffset(2024, 1, 2, 3, 5, 5, TimeSpan.Zero),
                    FileChanges =
                    [
                        new MigrationFileChange
                        {
                            Kind = MigrationFileChangeKind.Delete,
                            PathBefore = "README.md"
                        }
                    ]
                },
                NullMigrationProgress.Instance,
                CancellationToken.None);

            var committedFilePath = Path.Combine(committedWorkDirs.Last(), "README.md");
            Assert.IsFalse(File.Exists(committedFilePath));
        }
        finally
        {
            await sink.DisposeAsync();
            DeleteDirectoryIfExists(workspaceRootPath);
        }
    }

    [TestMethod]
    public async Task ApplyChangeSetAsync_WhenRenameTargetsExistingFile_PreservesContent()
    {
        var provider = CreateProvider(out var committedWorkDirs);
        var workspaceRootPath = CreateWorkspaceRootPath();
        Directory.CreateDirectory(workspaceRootPath);
        var sink = new GitStructuredChangeSetSink(provider, workspaceRootPath);
        var destination = CreateGitDestination(workspaceRootPath);

        try
        {
            await sink.InitializeAsync(destination, CancellationToken.None);
            await sink.ApplyChangeSetAsync(CreateAddChangeSet("README.md", "seed\n"), NullMigrationProgress.Instance, CancellationToken.None);
            await sink.ApplyChangeSetAsync(
                new MigrationChangeSet
                {
                    ChangeSetId = "rename-1",
                    Message = "rename",
                    AuthorName = "alice",
                    Timestamp = new DateTimeOffset(2024, 1, 2, 3, 5, 5, TimeSpan.Zero),
                    FileChanges =
                    [
                        new MigrationFileChange
                        {
                            Kind = MigrationFileChangeKind.Rename,
                            PathBefore = "README.md",
                            PathAfter = "docs/README.md"
                        }
                    ]
                },
                NullMigrationProgress.Instance,
                CancellationToken.None);

            var originalPath = Path.Combine(committedWorkDirs.Last(), "README.md");
            var renamedPath = Path.Combine(committedWorkDirs.Last(), "docs", "README.md");
            Assert.IsFalse(File.Exists(originalPath));
            Assert.AreEqual("seed\n", File.ReadAllText(renamedPath));
        }
        finally
        {
            await sink.DisposeAsync();
            DeleteDirectoryIfExists(workspaceRootPath);
        }
    }

    [TestMethod]
    public async Task ApplyChangeSetAsync_WhenCopyTargetsExistingFile_PreservesSourceContent()
    {
        var provider = CreateProvider(out var committedWorkDirs);
        var workspaceRootPath = CreateWorkspaceRootPath();
        Directory.CreateDirectory(workspaceRootPath);
        var sink = new GitStructuredChangeSetSink(provider, workspaceRootPath);
        var destination = CreateGitDestination(workspaceRootPath);

        try
        {
            await sink.InitializeAsync(destination, CancellationToken.None);
            await sink.ApplyChangeSetAsync(CreateAddChangeSet("README.md", "seed\n"), NullMigrationProgress.Instance, CancellationToken.None);
            await sink.ApplyChangeSetAsync(
                new MigrationChangeSet
                {
                    ChangeSetId = "copy-1",
                    Message = "copy",
                    AuthorName = "alice",
                    Timestamp = new DateTimeOffset(2024, 1, 2, 3, 5, 5, TimeSpan.Zero),
                    FileChanges =
                    [
                        new MigrationFileChange
                        {
                            Kind = MigrationFileChangeKind.Copy,
                            PathBefore = "README.md",
                            PathAfter = "docs/README-copy.md"
                        }
                    ]
                },
                NullMigrationProgress.Instance,
                CancellationToken.None);

            var originalPath = Path.Combine(committedWorkDirs.Last(), "README.md");
            var copiedPath = Path.Combine(committedWorkDirs.Last(), "docs", "README-copy.md");
            Assert.AreEqual("seed\n", File.ReadAllText(originalPath));
            Assert.AreEqual("seed\n", File.ReadAllText(copiedPath));
        }
        finally
        {
            await sink.DisposeAsync();
            DeleteDirectoryIfExists(workspaceRootPath);
        }
    }

    [TestMethod]
    public async Task ApplyChangeSetAsync_WhenCopyMetadataOverridesSourcePath_UsesPatchCopySource()
    {
        var provider = CreateProvider(out var committedWorkDirs);
        var workspaceRootPath = CreateWorkspaceRootPath();
        Directory.CreateDirectory(workspaceRootPath);
        var sink = new GitStructuredChangeSetSink(provider, workspaceRootPath);
        var destination = CreateGitDestination(workspaceRootPath);

        try
        {
            await sink.InitializeAsync(destination, CancellationToken.None);
            await sink.ApplyChangeSetAsync(CreateAddChangeSet("source/original.txt", "seed\n"), NullMigrationProgress.Instance, CancellationToken.None);
            await sink.ApplyChangeSetAsync(
                new MigrationChangeSet
                {
                    ChangeSetId = "copy-meta-1",
                    Message = "copy-meta",
                    AuthorName = "alice",
                    Timestamp = new DateTimeOffset(2024, 1, 2, 3, 5, 5, TimeSpan.Zero),
                    FileChanges =
                    [
                        new MigrationFileChange
                        {
                            Kind = MigrationFileChangeKind.Copy,
                            PathBefore = "wrong/source.txt",
                            PathAfter = "docs/copied.txt",
                            Metadata = new Dictionary<string, string>
                            {
                                [StructuredChangeMetadataKeys.PatchCopyFrom] = "source/original.txt",
                                [StructuredChangeMetadataKeys.PatchCopyTo] = "docs/copied.txt"
                            }
                        }
                    ]
                },
                NullMigrationProgress.Instance,
                CancellationToken.None);

            var copiedPath = Path.Combine(committedWorkDirs.Last(), "docs", "copied.txt");
            Assert.AreEqual("seed\n", File.ReadAllText(copiedPath));
        }
        finally
        {
            await sink.DisposeAsync();
            DeleteDirectoryIfExists(workspaceRootPath);
        }
    }

    [TestMethod]
    public async Task ApplyChangeSetAsync_WhenModeMetadataMarksFileReadOnly_AppliesReadOnlyAttribute()
    {
        var provider = CreateProvider(out var committedWorkDirs);
        var workspaceRootPath = CreateWorkspaceRootPath();
        Directory.CreateDirectory(workspaceRootPath);
        var sink = new GitStructuredChangeSetSink(provider, workspaceRootPath);
        var destination = CreateGitDestination(workspaceRootPath);

        try
        {
            await sink.InitializeAsync(destination, CancellationToken.None);
            await sink.ApplyChangeSetAsync(CreateAddChangeSet("scripts/run.cmd", "echo hi\n"), NullMigrationProgress.Instance, CancellationToken.None);
            await sink.ApplyChangeSetAsync(
                new MigrationChangeSet
                {
                    ChangeSetId = "mode-1",
                    Message = "mode",
                    AuthorName = "alice",
                    Timestamp = new DateTimeOffset(2024, 1, 2, 3, 5, 5, TimeSpan.Zero),
                    FileChanges =
                    [
                        new MigrationFileChange
                        {
                            Kind = MigrationFileChangeKind.ModeChange,
                            PathBefore = "scripts/run.cmd",
                            PathAfter = "scripts/run.cmd",
                            Metadata = new Dictionary<string, string>
                            {
                                [StructuredChangeMetadataKeys.PatchOldMode] = "100755",
                                [StructuredChangeMetadataKeys.PatchNewMode] = "100555"
                            }
                        }
                    ]
                },
                NullMigrationProgress.Instance,
                CancellationToken.None);

            var modeChangedPath = Path.Combine(committedWorkDirs.Last(), "scripts", "run.cmd");
            Assert.IsTrue(File.Exists(modeChangedPath));
            Assert.IsTrue(File.GetAttributes(modeChangedPath).HasFlag(FileAttributes.ReadOnly));
        }
        finally
        {
            await sink.DisposeAsync();
            DeleteDirectoryIfExists(workspaceRootPath);
        }
    }

    [TestMethod]
    public void CanHandle_WhenDestinationIsGitRepository_ReturnsTrue()
    {
        var provider = Substitute.For<IVersionControlProvider>();
        provider.Name.Returns("Git substitute");
        var sink = new GitStructuredChangeSetSink(provider, Path.GetTempPath());
        var destination = new MigrationDestinationDefinition
        {
            Kind = MigrationDestinationKind.Repository,
            Repository = new RepositoryEndpoint
            {
                ProviderKey = GitProvider.ProviderKey,
                UrlOrPath = @"C:\target",
                BranchOrTrunk = "main"
            }
        };

        Assert.IsTrue(sink.CanHandle(destination));
    }

    [TestMethod]
    public async Task GetCapabilitiesAsync_WhenDestinationIsGitRepository_ReturnsMaterializedStructuredCapabilities()
    {
        var provider = Substitute.For<IVersionControlProvider>();
        provider.Name.Returns("Git substitute");
        var sink = new GitStructuredChangeSetSink(provider, Path.GetTempPath());
        var destination = new MigrationDestinationDefinition
        {
            Kind = MigrationDestinationKind.Repository,
            Repository = new RepositoryEndpoint
            {
                ProviderKey = GitProvider.ProviderKey,
                UrlOrPath = @"C:\target",
                BranchOrTrunk = "main"
            }
        };

        var capabilities = await sink.GetCapabilitiesAsync(destination, CancellationToken.None);

        Assert.IsTrue(capabilities.SupportsStructuredChanges);
        Assert.IsTrue(capabilities.SupportsMaterializedWorkdirFallback);
        Assert.IsTrue(capabilities.SupportsTextChanges);
        Assert.IsTrue(capabilities.SupportsBinaryChanges);
        Assert.AreEqual(6, capabilities.SupportedChangeKinds.Count);
    }

    [TestMethod]
    public async Task ApplyChangeSetAsync_WhenInitialized_DelegatesCommitToVersionControlProvider()
    {
        var provider = Substitute.For<IVersionControlProvider>();
        provider.Name.Returns("Git substitute");
        provider.DisposeAsync().Returns(ValueTask.CompletedTask);
        var workspaceRootPath = CreateWorkspaceRootPath();
        Directory.CreateDirectory(workspaceRootPath);
        var sink = new GitStructuredChangeSetSink(provider, workspaceRootPath);
        var destination = CreateGitDestination(workspaceRootPath);
        var changeSet = new MigrationChangeSet
        {
            ChangeSetId = "change-1",
            Message = "apply change",
            AuthorName = "alice",
            AuthorEmail = "alice@example.invalid",
            Timestamp = new DateTimeOffset(2024, 1, 2, 3, 4, 5, TimeSpan.Zero),
            FileChanges =
            [
                new MigrationFileChange
                {
                    Kind = MigrationFileChangeKind.Add,
                    PathAfter = "README.md",
                    TextChange = new MigrationTextChange
                    {
                        Hunks =
                        [
                            new MigrationTextHunk
                            {
                                ModifiedStartLine = 1,
                                AddedText = "hello world"
                            }
                        ]
                    }
                }
            ]
        };

        try
        {
            await sink.InitializeAsync(destination, CancellationToken.None);
            await sink.ApplyChangeSetAsync(changeSet, NullMigrationProgress.Instance, CancellationToken.None);
            await sink.FinalizeAsync(CancellationToken.None);

            await provider.Received(1).InitializeTargetAsync(destination.Repository!, true, CancellationToken.None);
            await provider.Received(1).CommitSnapshotAsync(
                Arg.Any<string>(),
                Arg.Is<CommitMetadata>(metadata => metadata.Message == "apply change" && metadata.AuthorName == "alice" && metadata.TargetBranch == "main"),
                NullMigrationProgress.Instance,
                CancellationToken.None);
            await provider.Received(1).FlushAsync(CancellationToken.None);
        }
        finally
        {
            await sink.DisposeAsync();
            DeleteDirectoryIfExists(workspaceRootPath);
        }
    }

    private static IVersionControlProvider CreateProvider(out List<string> committedWorkDirs)
    {
        var provider = Substitute.For<IVersionControlProvider>();
        provider.Name.Returns("Git substitute");
        provider.DisposeAsync().Returns(ValueTask.CompletedTask);
        var localCommittedWorkDirs = new List<string>();
        committedWorkDirs = localCommittedWorkDirs;
        provider.CommitSnapshotAsync(Arg.Any<string>(), Arg.Any<CommitMetadata>(), Arg.Any<IMigrationProgress>(), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                localCommittedWorkDirs.Add(callInfo.ArgAt<string>(0));
                return Task.CompletedTask;
            });
        return provider;
    }

    private static string CreateWorkspaceRootPath()
        => Path.Combine(Path.GetTempPath(), $"RepoMigrator-GitSink-{Guid.NewGuid():N}");

    private static MigrationDestinationDefinition CreateGitDestination(string workspaceRootPath)
        => new()
        {
            Kind = MigrationDestinationKind.Repository,
            Repository = new RepositoryEndpoint
            {
                ProviderKey = GitProvider.ProviderKey,
                UrlOrPath = Path.Combine(workspaceRootPath, "target"),
                BranchOrTrunk = "main"
            }
        };

    private static MigrationChangeSet CreateAddChangeSet(string pathAfter, string addedText)
        => new()
        {
            ChangeSetId = $"add-{pathAfter}",
            Message = "seed",
            AuthorName = "alice",
            Timestamp = new DateTimeOffset(2024, 1, 2, 3, 4, 5, TimeSpan.Zero),
            FileChanges =
            [
                new MigrationFileChange
                {
                    Kind = MigrationFileChangeKind.Add,
                    PathAfter = pathAfter,
                    TextChange = new MigrationTextChange
                    {
                        LineEnding = "\n",
                        Hunks =
                        [
                            new MigrationTextHunk
                            {
                                ModifiedStartLine = 1,
                                AddedText = addedText
                            }
                        ]
                    }
                }
            ]
        };

    private static void DeleteDirectoryIfExists(string path)
    {
        if (Directory.Exists(path))
            Directory.Delete(path, recursive: true);
    }
}
