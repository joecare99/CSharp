using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using GenFree.Interfaces.DB;
using NSubstitute;
using GenFree.Interfaces;
using GenFree.Helper;
using static BaseLib.Helper.TestHelper;

namespace GenFree.Data.Tests
{
    [TestClass()]
    public class COFBDataTests
    {
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
        private COFBData testClass;
        private IRecordset testRS;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
        private string getTextFnc(int arg) => $"Text_{arg}";

        [TestInitialize]
        public void Init()
        {
            testRS = Substitute.For<IRecordset>();
            testRS.NoMatch.Returns(true);
            testRS.Fields[nameof(OFBFields.PerNr)].Value.Returns(1, 2, 3);
            testRS.Fields[nameof(OFBFields.Kennz)].Value.Returns("AA", "BB", "CC");
            testRS.Fields[nameof(OFBFields.TextNr)].Value.Returns(3, 4, 5);
            testClass = new(testRS);
            COFBData.SetGetText(getTextFnc);
            testRS.ClearReceivedCalls();
        }

        [DataTestMethod()]
        [DataRow("Text_3", EOFBProps.sText)]
        public void SetGetTextTest(string sExp, EOFBProps iAct)
        {
            Assert.AreEqual(sExp, iAct switch
            {
                EOFBProps.sText => testClass.sText,
            });
        }

        [TestMethod()]
        public void COFBDataTest()
        {
            Assert.IsNotNull(testClass);
            Assert.IsInstanceOfType(testClass, typeof(COFBData));
            Assert.IsInstanceOfType(testClass, typeof(IOFBData));
        }

        //[TestMethod()]
        //public void SetTableGtrTest()
        //{
        //    Assert.Fail();
        //}

        [DataTestMethod()]
        [DataRow(0, 2)]
        [DataRow(1, "BB")]
        [DataRow(2, 4)]
        [DataRow(3, "Text_4")]
        public void FillDataTest(EOFBProps eProp, object oExp)
        {
            testClass.FillData(testRS);
            Assert.AreEqual(oExp, testClass.GetPropValue(eProp));
        }

        [TestMethod()]
        public void IDTest()
        {
            Assert.AreEqual(1, testClass.ID.Item1);
            Assert.AreEqual("AA", testClass.ID.Item2);
            Assert.AreEqual(3, testClass.ID.Item3);
        }

        [DataTestMethod()]
        [DataRow(EOFBProps.iPerNr, TypeCode.Int32)]
        [DataRow(EOFBProps.sKennz, TypeCode.String)]
        [DataRow(EOFBProps.iTextNr, TypeCode.Int32)]
        [DataRow(EOFBProps.sText, TypeCode.String)]
        public void GetPropTypeTest(EOFBProps pAct, TypeCode eExp)
        {
            Assert.AreEqual(eExp, Type.GetTypeCode(testClass.GetPropType(pAct)));
        }
        [DataTestMethod()]
        [DataRow((EOFBProps)(0 - 1), TypeCode.Int32)]
        [DataRow((EOFBProps)4, TypeCode.Int32)]
        [DataRow((EOFBProps)100, TypeCode.Int32)]
        public void GetPropTypeTest2(EOFBProps pAct, TypeCode eExp)
        {
            Assert.ThrowsException<NotImplementedException>(() => testClass.GetPropType(pAct));
        }

        [DataTestMethod()]
        [DataRow(EOFBProps.iPerNr, 1)]
        [DataRow(EOFBProps.sKennz, "AA")]
        [DataRow(EOFBProps.iTextNr, 3)]
        [DataRow(EOFBProps.sText, "Text_3")]
        public void GetPropValueTest(EOFBProps eAct, object oExp)
        {
            if (oExp is int[] aiAct && aiAct.Length == 3 && aiAct[1] <= 12 && aiAct[2] <= 31)
                oExp = new DateTime(aiAct[0], aiAct[1], aiAct[2]);
            else if (oExp is string s && s.Length == 36 && Guid.TryParse(s, out var g))
                oExp = g;
            else if (oExp is object[] aO && aO.Length == 2 && aO[0] is int iX && aO[1] is string sD)
                oExp = new ListItem<int>(sD, iX);

            if (oExp is ListItem<int> l)
                Assert.AreEqual(l.ItemString, ((string[])testClass.GetPropValue(eAct))[l.ItemData]);
            else if (oExp is not string[] aS)
                Assert.AreEqual(oExp, testClass.GetPropValue(eAct));
            else
                AssertAreEqual(aS, (string[])testClass.GetPropValue(eAct));
        }

