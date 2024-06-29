using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using MVVM_25_RichTextEdit.ViewModels;

namespace MVVM_25_RichTextEdit.Views
{
    /// <summary>
    /// Interaktionslogik für RichTextEditView.xaml
    /// </summary>
    public partial class RichTextEditView : Page
    {
        public RichTextEditView()
        {
            InitializeComponent();
        }

        private void rtf_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is RichTextBox richTextBox2 && DataContext is RichTextEditViewModel vm)
            {
                vm.Document = XamlWriter.Save(richTextBox2.Document);
            }
        }
    }
}
