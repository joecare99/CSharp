using AA98_AvlnCodeStudio.UI.Resources;
using AA98_AvlnCodeStudio.Editor.Services;
using AA98_AvlnCodeStudio.Base.Building.Models;
using AA98_AvlnCodeStudio.Base.Building.Services;
using AA98_AvlnCodeStudio.Base.Versioning.Models;
using AA98_AvlnCodeStudio.Base.Versioning.Services;
using AA98_AvlnCodeStudio.Diagnostics.UI.ViewModels;
using AppKomponentBaseLib.Diagnostics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AA98_AvlnCodeStudio.Planning.Core.Services;
using Config.UI.Avalonia.ViewModels;
using Project.Explorer.Avalonia.ViewModels;
using AA98_AvlnCodeStudio.Model.Workspaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.UI.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        private readonly EditorDocumentWorkspace _editorWorkspace;
        private readonly AA98_AvlnCodeStudio.Base.UI.Services.IEditorFileDialogService _editorFileDialogService;
        private readonly CSharpWorkspaceLoader _workspaceLoader;
        private readonly Services.IWorkspaceFileDialogService _workspaceFileDialogService;
        private readonly ICodeStudioBuilderService _builderService;
        private readonly IVersionControlService _versionControlService;
        private EditorViewModel? _observedEditor;

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
                new Services.DesignEditorFileDialogService())
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
            AA98_AvlnCodeStudio.Base.UI.Services.IEditorFileDialogService? editorFileDialogService = null)
        {
            _editorWorkspace = editorWorkspace ?? new EditorDocumentWorkspace(editorComponent);
            _workspaceLoader = workspaceLoader ?? new CSharpWorkspaceLoader();
            _workspaceFileDialogService = workspaceFileDialogService ?? new Services.DesignWorkspaceFileDialogService();
            _builderService = builderService ?? new NullCodeStudioBuilderService();
            _versionControlService = versionControlService ?? new NullVersionControlService();
            _editorFileDialogService = editorFileDialogService ?? new Services.DesignEditorFileDialogService();
            _editorWorkspace.PropertyChanged += OnEditorWorkspacePropertyChanged;
            _observedEditor = _editorWorkspace.ActiveTab.Editor;
            _observedEditor.PropertyChanged += OnActiveEditorPropertyChanged;
            PlanningExplorer = planningExplorer;
            DiagnosticCollection = diagnosticCollection ?? throw new ArgumentNullException(nameof(diagnosticCollection));
            ConfigUi = configUi;
            ProjectExplorer = projectExplorer;
            PlanningExplorer.LoadAsync(Directory.GetCurrentDirectory()).GetAwaiter().GetResult();
            ProjectExplorer?.LoadAsync(Directory.GetCurrentDirectory()).GetAwaiter().GetResult();
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
        private CSharpWorkspace? _workspace;

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

        public string OpenWorkspaceCommandText => UiStrings.OpenWorkspaceCommandText;

        public string BuildWorkspaceCommandText => _builderService is NullCodeStudioBuilderService
            ? UiStrings.BuildUnavailableText
            : UiStrings.BuildWorkspaceCommandText;

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

            await DiagnosticCollection.ConsumeAsync(diagnostics, cancellationToken);
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
            OnPropertyChanged(nameof(ActiveEditorTab));
            OnPropertyChanged(nameof(Editor));
            OnPropertyChanged(nameof(EditorComponent));
            OnPropertyChanged(nameof(WindowTitle));
        }

        private void OnActiveEditorPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(EditorViewModel.WindowTitle))
            {
                OnPropertyChanged(nameof(WindowTitle));
            }
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
