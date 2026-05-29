using AA98_AvlnCodeStudio.UI.Resources;

namespace AA98_AvlnCodeStudio.UI.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel()
            : this(new EditorViewModel())
        {
        }

        public MainWindowViewModel(EditorViewModel editor)
        {
            Editor = editor;
        }

        public EditorViewModel Editor { get; }

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
