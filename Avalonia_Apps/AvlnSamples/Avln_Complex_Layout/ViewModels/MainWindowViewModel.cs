using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Avalonia.ViewModels;

namespace Avln.ComplexLayout.ViewModels;

public partial class MainWindowViewModel : BaseViewModelCT
{
 [ObservableProperty]
 private ObservableObject? _currentViewModel;

 public MainWindowViewModel()
 {
 CurrentViewModel = new ComplexLayoutViewModel();
 }
}
