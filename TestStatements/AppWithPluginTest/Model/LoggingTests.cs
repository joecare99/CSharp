using Microsoft.VisualStudio.TestTools.UnitTesting;
using AppWithPlugin.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using BaseLib.Helper;
using NSubstitute;

namespace AppWithPlugin.Model.Tests
{
    [TestClass()]
    public class LoggingTests
    {
        private Logging _testClass;
        private Action<string> _old;
        private string _DebugLog ="";

        [TestInitialize]
        public void Initialize()
        {
            _old = Logging._logAction;
            Logging._logAction = (s) => { _DebugLog += $"{s}{Environment.NewLine}"; };
            _testClass = new Logging();
            _DebugLog = "";
        }

        [TestCleanup]
        public void Cleanup()
        {
            Logging._logAction = _old;
        }

        [TestMethod]
        public void LoggingTest()
        {
            // Arrange
            // Act
            var result = new Logging();
            Logging._logAction("Test");

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(_testClass);

        }

        [TestMethod()]
        public void BeginScopeTest()
        {
            // Arrange
            var state = new { Message = "Test Scope" };

            // Act
            var result = _testClass.BeginScope(state);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(IDisposable));
            object? target = null;
            Assert.AreEqual(true, result.GetProp("State", (WeakReference<object>)null)?.TryGetTarget(out target));
            Assert.AreEqual(state, target);

            result.Dispose();
        } 
        
        [TestMethod()]
        public void BeginScopeTest2()
        {
            // Arrange
            var state = Substitute.For<IDisposable>();

            // Act
            var result = _testClass.BeginScope(state);
            var result2 = _testClass.BeginScope(state);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNull(result2);
            Assert.IsInstanceOfType(result, typeof(IDisposable));
            Assert.AreEqual(state, result);

            result.Dispose();
        }

        [TestMethod()]
        public void IsEnabledTest()
        {
            // Arrange
            var logLevel = LogLevel.Information;

            // Act
            var result = _testClass.IsEnabled(logLevel);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void LogTest()
        {
            // Arrange
            var logLevel = LogLevel.Information;
            var eventId = new EventId(1, "TestEvent");
            var state = new { Message = "Test Log" };
            Exception? exception = null;
            Func<object, Exception?, string> formatter = (s, e) => s.ToString();

            // Act
            _testClass.Log(logLevel, eventId, state, exception, formatter);

            // Assert
            Assert.AreEqual("[Information] 1 { Message = Test Log }\r\n", _DebugLog);
        }
    }
}