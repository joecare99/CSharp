using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel;
using MVVM.ViewModel;

namespace MVVM_24a_CTUserControl.ViewModels.Tests
{
    [TestClass()]
    public class UserControlViewModelTests: BaseTestViewModel<UserControlViewModel>
	{
        [TestMethod()]
        public void SetupTest()
        {
            Assert.IsNotNull(testModel);
            Assert.IsNotNull(testModel2);
            Assert.IsInstanceOfType(testModel, typeof(UserControlViewModel));
            Assert.IsInstanceOfType(testModel, typeof(BaseViewModelCT));
            Assert.IsInstanceOfType(testModel, typeof(INotifyPropertyChanged));
        }
    }
}
