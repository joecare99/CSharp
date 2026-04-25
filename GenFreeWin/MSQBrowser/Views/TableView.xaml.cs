using System.Windows.Controls;
using MSQBrowser.ViewModels;

namespace MSQBrowser.Views
{
    /// <summary>
    /// Interaktionslogik für TableView.xaml
    /// </summary>
    public partial class TableView : Page
    {
        public TableView()
        {
            InitializeComponent();
        }

        private async void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            if (DataContext is TableViewViewModel viewModel)
            {
                await viewModel.EnsureVisibleRowsLoadedAsync(e.Row.GetIndex());
            }
        }
    }
}
