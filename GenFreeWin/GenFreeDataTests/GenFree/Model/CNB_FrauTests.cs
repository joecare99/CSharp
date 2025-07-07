using System;
using GenFree.Models;
using GenFree.Interfaces.DB;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using BaseLib.Interfaces;
using GenFree.Data;

namespace GenFree.Models.Tests
{
    /// <summary>
    /// Testklasse für <see cref="CNB_Frau"/>.
    /// </summary>
    [TestClass]
    public class CNB_FrauTests
    {
        private IRecordset _recordsetMock;
        private CNB_Frau _cnbFrau;

        /// <summary>
        /// Initialisiert die Testumgebung vor jedem Test.
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            _recordsetMock = Substitute.For<IRecordset>();
            _cnbFrau = new CNB_Frau(() => _recordsetMock);
        }

        /// <summary>
        /// Testet die Methode Seek für einen gefundenen Datensatz.
        /// </summary>
        [TestMethod]
        public void Seek_Found_ReturnsRecordset()
        {
            // Arrange
            _recordsetMock.NoMatch.Returns(false);

            // Act
            var result = _cnbFrau.Seek(1, out bool xBreak);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(xBreak);
        }

        /// <summary>
        /// Testet die Methode Seek für einen nicht gefundenen Datensatz.
        /// </summary>
        [TestMethod]
        public void Seek_NotFound_ReturnsNull()
        {
            // Arrange
            _recordsetMock.NoMatch.Returns(true);

            // Act
            var result = _cnbFrau.Seek(1, out bool xBreak);

            // Assert
            Assert.IsNull(result);
            Assert.IsTrue(xBreak);
        }

        /// <summary>
        /// Testet, ob die Eigenschaft ID den Wert 3 zurückgibt, wenn das entsprechende Feld gesetzt ist.
        /// </summary>
        [TestMethod]
        public void GetID_Returns3()
        {
            // Arrange
            var fields = _recordsetMock.Fields;
            (fields[NB_Frau1Fields.LfNr] as IHasValue).Value.Returns(3);

            // Act
            var result = _cnbFrau.MaxID;

            // Assert
            Assert.AreEqual(3,result);
        }

        /// <summary>
        /// Testet die Methode ReadData mit vorhandenem Datensatz.
        /// </summary>
        [TestMethod]
        public void ReadData_Exists_ReturnsTrueAndOutputs()
        {
            // Arrange
            var fields = _recordsetMock.Fields;
            _recordsetMock.Fields.Returns(fields);
            _recordsetMock.NoMatch.Returns(false);
            (fields[NB_Frau1Fields.Gen] as IHasValue).Value.Returns(2);
            (fields[NB_Frau1Fields.Nr] as IHasValue).Value.Returns(3);
            (fields[NB_Frau1Fields.Alt] as IHasValue).Value.Returns(4);
            (fields[NB_Frau1Fields.Kek1] as IHasValue).Value.Returns(5);
            (fields[NB_Frau1Fields.Kek2] as IHasValue).Value.Returns(6);

            // Act
            var result = _cnbFrau.ReadData(1, out int persNr, out int famNr, out int gen, out int kek2, out int kek1);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(3, persNr);
            Assert.AreEqual(4, famNr);
            Assert.AreEqual(2, gen);
            Assert.AreEqual(6, kek2);
            Assert.AreEqual(5, kek1);
        }

        /// <summary>
        /// Testet die Methode ReadData mit nicht vorhandenem Datensatz.
        /// </summary>
        [TestMethod]
        public void ReadData_NotExists_ReturnsFalseAndDefaults()
        {
            // Arrange
            _recordsetMock.NoMatch.Returns(true);

            // Act
            var result = _cnbFrau.ReadData(1, out int persNr, out int famNr, out int gen, out int kek2, out int kek1);

            // Assert
            Assert.IsFalse(result);
            Assert.AreEqual(0, persNr);
            Assert.AreEqual(0, famNr);
            Assert.AreEqual(0, gen);
            Assert.AreEqual(0, kek2);
            Assert.AreEqual(0, kek1);
        }

        /// <summary>
        /// Testet die Methode CReadData mit vorhandenem Datensatz.
        /// </summary>
        [TestMethod]
        public void CReadData_Exists_ReturnsTrueAndTuple()
        {
            // Arrange
            _recordsetMock.NoMatch.Returns(false);
            var fields = _recordsetMock.Fields;
            (fields[NB_Frau1Fields.Gen] as IHasValue).Value.Returns(2);
            (fields[NB_Frau1Fields.Nr] as IHasValue).Value.Returns(3);
            (fields[NB_Frau1Fields.Alt] as IHasValue).Value.Returns(4);

            // Act
            var result = _cnbFrau.CReadData(1, out var value);

            // Assert
            Assert.IsTrue(result);
            Assert.IsNotNull(value);
            Assert.AreEqual((2, 3, 4), value.Value);
        }

        /// <summary>
        /// Testet die Methode CReadData mit nicht vorhandenem Datensatz.
        /// </summary>
        [TestMethod]
        public void CReadData_NotExists_ReturnsFalseAndNull()
        {
            // Arrange
            _recordsetMock.NoMatch.Returns(true);

            // Act
            var result = _cnbFrau.CReadData(1, out var value);

            // Assert
            Assert.IsFalse(result);
            Assert.IsNull(value);
        }

        /// <summary>
        /// Testet die Methode PersonExists für einen gefundenen Datensatz.
        /// </summary>
        [TestMethod]
        public void PersonExists_Found_ReturnsTrue()
        {
            // Arrange
            _recordsetMock.NoMatch.Returns(false);

            // Act
            var result = _cnbFrau.PersonExists(123);

            // Assert
            Assert.IsTrue(result);
        }

        /// <summary>
        /// Testet die Methode PersonExists für einen nicht gefundenen Datensatz.
        /// </summary>
        [TestMethod]
        public void PersonExists_NotFound_ReturnsFalse()
        {
            // Arrange
            _recordsetMock.NoMatch.Returns(true);

            // Act
            var result = _cnbFrau.PersonExists(123);

            // Assert
            Assert.IsFalse(result);
        }

        /// <summary>
        /// Testet die Methode AddRow mit verschiedenen Parametern.
        /// </summary>
        [TestMethod]
        [DataRow(1, 2, 3, 4, 5, 6)]
        [DataRow(10, 20, 30, 40, 50, 60)]
        public void AddRow_AddsRowWithCorrectValues(int iNr, int iGen, int iPersNr, int iFamNr, int iKek1, int iKek2)
        {
            // Arrange
            var fields = _recordsetMock.Fields;

            // Act
            _cnbFrau.AddRow(iNr, iGen, iPersNr, iFamNr, iKek1, iKek2);

            // Assert
            Received.InOrder(() =>
            {
                _recordsetMock.AddNew();
                fields[NB_Frau1Fields.Nr].Value = iPersNr;
                fields[NB_Frau1Fields.Kek1].Value = iKek1;
                fields[NB_Frau1Fields.Kek2].Value = iKek2;
                fields[NB_Frau1Fields.Gen].Value = iGen;
                fields[NB_Frau1Fields.LfNr].Value = iNr;
                fields[NB_Frau1Fields.Alt].Value = iFamNr;
                _recordsetMock.Update();
            });
        }
    }
}
