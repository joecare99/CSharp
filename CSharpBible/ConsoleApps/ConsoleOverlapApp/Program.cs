// ***********************************************************************
// Assembly         : ConsoleOverlapApp
// Author           : AI Assistant
// Created          : 2025-09-27
// ***********************************************************************
using System;
using System.Drawing;
using System.Threading;
using ConsoleLib;
using ConsoleLib.CommonControls;
using ConsoleLib.Interfaces;
using BaseLib.Interfaces;
using BaseLib.Models;

namespace ConsoleOverlapApp;

class Program
{
    private static Application? _app;
    private static Panel? _panelA;
    private static Panel? _panelB;
    private static Button? _btnA;
    private static Button? _btnB;
    private static Timer? _timer;
    private static int _dxA = 1, _dyA = 0;
    private static int _dxB = -1, _dyB = 0;
    private static int _tick;

    static void Main(string[] _)
    {
        Init();
        _app?.Run();
        ConsoleFramework.ExtendedConsole?.Stop();
    }

    private static void Init()
    {
        IExtendedConsole ext = new ExtendedConsole();
        IConsole con = new ConsoleProxy();
        _app = new Application(con, ext)
        {
            BackColor = ConsoleColor.DarkBlue,
            ForeColor = ConsoleColor.Gray,
            Border = ConsoleFramework.singleBorder,
            BoarderColor = ConsoleColor.Green
        };
        var cl = ConsoleFramework.Canvas.ClipRect;
        cl.Inflate(-2,-2);
        _app.Dimension = cl;

        _panelA = new Panel
        {
            Parent = _app,
            Dimension = new Rectangle(5,5,20,5),
            Border = ConsoleFramework.singleBorder,
            BackColor = ConsoleColor.DarkGray,
            ForeColor = ConsoleColor.White,
            BoarderColor = ConsoleColor.Yellow,
            Shadow = true
        };
        _btnA = new Button
        {
            Parent = _panelA,
            Position = new Point(2,2),
            Text = "A",
            BackColor = ConsoleColor.Blue,
            ForeColor = ConsoleColor.White
        };

        _panelB = new Panel
        {
            Parent = _app,
            Dimension = new Rectangle(25,5,20,5),
            Border = ConsoleFramework.singleBorder,
            BackColor = ConsoleColor.DarkGray,
            ForeColor = ConsoleColor.White,
            BoarderColor = ConsoleColor.Cyan,
            Shadow = true
        };
        _btnB = new Button
        {
            Parent = _panelB,
            Position = new Point(2,2),
            Text = "B",
            BackColor = ConsoleColor.DarkRed,
            ForeColor = ConsoleColor.White
        };

        _app.OnCanvasResize += (_,p)=> { var r = ConsoleFramework.Canvas.ClipRect; r.Inflate(-2,-2); _app.Dimension = r; };

        // movement timer (approx 5 fps)
        _timer = new Timer(MoveTick, null, 200, 200);
        _app.Visible = true;
        _app.Draw();
    }

    private static void MoveTick(object? _)
    {
        if (_app == null || _panelA == null || _panelB == null) return;
        // change direction slowly
        if (_panelA.Position.X + _dxA < 1 || _panelA.Position.X + _panelA.size.Width + _dxA > _app.size.Width-1)
        {
            // reverse A
            _dxA = -_dxA;
        }
        if (_dxA==0) _dxA = 1;
        if (_panelB.Position.X + _dxB < 1 || _panelB.Position.X + _panelB.size.Width + _dxB > _app.size.Width-1)
        {
            // reverse B
            _dxB = -_dxB;
        }
        if (_dxB==0) _dxB = -1;
        
        // move panels horizontally creating overlap
        Shift(_panelA, _dxA, _dyA);
        Shift(_panelB, _dxB, _dyB);
        // force redraw: invalidate both panels & app
    }

    private static void Shift(Panel p, int dx, int dy)
    {
        var d = p.Dimension;
        d.Offset(dx, dy);
        // wrap inside app client rect a bit
        var bounds = _app!.Dimension;
        if (d.Right > bounds.Width) d.X = bounds.Width - d.Width - 1;
        if (d.Left < 1) d.X = 1;
        p.Dimension = d;
    }
}
