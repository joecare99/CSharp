using AA98_AvlnCodeStudio.Base.Debugging.Models;
using AA98_AvlnCodeStudio.Base.OS.Models;
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
        Assert.IsTrue(request.IncludeChanges);
    }

    /// <summary>
    /// Verifies that a version control status starts with an empty change collection.
    /// </summary>
    [TestMethod]
    public void VersionControlStatus_StartsWithEmptyChanges()
    {
        var status = new VersionControlStatus();

        Assert.AreEqual(string.Empty, status.RepositoryRootPath);
        Assert.IsFalse(status.HasLocalChanges);
        Assert.AreEqual(0, status.Changes.Count);
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