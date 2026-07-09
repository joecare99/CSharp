using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using WinAhnenNew.ViewModels;

namespace WinAhnenNew.Views
{
    /// <summary>
    /// Interaction logic for AddressPageView.xaml.
    /// </summary>
    public partial class AddressPageView : Page
    {
        public AddressPageView()
        {
            InitializeComponent();
            DataContext = ((App)System.Windows.Application.Current).Services.GetRequiredService<AddressPageViewModel>();
        }
    }
}
