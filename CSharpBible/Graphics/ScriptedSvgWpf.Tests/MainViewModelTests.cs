using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ScriptedSvgWpf.Dsl;
using ScriptedSvgWpf.Rendering;
using ScriptedSvgWpf.Services;
using ScriptedSvgWpf.ViewModels;

namespace ScriptedSvgWpf.Tests;

[TestClass]
public sealed class MainViewModelTests
{
    [TestMethod]
    public void ConstructorInitializesCheckerboardDocument()
    {
        var fileService = new TestDocumentFileService();
        var viewModel = CreateViewModel(fileService);

        Assert.AreEqual("Untitled.ssvg", viewModel.DocumentName);
        Assert.IsFalse(viewModel.IsDirty);
        Assert.AreEqual("Rendered 72 commands", viewModel.StatusText);
        Assert.AreEqual(string.Empty, viewModel.ErrorText);
        Assert.IsNotNull(viewModel.PreviewDocument);
        Assert.AreEqual(640, viewModel.PreviewDocument!.Width);
        Assert.AreEqual(640, viewModel.PreviewDocument.Height);
    }

    [TestMethod]
    public void ChangingScriptMarksDocumentDirty()
    {
        var viewModel = CreateViewModel(new TestDocumentFileService());

        viewModel.ScriptText = "canvas(10, 20, \"red\");";

        Assert.IsTrue(viewModel.IsDirty);
    }

    [TestMethod]
    public void NewDocumentResetsStateAndRendersDocument()
    {
        var viewModel = CreateViewModel(new TestDocumentFileService());
        viewModel.ScriptText = "canvas(10, 20, \"red\");";
        viewModel.ErrorText = "old error";

        viewModel.NewDocumentCommand.Execute(null);

        Assert.AreEqual("Untitled.ssvg", viewModel.DocumentName);
        Assert.AreEqual("canvas(640, 480, \"white\");\n", viewModel.ScriptText);
        Assert.IsFalse(viewModel.IsDirty);
        Assert.AreEqual("New document", viewModel.StatusText);
        Assert.AreEqual(string.Empty, viewModel.ErrorText);
        Assert.IsNotNull(viewModel.PreviewDocument);
        Assert.AreEqual(640, viewModel.PreviewDocument!.Width);
        Assert.AreEqual(480, viewModel.PreviewDocument.Height);
    }

    [TestMethod]
    public void OpenDocumentCancelDoesNothing()
    {
        var fileService = new TestDocumentFileService { OpenPath = null };
        var viewModel = CreateViewModel(fileService);
        var originalText = viewModel.ScriptText;

        viewModel.OpenDocumentCommand.Execute(null);

        Assert.AreEqual(originalText, viewModel.ScriptText);
        Assert.AreEqual(0, fileService.ReadCount);
        Assert.AreEqual("Rendered 72 commands", viewModel.StatusText);
    }

    [TestMethod]
    public void OpenDocumentLoadsAndRendersFile()
    {
        var fileService = new TestDocumentFileService
        {
            OpenPath = "C:\\docs\\sample.ssvg",
            TextToRead = "canvas(100, 80, \"yellow\"); circle(20, 20, 5, \"blue\");"
        };
        var viewModel = CreateViewModel(fileService);

        viewModel.OpenDocumentCommand.Execute(null);

        Assert.AreEqual("sample.ssvg", viewModel.DocumentName);
        Assert.AreEqual(fileService.TextToRead, viewModel.ScriptText);
        Assert.IsFalse(viewModel.IsDirty);
        Assert.AreEqual("Opened sample.ssvg", viewModel.StatusText);
        Assert.AreEqual(string.Empty, viewModel.ErrorText);
        Assert.AreEqual(1, fileService.ReadCount);
        Assert.AreEqual(fileService.OpenPath, fileService.LastReadPath);
        Assert.IsNotNull(viewModel.PreviewDocument);
        Assert.AreEqual(1, viewModel.PreviewDocument!.Commands.Count);
    }

    [TestMethod]
    public void OpenDocumentReadFailureReportsError()
    {
        var fileService = new TestDocumentFileService
        {
            OpenPath = "broken.ssvg",
            ReadException = new InvalidOperationException("read failed")
        };
        var viewModel = CreateViewModel(fileService);

        viewModel.OpenDocumentCommand.Execute(null);

        Assert.AreEqual("read failed", viewModel.ErrorText);
        Assert.AreEqual("Open failed", viewModel.StatusText);
    }

