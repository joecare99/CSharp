using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using WinAhnenNew.ViewModels;

namespace WinAhnenNew.Views
{
    /// <summary>
    /// Interaction logic for frmAhnenWinMain.xaml.
    /// </summary>
    public partial class Form1 : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Form1"/> class.
        /// </summary>
        public Form1()
        {
            InitializeComponent();
            DataContext = ((App)Application.Current).Services.GetRequiredService<FrmAhnenWinMainViewModel>();
        }
    }
}
