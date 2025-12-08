using AA06_ValueConverter2.Models.Interfaces;

namespace AA06_ValueConverter2.Models.Tests
{
    [TestClass()]
    public class SysTimeTests
    {
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Fügen Sie ggf. den „erforderlichen“ Modifizierer hinzu, oder deklarieren Sie den Modifizierer als NULL-Werte zulassend.
        private SysTime testClass;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Fügen Sie ggf. den „erforderlichen“ Modifizierer hinzu, oder deklarieren Sie den Modifizierer als NULL-Werte zulassend.

        [TestInitialize()]
        public void TestInitialize()
        {
            testClass = new SysTime();
        }
        [TestMethod()]
        public void SetUpTest()
        {
            Assert.IsNotNull(testClass);
            Assert.IsInstanceOfType(testClass, typeof(SysTime));
            Assert.IsInstanceOfType(testClass, typeof(ISysTime));
        }

        [TestMethod()]
        public void NowTest()
        {
            Assert.IsNotNull(testClass.Now);
        }

        [TestMethod()]
        public void UtcNowTest()
        {
            Assert.IsNotNull(testClass.UtcNow);
        }

        [TestMethod()]
        public void TodayTest()
        {
            Assert.IsNotNull(testClass.Today);
        }
    }
}
