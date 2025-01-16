using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Avalonia.ViewModels;

namespace Avalonia.Views.Extension.Tests;

[TestClass()]
public class IoCTests : BaseTestViewModel
{
    private Func<Type, object?>? _gsOld;
    private Func<Type, object>? _grsOld;
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Fügen Sie ggf. den „erforderlichen“ Modifizierer hinzu, oder deklarieren Sie den Modifizierer als NULL-Werte zulassend.
    private Func<IServiceScope> _gscOld;
    private IServiceScopeFactory _f;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Fügen Sie ggf. den „erforderlichen“ Modifizierer hinzu, oder deklarieren Sie den Modifizierer als NULL-Werte zulassend.

    private object GetReqSrv(Type arg)
    {
        DoLog($"GetReqSrv({arg})");
        return this;
    }

    private object? GetSrv(Type arg)
    {
        DoLog($"GetSrv({arg})");
        return null;
    }
    private IServiceScope GetScope()
    {
        DoLog($"GetScope()");
        IServiceScope serviceScope = Substitute.For<IServiceScope>();
        serviceScope.ServiceProvider.GetService(typeof(IServiceScopeFactory)).Returns( _f);
        return serviceScope;
    }


    [TestInitialize]
    public void Init()
    {
        _gsOld = IoC.GetSrv;
        _grsOld = IoC.GetReqSrv;
        _gscOld = IoC.GetScope;
        Assert.ThrowsException<NotImplementedException>(() => _gscOld?.Invoke());
        IoC.GetSrv = GetSrv;
        IoC.GetReqSrv = GetReqSrv;
        IoC.GetScope = GetScope;
        _f = Substitute.For<IServiceScopeFactory>();
        _f.CreateScope().Returns((_)=>GetScope());
    }


    [TestCleanup]
    public void CleanUp()
    {
        IoC.GetSrv = _gsOld!;
        IoC.GetReqSrv = _grsOld!;
        IoC.GetScope = _gscOld;
    }

    [TestMethod()]
    public void SetupTest()
    {
        Assert.ThrowsException<NotImplementedException>(() => _grsOld?.Invoke(typeof(object)));
        Assert.AreEqual(null, _gsOld?.Invoke(typeof(object)));
    }

    [TestMethod()]
    public void GetRequiredServiceTest()
    {
        Assert.AreEqual(this, IoC.GetRequiredService<Object>());
        Assert.AreEqual("GetReqSrv(System.Object)\r\n", DebugLog);
    }

    [TestMethod()]
    public void GetServiceTest()
    {
        Assert.AreEqual(null, IoC.GetService<Object>());
        Assert.AreEqual("GetSrv(System.Object)\r\n", DebugLog);
    }

    [TestMethod()]
    public void ProvideValueTest()
    {
        var ioc = new IoC
        {
            Type = typeof(Object)
        };
        Assert.AreEqual(this, ioc.ProvideValue(null!));
        Assert.AreEqual("GetReqSrv(System.Object)\r\n", DebugLog);
    }

    [TestMethod()]
    public void GetNewScopeTest()
    {
        var s =IoC.GetNewScope() ;
        Assert.IsNotNull(s);
        var s2 = IoC.GetNewScope(s);
        Assert.IsNotNull(s2);
        Assert.AreNotEqual(s, s2);
        Assert.AreEqual("GetScope()\r\nGetScope()\r\n", DebugLog);
    }

    [TestMethod()]
    public void SetCurrentScopeTest()
    {
        var s = IoC.GetNewScope();
        Assert.AreEqual("GetScope()\r\n", DebugLog);
        Assert.AreEqual(s,IoC.Scope);
        s.ServiceProvider.GetService(typeof(object)).Returns(null);
        var s2 = IoC.GetNewScope(s);
        Assert.AreEqual(s2, IoC.Scope);
        s2.ServiceProvider.GetService(typeof(object)).Returns(this);
        IoC.SetCurrentScope(s2);
        Assert.AreEqual(s2, IoC.Scope);
        Assert.IsNotNull(IoC.GetRequiredService<Object>());
        IoC.SetCurrentScope(s);
        Assert.AreEqual(s, IoC.Scope);
        Assert.ThrowsException<InvalidOperationException>(()=> IoC.GetRequiredService<Object>());
        Assert.AreEqual("GetScope()\r\nGetScope()\r\n", DebugLog);
    }

    [TestMethod()]
    public void ConfigureTest()
    {
        var sp = Substitute.For<IServiceProvider>();
        sp.GetService(typeof(IServiceScopeFactory)).Returns(_f);
        sp.GetService(typeof(object)).Returns(this);
        IoC.Configure(sp);
        Assert.AreEqual(sp.GetService, IoC.GetSrv);
        Assert.AreEqual(sp.CreateScope, IoC.GetScope);
        Assert.AreEqual(sp.GetService(typeof(Object)), IoC.GetRequiredService<Object>());
        Assert.AreEqual("GetScope()\r\n", DebugLog);
    }
}