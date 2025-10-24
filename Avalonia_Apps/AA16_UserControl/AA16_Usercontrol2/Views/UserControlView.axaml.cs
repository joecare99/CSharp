using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace AA16_UserControl2.Views;

public partial class UserControlView : UserControl
{
 public UserControlView()
 {
 InitializeComponent();
 DataContext = App.Services.GetRequiredService<ViewModels.UserControlViewModel>();
 }
}
