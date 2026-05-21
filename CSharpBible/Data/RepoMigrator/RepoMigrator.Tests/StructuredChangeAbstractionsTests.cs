using NSubstitute;
using RepoMigrator.Core;
using RepoMigrator.Core.Abstractions;
using RepoMigrator.Core.Services;
using RepoMigrator.Providers.Git;
using RepoMigrator.Providers.Patch;
using RepoMigrator.Providers.Patch.Services;
using RepoMigrator.Providers.Svn;
using RepoMigrator.Providers.SvnCli;

namespace RepoMigrator.Tests;

[TestClass]
public sealed class StructuredChangeAbstractionsTests
{
    [TestMethod]
    public void MigrationExecutionPathSelection_Defaults_AreInitialized()
    {
        var selection = new MigrationExecutionPathSelection();

        Assert.AreEqual(MigrationExecutionPathKind.Unknown, selection.Kind);
        Assert.AreEqual(string.Empty, selection.Rationale);
        Assert.AreEqual(0, selection.RejectedAlternatives.Count);
        Assert.AreEqual(0, selection.Metadata.Count);
    }

    [TestMethod]
    public void MigrationExecutionPathRequest_Defaults_AreInitialized()
    {
        var request = new MigrationExecutionPathRequest();

        Assert.IsFalse(request.SourceSupportsDirectTransfer);
        Assert.IsFalse(request.DestinationSupportsDirectTransfer);
        Assert.IsNotNull(request.SourceCapabilities);
        Assert.IsNotNull(request.DestinationCapabilities);
        Assert.IsFalse(request.RequiresPathRewrites);
        Assert.IsFalse(request.RequiresStructuredNormalization);
        Assert.IsTrue(request.SupportsSnapshotCompatibility);
    }

    [TestMethod]
    public void MigrationExecutionPathSelector_WhenDirectTransferIsPossible_SelectsDirectTransfer()
    {
        var selector = new MigrationExecutionPathSelector();
        var request = new MigrationExecutionPathRequest
        {
            SourceSupportsDirectTransfer = true,
            DestinationSupportsDirectTransfer = true
        };

        var selection = selector.Select(request);

        Assert.AreEqual(MigrationExecutionPathKind.DirectTransfer, selection.Kind);
        Assert.AreEqual(0, selection.RejectedAlternatives.Count);
    }

    [TestMethod]
    public void MigrationExecutionPathSelector_WhenDirectTransferIsBlockedButStructuredChangesAreSupported_SelectsStructuredChange()
    {
        var selector = new MigrationExecutionPathSelector();
        var request = new MigrationExecutionPathRequest
        {
            SourceSupportsDirectTransfer = true,
            DestinationSupportsDirectTransfer = true,
            RequiresStructuredNormalization = true,
            SourceCapabilities = new ChangeApplicationCapabilities { SupportsStructuredChanges = true },
            DestinationCapabilities = new ChangeApplicationCapabilities { SupportsStructuredChanges = true }
        };

        var selection = selector.Select(request);

        Assert.AreEqual(MigrationExecutionPathKind.StructuredChange, selection.Kind);
        CollectionAssert.Contains(selection.RejectedAlternatives.ToArray(), MigrationExecutionPathKind.DirectTransfer);
    }

    [TestMethod]
    public void MigrationExecutionPathSelector_WhenOnlyCompatibilityMaterializationIsAvailable_SelectsStructuredChangeWithMaterializedWorkdir()
    {
        var selector = new MigrationExecutionPathSelector();
        var request = new MigrationExecutionPathRequest
        {
            SourceCapabilities = new ChangeApplicationCapabilities { SupportsStructuredChanges = true },
            DestinationCapabilities = new ChangeApplicationCapabilities { SupportsMaterializedWorkdirFallback = true },
            SupportsSnapshotCompatibility = false
        };

        var selection = selector.Select(request);

        Assert.AreEqual(MigrationExecutionPathKind.StructuredChangeWithMaterializedWorkdir, selection.Kind);
        CollectionAssert.Contains(selection.RejectedAlternatives.ToArray(), MigrationExecutionPathKind.DirectTransfer);
        CollectionAssert.Contains(selection.RejectedAlternatives.ToArray(), MigrationExecutionPathKind.StructuredChange);
    }

