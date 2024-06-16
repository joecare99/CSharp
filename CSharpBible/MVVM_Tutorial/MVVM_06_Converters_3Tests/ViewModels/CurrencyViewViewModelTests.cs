using Microsoft.VisualStudio.TestTools.UnitTesting;
using MVVM.ViewModel;
using System;
using System.Collections.Generic;

namespace MVVM_06_Converters_3.ViewModels.Tests
{
    [TestClass()]
    public class CurrencyViewViewModelTests:BaseTestViewModel<CurrencyViewViewModel>
    {
        [TestMethod()]
        public void SetupTest()
        {
            Assert.IsNotNull(testModel);
            Assert.IsNotNull(testModel2);
            Assert.IsInstanceOfType(testModel, typeof(CurrencyViewViewModel));
            Assert.IsInstanceOfType(testModel2, typeof(CurrencyViewViewModel));
            Assert.IsInstanceOfType(testModel, typeof(BaseViewModel));
        }

        [DataTestMethod]
        [DataRow(0d, new[] { "" })]
        [DataRow(1d, new[] { @"PropChg(MVVM_06_Converters_3.ViewModels.CurrencyViewViewModel,Value)=1
PropChg(MVVM_06_Converters_3.ViewModels.CurrencyViewViewModel,ValueIsNotZero)=True
" })]
        public void ValueTest(double fAct, string[] asExp)
        {
            Decimal dAct = (decimal)fAct;
            Assert.AreEqual(0m, testModel.Value);
            testModel.Value = dAct;
            Assert.AreEqual(dAct, testModel.Value);
            Assert.AreEqual(asExp[0], DebugLog);
        }

        protected override Dictionary<string, object?> GetDefaultData() 
            => new() { 
                { nameof(CurrencyViewViewModel.Value), 0m },
                { nameof(CurrencyViewViewModel.ValueIsNotZero), false },
            };
    }
}