    [TestMethod]
    public void OpenDocumentRenderFailureReportsError()
    {
        var fileService = new TestDocumentFileService
        {
            OpenPath = "invalid.ssvg",
            TextToRead = "int count = 1; count = 1.5;"
        };
        var viewModel = CreateViewModel(fileService);

        viewModel.OpenDocumentCommand.Execute(null);

        Assert.AreEqual("invalid.ssvg", viewModel.DocumentName);
        Assert.IsTrue(viewModel.ErrorText.Length > 0);
        Assert.AreEqual("Opened invalid.ssvg", viewModel.StatusText);
        Assert.IsNull(viewModel.PreviewDocument);
        Assert.IsFalse(viewModel.IsDirty);
    }

    [TestMethod]
    public void SaveDocumentUsesSuggestedPathAndWritesText()
    {
        var fileService = new TestDocumentFileService { SavePath = "saved.ssvg" };
        var viewModel = CreateViewModel(fileService);
        viewModel.ScriptText = "canvas(20, 30, \"white\");";

        viewModel.SaveDocumentCommand.Execute(null);

        Assert.AreEqual("saved.ssvg", viewModel.DocumentName);
        Assert.AreEqual("saved.ssvg", fileService.LastWrittenPath);
        Assert.AreEqual(viewModel.ScriptText, fileService.LastWrittenText);
        Assert.AreEqual("Scripted SVG (*.ssvg)|*.ssvg|Text files (*.txt)|*.txt|All files (*.*)|*.*", fileService.LastSuggestedFilter);
        Assert.AreEqual("Untitled.ssvg", fileService.LastSuggestedName);
        Assert.IsFalse(viewModel.IsDirty);
        Assert.AreEqual("Saved saved.ssvg", viewModel.StatusText);
    }

    [TestMethod]
    public void SaveDocumentCancelDoesNothing()
    {
        var fileService = new TestDocumentFileService { SavePath = null };
        var viewModel = CreateViewModel(fileService);
        viewModel.ScriptText = "canvas(20, 30, \"white\");";

        viewModel.SaveDocumentCommand.Execute(null);

        Assert.IsTrue(viewModel.IsDirty);
        Assert.AreEqual(0, fileService.WriteCount);
        Assert.AreEqual("Rendered 72 commands", viewModel.StatusText);
    }

    [TestMethod]
    public void SaveDocumentWithCurrentPathSkipsSaveDialog()
    {
        var fileService = new TestDocumentFileService { OpenPath = "existing.ssvg", TextToRead = "canvas(5, 6, \"white\");" };
        var viewModel = CreateViewModel(fileService);
        viewModel.OpenDocumentCommand.Execute(null);
        viewModel.ScriptText = "canvas(7, 8, \"black\");";
        fileService.SavePath = "should-not-be-used.ssvg";

        viewModel.SaveDocumentCommand.Execute(null);

        Assert.AreEqual("existing.ssvg", fileService.LastWrittenPath);
        Assert.AreEqual(0, fileService.SaveDialogCount);
        Assert.IsFalse(viewModel.IsDirty);
    }

    [TestMethod]
    public void SaveDocumentWriteFailureReportsError()
    {
        var fileService = new TestDocumentFileService
        {
            SavePath = "failed.ssvg",
            WriteException = new InvalidOperationException("write failed")
        };
        var viewModel = CreateViewModel(fileService);
        viewModel.ScriptText = "canvas(10, 10, \"white\");";

        viewModel.SaveDocumentCommand.Execute(null);

        Assert.AreEqual("write failed", viewModel.ErrorText);
        Assert.AreEqual("Save failed", viewModel.StatusText);
        Assert.IsTrue(viewModel.IsDirty);
    }

    [TestMethod]
    public void RunSuccessClearsPreviousErrorAndUpdatesPreview()
    {
        var viewModel = CreateViewModel(new TestDocumentFileService());
        viewModel.ErrorText = "old error";
        viewModel.ScriptText = "canvas(40, 50, \"green\");";

        viewModel.RunCommand.Execute(null);

        Assert.AreEqual(string.Empty, viewModel.ErrorText);
        Assert.AreEqual("Rendered 0 commands", viewModel.StatusText);
        Assert.IsNotNull(viewModel.PreviewDocument);
        Assert.AreEqual(40, viewModel.PreviewDocument!.Width);
    }

