using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Ollama.Tools.ContentAnalysis;
using Ollama.Wpf.TextAnalysis;
using Ollama.Wpf.TextAnalysis.Services;
using Ollama.Wpf.TextAnalysis.ViewModels;

namespace PeripheralProduction.Tests;

[TestClass]
public sealed class WpfTextAnalysisTests
{
    [TestMethod]
    public void ConfigureServices_RegistersTheApplicationGraph()
    {
        ServiceCollection services = new();

        App.ConfigureServices(services);
        using ServiceProvider provider = services.BuildServiceProvider();

        Assert.IsNotNull(provider.GetRequiredService<IContentAnalysisService>());
        Assert.IsNotNull(provider.GetRequiredService<ITextFilePicker>());
        Assert.IsNotNull(provider.GetRequiredService<MainWindowViewModel>());
    }

    [TestMethod]
    public void Constructors_ValidateDependencies()
    {
        ITextFilePicker picker = Substitute.For<ITextFilePicker>();
        IContentAnalysisService service = Substitute.For<IContentAnalysisService>();

        Assert.ThrowsExactly<ArgumentNullException>(() => new ContentAnalysisService(null!));
        Assert.ThrowsExactly<ArgumentNullException>(() => new MainWindowViewModel(null!, picker));
        Assert.ThrowsExactly<ArgumentNullException>(() => new MainWindowViewModel(service, null!));
    }

    [TestMethod]
    public async Task ContentAnalysisService_DelegatesToRouter()
    {
        ContentAnalysisRouter router = new(new TextAnalysisTool(), new CSharpCodeAnalysisTool());
        ContentAnalysisService service = new(router);

        ContentAnalysisExecutionResult result = await service.AnalyzeAsync(
            "A concise text with a second sentence.",
            "notes.txt",
            ContentAnalysisMode.Text);

        Assert.AreEqual("Text analysis", result.Decision.AnalysisLabel);
    }

    [TestMethod]
    public async Task ViewModel_AnalyzesResultAndFormatsFindingsAndSuggestions()
    {
        IContentAnalysisService service = Substitute.For<IContentAnalysisService>();
        ITextFilePicker picker = Substitute.For<ITextFilePicker>();
        service.AnalyzeAsync(Arg.Any<string>(), Arg.Any<string?>(), Arg.Any<ContentAnalysisMode>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(CreateExecutionResult(
                score: 0.75,
                confidence: 0.5,
                findings:
                [
                    new ContentAnalysisFinding
                    {
                        Title = "Finding",
                        Message = "Message",
                        Evidence = "Line 1",
                    },
                ],
                suggestions:
                [
                    new ContentAnalysisSuggestion
                    {
                        Title = "Suggestion",
                        Description = "Description",
                        Priority = "High",
                    },
                ])));
        MainWindowViewModel viewModel = new(service, picker)
        {
            InputText = "input",
        };

        await viewModel.AnalyzeTextCommand.ExecuteAsync(null);

        Assert.AreEqual("Summary", viewModel.Summary);
        StringAssert.Contains(viewModel.ScoreText, "75");
        StringAssert.Contains(viewModel.ConfidenceText, "5");
        StringAssert.Contains(viewModel.FindingsText, "Finding");
        StringAssert.Contains(viewModel.SuggestionsText, "Suggestion");
        Assert.AreEqual("Analysis completed.", viewModel.StatusText);
        Assert.IsFalse(viewModel.IsBusy);
        await service.Received(1).AnalyzeAsync("input", null, ContentAnalysisMode.Auto, Arg.Any<CancellationToken>());

        viewModel.SelectedMode = ContentAnalysisMode.CSharp;
        await viewModel.AnalyzeTextCommand.ExecuteAsync(null);
        Assert.AreEqual("Analysis completed.", viewModel.StatusText);
        viewModel.SelectedMode = ContentAnalysisMode.Text;
        await viewModel.AnalyzeTextCommand.ExecuteAsync(null);
        Assert.AreEqual("Analysis completed.", viewModel.StatusText);
    }

    [TestMethod]
    public async Task ViewModel_FormatsEmptyResultsAndReportsServiceExceptions()
    {
        IContentAnalysisService service = Substitute.For<IContentAnalysisService>();
        ITextFilePicker picker = Substitute.For<ITextFilePicker>();
        service.AnalyzeAsync(Arg.Any<string>(), Arg.Any<string?>(), Arg.Any<ContentAnalysisMode>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(CreateExecutionResult(null, null, [], [])));
        MainWindowViewModel viewModel = new(service, picker)
        {
            InputText = "input",
        };

        await viewModel.AnalyzeTextCommand.ExecuteAsync(null);

        Assert.AreEqual("-", viewModel.ScoreText);
        Assert.AreEqual("-", viewModel.ConfidenceText);
        Assert.AreEqual("No findings.", viewModel.FindingsText);
        Assert.AreEqual("No suggestions.", viewModel.SuggestionsText);

        service.AnalyzeAsync(Arg.Any<string>(), Arg.Any<string?>(), Arg.Any<ContentAnalysisMode>(), Arg.Any<CancellationToken>())
            .Returns<Task<ContentAnalysisExecutionResult>>(_ => throw new InvalidOperationException("analysis failed"));
        await viewModel.AnalyzeTextCommand.ExecuteAsync(null);

        Assert.AreEqual("analysis failed", viewModel.StatusText);
        Assert.IsFalse(viewModel.IsBusy);
    }

