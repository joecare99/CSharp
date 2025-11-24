using CommunityToolkit.Mvvm.Input;
using System.ComponentModel;

namespace AA16_UserControl1.ViewModels.Interfaces;

public interface IUserControlViewModel: INotifyPropertyChanged
{
    string Text { get; set; }
    string Daten { get; set; }

    IRelayCommand Do1Command { get; }
    IRelayCommand Do2Command { get; }
}