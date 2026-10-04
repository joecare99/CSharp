using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Document.Base.Factories;
using Document.Docx;
using Genealogy.Gedcom;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using OFBCreator.Avalonia.Services;
using OFBCreator.Avalonia.ViewModels;
using OFBCreator.Console.Services;
using OFBCreator.Core.Models;
using OFBCreator.Core.Services;
using OFBCreator.Projects.Models;
using OFBCreator.Projects.Services;

namespace OFBCreator.Avalonia.Tests;

[TestClass]
public sealed class OFBProjectWorkspaceViewModelTests : IDisposable
{
    private readonly string _testDirectory = Path.Combine(Path.GetTempPath(), $"ofb-workspace-{Guid.NewGuid():N}");

    public OFBProjectWorkspaceViewModelTests() => Directory.CreateDirectory(_testDirectory);

    [TestMethod]
    public void SaveAndOpenProject_PersistsSharedProjectSettings()
    {
        var viewModel = CreateViewModel();
        viewModel.ProjectFilePath = Path.Combine(_testDirectory, "FamilyBook.ofbproject");
        viewModel.Name = "FamilyBook";
        viewModel.Title = "Family Book";
        viewModel.InputPath = "data\\sample.ged";
        viewModel.OutputPath = "output\\book.docx";
        viewModel.EntryTemplate = "ak";
        viewModel.AutoAcceptThreshold = 91;
        viewModel.RuleTargetKind = "person";
        viewModel.RuleTargetId = OFBExportRuleTarget.Person("gedcom", "I1");
        viewModel.RuleAction = "replace";
        viewModel.RuleField = "givenName";
        viewModel.RuleValue = "Anna";
        viewModel.AddRuleCommand.Execute(null);
        viewModel.SaveProjectCommand.Execute(null);

        Assert.AreEqual(Path.GetFullPath(viewModel.ProjectFilePath), viewModel.ProjectFilePath);
        var saved = new OFBProjectStore(Path.Combine(_testDirectory, "catalog"))
            .Open(viewModel.ProjectFilePath).Project;
        Assert.AreEqual("FamilyBook", saved.Name);
        Assert.AreEqual("data\\sample.ged", saved.InputPath);
        Assert.AreEqual(91, saved.GroupingPolicy.AutoAcceptThreshold);
        Assert.AreEqual("Anna", saved.ExportRules.Single().Value);

        viewModel.NewProjectCommand.Execute(null);
        viewModel.ProjectFilePath = Path.Combine(_testDirectory, "FamilyBook.ofbproject");
        viewModel.OpenProjectCommand.Execute(null);
        Assert.AreEqual("FamilyBook", viewModel.Name);
        Assert.AreEqual(1, viewModel.ExportRules.Count);
    }

    [TestMethod]
    public void GroupingCandidateCommands_CreateReplaceableEditorialDecisions()
    {
        var viewModel = CreateViewModel();
        var candidate = new OFBCreator.Core.Models.OFBGroupingCandidate(
            "I1-F1|I2-F2",
            "Meyer",
            "Meier",
            OFBExportRuleTarget.Family("gedcom", "F1"),
            OFBExportRuleTarget.Family("gedcom", "F2"),
            88,
            "review",
            []);

        viewModel.AcceptCandidateCommand.Execute(candidate);
        Assert.AreEqual("acceptMerge", viewModel.GroupingDecisions.Single().Action);

        viewModel.ManualGroupName = "Meyer";
        viewModel.ManualMergeCandidateCommand.Execute(candidate);
        Assert.AreEqual(1, viewModel.GroupingDecisions.Count);
        Assert.AreEqual("manualMerge", viewModel.GroupingDecisions[0].Action);
        Assert.AreEqual("Meyer", viewModel.GroupingDecisions[0].GroupName);

        viewModel.RejectCandidateCommand.Execute(candidate);
        Assert.AreEqual("rejectMerge", viewModel.GroupingDecisions.Single().Action);
    }

    [TestMethod]
    public void AddRuleCommand_ReportsInvalidRulesWithoutAddingThem()
    {
        var viewModel = CreateViewModel();
        viewModel.RuleTargetId = " ";
        viewModel.RuleAction = "exclude";

        viewModel.AddRuleCommand.Execute(null);

        Assert.AreEqual(0, viewModel.ExportRules.Count);
        Assert.IsFalse(string.IsNullOrWhiteSpace(viewModel.StatusMessage));
    }

