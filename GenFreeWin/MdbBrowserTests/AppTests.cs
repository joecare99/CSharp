using BaseLib.Helper;
using MdbBrowser.ViewModels.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace MdbBrowser.Tests
{
    [TestClass()]
    public class AppTests
    {
        [TestMethod()]
        public void AppTest()
        {
            var testClass = new App();
            Assert.IsNotNull(testClass);
            Assert.ThrowsExactly<InvalidOperationException>(() => _ = IoC.GetRequiredService<IDBViewViewModel>());
        }
    }
}