using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using AA98_AvlnCodeStudio.Diagnostics.UI.ViewModels;
using AA98_AvlnCodeStudio.Editor.Components;
using AA98_AvlnCodeStudio.Editor.Services;
using AA98_AvlnCodeStudio.Model.Documents;
using AA98_AvlnCodeStudio.Planning.Core.Services;
using AA98_AvlnCodeStudio.UI.Components;
using AA98_AvlnCodeStudio.UI.Controls;
using AA98_AvlnCodeStudio.UI.Services;
using AA98_AvlnCodeStudio.UI.ViewModels;
using Avalonia;
using Avalonia.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AA98_AvlnCodeStudio.Tests.Engineering;

[TestClass]
public sealed class WorkbenchTargetFrameworkTests
{
    private const string IsolatedProcessVariable = "AA98_WORKBENCH_TFM_ISOLATED";

    [ClassInitialize]
    public static void Initialize(TestContext _)
    {
        if (OperatingSystem.IsLinux() && string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DISPLAY")))
            Assert.Inconclusive("The Avalonia editor integration tests require a graphical display.");
        if (Environment.GetEnvironmentVariable(IsolatedProcessVariable) != "1")
            return;
        TaskCompletionSource<bool> ready = new(TaskCreationOptions.RunContinuationsAsynchronously);
        Thread thread = new(() =>
        {
            try
            {
                AppBuilder.Configure<Application>().UsePlatformDetect().SetupWithoutStarting();
                ready.SetResult(true);
                Dispatcher.UIThread.MainLoop(CancellationToken.None);
            }
            catch (Exception exception)
            {
                ready.TrySetException(exception);
            }
        })
        { IsBackground = true };
        if (OperatingSystem.IsWindows())
            thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        ready.Task.GetAwaiter().GetResult();
    }

    [TestMethod]
    public async Task FrameworkSelection_RefreshesDiagnosticsAndActionsUsingUnsavedBuffers()
    {
        await RunIsolatedAsync(nameof(FrameworkSelection_RefreshesDiagnosticsAndActionsUsingUnsavedBuffers), async () =>
        {
            using Fixture fixture = await Fixture.CreateAsync();
            MainWindowViewModel vm = fixture.ViewModel;
            await fixture.OpenDraftsAsync();
            vm.SelectedWorkflowProject = vm.Workspace!.Projects.Single();
            vm.SelectedWorkflowFramework = "net9.0";
            await WaitUntilAsync(() => vm.DiagnosticCollection.Items.Any(d => d.Code == "CS0117"));

            vm.SelectedWorkflowFramework = "net10.0";
            await WaitUntilAsync(() => !vm.DiagnosticCollection.Items.Any(d => d.Code == "CS0117"));
            Assert.AreEqual(Fixture.SavedApi, await File.ReadAllTextAsync(fixture.ApiPath));
            Assert.AreEqual(Fixture.SavedConsumer, await File.ReadAllTextAsync(fixture.ConsumerPath));

            EditorTextArea editor = (EditorTextArea)vm.ActiveEditorTab.TextView!;
            editor.Text = vm.Editor.Text;
            editor.MoveCaretWithoutFocus(vm.Editor.Text.IndexOf("OnlyTen", StringComparison.Ordinal));
            await vm.RequestQuickInfoCommand.ExecuteAsync(null);
            StringAssert.Contains(vm.QuickInfoText, "OnlyTen");

            vm.SelectedWorkflowFramework = "net9.0";
            Assert.IsFalse(vm.HasQuickInfo);
            await vm.RequestQuickInfoCommand.ExecuteAsync(null);
            Assert.IsFalse(vm.QuickInfoText.Contains("OnlyTen", StringComparison.Ordinal));
            await WaitUntilAsync(() => vm.DiagnosticCollection.Items.Any(d => d.Code == "CS0117"));
        });
    }

    [TestMethod]
    public async Task FrameworkChange_RejectsInFlightCompletionAndCannotApplyOldSelection()
    {
        await RunIsolatedAsync(nameof(FrameworkChange_RejectsInFlightCompletionAndCannotApplyOldSelection), async () =>
        {
            using Fixture fixture = await Fixture.CreateAsync();
            MainWindowViewModel vm = fixture.ViewModel;
            await fixture.OpenDraftsAsync();
            vm.SelectedWorkflowProject = vm.Workspace!.Projects.Single();
            vm.SelectedWorkflowFramework = "net10.0";
            const string completionSource = "public class Consumer { void Run() { TargetApi.Only } }";
            vm.Editor.Text = completionSource;
            EditorTextArea editor = (EditorTextArea)vm.ActiveEditorTab.TextView!;
            editor.Text = completionSource;
            editor.MoveCaretWithoutFocus(completionSource.IndexOf("Only", StringComparison.Ordinal) + 4);
            await vm.RequestCompletionsCommand.ExecuteAsync(null);
            Assert.IsTrue(vm.CompletionSuggestions.Any(item => item.DisplayText == "OnlyTen"));
            vm.SelectedWorkflowFramework = "net9.0";
            Assert.AreEqual(0, vm.CompletionSuggestions.Count);
            await vm.ApplySelectedCompletionCommand.ExecuteAsync(null);
            Assert.AreEqual(completionSource, vm.Editor.Text);

            SemaphoreSlim gate = (SemaphoreSlim)typeof(CSharpSyntaxDiagnosticService)
                .GetField("_workspaceGate", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(fixture.Service)!;
            await gate.WaitAsync();
            Task pending;
            try
            {
                pending = vm.RequestCompletionsCommand.ExecuteAsync(null);
                Assert.IsFalse(pending.IsCompleted);
                vm.SelectedWorkflowFramework = "net10.0";
            }
            finally
            {
                gate.Release();
            }
            await pending;
            Assert.AreEqual(0, vm.CompletionSuggestions.Count);
            await vm.RequestCompletionsCommand.ExecuteAsync(null);
            Assert.IsTrue(vm.CompletionSuggestions.Any(item => item.DisplayText == "OnlyTen"));

            const string quickInfoSource = "public class Consumer { string Read() => TargetApi.OnlyTen(\"value\"); }";
            vm.Editor.Text = quickInfoSource;
            editor.Text = quickInfoSource;
            editor.MoveCaretWithoutFocus(quickInfoSource.IndexOf("OnlyTen", StringComparison.Ordinal));
            await gate.WaitAsync();
            try
            {
                pending = vm.RequestQuickInfoCommand.ExecuteAsync(null);
                Assert.IsFalse(pending.IsCompleted);
                vm.SelectedWorkflowFramework = "net9.0";
            }
            finally
            {
                gate.Release();
            }
            await pending;
            Assert.IsFalse(vm.HasQuickInfo);
            vm.SelectedWorkflowFramework = "net10.0";
            await vm.RequestQuickInfoCommand.ExecuteAsync(null);
            StringAssert.Contains(vm.QuickInfoText, "OnlyTen");
        });
    }

    [TestMethod]
    public async Task SelectionUsesOwningProjectAndDoesNotResolveAmbiguousOwnership()
    {
        await RunIsolatedAsync(nameof(SelectionUsesOwningProjectAndDoesNotResolveAmbiguousOwnership), async () =>
        {
            using Fixture fixture = await Fixture.CreateAsync();
            MainWindowViewModel vm = fixture.ViewModel;
            await fixture.OpenDraftsAsync();
            string root = Path.GetDirectoryName(fixture.ConsumerPath)!;
            string otherDirectory = Path.Combine(root, "Other");
            Directory.CreateDirectory(otherDirectory);
            await File.WriteAllTextAsync(Path.Combine(otherDirectory, "Other.csproj"), "<Project />");
            string solution = Path.Combine(root, "Projects.slnx");
            await File.WriteAllTextAsync(solution,
                "<Solution><Project Path=\"Sample.csproj\" /><Project Path=\"Other\\Other.csproj\" /></Solution>");
            await vm.OpenWorkspaceFileAsync(solution);
            var owner = vm.Workspace!.Projects.Single(p => Path.GetFileName(p.ProjectPath) == "Sample.csproj");
            var other = vm.Workspace.Projects.Single(p => Path.GetFileName(p.ProjectPath) == "Other.csproj");
            EditorTextArea editor = (EditorTextArea)vm.ActiveEditorTab.TextView!;
            editor.Text = vm.Editor.Text;
            editor.MoveCaretWithoutFocus(vm.Editor.Text.IndexOf("OnlyTen", StringComparison.Ordinal));

            vm.SelectedWorkflowProject = owner;
            vm.SelectedWorkflowFramework = "net10.0";
            await vm.RequestQuickInfoCommand.ExecuteAsync(null);
            StringAssert.Contains(vm.QuickInfoText, "OnlyTen");

            vm.SelectedWorkflowProject = other;
            vm.SelectedWorkflowFramework = "net10.0";
            vm.Editor.NotificationText = string.Empty;
            await vm.RequestQuickInfoCommand.ExecuteAsync(null);
            StringAssert.Contains(vm.Editor.NotificationText, "select a target framework");
            Assert.IsFalse(vm.QuickInfoText.Contains("OnlyTen", StringComparison.Ordinal));

            await File.WriteAllTextAsync(Path.Combine(root, "Ambiguous.csproj"), "<Project />");
            await File.WriteAllTextAsync(solution,
                "<Solution><Project Path=\"Sample.csproj\" /><Project Path=\"Ambiguous.csproj\" /></Solution>");
            await vm.OpenWorkspaceFileAsync(solution);
            vm.SelectedWorkflowProject = vm.Workspace!.Projects.Single(p => Path.GetFileName(p.ProjectPath) == "Sample.csproj");
            vm.SelectedWorkflowFramework = "net10.0";
            await vm.RequestQuickInfoCommand.ExecuteAsync(null);
            Assert.IsFalse(vm.QuickInfoText.Contains("OnlyTen", StringComparison.Ordinal));
            await WaitUntilAsync(() => vm.Editor.NotificationText.Contains("multiple workspace projects", StringComparison.Ordinal));
        });
    }

    [TestMethod]
    public void IsolatedResults_RejectSkippedOrUnmatchedTests()
    {
        XDocument skipped = XDocument.Parse("<TestRun><UnitTestResult testName=\"Expected\" outcome=\"NotExecuted\" /></TestRun>");
        XDocument unmatched = XDocument.Parse("<TestRun><UnitTestResult testName=\"Other\" outcome=\"Passed\" /></TestRun>");
        XDocument passed = XDocument.Parse("<TestRun><UnitTestResult testName=\"Expected\" outcome=\"Passed\" /></TestRun>");

        Assert.ThrowsExactly<AssertFailedException>(() => AssertPassedResult(skipped, "Expected", string.Empty));
        Assert.ThrowsExactly<AssertFailedException>(() => AssertPassedResult(unmatched, "Expected", string.Empty));
        AssertPassedResult(passed, "Expected", string.Empty);
    }

    private static async Task RunIsolatedAsync(string methodName, Func<Task> test)
    {
        if (Environment.GetEnvironmentVariable(IsolatedProcessVariable) == "1")
        {
            await Dispatcher.UIThread.InvokeAsync(test);
            return;
        }

        string resultsDirectory = Path.Combine(Path.GetTempPath(), $"workbench-tfm-results-{Guid.NewGuid():N}");
        Directory.CreateDirectory(resultsDirectory);
        try
        {
            ProcessStartInfo start = new("dotnet")
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            };
            start.Environment[IsolatedProcessVariable] = "1";
            start.ArgumentList.Add("vstest");
            start.ArgumentList.Add(typeof(WorkbenchTargetFrameworkTests).Assembly.Location);
            start.ArgumentList.Add($"--TestCaseFilter:FullyQualifiedName~{methodName}");
            start.ArgumentList.Add("--logger:console;verbosity=normal");
            start.ArgumentList.Add("--logger:trx;LogFileName=result.trx");
            start.ArgumentList.Add($"--ResultsDirectory:{resultsDirectory}");
            using Process process = Process.Start(start)!;
            Task<string> output = process.StandardOutput.ReadToEndAsync();
            Task<string> error = process.StandardError.ReadToEndAsync();
            using CancellationTokenSource timeout = new(TimeSpan.FromMinutes(2));
            try
            {
                await process.WaitForExitAsync(timeout.Token);
            }
            catch (OperationCanceledException) when (timeout.IsCancellationRequested)
            {
                try
                {
                    process.Kill(entireProcessTree: true);
                }
                catch (InvalidOperationException) when (process.HasExited)
                {
                }
                await process.WaitForExitAsync();
                Assert.Fail($"Timed out running {methodName} in the isolated test host.");
            }
            string result = await output + Environment.NewLine + await error;
            Assert.AreEqual(0, process.ExitCode, result);
            string trxPath = Path.Combine(resultsDirectory, "result.trx");
            Assert.IsTrue(File.Exists(trxPath), result);
            AssertPassedResult(XDocument.Load(trxPath), methodName, result);
        }
        finally
        {
            Directory.Delete(resultsDirectory, recursive: true);
        }
    }

    private static void AssertPassedResult(XDocument document, string methodName, string output)
    {
        var results = document.Descendants().Where(element => element.Name.LocalName == "UnitTestResult").ToArray();
        Assert.AreEqual(1, results.Length, output);
        Assert.AreEqual(methodName, (string?)results[0].Attribute("testName"), output);
        Assert.AreEqual("Passed", (string?)results[0].Attribute("outcome"), output);
    }

    private static async Task WaitUntilAsync(Func<bool> condition)
    {
        Stopwatch timeout = Stopwatch.StartNew();
        while (!condition() && timeout.Elapsed < TimeSpan.FromSeconds(30))
        {
            await Task.Delay(50);
        }
        Assert.IsTrue(condition(), "The expected language result did not arrive.");
    }

    private sealed class Fixture : IDisposable
    {
        public const string SavedApi = "public static class TargetApi { }";
        public const string SavedConsumer = "public class SavedConsumer { }";
        private const string DraftApi = """
            #if NET10_0
            public static class TargetApi { public static string OnlyTen(string value) => value; }
            #else
            public static class TargetApi { public static string OnlyNine(string value) => value; }
            #endif
            """;
        private const string DraftConsumer = "public class Consumer { string Read() => TargetApi.OnlyTen(\"value\"); }";
        private readonly string _root;
        private readonly EditorDocumentWorkspace _documents;
        public string ApiPath { get; }
        public string ConsumerPath { get; }
        public CSharpSyntaxDiagnosticService Service { get; }
        public MainWindowViewModel ViewModel { get; }

        private Fixture(string root)
        {
            _root = root;
            ApiPath = Path.Combine(root, "TargetApi.cs");
            ConsumerPath = Path.Combine(root, "Consumer.cs");
            Service = new CSharpSyntaxDiagnosticService();
            EditorFactory factory = new();
            IAvaloniaEditorComponent initial = factory.Create();
            _documents = new EditorDocumentWorkspace(initial, factory);
            ViewModel = new MainWindowViewModel(initial,
                new PlanningExplorerViewModel(new MarkdownPlanningReader()),
                new DiagnosticCollectionViewModel(), null, null,
                editorWorkspace: _documents, csharpSyntaxDiagnosticService: Service);
        }

        public static async Task<Fixture> CreateAsync()
        {
            string root = Path.Combine(Directory.GetCurrentDirectory(), $"workbench-tfm-{Guid.NewGuid():N}");
            Directory.CreateDirectory(root);
            string project = Path.Combine(root, "Sample.csproj");
            await File.WriteAllTextAsync(project, """
                <Project Sdk="Microsoft.NET.Sdk">
                  <PropertyGroup><TargetFrameworks>net9.0;net10.0</TargetFrameworks></PropertyGroup>
                </Project>
                """);
            await File.WriteAllTextAsync(Path.Combine(root, "TargetApi.cs"), SavedApi);
            await File.WriteAllTextAsync(Path.Combine(root, "Consumer.cs"), SavedConsumer);
            ProcessStartInfo start = new("dotnet") { UseShellExecute = false, RedirectStandardOutput = true, RedirectStandardError = true };
            start.ArgumentList.Add("restore");
            start.ArgumentList.Add(project);
            foreach (string key in new[] { "MSBuildSDKsPath", "MSBuildExtensionsPath", "MSBuildExtensionsPath32", "MSBuildToolsPath", "DOTNET_MSBUILD_SDK_RESOLVER_SDKS_DIR", "DOTNET_MSBUILD_SDK_RESOLVER_CLI_DIR" })
                start.Environment.Remove(key);
            using Process process = Process.Start(start)!;
            Task<string> output = process.StandardOutput.ReadToEndAsync();
            Task<string> error = process.StandardError.ReadToEndAsync();
            await process.WaitForExitAsync();
            Assert.AreEqual(0, process.ExitCode, await output + await error);
            Fixture fixture = new(root);
            await fixture.ViewModel.OpenWorkspaceFileAsync(project);
            return fixture;
        }

        public async Task OpenDraftsAsync()
        {
            await _documents.OpenOrActivateAsync(ApiPath);
            ViewModel.Editor.Text = DraftApi;
            await _documents.OpenOrActivateAsync(ConsumerPath);
            ViewModel.Editor.Text = DraftConsumer;
        }

        public void Dispose()
        {
            _documents.Dispose();
            Service.Dispose();
            Directory.Delete(_root, recursive: true);
        }
    }

    private sealed class EditorFactory : IAvaloniaEditorComponentFactory
    {
        public IAvaloniaEditorComponent Create()
        {
            EditorWorkflow workflow = new(new FileEditorDocument(), new DesignEditorFileDialogService(), new DesignTextDocumentStorageService());
            return new AvaloniaEditorComponent(workflow, new EditorViewModel(workflow), new EditorTextArea());
        }

        IEditorComponent IEditorComponentFactory.Create() => Create();
    }
}
