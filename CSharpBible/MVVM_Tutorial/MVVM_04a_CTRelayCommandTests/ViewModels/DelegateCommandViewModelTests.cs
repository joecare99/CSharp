using BaseLib.Helper;
using CommunityToolkit.Mvvm.Input;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MVVM.ViewModel;
using System;
using System.ComponentModel;
using static BaseLib.Helper.TestHelper;

namespace MVVM_04a_CTRelayCommand.ViewModels.Tests
{
    [TestClass()]
    public class DelegateCommandViewModelTests : BaseTestViewModel
    {
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
        DelegateCommandViewModel testModel;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.

        [TestInitialize]
        public void Init()
        {
            testModel = new();
            testModel.PropertyChanged += OnVMPropertyChanged;
            testModel.ClearCommand.CanExecuteChanged += OnCanExChanged;
            ClearLog();
        }

        [TestMethod]
        public void DelegateCommandViewModelTest()
        {
            Assert.IsNotNull(testModel);
            Assert.IsInstanceOfType(testModel, typeof(BaseViewModel));
            Assert.IsInstanceOfType(testModel, typeof(DelegateCommandViewModel));
            Assert.IsNotNull(testModel.ClearCommand);
            Assert.IsInstanceOfType(testModel.ClearCommand, typeof(RelayCommand<object>));
            Assert.AreEqual("Dave", testModel.Firstname);
            Assert.AreEqual("Dev", testModel.Lastname);
            Assert.AreEqual("Dev, Dave", testModel.Fullname);
            Assert.AreEqual("", DebugLog);
        }

        [DataTestMethod()]
        [DataRow("", new string[] { "","Dev, ", "PropChg(MVVM_04a_CTRelayCommand.ViewModels.DelegateCommandViewModel,Fullname)=Dev, \r\nCanExChanged(CommunityToolkit.Mvvm.Input.RelayCommand`1[System.Object])=True\r\nPropChg(MVVM_04a_CTRelayCommand.ViewModels.DelegateCommandViewModel,Firstname)=\r\n" })]
        [DataRow("Peter", new string[] { "Peter", "Dev, Peter", "PropChg(MVVM_04a_CTRelayCommand.ViewModels.DelegateCommandViewModel,Fullname)=Dev, Peter\r\nCanExChanged(CommunityToolkit.Mvvm.Input.RelayCommand`1[System.Object])=True\r\nPropChg(MVVM_04a_CTRelayCommand.ViewModels.DelegateCommandViewModel,Firstname)=Peter\r\n" })]
        [DataRow("Steve\tEugene", new string[] { "Eugene", "Dev, Eugene", @"PropChg(MVVM_04a_CTRelayCommand.ViewModels.DelegateCommandViewModel,Fullname)=Dev, Steve
CanExChanged(CommunityToolkit.Mvvm.Input.RelayCommand`1[System.Object])=True
PropChg(MVVM_04a_CTRelayCommand.ViewModels.DelegateCommandViewModel,Firstname)=Steve
PropChg(MVVM_04a_CTRelayCommand.ViewModels.DelegateCommandViewModel,Fullname)=Dev, Eugene
CanExChanged(CommunityToolkit.Mvvm.Input.RelayCommand`1[System.Object])=True
PropChg(MVVM_04a_CTRelayCommand.ViewModels.DelegateCommandViewModel,Firstname)=Eugene
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
        [DataRow("", new string[] { "", ", Dave", "PropChg(MVVM_04a_CTRelayCommand.ViewModels.DelegateCommandViewModel,Fullname)=, Dave\r\nCanExChanged(CommunityToolkit.Mvvm.Input.RelayCommand`1[System.Object])=True\r\nPropChg(MVVM_04a_CTRelayCommand.ViewModels.DelegateCommandViewModel,Lastname)=\r\n" })]
        [DataRow("Miller", new string[] { "Miller", "Miller, Dave", "PropChg(MVVM_04a_CTRelayCommand.ViewModels.DelegateCommandViewModel,Fullname)=Miller, Dave\r\nCanExChanged(CommunityToolkit.Mvvm.Input.RelayCommand`1[System.Object])=True\r\nPropChg(MVVM_04a_CTRelayCommand.ViewModels.DelegateCommandViewModel,Lastname)=Miller\r\n" })]
        [DataRow("Fry\tWebb", new string[] { "Webb", "Webb, Dave", @"PropChg(MVVM_04a_CTRelayCommand.ViewModels.DelegateCommandViewModel,Fullname)=Fry, Dave
CanExChanged(CommunityToolkit.Mvvm.Input.RelayCommand`1[System.Object])=True
PropChg(MVVM_04a_CTRelayCommand.ViewModels.DelegateCommandViewModel,Lastname)=Fry
PropChg(MVVM_04a_CTRelayCommand.ViewModels.DelegateCommandViewModel,Fullname)=Webb, Dave
CanExChanged(CommunityToolkit.Mvvm.Input.RelayCommand`1[System.Object])=True
PropChg(MVVM_04a_CTRelayCommand.ViewModels.DelegateCommandViewModel,Lastname)=Webb
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

        [TestMethod]
        public void ClearCommandExecuteTest()
        {
            testModel.ClearCommand.Execute(null);
            Assert.AreEqual("", testModel.Firstname);
            Assert.AreEqual("", testModel.Lastname);
            Assert.AreEqual(", ", testModel.Fullname);
            AssertAreEqual(@"PropChg(MVVM_04a_CTRelayCommand.ViewModels.DelegateCommandViewModel,Fullname)=Dev, 
CanExChanged(CommunityToolkit.Mvvm.Input.RelayCommand`1[System.Object])=True
PropChg(MVVM_04a_CTRelayCommand.ViewModels.DelegateCommandViewModel,Firstname)=
PropChg(MVVM_04a_CTRelayCommand.ViewModels.DelegateCommandViewModel,Fullname)=, 
CanExChanged(CommunityToolkit.Mvvm.Input.RelayCommand`1[System.Object])=False
PropChg(MVVM_04a_CTRelayCommand.ViewModels.DelegateCommandViewModel,Lastname)=
", DebugLog);
        }

    }
}