    [TestMethod]
    public void ValidateTemplateCommand_ReportsBuiltInTemplateValidity()
    {
        var viewModel = CreateViewModel();
        viewModel.EntryTemplate = "gc";

        viewModel.ValidateTemplateCommand.Execute(null);

        StringAssert.Contains(viewModel.StatusMessage, "is valid");
    }

    [TestMethod]
    public void RuleEditor_UpdatesFieldsAndPreservesExplicitOrder()
    {
        var viewModel = CreateViewModel();
        viewModel.RuleTargetKind = "person";
        viewModel.RuleAction = "replace";
        viewModel.RuleField = "givenName";
        viewModel.RuleValue = "Anna";
        viewModel.RuleTargetId = OFBExportRuleTarget.Person("gedcom", "I1");
        viewModel.AddRuleCommand.Execute(null);
        var firstId = viewModel.SelectedRule!.Id;

        viewModel.RuleValue = "Marie";
        viewModel.RuleTargetId = OFBExportRuleTarget.Person("gedcom", "I2");
        viewModel.AddRuleCommand.Execute(null);
        var secondId = viewModel.SelectedRule!.Id;

        viewModel.SelectedRule = viewModel.ExportRules[0];
        viewModel.RuleValue = "Elisabeth";
        viewModel.UpdateSelectedRuleCommand.Execute(null);
        Assert.AreEqual("Elisabeth", viewModel.ExportRules[0].Value);

        viewModel.MoveSelectedRuleDownCommand.Execute(null);

        CollectionAssert.AreEqual(new[] { secondId, firstId },
            viewModel.ExportRules.Select(rule => rule.Id).ToArray());
        CollectionAssert.AreEqual(new[] { 1, 2 },
            viewModel.ExportRules.Select(rule => rule.Order).ToArray());
    }

    [TestMethod]
    public void RuleEditor_CreatesFactOccurrenceExclusion()
    {
        var viewModel = CreateViewModel();
        viewModel.RuleTargetKind = "fact";
        viewModel.RuleTargetId = OFBExportRuleTarget.Person("gedcom", "I7");
        viewModel.RuleAction = "exclude";
        viewModel.RuleField = "OCCU";
        viewModel.RuleOccurrence = 2;

        viewModel.AddRuleCommand.Execute(null);

        Assert.AreEqual("OCCU", viewModel.ExportRules.Single().Field);
        Assert.AreEqual(2, viewModel.ExportRules.Single().Occurrence);
    }

