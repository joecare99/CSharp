using Microsoft.VisualStudio.TestTools.UnitTesting;
using MVVM.ViewModel;
using BaseLib.Helper.MVVM;

namespace MVVM_31_Validation2.ViewModels.Tests;

[TestClass()]
public class ValidationPageViewModelTests : BaseTestViewModel
{
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
    ValidationPageViewModel testModel;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.

    [TestInitialize]
    public void Init()
    {
        testModel = new();
        testModel.PropertyChanged += OnVMPropertyChanged;
        testModel.ErrorsChanged += OnVMErrorsChanged;
    }

    [DataTestMethod()]
    [DataRow("", 1, false, new[] { "PropChg(MVVM_31_Validation2.ViewModels.ValidationPageViewModel,VHelper)=BaseLib.Helper.MVVM.ValidationHelper\r\nErrorsChanged(MVVM_31_Validation2.ViewModels.ValidationPageViewModel,UserName)=\r\nPropChg(MVVM_31_Validation2.ViewModels.ValidationPageViewModel,VHelper)=BaseLib.Helper.MVVM.ValidationHelper\r\nErrorsChanged(MVVM_31_Validation2.ViewModels.ValidationPageViewModel,UserName)=Username must not be empty\r\n" })]
    [DataRow("DS", 2, false, new[] { "PropChg(MVVM_31_Validation2.ViewModels.ValidationPageViewModel,VHelper)=BaseLib.Helper.MVVM.ValidationHelper\r\nErrorsChanged(MVVM_31_Validation2.ViewModels.ValidationPageViewModel,UserName)=\r\nPropChg(MVVM_31_Validation2.ViewModels.ValidationPageViewModel,VHelper)=BaseLib.Helper.MVVM.ValidationHelper\r\nErrorsChanged(MVVM_31_Validation2.ViewModels.ValidationPageViewModel,UserName)=Username must have min. 6 Chars\r\n" })]
    [DataRow("DS1234", 0, true, new[] { "PropChg(MVVM_31_Validation2.ViewModels.ValidationPageViewModel,VHelper)=BaseLib.Helper.MVVM.ValidationHelper\r\nErrorsChanged(MVVM_31_Validation2.ViewModels.ValidationPageViewModel,UserName)=\r\n" })]
    public void TestUsernameTest(string sVal, int _, bool xExp, string[] asExp)
    {
        Assert.AreEqual(true,testModel.TestUsername(sVal,nameof(testModel.UserName)));
        Assert.AreEqual(!xExp, testModel.HasErrors);
        Assert.AreEqual(asExp[0], DebugLog);
    }

    [TestMethod()]
    public void TestUsername1Test()
    {
        testModel.ErrorsChanged -= OnVMErrorsChanged;
        testModel.UserName = "DS12";
        Assert.AreEqual(true, testModel.HasErrors);
        Assert.AreEqual("Username must have min. 6 Chars", testModel.ValidationText(nameof(testModel.UserName)));
        Assert.AreEqual("PropChg(MVVM_31_Validation2.ViewModels.ValidationPageViewModel,VHelper)=BaseLib.Helper.MVVM.ValidationHelper\r\nPropChg(MVVM_31_Validation2.ViewModels.ValidationPageViewModel,VHelper)=BaseLib.Helper.MVVM.ValidationHelper\r\nPropChg(MVVM_31_Validation2.ViewModels.ValidationPageViewModel,UserName)=DS12\r\n", DebugLog);
    }
}