using Microsoft.VisualStudio.TestTools.UnitTesting;
using MVVM.ViewModel;
using System.ComponentModel;

namespace MVVM_31a_CTValidation3.ViewModels.Tests
{
    [TestClass()]
    public class MainWindowViewModelTests
    {
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
        MainWindowViewModel testModel;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.

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
