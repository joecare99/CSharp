using AA98_AvlnCodeStudio.Model.Workspaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Property.Editor;
using System;
using System.IO;
using System.Linq;
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
