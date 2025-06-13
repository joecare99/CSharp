using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using GenFree.Interfaces.DB;
using NSubstitute;
using GenFree.Interfaces.Sys;
using GenFree.Interfaces.Model;
using GenFree.Interfaces.Data;
using BaseLib.Interfaces;

namespace GenFree.Data.Tests
{
    [TestClass()]
    public class CFamilyTests
    {
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
        private CFamily testClass;
        private IRecordset testRS;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.

        [TestInitialize]
        public void Init()
        {
            testRS = Substitute.For<IRecordset>();
            var testST = Substitute.For<ISysTime>();
            testST.Now.Returns(new DateTime(2022, 12, 31));
            testClass = new CFamily(() => testRS, testST);
            testRS.NoMatch.Returns(true);
            (testRS.Fields[FamilyFields.FamNr] as IHasValue).Value.Returns(2, 6, 4, 9);
            (testRS.Fields[FamilyFields.Eltern] as IHasValue).Value.Returns('N', 'V', 'V', 'A', 'B');
            (testRS.Fields[FamilyFields.Aeb] as IHasValue).Value.Returns(1, 3, 5);
            testRS.ClearReceivedCalls();
        }

        [TestMethod()]
        public void CFamilyTest()
        {
            Assert.IsNotNull(testClass);
            Assert.IsInstanceOfType(testClass, typeof(IFamily));
            Assert.IsInstanceOfType(testClass, typeof(CFamily));
        }

        [TestMethod()]
        public void MaxIDTest()
        {
            Assert.AreEqual(2, testClass.MaxID);
            Assert.AreEqual(nameof(FamilyIndex.Fam), testRS.Index);
            testRS.Received(1).MoveLast();
            testRS.Received(1).Fields[0].Value = 0;
        }

        [DataTestMethod()]
        [DataRow("Null", 0, ETextKennz.tkNone, 0, false)]
        [DataRow("1-None-0", 1, ETextKennz.tkNone, 0, false)]
        [DataRow("1-Name-2", 1, ETextKennz.tkName, 2, true)]
        public void SetNameNrTest(string sName, int iActFamNr, object _, int iName, bool xExp)
        {
            testRS.NoMatch.Returns(iActFamNr is not (> 0 and < 3) || iName / 2 != iActFamNr);
            testClass.SetNameNr(iActFamNr, iName);
            Assert.AreEqual(nameof(FamilyIndex.Fam), testRS.Index);
            testRS.Received(1).Seek("=", iActFamNr);
            _ = (testRS.Received(xExp ? 2 : 8).Fields[""] as IHasValue).Value;
            _ = testRS.Received(xExp ? 2 : 1).NoMatch;
            _ = testRS.Received(0).EOF;
            testRS.Received(xExp ? 0 : 1).AddNew();
            testRS.Received(xExp ? 1 : 0).Edit();
            testRS.Received(1).Update();
        }

        [DataTestMethod()]
        [DataRow("Null", 0, EFamilyProp.sBem, 0, false)]
        [DataRow("1-None-0", 1, EFamilyProp.iPrae, 0, false)]
        [DataRow("1-Name-2", 1, EFamilyProp.xAeB, 2, true)]

        public void SetValueTest(string sName, int iActFamNr, EFamilyProp eFProp, int iName, bool xExp)
        {
            testRS.NoMatch.Returns(iActFamNr is not (> 0 and < 3) || iName / 2 != iActFamNr);
            testClass.SetValue(iActFamNr, iName, new[] { (eFProp, (object)sName) });
            Assert.AreEqual(nameof(FamilyIndex.Fam), testRS.Index);
            testRS.Received(1).Seek("=", iActFamNr);
            _ = (testRS.Received(xExp ? 3 : 8).Fields[""] as IHasValue).Value;
            _ = testRS.Received(xExp ? 2 : 1).NoMatch;
            _ = testRS.Received(0).EOF;
            testRS.Received(xExp ? 0 : 1).AddNew();
            testRS.Received(xExp ? 2 : 1).Edit();
            testRS.Received(2).Update();
        }

