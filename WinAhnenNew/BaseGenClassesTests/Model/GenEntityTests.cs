using GenInterfaces.Data;
using GenInterfaces.Interfaces.Genealogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseGenClasses.Model.Tests;

internal class GenEntityTestClass : GenEntity
{
    public override EGenType eGenType => EGenType.GenPerson;

    protected override IGenFact? GetEndFactOfEntity()
    {
        throw new NotImplementedException();
    }

    protected override IGenFact? GetStartFactOfEntity()
    {
        throw new NotImplementedException();
    }
    public void SetLC(DateTime dateTime) => LastChange = dateTime;

}

[TestClass]
public class GenEntityTests
{
    private Guid _uid;
    private GenEntityTestClass _testClass;

    [TestInitialize]
    public void Initialize()
    {
        _uid = Guid.NewGuid();
        _testClass = new() { 
            UId = _uid            
        };
    }

    [TestMethod]
    public void UIdTest()
    {
        Assert.AreEqual(_uid, _testClass.UId);
    }
    [TestMethod]
    public void TypeTest()
    {
        Assert.AreEqual(EGenType.GenPerson, _testClass.eGenType);
    }
    [TestMethod]
    public void LastChangeTest()
    {
        Assert.AreEqual(null, _testClass.LastChange);
        _testClass.SetLC(new DateTime(2025, 01, 01));
        Assert.AreEqual(new DateTime(2025, 01, 01), _testClass.LastChange);
    }
    [TestMethod]
    public void StartTest()
    {
        Assert.ThrowsExactly<NotImplementedException>(() => _testClass.Start);
    }
    [TestMethod]
    public void EndTest()
    {
        Assert.ThrowsExactly<NotImplementedException>(() => _testClass.End);
    }
    [TestMethod]
    public void SourcesTest()
    {
        Assert.IsNotNull(_testClass.Sources);
        Assert.AreEqual(0, _testClass.Sources.Count);
    }
    [TestMethod]
    public void FactsTest()
    {
        Assert.IsNotNull(_testClass.Facts);
        Assert.AreEqual(0, _testClass.Facts.Count);
    }
    [TestMethod]
    public void ConnectsTest()
    {
        Assert.IsNotNull(_testClass.Connects);
        Assert.AreEqual(0, _testClass.Connects.Count);
    }
    [TestMethod]
    public void MediaTest()
    {
        Assert.IsNotNull(_testClass.Media);
        Assert.AreEqual(0, _testClass.Media.Count);
    }
}
