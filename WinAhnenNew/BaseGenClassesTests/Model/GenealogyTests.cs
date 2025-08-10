using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using CommunityToolkit.Mvvm.Messaging;
using BaseLib.Helper;
using GenInterfaces.Data;
using GenInterfaces.Interfaces;
using GenInterfaces.Interfaces.Genealogic;
using BaseGenClasses.Helper;
using BaseGenClasses.Helper.Interfaces;
using NSubstitute;
using System.Linq;

namespace BaseGenClasses.Model.Tests
{
    [TestClass()]
    public class GenealogyTests
    {
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Fügen Sie ggf. den „erforderlichen“ Modifizierer hinzu, oder deklarieren Sie den Modifizierer als NULL-Werte zulassend.
        private Genealogy _genealogy;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Fügen Sie ggf. den „erforderlichen“ Modifizierer hinzu, oder deklarieren Sie den Modifizierer als NULL-Werte zulassend.
        private Guid _uid;
#pragma warning disable CS0014 
        private readonly string _cGenealogyJS2= "{\"$id\":\"1\",\"eGenType\":11,\"Entitys\":{\"$id\":\"2\",\"$values\":[{\"$id\":\"3\",\"eGenType\":2,\"UId\":\"52d05107-58dc-4e08-a2cf-507e86f93e2f\",\"Facts\":{\"$id\":\"4\",\"$values\":[{\"$id\":\"5\",\"eGenType\":0,\"eFactType\":0,\"Data\":\"Peter\",\"Entities\":{\"$id\":\"6\",\"$values\":[]}},{\"$id\":\"7\",\"eGenType\":0,\"eFactType\":1,\"Data\":\"Mustermann\",\"Entities\":{\"$id\":\"8\",\"$values\":[]}},{\"$id\":\"9\",\"eGenType\":0,\"eFactType\":7,\"Date\":{\"$id\":\"10\",\"eGenType\":10,\"Date1\":\"1950-01-01T00:00:00\"},\"Data\":\"\",\"Entities\":{\"$id\":\"11\",\"$values\":[]}}]},\"Connects\":{\"$id\":\"12\",\"$values\":[]}},{\"$id\":\"13\",\"eGenType\":3,\"UId\":\"c8456df6-cb02-4bee-92d9-652c1fd7f40e\",\"Facts\":{\"$id\":\"14\",\"$values\":[{\"$id\":\"15\",\"eGenType\":0,\"eFactType\":1,\"Data\":\"Mustermann\",\"Entities\":{\"$id\":\"16\",\"$values\":[]}},{\"$id\":\"17\",\"eGenType\":0,\"eFactType\":28,\"Date\":{\"$id\":\"18\",\"eGenType\":10,\"Date1\":\"1950-01-01T00:00:00\"},\"Data\":\"\",\"Entities\":{\"$id\":\"19\",\"$values\":[]}}]},\"Connects\":{\"$id\":\"20\",\"$values\":[]}}]},\"Sources\":{\"$id\":\"21\",\"$values\":[]},\"Places\":{\"$id\":\"22\",\"$values\":[]},\"Medias\":{\"$id\":\"23\",\"$values\":[]},\"Transactions\":{\"$id\":\"24\",\"$values\":[]},\"UId\":\"60bf23eb-d293-4971-8b13-8845ff25d8dd\"}";
#pragma warning restore CS0014 
        private readonly string _cGenealogyJS = "{\"$id\":\"1\",\"eGenType\":11,\"UId\":\"60bf23eb-d293-4971-8b13-8845ff25d8dd\",\"Entitys\":{\"$id\":\"2\",\"$values\":[{\"$id\":\"3\",\"eGenType\":2,\"UId\":\"52d05107-58dc-4e08-a2cf-507e86f93e2f\",\"Facts\":{\"$id\":\"4\",\"$values\":[{\"$id\":\"5\",\"eGenType\":0,\"eFactType\":0,\"Data\":\"Peter\",\"Entities\":{\"$id\":\"6\",\"$values\":[]}},{\"$id\":\"7\",\"eGenType\":0,\"eFactType\":1,\"Data\":\"Mustermann\",\"Entities\":{\"$id\":\"8\",\"$values\":[]}},{\"$id\":\"9\",\"eGenType\":0,\"eFactType\":7,\"Date\":{\"$id\":\"10\",\"eGenType\":10,\"Date1\":\"1950-01-01T00:00:00\"},\"Data\":\"\",\"Entities\":{\"$id\":\"11\",\"$values\":[]}}]},\"Connects\":{\"$id\":\"12\",\"$values\":[]}},{\"$id\":\"13\",\"eGenType\":3,\"UId\":\"c8456df6-cb02-4bee-92d9-652c1fd7f40e\",\"Facts\":{\"$id\":\"14\",\"$values\":[{\"$id\":\"15\",\"eGenType\":0,\"eFactType\":1,\"Data\":\"Mustermann\",\"Entities\":{\"$id\":\"16\",\"$values\":[]}},{\"$id\":\"17\",\"eGenType\":0,\"eFactType\":28,\"Date\":{\"$id\":\"18\",\"eGenType\":10,\"Date1\":\"1950-01-01T00:00:00\"},\"Data\":\"\",\"Entities\":{\"$id\":\"19\",\"$values\":[]}}]},\"Connects\":{\"$id\":\"20\",\"$values\":[]}}]},\"Sources\":{\"$id\":\"21\",\"$values\":[]},\"Places\":{\"$id\":\"22\",\"$values\":[]},\"Repositorys\":{\"$id\":\"23\",\"$values\":[]},\"Medias\":{\"$id\":\"24\",\"$values\":[]},\"Transactions\":{\"$id\":\"25\",\"$values\":[]}}";