    [TestMethod]
    public async Task CliAndWorkspaceService_ExportSamePortableProjectWithEquivalentDocxSemantics()
    {
        var fixturePath = Path.Combine(AppContext.BaseDirectory, "TestData", "gedcom-5.5.1-synthetic.ged");
        Assert.IsTrue(File.Exists(fixturePath), $"Synthetic GEDCOM fixture was not found: {fixturePath}");
        var inputPath = Path.Combine(_testDirectory, "sample.ged");
        File.Copy(fixturePath, inputPath);
        var projectPath = Path.Combine(_testDirectory, "FamilyBook.ofbproject");
        var store = new OFBProjectStore(Path.Combine(_testDirectory, "catalog"));
        var project = new OFBProject
        {
            Name = "FamilyBook",
            Title = "Cross-host acceptance",
            DataSource = "gedcom",
            InputPath = "sample.ged",
            OutputPath = "workspace.docx",
            PlaceId = "München",
            Preface = "Shared project preface",
            Legend = "Shared project legend",
            ExportRules =
            [
                new OFBExportRule
                {
                    Order = 1,
                    TargetKind = "person",
                    TargetId = OFBExportRuleTarget.Person("gedcom", "I2"),
                    Action = "replace",
                    Field = "displayName",
                    Value = "Pseudonym"
                }
            ]
        };
        projectPath = store.Save(project, projectPath);

        UserDocumentFactory.ScanAssemblies(new[] { typeof(DocxDocument).Assembly });
        var documentFactory = Substitute.For<IUserDocumentFactory>();
        documentFactory.CreateDocument(OFBOutputFormat.Docx)
            .Returns(_ => UserDocumentFactory.Create(".docx"));
        var dataSource = new CanonicalGedcomFamilyDataSource(
            new GedcomInputDriver(),
            new CanonicalGenealogyAdapter());
        var workspaceService = new OFBWorkspaceService(new ConsoleExportService(dataSource, documentFactory));
        await workspaceService.ExportAsync(project, projectPath);

        var cliOutputPath = Path.Combine(_testDirectory, "cli.docx");
        using var process = Process.Start(CreateCliStartInfo(projectPath, cliOutputPath))
            ?? throw new InvalidOperationException("Could not start the OFBCreator CLI acceptance process.");
        var standardOutputTask = process.StandardOutput.ReadToEndAsync();
        var standardErrorTask = process.StandardError.ReadToEndAsync();
        await process.WaitForExitAsync();
        var standardOutput = await standardOutputTask;
        var standardError = await standardErrorTask;
        Assert.AreEqual(0, process.ExitCode, $"CLI export failed.{Environment.NewLine}{standardOutput}{standardError}");

        var workspaceSemantics = ReadDocxSemantics(Path.Combine(_testDirectory, "workspace.docx"));
        var cliSemantics = ReadDocxSemantics(cliOutputPath);
        CollectionAssert.AreEqual(
            cliSemantics.ParagraphTexts,
            workspaceSemantics.ParagraphTexts,
            $"CLI paragraphs:{Environment.NewLine}{string.Join(Environment.NewLine, cliSemantics.ParagraphTexts)}"
            + $"{Environment.NewLine}Workspace paragraphs:{Environment.NewLine}{string.Join(Environment.NewLine, workspaceSemantics.ParagraphTexts)}");
        CollectionAssert.AreEqual(cliSemantics.BookmarkNames, workspaceSemantics.BookmarkNames);
        CollectionAssert.AreEqual(cliSemantics.LinkTargets, workspaceSemantics.LinkTargets);
        StringAssert.Contains(string.Concat(workspaceSemantics.ParagraphTexts), "Pseudonym");
    }

    public void Dispose()
    {
        if (Directory.Exists(_testDirectory))
            Directory.Delete(_testDirectory, recursive: true);
    }

    private OFBProjectWorkspaceViewModel CreateViewModel()
    {
        var store = new OFBProjectStore(Path.Combine(_testDirectory, "catalog"));
        return new OFBProjectWorkspaceViewModel(store, Substitute.For<IOFBWorkspaceService>());
    }

    private ProcessStartInfo CreateCliStartInfo(string projectPath, string outputPath)
    {
        var startInfo = new ProcessStartInfo("dotnet")
        {
            WorkingDirectory = _testDirectory,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };
        startInfo.ArgumentList.Add(typeof(ConsoleExportService).Assembly.Location);
        startInfo.ArgumentList.Add("generate");
        startInfo.ArgumentList.Add("--project");
        startInfo.ArgumentList.Add(projectPath);
        startInfo.ArgumentList.Add("--output");
        startInfo.ArgumentList.Add(outputPath);
        startInfo.Environment["APPDATA"] = Path.Combine(_testDirectory, "appdata");
        startInfo.Environment["DOTNET_CLI_TELEMETRY_OPTOUT"] = "1";
        return startInfo;
    }

    private static DocxSemantics ReadDocxSemantics(string path)
    {
        using var archive = ZipFile.OpenRead(path);
        using var documentStream = archive.GetEntry("word/document.xml")?.Open()
            ?? throw new InvalidDataException($"DOCX document part is missing: {path}");
        var document = XDocument.Load(documentStream);
        XNamespace word = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";
        var paragraphs = document.Descendants(word + "p")
            .Select(paragraph => string.Concat(paragraph.Descendants(word + "t").Select(text => text.Value)))
            .ToArray();
        var bookmarks = document.Descendants(word + "bookmarkStart")
            .Select(element => (string?)element.Attribute(word + "name"))
            .Where(name => name is not null)
            .Select(name => name!)
            .OrderBy(name => name, StringComparer.Ordinal)
            .ToArray();
        var links = document.Descendants(word + "hyperlink")
            .Select(element => (string?)element.Attribute(word + "anchor"))
            .Where(target => target is not null)
            .Select(target => target!)
            .OrderBy(target => target, StringComparer.Ordinal)
            .ToArray();
        return new DocxSemantics(paragraphs, bookmarks, links);
    }

    private sealed record DocxSemantics(
        string[] ParagraphTexts,
        string[] BookmarkNames,
        string[] LinkTargets);
}
