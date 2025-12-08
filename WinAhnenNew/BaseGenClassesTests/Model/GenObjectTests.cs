using GenInterfaces.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace BaseGenClasses.Model.Tests;


internal class GenObjectTestClass : GenObject
{
    public override EGenType eGenType => EGenType.GenNote;

    public void SetLC(DateTime dateTime) => LastChange = dateTime;
}

[TestClass]
public class GenObjectTests 
{
    private Guid _guid;
    private GenObjectTestClass _genObject;
    [TestInitialize()]
    public void Initialize()
    {
        _guid = Guid.NewGuid();
        _genObject = new() {
            UId = _guid,
        };

    }

    [TestMethod()]
    public void UIdTest()
    {
        Assert.AreEqual(_guid, _genObject.UId);
    }

    [TestMethod()]
    public void TypeTest()
    {
        Assert.AreEqual(EGenType.GenNote, _genObject.eGenType);
    }


    [TestMethod()]
    public void LastChangeTest()
    {
        Assert.AreEqual(null, _genObject.LastChange);
        _genObject.SetLC(new DateTime(2025,01,01));
        Assert.AreEqual(new DateTime(2025, 01, 01), _genObject.LastChange);
    }
}
