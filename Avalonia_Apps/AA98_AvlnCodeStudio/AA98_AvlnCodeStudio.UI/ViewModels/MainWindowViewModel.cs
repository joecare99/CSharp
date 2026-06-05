using AA98_AvlnCodeStudio.UI.Resources;
using AA98_AvlnCodeStudio.Editor.Services;

namespace AA98_AvlnCodeStudio.UI.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        private readonly Components.IAvaloniaEditorComponent _editorComponent;

        public MainWindowViewModel()
            : this(new Components.AvaloniaEditorComponent(
                new EditorWorkflow(
                    new Model.Documents.FileEditorDocument(),
                    new Services.DesignEditorFileDialogService(),
                    new Services.DesignTextDocumentStorageService()),
                new EditorViewModel(),
                new Controls.EditorTextArea()))
        {
        }

        public MainWindowViewModel(Components.IAvaloniaEditorComponent editorComponent)
        {
            _editorComponent = editorComponent;
            Editor = editorComponent.EditorViewModel;
        }

        public EditorViewModel Editor { get; }

        public Components.IAvaloniaEditorComponent EditorComponent => _editorComponent;

        public string WindowTitle => Editor.WindowTitle;

        public string NavigationTitle => UiStrings.NavigationTitle;

        public string NavigationPlaceholder => UiStrings.NavigationPlaceholder;

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
