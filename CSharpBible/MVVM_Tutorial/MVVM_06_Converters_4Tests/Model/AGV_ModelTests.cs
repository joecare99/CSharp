using Microsoft.VisualStudio.TestTools.UnitTesting;
using MVVM.ViewModel;
using MVVM_06_Converters_4.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVM_06_Converters_4.Model.Tests
{
    [TestClass()]
    public class AGV_ModelTests:BaseTestViewModel<AGV_Model>
    {
        [TestMethod()]
        public void AGV_ModelTest()
        {
            Assert.IsNotNull(testModel);
            Assert.IsNotNull(testModel2);
            Assert.IsInstanceOfType(testModel, typeof(AGV_Model));
            Assert.IsInstanceOfType(testModel, typeof(NotificationObject));
        }

        [TestMethod()]
        public void SaveTest()
        {
            Assert.Fail();
        }
    }
}