using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel;
using MVVM.ViewModel;

namespace MVVM_03a_CTNotifyChange.ViewModels.Tests
{
    [TestClass()]
    public class MainWindowViewModelTests : BaseTestViewModel
    {
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erw�gen Sie die Deklaration als Nullable.
        MainWindowViewModel testModel;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erw�gen Sie die Deklaration als Nullable.

        [TestInitialize]
        public void Init()
        {
            testModel = new();
            testModel.PropertyChanging += OnVMPropertyChanging;
            testModel.PropertyChanged += OnVMPropertyChanged;
            ClearLog();
        }

        [TestMethod()]
        public void SetupTest()
        {
            Assert.IsNotNull(testModel);
            Assert.IsInstanceOfType(testModel, typeof(MainWindowViewModel));
            Assert.IsInstanceOfType(testModel, typeof(BaseViewModelCT));
            Assert.IsInstanceOfType(testModel, typeof(INotifyPropertyChanged));
            Assert.IsInstanceOfType(testModel, typeof(INotifyPropertyChanging));
        }
    }
}
