using Microsoft.VisualStudio.TestTools.UnitTesting;
using HelloPlugin.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;
using PluginBase.Interfaces;
using Microsoft.Extensions.Logging;

namespace HelloPlugin.Model.Tests
{
    [TestClass()]
    public class TestCommandTests
    {
        private TestCommand _testClass;
        [TestInitialize]
        public void Initialize()
        {
            _testClass = new TestCommand();
        }

        [TestMethod()]
        public void InitializeTest()
        {
            // Arrange
            var env = Substitute.For<IEnvironment>();

            // Act
            _testClass.Initialize(env);

            // Assert
            Assert.AreEqual(env, _testClass.GetType().GetField("_env", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(_testClass));
            env.Received().GetService<IRandom>();
            env.Received().GetService<ILogger>();
            env.Received().GetService<ISysTime>();            
        }

        [TestMethod()]
        public void ExecuteTest()
        {
            // Arrange
            var env = Substitute.For<IEnvironment>();
            var random = Substitute.For<IRandom>();
            var logger = Substitute.For<ILogger>();
            var sysTime = Substitute.For<ISysTime>();
            env.GetService<IRandom>().Returns(random);
            env.GetService<ILogger>().Returns(logger);
            env.GetService<ISysTime>().Returns(sysTime);
            _testClass.Initialize(env);

            // Act
            var result = _testClass.Execute();

            // Assert
            Assert.AreEqual(0, result); // Assuming Execute returns 0 on success
            random.Received().Next();
            _=sysTime.Received().Now;
            logger.ReceivedWithAnyArgs().LogInformation("");
        }
    }
}