using GenInterfaces.Data;
using GenInterfaces.Interfaces.Genealogic.Drivers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace GenInterfaces.Tests.Genealogic.Drivers;

[TestClass]
public class GenDriverContractsTests
{
    private sealed class SampleDriver : IGenImportExportDriver<string>
    {
        public Task<GenDriverResult<string>> ImportAsync(Stream sourceStream, CancellationToken cancellationToken = default)
            => Task.FromResult(GenDriverResult<string>.Successful("imported"));

        public Task<GenDriverResult> ExportAsync(string model, Stream targetStream, CancellationToken cancellationToken = default)
            => Task.FromResult(GenDriverResult.Successful([
                new GenDriverDiagnostic(GenDriverDiagnosticSeverity.Info, $"Exported {model}.", "sample.hej", 12, "DRV001")
            ]));
    }

    [TestMethod]
    public void GenDriverDiagnostic_StoresExpectedMetadata()
    {
        var diagnostic = new GenDriverDiagnostic(
            GenDriverDiagnosticSeverity.Warning,
            "Unknown GEDCOM tag.",
            "sample.ged",
            42,
            "GED001");

        Assert.AreEqual(GenDriverDiagnosticSeverity.Warning, diagnostic.Severity);
        Assert.AreEqual("Unknown GEDCOM tag.", diagnostic.Message);
        Assert.AreEqual("sample.ged", diagnostic.FileContext);
        Assert.AreEqual(42, diagnostic.LineNumber);
        Assert.AreEqual("GED001", diagnostic.Code);
    }

    [TestMethod]
    public void GenDriverDiagnostic_RejectsInvalidLineNumbers()
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
            new GenDriverDiagnostic(GenDriverDiagnosticSeverity.Error, "Invalid line.", lineNumber: 0));
    }

    [TestMethod]
    public void GenDriverResult_DefaultsDiagnosticsToEmptyCollection()
    {
        var result = new GenDriverResult(true);

        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Diagnostics);
        Assert.AreEqual(0, result.Diagnostics.Count);
    }

    [TestMethod]
    public void GenDriverResultOfT_StoresPayloadAndDiagnostics()
    {
        var result = GenDriverResult<string>.Failed(
            payload: "partial",
            diagnostics:
            [
                new GenDriverDiagnostic(GenDriverDiagnosticSeverity.Warning, "Partial import.", "partial.hej", 7)
            ]);

        Assert.IsFalse(result.Success);
        Assert.AreEqual("partial", result.Payload);
        Assert.AreEqual(1, result.Diagnostics.Count);
        Assert.AreEqual(GenDriverDiagnosticSeverity.Warning, result.Diagnostics[0].Severity);
    }

    [TestMethod]
    public async Task ImportExportDriver_SupportsAsynchronousExecution()
    {
        var driver = new SampleDriver();
        using var sourceStream = new MemoryStream();
        using var targetStream = new MemoryStream();

        var importResult = await driver.ImportAsync(sourceStream);
        var exportResult = await driver.ExportAsync(importResult.Payload!, targetStream);

        Assert.IsTrue(importResult.Success);
        Assert.AreEqual("imported", importResult.Payload);
        Assert.IsTrue(exportResult.Success);
        Assert.AreEqual(1, exportResult.Diagnostics.Count);
        Assert.AreEqual("sample.hej", exportResult.Diagnostics[0].FileContext);
    }

    [TestMethod]
    public void GenDriverResultOfT_SerializesStableContractShape()
    {
        var result = GenDriverResult<string>.Successful(
            "payload",
            [
                new GenDriverDiagnostic(GenDriverDiagnosticSeverity.Info, "Imported.", "sample.hej", 3, "HEJ001")
            ]);

        var json = JsonSerializer.Serialize(result);

        Assert.AreEqual("{\"Payload\":\"payload\",\"Success\":true,\"Diagnostics\":[{\"Severity\":1,\"Message\":\"Imported.\",\"FileContext\":\"sample.hej\",\"LineNumber\":3,\"Code\":\"HEJ001\"}]}", json);
    }
}
