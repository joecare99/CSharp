using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace AA14_ScreenX.Views;

public partial class ScreenXView : UserControl
{
 public ScreenXView()
 {
 InitializeComponent();
 }

 private void InitializeComponent()
 {
 AvaloniaXamlLoader.Load(this);
 }
}
