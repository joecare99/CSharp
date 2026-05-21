using RepoMigrator.Core;

namespace RepoMigrator.Tests;

[TestClass]
public sealed class StructuredChangeModelsTests
{
    [TestMethod]
    public void MigrationChangeSet_Defaults_AreInitialized()
    {
        var changeSet = new MigrationChangeSet();

        Assert.AreEqual(string.Empty, changeSet.ChangeSetId);
        Assert.AreEqual(string.Empty, changeSet.Message);
        Assert.AreEqual(string.Empty, changeSet.AuthorName);
        Assert.IsNull(changeSet.AuthorEmail);
        Assert.AreEqual(default, changeSet.Timestamp);
        Assert.AreEqual(0, changeSet.DirectoryChanges.Count);
        Assert.AreEqual(0, changeSet.FileChanges.Count);
        Assert.AreEqual(0, changeSet.Metadata.Count);
    }

    [TestMethod]
    public void MigrationDirectoryChange_Defaults_AreInitialized()
    {
        var directoryChange = new MigrationDirectoryChange();

        Assert.AreEqual(MigrationDirectoryChangeKind.Rename, directoryChange.Kind);
        Assert.AreEqual(string.Empty, directoryChange.PathBefore);
        Assert.AreEqual(string.Empty, directoryChange.PathAfter);
        Assert.AreEqual(0, directoryChange.AppliedPathRewrites.Count);
        Assert.AreEqual(0, directoryChange.Metadata.Count);
    }

    [TestMethod]
    public void MigrationFileChange_Defaults_AreInitialized()
    {
        var fileChange = new MigrationFileChange();

        Assert.AreEqual(MigrationFileChangeKind.Modify, fileChange.Kind);
        Assert.AreEqual(string.Empty, fileChange.PathBefore);
        Assert.AreEqual(string.Empty, fileChange.PathAfter);
        Assert.IsNull(fileChange.TextChange);
        Assert.IsNull(fileChange.BinaryChange);
        Assert.AreEqual(0, fileChange.AppliedPathRewrites.Count);
        Assert.AreEqual(0, fileChange.Metadata.Count);
    }

    [TestMethod]
    public void MigrationTextChange_Defaults_AreInitialized()
    {
        var textChange = new MigrationTextChange();

        Assert.IsNull(textChange.LineEnding);
        Assert.AreEqual(0, textChange.Hunks.Count);
    }

    [TestMethod]
    public void MigrationTextHunk_Defaults_AreInitialized()
    {
        var hunk = new MigrationTextHunk();

        Assert.AreEqual(0, hunk.OriginalStartLine);
        Assert.AreEqual(0, hunk.ModifiedStartLine);
        Assert.AreEqual(string.Empty, hunk.RemovedText);
        Assert.AreEqual(string.Empty, hunk.AddedText);
    }

    [TestMethod]
    public void MigrationBinaryChange_Defaults_AreInitialized()
    {
        var binaryChange = new MigrationBinaryChange();

        Assert.AreEqual(MigrationBinaryPayloadMode.Inline, binaryChange.PayloadMode);
        Assert.IsNull(binaryChange.InlinePayload);
        Assert.IsNull(binaryChange.PayloadReference);
        Assert.AreEqual(0L, binaryChange.PayloadLength);
        Assert.IsNull(binaryChange.PayloadHash);
        Assert.IsNull(binaryChange.SourceFormatHint);
    }

    [TestMethod]
    public void BinaryPayloadReference_Defaults_AreInitialized()
    {
        var reference = new BinaryPayloadReference();

        Assert.AreEqual(BinaryPayloadReferenceKind.RelativeArtifactPath, reference.Kind);
        Assert.AreEqual(string.Empty, reference.Value);
        Assert.IsTrue(reference.IsPortable);
    }

