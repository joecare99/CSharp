using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using GenFree.Interfaces.DB;
using NSubstitute;
using GenFree.Interfaces.Data;
using BaseLib.Interfaces;

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
            (testRS.Fields[SourceLinkFields._1] as IHasValue).Value.Returns(2, 3, 4);
            (testRS.Fields[SourceLinkFields._2] as IHasValue).Value.Returns(3, 4, 5);
            (testRS.Fields[SourceLinkFields._3] as IHasValue).Value.Returns(5, 6, 7);
            (testRS.Fields[SourceLinkFields._4] as IHasValue).Value.Returns("Field3");
            (testRS.Fields[SourceLinkFields.Aus] as IHasValue).Value.Returns("Aus");
            (testRS.Fields[SourceLinkFields.Orig] as IHasValue).Value.Returns("Orig");
            (testRS.Fields[SourceLinkFields.Kom] as IHasValue).Value.Returns("Kom");
            (testRS.Fields[SourceLinkFields.Art] as IHasValue).Value.Returns(101, 102, 103);
            (testRS.Fields[SourceLinkFields.LfNr] as IHasValue).Value.Returns(4, 5, 6);
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

        [TestMethod()]
        [DataRow(ESourceLinkProp.iLinkType, (short)3)]
        [DataRow(ESourceLinkProp.iPerFamNr, 4)]
        [DataRow(ESourceLinkProp.iQuNr, 6)]
        [DataRow(ESourceLinkProp.sEntry, "Field3")]
        [DataRow(ESourceLinkProp.sPage, "Aus")]
        [DataRow(ESourceLinkProp.sOriginalText, "Orig")]
        [DataRow(ESourceLinkProp.sComment, "Kom")]
        [DataRow(ESourceLinkProp.eArt, EEventArt.eA_Baptism)]
        [DataRow(ESourceLinkProp.iLfdNr, (short)5)]
        public void FillDataTest(ESourceLinkProp eProp, object oExp)
        {
            testClass.FillData(testRS);
            Assert.AreEqual(oExp, testClass.GetPropValue(eProp));
        }

        [TestMethod()]
        [DataRow(ESourceLinkProp.eArt,      TypeCode.Int16)]
        [DataRow(ESourceLinkProp.iLinkType, TypeCode.Int16)]
        [DataRow(ESourceLinkProp.iPerFamNr,   TypeCode.Int32)]
        [DataRow(ESourceLinkProp.iQuNr,     TypeCode.Int32)]
        [DataRow(ESourceLinkProp.sEntry,   TypeCode.String)]
        [DataRow(ESourceLinkProp.iLfdNr,    TypeCode.Int16)]
        [DataRow(ESourceLinkProp.sPage,      TypeCode.String)]
        [DataRow(ESourceLinkProp.sOriginalText,     TypeCode.String)]
        [DataRow(ESourceLinkProp.sComment,      TypeCode.String)]
        public void GetPropTypeTest(ESourceLinkProp pAct, TypeCode eExp)
        {
            Assert.AreEqual(eExp, Type.GetTypeCode(testClass.GetPropType(pAct)));
        }

        [TestMethod()]
        [DataRow((ESourceLinkProp)(0 - 1), TypeCode.Int32)]
        [DataRow((ESourceLinkProp)9, TypeCode.Int32)]
        [DataRow((ESourceLinkProp)100, TypeCode.Int32)]
        public void GetPropTypeTest2(ESourceLinkProp pAct, TypeCode eExp)
        {
            Assert.ThrowsException<NotImplementedException>(() => testClass.GetPropType(pAct));
        }

        [TestMethod()]
        [DataRow(ESourceLinkProp.eArt, EEventArt.eA_Birth)]
        [DataRow(ESourceLinkProp.iLinkType, (short)2)]
        [DataRow(ESourceLinkProp.iPerFamNr, 3)]
        [DataRow(ESourceLinkProp.iQuNr, 5)]
        [DataRow(ESourceLinkProp.sEntry, "Field3")]
        [DataRow(ESourceLinkProp.iLfdNr, (short)4)]
        [DataRow(ESourceLinkProp.sPage, "Aus")]
        [DataRow(ESourceLinkProp.sOriginalText, "Orig")]
        [DataRow(ESourceLinkProp.sComment, "Kom")]
        public void GetPropValueTest(ESourceLinkProp eExp, object oAct)
        {
            Assert.AreEqual(oAct, testClass.GetPropValue(eExp));
        }

        [TestMethod()]
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
            Assert.AreEqual(3, testClass.GetPropValue<int>(ESourceLinkProp.iPerFamNr));
        }

        [TestMethod()]
        [DataRow(ESourceLinkProp.eArt, (EEventArt)101)]
        [DataRow(ESourceLinkProp.eArt, (EEventArt)102)]
        [DataRow(ESourceLinkProp.iLinkType, (short)3)]
        [DataRow(ESourceLinkProp.iPerFamNr, 4)]
        [DataRow(ESourceLinkProp.iQuNr, 6)]
        [DataRow(ESourceLinkProp.sEntry, "Field2")]
        [DataRow(ESourceLinkProp.iLfdNr, (short)5)]
        [DataRow(ESourceLinkProp.sPage, "Aus_")]
        [DataRow(ESourceLinkProp.sOriginalText, "Orig_")]
        [DataRow(ESourceLinkProp.sComment, "Kom_")]
        public void SetPropValueTest(ESourceLinkProp eAct, object iVal)
        {
            testClass.SetPropValue(eAct, iVal);
            Assert.AreEqual(iVal, testClass.GetPropValue(eAct));
        }

        [TestMethod()]
        [DataRow((ESourceLinkProp)(0 - 1), TypeCode.Int32)]
        [DataRow((ESourceLinkProp)9, TypeCode.Int32)]
        [DataRow((ESourceLinkProp)100, TypeCode.Int32)]
        public void SetPropValueTest1(ESourceLinkProp eAct, object iVal)
        {
            Assert.ThrowsException<NotImplementedException>(() => testClass.SetPropValue(eAct, iVal));
        }


        [TestMethod()]
        [DataRow(ESourceLinkProp.eArt, (EEventArt)101)]
        [DataRow(ESourceLinkProp.eArt, (EEventArt)102)]
        [DataRow(ESourceLinkProp.iLinkType, (short)3)]
        [DataRow(ESourceLinkProp.iPerFamNr, 4)]
        [DataRow(ESourceLinkProp.iQuNr, 6)]
        [DataRow(ESourceLinkProp.sEntry, "Field2")]
        [DataRow(ESourceLinkProp.iLfdNr, (short)5)]
        [DataRow(ESourceLinkProp.sPage, "Aus_")]
        [DataRow(ESourceLinkProp.sOriginalText, "Orig_")]
        [DataRow(ESourceLinkProp.sComment, "Kom_")]
        public void SetDBValueTest(ESourceLinkProp eAct, object _)
        {
            testClass.SetDBValues(testRS, new[] { (Enum)eAct });
            _ = testRS.Received().Fields[eAct.ToString()];
        }

        [TestMethod()]
        [DataRow((ESourceLinkProp)(0 - 1), TypeCode.Int32)]
        [DataRow((ESourceLinkProp)17, TypeCode.Int32)]
        [DataRow((ESourceLinkProp)100, TypeCode.Int32)]
        public void SetDBValueTest1(ESourceLinkProp eAct, object _)
        {
            Assert.ThrowsException<NotImplementedException>(() => testClass.SetDBValues(testRS, new[] { (Enum)eAct }));
        }

        [TestMethod()]
        [DataRow(ESourceLinkProp.eArt, (EEventArt)102)]
        [DataRow(ESourceLinkProp.iLinkType, (short)3)]
        [DataRow(ESourceLinkProp.iPerFamNr, 4)]
        [DataRow(ESourceLinkProp.iQuNr, 6)]
        [DataRow(ESourceLinkProp.sEntry, "Field2")]
        [DataRow(ESourceLinkProp.iLfdNr, (short)5)]
        [DataRow(ESourceLinkProp.sPage, "Aus_")]
        [DataRow(ESourceLinkProp.sOriginalText, "Orig_")]
        [DataRow(ESourceLinkProp.sComment, "Kom_")]
        public void SetDBValueTest2(ESourceLinkProp eAct, object oVal)
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
            Assert.AreEqual(nameof(SourceLinkIndex.Tab22), testRS.Index);
            testRS.Received(xAct ? 0 : 1).Delete();
            testRS.Received(1).Seek("=", testClass.ID.Item1,  (int)testClass.ID.Item3, testClass.ID.Item2, testClass.ID.Item4);
        }

    }
}