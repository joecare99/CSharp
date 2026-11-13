using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AA98_AvlnCodeStudio.Base.Building.Models;
using AA98_AvlnCodeStudio.Base.Building.Services;
using AA98_AvlnCodeStudio.Base.Debugging.Models;
using AA98_AvlnCodeStudio.Base.Debugging.Services;
using AA98_AvlnCodeStudio.Base.Testing.Models;
using AA98_AvlnCodeStudio.Base.Testing.Services;
using AA98_AvlnCodeStudio.Diagnostics.UI.ViewModels;
using AA98_AvlnCodeStudio.Editor.Services;
using AA98_AvlnCodeStudio.Model.Documents;
using AA98_AvlnCodeStudio.Model.Workspaces;
using AA98_AvlnCodeStudio.Planning.Core.Services;
using AA98_AvlnCodeStudio.UI.Components;
using AA98_AvlnCodeStudio.UI.Services;
using AA98_AvlnCodeStudio.UI.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
#if NET10_0
using AA98_AvlnCodeStudio.Building.Workbench.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
#endif

namespace AA98_AvlnCodeStudio.Tests.Engineering;

[TestClass]
public sealed class ProjectWorkflowViewModelTests
{
    [TestMethod]
    public async Task Selection_IsExplicitAndBuildUsesOnlyTheInspectedProject()
    {
        using Fixture fixture = await Fixture.CreateAsync();
        MainWindowViewModel vm = fixture.ViewModel;
        Assert.IsNull(vm.SelectedWorkflowProject);
        Assert.IsFalse(vm.InspectSelectedProjectCommand.CanExecute(null));
        vm.SelectedWorkflowProject = vm.Workspace!.Projects[1];
        vm.WorkflowConfiguration = "Release";
        await fixture.Resolver.DidNotReceive().InspectAsync(Arg.Any<ProjectWorkflowContext>(), Arg.Any<CancellationToken>());
        Assert.IsFalse(vm.BuildSelectedProjectCommand.CanExecute(null));

        await vm.InspectSelectedProjectCommand.ExecuteAsync(null);
        Assert.AreEqual("net8.0", vm.SelectedWorkflowFramework);
        Assert.IsTrue(vm.BuildSelectedProjectCommand.CanExecute(null));
        await vm.BuildSelectedProjectCommand.ExecuteAsync(null);

        await fixture.Builder.Received(1).BuildProjectAsync(
            Arg.Is<BuilderBuildRequest>(request =>
                request.ProjectPath == fixture.SecondProject
                && request.Configuration == "Release"
                && request.TargetFramework == "net8.0"),
            Arg.Any<CancellationToken>());
        StringAssert.Contains(vm.ProjectWorkflowStatus, "Build succeeded");

        vm.WorkflowConfiguration = "Debug";
        Assert.IsNull(vm.ProjectInspection);
        Assert.IsFalse(vm.BuildSelectedProjectCommand.CanExecute(null));
    }

    [TestMethod]
    public async Task MultipleFrameworks_RequireASelectionAndAnotherInspection()
    {
        using Fixture fixture = await Fixture.CreateAsync();
        MainWindowViewModel vm = fixture.ViewModel;
        fixture.Resolver.InspectAsync(Arg.Any<ProjectWorkflowContext>(), Arg.Any<CancellationToken>())
            .Returns(call =>
            {
                ProjectWorkflowContext context = call.Arg<ProjectWorkflowContext>();
                return Task.FromResult(new ProjectTargetInspection
                {
                    Context = context,
                    Configurations = ["Debug"],
                    TargetFrameworks = ["net8.0", "net9.0"],
                    TargetFramework = context.TargetFramework,
                    TargetPath = context.TargetFramework is null
                        ? null
                        : Path.Combine(fixture.Root, "App.dll"),
                    OutputType = "Exe",
                    FrameworkIdentifier = ".NETCoreApp",
                    DebugUnavailableReason = context.TargetFramework is null ? "Choose a framework." : null,
                });
            });
        vm.SelectedWorkflowProject = vm.Workspace!.Projects[0];
        await vm.InspectSelectedProjectCommand.ExecuteAsync(null);
        Assert.AreEqual(2, vm.WorkflowFrameworks.Count);
        Assert.IsNull(vm.SelectedWorkflowFramework);
        Assert.IsFalse(vm.BuildSelectedProjectCommand.CanExecute(null));

        vm.SelectedWorkflowFramework = "net9.0";
        Assert.IsNull(vm.ProjectInspection);
        await vm.InspectSelectedProjectCommand.ExecuteAsync(null);
        Assert.AreEqual("net9.0", vm.ProjectInspection?.TargetFramework);
        Assert.IsTrue(vm.BuildSelectedProjectCommand.CanExecute(null));
    }

    [TestMethod]
    public async Task DebugSelectedProject_BuildsAndLaunchesOnlyTheVerifiedArtifact()
    {
        using Fixture fixture = await Fixture.CreateAsync();
        MainWindowViewModel vm = fixture.ViewModel;
        vm.SelectedWorkflowProject = vm.Workspace!.Projects[0];
        await vm.InspectSelectedProjectCommand.ExecuteAsync(null);
        string targetPath = fixture.CreateRuntimeFiles(fixture.FirstProject);

        await vm.DebugSelectedProjectCommand.ExecuteAsync(null);

        await fixture.Debugger.Received(1).StartSessionAsync(
            Arg.Is<DebugLaunchRequest>(request =>
                request.Target == targetPath
                && request.WorkingDirectory == Path.GetDirectoryName(fixture.FirstProject)
                && request.Arguments.Count == 0
                && request.StopAtEntry),
            Arg.Any<CancellationToken>());
        await fixture.RuntimeBuilder.Received(1).BuildAsync(
            Arg.Is<BuilderBuildRequest>(request => request.ProjectPath == fixture.FirstProject
                && request.Configuration == "Debug"
                && request.TargetFramework == "net8.0"),
            Arg.Any<CancellationToken>());
        Assert.IsTrue(vm.HasDebugSession);
        Assert.IsFalse(vm.DebugSelectedProjectCommand.CanExecute(null));
        Assert.IsTrue(vm.StopDebugSessionCommand.CanExecute(null));
    }

