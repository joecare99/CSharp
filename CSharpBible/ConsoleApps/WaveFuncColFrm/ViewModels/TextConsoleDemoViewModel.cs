using BaseLib.Interfaces;
using BaseLib.Models.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestConsoleDemo.ViewModels.Interfaces;
using WaveFunCollapse.Models.Interfaces;

namespace TestConsoleDemo.ViewModels;

public partial class TextConsoleDemoViewModel : ObservableObject, ITextConsoleDemoViewModel
{
    private IWFCModel _model;
    private IRandom _random;

    public IConsole? console { get; set; }

    public TextConsoleDemoViewModel(IWFCModel model,IRandom random)
    {
        _model = model;
        _random = random;
    }

    [RelayCommand]
    private void DoHello()
    {
        console.Clear();
        console.ForegroundColor = ConsoleColor.White;
        console.SetCursorPosition(20, 12);
        console.WriteLine("Hello World");
    }

    [RelayCommand]
    private void DoShowImages()
    {
        console.Clear();
        _model.ShowImage1("Resources\\GreenDot.png");
        _model.ShowImage2("Resources\\Joe_Care_n_h64.jpg");
    }

    [RelayCommand]
    private void DoAnalyseImage()
    {
        console.Clear();
        _model.AnalyseImage("Resources\\GreenDot.png");
    }

    [RelayCommand]
    private void DoDisplayTest()
    {
        _model.console = console;
        _model.DisplayTest1(_random);
        _model.DisplayTest2();
        _model.DisplayTest3(_random);
    }
}