        [DataTestMethod()]
        [DataRow("Null", 0, ETextKennz.tkNone, 0, false)]
        [DataRow("1-None-0", 1, ETextKennz.tkNone, 0, false)]
        [DataRow("1-Name-2", 1, ETextKennz.tkName, 2, true)]

        public void SeekTest(string sName, int iActFamNr, Enum eTKennz, int iLfNr, bool xExp)
        {
            testRS.NoMatch.Returns(iActFamNr is not (> 0 and < 3) || iLfNr / 2 != iActFamNr);
            Assert.AreEqual(xExp ? testRS : null, testClass.Seek(iActFamNr, out var xBreak));
            Assert.AreEqual(!xExp, xBreak);
            Assert.AreEqual(nameof(FamilyIndex.Fam), testRS.Index);
            testRS.Received().Seek("=", iActFamNr);
        }

        [DataTestMethod()]
        [DataRow("Null", 0, ETextKennz.tkNone, 0, false)]
        [DataRow("1-None-0", 1, ETextKennz.tkNone, 0, false)]
        [DataRow("1-Name-2", 1, ETextKennz.tkName, 2, true)]

        public void ReadDataTest(string sName, int iActFamNr, Enum eTKennz, int iLfNr, bool xExp)
        {
            testRS.NoMatch.Returns(iActFamNr is not (> 0 and < 3) || iLfNr / 2 != iActFamNr);
            Assert.AreEqual(xExp, testClass.ReadData(iActFamNr, out var cPd));
            Assert.AreEqual(nameof(FamilyIndex.Fam), testRS.Index);
            testRS.Received().Seek("=", iActFamNr);
        }

        [DataTestMethod()]
        [DataRow("Null", 0, ETextKennz.tkNone, 0, false)]
        [DataRow("1-None-0", 1, ETextKennz.tkNone, 0, false)]
        [DataRow("1-Name-2", 1, ETextKennz.tkName, 2, true)]

        public void ReadAllTest(string sName, int iActFamNr, Enum eTKennz, int iLfNr, bool xExp)
        {
            testRS.NoMatch.Returns(iActFamNr is not (> 0 and < 3) || iLfNr / 2 != iActFamNr, false, false, true);
            testRS.EOF.Returns(iActFamNr is not (> 0 and < 3) || iLfNr / 2 != iActFamNr, false, false, true);
            var iCnt = 0;
            foreach (var cNm in testClass.ReadAll())
            {
                Assert.IsNotNull(cNm);
                Assert.IsInstanceOfType(cNm, typeof(IFamilyData));
                Assert.IsInstanceOfType(cNm, typeof(CFamilyPersons));
                iCnt++;
            }
            Assert.AreEqual(xExp ? 2 : 0, iCnt);
            Assert.AreEqual(nameof(FamilyIndex.Fam), testRS.Index);
            testRS.Received(xExp ? 2 : 0).MoveNext();
            testRS.Received(1).MoveFirst();

        }

        [DataTestMethod()]
        [DataRow("Null", 0, ETextKennz.tkNone, 0, false)]
        [DataRow("1-None-0", 1, ETextKennz.tkNone, 0, false)]
        [DataRow("1-Name-2", 1, ETextKennz.tkName, 2, true)]

        public void SetDataTest(string sName, int iActFamNr, Enum eTKennz, int iLfNr, bool xExp)
        {
            testRS.NoMatch.Returns(iActFamNr is not (> 0 and < 3) || iLfNr / 2 != iActFamNr);
            var testPD = Substitute.For<IFamilyData>();
            testClass.SetData(iActFamNr, testPD);
            Assert.AreEqual(nameof(FamilyIndex.Fam), testRS.Index);
            testRS.Received().Seek("=", iActFamNr);
        }

