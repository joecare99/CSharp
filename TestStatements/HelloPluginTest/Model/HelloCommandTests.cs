using Microsoft.VisualStudio.TestTools.UnitTesting;
using HelloPlugin.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;
using PluginBase.Interfaces;
using HelloPlugin.Properties;

namespace HelloPlugin.Model.Tests
{
    [TestClass()]
    public class HelloCommandTests
    {
        private HelloCommand _testClass;

        [TestInitialize]
        public void Initialize()
        {
            _testClass = new HelloCommand();
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
        }

        [TestMethod()]
        public void ExecuteTest()
        {
            // Arrange
            var env = Substitute.For<IEnvironment>();
            _testClass.Initialize(env);

            // Act
            var result = _testClass.Execute();

            // Assert
            Assert.AreEqual(1, result); // Assuming Execute returns 0 on success
            env.ui.Received().ShowMessage(Resources.msgHello);
        }
    }
}