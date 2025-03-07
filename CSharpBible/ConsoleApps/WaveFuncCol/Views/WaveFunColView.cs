﻿using BaseLib.Interfaces;
using BaseLib.Models.Interfaces;
using ConsoleDisplay.View;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using WaveFunCollapse.Models.Interfaces;

namespace WaveFunCollapse.Views;

public class WaveFunColView : IView
{
    /// <summary>
    /// The first display (big)
    /// </summary>
    /// <autogeneratedoc />
    Display display1 = new Display(0, 0, 9, 10);
    /// <summary>
    /// The second display (smaler)
    /// </summary>
    /// <autogeneratedoc />
    Display display2 = new Display(10, 0, 27, 28);
    /// <summary>
    /// The third display (Mostly used as a gauge)
    /// </summary>
    /// <autogeneratedoc />
    Display display3 = new Display(10, 23, 50, 2);
    private IWFCModel _model;
    private IRandom _rnd;

    public WaveFunColView(IWFCModel model,IRandom rnd, IConsole console)
    {
        model.console = console;
        model.D1SBuffer = () => display1.ScreenBuffer;
        model.D2SBuffer = () => display2.ScreenBuffer;
        model.D3SBuffer = () => display3.ScreenBuffer;
        model.D1PutPixel = (x, y, c) => display1.PutPixel(x, y, c);
        model.D1PutPixelC = (x, y, r, g, b) => display1.PutPixel(x, y, r, g, b);
        model.D2PutPixelC = (x, y, r, g, b) => display2.PutPixel(x, y, r, g, b);
        model.DUpdate = () => { display1.Update();display2.Update();display3.Update(); };
        model.LoadImage = (filename) => Bitmap.FromFile(filename) as Bitmap;
        _model = model;
        _rnd = rnd;
    }

    public void Show()
    {
        //      _model.DisplayTest1(_rnd);
        //     _model.DisplayTest2();
           _model.DisplayTest3(_rnd);
        //        _model.ShowImage1("Resources\\GreenDot.png");
        _model.AnalyseImage("Resources\\GreenDot.png");
        _model.console.ReadLine();
  //      _model.ShowImage2("Resources\\Joe_Care_n_h64.jpg");
    }
}
