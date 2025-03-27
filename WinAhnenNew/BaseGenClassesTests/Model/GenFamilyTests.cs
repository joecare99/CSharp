using BaseGenClasses.Helper;
using BaseGenClasses.Helper.Interfaces;
using BaseLib.Helper;
using GenInterfaces.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseGenClasses.Model.Tests;
[TestClass]
public class GenFamilyTests
{
    private Guid _uid;
    private GenFamily _genFamily;

    [TestInitialize]
    public void Initialize()
    {
        IoC.GetReqSrv = (t) => (t) switch {
            _ when t == typeof(IGenFactBuilder) => new GenFactBuilder(),
            _ when t == typeof(IGenConnectBuilder) => new GenConnectBuilder(),
            _ when t == typeof(IGenILBuilder) => new GenILBuilder(),
            _ => throw new NotImplementedException("Please initialize the service first.")
        };

        _uid = new Guid("95742eb9-44c2-4d3f-9200-8c61abdd9b65");
        _genFamily = new()
        {
            UId = _uid
        };
        _genFamily.Facts.AddEvent(_genFamily, EFactType.Mariage, new GenDate(new DateTime(1900, 1, 1)),"",new Guid());
        _genFamily.Facts.AddFact(_genFamily, EFactType.Surname, "Mustermann", new Guid());
        _genFamily.Facts.AddFact(_genFamily, EFactType.Reference, "123456", new Guid());

    }

    [TestMethod]
    public void GenFamilyTest()
    {
        Assert.AreEqual(_uid, _genFamily.UId);
    }
    [TestMethod]
    public void GenFamilyTypeTest()
    {
        Assert.AreEqual(EGenType.GenFamily, _genFamily.eGenType);
    }

    [TestMethod]
    public void GenFamilyHusbandTest()
    {
        var husband = new GenPerson();
        husband.Facts.AddFact(husband, EFactType.Sex, "M");
        _genFamily.Husband = husband;
        Assert.AreEqual(husband, _genFamily.Husband);
    }
    [TestMethod]
    public void GenFamilyWifeTest()
    {
        var wife = new GenPerson();
        wife.Facts.AddFact(wife, EFactType.Sex, "F");
        _genFamily.Wife = wife;
        Assert.AreEqual(wife, _genFamily.Wife);
    }
    [TestMethod]
    public void GenFamilyChildrenCountTest()
    {
        Assert.AreEqual(0, _genFamily.ChildCount);
    }
    [TestMethod]
    public void GenFamilyChildrenTest()
    {
        Assert.AreEqual(0, _genFamily.Children.Count);
    }
    [TestMethod]
    public void GenFamilyMarriageDateTest()
    {
        Assert.AreEqual(new DateTime(1900, 1, 1), _genFamily.MarriageDate.Date1);
    }
    [TestMethod]
    public void GenFamilyFamilyRefIDTest()
    {
        Assert.AreEqual("123456", _genFamily.FamilyRefID);
    }
    [TestMethod]
    public void GenFamilyFamilyNameTest()
    {
        Assert.AreEqual("Mustermann", _genFamily.FamilyName);
    }

}
