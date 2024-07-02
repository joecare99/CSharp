using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using CommonDialogs.Interfaces;
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
            Loaded += Page_Loaded;
        }

        /// <summary>
        /// Handles the Loaded event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        public void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var vm = (RichTextEditViewModel)DataContext;
            vm.FileOpenDialog = DoFileDialog;
            vm.FileSaveAsDialog = DoFileDialog;
            vm.dPrintDialog = DoPrintDialog;
            vm.CloseApp = DoClose;
        }

        /// <summary>
        /// Does the file dialog.
        /// </summary>
        /// <param name="Filename">The filename.</param>
        /// <param name="Par">The par.</param>
        /// <param name="OnAccept">The on accept.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private bool? DoFileDialog(string Filename, IFileDialog Par, Action<string, IFileDialog>? OnAccept)
        {
            Par.FileName = Filename;
            bool? result = Par.ShowDialog(this.Parent as Window);
            if (result ?? false) OnAccept?.Invoke(Par.FileName, Par);
            return result;
        }

        /// <summary>
        /// Does the print dialog.
        /// </summary>
        /// <param name="par">The par.</param>
        /// <param name="OnPrint">The on print.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private bool? DoPrintDialog(IPrintDialog par, Action<IPrintDialog,DocumentPaginator>? OnPrint)
        {
            bool? result = par.ShowDialog();
            if (result ?? false) OnPrint?.Invoke(par, ((IDocumentPaginatorSource)rtf.Document).DocumentPaginator);
            return result;
        }

        private void DoClose()
        {
            Application.Current.Shutdown();
        }

    }
}
