using System;
using System.Text.Json;
using GenInterfaces.Interfaces.Genealogic;

namespace BaseGenClasses.Model.Tests;

[TestClass()]
public class GenPlaceTests
{
    private const string _cPlaceJS = "{\"eGenType\":6,\"UId\":\"164359f0-a3a4-4f9f-8824-af79ec666a45\",\"Name\":\"Musterstadt\",\"Type\":\"Deutschland\",\"GOV_ID\":\"GOV123\"}";
    private const string _cPlaceJ2 = "{\"eGenType\":6,\"UId\":\"164359f0-a3a4-4f9f-8824-af79ec666a45\",\"Name\":\"Musterstadt\",\"Type\":\"Deutschland\",\"GOV_ID\":\"GOV123\",\"ID\":0}";
    GenPlace _genPlace;

    [TestInitialize]
    public void Initialize()
    {
        _genPlace = new GenPlace("Musterstadt", "Deutschland","GOV123")
        {
            UId = Guid.Parse("164359f0-a3a4-4f9f-8824-af79ec666a45")
        };
    }

    [TestMethod()]
    public void GenPlaceTest()
    {
        var genPlace = new GenPlace("Mutterstadt", "Gemeinde", "MUTADTJN49EK", "-49.444", "-8.351", "No notes")
        {
            UId = Guid.Parse("164359f0-a3a4-4f9f-8824-000007338019")
        };
        Assert.AreEqual("Mutterstadt", genPlace.Name);
        Assert.AreEqual("Gemeinde", genPlace.Type);
    }
    [TestMethod()]
    public void GenPlaceNameTest()
    {
        Assert.AreEqual("Musterstadt", _genPlace.Name);
    }
    [TestMethod()]
    public void GenPlaceTypeTest()
    {
        Assert.AreEqual("Deutschland", _genPlace.Type);
    }
    [TestMethod()]
    public void GenPlaceGOVTest()
    {
        Assert.AreEqual("GOV123", _genPlace.GOV_ID);
    }
    [TestMethod()]
    public void GenPlaceLatitudeTest()
    {
        Assert.AreEqual(0, _genPlace.Latitude);
    }
    [TestMethod()]
    public void GenPlaceLongitudeTest()
    {
        Assert.AreEqual(0, _genPlace.Longitude);
    }
    [TestMethod()]
    public void GenPlaceNotesTest()
    {
        Assert.AreEqual(null, _genPlace.Notes);
    }
    [TestMethod()]
    public void GenPlaceParentTest()
    {
        Assert.IsNull(_genPlace.Parent);
    }
    [TestMethod()]
    public void GenPlaceUIdTest()
    {
        Assert.AreEqual(Guid.Parse("164359f0-a3a4-4f9f-8824-af79ec666a45"), _genPlace.UId);
    }
    [TestMethod()]
    public void SerializeGenPlaceTest()
    {
        var json = JsonSerializer.Serialize<IGenPlace>(_genPlace);
        Assert.AreEqual(_cPlaceJS, json);
    }  
    
    [TestMethod()]
    public void DeerializeGenPlaceTest()
    {
        var json = JsonSerializer.Serialize<IGenPlace>(_genPlace);
        Assert.AreEqual(_cPlaceJS, json);
        var genPlace = JsonSerializer.Deserialize<GenPlace>(json);
        Assert.AreEqual(_genPlace.Name, genPlace.Name);
        Assert.AreEqual(_genPlace.Type, genPlace.Type);
        Assert.AreEqual(_genPlace.GOV_ID, genPlace.GOV_ID);
    }
}