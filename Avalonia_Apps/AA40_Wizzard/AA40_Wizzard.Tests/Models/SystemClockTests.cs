using System;
using AA40_Wizzard.Model;

namespace AA40_Wizzard.Tests.Models;

[TestClass]
public class SystemClockTests
{
    [TestMethod]
    public void NowAndTodayUseConfiguredDelegateTest()
    {
        var original = SystemClock.GetNow;

        try
        {
            SystemClock.GetNow = static () => new DateTime(2025, 1, 2, 3, 4, 5);
            var clock = new SystemClock();

            Assert.AreEqual(new DateTime(2025, 1, 2, 3, 4, 5), clock.Now);
            Assert.AreEqual(new DateTime(2025, 1, 2), clock.Today);
        }
        finally
        {
            SystemClock.GetNow = original;
        }
    }
}
