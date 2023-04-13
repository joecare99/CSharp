using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using TestStatements.UnitTesting;

namespace TestStatements.Diagnostics.Tests
{
    [TestClass]
    public class StopWatchExampleTests:ConsoleTestsBase
    {
        private MyStopwatch _myStopwatch;

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
            StopWatchExample.GetStopwatch = () => _myStopwatch;
            StopWatchExample.ThreadSleep = MyStopwatch.Sleep;
        }

        [TestMethod]
        public void ExampleMain1Test() {
            AssertConsoleOutput("This will take aprox. 10s\r\nRunTime 00:00:10.00", StopWatchExample.ExampleMain1);
        }

        [TestMethod]
        public void ExampleMainTest()
        {
            AssertConsoleInOutputArgs(@"This will take aprox. 10s
RunTime 00:00:10.00
Operations timed using the system's high-resolution performance counter.
  Timer frequency in ticks per second = 10000000
  Timer is accurate within 100 nanoseconds
This will take some time

Press the Enter key to begin:


Operation: Int32.Parse(""0"") Summary:
  Slowest time:  #-1/1000 = 0 ticks
  Fastest time:  #1/1000 = 0 ticks
  Average time:  0 ticks = 0 nanoseconds
  Total time looping through 1000 operations: 0 milliseconds

Operation: Int32.TryParse(""0"") Summary:
  Slowest time:  #-1/1000 = 0 ticks
  Fastest time:  #1/1000 = 0 ticks
  Average time:  0 ticks = 0 nanoseconds
  Total time looping through 1000 operations: 0 milliseconds

Operation: Int32.Parse(""a"") Summary:
  Slowest time:  #-1/1000 = 0 ticks
  Fastest time:  #1/1000 = 0 ticks
  Average time:  0 ticks = 0 nanoseconds
  Total time looping through 1000 operations: 0 milliseconds

Operation: Int32.TryParse(""a"") Summary:
  Slowest time:  #-1/1000 = 0 ticks
  Fastest time:  #1/1000 = 0 ticks
  Average time:  0 ticks = 0 nanoseconds
  Total time looping through 1000 operations: 0 milliseconds", "\r\n",null,(a)=> StopWatchExample.ExampleMain());
        }

        [TestMethod]
        public void ExampleMain2Test()
        {
            AssertConsoleInOutputArgs(@"Operations timed using the system's high-resolution performance counter.
  Timer frequency in ticks per second = 10000000
  Timer is accurate within 100 nanoseconds
This will take some time

Press the Enter key to begin:


Operation: Int32.Parse(""0"") Summary:
  Slowest time:  #-1/1000 = 0 ticks
  Fastest time:  #1/1000 = 0 ticks
  Average time:  0 ticks = 0 nanoseconds
  Total time looping through 1000 operations: 0 milliseconds

Operation: Int32.TryParse(""0"") Summary:
  Slowest time:  #-1/1000 = 0 ticks
  Fastest time:  #1/1000 = 0 ticks
  Average time:  0 ticks = 0 nanoseconds
  Total time looping through 1000 operations: 0 milliseconds

Operation: Int32.Parse(""a"") Summary:
  Slowest time:  #-1/1000 = 0 ticks
  Fastest time:  #1/1000 = 0 ticks
  Average time:  0 ticks = 0 nanoseconds
  Total time looping through 1000 operations: 0 milliseconds

Operation: Int32.TryParse(""a"") Summary:
  Slowest time:  #-1/1000 = 0 ticks
  Fastest time:  #1/1000 = 0 ticks
  Average time:  0 ticks = 0 nanoseconds
  Total time looping through 1000 operations: 0 milliseconds", "\r\n", null, (a) => StopWatchExample.ExampleMain2());
        }

        [TestMethod]
        public void DisplayTimerPropertiesTest()
        {
            AssertConsoleOutput("Operations timed using the system's high-resolution performance counter.\r\n  Timer frequency in ticks per second = 10000000\r\n  Timer is accurate within 100 nanoseconds", StopWatchExample.DisplayTimerProperties);
        }

        [TestMethod]
        public void TimeOperationsTest()
        {
            AssertConsoleOutput("Operation: Int32.Parse(\"0\") Summary:\r\n  Slowest time:  #-1/1000 = 0 ticks\r\n  Fastest time:  #1/1000 = 0 ticks\r\n  Average time:  0 ticks = 0 nanoseconds\r\n  Total time looping through 1000 operations: 0 milliseconds\r\n\r\nOperation: Int32.TryParse(\"0\") Summary:\r\n  Slowest time:  #-1/1000 = 0 ticks\r\n  Fastest time:  #1/1000 = 0 ticks\r\n  Average time:  0 ticks = 0 nanoseconds\r\n  Total time looping through 1000 operations: 0 milliseconds\r\n\r\nOperation: Int32.Parse(\"a\") Summary:\r\n  Slowest time:  #-1/1000 = 0 ticks\r\n  Fastest time:  #1/1000 = 0 ticks\r\n  Average time:  0 ticks = 0 nanoseconds\r\n  Total time looping through 1000 operations: 0 milliseconds\r\n\r\nOperation: Int32.TryParse(\"a\") Summary:\r\n  Slowest time:  #-1/1000 = 0 ticks\r\n  Fastest time:  #1/1000 = 0 ticks\r\n  Average time:  0 ticks = 0 nanoseconds\r\n  Total time looping through 1000 operations: 0 milliseconds", StopWatchExample.TimeOperations);
        }

    }
}