        [DataTestMethod()]
        [DataRow((EOFBProps)(0 - 1), TypeCode.Int32)]
        [DataRow((EOFBProps)4, TypeCode.Int32)]
        [DataRow((EOFBProps)100, TypeCode.Int32)]
        public void GetPropValueTest2(EOFBProps eExp, object oAct)
        {
            Assert.ThrowsException<NotImplementedException>(() => testClass.GetPropValue(eExp));
        }


        [DataTestMethod()]
        [DataRow(EOFBProps.iPerNr, 1)]
        [DataRow(EOFBProps.iPerNr, 2)]
        [DataRow(EOFBProps.sKennz, "AA_")]
        [DataRow(EOFBProps.iTextNr, 4)]

        public void SetDBValueTest(EOFBProps eAct, object _)
        {
            testClass.SetDBValue(testRS, new[] { (Enum)eAct });
            _ = testRS.Received().Fields[eAct.ToString()];
        }

        [TestMethod()]
        [DataRow((EOFBProps)(0 - 1), TypeCode.Int32)]
        [DataRow((EOFBProps)3, TypeCode.Int32)]
        [DataRow((EOFBProps)100, TypeCode.Int32)]
        public void SetDBValueTest1(EOFBProps eAct, object _)
        {
            Assert.ThrowsException<NotImplementedException>(() => testClass.SetDBValue(testRS, new[] { (Enum)eAct }));
        }

        [DataTestMethod()]
        [DataRow(EOFBProps.iPerNr, 2)]
        [DataRow(EOFBProps.sKennz, "AA_")]
        [DataRow(EOFBProps.iTextNr, 4)]
        public void SetDBValueTest2(EOFBProps eAct, object oExp)
        {
            SetPropValueTest(eAct, oExp);
            testClass.SetDBValue(testRS, null);
            _ = testRS.Received().Fields[eAct.ToString()];
        }

        [DataTestMethod()]
        [DataRow(EOFBProps.iPerNr, 1)]
        [DataRow(EOFBProps.iPerNr, 2)]
        [DataRow(EOFBProps.sKennz, "AA_")]
        [DataRow(EOFBProps.iTextNr, 4)]
        public void SetPropValueTest(EOFBProps eAct, object oExp)
        {
            if (oExp is int[] aiAct && aiAct.Length == 3 && aiAct[1] <= 12 && aiAct[2] <= 31)
                oExp = new DateTime(aiAct[0], aiAct[1], aiAct[2]);
            else if (oExp is string s && s.Length == 36 && Guid.TryParse(s, out var g))
                oExp = g;
            else if (oExp is object[] aO && aO.Length == 2 && aO[0] is int iX && aO[1] is string sD)
                oExp = new ListItem<int>(sD, iX);

            testClass.SetPropValue(eAct, oExp);
            if (oExp is ListItem<int> l)
                Assert.AreEqual(l.ItemString, ((string[])testClass.GetPropValue(eAct))[l.ItemData]);
            else if (oExp is not string[] aS)
                Assert.AreEqual(oExp, testClass.GetPropValue(eAct));
            else
                AssertAreEqual(aS, (string[])testClass.GetPropValue(eAct));
        }

        [DataTestMethod()]
        [DataRow((EOFBProps)(0 - 1), TypeCode.Int32)]
        [DataRow((EOFBProps)3, TypeCode.Int32)]
        [DataRow((EOFBProps)100, TypeCode.Int32)]
        public void SetPropValueTest2(EOFBProps eExp, object oAct)
        {
            Assert.ThrowsException<NotImplementedException>(() => testClass.SetPropValue(eExp, oAct));
        }

        [DataTestMethod()]
        [DataRow(false)]
        [DataRow(true)]
        public void DeleteTest(bool xAct)
        {
            testRS.NoMatch.Returns(xAct);
            testClass.Delete();
            Assert.AreEqual(OFBIndex.Indn.AsFld(), testRS.Index);
            testRS.Received(xAct ? 0 : 1).Delete();
            testRS.Received(1).Seek("=", testClass.ID.Item1, testClass.ID.Item2, testClass.ID.Item3);
        }
    }
}