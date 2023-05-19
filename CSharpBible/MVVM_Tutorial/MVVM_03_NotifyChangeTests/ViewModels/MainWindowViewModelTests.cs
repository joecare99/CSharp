using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel;
using MVVM.ViewModel;

namespace MVVM_03_NotifyChange.ViewModels.Tests
{
    [TestClass()]
    public class MainWindowViewModelTests: BaseTestViewModel
    {
        MainWindowViewModel testModel;

        [TestInitialize]
        public void Init()
        {
            testModel = new();
            testModel.PropertyChanged += OnVMPropertyChanged;
            ClearLog();
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