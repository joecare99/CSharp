using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using GenFree.Interfaces.DB;
using NSubstitute;
using GenFree.Interfaces.Data;
using BaseLib.Interfaces;

namespace GenFree.Data.Tests
{
    [TestClass()]
    public class CPlaceDataTests
    {
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
        private CPlaceData testClass;
        private IRecordset testRS;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.

        [TestInitialize]
        public void Init()
        {
            testRS = Substitute.For<IRecordset>();
            testRS.NoMatch.Returns(true);
            (testRS.Fields[PlaceFields.OrtNr] as IHasValue).Value.Returns(1, 2, 3);
            (testRS.Fields[PlaceFields.Ort] as IHasValue).Value.Returns(2, 3, 4);
            (testRS.Fields[PlaceFields.Ortsteil] as IHasValue).Value.Returns(3, 4, 5);
            (testRS.Fields[PlaceFields.Kreis] as IHasValue).Value.Returns(4, 5, 6);
            (testRS.Fields[PlaceFields.Land] as IHasValue).Value.Returns(5, 6, 7);
            (testRS.Fields[PlaceFields.Staat] as IHasValue).Value.Returns(6, 7, 8);
            (testRS.Fields[PlaceFields.Staatk] as IHasValue).Value.Returns("Staatk");
            (testRS.Fields[PlaceFields.PLZ] as IHasValue).Value.Returns("PLZ");
            (testRS.Fields[PlaceFields.Terr] as IHasValue).Value.Returns("Terr");
            (testRS.Fields[PlaceFields.Loc] as IHasValue).Value.Returns("Loc");
            (testRS.Fields[PlaceFields.L] as IHasValue).Value.Returns("Lat");
            (testRS.Fields[PlaceFields.B] as IHasValue).Value.Returns("Long");
            (testRS.Fields[PlaceFields.Bem] as IHasValue).Value.Returns("Bem");
            (testRS.Fields[PlaceFields.Zusatz] as IHasValue).Value.Returns("Zusatz");
            (testRS.Fields[PlaceFields.GOV] as IHasValue).Value.Returns("GOV");
            (testRS.Fields[PlaceFields.PolName] as IHasValue).Value.Returns("PolName");
            (testRS.Fields[PlaceFields.g] as IHasValue).Value.Returns("g");
            testClass = new(testRS);
            CPlaceData.SetGetText(getTextFnc);
            testRS.ClearReceivedCalls();
        }

        private string getTextFnc(int arg)
        {
            return $"Text_{arg}";
        }

        [TestMethod()]
        [DataRow("Text_2", PlaceFields.Ort)]
        [DataRow("Text_3", PlaceFields.Ortsteil)]
        [DataRow("Text_4", PlaceFields.Kreis)]
        [DataRow("Text_5", PlaceFields.Land)]
        [DataRow("Text_6", PlaceFields.Staat)]
        public void SetGetTextTest(string sExp, PlaceFields iAct)
        {
            Assert.AreEqual(sExp, iAct switch
            {
                PlaceFields.Ort => testClass.sOrt,
                PlaceFields.Ortsteil => testClass.sOrtsteil,
                PlaceFields.Kreis => testClass.sKreis,
                PlaceFields.Land => testClass.sLand,
                PlaceFields.Staat => testClass.sStaat
            });
        }

        [TestMethod()]
        public void CPlaceDataTest()
        {
            Assert.IsNotNull(testClass);
            Assert.IsInstanceOfType(testClass, typeof(CPlaceData));
            Assert.IsInstanceOfType(testClass, typeof(IPlaceData));
        }

        [TestMethod()]
        [DataRow((EPlaceProp)1, 3)]
        [DataRow((EPlaceProp)2, 4)]
        [DataRow((EPlaceProp)3, 5)]
        [DataRow((EPlaceProp)4, 6)]
        [DataRow((EPlaceProp)5, 7)]
        public void FillDataTest(EPlaceProp eProp, object oExp)
        {
            testClass.FillData(testRS);
            Assert.AreEqual(oExp, testClass.GetPropValue(eProp));
        }

        [TestMethod()]
        public void AddChangedPropTest()
        {
            testClass.AddChangedProp(EPlaceProp.ID);
            testClass.AddChangedProp(EPlaceProp.iOrt);
            testClass.AddChangedProp(EPlaceProp.iOrt);
            Assert.AreEqual(2, testClass.ChangedProps.Count);
        }

        [TestMethod()]
        public void ClearChangedPropsTest()
        {
            AddChangedPropTest();
            testClass.ClearChangedProps();
            Assert.AreEqual(0, testClass.ChangedProps.Count);
        }

