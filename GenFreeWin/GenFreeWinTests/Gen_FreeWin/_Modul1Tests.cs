using Microsoft.VisualStudio.TestTools.UnitTesting;
using GenFree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseLib.Helper;
using GenFree.Interfaces.UI;
using NSubstitute;
using GenFree.ViewModels.Interfaces;
using GenFree.Data;
using GenFree.Interfaces.Model;
using GenFree.Interfaces.DB;
using GenFree.Interfaces.Sys;
using BaseLib.Interfaces;
using GenFree.Helper;
using GenFree.Interfaces.VB;

namespace GenFree.Tests
{
    [TestClass()]
    public class _Modul1Tests
    {
        private IApplUserTexts _ut;
        private IInteraction _ia;
        private IModul1 Modul1; // Property to access the instance of _Modul1

        [TestInitialize]
        public void Initialize()
        {

            IoC.GetReqSrv = type => type switch
            {
                Type t when t == typeof(_Modul1) => _Modul1.Instance,
                Type t when t == typeof(IApplUserTexts) => _ut ??= Substitute.For<IApplUserTexts>(),
                Type t when t == typeof(IProjectData) => Substitute.For<IProjectData>(),
                Type t when t == typeof(IInteraction) => _ia ??= Substitute.For<IInteraction>(),
                Type t when t == typeof(IVBInformation) => Substitute.For<IVBInformation>(),
                Type t when t == typeof(IOperators) => Substitute.For<IOperators>(),
                _ => throw new NotImplementedException($"Service for type {type.Name} not implemented.")
            };

            // Setze DataModul statisch
            DataModul.DB_TexteTable = Substitute.For<IRecordset>();
            DataModul.DB_NameTable = Substitute.For<IRecordset>();

            typeof(_Modul1).GetField("<Instance>k__BackingField", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic).SetValue(null, null);
            Modul1 = _Modul1.Instance;
        }

        [TestMethod]
        [DataRow("", "----")]
        [DataRow("0000", "000000")]
        [DataRow("5959", "999722")]
        public void DezRechnenTest(string sAct1, string sExp)
        {
            // Arrange
            var modul = _Modul1.Instance;

            // Act
            var result = modul.DezRechnen(sExp, sAct1);

            // Assert
            Assert.AreEqual(sExp, result);
        }

        [TestMethod()]
        public void InfoTest()
        {
            // Act
            Modul1.Info();

            // Assert
            var _ = _ia.ReceivedWithAnyArgs(1).MsgBox("");
           
        }

        [TestMethod]
        [DataRow("Test", "Leit", ETextKennz.A_, 1, 0, (short)0, true, false, false, DisplayName = "PersonText, PersInArb!=0, sText!=empty")]
        [DataRow("Test", "Leit", ETextKennz.A_, 0, 0, (short)0, false, true, false, DisplayName = "PersonText, PersInArb==0, sText!=empty")]
        [DataRow("Test", "Leit", ETextKennz.E_, 1, 0, (short)0, false, false, true, DisplayName = "XXText, sText!=empty")]
        [DataRow("", "Leit", ETextKennz.A_, 1, 0, (short)0, false, false, false, DisplayName = "PersonText, PersInArb!=0, sText==empty")]
        public void TextSpeich_VerschiedeneFaelle_RichtigesVerhalten(
      string sText, string sLeitName, ETextKennz eTKennz, int PersInArb, int LfNR, short ruf,
      bool expectUpdate, bool expectReturnSatz, bool expectReturnSatzXX)
        {
            // Arrange
            int outSatz = 42;
            (DataModul.DB_TexteTable.Fields[TexteFields.TxNr] as IHasValue).Value.Returns(outSatz);

            // Act
            var result = Modul1.TextSpeich(sText, sLeitName, eTKennz, PersInArb, LfNR, ruf % 2 != 0, ruf / 2 != 0);

            // Assert
            if (sText != "")
            {
                DataModul.DB_TexteTable.Received(1).Index = nameof(TexteIndex.ITexte);
                DataModul.DB_TexteTable.Received(1).Seek("=", sText, (char)eTKennz);
            }
            else
            {
                DataModul.DB_NameTable.Received(1).Index = NameIndex.NamKenn.AsFld();
                DataModul.DB_NameTable.Received(1).Seek("=", PersInArb, (char)eTKennz);
                DataModul.DB_NameTable.Received(1).Delete();
            }
            if (expectUpdate)
            {
                DataModul.DB_NameTable.Received(1).Index = NameIndex.Vollname.AsFld();
                DataModul.DB_NameTable.Received(1).Seek("=", PersInArb, (char)eTKennz, 0);
            }
        }

    }
}