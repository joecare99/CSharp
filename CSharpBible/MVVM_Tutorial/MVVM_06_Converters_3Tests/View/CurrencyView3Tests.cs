﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;

namespace MVVM_06_Converters_3.View.Tests;

[TestClass]
public class CurrencyView3Tests
{
    [TestMethod()]
    public void CurrencyViewTest()
    {
        CurrencyView3? testView = null;
        var t = new Thread(() => testView = new());
        t.SetApartmentState(ApartmentState.STA); //Set the thread to STA
        t.Start();
        t.Join(); //Wait for the thread to end
        Assert.IsNotNull(testView);
        Assert.IsInstanceOfType(testView, typeof(CurrencyView3));
    }
}
