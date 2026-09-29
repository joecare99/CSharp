using System;
using System.Windows.Controls;
using BaseLib.Helper;
using VBUnObfusicator.ViewModels;

namespace VBUnObfusicator.Views
{
    /// <summary>
    /// Interaktionslogik für CodeUnObFusView.xaml
    /// </summary>
    public partial class CodeUnObFusView : Page
    {
        public CodeUnObFusView()
        {
            InitializeComponent();
            DataContext = IoC.GetRequiredService<CodeUnObFusViewModel>();
        }

        private void FindingsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is not ListBox { SelectedItem: CodeAnalysisFinding finding })
                return;

            if (DataContext is CodeUnObFusViewModel viewModel)
                viewModel.SelectedOutputTabIndex = 0;
            NavigateToRange(OriginalCodeTextBox, finding.OriginalStart, finding.OriginalLength);
            NavigateToRange(GeneratedCodeTextBox, finding.OutputStart, finding.OutputLength);
            OriginalCodeTextBox.Focus();
        }

        private static void NavigateToRange(TextBox textBox, int? start, int? length)
        {
            if (start is not int position || length is not int spanLength || position < 0 || position > textBox.Text.Length)
                return;

            var selectionLength = Math.Min(spanLength, textBox.Text.Length - position);
            textBox.Focus();
            textBox.Select(position, selectionLength);
            var line = textBox.GetLineIndexFromCharacterIndex(position);
            if (line >= 0)
                textBox.ScrollToLine(line);
        }
    }
}
