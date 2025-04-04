using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using BaseLib.Helper;
using GenInterfaces.Data;
using GenInterfaces.Interfaces.Genealogic;
using BaseGenClasses.Helper;
using BaseGenClasses.Helper.Interfaces;

namespace BaseGenClasses.Model.Tests;
[TestClass]
public class GenFamilyTests
{
    private Guid _uid;
    private GenFamily _genFamily;
    private IGenPerson _husband;
    private IGenPerson _wife;
    private readonly string? _cFamilyJS= "{\"$id\":\"1\",\"eGenType\":3,\"UId\":\"95742eb9-44c2-4d3f-9200-8c61abdd9b65\",\"Facts\":{\"$id\":\"2\",\"$values\":[{\"$id\":\"3\",\"eGenType\":0,\"eFactType\":28,\"Date\":{\"$id\":\"4\",\"eGenType\":10,\"Date1\":\"1900-01-01T00:00:00\"},\"Data\":\"\",\"Entities\":{\"$id\":\"5\",\"$values\":[]}},{\"$id\":\"6\",\"eGenType\":0,\"eFactType\":29,\"Date\":{\"$id\":\"7\",\"eGenType\":10,\"Date1\":\"1930-01-01T00:00:00\"},\"Data\":\"\",\"Entities\":{\"$id\":\"8\",\"$values\":[]}},{\"$id\":\"9\",\"eGenType\":0,\"eFactType\":1,\"Data\":\"Mustermann\",\"Entities\":{\"$id\":\"10\",\"$values\":[]}},{\"$id\":\"11\",\"eGenType\":0,\"eFactType\":27,\"Data\":\"123456\",\"Entities\":{\"$id\":\"12\",\"$values\":[]}}]},\"Connects\":{\"$id\":\"13\",\"$values\":[{\"$id\":\"14\",\"eGenType\":4,\"Entity\":{\"$id\":\"15\",\"eGenType\":2,\"UId\":\"95742eb9-44c2-4d3f-9201-8c61abdd9b65\",\"Facts\":{\"$id\":\"16\",\"$values\":[{\"$id\":\"17\",\"eGenType\":0,\"eFactType\":6,\"Data\":\"M\",\"Entities\":{\"$id\":\"18\",\"$values\":[]}}]},\"Connects\":{\"$id\":\"19\",\"$values\":[]}},\"eGenConnectionType\":0},{\"$id\":\"20\",\"eGenType\":4,\"Entity\":{\"$id\":\"21\",\"eGenType\":2,\"UId\":\"95742eb9-44c2-4d3f-9202-8c61abdd9b65\",\"Facts\":{\"$id\":\"22\",\"$values\":[{\"$id\":\"23\",\"eGenType\":0,\"eFactType\":6,\"Data\":\"F\",\"Entities\":{\"$id\":\"24\",\"$values\":[]}}]},\"Connects\":{\"$id\":\"25\",\"$values\":[]}},\"eGenConnectionType\":0}]}}";

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
        _genFamily.AddEvent(EFactType.Mariage, new GenDate(new DateTime(1900, 1, 1)),"",new Guid());
        _genFamily.AddEvent(EFactType.Divorce, new GenDate(new DateTime(1930, 1, 1)), "", new Guid());
        _genFamily.AddFact(EFactType.Surname, "Mustermann", new Guid());
        _genFamily.AddFact(EFactType.Reference, "123456", new Guid());
        _husband = new GenPerson()
        {
            UId = new Guid("95742eb9-44c2-4d3f-9201-8c61abdd9b65")
        };
        _husband.AddFact(EFactType.Sex, "M");
        _genFamily.Husband = _husband;
        _wife = new GenPerson()
        {
            UId = new Guid("95742eb9-44c2-4d3f-9202-8c61abdd9b65")
        };
        _wife.AddFact(EFactType.Sex, "F");
        _genFamily.Wife = _wife;

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
        Assert.AreEqual(_husband, _genFamily.Husband);
    }
    [TestMethod]
    public void GenFamilyWifeTest()
    {
        Assert.AreEqual(_wife, _genFamily.Wife);
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
    [TestMethod]
    public void GenFamilyEndDateTest()
    {
        Assert.AreEqual(EFactType.Divorce, _genFamily.End.eFactType);
        Assert.AreEqual(new DateTime(1930, 1, 1), _genFamily.End.Date.Date1);
    }
    [TestMethod]
    public void GenFamilyStartDateTest()
    {
        Assert.AreEqual(EFactType.Mariage, _genFamily.Start.eFactType);
        Assert.AreEqual(new DateTime(1900, 1, 1), _genFamily.Start.Date.Date1);
    }
    [TestMethod]
    public void SerializationTest()
    {
        var options = new JsonSerializerOptions
        {
            ReferenceHandler = ReferenceHandler.Preserve,
        };
        var json = JsonSerializer.Serialize<IGenEntity>(_genFamily,options);
        Assert.AreEqual(_cFamilyJS, json);
    }   
    
    [TestMethod]
    public void DeserializationTest()
    {
        var options = new JsonSerializerOptions
        {
            ReferenceHandler = ReferenceHandler.Preserve,
        };
        var json = JsonSerializer.Serialize<IGenEntity>(_genFamily,options);
        Assert.AreEqual(_cFamilyJS, json);
        options = new JsonSerializerOptions(options);
        options.Converters.Add(new GenConverter<List<IGenFact>, IList<IGenFact>>());
        options.Converters.Add(new GenConverter<GenFact, IGenFact>());
        options.Converters.Add(new GenConverter<GenDate, IGenDate>());
        options.Converters.Add(new GenConverter<GenPlace, IGenPlace>());
        options.Converters.Add(new GenConverter<GenPerson, IGenEntity>());
        options.Converters.Add(new GenConverter<GenConnect, IGenConnects>());

        var genFamily = JsonSerializer.Deserialize<GenFamily>(json,options);
        Assert.AreEqual(_genFamily.UId, genFamily.UId);
        Assert.AreEqual(_genFamily.eGenType, genFamily.eGenType);
        Assert.AreEqual(_genFamily.Husband.UId, genFamily.Husband.UId);
        Assert.AreEqual(_genFamily.Wife.UId, genFamily.Wife.UId);
        Assert.AreEqual(_genFamily.ChildCount, genFamily.ChildCount);
        Assert.AreEqual(_genFamily.Children.Count, genFamily.Children.Count);
        Assert.AreEqual(_genFamily.MarriageDate.Date1, genFamily.MarriageDate.Date1);
        Assert.AreEqual(_genFamily.FamilyRefID, genFamily.FamilyRefID);
        Assert.AreEqual(_genFamily.FamilyName, genFamily.FamilyName);
        Assert.AreEqual(_genFamily.End.eFactType, genFamily.End.eFactType);
        Assert.AreEqual(_genFamily.End.Date.Date1, genFamily.End.Date.Date1);
        Assert.AreEqual(_genFamily.Start.eFactType, genFamily.Start.eFactType);
        Assert.AreEqual(_genFamily.Start.Date.Date1, genFamily.Start.Date.Date1);
    }
}
