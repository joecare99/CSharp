using AA98_AvlnCodeStudio.Model.Workspaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;

namespace AA98_AvlnCodeStudio.Tests.Engineering;

[TestClass]
public sealed class WorkspaceFileChangeServiceTests
{
    [TestMethod]
    public void PrepareAndApply_RequireConfirmationAndPreserveOriginalSnapshot()
    {
        WithTemporaryDirectory(root =>
        {
            string existingPath = Path.Combine(root, "View.axaml");
            File.WriteAllText(existingPath, "<TextBlock Text=\"Before\" />");
            WorkspaceFileChangeService service = CreateService();
            WorkspaceFileChangeProposal proposal = service.Prepare(
                root,
                [
                    WorkspaceFileChangeRequest.ReplaceExisting(
                        "View.axaml",
                        "<TextBlock Text=\"Before\" />",
                        "<TextBlock Text=\"After\" />"),
                    WorkspaceFileChangeRequest.CreateNew("ViewModel.cs", "partial class ViewModel { }"),
                ]);

            Assert.AreEqual(2, proposal.Changes.Count);
            Assert.AreEqual("<TextBlock Text=\"Before\" />", proposal.Changes[0].OriginalContent);
            Assert.IsFalse(proposal.Changes[0].IsNewFile);
            Assert.IsTrue(proposal.Changes[1].IsNewFile);
            Assert.AreEqual("<TextBlock Text=\"Before\" />", File.ReadAllText(existingPath));
            Assert.IsFalse(File.Exists(Path.Combine(root, "ViewModel.cs")));
            Assert.ThrowsExactly<InvalidOperationException>(() => service.Apply(proposal, confirmed: false));
            Assert.AreEqual("<TextBlock Text=\"Before\" />", File.ReadAllText(existingPath));

            service.Apply(proposal, confirmed: true);

            Assert.AreEqual("<TextBlock Text=\"After\" />", File.ReadAllText(existingPath));
            Assert.AreEqual("partial class ViewModel { }", File.ReadAllText(Path.Combine(root, "ViewModel.cs")));
        });
    }

    [TestMethod]
    public void Apply_RejectsStaleProposalBeforeChangingAnyTarget()
    {
        WithTemporaryDirectory(root =>
        {
            string existingPath = Path.Combine(root, "View.axaml");
            string newPath = Path.Combine(root, "ViewModel.cs");
            File.WriteAllText(existingPath, "before");
            WorkspaceFileChangeService service = CreateService();
            WorkspaceFileChangeProposal proposal = service.Prepare(
                root,
                [
                    WorkspaceFileChangeRequest.ReplaceExisting("View.axaml", "before", "generated"),
                    WorkspaceFileChangeRequest.CreateNew("ViewModel.cs", "new file"),
                ]);
            File.WriteAllText(existingPath, "external edit");

            WorkspaceFileChangeConflictException exception = Assert.ThrowsExactly<WorkspaceFileChangeConflictException>(
                () => service.Apply(proposal, confirmed: true));

            Assert.AreEqual("View.axaml", exception.RelativePath);
            Assert.AreEqual("external edit", File.ReadAllText(existingPath));
            Assert.IsFalse(File.Exists(newPath));
        });
    }

    [TestMethod]
    public void Prepare_EnforcesCreateAndReplacePreconditions()
    {
        WithTemporaryDirectory(root =>
        {
            string existingPath = Path.Combine(root, "View.axaml");
            File.WriteAllText(existingPath, "current");
            WorkspaceFileChangeService service = CreateService();

            WorkspaceFileChangeConflictException createConflict =
                Assert.ThrowsExactly<WorkspaceFileChangeConflictException>(() => service.Prepare(
                    root,
                    [WorkspaceFileChangeRequest.CreateNew("View.axaml", "replacement")]));
            WorkspaceFileChangeConflictException mismatchConflict =
                Assert.ThrowsExactly<WorkspaceFileChangeConflictException>(() => service.Prepare(
                    root,
                    [WorkspaceFileChangeRequest.ReplaceExisting("View.axaml", "stale", "replacement")]));
            WorkspaceFileChangeConflictException missingConflict =
                Assert.ThrowsExactly<WorkspaceFileChangeConflictException>(() => service.Prepare(
                    root,
                    [WorkspaceFileChangeRequest.ReplaceExisting("Missing.axaml", "expected", "replacement")]));

            Assert.AreEqual("View.axaml", createConflict.RelativePath);
            Assert.AreEqual("View.axaml", mismatchConflict.RelativePath);
            Assert.AreEqual("Missing.axaml", missingConflict.RelativePath);
            Assert.AreEqual("current", File.ReadAllText(existingPath));
        });
    }

