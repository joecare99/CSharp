// ***********************************************************************
// Assembly         : MVVM_40_Wizzard_netTests
// Author           : Mir
// Created          : 06-14-2024
//
// Last Modified By : Mir
// Last Modified On : 06-14-2024
// ***********************************************************************
// <copyright file="ListEntryTests.cs" company="JC-Soft">
//     Copyright © JC-Soft 2023
// </copyright>
// <summary></summary>
// ***********************************************************************
using Microsoft.VisualStudio.TestTools.UnitTesting;

/// <summary>
/// The Tests namespace.
/// </summary>
namespace MVVM_40_Wizzard.Models.Tests;

/// <summary>
/// Defines test class ListEntryTests.
/// </summary>
[TestClass]
public class ListEntryTests
{
        // Arrange
        ListEntry testClass;

    /// <summary>
    /// Defines the test method ListEntryTest.
    /// </summary>
    [DataTestMethod]
    [DataRow(1, "Test")]
    [DataRow(2, "")]
    [DataRow(-1, null)]

    public void ListEntryTest(int value,string text)
    {

        // Act
        testClass = new ListEntry(value, text);

        // Assert
        Assert.AreEqual(value, testClass.ID);
        Assert.AreEqual(text, testClass.Text);
    }

    /// <summary>
    /// Defines the test method ToStringTest.
    /// </summary>
    [TestMethod]
    [DataTestMethod]
    [DataRow(1, "Test")]
    [DataRow(2, "")]
    [DataRow(-1, null)]
    public void ToStringTest(int value, string text)
    {
        // Arrange
        testClass = new ListEntry(value, text);

        // Act
        string result = testClass.ToString();

        // Assert
        Assert.AreEqual(text, result);
    }
}
