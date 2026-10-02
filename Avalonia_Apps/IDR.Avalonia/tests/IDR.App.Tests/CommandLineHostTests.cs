using IDR.App;
using IDR.Core.Models;
using IDR.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace IDR.App.Tests;

[TestClass]
public sealed class CommandLineHostTests
{
    [TestMethod]
    public void TryParseDelphiVersionAcceptsSupportedNames()
    {
        (string Text, DelphiVersion Expected)[] cases =
        [
            ("7", DelphiVersion.Delphi7),
            ("Delphi2007", DelphiVersion.Delphi2007),
            ("2010", DelphiVersion.Delphi2010),
            ("XE2", DelphiVersion.DelphiXE2),
            ("DelphiXE4", DelphiVersion.DelphiXE4)
        ];
        foreach ((string text, DelphiVersion expected) in cases)
        {
            Assert.IsTrue(CommandLineHost.TryParseDelphiVersion(text, out DelphiVersion actual));
            Assert.AreEqual(expected, actual);
        }
    }

    [TestMethod]
    public void TryParseDelphiVersionRejectsUnsupportedNames()
    {
        foreach (string text in new[] { string.Empty, "5.5", "XE5", "unknown" })
        {
            Assert.IsFalse(CommandLineHost.TryParseDelphiVersion(text, out DelphiVersion actual));
            Assert.AreEqual(DelphiVersion.Unknown, actual);
        }
    }

    [TestMethod]
    public void AnalyzeCommandRequiresInputAndVersion()
    {
        using var services = App.CreateServices();
        CommandLineHost host = new(
            services.GetRequiredService<IPeImageLoader>(),
            services.GetRequiredService<IDelphiVersionDetector>(),
            services.GetRequiredService<IKnowledgeBaseProvider>(),
            services.GetRequiredService<IInstructionDecoder>(),
            services.GetRequiredService<IAnalysisService>());

        var parseResult = host.CreateRootCommand().Parse(["analyze"]);

        Assert.IsTrue(parseResult.Errors.Count >= 2);
    }

    [TestMethod]
    public void AnalyzeCommandAcceptsRequiredOptionsAndSummarySwitch()
    {
        using var services = App.CreateServices();
        CommandLineHost host = new(
            services.GetRequiredService<IPeImageLoader>(),
            services.GetRequiredService<IDelphiVersionDetector>(),
            services.GetRequiredService<IKnowledgeBaseProvider>(),
            services.GetRequiredService<IInstructionDecoder>(),
            services.GetRequiredService<IAnalysisService>());

        var parseResult = host.CreateRootCommand().Parse(
            ["analyze", "--input", "sample.exe", "--version", "7", "--summary-only"]);

        Assert.AreEqual(0, parseResult.Errors.Count);
    }

    [TestMethod]
    public async Task AnalyzeCommandIncludesFormsInFullJsonOutput()
    {
        string inputPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.exe");
        string outputPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.json");
        string summaryPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.json");
        await File.WriteAllBytesAsync(inputPath, []);

        try
        {
            PeImage image = new(inputPath, ReadOnlyMemory<byte>.Empty, [], 0)
            {
                Forms =
                [
                    new DelphiForm(
                        "FORM1",
                        new DelphiFormComponent(
                            "TForm",
                            "Form1",
                            [],
                            [new DelphiFormComponent("TButton", "Button1", [], [])]))
                ]
            };
            using var services = App.CreateServices();
            CommandLineHost host = new(
                new FakePeImageLoader(image),
                new FakeDelphiVersionDetector(),
                new FakeKnowledgeBaseProvider(),
                services.GetRequiredService<IInstructionDecoder>(),
                new FakeAnalysisService());

            int exitCode = await host.RunAsync(
            [
                "analyze",
                "--input", inputPath,
                "--version", "7",
                "--knowledge-base", "unused-kb.bin",
                "--output", outputPath
            ]);

            Assert.AreEqual(0, exitCode);
            using JsonDocument document = JsonDocument.Parse(await File.ReadAllTextAsync(outputPath));
            JsonElement root = document.RootElement;
            Assert.AreEqual(1, root.GetProperty("FormCount").GetInt32());
            JsonElement form = root.GetProperty("Forms").EnumerateArray().Single();
            Assert.AreEqual("FORM1", form.GetProperty("ResourceName").GetString());
            Assert.AreEqual("TForm", form.GetProperty("Root").GetProperty("ClassName").GetString());
            Assert.AreEqual(
                "Button1",
                form.GetProperty("Root").GetProperty("Children")[0].GetProperty("Name").GetString());

            int summaryExitCode = await host.RunAsync(
            [
                "analyze",
                "--input", inputPath,
                "--version", "7",
                "--knowledge-base", "unused-kb.bin",
                "--output", summaryPath,
                "--summary-only"
            ]);

            Assert.AreEqual(0, summaryExitCode);
            using JsonDocument summary = JsonDocument.Parse(await File.ReadAllTextAsync(summaryPath));
            Assert.AreEqual(1, summary.RootElement.GetProperty("FormCount").GetInt32());
            Assert.AreEqual(0, summary.RootElement.GetProperty("Forms").GetArrayLength());
        }
        finally
        {
            if (File.Exists(inputPath))
            {
                File.Delete(inputPath);
            }

            if (File.Exists(outputPath))
            {
                File.Delete(outputPath);
            }

            if (File.Exists(summaryPath))
            {
                File.Delete(summaryPath);
            }
        }
    }

    private sealed class FakePeImageLoader(PeImage image) : IPeImageLoader
    {
        public Task<PeImage> LoadAsync(string sourcePath, CancellationToken cancellationToken) =>
            Task.FromResult(image with { SourcePath = sourcePath });
    }

    private sealed class FakeDelphiVersionDetector : IDelphiVersionDetector
    {
        public DelphiVersionDetection Detect(PeImage image) =>
            new(DelphiVersion.Delphi7, [DelphiVersion.Delphi7], false);
    }

    private sealed class FakeKnowledgeBaseProvider : IKnowledgeBaseProvider
    {
        public Task<IKnowledgeBase> OpenAsync(string filePath, CancellationToken cancellationToken) =>
            Task.FromResult<IKnowledgeBase>(new FakeKnowledgeBase());
    }

    private sealed class FakeKnowledgeBase : IKnowledgeBase
    {
        public uint FormatVersion => 1;

        public IReadOnlyList<KnowledgeBaseModule> Modules => [];

        public IReadOnlyList<KnowledgeBaseProcedure> Procedures => [];

        public ushort GetModuleId(string moduleName) => 0;

        public string? GetModuleName(ushort moduleId) => null;
    }

    private sealed class FakeAnalysisService : IAnalysisService
    {
        public Task<AnalysisResult> AnalyzeAsync(
            AnalysisSession session,
            IProgress<AnalysisProgress>? progress,
            CancellationToken cancellationToken) =>
            Task.FromResult(new AnalysisResult(session, Array.Empty<string>()));
    }
}
