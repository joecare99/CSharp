using NSubstitute;
using TraceAnalysis.Base.Filters;
using TraceAnalysis.Base.Models.Interfaces;

namespace TraceAnalysis.Base.Tests.Filters;

[TestClass]
public class InputFilterSelectorTests
{
    [TestMethod]
    public void Select_WhenManualOverrideMatches_ReturnsManualFilter()
    {
        var selector = new InputFilterSelector();
        var automaticFilter = CreateFilter("AutoFilter", priority: 1, canHandle: true, confidence: 100, isExactExtensionMatch: true);
        var manualFilter = CreateFilter("ManualFilter", priority: 1, canHandle: true, confidence: 1, isExactExtensionMatch: false);

        var sourceDescriptor = new FilterSourceDescriptor(
            sourceId: "sample",
            suggestedExtension: ".csv",
            manualFilterId: "ManualFilter");

        var result = selector.Select(new[] { automaticFilter, manualFilter }, CreateSeekableStream(), sourceDescriptor);

        Assert.IsNotNull(result.SelectedFilter);
        Assert.AreEqual("ManualFilter", result.SelectedFilter.FilterId);
    }

    [TestMethod]
    public void Select_WhenManualOverrideCannotHandle_UsesDeterministicRanking()
    {
        var selector = new InputFilterSelector();
        var automaticFilter = CreateFilter("AutoFilter", priority: 1, canHandle: true, confidence: 50, isExactExtensionMatch: true);
        var manualFilter = CreateFilter("ManualFilter", priority: 1, canHandle: false, confidence: 200, isExactExtensionMatch: true);

        var sourceDescriptor = new FilterSourceDescriptor(
            sourceId: "sample",
            suggestedExtension: ".csv",
            manualFilterId: "ManualFilter");

        var result = selector.Select(new[] { automaticFilter, manualFilter }, CreateSeekableStream(), sourceDescriptor);

        Assert.IsNotNull(result.SelectedFilter);
        Assert.AreEqual("AutoFilter", result.SelectedFilter.FilterId);
    }

    [DataTestMethod]
    [DataRow(90, 70, "FilterA")]
    [DataRow(10, 40, "FilterB")]
    public void Select_WhenConfidenceDiffers_ChoosesHigherConfidence(int filterAConfidence, int filterBConfidence, string expectedFilterId)
    {
        var selector = new InputFilterSelector();
        var filterA = CreateFilter("FilterA", priority: 1, canHandle: true, confidence: filterAConfidence, isExactExtensionMatch: true);
        var filterB = CreateFilter("FilterB", priority: 1, canHandle: true, confidence: filterBConfidence, isExactExtensionMatch: true);

        var result = selector.Select(new[] { filterA, filterB }, CreateSeekableStream(), new FilterSourceDescriptor("sample", ".csv"));

        Assert.IsNotNull(result.SelectedFilter);
        Assert.AreEqual(expectedFilterId, result.SelectedFilter.FilterId);
    }

    [TestMethod]
    public void Select_WhenConfidenceTies_ChoosesExactExtensionMatch()
    {
        var selector = new InputFilterSelector();
        var exactMatchFilter = CreateFilter("ExactFilter", priority: 1, canHandle: true, confidence: 100, isExactExtensionMatch: true);
        var nonExactMatchFilter = CreateFilter("NonExactFilter", priority: 10, canHandle: true, confidence: 100, isExactExtensionMatch: false);

        var result = selector.Select(new[] { nonExactMatchFilter, exactMatchFilter }, CreateSeekableStream(), new FilterSourceDescriptor("sample", ".csv"));

        Assert.IsNotNull(result.SelectedFilter);
        Assert.AreEqual("ExactFilter", result.SelectedFilter.FilterId);
    }

    [TestMethod]
    public void Select_WhenConfidenceAndExtensionTie_ChoosesHigherPriority()
    {
        var selector = new InputFilterSelector();
        var lowPriorityFilter = CreateFilter("LowPriority", priority: 1, canHandle: true, confidence: 100, isExactExtensionMatch: true);
        var highPriorityFilter = CreateFilter("HighPriority", priority: 10, canHandle: true, confidence: 100, isExactExtensionMatch: true);

        var result = selector.Select(new[] { lowPriorityFilter, highPriorityFilter }, CreateSeekableStream(), new FilterSourceDescriptor("sample", ".csv"));

        Assert.IsNotNull(result.SelectedFilter);
        Assert.AreEqual("HighPriority", result.SelectedFilter.FilterId);
    }

