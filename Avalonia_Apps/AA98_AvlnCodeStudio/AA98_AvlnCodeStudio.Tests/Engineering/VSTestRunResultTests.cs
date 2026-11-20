using System;
using System.Linq;
using System.Threading.Tasks;
using AA98_AvlnCodeStudio.Base.Testing.Models;
using AA98_AvlnCodeStudio.Base.Testing.Services;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestResult = Microsoft.VisualStudio.TestPlatform.ObjectModel.TestResult;

namespace AA98_AvlnCodeStudio.Tests.Engineering;

[TestClass]
public sealed class VSTestRunResultTests
{
    [TestMethod]
    public void ResultForDifferentParameterizedCase_IsRejected()
    {
        TestCase selected = CreateCase(Guid.NewGuid());
        TestCase other = CreateCase(Guid.NewGuid());
        VSTestDiscoveryService.RunHandler handler = new(selected);
        Complete(handler, new TestResult(other) { Outcome = TestOutcome.Passed });

        InvalidOperationException error = Assert.ThrowsExactly<InvalidOperationException>(() => handler.GetResult());
        StringAssert.Contains(error.Message, "different adapter case");
    }

    [TestMethod]
    [DataRow("executor://other", "sample.dll", "Example.Theory")]
    [DataRow("executor://example", "other.dll", "Example.Theory")]
    [DataRow("executor://example", "sample.dll", "Example.Other")]
    public void ResultWithSameIdButDifferentIdentity_IsRejected(string executor, string source, string name)
    {
        Guid id = Guid.NewGuid();
        TestCase selected = CreateCase(id);
        TestCase other = new(name, new Uri(executor), source) { Id = id };
        VSTestDiscoveryService.RunHandler handler = new(selected);
        Complete(handler, new TestResult(other) { Outcome = TestOutcome.Passed });

        InvalidOperationException error = Assert.ThrowsExactly<InvalidOperationException>(() => handler.GetResult());
        StringAssert.Contains(error.Message, "different adapter case");
    }

    [TestMethod]
    public void ResultForSelectedCase_IsAcceptedEvenWithDifferentDisplayName()
    {
        Guid id = Guid.NewGuid();
        TestCase selected = CreateCase(id);
        TestCase returned = CreateCase(id);
        returned.DisplayName = "Adapter formatted result";
        VSTestDiscoveryService.RunHandler handler = new(selected);
        handler.HandleLogMessage(TestMessageLevel.Warning, "Adapter warning.");
        handler.HandleLogMessage(TestMessageLevel.Warning, null);
        Complete(handler, new TestResult(returned) { Outcome = TestOutcome.Passed });

        TestRunSummary summary = handler.GetResult();
        Assert.AreEqual(TestRunOutcome.Passed, summary.Outcome);
        Assert.AreEqual(id.ToString(), summary.Cases[0].TestId);
    }

    [TestMethod]
    [DataRow(TestOutcome.Passed)]
    [DataRow(TestOutcome.Failed)]
    [DataRow(TestOutcome.Skipped)]
    public void ErrorReportedDuringCaseExecution_IsRejected(TestOutcome outcome)
    {
        TestCase selected = CreateCase(Guid.NewGuid());
        VSTestDiscoveryService.RunHandler handler = new(selected);
        handler.HandleLogMessage(TestMessageLevel.Error, "Adapter failed after reporting a result.");
        Complete(handler, new TestResult(selected) { Outcome = outcome });

        InvalidOperationException error = Assert.ThrowsExactly<InvalidOperationException>(() => handler.GetResult());
        StringAssert.Contains(error.Message, "Adapter failed after reporting a result.");
    }

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow(" \t\r\n")]
    public void ErrorWithoutMessageDuringCaseExecution_IsRejected(string? message)
    {
        TestCase selected = CreateCase(Guid.NewGuid());
        VSTestDiscoveryService.RunHandler handler = new(selected);
        handler.HandleLogMessage(TestMessageLevel.Error, message);
        Complete(handler, new TestResult(selected) { Outcome = TestOutcome.Passed });

        InvalidOperationException error = Assert.ThrowsExactly<InvalidOperationException>(() => handler.GetResult());
        StringAssert.Contains(error.Message, "VSTest reported an error without a message.");
    }

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow(" \t\r\n")]
    public void ErrorWithoutMessageDuringDiscovery_IsRejected(string? message)
    {
        VSTestDiscoveryService.DiscoveryHandler handler = new();
        handler.HandleDiscoveredTests([CreateCase(Guid.NewGuid())]);
        handler.HandleLogMessage(TestMessageLevel.Error, message);
        handler.HandleDiscoveryComplete(new DiscoveryCompleteEventArgs(1, false), null);

        InvalidOperationException error = Assert.ThrowsExactly<InvalidOperationException>(() => handler.GetResult());
        StringAssert.Contains(error.Message, "VSTest reported an error without a message.");
    }

    [TestMethod]
    public void DiscoveryWarningsWithoutMessages_AreIgnored()
    {
        VSTestDiscoveryService.DiscoveryHandler handler = new();
        handler.HandleLogMessage(TestMessageLevel.Warning, null);
        handler.HandleLogMessage(TestMessageLevel.Warning, "  ");
        handler.HandleLogMessage(TestMessageLevel.Warning, "Adapter warning.");
        handler.HandleDiscoveryComplete(new DiscoveryCompleteEventArgs(0, false), null);

        CollectionAssert.AreEqual(new[] { "Adapter warning." }, handler.GetResult().Warnings.ToArray());
    }

    [TestMethod]
    public void CanceledRunWithResult_IsRejected()
    {
        TestCase selected = CreateCase(Guid.NewGuid());
        VSTestDiscoveryService.RunHandler handler = new(selected);
        TestRunChangedEventArgs chunk = new(null!, [new TestResult(selected) { Outcome = TestOutcome.Passed }], null!);
        TestRunCompleteEventArgs complete = new(null!, true, false, null!, null!, TimeSpan.Zero);
        handler.HandleTestRunComplete(complete, chunk, null, null);

        Assert.ThrowsExactly<InvalidOperationException>(() => handler.GetResult());
    }

    [TestMethod]
    public async Task RunCaseAsync_ExecutesSelectedMSTestCase()
    {
        string assembly = typeof(VSTestRunResultTests).Assembly.Location;
        DotNetTestDiscoveryService service = new();
        TestDiscoveryResult discovery = await service.DiscoverAsync(assembly, AppContext.BaseDirectory);
        DiscoveredTestCase selected = discovery.Cases.Single(test =>
            test.FullyQualifiedName.EndsWith(
                ".VSTestRunResultTests.ResultForSelectedCase_IsAcceptedEvenWithDifferentDisplayName",
                StringComparison.Ordinal));

        TestRunSummary result = await service.RunCaseAsync(assembly, AppContext.BaseDirectory, selected);

        Assert.AreEqual(1, result.TotalCount);
        Assert.AreEqual(1, result.PassedCount);
        Assert.AreEqual(selected.Id, result.Cases.Single().TestId);
    }

    private static TestCase CreateCase(Guid id) =>
        new("Example.Theory", new Uri("executor://example"), "sample.dll") { Id = id };

    private static void Complete(VSTestDiscoveryService.RunHandler handler, TestResult result)
    {
        TestRunChangedEventArgs chunk = new(null!, [result], null!);
        TestRunCompleteEventArgs complete = new(null!, false, false, null!, null!, TimeSpan.Zero);
        handler.HandleTestRunComplete(complete, chunk, null, null);
    }
}
