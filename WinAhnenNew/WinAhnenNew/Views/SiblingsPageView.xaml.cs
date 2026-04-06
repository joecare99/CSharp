using System.Windows.Controls;
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
            DataContext = new SiblingsPageViewModel();
        }
    }
}
