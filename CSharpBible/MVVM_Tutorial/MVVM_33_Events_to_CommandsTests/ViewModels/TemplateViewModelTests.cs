using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel;
using MVVM.ViewModel;

namespace MVVM_33_Events_To_Commands.ViewModels.Tests
{
    [TestClass()]
    public class TemplateViewModelTests
    {
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
        TemplateViewModel testModel;
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
            Assert.IsInstanceOfType(testModel, typeof(TemplateViewModel));
            Assert.IsInstanceOfType(testModel, typeof(BaseViewModel));
            Assert.IsInstanceOfType(testModel, typeof(INotifyPropertyChanged));
        }
    }
}