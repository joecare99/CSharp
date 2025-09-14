using BaseLib.Interfaces;
using GenFree.Data;
using GenFree.Interfaces.DB;
using GenFree.Interfaces.Model;
using GenFree.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using NSubstitute.ReceivedExtensions;
using System;

namespace GenFree.Models.Tests
{
    [TestClass()]
    public class COFBTests
    {
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
        private COFB testClass;
        private IRecordset testRS;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
        [TestInitialize]
        public void Init()
        {
            testRS = Substitute.For<IRecordset>();
            testClass = new COFB(() => testRS);
            testRS.NoMatch.Returns(true);
            (testRS.Fields[OFBFields.PerNr] as IHasValue).Value.Returns(1, 2, 3);
            (testRS.Fields[OFBFields.Kennz] as IHasValue).Value.Returns("AA", "BB", "OO");
            (testRS.Fields[OFBFields.TextNr] as IHasValue).Value.Returns(2, 3, 4);
            testRS.ClearReceivedCalls();
        }

        [TestMethod()]
        public void COFBTest()
        {
            Assert.IsNotNull(testClass);
            Assert.IsInstanceOfType(testClass, typeof(IOFB));
            Assert.IsInstanceOfType(testClass, typeof(COFB));
        }

        [TestMethod()]
        public void MaxIDTest()
        {
            Assert.AreEqual((1, "AA", 2), testClass.MaxID);
            testRS.Received().MoveLast();
            Assert.AreEqual(nameof(OFBIndex.Indn), testRS.Index);
        }

        [TestMethod()]
        [DataRow(OFBIndex.InDNr, OFBFields.PerNr)]
        [DataRow(OFBIndex.IndNum, OFBFields.TextNr)]
        public void GetIndex1FieldTest(OFBIndex eAct, OFBFields eExp)
        {
            Assert.AreEqual(eExp, testClass.GetIndex1Field(eAct));
        }

        [TestMethod()]
        [DataRow(OFBIndex.Indn, OFBFields.PerNr)]
        public void GetIndex1FieldTest1(OFBIndex eAct, OFBFields eExp)
        {
            Assert.ThrowsException<ArgumentException>(() => testClass.GetIndex1Field(eAct));
        }

        [TestMethod()]
        [DataRow("Null", 0, ETextKennz.tkNone, 0, false)]
        [DataRow("1-None-0", 1, ETextKennz.tkNone, 0, false)]
        [DataRow("1-Name-2", 1, ETextKennz.tkName, 2, true)]
        public void ReadDataTest(string sName, int iActOFB, Enum eTKennz, int iLfNr, bool xExp)
        {
            testRS.NoMatch.Returns(iActOFB is not (> 0 and < 3) || iLfNr / 2 != iActOFB);
            Assert.AreEqual(xExp, testClass.ReadData((iActOFB, "AA", iLfNr), out var cPd));

            Assert.AreEqual(nameof(OFBIndex.Indn), testRS.Index);
            testRS.Received().Seek("=", iActOFB, "AA", iLfNr);

            if (xExp)
            {
                Assert.IsNotNull(cPd);
                Assert.AreEqual(iActOFB, cPd.iPerNr);
                Assert.AreEqual("AA", cPd.sKennz);
                Assert.AreEqual(iLfNr, cPd.iTextNr);
            }
            else
            {
                Assert.IsNull(cPd);
            }
        }

        [TestMethod()]
        [DataRow("Null", 0, ETextKennz.tkNone, 0, false)]
        [DataRow("1-None-0", 1, ETextKennz.tkNone, 0, false)]
        [DataRow("1-Name-2", 1, ETextKennz.tkName, 2, true)]
        public void SeekTest(string sName, int iActOFB, Enum eTKennz, int iLfNr, bool xExp)
        {
            testRS.NoMatch.Returns(iActOFB is not (> 0 and < 3) || iLfNr / 2 != iActOFB);
            Assert.AreEqual(xExp ? testRS : null, testClass.Seek((iActOFB, "AA", iLfNr), out var xBr));
            Assert.AreEqual(!xExp, xBr);
            Assert.AreEqual(nameof(OFBIndex.Indn), testRS.Index);
            testRS.Received().Seek("=", iActOFB, "AA", iLfNr);

        }

