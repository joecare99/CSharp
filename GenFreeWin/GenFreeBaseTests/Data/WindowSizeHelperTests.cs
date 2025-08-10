using Microsoft.VisualStudio.TestTools.UnitTesting;
using GenFree.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenFree.Data.Tests;

[TestClass()]
public class WindowSizeHelperTests
{
    [TestMethod]
    [DataRow(EWindowSize.ws800x600, 800, 600, "Normal")]
    [DataRow(EWindowSize.ws900x670, 900, 670, "Custom1")]
    [DataRow(EWindowSize.ws900x710, 900, 710, "Custom2")]
    [DataRow(EWindowSize.ws1024x710, 1024, 710, "Custom3")]
    [DataRow(EWindowSize.ws1024x768, 1024, 768, "XGA")]
    [DataRow(EWindowSize.ws1150x800, 1150, 800, "Custom4")]
    [DataRow(EWindowSize.ws1150x835, 1150, 835, "Custom5")]
    [DataRow(EWindowSize.ws1280x920, 1280, 920, "Custom6")]
    [DataRow(EWindowSize.ws1400x1050, 1400, 1050, "Custom7")]
    [DataRow(EWindowSize.ws1600x1200, 1600, 1200, "UXGA")]
    public void GetWindowSizeTest(EWindowSize eAct, int width, int height, string expected)
    {
        // Annahme: WindowSizeHelper.GetWindowSize gibt einen string zurück, der die Größe beschreibt
        var (iRWidth,iRHeight) = eAct.GetWindowSize();
        Assert.AreEqual(width, iRWidth);
        Assert.AreEqual(height, iRHeight);
    }

    [TestMethod]
    [DataRow((EWindowSize)10, 1600, 1200, "UXGA")]
    public void GetWindowSizeTest2(EWindowSize eAct, int width, int height, string expected)
    {
        // Annahme: WindowSizeHelper.GetWindowSize gibt einen string zurück, der die Größe beschreibt
        Assert.Throws<NotImplementedException>( ()=>_ = eAct.GetWindowSize());
    }


    [TestMethod]
    [DataRow(600, EWindowSize.ws800x600)]
    [DataRow(670, EWindowSize.ws900x670)]
    [DataRow(700, EWindowSize.ws900x710)]
    [DataRow(710, EWindowSize.ws1024x710)]
    [DataRow(768, EWindowSize.ws1024x768)]
    [DataRow(800, EWindowSize.ws1150x800)]
    [DataRow(835, EWindowSize.ws1150x835)]
    [DataRow(920, EWindowSize.ws1280x920)]
    [DataRow(1050, EWindowSize.ws1400x1050)]
    [DataRow(1200, EWindowSize.ws1600x1200)]
    public void GetWindowSizeTest1(int iHeight, EWindowSize eExp)
    {
        var result = iHeight.GetWindowSize();
        Assert.AreEqual(eExp, result);
    }
}