using IDR.App.Services;
using IDR.App.ViewModels;
using IDR.Core.Models;
using IDR.Core.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace IDR.App.Tests;

[TestClass]
public sealed class MainWindowViewModelTests
{
    [TestMethod]
    public async Task OpenCommandLoadsAndDisplaysAnalysis()
    {
        FakeFileSelectionService fileSelection = new("sample.exe");
        FakePeImageLoader imageLoader = new(CreateImage());
        MainWindowViewModel viewModel = new(
            fileSelection,
            imageLoader,
            new FakeVersionDetector(),
            new FakeKnowledgeBasePathResolver(),
            new FakeKnowledgeBaseProvider(),
            new FakeAnalysisService());

        await viewModel.OpenCommand.ExecuteAsync(null);

        Assert.AreEqual("sample.exe", viewModel.SourcePath);
        Assert.AreEqual("Delphi version: Delphi2009", viewModel.DetectedVersionText);
        Assert.AreEqual("Knowledge Base: 1 modules", viewModel.KnowledgeBaseText);
        Assert.AreEqual("Analysis complete", viewModel.StatusText);
        Assert.AreEqual(1, viewModel.Items.Count);
        Assert.AreEqual(1, imageLoader.LoadCount);
    }

    [TestMethod]
    public async Task OpenCommandDoesNothingWhenSelectionIsCanceled()
    {
        MainWindowViewModel viewModel = new(
            new FakeFileSelectionService(null),
            new FakePeImageLoader(CreateImage()),
            new FakeVersionDetector(),
            new FakeKnowledgeBasePathResolver(),
            new FakeKnowledgeBaseProvider(),
            new FakeAnalysisService());

        await viewModel.OpenCommand.ExecuteAsync(null);

        Assert.AreEqual(string.Empty, viewModel.SourcePath);
        Assert.AreEqual("Ready", viewModel.StatusText);
        Assert.AreEqual(0, viewModel.Items.Count);
    }

    [TestMethod]
    public async Task OpenCommandReportsMissingKnowledgeBaseWithoutHidingPartialAnalysis()
    {
        FakeKnowledgeBaseProvider knowledgeBaseProvider = new(throwFileNotFound: true);
        MainWindowViewModel viewModel = new(
            new FakeFileSelectionService("sample.exe"),
            new FakePeImageLoader(CreateImage()),
            new FakeVersionDetector(),
            new FakeKnowledgeBasePathResolver(),
            knowledgeBaseProvider,
            new FakeAnalysisService());

        await viewModel.OpenCommand.ExecuteAsync(null);

        Assert.AreEqual("Knowledge Base not found", viewModel.KnowledgeBaseText);
        StringAssert.Contains(viewModel.DiagnosticText, "Knowledge Base file not found");
        Assert.AreEqual("Analysis completed with diagnostics", viewModel.StatusText);
        Assert.AreEqual(1, viewModel.Items.Count);
    }

    [TestMethod]
    public async Task AmbiguousVersionCanBeSelectedAndLoadsMatchingKnowledgeBase()
    {
        FakeKnowledgeBasePathResolver pathResolver = new();
        FakeKnowledgeBaseProvider provider = new();
        FakeAnalysisService analysisService = new();
        MainWindowViewModel viewModel = new(
            new FakeFileSelectionService("sample.exe"),
            new FakePeImageLoader(CreateImage()),
            new FakeVersionDetector(new DelphiVersionDetection(
                DelphiVersion.Unknown,
                [DelphiVersion.Delphi2006, DelphiVersion.Delphi2007],
                true)),
            pathResolver,
            provider,
            analysisService);

        await viewModel.OpenCommand.ExecuteAsync(null);
        Assert.AreEqual(2, viewModel.VersionOptions.Count);
        Assert.IsNull(viewModel.SelectedVersion);
        Assert.IsFalse(viewModel.LoadKnowledgeBaseCommand.CanExecute(null));

        viewModel.SelectedVersion = viewModel.VersionOptions[1];
        Assert.IsTrue(viewModel.LoadKnowledgeBaseCommand.CanExecute(null));
        await viewModel.LoadKnowledgeBaseCommand.ExecuteAsync(null);

        Assert.AreEqual("kb2007.bin", provider.OpenedPath);
        Assert.AreEqual("Delphi version: Delphi2007", viewModel.DetectedVersionText);
        Assert.AreEqual("Knowledge Base: 1 modules", viewModel.KnowledgeBaseText);
        Assert.IsTrue(analysisService.LastSession?.HasKnowledgeBase);
        Assert.AreEqual(DelphiVersion.Delphi2007, analysisService.LastSession?.SelectedDelphiVersion);
        Assert.AreEqual(1, analysisService.LastSession?.KnowledgeBaseModules.Count);
    }

