using AA98_AvlnCodeStudio.UI.Resources;
using AA98_AvlnCodeStudio.Editor.Services;
using AA98_AvlnCodeStudio.Diagnostics.UI.ViewModels;
using System;
using System.IO;
using AA98_AvlnCodeStudio.Planning.Core.Services;
using Config.UI.Avalonia.ViewModels;
using Project.Explorer.Avalonia.ViewModels;

namespace AA98_AvlnCodeStudio.UI.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        private readonly Components.IAvaloniaEditorComponent _editorComponent;

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
                null)
        {
        }

        public MainWindowViewModel(
            Components.IAvaloniaEditorComponent editorComponent,
            PlanningExplorerViewModel planningExplorer,
            DiagnosticCollectionViewModel diagnosticCollection,
            ConfigUiViewModel? configUi,
            ProjectExplorerViewModel? projectExplorer)
        {
            _editorComponent = editorComponent;
            Editor = editorComponent.EditorViewModel;
            PlanningExplorer = planningExplorer;
            DiagnosticCollection = diagnosticCollection ?? throw new ArgumentNullException(nameof(diagnosticCollection));
            ConfigUi = configUi;
            ProjectExplorer = projectExplorer;
            PlanningExplorer.LoadAsync(Directory.GetCurrentDirectory()).GetAwaiter().GetResult();
            ProjectExplorer?.LoadAsync(Directory.GetCurrentDirectory()).GetAwaiter().GetResult();
        }

        public EditorViewModel Editor { get; }

        public PlanningExplorerViewModel PlanningExplorer { get; }

        public DiagnosticCollectionViewModel DiagnosticCollection { get; }

        /// <summary>
        /// Gets the host-composed reusable configuration UI state.
        /// </summary>
        public ConfigUiViewModel? ConfigUi { get; }

        /// <summary>Gets the host-composed reusable project explorer state.</summary>
        public ProjectExplorerViewModel? ProjectExplorer { get; }

        public Components.IAvaloniaEditorComponent EditorComponent => _editorComponent;

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

        public string NavigationPlaceholderMenuText => UiStrings.NavigationPlaceholderMenuText;

        public string StatusPlaceholderMenuText => UiStrings.StatusPlaceholderMenuText;
    }
}
