using System;
using AA98_AvlnCodeStudio.Base.Testing.Models;
using AA98_AvlnCodeStudio.Base.Testing.Services;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
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
    public void ResultForSelectedCase_IsAcceptedEvenWithDifferentDisplayName()
    {
        Guid id = Guid.NewGuid();
        TestCase selected = CreateCase(id);
        TestCase returned = CreateCase(id);
        returned.DisplayName = "Adapter formatted result";
        VSTestDiscoveryService.RunHandler handler = new(selected);
        Complete(handler, new TestResult(returned) { Outcome = TestOutcome.Passed });

        TestRunSummary summary = handler.GetResult();
        Assert.AreEqual(TestRunOutcome.Passed, summary.Outcome);
        Assert.AreEqual(id.ToString(), summary.Cases[0].TestId);
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