    [TestMethod]
    public void RunFailureClearsPreviewAndFormatsError()
    {
        var viewModel = CreateViewModel(new TestDocumentFileService());
        viewModel.ScriptText = "canvas(";
        viewModel.RunCommand.Execute(null);

        Assert.IsNull(viewModel.PreviewDocument);
        Assert.IsTrue(viewModel.ErrorText.Contains("Line", StringComparison.OrdinalIgnoreCase));
        Assert.AreEqual("Render failed", viewModel.StatusText);
    }

    [TestMethod]
    public void ExportSvgWritesExportedDocument()
    {
        var fileService = new TestDocumentFileService { SavePath = "image.svg" };
        var viewModel = CreateViewModel(fileService);

        viewModel.ExportSvgCommand.Execute(null);

        Assert.AreEqual("image.svg", fileService.LastWrittenPath);
        StringAssert.Contains(fileService.LastWrittenText!, "<svg");
        Assert.AreEqual("Untitled.svg", fileService.LastSuggestedName);
        Assert.AreEqual("SVG files (*.svg)|*.svg|All files (*.*)|*.*", fileService.LastSuggestedFilter);
        Assert.AreEqual("Exported image.svg", viewModel.StatusText);
    }

    [TestMethod]
    public void ExportSvgCancelDoesNothing()
    {
        var fileService = new TestDocumentFileService { SavePath = null };
        var viewModel = CreateViewModel(fileService);

        viewModel.ExportSvgCommand.Execute(null);

        Assert.AreEqual(0, fileService.WriteCount);
        Assert.AreEqual("Rendered 72 commands", viewModel.StatusText);
    }

    [TestMethod]
    public void ExportSvgRenderFailureDoesNotOpenSaveDialog()
    {
        var fileService = new TestDocumentFileService();
        var viewModel = CreateViewModel(fileService);
        viewModel.ScriptText = "canvas(";
        viewModel.PreviewDocument = null;
        viewModel.ErrorText = string.Empty;

        viewModel.ExportSvgCommand.Execute(null);

        Assert.IsNull(viewModel.PreviewDocument);
        Assert.AreEqual("Render failed", viewModel.StatusText);
        Assert.AreEqual(0, fileService.SaveDialogCount);
    }

    [TestMethod]
    public void ExportSvgWriteFailureReportsError()
    {
        var fileService = new TestDocumentFileService
        {
            SavePath = "failed.svg",
            WriteException = new InvalidOperationException("export failed")
        };
        var viewModel = CreateViewModel(fileService);

        viewModel.ExportSvgCommand.Execute(null);

        Assert.AreEqual("export failed", viewModel.ErrorText);
        Assert.AreEqual("Export failed", viewModel.StatusText);
    }

    private static MainViewModel CreateViewModel(TestDocumentFileService fileService) =>
        new(new ScriptInterpreter(), new SvgExporter(), fileService);

    private sealed class TestDocumentFileService : IDocumentFileService
    {
        public string? OpenPath { get; set; }
        public string? SavePath { get; set; }
        public string TextToRead { get; set; } = string.Empty;
        public Exception? ReadException { get; set; }
        public Exception? WriteException { get; set; }
        public int ReadCount { get; private set; }
        public int SaveDialogCount { get; private set; }
        public int WriteCount { get; private set; }
        public string? LastReadPath { get; private set; }
        public string? LastWrittenPath { get; private set; }
        public string? LastWrittenText { get; private set; }
        public string? LastSuggestedName { get; private set; }
        public string? LastSuggestedFilter { get; private set; }

        public string? ChooseOpenPath() => OpenPath;

        public string? ChooseSavePath(string suggestedName, string filter)
        {
            SaveDialogCount++;
            LastSuggestedName = suggestedName;
            LastSuggestedFilter = filter;
            return SavePath;
        }

        public string ReadText(string path)
        {
            ReadCount++;
            LastReadPath = path;
            if (ReadException is not null)
            {
                throw ReadException;
            }

            return TextToRead;
        }

        public void WriteText(string path, string text)
        {
            WriteCount++;
            LastWrittenPath = path;
            LastWrittenText = text;
            if (WriteException is not null)
            {
                throw WriteException;
            }
        }
    }
}
