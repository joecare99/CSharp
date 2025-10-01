using BaseLib.Interfaces;
using CommunityToolkit.Mvvm.Input;
using System.ComponentModel;

namespace TestConsoleDemo.ViewModels.Interfaces;

public interface ITextConsoleDemoViewModel : INotifyPropertyChanged
{
    IConsole? console { get; set; }
    IRelayCommand DoHelloCommand { get; }
    IRelayCommand DoLongTextCommand { get; }
    IRelayCommand DoColorTextCommand { get; }
    IRelayCommand DoDisplayTestCommand { get; }
    string Title { get; set; }
}
