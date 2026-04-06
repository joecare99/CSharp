using System.Windows.Controls;
using WinAhnenNew.ViewModels;

namespace WinAhnenNew.Views
{
    /// <summary>
    /// Interaktionslogik für DetailPageView.xaml
    /// </summary>
    public partial class DetailPageView : Page
    {
        public DetailPageView()
        {
            InitializeComponent();
            DataContext = new DetailPageViewModel();
        }
    }
}