    [TestMethod]
    public void MigrationExecutionPathSelector_WhenStructuredChangesAreUnavailable_SelectsSnapshotCompatibility()
    {
        var selector = new MigrationExecutionPathSelector();

        var selection = selector.Select(new MigrationExecutionPathRequest());

        Assert.AreEqual(MigrationExecutionPathKind.SnapshotCompatibility, selection.Kind);
    }

    [TestMethod]
    public void MigrationExecutionPathSelector_WhenNoFallbackExists_ReturnsUnknown()
    {
        var selector = new MigrationExecutionPathSelector();
        var request = new MigrationExecutionPathRequest
        {
            SupportsSnapshotCompatibility = false
        };

        var selection = selector.Select(request);

        Assert.AreEqual(MigrationExecutionPathKind.Unknown, selection.Kind);
        CollectionAssert.Contains(selection.RejectedAlternatives.ToArray(), MigrationExecutionPathKind.SnapshotCompatibility);
    }

    [TestMethod]
    public void StructuredChangeInterfaces_AreAssignable_FromTestDoubles()
    {
        IMigrationChangeSetSource source = new TestMigrationChangeSetSource();
        IMigrationChangeSetSink sink = new TestMigrationChangeSetSink();
        var sourceDefinition = new MigrationSourceDefinition();
        var destinationDefinition = new MigrationDestinationDefinition();

        Assert.AreEqual("Structured source", source.Name);
        Assert.AreEqual("Structured sink", sink.Name);
        Assert.IsTrue(source.CanHandle(sourceDefinition));
        Assert.IsTrue(sink.CanHandle(destinationDefinition));
    }

    [TestMethod]
    public async Task StructuredChangeInterfaces_ReturnConfiguredCapabilitiesAndChangeSets()
    {
        IMigrationChangeSetSource source = new TestMigrationChangeSetSource();
        IMigrationChangeSetSink sink = new TestMigrationChangeSetSink();
        var sourceDefinition = new MigrationSourceDefinition();
        var destinationDefinition = new MigrationDestinationDefinition();
        var progress = NullMigrationProgress.Instance;
        var changeSet = (await source.GetChangeSetsAsync(sourceDefinition, CancellationToken.None)).Single();

        var sourceCapabilities = await source.GetCapabilitiesAsync(sourceDefinition, CancellationToken.None);
        var sinkCapabilities = await sink.GetCapabilitiesAsync(destinationDefinition, CancellationToken.None);

        Assert.IsTrue(sourceCapabilities.SupportsStructuredChanges);
        Assert.IsTrue(sourceCapabilities.SupportsDirectoryChanges);
        Assert.IsTrue(sinkCapabilities.SupportsStructuredChanges);
        Assert.IsTrue(sinkCapabilities.SupportsMaterializedWorkdirFallback);
        Assert.AreEqual("change-1", changeSet.ChangeSetId);

        await sink.InitializeAsync(destinationDefinition, CancellationToken.None);
        await sink.ApplyChangeSetAsync(changeSet, progress, CancellationToken.None);
        await sink.FinalizeAsync(CancellationToken.None);
    }

    [TestMethod]
    public void MigrationChangeSetSourceFactory_WhenSourceMatches_ReturnsSupportingSource()
    {
        var sourceDefinition = new PatchMigrationSourceDefinition
        {
            Location = @"C:\patches"
        }.ToMigrationSourceDefinition();
        IMigrationChangeSetSource[] sources = [new DirectoryPatchChangeSetSource()];
        var factory = new MigrationChangeSetSourceFactory(sources);

        var source = factory.Create(sourceDefinition);

        Assert.IsInstanceOfType<DirectoryPatchChangeSetSource>(source);
    }

