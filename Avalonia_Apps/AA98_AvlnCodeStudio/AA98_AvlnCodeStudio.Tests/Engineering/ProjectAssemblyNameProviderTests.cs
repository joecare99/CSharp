using AA98_AvlnCodeStudio.Model.Workspaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Property.Editor;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.Tests.Engineering;

[TestClass]
public sealed class ProjectAssemblyNameProviderTests
{
    [TestMethod]
    public async Task ApplyAsync_UnconditionalLiteral_RequiresReviewAndSupportsWorkspaceUndo()
    {
        await WithTemporaryDirectoryAsync(async root =>
        {
            const string original = "<Project Sdk=\"Microsoft.NET.Sdk\">\r\n  <PropertyGroup>\r\n    <AssemblyName>OldName</AssemblyName>\r\n  </PropertyGroup>\r\n</Project>";
            string projectPath = Path.Combine(root, "Sample.csproj");
            File.WriteAllText(projectPath, original);
            WorkspaceFileChangeService changes = CreateChangeService();
            WorkspaceFileChangeProposal? reviewedProposal = null;
            ProjectAssemblyNameProvider provider = new(
                root,
                projectPath,
                changes,
                (proposal, _) =>
                {
                    reviewedProposal = proposal;
                    return Task.FromResult(true);
                });

            await provider.LoadAsync();
            Assert.IsFalse(provider.IsReadOnly);
            Assert.AreEqual(PropertyScope.Project, provider.Context.Scope);
            Assert.IsTrue(provider.Properties.Single().TrySetValue("NewName"));

            await provider.ApplyAsync();

            string expected = original.Replace("<AssemblyName>OldName</AssemblyName>", "<AssemblyName>NewName</AssemblyName>", StringComparison.Ordinal);
            Assert.AreEqual(expected, File.ReadAllText(projectPath));
            Assert.IsNotNull(reviewedProposal);
            Assert.AreEqual(1, reviewedProposal!.Changes.Count);
            Assert.IsTrue(changes.CanUndo);
            Assert.IsTrue(changes.Undo());
            Assert.AreEqual(original, File.ReadAllText(projectPath));
        });
    }

    [TestMethod]
    public async Task ApplyAsync_PreservesProjectEncodingThroughUndoAndRedo()
    {
        await WithTemporaryDirectoryAsync(async root =>
        {
            const string original = "<Project><PropertyGroup><AssemblyName>Old</AssemblyName></PropertyGroup></Project>";
            const string updated = "<Project><PropertyGroup><AssemblyName>New</AssemblyName></PropertyGroup></Project>";
            Encoding[] encodings =
            [
                new UTF8Encoding(encoderShouldEmitUTF8Identifier: true, throwOnInvalidBytes: true),
                new UnicodeEncoding(bigEndian: false, byteOrderMark: true, throwOnInvalidBytes: true),
                new UTF32Encoding(bigEndian: true, byteOrderMark: true, throwOnInvalidCharacters: true),
            ];
            string projectPath = Path.Combine(root, "Sample.csproj");

            foreach (Encoding encoding in encodings)
            {
                byte[] originalBytes = [.. encoding.GetPreamble(), .. encoding.GetBytes(original)];
                byte[] updatedBytes = [.. encoding.GetPreamble(), .. encoding.GetBytes(updated)];
                await File.WriteAllBytesAsync(projectPath, originalBytes);
                WorkspaceFileChangeService changes = CreateChangeService();
                ProjectAssemblyNameProvider provider = new(root, projectPath, changes, (proposal, _) =>
                {
                    Assert.AreEqual(original, proposal.Changes.Single().OriginalContent);
                    Assert.AreEqual(updated, proposal.Changes.Single().ProposedContent);
                    return Task.FromResult(true);
                });
                await provider.LoadAsync();
                Assert.IsTrue(provider.Properties.Single().TrySetValue("New"));

                await provider.ApplyAsync();

                CollectionAssert.AreEqual(updatedBytes, await File.ReadAllBytesAsync(projectPath), encoding.WebName);
                Assert.IsTrue(changes.Undo());
                CollectionAssert.AreEqual(originalBytes, await File.ReadAllBytesAsync(projectPath), encoding.WebName);
                Assert.IsTrue(changes.Redo());
                CollectionAssert.AreEqual(updatedBytes, await File.ReadAllBytesAsync(projectPath), encoding.WebName);
            }
        });
    }

