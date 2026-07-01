using AA98_AvlnCodeStudio.Base.Debugging.Models;
using AppKomponentBaseLib.Diagnostics;
using AA98_AvlnCodeStudio.Base.OS.Models;
using AA98_AvlnCodeStudio.Base.Planning.Models;
using AA98_AvlnCodeStudio.Base.Testing.Models;
using AA98_AvlnCodeStudio.Base.Versioning.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AA98_AvlnCodeStudio.Tests.Engineering;

/// <summary>
/// Verifies baseline defaults for the shared engineering foundation models.
/// </summary>
[TestClass]
public class EngineeringFoundationModelTests
{
    /// <summary>
    /// Verifies that a version control status request includes changes by default.
    /// </summary>
    [TestMethod]
    public void VersionControlStatusRequest_UsesExpectedDefaults()
    {
        var request = new VersionControlStatusRequest();

        Assert.AreEqual(string.Empty, request.RepositoryRootPath);
        Assert.IsNull(request.RepositoryContextPath);
        Assert.IsTrue(request.IncludeChanges);
        Assert.IsTrue(request.IncludeCapabilities);
    }

    /// <summary>
    /// Verifies that a version control status starts with an empty change collection.
    /// </summary>
    [TestMethod]
    public void VersionControlStatus_StartsWithEmptyChanges()
    {
        var status = new VersionControlStatus();

        Assert.AreEqual(string.Empty, status.RepositoryRootPath);
        Assert.IsNull(status.RepositoryName);
        Assert.IsNull(status.ActiveReferenceName);
        Assert.AreEqual(VersionControlReferenceKind.Unknown, status.ActiveReferenceKind);
        Assert.IsFalse(status.IsRepositoryRootDiscovered);
        Assert.IsFalse(status.HasLocalChanges);
        Assert.IsFalse(status.IsDetached);
        Assert.AreEqual(0, status.Capabilities.Count);
        Assert.AreEqual(0, status.Changes.Count);
    }

    /// <summary>
    /// Verifies that a version control change summary starts with neutral local-state defaults.
    /// </summary>
    [TestMethod]
    public void VersionControlChangeSummary_UsesExpectedDefaults()
    {
        var summary = new VersionControlChangeSummary();

        Assert.AreEqual(string.Empty, summary.Path);
        Assert.IsNull(summary.PreviousPath);
        Assert.IsFalse(summary.IsStaged);
        Assert.IsFalse(summary.IsIgnored);
        Assert.AreEqual(VersionControlChangeKind.Unknown, summary.ChangeKind);
    }

    /// <summary>
    /// Verifies that a planning item starts with neutral local-model defaults.
    /// </summary>
    [TestMethod]
    public void PlanningItem_UsesExpectedDefaults()
    {
        var item = new PlanningItem();

        Assert.AreEqual(string.Empty, item.Id);
        Assert.AreEqual(string.Empty, item.Title);
        Assert.AreEqual(PlanningItemKind.Unknown, item.Kind);
        Assert.AreEqual(PlanningItemStatus.Unknown, item.Status);
        Assert.AreEqual(string.Empty, item.SourcePath);
        Assert.IsNull(item.Parent);
        Assert.AreEqual(0, item.RelatedParents.Count);
        Assert.AreEqual(0, item.Children.Count);
        Assert.AreEqual(0, item.Diagnostics.Count);
    }

    /// <summary>
    /// Verifies that a planning item link starts with neutral reference defaults.
    /// </summary>
    [TestMethod]
    public void PlanningItemLink_UsesExpectedDefaults()
    {
        var link = new PlanningItemLink();

        Assert.AreEqual(string.Empty, link.ItemId);
        Assert.IsNull(link.Title);
        Assert.AreEqual(PlanningItemKind.Unknown, link.Kind);
        Assert.IsNull(link.SourcePath);
    }

    /// <summary>
    /// Verifies that a diagnostic starts with neutral defaults.
    /// </summary>
    [TestMethod]
    public void Diagnostic_UsesExpectedDefaults()
    {
        var diagnostic = new Diagnostic();

        Assert.AreEqual(string.Empty, diagnostic.Code);
        Assert.AreEqual(string.Empty, diagnostic.Message);
        Assert.AreEqual(DiagnosticSeverity.Unknown, diagnostic.Severity);
        Assert.IsNull(diagnostic.SourcePath);
        Assert.IsNull(diagnostic.LineNumber);
    }

    /// <summary>
    /// Verifies that a test run request starts without targets and without coverage collection.
    /// </summary>
    [TestMethod]
    public void TestRunRequest_UsesExpectedDefaults()
    {
        var request = new TestRunRequest();

        Assert.IsNull(request.WorkspaceRootPath);
        Assert.IsNull(request.ProjectPath);
        Assert.IsNull(request.TargetFramework);
        Assert.AreEqual(0, request.Targets.Count);
        Assert.IsFalse(request.CollectCoverage);
    }

    /// <summary>
    /// Verifies that a test run summary starts with an unknown outcome and zero counters.
    /// </summary>
    [TestMethod]
    public void TestRunSummary_UsesExpectedDefaults()
    {
        var summary = new TestRunSummary();

        Assert.AreEqual(TestRunOutcome.Unknown, summary.Outcome);
        Assert.AreEqual(0, summary.TotalCount);
        Assert.AreEqual(0, summary.PassedCount);
        Assert.AreEqual(0, summary.FailedCount);
        Assert.AreEqual(0, summary.SkippedCount);
    }

    /// <summary>
    /// Verifies that a debug launch request starts detached and without arguments.
    /// </summary>
    [TestMethod]
    public void DebugLaunchRequest_UsesExpectedDefaults()
    {
        var request = new DebugLaunchRequest();

        Assert.IsNull(request.Target);
        Assert.AreEqual(0, request.Arguments.Count);
        Assert.IsNull(request.WorkingDirectory);
        Assert.IsFalse(request.AttachToExistingProcess);
    }

    /// <summary>
    /// Verifies that a debug session info starts with an unknown state.
    /// </summary>
    [TestMethod]
    public void DebugSessionInfo_UsesExpectedDefaults()
    {
        var session = new DebugSessionInfo();

        Assert.AreEqual(string.Empty, session.SessionId);
        Assert.IsNull(session.DisplayName);
        Assert.AreEqual(DebugSessionState.Unknown, session.State);
    }

    /// <summary>
    /// Verifies that a terminal session start request starts with empty optional values.
    /// </summary>
    [TestMethod]
    public void TerminalSessionStartRequest_UsesExpectedDefaults()
    {
        var request = new TerminalSessionStartRequest();

        Assert.IsNull(request.WorkspaceRootPath);
        Assert.IsNull(request.WorkingDirectory);
        Assert.IsNull(request.ShellPath);
        Assert.IsNull(request.ShellDisplayName);
        Assert.AreEqual(0, request.Arguments.Count);
        Assert.AreEqual(0, request.EnvironmentVariables.Count);
    }

    /// <summary>
    /// Verifies that a terminal shell descriptor starts with empty optional values.
    /// </summary>
    [TestMethod]
    public void TerminalShellDescriptor_UsesExpectedDefaults()
    {
        var descriptor = new TerminalShellDescriptor();

        Assert.IsNull(descriptor.DisplayName);
        Assert.IsNull(descriptor.ExecutablePath);
        Assert.AreEqual(0, descriptor.Arguments.Count);
        Assert.IsFalse(descriptor.IsFallback);
    }
}