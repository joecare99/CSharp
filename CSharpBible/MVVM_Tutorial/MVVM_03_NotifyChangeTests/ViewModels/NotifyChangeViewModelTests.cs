using Microsoft.VisualStudio.TestTools.UnitTesting;
using MVVM.ViewModel;

namespace MVVM_03_NotifyChange.ViewModels.Tests
{
    [TestClass()]
    public class NotifyChangeViewModelTests : BaseTestViewModel
    {
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
        NotifyChangeViewModel testModel;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.

        [TestInitialize]
        public void Init()
        {
            testModel = new();
            testModel.PropertyChanged += OnVMPropertyChanged;
            ClearLog();
        }

        [TestMethod]
        public void NotifyChangeViewModelTest()
        {
            Assert.IsNotNull(testModel);
            Assert.IsInstanceOfType(testModel, typeof(BaseViewModel));
            Assert.IsInstanceOfType(testModel, typeof(NotifyChangeViewModel));
            Assert.AreEqual("Dave", testModel.Firstname);
            Assert.AreEqual("Dev", testModel.Lastname);
            Assert.AreEqual("Dev, Dave", testModel.Fullname);
            Assert.AreEqual("", DebugLog);
        }

        [DataTestMethod()]
        [DataRow("", new string[] { "","Dev, ", "PropChg(MVVM_03_NotifyChange.ViewModels.NotifyChangeViewModel,Fullname)=Dev, \r\nPropChg(MVVM_03_NotifyChange.ViewModels.NotifyChangeViewModel,Firstname)=\r\n" })]
        [DataRow("Peter", new string[] { "Peter", "Dev, Peter", "PropChg(MVVM_03_NotifyChange.ViewModels.NotifyChangeViewModel,Fullname)=Dev, Peter\r\nPropChg(MVVM_03_NotifyChange.ViewModels.NotifyChangeViewModel,Firstname)=Peter\r\n" })]
        [DataRow("Steve\tEugene", new string[] { "Eugene", "Dev, Eugene", @"PropChg(MVVM_03_NotifyChange.ViewModels.NotifyChangeViewModel,Fullname)=Dev, Steve
PropChg(MVVM_03_NotifyChange.ViewModels.NotifyChangeViewModel,Firstname)=Steve
PropChg(MVVM_03_NotifyChange.ViewModels.NotifyChangeViewModel,Fullname)=Dev, Eugene
PropChg(MVVM_03_NotifyChange.ViewModels.NotifyChangeViewModel,Firstname)=Eugene
" })]
        public void FirstnameTest(string name,  string[] asExp)
        {
            // Arrange
            var asName=name.Split('\t');
            // Act    
            foreach (var item in asName) 
                testModel.Firstname = item;
            // Assert
            Assert.AreEqual(asExp[0], testModel.Firstname,"Firstname");
            Assert.AreEqual(asExp[1], testModel.Fullname, "Fullname");
            Assert.AreEqual(asExp[2], DebugLog, "DebugOut");
        }

        [DataTestMethod()]
        [DataRow("", new string[] { "", ", Dave", "PropChg(MVVM_03_NotifyChange.ViewModels.NotifyChangeViewModel,Fullname)=, Dave\r\nPropChg(MVVM_03_NotifyChange.ViewModels.NotifyChangeViewModel,Lastname)=\r\n" })]
        [DataRow("Miller", new string[] { "Miller", "Miller, Dave", "PropChg(MVVM_03_NotifyChange.ViewModels.NotifyChangeViewModel,Fullname)=Miller, Dave\r\nPropChg(MVVM_03_NotifyChange.ViewModels.NotifyChangeViewModel,Lastname)=Miller\r\n" })]
        [DataRow("Fry\tWebb", new string[] { "Webb", "Webb, Dave", @"PropChg(MVVM_03_NotifyChange.ViewModels.NotifyChangeViewModel,Fullname)=Fry, Dave
PropChg(MVVM_03_NotifyChange.ViewModels.NotifyChangeViewModel,Lastname)=Fry
PropChg(MVVM_03_NotifyChange.ViewModels.NotifyChangeViewModel,Fullname)=Webb, Dave
PropChg(MVVM_03_NotifyChange.ViewModels.NotifyChangeViewModel,Lastname)=Webb
" })]
        public void LastnameTest(string name, string[] asExp)
        {
            // Arrange
            var asName = name.Split('\t');
            // Act    
            foreach (var item in asName)
                testModel.Lastname = item;
            // Assert
            Assert.AreEqual(asExp[0], testModel.Lastname, "Firstname");
            Assert.AreEqual(asExp[1], testModel.Fullname, "Fullname");
            Assert.AreEqual(asExp[2], DebugLog, "DebugOut");
        }

    }
}