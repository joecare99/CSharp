using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using MVVM.View.Extension;
using MdbBrowser.ViewModels.Interfaces;

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
            Assert.ThrowsException<InvalidOperationException>(()=>_=IoC.GetRequiredService<IDBViewViewModel>());
        }
    }
}