using BaseLib.Interfaces;
using BaseLib.Models.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using ConsoleDisplay.View;
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

    public Display display1 { get; set; }
    public Display display2 { get; set; }
    public Display display3 { get; set; }

    public IConsole? console { get; set; }

    public TextConsoleDemoViewModel(IWFCModel model,IRandom random)
    {
        _model = model;
        _random = random;
    }

    [RelayCommand]
    private void DoHello()
    {
        display1.Clear();
        display2.Clear();
        display3.Clear();
        console.Clear();
        console.ForegroundColor = ConsoleColor.White;
        console.SetCursorPosition(20, 12);
        console.WriteLine("Hello World");
    }

    [RelayCommand]
    private void DoShowImages()
    {
        console.Clear();
        _model.console = console;
        _model.D1SBuffer = () => display1.ScreenBuffer;
        _model.D2SBuffer = () => display2.ScreenBuffer;
        _model.D3SBuffer = () => display3.ScreenBuffer;
        _model.D1PutPixel = (x, y, c) => display1.PutPixel(x, y, c);
        _model.D1PutPixelC = (x, y, r, g, b) => display1.PutPixel(x, y, r, g, b);
        _model.D2PutPixelC = (x, y, r, g, b) => display2.PutPixel(x, y, r, g, b);
        _model.DUpdate = () => { display1.Update(); display2.Update(); display3.Update(); };
        _model.LoadImage = (filename) => Bitmap.FromFile(filename) as Bitmap;
        _model.ShowImage1("Resources\\GreenDot.png");
        _model.ShowImage2("Resources\\Joe_Care_n_h64.jpg");
    }

    [RelayCommand]
    private void DoAnalyseImage()
    {
        console.Clear();
        _model.console = console;
        _model.D1SBuffer = () => display1.ScreenBuffer;
        _model.D2SBuffer = () => display2.ScreenBuffer;
        _model.D3SBuffer = () => display3.ScreenBuffer;
        _model.D1PutPixel = (x, y, c) => display1.PutPixel(x, y, c);
        _model.D1PutPixelC = (x, y, r, g, b) => display1.PutPixel(x, y, r, g, b);
        _model.D2PutPixelC = (x, y, r, g, b) => display2.PutPixel(x, y, r, g, b);
        _model.DUpdate = () => { display1.Update(); display2.Update(); display3.Update(); };
        _model.LoadImage = (filename) => Bitmap.FromFile(filename) as Bitmap;
        _model.AnalyseImage("Resources\\GreenDot.png");
    }

    [RelayCommand]
    private void DoDisplayTest()
    {
        _model.console = console;
        _model.D1SBuffer = () => display1.ScreenBuffer;
        _model.D2SBuffer = () => display2.ScreenBuffer;
        _model.D3SBuffer = () => display3.ScreenBuffer;
        _model.D1PutPixel = (x, y, c) => display1.PutPixel(x, y, c);
        _model.D1PutPixelC = (x, y, r, g, b) => display1.PutPixel(x, y, r, g, b);
        _model.D2PutPixelC = (x, y, r, g, b) => display2.PutPixel(x, y, r, g, b);
        _model.DUpdate = () => { display1.Update(); display2.Update(); display3.Update(); };
        _model.LoadImage = (filename) => Bitmap.FromFile(filename) as Bitmap;
        _model.DisplayTest1(_random);
        _model.DisplayTest2();
        _model.DisplayTest3(_random);
    }
}
