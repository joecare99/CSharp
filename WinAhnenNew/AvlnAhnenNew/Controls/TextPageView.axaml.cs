using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using WinAhnenNew.ViewModels;

namespace AvlnAhnenNew.Controls
{
    public partial class TextPageView : UserControl
    {
        public TextPageView()
        {
            InitializeComponent();
            DataContext = ((App)Avalonia.Application.Current!).Services.GetRequiredService<TextPageViewModel>();
        }
    }
}
