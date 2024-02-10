using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel;
using MVVM.ViewModel;
using MVVM_BaseLibTests.ViewModel;

namespace MVVM_24_UserControl.ViewModels.Tests
{
    [TestClass()]
    public class UserControlViewModelTests: BaseTestViewModel<UserControlViewModel>
	{
        [TestMethod()]
        public void SetupTest()
        {
            Assert.IsNotNull(testModel);
            Assert.IsNotNull(testModel2);
            Assert.IsInstanceOfType(testModel, typeof(MainWindowViewModel));
            Assert.IsInstanceOfType(testModel, typeof(BaseViewModel));
            Assert.IsInstanceOfType(testModel, typeof(INotifyPropertyChanged));
        }
    }
}
