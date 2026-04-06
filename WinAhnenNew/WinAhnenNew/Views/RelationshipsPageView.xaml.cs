using System.Windows.Controls;
using WinAhnenNew.ViewModels;

namespace WinAhnenNew.Views
{
    /// <summary>
    /// Interaction logic for RelationshipsPageView.xaml.
    /// </summary>
    public partial class RelationshipsPageView : Page
    {
        public RelationshipsPageView()
        {
            InitializeComponent();
            DataContext = new RelationshipsPageViewModel();
        }
    }
}
