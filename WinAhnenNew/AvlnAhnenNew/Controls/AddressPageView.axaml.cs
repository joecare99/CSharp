using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using WinAhnenNew.ViewModels;

namespace AvlnAhnenNew.Controls
{
    public partial class AddressPageView : UserControl
    {
        public AddressPageView()
        {
            InitializeComponent();
            DataContext = ((App)Avalonia.Application.Current!).Services.GetRequiredService<AddressPageViewModel>();
        }
    }
}
