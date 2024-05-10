using Microsoft.VisualStudio.TestTools.UnitTesting;
using MVVM.ViewModel;

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
            Assert.IsInstanceOfType(testModel, typeof(NotificationObjectCT));
        }

        [TestMethod()]
        public void SaveTest()
        {
            testModel.Save();
            Assert.IsFalse(testModel.IsDirty);
        }
    }
}