    [TestMethod]
    [DataRow("build-failed")]
    [DataRow("wrong-artifact")]
    [DataRow("missing-runtime")]
    [DataRow("metadata-changed")]
    [DataRow("runtime-build-failed")]
    public async Task DebugSelectedProject_RejectsUnsafeOrStaleResults(string scenario)
    {
        using Fixture fixture = await Fixture.CreateAsync();
        MainWindowViewModel vm = fixture.ViewModel;
        vm.SelectedWorkflowProject = vm.Workspace!.Projects[0];
        await vm.InspectSelectedProjectCommand.ExecuteAsync(null);
        string target = fixture.CreateRuntimeFiles(fixture.FirstProject);

        switch (scenario)
        {
            case "build-failed":
                fixture.Builder.BuildProjectAsync(Arg.Any<BuilderBuildRequest>(), Arg.Any<CancellationToken>())
                    .Returns(Task.FromResult(new BuilderBuildResult { ProjectPath = fixture.FirstProject, Succeeded = false }));
                break;
            case "wrong-artifact":
                fixture.Builder.BuildProjectAsync(Arg.Any<BuilderBuildRequest>(), Arg.Any<CancellationToken>())
                    .Returns(Task.FromResult(new BuilderBuildResult
                    {
                        ProjectPath = fixture.FirstProject,
                        Succeeded = true,
                        Artifacts =
                        {
                            new BuilderCompilationArtifact
                            {
                                Kind = "PrimaryOutput",
                                Path = Path.Combine(Path.GetDirectoryName(target)!, "Wrong.dll"),
                                TargetFramework = "net8.0",
                            },
                        },
                    }));
                break;
            case "missing-runtime":
                File.Delete(Path.ChangeExtension(target, ".runtimeconfig.json"));
                break;
            case "metadata-changed":
                fixture.OutputName = "Changed";
                break;
            case "runtime-build-failed":
                fixture.RuntimeBuilder.BuildAsync(Arg.Any<BuilderBuildRequest>(), Arg.Any<CancellationToken>())
                    .Returns(Task.FromException(new InvalidOperationException("Runtime build failed.")));
                break;
        }

        await vm.DebugSelectedProjectCommand.ExecuteAsync(null);
        await fixture.Debugger.DidNotReceive().StartSessionAsync(Arg.Any<DebugLaunchRequest>(), Arg.Any<CancellationToken>());
        StringAssert.Contains(vm.ProjectWorkflowStatus, scenario == "build-failed" ? "failed" : "failed:");
        if (scenario == "metadata-changed")
        {
            Assert.IsNull(vm.ProjectInspection);
            await fixture.Builder.DidNotReceive().BuildProjectAsync(Arg.Any<BuilderBuildRequest>(), Arg.Any<CancellationToken>());
        }
    }

    [TestMethod]
    public async Task DebugSelectedProject_SelectionChangeCancelsLateBuildAndBlocksConcurrentActions()
    {
        using Fixture fixture = await Fixture.CreateAsync();
        MainWindowViewModel vm = fixture.ViewModel;
        vm.SelectedWorkflowProject = vm.Workspace!.Projects[0];
        await vm.InspectSelectedProjectCommand.ExecuteAsync(null);
        vm.DebugTargetPath = fixture.CreateRuntimeFiles(fixture.FirstProject);
        Assert.IsTrue(vm.StartDebugSessionCommand.CanExecute(null));
        TaskCompletionSource<bool> entered = NewCompletion<bool>();
        TaskCompletionSource<BuilderBuildResult> completed = NewCompletion<BuilderBuildResult>();
        fixture.Builder.BuildProjectAsync(Arg.Any<BuilderBuildRequest>(), Arg.Any<CancellationToken>())
            .Returns(_ =>
            {
                entered.TrySetResult(true);
                return completed.Task;
            });

        Task operation = vm.DebugSelectedProjectCommand.ExecuteAsync(null);
        await entered.Task.WaitAsync(TimeSpan.FromSeconds(10));
        Assert.IsFalse(vm.BuildWorkspaceCommand.CanExecute(null));
        Assert.IsFalse(vm.RunWorkspaceTestsCommand.CanExecute(null));
        Assert.IsFalse(vm.StartDebugSessionCommand.CanExecute(null));
        vm.SelectedWorkflowProject = vm.Workspace.Projects[1];
        completed.SetResult(fixture.SuccessfulBuild(fixture.FirstProject, "Debug"));
        await operation;

        await fixture.Debugger.DidNotReceive().StartSessionAsync(Arg.Any<DebugLaunchRequest>(), Arg.Any<CancellationToken>());
        Assert.IsNull(vm.ProjectInspection);
        Assert.IsFalse(vm.ProjectWorkflowBusy);
        StringAssert.Contains(vm.ProjectWorkflowStatus, "Selection changed");
    }

