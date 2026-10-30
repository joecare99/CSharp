using IDR.Cli;
using IDR.Core.Models;
using IDR.Core.Services;
using IDR.Infrastructure.Analysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace IDR.Cli.Tests;

[TestClass]
public sealed class CliTests
{
    [TestMethod]
    public void DelphiVersionParserAcceptsSupportedForms()
    {
        Assert.IsTrue(CommandLineHost.TryParseDelphiVersion("Delphi7", out DelphiVersion version));
        Assert.AreEqual(DelphiVersion.Delphi7, version);
        Assert.IsTrue(CommandLineHost.TryParseDelphiVersion("XE4", out version));
        Assert.AreEqual(DelphiVersion.DelphiXE4, version);
        Assert.IsFalse(CommandLineHost.TryParseDelphiVersion("XE5", out version));
        Assert.AreEqual(DelphiVersion.Unknown, version);
    }

    [TestMethod]
    public void DecompileCommandRequiresInputVersionAndOutputDirectory()
    {
        CommandLineHost host = new(CreateRunner(new FakeAnalysisService()));

        var parseResult = host.CreateRootCommand().Parse(["decompile"]);

        Assert.IsTrue(parseResult.Errors.Count >= 3);
    }

    [TestMethod]
    public async Task DecompileRejectsUnsupportedVersionAndConflictingKnowledgeBaseOptions()
    {
        string root = Path.Combine(Path.GetTempPath(), $"idr-cli-{Guid.NewGuid():N}");
        string inputPath = Path.Combine(root, "sample.exe");
        Directory.CreateDirectory(root);
        await File.WriteAllBytesAsync(inputPath, []);
        try
        {
            CommandLineHost host = new(CreateRunner(new FakeAnalysisService()));
            string[] common =
            [
                "decompile",
                "--input", inputPath,
                "--output-directory", Path.Combine(root, "out")
            ];
            int versionCode = await host.RunAsync([.. common, "--delphi-version", "XE5"]);
            int knowledgeBaseCode = await host.RunAsync(
            [
                .. common,
                "--delphi-version", "7",
                "--knowledge-base", "kb7.bin",
                "--knowledge-base-directory", root
            ]);

            Assert.AreEqual(2, versionCode);
            Assert.AreEqual(2, knowledgeBaseCode);
            Assert.IsFalse(Directory.Exists(Path.Combine(root, "out")));
        }
        finally
        {
            if (Directory.Exists(root))
            {
                Directory.Delete(root, recursive: true);
            }
        }
    }

    [TestMethod]
    public async Task DecompileWritesJsonPascalAndDisassemblyArtifacts()
    {
        string root = Path.Combine(Path.GetTempPath(), $"idr-cli-{Guid.NewGuid():N}");
        string inputPath = Path.Combine(root, "sample.exe");
        string outputPath = Path.Combine(root, "out");
        Directory.CreateDirectory(root);
        await File.WriteAllBytesAsync(inputPath, []);
        try
        {
            CommandLineHost host = new(CreateRunner(new FakeAnalysisService()));
            int exitCode = await host.RunAsync(
            [
                "decompile",
                "--input", inputPath,
                "--delphi-version", "7",
                "--knowledge-base", "fake-kb.bin",
                "--output-directory", outputPath
            ]);

            Assert.AreEqual(0, exitCode);
            CollectionAssert.AreEquivalent(
                new[] { "analysis.json", "classes.pas", "disassembly.asm" },
                Directory.GetFiles(outputPath).Select(Path.GetFileName).ToArray());
            string json = await File.ReadAllTextAsync(Path.Combine(outputPath, "analysis.json"));
            StringAssert.Contains(json, "\"SchemaVersion\": 1");
            string pascal = await File.ReadAllTextAsync(Path.Combine(outputPath, "classes.pas"));
            StringAssert.Contains(pascal, "TButton = class(TControl)");
            StringAssert.Contains(pascal, "Text: Integer;");
            StringAssert.Contains(pascal, "Click;");
            string disassembly = await File.ReadAllTextAsync(Path.Combine(outputPath, "disassembly.asm"));
            StringAssert.Contains(disassembly, "mov eax, 1");

            await File.WriteAllTextAsync(Path.Combine(outputPath, "keep.txt"), "preserve");
            int collisionExitCode = await host.RunAsync(
            [
                "decompile",
                "--input", inputPath,
                "--delphi-version", "7",
                "--knowledge-base", "fake-kb.bin",
                "--output-directory", outputPath
            ]);
            Assert.AreEqual(1, collisionExitCode);
            Assert.IsTrue(File.Exists(Path.Combine(outputPath, "keep.txt")));

            int replaceExitCode = await host.RunAsync(
            [
                "decompile",
                "--input", inputPath,
                "--delphi-version", "7",
                "--knowledge-base", "fake-kb.bin",
                "--output-directory", outputPath,
                "--force"
            ]);
            Assert.AreEqual(0, replaceExitCode);
            Assert.IsFalse(File.Exists(Path.Combine(outputPath, "keep.txt")));

            int summaryExitCode = await host.RunAsync(
            [
                "decompile",
                "--input", inputPath,
                "--delphi-version", "7",
                "--knowledge-base", "fake-kb.bin",
                "--output-directory", outputPath,
                "--summary-only",
                "--force"
            ]);
            Assert.AreEqual(0, summaryExitCode);
            using System.Text.Json.JsonDocument summary = System.Text.Json.JsonDocument.Parse(
                await File.ReadAllTextAsync(Path.Combine(outputPath, "analysis.json")));
            Assert.AreEqual(3, summary.RootElement.GetProperty("ItemCount").GetInt32());
            Assert.AreEqual(0, summary.RootElement.GetProperty("Items").GetArrayLength());
        }
        finally
        {
            if (Directory.Exists(root))
            {
                Directory.Delete(root, recursive: true);
            }
        }
    }

