using System.Collections.ObjectModel;
using CSharpBible.CharGrid.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CSharpBible.CharGrid.ViewModels.Interfaces;
using System.Windows.Forms;

namespace CSharpBible.CharGrid.ViewModels;

public partial class CharGridViewModel : ViewModelBase, ICharGridViewModel
{
    private readonly ICharGridProvider _provider;

    [ObservableProperty]
    private ObservableCollection<ObservableCollection<char>> rows = new();

    public int RowCount => _provider.Rows;
    public int ColumnCount => _provider.Columns;

    public CharGridViewModel(ICharGridProvider provider)
    {
        _provider = provider;
        for (int r = 0; r < _provider.Rows; r++)
        {
            var row = new ObservableCollection<char>();
            for (int c = 0; c < _provider.Columns; c++)
                row.Add(_provider.GetChar(r, c));
            rows.Add(row);
        }
    }

    [RelayCommand]
    private void UpdateCell((int r, int c, char value) args)
    {
        var (r, c, value) = args;
        _provider.SetChar(r, c, value);
        rows[r][c] = value;
        // ObservableProperty will raise PropertyChanged for Rows when set; direct item change triggers collection notifications
    }

    [RelayCommand]
    private void Exit()
    {
        Application.Exit();
    }

    [RelayCommand]
    private void About()
    {
       
    }
}
