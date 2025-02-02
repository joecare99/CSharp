using BaseLib.Interfaces;
using BaseLib.Models.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MVVM.ViewModel;
using NSubstitute;
using Sudoku_Base.Models;
using System;
using System.Linq;

namespace MVVM_41_Sudoku.Models.Tests;

[TestClass]
public class SimpleLogTests:BaseTestViewModel
{
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Fügen Sie ggf. den „erforderlichen“ Modifizierer hinzu, oder deklarieren Sie den Modifizierer als NULL-Werte zulassend.
    private Action<string> _gsOld;
    private SimpleLog simpleLog;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Fügen Sie ggf. den „erforderlichen“ Modifizierer hinzu, oder deklarieren Sie den Modifizierer als NULL-Werte zulassend.
    private ISysTime? _sysTime;


    [TestInitialize]
    public void TestInitialize()
    {
        _gsOld= SimpleLog.LogAction;
        _gsOld("Test message");
        SimpleLog.LogAction = DoLog; 
        _sysTime = Substitute.For<ISysTime>();
        _sysTime.Now.Returns(new DateTime(2022, 08, 24, 12, 0, 0));
        simpleLog = new SimpleLog(_sysTime);
    }

    [TestCleanup]
    public void TestCleanup()
    {
        SimpleLog.LogAction = _gsOld;
    }

    [DataTestMethod]
    [DataRow("Test message",new[] { "08/24/2022 12:00:00: Msg: Test message\r\n" })]
    [DataRow(null, new[] { "08/24/2022 12:00:00: Msg: \r\n" })]
    [DataRow("Some other test", new[] { "08/24/2022 12:00:00: Msg: Some other test\r\n" })]

    public void LogTest(string message, string[] asExp)
    {

        // Act
        simpleLog.Log(message);

        // Assert
        Assert.AreEqual(1, _sysTime.ReceivedCalls().Count());
        Assert.AreEqual(asExp[0],DebugLog);
    }

    [TestMethod]
    public void LogTest1()
    {
        // Arrange
        var message = "Test message";
        var exception = new Exception("Test exception");

        // Act
        simpleLog.Log(message, exception);

        // Assert
        Assert.AreEqual(1, _sysTime.ReceivedCalls().Count());
        Assert.AreEqual("08/24/2022 12:00:00: Err: Test message, System.Exception: Test exception\r\n", DebugLog);
    }
}