    [TestMethod]
    public async Task DisassemblyBranchCommandSelectsTargetInstruction()
    {
        MainWindowViewModel viewModel = new(
            new FakeFileSelectionService("sample.exe"),
            new FakePeImageLoader(CreateImage()),
            new FakeVersionDetector(),
            new FakeKnowledgeBasePathResolver(),
            new FakeKnowledgeBaseProvider(),
            new FakeAnalysisService());

        await viewModel.OpenCommand.ExecuteAsync(null);

        Assert.AreEqual(2, viewModel.DisassemblyLines.Count);
        Assert.AreEqual(0u, viewModel.SelectedDisassemblyLine?.Address);
        Assert.IsTrue(viewModel.FollowBranchCommand.CanExecute(null));
        viewModel.FollowBranchCommand.Execute(null);
        Assert.AreEqual(1u, viewModel.SelectedDisassemblyLine?.Address);
    }

    [TestMethod]
    public async Task AnalysisItemCommandNavigatesToCrossReferenceTarget()
    {
        MainWindowViewModel viewModel = new(
            new FakeFileSelectionService("sample.exe"),
            new FakePeImageLoader(CreateImage()),
            new FakeVersionDetector(),
            new FakeKnowledgeBasePathResolver(),
            new FakeKnowledgeBaseProvider(),
            new CrossReferenceAnalysisService());

        await viewModel.OpenCommand.ExecuteAsync(null);

        AnalysisItemViewModel source = viewModel.Items.Single(item => item.Address == 0);
        AnalysisItemViewModel target = viewModel.Items.Single(item => item.Address == 1);
        Assert.AreEqual("ExportedRoutine (0x00000001)", source.CrossReferenceTargets);
        Assert.AreEqual(1, target.IncomingCrossReferenceCount);
        viewModel.SelectedAnalysisItem = source;
        Assert.IsTrue(viewModel.FollowCrossReferenceCommand.CanExecute(null));
        viewModel.FollowCrossReferenceCommand.Execute(null);
        Assert.AreSame(target, viewModel.SelectedAnalysisItem);
    }

    [TestMethod]
    public void AnalysisItemViewModelDisplaysDataTypeCandidate()
    {
        AnalysisSession session = new("sample.exe", ReadOnlyMemory<byte>.Empty);
        AnalysisItem item = session.GetOrAddItem(0x2000);
        item.DataTypeCandidate = "IInterface";

        AnalysisItemViewModel viewModel = new(item, session);

        Assert.AreEqual("IInterface", viewModel.DataTypeCandidate);
    }

    [TestMethod]
    public void AnalysisItemViewModelDisplaysKnownMemberAccessCandidates()
    {
        AnalysisSession session = new("sample.exe", ReadOnlyMemory<byte>.Empty);
        AnalysisItem item = session.GetOrAddItem(0x1000);
        item.RecordSizeBytes = 12;
        item.RecordFields = [new DelphiRttiRecordField("f4", 4, 0x2000)];
        session.GetOrAddItem(0x2000).Name = "Integer";
        item.MemberAccessCandidates =
        [
            new MemberAccessCandidate("TForm", 8, "FCount", 0x2000, "Integer")
        ];

        AnalysisItemViewModel viewModel = new(item, session);

        Assert.AreEqual("TForm.FCount (+0x8): Integer", viewModel.MemberAccessCandidates);
        Assert.AreEqual("12 bytes", viewModel.RecordSize);
        Assert.AreEqual("f4 @ 4: Integer", viewModel.RecordFields);
    }

