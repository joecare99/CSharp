using System;
using AA40_Wizzard.Model;
using NSubstitute;

namespace AA40_Wizzard.Tests.Models;

[TestClass]
public class SimpleLogTests
{
    [TestMethod]
    public void LogWritesTimestampedMessageTest()
    {
        var clock = Substitute.For<ISystemClock>();
        clock.Now.Returns(new DateTime(2025, 1, 2, 3, 4, 5));
        var received = string.Empty;
        var original = SimpleLog.LogAction;

        try
        {
            SimpleLog.LogAction = message => received = message;
            var log = new SimpleLog(clock);

            log.Log("hello");

            StringAssert.Contains(received, "Msg: hello");
            StringAssert.Contains(received, "01/02/2025 03:04:05");
        }
        finally
        {
            SimpleLog.LogAction = original;
        }
    }
}
