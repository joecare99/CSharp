using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using GenFree.Interfaces.DB;
using NSubstitute;
using GenFree.Helper;
using GenFree.Interfaces;

namespace GenFree.Data.Tests
{
    [TestClass()]
    public class CSourceLinkDataTests
    {
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
        private CSourceLinkData testClass;
        private IRecordset testRS;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.

        [TestInitialize]
        public void Init()
        {
            testRS = Substitute.For<IRecordset>();
            testRS.NoMatch.Returns(true);
            testRS.Fields[SourceLinkFields.Art.AsFld()].Value.Returns(1, 2, 3);
            testRS.Fields[SourceLinkFields._1.AsFld()].Value.Returns(2, 3, 4);
            testRS.Fields[SourceLinkFields._2.AsFld()].Value.Returns(3, 4, 5);
            testRS.Fields[SourceLinkFields.LfNr.AsFld()].Value.Returns(4, 5, 6);
            testRS.Fields[SourceLinkFields._3.AsFld()].Value.Returns(5, 6, 7);
            testRS.Fields[SourceLinkFields._4.AsFld()].Value.Returns("Field3");
            testRS.Fields[SourceLinkFields.Aus.AsFld()].Value.Returns("Aus");
            testRS.Fields[SourceLinkFields.Orig.AsFld()].Value.Returns("Orig");
            testRS.Fields[SourceLinkFields.Kom.AsFld()].Value.Returns("Kom");
            testClass = new(testRS);
            testRS.ClearReceivedCalls();
        }
        [TestMethod()]
        public void CSourceLinkDataTest()
        {
            Assert.IsNotNull(testClass);
            Assert.IsInstanceOfType(testClass, typeof(CSourceLinkData));
            Assert.IsInstanceOfType(testClass, typeof(ISourceLinkData));
        }

        [DataTestMethod()]
        [DataRow(1, 3)]
        [DataRow(2, 4)]
        [DataRow(3, 6)]
        [DataRow(4, "Field3")]
        [DataRow(5, 5)]
        public void FillDataTest(ESourceLinkProp eProp, object oExp)
        {
            testClass.FillData(testRS);
            Assert.AreEqual(oExp, testClass.GetPropValue(eProp));
        }

        [DataTestMethod()]
        [DataRow(ESourceLinkProp.eArt,      TypeCode.Int32)]
        [DataRow(ESourceLinkProp.iLinkType, TypeCode.Int32)]
        [DataRow(ESourceLinkProp.iPersNr,   TypeCode.Int32)]
        [DataRow(ESourceLinkProp.iQuNr,     TypeCode.Int32)]
        [DataRow(ESourceLinkProp.sField3,   TypeCode.String)]
        [DataRow(ESourceLinkProp.iLfdNr,    TypeCode.Int32)]
        [DataRow(ESourceLinkProp.sAus,      TypeCode.String)]
        [DataRow(ESourceLinkProp.sOrig,     TypeCode.String)]
        [DataRow(ESourceLinkProp.sKom,      TypeCode.String)]
        public void GetPropTypeTest(ESourceLinkProp pAct, TypeCode eExp)
        {
            Assert.AreEqual(eExp, Type.GetTypeCode(testClass.GetPropType(pAct)));
        }

        [DataTestMethod()]
        [DataRow((ESourceLinkProp)(0 - 1), TypeCode.Int32)]
        [DataRow((ESourceLinkProp)9, TypeCode.Int32)]
        [DataRow((ESourceLinkProp)100, TypeCode.Int32)]
        public void GetPropTypeTest2(ESourceLinkProp pAct, TypeCode eExp)
        {
            Assert.ThrowsException<NotImplementedException>(() => testClass.GetPropType(pAct));
        }

        [DataTestMethod()]
        [DataRow(ESourceLinkProp.eArt, (EEventArt)1)]
        [DataRow(ESourceLinkProp.iLinkType, 2)]
        [DataRow(ESourceLinkProp.iPersNr, 3)]
        [DataRow(ESourceLinkProp.iQuNr, 5)]
        [DataRow(ESourceLinkProp.sField3, "Field3")]
        [DataRow(ESourceLinkProp.iLfdNr, 4)]
        [DataRow(ESourceLinkProp.sAus, "Aus")]
        [DataRow(ESourceLinkProp.sOrig, "Orig")]
        [DataRow(ESourceLinkProp.sKom, "Kom")]
        public void GetPropValueTest(ESourceLinkProp eExp, object oAct)
        {
            Assert.AreEqual(oAct, testClass.GetPropValue(eExp));
        }

