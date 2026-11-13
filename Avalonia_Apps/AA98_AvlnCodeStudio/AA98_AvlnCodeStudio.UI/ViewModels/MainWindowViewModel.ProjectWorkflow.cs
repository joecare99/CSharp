using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Threading;
using AA98_AvlnCodeStudio.Base.Building.Models;
using AA98_AvlnCodeStudio.Base.Building.Services;
using AA98_AvlnCodeStudio.Base.Debugging.Models;
using AA98_AvlnCodeStudio.Base.Debugging.Services;
using AA98_AvlnCodeStudio.Base.Testing.Models;
using AA98_AvlnCodeStudio.Base.Testing.Services;
using AA98_AvlnCodeStudio.Model.Workspaces;
using AA98_AvlnCodeStudio.UI.Resources;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AA98_AvlnCodeStudio.UI.ViewModels
{
    public partial class MainWindowViewModel
    {
        private readonly IProjectTargetResolver _projectTargetResolver;
        private readonly IProjectRuntimeBuilder _projectRuntimeBuilder;
        private readonly ITestDiscoveryService _testDiscoveryService;
        private readonly ITestCaseExecutionService _testCaseExecutionService;
        private long _projectWorkflowRevision;
        private bool _updatingProjectInspection;
        private bool _projectActionInProgress;
        private string? _debuggedTestSessionId;

        public ObservableCollection<string> WorkflowFrameworks { get; } = [];
        public ObservableCollection<TestMethodCandidate> TestMethodCandidates { get; } = [];
        public ObservableCollection<DiscoveredTestCase> DiscoveredTestCases { get; } = [];
        public ObservableCollection<TestResultNode> ProjectTestResults { get; } = [];

        public string ProjectWorkflowTabText => UiStrings.ProjectWorkflowTabText;
        public string ProjectWorkflowProjectText => UiStrings.ProjectWorkflowProjectText;
        public string ProjectWorkflowConfigurationText => UiStrings.ProjectWorkflowConfigurationText;
        public string ProjectWorkflowFrameworkText => UiStrings.ProjectWorkflowFrameworkText;
        public string ProjectWorkflowTrustWarningText => UiStrings.ProjectWorkflowTrustWarningText;
        public string InspectProjectCommandText => UiStrings.InspectProjectCommandText;
        public string BuildSelectedProjectCommandText => UiStrings.BuildSelectedProjectCommandText;
        public string TestSelectedProjectCommandText => UiStrings.TestSelectedProjectCommandText;
        public string TestMethodNameText => UiStrings.TestMethodNameText;
        public string TestMethodCommandText => UiStrings.TestMethodCommandText;
        public string TestCandidatesCommandText => UiStrings.TestCandidatesCommandText;
        public string TestCandidatesLabelText => UiStrings.TestCandidatesLabelText;
        public string DiscoverAdapterTestsCommandText => UiStrings.DiscoverAdapterTestsCommandText;
        public string DiscoveredTestCasesLabelText => UiStrings.DiscoveredTestCasesLabelText;
        public string RunDiscoveredTestCaseCommandText => UiStrings.RunDiscoveredTestCaseCommandText;
        public string DebugDiscoveredTestCaseCommandText => UiStrings.DebugDiscoveredTestCaseCommandText;
        public string ProjectTestResultsLabelText => UiStrings.ProjectTestResultsLabelText;
        public string DebugSelectedProjectCommandText => UiStrings.DebugSelectedProjectCommandText;
        public string CancelProjectWorkflowCommandText => UiStrings.CancelProjectWorkflowCommandText;

        [ObservableProperty]
        private CSharpWorkspaceProject? _selectedWorkflowProject;

        [ObservableProperty]
        private string _workflowConfiguration = "Debug";

        [ObservableProperty]
        private string? _selectedWorkflowFramework;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(TestSelectedMethodCommand))]
        private string _workflowTestMethod = string.Empty;

        [ObservableProperty]
        private TestMethodCandidate? _selectedTestMethodCandidate;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(RunDiscoveredTestCaseCommand))]
        [NotifyCanExecuteChangedFor(nameof(DebugDiscoveredTestCaseCommand))]
        private DiscoveredTestCase? _selectedDiscoveredTestCase;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(BuildSelectedProjectCommand))]
        [NotifyCanExecuteChangedFor(nameof(TestSelectedProjectCommand))]
        [NotifyCanExecuteChangedFor(nameof(TestSelectedMethodCommand))]
        [NotifyCanExecuteChangedFor(nameof(FindTestCandidatesCommand))]
        [NotifyCanExecuteChangedFor(nameof(DiscoverAdapterTestsCommand))]
        [NotifyCanExecuteChangedFor(nameof(RunDiscoveredTestCaseCommand))]
        [NotifyCanExecuteChangedFor(nameof(DebugDiscoveredTestCaseCommand))]
        [NotifyCanExecuteChangedFor(nameof(DebugSelectedProjectCommand))]
        private ProjectTargetInspection? _projectInspection;

        [ObservableProperty]
        private string _projectWorkflowStatus = "Select a project and inspect it before running project actions.";

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(InspectSelectedProjectCommand))]
        [NotifyCanExecuteChangedFor(nameof(BuildSelectedProjectCommand))]
        [NotifyCanExecuteChangedFor(nameof(TestSelectedProjectCommand))]
        [NotifyCanExecuteChangedFor(nameof(TestSelectedMethodCommand))]
        [NotifyCanExecuteChangedFor(nameof(FindTestCandidatesCommand))]
        [NotifyCanExecuteChangedFor(nameof(DiscoverAdapterTestsCommand))]
        [NotifyCanExecuteChangedFor(nameof(RunDiscoveredTestCaseCommand))]
        [NotifyCanExecuteChangedFor(nameof(DebugDiscoveredTestCaseCommand))]
        [NotifyCanExecuteChangedFor(nameof(DebugSelectedProjectCommand))]
        [NotifyCanExecuteChangedFor(nameof(BuildWorkspaceCommand))]
        [NotifyCanExecuteChangedFor(nameof(RunWorkspaceTestsCommand))]
        [NotifyCanExecuteChangedFor(nameof(StartDebugSessionCommand))]
        private bool _projectWorkflowBusy;

        partial void OnSelectedWorkflowProjectChanged(CSharpWorkspaceProject? value)
        {
            WorkflowTestMethod = string.Empty;
            InvalidateProjectInspection(clearFrameworks: true);
        }

        partial void OnWorkflowTestMethodChanged(string value)
        {
            TestSelectedMethodCommand.Cancel();
            if (SelectedTestMethodCandidate is not null
                && !string.Equals(value, SelectedTestMethodCandidate.FullyQualifiedName, StringComparison.Ordinal))
            {
                SelectedTestMethodCandidate = null;
            }
        }

        partial void OnSelectedTestMethodCandidateChanged(TestMethodCandidate? value)
        {
            if (value is not null)
            {
                WorkflowTestMethod = value.FullyQualifiedName;
            }
        }

        partial void OnSelectedDiscoveredTestCaseChanged(DiscoveredTestCase? value)
        {
            RunDiscoveredTestCaseCommand.Cancel();
            DebugDiscoveredTestCaseCommand.Cancel();
        }

        partial void OnWorkflowConfigurationChanged(string value)
        {
            InvalidateProjectInspection(clearFrameworks: true);
        }

        partial void OnSelectedWorkflowFrameworkChanged(string? value)
        {
            InvalidateProjectInspection(clearFrameworks: false);
        }

        private void ResetProjectWorkflow()
        {
            InvalidateProjectInspection(clearFrameworks: true);
            _updatingProjectInspection = true;
            try
            {
                SelectedWorkflowProject = null;
                WorkflowConfiguration = "Debug";
            }
            finally
            {
                _updatingProjectInspection = false;
            }

            BuildWorkspaceCommand.Cancel();
            RunWorkspaceTestsCommand.Cancel();
            StartDebugSessionCommand.Cancel();
            InspectSelectedProjectCommand.NotifyCanExecuteChanged();
        }

        private void InvalidateProjectInspection(bool clearFrameworks)
        {
            if (_updatingProjectInspection)
            {
                return;
            }

            Interlocked.Increment(ref _projectWorkflowRevision);
            InspectSelectedProjectCommand.Cancel();
            BuildSelectedProjectCommand.Cancel();
            TestSelectedProjectCommand.Cancel();
            TestSelectedMethodCommand.Cancel();
            FindTestCandidatesCommand.Cancel();
            DiscoverAdapterTestsCommand.Cancel();
            RunDiscoveredTestCaseCommand.Cancel();
            DebugDiscoveredTestCaseCommand.Cancel();
            DebugSelectedProjectCommand.Cancel();
            ProjectInspection = null;
            SelectedTestMethodCandidate = null;
            TestMethodCandidates.Clear();
            SelectedDiscoveredTestCase = null;
            DiscoveredTestCases.Clear();
            ProjectTestResults.Clear();
            if (clearFrameworks)
            {
                _updatingProjectInspection = true;
                try
                {
                    SelectedWorkflowFramework = null;
                    WorkflowFrameworks.Clear();
                }
                finally
                {
                    _updatingProjectInspection = false;
                }
            }

            ProjectWorkflowStatus = "Selection changed. Inspect the selected project before running project actions.";
            InspectSelectedProjectCommand.NotifyCanExecuteChanged();
        }

        [RelayCommand(CanExecute = nameof(CanInspectSelectedProject))]
        private async Task InspectSelectedProjectAsync(CancellationToken cancellationToken)
        {
            if (!CanInspectSelectedProject())
            {
                return;
            }

            CSharpWorkspace workspace = Workspace!;
            long revision = Interlocked.Read(ref _projectWorkflowRevision);
            SetProjectActionBusy(true);
            try
            {
                ProjectWorkflowContext context = CreateWorkflowContext(workspace);
                ProjectWorkflowStatus = "Inspecting the selected project...";
                ProjectTargetInspection inspection = await _projectTargetResolver.InspectAsync(context, cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();
                ThrowIfProjectSelectionChanged(workspace, revision);
                _updatingProjectInspection = true;
                try
                {
                    WorkflowFrameworks.Clear();
                    foreach (string framework in inspection.TargetFrameworks)
                    {
                        WorkflowFrameworks.Add(framework);
                    }

                    SelectedWorkflowFramework = inspection.TargetFramework;
                }
                finally
                {
                    _updatingProjectInspection = false;
                }

                ProjectInspection = inspection;
                ProjectWorkflowStatus = inspection.TargetFramework is null
                    ? inspection.DebugUnavailableReason ?? "No target framework was resolved."
                    : $"{inspection.Context.Configuration} / {inspection.TargetFramework}: "
                        + $"{inspection.TargetPath ?? "no target path"}"
                        + (inspection.DebugUnavailableReason is null
                            ? string.Empty
                            : $"{Environment.NewLine}{inspection.DebugUnavailableReason}");
            }
            catch (OperationCanceledException)
            {
                ReportProjectCancellation(workspace, revision);
            }
            catch (Exception exception) when (IsProjectWorkflowException(exception))
            {
                ReportProjectFailure(workspace, revision, "Project inspection", exception);
            }
            finally
            {
                SetProjectActionBusy(false);
            }
        }

        [RelayCommand(CanExecute = nameof(CanBuildSelectedProject))]
        private async Task BuildSelectedProjectAsync(CancellationToken cancellationToken)
        {
            if (!CanBuildSelectedProject())
            {
                return;
            }

            ProjectTargetInspection approved = ProjectInspection!;
            CSharpWorkspace workspace = Workspace!;
            long revision = Interlocked.Read(ref _projectWorkflowRevision);
            SetProjectActionBusy(true);
            try
            {
                BuilderBuildResult result = await BuildProjectForWorkflowAsync(approved, workspace, revision, cancellationToken);
                ThrowIfProjectSelectionChanged(workspace, revision);
                ProjectWorkflowStatus = result.Succeeded
                    ? $"Build succeeded: {approved.Context.Project.Name}."
                    : $"Build failed: {approved.Context.Project.Name}. See diagnostics.";
                Editor.StatusText = ProjectWorkflowStatus;
            }
            catch (OperationCanceledException)
            {
                ReportProjectCancellation(workspace, revision);
            }
            catch (Exception exception) when (IsProjectWorkflowException(exception))
            {
                ReportProjectFailure(workspace, revision, "Project build", exception);
            }
            finally
            {
                SetProjectActionBusy(false);
            }
        }

        [RelayCommand(CanExecute = nameof(CanTestSelectedProject))]
        private async Task FindTestCandidatesAsync(CancellationToken cancellationToken)
        {
            if (!CanTestSelectedProject())
            {
                return;
            }

            ProjectTargetInspection approved = ProjectInspection!;
            CSharpWorkspace workspace = Workspace!;
            long revision = Interlocked.Read(ref _projectWorkflowRevision);
            SetProjectActionBusy(true);
            try
            {
                ProjectWorkflowContext context = CreateWorkflowContext(workspace);
                await VerifyProjectInspectionAsync(approved, context, workspace, revision, cancellationToken);
                ProjectWorkflowStatus = "Finding source test candidates...";
                var candidates = await _csharpSyntaxDiagnosticService.FindTestMethodCandidatesAsync(
                    context.Project.ProjectPath, context.Configuration, approved.TargetFramework!, cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();
                ThrowIfProjectSelectionChanged(workspace, revision);
                SelectedTestMethodCandidate = null;
                TestMethodCandidates.Clear();
                foreach (TestMethodCandidate candidate in candidates)
                {
                    TestMethodCandidates.Add(candidate);
                }
                ProjectWorkflowStatus = $"Found {candidates.Count} source test candidates; adapter discovery may differ.";
            }
            catch (OperationCanceledException)
            {
                ReportProjectCancellation(workspace, revision);
            }
            catch (Exception exception) when (IsProjectWorkflowException(exception))
            {
                ReportProjectFailure(workspace, revision, "Source test discovery", exception);
            }
            finally
            {
                SetProjectActionBusy(false);
            }
        }

        [RelayCommand(CanExecute = nameof(CanTestSelectedProject))]
        private async Task DiscoverAdapterTestsAsync(CancellationToken cancellationToken)
        {
            if (!CanTestSelectedProject())
            {
                return;
            }

            ProjectTargetInspection approved = ProjectInspection!;
            CSharpWorkspace workspace = Workspace!;
            long revision = Interlocked.Read(ref _projectWorkflowRevision);
            SetProjectActionBusy(true);
            try
            {
                ProjectWorkflowContext context = CreateWorkflowContext(workspace);
                SelectedDiscoveredTestCase = null;
                DiscoveredTestCases.Clear();
                string assemblyPath = await PrepareTestAssemblyAsync(approved, context, workspace, revision, cancellationToken);
                ProjectWorkflowStatus = "Discovering test cases with VSTest adapters...";
                TestDiscoveryResult discovery = await _testDiscoveryService.DiscoverAsync(
                    assemblyPath, Path.GetDirectoryName(context.Project.ProjectPath)!, cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();
                ThrowIfProjectSelectionChanged(workspace, revision);
                foreach (DiscoveredTestCase testCase in discovery.Cases)
                {
                    DiscoveredTestCases.Add(testCase);
                }
                ProjectWorkflowStatus = $"VSTest discovered {discovery.Cases.Count} cases."
                    + (discovery.Warnings.Count == 0 ? string.Empty : Environment.NewLine + string.Join(Environment.NewLine, discovery.Warnings));
            }
            catch (OperationCanceledException)
            {
                ReportProjectCancellation(workspace, revision);
            }
            catch (Exception exception) when (IsProjectWorkflowException(exception))
            {
                ReportProjectFailure(workspace, revision, "Adapter test discovery", exception);
            }
            finally
            {
                SetProjectActionBusy(false);
            }
        }

        [RelayCommand(CanExecute = nameof(CanDebugDiscoveredTestCase))]
        private async Task DebugDiscoveredTestCaseAsync(CancellationToken cancellationToken)
        {
            if (!CanDebugDiscoveredTestCase())
            {
                return;
            }

            DiscoveredTestCase selected = SelectedDiscoveredTestCase!;
            ProjectTargetInspection approved = ProjectInspection!;
            CSharpWorkspace workspace = Workspace!;
            long revision = Interlocked.Read(ref _projectWorkflowRevision);
            EditorViewModel editor = Editor;
            string sourcePath = Path.GetFullPath(editor.CurrentFilePath!);
            string sourceText = editor.Text ?? string.Empty;
            int line = GetLineNumber(sourceText, _observedEditorTextArea!.CaretOffset);
            SetProjectActionBusy(true);
            try
            {
                if (editor.IsDirty || !string.Equals(File.ReadAllText(sourcePath), sourceText, StringComparison.Ordinal))
                {
                    throw new InvalidOperationException("Save the active source file before debugging at its caret.");
                }
                ProjectWorkflowContext context = CreateWorkflowContext(workspace);
                ProjectTestResults.Clear();
                string assemblyPath = await PrepareTestAssemblyAsync(approved, context, workspace, revision, cancellationToken);
                ProjectWorkflowStatus = $"Debugging adapter case: {selected.DisplayName}...";
                var debugService = (ITestCaseDebugExecutionService)_testCaseExecutionService;
                TestRunSummary summary = await debugService.DebugCaseAsync(
                    assemblyPath, Path.GetDirectoryName(context.Project.ProjectPath)!, selected,
                    async (pid, token) =>
                    {
                        if (!string.Equals(File.ReadAllText(sourcePath), sourceText, StringComparison.Ordinal))
                        {
                            throw new InvalidOperationException("The breakpoint source changed after the test build.");
                        }
                        DebugSessionInfo session = await _debugSessionService.StartSessionAsync(new DebugLaunchRequest
                        {
                            AttachToExistingProcess = true,
                            ProcessId = pid,
                            WorkingDirectory = Path.GetDirectoryName(context.Project.ProjectPath),
                        }, token);
                        try
                        {
                            await _debugSessionService.SetBreakpointsAsync(session.SessionId, sourcePath, [line], token);
                            await Dispatcher.UIThread.InvokeAsync(() =>
                            {
                                token.ThrowIfCancellationRequested();
                                ThrowIfProjectSelectionChanged(workspace, revision);
                                if (editor.IsDirty || !string.Equals(editor.Text, sourceText, StringComparison.Ordinal))
                                {
                                    throw new InvalidOperationException("The breakpoint source changed after the test build.");
                                }
                                AdoptDebugSession(session, sourcePath);
                                _debuggedTestSessionId = session.SessionId;
                                _debugBreakpoints[sourcePath] = [line];
                            });
                            return new TestDebugSessionLease(this, session.SessionId);
                        }
                        catch
                        {
                            await _debugSessionService.StopSessionAsync(session.SessionId, CancellationToken.None);
                            throw;
                        }
                    }, cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();
                ThrowIfProjectSelectionChanged(workspace, revision);
                await ApplyTestDiagnosticsAsync(summary, workspace, cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();
                ThrowIfProjectSelectionChanged(workspace, revision);
                UpdateProjectTestResults(summary);
                ProjectWorkflowStatus = $"Debugged adapter case ({selected.DisplayName}): {summary.Outcome}."
                    + (summary.Messages.Count == 0 ? string.Empty : Environment.NewLine + string.Join(Environment.NewLine, summary.Messages));
                Editor.StatusText = ProjectWorkflowStatus;
            }
            catch (OperationCanceledException)
            {
                ReportProjectCancellation(workspace, revision);
            }
            catch (Exception exception) when (IsProjectWorkflowException(exception))
            {
                ReportProjectFailure(workspace, revision, "Adapter case debugging", exception);
            }
            finally
            {
                SetProjectActionBusy(false);
            }
        }

        private sealed class TestDebugSessionLease(MainWindowViewModel owner, string sessionId) : IAsyncDisposable
        {
            public async ValueTask DisposeAsync()
            {
                try
                {
                    await owner._debugSessionService.StopSessionAsync(sessionId, CancellationToken.None);
                }
                finally
                {
                    await Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        if (string.Equals(owner._debuggedTestSessionId, sessionId, StringComparison.Ordinal))
                        {
                            owner._debuggedTestSessionId = null;
                            owner.ClearActiveDebugSession(sessionId);
                        }
                    });
                }
            }
        }

        [RelayCommand(CanExecute = nameof(CanRunDiscoveredTestCase))]
        private async Task RunDiscoveredTestCaseAsync(CancellationToken cancellationToken)
        {
            if (!CanRunDiscoveredTestCase())
            {
                return;
            }

            DiscoveredTestCase selected = SelectedDiscoveredTestCase!;
            ProjectTargetInspection approved = ProjectInspection!;
            CSharpWorkspace workspace = Workspace!;
            long revision = Interlocked.Read(ref _projectWorkflowRevision);
            SetProjectActionBusy(true);
            try
            {
                ProjectWorkflowContext context = CreateWorkflowContext(workspace);
                ProjectTestResults.Clear();
                string assemblyPath = await PrepareTestAssemblyAsync(approved, context, workspace, revision, cancellationToken);
                ProjectWorkflowStatus = $"Running adapter case: {selected.DisplayName}...";
                TestRunSummary summary = await _testCaseExecutionService.RunCaseAsync(
                    assemblyPath, Path.GetDirectoryName(context.Project.ProjectPath)!, selected, cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();
                ThrowIfProjectSelectionChanged(workspace, revision);
                await ApplyTestDiagnosticsAsync(summary, workspace, cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();
                ThrowIfProjectSelectionChanged(workspace, revision);
                UpdateProjectTestResults(summary);
                ProjectWorkflowStatus = $"Adapter case ({selected.DisplayName}): {summary.Outcome}."
                    + (summary.Messages.Count == 0 ? string.Empty : Environment.NewLine + string.Join(Environment.NewLine, summary.Messages));
                Editor.StatusText = ProjectWorkflowStatus;
            }
            catch (OperationCanceledException)
            {
                ReportProjectCancellation(workspace, revision);
            }
            catch (Exception exception) when (IsProjectWorkflowException(exception))
            {
                ReportProjectFailure(workspace, revision, "Adapter case execution", exception);
            }
            finally
            {
                SetProjectActionBusy(false);
            }
        }

        [RelayCommand(CanExecute = nameof(CanTestSelectedProject))]
        private async Task TestSelectedProjectAsync(CancellationToken cancellationToken)
        {
            await RunSelectedProjectTestsAsync(null, cancellationToken);
        }

        [RelayCommand(CanExecute = nameof(CanTestSelectedMethod))]
        private async Task TestSelectedMethodAsync(CancellationToken cancellationToken)
        {
            await RunSelectedProjectTestsAsync(WorkflowTestMethod.Trim(), cancellationToken);
        }

        private async Task RunSelectedProjectTestsAsync(string? testMethod, CancellationToken cancellationToken)
        {
            if (!CanTestSelectedProject() || testMethod is not null && string.IsNullOrWhiteSpace(testMethod))
            {
                return;
            }

            ProjectTargetInspection approved = ProjectInspection!;
            CSharpWorkspace workspace = Workspace!;
            long revision = Interlocked.Read(ref _projectWorkflowRevision);
            SetProjectActionBusy(true);
            try
            {
                ProjectWorkflowContext context = CreateWorkflowContext(workspace);
                await VerifyProjectInspectionAsync(approved, context, workspace, revision, cancellationToken);
                ProjectWorkflowStatus = testMethod is null
                    ? $"Testing project: {context.Project.Name}..."
                    : $"Testing method: {testMethod}...";
                ProjectTestResults.Clear();
                TestRunRequest request = new()
                {
                    WorkspaceRootPath = workspace.RootPath,
                    ProjectPath = context.Project.ProjectPath,
                    Configuration = context.Configuration,
                    TargetFramework = approved.TargetFramework,
                    MatchTargetsExactly = testMethod is not null,
                };
                if (testMethod is not null)
                {
                    request.Targets.Add(testMethod);
                }
                TestRunSummary summary = await _testExecutionService.RunTestsAsync(request, cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();
                ThrowIfProjectSelectionChanged(workspace, revision);
                await ApplyTestDiagnosticsAsync(summary, workspace, cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();
                ThrowIfProjectSelectionChanged(workspace, revision);
                UpdateProjectTestResults(summary);
                ProjectWorkflowStatus = $"Project tests ({context.Project.Name}): {summary.Outcome} ({summary.TotalCount} total).";
                Editor.StatusText = ProjectWorkflowStatus;
                Editor.NotificationText =
                    $"Project tests: {summary.PassedCount} passed, {summary.FailedCount} failed, {summary.SkippedCount} skipped.";
            }
            catch (OperationCanceledException)
            {
                ReportProjectCancellation(workspace, revision);
            }
            catch (Exception exception) when (IsProjectWorkflowException(exception))
            {
                ReportProjectFailure(workspace, revision, "Project tests", exception);
            }
            finally
            {
                SetProjectActionBusy(false);
            }
        }

        [RelayCommand(CanExecute = nameof(CanDebugSelectedProject))]
        private async Task DebugSelectedProjectAsync(CancellationToken cancellationToken)
        {
            if (!CanDebugSelectedProject())
            {
                return;
            }

            ProjectTargetInspection approved = ProjectInspection!;
            CSharpWorkspace workspace = Workspace!;
            long revision = Interlocked.Read(ref _projectWorkflowRevision);
            SetProjectActionBusy(true);
            try
            {
                BuilderBuildResult result = await BuildProjectForWorkflowAsync(approved, workspace, revision, cancellationToken);
                if (!result.Succeeded)
                {
                    ProjectWorkflowStatus = "Project build failed; debugging was not started. See diagnostics.";
                    Editor.StatusText = ProjectWorkflowStatus;
                    return;
                }

                ProjectWorkflowContext context = CreateWorkflowContext(workspace);
                string targetPath = GetVerifiedPrimaryArtifact(result, approved);
                ProjectWorkflowStatus = $"Preparing runtime dependencies: {context.Project.Name}...";
                await _projectRuntimeBuilder.BuildAsync(new BuilderBuildRequest
                {
                    WorkspaceRootPath = workspace.RootPath,
                    ProjectPath = context.Project.ProjectPath,
                    Configuration = context.Configuration,
                    TargetFramework = approved.TargetFramework,
                }, cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();
                ThrowIfProjectSelectionChanged(workspace, revision);
                await VerifyProjectInspectionAsync(approved, context, workspace, revision, cancellationToken);
                VerifyDebugRuntimeFiles(targetPath);
                cancellationToken.ThrowIfCancellationRequested();
                ThrowIfProjectSelectionChanged(workspace, revision);
                ProjectWorkflowStatus = $"Starting debugger: {targetPath}";
                await StartAndAdoptDebugSessionAsync(
                    new DebugLaunchRequest
                    {
                        Target = targetPath,
                        WorkingDirectory = Path.GetDirectoryName(context.Project.ProjectPath),
                        StopAtEntry = true,
                    },
                    () => IsProjectSelectionCurrent(workspace, revision),
                    cancellationToken);
                if (IsProjectSelectionCurrent(workspace, revision) && !cancellationToken.IsCancellationRequested)
                {
                    ProjectWorkflowStatus = Editor.StatusText;
                }
            }
            catch (OperationCanceledException)
            {
                ReportProjectCancellation(workspace, revision);
            }
            catch (Exception exception) when (IsProjectWorkflowException(exception))
            {
                ReportProjectFailure(workspace, revision, "Project debugging", exception);
            }
            finally
            {
                SetProjectActionBusy(false);
            }
        }

        [RelayCommand(CanExecute = nameof(CanCancelProjectWorkflow))]
        private void CancelProjectWorkflow()
        {
            InspectSelectedProjectCommand.Cancel();
            BuildSelectedProjectCommand.Cancel();
            TestSelectedProjectCommand.Cancel();
            TestSelectedMethodCommand.Cancel();
            FindTestCandidatesCommand.Cancel();
            DiscoverAdapterTestsCommand.Cancel();
            RunDiscoveredTestCaseCommand.Cancel();
            DebugDiscoveredTestCaseCommand.Cancel();
            DebugSelectedProjectCommand.Cancel();
            ProjectWorkflowStatus = "Cancellation requested. An in-progress builder may need to finish first.";
        }

        private bool CanInspectSelectedProject()
        {
            return !ProjectWorkflowBusy
                && Workspace is not null
                && SelectedWorkflowProject is not null
                && Workspace.Projects.Contains(SelectedWorkflowProject);
        }

        private bool CanBuildSelectedProject()
        {
            return !ProjectWorkflowBusy
                && _builderService is not NullCodeStudioBuilderService
                && ProjectInspection?.TargetFramework is not null;
        }

        private bool CanTestSelectedProject()
        {
            return !ProjectWorkflowBusy
                && _testExecutionService is not NullTestExecutionService
                && ProjectInspection?.TargetFramework is not null;
        }

        private bool CanTestSelectedMethod()
        {
            return CanTestSelectedProject() && !string.IsNullOrWhiteSpace(WorkflowTestMethod);
        }

        private bool CanRunDiscoveredTestCase()
        {
            return CanTestSelectedProject()
                && SelectedDiscoveredTestCase is not null
                && DiscoveredTestCases.Contains(SelectedDiscoveredTestCase);
        }

        private bool CanDebugDiscoveredTestCase()
        {
            return CanRunDiscoveredTestCase()
                && _testCaseExecutionService is ITestCaseDebugExecutionService
                && _debugSessionService is not NullDebugSessionService
                && _activeDebugSessionId is null
                && _observedEditorTextArea is not null
                && !string.IsNullOrWhiteSpace(Editor.CurrentFilePath)
                && string.Equals(Path.GetExtension(Editor.CurrentFilePath), ".cs", StringComparison.OrdinalIgnoreCase);
        }

        private void UpdateProjectTestResults(TestRunSummary summary)
        {
            foreach (var group in summary.Cases.GroupBy(
                static result => result.TestClassName ?? result.QualifiedTestName ?? result.TestName, StringComparer.Ordinal))
            {
                ProjectTestResults.Add(new TestResultNode(group.Key, group
                    .Select(static result => new TestResultNode($"{result.Outcome}: {result.TestName}", []))
                    .ToArray()));
            }
        }

        private async Task<string> PrepareTestAssemblyAsync(
            ProjectTargetInspection approved,
            ProjectWorkflowContext context,
            CSharpWorkspace workspace,
            long revision,
            CancellationToken cancellationToken)
        {
            await VerifyProjectInspectionAsync(approved, context, workspace, revision, cancellationToken);
            if (string.IsNullOrWhiteSpace(approved.TargetPath))
            {
                throw new InvalidOperationException("The selected project has no target assembly path.");
            }

            ProjectWorkflowStatus = $"Building test project: {context.Project.Name}...";
            await _projectRuntimeBuilder.BuildAsync(new BuilderBuildRequest
            {
                WorkspaceRootPath = workspace.RootPath,
                ProjectPath = context.Project.ProjectPath,
                Configuration = context.Configuration,
                TargetFramework = approved.TargetFramework,
            }, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfProjectSelectionChanged(workspace, revision);
            await VerifyProjectInspectionAsync(approved, context, workspace, revision, cancellationToken);
            return approved.TargetPath;
        }

        private bool CanDebugSelectedProject()
        {
            return CanBuildSelectedProject()
                && _debugSessionService is not NullDebugSessionService
                && _activeDebugSessionId is null
                && ProjectInspection?.CanDebug == true;
        }

        private bool CanCancelProjectWorkflow() => _projectActionInProgress;

        private void SetProjectActionBusy(bool busy)
        {
            _projectActionInProgress = busy;
            ProjectWorkflowBusy = busy;
            CancelProjectWorkflowCommand.NotifyCanExecuteChanged();
        }

        private ProjectWorkflowContext CreateWorkflowContext(CSharpWorkspace workspace)
        {
            return ProjectWorkflowContext.Create(
                workspace,
                SelectedWorkflowProject?.ProjectPath ?? throw new InvalidOperationException("Select a project first."),
                WorkflowConfiguration,
                SelectedWorkflowFramework);
        }

        private async Task<BuilderBuildResult> BuildProjectForWorkflowAsync(
            ProjectTargetInspection approved,
            CSharpWorkspace workspace,
            long revision,
            CancellationToken cancellationToken)
        {
            ProjectWorkflowContext context = CreateWorkflowContext(workspace);
            await VerifyProjectInspectionAsync(approved, context, workspace, revision, cancellationToken);
            ProjectWorkflowStatus = $"Building project: {context.Project.Name}...";
            BuilderBuildRequest request = new()
            {
                WorkspaceRootPath = workspace.RootPath,
                ProjectPath = context.Project.ProjectPath,
                Configuration = context.Configuration,
                TargetFramework = approved.TargetFramework,
            };
            BuilderBuildResult result = await Task.Run(
                () => _builderService.BuildProjectAsync(request, cancellationToken),
                CancellationToken.None);
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfProjectSelectionChanged(workspace, revision);
            if (result.Succeeded
                && (string.IsNullOrWhiteSpace(result.ProjectPath)
                    || !WorkspacePathIdentity.AreEqual(result.ProjectPath, context.Project.ProjectPath)))
            {
                throw new InvalidOperationException("The builder returned a result for a different project.");
            }

            _buildDiagnostics.Clear();
            _buildDiagnostics.AddRange(result.Diagnostics.Select(MapDiagnostic));
            await RefreshDiagnosticsAsync(cancellationToken);
            ThrowIfProjectSelectionChanged(workspace, revision);
            return result;
        }

        private async Task VerifyProjectInspectionAsync(
            ProjectTargetInspection approved,
            ProjectWorkflowContext context,
            CSharpWorkspace workspace,
            long revision,
            CancellationToken cancellationToken)
        {
            ProjectTargetInspection current = await _projectTargetResolver.InspectAsync(context, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfProjectSelectionChanged(workspace, revision);
            if (!string.Equals(approved.TargetFramework, current.TargetFramework, StringComparison.OrdinalIgnoreCase)
                || !string.Equals(approved.OutputType, current.OutputType, StringComparison.OrdinalIgnoreCase)
                || !string.Equals(approved.FrameworkIdentifier, current.FrameworkIdentifier, StringComparison.OrdinalIgnoreCase)
                || !string.Equals(approved.IsTestProject, current.IsTestProject, StringComparison.OrdinalIgnoreCase)
                || !SameOptionalPath(approved.TargetPath, current.TargetPath)
                || !approved.TargetFrameworks.SequenceEqual(current.TargetFrameworks, StringComparer.OrdinalIgnoreCase)
                || !approved.Configurations.SequenceEqual(current.Configurations, StringComparer.OrdinalIgnoreCase))
            {
                ProjectInspection = null;
                throw new InvalidOperationException("Project metadata changed. Inspect the project again before continuing.");
            }
        }

        private static bool SameOptionalPath(string? first, string? second)
        {
            if (string.IsNullOrWhiteSpace(first) || string.IsNullOrWhiteSpace(second))
            {
                return string.IsNullOrWhiteSpace(first) && string.IsNullOrWhiteSpace(second);
            }

            return WorkspacePathIdentity.AreEqual(first, second);
        }

        private static string GetVerifiedPrimaryArtifact(BuilderBuildResult result, ProjectTargetInspection inspection)
        {
            BuilderCompilationArtifact[] artifacts = result.Artifacts
                .Where(static artifact => string.Equals(artifact.Kind, "PrimaryOutput", StringComparison.OrdinalIgnoreCase))
                .ToArray();
            if (artifacts.Length != 1)
            {
                throw new InvalidOperationException("The builder did not report exactly one primary assembly.");
            }

            BuilderCompilationArtifact artifact = artifacts[0];
            if (string.IsNullOrWhiteSpace(artifact.Path)
                || !Path.IsPathFullyQualified(artifact.Path)
                || inspection.TargetPath is null
                || !WorkspacePathIdentity.AreEqual(artifact.Path, inspection.TargetPath))
            {
                throw new InvalidOperationException(
                    $"The builder artifact '{artifact.Path}' does not match the inspected target '{inspection.TargetPath}'.");
            }

            if (!string.IsNullOrWhiteSpace(artifact.TargetFramework)
                && !string.Equals(artifact.TargetFramework, inspection.TargetFramework, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("The builder artifact uses a different target framework.");
            }

            if (!File.Exists(artifact.Path))
            {
                throw new InvalidOperationException($"The builder artifact '{artifact.Path}' does not exist.");
            }

            return artifact.Path;
        }

        private static void VerifyDebugRuntimeFiles(string path)
        {
            if (!File.Exists(path))
            {
                throw new InvalidOperationException($"The built assembly '{path}' does not exist.");
            }

            if (!File.Exists(Path.ChangeExtension(path, ".runtimeconfig.json"))
                || !File.Exists(Path.ChangeExtension(path, ".deps.json")))
            {
                throw new InvalidOperationException("The built assembly is missing its runtime configuration or dependency metadata.");
            }
        }

        private bool IsProjectSelectionCurrent(CSharpWorkspace workspace, long revision)
        {
            return ReferenceEquals(workspace, Workspace)
                && revision == Interlocked.Read(ref _projectWorkflowRevision);
        }

        private void ThrowIfProjectSelectionChanged(CSharpWorkspace workspace, long revision)
        {
            if (!IsProjectSelectionCurrent(workspace, revision))
            {
                throw new OperationCanceledException("The selected project or workspace changed.");
            }
        }

        private void ReportProjectCancellation(CSharpWorkspace workspace, long revision)
        {
            if (IsProjectSelectionCurrent(workspace, revision))
            {
                ProjectWorkflowStatus = "Project action canceled.";
                Editor.StatusText = ProjectWorkflowStatus;
            }
        }

        private void ReportProjectFailure(CSharpWorkspace workspace, long revision, string action, Exception exception)
        {
            if (IsProjectSelectionCurrent(workspace, revision))
            {
                ProjectWorkflowStatus = $"{action} failed: {exception.Message}";
                Editor.StatusText = $"{action} failed.";
                Editor.NotificationText = exception.Message;
            }
        }

        private static bool IsProjectWorkflowException(Exception exception)
        {
            return exception is IOException or UnauthorizedAccessException or InvalidOperationException
                or ArgumentException or Win32Exception or JsonException or NotSupportedException;
        }
    }
}
