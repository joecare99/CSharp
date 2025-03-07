﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using BaseLib.Helper;
using MVVM_37_TreeView.Services;
using System;

namespace MVVM_37_TreeView.Tests;

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
    private Func<Type, object?> _gsold;
    private Func<Type, object> _grsold;

    /// <summary>
    /// Initializes this instance.
    /// </summary>
    /// <autogeneratedoc />
    [TestInitialize]
    public void Init()
    {
        _gsold = IoC.GetSrv;
        _grsold = IoC.GetReqSrv;
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
        Assert.IsNotNull(IoC.GetReqSrv(typeof(IBooksService)));
        Assert.IsNull(IoC.GetSrv(typeof(App)));
    }
}