using Microsoft.VisualStudio.TestTools.UnitTesting;
using MdbBrowser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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