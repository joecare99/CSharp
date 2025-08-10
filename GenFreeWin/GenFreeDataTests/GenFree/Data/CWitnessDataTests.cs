﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using GenFree.Interfaces.DB;
using NSubstitute;
using GenFree.Interfaces.Data;
using BaseLib.Interfaces;
using BaseLib.Helper;

namespace GenFree.Data.Tests
{
    [TestClass()]
    public class CWitnessDataTests
    {
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
        private CWitnessData testClass;
        private IRecordset testRS;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.

        [TestInitialize]
        public void Init()
        {
            testRS = Substitute.For<IRecordset>();
            testRS.NoMatch.Returns(true);
            (testRS.Fields[WitnessFields.PerNr] as IHasValue).Value.Returns(1, 2, 3);
            (testRS.Fields[WitnessFields.Kennz] as IHasValue).Value.Returns(2, 3, 4);
            (testRS.Fields[WitnessFields.FamNr] as IHasValue).Value.Returns(3, 4, 5);
            (testRS.Fields[WitnessFields.Art] as IHasValue).Value.Returns(4, 5, 6);
            (testRS.Fields[WitnessFields.LfNr] as IHasValue).Value.Returns(5, 6, 7);
            testClass = new(testRS);
            testRS.ClearReceivedCalls();
        }

        [TestMethod()]
        public void CWitnessDataTest()
        {
            Assert.IsNotNull(testClass);
            Assert.IsInstanceOfType(testClass, typeof(CWitnessData));
            Assert.IsInstanceOfType(testClass, typeof(IWitnessData));
        }

        [TestMethod()]
        [DataRow((EWitnessProp)0, 4)]
        [DataRow((EWitnessProp)1, 2)]
        [DataRow((EWitnessProp)2, 3)]
        [DataRow((EWitnessProp)3, (EEventArt)5)]
        [DataRow((EWitnessProp)4, (short)6)]
        public void FillDataTest(EWitnessProp eProp, object oExp)
        {
            testClass.FillData(testRS);
            Assert.AreEqual(oExp, testClass.GetPropValue(eProp));
        }

        [TestMethod()]
        [DataRow(EWitnessProp.iPers, TypeCode.Int32)]
        [DataRow(EWitnessProp.iWKennz, TypeCode.Int32)]
        [DataRow(EWitnessProp.iLink, TypeCode.Int32)]
        [DataRow(EWitnessProp.eArt, TypeCode.Int16)]
        [DataRow(EWitnessProp.iLfNr, TypeCode.Int16)]
        public void GetPropTypeTest(EWitnessProp pAct, TypeCode eExp)
        {
            Assert.AreEqual(eExp, Type.GetTypeCode(testClass.GetPropType(pAct)));
        }

        [TestMethod()]
        [DataRow((EWitnessProp)(0 - 1), TypeCode.Int32)]
        [DataRow((EWitnessProp)5, TypeCode.Int32)]
        [DataRow((EWitnessProp)100, TypeCode.Int32)]
        public void GetPropTypeTest2(EWitnessProp pAct, TypeCode eExp)
        {
            Assert.ThrowsException<NotImplementedException>(() => testClass.GetPropType(pAct));
        }

        [TestMethod()]
        [DataRow(EWitnessProp.iPers, 1)]
        [DataRow(EWitnessProp.iWKennz, 2)]
        [DataRow(EWitnessProp.iLink, 3)]
        [DataRow(EWitnessProp.eArt, (EEventArt)4)]
        [DataRow(EWitnessProp.iLfNr, (short)5)]
        public void GetPropValueTest(EWitnessProp eExp, object oAct)
        {
            Assert.AreEqual(oAct, testClass.GetPropValue(eExp));
        }

        [TestMethod()]
        [DataRow((EWitnessProp)(0 - 1), TypeCode.Int32)]
        [DataRow((EWitnessProp)5, TypeCode.Int32)]
        [DataRow((EWitnessProp)100, TypeCode.Int32)]
        public void GetPropValueTest2(EWitnessProp eExp, object oAct)
        {
            Assert.ThrowsException<NotImplementedException>(() => testClass.GetPropValue(eExp));
        }

        [TestMethod()]
        [DataRow(EWitnessProp.iPers, 1)]
        [DataRow(EWitnessProp.iPers, 2)]
        [DataRow(EWitnessProp.iWKennz, 3)]
        [DataRow(EWitnessProp.iLink, 4)]
        [DataRow(EWitnessProp.eArt, (EEventArt)5)]
        [DataRow(EWitnessProp.iLfNr, (short)6)]
        public void SetPropValueTest(EWitnessProp eAct, object iVal)
        {
            testClass.SetPropValue(eAct, iVal);
            Assert.AreEqual(iVal, testClass.GetPropValue(eAct));
        }

        [TestMethod()]
        [DataRow((EWitnessProp)(0 - 1), TypeCode.Int32)]
        [DataRow((EWitnessProp)5, TypeCode.Int32)]
        [DataRow((EWitnessProp)100, TypeCode.Int32)]
        public void SetPropValueTest1(EWitnessProp eAct, object iVal)
        {
            Assert.ThrowsException<NotImplementedException>(() => testClass.SetPropValue(eAct, iVal));
        }


        [TestMethod()]
        [DataRow(EWitnessProp.iPers, 1)]
        [DataRow(EWitnessProp.iPers, 2)]
        [DataRow(EWitnessProp.iWKennz, 3)]
        [DataRow(EWitnessProp.iLink, 4)]
        [DataRow(EWitnessProp.eArt, (EEventArt)5)]
        [DataRow(EWitnessProp.iLfNr, (short)6)]
        public void SetDBValueTest(EWitnessProp eAct, object _)
        {
            testClass.SetDBValues(testRS, new[] { (Enum)eAct });
            _ = testRS.Received().Fields[eAct.ToString()];
        }

        [TestMethod()]
        [DataRow((EWitnessProp)(0 - 1), TypeCode.Int32)]
        [DataRow((EWitnessProp)17, TypeCode.Int32)]
        [DataRow((EWitnessProp)100, TypeCode.Int32)]
        public void SetDBValueTest1(EWitnessProp eAct, object _)
        {
            Assert.ThrowsException<NotImplementedException>(() => testClass.SetDBValues(testRS, new[] { (Enum)eAct }));
        }

        [TestMethod()]
        [DataRow(EWitnessProp.iPers, 2)]
        [DataRow(EWitnessProp.iWKennz, 3)]
        [DataRow(EWitnessProp.iLink, 4)]
        [DataRow(EWitnessProp.eArt, (EEventArt)5)]
        [DataRow(EWitnessProp.iLfNr, (short)6)]
        public void SetDBValueTest2(EWitnessProp eAct, object oVal)
        {
            testClass.SetPropValue(eAct, oVal);
            testClass.SetDBValues(testRS, null);
            _ = testRS.Received().Fields[eAct.AsEnum<WitnessFields>()];
        }

        [TestMethod()]
        [DataRow(false)]
        [DataRow(true)]
        public void DeleteTest(bool xAct)
        {
            testRS.NoMatch.Returns(xAct);
            testClass.Delete();
            Assert.AreEqual(nameof(WitnessIndex.Fampruef), testRS.Index);
            testRS.Received(xAct ? 0 : 1).Delete();
            testRS.Received(1).Seek("=", testClass.ID.Item1, testClass.ID.Item2, testClass.ID.Item3, testClass.ID.Item4, testClass.ID.Item5);
        }
    }
}