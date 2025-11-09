using System.Collections.Generic;
using System.Collections.ObjectModel;
using IntegrationTestApp.Models;

namespace IntegrationTestApp.ViewModels;

internal class MainWindowViewModel : ViewModelBase
{
    private Page? _selectedPage;

    public MainWindowViewModel(IEnumerable<Page> pages)
    {
        Pages = new(pages);
    }

    public ObservableCollection<Page> Pages { get; }
    
    public Page? SelectedPage
    { 
        get => _selectedPage;
        set
        {
            if (!EqualityComparer<Page?>.Default.Equals(_selectedPage, value))
            {
                _selectedPage = value;
                OnPropertyChanged();
            }
        }
    }
}
