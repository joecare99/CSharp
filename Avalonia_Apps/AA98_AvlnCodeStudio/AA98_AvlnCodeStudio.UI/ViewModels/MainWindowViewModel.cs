using AA98_AvlnCodeStudio.UI.Resources;
using AA98_AvlnCodeStudio.Base.AI.Models;
using AA98_AvlnCodeStudio.Base.AI.Services;
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
using System.Security.Cryptography;
using System.Text;
using AA98_AvlnCodeStudio.Planning.Core.Services;
using Config.UI.Avalonia.ViewModels;
using Project.Explorer.Avalonia.ViewModels;
using AA98_AvlnCodeStudio.Model.Workspaces;
using AA98_AvlnCodeStudio.AxamlDesigner.UI.ViewModels;
using AA98_AvlnCodeStudio.UI.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Threading;
using Avalonia.Controls;
using Code.Navigation;
using System.Text.Json;
using System.Diagnostics.CodeAnalysis;
using Config.Service;
using AA98_AvlnCodeStudio.Versioning.Svn.Services;
using AA98_AvlnCodeStudio.UI.Configuration;
using AA98_AvlnCodeStudio.Config.Secrets;
using Property.Editor;
using Property.Editor.Avalonia;
using Project.Explorer;

namespace AA98_AvlnCodeStudio.UI.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        private readonly EditorDocumentWorkspace _editorWorkspace;
        private readonly AA98_AvlnCodeStudio.Base.UI.Services.IEditorFileDialogService _editorFileDialogService;
        private readonly CSharpWorkspaceLoader _workspaceLoader;
        private readonly CSharpProjectTemplateService _projectTemplateService;
        private readonly AxamlMvvmGenerator _axamlMvvmGenerator;
        private readonly WorkspaceFileChangeService _workspaceFileChangeService;
        private readonly ICSharpProjectPackageService _projectPackageService;
        private readonly AxamlDesignerViewModel _emptyDesigner;
        private readonly Services.IWorkspaceFileDialogService _workspaceFileDialogService;
        private readonly ICodeStudioBuilderService _builderService;
        private readonly IVersionControlService _versionControlService;
        private readonly ISvnWorkingCopyOperations? _svnWorkingCopyOperations;
        private readonly Services.CSharpSyntaxDiagnosticService _csharpSyntaxDiagnosticService;
        private readonly ITestExecutionService _testExecutionService;
        private readonly IDebugSessionService _debugSessionService;
        private readonly AIChatService? _aiChatService;
        private readonly OpenAICompatibleAISettings? _aiSettings;
        private readonly Configuration.CodeStudioSettings? _codeStudioSettings;
        private readonly ConfigService? _configService;
        private readonly WorkbenchSettingsRuntimeActivator? _settingsRuntimeActivator;
        private readonly CredentialProviderCatalog? _credentialProviderCatalog;
        private long _credentialContextVersion;
        private long _credentialStatusRevision;
        private long _workspaceOpenVersion;
        private readonly List<AIMessage> _aiConversation = [];
        private readonly List<Diagnostic> _buildDiagnostics = [];
        private readonly List<Diagnostic> _testDiagnostics = [];
        private readonly Dictionary<string, HashSet<int>> _debugBreakpoints = new(
            OperatingSystem.IsWindows() ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal);
        private List<Diagnostic> _languageDiagnostics = [];
        private CancellationTokenSource? _languageAnalysisCancellation;
        private CancellationTokenSource? _axamlBindingAnalysisCancellation;
        private EditorViewModel? _observedEditor;
        private EditorTabViewModel? _observedTab;
        private EditorTabViewModel? _selectedPropertyTab;
        private bool _editorPropertySelection;
        private AxamlDesignerViewModel? _observedDesigner;
        private Controls.EditorTextArea? _observedEditorTextArea;
        private GridLength _workspacePaneWidth = new(280);
        private bool _isWorkspacePaneVisible = true;

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
            AxamlDesignerViewModel? axamlDesignerViewModel = null,
            AIChatService? aiChatService = null,
            OpenAICompatibleAISettings? aiSettings = null,
            Configuration.CodeStudioSettings? codeStudioSettings = null,
            ConfigService? configService = null,
            ISvnWorkingCopyOperations? svnWorkingCopyOperations = null,
            WorkspaceFileChangeService? workspaceFileChangeService = null,
            AxamlMvvmGenerator? axamlMvvmGenerator = null,
            WorkbenchSettingsRuntimeActivator? settingsRuntimeActivator = null,
            CredentialProviderCatalog? credentialProviderCatalog = null,
            IProjectTargetResolver? projectTargetResolver = null,
            IProjectRuntimeBuilder? projectRuntimeBuilder = null)
        {
            _editorWorkspace = editorWorkspace ?? new EditorDocumentWorkspace(editorComponent);
            _workspaceLoader = workspaceLoader ?? new CSharpWorkspaceLoader();
            _projectTemplateService = projectTemplateService ?? new CSharpProjectTemplateService();
            _axamlMvvmGenerator = axamlMvvmGenerator ?? new AxamlMvvmGenerator();
            _workspaceFileChangeService = workspaceFileChangeService
                ?? new WorkspaceFileChangeService(_editorWorkspace);
            _workspaceFileChangeService.HistoryChanged += OnWorkspaceChangeHistoryChanged;
            _projectPackageService = projectPackageService ?? new DotNetCSharpProjectPackageService();
            _emptyDesigner = axamlDesignerViewModel ?? new AxamlDesignerViewModel();
            _workspaceFileDialogService = workspaceFileDialogService ?? new Services.DesignWorkspaceFileDialogService();
            _builderService = builderService ?? new NullCodeStudioBuilderService();
            _versionControlService = versionControlService ?? new NullVersionControlService();
            _svnWorkingCopyOperations = svnWorkingCopyOperations;
            _editorFileDialogService = editorFileDialogService ?? new Services.DesignEditorFileDialogService();
            _csharpSyntaxDiagnosticService = csharpSyntaxDiagnosticService ?? new Services.CSharpSyntaxDiagnosticService();
            _testExecutionService = testExecutionService ?? new NullTestExecutionService();
            _debugSessionService = debugSessionService ?? new NullDebugSessionService();
            _projectTargetResolver = projectTargetResolver ?? new DotNetProjectTargetResolver();
            _projectRuntimeBuilder = projectRuntimeBuilder ?? new DotNetProjectRuntimeBuilder();
            _aiChatService = aiChatService;
            _aiSettings = aiSettings;
            _codeStudioSettings = codeStudioSettings;
            _configService = configService;
            _settingsRuntimeActivator = settingsRuntimeActivator;
            _credentialProviderCatalog = credentialProviderCatalog;
            if (_aiSettings is not null)
            {
                string initialKey = _aiSettings.ApiKey;
                AiEndpoint = _aiSettings.Endpoint;
                AiModel = _aiSettings.ModelId;
                AiApiKey = initialKey;
            }
            else if (_codeStudioSettings is not null)
            {
                AiEndpoint = _codeStudioSettings.GetEffectiveAIEndpoint();
                AiModel = _codeStudioSettings.GetEffectiveAIModelId();
            }
            SelectedCredentialProviderId = GetCredentialProviderId(_codeStudioSettings?.CredentialManager);
            _debugSessionService.SessionEventReceived += OnDebugSessionEventReceived;
            _editorWorkspace.PropertyChanged += OnEditorWorkspacePropertyChanged;
            _editorWorkspace.TabClosed += OnEditorTabClosed;
            _editorWorkspace.NavigationRequested += OnEditorNavigationRequested;
            _observedEditor = _editorWorkspace.ActiveTab.Editor;
            _observedEditor.PropertyChanged += OnActiveEditorPropertyChanged;
            ObserveActiveTab();
            ObserveActiveEditorControl();
            CompletionSuggestions.CollectionChanged += (_, _) => OnPropertyChanged(nameof(HasCompletionSuggestions));
            PlanningExplorer = planningExplorer;
            DiagnosticCollection = diagnosticCollection ?? throw new ArgumentNullException(nameof(diagnosticCollection));
            ConfigUi = configUi;
            if (ConfigUi is not null)
            {
                ConfigUi.SectionAppliedAsync = OnConfigurationSectionAppliedAsync;
                if (_codeStudioSettings is not null)
                {
                    ConfigUi.ValidateChangesAsync = _settingsRuntimeActivator is not null
                        ? _settingsRuntimeActivator.ValidateChangesAsync
                        : sections => WorkbenchSettingsValidation.ValidateAsync(sections, _codeStudioSettings);
                }
            }
            ProjectExplorer = projectExplorer;
            if (ProjectExplorer is not null)
            {
                ProjectExplorer.UserSelectionChanged += OnProjectExplorerUserSelectionChanged;
            }
            PlanningExplorer.LoadAsync(Directory.GetCurrentDirectory()).GetAwaiter().GetResult();
            if (ProjectExplorer is not null)
            {
                _ = ProjectExplorer.LoadAsync(Directory.GetCurrentDirectory());
            }
            LoadProjectTemplates(showStatus: false);
            RefreshAxamlPreview();
        }

        public EditorDocumentWorkspace EditorWorkspace => _editorWorkspace;

        public WorkspaceFileChangeService WorkspaceFileChangeService => _workspaceFileChangeService;

        /// <summary>Reviews and applies a proposal; returns true only after successful application.</summary>
        public Func<WorkspaceFileChangeProposal, CancellationToken, Task<bool>>? ReviewWorkspaceFileChangesAsync { get; set; }

        public EditorTabViewModel ActiveEditorTab
        {
            get => _editorWorkspace.ActiveTab;
            set
            {
                if (value is not null && _editorWorkspace.Tabs.Contains(value))
                {
                    _editorWorkspace.Activate(value);
                }
            }
        }

        public EditorViewModel Editor => _editorWorkspace.ActiveTab.Editor;

        public PlanningExplorerViewModel PlanningExplorer { get; }

        public DiagnosticCollectionViewModel DiagnosticCollection { get; }

        /// <summary>
        /// Gets the host-composed reusable configuration UI state.
        /// </summary>
        public ConfigUiViewModel? ConfigUi { get; }

        /// <summary>Opens the host's central settings dialog.</summary>
        public Func<Task>? OpenConfigurationDialogAsync { get; set; }

        /// <summary>Opens the settings dialog for an explicitly selected project.</summary>
        public Func<Task>? OpenProjectConfigurationDialogAsync { get; set; }

        /// <summary>Opens settings for an explicitly selected configuration layer.</summary>
        public Func<ConfigLayer, Task>? OpenConfigurationLayerDialogAsync { get; set; }

        /// <summary>Opens the AI credential manager outside the chat pane.</summary>
        public Func<Task>? OpenAICredentialsDialogAsync { get; set; }

        /// <summary>Gets the host-composed reusable project explorer state.</summary>
        public ProjectExplorerViewModel? ProjectExplorer { get; }

        public Components.IAvaloniaEditorComponent? EditorComponent => _editorWorkspace.ActiveTab.Component;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(WorkspaceTitle))]
        [NotifyPropertyChangedFor(nameof(CanUpdateSvnWorkspace))]
        [NotifyCanExecuteChangedFor(nameof(BuildWorkspaceCommand))]
        [NotifyCanExecuteChangedFor(nameof(RunWorkspaceTestsCommand))]
        [NotifyCanExecuteChangedFor(nameof(StartDebugSessionCommand))]
        [NotifyCanExecuteChangedFor(nameof(AddPackageCommand))]
        [NotifyCanExecuteChangedFor(nameof(RemovePackageCommand))]
        [NotifyCanExecuteChangedFor(nameof(UpdateSvnWorkspaceCommand))]
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
        [NotifyPropertyChangedFor(nameof(CanUpdateSvnWorkspace))]
        [NotifyCanExecuteChangedFor(nameof(UpdateSvnWorkspaceCommand))]
        private VersionControlStatus? _versionControlStatus;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(UpdateSvnWorkspaceCommand))]
        private bool _svnUpdateBusy;

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

        public bool CanUpdateSvnWorkspace => Workspace is not null
            && _svnWorkingCopyOperations is not null
            && VersionControlStatus?.Capabilities.Contains(VersionControlCapability.UpdateWorkingCopy) == true;

        public string UpdateSvnWorkspaceCommandText => "Update SVN";

        public Func<string, CancellationToken, Task<bool>>? ApproveSvnUpdateAsync { get; set; }

        public string WindowTitle => ActiveEditorTab.WindowTitle;

        public string NavigationTitle => UiStrings.NavigationTitle;

        public string NavigationPlaceholder => UiStrings.NavigationPlaceholder;

        public string PlanningExplorerTitle => UiStrings.PlanningExplorerTitle;

        public string EditorRegionTitle => UiStrings.EditorRegionTitle;

        public GridLength WorkspacePaneWidth
        {
            get => IsWorkspacePaneVisible ? _workspacePaneWidth : new GridLength(0);
            set
            {
                if (value.Value <= 0 || _workspacePaneWidth == value)
                {
                    return;
                }

                _workspacePaneWidth = value;
                if (IsWorkspacePaneVisible)
                {
                    OnPropertyChanged();
                }
            }
        }

        public GridLength WorkspacePaneSplitterWidth => IsWorkspacePaneVisible ? new GridLength(8) : new GridLength(0);

        public bool IsWorkspacePaneVisible
        {
            get => _isWorkspacePaneVisible;
            set
            {
                if (SetProperty(ref _isWorkspacePaneVisible, value))
                {
                    OnPropertyChanged(nameof(WorkspacePaneWidth));
                    OnPropertyChanged(nameof(WorkspacePaneSplitterWidth));
                }
            }
        }

        public string StatusRegionTitle => UiStrings.StatusRegionTitle;

        public string NotificationRegionTitle => UiStrings.NotificationRegionTitle;

        public string DocumentStatusLabel => UiStrings.DocumentStatusLabel;

        public string EditorStatusLabel => UiStrings.EditorStatusLabel;

        public string FileMenuHeader => UiStrings.FileMenuHeader;

        public string ViewMenuHeader => UiStrings.ViewMenuHeader;

        public string WorkspaceSidebarMenuText => UiStrings.WorkspaceSidebarMenuText;

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

        public ObservableCollection<AIChatMessageViewModel> AIChatMessages { get; } = [];

        public ObservableCollection<CSharpProjectTemplateDefinition> ProjectTemplates { get; } = [];

        public AxamlDesignerViewModel AxamlDesigner => ActiveEditorTab.Designer ?? _emptyDesigner;

        public bool HasAxamlPreview => ActiveEditorTab.Designer?.HasPreview == true && ActiveEditorTab.HasPalette;

        public bool CanEditActiveDocument => !ActiveEditorTab.IsDiagnostic;

        public PropertyEditorViewModel SelectionProperties { get; } = new();

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasDesignerPropertySelection))]
        private AxamlDesignerViewModel? _selectedPropertyDesigner;

        [ObservableProperty]
        private string _propertySelectionCaption = "Properties - no selection";

        public bool HasDesignerPropertySelection => SelectedPropertyDesigner is not null;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasEditorPropertyView))]
        private Control? _selectedEditorPropertyView;

        public bool HasEditorPropertyView => SelectedEditorPropertyView is not null;

        public bool HasStandardEditorPropertySelection =>
            _editorPropertySelection && SelectedEditorPropertyView is null && _selectedPropertyTab?.SelectedProperties is not null;

        public bool CanEditSelectedProperties =>
            HasStandardEditorPropertySelection && _selectedPropertyTab?.SelectedProperties?.IsReadOnly == false;

        [ObservableProperty]
        private string _propertySelectionError = string.Empty;

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

            await _editorWorkspace.OpenPreviewAsync(path, cancellationToken);
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

            IEditorDocument? document = await _editorWorkspace.OpenPreviewAsync(location.Path, cancellationToken);
            if (document is null)
            {
                Editor.NotificationText = $"The definition file '{location.Path}' is no longer available.";
                return;
            }

            EditorTabViewModel? targetTab = _editorWorkspace.FindByPath(location.Path);
            if (!ReferenceEquals(targetTab, _editorWorkspace.ActiveTab))
            {
                return;
            }
            if (targetTab?.TextView is not Controls.EditorTextArea targetEditor)
            {
                throw new InvalidOperationException($"The definition file '{location.Path}' has no caret-capable editor.");
            }

            targetEditor.FocusAndMoveCaret(location);
            targetTab.Editor.StatusText = $"Navigated to definition in {Path.GetFileName(location.Path)}.";
        }

        public string OpenWorkspaceCommandText => UiStrings.OpenWorkspaceCommandText;

        public string UndoWorkspaceChangeCommandText => "Undo workspace change";

        public string RedoWorkspaceChangeCommandText => "Redo workspace change";

        public string CreateConsoleProjectCommandText => UiStrings.CreateConsoleProjectCommandText;

        public string CreateClassLibraryProjectCommandText => UiStrings.CreateClassLibraryProjectCommandText;

        public string CreateTestProjectCommandText => UiStrings.CreateTestProjectCommandText;

        public string ProjectTemplatesMenuText => "Project Templates";

        public string GenerateAxamlMvvmCommandText => "Generate MVVM from AXAML";

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

        public string AIChatStatus => string.IsNullOrWhiteSpace(EffectiveAIChatModel)
            ? "Enter a model ID or configure a default model before sending."
            : AiChatBusy ? "Waiting for the AI provider..." : $"Tools available to the assistant: {AIChatToolCount}.";

        public string EffectiveAIChatModel => string.IsNullOrWhiteSpace(SessionAIModel)
            ? AiModel
            : SessionAIModel.Trim();

        public string AIChatModelHint => string.IsNullOrWhiteSpace(SessionAIModel)
            ? $"Configured model: {(string.IsNullOrWhiteSpace(AiModel) ? "(not configured)" : AiModel)}"
            : "Session override; clear for configured model. Chat history remains shared.";

        public int AIChatToolCount => GetAllowedAIChatTools().Count;

        public string AIChatApiKeyPlaceholderText =>
            "Session-only API key";

        public string CredentialProviderDisplay => SelectedCredentialProviderId ?? "None (session only)";

        public bool IsEncryptedVaultSelected =>
            string.Equals(SelectedCredentialProviderId, "encrypted-file-vault", StringComparison.Ordinal);

        public bool EncryptedVaultNeedsInitialization =>
            IsEncryptedVaultSelected && TryGetSelectedEncryptedVault(out EncryptedFileCredentialProvider? vault) && !vault.IsInitialized;

        public bool EncryptedVaultNeedsUnlock =>
            IsEncryptedVaultSelected && TryGetSelectedEncryptedVault(out EncryptedFileCredentialProvider? vault)
            && vault.IsInitialized && !vault.IsUnlocked;

        public bool EncryptedVaultUnlocked =>
            IsEncryptedVaultSelected && TryGetSelectedEncryptedVault(out EncryptedFileCredentialProvider? vault) && vault.IsUnlocked;

        public string EncryptedVaultPath =>
            IsEncryptedVaultSelected
                && TryGetSelectedEncryptedVault(out EncryptedFileCredentialProvider? vault)
                ? vault.VaultPath
                : string.Empty;

        public IReadOnlyList<string> AIChatAgentModes { get; } = ["Plan", "Build", "Debug"];

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(AIChatStatus))]
        private string _selectedAIChatAgent = "Plan";

        partial void OnSelectedAIChatAgentChanged(string value)
        {
            OnPropertyChanged(nameof(AIChatToolCount));
        }

        public string CredentialStorageStatus
        {
            get => _credentialStorageStatus;
            private set
            {
                Interlocked.Increment(ref _credentialStatusRevision);
                SetProperty(ref _credentialStorageStatus, value);
            }
        }

        private string _credentialStorageStatus = "Choose a credential provider. No automatic fallback is used.";

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(EncryptedVaultNeedsInitialization))]
        [NotifyPropertyChangedFor(nameof(EncryptedVaultNeedsUnlock))]
        [NotifyPropertyChangedFor(nameof(EncryptedVaultUnlocked))]
        [NotifyPropertyChangedFor(nameof(EncryptedVaultPath))]
        private string? _selectedCredentialProviderId;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(EncryptedVaultNeedsInitialization))]
        [NotifyPropertyChangedFor(nameof(EncryptedVaultNeedsUnlock))]
        [NotifyPropertyChangedFor(nameof(EncryptedVaultUnlocked))]
        [NotifyPropertyChangedFor(nameof(EncryptedVaultPath))]
        [NotifyCanExecuteChangedFor(nameof(SaveAICredentialCommand))]
        [NotifyCanExecuteChangedFor(nameof(LoadAICredentialCommand))]
        [NotifyCanExecuteChangedFor(nameof(DeleteAICredentialCommand))]
        [NotifyCanExecuteChangedFor(nameof(RefreshCredentialProviderStatusCommand))]
        private bool _credentialOperationBusy;

        public string AIChatInputPlaceholderText => "Ask a question or request a workspace action...";

        public string AIChatSendCommandText => AiChatBusy ? "Working..." : "Send";

        public Func<AIChatToolDescriptor, JsonElement, CancellationToken, Task<bool>>? ApproveAIChatToolExecutionAsync { get; set; }

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SendAIChatMessageCommand))]
        private string _aiChatInput = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(AIChatStatus))]
        [NotifyPropertyChangedFor(nameof(AIChatSendCommandText))]
        [NotifyCanExecuteChangedFor(nameof(SendAIChatMessageCommand))]
        [NotifyCanExecuteChangedFor(nameof(OpenConfigurationCommand))]
        [NotifyCanExecuteChangedFor(nameof(OpenAICredentialsCommand))]
        [NotifyCanExecuteChangedFor(nameof(ClearAIChatCommand))]
        private bool _aiChatBusy;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(AIChatStatus))]
        private string _aiEndpoint = "http://localhost:11434/v1/chat/completions";

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(AIChatStatus))]
        [NotifyPropertyChangedFor(nameof(AIChatModelHint))]
        [NotifyPropertyChangedFor(nameof(EffectiveAIChatModel))]
        private string _aiModel = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(AIChatStatus))]
        [NotifyPropertyChangedFor(nameof(AIChatModelHint))]
        [NotifyPropertyChangedFor(nameof(EffectiveAIChatModel))]
        private string _sessionAIModel = string.Empty;

        [ObservableProperty]
        private string _aiApiKey = string.Empty;

        private IReadOnlyList<string> GetAllowedAIChatTools()
        {
            if (_aiChatService is null)
            {
                return Array.Empty<string>();
            }

            string[] modeTools = SelectedAIChatAgent switch
            {
                "Build" => ["workspace.inspect", "workspace.versioning.status", "workspace.build", "workspace.test"],
                "Debug" => ["workspace.inspect", "workspace.versioning.status"],
                _ => ["workspace.inspect", "workspace.versioning.status"],
            };
            HashSet<string> registeredTools = new(
                _aiChatService.AvailableTools.Select(static tool => tool.Name),
                StringComparer.Ordinal);
            return modeTools.Where(registeredTools.Contains).ToArray();
        }

        private static string? GetCredentialProviderId(Configuration.CredentialManagerSelection? selection) =>
            selection switch
            {
                Configuration.CredentialManagerSelection.WindowsCredentialManager when OperatingSystem.IsWindows() =>
                    "windows-credential-manager",
                Configuration.CredentialManagerSelection.LinuxSecretService when OperatingSystem.IsLinux() =>
                    "linux-secret-service",
                Configuration.CredentialManagerSelection.EncryptedFileVault =>
                    "encrypted-file-vault",
                _ => null,
            };

        public async Task InitializeEncryptedVaultAsync(
            CredentialSecret passphrase,
            CredentialSecret confirmation,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(passphrase);
            ArgumentNullException.ThrowIfNull(confirmation);
            if (!passphrase.Reveal().SequenceEqual(confirmation.Reveal()))
            {
                CredentialStorageStatus = "The vault passphrases do not match.";
                return;
            }

            await RunEncryptedVaultOperationAsync(
                "The encrypted vault was initialized and is unlocked for this session.",
                (vault, token) => vault.InitializeAsync(passphrase, token),
                mayHaveTakenEffect: true,
                "Could not initialize the vault",
                cancellationToken);
        }

        public async Task UnlockEncryptedVaultAsync(
            CredentialSecret passphrase,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(passphrase);
            await RunEncryptedVaultOperationAsync(
                "The encrypted vault is unlocked for this session.",
                (vault, token) => vault.UnlockAsync(passphrase, token),
                mayHaveTakenEffect: false,
                "Could not unlock the vault",
                cancellationToken);
        }

        public async Task LockEncryptedVaultAsync(CancellationToken cancellationToken = default)
        {
            long version = _credentialContextVersion;
            bool locked = await RunEncryptedVaultOperationAsync(
                "The encrypted vault is locked.",
                static (vault, token) => vault.LockAsync(token).AsTask(),
                mayHaveTakenEffect: false,
                "Could not lock the vault",
                cancellationToken);
            if (locked)
            {
                if (version != _credentialContextVersion)
                {
                    CredentialStorageStatus = "The encrypted vault was locked for the previous credential context; the current session key was left unchanged.";
                    return;
                }

                AiApiKey = string.Empty;
                if (_aiSettings is not null)
                {
                    _aiSettings.ApiKey = string.Empty;
                }

                CredentialStorageStatus = "The encrypted vault is locked and the session API key was cleared.";
            }
        }

        partial void OnSelectedCredentialProviderIdChanged(string? value)
        {
            _credentialContextVersion++;
            OnPropertyChanged(nameof(CredentialProviderDisplay));
            OnPropertyChanged(nameof(IsEncryptedVaultSelected));
            OnPropertyChanged(nameof(EncryptedVaultNeedsInitialization));
            OnPropertyChanged(nameof(EncryptedVaultNeedsUnlock));
            OnPropertyChanged(nameof(EncryptedVaultUnlocked));
            OnPropertyChanged(nameof(EncryptedVaultPath));
            SaveAICredentialCommand.NotifyCanExecuteChanged();
            LoadAICredentialCommand.NotifyCanExecuteChanged();
            DeleteAICredentialCommand.NotifyCanExecuteChanged();
            RefreshCredentialProviderStatusCommand.NotifyCanExecuteChanged();
            _ = UpdateCredentialProviderStatusAsync(value);
        }

        partial void OnAiApiKeyChanged(string value)
        {
            _credentialContextVersion++;
            SaveAICredentialCommand.NotifyCanExecuteChanged();
            if (_aiSettings is not null)
            {
                _aiSettings.ApiKey = value;
            }
        }

        partial void OnAiEndpointChanged(string value)
        {
            if (AiChatBusy)
            {
                SendAIChatMessageCancelCommand.Execute(null);
            }
            _credentialContextVersion++;
            SessionAIModel = string.Empty;
            AiApiKey = string.Empty;
            if (_aiSettings is not null)
            {
                _aiSettings.ApiKey = string.Empty;
            }

            if (_aiConversation.Count > 0)
            {
                _aiConversation.Clear();
                AIChatMessages.Add(new AIChatMessageViewModel(
                    "System",
                    "The AI endpoint changed. Earlier messages remain visible but will not be sent to the new endpoint."));
            }
        }

        private bool CanManageAICredential() =>
            _credentialProviderCatalog is not null
            && !CredentialOperationBusy
            && !string.IsNullOrWhiteSpace(SelectedCredentialProviderId);

        private bool CanSaveAICredential() =>
            CanManageAICredential() && !string.IsNullOrEmpty(AiApiKey);

        [RelayCommand(CanExecute = nameof(CanSaveAICredential))]
        private async Task SaveAICredentialAsync(CancellationToken cancellationToken)
        {
            if (CredentialOperationBusy || !TryGetSelectedCredentialProvider(out ICredentialProvider? provider))
            {
                return;
            }

            Interlocked.Increment(ref _credentialStatusRevision);
            long version = _credentialContextVersion;
            bool operationStarted = false;
            CredentialOperationBusy = true;
            try
            {
                using CredentialSecret secret = new(AiApiKey.AsSpan());
                CredentialId id = GetAICredentialId(AiEndpoint);
                operationStarted = true;
                await provider.SetAsync(id, secret, cancellationToken);
                CredentialStorageStatus = version == _credentialContextVersion
                    ? $"API key saved in {provider.ProviderId}."
                    : $"API key saved in {provider.ProviderId} for the previous credential context; the current session key was left unchanged.";
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                CredentialStorageStatus = operationStarted
                    ? "Saving was cancelled; the key may already have been saved. Check the original credential context before retrying."
                    : "Saving was cancelled before contacting the credential manager.";
                throw;
            }
            catch (Exception exception)
            {
                string uncertainty = operationStarted
                    ? " The key may already have been saved; check the original credential context before retrying."
                    : string.Empty;
                string context = version == _credentialContextVersion ? "" : " for the previous credential context";
                string reason = operationStarted ? DescribeCredentialProviderFailure(exception) : DescribeCredentialPreparationFailure(exception);
                CredentialStorageStatus = $"Could not save API key{context}: {reason}{uncertainty}";
            }
            finally
            {
                CredentialOperationBusy = false;
            }
        }

        [RelayCommand(CanExecute = nameof(CanManageAICredential))]
        private async Task LoadAICredentialAsync(CancellationToken cancellationToken)
        {
            if (CredentialOperationBusy || !TryGetSelectedCredentialProvider(out ICredentialProvider? provider))
            {
                return;
            }

            Interlocked.Increment(ref _credentialStatusRevision);
            long version = _credentialContextVersion;
            bool operationStarted = false;
            CredentialOperationBusy = true;
            try
            {
                CredentialId id = GetAICredentialId(AiEndpoint);
                operationStarted = true;
                using CredentialSecret? secret = await provider.GetAsync(id, cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();
                if (version != _credentialContextVersion)
                {
                    CredentialStorageStatus = "The endpoint, credential manager, or session key changed while loading; the result was discarded.";
                    return;
                }

                if (secret is null)
                {
                    CredentialStorageStatus = $"No API key is stored in {provider.ProviderId} for this endpoint; the current session key was left unchanged.";
                    return;
                }

                AiApiKey = new string(secret.Reveal());
                CredentialStorageStatus = $"API key loaded from {provider.ProviderId} for this session.";
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                string context = version == _credentialContextVersion ? "" : " for the previous credential context";
                CredentialStorageStatus = $"Loading API key{context} was cancelled; the current session key was left unchanged.";
                throw;
            }
            catch (Exception exception)
            {
                string context = version == _credentialContextVersion ? "" : " for the previous credential context";
                string reason = operationStarted ? DescribeCredentialProviderFailure(exception) : DescribeCredentialPreparationFailure(exception);
                CredentialStorageStatus = $"Could not load API key{context}: {reason}";
            }
            finally
            {
                CredentialOperationBusy = false;
            }
        }

        [RelayCommand(CanExecute = nameof(CanManageAICredential))]
        private async Task DeleteAICredentialAsync(CancellationToken cancellationToken)
        {
            if (CredentialOperationBusy || !TryGetSelectedCredentialProvider(out ICredentialProvider? provider))
            {
                return;
            }

            Interlocked.Increment(ref _credentialStatusRevision);
            long version = _credentialContextVersion;
            bool operationStarted = false;
            CredentialOperationBusy = true;
            try
            {
                CredentialId id = GetAICredentialId(AiEndpoint);
                operationStarted = true;
                await provider.DeleteAsync(id, cancellationToken);
                if (version != _credentialContextVersion)
                {
                    CredentialStorageStatus = "The stored key was deleted for the previous credential context; the current session key was left unchanged.";
                    return;
                }
                AiApiKey = string.Empty;
                CredentialStorageStatus = $"Stored API key deleted from {provider.ProviderId}; the session key was cleared.";
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                CredentialStorageStatus = operationStarted
                    ? "Deleting was cancelled; the stored key may already have been deleted. Check the original credential context before retrying."
                    : "Deleting was cancelled before contacting the credential manager.";
                throw;
            }
            catch (Exception exception)
            {
                string uncertainty = operationStarted
                    ? " The stored key may already have been deleted; check the original credential context before retrying."
                    : string.Empty;
                string context = version == _credentialContextVersion ? "" : " for the previous credential context";
                string reason = operationStarted ? DescribeCredentialProviderFailure(exception) : DescribeCredentialPreparationFailure(exception);
                CredentialStorageStatus = $"Could not delete API key{context}: {reason}{uncertainty}";
            }
            finally
            {
                CredentialOperationBusy = false;
            }
        }

        [RelayCommand(CanExecute = nameof(CanManageAICredential))]
        private async Task RefreshCredentialProviderStatusAsync(CancellationToken cancellationToken)
        {
            if (CredentialOperationBusy || !TryGetSelectedCredentialProvider(out ICredentialProvider? provider))
            {
                return;
            }

            await UpdateCredentialProviderStatusAsync(provider.ProviderId, cancellationToken);
        }

        private async Task UpdateCredentialProviderStatusAsync(
            string? providerId,
            CancellationToken cancellationToken = default)
        {
            long revision = Interlocked.Increment(ref _credentialStatusRevision);
            if (string.IsNullOrWhiteSpace(providerId) || _credentialProviderCatalog is null)
            {
                CredentialStorageStatus = "Choose a credential provider. No automatic fallback is used.";
                return;
            }

            try
            {
                CredentialProviderStatus status = await _credentialProviderCatalog
                    .GetRequired(providerId)
                    .GetStatusAsync(cancellationToken);
                if (revision != Interlocked.Read(ref _credentialStatusRevision)
                    || !string.Equals(SelectedCredentialProviderId, providerId, StringComparison.Ordinal))
                {
                    return;
                }

                CredentialStorageStatus = status.Availability switch
                {
                    CredentialProviderAvailability.Available => $"{providerId} is available.",
                    CredentialProviderAvailability.Locked => $"{providerId} is locked. Unlock it and retry.",
                    _ => $"{providerId} is unavailable. Select or configure a provider explicitly; no fallback is used.",
                };
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception exception)
            {
                if (revision == Interlocked.Read(ref _credentialStatusRevision)
                    && string.Equals(SelectedCredentialProviderId, providerId, StringComparison.Ordinal))
                {
                    CredentialStorageStatus = $"Could not check {providerId}: {DescribeCredentialProviderFailure(exception)}";
                }
            }
        }

        private bool TryGetSelectedCredentialProvider([NotNullWhen(true)] out ICredentialProvider? provider)
        {
            provider = null;
            if (_credentialProviderCatalog is null || string.IsNullOrWhiteSpace(SelectedCredentialProviderId))
            {
                CredentialStorageStatus = "Choose a credential provider. No automatic fallback is used.";
                return false;
            }

            try
            {
                provider = _credentialProviderCatalog.GetRequired(SelectedCredentialProviderId);
                return true;
            }
            catch (Exception)
            {
                CredentialStorageStatus = "Credential provider selection failed. Select a registered credential manager.";
                return false;
            }
        }

        private bool TryGetSelectedEncryptedVault([NotNullWhen(true)] out EncryptedFileCredentialProvider? vault)
        {
            vault = null;
            if (!IsEncryptedVaultSelected || _credentialProviderCatalog is null)
            {
                return false;
            }

            try
            {
                vault = _credentialProviderCatalog.GetRequired(SelectedCredentialProviderId!) as EncryptedFileCredentialProvider;
                if (vault is not null)
                {
                    return true;
                }
            }
            catch (KeyNotFoundException)
            {
                CredentialStorageStatus = "The encrypted vault provider is not registered in this host.";
                return false;
            }

            CredentialStorageStatus = "The selected provider is not an encrypted vault.";
            return false;
        }

        private async Task<bool> RunEncryptedVaultOperationAsync(
            string successMessage,
            Func<EncryptedFileCredentialProvider, CancellationToken, Task> operation,
            bool mayHaveTakenEffect,
            string failureMessage,
            CancellationToken cancellationToken)
        {
            if (CredentialOperationBusy || !TryGetSelectedEncryptedVault(out EncryptedFileCredentialProvider? vault))
            {
                return false;
            }

            Interlocked.Increment(ref _credentialStatusRevision);
            long version = _credentialContextVersion;
            bool operationStarted = false;
            CredentialOperationBusy = true;
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                operationStarted = true;
                await operation(vault, cancellationToken);
                CredentialStorageStatus = version == _credentialContextVersion
                    ? successMessage
                    : $"{successMessage} This action concerned the previous credential context.";
                return true;
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                string context = version == _credentialContextVersion ? "" : " for the previous credential context";
                CredentialStorageStatus = mayHaveTakenEffect
                    ? $"{failureMessage}{context}: cancelled; the vault may already have been created. Check its status before retrying."
                    : $"{failureMessage}{context}: cancelled.";
                throw;
            }
            catch (Exception exception)
            {
                string uncertainty = operationStarted && mayHaveTakenEffect
                    ? " The vault may already have been created; check its status before retrying."
                    : string.Empty;
                string context = version == _credentialContextVersion ? "" : " for the previous credential context";
                CredentialStorageStatus = $"{failureMessage}{context}: {DescribeCredentialProviderFailure(exception)}{uncertainty}";
                return false;
            }
            finally
            {
                CredentialOperationBusy = false;
                OnPropertyChanged(nameof(EncryptedVaultNeedsInitialization));
                OnPropertyChanged(nameof(EncryptedVaultNeedsUnlock));
                OnPropertyChanged(nameof(EncryptedVaultUnlocked));
                OnPropertyChanged(nameof(EncryptedVaultPath));
            }
        }

        private static string DescribeCredentialPreparationFailure(Exception exception) =>
            exception is InvalidOperationException
                ? "A valid absolute HTTP(S) endpoint is required to manage its API key."
                : "Credential preparation failed.";

        private static string DescribeCredentialProviderFailure(Exception exception) =>
            exception switch
            {
                CredentialProviderLockedException => "The credential provider is locked. Unlock it and retry.",
                CredentialVaultUnlockException => "The vault could not be unlocked; the passphrase may be wrong or the file may be damaged.",
                UnauthorizedAccessException => "Access to the credential manager was denied.",
                PlatformNotSupportedException => "The credential manager is not supported on this platform.",
                InvalidDataException => "The credential manager returned invalid or ambiguous data. Inspect the store before retrying.",
                IOException => "The credential manager encountered an I/O or connection error.",
                ArgumentException => "The credential manager rejected the credential or its identifier.",
                _ => "The credential manager reported an error. Check its availability and configuration.",
            };

        private static CredentialId GetAICredentialId(string endpoint)
        {
            if (!Uri.TryCreate(endpoint, UriKind.Absolute, out Uri? uri)
                || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            {
                throw new InvalidOperationException("A valid absolute HTTP(S) endpoint is required to manage its API key.");
            }

            string normalizedEndpoint = uri.GetComponents(
                UriComponents.SchemeAndServer | UriComponents.PathAndQuery,
                UriFormat.UriEscaped);
            byte[] endpointBytes = Encoding.UTF8.GetBytes(normalizedEndpoint);
            try
            {
                byte[] scopeHash = SHA256.HashData(endpointBytes);
                try
                {
                    return new CredentialId(
                        "AA98-CodeStudio",
                        "AI-API-Key",
                        Convert.ToHexString(scopeHash));
                }
                finally
                {
                    CryptographicOperations.ZeroMemory(scopeHash);
                }
            }
            finally
            {
                CryptographicOperations.ZeroMemory(endpointBytes);
            }
        }

        private bool CanSendAIChatMessage()
        {
            return !AiChatBusy && !string.IsNullOrWhiteSpace(AiChatInput);
        }

        [RelayCommand(CanExecute = nameof(CanSendAIChatMessage), IncludeCancelCommand = true)]
        private async Task SendAIChatMessageAsync(CancellationToken cancellationToken)
        {
            if (_aiChatService is null)
            {
                AIChatMessages.Add(new AIChatMessageViewModel("System", "AI chat is not available in this host."));
                return;
            }

            string model = EffectiveAIChatModel;
            if (string.IsNullOrWhiteSpace(model))
            {
                AIChatMessages.Add(new AIChatMessageViewModel("System", "Enter a model ID before sending a message."));
                return;
            }

            string userText = AiChatInput.Trim();
            AiChatInput = string.Empty;
            AIMessage pendingMessage = new(AIMessageRole.User, userText);
            _aiConversation.Add(pendingMessage);
            AIChatMessages.Add(new AIChatMessageViewModel("You", userText));
            AiChatBusy = true;
            bool committed = false;
            IReadOnlyList<string> allowedTools = GetAllowedAIChatTools();
            string agent = SelectedAIChatAgent;
            OnPropertyChanged(nameof(AIChatStatus));
            try
            {
                List<AIMessage> requestMessages =
                [
                    new AIMessage(AIMessageRole.System, await CreateAIChatContextAsync(agent, cancellationToken)),
                ];
                cancellationToken.ThrowIfCancellationRequested();
                requestMessages.AddRange(_aiConversation);
                AIChatTurnResult result = await _aiChatService.SendWithAllowedToolsAsync(
                    requestMessages,
                    options: new AICompletionOptions { ModelId = model },
                    approveToolExecution: ApproveAIChatToolExecutionAsync,
                    cancellationToken: cancellationToken,
                    allowedToolNames: allowedTools);
                cancellationToken.ThrowIfCancellationRequested();
                _aiConversation.Clear();
                _aiConversation.AddRange(result.Messages.Where(static message => message.Role != AIMessageRole.System));
                committed = true;
                foreach (AIChatToolExecution execution in result.ToolExecutions)
                {
                    string state = execution.WasExecuted ? "completed" : "not approved";
                    AIChatMessages.Add(new AIChatMessageViewModel(
                        $"Tool: {execution.Call.Name} ({state})",
                        execution.Result));
                }

                if (!string.IsNullOrWhiteSpace(result.AssistantMessage.Content))
                {
                    AIChatMessages.Add(new AIChatMessageViewModel("Assistant", result.AssistantMessage.Content));
                }

                bool buildDiagnosticsCleared = false;
                bool testDiagnosticsCleared = false;
                foreach (AIChatToolExecution execution in result.ToolExecutions)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    if (execution.WasExecuted
                        && execution.Call.Name is "workspace.build" or "workspace.inspect" or "workspace.test")
                    {
                        if (execution.Call.Name == "workspace.test")
                        {
                            if (!testDiagnosticsCleared)
                            {
                                _testDiagnostics.Clear();
                                testDiagnosticsCleared = true;
                            }
                        }
                        else if (!buildDiagnosticsCleared)
                        {
                            _buildDiagnostics.Clear();
                            buildDiagnosticsCleared = true;
                        }

                        await AddWorkspaceToolDiagnosticsToWorkbenchAsync(execution, cancellationToken);
                    }
                }

            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                if (!committed)
                {
                    _aiConversation.Remove(pendingMessage);
                }
                AIChatMessages.Add(new AIChatMessageViewModel(
                    "System",
                    committed
                        ? "The chat response was received, but diagnostic processing was interrupted. Any tool already started may have made changes."
                        : "The chat request was cancelled. Any tool already started may have made changes. The incomplete turn will not be sent again."));
            }
            catch (Exception exception)
            {
                if (!committed)
                {
                    _aiConversation.Remove(pendingMessage);
                }
                AIChatMessages.Add(new AIChatMessageViewModel("Error", exception.Message));
                if (!committed)
                {
                    AIChatMessages.Add(new AIChatMessageViewModel(
                        "System",
                        "The incomplete turn will not be sent again. Any tool already started may have made changes."));
                }
            }
            finally
            {
                AiChatBusy = false;
                OnPropertyChanged(nameof(AIChatStatus));
            }
        }

        private async Task AddWorkspaceToolDiagnosticsToWorkbenchAsync(
            AIChatToolExecution execution,
            CancellationToken cancellationToken)
        {
            CSharpWorkspace? workspace = Workspace;
            if (workspace is null)
            {
                return;
            }

            if (!execution.Call.Arguments.TryGetProperty("workspacePath", out JsonElement workspacePathElement)
                || workspacePathElement.ValueKind != JsonValueKind.String
                || !PathsEqual(workspacePathElement.GetString()!, workspace.EntryPath))
            {
                AIChatMessages.Add(new AIChatMessageViewModel(
                    "System",
                    "Workspace diagnostics were not attached because the tool used a different workspace than the one currently open."));
                return;
            }

            using JsonDocument resultDocument = JsonDocument.Parse(execution.Result);
            JsonElement root = resultDocument.RootElement;
            IEnumerable<JsonElement> projects = root.TryGetProperty("Projects", out JsonElement projectResults)
                ? projectResults.EnumerateArray().ToArray()
                : [root];
            List<Diagnostic> diagnostics = [];
            int rejectedLocations = 0;
            bool isTestRun = execution.Call.Name == "workspace.test";
            string sourceDescription = execution.Call.Name switch
            {
                "workspace.build" => "Build",
                "workspace.inspect" => "Inspection",
                "workspace.test" => "Test",
                _ => throw new InvalidOperationException(
                    $"Workspace tool '{execution.Call.Name}' does not produce diagnostics."),
            };
            foreach (JsonElement projectResult in projects)
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (!projectResult.TryGetProperty("ProjectPath", out JsonElement projectPathElement)
                    || projectPathElement.ValueKind != JsonValueKind.String)
                {
                    throw new JsonException("Build result is missing a project path.");
                }

                string projectPath = Path.GetFullPath(projectPathElement.GetString()!);
                bool belongsToWorkspace = workspace.Projects.Any(project =>
                    PathsEqual(Path.GetFullPath(project.ProjectPath), projectPath));
                if (!belongsToWorkspace)
                {
                    throw new JsonException(
                        $"{sourceDescription} result project '{projectPath}' is outside the current workspace.");
                }

                string resultsPropertyName = isTestRun ? "Failures" : "Diagnostics";
                if (!projectResult.TryGetProperty(resultsPropertyName, out JsonElement projectDiagnostics)
                    || projectDiagnostics.ValueKind != JsonValueKind.Array)
                {
                    throw new JsonException(
                        $"{sourceDescription} result for '{projectPath}' has no {resultsPropertyName} array.");
                }

                foreach (JsonElement item in projectDiagnostics.EnumerateArray())
                {
                    if (isTestRun)
                    {
                        string? sourcePath = GetOptionalString(item, "SourcePath");
                        string? navigableSourcePath = TryGetWorkspaceTestSourcePath(sourcePath, workspace.RootPath);
                        int? lineNumber = GetOptionalInt32(item, "LineNumber");
                        if (!string.IsNullOrWhiteSpace(sourcePath) && navigableSourcePath is null)
                        {
                            rejectedLocations++;
                        }

                        diagnostics.Add(new Diagnostic
                        {
                            Code = "TEST",
                            Severity = DiagnosticSeverity.Error,
                            Message = FormatTestFailureMessage(
                                GetOptionalString(item, "TestName"),
                                GetOptionalString(item, "Message")),
                            SourcePath = navigableSourcePath,
                            LineNumber = navigableSourcePath is not null && lineNumber is > 0 ? lineNumber : null,
                        });
                        continue;
                    }

                    string? filePath = GetOptionalString(item, "FilePath");
                    if (!string.IsNullOrWhiteSpace(filePath)
                        && !IsPathWithinDirectory(Path.GetFullPath(filePath), workspace.RootPath))
                    {
                        rejectedLocations++;
                        continue;
                    }

                    string code = GetOptionalString(item, "Code") ?? string.Empty;
                    string message = GetOptionalString(item, "Message") ?? string.Empty;
                    string severity = GetOptionalString(item, "Severity") ?? string.Empty;
                    diagnostics.Add(MapDiagnostic(new BuilderDiagnostic
                    {
                        Code = code,
                        Message = message,
                        Severity = severity switch
                        {
                            "Info" => BuilderDiagnosticSeverity.Info,
                            "Warning" => BuilderDiagnosticSeverity.Warning,
                            "Error" => BuilderDiagnosticSeverity.Error,
                            _ => BuilderDiagnosticSeverity.Unknown,
                        },
                        FilePath = filePath,
                        LineNumber = GetOptionalInt32(item, "LineNumber"),
                        ColumnNumber = GetOptionalInt32(item, "ColumnNumber"),
                    }));
                }
            }

            if (isTestRun)
            {
                _testDiagnostics.AddRange(diagnostics);
            }
            else
            {
                _buildDiagnostics.AddRange(diagnostics);
            }

            await RefreshDiagnosticsAsync(cancellationToken);
            if (rejectedLocations > 0)
            {
                string warning = isTestRun
                    ? $"Removed {rejectedLocations} test failure source location(s) outside the current workspace."
                    : $"Ignored {rejectedLocations} {sourceDescription.ToLowerInvariant()} diagnostic location(s) outside the current workspace.";
                AIChatMessages.Add(new AIChatMessageViewModel("System", warning));
            }
        }

        private static string FormatTestFailureMessage(string? testName, string? message)
        {
            string name = string.IsNullOrWhiteSpace(testName) ? "Test failure" : testName;
            return string.IsNullOrWhiteSpace(message) ? $"{name} failed." : $"{name}: {message}";
        }

        private static string? GetOptionalString(JsonElement element, string propertyName)
        {
            if (!element.TryGetProperty(propertyName, out JsonElement value)
                || value.ValueKind == JsonValueKind.Null)
            {
                return null;
            }

            return value.GetString();
        }

        private static int? GetOptionalInt32(JsonElement element, string propertyName)
        {
            return element.TryGetProperty(propertyName, out JsonElement value)
                && value.ValueKind == JsonValueKind.Number
                && value.TryGetInt32(out int parsed)
                    ? parsed
                    : null;
        }

        private static bool IsPathWithinDirectory(string filePath, string directoryPath)
        {
            string relativePath = Path.GetRelativePath(Path.GetFullPath(directoryPath), filePath);
            return !Path.IsPathRooted(relativePath)
                && relativePath != ".."
                && !relativePath.StartsWith($"..{Path.DirectorySeparatorChar}", StringComparison.Ordinal);
        }

        private async Task<string> CreateAIChatContextAsync(string agent, CancellationToken cancellationToken)
        {
            CSharpWorkspace? workspace = Workspace;
            if (workspace is null)
            {
                return "You are the AI assistant inside AvlnCodeStudio. No C# workspace is currently open. Ask the user to open a solution or project before using workspace tools.";
            }

            string projects = string.Join(
                Environment.NewLine,
                workspace.Projects.Select(static project => $"- {project.Name}: {project.ProjectPath}"));
            VersionControlStatus status = await _versionControlService.GetStatusAsync(
                new VersionControlStatusRequest
                {
                    RepositoryContextPath = workspace.RootPath,
                    IncludeChanges = true,
                    IncludeCapabilities = true,
                },
                cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            if (Workspace is not null && PathsEqual(Workspace.RootPath, workspace.RootPath))
            {
                VersionControlStatus = status;
            }

            string versionControlContext = !status.IsRepositoryRootDiscovered
                ? "No Git or SVN repository was detected."
                : $"Repository: {status.RepositoryName ?? status.RepositoryRootPath}; reference: {status.ActiveReferenceName ?? UiStrings.UnknownReferenceText}; local changes: {status.Changes.Count} ({(status.HasLocalChanges ? "present" : "none")}).";
            string agentInstructions = agent switch
            {
                "Build" => "You are in Build agent mode. Inspect the workspace and help build or test it. Build and test tools require explicit user approval. Do not update source control.",
                "Debug" => "You are in Debug agent mode. You can inspect workspace metadata, but cannot control the debugger from chat. Give debugging guidance and direct the user to the IDE Debug controls for execution.",
                _ => "You are in Plan agent mode. Help analyze and plan changes. Only read-only workspace inspection and version-control status tools are available.",
            };
            return $"You are the AI assistant inside AvlnCodeStudio. {agentInstructions} The current C# workspace is '{workspace.EntryPath}'. Its projects are:{Environment.NewLine}{projects}{Environment.NewLine}Version-control status (refreshed for this turn): {versionControlContext}{Environment.NewLine}Use workspace.inspect for more workspace metadata. Only build projects listed in this workspace; workspace.build requires explicit user approval.";
        }

        private bool CanClearAIChat() => !AiChatBusy;

        [RelayCommand(CanExecute = nameof(CanClearAIChat))]
        private void ClearAIChat()
        {
            if (AiChatBusy)
            {
                return;
            }
            _aiConversation.Clear();
            AIChatMessages.Clear();
        }

        private async Task OnConfigurationSectionAppliedAsync(string sectionName)
        {
            if (!string.Equals(sectionName, "Workbench", StringComparison.Ordinal)
                || (_settingsRuntimeActivator is null && _configService is null)
                || _codeStudioSettings is null)
            {
                return;
            }

            if (_settingsRuntimeActivator is not null)
            {
                await _settingsRuntimeActivator.ActivateAsync(sectionName);
            }
            else
            {
                Configuration.CodeStudioSettings applied = await _configService!.LoadAsync<Configuration.CodeStudioSettings>("Workbench")
                    ?? throw new InvalidOperationException("The saved Workbench settings could not be reloaded.");
                _codeStudioSettings.AIEndpoint = applied.AIEndpoint;
                _codeStudioSettings.AIModelId = applied.AIModelId;
                _codeStudioSettings.AIProvider = applied.AIProvider;
                _codeStudioSettings.UseAIProviderProfiles = applied.UseAIProviderProfiles;
                _codeStudioSettings.AIProfileOpenAIEndpoint = applied.AIProfileOpenAIEndpoint;
                _codeStudioSettings.AIProfileOpenAIModelId = applied.AIProfileOpenAIModelId;
                _codeStudioSettings.AIProfileOllamaEndpoint = applied.AIProfileOllamaEndpoint;
                _codeStudioSettings.AIProfileOllamaModelId = applied.AIProfileOllamaModelId;
                _codeStudioSettings.AIProfileIFMEndpoint = applied.AIProfileIFMEndpoint;
                _codeStudioSettings.AIProfileIFMModelId = applied.AIProfileIFMModelId;
                _codeStudioSettings.AIProfileCustomEndpoint = applied.AIProfileCustomEndpoint;
                _codeStudioSettings.AIProfileCustomModelId = applied.AIProfileCustomModelId;
                _codeStudioSettings.CredentialManager = applied.CredentialManager;
                _codeStudioSettings.KnownAIProviderTemplates = applied.KnownAIProviderTemplates;
                _codeStudioSettings.WorkspacePath = applied.WorkspacePath;
                _codeStudioSettings.ReopenLastDocument = applied.ReopenLastDocument;

                if (_aiSettings is not null)
                {
                    string endpoint = applied.GetEffectiveAIEndpoint();
                    if (!string.Equals(_aiSettings.Endpoint, endpoint, StringComparison.Ordinal))
                    {
                        _aiSettings.ApiKey = string.Empty;
                    }
                    _aiSettings.Endpoint = endpoint;
                    _aiSettings.ModelId = applied.GetEffectiveAIModelId();
                }
            }

            AiEndpoint = _aiSettings?.Endpoint ?? _codeStudioSettings.GetEffectiveAIEndpoint();
            AiModel = _aiSettings?.ModelId ?? _codeStudioSettings.GetEffectiveAIModelId();
            SelectedCredentialProviderId = GetCredentialProviderId(_codeStudioSettings.CredentialManager);
        }

        private bool CanOpenAIConfiguration() => !AiChatBusy;

        public Configuration.AIProviderDraftViewModel? CreateAIProviderDraft()
        {
            if (ConfigUi is null || _codeStudioSettings is null)
            {
                return null;
            }

            return new Configuration.AIProviderDraftViewModel(ConfigUi, _codeStudioSettings);
        }

        public Task<string?> ConfirmAIProviderTemplatesAsync(
            Configuration.AIProviderDraftViewModel draft,
            IReadOnlyList<global::Config.UI.IConfigUiSection> sections) =>
            _settingsRuntimeActivator is null
                ? Task.FromResult<string?>(null)
                : draft.ConfirmCurrentTemplatesAsync(sections, _settingsRuntimeActivator.LoadProviderTemplatesAsync);

        [RelayCommand(CanExecute = nameof(CanOpenAIConfiguration))]
        private async Task OpenConfigurationAsync()
        {
            if (OpenConfigurationDialogAsync is null)
            {
                Editor.StatusText = "The settings dialog is not available in this host.";
                return;
            }

            await OpenConfigurationDialogAsync();
        }

        [RelayCommand]
        private async Task OpenSystemConfigurationAsync()
        {
            await OpenConfigurationLayerAsync(ConfigLayer.System);
        }

        [RelayCommand]
        private async Task OpenGroupConfigurationAsync()
        {
            await OpenConfigurationLayerAsync(ConfigLayer.Group);
        }

        private async Task OpenConfigurationLayerAsync(ConfigLayer layer)
        {
            if (OpenConfigurationLayerDialogAsync is null)
            {
                Editor.StatusText = "Layer-specific settings are not available in this host.";
                return;
            }

            await OpenConfigurationLayerDialogAsync(layer);
        }

        [RelayCommand(CanExecute = nameof(CanOpenAIConfiguration))]
        private async Task OpenAICredentialsAsync()
        {
            if (OpenAICredentialsDialogAsync is null)
            {
                Editor.StatusText = "The AI credential manager is not available in this host.";
                return;
            }

            await OpenAICredentialsDialogAsync();
        }

        [RelayCommand]
        private async Task OpenProjectConfigurationAsync()
        {
            if (OpenProjectConfigurationDialogAsync is null)
            {
                Editor.StatusText = "Project settings are not available in this host.";
                return;
            }

            await OpenProjectConfigurationDialogAsync();
        }

        public string NavigationPlaceholderMenuText => UiStrings.NavigationPlaceholderMenuText;

        public string StatusPlaceholderMenuText => UiStrings.StatusPlaceholderMenuText;

        [RelayCommand(CanExecute = nameof(CanUndoWorkspaceChange))]
        private void UndoWorkspaceChange()
        {
            try
            {
                if (_workspaceFileChangeService.Undo())
                {
                    Editor.StatusText = "Undid the last confirmed workspace change.";
                    Editor.NotificationText = _workspaceFileChangeService.HistoryWarning
                        ?? "All affected files and open documents were restored.";
                }
            }
            catch (Exception exception) when (exception is IOException
                or UnauthorizedAccessException
                or InvalidOperationException
                or ArgumentException
                or AggregateException)
            {
                Editor.StatusText = "Workspace undo failed.";
                Editor.NotificationText = exception.Message;
            }
        }

        [RelayCommand(CanExecute = nameof(CanRedoWorkspaceChange))]
        private void RedoWorkspaceChange()
        {
            try
            {
                if (_workspaceFileChangeService.Redo())
                {
                    Editor.StatusText = "Redid the last undone workspace change.";
                    Editor.NotificationText = _workspaceFileChangeService.HistoryWarning
                        ?? "All affected files and open documents were reapplied.";
                }
            }
            catch (Exception exception) when (exception is IOException
                or UnauthorizedAccessException
                or InvalidOperationException
                or ArgumentException
                or AggregateException)
            {
                Editor.StatusText = "Workspace redo failed.";
                Editor.NotificationText = exception.Message;
            }
        }

        private bool CanUndoWorkspaceChange() => _workspaceFileChangeService.CanUndo;

        private bool CanRedoWorkspaceChange() => _workspaceFileChangeService.CanRedo;

        private void OnWorkspaceChangeHistoryChanged(object? sender, EventArgs e)
        {
            UndoWorkspaceChangeCommand.NotifyCanExecuteChanged();
            RedoWorkspaceChangeCommand.NotifyCanExecuteChanged();
        }

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

        [RelayCommand(CanExecute = nameof(CanGenerateAxamlMvvm))]
        private async Task GenerateAxamlMvvmAsync(CancellationToken cancellationToken)
        {
            string filePath = Editor.CurrentFilePath
                ?? throw new InvalidOperationException("Open an AXAML file before generating MVVM code.");
            CSharpWorkspace workspace = Workspace
                ?? throw new InvalidOperationException("Open the containing C# workspace before generating MVVM code.");
            CSharpWorkspaceProject project = workspace.Projects
                .Where(candidate => IsPathWithinDirectory(
                    filePath,
                    Path.GetDirectoryName(candidate.ProjectPath)!))
                .OrderByDescending(candidate => candidate.ProjectPath.Length)
                .FirstOrDefault()
                ?? throw new InvalidOperationException("The active AXAML file is not inside a project in this workspace.");

            try
            {
                if (!ProjectReferencesMvvmToolkit(project.ProjectPath))
                {
                    Editor.StatusText = "MVVM generation requires CommunityToolkit.Mvvm.";
                    Editor.NotificationText = "Add CommunityToolkit.Mvvm to the active project, then retry.";
                    return;
                }

                string relativeAxamlPath = Path.GetRelativePath(workspace.RootPath, filePath);
                AxamlMvvmSemanticResolution existingResolution =
                    await _csharpSyntaxDiagnosticService.AnalyzeAxamlMvvmAsync(
                        Editor.Text,
                        filePath,
                        workspace,
                        cancellationToken,
                        allowExistingRuntimeDataContext: true);
                if (!existingResolution.IsSuccessful)
                {
                    throw new InvalidDataException(
                        "MVVM generation stopped because the view or its DataContext could not be resolved safely:"
                        + Environment.NewLine
                        + string.Join(Environment.NewLine, existingResolution.Diagnostics));
                }

                if (existingResolution.HasRuntimeDataContext
                    && !string.IsNullOrWhiteSpace(existingResolution.ViewModelTypeName))
                {
                    IReadOnlyList<AxamlMvvmMemberRequirement> requirements =
                        new AxamlMvvmBindingAnalyzer().Analyze(Editor.Text);
                    IReadOnlyList<AxamlMvvmMemberPlanEntry> memberPlan = new AxamlMvvmMemberPlanner().Plan(
                        requirements,
                        existingResolution.ViewModelMembers,
                        createMissingMembers: true);
                    AxamlMvvmMemberPlanEntry[] conflicts = memberPlan
                        .Where(static entry => entry.Action is AxamlMvvmMemberPlanAction.Conflict
                            or AxamlMvvmMemberPlanAction.NeedsCreationChoice)
                        .ToArray();
                    if (conflicts.Length > 0)
                    {
                        throw new InvalidDataException(
                            "Existing ViewModel bindings are not compatible:"
                            + Environment.NewLine
                            + string.Join(
                                Environment.NewLine,
                                conflicts.Select(static entry => entry.Diagnostic)));
                    }

                    AxamlMvvmMemberPlanEntry[] additions = memberPlan
                        .Where(static entry => entry.Action == AxamlMvvmMemberPlanAction.Create)
                        .ToArray();
                    if (additions.Length == 0)
                    {
                        Editor.StatusText = "Existing ViewModel bindings are already compatible; no changes were needed.";
                        Editor.NotificationText = "The AXAML and ViewModel were left unchanged.";
                        return;
                    }

                    WorkspaceFileChangeRequest sourceRequest =
                        await _csharpSyntaxDiagnosticService.PlanViewModelSourceAdditionAsync(
                            workspace.RootPath,
                            project.ProjectPath,
                            existingResolution.ViewModelTypeName!,
                            additions,
                            cancellationToken);
                    IReadOnlyList<string> newCompilerErrors =
                        await _csharpSyntaxDiagnosticService.GetNewCompilerErrorsAsync(
                            project.ProjectPath,
                            sourceRequest,
                            cancellationToken,
                            workspace.RootPath);
                    if (newCompilerErrors.Count > 0)
                    {
                        throw new InvalidDataException(
                            "MVVM generation would introduce C# compiler errors:"
                            + Environment.NewLine
                            + string.Join(Environment.NewLine, newCompilerErrors));
                    }

                    WorkspaceFileChangeProposal incrementalProposal = _workspaceFileChangeService.Prepare(
                        workspace.RootPath,
                        [sourceRequest]);
                    Func<WorkspaceFileChangeProposal, CancellationToken, Task<bool>>? incrementalReview =
                        ReviewWorkspaceFileChangesAsync;
                    if (incrementalReview is null)
                    {
                        Editor.StatusText = "MVVM generation is unavailable in this host.";
                        Editor.NotificationText = "This host does not provide the workspace change review dialog.";
                        return;
                    }

                    if (!await incrementalReview(incrementalProposal, cancellationToken))
                    {
                        Editor.StatusText = "MVVM generation was cancelled; no files were changed.";
                        return;
                    }

                    await _csharpSyntaxDiagnosticService.RefreshProjectContextAsync(
                        project.ProjectPath,
                        cancellationToken);
                    QueueAxamlMvvmBindingContext(Editor.Text, filePath);
                    Editor.StatusText = $"Extended ViewModel '{existingResolution.ViewModelTypeName}'.";
                    Editor.NotificationText = string.Join(
                        Environment.NewLine,
                        incrementalProposal.Changes.Select(static change => change.RelativePath));
                    return;
                }

                bool usePartialProperties = await _csharpSyntaxDiagnosticService
                    .SupportsObservablePartialPropertiesAsync(project.ProjectPath, cancellationToken);
                IReadOnlyList<WorkspaceFileChangeRequest> requests =
                    _axamlMvvmGenerator.Generate(relativeAxamlPath, Editor.Text, usePartialProperties);
                string viewModelPath = Path.GetFullPath(
                    Path.Combine(workspace.RootPath, requests[1].RelativePath));
                if (File.Exists(viewModelPath))
                {
                    throw new InvalidDataException(
                        $"'{requests[1].RelativePath}' already exists. Incremental MVVM generation is not supported yet; the existing ViewModel was left unchanged.");
                }

                IReadOnlyList<string> generatedCompilerErrors =
                    await _csharpSyntaxDiagnosticService.GetNewSourceCompilerErrorsAsync(
                        workspace.RootPath, project.ProjectPath, requests[1], cancellationToken);
                if (generatedCompilerErrors.Count > 0)
                {
                    throw new InvalidDataException(
                        "The generated ViewModel would introduce C# compiler errors:"
                        + Environment.NewLine
                        + string.Join(Environment.NewLine, generatedCompilerErrors));
                }

                AxamlMvvmSemanticResolution semanticResolution =
                    await _csharpSyntaxDiagnosticService.AnalyzeAxamlMvvmAsync(
                        Editor.Text,
                        filePath,
                        workspace,
                        cancellationToken);
                if (!semanticResolution.IsSuccessful)
                {
                    throw new InvalidDataException(
                        "MVVM generation stopped because the view or its DataContext could not be resolved safely:"
                        + Environment.NewLine
                        + string.Join(Environment.NewLine, semanticResolution.Diagnostics));
                }

                WorkspaceFileChangeProposal proposal = _workspaceFileChangeService.Prepare(
                    workspace.RootPath,
                    requests);
                Func<WorkspaceFileChangeProposal, CancellationToken, Task<bool>>? reviewChanges =
                    ReviewWorkspaceFileChangesAsync;
                if (reviewChanges is null)
                {
                    Editor.StatusText = "MVVM generation is unavailable in this host.";
                    Editor.NotificationText = "This host does not provide the workspace change review dialog.";
                    return;
                }

                if (!await reviewChanges(proposal, cancellationToken))
                {
                    Editor.StatusText = "MVVM generation was cancelled; no files were changed.";
                    return;
                }

                await _csharpSyntaxDiagnosticService.RefreshProjectContextAsync(
                    project.ProjectPath,
                    cancellationToken);
                QueueAxamlMvvmBindingContext(Editor.Text, filePath);
                Editor.StatusText = $"Generated MVVM code for {Path.GetFileName(filePath)}.";
                Editor.NotificationText = string.Join(
                    Environment.NewLine,
                    proposal.Changes.Select(static change => change.RelativePath));
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                Editor.StatusText = "MVVM generation was cancelled; no files were changed.";
            }
            catch (Exception exception) when (exception is IOException
                or UnauthorizedAccessException
                or InvalidOperationException
                or InvalidDataException
                or ArgumentException
                or System.Xml.XmlException
                or AggregateException)
            {
                Editor.StatusText = "MVVM generation failed.";
                Editor.NotificationText = exception.Message;
            }
        }

        private bool CanGenerateAxamlMvvm()
        {
            return Workspace is { Projects.Count: > 0 }
                && !string.IsNullOrWhiteSpace(Editor.CurrentFilePath)
                && string.Equals(
                    Path.GetExtension(Editor.CurrentFilePath),
                    ".axaml",
                    StringComparison.OrdinalIgnoreCase);
        }

        private static bool ProjectReferencesMvvmToolkit(string projectPath)
        {
            System.Xml.Linq.XDocument project = System.Xml.Linq.XDocument.Load(projectPath);
            return project.Descendants().Any(element =>
                element.Name.LocalName is "PackageReference" or "PackageVersion"
                && string.Equals(
                    (string?)element.Attribute("Include") ?? (string?)element.Attribute("Update"),
                    "CommunityToolkit.Mvvm",
                    StringComparison.OrdinalIgnoreCase));
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

            WorkspaceFileChangeProposal proposal;
            try
            {
                string projectDirectory = Path.GetDirectoryName(Path.GetFullPath(path))
                    ?? throw new ArgumentException("The project file must have a parent directory.", nameof(path));
                IReadOnlyList<WorkspaceFileChangeRequest> requests =
                    _projectTemplateService.PrepareProjectChangeRequests(template.Id, path);
                proposal = _workspaceFileChangeService.Prepare(projectDirectory, requests);
                if (proposal.Changes.Any(static change => !change.IsNewFile))
                {
                    throw new IOException("Project template creation cannot replace existing files.");
                }
            }
            catch (Exception exception) when (exception is IOException
                or UnauthorizedAccessException
                or InvalidOperationException
                or ArgumentException
                or KeyNotFoundException)
            {
                Editor.StatusText = "C# project creation failed.";
                Editor.NotificationText = exception.Message;
                return;
            }

            Func<WorkspaceFileChangeProposal, CancellationToken, Task<bool>>? reviewChanges =
                ReviewWorkspaceFileChangesAsync;
            if (reviewChanges is null)
            {
                Editor.StatusText = "C# project creation is unavailable.";
                Editor.NotificationText = "This host does not provide a workspace change review dialog.";
                return;
            }

            bool applied;
            try
            {
                applied = await reviewChanges(proposal, cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                Editor.StatusText = "C# project creation was cancelled; no files were created.";
                return;
            }
            catch (Exception exception) when (exception is IOException
                or UnauthorizedAccessException
                or InvalidOperationException
                or ArgumentException
                or AggregateException)
            {
                Editor.StatusText = "C# project creation failed.";
                Editor.NotificationText = exception.Message;
                return;
            }

            if (!applied)
            {
                Editor.StatusText = "C# project creation was cancelled; no files were created.";
                return;
            }

            string projectPath = Path.GetFullPath(path);
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
            ResetProjectWorkflow();
            if (AiChatBusy)
            {
                SendAIChatMessageCancelCommand.Execute(null);
            }
            GenerateAxamlMvvmCommand.NotifyCanExecuteChanged();
            foreach (EditorTabViewModel tab in _editorWorkspace.Tabs)
            {
                RefreshAxamlResourceContext(tab);
            }
            RefreshAxamlPreview();
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
            if (ProjectWorkflowBusy)
            {
                return;
            }

            CSharpWorkspace workspace = Workspace
                ?? throw new InvalidOperationException("Open a C# workspace before building.");
            ProjectWorkflowBusy = true;
            try
            {
                List<Diagnostic> diagnostics = [];
                int succeededCount = 0;

                foreach (CSharpWorkspaceProject project in workspace.Projects)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    BuilderBuildResult result = await Task.Run(() => _builderService.BuildProjectAsync(new BuilderBuildRequest
                    {
                        WorkspaceRootPath = workspace.RootPath,
                        ProjectPath = project.ProjectPath,
                    }, cancellationToken), CancellationToken.None);
                    cancellationToken.ThrowIfCancellationRequested();
                    if (!ReferenceEquals(workspace, Workspace))
                    {
                        return;
                    }

                    succeededCount += result.Succeeded ? 1 : 0;
                    diagnostics.AddRange(result.Diagnostics.Select(MapDiagnostic));
                }

                _buildDiagnostics.Clear();
                _buildDiagnostics.AddRange(diagnostics);
                await RefreshDiagnosticsAsync(cancellationToken);
                if (ReferenceEquals(workspace, Workspace))
                {
                    Editor.StatusText = $"Build: {succeededCount}/{workspace.Projects.Count} projects succeeded.";
                    Editor.NotificationText = $"Build diagnostics: {diagnostics.Count}.";
                }
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested || !ReferenceEquals(workspace, Workspace))
            {
                if (ReferenceEquals(workspace, Workspace))
                {
                    Editor.StatusText = "Workspace build canceled.";
                }
            }
            finally
            {
                ProjectWorkflowBusy = false;
            }
        }

        public async Task OpenWorkspaceFileAsync(string path, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            long version = Interlocked.Increment(ref _workspaceOpenVersion);
            _editorWorkspace.CancelPendingPreviews();
            CSharpWorkspace workspace = _workspaceLoader.Load(path);
            if (ProjectExplorer is not null)
            {
                await ProjectExplorer.LoadAsync(workspace.EntryPath, cancellationToken);
            }

            cancellationToken.ThrowIfCancellationRequested();
            if (version != Interlocked.Read(ref _workspaceOpenVersion))
            {
                return;
            }

            bool workspaceRootChanged = Workspace is null
                || !WorkspacePathIdentity.AreEqual(Workspace.RootPath, workspace.RootPath);
            bool workspaceChanged = Workspace is null
                || !WorkspacePathIdentity.AreEqual(Workspace.EntryPath, workspace.EntryPath);
            if (workspaceRootChanged)
            {
                _workspaceFileChangeService.ResetHistory();
            }

            if (workspaceChanged)
            {
                _buildDiagnostics.Clear();
                _testDiagnostics.Clear();
            }

            Workspace = workspace;
            if (workspaceChanged)
            {
                await RefreshDiagnosticsAsync(cancellationToken);
            }

            QueueLanguageAnalysis();
            RefreshAxamlPreview();
            VersionControlStatus status = await _versionControlService.GetStatusAsync(new VersionControlStatusRequest
            {
                RepositoryContextPath = workspace.RootPath,
                IncludeChanges = true,
                IncludeCapabilities = true,
            }, cancellationToken);
            if (version == Interlocked.Read(ref _workspaceOpenVersion) && ReferenceEquals(workspace, Workspace))
            {
                VersionControlStatus = status;
            }
        }

        [RelayCommand(CanExecute = nameof(CanUpdateSvnWorkspaceNow))]
        private async Task UpdateSvnWorkspaceAsync(CancellationToken cancellationToken)
        {
            CSharpWorkspace workspace = Workspace
                ?? throw new InvalidOperationException("Open an SVN workspace before updating.");
            Func<string, CancellationToken, Task<bool>>? approveUpdate = ApproveSvnUpdateAsync;
            if (approveUpdate is null
                || !await approveUpdate(workspace.RootPath, cancellationToken))
            {
                Editor.StatusText = "SVN update was not approved; no changes were made.";
                return;
            }

            if (_svnWorkingCopyOperations is null)
            {
                Editor.StatusText = "SVN update is not available in this host.";
                return;
            }

            SvnUpdateBusy = true;
            try
            {
                string output = await _svnWorkingCopyOperations.UpdateAsync(workspace.RootPath, cancellationToken);
                VersionControlStatus refreshedStatus = await _versionControlService.GetStatusAsync(
                    new VersionControlStatusRequest
                    {
                        RepositoryContextPath = workspace.RootPath,
                        IncludeChanges = true,
                        IncludeCapabilities = true,
                    },
                    cancellationToken);
                if (Workspace is not null && PathsEqual(Workspace.RootPath, workspace.RootPath))
                {
                    VersionControlStatus = refreshedStatus;
                }

                Editor.StatusText = "SVN update completed.";
                Editor.NotificationText = string.IsNullOrWhiteSpace(output)
                    ? VersionControlSummary
                    : output;
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                Editor.StatusText = "SVN update was cancelled.";
            }
            catch (Exception exception) when (exception is IOException
                or InvalidOperationException
                or ArgumentException
                or System.ComponentModel.Win32Exception)
            {
                Editor.StatusText = "SVN update failed.";
                Editor.NotificationText = exception.Message;
            }
            finally
            {
                SvnUpdateBusy = false;
            }
        }

        private bool CanUpdateSvnWorkspaceNow()
        {
            return !SvnUpdateBusy && CanUpdateSvnWorkspace;
        }

        private static bool PathsEqual(string left, string right)
        {
            return string.Equals(
                Path.GetFullPath(left),
                Path.GetFullPath(right),
                OperatingSystem.IsWindows() ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
        }

        private bool CanBuildWorkspace()
        {
            return !ProjectWorkflowBusy
                && _builderService is not NullCodeStudioBuilderService
                && Workspace is { Projects.Count: > 0 };
        }

        [RelayCommand(CanExecute = nameof(CanRunWorkspaceTests))]
        private async Task RunWorkspaceTestsAsync(CancellationToken cancellationToken)
        {
            if (ProjectWorkflowBusy)
            {
                return;
            }

            CSharpWorkspace workspace = Workspace
                ?? throw new InvalidOperationException("Open a C# workspace before running tests.");
            ProjectWorkflowBusy = true;
            try
            {
                TestRunSummary summary = await _testExecutionService.RunTestsAsync(new TestRunRequest
                {
                    WorkspaceRootPath = workspace.RootPath,
                    ProjectPath = workspace.EntryPath,
                }, cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();
                if (!ReferenceEquals(workspace, Workspace))
                {
                    return;
                }

                await ApplyTestDiagnosticsAsync(summary, workspace, cancellationToken);
                if (ReferenceEquals(workspace, Workspace))
                {
                    Editor.StatusText = $"Tests: {summary.Outcome} ({summary.TotalCount} total).";
                    Editor.NotificationText =
                        $"Tests: {summary.PassedCount} passed, {summary.FailedCount} failed, {summary.SkippedCount} skipped.";
                }
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested || !ReferenceEquals(workspace, Workspace))
            {
                if (ReferenceEquals(workspace, Workspace))
                {
                    Editor.StatusText = "Workspace tests canceled.";
                }
            }
            finally
            {
                ProjectWorkflowBusy = false;
            }
        }

        private async Task ApplyTestDiagnosticsAsync(
            TestRunSummary summary,
            CSharpWorkspace workspace,
            CancellationToken cancellationToken)
        {
            _testDiagnostics.Clear();
            foreach (TestRunFailure failure in summary.Failures)
            {
                string? sourcePath = TryGetWorkspaceTestSourcePath(failure.SourcePath, workspace.RootPath);
                _testDiagnostics.Add(new Diagnostic
                {
                    Code = "TEST",
                    Severity = DiagnosticSeverity.Error,
                    Message = FormatTestFailureMessage(failure.TestName, failure.Message),
                    SourcePath = sourcePath,
                    LineNumber = sourcePath is null || failure.LineNumber is <= 0
                        ? null
                        : failure.LineNumber,
                });
            }

            await RefreshDiagnosticsAsync(cancellationToken);
        }

        private static string? TryGetWorkspaceTestSourcePath(string? sourcePath, string workspaceRoot)
        {
            if (string.IsNullOrWhiteSpace(sourcePath))
            {
                return null;
            }

            try
            {
                string fullPath = Path.GetFullPath(sourcePath, workspaceRoot);
                return IsPathWithinDirectory(fullPath, workspaceRoot) ? fullPath : null;
            }
            catch (Exception exception) when (exception is ArgumentException or NotSupportedException or PathTooLongException)
            {
                return null;
            }
        }

        private bool CanRunWorkspaceTests()
        {
            return !ProjectWorkflowBusy
                && _testExecutionService is not NullTestExecutionService
                && Workspace is { Projects.Count: > 0 };
        }

        [RelayCommand(CanExecute = nameof(CanStartDebugSession))]
        private async Task StartDebugSessionAsync(CancellationToken cancellationToken)
        {
            if (ProjectWorkflowBusy)
            {
                return;
            }

            CSharpWorkspace workspace = Workspace
                ?? throw new InvalidOperationException("Open a C# workspace before starting a debug session.");
            string targetPath = Path.IsPathRooted(DebugTargetPath)
                ? DebugTargetPath
                : Path.Combine(workspace.RootPath, DebugTargetPath);
            ProjectWorkflowBusy = true;
            try
            {
                await StartAndAdoptDebugSessionAsync(new DebugLaunchRequest
                {
                    Target = targetPath,
                    WorkingDirectory = workspace.RootPath,
                    StopAtEntry = true,
                }, () => ReferenceEquals(workspace, Workspace), cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested || !ReferenceEquals(workspace, Workspace))
            {
                if (ReferenceEquals(workspace, Workspace))
                {
                    Editor.StatusText = "Debug session start canceled.";
                }
            }
            finally
            {
                ProjectWorkflowBusy = false;
            }
        }

        private async Task StartAndAdoptDebugSessionAsync(
            DebugLaunchRequest request,
            Func<bool> isCurrent,
            CancellationToken cancellationToken)
        {
            DebugSessionInfo session = await _debugSessionService.StartSessionAsync(request, cancellationToken);
            if (cancellationToken.IsCancellationRequested || !isCurrent())
            {
                try
                {
                    await _debugSessionService.StopSessionAsync(session.SessionId, CancellationToken.None);
                }
                catch (Exception exception) when (IsProjectWorkflowException(exception))
                {
                    AdoptDebugSession(session, request.Target);
                    Editor.StatusText = "Could not stop a debug session after its context changed.";
                    Editor.NotificationText = exception.Message;
                }

                return;
            }

            AdoptDebugSession(session, request.Target);
        }

        private void AdoptDebugSession(DebugSessionInfo session, string? targetPath)
        {
            _activeDebugSessionId = session.SessionId;
            ActiveDebugSessionState = session.State;
            _debugBreakpoints.Clear();
            DebugStackFrames.Clear();
            DebugVariables.Clear();
            SelectedDebugStackFrame = null;
            OnPropertyChanged(nameof(HasDebugSession));
            StartDebugSessionCommand.NotifyCanExecuteChanged();
            DebugSelectedProjectCommand.NotifyCanExecuteChanged();
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
                DebugSelectedProjectCommand.NotifyCanExecuteChanged();
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
                || ProjectWorkflowBusy
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
                await _editorWorkspace.OpenPreviewAsync(selectedFrame.SourcePath, cancellationToken);
                if (!string.Equals(_activeDebugSessionId, sessionId, StringComparison.Ordinal))
                {
                    return;
                }

                EditorTabViewModel? targetTab = _editorWorkspace.FindByPath(selectedFrame.SourcePath);
                if (ReferenceEquals(targetTab, _editorWorkspace.ActiveTab)
                    && targetTab?.TextView is Controls.EditorTextArea editorTextArea)
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
            if (_selectedPropertyTab is not null && !ReferenceEquals(_selectedPropertyTab, _editorWorkspace.ActiveTab))
            {
                ClearEditorPropertySelection();
            }
            ObserveActiveTab();
            ObserveActiveEditorControl();
            OnPropertyChanged(nameof(ActiveEditorTab));
            OnPropertyChanged(nameof(Editor));
            OnPropertyChanged(nameof(EditorComponent));
            OnPropertyChanged(nameof(WindowTitle));
            OnPropertyChanged(nameof(AxamlDesigner));
            OnPropertyChanged(nameof(HasAxamlPreview));
            OnPropertyChanged(nameof(CanEditActiveDocument));
            ToggleBreakpointCommand.NotifyCanExecuteChanged();
            QueueLanguageAnalysis();
            RefreshAxamlPreview();
        }

        private void ObserveActiveTab()
        {
            if (_observedTab is not null)
            {
                _observedTab.UserActivated -= OnEditorTabUserActivated;
                _observedTab.EditorSelectionChanged -= OnEditorSelectionChanged;
                _observedTab.PropertyChanged -= OnActiveTabPropertyChanged;
            }

            _observedTab = _editorWorkspace.ActiveTab;
            _observedTab.UserActivated += OnEditorTabUserActivated;
            _observedTab.EditorSelectionChanged += OnEditorSelectionChanged;
            _observedTab.PropertyChanged += OnActiveTabPropertyChanged;
            ObserveActiveDesigner();
        }

        private void OnActiveTabPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(EditorTabViewModel.WindowTitle))
            {
                OnPropertyChanged(nameof(WindowTitle));
            }
        }

        private void ObserveActiveDesigner()
        {
            AxamlDesignerViewModel? designer = _editorWorkspace.ActiveTab.Designer;
            if (ReferenceEquals(designer, _observedDesigner))
            {
                return;
            }

            if (_observedDesigner is not null)
            {
                _observedDesigner.PropertyChanged -= OnAxamlDesignerPropertyChanged;
                _observedDesigner.ApplyMissingBindingAsync = null;
            }

            _observedDesigner = designer;
            if (designer is not null)
            {
                designer.PropertyChanged += OnAxamlDesignerPropertyChanged;
                designer.ApplyMissingBindingAsync = ApplyMissingAxamlBindingAsync;
            }

            OnPropertyChanged(nameof(AxamlDesigner));
            OnPropertyChanged(nameof(HasAxamlPreview));
        }

        private void OnProjectExplorerUserSelectionChanged(ProjectExplorerItem? item)
        {
            _editorWorkspace.CancelPendingPreviews();
            _selectedPropertyTab = null;
            _editorPropertySelection = false;
            SelectedPropertyDesigner = null;
            SelectedEditorPropertyView = null;
            PropertySelectionError = string.Empty;
            NotifyStandardPropertySelectionChanged();
            if (item is null)
            {
                SelectionProperties.SetItems([]);
                PropertySelectionCaption = "Properties - no selection";
                return;
            }

            PropertyCategory category = new("Navigation", "Navigation");
            SelectionProperties.SetItems(
            [
                new PropertyItem(category, "Name", "Name", typeof(string), item.Name, isEditable: false),
                new PropertyItem(category, "Path", "Path", typeof(string), item.Path, isEditable: false),
                new PropertyItem(category, "Kind", "Kind", typeof(string), item.Kind.ToString(), isEditable: false),
                new PropertyItem(category, "Available", "Available", typeof(bool), item.IsAvailable, isEditable: false),
            ]);
            PropertySelectionCaption = $"Navigation - {item.Name}";
        }

        private void OnEditorTabUserActivated(EditorTabViewModel tab, bool designerSelection)
        {
            ProjectExplorer?.CancelPendingNavigation();
            _editorWorkspace.CancelPendingPreviews();
            _selectedPropertyTab = tab;
            if (designerSelection)
            {
                _editorPropertySelection = true;
                ShowEditorPropertySelection(tab);
                return;
            }

            _editorPropertySelection = false;
            SelectedPropertyDesigner = null;
            SelectedEditorPropertyView = null;
            PropertySelectionError = string.Empty;
            NotifyStandardPropertySelectionChanged();
            PropertyCategory category = new("Document", "Document");
            SelectionProperties.SetItems(
            [
                new PropertyItem(category, "Name", "Name", typeof(string), tab.Title, isEditable: false),
                new PropertyItem(category, "Path", "Path", typeof(string), tab.FilePath ?? "Untitled", isEditable: false),
                new PropertyItem(category, "Editor", "Editor", typeof(string), tab.EditorKind, isEditable: false),
            ]);
            PropertySelectionCaption = $"Editor - {tab.FilePath ?? "Untitled"}";
        }

        private void OnEditorNavigationRequested(CancellationToken cancellationToken) =>
            ProjectExplorer?.CancelPendingNavigation(cancellationToken);

        private void OnEditorTabClosed(EditorTabViewModel tab)
        {
            if (ReferenceEquals(_selectedPropertyTab, tab))
            {
                ClearEditorPropertySelection();
            }
        }

        private void OnEditorSelectionChanged(object? sender, EventArgs e)
        {
            if (_editorPropertySelection
                && sender is EditorTabViewModel tab
                && ReferenceEquals(tab, _selectedPropertyTab)
                && ReferenceEquals(tab, ActiveEditorTab))
            {
                ShowEditorPropertySelection(tab);
            }
        }

        private void ShowEditorPropertySelection(EditorTabViewModel tab)
        {
            if (tab.SelectedProperties is not { } provider)
            {
                SelectedPropertyDesigner = null;
                SelectedEditorPropertyView = null;
                PropertySelectionError = string.Empty;
                SelectionProperties.SetItems([]);
                PropertySelectionCaption = $"Editor - {tab.FilePath ?? "Untitled"} - no selection";
                NotifyStandardPropertySelectionChanged();
                return;
            }

            Control? propertyView = tab.PropertiesView;
            SelectionProperties.SetItems(propertyView is null ? provider.Properties : []);
            SelectedPropertyDesigner = tab.Designer;
            SelectedEditorPropertyView = propertyView;
            PropertySelectionError = string.Empty;
            PropertySelectionCaption = $"Editor - {tab.FilePath ?? "Untitled"} - {provider.Context.TargetDisplayName}";
            NotifyStandardPropertySelectionChanged();
        }

        private void ClearEditorPropertySelection()
        {
            _selectedPropertyTab = null;
            _editorPropertySelection = false;
            SelectedPropertyDesigner = null;
            SelectedEditorPropertyView = null;
            PropertySelectionError = string.Empty;
            SelectionProperties.SetItems([]);
            PropertySelectionCaption = "Properties - no selection";
            NotifyStandardPropertySelectionChanged();
        }

        private void NotifyStandardPropertySelectionChanged()
        {
            OnPropertyChanged(nameof(HasStandardEditorPropertySelection));
            OnPropertyChanged(nameof(CanEditSelectedProperties));
        }

        [RelayCommand]
        private Task ApplyEditorPropertiesAsync() => CompleteEditorPropertyOperationAsync(apply: true);

        [RelayCommand]
        private Task DiscardEditorPropertiesAsync() => CompleteEditorPropertyOperationAsync(apply: false);

        private async Task CompleteEditorPropertyOperationAsync(bool apply)
        {
            EditorTabViewModel? tab = _selectedPropertyTab;
            IPropertyProvider? provider = tab?.SelectedProperties;
            if (!HasStandardEditorPropertySelection || tab is null || provider is null || provider.IsReadOnly)
            {
                return;
            }

            try
            {
                if (apply)
                {
                    await provider.ApplyAsync();
                }
                else
                {
                    await provider.DiscardAsync();
                }

                if (ReferenceEquals(tab, _selectedPropertyTab)
                    && ReferenceEquals(provider, tab.SelectedProperties)
                    && HasStandardEditorPropertySelection)
                {
                    SelectionProperties.SetItems(provider.Properties);
                    PropertySelectionError = string.Empty;
                }
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                if (ReferenceEquals(tab, _selectedPropertyTab))
                {
                    PropertySelectionError = exception.Message;
                }
            }
        }

        private void UpdateDesignerSelectionCaption(EditorTabViewModel tab, AxamlDesignerViewModel designer)
        {
            PropertySelectionCaption = $"Editor - {tab.FilePath ?? "Untitled"} - {designer.SelectedElementName}";
        }

        private void ObserveActiveEditorControl()
        {
            if (_observedEditorTextArea is not null)
            {
                _observedEditorTextArea.CompletionRequested -= OnCompletionRequested;
                _observedEditorTextArea.GoToDefinitionRequested -= OnGoToDefinitionRequested;
            }

            _observedEditorTextArea = _editorWorkspace.ActiveTab.TextView as Controls.EditorTextArea;
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
                    ObserveActiveDesigner();
                }

                QueueLanguageAnalysis();
                RefreshAxamlPreview();
                GenerateAxamlMvvmCommand.NotifyCanExecuteChanged();
            }
        }

        private void RefreshAxamlPreview()
        {
            string? filePath = Editor.CurrentFilePath;
            bool isAxamlDocument = !string.IsNullOrWhiteSpace(filePath)
                && string.Equals(Path.GetExtension(filePath), ".axaml", StringComparison.OrdinalIgnoreCase);
            string? markup = isAxamlDocument ? Editor.Text : null;
            RefreshAxamlResourceContext(ActiveEditorTab);
            QueueAxamlMvvmBindingContext(markup, filePath);
        }

        private void RefreshAxamlResourceContext(EditorTabViewModel tab)
        {
            string? path = tab.Editor.CurrentFilePath;
            if (!string.Equals(Path.GetExtension(path), ".axaml", StringComparison.OrdinalIgnoreCase))
            {
                path = null;
            }

            tab.Designer?.SetResourceContext(
                path,
                AxamlResourceProjectResolver.FindProjectRoot(Workspace, Workspace?.RootPath, path));
        }

        private void QueueAxamlMvvmBindingContext(string? markup, string? filePath)
        {
            _axamlBindingAnalysisCancellation?.Cancel();
            _axamlBindingAnalysisCancellation?.Dispose();
            CancellationTokenSource cancellation = new();
            _axamlBindingAnalysisCancellation = cancellation;
            EditorTabViewModel tab = ActiveEditorTab;
            if (tab.Designer is { } designer)
            {
                _ = RefreshAxamlMvvmBindingContextAsync(tab, designer, markup, filePath, cancellation.Token);
            }
        }

        private async Task RefreshAxamlMvvmBindingContextAsync(
            EditorTabViewModel tab,
            AxamlDesignerViewModel designer,
            string? markup,
            string? filePath,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(markup)
                || string.IsNullOrWhiteSpace(filePath)
                || Workspace is not { Projects.Count: > 0 } workspace)
            {
                designer.SetBindingContext(
                    [],
                    "Open a C# workspace and a view with a resolvable runtime DataContext to select ViewModel members.");
                return;
            }

            try
            {
                await Task.Delay(TimeSpan.FromMilliseconds(300), cancellationToken);
                AxamlMvvmSemanticResolution resolution =
                    await _csharpSyntaxDiagnosticService.AnalyzeAxamlMvvmAsync(
                        markup,
                        filePath,
                        workspace,
                        cancellationToken,
                        allowExistingRuntimeDataContext: true);
                if (!ReferenceEquals(workspace, Workspace)
                    || !ReferenceEquals(tab, ActiveEditorTab)
                    || tab.Editor.CurrentFilePath is not { } currentPath
                    || !WorkspacePathIdentity.AreEqual(filePath, currentPath)
                    || !string.Equals(markup, tab.Editor.Text, StringComparison.Ordinal))
                {
                    return;
                }

                string? diagnostic = resolution.IsSuccessful && resolution.HasRuntimeDataContext
                    ? null
                    : resolution.Diagnostics.Count > 0
                        ? string.Join(Environment.NewLine, resolution.Diagnostics)
                        : "The view has no uniquely resolved runtime DataContext.";
                designer.SetBindingContext(
                    resolution.IsSuccessful && resolution.HasRuntimeDataContext
                        ? resolution.ViewModelMembers
                        : [],
                    diagnostic);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
            }
            catch (Exception exception) when (exception is InvalidOperationException or InvalidDataException or IOException)
            {
                if (cancellationToken.IsCancellationRequested || !ReferenceEquals(tab, ActiveEditorTab))
                {
                    return;
                }

                designer.SetBindingContext([], $"ViewModel member analysis failed: {exception.Message}");
            }
        }

        private async Task<bool> ApplyMissingAxamlBindingAsync(AxamlBindingRowViewModel row)
        {
            ArgumentNullException.ThrowIfNull(row);
            EditorTabViewModel tab = ActiveEditorTab;
            EditorViewModel editor = tab.Editor;
            AxamlDesignerViewModel designer = tab.Designer
                ?? throw new InvalidOperationException("The active document does not have an AXAML designer.");
            string filePath = editor.CurrentFilePath
                ?? throw new InvalidOperationException("Save the AXAML view before creating a ViewModel binding.");
            CSharpWorkspace workspace = Workspace
                ?? throw new InvalidOperationException("Open the containing C# workspace before creating a ViewModel binding.");
            string markup = editor.Text;
            AxamlPreviewNode selectedNode = designer.SelectedNode
                ?? throw new InvalidOperationException("Select a supported control before applying a ViewModel binding.");
            bool IsTargetCurrent() => ReferenceEquals(tab, ActiveEditorTab)
                && ReferenceEquals(workspace, Workspace)
                && editor.CurrentFilePath is { } currentPath
                && WorkspacePathIdentity.AreEqual(filePath, currentPath)
                && string.Equals(markup, editor.Text, StringComparison.Ordinal)
                && ReferenceEquals(selectedNode, designer.SelectedNode);
            CSharpWorkspaceProject project = workspace.Projects
                .Where(candidate => IsPathWithinDirectory(
                    filePath,
                    Path.GetDirectoryName(candidate.ProjectPath)!))
                .OrderByDescending(candidate => candidate.ProjectPath.Length)
                .FirstOrDefault()
                ?? throw new InvalidOperationException("The active AXAML file is not inside a project in this workspace.");

            if (!ProjectReferencesMvvmToolkit(project.ProjectPath))
            {
                throw new InvalidDataException("Add CommunityToolkit.Mvvm to the active project before creating a ViewModel member.");
            }

            string? memberName = row.GetRequestedMember();
            if (memberName is null
                || !AxamlControlCatalog.TryGet(row.ControlType, out AxamlControlCapability capability)
                || capability.BindableProperties.FirstOrDefault(property =>
                    string.Equals(property.Name, row.PropertyName, StringComparison.Ordinal)) is not { } bindableProperty)
            {
                throw new InvalidDataException(row.Diagnostic ?? "The selected binding is not supported by the designer.");
            }

            AxamlMvvmSemanticResolution resolution =
                await _csharpSyntaxDiagnosticService.AnalyzeAxamlMvvmAsync(
                    markup,
                    filePath,
                    workspace,
                    CancellationToken.None,
                    allowExistingRuntimeDataContext: true);
            if (!IsTargetCurrent())
            {
                throw new InvalidOperationException("The workspace or AXAML document changed during binding analysis. Retry the binding.");
            }

            if (!resolution.IsSuccessful
                || !resolution.HasRuntimeDataContext
                || string.IsNullOrWhiteSpace(resolution.ViewModelTypeName))
            {
                throw new InvalidDataException(
                    "A missing ViewModel member cannot be created because the runtime DataContext is not uniquely resolved:"
                    + Environment.NewLine
                    + string.Join(Environment.NewLine, resolution.Diagnostics));
            }

            AxamlMvvmMemberRequirement requirement = new(
                memberName,
                bindableProperty.TypeName,
                bindableProperty.IsCommand);
            AxamlMvvmMemberPlanEntry memberPlan = new AxamlMvvmMemberPlanner()
                .Plan([requirement], resolution.ViewModelMembers, createMissingMembers: true)
                .Single();
            if (memberPlan.Action == AxamlMvvmMemberPlanAction.Reuse)
            {
                designer.ApplyBindingRow(row);
                return true;
            }

            if (memberPlan.Action != AxamlMvvmMemberPlanAction.Create)
            {
                throw new InvalidDataException(
                    memberPlan.Diagnostic ?? $"ViewModel member '{memberName}' cannot be created safely.");
            }

            WorkspaceFileChangeRequest sourceRequest =
                await _csharpSyntaxDiagnosticService.PlanViewModelSourceAdditionAsync(
                    workspace.RootPath,
                    project.ProjectPath,
                    resolution.ViewModelTypeName,
                    [memberPlan],
                    CancellationToken.None);
            IReadOnlyList<string> newCompilerErrors =
                await _csharpSyntaxDiagnosticService.GetNewCompilerErrorsAsync(
                    project.ProjectPath,
                    sourceRequest,
                    CancellationToken.None,
                    workspace.RootPath);
            if (newCompilerErrors.Count > 0)
            {
                throw new InvalidDataException(
                    "Creating the ViewModel member would introduce C# compiler errors:"
                    + Environment.NewLine
                    + string.Join(Environment.NewLine, newCompilerErrors));
            }

            if (!IsTargetCurrent())
            {
                throw new InvalidOperationException("The selected control or AXAML document changed during member planning. Retry the binding.");
            }

            string proposedMarkup = designer.CreateBindingMarkupForRow(row);
            string relativeAxamlPath = Path.GetRelativePath(workspace.RootPath, filePath);
            string savedMarkup = File.ReadAllText(filePath);
            List<WorkspaceFileChangeRequest> requests = [sourceRequest];
            if (!string.Equals(markup, proposedMarkup, StringComparison.Ordinal))
            {
                requests.Add(WorkspaceFileChangeRequest.ReplaceExisting(
                    relativeAxamlPath,
                    savedMarkup,
                    proposedMarkup,
                    expectedOpenDocumentContent: markup));
            }

            WorkspaceFileChangeProposal proposal = _workspaceFileChangeService.Prepare(workspace.RootPath, requests);
            Func<WorkspaceFileChangeProposal, CancellationToken, Task<bool>>? review =
                ReviewWorkspaceFileChangesAsync;
            if (review is null)
            {
                throw new InvalidOperationException("This host does not provide a workspace change review dialog.");
            }

            if (!await review(proposal, CancellationToken.None))
            {
                editor.StatusText = "Binding and ViewModel member creation cancelled; no files were changed.";
                return false;
            }

            await _csharpSyntaxDiagnosticService.RefreshProjectContextAsync(project.ProjectPath);
            if (ReferenceEquals(tab, ActiveEditorTab) && ReferenceEquals(workspace, Workspace))
            {
                QueueAxamlMvvmBindingContext(editor.Text, filePath);
            }
            editor.StatusText = $"Created ViewModel member '{memberName}' and applied the AXAML binding.";
            editor.NotificationText = string.Join(
                Environment.NewLine,
                proposal.Changes.Select(static change => change.RelativePath));
            return true;
        }

        private void OnAxamlDesignerPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(AxamlDesignerViewModel.HasPreview))
            {
                OnPropertyChanged(nameof(HasAxamlPreview));
            }

            if (e.PropertyName == nameof(AxamlDesignerViewModel.SelectedNode)
                && sender is AxamlDesignerViewModel designer
                && ReferenceEquals(designer, SelectedPropertyDesigner)
                && _selectedPropertyTab is { } tab)
            {
                UpdateDesignerSelectionCaption(tab, designer);
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
            return DiagnosticCollection.ConsumeAsync(
                _buildDiagnostics.Concat(_languageDiagnostics).Concat(_testDiagnostics),
                cancellationToken);
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
