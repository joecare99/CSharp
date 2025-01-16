using CommunityToolkit.Mvvm.ComponentModel;
using MVVM_37_TreeView.Models;
using System.Collections.ObjectModel;

namespace MVVM_37_TreeView.ViewModels;

public partial class CategorizedBooksViewModel :ObservableObject
{
    [ObservableProperty]
    private string _category ="";

    [ObservableProperty]
    private Book? _this=null;

    [ObservableProperty]
    private ObservableCollection<CategorizedBooksViewModel> _books = new();
}
