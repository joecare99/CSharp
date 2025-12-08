namespace Avalonia_App02.Models.Tests
{
    [TestClass()]
    public class TimerProxyTests
    {
        [TestMethod()]
        public void TimerProxyTest()
        {
            // Arrange
            var tp = new TimerProxy();

            // Act
            tp.Interval = 1000;

            // Assert
            Assert.IsNotNull(tp);
            Assert.IsInstanceOfType(tp, typeof(TimerProxy));
            Assert.AreEqual(1000, tp.Interval);
        }
    }
}