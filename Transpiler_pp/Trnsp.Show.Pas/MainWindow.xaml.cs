using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Trnsp.Show.Pas;
using Trnsp.Show.Pas.ViewModels;

namespace Transp.Show.Pas
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = App.Current.Services.GetService<MainViewModel>();
            if (DataContext is MainViewModel vm)
            {
                vm.PropertyChanged += ViewModel_PropertyChanged;
            }
        }

        private void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender is MainViewModel vm)
            {
                if (e.PropertyName == nameof(MainViewModel.SourceSelectionStart) || 
                    e.PropertyName == nameof(MainViewModel.SourceSelectionLength))
                {
                    // Delay the highlight to avoid interrupting TreeView selection logic
                    Dispatcher.InvokeAsync(() => HighlightSource(vm.SourceSelectionStart, vm.SourceSelectionLength));
                }
            }
        }

        private void HighlightSource(int start, int length)
        {
            if (start >= 0 && length > 0 && start + length <= SourceCodeEditor.Text.Length)
            {
                SourceCodeEditor.Focus();
                SourceCodeEditor.Select(start, length);
                SourceCodeEditor.ScrollToLine(SourceCodeEditor.GetLineIndexFromCharacterIndex(start));
            }
        }

        private void CodeStructureTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (DataContext is MainViewModel vm)
            {
                vm.SelectedNode = e.NewValue as CodeBlockNode;
            }
        }
    }
}