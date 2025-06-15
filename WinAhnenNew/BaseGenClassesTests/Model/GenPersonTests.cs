using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using BaseLib.Helper;
using GenInterfaces.Data;
using BaseGenClasses.Helper;
using BaseGenClasses.Helper.Interfaces;

namespace BaseGenClasses.Model.Tests;

[TestClass]
public class GenPersonTests
{
    private Guid _uid;
    GenPerson _genPerson;
    private readonly string? _cJsonGP = "{\"$id\":\"1\",\"$type\":\"GenPerson\",\"eGenType\":2,\"Facts\":{\"$id\":\"2\",\"$values\":[{\"$id\":\"3\",\"eGenType\":0,\"UId\":\"54883a8e-c59f-45ce-946e-5d24b152b546\",\"eFactType\":0,\"Data\":\"Peter\",\"Entities\":{\"$id\":\"4\",\"$values\":[]}},{\"$id\":\"5\",\"eGenType\":0,\"UId\":\"598acdd9-013d-44c6-aced-29ae8739fff8\",\"eFactType\":1,\"Data\":\"Mustermann\",\"Entities\":{\"$id\":\"6\",\"$values\":[]}},{\"$id\":\"7\",\"eGenType\":0,\"UId\":\"c9b104e8-ef27-40c3-8312-1ebc7009b697\",\"eFactType\":2,\"Data\":\"Dr.\",\"Entities\":{\"$id\":\"8\",\"$values\":[]}},{\"$id\":\"9\",\"eGenType\":0,\"UId\":\"ec56238f-d595-4ef1-8960-501ee9622a80\",\"eFactType\":4,\"Data\":\"Pemu\",\"Entities\":{\"$id\":\"10\",\"$values\":[]}},{\"$id\":\"11\",\"eGenType\":0,\"UId\":\"11dcf8d7-0f84-4962-9feb-2a7c88b1d203\",\"eFactType\":6,\"Data\":\"M\",\"Entities\":{\"$id\":\"12\",\"$values\":[]}},{\"$id\":\"13\",\"eGenType\":0,\"UId\":\"b1e257d5-114c-4c59-b775-8d621788428c\",\"eFactType\":7,\"Date\":{\"$id\":\"14\",\"eGenType\":10,\"Date1\":\"1960-01-01T00:00:00\"},\"Place\":{\"$id\":\"15\",\"eGenType\":6,\"UId\":\"0f9718ce-e40a-45a0-a33b-76a34b99838d\",\"Name\":\"Bad Nauheim\"},\"Entities\":{\"$id\":\"16\",\"$values\":[]}},{\"$id\":\"17\",\"eGenType\":0,\"UId\":\"54883a8e-c59f-45ce-946e-5d24b152b546\",\"eFactType\":8,\"Date\":{\"$id\":\"18\",\"eGenType\":10,\"Date1\":\"1960-03-01T00:00:00\"},\"Place\":{\"$id\":\"19\",\"eGenType\":6,\"UId\":\"3e9319ee-3970-4db6-8836-280cb605bc1c\",\"Name\":\"Dortmund\"},\"Entities\":{\"$id\":\"20\",\"$values\":[]}},{\"$id\":\"21\",\"eGenType\":0,\"UId\":\"54883a8e-c59f-45ce-946e-5d24b152b546\",\"eFactType\":9,\"Date\":{\"$id\":\"22\",\"eGenType\":10,\"Date1\":\"2025-01-01T00:00:00\"},\"Place\":{\"$id\":\"23\",\"eGenType\":6,\"UId\":\"f67220c2-9c73-4251-a47a-d98ab0db6726\",\"Name\":\"Hamburg\"},\"Entities\":{\"$id\":\"24\",\"$values\":[]}},{\"$id\":\"25\",\"eGenType\":0,\"UId\":\"54883a8e-c59f-45ce-946e-5d24b152b546\",\"eFactType\":10,\"Date\":{\"$id\":\"26\",\"eGenType\":10,\"Date1\":\"2025-02-01T00:00:00\"},\"Place\":{\"$id\":\"27\",\"eGenType\":6,\"UId\":\"4c83ba6b-20af-4d45-b216-c11c9c9a78ec\",\"Name\":\"Kiel\"},\"Entities\":{\"$id\":\"28\",\"$values\":[]}},{\"$id\":\"29\",\"eGenType\":0,\"UId\":\"54883a8e-c59f-45ce-946e-5d24b152b546\",\"eFactType\":27,\"Data\":\"123456\",\"Entities\":{\"$id\":\"30\",\"$values\":[]}},{\"$id\":\"31\",\"eGenType\":0,\"UId\":\"11dcf8d7-0f84-4962-9feb-2a7c88b1d204\",\"eFactType\":12,\"Place\":{\"$id\":\"32\",\"eGenType\":6,\"UId\":\"f67220c2-9c73-4251-a47a-d98ab0db6800\",\"Name\":\"Hannover-East\"},\"Data\":\"Salesman\",\"Entities\":{\"$id\":\"33\",\"$values\":[]}},{\"$id\":\"34\",\"eGenType\":0,\"UId\":\"11dcf8d7-0f84-4962-9feb-2a7c88b1d205\",\"eFactType\":13,\"Place\":{\"$id\":\"35\",\"eGenType\":6,\"UId\":\"f67220c2-9c73-4251-a47a-d98ab0db6799\",\"Name\":\"Hannover\"},\"Entities\":{\"$id\":\"36\",\"$values\":[]}},{\"$id\":\"37\",\"eGenType\":0,\"UId\":\"11dcf8d7-0f84-4962-9feb-2a7c88b1d206\",\"eFactType\":11,\"Data\":\"ev.\",\"Entities\":{\"$id\":\"38\",\"$values\":[]}}]},\"Connects\":{\"$id\":\"39\",\"$values\":[]},\"UId\":\"95742eb9-44c2-4d3f-9242-8c61abdd9b65\",\"LastChange\":null,\"ID\":0}";
 // private readonly string? _cJsonG2 = "{\"$id\":\"1\",\"$type\":\"GenPerson\",\"eGenType\":2,\"Facts\":{\"$id\":\"2\",\"$values\":[{\"$id\":\"3\",\"eGenType\":0,\"UId\":\"54883a8e-c59f-45ce-946e-5d24b152b546\",\"eFactType\":0,\"Data\":\"Peter\",\"Entities\":{\"$id\":\"4\",\"$values\":[]},\"ID\":0},{\"$id\":\"5\",\"eGenType\":0,\"UId\":\"598acdd9-013d-44c6-aced-29ae8739fff8\",\"eFactType\":1,\"Data\":\"Mustermann\",\"Entities\":{\"$id\":\"6\",\"$values\":[]},\"ID\":0},{\"$id\":\"7\",\"eGenType\":0,\"UId\":\"c9b104e8-ef27-40c3-8312-1ebc7009b697\",\"eFactType\":2,\"Data\":\"Dr.\",\"Entities\":{\"$id\":\"8\",\"$values\":[]},\"ID\":0},{\"$id\":\"9\",\"eGenType\":0,\"UId\":\"ec56238f-d595-4ef1-8960-501ee9622a80\",\"eFactType\":4,\"Data\":\"Pemu\",\"Entities\":{\"$id\":\"10\",\"$values\":[]},\"ID\":0},{\"$id\":\"11\",\"eGenType\":0,\"UId\":\"11dcf8d7-0f84-4962-9feb-2a7c88b1d203\",\"eFactType\":6,\"Data\":\"M\",\"Entities\":{\"$id\":\"12\",\"$values\":[]},\"ID\":0},{\"$id\":\"13\",\"eGenType\":0,\"UId\":\"b1e257d5-114c-4c59-b775-8d621788428c\",\"eFactType\":7,\"Date\":{\"$id\":\"14\",\"eGenType\":10,\"Date1\":\"1960-01-01T00:00:00\",\"ID\":0},\"Place\":{\"$id\":\"15\",\"eGenType\":6,\"UId\":\"0f9718ce-e40a-45a0-a33b-76a34b99838d\",\"Name\":\"Bad Nauheim\",\"ID\":0},\"Entities\":{\"$id\":\"16\",\"$values\":[]},\"ID\":0},{\"$id\":\"17\",\"eGenType\":0,\"UId\":\"54883a8e-c59f-45ce-946e-5d24b152b546\",\"eFactType\":8,\"Date\":{\"$id\":\"18\",\"eGenType\":10,\"Date1\":\"1960-03-01T00:00:00\",\"ID\":0},\"Place\":{\"$id\":\"19\",\"eGenType\":6,\"UId\":\"3e9319ee-3970-4db6-8836-280cb605bc1c\",\"Name\":\"Dortmund\",\"ID\":0},\"Entities\":{\"$id\":\"20\",\"$values\":[]},\"ID\":0},{\"$id\":\"21\",\"eGenType\":0,\"UId\":\"54883a8e-c59f-45ce-946e-5d24b152b546\",\"eFactType\":9,\"Date\":{\"$id\":\"22\",\"eGenType\":10,\"Date1\":\"2025-01-01T00:00:00\",\"ID\":0},\"Place\":{\"$id\":\"23\",\"eGenType\":6,\"UId\":\"f67220c2-9c73-4251-a47a-d98ab0db6726\",\"Name\":\"Hamburg\",\"ID\":0},\"Entities\":{\"$id\":\"24\",\"$values\":[]},\"ID\":0},{\"$id\":\"25\",\"eGenType\":0,\"UId\":\"54883a8e-c59f-45ce-946e-5d24b152b546\",\"eFactType\":10,\"Date\":{\"$id\":\"26\",\"eGenType\":10,\"Date1\":\"2025-02-01T00:00:00\",\"ID\":0},\"Place\":{\"$id\":\"27\",\"eGenType\":6,\"UId\":\"4c83ba6b-20af-4d45-b216-c11c9c9a78ec\",\"Name\":\"Kiel\",\"ID\":0},\"Entities\":{\"$id\":\"28\",\"$values\":[]},\"ID\":0},{\"$id\":\"29\",\"eGenType\":0,\"UId\":\"54883a8e-c59f-45ce-946e-5d24b152b546\",\"eFactType\":27,\"Data\":\"123456\",\"Entities\":{\"$id\":\"30\",\"$values\":[]},\"ID\":0},{\"$id\":\"31\",\"eGenType\":0,\"UId\":\"11dcf8d7-0f84-4962-9feb-2a7c88b1d204\",\"eFactType\":12,\"Place\":{\"$id\":\"32\",\"eGenType\":6,\"UId\":\"f67220c2-9c73-4251-a47a-d98ab0db6800\",\"Name\":\"Hannover-East\",\"ID\":0},\"Data\":\"Salesman\",\"Entities\":{\"$id\":\"33\",\"$values\":[]},\"ID\":0},{\"$id\":\"34\",\"eGenType\":0,\"UId\":\"11dcf8d7-0f84-4962-9feb-2a7c88b1d205\",\"eFactType\":13,\"Place\":{\"$id\":\"35\",\"eGenType\":6,\"UId\":\"f67220c2-9c73-4251-a47a-d98ab0db6799\",\"Name\":\"Hannover\",\"ID\":0},\"Entities\":{\"$id\":\"36\",\"$values\":[]},\"ID\":0},{\"$id\":\"37\",\"eGenType\":0,\"UId\":\"11dcf8d7-0f84-4962-9feb-2a7c88b1d206\",\"eFactType\":11,\"Data\":\"ev.\",\"Entities\":{\"$id\":\"38\",\"$values\":[]},\"ID\":0}]},\"Connects\":{\"$id\":\"39\",\"$values\":[]},\"UId\":\"95742eb9-44c2-4d3f-9242-8c61abdd9b65\",\"LastChange\":null,\"ID\":0}";

