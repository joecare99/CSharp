using MSQBrowser.ViewModels;
using System;
using System.Windows;

namespace MSQBrowser.Views
{
    /// <summary>
    /// Interaktionslogik für DBConnect.xaml
    /// </summary>
    public partial class DBConnect : Window, IDialogWindow
    {
        public DBConnect()
        {
            InitializeComponent();
        }

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);
            var vm = (DBConnectViewModel)DataContext;
            vm.OnAccept = OnVMAccept;
        }

        private void OnVMAccept(object? obj)
        {
            DialogResult = true;
            Close();
        }
    }
}