    [TestMethod]
    public async Task LoadAsync_ConditionalOrDynamicAssemblyName_IsReadOnly()
    {
        await AssertReadOnlyAsync(
            "<Project><PropertyGroup Condition=\"'$(Configuration)' == 'Debug'\"><AssemblyName>Sample</AssemblyName></PropertyGroup></Project>",
            "Conditional");
        await AssertReadOnlyAsync(
            "<Project><PropertyGroup><AssemblyName>$(RootName)</AssemblyName></PropertyGroup></Project>",
            "simple");
        await AssertReadOnlyAsync(
            "<Project><Import Project=\"common.props\"/><PropertyGroup><AssemblyName>Sample</AssemblyName></PropertyGroup></Project>",
            "imports");
    }

    [TestMethod]
    public async Task ApplyAsync_ReviewDeclined_LeavesProjectUnchanged()
    {
        await WithTemporaryDirectoryAsync(async root =>
        {
            const string original = "<Project><PropertyGroup><AssemblyName>Old</AssemblyName></PropertyGroup></Project>";
            string projectPath = Path.Combine(root, "Sample.csproj");
            await File.WriteAllTextAsync(projectPath, original);
            ProjectAssemblyNameProvider provider = new(
                root,
                projectPath,
                CreateChangeService(),
                (_, _) => Task.FromResult(false));
            await provider.LoadAsync();
            Assert.IsTrue(provider.Properties.Single().TrySetValue("New"));

            await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => provider.ApplyAsync());

            Assert.AreEqual(original, await File.ReadAllTextAsync(projectPath));
            StringAssert.Contains(provider.ErrorMessage, "review was declined");
        });
    }

    [TestMethod]
    public async Task ApplyAsync_ExternalFileChangeAfterLoad_IsRejectedBeforeReview()
    {
        await WithTemporaryDirectoryAsync(async root =>
        {
            const string original = "<Project><PropertyGroup><AssemblyName>Old</AssemblyName></PropertyGroup></Project>";
            string projectPath = Path.Combine(root, "Sample.csproj");
            await File.WriteAllTextAsync(projectPath, original);
            bool reviewCalled = false;
            ProjectAssemblyNameProvider provider = new(
                root,
                projectPath,
                CreateChangeService(),
                (_, _) =>
                {
                    reviewCalled = true;
                    return Task.FromResult(true);
                });
            await provider.LoadAsync();
            Assert.IsTrue(provider.Properties.Single().TrySetValue("New"));
            const string externalChange = "<Project><PropertyGroup><AssemblyName>External</AssemblyName></PropertyGroup></Project>";
            await File.WriteAllTextAsync(projectPath, externalChange);

            await Assert.ThrowsExactlyAsync<WorkspaceFileChangeConflictException>(() => provider.ApplyAsync());

            Assert.IsFalse(reviewCalled);
            Assert.AreEqual(externalChange, await File.ReadAllTextAsync(projectPath));
        });
    }

    [TestMethod]
    public async Task ApplyAsync_ExternalChangeDuringReview_IsRejectedWithoutOverwriting()
    {
        await WithTemporaryDirectoryAsync(async root =>
        {
            const string original = "<Project><PropertyGroup><AssemblyName>Old</AssemblyName></PropertyGroup></Project>";
            const string external = "<Project><PropertyGroup><AssemblyName>External</AssemblyName></PropertyGroup></Project>";
            string projectPath = Path.Combine(root, "Sample.csproj");
            await File.WriteAllTextAsync(projectPath, original);
            WorkspaceFileChangeService changes = CreateChangeService();
            ProjectAssemblyNameProvider provider = new(
                root,
                projectPath,
                changes,
                async (_, _) =>
                {
                    await File.WriteAllTextAsync(projectPath, external);
                    return true;
                });
            await provider.LoadAsync();
            Assert.IsTrue(provider.Properties.Single().TrySetValue("New"));

            await Assert.ThrowsExactlyAsync<WorkspaceFileChangeConflictException>(() => provider.ApplyAsync());

            Assert.AreEqual(external, await File.ReadAllTextAsync(projectPath));
            Assert.IsFalse(changes.CanUndo);
        });
    }

    [TestMethod]
    public async Task ApplyAsync_CancellationDuringReview_DoesNotChangeTheProject()
    {
        await WithTemporaryDirectoryAsync(async root =>
        {
            const string original = "<Project><PropertyGroup><AssemblyName>Old</AssemblyName></PropertyGroup></Project>";
            string projectPath = Path.Combine(root, "Sample.csproj");
            await File.WriteAllTextAsync(projectPath, original);
            using CancellationTokenSource cancellation = new();
            WorkspaceFileChangeService changes = CreateChangeService();
            ProjectAssemblyNameProvider provider = new(
                root,
                projectPath,
                changes,
                (_, _) =>
                {
                    cancellation.Cancel();
                    return Task.FromResult(true);
                });
            await provider.LoadAsync();
            Assert.IsTrue(provider.Properties.Single().TrySetValue("New"));

            await Assert.ThrowsExactlyAsync<OperationCanceledException>(() => provider.ApplyAsync(cancellation.Token));

            Assert.AreEqual(original, await File.ReadAllTextAsync(projectPath));
            Assert.IsFalse(changes.CanUndo);
        });
    }

    [TestMethod]
    public async Task ApplyAsync_CancellationAfterWrite_ReportsSuccessAndKeepsTheNewSnapshot()
    {
        await WithTemporaryDirectoryAsync(async root =>
        {
            string projectPath = Path.Combine(root, "Sample.csproj");
            await File.WriteAllTextAsync(projectPath, "<Project><PropertyGroup><AssemblyName>Old</AssemblyName></PropertyGroup></Project>");
            using CancellationTokenSource cancellation = new();
            WorkspaceFileChangeService changes = CreateChangeService();
            changes.HistoryChanged += (_, _) => cancellation.Cancel();
            ProjectAssemblyNameProvider provider = new(root, projectPath, changes, (_, _) => Task.FromResult(true));
            await provider.LoadAsync();
            Assert.IsTrue(provider.Properties.Single().TrySetValue("New"));

            await provider.ApplyAsync(cancellation.Token);

            Assert.IsTrue(cancellation.IsCancellationRequested);
            StringAssert.Contains(await File.ReadAllTextAsync(projectPath), "<AssemblyName>New</AssemblyName>");
            Assert.AreEqual("New", provider.Properties.Single().Value);
            Assert.IsTrue(changes.CanUndo);
            Assert.IsTrue(provider.Properties.Single().TrySetValue("Next"));
            await provider.ApplyAsync();
            StringAssert.Contains(await File.ReadAllTextAsync(projectPath), "<AssemblyName>Next</AssemblyName>");
        });
    }

    [TestMethod]
    public async Task AssemblyNameProperty_InvalidValue_IsRejected()
    {
        await WithTemporaryDirectoryAsync(async root =>
        {
            string projectPath = Path.Combine(root, "Sample.csproj");
            await File.WriteAllTextAsync(projectPath, "<Project><PropertyGroup><AssemblyName>Old</AssemblyName></PropertyGroup></Project>");
            ProjectAssemblyNameProvider provider = new(root, projectPath, CreateChangeService(), (_, _) => Task.FromResult(true));
            await provider.LoadAsync();

            Assert.IsFalse(provider.Properties.Single().TrySetValue("invalid name"));
            Assert.IsFalse(provider.Properties.Single().ValidationResult.IsValid);
        });
    }

    private static async Task AssertReadOnlyAsync(string projectContent, string expectedReason)
    {
        await WithTemporaryDirectoryAsync(async root =>
        {
            string projectPath = Path.Combine(root, "Sample.csproj");
            await File.WriteAllTextAsync(projectPath, projectContent);
            ProjectAssemblyNameProvider provider = new(
                root,
                projectPath,
                CreateChangeService(),
                (_, _) => Task.FromResult(true));

            await provider.LoadAsync();

            Assert.IsTrue(provider.IsReadOnly);
            Assert.IsFalse(provider.Properties.Single().IsEditable);
            StringAssert.Contains(provider.Context.ReadOnlyReason, expectedReason);
        });
    }

    private static WorkspaceFileChangeService CreateChangeService() =>
        new(new NoOpenDocuments());

    private static async Task WithTemporaryDirectoryAsync(Func<string, Task> action)
    {
        string root = Path.Combine(Path.GetTempPath(), "ProjectAssemblyNameProviderTests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        try
        {
            await action(root);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    private sealed class NoOpenDocuments : IWorkspaceOpenDocumentProvider
    {
        public bool IsDocumentOpen(string fullPath) => false;
    }
}
