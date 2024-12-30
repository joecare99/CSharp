using Microsoft.VisualStudio.TestTools.UnitTesting;
using AppWithPlugin.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppWithPlugin.Model.Tests
{
    [TestClass()]
    public class AppWithPluginTests
    {
        private AppWithPlugin _testClass;

        private interface ISomeService
        {
            string Test { get; set; }
        }

        private class SomeService : ISomeService
        {
            public string Test { get; set; }
        }

        [TestInitialize]
        public void Initialize()
        {
            _testClass = new AppWithPlugin();
        }

        [TestMethod()]
        public void SetUp()
        {
            Assert.IsNotNull(_testClass);
        }

        [TestMethod]
        public void InitializeTest()
        {
            // Arrange
            // Act
            _testClass.Initialize([""]);
            // Assert
            Assert.IsTrue(true);
        }

        [TestMethod()]
        public void MainTest()
        {
            // Arrange
            string[] args = new string[] { "Test" };
            // Act
            _testClass.Main(args);
            // Assert
            Assert.IsTrue(true);
        }

        [TestMethod()]
        public void GetServiceTest()
        {
            // Arrange
            _testClass.AddService<ISomeService, SomeService>();

            // Act
            var result = _testClass.GetService<ISomeService>();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType( result,typeof(SomeService));
            Assert.IsInstanceOfType(result, typeof(ISomeService));
        }

        [TestMethod()]
        public void AddServiceTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ShowMessageTest()
        {
            Assert.Fail();
        }
    }
}