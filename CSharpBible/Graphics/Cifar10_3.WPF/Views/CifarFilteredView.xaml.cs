using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Cifar10.WPF.ViewModels;

namespace Cifar10.WPF.Views
{
    /// <summary>
    /// Interaktionslogik für CifarFilteredView.xaml
    /// </summary>
    public partial class CifarFilteredView : Page
    {
        public CifarFilteredView()
        {
            InitializeComponent();
            var vm = new CifarFilteredViewModel();
            vm.showfiledlg = (dlg) => Application.Current.Dispatcher.Invoke(() =>
            {
                return dlg.ShowDialog(this.Parent) == true;
            });
            DataContext = vm;
        }
    }
}
