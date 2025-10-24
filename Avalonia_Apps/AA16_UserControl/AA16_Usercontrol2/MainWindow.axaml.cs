using Avalonia.Controls;

namespace AA16_UserControl2;

public partial class MainWindow : Window
{
 public MainWindow(Views.UserControlView view)
 {
 InitializeComponent();
 Content = view;
 }
}
