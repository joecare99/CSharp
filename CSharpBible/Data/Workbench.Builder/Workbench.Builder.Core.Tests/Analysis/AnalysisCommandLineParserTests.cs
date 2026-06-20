using Microsoft.VisualStudio.TestTools.UnitTesting;
using Workbench.Builder.Analysis;
using Workbench.Builder.Core.Models.Inspection;

namespace Workbench.Builder.Core.Tests.Analysis;

/// <summary>
/// Verifies command-line parsing for the analysis host.
/// </summary>
[TestClass]
public class AnalysisCommandLineParserTests
{
    /// <summary>
    /// Verifies that a project path with an explicit JSON format is parsed correctly.
    /// </summary>
    [TestMethod]
    public void Parse_ProjectPathAndJsonFormat_ReturnsExpectedOptions()
    {
        AnalysisCommandLineParser parser = new();

        AnalysisCommandOptions options = parser.Parse(new[] { "sample.csproj", AnalysisArgumentNames.Format, "json" });

        Assert.AreEqual("sample.csproj", options.ProjectFilePath);
        Assert.AreEqual(ProjectInspectionOutputFormat.Json, options.OutputFormat);
        Assert.IsFalse(options.ShowHelp);
    }

    /// <summary>
    /// Verifies that a help argument is detected without requiring a project path.
    /// </summary>
    [TestMethod]
    public void Parse_HelpArgument_ReturnsHelpOption()
    {
        AnalysisCommandLineParser parser = new();

        AnalysisCommandOptions options = parser.Parse(new[] { AnalysisArgumentNames.HelpLong });

        Assert.IsTrue(options.ShowHelp);
        Assert.IsNull(options.ProjectFilePath);
        Assert.AreEqual(ProjectInspectionOutputFormat.PlainText, options.OutputFormat);
    }
}
