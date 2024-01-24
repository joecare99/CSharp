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
using NSubstitute.Core;

namespace GenFree.Data.Tests
{
    [TestClass()]
    public class CLinkDataTests
    {
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
        private CLinkData testClass;
        private IRecordset testRS;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.

        [TestInitialize]
        public void Init()
        {
            testRS = Substitute.For<IRecordset>();
            testRS.NoMatch.Returns(true);
            testRS.Fields[nameof(ILinkData.LinkFields.Kennz)].Value.Returns(1, 2, 3);
            testRS.Fields[nameof(ILinkData.LinkFields.FamNr)].Value.Returns(2, 3, 4);
            testRS.Fields[nameof(ILinkData.LinkFields.PerNr)].Value.Returns(3, 4, 5);
            testClass = new(testRS);
            testRS.ClearReceivedCalls();
        }

        [TestCleanup]
        public void Cleanup()
        {
            CLinkData.Reset();
        }

        [TestMethod()]
        public void CLinkDataTest()
        {
            Assert.IsNotNull(testClass);
            Assert.IsInstanceOfType(testClass, typeof(CLinkData));
            Assert.IsInstanceOfType(testClass, typeof(ILinkData));
        }

        [TestMethod()]
        public void CLinkDataTest1()
        {
            var testClass = new CLinkData(ELinkKennz.lkGodparent, 1, 2);
            Assert.IsNotNull(testClass);
            Assert.IsInstanceOfType(testClass, typeof(CLinkData));
            Assert.IsInstanceOfType(testClass, typeof(ILinkData));
        }

        [TestMethod()]
        public void CLinkDataTest2()
        {
            var testClass = new CLinkData();
            Assert.IsNotNull(testClass);
            Assert.IsInstanceOfType(testClass, typeof(CLinkData));
            Assert.IsInstanceOfType(testClass, typeof(ILinkData));
            try
            {
                testClass.Delete();
            }
            catch (Exception)
            {
            }
        }

        [TestMethod()]
        public void SetLinkTblGetterTest()
        {
            var testTable = Substitute.For<IRecordset>();
            CLinkData.SetTableGtr(() => testRS);
        }

        [TestMethod()]
        public void FillDataTest()
        {
            testClass.FillData(testRS);
            Assert.AreEqual(3,testClass.iFamNr);
            Assert.AreEqual(4,testClass.iPersNr);
            Assert.AreEqual(ELinkKennz.lkMother,testClass.eKennz);
        }

        [TestMethod()]
        public void AppendDBTest()
        {
            testClass.AppendDB();
            testRS.Received(1).AddNew();
            _ = testRS.Received(3).Fields[""];
            testRS.Received(1).Update();
        }

        [TestMethod()]
        [DataRow(true)]
        [DataRow(false)]
        public void DeleteTest(bool xAct)
        {
            testRS.NoMatch.Returns(!xAct);
            testClass.Delete();
            Assert.AreEqual(nameof(LinkIndex.FamPruef),testRS.Index);
            testRS.Received(1).Seek("=",2,3,ELinkKennz.lkFather);
            _ = testRS.Received(0).Fields[""];
            testRS.Received(xAct ?1:0).Delete();
        }

        [DataTestMethod()]
        [DataRow(1,true,1,2,ELinkKennz.lkNone,3)]
        [DataRow(2,false,3,2,ELinkKennz.lkFather,2)]
        public void SetPersTest(int iAct,bool xPre, int iPre1,int iPre2, ELinkKennz ePre3, int iExp)
        {
            testRS.NoMatch.Returns(xPre);
            testRS.Fields[nameof(ILinkData.LinkFields.PerNr)].Value.Returns(iPre1);
            testRS.Fields[nameof(ILinkData.LinkFields.FamNr)].Value.Returns(iPre2);
            testRS.Fields[nameof(ILinkData.LinkFields.Kennz)].Value.Returns(ePre3);

            testClass.SetPers(iAct);

            Assert.AreEqual(iExp,testClass.iPersNr);
        }

        [DataTestMethod()]
        [DataRow(1, true, 1, 2, ELinkKennz.lkNone, 2)]
        [DataRow(3, false, 3, 2, ELinkKennz.lkFather, 3)]
        public void SetFamTest(int iAct, bool xPre, int iPre1, int iPre2, ELinkKennz ePre3, int iExp)
        {
            testRS.NoMatch.Returns(xPre);
            testRS.Fields[nameof(ILinkData.LinkFields.Kennz)].Value.Returns(iPre1);
            testRS.Fields[nameof(ILinkData.LinkFields.FamNr)].Value.Returns(iPre2);
            testRS.Fields[nameof(ILinkData.LinkFields.PerNr)].Value.Returns(ePre3);

            testClass.SetFam(iAct);

            Assert.AreEqual(iExp, testClass.iFamNr);
        }

