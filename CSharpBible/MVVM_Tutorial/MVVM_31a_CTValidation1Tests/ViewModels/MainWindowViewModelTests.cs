using Microsoft.VisualStudio.TestTools.UnitTesting;
using MVVM.ViewModel;
using System.ComponentModel;

namespace MVVM_31a_CTValidation1.ViewModels.Tests
{
    [TestClass()]
    public class MainWindowViewModelTests
    {
        MainWindowViewModel testModel;

        [TestInitialize]
        public void Init()
        {
            testModel = new();
        }

        [TestMethod()]
        public void SetupTest()
        {
            Assert.IsNotNull(testModel);
            Assert.IsInstanceOfType(testModel, typeof(MainWindowViewModel));
            Assert.IsInstanceOfType(testModel, typeof(BaseViewModel));
            Assert.IsInstanceOfType(testModel, typeof(INotifyPropertyChanged));
        }
    }
}