    [TestMethod]
    public async Task DebugSelectedProject_CancelStopsLaunchAfterBuilderReturns()
    {
        using Fixture fixture = await Fixture.CreateAsync();
        MainWindowViewModel vm = fixture.ViewModel;
        vm.SelectedWorkflowProject = vm.Workspace!.Projects[0];
        await vm.InspectSelectedProjectCommand.ExecuteAsync(null);
        fixture.CreateRuntimeFiles(fixture.FirstProject);
        TaskCompletionSource<bool> entered = NewCompletion<bool>();
        TaskCompletionSource<BuilderBuildResult> completed = NewCompletion<BuilderBuildResult>();
        fixture.Builder.BuildProjectAsync(Arg.Any<BuilderBuildRequest>(), Arg.Any<CancellationToken>())
            .Returns(_ =>
            {
                entered.TrySetResult(true);
                return completed.Task;
            });

        Task operation = vm.DebugSelectedProjectCommand.ExecuteAsync(null);
        await entered.Task.WaitAsync(TimeSpan.FromSeconds(10));
        vm.CancelProjectWorkflowCommand.Execute(null);
        completed.SetResult(fixture.SuccessfulBuild(fixture.FirstProject, "Debug"));
        await operation;

        await fixture.Debugger.DidNotReceive().StartSessionAsync(Arg.Any<DebugLaunchRequest>(), Arg.Any<CancellationToken>());
        StringAssert.Contains(vm.ProjectWorkflowStatus, "canceled");
    }

    [TestMethod]
    public async Task DebugSelectedProject_StopsLateSessionWhenWorkspaceChanges()
    {
        using Fixture fixture = await Fixture.CreateAsync();
        MainWindowViewModel vm = fixture.ViewModel;
        vm.SelectedWorkflowProject = vm.Workspace!.Projects[0];
        await vm.InspectSelectedProjectCommand.ExecuteAsync(null);
        fixture.CreateRuntimeFiles(fixture.FirstProject);
        TaskCompletionSource<bool> entered = NewCompletion<bool>();
        TaskCompletionSource<DebugSessionInfo> completed = NewCompletion<DebugSessionInfo>();
        fixture.Debugger.StartSessionAsync(Arg.Any<DebugLaunchRequest>(), Arg.Any<CancellationToken>())
            .Returns(_ =>
            {
                entered.TrySetResult(true);
                return completed.Task;
            });

        Task operation = vm.DebugSelectedProjectCommand.ExecuteAsync(null);
        await entered.Task.WaitAsync(TimeSpan.FromSeconds(10));
        await vm.OpenWorkspaceFileAsync(fixture.SecondProject);
        completed.SetResult(new DebugSessionInfo { SessionId = "late-session", State = DebugSessionState.Running });
        await operation;

        await fixture.Debugger.Received(1).StopSessionAsync("late-session", CancellationToken.None);
        Assert.IsFalse(vm.HasDebugSession);
        Assert.IsNull(vm.SelectedWorkflowProject);
        Assert.IsNull(vm.ProjectInspection);
    }

    [TestMethod]
    public async Task DebugSelectedProject_SelectionChangeDiscardsLateRuntimeBuild()
    {
        using Fixture fixture = await Fixture.CreateAsync();
        MainWindowViewModel vm = fixture.ViewModel;
        vm.SelectedWorkflowProject = vm.Workspace!.Projects[0];
        await vm.InspectSelectedProjectCommand.ExecuteAsync(null);
        fixture.CreateRuntimeFiles(fixture.FirstProject);
        TaskCompletionSource<bool> entered = NewCompletion<bool>();
        TaskCompletionSource<bool> completed = NewCompletion<bool>();
        fixture.RuntimeBuilder.BuildAsync(Arg.Any<BuilderBuildRequest>(), Arg.Any<CancellationToken>())
            .Returns(_ =>
            {
                entered.TrySetResult(true);
                return completed.Task;
            });

        Task operation = vm.DebugSelectedProjectCommand.ExecuteAsync(null);
        await entered.Task.WaitAsync(TimeSpan.FromSeconds(10));
        vm.SelectedWorkflowProject = vm.Workspace.Projects[1];
        completed.SetResult(true);
        await operation;

        await fixture.Debugger.DidNotReceive().StartSessionAsync(Arg.Any<DebugLaunchRequest>(), Arg.Any<CancellationToken>());
        Assert.IsNull(vm.ProjectInspection);
    }

    [TestMethod]
    public async Task TestSelectedProject_UsesSelectionAndPreservesSafeDiagnosticPaths()
    {
        using Fixture fixture = await Fixture.CreateAsync();
        MainWindowViewModel vm = fixture.ViewModel;
        string source = Path.Combine(fixture.Root, "Tests.cs");
        string outside = Path.Combine(Path.GetDirectoryName(fixture.Root)!, "OutsideTests.cs");
        fixture.Tests.RunTestsAsync(Arg.Any<TestRunRequest>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new TestRunSummary
            {
                Outcome = TestRunOutcome.Failed,
                TotalCount = 2,
                FailedCount = 2,
                Failures =
                {
                    new TestRunFailure { TestName = "Inside", Message = "failed", SourcePath = source, LineNumber = 7 },
                    new TestRunFailure { TestName = "Outside", Message = "failed", SourcePath = outside, LineNumber = 9 },
                },
                Cases =
                {
                    new TestCaseResult("Case(1)", "Passed", null, "Example.Tests", "Example.Tests.Case(1)", "1", "a"),
                    new TestCaseResult("Case(2)", "Failed", null, "Example.Tests", "Example.Tests.Case(2)", "2", "b"),
                },
            }));
        vm.SelectedWorkflowProject = vm.Workspace!.Projects[1];
        vm.WorkflowConfiguration = "Release";
        await vm.InspectSelectedProjectCommand.ExecuteAsync(null);
        await vm.TestSelectedProjectCommand.ExecuteAsync(null);