    [TestMethod]
    public void Apply_PreservesFileCreatedAfterPreparation()
    {
        WithTemporaryDirectory(root =>
        {
            WorkspaceFileChangeService service = CreateService();
            WorkspaceFileChangeProposal proposal = service.Prepare(
                root,
                [WorkspaceFileChangeRequest.CreateNew("ViewModel.cs", "generated")]);
            string path = Path.Combine(root, "ViewModel.cs");
            const string externalContent = "created by another process";
            File.WriteAllText(path, externalContent);

            WorkspaceFileChangeConflictException exception =
                Assert.ThrowsExactly<WorkspaceFileChangeConflictException>(
                    () => service.Apply(proposal, confirmed: true));

            Assert.AreEqual("ViewModel.cs", exception.RelativePath);
            Assert.AreEqual(externalContent, File.ReadAllText(path));
        });
    }

    [TestMethod]
    public void Prepare_RejectsPathsOutsideWorkspace()
    {
        WithTemporaryDirectory(root =>
        {
            WorkspaceFileChangeService service = CreateService();
            string[] paths = ["../outside.cs", "nested/../../outside.cs", "C:\\outside.cs", "/outside.cs"];
            foreach (string relativePath in paths)
            {
                Assert.ThrowsExactly<ArgumentException>(() => service.Prepare(
                    root,
                    [WorkspaceFileChangeRequest.CreateNew(relativePath, "content")]));
            }
        });
    }

    [TestMethod]
    public void Prepare_RejectsDuplicateTargetsAndFileAsDestinationDirectory()
    {
        WithTemporaryDirectory(root =>
        {
            WorkspaceFileChangeService service = CreateService();

            Assert.ThrowsExactly<InvalidDataException>(() => service.Prepare(
                root,
                [
                    WorkspaceFileChangeRequest.CreateNew("View.axaml", "one"),
                    WorkspaceFileChangeRequest.CreateNew("View.axaml", "two"),
                ]));
            File.WriteAllText(Path.Combine(root, "Views"), "not a directory");
            Assert.ThrowsExactly<IOException>(() => service.Prepare(
                root,
                [WorkspaceFileChangeRequest.CreateNew(Path.Combine("Views", "View.axaml"), "content")]));
        });
    }

    [TestMethod]
    public void PrepareAndApply_CreatesMissingSubdirectoriesOnlyAfterConfirmation()
    {
        WithTemporaryDirectory(root =>
        {
            WorkspaceFileChangeService service = CreateService();
            WorkspaceFileChangeProposal proposal = service.Prepare(
                root,
                [
                    WorkspaceFileChangeRequest.CreateNew(Path.Combine("Views", "Login.axaml"), "<Window />"),
                    WorkspaceFileChangeRequest.CreateNew(
                        Path.Combine("ViewModels", "LoginViewModel.cs"),
                        "class LoginViewModel { }"),
                ]);
            string viewsDirectory = Path.Combine(root, "Views");
            string viewModelsDirectory = Path.Combine(root, "ViewModels");

            Assert.IsFalse(Directory.Exists(viewsDirectory));
            Assert.IsFalse(Directory.Exists(viewModelsDirectory));
            Assert.ThrowsExactly<InvalidOperationException>(() => service.Apply(proposal, confirmed: false));
            Assert.IsFalse(Directory.Exists(viewsDirectory));

            service.Apply(proposal, confirmed: true);

            Assert.AreEqual("<Window />", File.ReadAllText(Path.Combine(viewsDirectory, "Login.axaml")));
            Assert.AreEqual(
                "class LoginViewModel { }",
                File.ReadAllText(Path.Combine(viewModelsDirectory, "LoginViewModel.cs")));
        });
    }

    [TestMethod]
    public void Apply_RollsBackCreatedSubdirectoriesWhenOpenDocumentSynchronizationFails()
    {
        WithTemporaryDirectory(root =>
        {
            string viewPath = Path.Combine(root, "View.axaml");
            string generatedPath = Path.Combine(root, "ViewModels", "ViewModel.cs");
            File.WriteAllText(viewPath, "before");
            WorkspaceFileChangeService service = new(new FailingOpenDocumentChangeProvider(viewPath));
            WorkspaceFileChangeProposal proposal = service.Prepare(
                root,
                [
                    WorkspaceFileChangeRequest.ReplaceExisting("View.axaml", "before", "generated"),
                    WorkspaceFileChangeRequest.CreateNew(
                        Path.Combine("ViewModels", "ViewModel.cs"),
                        "class ViewModel { }"),
                ]);

            Assert.ThrowsExactly<InvalidOperationException>(() => service.Apply(proposal, confirmed: true));

            Assert.AreEqual("before", File.ReadAllText(viewPath));
            Assert.IsFalse(File.Exists(generatedPath));
            Assert.IsFalse(Directory.Exists(Path.GetDirectoryName(generatedPath)));
        });
    }