    [TestMethod]
    public void MigrationChangeSetSourceFactory_WhenNoSourceMatches_ThrowsNotSupportedException()
    {
        var sourceDefinition = new MigrationSourceDefinition();
        IMigrationChangeSetSource[] sources = [new DirectoryPatchChangeSetSource()];
        var factory = new MigrationChangeSetSourceFactory(sources);

        NotSupportedException? ex = null;
        try
        {
            _ = factory.Create(sourceDefinition);
            Assert.Fail("Expected NotSupportedException.");
        }
        catch (NotSupportedException caughtEx)
        {
            ex = caughtEx;
        }

        Assert.IsNotNull(ex);
    }

    [TestMethod]
    public async Task StructuredMigrationPlanner_WhenBothSidesSupportStructuredChanges_SelectsStructuredChange()
    {
        var sourceFactory = new MigrationChangeSetSourceFactory([new TestMigrationChangeSetSource()]);
        var sinkFactory = new MigrationChangeSetSinkFactory([new TestMigrationChangeSetSink()]);
        var planner = new StructuredMigrationPlanner(sourceFactory, sinkFactory, new MigrationExecutionPathSelector());

        var result = await planner.PlanAsync(
            new StructuredMigrationPlanningRequest
            {
                Source = new MigrationSourceDefinition(),
                Destination = new MigrationDestinationDefinition(),
                RequiresStructuredNormalization = true,
                SupportsSnapshotCompatibility = false
            },
            CancellationToken.None);

        Assert.AreEqual(MigrationExecutionPathKind.StructuredChange, result.ExecutionPath.Kind);
        Assert.IsInstanceOfType<TestMigrationChangeSetSource>(result.Source);
        Assert.IsInstanceOfType<TestMigrationChangeSetSink>(result.Sink);
        Assert.IsTrue(result.SourceCapabilities.SupportsStructuredChanges);
        Assert.IsTrue(result.DestinationCapabilities.SupportsStructuredChanges);
    }

    [TestMethod]
    public async Task StructuredMigrationPlanner_WhenDestinationNeedsCompatibilityFallback_SelectsMaterializedWorkdir()
    {
        var sourceFactory = new MigrationChangeSetSourceFactory([new TestMigrationChangeSetSource()]);
        var sinkFactory = new MigrationChangeSetSinkFactory([new TestMaterializingMigrationChangeSetSink()]);
        var planner = new StructuredMigrationPlanner(sourceFactory, sinkFactory, new MigrationExecutionPathSelector());

        var result = await planner.PlanAsync(
            new StructuredMigrationPlanningRequest
            {
                Source = new MigrationSourceDefinition(),
                Destination = new MigrationDestinationDefinition(),
                SupportsSnapshotCompatibility = false
            },
            CancellationToken.None);

        Assert.AreEqual(MigrationExecutionPathKind.StructuredChangeWithMaterializedWorkdir, result.ExecutionPath.Kind);
        Assert.IsTrue(result.SourceCapabilities.SupportsStructuredChanges);
        Assert.IsTrue(result.DestinationCapabilities.SupportsMaterializedWorkdirFallback);
        Assert.IsFalse(result.DestinationCapabilities.SupportsStructuredChanges);
    }

    [TestMethod]
    public async Task StructuredMigrationPlanner_WhenSourceCannotBeResolved_ThrowsNotSupportedException()
    {
        var sourceFactory = new MigrationChangeSetSourceFactory([new TestRejectingMigrationChangeSetSource()]);
        var sinkFactory = new MigrationChangeSetSinkFactory([new TestMigrationChangeSetSink()]);
        var planner = new StructuredMigrationPlanner(sourceFactory, sinkFactory, new MigrationExecutionPathSelector());

        NotSupportedException? ex = null;
        try
        {
            _ = await planner.PlanAsync(
                new StructuredMigrationPlanningRequest
                {
                    Source = new MigrationSourceDefinition(),
                    Destination = new MigrationDestinationDefinition()
                },
                CancellationToken.None);
            Assert.Fail("Expected NotSupportedException.");
        }
        catch (NotSupportedException caughtEx)
        {
            ex = caughtEx;
        }

        Assert.IsNotNull(ex);
    }

