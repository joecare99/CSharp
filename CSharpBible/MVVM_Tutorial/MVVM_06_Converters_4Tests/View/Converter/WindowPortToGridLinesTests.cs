using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace MVVM_06_Converters_4.View.Converter.Tests;

[TestClass()]
public class WindowPortToGridLinesTests
{
    WindowPortToGridLines testVC;
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
        testVC.WindowSize = new System.Windows.Size(200, 100);
        wp = new ViewModels.SWindowPort() { Parent=null!,port=new System.Drawing.RectangleF(-10,-10,20,20) };
    }

    [TestMethod()]
    public void WindowPortToGridLinesTest()
    {
        Assert.Fail();
    }

    [DataTestMethod()]
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
        Assert.ThrowsException<NotImplementedException>(() => testVC.ConvertBack(null!, null, null, null));
    }
}