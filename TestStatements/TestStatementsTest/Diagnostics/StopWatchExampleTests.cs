using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TestStatements.UnitTesting;

namespace TestStatements.Diagnostics.Tests
{
    [TestClass]
    public class StopWatchExampleTests:ConsoleTestsBase
    {
        private MyStopwatch _myStopwatch;
        private MyStopwatch _myStopwatch2;

        private class MyStopwatch : IStopwatch
        {
            TimeSpan _myElapsed = TimeSpan.Zero;
            TimeSpan _epsilon;
            bool _IsRunning;
            private static event EventHandler<int> _onSleep;

            public MyStopwatch() {
                _epsilon = TimeSpan.Zero.Add(TimeSpan.FromMilliseconds(0.001d));
                _IsRunning = false;
            }

            private void doSleep(object sender, int e)
            {
                if (_IsRunning)
                    _myElapsed = _myElapsed.Add(TimeSpan.FromMilliseconds(e));
            }

            public static void Sleep(int obj)
            {
                _onSleep?.Invoke(null,obj);
            }

            public virtual void Start() {
                if (!_IsRunning)
                {
                    _IsRunning = true;
                    _onSleep += doSleep;
                    _myElapsed = _myElapsed.Add(_epsilon);
                }
            }
            
            public virtual void Stop()
            {
                _myElapsed = _myElapsed.Add(_epsilon);
                _onSleep -= doSleep;
                _IsRunning = false;
            }
            public virtual void Reset()
            {
                _IsRunning = false;
                _onSleep -= doSleep;
                _myElapsed = TimeSpan.Zero;
            }
            public TimeSpan Elapsed
                => _myElapsed = _IsRunning? _myElapsed.Add(_epsilon):_myElapsed;

            public long ElapsedTicks => Elapsed.Ticks;

            public long ElapsedMilliseconds => (long)Elapsed.TotalMilliseconds;
        }

        [TestInitialize]
        public virtual void Init() { 
            _myStopwatch = new MyStopwatch();
            _myStopwatch2 = new MyStopwatch();
            StopWatchExample.GetStopwatch = () => _myStopwatch;
            StopWatchExample.GetStopwatch2 = () => _myStopwatch2;
            StopWatchExample.ThreadSleep = MyStopwatch.Sleep;
        }

        [TestMethod]
        public void ExampleMain1Test() {
            AssertConsoleOutput("This will take aprox. 10s\r\nRunTime 00:00:10.00", StopWatchExample.ExampleMain1);
        }

        [TestMethod]
        public void ExampleMainTest()
        {
            var result = GetConsoleInOutputArgs("\r\n", null, (a) => StopWatchExample.ExampleMain());
            Assert.IsTrue(result.Contains("This will take aprox. 10s"));
            Assert.IsTrue(result.Contains("RunTime 00:00:10.00"));
            Assert.IsTrue(result.Contains("Operations timed using the"));
            Assert.IsTrue(result.Contains("Operation: Int32.Parse(\"0\") Summary:"));
            Assert.IsTrue(result.Contains("Operation: Int32.TryParse(\"a\") Summary:"));
        }

        [TestMethod]
        public void ExampleMain2Test()
        {
            var result = GetConsoleInOutputArgs("\r\n", null, (a) => StopWatchExample.ExampleMain2());
            Assert.IsTrue(result.Contains("Press the Enter key to begin:"));
            Assert.IsTrue(result.Contains("Operation: Int32.Parse(\"0\") Summary:"));
            Assert.IsTrue(result.Contains("Operation: Int32.TryParse(\"0\") Summary:"));
            Assert.IsTrue(result.Contains("Operation: Int32.Parse(\"a\") Summary:"));
            Assert.IsTrue(result.Contains("Operation: Int32.TryParse(\"a\") Summary:"));
        }

        [TestMethod]
        public void DisplayTimerPropertiesTest()
        {
            AssertConsoleOutput("Operations timed using the system's high-resolution performance counter.\r\n  Timer frequency in ticks per second = 10000000\r\n  Timer is accurate within 100 nanoseconds", StopWatchExample.DisplayTimerProperties);
        }

        [TestMethod]
        public void TimeOperationsTest()
        {
            var result = GetConsoleOutput(StopWatchExample.TimeOperations);
            Assert.IsTrue(result.Contains("Operation: Int32.Parse(\"0\") Summary:"));
            Assert.IsTrue(result.Contains("Operation: Int32.TryParse(\"0\") Summary:"));
            Assert.IsTrue(result.Contains("Operation: Int32.Parse(\"a\") Summary:"));
            Assert.IsTrue(result.Contains("Operation: Int32.TryParse(\"a\") Summary:"));
            Assert.IsTrue(result.Contains("Average time:"));
            Assert.IsTrue(result.Contains("Total time looping through 1000 operations:"));
        }

    }
}
