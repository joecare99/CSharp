using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.ComponentModel;
using MVVM.ViewModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace MVVM_31a_CTValidation3.ViewModels.Tests;

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
        testModel.PropertyChanging += OnVMPropertyChanging;
        testModel.ErrorsChanged += OnVMErrorsChanged;
        CultureInfo.CurrentUICulture = CultureInfo.InvariantCulture;    
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
    [DataRow("", 1, false, new[] { @"PropChgn(MVVM_31a_CTValidation3.ViewModels.ValidationPageViewModel,UserName)=.
PropChg(MVVM_31a_CTValidation3.ViewModels.ValidationPageViewModel,HasErrors)=True
ErrorsChanged(MVVM_31a_CTValidation3.ViewModels.ValidationPageViewModel,UserName)=UserName may not be empty
PropChg(MVVM_31a_CTValidation3.ViewModels.ValidationPageViewModel,UserName)=
PropChg(MVVM_31a_CTValidation3.ViewModels.ValidationPageViewModel,TTUserName)=UserName may not be empty
" })]
    [DataRow("DS", 2, false, new[] { @"PropChgn(MVVM_31a_CTValidation3.ViewModels.ValidationPageViewModel,UserName)=.
PropChg(MVVM_31a_CTValidation3.ViewModels.ValidationPageViewModel,HasErrors)=True
ErrorsChanged(MVVM_31a_CTValidation3.ViewModels.ValidationPageViewModel,UserName)=UserName must have min. 6 Chars
PropChg(MVVM_31a_CTValidation3.ViewModels.ValidationPageViewModel,UserName)=DS
PropChg(MVVM_31a_CTValidation3.ViewModels.ValidationPageViewModel,TTUserName)=UserName must have min. 6 Chars
" })]
    [DataRow("DS1234", 0, true, new[] { @"PropChgn(MVVM_31a_CTValidation3.ViewModels.ValidationPageViewModel,UserName)=.
PropChg(MVVM_31a_CTValidation3.ViewModels.ValidationPageViewModel,UserName)=DS1234
PropChg(MVVM_31a_CTValidation3.ViewModels.ValidationPageViewModel,TTUserName)=
" })]
    [DataRow("BlaBla", 2, false, new[] { @"PropChgn(MVVM_31a_CTValidation3.ViewModels.ValidationPageViewModel,UserName)=.
PropChg(MVVM_31a_CTValidation3.ViewModels.ValidationPageViewModel,HasErrors)=True
ErrorsChanged(MVVM_31a_CTValidation3.ViewModels.ValidationPageViewModel,UserName)=UserName is a known name 
PropChg(MVVM_31a_CTValidation3.ViewModels.ValidationPageViewModel,UserName)=BlaBla
PropChg(MVVM_31a_CTValidation3.ViewModels.ValidationPageViewModel,TTUserName)=UserName is a known name 
" })]
    [DataRow("Dududu", 2, false, new[] { @"PropChgn(MVVM_31a_CTValidation3.ViewModels.ValidationPageViewModel,UserName)=.
PropChg(MVVM_31a_CTValidation3.ViewModels.ValidationPageViewModel,HasErrors)=True
ErrorsChanged(MVVM_31a_CTValidation3.ViewModels.ValidationPageViewModel,UserName)=UserName Somethin went wrong
PropChg(MVVM_31a_CTValidation3.ViewModels.ValidationPageViewModel,UserName)=Dududu
PropChg(MVVM_31a_CTValidation3.ViewModels.ValidationPageViewModel,TTUserName)=UserName Somethin went wrong
" })]
    public void TestUsernameTest(string sVal, int _, bool xExp, string[] asExp)
    {
        testModel.UserName = sVal;
        Assert.AreEqual(!xExp, testModel.HasErrors);
        Assert.AreEqual(asExp[0], DebugLog);
    }

    [TestMethod()]
    public void TestUsername1Test()
    {
        testModel.ErrorsChanged -= OnVMErrorsChanged;
        testModel.UserName = "DS12";
        Assert.AreEqual(true, testModel.HasErrors);
        Assert.AreEqual("UserName must have min. 6 Chars", string.Join(",", ((List<ValidationResult>)testModel.GetErrors(nameof(testModel.UserName))).ConvertAll(o=>o.ErrorMessage)));
        Assert.AreEqual(@"PropChgn(MVVM_31a_CTValidation3.ViewModels.ValidationPageViewModel,UserName)=.
PropChg(MVVM_31a_CTValidation3.ViewModels.ValidationPageViewModel,HasErrors)=True
PropChg(MVVM_31a_CTValidation3.ViewModels.ValidationPageViewModel,UserName)=DS12
PropChg(MVVM_31a_CTValidation3.ViewModels.ValidationPageViewModel,TTUserName)=UserName must have min. 6 Chars
", DebugLog);
    }

    [TestMethod()]
    public void UserLoginCommandTest()
    {
        testModel.UserName = "Dev Dave";
        Assert.AreEqual(true, testModel.UserLoginCommand.CanExecute(null));
        testModel.UserLoginCommand.Execute(null);
    }
}