        /// <summary>
        /// Testet die Methode TextExist von COFB für verschiedene TextNr-Werte.
        /// Erwartet wird, dass TextExist true zurückgibt, wenn der Recordset nicht auf NoMatch steht.
        /// </summary>
        /// <param name="textNr">Die zu prüfende TextNr</param>
        /// <param name="noMatch">Ob der Recordset NoMatch zurückgeben soll</param>
        /// <param name="expected">Das erwartete Ergebnis von TextExist</param>
        [TestMethod]
        [DataRow(0, true, false, DisplayName = "TextNr 0, NoMatch true => Existiert nicht")]
        [DataRow(2, false, true, DisplayName = "TextNr 2, NoMatch false => Existiert")]
        [DataRow(3, true, false, DisplayName = "TextNr 3, NoMatch true => Existiert nicht")]
        [DataRow(4, false, true, DisplayName = "TextNr 4, NoMatch false => Existiert")]
        public void TextExistTest(int textNr, bool noMatch, bool expected)
        {
            // Arrange
            testRS.NoMatch.Returns(noMatch);

            // Act
            var result = testClass.TextExist(textNr);

            // Assert
            Assert.AreEqual(expected, result);
            Assert.AreEqual(nameof(OFBIndex.IndNum), testRS.Index);
            testRS.Received().Seek("=", textNr);
        }

        /// <summary>
        /// Testet die Exists-Methode von COFB für verschiedene Kombinationen von Index, PerNr und Kennz.
        /// Erwartet wird, dass Exists true zurückgibt, wenn der Recordset nicht auf NoMatch steht.
        /// </summary>
        /// <param name="index">Der zu verwendende Index</param>
        /// <param name="persInArb">Die Personennummer</param>
        /// <param name="kennz">Das Kennzeichen</param>
        /// <param name="noMatch">Ob der Recordset NoMatch zurückgeben soll</param>
        /// <param name="expected">Das erwartete Ergebnis von Exists</param>
        [TestMethod]
        [DataRow(OFBIndex.InDNr, 1, "AA", false, true, DisplayName = "InDNr, PerNr 1, Kennz 'AA', NoMatch false => Existiert")]
        [DataRow(OFBIndex.InDNr, 2, "BB", true, false, DisplayName = "InDNr, PerNr 2, Kennz 'BB', NoMatch true => Existiert nicht")]
        [DataRow(OFBIndex.IndNum, 3, "OO", false, true, DisplayName = "IndNum, PerNr 3, Kennz 'OO', NoMatch false => Existiert")]
        [DataRow(OFBIndex.IndNum, 4, "XX", true, false, DisplayName = "IndNum, PerNr 4, Kennz 'XX', NoMatch true => Existiert nicht")]
        public void ExistsTest(OFBIndex index, int persInArb, string kennz, bool noMatch, bool expected)
        {
            // Arrange
            testRS.NoMatch.Returns(noMatch);

            // Act
            var result = testClass.Exists(index, persInArb, kennz);

            // Assert
            Assert.AreEqual(expected, result);
            Assert.AreEqual(index.ToString(), testRS.Index);
            testRS.Received().Seek("=", persInArb, kennz);
        }

        /// <summary>
        /// Testet die Update-Methode von COFB für verschiedene Kombinationen von Kennz, PerNr und Satz.
        /// Erwartet wird, dass die Update-Methode den Recordset in den Edit-Modus versetzt und Update aufruft.
        /// </summary>
        /// <param name="kennz">Das Kennzeichen</param>
        /// <param name="persInArb">Die Personennummer</param>
        /// <param name="satz">Der Satz</param>
        [TestMethod]
        [DataRow("AA", 1, 2, DisplayName = "Update mit Kennz 'AA', PerNr 1, Satz 2")]
        [DataRow("BB", 2, 3, DisplayName = "Update mit Kennz 'BB', PerNr 2, Satz 3")]
        [DataRow("CC", 3, 4, DisplayName = "Update mit Kennz 'CC', PerNr 3, Satz 4")]
        public void UpdateTest(string kennz, int persInArb, int satz)
        {
            // Arrange
            testRS.NoMatch.Returns(persInArb != 1);

            // Act
            testClass.Update(kennz, persInArb, satz);

            // Assert
            Assert.AreEqual(nameof(OFBIndex.Indn), testRS.Index);
            testRS.Received().Seek("=", persInArb, kennz, satz);
            testRS.Received(persInArb != 1 ? 1 : 0).AddNew();
            testRS.Received(persInArb != 1 ? 1 : 0).Update();
        }