    [TestInitialize]
    public void Initialize()
    {
        _uid = new Guid("95742eb9-44c2-4d3f-9242-8c61abdd9b65");
        _genPerson = new()
        {
            UId = _uid
        };
        _genPerson.Facts.Add(new GenFact(_genPerson) { eFactType = EFactType.Givenname,UId=new Guid("54883a8e-c59f-45ce-946e-5d24b152b546"),Data="Peter" }) ;
        _genPerson.Facts.Add(new GenFact(_genPerson) { eFactType = EFactType.Surname, UId = new Guid("598acdd9-013d-44c6-aced-29ae8739fff8"), Data = "Mustermann" });
        _genPerson.Facts.Add(new GenFact(_genPerson) { eFactType = EFactType.Title, UId = new Guid("c9b104e8-ef27-40c3-8312-1ebc7009b697"), Data = "Dr." });
        _genPerson.Facts.Add(new GenFact(_genPerson) { eFactType = EFactType.Nickname, UId = new Guid("ec56238f-d595-4ef1-8960-501ee9622a80"), Data = "Pemu" });
        _genPerson.Facts.Add(new GenFact(_genPerson) { eFactType = EFactType.Sex, UId = new Guid("11dcf8d7-0f84-4962-9feb-2a7c88b1d203"), Data = "M" });
        _genPerson.Facts.Add(new GenFact(_genPerson) { eFactType = EFactType.Birth, UId = new Guid("b1e257d5-114c-4c59-b775-8d621788428c"), Date = new GenDate(new DateTime(1960, 01, 01)), Place = new GenPlace("Bad Nauheim") {UId=new Guid("0f9718ce-e40a-45a0-a33b-76a34b99838d") } });
        _genPerson.Facts.Add(new GenFact(_genPerson) { eFactType = EFactType.Baptism, UId = new Guid("54883a8e-c59f-45ce-946e-5d24b152b546"), Date = new GenDate(new DateTime(1960, 03, 01)), Place = new GenPlace("Dortmund") { UId = new Guid("3e9319ee-3970-4db6-8836-280cb605bc1c") } });
        _genPerson.Facts.Add(new GenFact(_genPerson) { eFactType = EFactType.Death, UId = new Guid("54883a8e-c59f-45ce-946e-5d24b152b546"), Date = new GenDate(new DateTime(2025, 01, 01)), Place = new GenPlace("Hamburg") { UId = new Guid("f67220c2-9c73-4251-a47a-d98ab0db6726") } });
        _genPerson.Facts.Add(new GenFact(_genPerson) { eFactType = EFactType.Burial, UId = new Guid("54883a8e-c59f-45ce-946e-5d24b152b546"), Date = new GenDate(new DateTime(2025, 02, 01)), Place = new GenPlace("Kiel") { UId = new Guid("4c83ba6b-20af-4d45-b216-c11c9c9a78ec") } });
        _genPerson.Facts.Add(new GenFact(_genPerson) { eFactType = EFactType.Reference, UId = new Guid("54883a8e-c59f-45ce-946e-5d24b152b546"), Data = "123456" });
        _genPerson.Facts.Add(new GenFact(_genPerson) { eFactType = EFactType.Occupation, UId = new Guid("11dcf8d7-0f84-4962-9feb-2a7c88b1d204"), Data = "Salesman", Place = new GenPlace("Hannover-East") { UId = new Guid("f67220c2-9c73-4251-a47a-d98ab0db6800") } });
        _genPerson.Facts.Add(new GenFact(_genPerson) { eFactType = EFactType.Residence, UId = new Guid("11dcf8d7-0f84-4962-9feb-2a7c88b1d205"), Place = new GenPlace("Hannover") { UId = new Guid("f67220c2-9c73-4251-a47a-d98ab0db6799") } });
        _genPerson.Facts.Add(new GenFact(_genPerson) { eFactType = EFactType.Religion, UId = new Guid("11dcf8d7-0f84-4962-9feb-2a7c88b1d206"), Data = "ev." });

        //  _genPerson.Connects.Add(new GenConnect(_genPerson) { eGenConnectionType = EGenConnectionType.Parent, Entity = new GenPerson() { UId = Guid.NewGuid(), Facts = new List<IGenFact> { new GenFact(_genPerson) { eFactType = EFactType.Givenname, Data = "Hans" }, new GenFact(_genPerson) { eFactType = EFactType.Surname, Data = "Mustermann" } } } });
        //  _genPerson.Connects.Add(new GenConnect(_genPerson) { eGenConnectionType = EGenConnectionType.Parent, Entity = new GenPerson() { UId = Guid.NewGuid(), Facts = new List<IGenFact> { new GenFact(_genPerson) { eFactType = EFactType.Givenname, Data = "Maria" }, new GenFact(_genPerson) { eFactType = EFactType.Surname, Data = "Mustermann" } } } });
        //  _genPerson.Connects.Add(new GenConnect(_genPerson) { eGenConnectionType = EGenConnectionType.Spouse, Entity = new GenPerson() { UId = Guid.NewGuid(), Facts = new List<IGenFact> { new GenFact(_genPerson) { eFactType = EFactType.Givenname, Data = "Eva" }, new GenFact(_genPerson) { eFactType = EFactType.Surname, Data = "Mustermann" } } } });

        IoC.GetReqSrv = (t) => t switch
        {
            _ when t == typeof(IGenILBuilder) => new GenILBuilder(),
            _ => throw new NotImplementedException()
        };
    }

