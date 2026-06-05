using Avalonia.Controls;

namespace AA98_AvlnCodeStudio.UI.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public MainWindow(ViewModels.MainWindowViewModel viewModel)
            : this()
        {
            DataContext = viewModel;
            var editorHost = this.FindControl<ContentControl>(nameof(EditorHost));
            if (editorHost is not null)
            {
                editorHost.Content = viewModel.EditorComponent.View;
            }
        }
    }
}