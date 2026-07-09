using Microsoft.VisualStudio.TestTools.UnitTesting;
using Workbench.Builder.Host;

namespace Workbench.Builder.Core.Tests.Host;

/// <summary>
/// Verifies command-line parsing for the thin builder host.
/// </summary>
[TestClass]
public class HostCommandLineParserTests
{
    /// <summary>
    /// Verifies that a project path with an explicit output directory is parsed correctly.
    /// </summary>
    [TestMethod]
    public void Parse_ProjectPathAndOutputDirectory_ReturnsExpectedOptions()
    {
        HostCommandLineParser parser = new();

        HostCommandOptions options = parser.Parse(new[] { "sample.csproj", HostArgumentNames.Output, "artifacts" });

        Assert.AreEqual("sample.csproj", options.ProjectFilePath);
        Assert.AreEqual("artifacts", options.OutputDirectory);
        Assert.AreEqual(HostVerbosity.Normal, options.Verbosity);
        Assert.IsFalse(options.ShowHelp);
    }

    /// <summary>
    /// Verifies that a plain project path is parsed correctly.
    /// </summary>
    [TestMethod]
    public void Parse_ProjectPathOnly_ReturnsExpectedOptions()
    {
        HostCommandLineParser parser = new();

        HostCommandOptions options = parser.Parse(new[] { "sample.csproj" });

        Assert.AreEqual("sample.csproj", options.ProjectFilePath);
        Assert.IsNull(options.OutputDirectory);
        Assert.AreEqual(HostVerbosity.Normal, options.Verbosity);
        Assert.IsFalse(options.ShowHelp);
    }

    /// <summary>
    /// Verifies that an explicit verbosity argument is parsed correctly.
    /// </summary>
    [TestMethod]
    public void Parse_ProjectPathAndVerbosity_ReturnsExpectedOptions()
    {
        HostCommandLineParser parser = new();

        HostCommandOptions options = parser.Parse(new[] { "sample.csproj", HostArgumentNames.Verbosity, "detailed" });

        Assert.AreEqual("sample.csproj", options.ProjectFilePath);
        Assert.AreEqual(HostVerbosity.Detailed, options.Verbosity);
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
        Assert.IsNull(options.OutputDirectory);
        Assert.AreEqual(HostVerbosity.Normal, options.Verbosity);
    }
}
