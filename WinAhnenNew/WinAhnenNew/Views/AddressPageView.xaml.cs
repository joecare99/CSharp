using System.Windows.Controls;
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
            DataContext = new AddressPageViewModel();
        }
    }
}
