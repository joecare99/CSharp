using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using Microsoft.Extensions.DependencyInjection;
using WinAhnenNew.ViewModels;

namespace WinAhnenNew.Views
{
    /// <summary>
    /// Interaktionslogik für EditPageView.xaml
    /// </summary>
    public partial class EditPageView : Page
    {
        public EditPageView()
        {
            InitializeComponent();
            DataContext = ((App)Application.Current).Services.GetRequiredService<EditPageViewModel>();
        }

        public void CommitPendingEdits()
        {
            UpdateBindingSources(this);

            if (DataContext is EditPageViewModel vmEdit)
            {
                vmEdit.PersistSelectedPersonChanges();
            }
        }

        private static void UpdateBindingSources(DependencyObject objRoot)
        {
            UpdateBinding(objRoot, TextBox.TextProperty);
            UpdateBinding(objRoot, ComboBox.TextProperty);
            UpdateBinding(objRoot, ComboBox.SelectedItemProperty);
            UpdateBinding(objRoot, ToggleButton.IsCheckedProperty);

            var iChildren = VisualTreeHelper.GetChildrenCount(objRoot);
            for (var iIndex = 0; iIndex < iChildren; iIndex++)
            {
                UpdateBindingSources(VisualTreeHelper.GetChild(objRoot, iIndex));
            }
        }

        private static void UpdateBinding(DependencyObject objTarget, DependencyProperty dpProperty)
        {
            BindingOperations.GetBindingExpression(objTarget, dpProperty)?.UpdateSource();
        }
    }
}
