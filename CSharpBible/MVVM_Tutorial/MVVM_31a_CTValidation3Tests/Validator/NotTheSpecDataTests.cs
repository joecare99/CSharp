using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MVVM_31a_CTValidation3.Validator.Tests;

[TestClass]
public class NotTheSpecDataTests
{

    [DataTestMethod()]
    [DataRow("BlaBla", "BlaBla", false)]
    [DataRow("BlaBla", "BlaBla1", true)]
    [DataRow("BlaBla", null, true)]
    [DataRow("BlaBla", 5, true)]
    public void IsValidTest(string sAct1,object? sAct2,bool xExp)
    {
        var test = new NotTheSpecData(sAct1);
        Assert.AreEqual(xExp,test.IsValid(sAct2));
    }
}
