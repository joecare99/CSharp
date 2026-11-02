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
using System.Text.Json;
using Config.Service;
using AA98_AvlnCodeStudio.Versioning.Svn.Services;

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
        private readonly AxamlDesignerViewModel _axamlDesigner;
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
        private readonly List<AIMessage> _aiConversation = [];
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
            AxamlDesignerViewModel? axamlDesignerViewModel = null,
            AIChatService? aiChatService = null,
            OpenAICompatibleAISettings? aiSettings = null,
            Configuration.CodeStudioSettings? codeStudioSettings = null,
            ConfigService? configService = null,
            ISvnWorkingCopyOperations? svnWorkingCopyOperations = null,
            WorkspaceFileChangeService? workspaceFileChangeService = null,
            AxamlMvvmGenerator? axamlMvvmGenerator = null)
        {
            _editorWorkspace = editorWorkspace ?? new EditorDocumentWorkspace(editorComponent);
            _workspaceLoader = workspaceLoader ?? new CSharpWorkspaceLoader();
            _projectTemplateService = projectTemplateService ?? new CSharpProjectTemplateService();
            _axamlMvvmGenerator = axamlMvvmGenerator ?? new AxamlMvvmGenerator();
            _workspaceFileChangeService = workspaceFileChangeService
                ?? new WorkspaceFileChangeService(_editorWorkspace);
            _projectPackageService = projectPackageService ?? new DotNetCSharpProjectPackageService();
            _axamlDesigner = axamlDesignerViewModel ?? new AxamlDesignerViewModel();
            _axamlDesigner.MarkupChanged += OnAxamlDesignerMarkupChanged;
            _axamlDesigner.PropertyChanged += OnAxamlDesignerPropertyChanged;
            _workspaceFileDialogService = workspaceFileDialogService ?? new Services.DesignWorkspaceFileDialogService();
            _builderService = builderService ?? new NullCodeStudioBuilderService();
            _versionControlService = versionControlService ?? new NullVersionControlService();
            _svnWorkingCopyOperations = svnWorkingCopyOperations;
            _editorFileDialogService = editorFileDialogService ?? new Services.DesignEditorFileDialogService();
            _csharpSyntaxDiagnosticService = csharpSyntaxDiagnosticService ?? new Services.CSharpSyntaxDiagnosticService();
            _testExecutionService = testExecutionService ?? new NullTestExecutionService();
            _debugSessionService = debugSessionService ?? new NullDebugSessionService();
            _aiChatService = aiChatService;
            _aiSettings = aiSettings;
            _codeStudioSettings = codeStudioSettings;
            _configService = configService;
            if (_aiSettings is not null)
            {
                AiEndpoint = _aiSettings.Endpoint;
                AiModel = _aiSettings.ModelId;
                AiApiKey = _aiSettings.ApiKey;
            }
            else if (_codeStudioSettings is not null)
            {
                AiEndpoint = _codeStudioSettings.AIEndpoint;
                AiModel = _codeStudioSettings.AIModelId;
            }
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

        public WorkspaceFileChangeService WorkspaceFileChangeService => _workspaceFileChangeService;

        public Func<WorkspaceFileChangeProposal, CancellationToken, Task<bool>>? ReviewWorkspaceFileChangesAsync { get; set; }

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

        public string WindowTitle => Editor.WindowTitle;

        public string NavigationTitle => UiStrings.NavigationTitle;

        public string NavigationPlaceholder => UiStrings.NavigationPlaceholder;

        public string PlanningExplorerTitle => UiStrings.PlanningExplorerTitle;

        public string EditorRegionTitle => UiStrings.EditorRegionTitle;

        public string StatusRegionTitle => UiStrings.StatusRegionTitle;

        public string NotificationRegionTitle => UiStrings.NotificationRegionTitle;

        public string DocumentStatusLabel => UiStrings.DocumentStatusLabel;

        public string EditorStatusLabel => UiStrings.EditorStatusLabel;

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

        public ObservableCollection<AIChatMessageViewModel> AIChatMessages { get; } = [];

        public ObservableCollection<CSharpProjectTemplateDefinition> ProjectTemplates { get; } = [];

        public AxamlDesignerViewModel AxamlDesigner => _axamlDesigner;

        public bool HasAxamlPreview => _axamlDesigner.HasPreview;

        public Avalonia.Controls.GridLength PreviewColumnWidth => HasAxamlPreview
            ? new Avalonia.Controls.GridLength(1, Avalonia.Controls.GridUnitType.Star)
            : new Avalonia.Controls.GridLength(0);

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

        public string AIChatStatus => string.IsNullOrWhiteSpace(AiModel)
            ? "Enter a model ID to connect to the configured OpenAI-compatible endpoint."
            : AiChatBusy ? "Waiting for the AI provider..." : $"Tools available to the assistant: {AIChatToolCount}.";

        public int AIChatToolCount => _aiChatService?.AvailableTools.Count ?? 0;

        public string AIChatEndpointPlaceholderText => "Chat Completions endpoint (e.g. http://localhost:11434/v1/chat/completions)";

        public string AIChatModelPlaceholderText => "Model ID (required)";

        public string AIChatApiKeyPlaceholderText => "API key or env:VARIABLE_NAME (never persisted)";

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
        private bool _aiChatBusy;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(AIChatStatus))]
        private string _aiEndpoint = "http://localhost:11434/v1/chat/completions";

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(AIChatStatus))]
        private string _aiModel = string.Empty;

        [ObservableProperty]
        private string _aiApiKey = string.Empty;

        partial void OnAiEndpointChanged(string value)
        {
            if (_aiSettings is not null)
            {
                _aiSettings.Endpoint = value;
            }

            if (_codeStudioSettings is not null)
            {
                _codeStudioSettings.AIEndpoint = value;
            }
        }

        partial void OnAiModelChanged(string value)
        {
            if (_aiSettings is not null)
            {
                _aiSettings.ModelId = value;
            }

            if (_codeStudioSettings is not null)
            {
                _codeStudioSettings.AIModelId = value;
            }
        }

        partial void OnAiApiKeyChanged(string value)
        {
            if (_aiSettings is not null)
            {
                _aiSettings.ApiKey = value;
            }
        }

        private bool CanSendAIChatMessage()
        {
            return !AiChatBusy && !string.IsNullOrWhiteSpace(AiChatInput);
        }

        [RelayCommand(CanExecute = nameof(CanSendAIChatMessage))]
        private async Task SendAIChatMessageAsync(CancellationToken cancellationToken)
        {
            if (_aiChatService is null)
            {
                AIChatMessages.Add(new AIChatMessageViewModel("System", "AI chat is not available in this host."));
                return;
            }

            if (string.IsNullOrWhiteSpace(AiModel))
            {
                AIChatMessages.Add(new AIChatMessageViewModel("System", "Enter a model ID before sending a message."));
                return;
            }

            string userText = AiChatInput.Trim();
            AiChatInput = string.Empty;
            _aiConversation.Add(new AIMessage(AIMessageRole.User, userText));
            AIChatMessages.Add(new AIChatMessageViewModel("You", userText));
            AiChatBusy = true;
            OnPropertyChanged(nameof(AIChatStatus));
            try
            {
                List<AIMessage> requestMessages =
                [
                    new AIMessage(AIMessageRole.System, await CreateAIChatContextAsync(cancellationToken)),
                ];
                requestMessages.AddRange(_aiConversation);
                AIChatTurnResult result = await _aiChatService.SendAsync(
                    requestMessages,
                    approveToolExecution: ApproveAIChatToolExecutionAsync,
                    cancellationToken: cancellationToken);
                _aiConversation.Clear();
                _aiConversation.AddRange(result.Messages.Where(static message => message.Role != AIMessageRole.System));
                bool buildDiagnosticsCleared = false;
                foreach (AIChatToolExecution execution in result.ToolExecutions)
                {
                    string state = execution.WasExecuted ? "completed" : "not approved";
                    AIChatMessages.Add(new AIChatMessageViewModel(
                        $"Tool: {execution.Call.Name} ({state})",
                        execution.Result));
                    if (execution.WasExecuted
                        && execution.Call.Name is "workspace.build" or "workspace.inspect")
                    {
                        if (!buildDiagnosticsCleared)
                        {
                            _buildDiagnostics.Clear();
                            buildDiagnosticsCleared = true;
                        }

                        await AddWorkspaceToolDiagnosticsToWorkbenchAsync(execution, cancellationToken);
                    }
                }

                if (!string.IsNullOrWhiteSpace(result.AssistantMessage.Content))
                {
                    AIChatMessages.Add(new AIChatMessageViewModel("Assistant", result.AssistantMessage.Content));
                }
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                AIChatMessages.Add(new AIChatMessageViewModel("System", "The chat request was cancelled."));
            }
            catch (Exception exception)
            {
                AIChatMessages.Add(new AIChatMessageViewModel("Error", exception.Message));
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
            string sourceDescription = execution.Call.Name switch
            {
                "workspace.build" => "Build",
                "workspace.inspect" => "Inspection",
                _ => "Workspace",
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

                if (!projectResult.TryGetProperty("Diagnostics", out JsonElement projectDiagnostics)
                    || projectDiagnostics.ValueKind != JsonValueKind.Array)
                {
                    throw new JsonException($"Build result for '{projectPath}' has no diagnostics array.");
                }

                foreach (JsonElement item in projectDiagnostics.EnumerateArray())
                {
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

            _buildDiagnostics.AddRange(diagnostics);
            await RefreshDiagnosticsAsync(cancellationToken);
            if (rejectedLocations > 0)
            {
                AIChatMessages.Add(new AIChatMessageViewModel(
                    "System",
                    $"Ignored {rejectedLocations} {sourceDescription.ToLowerInvariant()} diagnostic location(s) outside the current workspace."));
            }
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

        private async Task<string> CreateAIChatContextAsync(CancellationToken cancellationToken)
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
            if (Workspace is not null && PathsEqual(Workspace.RootPath, workspace.RootPath))
            {
                VersionControlStatus = status;
            }

            string versionControlContext = !status.IsRepositoryRootDiscovered
                ? "No Git or SVN repository was detected."
                : $"Repository: {status.RepositoryName ?? status.RepositoryRootPath}; reference: {status.ActiveReferenceName ?? UiStrings.UnknownReferenceText}; local changes: {status.Changes.Count} ({(status.HasLocalChanges ? "present" : "none")}).";
            return $"You are the AI assistant inside AvlnCodeStudio. The current C# workspace is '{workspace.EntryPath}'. Its projects are:{Environment.NewLine}{projects}{Environment.NewLine}Version-control status (refreshed for this turn): {versionControlContext}{Environment.NewLine}Use workspace.inspect for more workspace metadata. Only build projects listed in this workspace; workspace.build requires explicit user approval.";
        }

        [RelayCommand]
        private void ClearAIChat()
        {
            _aiConversation.Clear();
            AIChatMessages.Clear();
        }

        [RelayCommand]
        private async Task SaveAIChatSettingsAsync(CancellationToken cancellationToken)
        {
            if (_configService is null || _codeStudioSettings is null)
            {
                AIChatMessages.Add(new AIChatMessageViewModel(
                    "Error",
                    "Persistent AI settings are not available in this host."));
                return;
            }

            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                AiEndpoint = AiEndpoint.Trim();
                AiModel = AiModel.Trim();
                _codeStudioSettings.AIEndpoint = AiEndpoint;
                _codeStudioSettings.AIModelId = AiModel;
                await _configService.SaveAsync("Workbench", _codeStudioSettings);
                AIChatMessages.Add(new AIChatMessageViewModel(
                    "System",
                    "AI endpoint and model saved. The API key remains in memory only."));
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                AIChatMessages.Add(new AIChatMessageViewModel(
                    "Error",
                    $"Could not save AI settings: {exception.Message}"));
            }
        }

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
                IReadOnlyList<WorkspaceFileChangeRequest> requests =
                    _axamlMvvmGenerator.Generate(relativeAxamlPath, Editor.Text);
                string viewModelPath = Path.GetFullPath(
                    Path.Combine(workspace.RootPath, requests[1].RelativePath));
                if (File.Exists(viewModelPath))
                {
                    throw new InvalidDataException(
                        $"'{requests[1].RelativePath}' already exists. Incremental MVVM generation is not supported yet; the existing ViewModel was left unchanged.");
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
            GenerateAxamlMvvmCommand.NotifyCanExecuteChanged();
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
                GenerateAxamlMvvmCommand.NotifyCanExecuteChanged();
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
                OnPropertyChanged(nameof(PreviewColumnWidth));
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
