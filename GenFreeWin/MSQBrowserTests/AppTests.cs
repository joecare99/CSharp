using Microsoft.VisualStudio.TestTools.UnitTesting;
using MSQBrowser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVVM.Views.Extension;
using MSQBrowser.ViewModels.Interfaces;
using BaseLib.Helper;

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
            Assert.IsNotNull(IoC.GetRequiredService<ITableViewViewModel>());
        }
    }
}
