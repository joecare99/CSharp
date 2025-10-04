using System.Windows;
using GenFreeBrowser.ViewModels;
using GenFreeBrowser.ViewModels.Interfaces;

namespace GenFreeBrowser.ViewHost;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        // Simple manual VM wiring for quick design host
        DataContext = new DummyHostViewModel();
    }
}

public class DummyHostViewModel
{
    public IPersonenListViewModel PersonenVM { get; }
    public KernFamilieViewModel KernFamilieVM { get; }

    public DummyHostViewModel()
    {
        var service = new PersonenService();
        PersonenVM = new FraPersonenListViewModel(service);
        KernFamilieVM = new KernFamilieViewModel();
        _ = PersonenVM.LadeAsync();
    }
}
