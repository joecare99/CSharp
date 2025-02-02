// ***********************************************************************
// Assembly         : MVVM_40_WizzardTests
// Author           : Mir
// Created          : 06-14-2024
//
// Last Modified By : Mir
// Last Modified On : 06-14-2024
// ***********************************************************************
// <copyright file="SimpleLogTests.cs" company="JC-Soft">
//     Copyright (c) HP Inc.. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using BaseLib.Interfaces;
using BaseLib.Models.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MVVM.ViewModel;
using NSubstitute;
using System;
using System.Linq;

/// <summary>
/// The Tests namespace.
/// </summary>
namespace MVVM_40_Wizzard.Models.Tests;

/// <summary>
/// Defines test class SimpleLogTests.
/// Implements the <see cref="BaseTestViewModel" />
/// </summary>
/// <seealso cref="BaseTestViewModel" />
[TestClass]
public class SimpleLogTests:BaseTestViewModel
{
    /// <summary>
    /// The gs old
    /// </summary>
    private Action<string> _gsOld;
    /// <summary>
    /// The system time
    /// </summary>
    private ISysTime? _sysTime;
    /// <summary>
    /// The simple log
    /// </summary>
    private SimpleLog simpleLog;


    /// <summary>
    /// Tests the initialize.
    /// </summary>
    [TestInitialize]
    public void TestInitialize()
    {
        _gsOld= SimpleLog.LogAction;
        _gsOld("Test message");
        SimpleLog.LogAction = DoLog; 
        _sysTime = Substitute.For<ISysTime>();
        _sysTime.Now.Returns(new DateTime(2022, 08, 24, 12, 0, 0));
        simpleLog = new SimpleLog(_sysTime);
    }

    /// <summary>
    /// Tests the cleanup.
    /// </summary>
    [TestCleanup]
    public void TestCleanup()
    {
        SimpleLog.LogAction = _gsOld;
    }

    /// <summary>
    /// Logs the test.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="asExp">As exp.</param>
    [DataTestMethod]
    [DataRow("Test message",new[] { "08/24/2022 12:00:00: Msg: Test message\r\n" })]
    [DataRow(null, new[] { "08/24/2022 12:00:00: Msg: \r\n" })]
    [DataRow("Some other test", new[] { "08/24/2022 12:00:00: Msg: Some other test\r\n" })]

    public void LogTest(string message, string[] asExp)
    {

        // Act
        simpleLog.Log(message);

        // Assert
        Assert.AreEqual(1, _sysTime.ReceivedCalls().Count());
        Assert.AreEqual(asExp[0],DebugLog);
    }

    /// <summary>
    /// Defines the test method LogTest1.
    /// </summary>
    [TestMethod]
    public void LogTest1()
    {
        // Arrange
        var message = "Test message";
        var exception = new Exception("Test exception");

        // Act
        simpleLog.Log(message, exception);

        // Assert
        Assert.AreEqual(1, _sysTime.ReceivedCalls().Count());
        Assert.AreEqual("08/24/2022 12:00:00: Err: Test message, System.Exception: Test exception\r\n", DebugLog);
    }
}