    [TestMethod]
    public void MigrationChangeSetSinkFactory_WhenDestinationMatches_ReturnsSupportingSink()
    {
        var destinationDefinition = new MigrationDestinationDefinition
        {
            Kind = MigrationDestinationKind.Repository,
            Repository = new RepositoryEndpoint
            {
                ProviderKey = GitProvider.ProviderKey,
                UrlOrPath = @"C:\target",
                BranchOrTrunk = "main"
            }
        };
        IMigrationChangeSetSink[] sinks = [new TestMigrationChangeSetSink()];
        var factory = new MigrationChangeSetSinkFactory(sinks);

        var sink = factory.Create(destinationDefinition);

        Assert.IsInstanceOfType<TestMigrationChangeSetSink>(sink);
    }

    [TestMethod]
    public void MigrationChangeSetSinkFactory_WhenMultipleStructuredRepositorySinksAreRegistered_SelectsMatchingProviderSink()
    {
        var workspaceRootPath = Path.Combine(Path.GetTempPath(), $"RepoMigrator-StructuredSinkFactory-{Guid.NewGuid():N}");
        Directory.CreateDirectory(workspaceRootPath);

        try
        {
            var gitProvider = CreateVersionControlProvider("Git substitute");
            var svnProvider = CreateVersionControlProvider("SVN substitute");
            IMigrationChangeSetSink[] sinks =
            [
                new SvnStructuredChangeSetSink(svnProvider, workspaceRootPath),
                new GitStructuredChangeSetSink(gitProvider, workspaceRootPath)
            ];
            var factory = new MigrationChangeSetSinkFactory(sinks);

            var gitSink = factory.Create(new MigrationDestinationDefinition
            {
                Kind = MigrationDestinationKind.Repository,
                Repository = new RepositoryEndpoint
                {
                    ProviderKey = GitProvider.ProviderKey,
                    UrlOrPath = Path.Combine(workspaceRootPath, "git-target"),
                    BranchOrTrunk = "main"
                }
            });
            var svnSink = factory.Create(new MigrationDestinationDefinition
            {
                Kind = MigrationDestinationKind.Repository,
                Repository = new RepositoryEndpoint
                {
                    ProviderKey = SvnCliProvider.ProviderKey,
                    UrlOrPath = Path.Combine(workspaceRootPath, "svn-target"),
                    BranchOrTrunk = "trunk"
                }
            });

            Assert.IsInstanceOfType<GitStructuredChangeSetSink>(gitSink);
            Assert.IsInstanceOfType<SvnStructuredChangeSetSink>(svnSink);
        }
        finally
        {
            if (Directory.Exists(workspaceRootPath))
                Directory.Delete(workspaceRootPath, recursive: true);
        }
    }

    [TestMethod]
    public void MigrationChangeSetSinkFactory_WhenNoSinkMatches_ThrowsNotSupportedException()
    {
        var destinationDefinition = new MigrationDestinationDefinition();
        IMigrationChangeSetSink[] sinks = [new TestRejectingMigrationChangeSetSink()];
        var factory = new MigrationChangeSetSinkFactory(sinks);

        NotSupportedException? ex = null;
        try
        {
            _ = factory.Create(destinationDefinition);
            Assert.Fail("Expected NotSupportedException.");
        }
        catch (NotSupportedException caughtEx)
        {
            ex = caughtEx;
        }

        Assert.IsNotNull(ex);
    }

