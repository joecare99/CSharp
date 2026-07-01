using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using WinAhnenNew.ViewModels;

namespace AvlnAhnenNew.Controls
{
    public partial class SiblingsPageView : UserControl
    {
        public SiblingsPageView()
        {
            InitializeComponent();
            DataContext = ((App)Avalonia.Application.Current!).Services.GetRequiredService<SiblingsPageViewModel>();
        }
    }
}
