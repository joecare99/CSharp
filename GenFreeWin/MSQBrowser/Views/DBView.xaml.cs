using MSQBrowser.ViewModels;
using Microsoft.Win32;
using System.Windows;
using System;
using System.Windows.Controls;
using CommonDialogs.Interfaces;
using MVVM.View.Extension;
using System.Security;

namespace MSQBrowser.Views
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

        public void Page_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            var vm = (DBViewViewModel)DataContext;
            vm.FileOpenDialog = DoFileDialog;
            vm.FileSaveAsDialog = DoFileDialog;
            vm.DBConnectDialog = DoDBConDialog;
        }

        private bool? DoDBConDialog(string[] Params,SecureString p, Action<string[], SecureString>? OnAccept)
        {
            IDialogWindow dialog = IoC.GetRequiredService<IDialogWindow>();
            var dialogViewModel = ((DBConnectViewModel)dialog.DataContext);
            (dialogViewModel.Server, 
                dialogViewModel.User,
                dialogViewModel.Password,
                dialogViewModel.Db) = (Params[0], Params[1],p, Params[2]);
            if (dialog.ShowDialog() == true)
            {
                OnAccept?.Invoke(new string[] { dialogViewModel.Server, dialogViewModel.User, dialogViewModel.Db }, dialogViewModel.Password);
                return true;
            }
            else
                return false;
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
            bool? result = Par.ShowDialog(this.Parent);
            if (result ?? false) OnAccept?.Invoke(Par.FileName, Par);
            return result;
        }



    }
}
