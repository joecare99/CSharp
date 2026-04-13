using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
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

        private void Page_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter || Keyboard.Modifiers != ModifierKeys.None)
            {
                return;
            }

            if (!TryCommitSingleLineInput(e.OriginalSource as DependencyObject))
            {
                return;
            }

            e.Handled = true;
        }

        private static bool TryCommitSingleLineInput(DependencyObject? objSource)
        {
            if (objSource is null)
            {
                return false;
            }

            var cboComboBox = FindAncestor<ComboBox>(objSource);
            if (cboComboBox is not null && cboComboBox.IsEditable)
            {
                UpdateBinding(cboComboBox, ComboBox.TextProperty);
                return true;
            }

            if (FindAncestor<TextBox>(objSource) is not TextBox txtTextBox || txtTextBox.AcceptsReturn)
            {
                return false;
            }

            UpdateBinding(txtTextBox, TextBox.TextProperty);
            return true;
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

        private static T? FindAncestor<T>(DependencyObject? objChild)
            where T : DependencyObject
        {
            while (objChild is not null)
            {
                if (objChild is T objTarget)
                {
                    return objTarget;
                }

                objChild = VisualTreeHelper.GetParent(objChild);
            }

            return null;
        }
    }
}