    [TestMethod]
    public void AnalysisItemViewModelDisplaysStackLocalVariables()
    {
        AnalysisSession session = new("sample.exe", ReadOnlyMemory<byte>.Empty);
        AnalysisItem item = session.GetOrAddItem(0x2000);
        item.StackLocalVariables = [new StackLocalVariable(-8, 4, "AnsiString")];

        AnalysisItemViewModel viewModel = new(item, session);

        Assert.AreEqual("AnsiString at EBP-8 (4 bytes)", viewModel.StackLocalVariables);
    }

    [TestMethod]
    public void AnalysisItemViewModelDisplaysResolvedVmtParent()
    {
        AnalysisSession session = new("sample.exe", ReadOnlyMemory<byte>.Empty);
        AnalysisItem parent = session.GetOrAddItem(0x1000);
        parent.SetFlags(AnalysisFlags.Vmt);
        parent.Name = "TObject";
        parent.TypeKind = DelphiTypeKind.Class;
        parent.UnitName = "System";
        parent.ClassVmtAddress = 0x1000;
        parent.ParentTypeInfoAddress = 0x900;
        AnalysisItem child = session.GetOrAddItem(0x2000);
        child.SetFlags(AnalysisFlags.Vmt);
        child.Name = "TChild";
        child.TypeKind = DelphiTypeKind.Class;
        child.UnitName = "System";
        child.ClassVmtAddress = 0x1000;
        child.ParentTypeInfoAddress = 0x900;
        child.PropertyNames = ["Caption", "OnClick"];
        child.Methods =
        [
            new DelphiVmtMethod("DoWork", 0x3000),
            new DelphiVmtMethod("Execute", 0x3010, true, 1, 2)
        ];
        child.Fields =
        [
            new DelphiVmtField("Caption", -4, 0x900),
            new DelphiVmtField("Count", 8, 0x901, true, 1)
        ];
        child.Interfaces =
        [
            new DelphiVmtInterface(
                Guid.Parse("00112233-4455-6677-8899-aabbccddeeff"),
                0xA00,
                -12,
                0x1234,
                0x902)
        ];
        child.DynamicMethods =
        [
            new DelphiVmtDynamicMethod(0x46, 0xA10),
            new DelphiVmtDynamicMethod(0x55, null)
        ];
        child.AutoMethods =
        [
            new DelphiVmtAutoMethod(
                0x100,
                "TChild.GetCaption",
                0xA20,
                3,
                0x21,
                new byte[] { 0x10 })
        ];
        child.InitializationFields =
        [
            new DelphiVmtInitializationField(0x903, 12),
            new DelphiVmtInitializationField(null, 24)
        ];
        child.VirtualMethods =
        [
            new DelphiVmtVirtualMethod(-0x2c, 0xA30),
            new DelphiVmtVirtualMethod(4, 0xA34)
        ];
        session.GetOrAddItem(0x900).Name = "UnicodeString";
        session.GetOrAddItem(0x902).Name = "IMyService";
        session.GetOrAddItem(0x903).Name = "TRecord";
        child.ParentAddress = parent.Address;

        AnalysisItemViewModel item = new(child, session);

        Assert.AreEqual("TObject (0x00001000)", item.ParentVmt);
        Assert.AreEqual("Class", item.TypeKind);
        Assert.AreEqual("System", item.UnitName);
        Assert.AreEqual("0x00001000", item.ClassVmt);
        Assert.AreEqual("0x00000900", item.ParentTypeInfo);
        Assert.AreEqual("Caption, OnClick", item.Properties);
        Assert.AreEqual("DoWork (0x00003000), Execute (0x00003010) [virtual 2]", item.Methods);
        Assert.AreEqual(
            "Caption @ -4 (type UnicodeString (0x00000900)), Count @ 8 (type 0x00000901, flags 0x01)",
            item.Fields);
        Assert.AreEqual(
            "IMyService (00112233-4455-6677-8899-aabbccddeeff) (vtable 0x00000A00, offset -12, getter 4660)",
            item.Interfaces);
        Assert.AreEqual("0x0046 -> 0x00000A10, 0x0055 -> unknown", item.DynamicMethods);
        Assert.AreEqual("0x00000100 TChild.GetCaption -> 0x00000A20", item.AutoMethods);
        Assert.AreEqual("TRecord @ 12, type unknown @ 24", item.InitializationFields);
        Assert.AreEqual("0xFFFFFFD4 -> 0x00000A30, 0x4 -> 0x00000A34", item.VirtualMethods);
    }

