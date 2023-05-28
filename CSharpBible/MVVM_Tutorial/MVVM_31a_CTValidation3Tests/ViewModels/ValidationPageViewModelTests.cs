using Microsoft.VisualStudio.TestTools.UnitTesting;
using MVVM_31a_CTValidation3.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using MVVM.ViewModel;
using System.Threading.Tasks;

namespace MVVM_31a_CTValidation3.ViewModels.Tests
{
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
        [DataRow("", 1, false, new[] { @"PropChg(MVVM_31a_CTValidation3.ViewModels.ValidationPageViewModel,VHelper)=MVVM_BaseLib.Helper.MVVM.ValidationHelper
ErrorsChanged(MVVM_31a_CTValidation3.ViewModels.ValidationPageViewModel,UserName)=
PropChg(MVVM_31a_CTValidation3.ViewModels.ValidationPageViewModel,VHelper)=MVVM_BaseLib.Helper.MVVM.ValidationHelper
ErrorsChanged(MVVM_31a_CTValidation3.ViewModels.ValidationPageViewModel,UserName)=Username may not be empty
PropChgn(MVVM_31a_CTValidation3.ViewModels.ValidationPageViewModel,UserName)=.
PropChg(MVVM_31a_CTValidation3.ViewModels.ValidationPageViewModel,UserName)=
" })]
        [DataRow("DS", 2, false, new[] { @"PropChg(MVVM_31a_CTValidation3.ViewModels.ValidationPageViewModel,VHelper)=MVVM_BaseLib.Helper.MVVM.ValidationHelper
ErrorsChanged(MVVM_31a_CTValidation3.ViewModels.ValidationPageViewModel,UserName)=
PropChg(MVVM_31a_CTValidation3.ViewModels.ValidationPageViewModel,VHelper)=MVVM_BaseLib.Helper.MVVM.ValidationHelper
ErrorsChanged(MVVM_31a_CTValidation3.ViewModels.ValidationPageViewModel,UserName)=Username must have min. 6 Chars
PropChgn(MVVM_31a_CTValidation3.ViewModels.ValidationPageViewModel,UserName)=.
PropChg(MVVM_31a_CTValidation3.ViewModels.ValidationPageViewModel,UserName)=DS
" })]
        [DataRow("DS1234", 0, true, new[] { @"PropChg(MVVM_31a_CTValidation3.ViewModels.ValidationPageViewModel,VHelper)=MVVM_BaseLib.Helper.MVVM.ValidationHelper
ErrorsChanged(MVVM_31a_CTValidation3.ViewModels.ValidationPageViewModel,UserName)=
PropChgn(MVVM_31a_CTValidation3.ViewModels.ValidationPageViewModel,UserName)=.
PropChg(MVVM_31a_CTValidation3.ViewModels.ValidationPageViewModel,UserName)=DS1234
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
            Assert.AreEqual("Username must have min. 6 Chars", string.Join(",", (List<string>)testModel.GetErrors(nameof(testModel.UserName))));
            Assert.AreEqual(@"PropChg(MVVM_31a_CTValidation3.ViewModels.ValidationPageViewModel,VHelper)=MVVM_BaseLib.Helper.MVVM.ValidationHelper
PropChg(MVVM_31a_CTValidation3.ViewModels.ValidationPageViewModel,VHelper)=MVVM_BaseLib.Helper.MVVM.ValidationHelper
PropChgn(MVVM_31a_CTValidation3.ViewModels.ValidationPageViewModel,UserName)=.
PropChg(MVVM_31a_CTValidation3.ViewModels.ValidationPageViewModel,UserName)=DS12
", DebugLog);
        }
    }
}
