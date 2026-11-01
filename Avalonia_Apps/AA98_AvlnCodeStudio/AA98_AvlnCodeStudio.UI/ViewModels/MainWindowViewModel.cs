using AA98_AvlnCodeStudio.UI.Resources;
using AA98_AvlnCodeStudio.Editor.Services;
using AA98_AvlnCodeStudio.Editor.Navigation;
using AA98_AvlnCodeStudio.Base.Building.Models;
using AA98_AvlnCodeStudio.Base.Building.Services;
using AA98_AvlnCodeStudio.Base.Versioning.Models;
using AA98_AvlnCodeStudio.Base.Versioning.Services;
using AA98_AvlnCodeStudio.Base.Testing.Models;
using AA98_AvlnCodeStudio.Base.Testing.Services;
using AA98_AvlnCodeStudio.Base.Debugging.Models;
using AA98_AvlnCodeStudio.Base.Debugging.Services;
using AA98_AvlnCodeStudio.Diagnostics.UI.ViewModels;
using AppKomponentBaseLib.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using AA98_AvlnCodeStudio.Planning.Core.Services;
using Config.UI.Avalonia.ViewModels;
using Project.Explorer.Avalonia.ViewModels;
using AA98_AvlnCodeStudio.Model.Workspaces;
using AA98_AvlnCodeStudio.AxamlDesigner.UI.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Threading;
using Code.Navigation;

