using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using WinAhnenNew.ViewModels;

namespace AvlnAhnenNew.Controls
{
    public partial class DetailPageView : UserControl
    {
        public DetailPageView()
        {
            InitializeComponent();
            DataContext = ((App)Avalonia.Application.Current!).Services.GetRequiredService<DetailPageViewModel>();
        }
    }
}
