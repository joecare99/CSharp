using Microsoft.VisualStudio.TestTools.UnitTesting;
using MVVM.ViewModel;
using System;
using System.ComponentModel;

namespace MVVM_31a_CTValidation1.ViewModels.Tests;

[TestClass()]
public class ValidationPageViewModelTests :BaseTestViewModel
{
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
    ValidationPageViewModel testModel;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.

    [TestInitialize]
    public void Init()
    {
        testModel = new();
        testModel.PropertyChanging += OnVMPropertyChanging;
        testModel.PropertyChanged += OnVMPropertyChanged;
    }

    [TestMethod()]
    public void SetupTest()
    {
        Assert.IsNotNull(testModel);
        Assert.IsInstanceOfType(testModel, typeof(ValidationPageViewModel));
        Assert.IsInstanceOfType(testModel, typeof(BaseViewModelCT));
        Assert.IsInstanceOfType(testModel, typeof(INotifyPropertyChanged));
        Assert.IsInstanceOfType(testModel, typeof(INotifyPropertyChanging));
        Assert.IsInstanceOfType(testModel, typeof(INotifyDataErrorInfo));
        Assert.AreEqual("", DebugLog);
    }

    [DataTestMethod()]
    [DataRow("",1,false)]
    [DataRow("DS", 2, false)]
    [DataRow("DS1234", 0, true)]
    public void TestUsernameTest(string sVal, int iErg, bool _)
    {
        void f(string s) => testModel.UserName = s;
        switch (iErg)
        {
            case 1: Assert.ThrowsException<ArgumentNullException>(() => f(sVal)); break;
            case 2: Assert.ThrowsException<ArgumentException>(() => f(sVal)); break;
            default: f(sVal); break;
        }
    }
}
