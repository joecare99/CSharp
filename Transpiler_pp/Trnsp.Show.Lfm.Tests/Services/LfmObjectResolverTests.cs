using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Threading.Tasks;
using TranspilerLib.Pascal.Models;
using Trnsp.Show.Lfm.Models.Components;

namespace Trnsp.Show.Lfm.Services.Tests;

[TestClass]
public class LfmObjectResolverTests
{
    private sealed class TestComponent : LfmComponentBase
    {
    }

    private sealed class TestAction : TAction
    {
        public TestAction()
        {
            Caption = "TestCaption";
            Hint = "TestHint";
        }
    }

    [TestMethod]
    public void ResolveOrDefer_ResolvesAlreadyRegisteredObject()
    {
        var resolver = new LfmObjectResolver();
        LfmComponentBase.ObjectResolver = resolver;

        var action = new TestAction();
        resolver.RegisterObject("TestAction", action);

        var comp = new TestComponent();
        comp.ApplyProperties(new LfmObject
        {
            Name = "Comp1",
            TypeName = "TButton",
            Properties =
            {
                new() { Name = "Action", Value = "TestAction" }
            }
        });

        Assert.AreSame(action, comp.LinkedAction!.TryGetTarget(out var t)?t:t);
        Assert.AreEqual("TestCaption", comp.EffectiveCaption);
        Assert.AreEqual("TestHint", comp.EffectiveHint);
    }

    [TestMethod]
    public void ResolveOrDefer_ResolvesDeferredObject()
    {
        var resolver = new LfmObjectResolver();
        LfmComponentBase.ObjectResolver = resolver;

        var comp = new TestComponent();
        comp.ApplyProperties(new LfmObject
        {
            Name = "Comp1",
            TypeName = "TButton",
            Properties =
            {
                new() { Name = "Action", Value = "TestAction" }
            }
        });

        Assert.IsNull(comp.LinkedAction);

        var action = new TestAction();
        resolver.RegisterObject("TestAction", action);

        Assert.AreSame(action, comp.LinkedAction!.TryGetTarget(out var t) ? t : t);
        Assert.AreEqual("TestCaption", comp.EffectiveCaption);
        Assert.AreEqual("TestHint", comp.EffectiveHint);
    }

    [TestMethod]
    public void RegisterObject_RegistersComponentItself()
    {
        var resolver = new LfmObjectResolver();
        LfmComponentBase.ObjectResolver = resolver;

        var comp = new TestComponent();
        comp.ApplyProperties(new LfmObject
        {
            Name = "Comp1",
            TypeName = "TButton"
        });

        resolver.ResolveOrDefer(
            "Comp1",
            new object(),
            (resolved) =>
            {
                Assert.AreSame(comp, resolved);
            });
    }

}