    [TestMethod]
    public async Task DecompileReportsProgressAndCancellationDoesNotPublishArtifacts()
    {
        string root = Path.Combine(Path.GetTempPath(), $"idr-cli-{Guid.NewGuid():N}");
        string inputPath = Path.Combine(root, "sample.exe");
        string outputPath = Path.Combine(root, "out");
        Directory.CreateDirectory(root);
        await File.WriteAllBytesAsync(inputPath, []);
        TextWriter originalError = Console.Error;
        using StringWriter capturedError = new();
        Console.SetError(capturedError);
        try
        {
            CommandLineHost host = new(CreateRunner(new FakeAnalysisService()));
            int exitCode = await host.RunAsync(
            [
                "decompile",
                "--input", inputPath,
                "--delphi-version", "7",
                "--knowledge-base", "fake-kb.bin",
                "--output-directory", outputPath,
                "--progress"
            ]);
            Assert.AreEqual(0, exitCode);
            StringAssert.Contains(capturedError.ToString(), "Analyze\t1\t1");

            string cancelledOutput = Path.Combine(root, "cancelled");
            using CancellationTokenSource cancellation = new();
            cancellation.Cancel();
            int cancelledCode = await host.RunAsync(
            [
                "decompile",
                "--input", inputPath,
                "--delphi-version", "7",
                "--knowledge-base", "fake-kb.bin",
                "--output-directory", cancelledOutput
            ],
            cancellation.Token);
            Assert.AreEqual(130, cancelledCode);
            Assert.IsFalse(Directory.Exists(cancelledOutput));
        }
        finally
        {
            Console.SetError(originalError);
            if (Directory.Exists(root))
            {
                Directory.Delete(root, recursive: true);
            }
        }
    }

    [TestMethod]
    public async Task PascalEmitterSanitizesNamesAndUnknownFieldTypes()
    {
        AnalysisSession session = new(
            "sample.exe",
            ReadOnlyMemory<byte>.Empty,
            imageBase: 0x400000,
            selectedDelphiVersion: DelphiVersion.Delphi7);
        AnalysisItem vmt = session.GetOrAddItem(0x1000);
        vmt.Name = "TUnit.TWidget";
        vmt.SetFlags(AnalysisFlags.Vmt);
        vmt.Fields = [new DelphiVmtField("end", 12, 0x2000)];
        session.GetOrAddItem(0x2000).Name = "Unknown; begin end";

        string text = new PascalClassEmitter().Emit(session);

        StringAssert.Contains(text, "TUnit_TWidget = class(TObject)");
        StringAssert.Contains(text, "&end: Pointer;");
        StringAssert.Contains(text, "TypeInfo RVA 0x00002000");
        Assert.IsFalse(text.Contains("Unknown; begin end", StringComparison.Ordinal));
    }

    [TestMethod]
    public void PascalEmitterDoesNotEmitSelfInheritedRootClass()
    {
        AnalysisSession session = new(
            "sample.exe",
            ReadOnlyMemory<byte>.Empty,
            imageBase: 0x400000,
            selectedDelphiVersion: DelphiVersion.Delphi7);
        AnalysisItem root = session.GetOrAddItem(0x1000);
        root.Name = "TObject";
        root.ParentAddress = root.Address;
        root.SetFlags(AnalysisFlags.Vmt);

        string text = new PascalClassEmitter().Emit(session);

        StringAssert.Contains(text, "TObject = class");
        Assert.IsFalse(text.Contains("TObject = class(TObject)", StringComparison.Ordinal));
    }

    private static AnalysisRunner CreateRunner(IAnalysisService analysisService) =>
        new(
            new FakePeImageLoader(),
            new FakeVersionDetector(),
            new FakeKnowledgeBaseProvider(),
            analysisService);

    private sealed class FakePeImageLoader : IPeImageLoader
    {
        public Task<PeImage> LoadAsync(string sourcePath, CancellationToken cancellationToken) =>
            Task.FromResult(new PeImage(sourcePath, ReadOnlyMemory<byte>.Empty, [], 0)
            {
                ImageBase = 0x400000
            });
    }

    private sealed class FakeVersionDetector : IDelphiVersionDetector
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
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            progress?.Report(new AnalysisProgress("Analyze", 1, 1));
            AnalysisItem parent = session.GetOrAddItem(0x1100);
            parent.Name = "TControl";
            parent.SetFlags(AnalysisFlags.Vmt);
            AnalysisItem fieldType = session.GetOrAddItem(0x2000);
            fieldType.Name = "Integer";
            AnalysisItem type = session.GetOrAddItem(0x1200);
            type.Name = "TButton";
            type.ParentAddress = parent.Address;
            type.ClassInstanceSizeBytes = 32;
            type.Fields = [new DelphiVmtField("Text", 12, fieldType.Address)];
            type.Methods = [new DelphiVmtMethod("TButton.Click", 0x1300)];
            type.SetFlags(AnalysisFlags.Vmt);
            session.AddDisassemblyLine(new DisassemblyLine(
                0x1300,
                "B801000000",
                "Mov",
                "mov eax, 1",
                null,
                InstructionFlowControl.Next));
            return Task.FromResult(new AnalysisResult(session, []));
        }
    }
}
