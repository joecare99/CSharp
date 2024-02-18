using GenFree.Helper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using NSubstitute;
using GenFree.Data;
using GenFree.Interfaces.DB;

namespace GenFree.Helper.Tests
{
    [TestClass()]
    public class ObjectHelperTests
    {
        [System.Diagnostics.DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
        private class CTest
        {
            public override string? ToString() => null;
            private string GetDebuggerDisplay() => "CTest: Null";
        }

        [TestMethod()]
        [DataTestMethod()]
        [DataRow(0, null)]
        [DataRow(0, default(DBNull), DisplayName = "DBNull")]
        [DataRow(0, "Zero")]
        [DataRow(1, "1")]
        [DataRow(50, '2')]
        [DataRow(3, 3)]
        [DataRow(4, 4.0)]
        [DataRow(5, 5.0f)]
        [DataRow(6, 6L)]
        [DataRow(7, 7U)]
        [DataRow(8, 8UL)]
        [DataRow(9, 9)]
        [DataRow(-1, 0xffffffff)]
        public void AsIntTest(int iExp, object sAct)
        {
            Assert.AreEqual(iExp, sAct.AsInt(), $"AsInt({sAct})");
        }

        [DataTestMethod()]
        [DataRow(0, null)]
        [DataRow(1, "1")]
        [DataRow(3, 3)]
        public void AsIntTest2(int iExp, object sAct0)
        {
            var sAct = Substitute.For<IField>();
            sAct.Value.Returns(sAct0);
            AsIntTest(iExp, sAct);
        }

        [DataTestMethod()]
        public void AsIntTest3()
        {
            var sAct = new CTest();
            AsIntTest(default, sAct);
        }

        [DataTestMethod()]
        public void AsIntTest4()
        {
            var sAct = DBNull.Value;
            AsIntTest(default, sAct);
        }

        [TestMethod()]
        [DataTestMethod()]
        [DataRow(0, null)]
        [DataRow(0, default(DBNull), DisplayName = "DBNull")]
        [DataRow(0, "Zero")]
        [DataRow(1, "1")]
        [DataRow(50, '2')]
        [DataRow(3, 3)]
        [DataRow(4, 4.0)]
        [DataRow(5, 5.0f)]
        [DataRow(6, 6L)]
        [DataRow(7, 7U)]
        [DataRow(8, 8UL)]
        [DataRow(9, 9)]
        [DataRow(-1, 0xffffffffffffffff)]
        public void AsLongTest(long lExp, object sAct)
        {
            Assert.AreEqual(lExp, sAct.AsLong(), $"AsLong({sAct})");
        }

        [DataTestMethod()]
        [DataRow(0, null)]
        [DataRow(1, "1")]
        [DataRow(3, 3)]
        public void AsLongTest2(long lExp, object sAct0)
        {
            var sAct = Substitute.For<IField>();
            sAct.Value.Returns(sAct0);
            AsLongTest(lExp, sAct);
        }

        [DataTestMethod()]
        public void AsLongTest3()
        {
            var sAct = new CTest();
            AsLongTest(default, sAct);
        }

        [DataTestMethod()]
        public void AsLongTest4()
        {
            var sAct = DBNull.Value;
            AsLongTest(default, sAct);
        }

        [TestMethod()]
        [DataTestMethod()]
        [DataRow(0, null)]
        [DataRow(0, default(ENameKennz))]
        [DataRow(0, "Zero")]
        [DataRow(1, "1")]
        [DataRow(1, nameof(ENameKennz.nkName))]
        [DataRow(50, '2')]
        [DataRow(3, 3)]
        [DataRow(4, 4.0)]
        [DataRow(5, 5.0f)]
        [DataRow(6, 6L)]
        [DataRow(7, 7U)]
        [DataRow(8, 8UL)]
        [DataRow(9, 9)]
        [DataRow(25, "25")]
        [DataRow(-1, 0xffffffff)]
        public void AsEnumTest(int sExp, object sAct)
        {
            var eExp = (ENameKennz)sExp;
            Assert.AreEqual(eExp, sAct.AsEnum<ENameKennz>(), $"AsEnum({sAct})");
        }

        [DataTestMethod()]
        [DataRow(0, null)]
        [DataRow(1, "1")]
        [DataRow(3, 3)]
        public void AsEnumTest2(int sExp, object sAct0)
        {
            var eExp = sExp.AsEnum<ENameKennz>();
            var sAct = Substitute.For<IField>();
            sAct.Value.Returns(sAct0);
            AsEnumTest(sExp, sAct);
        }

        [DataTestMethod()]
        public void AsEnumTest3()
        {
            var sAct = new CTest();
            AsEnumTest(default, sAct);
        }

        [DataTestMethod()]
        public void AsEnumTest4()
        {
            var sAct = DBNull.Value;
            AsEnumTest(default, sAct);
        }

        [DataTestMethod()]
        [DataRow(0, null)]
        [DataRow(0, 0)]
        [DataRow(624511296000000000, 19800101)]
        [DataRow(0, "Zero")]
        [DataRow(0, "0")]
        [DataRow(624512160000000000, "19800102")]
        [DataRow(624513024000000000, "03.01.1980")]
        [DataRow(624512160000000000, "1980/01/02")]
        [DataRow(599266080000000000, "2")]
        [DataRow(0, nameof(ENameKennz.nkName))]
        [DataRow(615044448000000000, '2')]
        [DataRow(599266944000000000, 3)]
        [DataRow(599267808000000000, 4.0)]
        [DataRow(599268672000000000, 5.0f)]
        [DataRow(6, 6L)]
        [DataRow(599270400000000000, 7U)]
        [DataRow(599271264000000000, 8UL)]
        [DataRow(599272128000000000, 9)]
        [DataRow(599263488000000000, 0xffffffff)]
        public void AsDateTest(long dExp, object sAct)
        {
            DateTime d;
            Assert.AreEqual(DateTime.FromBinary(dExp), d = sAct.AsDate(), $"AsDate({sAct}) ({d.ToBinary()})");
        }

        [DataTestMethod()]
        [DataRow(0, null)]
        [DataRow(599265216000000000, "1")]
        [DataRow(599266944000000000, 3)]
        public void AsDateTest2(long lExp, object sAct0)
        {
            var sAct = Substitute.For<IField>();
            sAct.Value.Returns(sAct0);
            AsDateTest(lExp, sAct);
        }

        [DataTestMethod()]
        public void AsDateTest3()
        {
            var sAct = new CTest();
            AsDateTest(default, sAct);
        }

        [DataTestMethod()]
        public void AsDateTest4()
        {
            var sAct = DBNull.Value;
            AsDateTest(default, sAct);
        }

        [DataTestMethod()]
        public void AsDateTest5()
        {
            var sAct = default(DateTime);
            AsDateTest(default, sAct);
        }

        [DataTestMethod()]
        [DataRow(0.0, null)]
        [DataRow(0.0, default(DBNull), DisplayName = "DBNull")]
        [DataRow(0.0, "Zero")]
        [DataRow(1.1, "1.1")]
        [DataRow(50, '2')]
        [DataRow(3.0, 3)]
        [DataRow(4.0, 4.0)]
        [DataRow(5.0, 5.0f)]
        [DataRow(6.0, 6L)]
        [DataRow(7.0, 7U)]
        [DataRow(8.0, 8UL)]
        [DataRow(9.0, 9)]
        [DataRow(4294967295.0, 0xffffffff)]
        public void AsDoubleTest(double fExp, object sAct)
        {
            Assert.AreEqual(fExp, sAct.AsDouble(), $"AsDouble({sAct})");
        }
        [DataTestMethod()]
        [DataRow(0, null)]
        [DataRow(1, "1")]
        [DataRow(3, 3)]
        public void AsDoubleTest2(double fExp, object sAct0)
        {
            var sAct = Substitute.For<IField>();
            sAct.Value.Returns(sAct0);
            AsDoubleTest(fExp, sAct);
        }

        [DataTestMethod()]
        public void AsDoubleTest3()
        {
            var sAct = new CTest();
            AsDoubleTest(default, sAct);
        }

        [DataTestMethod()]
        public void AsDoubleTest4()
        {
            var sAct = DBNull.Value;
            AsDoubleTest(default, sAct);
        }

        [DataTestMethod()]
        [DataRow(false, null)]
        [DataRow(true, "true")]
        [DataRow(true, true)]
        [DataRow(true, 'b')]
        [DataRow(true, 3)]
        [DataRow(true, 4.0)]
        [DataRow(true, 5.0f)]
        [DataRow(true, 6L)]
        [DataRow(true, 7U)]
        [DataRow(true, 8UL)]
        [DataRow(true, 9)]
        public void AsBoolTest(bool xExp, object sAct)
        {
            Assert.AreEqual(xExp, sAct.AsBool(), $"AsBool({sAct})");
        }
        [DataTestMethod()]
        [DataRow(false, null)]
        [DataRow(true, "true")]
        [DataRow(true, 3)]
        public void AsBoolTest2(bool xExp, object sAct0)
        {
            var sAct = Substitute.For<IField>();
            sAct.Value.Returns(sAct0);
            AsBoolTest(xExp, sAct);
        }

        [DataTestMethod()]
        public void AsBoolTest3()
        {
            var sAct = new CTest();
            AsBoolTest(default, sAct);
        }

        [DataTestMethod()]
        public void AsBoolTest4()
        {
            var sAct = DBNull.Value;
            AsBoolTest(default, sAct);
        }

        [DataTestMethod()]
        [DataRow(0, null)]
        [DataRow(0, "True")]
        [DataRow(255, "255")]
        [DataRow(16, "00000010-0000-0000-0000-000000000000")]
        [DataRow(0x62, 'b')]
        [DataRow(3, 3)]
        [DataRow(4, 4.0)]
        [DataRow(5, 5.0f)]
        [DataRow(6, 6L)]
        [DataRow(7, 7U)]
        [DataRow(8, 8UL)]
        [DataRow(9, 9)]
        public void AsGUIDTest(int iExp, object sAct)
        {
            var gExp = new Guid(iExp, 0, 0, new byte[8]);
            Assert.AreEqual(gExp, sAct.AsGUID(), $"AsGUID({sAct})");
        }
        [DataTestMethod()]
        [DataRow(0, null)]
        [DataRow(1, "1")]
        [DataRow(3, 3)]
        public void AsGUIDTest2(int iExp, object sAct0)
        {
            var sAct = Substitute.For<IField>();
            sAct.Value.Returns(sAct0);
            AsGUIDTest(iExp, sAct);
        }

        [DataTestMethod()]
        public void AsGUIDTest3()
        {
            var sAct = new CTest();
            AsGUIDTest(default, sAct);
        }

        [DataTestMethod()]
        public void AsGUIDTest4()
        {
            var sAct = DBNull.Value;
            AsGUIDTest(default, sAct);
        }

        [DataTestMethod()]
        public void AsGUIDTest5()
        {
            var sAct = Guid.Empty;
            AsGUIDTest(default, sAct);
        }

        [DataTestMethod()]
        [DataRow("Zero", null,null)]
        [DataRow("1-0", 1, 0.0)]
        [DataRow("0-1", 0, 1.0)]
        [DataRow("2-1", 2, double.PositiveInfinity)]
        public void SetRetTest(string sName,int iAct,double dAct)
        {
            Assert.AreEqual(iAct,dAct.SetRet((d)=>Assert.AreEqual(dAct,d),iAct));
        }
    }
}