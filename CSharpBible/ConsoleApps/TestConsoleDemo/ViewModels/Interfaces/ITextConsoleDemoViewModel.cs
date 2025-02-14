using BaseLib.Interfaces;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestConsoleDemo.ViewModels.Interfaces;

public interface ITextConsoleDemoViewModel : INotifyPropertyChanged
{
    IConsole? console { get; set; }
    IRelayCommand DoHelloCommand { get; }
    IRelayCommand DoLongTextCommand { get; }
    IRelayCommand DoColorTextCommand { get; }
    IRelayCommand DoDisplayTestCommand { get; }

}
