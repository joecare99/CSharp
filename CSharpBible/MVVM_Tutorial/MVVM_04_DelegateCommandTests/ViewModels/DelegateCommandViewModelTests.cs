using BaseLib.Helper;
using CommunityToolkit.Mvvm.Input;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MVVM.ViewModel;
using System;
using System.ComponentModel;

namespace MVVM_04_DelegateCommand.ViewModels.Tests
{
    [TestClass()]
    public class DelegateCommandViewModelTests
    {
        DelegateCommandViewModel testModel;

        public string DebugOut = "";

        [TestInitialize]
        public void Init()
        {
            testModel = new();
            testModel.PropertyChanged += OnPropertyChanged;
            testModel.ClearCommand.CanExecuteChanged += OnCanExecuteChanged;
            DebugOut = "";
        }

        private void OnCanExecuteChanged(object? sender, EventArgs e)
        {
            DoLog($"CanExecuteChange({sender})={(sender as RelayCommand<object>)?.CanExecute(e)}");
        }

        private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            DoLog($"PropChange({sender},{e.PropertyName})={sender.GetProp(e.PropertyName)}");
        }

        private void DoLog(string v)
        {
            DebugOut += $"{v}{Environment.NewLine}";
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
            Assert.AreEqual("", DebugOut);
        }

        [DataTestMethod()]
        [DataRow("", new string[] { "","Dev, ", "PropChange(MVVM_04_DelegateCommand.ViewModels.DelegateCommandViewModel,Fullname)=Dev, \r\nCanExecuteChange(CommunityToolkit.Mvvm.Input.RelayCommand`1[System.Object])=True\r\nPropChange(MVVM_04_DelegateCommand.ViewModels.DelegateCommandViewModel,Firstname)=\r\n" })]
        [DataRow("Peter", new string[] { "Peter", "Dev, Peter", "PropChange(MVVM_04_DelegateCommand.ViewModels.DelegateCommandViewModel,Fullname)=Dev, Peter\r\nCanExecuteChange(CommunityToolkit.Mvvm.Input.RelayCommand`1[System.Object])=True\r\nPropChange(MVVM_04_DelegateCommand.ViewModels.DelegateCommandViewModel,Firstname)=Peter\r\n" })]
        [DataRow("Steve\tEugene", new string[] { "Eugene", "Dev, Eugene", @"PropChange(MVVM_04_DelegateCommand.ViewModels.DelegateCommandViewModel,Fullname)=Dev, Steve
CanExecuteChange(CommunityToolkit.Mvvm.Input.RelayCommand`1[System.Object])=True
PropChange(MVVM_04_DelegateCommand.ViewModels.DelegateCommandViewModel,Firstname)=Steve
PropChange(MVVM_04_DelegateCommand.ViewModels.DelegateCommandViewModel,Fullname)=Dev, Eugene
CanExecuteChange(CommunityToolkit.Mvvm.Input.RelayCommand`1[System.Object])=True
PropChange(MVVM_04_DelegateCommand.ViewModels.DelegateCommandViewModel,Firstname)=Eugene
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
            Assert.AreEqual(asExp[2], DebugOut, "DebugOut");
        }

        [DataTestMethod()]
        [DataRow("", new string[] { "", ", Dave", "PropChange(MVVM_04_DelegateCommand.ViewModels.DelegateCommandViewModel,Fullname)=, Dave\r\nCanExecuteChange(CommunityToolkit.Mvvm.Input.RelayCommand`1[System.Object])=True\r\nPropChange(MVVM_04_DelegateCommand.ViewModels.DelegateCommandViewModel,Lastname)=\r\n" })]
        [DataRow("Miller", new string[] { "Miller", "Miller, Dave", "PropChange(MVVM_04_DelegateCommand.ViewModels.DelegateCommandViewModel,Fullname)=Miller, Dave\r\nCanExecuteChange(CommunityToolkit.Mvvm.Input.RelayCommand`1[System.Object])=True\r\nPropChange(MVVM_04_DelegateCommand.ViewModels.DelegateCommandViewModel,Lastname)=Miller\r\n" })]
        [DataRow("Fry\tWebb", new string[] { "Webb", "Webb, Dave", @"PropChange(MVVM_04_DelegateCommand.ViewModels.DelegateCommandViewModel,Fullname)=Fry, Dave
CanExecuteChange(CommunityToolkit.Mvvm.Input.RelayCommand`1[System.Object])=True
PropChange(MVVM_04_DelegateCommand.ViewModels.DelegateCommandViewModel,Lastname)=Fry
PropChange(MVVM_04_DelegateCommand.ViewModels.DelegateCommandViewModel,Fullname)=Webb, Dave
CanExecuteChange(CommunityToolkit.Mvvm.Input.RelayCommand`1[System.Object])=True
PropChange(MVVM_04_DelegateCommand.ViewModels.DelegateCommandViewModel,Lastname)=Webb
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
            Assert.AreEqual(asExp[2], DebugOut, "DebugOut");
        }

        [TestMethod]
        public void ClearCommandExecuteTest()
        {
            testModel.ClearCommand.Execute(null);
            Assert.AreEqual("", testModel.Firstname);
            Assert.AreEqual("", testModel.Lastname);
            Assert.AreEqual(", ", testModel.Fullname);
            Assert.AreEqual(@"PropChange(MVVM_04_DelegateCommand.ViewModels.DelegateCommandViewModel,Fullname)=Dev, 
CanExecuteChange(CommunityToolkit.Mvvm.Input.RelayCommand`1[System.Object])=True
PropChange(MVVM_04_DelegateCommand.ViewModels.DelegateCommandViewModel,Firstname)=
PropChange(MVVM_04_DelegateCommand.ViewModels.DelegateCommandViewModel,Fullname)=, 
CanExecuteChange(CommunityToolkit.Mvvm.Input.RelayCommand`1[System.Object])=False
PropChange(MVVM_04_DelegateCommand.ViewModels.DelegateCommandViewModel,Lastname)=
", DebugOut);
        }

    }
}