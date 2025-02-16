using BaseLib.Interfaces;
using CommunityToolkit.Mvvm.Input;
using System.ComponentModel;

namespace TestConsoleDemo.ViewModels.Interfaces;

public interface ITextConsoleDemoViewModel : INotifyPropertyChanged
{
    IConsole? console { get; set; }
    IRelayCommand DoHelloCommand { get; }
    IRelayCommand DoShowImagesCommand { get; }
    IRelayCommand DoAnalyseImageCommand { get; }
    IRelayCommand DoDisplayTestCommand { get; }

}
