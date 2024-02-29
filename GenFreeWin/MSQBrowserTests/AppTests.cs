using Microsoft.VisualStudio.TestTools.UnitTesting;
using MSQBrowser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVVM.View.Extension;
using MSQBrowser.ViewModels.Interfaces;

namespace MSQBrowser.Tests
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
