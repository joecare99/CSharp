using GenFree.ViewModels.Interfaces;
using System.Windows.Controls;

namespace GenFreeWpf.Views;

public partial class AdresseView : Page
{
    public AdresseView(IAdresseViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        if (viewModel.FormLoadCommand.CanExecute(null))
            viewModel.FormLoadCommand.Execute(null);
    }
}
