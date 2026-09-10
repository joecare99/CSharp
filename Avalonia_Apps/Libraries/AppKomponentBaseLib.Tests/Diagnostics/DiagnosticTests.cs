using AppKomponentBaseLib.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AppKomponentBaseLib.Tests.Diagnostics;

/// <summary>
/// Verifies the shared diagnostic contract used by component adapters.
/// </summary>
[TestClass]
public sealed class DiagnosticTests
{
    [TestMethod]
    public void NewDiagnostic_HasOptionalSourceCoordinates()
    {
        var diagnostic = new Diagnostic
        {
            Code = "CS0001",
            Message = "Compilation failed.",
            Severity = DiagnosticSeverity.Error,
            SourcePath = "source.cs",
            LineNumber = 12,
            ColumnNumber = 8,
        };

        Assert.AreEqual("CS0001", diagnostic.Code);
        Assert.AreEqual("Compilation failed.", diagnostic.Message);
        Assert.AreEqual(DiagnosticSeverity.Error, diagnostic.Severity);
        Assert.AreEqual("source.cs", diagnostic.SourcePath);
        Assert.AreEqual(12, diagnostic.LineNumber);
        Assert.AreEqual(8, diagnostic.ColumnNumber);
    }

    [TestMethod]
    public void NewDiagnostic_AllowsMissingSourceCoordinates()
    {
        var diagnostic = new Diagnostic();

        Assert.IsNull(diagnostic.SourcePath);
        Assert.IsNull(diagnostic.LineNumber);
        Assert.IsNull(diagnostic.ColumnNumber);
    }
}