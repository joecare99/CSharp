using System;
using System.Data;
using Db.Core.Abstractions.Sql;
using Db.Core.Abstractions.Sql.Interfaaces;
using GenFree.Data.DB;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace MdbBrowser.Models.Tests;

[TestClass]
public class OleDbStatementRendererTests
{
    [TestMethod]
    public void RenderSelect_QuotesIdentifiers()
    {
        var dbc = Substitute.For<IDbConnection>();
        var sut = new OleDbStatementRenderer(dbc);
        var statement = new DbSelectStatement("Users", new[] { "Id", "Name" }, Array.Empty<IDbFilterClause>());

        var result = sut.RenderSelect(statement);

        Assert.AreEqual("SELECT [Id],[Name] FROM [Users]", result);
    }
}
