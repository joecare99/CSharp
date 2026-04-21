using Microsoft.VisualStudio.TestTools.UnitTesting;
using MSQBrowser.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSQBrowser.Models.Tests
{

    [TestClass()]
    public class DBModelTests
    {
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
        DBModel testModel;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
        [TestInitialize]
        public void Init()
        {
            testModel = new DBModel("Resources\\mydb.mdb");
        }

        [TestMethod()]
        public void DBModelTest()
        {
            Assert.IsNotNull(testModel);
            Assert.IsInstanceOfType(testModel, typeof(DBModel));
            Assert.IsInstanceOfType(testModel, typeof(IDBModel));
            Assert.AreEqual(@"(N:MetaDataCollections, K:Schema D:())
(N:DataSourceInformation, K:Schema D:())
(N:DataTypes, K:Schema D:())
(N:Restrictions, K:Schema D:())
(N:ReservedWords, K:Schema D:())
(N:Columns, K:Schema D:())
(N:Indexes, K:Schema D:())
(N:Procedures, K:Schema D:())
(N:Tables, K:Schema D:())
(N:Views, K:Schema D:())
(N:Users, K:Table D:())
(N:Id, K:Column D:())
(N:Name, K:Column D:())
(N:Surname, K:Column D:())", string.Join("\r\n", testModel.dbMetaData));
            Assert.AreEqual(@"(N:Short, K:DataTypes D:(System.Int16, 2, 5))
(N:Long, K:DataTypes D:(System.Int32, 3, 10))
(N:Single, K:DataTypes D:(System.Single, 4, 7))
(N:Double, K:DataTypes D:(System.Double, 5, 15))
(N:Currency, K:DataTypes D:(System.Decimal, 6, 19))
(N:DateTime, K:DataTypes D:(System.DateTime, 7, 8))
(N:Bit, K:DataTypes D:(System.Boolean, 11, 2))
(N:Byte, K:DataTypes D:(System.Byte, 17, 3))
(N:GUID, K:DataTypes D:(System.Guid, 72, 16))
(N:BigBinary, K:DataTypes D:(System.Byte[], 204, 4000))
(N:LongBinary, K:DataTypes D:(System.Byte[], 205, 1073741823))
(N:VarBinary, K:DataTypes D:(System.Byte[], 204, 510))
(N:LongText, K:DataTypes D:(System.String, 203, 536870910))
(N:VarChar, K:DataTypes D:(System.String, 202, 255))
(N:Decimal, K:DataTypes D:(System.Decimal, 131, 28))", string.Join("\r\n", testModel.dbDataTypes));

        }

        [TestMethod()]
        public void DBModelTest1()
        {
            var testModel = new DBModel("");
            Assert.IsNotNull(testModel);
            Assert.IsInstanceOfType(testModel, typeof(DBModel));
            Assert.IsInstanceOfType(testModel, typeof(IDBModel));
            Assert.AreEqual("", string.Join("\r\n", testModel.dbMetaData));
            Assert.AreEqual("", string.Join("\r\n", testModel.dbDataTypes));
        }

        [DataTestMethod()]
        [DataRow("", new[] { "" })]
        [DataRow("Test", new[] { "" })]
        [DataRow("Users", new[] { "(N:Id, K:Column D:(Long, )), (N:Name, K:Column D:(VarChar, 60)), (N:Surname, K:Column D:(VarChar, 60))" })]
        public void QueryTableTest(string sName, string[] asExp)
        {
            var r = testModel.QueryTable(sName);
            Assert.AreEqual(asExp[0], string.Join(", ", r));
        }

        [DataTestMethod()]
        [DataRow("", new[] { "" })]
        [DataRow("Test", new[] { "" })]
        [DataRow("Users", new[] { "" })]
        public void QueryTableDataTest(string sName, string[] asExp)
        {
            var r = testModel.QueryTableData(sName);
            Assert.AreEqual(asExp[0], string.Join(", ", r));
        }

        [DataTestMethod()]
        [DataRow("", new[] { "" })]
        [DataRow("Tables", new[] { "Tables" })]
        public void QuerySchemaTest(string sName, string[] asExp)
        {
            var r = testModel.QuerySchema(sName);
            Assert.AreEqual(asExp[0], string.Join(", ", r));
        }
        [DataTestMethod()]
        [DataRow("Users", new[] { "" })]
        public void QuerySchemaTest2(string sName, string[] asExp)
        {
            Assert.ThrowsException<ArgumentException>(() => _ = testModel.QuerySchema(sName));
        }

        [DataTestMethod()]
        [DataRow(2, "Short")]
        [DataRow(130, "VarChar")]
        [DataRow(131, "Decimal")]
        [DataRow(-1, "Unknown")]
        public void GetTypeNameTest(int iAct, string sExp)
        {
            Assert.AreEqual(sExp, testModel.GetTypeName(iAct));
        }
    }
}
