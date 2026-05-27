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
    }
}
