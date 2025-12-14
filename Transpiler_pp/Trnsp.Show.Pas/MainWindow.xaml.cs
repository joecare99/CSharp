using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Trnsp.Show.Pas;
using Trnsp.Show.Pas.ViewModels;

namespace Transp.Show.Pas
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = App.Current.Services.GetService<MainViewModel>();
        }
    }
}