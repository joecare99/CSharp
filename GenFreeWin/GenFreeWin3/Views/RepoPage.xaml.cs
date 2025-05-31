using System.Windows.Controls;
using System.Windows.Navigation;
using GenFree.ViewModels.Interfaces;

namespace GenFreeWpf.Views;

public partial class RepoPage : Page
{
    public IRepoViewModel RepoViewModel
    {
        get => (IRepoViewModel)DataContext;
        set => DataContext = value;
    }

    public RepoPage(IRepoViewModel viewModel)
    {
        InitializeComponent();
        RepoViewModel = viewModel;
        RepoViewModel.DoClose = () => ClosePage();
    }

    private void ClosePage()
    {
        // NavigationService kann null sein, wenn die Seite nicht in einem Frame ist
        NavigationService?.GoBack();
    }
}