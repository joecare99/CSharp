using System.ComponentModel;

namespace AA16_UserControl1.ViewModels.Interfaces
{
    public interface IMainWindowViewModel: INotifyPropertyChanged
    {
        INotifyPropertyChanged CurrentViewModel { get; }
    }
}