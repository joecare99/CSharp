using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestConsole;
using ConsoleLib;
using System;
using System.Collections.Generic;

namespace ConsoleLibTests.CommonControls;

[TestClass]
public class TestBase
{
    protected static TstConsole _tstCon;
    private ConsoleLib.Interfaces.IConsole _oldCon;

    [TestInitialize]
    public void BaseInit()
    {
        _tstCon ??= new TstConsole(){WindowWidth=120,WindowHeight=40,ForegroundColor=ConsoleColor.Gray,BackgroundColor=ConsoleColor.Black};
        _oldCon = ConsoleFramework.console;
        ConsoleFramework.console = _tstCon;
        ConsoleFramework.Canvas._dimension.Width = _tstCon.WindowWidth; // ensure canvas dimension ok
        ConsoleFramework.Canvas._dimension.Height = Math.Min(50,_tstCon.WindowHeight);
        ConsoleLib.Control.MessageQueue = new Stack<(Action<object,EventArgs>,object,EventArgs)>();
    }

    [TestCleanup]
    public void BaseCleanup()
    {
        ConsoleFramework.console = _oldCon;
    }
}
