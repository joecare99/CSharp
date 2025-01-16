using Microsoft.VisualStudio.TestTools.UnitTesting;
using MVVM.ViewModel;
using System.Collections.Generic;

namespace MVVM_06_Converters.ViewModels.Tests;

[TestClass]
public class CurrencyViewViewModelTests : BaseTestViewModel<CurrencyViewModel>
{
    protected override Dictionary<string, object?> GetDefaultData() => new()
    {
        { nameof(CurrencyViewModel.Value), 10m }
    };

    [TestMethod()]
    public void SetupTest()
    {
        Assert.IsNotNull(testModel);
        Assert.IsInstanceOfType(testModel, typeof(CurrencyViewModel));
        Assert.IsInstanceOfType(testModel, typeof(BaseViewModel));
    }

    [TestMethod()]
    public void ValueTest()
    {
        Assert.AreEqual(10m, testModel.Value);
        testModel.Value = 1m;
        Assert.AreEqual(1m, testModel.Value);
        Assert.AreEqual(@"PropChg(MVVM_06_Converters.ViewModels.CurrencyViewModel,Value)=1
", DebugLog);
    }

    [TestMethod()]
    public void EnterKeyTest()
    {
        testModel.EnterKey.Execute(null);
        Assert.AreEqual("", DebugLog);
    }

}
