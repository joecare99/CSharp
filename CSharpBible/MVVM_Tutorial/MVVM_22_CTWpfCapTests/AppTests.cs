﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using BaseLib.Helper;
using MVVM_22_CTWpfCap.Model;
using System;

namespace MVVM_22_CTWpfCap.Tests;

internal class TestApp : App
{
    public void DoStartUp()
    {
        OnStartup(null!);
    }
}
[TestClass()]
public class AppTests 
{
    static TestApp app = new();
    private Func<Type, object?> _gsold = null!;
    private Func<Type, object> _grsold = null!;

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
        Assert.IsNotNull(IoC.GetReqSrv(typeof(IWpfCapModel)));
        Assert.IsNull(IoC.GetSrv(typeof(App)));
    }
}
