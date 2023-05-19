using Microsoft.VisualStudio.TestTools.UnitTesting;
using MVVM.ViewModel;
using static BaseLib.Helper.TestHelper;

namespace MVVM_03a_CTNotifyChange.ViewModels.Tests
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
            testModel.PropertyChanging += OnVMPropertyChanging;
            testModel.PropertyChanged += OnVMPropertyChanged;
            ClearLog();
        }

        [TestMethod]
        public void NotifyChangeViewModelTest()
        {
            Assert.IsNotNull(testModel);
            Assert.IsInstanceOfType(testModel, typeof(BaseViewModelCT));
            Assert.IsInstanceOfType(testModel, typeof(NotifyChangeViewModel));
            Assert.AreEqual("Dave", testModel.Firstname);
            Assert.AreEqual("Dev", testModel.Lastname);
            Assert.AreEqual("Dev, Dave", testModel.Fullname);
            Assert.AreEqual("", DebugLog);
        }

        [DataTestMethod()]
        [DataRow("", new string[] { "","Dev, ", @"PropChgn(MVVM_03a_CTNotifyChange.ViewModels.NotifyChangeViewModel,Firstname)=Dave
PropChg(MVVM_03a_CTNotifyChange.ViewModels.NotifyChangeViewModel,Firstname)=
PropChg(MVVM_03a_CTNotifyChange.ViewModels.NotifyChangeViewModel,Fullname)=Dev, 
" })]
        [DataRow("Peter", new string[] { "Peter", "Dev, Peter", @"PropChgn(MVVM_03a_CTNotifyChange.ViewModels.NotifyChangeViewModel,Firstname)=Dave
PropChg(MVVM_03a_CTNotifyChange.ViewModels.NotifyChangeViewModel,Firstname)=Peter
PropChg(MVVM_03a_CTNotifyChange.ViewModels.NotifyChangeViewModel,Fullname)=Dev, Peter
" })]
        [DataRow("Steve\tEugene", new string[] { "Eugene", "Dev, Eugene", @"PropChgn(MVVM_03a_CTNotifyChange.ViewModels.NotifyChangeViewModel,Firstname)=Dave
PropChg(MVVM_03a_CTNotifyChange.ViewModels.NotifyChangeViewModel,Firstname)=Steve
PropChg(MVVM_03a_CTNotifyChange.ViewModels.NotifyChangeViewModel,Fullname)=Dev, Steve
PropChgn(MVVM_03a_CTNotifyChange.ViewModels.NotifyChangeViewModel,Firstname)=Steve
PropChg(MVVM_03a_CTNotifyChange.ViewModels.NotifyChangeViewModel,Firstname)=Eugene
PropChg(MVVM_03a_CTNotifyChange.ViewModels.NotifyChangeViewModel,Fullname)=Dev, Eugene
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
            AssertAreEqual(asExp[2], DebugLog, "DebugOut");
        }

        [DataTestMethod()]
        [DataRow("", new string[] { "", ", Dave", @"PropChgn(MVVM_03a_CTNotifyChange.ViewModels.NotifyChangeViewModel,Lastname)=Dev
PropChg(MVVM_03a_CTNotifyChange.ViewModels.NotifyChangeViewModel,Lastname)=
PropChg(MVVM_03a_CTNotifyChange.ViewModels.NotifyChangeViewModel,Fullname)=, Dave
" })]
        [DataRow("Miller", new string[] { "Miller", "Miller, Dave", @"PropChgn(MVVM_03a_CTNotifyChange.ViewModels.NotifyChangeViewModel,Lastname)=Dev
PropChg(MVVM_03a_CTNotifyChange.ViewModels.NotifyChangeViewModel,Lastname)=Miller
PropChg(MVVM_03a_CTNotifyChange.ViewModels.NotifyChangeViewModel,Fullname)=Miller, Dave
" })]
        [DataRow("Fry\tWebb", new string[] { "Webb", "Webb, Dave", @"PropChgn(MVVM_03a_CTNotifyChange.ViewModels.NotifyChangeViewModel,Lastname)=Dev
PropChg(MVVM_03a_CTNotifyChange.ViewModels.NotifyChangeViewModel,Lastname)=Fry
PropChg(MVVM_03a_CTNotifyChange.ViewModels.NotifyChangeViewModel,Fullname)=Fry, Dave
PropChgn(MVVM_03a_CTNotifyChange.ViewModels.NotifyChangeViewModel,Lastname)=Fry
PropChg(MVVM_03a_CTNotifyChange.ViewModels.NotifyChangeViewModel,Lastname)=Webb
PropChg(MVVM_03a_CTNotifyChange.ViewModels.NotifyChangeViewModel,Fullname)=Webb, Dave
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
            AssertAreEqual(asExp[2], DebugLog, "DebugOut");
        }

    }
}