    [TestMethod]
    public async Task DirectoryPatchChangeSetSource_WhenLocalPatchExists_ParsesNormalizedChangeSet()
    {
        var directoryPath = Path.Combine(Path.GetTempPath(), $"RepoMigrator-PatchSource-{Guid.NewGuid():N}");
        Directory.CreateDirectory(directoryPath);

        try
        {
            var patchFilePath = Path.Combine(directoryPath, "0001-rename.patch");
            await File.WriteAllTextAsync(
                patchFilePath,
                "--- a/old/root/file.txt\n+++ b/new/root/file.txt\n@@ -1,1 +1,1 @@\n-old value\n+new value\n",
                CancellationToken.None);

            var sourceDefinition = new PatchMigrationSourceDefinition
            {
                Location = directoryPath,
                PathRewrites =
                [
                    new PathRewriteRule
                    {
                        FromPrefix = "old/root",
                        ToPrefix = "normalized/root",
                        NormalizeDirectorySeparators = true
                    },
                    new PathRewriteRule
                    {
                        FromPrefix = "new/root",
                        ToPrefix = "normalized/root",
                        NormalizeDirectorySeparators = true
                    }
                ]
            }.ToMigrationSourceDefinition();

            var source = new DirectoryPatchChangeSetSource();
            var capabilities = await source.GetCapabilitiesAsync(sourceDefinition, CancellationToken.None);
            var changeSet = (await source.GetChangeSetsAsync(sourceDefinition, CancellationToken.None)).Single();

            Assert.IsTrue(source.CanHandle(sourceDefinition));
            Assert.IsTrue(capabilities.SupportsStructuredChanges);
            Assert.IsTrue(capabilities.SupportsDirectoryChanges);
            Assert.IsTrue(capabilities.SupportsPathRewrites);
            Assert.AreEqual(nameof(MigrationSourceKind.PatchCollection), changeSet.Metadata[StructuredChangeMetadataKeys.SourceKind]);
            Assert.AreEqual(1, changeSet.FileChanges.Count);
            Assert.AreEqual(MigrationFileChangeKind.Rename, changeSet.FileChanges[0].Kind);
            Assert.AreEqual("normalized/root/file.txt", changeSet.FileChanges[0].PathBefore);
            Assert.AreEqual("normalized/root/file.txt", changeSet.FileChanges[0].PathAfter);
            Assert.IsNotNull(changeSet.FileChanges[0].TextChange);
            Assert.AreEqual(1, changeSet.FileChanges[0].TextChange!.Hunks.Count);
            Assert.AreEqual($"old value{Environment.NewLine}", changeSet.FileChanges[0].TextChange.Hunks[0].RemovedText);
            Assert.AreEqual($"new value{Environment.NewLine}", changeSet.FileChanges[0].TextChange.Hunks[0].AddedText);
        }
        finally
        {
            if (Directory.Exists(directoryPath))
                Directory.Delete(directoryPath, recursive: true);
        }
    }

    [TestMethod]
    public void PatchChangeSetParser_WhenPathsMoveDirectories_EmitsDirectoryRename()
    {
        var parser = new PatchChangeSetParser(Array.Empty<PathRewriteRule>());

        var changeSet = parser.ParseFile(
            "sample.patch",
            "--- a/src/old/file.txt\n+++ b/src/new/file.txt\n@@ -1,1 +1,1 @@\n-old\n+new\n");

        Assert.AreEqual(1, changeSet.DirectoryChanges.Count);
        Assert.AreEqual(MigrationDirectoryChangeKind.Rename, changeSet.DirectoryChanges[0].Kind);
        Assert.AreEqual("src/old", changeSet.DirectoryChanges[0].PathBefore);
        Assert.AreEqual("src/new", changeSet.DirectoryChanges[0].PathAfter);
    }

