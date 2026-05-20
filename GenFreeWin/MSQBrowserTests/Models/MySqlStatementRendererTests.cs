using System;
using System.Data;
using Db.Core.Abstractions.Sql;
using Db.Core.Abstractions.Sql.Interfaaces;
using Db.Provider.MySql;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace MSQBrowser.Models.Tests
{
    [TestClass]
    public class MySqlStatementRendererTests
    {
        [TestMethod]
        public void RenderSelect_QuotesIdentifiersAndLimit()
        {
            var _dbconn = Substitute.For<IDbConnection>();
            var sut = new MySqlStatementRenderer(_dbconn);
            var statement = new DbSelectStatement("Users", new[] { "Id", "Name" }, Array.Empty<IDbFilterClause>(), 10);

            var result = sut.RenderSelect(statement);

            Assert.AreEqual("SELECT `Id`,`Name` FROM `Users` limit 10", result);
        }
    }
}
