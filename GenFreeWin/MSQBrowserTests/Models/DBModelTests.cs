using Microsoft.VisualStudio.TestTools.UnitTesting;
using MSQBrowser.Models;
using MSQBrowser.Models.Interfaces;
using MySqlConnector;
using System.Threading.Tasks;

namespace MSQBrowser.Models.Tests
{
    [TestClass]
    public class DBModelTests
    {
        private DBModel testModel = null!;

        [TestInitialize]
        public void Init()
        {
            testModel = new DBModel(CreateDisconnectedConnection());
        }

        [TestMethod]
        public void DBModelTest()
        {
            Assert.IsNotNull(testModel);
            Assert.IsInstanceOfType(testModel, typeof(DBModel));
            Assert.IsInstanceOfType(testModel, typeof(IDBModel));
            Assert.AreEqual("testdb", testModel.DbName);
            Assert.AreEqual(0, testModel.dbMetaData.Count);
            Assert.AreEqual(0, testModel.dbDataTypes.Count);
        }

        [TestMethod]
        public void QueryTableTest_ReturnsEmptyWithoutOpenConnection()
        {
            var result = testModel.QueryTable("Users");

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void QueryTableDataTest_ReturnsEmptyTableWithoutOpenConnection()
        {
            var result = testModel.QueryTableData("Users");

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result!.Rows.Count);
            Assert.AreEqual(0, result.Columns.Count);
        }

        [TestMethod]
        public async Task QueryTableDataPageAsyncTest_ReturnsNormalizedEmptyPageWithoutOpenConnection()
        {
            var result = await testModel.QueryTableDataPageAsync("Users", [], -5, 0);

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Offset);
            Assert.AreEqual(1, result.PageSize);
            Assert.IsFalse(result.HasMoreRows);
            Assert.AreEqual(0, result.Data.Rows.Count);
            Assert.AreEqual(0, result.Data.Columns.Count);
        }

        [TestMethod]
        public async Task QueryTableDataPageAsyncTest_ReturnsEmptyForInvalidIdentifier()
        {
            var result = await testModel.QueryTableDataPageAsync("bad name", [], 10, 25);

            Assert.AreEqual(10, result.Offset);
            Assert.AreEqual(25, result.PageSize);
            Assert.IsFalse(result.HasMoreRows);
            Assert.AreEqual(0, result.Data.Rows.Count);
        }

        [DataTestMethod]
        [DataRow(130, "VarChar")]
        [DataRow(-1, "Unknown")]
        public void GetTypeNameTest(int iAct, string sExp)
        {
            Assert.AreEqual(sExp, testModel.GetTypeName(iAct));
        }

        private static MySqlConnectionStringBuilder CreateDisconnectedConnection()
        {
            return new MySqlConnectionStringBuilder
            {
                Server = "127.0.0.1",
                Port = 1,
                UserID = "test",
                Password = "test",
                Database = "testdb",
                CharacterSet = "UTF8",
                ConnectionTimeout = 1
            };
        }
    }
}
