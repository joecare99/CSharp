using AppKomponentBaseLib.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace Diagnostics.Navigation.Tests;

/// <summary>
/// Verifies diagnostic-to-code-location mapping behavior.
/// </summary>
[TestClass]
public sealed class DiagnosticLocationMapperTests
{
    [TestMethod]
    public void Map_ValidDiagnostic_ReturnsNormalizedLocation()
    {
        var diagnostic = CreateDiagnostic(Path.Combine("C:\\", "src", "Sample.cs"), 12, 8);
        var result = new DiagnosticLocationMapper().Map(diagnostic);

        Assert.IsTrue(result.IsAvailable);
        Assert.AreEqual(DiagnosticLocationMappingStatus.Available, result.Status);
        Assert.AreEqual(Path.GetFullPath(diagnostic.SourcePath!), result.Location!.Path);
        Assert.AreEqual(12, result.Location.Line);
        Assert.AreEqual(8, result.Location.Column);
    }

    [TestMethod]
    public void Map_FileOnlyDiagnostic_ReturnsPathOnlyLocation()
    {
        var diagnostic = CreateDiagnostic(Path.Combine("C:\\", "src", "Sample.cs"), null, null);
        var result = new DiagnosticLocationMapper().Map(diagnostic);

        Assert.IsTrue(result.IsAvailable);
        Assert.AreEqual(result.Location!.Path, Path.GetFullPath(diagnostic.SourcePath!));
        Assert.IsNull(result.Location.Line);
        Assert.IsNull(result.Location.Column);
    }

    [TestMethod]
    public void Map_MissingSourcePath_ReturnsUnavailable()
    {
        var result = new DiagnosticLocationMapper().Map(CreateDiagnostic(null, null, null));

        Assert.IsFalse(result.IsAvailable);
        Assert.AreEqual(DiagnosticLocationMappingStatus.MissingSourcePath, result.Status);
    }

    [TestMethod]
    public void Map_RelativeSourcePath_ReturnsInvalidPath()
    {
        var result = new DiagnosticLocationMapper().Map(CreateDiagnostic("relative\\Sample.cs", 1, 1));

        Assert.IsFalse(result.IsAvailable);
        Assert.AreEqual(DiagnosticLocationMappingStatus.InvalidSourcePath, result.Status);
    }

    [TestMethod]
    [DataRow(0, 1)]
    [DataRow(-1, 1)]
    [DataRow(1, 0)]
    [DataRow(1, -1)]
    public void Map_InvalidCoordinates_ReturnsUnavailable(int line, int column)
    {
        var result = new DiagnosticLocationMapper().Map(
            CreateDiagnostic(Path.Combine("C:\\", "src", "Sample.cs"), line, column));

        Assert.IsFalse(result.IsAvailable);
        Assert.AreEqual(DiagnosticLocationMappingStatus.InvalidCoordinates, result.Status);
    }

    [TestMethod]
    public void Map_DoesNotChangeDiagnosticPayload()
    {
        var diagnostic = CreateDiagnostic(Path.Combine("C:\\", "src", "Sample.cs"), 4, 2);
        var code = diagnostic.Code;
        var message = diagnostic.Message;
        var severity = diagnostic.Severity;
        var sourcePath = diagnostic.SourcePath;
        var line = diagnostic.LineNumber;
        var column = diagnostic.ColumnNumber;

        _ = new DiagnosticLocationMapper().Map(diagnostic);

        Assert.AreEqual(code, diagnostic.Code);
        Assert.AreEqual(message, diagnostic.Message);
        Assert.AreEqual(severity, diagnostic.Severity);
        Assert.AreEqual(sourcePath, diagnostic.SourcePath);
        Assert.AreEqual(line, diagnostic.LineNumber);
        Assert.AreEqual(column, diagnostic.ColumnNumber);
    }

    private static Diagnostic CreateDiagnostic(string? sourcePath, int? line, int? column)
    {
        return new Diagnostic
        {
            Code = "DIAG001",
            Message = "Diagnostic message",
            Severity = DiagnosticSeverity.Warning,
            SourcePath = sourcePath,
            LineNumber = line,
            ColumnNumber = column,
        };
    }
}
