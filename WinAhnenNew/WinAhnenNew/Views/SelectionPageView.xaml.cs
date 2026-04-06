using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using WinAhnenNew.ViewModels;

namespace WinAhnenNew.Views
{
    /// <summary>
    /// Interaction logic for SelectionPageView.xaml.
    /// </summary>
    public partial class SelectionPageView : Page
    {
        public SelectionPageView()
        {
            InitializeComponent();
            DataContext = ((App)Application.Current).Services.GetRequiredService<SelectionPageViewModel>();
        }
    }
}
