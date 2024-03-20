using MVVM_26_BindingGroupExp.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace MVVM_26_BindingGroupExp.Views
{
    /// <summary>
    /// Interaktionslogik für BindingGroupView.xaml
    /// </summary>
    public partial class BindingGroupView : Page
    {
        public BindingGroupView()
        {
            InitializeComponent();
            Loaded += Page_Loaded;

        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is BindingGroupViewModel vm)
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
        private void ItemError(object sender, ValidationErrorEventArgs e) 
            => (DataContext as BindingGroupViewModel)?.ItemError(sender, e);

        void stackPanel1_Loaded(object sender, RoutedEventArgs e)
        {
            // Begin an edit transaction that enables
            // the object to accept or roll back changes.
            stackPanel1.BindingGroup.BeginEdit();            
        }

    }
}
