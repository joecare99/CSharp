using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using AA16_UserControl1.ViewModels;
using AA16_UserControl1.ViewModels.Interfaces;
using Avalonia.Views.Extension;

namespace AA16_UserControl1.Views;

public partial class UserControlView : UserControl
{
    public UserControlView()
    {
        InitializeComponent();
        DataContext ??= IoC.GetRequiredService<IUserControlViewModel>();
    }
}