    [TestMethod]
    public void UIdTest()
    {
        Assert.AreEqual(_uid, _genPerson.UId);
    }

    [TestMethod]
    public void NameTest()
    {
        Assert.AreEqual("Dr. Peter Mustermann", _genPerson.Name);
    }

    [TestMethod]
    public void GivenNameTest()
    {
        Assert.AreEqual("Peter", _genPerson.GivenName);
    }
    [TestMethod]
    public void SurnameTest()
    {
        Assert.AreEqual("Mustermann", _genPerson.Surname);
    }
    [TestMethod]
    public void TitleTest()
    {
        Assert.AreEqual("Dr.", _genPerson.Title);
    }
    [TestMethod]
    public void SexTest()
    {
        Assert.AreEqual("M", _genPerson.Sex);
    }
    [TestMethod]
    public void IndRefIDTest()
    {
        Assert.AreEqual("123456", _genPerson.IndRefID);
    }
    [TestMethod]
    public void BirthTest()
    {
        Assert.AreEqual(new DateTime(1960, 01, 01), _genPerson.Birth?.Date.Date1);
    }
    [TestMethod]
    public void BirthDateTest()
    {
        Assert.AreEqual(new DateTime(1960, 01, 01), _genPerson.BirthDate?.Date1);
    }
    [TestMethod]
    public void BirthPlaceTst()
    {
        Assert.AreEqual("Bad Nauheim", _genPerson.BirthPlace?.Name);
    }