        /// <summary>
        /// Testet die SeekIndNr-Methode von COFB für verschiedene Kombinationen von PerNr und Kennz.
        /// Erwartet wird, dass SeekIndNr den Recordset zurückgibt, wenn NoMatch false ist, sonst null.
        /// </summary>
        /// <param name="persInArb">Die Personennummer</param>
        /// <param name="kennz">Das Kennzeichen</param>
        /// <param name="noMatch">Ob der Recordset NoMatch zurückgeben soll</param>
        /// <param name="expectedFound">Ob ein Datensatz gefunden werden soll</param>
        [TestMethod]
        [DataRow(1, "AA", false, true, DisplayName = "PerNr 1, Kennz 'AA', NoMatch false => gefunden")]
        [DataRow(2, "BB", true, false, DisplayName = "PerNr 2, Kennz 'BB', NoMatch true => nicht gefunden")]
        [DataRow(3, "CC", false, true, DisplayName = "PerNr 3, Kennz 'CC', NoMatch false => gefunden")]
        [DataRow(4, "DD", true, false, DisplayName = "PerNr 4, Kennz 'DD', NoMatch true => nicht gefunden")]
        public void SeekIndNrTest(int persInArb, string kennz, bool noMatch, bool expectedFound)
        {
            // Arrange
            testRS.NoMatch.Returns(noMatch);

            // Act
            var result = testClass.SeekIndNr(persInArb, kennz, out var xBreak);

            // Assert
            Assert.AreEqual(expectedFound ? testRS : null, result);
            Assert.AreEqual(!expectedFound, xBreak);
            Assert.AreEqual(nameof(OFBIndex.InDNr), testRS.Index);
            testRS.Received().Seek("=", persInArb, kennz);
        }

        /// <summary>
        /// Testet die Methode DeleteIndNr von COFB für verschiedene Kombinationen von PerNr und Kennz.
        /// Erwartet wird, dass DeleteIndNr true zurückgibt, wenn der Recordset nicht auf NoMatch steht und Delete aufgerufen wird,
        /// andernfalls false zurückgibt und Delete nicht aufgerufen wird.
        /// </summary>
        /// <param name="persInArb">Die Personennummer</param>
        /// <param name="kennz">Das Kennzeichen</param>
        /// <param name="noMatch">Ob der Recordset NoMatch zurückgeben soll</param>
        /// <param name="expected">Das erwartete Ergebnis von DeleteIndNr</param>
        [TestMethod]
        [DataRow(1, "AA", false, true, DisplayName = "PerNr 1, Kennz 'AA', NoMatch false => gelöscht")]
        [DataRow(2, "BB", true, false, DisplayName = "PerNr 2, Kennz 'BB', NoMatch true => nicht gelöscht")]
        [DataRow(3, "CC", false, true, DisplayName = "PerNr 3, Kennz 'CC', NoMatch false => gelöscht")]
        [DataRow(4, "DD", true, false, DisplayName = "PerNr 4, Kennz 'DD', NoMatch true => nicht gelöscht")]
        public void DeleteIndNrTest(int persInArb, string kennz, bool noMatch, bool expected)
        {
            // Arrange
            testRS.NoMatch.Returns(noMatch);

            // Act
            var result = testClass.DeleteIndNr(persInArb, kennz);

            // Assert
            Assert.AreEqual(expected, result, $"DeleteIndNr sollte für PerNr={persInArb}, Kennz={kennz}, NoMatch={noMatch} {expected} zurückgeben.");
            Assert.AreEqual(nameof(OFBIndex.InDNr), testRS.Index, "Index sollte auf InDNr gesetzt werden.");
            testRS.Received().Seek("=", persInArb, kennz);
            if (expected)
            {
                testRS.Received(1).Delete();
            }
            else
            {
                testRS.DidNotReceive().Delete();
            }
        }
    }
}