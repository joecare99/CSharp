using MSQBrowser.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
