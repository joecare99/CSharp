using IDR.App;
using IDR.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace IDR.App.Tests;

[TestClass]
public sealed class RealEngineIntegrationTests
{
    [TestMethod]
    public async Task AnalyzeCommandRunsRealPeAndKnowledgeBaseThroughEngine()
    {
        string? imagePath = Environment.GetEnvironmentVariable("IDR_INTEGRATION_PE");
        string? knowledgeBasePath = Environment.GetEnvironmentVariable("IDR_INTEGRATION_KB");
        if (string.IsNullOrWhiteSpace(imagePath)
            || string.IsNullOrWhiteSpace(knowledgeBasePath)
            || !File.Exists(imagePath)
            || !File.Exists(knowledgeBasePath))
        {
            Assert.Inconclusive(
                "Set IDR_INTEGRATION_PE and IDR_INTEGRATION_KB to run the real-image engine integration test.");
        }

        string version = Environment.GetEnvironmentVariable("IDR_INTEGRATION_DELPHI_VERSION") ?? "7";
        string outputPath = Path.Combine(
            Path.GetTempPath(),
            $"idr-analysis-{Guid.NewGuid():N}.json");
        try
        {
            using var services = App.CreateServices();
            CommandLineHost host = new(
                services.GetRequiredService<IPeImageLoader>(),
                services.GetRequiredService<IDelphiVersionDetector>(),
                services.GetRequiredService<IKnowledgeBaseProvider>(),
                services.GetRequiredService<IInstructionDecoder>(),
                services.GetRequiredService<IAnalysisService>());

            int exitCode = await host.RunAsync(
                [
                    "analyze",
                    "--input", imagePath,
                    "--delphi-version", version,
                    "--knowledge-base", knowledgeBasePath,
                    "--output", outputPath
                ]);

            Assert.AreEqual(0, exitCode);
            using JsonDocument document = JsonDocument.Parse(await File.ReadAllTextAsync(outputPath));
            JsonElement root = document.RootElement;
            Assert.AreEqual(1, root.GetProperty("SchemaVersion").GetInt32());
            Assert.IsTrue(root.GetProperty("KnowledgeBaseModuleCount").GetInt32() > 0);
            Assert.IsTrue(root.GetProperty("SystemModuleId").GetUInt16() > 0);
            Assert.IsTrue(root.GetProperty("VmtCount").GetInt32() > 0);
            Assert.IsTrue(root.GetProperty("RttiCount").GetInt32() > 0);
            Assert.IsTrue(root.GetProperty("StringCount").GetInt32() > 0);
            Assert.IsTrue(root.GetProperty("DisassemblyLineCount").GetInt32() > 0);
            Assert.IsTrue(root.GetProperty("Items").GetArrayLength() > 0);
            Assert.IsTrue(root.GetProperty("Disassembly").GetArrayLength() > 0);
            Assert.AreEqual(
                root.GetProperty("FormCount").GetInt32(),
                root.GetProperty("Forms").GetArrayLength());
            Assert.IsTrue(root.GetProperty("Forms").EnumerateArray().All(form =>
                form.TryGetProperty("Root", out _)
                && form.TryGetProperty("ResourceName", out _)));
            Assert.IsTrue(root.GetProperty("Items").EnumerateArray().All(item =>
                item.TryGetProperty("RegisterArgumentCandidates", out _)));
            Assert.IsTrue(root.GetProperty("Items").EnumerateArray().All(item =>
                item.TryGetProperty("ReturnTypeCandidate", out _)));
            Assert.IsTrue(root.GetProperty("Items").EnumerateArray().All(item =>
                item.TryGetProperty("DataTypeCandidate", out _)));
            Assert.IsTrue(root.GetProperty("Items").EnumerateArray().All(item =>
                item.TryGetProperty("ResourceStringCandidate", out _)));
            Assert.IsTrue(root.GetProperty("Items").EnumerateArray().All(item =>
                item.TryGetProperty("ThreadVariableCandidate", out _)));
            Assert.IsTrue(root.GetProperty("Items").EnumerateArray().All(item =>
                item.TryGetProperty("StackLocalVariables", out _)));
            Assert.IsTrue(root.GetProperty("Items").EnumerateArray().All(item =>
                item.TryGetProperty("MemberAccessCandidates", out _)));
            Assert.IsTrue(root.GetProperty("Items").EnumerateArray().All(item =>
                item.TryGetProperty("RecordFields", out _)));
            Assert.IsTrue(root.GetProperty("Items").EnumerateArray().All(item =>
                item.TryGetProperty("RecordSizeBytes", out _)));
            Assert.IsTrue(root.GetProperty("Items").EnumerateArray().All(item =>
                item.TryGetProperty("ClassInstanceSizeBytes", out _)));
        }
        finally
        {
            if (File.Exists(outputPath))
            {
                File.Delete(outputPath);
            }
        }
    }
}