    [TestMethod]
    public void PatchChangeSetParser_WhenCopyHeadersExist_EmitsCopyChange()
    {
        var parser = new PatchChangeSetParser(Array.Empty<PathRewriteRule>());

        var changeSet = parser.ParseFile(
            "copy.patch",
            "diff --git a/src/file.txt b/src/file-copy.txt\ncopy from src/file.txt\ncopy to src/file-copy.txt\n--- a/src/file.txt\n+++ b/src/file-copy.txt\n@@ -1,1 +1,1 @@\n-old\n+new\n");

        Assert.AreEqual(1, changeSet.FileChanges.Count);
        Assert.AreEqual(MigrationFileChangeKind.Copy, changeSet.FileChanges[0].Kind);
        Assert.AreEqual("src/file.txt", changeSet.FileChanges[0].PathBefore);
        Assert.AreEqual("src/file-copy.txt", changeSet.FileChanges[0].PathAfter);
        Assert.AreEqual("src/file.txt", changeSet.FileChanges[0].Metadata[StructuredChangeMetadataKeys.PatchCopyFrom]);
        Assert.AreEqual("src/file-copy.txt", changeSet.FileChanges[0].Metadata[StructuredChangeMetadataKeys.PatchCopyTo]);
    }

    [TestMethod]
    public void PatchChangeSetParser_WhenOnlyModeChangesExist_EmitsModeChange()
    {
        var parser = new PatchChangeSetParser(Array.Empty<PathRewriteRule>());

        var changeSet = parser.ParseFile(
            "mode.patch",
            "diff --git a/scripts/run.sh b/scripts/run.sh\nold mode 100644\nnew mode 100755\n--- a/scripts/run.sh\n+++ b/scripts/run.sh\n");

        Assert.AreEqual(1, changeSet.FileChanges.Count);
        Assert.AreEqual(MigrationFileChangeKind.ModeChange, changeSet.FileChanges[0].Kind);
        Assert.AreEqual("scripts/run.sh", changeSet.FileChanges[0].PathBefore);
        Assert.AreEqual("scripts/run.sh", changeSet.FileChanges[0].PathAfter);
        Assert.AreEqual("100644", changeSet.FileChanges[0].Metadata[StructuredChangeMetadataKeys.PatchOldMode]);
        Assert.AreEqual("100755", changeSet.FileChanges[0].Metadata[StructuredChangeMetadataKeys.PatchNewMode]);
    }

    private sealed class TestMigrationChangeSetSource : IMigrationChangeSetSource
    {
        public string Name => "Structured source";

        public bool CanHandle(MigrationSourceDefinition source) => true;

        public Task<ChangeApplicationCapabilities> GetCapabilitiesAsync(MigrationSourceDefinition source, CancellationToken ct)
            => Task.FromResult(new ChangeApplicationCapabilities
            {
                SupportsStructuredChanges = true,
                SupportsDirectoryChanges = true,
                SupportsTextChanges = true,
                SupportsTextHunks = true,
                SupportsBinaryChanges = true,
                SupportsPathRewrites = true,
                SupportedDirectoryChangeKinds = [MigrationDirectoryChangeKind.Copy, MigrationDirectoryChangeKind.Rename],
                SupportedChangeKinds = [MigrationFileChangeKind.Add, MigrationFileChangeKind.Rename]
            });