namespace AA98_AvlnCodeStudio.UI.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        private readonly EditorDocumentWorkspace _editorWorkspace;
        private readonly AA98_AvlnCodeStudio.Base.UI.Services.IEditorFileDialogService _editorFileDialogService;
        private readonly CSharpWorkspaceLoader _workspaceLoader;
        private readonly CSharpProjectTemplateService _projectTemplateService;
        private readonly ICSharpProjectPackageService _projectPackageService;
        private readonly AxamlDesignerViewModel _axamlDesigner;
        private readonly Services.IWorkspaceFileDialogService _workspaceFileDialogService;
        private readonly ICodeStudioBuilderService _builderService;
        private readonly IVersionControlService _versionControlService;
        private readonly Services.CSharpSyntaxDiagnosticService _csharpSyntaxDiagnosticService;
        private readonly ITestExecutionService _testExecutionService;
        private readonly IDebugSessionService _debugSessionService;
        private readonly List<Diagnostic> _buildDiagnostics = [];
        private readonly Dictionary<string, HashSet<int>> _debugBreakpoints = new(
            OperatingSystem.IsWindows() ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal);
        private List<Diagnostic> _languageDiagnostics = [];
        private CancellationTokenSource? _languageAnalysisCancellation;
        private EditorViewModel? _observedEditor;
        private Controls.EditorTextArea? _observedEditorTextArea;
        public MainWindowViewModel()
            : this(
                new Components.AvaloniaEditorComponent(
                    new EditorWorkflow(
                        new Model.Documents.FileEditorDocument(),
                        new Services.DesignEditorFileDialogService(),
                        new Services.DesignTextDocumentStorageService()),
                    new EditorViewModel(),
                    new Controls.EditorTextArea()),
                new PlanningExplorerViewModel(new MarkdownPlanningReader()),
                new DiagnosticCollectionViewModel(),
                null,
                null,
                new CSharpWorkspaceLoader(),
                new Services.DesignWorkspaceFileDialogService(),
                new NullCodeStudioBuilderService(),
                new NullVersionControlService(),
                null,
                new Services.DesignEditorFileDialogService(),
                new Services.CSharpSyntaxDiagnosticService())
        {
        }

        public MainWindowViewModel(
            Components.IAvaloniaEditorComponent editorComponent,
            PlanningExplorerViewModel planningExplorer,
            DiagnosticCollectionViewModel diagnosticCollection,
            ConfigUiViewModel? configUi,
            ProjectExplorerViewModel? projectExplorer,
            CSharpWorkspaceLoader? workspaceLoader = null,
            Services.IWorkspaceFileDialogService? workspaceFileDialogService = null,
            ICodeStudioBuilderService? builderService = null,
            IVersionControlService? versionControlService = null,
            EditorDocumentWorkspace? editorWorkspace = null,
            AA98_AvlnCodeStudio.Base.UI.Services.IEditorFileDialogService? editorFileDialogService = null,
            Services.CSharpSyntaxDiagnosticService? csharpSyntaxDiagnosticService = null,
            ITestExecutionService? testExecutionService = null,
            IDebugSessionService? debugSessionService = null,
            CSharpProjectTemplateService? projectTemplateService = null,
            ICSharpProjectPackageService? projectPackageService = null,
            AxamlDesignerViewModel? axamlDesignerViewModel = null)
        {
            _editorWorkspace = editorWorkspace ?? new EditorDocumentWorkspace(editorComponent);
            _workspaceLoader = workspaceLoader ?? new CSharpWorkspaceLoader();
            _projectTemplateService = projectTemplateService ?? new CSharpProjectTemplateService();
            _projectPackageService = projectPackageService ?? new DotNetCSharpProjectPackageService();
            _axamlDesigner = axamlDesignerViewModel ?? new AxamlDesignerViewModel();
            _axamlDesigner.MarkupChanged += OnAxamlDesignerMarkupChanged;
            _axamlDesigner.PropertyChanged += OnAxamlDesignerPropertyChanged;
            _workspaceFileDialogService = workspaceFileDialogService ?? new Services.DesignWorkspaceFileDialogService();
            _builderService = builderService ?? new NullCodeStudioBuilderService();
            _versionControlService = versionControlService ?? new NullVersionControlService();
            _editorFileDialogService = editorFileDialogService ?? new Services.DesignEditorFileDialogService();
            _csharpSyntaxDiagnosticService = csharpSyntaxDiagnosticService ?? new Services.CSharpSyntaxDiagnosticService();
            _testExecutionService = testExecutionService ?? new NullTestExecutionService();
            _debugSessionService = debugSessionService ?? new NullDebugSessionService();
            _debugSessionService.SessionEventReceived += OnDebugSessionEventReceived;
            _editorWorkspace.PropertyChanged += OnEditorWorkspacePropertyChanged;
            _observedEditor = _editorWorkspace.ActiveTab.Editor;
            _observedEditor.PropertyChanged += OnActiveEditorPropertyChanged;
            ObserveActiveEditorControl();
            CompletionSuggestions.CollectionChanged += (_, _) => OnPropertyChanged(nameof(HasCompletionSuggestions));
            PlanningExplorer = planningExplorer;
            DiagnosticCollection = diagnosticCollection ?? throw new ArgumentNullException(nameof(diagnosticCollection));
            ConfigUi = configUi;
            ProjectExplorer = projectExplorer;
            PlanningExplorer.LoadAsync(Directory.GetCurrentDirectory()).GetAwaiter().GetResult();
            ProjectExplorer?.LoadAsync(Directory.GetCurrentDirectory()).GetAwaiter().GetResult();
            LoadProjectTemplates(showStatus: false);
            RefreshAxamlPreview();
        }

        public EditorDocumentWorkspace EditorWorkspace => _editorWorkspace;

        public EditorTabViewModel ActiveEditorTab
        {
            get => _editorWorkspace.ActiveTab;
            set => _editorWorkspace.Activate(value);
        }

        public EditorViewModel Editor => _editorWorkspace.ActiveTab.Editor;

        public PlanningExplorerViewModel PlanningExplorer { get; }

        public DiagnosticCollectionViewModel DiagnosticCollection { get; }

        /// <summary>
        /// Gets the host-composed reusable configuration UI state.
        /// </summary>
        public ConfigUiViewModel? ConfigUi { get; }

        /// <summary>Gets the host-composed reusable project explorer state.</summary>
        public ProjectExplorerViewModel? ProjectExplorer { get; }

        public Components.IAvaloniaEditorComponent EditorComponent => _editorWorkspace.ActiveTab.Component;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(WorkspaceTitle))]
        [NotifyCanExecuteChangedFor(nameof(BuildWorkspaceCommand))]
        [NotifyCanExecuteChangedFor(nameof(RunWorkspaceTestsCommand))]
        [NotifyCanExecuteChangedFor(nameof(StartDebugSessionCommand))]
        [NotifyCanExecuteChangedFor(nameof(AddPackageCommand))]
        [NotifyCanExecuteChangedFor(nameof(RemovePackageCommand))]
        private CSharpWorkspace? _workspace;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(StartDebugSessionCommand))]
        private string _debugTargetPath = string.Empty;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(LoadDebugVariablesCommand))]
        private DebugStackFrame? _selectedDebugStackFrame;

        [ObservableProperty]
        private DebugSessionState? _activeDebugSessionState;

        private string? _activeDebugSessionId;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(VersionControlSummary))]
        private VersionControlStatus? _versionControlStatus;

        public string WorkspaceTitle => Workspace is null
            ? UiStrings.NoWorkspaceOpenText
            : string.Format(UiStrings.WorkspaceOpenStatusFormat, Workspace.EntryPath);

        public string VersionControlSummary => VersionControlStatus is null || !VersionControlStatus.IsRepositoryRootDiscovered
            ? UiStrings.NoRepositoryStatusText
            : string.Format(
                UiStrings.RepositoryStatusFormat,
                VersionControlStatus.RepositoryName ?? VersionControlStatus.RepositoryRootPath,
                VersionControlStatus.ActiveReferenceName ?? UiStrings.UnknownReferenceText,
                VersionControlStatus.Changes.Count);

        public string WindowTitle => Editor.WindowTitle;

        public string NavigationTitle => UiStrings.NavigationTitle;

        public string NavigationPlaceholder => UiStrings.NavigationPlaceholder;

        public string PlanningExplorerTitle => UiStrings.PlanningExplorerTitle;

        public string EditorRegionTitle => UiStrings.EditorRegionTitle;

        public string StatusRegionTitle => UiStrings.StatusRegionTitle;

        public string NotificationRegionTitle => UiStrings.NotificationRegionTitle;

        public string FileMenuHeader => UiStrings.FileMenuHeader;

        public string ViewMenuHeader => UiStrings.ViewMenuHeader;

        public string NewCommandText => UiStrings.NewCommandText;

        public string OpenCommandText => UiStrings.OpenCommandText;

        public string SaveCommandText => UiStrings.SaveCommandText;

        public string SaveAsCommandText => UiStrings.SaveAsCommandText;

        public string CompleteCommandText => UiStrings.CompleteCommandText;

        public string ApplyCompletionCommandText => UiStrings.ApplyCompletionCommandText;

        public string GoToDefinitionCommandText => UiStrings.GoToDefinitionCommandText;

        public string RunTestsCommandText => _testExecutionService is NullTestExecutionService
            ? UiStrings.RunTestsUnavailableText
            : UiStrings.RunTestsCommandText;

        public ObservableCollection<string> CompletionSuggestions { get; } = [];

        public ObservableCollection<CSharpProjectTemplateDefinition> ProjectTemplates { get; } = [];

        public AxamlDesignerViewModel AxamlDesigner => _axamlDesigner;

        public bool HasAxamlPreview => _axamlDesigner.HasPreview;

        public ObservableCollection<CSharpWorkspaceProject> PackageProjects { get; } = [];

        public ObservableCollection<CSharpProjectPackageReference> InstalledPackages { get; } = [];

        public ObservableCollection<NuGetPackageSearchResult> PackageSearchResults { get; } = [];

        public ObservableCollection<DebugStackFrame> DebugStackFrames { get; } = [];

        public ObservableCollection<DebugVariable> DebugVariables { get; } = [];

        public string? SelectedCompletion { get; set; }

        public bool HasCompletionSuggestions => CompletionSuggestions.Count > 0;

        public bool HasDebugSession => _activeDebugSessionId is not null;

        [ObservableProperty]
        private CSharpProjectTemplateDefinition? _selectedProjectTemplate;

        [ObservableProperty]
        private CSharpWorkspaceProject? _selectedPackageProject;

        [ObservableProperty]
        private CSharpProjectPackageReference? _selectedInstalledPackage;

        [ObservableProperty]
        private NuGetPackageSearchResult? _selectedPackageSearchResult;

        [ObservableProperty]
        private string _packageId = string.Empty;

        [ObservableProperty]
        private string _packageVersion = string.Empty;

        [ObservableProperty]
        private string _packageSearchText = string.Empty;

        [RelayCommand]
        private void NewDocument()
        {
            _editorWorkspace.CreateNewDocument();
        }

        [RelayCommand]
        private async Task OpenDocumentAsync(CancellationToken cancellationToken)
        {
            string? path = await _editorFileDialogService.ShowOpenFileDialogAsync(
                Workspace?.RootPath ?? Directory.GetCurrentDirectory(),
                cancellationToken);
            if (string.IsNullOrWhiteSpace(path))
            {
                return;
            }

            await _editorWorkspace.OpenOrActivateAsync(path, cancellationToken);
        }

        [RelayCommand]
        private async Task RequestCompletionsAsync(CancellationToken cancellationToken)
        {
            string? filePath = Editor.CurrentFilePath;
            if (Workspace is null
                || string.IsNullOrWhiteSpace(filePath)
                || _observedEditorTextArea is null)
            {
                CompletionSuggestions.Clear();
                return;
            }

            IReadOnlyList<string> suggestions = await _csharpSyntaxDiagnosticService.GetCompletionsAsync(
                Editor.Text,
                filePath,
                _observedEditorTextArea.CaretOffset,
                Workspace,
                cancellationToken);
            CompletionSuggestions.Clear();
            foreach (string suggestion in suggestions.Take(50))
            {
                CompletionSuggestions.Add(suggestion);
            }

            SelectedCompletion = CompletionSuggestions.FirstOrDefault();
            OnPropertyChanged(nameof(SelectedCompletion));
            if (CompletionSuggestions.Count == 0)
            {
                Editor.NotificationText = "No C# completions are available at the current position.";
            }
        }

        [RelayCommand]
        private void ApplySelectedCompletion()
        {
            if (string.IsNullOrWhiteSpace(SelectedCompletion) || _observedEditorTextArea is null)
            {
                return;
            }

            string source = Editor.Text;
            int caretOffset = Math.Clamp(_observedEditorTextArea.CaretOffset, 0, source.Length);
            int replacementStart = caretOffset;
            while (replacementStart > 0 && IsIdentifierCharacter(source[replacementStart - 1]))
            {
                replacementStart--;
            }

            string updatedSource = string.Concat(
                source[..replacementStart],
                SelectedCompletion,
                source[caretOffset..]);
            Editor.Text = updatedSource;
            _observedEditorTextArea.SetCaretOffset(replacementStart + SelectedCompletion.Length);
            CompletionSuggestions.Clear();
            SelectedCompletion = null;
            OnPropertyChanged(nameof(SelectedCompletion));
        }

        [RelayCommand]
        private async Task GoToDefinitionAsync(CancellationToken cancellationToken)
        {
            string? filePath = Editor.CurrentFilePath;
            if (Workspace is null
                || string.IsNullOrWhiteSpace(filePath)
                || _observedEditorTextArea is null)
            {
                return;
            }

            Code.Navigation.CodeLocation? location = await _csharpSyntaxDiagnosticService.FindDefinitionAsync(
                Editor.Text,
                filePath,
                _observedEditorTextArea.CaretOffset,
                Workspace,
                cancellationToken);
            if (location is null)
            {
                Editor.NotificationText = "No source definition was found at the current position.";
                return;
            }

            IEditorDocument? document = await _editorWorkspace.OpenOrActivateAsync(location.Path, cancellationToken);
            if (document is null)
            {
                Editor.NotificationText = $"The definition file '{location.Path}' is no longer available.";
                return;
            }

            EditorTabViewModel? targetTab = _editorWorkspace.FindByPath(location.Path);
            if (targetTab?.View is not Controls.EditorTextArea targetEditor)
            {
                throw new InvalidOperationException($"The definition file '{location.Path}' has no caret-capable editor.");
            }

            targetEditor.FocusAndMoveCaret(location);
            targetTab.Editor.StatusText = $"Navigated to definition in {Path.GetFileName(location.Path)}.";
        }

        public string OpenWorkspaceCommandText => UiStrings.OpenWorkspaceCommandText;

        public string CreateConsoleProjectCommandText => UiStrings.CreateConsoleProjectCommandText;

        public string CreateClassLibraryProjectCommandText => UiStrings.CreateClassLibraryProjectCommandText;

        public string CreateTestProjectCommandText => UiStrings.CreateTestProjectCommandText;

        public string ProjectTemplatesMenuText => "Project Templates";

        public string CreateSelectedProjectCommandText => "Create Selected Project";

        public string ReloadProjectTemplatesCommandText => "Reload Templates";

        public string PackageProjectPlaceholderText => "Project";

        public string PackageIdPlaceholderText => "NuGet package ID";

        public string PackageVersionPlaceholderText => "Version (optional)";

        public string AddPackageCommandText => "Add Package";

        public string RemovePackageCommandText => "Remove Package";

        public string ListPackagesCommandText => "Refresh";

        public string SearchPackagesCommandText => "Search NuGet";

        public string PackageSearchPlaceholderText => "Search NuGet.org";

        public string BuildWorkspaceCommandText => _builderService is NullCodeStudioBuilderService
            ? UiStrings.BuildUnavailableText
            : UiStrings.BuildWorkspaceCommandText;

        public string StartDebuggingCommandText => UiStrings.StartDebuggingCommandText;

        public string StopDebuggingCommandText => UiStrings.StopDebuggingCommandText;

        public string ToggleBreakpointCommandText => UiStrings.ToggleBreakpointCommandText;

        public string ContinueDebuggingCommandText => UiStrings.ContinueDebuggingCommandText;

        public string PauseDebuggingCommandText => UiStrings.PauseDebuggingCommandText;

        public string StepIntoCommandText => UiStrings.StepIntoCommandText;

        public string StepOverCommandText => UiStrings.StepOverCommandText;

        public string StepOutCommandText => UiStrings.StepOutCommandText;

        public string RefreshDebugStackCommandText => UiStrings.RefreshDebugStackCommandText;

        public string LoadDebugVariablesCommandText => UiStrings.LoadDebugVariablesCommandText;

        public string DebugStackFramesTitle => UiStrings.DebugStackFramesTitle;

        public string DebugVariablesTitle => UiStrings.DebugVariablesTitle;

        public string DebugTargetPlaceholderText => UiStrings.DebugTargetPlaceholderText;

        public string NavigationPlaceholderMenuText => UiStrings.NavigationPlaceholderMenuText;

        public string StatusPlaceholderMenuText => UiStrings.StatusPlaceholderMenuText;

        [RelayCommand]
        private async Task OpenWorkspaceAsync(CancellationToken cancellationToken)
        {
            string? path = await _workspaceFileDialogService
                .ShowOpenWorkspaceDialogAsync(Workspace?.RootPath ?? Directory.GetCurrentDirectory(), cancellationToken);
            if (string.IsNullOrWhiteSpace(path))
            {
                return;
            }

            await OpenWorkspaceFileAsync(path, cancellationToken);
        }

        [RelayCommand]
        private async Task CreateConsoleProjectAsync(CancellationToken cancellationToken)
        {
            await CreateProjectFromTemplateAsync("console", cancellationToken);
        }

        [RelayCommand]
        private async Task CreateClassLibraryProjectAsync(CancellationToken cancellationToken)
        {
            await CreateProjectFromTemplateAsync("class-library", cancellationToken);
        }

        [RelayCommand]
        private async Task CreateTestProjectAsync(CancellationToken cancellationToken)
        {
            await CreateProjectFromTemplateAsync("mstest", cancellationToken);
        }

        [RelayCommand]
        private async Task CreateSelectedProjectAsync(CancellationToken cancellationToken)
        {
            if (SelectedProjectTemplate is null)
            {
                Editor.NotificationText = "Select a project template first.";
                return;
            }

            await CreateProjectFromTemplateAsync(SelectedProjectTemplate.Id, cancellationToken);
        }

        [RelayCommand]
        private void ReloadProjectTemplates()
        {
            LoadProjectTemplates(showStatus: true);
        }

        private void LoadProjectTemplates(bool showStatus)
        {
            try
            {
                string? selectedTemplateId = SelectedProjectTemplate?.Id;
                IReadOnlyList<CSharpProjectTemplateDefinition> templates = _projectTemplateService.ReloadTemplates();
                ProjectTemplates.Clear();
                foreach (CSharpProjectTemplateDefinition template in templates)
                {
                    ProjectTemplates.Add(template);
                }

                SelectedProjectTemplate = ProjectTemplates.FirstOrDefault(template =>
                    string.Equals(template.Id, selectedTemplateId, StringComparison.OrdinalIgnoreCase))
                    ?? ProjectTemplates.FirstOrDefault();

                if (showStatus)
                {
                    Editor.StatusText = $"Loaded {ProjectTemplates.Count} project templates.";
                    Editor.NotificationText = string.Join(
                        Environment.NewLine,
                        _projectTemplateService.TemplateDirectories);
                }
            }
            catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
            {
                Editor.StatusText = "Project template loading failed.";
                Editor.NotificationText = exception.Message;
            }
        }

        private async Task CreateProjectFromTemplateAsync(string templateId, CancellationToken cancellationToken)
        {
            CSharpProjectTemplateDefinition? template = ProjectTemplates.FirstOrDefault(candidate =>
                string.Equals(candidate.Id, templateId, StringComparison.OrdinalIgnoreCase));
            if (template is null)
            {
                Editor.StatusText = "Project template is unavailable.";
                Editor.NotificationText = $"Template '{templateId}' is not loaded. Reload the templates and try again.";
                return;
            }

            string defaultFileName = template.DefaultProjectFileName.Replace(
                "{{ProjectName}}",
                "NewProject",
                StringComparison.Ordinal);
            string? path = await _workspaceFileDialogService.ShowSaveCSharpProjectDialogAsync(
                Workspace?.RootPath ?? Directory.GetCurrentDirectory(),
                defaultFileName,
                cancellationToken);
            if (string.IsNullOrWhiteSpace(path))
            {
                return;
            }

            string projectPath;
            try
            {
                projectPath = _projectTemplateService.CreateProject(template.Id, path);
            }
            catch (Exception exception) when (exception is IOException or ArgumentException or KeyNotFoundException)
            {
                Editor.StatusText = "C# project creation failed.";
                Editor.NotificationText = exception.Message;
                return;
            }

            await OpenWorkspaceFileAsync(projectPath, cancellationToken);
            Editor.StatusText = $"Created {template.Name} '{Path.GetFileNameWithoutExtension(projectPath)}'.";
            Editor.NotificationText = projectPath;
        }

        [RelayCommand(CanExecute = nameof(CanManagePackages))]
        private async Task AddPackageAsync(CancellationToken cancellationToken)
        {
            CSharpWorkspaceProject project = SelectedPackageProject
                ?? throw new InvalidOperationException("Select a C# project before managing packages.");
            try
            {
                string output = await _projectPackageService.AddPackageAsync(
                    project.ProjectPath,
                    PackageId,
                    string.IsNullOrWhiteSpace(PackageVersion) ? null : PackageVersion,
                    cancellationToken);
                Editor.StatusText = $"Added NuGet package '{PackageId}' to {project.Name}.";
                Editor.NotificationText = output;
                await LoadInstalledPackagesAsync(project, cancellationToken, updateStatus: false);
            }
            catch (Exception exception) when (exception is IOException
                or InvalidOperationException
                or ArgumentException
                or System.ComponentModel.Win32Exception)
            {
                Editor.StatusText = "Adding NuGet package failed.";
                Editor.NotificationText = exception.Message;
            }
        }

        [RelayCommand(CanExecute = nameof(CanManagePackages))]
        private async Task RemovePackageAsync(CancellationToken cancellationToken)
        {
            CSharpWorkspaceProject project = SelectedPackageProject
                ?? throw new InvalidOperationException("Select a C# project before managing packages.");
            try
            {
                string output = await _projectPackageService.RemovePackageAsync(
                    project.ProjectPath,
                    PackageId,
                    cancellationToken);
                Editor.StatusText = $"Removed NuGet package '{PackageId}' from {project.Name}.";
                Editor.NotificationText = output;
                await LoadInstalledPackagesAsync(project, cancellationToken, updateStatus: false);
            }
            catch (Exception exception) when (exception is IOException
                or InvalidOperationException
                or ArgumentException
                or System.ComponentModel.Win32Exception)
            {
                Editor.StatusText = "Removing NuGet package failed.";
                Editor.NotificationText = exception.Message;
            }
        }

        private bool CanManagePackages()
        {
            return SelectedPackageProject is not null
                && !string.IsNullOrWhiteSpace(PackageId);
        }

        [RelayCommand]
        private async Task RefreshPackagesAsync(CancellationToken cancellationToken)
        {
            if (SelectedPackageProject is { } project)
            {
                await LoadInstalledPackagesAsync(project, cancellationToken);
            }
        }

        private async Task LoadInstalledPackagesAsync(
            CSharpWorkspaceProject project,
            CancellationToken cancellationToken,
            bool updateStatus = true)
        {
            try
            {
                IReadOnlyList<CSharpProjectPackageReference> packages =
                    await _projectPackageService.ListPackagesAsync(project.ProjectPath, cancellationToken);
                InstalledPackages.Clear();
                foreach (CSharpProjectPackageReference package in packages)
                {
                    InstalledPackages.Add(package);
                }

                string? selectedPackageId = SelectedInstalledPackage?.Id;
                SelectedInstalledPackage = InstalledPackages.FirstOrDefault(package =>
                    string.Equals(package.Id, selectedPackageId, StringComparison.OrdinalIgnoreCase))
                    ?? InstalledPackages.FirstOrDefault();
                if (updateStatus)
                {
                    Editor.StatusText = $"Loaded {InstalledPackages.Count} package references for {project.Name}.";
                    Editor.NotificationText = string.Join(
                        Environment.NewLine,
                        InstalledPackages.Select(static package =>
                            $"{package.Id} {package.ResolvedVersion} ({package.TargetFramework})"));
                }
            }
            catch (Exception exception) when (exception is IOException
                or InvalidOperationException
                or ArgumentException
                or System.ComponentModel.Win32Exception
                or System.Net.Http.HttpRequestException
                or System.Text.Json.JsonException)
            {
                Editor.StatusText = "Loading NuGet packages failed.";
                Editor.NotificationText = exception.Message;
            }
        }

        [RelayCommand]
        private async Task SearchPackagesAsync(CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(PackageSearchText))
            {
                Editor.StatusText = "Enter a package name or search term.";
                return;
            }

            try
            {
                IReadOnlyList<NuGetPackageSearchResult> results =
                    await _projectPackageService.SearchPackagesAsync(PackageSearchText, cancellationToken: cancellationToken);
                PackageSearchResults.Clear();
                foreach (NuGetPackageSearchResult result in results)
                {
                    PackageSearchResults.Add(result);
                }

                SelectedPackageSearchResult = PackageSearchResults.FirstOrDefault();
                Editor.StatusText = $"Found {PackageSearchResults.Count} NuGet packages.";
                Editor.NotificationText = string.Join(
                    Environment.NewLine,
                    PackageSearchResults.Select(static package => $"{package.Id} {package.LatestVersion}: {package.Description}"));
            }
            catch (Exception exception) when (exception is IOException
                or InvalidOperationException
                or ArgumentException
                or System.Net.Http.HttpRequestException
                or System.Text.Json.JsonException)
            {
                Editor.StatusText = "NuGet package search failed.";
                Editor.NotificationText = exception.Message;
            }
        }

        partial void OnWorkspaceChanged(CSharpWorkspace? value)
        {
            string? previousProjectPath = SelectedPackageProject?.ProjectPath;
            PackageProjects.Clear();
            if (value is not null)
            {
                foreach (CSharpWorkspaceProject project in value.Projects)
                {
                    PackageProjects.Add(project);
                }
            }

            SelectedPackageProject = PackageProjects.FirstOrDefault(project =>
                string.Equals(project.ProjectPath, previousProjectPath, OperatingSystem.IsWindows()
                    ? StringComparison.OrdinalIgnoreCase
                    : StringComparison.Ordinal))
                ?? PackageProjects.FirstOrDefault();
        }

        partial void OnSelectedPackageProjectChanged(CSharpWorkspaceProject? value)
        {
            AddPackageCommand.NotifyCanExecuteChanged();
            RemovePackageCommand.NotifyCanExecuteChanged();
            InstalledPackages.Clear();
            SelectedInstalledPackage = null;
        }

        partial void OnSelectedInstalledPackageChanged(CSharpProjectPackageReference? value)
        {
            if (value is not null)
            {
                PackageId = value.Id;
                PackageVersion = value.RequestedVersion;
            }
        }

        partial void OnSelectedPackageSearchResultChanged(NuGetPackageSearchResult? value)
        {
            if (value is not null)
            {
                PackageId = value.Id;
                PackageVersion = value.LatestVersion;
            }
        }

        partial void OnPackageIdChanged(string value)
        {
            AddPackageCommand.NotifyCanExecuteChanged();
            RemovePackageCommand.NotifyCanExecuteChanged();
        }

        [RelayCommand(CanExecute = nameof(CanBuildWorkspace))]
        public async Task BuildWorkspaceAsync(CancellationToken cancellationToken = default)
        {
            CSharpWorkspace workspace = Workspace
                ?? throw new InvalidOperationException("Open a C# workspace before building.");
            List<Diagnostic> diagnostics = [];
            int succeededCount = 0;

            foreach (CSharpWorkspaceProject project in workspace.Projects)
            {
                cancellationToken.ThrowIfCancellationRequested();
                BuilderBuildResult result = await _builderService.BuildProjectAsync(new BuilderBuildRequest
                {
                    WorkspaceRootPath = workspace.RootPath,
                    ProjectPath = project.ProjectPath,
                }, cancellationToken);
                succeededCount += result.Succeeded ? 1 : 0;
                diagnostics.AddRange(result.Diagnostics.Select(MapDiagnostic));
            }

            _buildDiagnostics.Clear();
            _buildDiagnostics.AddRange(diagnostics);
            await RefreshDiagnosticsAsync(cancellationToken);
            Editor.StatusText = $"Build: {succeededCount}/{workspace.Projects.Count} projects succeeded.";
            Editor.NotificationText = $"Build diagnostics: {diagnostics.Count}.";
        }

        public async Task OpenWorkspaceFileAsync(string path, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            CSharpWorkspace workspace = _workspaceLoader.Load(path);
            if (ProjectExplorer is not null)
            {
                await ProjectExplorer.LoadAsync(workspace.RootPath, cancellationToken);
            }

            Workspace = workspace;
            QueueLanguageAnalysis();
            VersionControlStatus = await _versionControlService.GetStatusAsync(new VersionControlStatusRequest
            {
                RepositoryContextPath = workspace.RootPath,
                IncludeChanges = true,
                IncludeCapabilities = true,
            }, cancellationToken);
        }

        private bool CanBuildWorkspace()
        {
            return _builderService is not NullCodeStudioBuilderService
                && Workspace is { Projects.Count: > 0 };
        }

        [RelayCommand(CanExecute = nameof(CanRunWorkspaceTests))]
        private async Task RunWorkspaceTestsAsync(CancellationToken cancellationToken)
        {
            CSharpWorkspace workspace = Workspace
                ?? throw new InvalidOperationException("Open a C# workspace before running tests.");
            TestRunSummary summary = await _testExecutionService.RunTestsAsync(new TestRunRequest
            {
                WorkspaceRootPath = workspace.RootPath,
                ProjectPath = workspace.EntryPath,
            }, cancellationToken);

            Editor.StatusText = $"Tests: {summary.Outcome} ({summary.TotalCount} total).";
            Editor.NotificationText =
                $"Tests: {summary.PassedCount} passed, {summary.FailedCount} failed, {summary.SkippedCount} skipped.";
        }

        private bool CanRunWorkspaceTests()
        {
            return _testExecutionService is not NullTestExecutionService
                && Workspace is { Projects.Count: > 0 };
        }

        [RelayCommand(CanExecute = nameof(CanStartDebugSession))]
        private async Task StartDebugSessionAsync(CancellationToken cancellationToken)
        {
            CSharpWorkspace workspace = Workspace
                ?? throw new InvalidOperationException("Open a C# workspace before starting a debug session.");
            string targetPath = Path.IsPathRooted(DebugTargetPath)
                ? DebugTargetPath
                : Path.Combine(workspace.RootPath, DebugTargetPath);
            DebugSessionInfo session = await _debugSessionService.StartSessionAsync(new DebugLaunchRequest
            {
                Target = targetPath,
                WorkingDirectory = workspace.RootPath,
                StopAtEntry = true,
            }, cancellationToken);

            _activeDebugSessionId = session.SessionId;
            ActiveDebugSessionState = session.State;
            _debugBreakpoints.Clear();
            DebugStackFrames.Clear();
            DebugVariables.Clear();
            SelectedDebugStackFrame = null;
            OnPropertyChanged(nameof(HasDebugSession));
            StartDebugSessionCommand.NotifyCanExecuteChanged();
            StopDebugSessionCommand.NotifyCanExecuteChanged();
            NotifyDebugControlCommands();
            Editor.StatusText = session.State == DebugSessionState.Paused
                ? $"Debugging paused at entry: {session.DisplayName ?? targetPath}."
                : $"Debugging: {session.DisplayName ?? targetPath}.";
            if (session.State == DebugSessionState.Paused)
            {
                _ = RefreshDebugStackForStoppedEventAsync(session.SessionId);
            }
        }

        [RelayCommand(CanExecute = nameof(CanStopDebugSession))]
        private async Task StopDebugSessionAsync(CancellationToken cancellationToken)
        {
            string sessionId = _activeDebugSessionId
                ?? throw new InvalidOperationException("There is no active debug session.");
            try
            {
                await _debugSessionService.StopSessionAsync(sessionId, cancellationToken);
                Editor.StatusText = "Debug session stopped.";
            }
            finally
            {
                _activeDebugSessionId = null;
                ActiveDebugSessionState = null;
                _debugBreakpoints.Clear();
                DebugStackFrames.Clear();
                DebugVariables.Clear();
                SelectedDebugStackFrame = null;
                OnPropertyChanged(nameof(HasDebugSession));
                StartDebugSessionCommand.NotifyCanExecuteChanged();
                StopDebugSessionCommand.NotifyCanExecuteChanged();
                NotifyDebugControlCommands();
            }
        }

        [RelayCommand(CanExecute = nameof(CanContinueDebugSession))]
        private async Task ContinueDebugSessionAsync(CancellationToken cancellationToken)
        {
            await _debugSessionService.ContinueSessionAsync(GetActiveDebugSessionId(), cancellationToken);
            ActiveDebugSessionState = DebugSessionState.Running;
            ClearDebugInspection();
            NotifyDebugControlCommands();
            Editor.StatusText = "Debug session continued.";
        }

        [RelayCommand(CanExecute = nameof(CanPauseDebugSession))]
        private async Task PauseDebugSessionAsync(CancellationToken cancellationToken)
        {
            await _debugSessionService.PauseSessionAsync(GetActiveDebugSessionId(), cancellationToken);
            ClearDebugInspection();
            Editor.StatusText = "Debug session pause requested.";
        }

        [RelayCommand(CanExecute = nameof(CanStepDebugSession))]
        private async Task StepIntoAsync(CancellationToken cancellationToken)
        {
            await StepDebugSessionAsync(DebugStepKind.Into, cancellationToken);
        }

        [RelayCommand(CanExecute = nameof(CanStepDebugSession))]
        private async Task StepOverAsync(CancellationToken cancellationToken)
        {
            await StepDebugSessionAsync(DebugStepKind.Over, cancellationToken);
        }

        [RelayCommand(CanExecute = nameof(CanStepDebugSession))]
        private async Task StepOutAsync(CancellationToken cancellationToken)
        {
            await StepDebugSessionAsync(DebugStepKind.Out, cancellationToken);
        }

        [RelayCommand(CanExecute = nameof(CanInspectDebugSession))]
        private async Task RefreshDebugStackAsync(CancellationToken cancellationToken)
        {
            await RefreshDebugStackAsync(GetActiveDebugSessionId(), cancellationToken);
        }

        [RelayCommand(CanExecute = nameof(CanLoadDebugVariables))]
        private async Task LoadDebugVariablesAsync(CancellationToken cancellationToken)
        {
            DebugStackFrame selectedFrame = SelectedDebugStackFrame
                ?? throw new InvalidOperationException("Select a stack frame before loading variables.");
            await LoadVariablesForFrameAsync(selectedFrame.Id, cancellationToken);
        }

        [RelayCommand(CanExecute = nameof(CanToggleBreakpoint))]
        private async Task ToggleBreakpointAsync(CancellationToken cancellationToken)
        {
            string sessionId = GetActiveDebugSessionId();
            string sourcePath = Path.GetFullPath(Editor.CurrentFilePath
                ?? throw new InvalidOperationException("Open a C# source file before setting a breakpoint."));
            string source = Editor.Text ?? string.Empty;
            int line = GetLineNumber(source, _observedEditorTextArea?.CaretOffset ?? 0);
            HashSet<int> updatedLines = _debugBreakpoints.TryGetValue(sourcePath, out HashSet<int>? currentLines)
                ? new HashSet<int>(currentLines)
                : [];
            bool added = updatedLines.Add(line);
            if (!added)
            {
                updatedLines.Remove(line);
            }

            await _debugSessionService.SetBreakpointsAsync(
                sessionId,
                sourcePath,
                updatedLines.Order().ToArray(),
                cancellationToken);

            if (updatedLines.Count == 0)
            {
                _debugBreakpoints.Remove(sourcePath);
            }
            else
            {
                _debugBreakpoints[sourcePath] = updatedLines;
            }

            Editor.StatusText = added
                ? $"Breakpoint set at {Path.GetFileName(sourcePath)}:{line}."
                : $"Breakpoint removed at {Path.GetFileName(sourcePath)}:{line}.";
        }

        private bool CanStartDebugSession()
        {
            if (_debugSessionService is NullDebugSessionService
                || Workspace is null
                || _activeDebugSessionId is not null
                || string.IsNullOrWhiteSpace(DebugTargetPath))
            {
                return false;
            }

            string targetPath = Path.IsPathRooted(DebugTargetPath)
                ? DebugTargetPath
                : Path.Combine(Workspace.RootPath, DebugTargetPath);
            return File.Exists(targetPath);
        }

        private bool CanStopDebugSession()
        {
            return _activeDebugSessionId is not null;
        }

        private bool CanContinueDebugSession()
        {
            return _activeDebugSessionId is not null && ActiveDebugSessionState == DebugSessionState.Paused;
        }

        private bool CanPauseDebugSession()
        {
            return _activeDebugSessionId is not null && ActiveDebugSessionState == DebugSessionState.Running;
        }

        private bool CanStepDebugSession()
        {
            return CanContinueDebugSession();
        }

        private bool CanInspectDebugSession()
        {
            return CanContinueDebugSession();
        }

        private bool CanLoadDebugVariables()
        {
            return CanInspectDebugSession() && SelectedDebugStackFrame is not null;
        }

        private bool CanToggleBreakpoint()
        {
            return _activeDebugSessionId is not null
                && (ActiveDebugSessionState is DebugSessionState.Running or DebugSessionState.Paused)
                && !string.IsNullOrWhiteSpace(Editor.CurrentFilePath)
                && string.Equals(Path.GetExtension(Editor.CurrentFilePath), ".cs", StringComparison.OrdinalIgnoreCase);
        }

        private string GetActiveDebugSessionId()
        {
            return _activeDebugSessionId
                ?? throw new InvalidOperationException("There is no active debug session.");
        }

        private async Task StepDebugSessionAsync(DebugStepKind stepKind, CancellationToken cancellationToken)
        {
            await _debugSessionService.StepSessionAsync(GetActiveDebugSessionId(), stepKind, cancellationToken);
            ActiveDebugSessionState = DebugSessionState.Running;
            ClearDebugInspection();
            NotifyDebugControlCommands();
            Editor.StatusText = stepKind switch
            {
                DebugStepKind.Into => "Stepped into the next statement.",
                DebugStepKind.Over => "Stepped over the current statement.",
                DebugStepKind.Out => "Stepped out of the current function.",
                _ => throw new ArgumentOutOfRangeException(nameof(stepKind), stepKind, "Unknown debugger step operation."),
            };
        }

        private void ClearDebugInspection()
        {
            DebugStackFrames.Clear();
            DebugVariables.Clear();
            SelectedDebugStackFrame = null;
        }

        private async Task LoadVariablesForFrameAsync(int frameId, CancellationToken cancellationToken)
        {
            IReadOnlyList<DebugVariable> variables = await _debugSessionService.GetVariablesAsync(
                GetActiveDebugSessionId(),
                frameId,
                cancellationToken);
            DebugVariables.Clear();
            foreach (DebugVariable variable in variables)
            {
                DebugVariables.Add(variable);
            }

            Editor.NotificationText = $"Loaded {variables.Count} variables.";
        }

        private async Task RefreshDebugStackAsync(string sessionId, CancellationToken cancellationToken)
        {
            IReadOnlyList<DebugStackFrame> frames = await _debugSessionService.GetStackTraceAsync(
                sessionId,
                cancellationToken);
            if (!string.Equals(_activeDebugSessionId, sessionId, StringComparison.Ordinal))
            {
                return;
            }

            DebugStackFrames.Clear();
            foreach (DebugStackFrame frame in frames)
            {
                DebugStackFrames.Add(frame);
            }

            SelectedDebugStackFrame = DebugStackFrames.FirstOrDefault();
            if (SelectedDebugStackFrame is null)
            {
                DebugVariables.Clear();
                Editor.NotificationText = "No stack frames are available for this stop.";
                return;
            }

            DebugStackFrame selectedFrame = SelectedDebugStackFrame;
            if (!string.IsNullOrWhiteSpace(selectedFrame.SourcePath)
                && selectedFrame.Line > 0
                && File.Exists(selectedFrame.SourcePath))
            {
                await _editorWorkspace.OpenOrActivateAsync(selectedFrame.SourcePath, cancellationToken);
                if (!string.Equals(_activeDebugSessionId, sessionId, StringComparison.Ordinal))
                {
                    return;
                }

                EditorTabViewModel? targetTab = _editorWorkspace.FindByPath(selectedFrame.SourcePath);
                if (targetTab?.View is Controls.EditorTextArea editorTextArea)
                {
                    editorTextArea.FocusAndMoveCaret(new CodeLocation(
                        selectedFrame.SourcePath,
                        selectedFrame.Line,
                        selectedFrame.Column));
                }
            }

            await LoadVariablesForFrameAsync(selectedFrame.Id, cancellationToken);
        }

        private void OnDebugSessionEventReceived(object? sender, DebugSessionEventArgs e)
        {
            Dispatcher.UIThread.Post(() =>
            {
                if (!string.Equals(_activeDebugSessionId, e.SessionId, StringComparison.Ordinal))
                {
                    return;
                }

                if (e.State is { } state)
                {
                    ActiveDebugSessionState = state;
                    NotifyDebugControlCommands();
                }

                switch (e.EventName)
                {
                    case "stopped":
                        Editor.StatusText = string.IsNullOrWhiteSpace(e.Reason)
                            ? "Debug session paused."
                            : $"Debug session paused: {e.Reason}.";
                        _ = RefreshDebugStackForStoppedEventAsync(e.SessionId);
                        break;
                    case "continued":
                        ClearDebugInspection();
                        Editor.StatusText = "Debug session running.";
                        break;
                    case "terminated":
                    case "exited":
                        ClearDebugInspection();
                        Editor.StatusText = "Debug target exited.";
                        break;
                    case "output":
                        if (!string.IsNullOrWhiteSpace(e.Output))
                        {
                            Editor.NotificationText = e.Output.TrimEnd();
                        }

                        break;
                }
            });
        }

        private async Task RefreshDebugStackForStoppedEventAsync(string sessionId)
        {
            try
            {
                await RefreshDebugStackAsync(sessionId, CancellationToken.None);
            }
            catch (Exception exception)
            {
                if (string.Equals(_activeDebugSessionId, sessionId, StringComparison.Ordinal))
                {
                    Editor.NotificationText = $"Could not refresh debugger stack: {exception.Message}";
                }
            }
        }

        private static int GetLineNumber(string text, int caretOffset)
        {
            int line = 1;
            int boundedOffset = Math.Clamp(caretOffset, 0, text.Length);
            for (int index = 0; index < boundedOffset; index++)
            {
                if (text[index] == '\n')
                {
                    line++;
                }
            }

            return line;
        }

        private void NotifyDebugControlCommands()
        {
            ContinueDebugSessionCommand.NotifyCanExecuteChanged();
            PauseDebugSessionCommand.NotifyCanExecuteChanged();
            StepIntoCommand.NotifyCanExecuteChanged();
            StepOverCommand.NotifyCanExecuteChanged();
            StepOutCommand.NotifyCanExecuteChanged();
            ToggleBreakpointCommand.NotifyCanExecuteChanged();
            RefreshDebugStackCommand.NotifyCanExecuteChanged();
            LoadDebugVariablesCommand.NotifyCanExecuteChanged();
        }

        private void OnEditorWorkspacePropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(EditorDocumentWorkspace.ActiveTab))
            {
                return;
            }

            if (_observedEditor is not null)
            {
                _observedEditor.PropertyChanged -= OnActiveEditorPropertyChanged;
            }

            _observedEditor = _editorWorkspace.ActiveTab.Editor;
            _observedEditor.PropertyChanged += OnActiveEditorPropertyChanged;
            ObserveActiveEditorControl();
            OnPropertyChanged(nameof(ActiveEditorTab));
            OnPropertyChanged(nameof(Editor));
            OnPropertyChanged(nameof(EditorComponent));
            OnPropertyChanged(nameof(WindowTitle));
            ToggleBreakpointCommand.NotifyCanExecuteChanged();
            QueueLanguageAnalysis();
            RefreshAxamlPreview();
        }

        private void ObserveActiveEditorControl()
        {
            if (_observedEditorTextArea is not null)
            {
                _observedEditorTextArea.CompletionRequested -= OnCompletionRequested;
                _observedEditorTextArea.GoToDefinitionRequested -= OnGoToDefinitionRequested;
            }

            _observedEditorTextArea = _editorWorkspace.ActiveTab.View as Controls.EditorTextArea;
            if (_observedEditorTextArea is not null)
            {
                _observedEditorTextArea.CompletionRequested += OnCompletionRequested;
                _observedEditorTextArea.GoToDefinitionRequested += OnGoToDefinitionRequested;
            }
        }

        private void OnCompletionRequested(object? sender, EventArgs e)
        {
            RequestCompletionsCommand.Execute(null);
        }

        private void OnGoToDefinitionRequested(object? sender, EventArgs e)
        {
            GoToDefinitionCommand.Execute(null);
        }

        private static bool IsIdentifierCharacter(char character)
        {
            return char.IsLetterOrDigit(character) || character == '_';
        }

        private void OnActiveEditorPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(EditorViewModel.WindowTitle))
            {
                OnPropertyChanged(nameof(WindowTitle));
            }

            if (e.PropertyName is nameof(EditorViewModel.Text) or nameof(EditorViewModel.CurrentFilePath))
            {
                if (e.PropertyName == nameof(EditorViewModel.CurrentFilePath))
                {
                    ToggleBreakpointCommand.NotifyCanExecuteChanged();
                }

                QueueLanguageAnalysis();
                RefreshAxamlPreview();
            }
        }

        private void RefreshAxamlPreview()
        {
            string? filePath = Editor.CurrentFilePath;
            bool isAxamlDocument = !string.IsNullOrWhiteSpace(filePath)
                && string.Equals(Path.GetExtension(filePath), ".axaml", StringComparison.OrdinalIgnoreCase);
            _axamlDesigner.LoadMarkup(isAxamlDocument ? Editor.Text : null);
        }

        private void OnAxamlDesignerMarkupChanged(object? sender, AxamlDesignerMarkupChangedEventArgs e)
        {
            Editor.Text = e.Markup;
            Editor.NotificationText = _axamlDesigner.StatusText;
        }

        private void OnAxamlDesignerPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(AxamlDesignerViewModel.HasPreview))
            {
                OnPropertyChanged(nameof(HasAxamlPreview));
            }
        }

        private async void QueueLanguageAnalysis()
        {
            _languageAnalysisCancellation?.Cancel();
            _languageAnalysisCancellation?.Dispose();
            CancellationTokenSource cancellation = new();
            _languageAnalysisCancellation = cancellation;

            try
            {
                await AnalyzeActiveDocumentAsync(cancellation.Token);
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception exception)
            {
                Editor.NotificationText = $"C# analysis failed: {exception.Message}";
            }
        }

        private async Task AnalyzeActiveDocumentAsync(CancellationToken cancellationToken)
        {
            string? filePath = Editor.CurrentFilePath;
            if (string.IsNullOrWhiteSpace(filePath)
                || !string.Equals(Path.GetExtension(filePath), ".cs", StringComparison.OrdinalIgnoreCase))
            {
                _languageDiagnostics = [];
                await RefreshDiagnosticsAsync(cancellationToken);
                return;
            }

            string source = Editor.Text;
            await Task.Delay(TimeSpan.FromMilliseconds(300), cancellationToken);
            List<Diagnostic> diagnostics = (await _csharpSyntaxDiagnosticService.AnalyzeAsync(
                source,
                filePath,
                Workspace,
                cancellationToken)).ToList();
            cancellationToken.ThrowIfCancellationRequested();
            if (!string.Equals(filePath, Editor.CurrentFilePath, StringComparison.OrdinalIgnoreCase)
                || !string.Equals(source, Editor.Text, StringComparison.Ordinal))
            {
                return;
            }

            _languageDiagnostics = diagnostics;
            await RefreshDiagnosticsAsync(cancellationToken);
        }

        private ValueTask RefreshDiagnosticsAsync(CancellationToken cancellationToken)
        {
            return DiagnosticCollection.ConsumeAsync(_buildDiagnostics.Concat(_languageDiagnostics), cancellationToken);
        }

        private static Diagnostic MapDiagnostic(BuilderDiagnostic diagnostic)
        {
            return new Diagnostic
            {
                Code = diagnostic.Code,
                Message = diagnostic.Message,
                Severity = diagnostic.Severity switch
                {
                    BuilderDiagnosticSeverity.Info => DiagnosticSeverity.Info,
                    BuilderDiagnosticSeverity.Warning => DiagnosticSeverity.Warning,
                    BuilderDiagnosticSeverity.Error => DiagnosticSeverity.Error,
                    _ => DiagnosticSeverity.Info,
                },
                SourcePath = diagnostic.FilePath,
                LineNumber = diagnostic.LineNumber,
                ColumnNumber = diagnostic.ColumnNumber,
            };
        }
    }
}
