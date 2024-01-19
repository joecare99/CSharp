using MdbBrowser.ViewModels;
using Microsoft.Win32;
using System.Windows;
using System;
using System.Windows.Controls;
using CommonDialogs.Interfaces;

namespace MdbBrowser.Views
{
    /// <summary>
    /// Interaktionslogik für DBView.xaml
    /// </summary>
    public partial class DBView : Page
    {
        public DBView()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            var vm = (DBViewViewModel)DataContext;
            vm.FileOpenDialog = DoFileDialog;
            vm.FileSaveAsDialog = DoFileDialog;
        }

        /// <summary>
        /// Does the file dialog.
        /// </summary>
        /// <param name="Filename">The filename.</param>
        /// <param name="Par">The par.</param>
        /// <param name="OnAccept">The on accept.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool? DoFileDialog(string Filename, IFileDialog Par, Action<string, IFileDialog>? OnAccept)
        {
            Par.FileName = Filename;
            bool? result = Par.ShowDialog(this.Parent as Window);
            if (result ?? false) OnAccept?.Invoke(Par.FileName, Par);
            return result;
        }

    }
}
