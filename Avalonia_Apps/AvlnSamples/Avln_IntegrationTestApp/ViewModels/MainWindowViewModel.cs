using System.Collections.Generic;
using System.Collections.ObjectModel;
using IntegrationTestApp.Models;

namespace IntegrationTestApp.ViewModels;

internal class MainWindowViewModel : ViewModelBase
{
    private DemoPage? _selectedPage;

    public MainWindowViewModel(IEnumerable<DemoPage> pages)
    {
        Pages = new(pages);
    }

    public ObservableCollection<DemoPage> Pages { get; }
    
    public DemoPage? SelectedPage
    { 
        get => _selectedPage;
        set
        {
            if (!EqualityComparer<DemoPage?>.Default.Equals(_selectedPage, value))
            {
                _selectedPage = value;
                OnPropertyChanged();
            }
        }
    }
}