        [DataTestMethod()]
        [DataRow(FamilyIndex.Fam, FamilyFields.FamNr)]
        [DataRow(FamilyIndex.Fuid, FamilyFields.Fuid)]
        [DataRow(FamilyIndex.BeaDat, FamilyFields.EditDat)]
        public void GetIndex1FieldTest(FamilyIndex eAct, FamilyFields eExp)
        {
            Assert.AreEqual(eExp, testClass.GetIndex1Field(eAct));
        }

        [DataTestMethod()]
        [DataRow((FamilyIndex)5, FamilyFields.FamNr)]
        [DataRow((FamilyIndex)7, FamilyFields.FamNr)]
        public void GetIndex1FieldTest2(FamilyIndex eAct, FamilyFields eExp)
        {
            Assert.ThrowsException<ArgumentException>(() => testClass.GetIndex1Field(eAct));
        }

        [DataTestMethod()]
        [DataRow(1, "2023-01-01")]
        [DataRow(2, "default")]
        public void AllSetEditDateTest(int famNr, string expectedDateStr)
        {
            // Arrange
            DateTime expectedDate = default;
            if (expectedDateStr != "default")
            {
                expectedDate = DateTime.Parse(expectedDateStr);

            }
            var testData = Substitute.For<IFamilyData>();
            testData.dEditDat.Returns(DateTime.MinValue);
            testData.ID.Returns(famNr);
            testRS.RecordCount.Returns(1);
            testRS.EOF.Returns(false, true);
            testRS.MoveFirst();
            (testRS.Fields[FamilyFields.FamNr] as IHasValue).Value.Returns(famNr);
            (testRS.Fields[FamilyFields.EditDat] as IHasValue).Value.Returns(expectedDate);
            testClass = new CFamily(() => testRS, Substitute.For<ISysTime>());
            // Act
            testClass.AllSetEditDate();
            // Assert
            testRS.Received(2).MoveFirst();
            testRS.Received(famNr-1).Edit();
            testRS.Received(famNr-1).Update();
            Assert.AreEqual(expectedDate, (testRS.Fields[FamilyFields.EditDat] as IHasValue).Value);
        }

        [DataTestMethod()]
        [DataRow(1, 2, 3, "Bemerkung1")]
        [DataRow(5, 10, 0, "")]
        [DataRow(0, 0, 0, null)]
        public void AppendRawTest(int famNr, int name, int aeb, string bem1)
        {
            // Arrange
            testRS.NoMatch.Returns(true, false);
            testRS.EOF.Returns(false, true);
            (testRS.Fields[FamilyFields.FamNr] as IHasValue).Value.Returns(famNr);
            (testRS.Fields[FamilyFields.Name] as IHasValue).Value.Returns(name);
            (testRS.Fields[FamilyFields.Aeb] as IHasValue).Value.Returns(aeb);
            (testRS.Fields[FamilyFields.Bem1] as IHasValue).Value.Returns(bem1);
            testRS.ClearReceivedCalls();

            // Act
            testClass.AppendRaw(famNr, name, aeb, bem1);

            // Assert
            testRS.Received(1).AddNew();
            testRS.Fields[FamilyFields.FamNr].Received(1).Value = famNr;
            testRS.Fields[FamilyFields.Name].Received(1).Value = name;
            testRS.Fields[FamilyFields.Aeb].Received(1).Value = aeb;
            testRS.Fields[FamilyFields.Bem1].Received(1).Value = bem1;
            testRS.Received(1).Update();
        }

        [TestMethod()]
        public void Get_AebTest()
        {
            // Arrange
            testRS.NoMatch.Returns(false);
            testRS.EOF.Returns(false, true);
            (testRS.Fields[FamilyFields.Aeb] as IHasValue).Value.Returns(3);
            // Act
            var result = testClass.Get_Aeb(1);
            // Assert
            Assert.AreEqual(true, result);
            Assert.AreEqual(nameof(FamilyIndex.Fam), testRS.Index);
            testRS.Received(1).Seek("=", 1);
        }
    }
}