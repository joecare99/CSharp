using BaseLib.Interfaces;
using BaseLib.Models.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DisplayTest.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestConsoleDemo.ViewModels.Interfaces;

namespace TestConsoleDemo.ViewModels;

public partial class TextConsoleDemoViewModel : ObservableObject, ITextConsoleDemoViewModel
{
    private IDisplayTest _model;
    private IRandom _random;

    public IConsole? console { get; set; }

    public TextConsoleDemoViewModel(IDisplayTest model,IRandom random)
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
    private void DoLongText()
    {
        console.Clear();
        var rnd = new Random();
        var newPara = true;
        for (int i = 0; i < 3000; i++)
        {
            var word = "";
            for (int j = 0; j < rnd.Next(3, 14); j++)
            {
                word += (j == 0) && (rnd.Next(5) == 0 || newPara) ? (char)rnd.Next(65, 91) : (char)rnd.Next(97, 123);
            }
            newPara = false;
            if (console.GetCursorPosition().Left + word.Length + 2 > console.WindowWidth)
                console.WriteLine();
            console.Write(word);
            switch (rnd.Next(8))
            {
                case 0: console.Write(". "); newPara = true; break;
                case 1: console.Write(", "); break;
                default:
                    console.Write(" "); break;
            }
            Thread.Sleep(0);
        }
    }

    [RelayCommand]
    private void DoColorText()
    {
        console.Clear();
        var rnd = new Random();
        var newPara = true;
        for (int i = 0; i < 3000; i++)
        {
            var word = "";
            for (int j = 0; j < rnd.Next(3, 14); j++)
            {
                word += (j == 0) && (rnd.Next(5) == 0 || newPara) ? (char)rnd.Next(65, 91) : (char)rnd.Next(97, 123);
            }
            newPara = false;
            if (console.GetCursorPosition().Left + word.Length + 2 > console.WindowWidth)
                console.WriteLine();
            console.ForegroundColor = (ConsoleColor)rnd.Next(8, 16);
            console.BackgroundColor = (ConsoleColor)rnd.Next(0, 8);
            console.Write(word);
            switch (rnd.Next(8))
            {
                case 0: console.Write(". "); newPara = true; break;
                case 1: console.Write(", "); break;
                default:
                    console.Write(" "); break;
            }
            Thread.Sleep(0);
        }
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
