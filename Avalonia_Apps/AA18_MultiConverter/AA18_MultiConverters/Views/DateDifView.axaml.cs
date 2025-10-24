using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace AA18_MultiConverter.Views;

public partial class DateDifView : UserControl
{
 public DateDifView()
 {
 InitializeComponent();
 DataContext = AA18_MultiConverter.App.Services.GetRequiredService<ViewModels.DateDifViewModel>();
 }
}
