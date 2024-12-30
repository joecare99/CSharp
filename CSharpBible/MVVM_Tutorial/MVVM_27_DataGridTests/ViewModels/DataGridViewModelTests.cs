using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MVVM_27_DataGrid.ViewModels.Tests
{
    [TestClass]
    public class DataGridViewModelTests
    {
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
        DataGridViewModel testModel;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.

        [TestInitialize]
        public void Init()
        {
            testModel = new DataGridViewModel();
        }

        [TestMethod]
        public void SetupTest() {
            Assert.IsNotNull(testModel);
            Assert.IsInstanceOfType(testModel, typeof(DataGridViewModel));
        }
    }
}
