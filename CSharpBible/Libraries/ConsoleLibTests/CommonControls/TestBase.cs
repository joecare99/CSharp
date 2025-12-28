using BaseLib.Interfaces;
using ConsoleLib;
using ConsoleLib.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.Reflection;
using TestConsole;

namespace ConsoleLib.CommonControls.Tests;

public class TestBase
{
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Fügen Sie ggf. den „erforderlichen“ Modifizierer hinzu, oder deklarieren Sie den Modifizierer als NULL-Werte zulassend.
    protected static TstConsole _tstCon;
    private IConsole _oldCon;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Fügen Sie ggf. den „erforderlichen“ Modifizierer hinzu, oder deklarieren Sie den Modifizierer als NULL-Werte zulassend.

    [TestInitialize]
    public void BaseInit()
    {
        _tstCon ??= new TstConsole(){WindowWidth=120,WindowHeight=40,ForegroundColor=ConsoleColor.Gray,BackgroundColor=ConsoleColor.Black};
        _oldCon = ConsoleFramework.console;
        ConsoleFramework.console = _tstCon;
        // adjust canvas dimension via reflection (Rectangle struct -> assign back)
        var canvas = ConsoleFramework.Canvas;
        var dimField = canvas.GetType().GetField("_dimension", BindingFlags.Instance | BindingFlags.NonPublic);
        if (dimField != null)
        {
            var rect = (Rectangle)dimField.GetValue(canvas)!;
            rect.Width = _tstCon.WindowWidth;
            rect.Height = Math.Min(50,_tstCon.WindowHeight);
            dimField.SetValue(canvas, rect);
        }
        ConsoleLib.Control.MessageQueue = new ConcurrentQueue<(Action<object,EventArgs>,object,EventArgs)>();
    }

    [TestCleanup]
    public void BaseCleanup()
    {
        ConsoleFramework.console = _oldCon;
    }
}
