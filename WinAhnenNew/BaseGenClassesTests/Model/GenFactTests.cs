using Microsoft.VisualStudio.TestTools.UnitTesting;
using BaseGenClasses.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GenInterfaces.Interfaces.Genealogic;
using GenInterfaces.Data;
using GenInterfaces.Interfaces;
using System.Text.Json.Serialization.Metadata;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BaseGenClasses.Model.Tests;

[TestClass()]
public class GenFactTests : IGenEntity
{
    GenFact _genFact;
    private readonly string _cGenFJS = "{\"$id\":\"1\",\"$type\":\"GenFact\",\"eFactType\":7,\"Date\":{\"$id\":\"2\",\"Date1\":\"1960-01-01T00:00:00\",\"eGenType\":10},\"Place\":{\"$id\":\"3\",\"Name\":\"Musterstadt\",\"Type\":\"Deutschland\",\"UId\":\"164359f0-a3a4-4f9f-8824-af79ec666a45\",\"eGenType\":6},\"Entities\":{\"$id\":\"4\",\"$values\":[]},\"eGenType\":0,\"UId\":\"ee43c2a5-2259-4bc5-9913-ce08b984eeac\",\"LastChange\":null}";

    public IList<IGenFact> Facts { get => throw new NotImplementedException(); init => throw new NotImplementedException(); }
    public IList<IGenConnects> Connects { get => throw new NotImplementedException(); init => throw new NotImplementedException(); }

    public IGenFact? Start => throw new NotImplementedException();

    public IGenFact? End => throw new NotImplementedException();

    public IList<IGenSources> Sources { get => throw new NotImplementedException(); init => throw new NotImplementedException(); }
    public IList<IGenMedia> Media { get => throw new NotImplementedException(); init => throw new NotImplementedException(); }

    public DateTime? LastChange => throw new NotImplementedException();

    public Guid UId { get => throw new NotImplementedException(); init => throw new NotImplementedException(); }

    public EGenType eGenType => throw new NotImplementedException();

    [TestInitialize]
    public void Initialize()
    {
        _genFact = new(this)
        {
            UId = Guid.Parse("ee43c2a5-2259-4bc5-9913-ce08b984eeac"),
            eFactType = EFactType.Birth,
            Date = new GenDate(new DateTime(1960, 01, 01)),
            Place = new GenPlace("Musterstadt", "Deutschland") { UId = Guid.Parse("164359f0-a3a4-4f9f-8824-af79ec666a45") }
        };
    }

    [TestMethod()]
    public void GenFactTest()
    {
        var genFact = new GenFact(this, EFactType.Age)
        {
            Date = new GenDate(new DateTime(1978, 01, 01)),
            Data = "18"
        };
        Assert.AreEqual(EFactType.Age, genFact.eFactType);
        Assert.AreEqual(new DateTime(1978, 01, 01), genFact.Date?.Date1);
    }

    [TestMethod()]
    public void DateTest()
    {
        Assert.AreEqual(new DateTime(1960, 01, 01), _genFact.Date?.Date1);
    }
    [TestMethod()]
    public void PlaceTest()
    {
        Assert.AreEqual("Musterstadt", _genFact.Place?.Name);
    }
    [TestMethod()]
    public void DataTest()
    {
        Assert.AreEqual(null, _genFact.Data);
    }
    [TestMethod()]
    public void SourcesTest()
    {
        Assert.IsNotNull(_genFact.Sources);
        Assert.AreEqual(0, _genFact.Sources.Count);
    }
    [TestMethod()]
    public void OwnerTest()
    {
        Assert.AreEqual(this, (_genFact as IHasOwner<IGenEntity>).Owner);
    }
    [TestMethod()]
    public void MainEntityTest()
    {
        Assert.AreEqual(this, _genFact.MainEntity);
    }
    [TestMethod()]
    public void EntitiesTest()
    {
        Assert.IsNotNull(_genFact.Entities);
        Assert.AreEqual(0, _genFact.Entities.Count);
    }
    [TestMethod()]
    public void GenTypeTest()
    {
        Assert.AreEqual(EGenType.GenFact, _genFact.eGenType);
    }

    [TestMethod()]
    public void MediasTest()
    {
        Assert.IsNotNull(_genFact.Medias);
        Assert.AreEqual(0, _genFact.Medias.Count);
    }

    public class GenFactConverter : JsonConverter<IGenFact>
    {
        public override IGenFact Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return JsonSerializer.Deserialize<GenFact>(ref reader, options);
        }
        public override void Write(Utf8JsonWriter writer, IGenFact value, JsonSerializerOptions options)
        {
        }
    }

    public class GenDateConverter : JsonConverter<IGenDate>
    {
        public override IGenDate Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return JsonSerializer.Deserialize<GenDate>(ref reader, options);
        }
        public override void Write(Utf8JsonWriter writer, IGenDate value, JsonSerializerOptions options)
        {
        }
    }

    public class GenPlaceConverter : JsonConverter<IGenPlace>
    {
        public override IGenPlace Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return JsonSerializer.Deserialize<GenPlace>(ref reader, options);
        }
        public override void Write(Utf8JsonWriter writer, IGenPlace value, JsonSerializerOptions options)
        {
        }
    }
    [TestMethod()]
    public void SerializationTest()
    {
        JsonSerializerOptions? options = new()
        {
            ReferenceHandler = ReferenceHandler.Preserve,
        };
        var json = System.Text.Json.JsonSerializer.Serialize<GenFact>(_genFact, options);
        Assert.AreEqual(_cGenFJS, json);
        options = new JsonSerializerOptions(options);
        options.Converters.Add(new GenFactConverter());
        options.Converters.Add(new GenDateConverter());
        options.Converters.Add(new GenPlaceConverter());
        var genFact = System.Text.Json.JsonSerializer.Deserialize<IGenFact>(json, options);
          Assert.AreEqual(_genFact.eFactType, genFact.eFactType);
          Assert.AreEqual(_genFact.Date?.Date1, genFact.Date?.Date1);
          Assert.AreEqual(_genFact.Place?.Name, genFact.Place?.Name);
          Assert.AreEqual(_genFact.Data, genFact.Data);
      
    }
}