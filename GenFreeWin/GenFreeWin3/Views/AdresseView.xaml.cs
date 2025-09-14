using System.Windows.Controls;
using GenFree.ViewModels.Interfaces;

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