        public Task<IReadOnlyList<MigrationChangeSet>> GetChangeSetsAsync(MigrationSourceDefinition source, CancellationToken ct)
            => Task.FromResult<IReadOnlyList<MigrationChangeSet>>(
            [
                new MigrationChangeSet
                {
                    ChangeSetId = "change-1",
                    Message = "test",
                    AuthorName = "alice",
                    DirectoryChanges =
                    [
                        new MigrationDirectoryChange
                        {
                            Kind = MigrationDirectoryChangeKind.Copy,
                            PathBefore = "branches/release-1",
                            PathAfter = "branches/release-1-hotfix"
                        }
                    ],
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
                                        AddedText = "hello"
                                    }
                                ]
                            }
                        }
                    ]
                }
            ]);
    }

    private sealed class TestRejectingMigrationChangeSetSource : IMigrationChangeSetSource
    {
        public string Name => "Rejecting structured source";

        public bool CanHandle(MigrationSourceDefinition source) => false;

        public Task<ChangeApplicationCapabilities> GetCapabilitiesAsync(MigrationSourceDefinition source, CancellationToken ct)
            => Task.FromResult(new ChangeApplicationCapabilities());

        public Task<IReadOnlyList<MigrationChangeSet>> GetChangeSetsAsync(MigrationSourceDefinition source, CancellationToken ct)
            => Task.FromResult<IReadOnlyList<MigrationChangeSet>>(Array.Empty<MigrationChangeSet>());
    }

    private sealed class TestMigrationChangeSetSink : IMigrationChangeSetSink
    {
        public string Name => "Structured sink";

        public bool CanHandle(MigrationDestinationDefinition destination) => true;

        public Task<ChangeApplicationCapabilities> GetCapabilitiesAsync(MigrationDestinationDefinition destination, CancellationToken ct)
            => Task.FromResult(new ChangeApplicationCapabilities
            {
                SupportsStructuredChanges = true,
                SupportsDirectoryChanges = true,
                SupportsTextChanges = true,
                SupportsTextHunks = true,
                SupportsBinaryChanges = true,
                SupportsPathRewrites = true,
                SupportsMaterializedWorkdirFallback = true,
                SupportedDirectoryChangeKinds = [MigrationDirectoryChangeKind.Copy, MigrationDirectoryChangeKind.Rename],
                SupportedChangeKinds = [MigrationFileChangeKind.Add, MigrationFileChangeKind.Rename]
            });

        public Task InitializeAsync(MigrationDestinationDefinition destination, CancellationToken ct)
            => Task.CompletedTask;

        public Task ApplyChangeSetAsync(MigrationChangeSet changeSet, IMigrationProgress progress, CancellationToken ct)
            => Task.CompletedTask;

        public Task FinalizeAsync(CancellationToken ct)
            => Task.CompletedTask;

        public ValueTask DisposeAsync()
            => ValueTask.CompletedTask;
    }

    private sealed class TestRejectingMigrationChangeSetSink : IMigrationChangeSetSink
    {
        public string Name => "Rejecting structured sink";

        public bool CanHandle(MigrationDestinationDefinition destination) => false;

        public Task<ChangeApplicationCapabilities> GetCapabilitiesAsync(MigrationDestinationDefinition destination, CancellationToken ct)
            => Task.FromResult(new ChangeApplicationCapabilities());

        public Task InitializeAsync(MigrationDestinationDefinition destination, CancellationToken ct)
            => Task.CompletedTask;

        public Task ApplyChangeSetAsync(MigrationChangeSet changeSet, IMigrationProgress progress, CancellationToken ct)
            => Task.CompletedTask;

        public Task FinalizeAsync(CancellationToken ct)
            => Task.CompletedTask;

        public ValueTask DisposeAsync()
            => ValueTask.CompletedTask;
    }

    private sealed class TestMaterializingMigrationChangeSetSink : IMigrationChangeSetSink
    {
        public string Name => "Materializing structured sink";

        public bool CanHandle(MigrationDestinationDefinition destination) => true;

        public Task<ChangeApplicationCapabilities> GetCapabilitiesAsync(MigrationDestinationDefinition destination, CancellationToken ct)
            => Task.FromResult(new ChangeApplicationCapabilities
            {
                SupportsMaterializedWorkdirFallback = true
            });

        public Task InitializeAsync(MigrationDestinationDefinition destination, CancellationToken ct)
            => Task.CompletedTask;

        public Task ApplyChangeSetAsync(MigrationChangeSet changeSet, IMigrationProgress progress, CancellationToken ct)
            => Task.CompletedTask;

        public Task FinalizeAsync(CancellationToken ct)
            => Task.CompletedTask;

        public ValueTask DisposeAsync()
            => ValueTask.CompletedTask;
    }

    private static IVersionControlProvider CreateVersionControlProvider(string providerName)
    {
        var provider = NSubstitute.Substitute.For<IVersionControlProvider>();
        provider.Name.Returns(providerName);
        provider.DisposeAsync().Returns(ValueTask.CompletedTask);
        return provider;
    }
}
