using CommunityToolkit.Mvvm.Input;
using System.Collections;
using System.ComponentModel;
using System.Windows.Input;

public interface IGameViewModel: INotifyPropertyChanged
{
    IRelayCommand StartCommand { get; }
    IRelayCommand SuggestCommand { get; }
    IRelayCommand AccuseCommand { get; }
    IRelayCommand NextCommand { get; }
    IRelayCommand HelpCommand { get; }
    IList Players { get; }
    IList History { get; }
    string CurrentTitle { get; set; }
    Action DisplayHelp { get; set; }
}