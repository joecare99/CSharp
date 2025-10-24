using CommunityToolkit.Mvvm.ComponentModel;

namespace AA16_UserControl2.ViewModels;

public partial class UserControlViewModel : ObservableObject
{
	[ObservableProperty]
	private string text = "";
}
