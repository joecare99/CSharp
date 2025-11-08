using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace CSharpBible.CharGrid.ViewModels.Interfaces;

public interface ICharGridViewModel
{
    ObservableCollection<ObservableCollection<char>> Rows { get; }

    IRelayCommand ExitCommand { get; }
    IRelayCommand AboutCommand { get; }
    IRelayCommand<(int r, int c, char value)> UpdateCellCommand { get; }
    int ColumnCount { get; }
    int RowCount { get; }
}