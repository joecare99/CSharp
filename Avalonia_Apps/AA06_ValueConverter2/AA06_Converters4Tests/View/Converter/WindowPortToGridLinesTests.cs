using Avalonia;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace AA06_Converters_4.View.Converter.Tests;

[TestClass()]
public class WindowPortToGridLinesTests
{
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Fügen Sie ggf. den „erforderlichen“ Modifizierer hinzu, oder deklarieren Sie den Modifizierer als NULL-Werte zulassend.
    WindowPortToGridLines testVC;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Fügen Sie ggf. den „erforderlichen“ Modifizierer hinzu, oder deklarieren Sie den Modifizierer als NULL-Werte zulassend.
    ViewModels.SWindowPort wp;

    public static IEnumerable<object[]> ConvertTestData
    {
        get
        {
            yield return new object[] { new ViewModels.SWindowPort() { Parent=null!,port=new System.Drawing.RectangleF(-10,-10,20,20) } };
            yield return new object[] { new ViewModels.SWindowPort() { Parent=null!,port=new System.Drawing.RectangleF(-10,-10,20,20) } };
        }
    }

    [TestInitialize]
    public void TestInit()
    {
        testVC = new WindowPortToGridLines();
        testVC.WindowSize = new Size(200, 100);
        wp = new ViewModels.SWindowPort() { Parent=null!,port=new System.Drawing.RectangleF(-10,-10,20,20) };
    }

    [TestMethod()]
    public void WindowPortToGridLinesTest()
    {
        Assert.Fail();
    }

    [TestMethod()]
    [DynamicData(nameof(ConvertTestData))]
    public void ConvertTest(object o)
    {
        var test = testVC.Convert(o, null, null, null);
        Assert.Fail();
    }

    [TestMethod()]
    public void GetAdjustedRectTest()
    {
        var r2 = testVC.GetAdjustedRect(wp);
        System.Drawing.RectangleF rExp = new(-20,-10,40,20);
        Assert.AreEqual(rExp,r2);
    }

    [TestMethod()]
    public void ConvertBackTest()
    {
        Assert.ThrowsExactly<NotImplementedException>(() => testVC.ConvertBack(null!, null, null, null));
    }
}