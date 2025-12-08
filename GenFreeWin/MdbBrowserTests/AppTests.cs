using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using MVVM.View.Extension;
using MdbBrowser.ViewModels.Interfaces;
using BaseLib.Helper;

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
            Assert.ThrowsExactly<InvalidOperationException>(()=>_=IoC.GetRequiredService<IDBViewViewModel>());
        }
    }
}