        [TestMethod()]
        [DataRow(EPlaceProp.ID, TypeCode.Int32)]
        [DataRow(EPlaceProp.iOrt, TypeCode.Int32)]
        [DataRow(EPlaceProp.iOrtsteil, TypeCode.Int32)]
        [DataRow(EPlaceProp.iKreis, TypeCode.Int32)]
        [DataRow(EPlaceProp.iLand, TypeCode.Int32)]
        [DataRow(EPlaceProp.iStaat, TypeCode.Int32)]
        [DataRow(EPlaceProp.sStaatk, TypeCode.String)]
        [DataRow(EPlaceProp.sPLZ, TypeCode.String)]
        [DataRow(EPlaceProp.sTerr, TypeCode.String)]
        [DataRow(EPlaceProp.sLoc, TypeCode.String)]
        [DataRow(EPlaceProp.sL, TypeCode.String)]
        [DataRow(EPlaceProp.sB, TypeCode.String)]
        [DataRow(EPlaceProp.sBem, TypeCode.String)]
        [DataRow(EPlaceProp.sZusatz, TypeCode.String)]
        [DataRow(EPlaceProp.sGOV, TypeCode.String)]
        [DataRow(EPlaceProp.sPolName, TypeCode.String)]
        [DataRow(EPlaceProp.ig, TypeCode.Int32)]
        [DataRow(EPlaceProp.sOrt, TypeCode.String)]
        [DataRow(EPlaceProp.sOrtsteil, TypeCode.String)]
        [DataRow(EPlaceProp.sKreis, TypeCode.String)]
        [DataRow(EPlaceProp.sLand, TypeCode.String)]
        [DataRow(EPlaceProp.sStaat, TypeCode.String)]
        public void GetPropTypeTest(EPlaceProp pAct, TypeCode eExp)
        {
            Assert.AreEqual(eExp, Type.GetTypeCode(testClass.GetPropType(pAct)));
        }

        [TestMethod()]
        [DataRow((EPlaceProp)(0 - 1), TypeCode.Int32)]
        [DataRow((EPlaceProp)22, TypeCode.Int32)]
        [DataRow((EPlaceProp)100, TypeCode.Int32)]
        public void GetPropTypeTest2(EPlaceProp pAct, TypeCode eExp)
        {
            Assert.ThrowsExactly<NotImplementedException>(() => testClass.GetPropType(pAct));
        }

        [TestMethod()]
        [DataRow(EPlaceProp.ID, 1)]
        [DataRow(EPlaceProp.iOrt, 2)]
        [DataRow(EPlaceProp.iOrtsteil, 3)]
        [DataRow(EPlaceProp.iKreis, 4)]
        [DataRow(EPlaceProp.iLand, 5)]
        [DataRow(EPlaceProp.iStaat, 6)]
        [DataRow(EPlaceProp.sStaatk, "Staatk")]
        [DataRow(EPlaceProp.sPLZ, "PLZ")]
        [DataRow(EPlaceProp.sTerr, "Terr")]
        [DataRow(EPlaceProp.sLoc, "Loc")]
        [DataRow(EPlaceProp.sL, "Lat")]
        [DataRow(EPlaceProp.sB, "Long")]
        [DataRow(EPlaceProp.sBem, "Bem")]
        [DataRow(EPlaceProp.sZusatz, "Zusatz")]
        [DataRow(EPlaceProp.sGOV, "GOV")]
        [DataRow(EPlaceProp.sPolName, "PolName")]
        [DataRow(EPlaceProp.ig, 0)]
        [DataRow(EPlaceProp.sOrt, "Text_2")]
        [DataRow(EPlaceProp.sOrtsteil, "Text_3")]
        [DataRow(EPlaceProp.sKreis, "Text_4")]
        [DataRow(EPlaceProp.sLand, "Text_5")]
        [DataRow(EPlaceProp.sStaat, "Text_6")]
        public void GetPropValueTest(EPlaceProp eExp, object oAct)
        {
            Assert.AreEqual(oAct, testClass.GetPropValue(eExp));
        }

        [TestMethod()]
        [DataRow((EPlaceProp)(0 - 1), TypeCode.Int32)]
        [DataRow((EPlaceProp)22, TypeCode.Int32)]
        [DataRow((EPlaceProp)100, TypeCode.Int32)]
        public void GetPropValueTest2(EPlaceProp eExp, object oAct)
        {
            Assert.ThrowsExactly<NotImplementedException>(() => testClass.GetPropValue(eExp));
        }

        [TestMethod()]
        public void GetPropValueTest1()
        {
            Assert.AreEqual(1, testClass.GetPropValue<int>(EPlaceProp.ID));
        }

