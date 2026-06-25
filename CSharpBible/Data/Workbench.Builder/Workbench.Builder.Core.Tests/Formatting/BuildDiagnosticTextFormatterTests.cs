using Microsoft.VisualStudio.TestTools.UnitTesting;
using Workbench.Builder.Core.Models.Diagnostics;
using Workbench.Builder.Core.Services.Formatting;

namespace Workbench.Builder.Core.Tests.Formatting;

/// <summary>
/// Verifies plain-text formatting for individual build diagnostics.
/// </summary>
[TestClass]
public class BuildDiagnosticTextFormatterTests
{
    /// <summary>
    /// Verifies that informational diagnostics are formatted with the info severity label.
    /// </summary>
    [TestMethod]
    public void Format_WhenDiagnosticIsInformationWithoutLocation_ReturnsInfoMessage()
    {
        BuildDiagnostic diagnostic = new(BuildDiagnosticSeverity.Information, "WB1000", "Informational message");

        string output = BuildDiagnosticTextFormatter.Format(diagnostic);

        Assert.AreEqual("info WB1000: Informational message", output);
    }

    /// <summary>
    /// Verifies that a diagnostic without a file path omits the location prefix.
    /// </summary>
    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow(" ")]
    public void Format_WhenFilePathIsMissing_OmitsLocationPrefix(string? filePath)
    {
        BuildDiagnostic diagnostic = new(BuildDiagnosticSeverity.Warning, "WB1001", "Warning message", filePath, 12, 34);

        string output = BuildDiagnosticTextFormatter.Format(diagnostic);

        Assert.AreEqual("warning WB1001: Warning message", output);
    }

    /// <summary>
    /// Verifies that a diagnostic with only a line number emits a line-only location prefix.
    /// </summary>
    [TestMethod]
    public void Format_WhenOnlyLineIsAvailable_ReturnsLineOnlyLocation()
    {
        BuildDiagnostic diagnostic = new(BuildDiagnosticSeverity.Error, "WB1002", "Error message", @"C:\Temp\Sample.csproj", 27);

        string output = BuildDiagnosticTextFormatter.Format(diagnostic);

        Assert.AreEqual(@"C:\Temp\Sample.csproj(27): error WB1002: Error message", output);
    }

    /// <summary>
    /// Verifies that a diagnostic with only a file path emits the file path as the location prefix.
    /// </summary>
    [TestMethod]
    public void Format_WhenOnlyFilePathIsAvailable_ReturnsFilePathLocation()
    {
        BuildDiagnostic diagnostic = new(BuildDiagnosticSeverity.Warning, "WB1003", "Warning message", @"C:\Temp\Sample.csproj");

        string output = BuildDiagnosticTextFormatter.Format(diagnostic);

        Assert.AreEqual(@"C:\Temp\Sample.csproj: warning WB1003: Warning message", output);
    }

    /// <summary>
    /// Verifies that IDE-friendly project warning diagnostics preserve file, line, and column information.
    /// </summary>
    [TestMethod]
    public void Format_WhenDiagnosticHasFileLineAndColumn_ReturnsIdeFriendlyWarningLocation()
    {
        BuildDiagnostic diagnostic = new(BuildDiagnosticSeverity.Warning, "SYSLIB0057", "Target project build: Loading certificate data through the constructor or Import is obsolete.", @"C:\Temp\Program.cs", 214, 15);

        string output = BuildDiagnosticTextFormatter.Format(diagnostic);

        Assert.AreEqual(@"C:\Temp\Program.cs(214,15): warning SYSLIB0057: Target project build: Loading certificate data through the constructor or Import is obsolete.", output);
    }
}
