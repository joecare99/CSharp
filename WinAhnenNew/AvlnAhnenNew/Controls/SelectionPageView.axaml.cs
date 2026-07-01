using Avalonia.Controls;
using Avalonia.Input;
using Microsoft.Extensions.DependencyInjection;
using WinAhnenNew.ViewModels;

namespace AvlnAhnenNew.Controls
{
    public partial class SelectionPageView : UserControl
    {
        public SelectionPageView()
        {
            InitializeComponent();
            DataContext = ((App)Avalonia.Application.Current!).Services.GetRequiredService<SelectionPageViewModel>();
        }

        private void PersonsDataGrid_OnDoubleTapped(object? sender, TappedEventArgs e)
        {
            if (DataContext is not SelectionPageViewModel vmSelection)
            {
                return;
            }

            if (!vmSelection.SelectPersonCommand.CanExecute(null))
            {
                return;
            }

            vmSelection.SelectPersonCommand.Execute(null);
            e.Handled = true;
        }
    }
}
