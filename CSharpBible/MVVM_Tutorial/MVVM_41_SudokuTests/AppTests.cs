﻿using BaseLib.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MVVM.View.Extension;
using Sudoku_Base.Models.Interfaces;
using NSubstitute;
using System;

namespace MVVM_41_Sudoku.Tests;

internal class TestApp : App
{
    public void DoStartUp()
    {
        OnStartup(null);
    }
}
[TestClass()]
public class AppTests 
{
    static TestApp app = new();
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
    private Func<Type, object?> _gsold;
    private Func<Type, object> _grsold;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
    private ISysTime? _sysTime;
    private ILog? _log;

    /// <summary>
    /// Initializes this instance.
    /// </summary>
    /// <autogeneratedoc />
    [TestInitialize]
    public void Init()
    {
        _gsold = IoC.GetSrv;
        _grsold = IoC.GetReqSrv;
        IoC.GetReqSrv = (t) =>t switch {
            _ when t == typeof(ISudokuModel) => Substitute.For<ISudokuModel>(),
            _ => throw new ArgumentException() };          
    }

    [TestCleanup]
    public void CleanUp()
    {
        IoC.GetSrv = _gsold;
        IoC.GetReqSrv = _grsold;
    }

    [TestMethod]
    public void AppTest()
    {
        Assert.IsNotNull(app);
    }

    [TestMethod]
    public void AppTest2()
    {
        app.DoStartUp();
        Assert.IsNotNull(IoC.GetReqSrv(typeof(ISudokuModel)));
        Assert.IsNull(IoC.GetSrv(typeof(App)));
    }
}
