using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using WinAhnenNew.ViewModels;

namespace WinAhnenNew.Views
{
    /// <summary>
    /// Interaction logic for TextPageView.xaml.
    /// </summary>
    public partial class TextPageView : Page
    {
        public TextPageView()
        {
            InitializeComponent();
            DataContext = ((App)System.Windows.Application.Current).Services.GetRequiredService<TextPageViewModel>();
        }
    }
}