    [TestMethod]
    public void Select_WhenAllRankingDimensionsTie_ChoosesStableFilterIdOrder()
    {
        var selector = new InputFilterSelector();
        var filterB = CreateFilter("FilterB", priority: 1, canHandle: true, confidence: 100, isExactExtensionMatch: true);
        var filterA = CreateFilter("FilterA", priority: 1, canHandle: true, confidence: 100, isExactExtensionMatch: true);

        var result = selector.Select(new[] { filterB, filterA }, CreateSeekableStream(), new FilterSourceDescriptor("sample", ".csv"));

        Assert.IsNotNull(result.SelectedFilter);
        Assert.AreEqual("FilterA", result.SelectedFilter.FilterId);
    }

    [TestMethod]
    public void Select_WhenNoFilterMatches_ReturnsNullSelectionAndAllAnalyses()
    {
        var selector = new InputFilterSelector();
        var filterA = CreateFilter("FilterA", priority: 1, canHandle: false, confidence: 0, isExactExtensionMatch: false, decisionLines: ["A"]);
        var filterB = CreateFilter("FilterB", priority: 1, canHandle: false, confidence: 0, isExactExtensionMatch: false, decisionLines: ["B"]);

        var result = selector.Select(new[] { filterA, filterB }, CreateSeekableStream(), new FilterSourceDescriptor("sample", ".csv"));

        Assert.IsNull(result.SelectedFilter);
        Assert.AreEqual(2, result.Analyses.Count);
    }

    [TestMethod]
    public void Select_WithNonSeekableStream_PerformsAnalysisAndReturnsDecisionLines()
    {
        var selector = new InputFilterSelector();
        var filterA = CreateFilter("FilterA", priority: 1, canHandle: true, confidence: 10, isExactExtensionMatch: true, decisionLines: ["HeaderOk", "Delimiter=;" ]);
        var filterB = CreateFilter("FilterB", priority: 1, canHandle: true, confidence: 5, isExactExtensionMatch: true, decisionLines: ["Fallback"]);

        using var stream = new NonSeekableReadStream(System.Text.Encoding.UTF8.GetBytes("timestamp;value\n2024-01-01T00:00:00.000Z;1\n"));
        var result = selector.Select(new[] { filterA, filterB }, stream, new FilterSourceDescriptor("sample", ".csv"));

        Assert.IsNotNull(result.SelectedFilter);
        Assert.AreEqual("FilterA", result.SelectedFilter.FilterId);
        CollectionAssert.Contains(result.Analyses[0].DecisionLines.ToList(), "HeaderOk");
    }

    private static IAnalyzableInputFilter CreateFilter(
        string filterId,
        int priority,
        bool canHandle,
        int confidence,
        bool isExactExtensionMatch,
        IEnumerable<string>? decisionLines = null)
    {
        var filter = Substitute.For<IAnalyzableInputFilter>();
        filter.FilterId.Returns(filterId);
        filter.Priority.Returns(priority);

        filter.Analyze(Arg.Any<Stream>(), Arg.Any<FilterSourceDescriptor>())
            .Returns(new InputFilterAnalysisResult(
                filterId: filterId,
                canHandle: canHandle,
                confidenceScore: confidence,
                isExactExtensionMatch: isExactExtensionMatch,
                decisionLines: decisionLines));

        filter.CanHandle(Arg.Any<Stream>(), Arg.Any<string>()).Returns(canHandle);
        filter.Read(Arg.Any<Stream>(), Arg.Any<string>()).Returns(Substitute.For<ITraceDataSet>());
        filter.Read(Arg.Any<Stream>(), Arg.Any<FilterSourceDescriptor>()).Returns(Substitute.For<ITraceDataSet>());

        return filter;
    }

    private static Stream CreateSeekableStream()
    {
        return new MemoryStream(System.Text.Encoding.UTF8.GetBytes("timestamp;value\n"));
    }
}
