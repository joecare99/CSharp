using System.Windows.Controls;
using WinAhnenNew.ViewModels;

namespace WinAhnenNew.Views
{
    /// <summary>
    /// Interaction logic for ImagesPageView.xaml.
    /// </summary>
    public partial class ImagesPageView : Page
    {
        public ImagesPageView()
        {
            InitializeComponent();
            DataContext = new ImagesPageViewModel();
        }
    }
}
