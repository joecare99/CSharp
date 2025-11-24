using AA16_UserControl1.ViewModels.Interfaces;
using Avalonia.Views.Extension;
using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;

namespace AA16_UserControl1.ViewModels;

public partial class MainWindowViewModel : ViewModelBase, IMainWindowViewModel
{
    public MainWindowViewModel()
        : this(IoC.GetRequiredService<IUserControlViewModel>())
    {
    }

    public MainWindowViewModel(IUserControlViewModel content)
    {
        CurrentViewModel = content;
    }

    public INotifyPropertyChanged CurrentViewModel { get; }
}
