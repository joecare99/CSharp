using Avalonia.Controls;

namespace AA18_MultiConverter;

public partial class MainWindow : Window
{
 public MainWindow(View.DateDifView view)
 {
 InitializeComponent();
 Content = view;
 }
}