    [TestMethod]
    public void BaptismTest()
    {
        Assert.AreEqual(new DateTime(1960, 03, 01), _genPerson.Baptism?.Date.Date1);
    }
    [TestMethod]
    public void BaptismDateTest()
    {
        Assert.AreEqual(new DateTime(1960, 03, 01), _genPerson.BaptDate?.Date1);
    }
    [TestMethod]
    public void BaptismPlaceTest()
    {
        Assert.AreEqual("Dortmund", _genPerson.BaptPlace?.Name);
    }

    [TestMethod]
    public void DeathTest()
    {
        Assert.AreEqual(new DateTime(2025, 01, 01), _genPerson.Death?.Date.Date1);
    }
    [TestMethod]
    public void DeathDateTest()
    {
        Assert.AreEqual(new DateTime(2025, 01, 01), _genPerson.DeathDate?.Date1);
    }
    [TestMethod]
    public void DeathPlaceTest()
    {
        Assert.AreEqual("Hamburg", _genPerson.DeathPlace?.Name);
    }
    [TestMethod]
    public void BurialTest()
    {
        Assert.AreEqual(new DateTime(2025, 02, 01), _genPerson.Burial?.Date.Date1);
    }
    [TestMethod]
    public void BurialDateTest()
    {
        Assert.AreEqual(new DateTime(2025, 02, 01), _genPerson.BurialDate?.Date1);
    }
    [TestMethod]
    public void BurialPlaceTest()
    {
        Assert.AreEqual("Kiel", _genPerson.BurialPlace?.Name);
    }
    [TestMethod]
    public void ResisdenceTest()
    {
        Assert.AreEqual("Hannover", _genPerson.Residence?.Name);
    }
    [TestMethod]
    public void ReligionTest()
    {
        Assert.AreEqual("ev.", _genPerson.Religion);
    }
    [TestMethod]
    public void OccupationTest()
    {
        Assert.AreEqual("Salesman", _genPerson.Occupation);
    }
    [TestMethod]
    public void OccupationPlaceTest()
    {
        Assert.AreEqual("Hannover-East", _genPerson.OccuPlace.Name);
    }

    [TestMethod]
    public void SerializeTest()
    {
        JsonSerializerOptions _options = new() { 
            WriteIndented = false,
            ReferenceHandler = ReferenceHandler.Preserve };
        var result = JsonSerializer.Serialize(_genPerson,options: _options);
        Assert.AreEqual(_cJsonGP, result);
    }
}
