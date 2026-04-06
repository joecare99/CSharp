using System.Windows.Controls;
using WinAhnenNew.ViewModels;

namespace WinAhnenNew.Views
{
    /// <summary>
    /// Interaction logic for AdditionalPageView.xaml.
    /// </summary>
    public partial class AdditionalPageView : Page
    {
        public AdditionalPageView()
        {
            InitializeComponent();
            DataContext = new AdditionalPageViewModel();
        }
    }
}
