using Microsoft.VisualStudio.TestTools.UnitTesting;
using BaseGenClasses.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GenInterfaces.Interfaces.Genealogic;
using NSubstitute;
using GenInterfaces.Data;
using BaseLib.Helper;
using System.Runtime.InteropServices;
using BaseGenClasses.Helper.Interfaces;
using GenInterfaces.Interfaces;

namespace BaseGenClasses.Helper.Tests
{

    [TestClass()]
    public class GenExtensionsTests
    {
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Fügen Sie ggf. den „erforderlichen“ Modifizierer hinzu, oder deklarieren Sie den Modifizierer als NULL-Werte zulassend.
        private IList<IGenConnects?> iConnect;
        private IList<IGenFact?> iFacts;
        private IGenConnectBuilder _GCBuilder;
        private IGenFactBuilder _GFBuilder;
        private IGenILBuilder _GILBuilder;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Fügen Sie ggf. den „erforderlichen“ Modifizierer hinzu, oder deklarieren Sie den Modifizierer als NULL-Werte zulassend.

        [TestInitialize]
        public void Init()
        {
            iConnect = [];
            iFacts = [];
            _GCBuilder = Substitute.For<IGenConnectBuilder>();
            _GFBuilder = Substitute.For<IGenFactBuilder>();
            _GILBuilder = Substitute.For<IGenILBuilder>();
            IoC.GetReqSrv = (t) => t switch
                {
                    _ when t == typeof(IList<IGenConnects>) => iConnect,
                    _ when t == typeof(IGenConnectBuilder) => _GCBuilder,
                    _ when t == typeof(IGenFactBuilder) => _GFBuilder,
                    _ when t == typeof(IGenILBuilder) => _GILBuilder,
                    _ => null!
                };
            _GFBuilder.Emit(Arg.Any<EFactType>(), Arg.Any<IGenPerson>(), Arg.Any<string>(), Arg.Any<Guid?>()).Returns(
                (c) =>
                {
                    IGenFact fact = MakeFact(c);
                    iFacts.Add(fact);
                    return fact;
                });
            _GFBuilder.Emit(Arg.Any<EFactType>(), Arg.Any<IGenPerson>(), Arg.Any<IGenDate>(), Arg.Any<IGenPlace>(), Arg.Any<string>(), Arg.Any<Guid?>()).Returns(
              (c) =>
              {
                  IGenFact fact = MakeFact(c);
                  iFacts.Add(fact);
                  return fact;
              });
        }

        private static IGenFact MakeFact(NSubstitute.Core.CallInfo c)
        {
            var eFactType = c.Arg<EFactType>();
            var MainEntity = c.Arg<IGenPerson>();
            var text = c.Arg<string>();
            var guid = c.Arg<Guid?>();
            var fact = Substitute.For<IGenFact>();
            fact.eGenType.Returns(EGenType.GenFact);
            fact.eFactType.Returns(eFactType);
            fact.MainEntity.Returns(MainEntity);
            fact.Data.Returns(text);
            fact.UId.Returns(guid ?? Guid.NewGuid());
            return fact;
        }

        [TestMethod]
        [DataRow(EGenConnectionType.ParentFamily)]
        [DataRow(EGenConnectionType.ChildFamily)]
        public void AddFamilyTest(EGenConnectionType connectionType)
        {
            // Arrange
            var familyEntity = Substitute.For<IGenFamily>();

            // Act
            iConnect.AddFamily(connectionType, familyEntity);

            // Assert
            Assert.IsNotNull(iConnect[0]);
        }

        [TestMethod]
        [DataRow(EGenConnectionType.Friend)]
        [DataRow(EGenConnectionType.Child)]
        public void AddPersonTest(EGenConnectionType connectionType)
        {
            // Arrange
            var personEntity = Substitute.For<IGenPerson>();

            // Act
            iConnect.AddPerson(connectionType, personEntity);

            // Assert
            Assert.IsNotNull(iConnect[0]);
        }

        [TestMethod]
        [DataRow(EFactType.Info)]
        [DataRow(EFactType.Sex)]
        public void AddFactTest(EFactType eFactType)
        {
            // Arrange
            var MainEntity = Substitute.For<IGenPerson>();
            Guid? g;
            // Act
            var result = iFacts.AddFact(MainEntity, eFactType, "Test", g = Guid.NewGuid());

            // Assert
            Assert.IsNotNull(iFacts[0]);
            Assert.AreEqual(result, iFacts[0]);
            Assert.AreEqual(g, result.UId);

        }
        [TestMethod]
        [DataRow(EFactType.Info)]
        [DataRow(EFactType.Sex)]
        public void AddFact2Test(EFactType eFactType)
        {
            // Arrange
            var MainEntity = Substitute.For<IGenPerson>();
            MainEntity.Facts.Returns(iFacts);
            Guid? g;
            // Act
            var result = MainEntity.AddFact(eFactType, "Test", g = Guid.NewGuid());

            // Assert
            Assert.IsNotNull(iFacts[0]);
            Assert.AreEqual(result, iFacts[0]);
            Assert.AreEqual(g, result.UId);

        }

        [TestMethod()]
        [DataRow(EFactType.Birth)]
        [DataRow(EFactType.Death)]
        public void AddEventTest(EFactType eFactType)
        {
            // Arrange
            var MainEntity = Substitute.For<IGenPerson>();
            var GDate = Substitute.For<IGenDate>();
            var GPlace = Substitute.For<IGenPlace>();
            MainEntity.Facts.Returns(iFacts);
            Guid? g;
            // Act
            var result = MainEntity.AddEvent(eFactType, GDate, GPlace, "Test", g = Guid.NewGuid());

            // Assert
            Assert.IsNotNull(iFacts[0]);
            Assert.AreEqual(result, iFacts[0]);
            Assert.AreEqual(g, result.UId);
        }

        [TestMethod()]
        public void ToIndexedListTest()
        {
            // Arrange
            iConnect.AddFamily(EGenConnectionType.ParentFamily, Substitute.For<IGenFamily>());
            iConnect.AddPerson(EGenConnectionType.Friend, Substitute.For<IGenPerson>());

            // Act
            var result = iConnect.Where(i=>i?.Entity != null).ToIndexedList((I)=>I.Entity!.eGenType);

            // Assert
            Assert.IsNotNull(result);
            result.ReceivedWithAnyArgs(2).Add(null!,null!);
        }
    }
}