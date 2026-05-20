using System;
using System.Collections.Generic;
using Db.Core.Abstractions.Sql;
using Db.Provider.MySql;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Db.Provider.MySql.Tests
{
    [TestClass]
    public class MySqlStatementRendererTests
    {
        [TestMethod]
        public void RenderSelect_WithFiltersAndLimit_RendersExpectedSql()
        {
            var xRenderer = new MySqlStatementRenderer();
            var xFilter = Substitute.For<IDbFilterClause>();
            xFilter.Field.Returns("Person.Id");
            xFilter.Operator.Returns(DbFilterOperator.Equal);
            xFilter.ParameterName.Returns("@id");

            var xStatement = Substitute.For<IDbSelectStatement>();
            xStatement.Table.Returns("Person");
            xStatement.Fields.Returns(new[] { "Person.Id", "Name" });
            xStatement.Filters.Returns(new[] { xFilter });
            xStatement.Limit.Returns(5);

            var sResult = xRenderer.RenderSelect(xStatement);

            Assert.AreEqual("SELECT `Person`.`Id`,`Name` FROM `Person` WHERE `Person`.`Id`=@id limit 5", sResult);
        }

        [TestMethod]
        public void RenderInsert_RendersExpectedSql()
        {
            var xRenderer = new MySqlStatementRenderer();
            var xStatement = Substitute.For<IDbInsertStatement>();
            xStatement.Table.Returns("Person");
            xStatement.Fields.Returns(new List<KeyValuePair<string, string>>
            {
                new("Name", "@name"),
                new("City", "@city")
            });

            var sResult = xRenderer.RenderInsert(xStatement);

            Assert.AreEqual("INSERT INTO `Person` (`Name`, `City`) VALUES (@name, @city);", sResult);
        }

        [TestMethod]
        public void RenderUpdate_WithIsNullFilter_RendersExpectedSql()
        {
            var xRenderer = new MySqlStatementRenderer();
            var xFilter = Substitute.For<IDbFilterClause>();
            xFilter.Field.Returns("DeletedAt");
            xFilter.Operator.Returns(DbFilterOperator.IsNull);
            xFilter.ParameterName.Returns((string?)null);

            var xStatement = Substitute.For<IDbUpdateStatement>();
            xStatement.Table.Returns("Person");
            xStatement.Fields.Returns(new List<KeyValuePair<string, string>>
            {
                new("Name", "@name")
            });
            xStatement.Filters.Returns(new[] { xFilter });

            var sResult = xRenderer.RenderUpdate(xStatement);

            Assert.AreEqual("UPDATE `Person` SET `Name`=@name WHERE `DeletedAt` is null", sResult);
        }

        [TestMethod]
        public void RenderSelect_WithNullStatement_Throws()
        {
            var xRenderer = new MySqlStatementRenderer();

            Assert.ThrowsExactly<ArgumentNullException>(() => xRenderer.RenderSelect(null!));
        }
    }
}
