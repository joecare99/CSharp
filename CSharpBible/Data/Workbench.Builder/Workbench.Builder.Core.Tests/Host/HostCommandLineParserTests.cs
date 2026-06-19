using Microsoft.VisualStudio.TestTools.UnitTesting;
using Workbench.Builder.Core.Models.Inspection;
using Workbench.Builder.Host;

namespace Workbench.Builder.Core.Tests.Host;

/// <summary>
/// Verifies command-line parsing for the thin builder host.
/// </summary>
[TestClass]
public class HostCommandLineParserTests
{
    /// <summary>
    /// Verifies that a project path with an explicit JSON format is parsed correctly.
    /// </summary>
    [TestMethod]
    public void Parse_ProjectPathAndJsonFormat_ReturnsExpectedOptions()
    {
        HostCommandLineParser parser = new();

        HostCommandOptions options = parser.Parse(new[] { "sample.csproj", HostArgumentNames.Format, "json" });

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
        HostCommandLineParser parser = new();

        HostCommandOptions options = parser.Parse(new[] { HostArgumentNames.HelpLong });

        Assert.IsTrue(options.ShowHelp);
        Assert.IsNull(options.ProjectFilePath);
        Assert.AreEqual(ProjectInspectionOutputFormat.PlainText, options.OutputFormat);
    }
}
