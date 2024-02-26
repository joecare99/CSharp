using BindingGroupExp.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace BindingGroupExp.View
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += Window_Loaded;

        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainWindowViewModel vm)
            {
                vm.DataGroup = stackPanel1.BindingGroup;
                vm.OnShowMessage = ShowMessage;
            }
        }

        private bool ShowMessage(string arg)
        {
            return MessageBox.Show(arg) == MessageBoxResult.OK;
        }

        // This event occurs when a ValidationRule in the BindingGroup
        // or in a Binding fails.
        private void ItemError(object sender, ValidationErrorEventArgs e) => (DataContext as MainWindowViewModel)?.ItemError(sender, e);

        void stackPanel1_Loaded(object sender, RoutedEventArgs e)
        {
            // Begin an edit transaction that enables
            // the object to accept or roll back changes.
            stackPanel1.BindingGroup.BeginEdit();            
        }

    }
}
