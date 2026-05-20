using System;
using Db.Core.Abstractions.Sql;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DbCoreAbstractions.UnitTests.Sql
{
    /// <summary>
    /// Unit tests for the <see cref="DbFilterClause"/> constructor.
    /// </summary>
    [TestClass]
    public class DbFilterClauseTests
    {
        /// <summary>
        /// Tests that the constructor correctly assigns all properties when all arguments are provided.
        /// </summary>
        [TestMethod]
        public void Constructor_AllArgumentsProvided_PropertiesAssignedCorrectly()
        {
            // Arrange
            string field = "TestField";
            DbFilterOperator op = DbFilterOperator.Equal;
            string parameterName = "@param";

            // Act
            var clause = new DbFilterClause(field, op, parameterName);

            // Assert
            Assert.AreEqual(field, clause.Field);
            Assert.AreEqual(op, clause.Operator);
            Assert.AreEqual(parameterName, clause.ParameterName);
        }

        /// <summary>
        /// Tests that the constructor assigns ParameterName to null when not provided.
        /// </summary>
        [TestMethod]
        public void Constructor_ParameterNameNotProvided_ParameterNameIsNull()
        {
            // Arrange
            string field = "TestField";
            DbFilterOperator op = DbFilterOperator.IsNull;

            // Act
            var clause = new DbFilterClause(field, op);

            // Assert
            Assert.AreEqual(field, clause.Field);
            Assert.AreEqual(op, clause.Operator);
            Assert.IsNull(clause.ParameterName);
        }

        /// <summary>
        /// Tests that the constructor allows an empty string for field and parameterName.
        /// </summary>
        [TestMethod]
        public void Constructor_EmptyStrings_PropertiesAssignedCorrectly()
        {
            // Arrange
            string field = string.Empty;
            DbFilterOperator op = DbFilterOperator.Equal;
            string parameterName = string.Empty;

            // Act
            var clause = new DbFilterClause(field, op, parameterName);

            // Assert
            Assert.AreEqual(field, clause.Field);
            Assert.AreEqual(op, clause.Operator);
            Assert.AreEqual(parameterName, clause.ParameterName);
        }

    }
}