        [TestMethod()]
        [DataRow(EPlaceProp.ID, 1)]
        [DataRow(EPlaceProp.ID, 2)]
        [DataRow(EPlaceProp.iOrt, 3)]
        [DataRow(EPlaceProp.iOrtsteil, 4)]
        [DataRow(EPlaceProp.iKreis, 5)]
        [DataRow(EPlaceProp.iLand, 6)]
        [DataRow(EPlaceProp.iStaat, 7)]
        [DataRow(EPlaceProp.sStaatk, "8")]
        [DataRow(EPlaceProp.sPLZ, "9")]
        [DataRow(EPlaceProp.sTerr, "10")]
        [DataRow(EPlaceProp.sLoc, "11")]
        [DataRow(EPlaceProp.sL, "12")]
        [DataRow(EPlaceProp.sB, "13")]
        [DataRow(EPlaceProp.sBem, "14")]
        [DataRow(EPlaceProp.sZusatz, "15")]
        [DataRow(EPlaceProp.sGOV, "16")]
        [DataRow(EPlaceProp.sPolName, "17")]
        [DataRow(EPlaceProp.ig, 1)]
        public void SetPropValueTest(EPlaceProp eAct, object iVal)
        {
            testClass.SetPropValue(eAct, iVal);
            Assert.AreEqual(iVal, testClass.GetPropValue(eAct));
        }

        [TestMethod()]
        [DataRow((EPlaceProp)(0 - 1), TypeCode.Int32)]
        [DataRow((EPlaceProp)17, TypeCode.Int32)]
        [DataRow((EPlaceProp)100, TypeCode.Int32)]
        public void SetPropValueTest1(EPlaceProp eAct, object iVal)
        {
            Assert.ThrowsExactly<NotImplementedException>(() => testClass.SetPropValue(eAct, iVal));
        }


        [TestMethod()]
        [DataRow(EPlaceProp.ID, 2)]
        [DataRow(EPlaceProp.iOrt, 3)]
        [DataRow(EPlaceProp.iOrtsteil, 4)]
        [DataRow(EPlaceProp.iKreis, 5)]
        [DataRow(EPlaceProp.iLand, 6)]
        [DataRow(EPlaceProp.iStaat, 7)]
        [DataRow(EPlaceProp.sStaatk, "8")]
        [DataRow(EPlaceProp.sPLZ, "9")]
        [DataRow(EPlaceProp.sTerr, "10")]
        [DataRow(EPlaceProp.sLoc, "11")]
        [DataRow(EPlaceProp.sL, "12")]
        [DataRow(EPlaceProp.sB, "13")]
        [DataRow(EPlaceProp.sBem, "14")]
        [DataRow(EPlaceProp.sZusatz, "15")]
        [DataRow(EPlaceProp.sGOV, "16")]
        [DataRow(EPlaceProp.sPolName, "17")]
        [DataRow(EPlaceProp.ig, 1)]
        public void SetDBValueTest(EPlaceProp eAct, object _)
        {
            testClass.SetDBValues(testRS, new[] { (Enum)eAct });
            _ = testRS.Received().Fields[eAct.ToString()];
        }

        [TestMethod()]
        [DataRow((EPlaceProp)(0 - 1), TypeCode.Int32)]
        [DataRow((EPlaceProp)17, TypeCode.Int32)]
        [DataRow((EPlaceProp)100, TypeCode.Int32)]
        public void SetDBValueTest1(EPlaceProp eAct, object _)
        {
            Assert.ThrowsExactly<NotImplementedException>(() => testClass.SetDBValues(testRS, new[] { (Enum)eAct }));
        }

        [TestMethod()]
        [DataRow(EPlaceProp.ID, 2)]
        [DataRow(EPlaceProp.iOrt, 3)]
        [DataRow(EPlaceProp.iOrtsteil, 4)]
        [DataRow(EPlaceProp.iKreis, 5)]
        [DataRow(EPlaceProp.iLand, 6)]
        [DataRow(EPlaceProp.iStaat, 7)]
        [DataRow(EPlaceProp.sStaatk, "8")]
        [DataRow(EPlaceProp.sPLZ, "9")]
        [DataRow(EPlaceProp.sTerr, "10")]
        [DataRow(EPlaceProp.sLoc, "11")]
        [DataRow(EPlaceProp.sL, "12")]
        [DataRow(EPlaceProp.sB, "13")]
        [DataRow(EPlaceProp.sBem, "14")]
        [DataRow(EPlaceProp.sZusatz, "15")]
        [DataRow(EPlaceProp.sGOV, "16")]
        [DataRow(EPlaceProp.sPolName, "17")]
        [DataRow(EPlaceProp.ig, 1)]
        public void SetDBValueTest2(EPlaceProp eAct, object oVal)
        {
            testClass.SetPropValue(eAct, oVal);
            testClass.SetDBValues(testRS, null);
            _ = testRS.Received().Fields[eAct.ToString()];
        }

        [TestMethod()]
        [DataRow(false)]
        [DataRow(true)]
        public void DeleteTest(bool xAct)
        {
            testRS.NoMatch.Returns(xAct);
            testClass.Delete();
            Assert.AreEqual("OrtNr", testRS.Index);
            testRS.Received(xAct ? 0 : 1).Delete();
            testRS.Received(1).Seek("=", testClass.ID);
        }
    }
}