    [TestMethod]
    public async Task ViewModel_LoadFileAndSelectModeCommands_UpdateStateDeterministically()
    {
        IContentAnalysisService service = Substitute.For<IContentAnalysisService>();
        ITextFilePicker picker = Substitute.For<ITextFilePicker>();
        picker.PickAndReadAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<TextFileSelection?>(null));
        MainWindowViewModel viewModel = new(service, picker);

        await viewModel.LoadFileCommand.ExecuteAsync(null);
        Assert.AreEqual("Ready.", viewModel.StatusText);

        picker.PickAndReadAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<TextFileSelection?>(new TextFileSelection(@"C:\work\input.md", "loaded input")));
        await viewModel.LoadFileCommand.ExecuteAsync(null);
        Assert.AreEqual("loaded input", viewModel.InputText);
        Assert.AreEqual("Loaded input.md.", viewModel.StatusText);

        viewModel.SelectTextModeCommand.Execute(null);
        Assert.AreEqual(ContentAnalysisMode.Text, viewModel.SelectedMode);
        Assert.AreEqual("Text analysis", viewModel.SelectedModeText);
        viewModel.SelectCSharpModeCommand.Execute(null);
        Assert.AreEqual(ContentAnalysisMode.CSharp, viewModel.SelectedMode);
        Assert.AreEqual("C# source analysis", viewModel.SelectedModeText);
        viewModel.SelectAutoModeCommand.Execute(null);
        Assert.AreEqual(ContentAnalysisMode.Auto, viewModel.SelectedMode);
        Assert.AreEqual("Auto", viewModel.SelectedModeText);
    }

    [TestMethod]
    public void ViewModel_AnalyzeCommandTracksInputAndBusyState()
    {
        MainWindowViewModel viewModel = new(Substitute.For<IContentAnalysisService>(), Substitute.For<ITextFilePicker>());

        viewModel.InputText = " ";
        Assert.IsFalse(viewModel.AnalyzeTextCommand.CanExecute(null));

        viewModel.InputText = "input";
        Assert.IsTrue(viewModel.AnalyzeTextCommand.CanExecute(null));
        viewModel.IsBusy = true;
        Assert.IsFalse(viewModel.AnalyzeTextCommand.CanExecute(null));
    }

    [TestMethod]
    [DoNotParallelize]
    public void WpfCompositionRootAndWindow_AreExercisedOnAnStaThreadWithoutShowingUi()
    {
        RunOnSta(() =>
        {
            bool presenterInvoked = false;
            App.MainWindowPresenter = _ => presenterInvoked = true;
            TestApp application = new();
            application.DisposeServices();
            application.OnStartup(application, null!);

            Assert.IsTrue(presenterInvoked);
            MainWindowViewModel viewModel = new(Substitute.For<IContentAnalysisService>(), Substitute.For<ITextFilePicker>());
            Assert.ThrowsExactly<ArgumentNullException>(() => new MainWindow(null!));
            MainWindow window = new(viewModel);
            Assert.AreSame(viewModel, window.DataContext);

            application.DisposeServices();
            application.ExitApplication();
        });
    }

    [TestMethod]
    [DoNotParallelize]
    public void OpenFileDialogTextFilePicker_UsesDeterministicDialogPresenter()
    {
        string path = Path.Combine(Environment.CurrentDirectory, "picker-input.txt");
        File.WriteAllText(path, "picker content");
        try
        {
            RunOnSta(() =>
            {
                OpenFileDialogTextFilePicker.DialogPresenter = dialog =>
                {
                    dialog.FileName = path;
                    return true;
                };
                OpenFileDialogTextFilePicker picker = new();
                TextFileSelection? selection = picker.PickAndReadAsync().GetAwaiter().GetResult();
                Assert.IsNotNull(selection);
                Assert.AreEqual(path, selection.FilePath);
                Assert.AreEqual("picker content", selection.Content);

                OpenFileDialogTextFilePicker.DialogPresenter = _ => false;
                Assert.IsNull(picker.PickAndReadAsync().GetAwaiter().GetResult());
            });
        }
        finally
        {
            File.Delete(path);
        }
    }

    [TestMethod]
    public void DefaultWindowPresenter_RejectsNullWithoutShowingUi()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() => App.ShowMainWindow(null!));
    }

    [TestMethod]
    [DoNotParallelize]
    public void DefaultWindowPresenter_ShowsAndClosesAWindowOnAnStaThread()
    {
        RunOnSta(() =>
        {
            MainWindowViewModel viewModel = new(Substitute.For<IContentAnalysisService>(), Substitute.For<ITextFilePicker>());
            MainWindow window = new(viewModel);
            App.ShowMainWindow(window);
            Assert.IsTrue(window.IsVisible);
            window.Close();
        });
    }

    private static ContentAnalysisExecutionResult CreateExecutionResult(
        double? score,
        double? confidence,
        ContentAnalysisFinding[] findings,
        ContentAnalysisSuggestion[] suggestions)
        => new()
        {
            Decision = new ContentAnalysisRoutingDecision
            {
                AnalysisLabel = "Text analysis",
                Reason = "Test routing.",
            },
            Result = new ContentAnalysisResult
            {
                Summary = "Summary",
                Score = score,
                Confidence = confidence,
                Rationale = "Rationale",
                Findings = findings,
                Suggestions = suggestions,
            },
        };

    private static void RunOnSta(Action action)
    {
        Exception? exception = null;
        Thread thread = new(() =>
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                exception = ex;
            }
        });
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        thread.Join();
        if (exception is not null)
        {
            throw new AssertFailedException(exception.ToString());
        }
    }

    private sealed class TestApp : App
    {
        public void ExitApplication() => OnExit(null!);
    }
}