    [TestMethod]
    public void PathRewriteRule_Defaults_AreInitialized()
    {
        var rule = new PathRewriteRule();

        Assert.AreEqual(string.Empty, rule.FromPrefix);
        Assert.AreEqual(string.Empty, rule.ToPrefix);
        Assert.IsFalse(rule.NormalizeDirectorySeparators);
        Assert.IsFalse(rule.IgnoreCase);
    }

    [TestMethod]
    public void ChangeApplicationCapabilities_Defaults_AreInitialized()
    {
        var capabilities = new ChangeApplicationCapabilities();

        Assert.IsFalse(capabilities.SupportsStructuredChanges);
        Assert.IsFalse(capabilities.SupportsDirectoryChanges);
        Assert.IsFalse(capabilities.SupportsTextChanges);
        Assert.IsFalse(capabilities.SupportsTextHunks);
        Assert.IsFalse(capabilities.SupportsBinaryChanges);
        Assert.IsFalse(capabilities.SupportsBinaryPayloadReferences);
        Assert.IsFalse(capabilities.SupportsPathRewrites);
        Assert.IsFalse(capabilities.SupportsMaterializedWorkdirFallback);
        Assert.AreEqual(0, capabilities.SupportedDirectoryChangeKinds.Count);
        Assert.AreEqual(0, capabilities.SupportedChangeKinds.Count);
        Assert.IsNull(capabilities.MaxInlineBinaryPayloadBytes);
    }

