using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using WinAhnenNew.ViewModels;

namespace WinAhnenNew.Views
{
    /// <summary>
    /// Interaction logic for SiblingsPageView.xaml.
    /// </summary>
    public partial class SiblingsPageView : Page
    {
        public SiblingsPageView()
        {
            InitializeComponent();
            DataContext = ((App)System.Windows.Application.Current).Services.GetRequiredService<SiblingsPageViewModel>();
        }
    }
}
