using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
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
            DataContext = ((App)System.Windows.Application.Current).Services.GetRequiredService<ImagesPageViewModel>();
        }
    }
}
