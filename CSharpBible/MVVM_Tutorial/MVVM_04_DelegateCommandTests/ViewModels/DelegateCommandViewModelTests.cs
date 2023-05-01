using BaseLib.Helper;
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
            DebugOut = "";
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
            Assert.AreEqual("Dave", testModel.Firstname);
            Assert.AreEqual("Dev", testModel.Lastname);
            Assert.AreEqual("Dev, Dave", testModel.Fullname);
            Assert.AreEqual("", DebugOut);
        }

        [DataTestMethod()]
        [DataRow("", new string[] { "","Dev, ", "PropChange(MVVM_04_DelegateCommand.ViewModels.DelegateCommandViewModel,Fullname)=Dev, \r\nPropChange(MVVM_04_DelegateCommand.ViewModels.DelegateCommandViewModel,Firstname)=\r\n" })]
        [DataRow("Peter", new string[] { "Peter", "Dev, Peter", "PropChange(MVVM_04_DelegateCommand.ViewModels.DelegateCommandViewModel,Fullname)=Dev, Peter\r\nPropChange(MVVM_04_DelegateCommand.ViewModels.DelegateCommandViewModel,Firstname)=Peter\r\n" })]
        [DataRow("Steve\tEugene", new string[] { "Eugene", "Dev, Eugene", @"PropChange(MVVM_04_DelegateCommand.ViewModels.DelegateCommandViewModel,Fullname)=Dev, Steve
PropChange(MVVM_04_DelegateCommand.ViewModels.DelegateCommandViewModel,Firstname)=Steve
PropChange(MVVM_04_DelegateCommand.ViewModels.DelegateCommandViewModel,Fullname)=Dev, Eugene
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
        [DataRow("", new string[] { "", ", Dave", "PropChange(MVVM_04_DelegateCommand.ViewModels.DelegateCommandViewModel,Fullname)=, Dave\r\nPropChange(MVVM_04_DelegateCommand.ViewModels.DelegateCommandViewModel,Lastname)=\r\n" })]
        [DataRow("Miller", new string[] { "Miller", "Miller, Dave", "PropChange(MVVM_04_DelegateCommand.ViewModels.DelegateCommandViewModel,Fullname)=Miller, Dave\r\nPropChange(MVVM_04_DelegateCommand.ViewModels.DelegateCommandViewModel,Lastname)=Miller\r\n" })]
        [DataRow("Fry\tWebb", new string[] { "Webb", "Webb, Dave", @"PropChange(MVVM_04_DelegateCommand.ViewModels.DelegateCommandViewModel,Fullname)=Fry, Dave
PropChange(MVVM_04_DelegateCommand.ViewModels.DelegateCommandViewModel,Lastname)=Fry
PropChange(MVVM_04_DelegateCommand.ViewModels.DelegateCommandViewModel,Fullname)=Webb, Dave
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

    }
}