    private static PeImage CreateImage()
    {
        return new PeImage(
            "sample.exe",
            new byte[] { 0x90 },
            [new PeSection(".text", 0, 1, 0, 1, true)],
            0);
    }

    private sealed class FakeFileSelectionService(string? path) : IFileSelectionService
    {
        public Task<string?> SelectPeImageAsync(CancellationToken cancellationToken) =>
            Task.FromResult(path);
    }

    private sealed class FakePeImageLoader(PeImage image) : IPeImageLoader
    {
        public int LoadCount { get; private set; }

        public Task<PeImage> LoadAsync(string sourcePath, CancellationToken cancellationToken)
        {
            LoadCount++;
            return Task.FromResult(image with { SourcePath = sourcePath });
        }
    }

    private sealed class FakeVersionDetector(
        DelphiVersionDetection? detection = null) : IDelphiVersionDetector
    {
        public DelphiVersionDetection Detect(PeImage image) =>
            detection ?? new(DelphiVersion.Delphi2009, [DelphiVersion.Delphi2009], false);
    }

    private sealed class FakeKnowledgeBasePathResolver : IKnowledgeBasePathResolver
    {
        public string ResolvePath(DelphiVersion version)
        {
            string path = $"kb{(int)version}.bin";
            LastResolvedPath = path;
            return path;
        }

        public string? LastResolvedPath { get; private set; }
    }

    private sealed class FakeKnowledgeBaseProvider(bool throwFileNotFound = false) : IKnowledgeBaseProvider
    {
        public string? OpenedPath { get; private set; }

        public Task<IKnowledgeBase> OpenAsync(string filePath, CancellationToken cancellationToken)
        {
            OpenedPath = filePath;
            if (throwFileNotFound)
            {
                throw new FileNotFoundException("Missing test KB.");
            }

            return Task.FromResult<IKnowledgeBase>(new FakeKnowledgeBase());
        }
    }

    private sealed class FakeKnowledgeBase : IKnowledgeBase
    {
        public uint FormatVersion => 2;

        public IReadOnlyList<KnowledgeBaseModule> Modules { get; } =
            [new KnowledgeBaseModule(1, "System")];

        public IReadOnlyList<KnowledgeBaseProcedure> Procedures { get; } =
            Array.Empty<KnowledgeBaseProcedure>();

        public ushort GetModuleId(string moduleName) => 1;

        public string? GetModuleName(ushort moduleId) => "System";
    }

    private sealed class FakeAnalysisService : IAnalysisService
    {
        public AnalysisSession? LastSession { get; private set; }

        public Task<AnalysisResult> AnalyzeAsync(
            AnalysisSession session,
            IProgress<AnalysisProgress>? progress,
            CancellationToken cancellationToken)
        {
            LastSession = session;
            session.GetOrAddItem(0).SetFlags(AnalysisFlags.Code);
            session.AddDisassemblyLine(new DisassemblyLine(
                0,
                "E800000000",
                "Call",
                "call 00000006",
                1,
                InstructionFlowControl.Call));
            session.AddDisassemblyLine(new DisassemblyLine(
                1,
                "C3",
                "Ret",
                "ret",
                null,
                InstructionFlowControl.Return));
            return Task.FromResult(new AnalysisResult(session, Array.Empty<string>()));
        }
    }

    private sealed class CrossReferenceAnalysisService : IAnalysisService
    {
        public Task<AnalysisResult> AnalyzeAsync(
            AnalysisSession session,
            IProgress<AnalysisProgress>? progress,
            CancellationToken cancellationToken)
        {
            session.GetOrAddItem(1).Name = "ExportedRoutine";
            session.AddCrossReference(0, 1, CrossReferenceKind.Call);
            return Task.FromResult(new AnalysisResult(session, Array.Empty<string>()));
        }
    }
}