        [TestInitialize]
        public void TestInitialize()
        {
            IoC.GetReqSrv = (t) => t switch
            {
                _ when t == typeof(IGenDateBuilder) => new GenDateBuilder(),
                _ when t == typeof(IGenFactBuilder) => new GenFactBuilder(),
                _ when t == typeof(IGenConnectBuilder) => new GenConnectBuilder(),
                _ when t == typeof(IGenILBuilder) => new GenILBuilder(),
                _ when t == typeof(IMessenger) => WeakReferenceMessenger.Default,
                _ => throw new NotImplementedException("Please initialize the service first.")
            };
            _genealogy = new Genealogy(WeakReferenceMessenger.Default)
            {
                UId = _uid = new Guid("60bf23eb-d293-4971-8b13-8845ff25d8dd")
            };
            _genealogy.GetEntity([EGenType.GenPerson, new Guid("52d05107-58dc-4e08-a2cf-507e86f93e2f"), "Peter Mustermann", new DateTime(1950, 1, 1)]);
            _genealogy.GetEntity([EGenType.GenFamily, new Guid("c8456df6-cb02-4bee-92d9-652c1fd7f40e"), "Mustermann", new DateTime(1950, 1, 1)]);
        }

        [TestMethod()]
        public void GenealogyTest()
        {
            Assert.AreEqual(_uid, _genealogy.UId);
            Assert.AreEqual(EGenType.Genealogy, _genealogy.eGenType);
            Assert.AreEqual(2, _genealogy.Entitys.Count);
        }

        static IEnumerable<object> GetEnityTestData => new List<object?[]>()
        {
            new object?[] { null, new ArgumentException("No data to create an entity"),EGenType.GenFact,"",null },
            new object?[] { new object?[] { }, new ArgumentException("No data to create an entity"),EGenType.GenFact,"",null },
            new object?[] { new object?[] { "Bumlux" }, new ArgumentException("No data to create an entity"),EGenType.GenFact,"",null },
            new object?[] { new object?[] { EGenType.GenFact },new ArgumentException("No data to create an entity"),EGenType.GenFact,"",null! },
            new object?[] { new object?[] { new Guid("52d05107-58dc-4e08-a2cf-507e86f93e2f")},null,EGenType.GenPerson,"Peter Mustermann",new DateTime(1950, 1, 1)},
            new object?[] { new object?[] { EGenType.GenPerson },null,EGenType.GenPerson,"",null! },
            new object?[] { new object?[] { EGenType.GenPerson, new Guid("52d05107-58dc-4e08-a2cf-507e86f93e2f"), "Max Testmann", new DateTime(1950, 1, 1) },null,EGenType.GenPerson,"Peter Mustermann", new DateTime(1950, 1, 1) },
            new object?[] { new object?[] { EGenType.GenPerson, new Guid("52d05107-58dc-4e08-a2cf-507e86f93e2e"), new DateTime(1950, 2, 1), "Max Testmann" },null,EGenType.GenPerson,"Max Testmann", new DateTime(1950, 2, 1)  },
            new object?[] { new object?[] { EGenType.GenPerson, "Xaver Testmann", new DateTime(1950, 3, 1) },null,EGenType.GenPerson,"Xaver Testmann", new DateTime(1950, 3, 1) },
            new object?[] { new object?[] { EGenType.GenPerson, "Gerd Altermann" },null,EGenType.GenPerson,"Gerd Altermann", null },
            new object?[] { new object?[] { EGenType.GenFamily, "Testmann1", new DateTime(1960, 1, 1) },null, EGenType.GenFamily, "Testmann1", new DateTime(1960, 1, 1) },
            new object?[] { new object?[] { EGenType.GenFamily, new Guid("c8456df6-cb02-4bee-92d9-652c1fd7f40e"), "Testmann", new DateTime(1960, 1, 1) },null, EGenType.GenFamily, "Mustermann", new DateTime(1950, 1, 1) },
            new object?[] { new object?[] { EGenType.GenFamily, new Guid("c8456df6-cb02-4bee-92d9-652c1fd7f40f"), "Testmann", new DateTime(1960, 2, 1) },null, EGenType.GenFamily, "Testmann", new DateTime(1960, 2, 1) },
            new object?[] { new object?[] { EGenType.GenFamily, new Guid("c8456df6-cb02-4bee-92d9-652c1fd7f40d") },null, EGenType.GenFamily, null, null }
        };