        await fixture.Tests.Received(1).RunTestsAsync(
            Arg.Is<TestRunRequest>(request => request.ProjectPath == fixture.SecondProject
                && request.WorkspaceRootPath == fixture.Root
                && request.Configuration == "Release"
                && request.TargetFramework == "net8.0"),
            Arg.Any<CancellationToken>());
        Assert.AreEqual(source, vm.DiagnosticCollection.Items[0].SourcePath);
        Assert.AreEqual(7, vm.DiagnosticCollection.Items[0].LineNumber);
        Assert.IsNull(vm.DiagnosticCollection.Items[1].SourcePath);
        Assert.IsNull(vm.DiagnosticCollection.Items[1].LineNumber);
        StringAssert.Contains(vm.ProjectWorkflowStatus, "Project tests");
        Assert.AreEqual("Example.Tests", vm.ProjectTestResults.Single().Label);
        CollectionAssert.AreEqual(new[] { "Passed: Case(1)", "Failed: Case(2)" },
            vm.ProjectTestResults.Single().Children.Select(item => item.Label).ToArray());
        vm.SelectedWorkflowProject = vm.Workspace.Projects[0];
        Assert.AreEqual(0, vm.ProjectTestResults.Count);
    }

    [TestMethod]
    public async Task FindTestCandidates_DiscardsRequestWhenProjectChanges()
    {
        using Fixture fixture = await Fixture.CreateAsync();
        MainWindowViewModel vm = fixture.ViewModel;
        vm.SelectedWorkflowProject = vm.Workspace!.Projects[0];
        await vm.InspectSelectedProjectCommand.ExecuteAsync(null);
        TaskCompletionSource<bool> entered = NewCompletion<bool>();
        TaskCompletionSource<ProjectTargetInspection> completed = NewCompletion<ProjectTargetInspection>();
        fixture.Resolver.InspectAsync(Arg.Any<ProjectWorkflowContext>(), Arg.Any<CancellationToken>())
            .Returns(_ =>
            {
                entered.TrySetResult(true);
                return completed.Task;
            });
        ProjectTargetInspection previous = vm.ProjectInspection!;
        Task operation = vm.FindTestCandidatesCommand.ExecuteAsync(null);
        await entered.Task.WaitAsync(TimeSpan.FromSeconds(10));
        vm.SelectedWorkflowProject = vm.Workspace.Projects[1];
        completed.SetResult(previous);
        await operation;
        Assert.AreEqual(0, vm.TestMethodCandidates.Count);
        Assert.IsNull(vm.ProjectInspection);
    }

    [TestMethod]
    public async Task DiscoverAdapterTests_UsesBuiltProjectAndDiscardsStaleResults()
    {
        using Fixture fixture = await Fixture.CreateAsync();
        MainWindowViewModel vm = fixture.ViewModel;
        vm.SelectedWorkflowProject = vm.Workspace!.Projects[1];
        await vm.InspectSelectedProjectCommand.ExecuteAsync(null);
        TaskCompletionSource<bool> entered = NewCompletion<bool>();
        TaskCompletionSource<TestDiscoveryResult> completed = NewCompletion<TestDiscoveryResult>();
        fixture.Discovery.DiscoverAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(_ =>
            {
                entered.TrySetResult(true);
                return completed.Task;
            });
        Task operation = vm.DiscoverAdapterTestsCommand.ExecuteAsync(null);
        await entered.Task.WaitAsync(TimeSpan.FromSeconds(10));
        await fixture.RuntimeBuilder.Received(1).BuildAsync(
            Arg.Is<BuilderBuildRequest>(request => request.ProjectPath == fixture.SecondProject
                && request.Configuration == "Debug" && request.TargetFramework == "net8.0"),
            Arg.Any<CancellationToken>());
        _ = fixture.Discovery.Received(1).DiscoverAsync(
            Arg.Is<string>(path => path == vm.ProjectInspection!.TargetPath),
            Path.GetDirectoryName(fixture.SecondProject)!,
            Arg.Any<CancellationToken>());
        vm.SelectedWorkflowProject = vm.Workspace.Projects[0];
        completed.SetResult(new TestDiscoveryResult(
            [new DiscoveredTestCase("id", "executor", "source", "Example.Test", "Old case", null, null)], []));
        await operation;
        Assert.AreEqual(0, vm.DiscoveredTestCases.Count);
        Assert.IsNull(vm.ProjectInspection);
    }

    [TestMethod]
    public async Task DiscoverAdapterTests_ShowsCasesAndWarnings()
    {
        using Fixture fixture = await Fixture.CreateAsync();
        MainWindowViewModel vm = fixture.ViewModel;
        fixture.Discovery.DiscoverAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new TestDiscoveryResult(
                [new DiscoveredTestCase("id", "executor", "source", "Example.Test(1)", "Test(1)", null, null)],
                ["Adapter warning"])));
        vm.SelectedWorkflowProject = vm.Workspace!.Projects[1];
        await vm.InspectSelectedProjectCommand.ExecuteAsync(null);
        await vm.DiscoverAdapterTestsCommand.ExecuteAsync(null);
        Assert.AreEqual("Test(1)", vm.DiscoveredTestCases.Single().DisplayName);
        StringAssert.Contains(vm.ProjectWorkflowStatus, "Adapter warning");
        vm.SelectedWorkflowProject = vm.Workspace.Projects[0];
        Assert.AreEqual(0, vm.DiscoveredTestCases.Count);
    }

    [TestMethod]
    public async Task RunDiscoveredTestCase_UsesSelectionAndDiscardsChangedSelection()
    {
        using Fixture fixture = await Fixture.CreateAsync();
        MainWindowViewModel vm = fixture.ViewModel;
        vm.SelectedWorkflowProject = vm.Workspace!.Projects[1];
        await vm.InspectSelectedProjectCommand.ExecuteAsync(null);
        DiscoveredTestCase first = new("id1", "executor", "assembly", "Example.First", "First", null, null);
        DiscoveredTestCase second = new("id2", "executor", "assembly", "Example.Second", "Second", null, null);
        vm.DiscoveredTestCases.Add(first);
        vm.DiscoveredTestCases.Add(second);
        vm.SelectedDiscoveredTestCase = first;
        TaskCompletionSource<bool> entered = NewCompletion<bool>();
        TaskCompletionSource<TestRunSummary> completed = NewCompletion<TestRunSummary>();
        fixture.CaseExecution.RunCaseAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DiscoveredTestCase>(), Arg.Any<CancellationToken>())
            .Returns(_ =>
            {
                entered.TrySetResult(true);
                return completed.Task;
            });
        Task operation = vm.RunDiscoveredTestCaseCommand.ExecuteAsync(null);
        await entered.Task.WaitAsync(TimeSpan.FromSeconds(10));
        _ = fixture.CaseExecution.Received(1).RunCaseAsync(
            vm.ProjectInspection!.TargetPath!, Path.GetDirectoryName(fixture.SecondProject)!, first, Arg.Any<CancellationToken>());
        vm.SelectedDiscoveredTestCase = second;
        completed.SetResult(new TestRunSummary
        {
            Cases = { new TestCaseResult("First", "Passed", null, null, "Example.First", "id1", null) },
        });
        await operation;
        Assert.AreEqual(0, vm.ProjectTestResults.Count);
    }

    [TestMethod]
    public async Task TestSelectedMethod_UsesExactFilterAndInvalidatesLateResult()
    {
        using Fixture fixture = await Fixture.CreateAsync();
        MainWindowViewModel vm = fixture.ViewModel;
        vm.SelectedWorkflowProject = vm.Workspace!.Projects[1];
        await vm.InspectSelectedProjectCommand.ExecuteAsync(null);
        Assert.IsFalse(vm.TestSelectedMethodCommand.CanExecute(null));
        vm.WorkflowTestMethod = "  Example.Tests.Passing  ";
        Assert.IsTrue(vm.TestSelectedMethodCommand.CanExecute(null));

        TaskCompletionSource<bool> entered = NewCompletion<bool>();
        TaskCompletionSource<TestRunSummary> completed = NewCompletion<TestRunSummary>();
        fixture.Tests.RunTestsAsync(Arg.Any<TestRunRequest>(), Arg.Any<CancellationToken>())
            .Returns(_ =>
            {
                entered.TrySetResult(true);
                return completed.Task;
            });
        Task operation = vm.TestSelectedMethodCommand.ExecuteAsync(null);
        await entered.Task.WaitAsync(TimeSpan.FromSeconds(10));
        await fixture.Tests.Received(1).RunTestsAsync(
            Arg.Is<TestRunRequest>(request => request.ProjectPath == fixture.SecondProject
                && request.MatchTargetsExactly
                && request.Targets.Count == 1
                && request.Targets[0] == "Example.Tests.Passing"
                && request.TargetFramework == "net8.0"),
            Arg.Any<CancellationToken>());
        vm.WorkflowTestMethod = "Example.Tests.Other";
        completed.SetResult(new TestRunSummary
        {
            FailedCount = 1,
            Failures = { new TestRunFailure { TestName = "Stale", Message = "stale" } },
            Cases = { new TestCaseResult("Stale", "Failed", null, "Example", "Example.Stale", "1", "a") },
        });
        await operation;
        Assert.IsFalse(vm.DiagnosticCollection.Items.Any(item => item.Message.Contains("Stale", StringComparison.Ordinal)));
        Assert.AreEqual(0, vm.ProjectTestResults.Count);
        vm.SelectedWorkflowProject = vm.Workspace.Projects[0];
        Assert.AreEqual(string.Empty, vm.WorkflowTestMethod);
        Assert.IsFalse(vm.TestSelectedMethodCommand.CanExecute(null));
    }

    [TestMethod]
    public async Task TestSelectedProject_DiscardsLateResultsAfterWorkspaceChange()
    {
        using Fixture fixture = await Fixture.CreateAsync();
        MainWindowViewModel vm = fixture.ViewModel;
        vm.SelectedWorkflowProject = vm.Workspace!.Projects[0];
        await vm.InspectSelectedProjectCommand.ExecuteAsync(null);
        TaskCompletionSource<bool> entered = NewCompletion<bool>();
        TaskCompletionSource<TestRunSummary> completed = NewCompletion<TestRunSummary>();
        fixture.Tests.RunTestsAsync(Arg.Any<TestRunRequest>(), Arg.Any<CancellationToken>())
            .Returns(_ =>
            {
                entered.TrySetResult(true);
                return completed.Task;
            });
        Task operation = vm.TestSelectedProjectCommand.ExecuteAsync(null);
        await entered.Task.WaitAsync(TimeSpan.FromSeconds(10));
        await vm.OpenWorkspaceFileAsync(fixture.SecondProject);
        completed.SetResult(new TestRunSummary
        {
            FailedCount = 1,
            Failures = { new TestRunFailure { TestName = "Stale", Message = "stale" } },
        });
        await operation;

        Assert.IsFalse(vm.DiagnosticCollection.Items.Any(item => item.Message.Contains("Stale", StringComparison.Ordinal)));
        Assert.IsNull(vm.SelectedWorkflowProject);
    }

    [TestMethod]
    public async Task WorkspaceTests_DiscardLateResultsWhenEntryChangesWithinSameRoot()
    {
        using Fixture fixture = await Fixture.CreateAsync();
        MainWindowViewModel vm = fixture.ViewModel;
        TaskCompletionSource<bool> entered = NewCompletion<bool>();
        TaskCompletionSource<TestRunSummary> completed = NewCompletion<TestRunSummary>();
        fixture.Tests.RunTestsAsync(Arg.Any<TestRunRequest>(), Arg.Any<CancellationToken>())
            .Returns(_ =>
            {
                entered.TrySetResult(true);
                return completed.Task;
            });
        Task operation = vm.RunWorkspaceTestsCommand.ExecuteAsync(null);
        await entered.Task.WaitAsync(TimeSpan.FromSeconds(10));
        string otherSolution = Path.Combine(fixture.Root, "Other.slnx");
        await File.WriteAllTextAsync(otherSolution, "<Solution><Project Path=\"second\\Tests.csproj\" /></Solution>");
        await vm.OpenWorkspaceFileAsync(otherSolution);
        completed.SetResult(new TestRunSummary
        {
            Failures = { new TestRunFailure { TestName = "OldWorkspace", Message = "stale" } },
        });
        await operation;

        Assert.IsFalse(vm.DiagnosticCollection.Items.Any(item => item.Message.Contains("OldWorkspace", StringComparison.Ordinal)));
        Assert.AreEqual(otherSolution, vm.Workspace?.EntryPath);
        Assert.IsFalse(vm.ProjectWorkflowBusy);
    }

    [TestMethod]
    public async Task WorkspaceBuild_DiscardsLateDiagnosticsWhenWorkspaceChanges()
    {
        using Fixture fixture = await Fixture.CreateAsync();
        MainWindowViewModel vm = fixture.ViewModel;
        TaskCompletionSource<bool> entered = NewCompletion<bool>();
        TaskCompletionSource<BuilderBuildResult> completed = NewCompletion<BuilderBuildResult>();
        fixture.Builder.BuildProjectAsync(Arg.Any<BuilderBuildRequest>(), Arg.Any<CancellationToken>())
            .Returns(_ =>
            {
                entered.TrySetResult(true);
                return completed.Task;
            });
        Task operation = vm.BuildWorkspaceCommand.ExecuteAsync(null);
        await entered.Task.WaitAsync(TimeSpan.FromSeconds(10));
        await vm.OpenWorkspaceFileAsync(fixture.SecondProject);
        completed.SetResult(new BuilderBuildResult
        {
            ProjectPath = fixture.FirstProject,
            Diagnostics =
            {
                new BuilderDiagnostic { Code = "OLD", Message = "stale diagnostic" },
            },
        });
        await operation;

        Assert.IsFalse(vm.DiagnosticCollection.Items.Any(item => item.Code == "OLD"));
        Assert.AreEqual(fixture.SecondProject, vm.Workspace?.EntryPath);
        Assert.IsFalse(vm.ProjectWorkflowBusy);
    }

    [TestMethod]
    public async Task Inspection_DiscardsLateResultWhenSelectionChanges()
    {
        using Fixture fixture = await Fixture.CreateAsync();
        MainWindowViewModel vm = fixture.ViewModel;
        TaskCompletionSource<bool> entered = NewCompletion<bool>();
        TaskCompletionSource<ProjectTargetInspection> completed = NewCompletion<ProjectTargetInspection>();
        ProjectWorkflowContext? captured = null;
        fixture.Resolver.InspectAsync(Arg.Any<ProjectWorkflowContext>(), Arg.Any<CancellationToken>())
            .Returns(call =>
            {
                captured = call.Arg<ProjectWorkflowContext>();
                entered.TrySetResult(true);
                return completed.Task;
            });
        vm.SelectedWorkflowProject = vm.Workspace!.Projects[0];
        Task inspection = vm.InspectSelectedProjectCommand.ExecuteAsync(null);
        await entered.Task.WaitAsync(TimeSpan.FromSeconds(10));
        vm.SelectedWorkflowProject = vm.Workspace.Projects[1];
        completed.SetResult(new ProjectTargetInspection
        {
            Context = captured!,
            Configurations = ["Debug"],
            TargetFrameworks = ["net8.0"],
            TargetFramework = "net8.0",
        });
        await inspection;

        Assert.IsNull(vm.ProjectInspection);
        Assert.AreEqual(0, vm.WorkflowFrameworks.Count);
        Assert.AreEqual(fixture.SecondProject, vm.SelectedWorkflowProject.ProjectPath);
    }

