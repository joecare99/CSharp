using Microsoft.VisualStudio.TestTools.UnitTesting;
using MVVM.ViewModel;
using System;
using BaseLib.Helper;

namespace MVVM.View.Extension.Tests;

[TestClass()]
public class IoC2Tests : BaseTestViewModel
{
    private Func<Type, object>? _grsOld;

    private object GetReqSrv(Type arg)
    {
        DoLog($"GetReqSrv({arg})");
        return this;
    }

    [TestInitialize]
    public void Init()
    {
        _grsOld = IoC.GetReqSrv;
        Assert.ThrowsExactly<NotImplementedException>(() => _grsOld?.Invoke(null));
        IoC.GetReqSrv = GetReqSrv;
    }

    [TestCleanup]
    public void CleanUp()
    {
        IoC.GetReqSrv = _grsOld!;
    }

    [TestMethod()]
    public void ProvideValueTest()
    {
        var ioc = new IoC2
        {
            Type = typeof(Object)
        };
        Assert.AreEqual(this, ioc.ProvideValue(null!));
        Assert.AreEqual("GetReqSrv(System.Object)\r\n", DebugLog);
    }

  
}