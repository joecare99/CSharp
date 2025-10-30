using Microsoft.VisualStudio.TestTools.UnitTesting;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AA06_Converters_4.Model.Tests;

[TestClass()]
public class AGV_ModelTests
{
    private AGV_Model? testModel;
    private AGV_Model? testModel2;

    [TestInitialize]
    public void Init()
    {
        testModel = new AGV_Model();
        testModel2 = new AGV_Model();
    }

    [TestMethod()]
    public void AGV_ModelTest()
    {
        Assert.IsNotNull(testModel);
        Assert.IsNotNull(testModel2);
        Assert.IsInstanceOfType(testModel, typeof(AGV_Model));
        Assert.IsInstanceOfType(testModel, typeof(ObservableObject));
    }

    [TestMethod()]
    public void SaveTest()
    {
        testModel!.Save();
        Assert.IsFalse(testModel.IsDirty);
    }
}