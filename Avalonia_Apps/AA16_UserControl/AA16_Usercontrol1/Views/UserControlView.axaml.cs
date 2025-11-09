using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using AA16_UserControl1.ViewModels;

namespace AA16_UserControl1.Views;

public partial class UserControlView : UserControl
{
    public UserControlView()
    {
        InitializeComponent();
        DataContext ??= App.Services.GetRequiredService<UserControlViewModel>();
    }
}
