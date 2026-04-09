using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using WinAhnenNew.ViewModels;

namespace WinAhnenNew.Views
{
    /// <summary>
    /// Interaction logic for SelectionPageView.xaml.
    /// </summary>
    public partial class SelectionPageView : Page
    {
        public SelectionPageView()
        {
            InitializeComponent();
            DataContext = ((App)Application.Current).Services.GetRequiredService<SelectionPageViewModel>();
        }

        private void PersonsDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
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