    [TestMethod]
    public void StructuredChangeModels_AssignedValues_ArePreserved()
    {
        var rule = new PathRewriteRule
        {
            FromPrefix = "src/legacy",
            ToPrefix = "src/current",
            NormalizeDirectorySeparators = true,
            IgnoreCase = true
        };
        var binaryReference = new BinaryPayloadReference
        {
            Kind = BinaryPayloadReferenceKind.ProviderOwnedHandle,
            Value = "payload-42",
            IsPortable = false
        };
        var binaryChange = new MigrationBinaryChange
        {
            PayloadMode = MigrationBinaryPayloadMode.FileReference,
            PayloadReference = binaryReference,
            PayloadLength = 102400,
            PayloadHash = "sha256:abc",
            SourceFormatHint = "git-binary"
        };
        var directoryChange = new MigrationDirectoryChange
        {
            Kind = MigrationDirectoryChangeKind.Copy,
            PathBefore = "branches/release-1",
            PathAfter = "branches/release-1-hotfix",
            AppliedPathRewrites = [rule],
            Metadata = new Dictionary<string, string> { [StructuredChangeMetadataKeys.Origin] = "svn-copy" }
        };
        var textChange = new MigrationTextChange
        {
            LineEnding = "\n",
            Hunks =
            [
                new MigrationTextHunk
                {
                    OriginalStartLine = 10,
                    ModifiedStartLine = 10,
                    RemovedText = "old",
                    AddedText = "new"
                }
            ]
        };
        var fileChange = new MigrationFileChange
        {
            Kind = MigrationFileChangeKind.Rename,
            PathBefore = "src/legacy/file.bin",
            PathAfter = "src/current/file.bin",
            TextChange = textChange,
            BinaryChange = binaryChange,
            AppliedPathRewrites = [rule],
            Metadata = new Dictionary<string, string> { [StructuredChangeMetadataKeys.Origin] = "patch" }
        };
        var changeSet = new MigrationChangeSet
        {
            ChangeSetId = "c42",
            Message = "Normalize rename",
            AuthorName = "Alice",
            AuthorEmail = "alice@example.invalid",
            Timestamp = new DateTimeOffset(2024, 1, 2, 3, 4, 5, TimeSpan.Zero),
            DirectoryChanges = [directoryChange],
            FileChanges = [fileChange],
            Metadata = new Dictionary<string, string> { [StructuredChangeMetadataKeys.SourceKind] = nameof(MigrationSourceKind.ArchiveCollection) }
        };
        var capabilities = new ChangeApplicationCapabilities
        {
            SupportsStructuredChanges = true,
            SupportsDirectoryChanges = true,
            SupportsTextChanges = true,
            SupportsTextHunks = true,
            SupportsBinaryChanges = true,
            SupportsBinaryPayloadReferences = true,
            SupportsPathRewrites = true,
            SupportsMaterializedWorkdirFallback = true,
            SupportedDirectoryChangeKinds = [MigrationDirectoryChangeKind.Copy, MigrationDirectoryChangeKind.Rename],
            SupportedChangeKinds = [MigrationFileChangeKind.Add, MigrationFileChangeKind.Rename],
            MaxInlineBinaryPayloadBytes = 1024
        };

        Assert.AreEqual("c42", changeSet.ChangeSetId);
        Assert.AreEqual("Normalize rename", changeSet.Message);
        Assert.AreEqual("Alice", changeSet.AuthorName);
        Assert.AreEqual("alice@example.invalid", changeSet.AuthorEmail);
        Assert.AreEqual(1, changeSet.DirectoryChanges.Count);
        Assert.AreEqual(1, changeSet.FileChanges.Count);
        Assert.AreEqual(nameof(MigrationSourceKind.ArchiveCollection), changeSet.Metadata[StructuredChangeMetadataKeys.SourceKind]);

        Assert.AreEqual(MigrationDirectoryChangeKind.Copy, directoryChange.Kind);
        Assert.AreEqual("branches/release-1", directoryChange.PathBefore);
        Assert.AreEqual("branches/release-1-hotfix", directoryChange.PathAfter);
        Assert.AreEqual("svn-copy", directoryChange.Metadata[StructuredChangeMetadataKeys.Origin]);
        Assert.AreEqual(1, directoryChange.AppliedPathRewrites.Count);

        Assert.AreEqual(MigrationFileChangeKind.Rename, fileChange.Kind);
        Assert.AreEqual("src/legacy/file.bin", fileChange.PathBefore);
        Assert.AreEqual("src/current/file.bin", fileChange.PathAfter);
        Assert.AreEqual("patch", fileChange.Metadata[StructuredChangeMetadataKeys.Origin]);
        Assert.AreEqual(1, fileChange.AppliedPathRewrites.Count);

        Assert.AreEqual("\n", textChange.LineEnding);
        Assert.AreEqual(1, textChange.Hunks.Count);
        Assert.AreEqual("old", textChange.Hunks[0].RemovedText);
        Assert.AreEqual("new", textChange.Hunks[0].AddedText);

        Assert.AreEqual(MigrationBinaryPayloadMode.FileReference, binaryChange.PayloadMode);
        Assert.AreSame(binaryReference, binaryChange.PayloadReference);
        Assert.AreEqual(102400L, binaryChange.PayloadLength);
        Assert.AreEqual("sha256:abc", binaryChange.PayloadHash);
        Assert.AreEqual("git-binary", binaryChange.SourceFormatHint);

        Assert.AreEqual(BinaryPayloadReferenceKind.ProviderOwnedHandle, binaryReference.Kind);
        Assert.AreEqual("payload-42", binaryReference.Value);
        Assert.IsFalse(binaryReference.IsPortable);

        Assert.AreEqual("src/legacy", rule.FromPrefix);
        Assert.AreEqual("src/current", rule.ToPrefix);
        Assert.IsTrue(rule.NormalizeDirectorySeparators);
        Assert.IsTrue(rule.IgnoreCase);

        Assert.IsTrue(capabilities.SupportsStructuredChanges);
        Assert.IsTrue(capabilities.SupportsDirectoryChanges);
        Assert.IsTrue(capabilities.SupportsBinaryPayloadReferences);
        Assert.IsTrue(capabilities.SupportsMaterializedWorkdirFallback);
        Assert.AreEqual(2, capabilities.SupportedDirectoryChangeKinds.Count);
        Assert.AreEqual(2, capabilities.SupportedChangeKinds.Count);
        Assert.AreEqual(1024, capabilities.MaxInlineBinaryPayloadBytes);
    }
}
