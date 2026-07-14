using Microsoft.Extensions.DependencyInjection;
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
            DataContext = ((App)System.Windows.Application.Current).Services.GetRequiredService<RelationshipsPageViewModel>();
        }
    }
}