#if NET10_0
    [TestMethod]
    public async Task DebugSelectedProject_UsesTheRealBuilderAndProducesARunnableAssembly()
    {
        string root = Path.Combine(Path.GetTempPath(), $"AA98_RealWorkflow_{Guid.NewGuid():N}");
        Directory.CreateDirectory(root);
        try
        {
            string project = Path.Combine(root, "RealApp.csproj");
            await File.WriteAllTextAsync(project,
                "<Project Sdk=\"Microsoft.NET.Sdk\"><PropertyGroup>"
                + "<OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework>"
                + "</PropertyGroup></Project>");
            await File.WriteAllTextAsync(Path.Combine(root, "Program.cs"),
                "System.Console.WriteLine(\"real workflow build\");");
            (int restoreExit, string restoreOutput) = await RunDotNetAsync(root, "restore", project, "--ignore-failed-sources", "-p:NuGetAudit=false");
            Assert.AreEqual(0, restoreExit, restoreOutput);

            using ServiceProvider services = new ServiceCollection().AddWorkbenchCodeStudioBuilding().BuildServiceProvider();
            IDebugSessionService debugger = Substitute.For<IDebugSessionService>();
            debugger.StartSessionAsync(Arg.Any<DebugLaunchRequest>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(new DebugSessionInfo { SessionId = "real-build", State = DebugSessionState.Running }));
            EditorWorkflow workflow = new(new FileEditorDocument(), new DesignEditorFileDialogService(), new DesignTextDocumentStorageService());
            MainWindowViewModel vm = new(
                new AvaloniaEditorComponent(workflow, new EditorViewModel(workflow), null!),
                new PlanningExplorerViewModel(new MarkdownPlanningReader()),
                new DiagnosticCollectionViewModel(),
                null,
                null,
                new CSharpWorkspaceLoader(),
                new DesignWorkspaceFileDialogService(),
                services.GetRequiredService<ICodeStudioBuilderService>(),
                debugSessionService: debugger);
            await vm.OpenWorkspaceFileAsync(project);
            vm.SelectedWorkflowProject = vm.Workspace!.Projects[0];
            await vm.InspectSelectedProjectCommand.ExecuteAsync(null);
            Assert.IsTrue(vm.DebugSelectedProjectCommand.CanExecute(null), vm.ProjectWorkflowStatus);

            await vm.DebugSelectedProjectCommand.ExecuteAsync(null);

            string target = Path.Combine(root, "bin", "Debug", "net10.0", "RealApp.dll");
            Assert.IsTrue(vm.HasDebugSession, vm.ProjectWorkflowStatus + Environment.NewLine
                + string.Join(Environment.NewLine, vm.DiagnosticCollection.Items.Select(item => $"{item.Code}: {item.Message}")));
            await debugger.Received(1).StartSessionAsync(
                Arg.Is<DebugLaunchRequest>(request => request.Target == target),
                Arg.Any<CancellationToken>());
            (int runExit, string runOutput) = await RunDotNetAsync(root, target);
            Assert.AreEqual(0, runExit, vm.ProjectWorkflowStatus + Environment.NewLine + runOutput);
            StringAssert.Contains(runOutput, "real workflow build");

            await vm.StopDebugSessionCommand.ExecuteAsync(null);
            string dependencyDirectory = Path.Combine(root, "Dependency");
            Directory.CreateDirectory(dependencyDirectory);
            string dependencyProject = Path.Combine(dependencyDirectory, "Dependency.csproj");
            await File.WriteAllTextAsync(dependencyProject,
                "<Project Sdk=\"Microsoft.NET.Sdk\"><PropertyGroup>"
                + "<TargetFramework>net10.0</TargetFramework></PropertyGroup></Project>");
            await File.WriteAllTextAsync(Path.Combine(dependencyDirectory, "Greeting.cs"),
                "namespace ProbeDependency; public static class Greeting { public static string Value => \"dependency refreshed\"; }");
            await File.WriteAllTextAsync(project,
                "<Project Sdk=\"Microsoft.NET.Sdk\"><PropertyGroup>"
                + "<OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework>"
                + "</PropertyGroup><ItemGroup><Compile Remove=\"Dependency\\**\\*.cs\" />"
                + "<ProjectReference Include=\"Dependency\\Dependency.csproj\" /></ItemGroup></Project>");
            await File.WriteAllTextAsync(Path.Combine(root, "Program.cs"),
                "System.Console.WriteLine(ProbeDependency.Greeting.Value);");
            (int dependencyExit, string dependencyOutput) = await RunDotNetAsync(
                root, "build", dependencyProject, "-p:NuGetAudit=false", "--verbosity", "quiet");
            Assert.AreEqual(0, dependencyExit, dependencyOutput);
            (restoreExit, restoreOutput) = await RunDotNetAsync(root, "restore", project, "--ignore-failed-sources", "-p:NuGetAudit=false");
            Assert.AreEqual(0, restoreExit, restoreOutput);

            await vm.DebugSelectedProjectCommand.ExecuteAsync(null);
            Assert.IsTrue(vm.HasDebugSession, vm.ProjectWorkflowStatus);
            (runExit, runOutput) = await RunDotNetAsync(root, target);
            Assert.AreEqual(0, runExit, runOutput);
            StringAssert.Contains(runOutput, "dependency refreshed");
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    private static async Task<(int ExitCode, string Output)> RunDotNetAsync(string workingDirectory, params string[] arguments)
    {
        ProcessStartInfo startInfo = new("dotnet")
        {
            WorkingDirectory = workingDirectory,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
        };
        startInfo.Environment.Remove("MSBuildSDKsPath");
        startInfo.Environment.Remove("MSBuildExtensionsPath");
        startInfo.Environment.Remove("MSBuildExtensionsPath32");
        startInfo.Environment.Remove("MSBuildToolsPath");
        startInfo.Environment.Remove("DOTNET_MSBUILD_SDK_RESOLVER_SDKS_DIR");
        startInfo.Environment.Remove("DOTNET_MSBUILD_SDK_RESOLVER_CLI_DIR");
        foreach (string argument in arguments)
        {
            startInfo.ArgumentList.Add(argument);
        }

        using Process process = Process.Start(startInfo)!;
        Task<string> output = process.StandardOutput.ReadToEndAsync();
        Task<string> error = process.StandardError.ReadToEndAsync();
        await process.WaitForExitAsync();
        return (process.ExitCode, await output + Environment.NewLine + await error);
    }
#endif

    private static TaskCompletionSource<T> NewCompletion<T>()
    {
        return new TaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously);
    }

    private sealed class Fixture : IDisposable
    {
        private Fixture(string root)
        {
            Root = root;
            FirstProject = Path.Combine(root, "first", "App.csproj");
            SecondProject = Path.Combine(root, "second", "Tests.csproj");
            Directory.CreateDirectory(Path.GetDirectoryName(FirstProject)!);
            Directory.CreateDirectory(Path.GetDirectoryName(SecondProject)!);
            File.WriteAllText(FirstProject, "<Project />");
            File.WriteAllText(SecondProject, "<Project />");
            Solution = Path.Combine(root, "Projects.slnx");
            File.WriteAllText(Solution, "<Solution><Project Path=\"first\\App.csproj\" /><Project Path=\"second\\Tests.csproj\" /></Solution>");

            Resolver = Substitute.For<IProjectTargetResolver>();
            Resolver.InspectAsync(Arg.Any<ProjectWorkflowContext>(), Arg.Any<CancellationToken>())
                .Returns(call => Task.FromResult(CreateInspection(call.Arg<ProjectWorkflowContext>())));
            Builder = Substitute.For<ICodeStudioBuilderService>();
            Builder.BuildProjectAsync(Arg.Any<BuilderBuildRequest>(), Arg.Any<CancellationToken>())
                .Returns(call =>
                {
                    BuilderBuildRequest request = call.Arg<BuilderBuildRequest>();
                    return Task.FromResult(SuccessfulBuild(request.ProjectPath, request.Configuration!));
                });
            Tests = Substitute.For<ITestExecutionService>();
            Tests.RunTestsAsync(Arg.Any<TestRunRequest>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(new TestRunSummary()));
            Debugger = Substitute.For<IDebugSessionService>();
            Debugger.StartSessionAsync(Arg.Any<DebugLaunchRequest>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(new DebugSessionInfo { SessionId = "session", State = DebugSessionState.Running }));
            RuntimeBuilder = Substitute.For<IProjectRuntimeBuilder>();
            Discovery = Substitute.For<ITestDiscoveryService>();
            CaseExecution = Substitute.For<ITestCaseExecutionService>();

            EditorWorkflow workflow = new(new FileEditorDocument(), new DesignEditorFileDialogService(), new DesignTextDocumentStorageService());
            ViewModel = new MainWindowViewModel(
                new AvaloniaEditorComponent(workflow, new EditorViewModel(workflow), null!),
                new PlanningExplorerViewModel(new MarkdownPlanningReader()),
                new DiagnosticCollectionViewModel(),
                null,
                null,
                new CSharpWorkspaceLoader(),
                new DesignWorkspaceFileDialogService(),
                Builder,
                testExecutionService: Tests,
                debugSessionService: Debugger,
                projectTargetResolver: Resolver,
                projectRuntimeBuilder: RuntimeBuilder,
                testDiscoveryService: Discovery,
                testCaseExecutionService: CaseExecution);
        }

        public string Root { get; }
        public string Solution { get; }
        public string FirstProject { get; }
        public string SecondProject { get; }
        public string? OutputName { get; set; }
        public IProjectTargetResolver Resolver { get; }
        public ICodeStudioBuilderService Builder { get; }
        public ITestExecutionService Tests { get; }
        public IDebugSessionService Debugger { get; }
        public IProjectRuntimeBuilder RuntimeBuilder { get; }
        public ITestDiscoveryService Discovery { get; }
        public ITestCaseExecutionService CaseExecution { get; }
        public MainWindowViewModel ViewModel { get; }

        public static async Task<Fixture> CreateAsync()
        {
            string root = Path.Combine(Path.GetTempPath(), $"AA98_WorkflowTests_{Guid.NewGuid():N}");
            Directory.CreateDirectory(root);
            Fixture fixture = new(root);
            await fixture.ViewModel.OpenWorkspaceFileAsync(fixture.Solution);
            return fixture;
        }

        public string CreateRuntimeFiles(string project, string configuration = "Debug")
        {
            string target = GetTargetPath(project, configuration);
            Directory.CreateDirectory(Path.GetDirectoryName(target)!);
            File.WriteAllText(target, "assembly");
            File.WriteAllText(Path.ChangeExtension(target, ".runtimeconfig.json"), "{}");
            File.WriteAllText(Path.ChangeExtension(target, ".deps.json"), "{}");
            return target;
        }

        public BuilderBuildResult SuccessfulBuild(string project, string configuration)
        {
            return new BuilderBuildResult
            {
                ProjectPath = project,
                Succeeded = true,
                Artifacts =
                {
                    new BuilderCompilationArtifact
                    {
                        Kind = "PrimaryOutput",
                        Path = GetTargetPath(project, configuration),
                        TargetFramework = "net8.0",
                    },
                },
            };
        }

        public void Dispose()
        {
            Directory.Delete(Root, recursive: true);
        }

        private ProjectTargetInspection CreateInspection(ProjectWorkflowContext context)
        {
            return new ProjectTargetInspection
            {
                Context = context,
                Configurations = ["Debug", "Release"],
                TargetFrameworks = ["net8.0"],
                TargetFramework = "net8.0",
                TargetPath = GetTargetPath(context.Project.ProjectPath, context.Configuration, OutputName),
                OutputType = "Exe",
                FrameworkIdentifier = ".NETCoreApp",
                IsTestProject = "false",
            };
        }

        private static string GetTargetPath(string project, string configuration, string? name = null)
        {
            return Path.Combine(
                Path.GetDirectoryName(project)!,
                "bin",
                configuration,
                "net8.0",
                (name ?? Path.GetFileNameWithoutExtension(project)) + ".dll");
        }
    }
}