        [TestMethod]
        [DynamicData(nameof(GetEnityTestData))]
        public void GetEntityTest(IEnumerable<object?> param, Exception? ex,EGenType eExpType, string sExpStr, DateTime? dExpDate)
        {
            object? Exec(Action action) { action(); return null; }
            if (ex != null)
            {
                _ = ex switch
                {
                    ArgumentException argEx => Exec(() => Assert.ThrowsException<ArgumentException>(() => _genealogy.GetEntity(param?.ToList()))),
                    _ => Exec(() => Assert.Fail("Unknown exception"))
                };
                return;
            }

            var entity = _genealogy.GetEntity(param?.ToList());
            Assert.IsNotNull(entity);
            Assert.AreEqual(eExpType, entity.eGenType);
            if (entity is IGenPerson person)
            {
                Assert.AreEqual(sExpStr, person.Name);
                Assert.AreEqual(dExpDate, person.BirthDate?.Date1);
            }
            if (entity is IGenFamily family)
            {
                Assert.AreEqual(sExpStr, family.FamilyName);
                Assert.AreEqual(dExpDate, family.MarriageDate?.Date1);
            }
        }

        [TestMethod()]
        public void ReceiveTest()
        {
            WeakReferenceMessenger.Default.Send(Substitute.For<IGenTransaction>());
            Assert.AreEqual(1, _genealogy.Transactions.Count);
        }

        [TestMethod()]
        public void DisposeTest()
        {
            // Arrange
            var genealogy = new Genealogy(WeakReferenceMessenger.Default);

            // Act
            genealogy.Dispose();

            // Assert
            // Nach Dispose sollten keine weiteren Transaktionen empfangen werden
            var initialCount = genealogy.Transactions.Count;
            WeakReferenceMessenger.Default.Send(Substitute.For<IGenTransaction>());
            Assert.AreEqual(initialCount, genealogy.Transactions.Count);
        }

        [TestMethod()]
        public void SerializeTest()
        {
            var options = new JsonSerializerOptions()
            {
                ReferenceHandler = ReferenceHandler.Preserve
            };
            var json = JsonSerializer.Serialize(_genealogy, options);
            Assert.AreEqual(_cGenealogyJS, json);
        }

        [TestMethod()]
        public void DeSerializeTest()
        {
            var options = new JsonSerializerOptions()
            {
                ReferenceHandler = ReferenceHandler.Preserve
            };
            var json = JsonSerializer.Serialize(_genealogy, options);
            Assert.AreEqual(_cGenealogyJS, json);
            var options2 = new JsonSerializerOptions()
            {
                ReferenceHandler = ReferenceHandler.Preserve
            };
            options2.Converters.Add(new GenConverter<List<IGenFact>, IList<IGenFact>>());
            options2.Converters.Add(new GenConverter<GenFact, IGenFact>());
            options2.Converters.Add(new GenConverter<GenDate, IGenDate>());
            options2.Converters.Add(new GenConverter<GenPlace, IGenPlace>());
            options2.Converters.Add(new GenConverter<GenConnect, IGenConnects>());
            options2.Converters.Add(new GenPolyConverter<IGenEntity>([(EGenType.GenPerson, typeof(GenPerson)), (EGenType.GenFamily, typeof(GenFamily))]));
            var genealogy = JsonSerializer.Deserialize<Genealogy>(json, options2);
        }
    }
}