    [TestMethod]
    public void Prepare_RejectsSymbolicLinksThatEscapeWorkspace()
    {
        string parent = Path.Combine(Path.GetTempPath(), $"AvlnWorkspaceLinks-{Guid.NewGuid():N}");
        string root = Path.Combine(parent, "workspace");
        string outside = Path.Combine(parent, "outside");
        Directory.CreateDirectory(root);
        Directory.CreateDirectory(outside);
        try
        {
            try
            {
                Directory.CreateSymbolicLink(Path.Combine(root, "linked"), outside);
            }
            catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or PlatformNotSupportedException)
            {
                Assert.Inconclusive($"The current platform does not allow creating a directory symbolic link: {exception.Message}");
                return;
            }

            WorkspaceFileChangeService service = CreateService();
            Assert.ThrowsExactly<IOException>(() => service.Prepare(
                root,
                [WorkspaceFileChangeRequest.CreateNew(Path.Combine("linked", "Outside.cs"), "content")]));
        }
        finally
        {
            Directory.Delete(parent, recursive: true);
        }
    }

    [TestMethod]
    public void Apply_RejectsDocumentOpenedAfterTheProposalWasPrepared()
    {
        WithTemporaryDirectory(root =>
        {
            string path = Path.Combine(root, "View.axaml");
            File.WriteAllText(path, "before");
            TestOpenDocumentProvider openDocuments = new();
            WorkspaceFileChangeService service = new(openDocuments);
            WorkspaceFileChangeProposal proposal = service.Prepare(
                root,
                [WorkspaceFileChangeRequest.ReplaceExisting("View.axaml", "before", "generated")]);
            openDocuments.Open(path);

            WorkspaceOpenDocumentException exception = Assert.ThrowsExactly<WorkspaceOpenDocumentException>(
                () => service.Apply(proposal, confirmed: true));

            Assert.AreEqual("View.axaml", exception.RelativePath);
            Assert.AreEqual("before", File.ReadAllText(path));
        });
    }

    [TestMethod]
    public void Prepare_RejectsChangesToAlreadyOpenDocuments()
    {
        WithTemporaryDirectory(root =>
        {
            string path = Path.Combine(root, "View.axaml");
            File.WriteAllText(path, "before");
            TestOpenDocumentProvider openDocuments = new();
            openDocuments.Open(path);
            WorkspaceFileChangeService service = new(openDocuments);

            WorkspaceOpenDocumentException exception = Assert.ThrowsExactly<WorkspaceOpenDocumentException>(
                () => service.Prepare(
                    root,
                    [WorkspaceFileChangeRequest.ReplaceExisting("View.axaml", "before", "generated")]));

            Assert.AreEqual("View.axaml", exception.RelativePath);
            Assert.AreEqual("before", File.ReadAllText(path));
        });
    }

    private static WorkspaceFileChangeService CreateService()
    {
        return new WorkspaceFileChangeService(new TestOpenDocumentProvider());
    }

    private static void WithTemporaryDirectory(Action<string> action)
    {
        string root = Path.Combine(Path.GetTempPath(), $"AvlnWorkspaceChanges-{Guid.NewGuid():N}");
        Directory.CreateDirectory(root);
        try
        {
            action(root);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    private sealed class TestOpenDocumentProvider : IWorkspaceOpenDocumentProvider
    {
        private readonly HashSet<string> _openPaths = new(OperatingSystem.IsWindows()
            ? StringComparer.OrdinalIgnoreCase
            : StringComparer.Ordinal);

        public bool IsDocumentOpen(string fullPath) => _openPaths.Contains(Path.GetFullPath(fullPath));

        public void Open(string fullPath) => _openPaths.Add(Path.GetFullPath(fullPath));
    }

    private sealed class FailingOpenDocumentChangeProvider(string openPath)
        : IWorkspaceOpenDocumentChangeProvider
    {
        private readonly string _openPath = Path.GetFullPath(openPath);

        public bool IsDocumentOpen(string fullPath)
        {
            return string.Equals(
                Path.GetFullPath(fullPath),
                _openPath,
                OperatingSystem.IsWindows() ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
        }

        public bool CanApplyChange(string fullPath, string expectedContent) => true;

        public void ApplyChange(string fullPath, string expectedContent, string proposedContent)
        {
            throw new InvalidOperationException("Simulated editor synchronization failure.");
        }
    }
}
