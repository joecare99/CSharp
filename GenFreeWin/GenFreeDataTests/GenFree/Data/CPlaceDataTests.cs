using Microsoft.VisualStudio.TestTools.UnitTesting;
using GenFree.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GenFree.Interfaces.DB;
using NSubstitute;
using GenFree.Interfaces;
using System.Reflection;
using GenFree.Helper;

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
            testRS.Fields[nameof(PlaceFields.OrtNr)].Value.Returns(1, 2, 3);
            testRS.Fields[nameof(PlaceFields.Ort)].Value.Returns(2, 3, 4);
            testRS.Fields[nameof(PlaceFields.Ortsteil)].Value.Returns(3, 4, 5);
            testRS.Fields[nameof(PlaceFields.Kreis)].Value.Returns(4, 5, 6);
            testRS.Fields[nameof(PlaceFields.Land)].Value.Returns(5, 6, 7);
            testRS.Fields[nameof(PlaceFields.Staat)].Value.Returns(6, 7, 8);
            testRS.Fields[nameof(PlaceFields.Staatk)].Value.Returns("Staatk");
            testRS.Fields[nameof(PlaceFields.PLZ)].Value.Returns("PLZ");
            testRS.Fields[nameof(PlaceFields.Terr)].Value.Returns("Terr");
            testRS.Fields[nameof(PlaceFields.Loc)].Value.Returns("Loc");
            testRS.Fields[nameof(PlaceFields.L)].Value.Returns("Lat");
            testRS.Fields[nameof(PlaceFields.B)].Value.Returns("Long");
            testRS.Fields[nameof(PlaceFields.Bem)].Value.Returns("Bem");
            testRS.Fields[nameof(PlaceFields.Zusatz)].Value.Returns("Zusatz");
            testRS.Fields[nameof(PlaceFields.GOV)].Value.Returns("GOV");
            testRS.Fields[nameof(PlaceFields.PolName)].Value.Returns("PolName");
            testRS.Fields[nameof(PlaceFields.g)].Value.Returns("g");
            testClass = new(testRS);
            CPlaceData.SetGetText(getTextFnc);
            testRS.ClearReceivedCalls();
        }

        private string getTextFnc(int arg)
        {
            return $"Text_{arg}";
        }

        [TestMethod()]
        public void SetTableTest()
        {
            var testTable = Substitute.For<IRecordset>();
            CPlaceData.SetTableGtr(() => testRS);
        }

        [TestMethod()]
        public void ResetTest()
        {
            CPlaceData.Reset();
            try
            {
                var testClass = new CPlaceData(null!);
            }
            catch
            {
            }
        }

        [DataTestMethod()]
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

        [DataTestMethod()]
        [DataRow(1, 3)]
        [DataRow(2, 4)]
        [DataRow(3, 5)]
        [DataRow(4, 6)]
        [DataRow(5, 7)]
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

        [DataTestMethod()]
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
        public void GetPropTypeTest(EPlaceProp pAct, TypeCode eExp)
        {
            Assert.AreEqual(eExp, Type.GetTypeCode(testClass.GetPropType(pAct)));
        }

        [DataTestMethod()]
        [DataRow((EPlaceProp)(0 - 1), TypeCode.Int32)]
        [DataRow((EPlaceProp)17, TypeCode.Int32)]
        [DataRow((EPlaceProp)100, TypeCode.Int32)]
        public void GetPropTypeTest2(EPlaceProp pAct, TypeCode eExp)
        {
            Assert.ThrowsException<NotImplementedException>(() => testClass.GetPropType(pAct));
        }

        [DataTestMethod()]
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
        public void GetPropValueTest(EPlaceProp eExp, object oAct)
        {
            Assert.AreEqual(oAct, testClass.GetPropValue(eExp));
        }

        [DataTestMethod()]
        [DataRow((EPlaceProp)(0 - 1), TypeCode.Int32)]
        [DataRow((EPlaceProp)17, TypeCode.Int32)]
        [DataRow((EPlaceProp)100, TypeCode.Int32)]
        public void GetPropValueTest2(EPlaceProp eExp, object oAct)
        {
            Assert.ThrowsException<NotImplementedException>(() => testClass.GetPropValue(eExp));
        }

        [TestMethod()]
        public void GetPropValueTest1()
        {
            Assert.AreEqual(1, testClass.GetPropValue<int>(EPlaceProp.ID));
        }

        [DataTestMethod()]
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

        [DataTestMethod()]
        [DataRow((EPlaceProp)(0 - 1), TypeCode.Int32)]
        [DataRow((EPlaceProp)17, TypeCode.Int32)]
        [DataRow((EPlaceProp)100, TypeCode.Int32)]
        public void SetPropValueTest1(EPlaceProp eAct, object iVal)
        {
            Assert.ThrowsException<NotImplementedException>(() => testClass.SetPropValue(eAct, iVal));
        }


        [DataTestMethod()]
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
            testClass.SetDBValue(testRS, new[] { $"{eAct}" });
            _ = testRS.Received().Fields[eAct.ToString()];
        }

        //[DataTestMethod()]
        //[DataRow((EPlaceProp)(0 - 1), TypeCode.Int32)]
        //[DataRow((EPlaceProp)17, TypeCode.Int32)]
        //[DataRow((EPlaceProp)100, TypeCode.Int32)]
        //public void SetDBValueTest1(EPlaceProp eAct, object _)
        //{
        //    Assert.ThrowsException<NotImplementedException>(() => testClass.SetDBValue(testRS, new[] { $"{eAct}" }));
        //}

        [DataTestMethod()]
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
            testClass.SetDBValue(testRS, null);
            _ = testRS.Received().Fields[eAct.ToString()];
        }

        [DataTestMethod()]
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