        [DataTestMethod()]
        [DataRow((ESourceLinkProp)(0 - 1), TypeCode.Int32)]
        [DataRow((ESourceLinkProp)9, TypeCode.Int32)]
        [DataRow((ESourceLinkProp)100, TypeCode.Int32)]
        public void GetPropValueTest2(ESourceLinkProp eExp, object oAct)
        {
            Assert.ThrowsException<NotImplementedException>(() => testClass.GetPropValue(eExp));
        }

        [TestMethod()]
        public void GetPropValueTest1()
        {
            Assert.AreEqual(3, testClass.GetPropValue<int>(ESourceLinkProp.iPersNr));
        }

        [DataTestMethod()]
        [DataRow(ESourceLinkProp.eArt, (EEventArt)1)]
        [DataRow(ESourceLinkProp.eArt, (EEventArt)2)]
        [DataRow(ESourceLinkProp.iLinkType, 3)]
        [DataRow(ESourceLinkProp.iPersNr, 4)]
        [DataRow(ESourceLinkProp.iQuNr, 6)]
        [DataRow(ESourceLinkProp.sField3, "Field2")]
        [DataRow(ESourceLinkProp.iLfdNr, 5)]
        [DataRow(ESourceLinkProp.sAus, "Aus_")]
        [DataRow(ESourceLinkProp.sOrig, "Orig_")]
        [DataRow(ESourceLinkProp.sKom, "Kom_")]
        public void SetPropValueTest(ESourceLinkProp eAct, object iVal)
        {
            testClass.SetPropValue(eAct, iVal);
            Assert.AreEqual(iVal, testClass.GetPropValue(eAct));
        }

        [DataTestMethod()]
        [DataRow((ESourceLinkProp)(0 - 1), TypeCode.Int32)]
        [DataRow((ESourceLinkProp)9, TypeCode.Int32)]
        [DataRow((ESourceLinkProp)100, TypeCode.Int32)]
        public void SetPropValueTest1(ESourceLinkProp eAct, object iVal)
        {
            Assert.ThrowsException<NotImplementedException>(() => testClass.SetPropValue(eAct, iVal));
        }


        [DataTestMethod()]
        [DataRow(ESourceLinkProp.eArt, (EEventArt)1)]
        [DataRow(ESourceLinkProp.eArt, (EEventArt)2)]
        [DataRow(ESourceLinkProp.iLinkType, 3)]
        [DataRow(ESourceLinkProp.iPersNr, 4)]
        [DataRow(ESourceLinkProp.iQuNr, 6)]
        [DataRow(ESourceLinkProp.sField3, "Field2")]
        [DataRow(ESourceLinkProp.iLfdNr, 5)]
        [DataRow(ESourceLinkProp.sAus, "Aus_")]
        [DataRow(ESourceLinkProp.sOrig, "Orig_")]
        [DataRow(ESourceLinkProp.sKom, "Kom_")]
        public void SetDBValueTest(ESourceLinkProp eAct, object _)
        {
            testClass.SetDBValue(testRS, new[] { (Enum)eAct });
            _ = testRS.Received().Fields[eAct.ToString()];
        }

        [DataTestMethod()]
        [DataRow((ESourceLinkProp)(0 - 1), TypeCode.Int32)]
        [DataRow((ESourceLinkProp)17, TypeCode.Int32)]
        [DataRow((ESourceLinkProp)100, TypeCode.Int32)]
        public void SetDBValueTest1(ESourceLinkProp eAct, object _)
        {
            Assert.ThrowsException<NotImplementedException>(() => testClass.SetDBValue(testRS, new[] { (Enum)eAct }));
        }

        [DataTestMethod()]
        [DataRow(ESourceLinkProp.eArt, (EEventArt)2)]
        [DataRow(ESourceLinkProp.iLinkType, 3)]
        [DataRow(ESourceLinkProp.iPersNr, 4)]
        [DataRow(ESourceLinkProp.iQuNr, 6)]
        [DataRow(ESourceLinkProp.sField3, "Field2")]
        [DataRow(ESourceLinkProp.iLfdNr, 5)]
        [DataRow(ESourceLinkProp.sAus, "Aus_")]
        [DataRow(ESourceLinkProp.sOrig, "Orig_")]
        [DataRow(ESourceLinkProp.sKom, "Kom_")]
        public void SetDBValueTest2(ESourceLinkProp eAct, object oVal)
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
            Assert.AreEqual(nameof(SourceLinkIndex.Tab22), testRS.Index);
            testRS.Received(xAct ? 0 : 1).Delete();
            testRS.Received(1).Seek("=", testClass.ID.Item1, testClass.ID.Item2, testClass.ID.Item3);
        }

    }
}