        [DataTestMethod()]
        [DataRow(ELinkProp.iPersNr, TypeCode.Int32)]
        [DataRow(ELinkProp.iFamNr, TypeCode.Int32)]
        [DataRow(ELinkProp.eKennz, TypeCode.Int32)]
        public void GetPropTypeTest(ELinkProp pAct, TypeCode eExp)
        {
            Assert.AreEqual(eExp, Type.GetTypeCode(testClass.GetPropType(pAct)));
        } 
        
        [DataTestMethod()]
        [DataRow((ELinkProp)(0-1), TypeCode.Int32)]
        [DataRow((ELinkProp)3, TypeCode.Int32)]
        [DataRow((ELinkProp)100, TypeCode.Int32)]
        public void GetPropTypeTest2(ELinkProp pAct, TypeCode eExp)
        {
            Assert.ThrowsException<NotImplementedException>(() => testClass.GetPropType(pAct));
        }

        [DataTestMethod()]
        [DataRow(ELinkProp.iPersNr, 3)]
        [DataRow(ELinkProp.iFamNr, 2)]
        [DataRow(ELinkProp.eKennz, ELinkKennz.lkFather)]
        public void GetPropValueTest(ELinkProp eExp, object oAct)
        {
            Assert.AreEqual(oAct, testClass.GetPropValue(eExp));
        }

        [DataTestMethod()]
        [DataRow((ELinkProp)(0 - 1), TypeCode.Int32)]
        [DataRow((ELinkProp)3, TypeCode.Int32)]
        [DataRow((ELinkProp)100, TypeCode.Int32)]
        public void GetPropValueTest1(ELinkProp eExp, object oAct)
        {
            Assert.ThrowsException<NotImplementedException>(() => testClass.GetPropValue(eExp));
        }

        [TestMethod()]
        public void GetPropValueTest2()
        {
            Assert.AreEqual(3, testClass.GetPropValue<int>(ELinkProp.iPersNr));
        }

        [DataTestMethod()]
        [DataRow(ELinkProp.iPersNr, 1)]
        [DataRow(ELinkProp.iFamNr, 4)]
        [DataRow(ELinkProp.eKennz, ELinkKennz.lkChild)]
        [DataRow(ELinkProp.iPersNr, 3)]
        [DataRow(ELinkProp.iFamNr, 2)]
        [DataRow(ELinkProp.eKennz, ELinkKennz.lkFather)]
        public void SetPropValueTest(ELinkProp eAct, object iVal)
        {
            testClass.SetPropValue(eAct, iVal);
            Assert.AreEqual(iVal, testClass.GetPropValue(eAct));
        }

        [DataTestMethod()]
        [DataRow((ELinkProp)(0 - 1), TypeCode.Int32)]
        [DataRow((ELinkProp)3, TypeCode.Int32)]
        [DataRow((ELinkProp)100, TypeCode.Int32)]
        public void SetPropValueTest2(ELinkProp eAct, object iVal)
        {
            Assert.ThrowsException<NotImplementedException>(() => testClass.SetPropValue(eAct, iVal));
        }

        [TestMethod()]
        public void ClearChangedPropsTest()
        {
            AddChangedPropTest();
            testClass.ClearChangedProps();
            Assert.AreEqual(0, testClass.ChangedProps.Count);
        }

        [TestMethod()]
        public void AddChangedPropTest()
        {
            testClass.AddChangedProp(ELinkProp.iPersNr);
            testClass.AddChangedProp(ELinkProp.iFamNr);
            testClass.AddChangedProp(ELinkProp.iPersNr);
            Assert.AreEqual(2, testClass.ChangedProps.Count);
        }

        [DataTestMethod()]
        [DataRow(ELinkProp.iPersNr, 1)]
        [DataRow(ELinkProp.iFamNr, 2)]
        [DataRow(ELinkProp.eKennz, 3)]
        public void SetDBValueTest(ELinkProp eAct, object _)
        {
            testClass.SetDBValue(testRS, new[] { (Enum)eAct });
            _ = testRS.Received().Fields[eAct.ToString()];
        }
        [DataTestMethod()]
        [DataRow((ELinkProp)3, 4)]
        public void SetDBValueTest3(ELinkProp eAct, object _)
        {
            Assert.ThrowsException<NotImplementedException>(()=> testClass.SetDBValue(testRS, new[] { (Enum)eAct }));
        }

        [DataTestMethod()]
        [DataRow(ELinkProp.iPersNr, 1)]
        [DataRow(ELinkProp.iFamNr, 4)]
        [DataRow(ELinkProp.eKennz, ELinkKennz.lkChild)]
        public void SetDBValueTest2(ELinkProp eAct, object oVal)
        {
            testClass.SetPropValue(eAct, oVal);
            testClass.SetDBValue(testRS, null);
            _ = testRS.Received().Fields[eAct.ToString()];
        }
    }
}