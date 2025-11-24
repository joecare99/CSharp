using AA18_MultiConverter.ViewModels.Interfaces;
using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace AA18_MultiConverter.Views;

public partial class DateDifView : UserControl
{
    public DateDifView()
    {
        InitializeComponent();
        DataContext = App.Services.GetRequiredService<IDateDifViewModel>();
    }
}
