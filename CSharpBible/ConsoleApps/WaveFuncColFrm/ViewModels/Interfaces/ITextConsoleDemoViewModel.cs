using BaseLib.Interfaces;
using CommunityToolkit.Mvvm.Input;
using System.ComponentModel;
using ConsoleDisplay.View;

namespace TestConsoleDemo.ViewModels.Interfaces;

public interface ITextConsoleDemoViewModel : INotifyPropertyChanged
{
    IConsole? console { get; set; }
    IRelayCommand DoHelloCommand { get; }
    IRelayCommand DoShowImagesCommand { get; }
    IRelayCommand DoAnalyseImageCommand { get; }
    IRelayCommand DoDisplayTestCommand { get; }

    Display display1 { get; set; }
    Display display2 { get; set; }
    Display display3 { get; set; }

}
