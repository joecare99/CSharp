using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVM_38_CTDependencyInjection.Models.Tests;
[TestClass]
public class CTimerTests
{
    CTimer? testTimer = new();
    
    [TestMethod()]
    public void CTimerTest()
    {
        //var t = new Thread(() => testTimer = new());
        //t.SetApartmentState(ApartmentState.STA); //Set the thread to STA
        //t.Start();
        //t.Join(); //Wait for the thread to end
        Assert.IsNotNull(testTimer);
        Assert.IsInstanceOfType(testTimer, typeof(CTimer));
        Assert.AreEqual(false, testTimer.Enabled);
    }

    [TestMethod()]
    [DataRow(1000)]
    [DataRow(2000)]        
    public void IntervallTest(double dAct)
    {
        Assert.AreEqual(100d, testTimer.Interval);
        testTimer.Interval = dAct;
        Assert.AreEqual(dAct, testTimer.Interval);
    }

    [TestMethod()]
    public void StartTest()
    {
        Assert.AreEqual(false, testTimer.Enabled);
        testTimer.Start();
        Assert.AreEqual(true, testTimer.Enabled);
    }

    [TestMethod()]
    public void StopTest()
    {
        testTimer.Start();
        Assert.AreEqual(true, testTimer.Enabled);
        testTimer.Stop();
        Assert.AreEqual(false, testTimer.Enabled);
    }
}
