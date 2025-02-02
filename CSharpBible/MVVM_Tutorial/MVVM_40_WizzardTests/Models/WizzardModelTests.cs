﻿// ***********************************************************************
// Assembly         : MVVM_40_WizzardTests
// Author           : Mir
// Created          : 05-19-2023
//
// Last Modified By : Mir
// Last Modified On : 05-19-2023
// ***********************************************************************
// <copyright file="WizzardModelTests.cs" company="JC-Soft">
//     Copyright © JC-Soft 2023
// </copyright>
// <summary></summary>
// ***********************************************************************
using BaseLib.Helper;
using BaseLib.Interfaces;
using BaseLib.Models.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MVVM.ViewModel;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

/// <summary>
/// The Tests namespace.
/// </summary>
/// <autogeneratedoc />
namespace MVVM_40_Wizzard.Models.Tests;

/// <summary>
/// Defines test class WizzardModelTests.
/// Implements the <see cref="BaseTestViewModel" />
/// </summary>
/// <seealso cref="BaseTestViewModel" />
/// <autogeneratedoc />
[TestClass()]
public class WizzardModelTests : BaseTestViewModel
{
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
    /// <summary>
    /// The test model
    /// </summary>
    /// <autogeneratedoc />
    WizzardModel testModel;
    ISysTime _sysTime;
    ILog _log;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
    protected virtual Dictionary<string, object?> GetDefaultData() => new() {
        { nameof(WizzardModel.Now), new DateTime(2023, 5, 19, 12, 0, 0) },
        { nameof(WizzardModel.MainSelection),-1 },
        { nameof(WizzardModel.SubSelection),-1 },
        { nameof(WizzardModel.MainOptions),new List<int> { 0, 1, 3, 4, 6, 8, 9, 10 } },
        { nameof(WizzardModel.SubOptions),new List<int> { 1, 2, 3, 5, 6, 7, 9, 11 } },
        { nameof(WizzardModel.Additional1),-1 }, 
        { nameof(WizzardModel.Additional2),-1 }, 
        { nameof(WizzardModel.Additional3),-1 }, 
    };

    /// <summary>
    /// Initializes this instance.
    /// </summary>
    [TestInitialize]
    public void Init()
    {
        _sysTime = Substitute.For<ISysTime>();
        _sysTime.Now.Returns(new DateTime(2023, 5, 19, 12, 0, 0), new DateTime(2023, 5, 19, 12, 0, 1), new DateTime(2023, 5, 19, 12, 0, 2));
        _log = Substitute.For<ILog>();
        testModel = new(_sysTime,_log);
        testModel.PropertyChanged += OnVMPropertyChanged;
        if (testModel is INotifyPropertyChanging npchgn)
            npchgn.PropertyChanging += OnVMPropertyChanging;
        ClearLog();
    }

    /// <summary>
    /// Defines the test method SetupTest.
    /// </summary>
    [TestMethod()]
    public void SetupTest()
    {
        Assert.IsNotNull(testModel);
        Assert.IsInstanceOfType(testModel, typeof(WizzardModel));
        Assert.IsInstanceOfType(testModel, typeof(ObservableObject));
        Assert.IsInstanceOfType(testModel, typeof(INotifyPropertyChanged));
    }

    protected static IEnumerable<object[]> TestModelProperies 
        => typeof(WizzardModel).GetProperties().Select(o => new object[] { o.Name, o.PropertyType.TC(), o.CanRead, o.CanWrite });

    [DataTestMethod]
    [DynamicData(nameof(TestModelProperies))]
    public virtual void TestModelProperiesTest(string sPropName, TypeCode tPropType, bool xCanRead, bool xCanWrite)
    {
        var p = typeof(WizzardModel).GetProperty(sPropName);
        Assert.IsNotNull(p);
        Assert.AreEqual(xCanRead, p!.CanRead);
        Assert.AreEqual(xCanWrite, p!.CanWrite);
        if (xCanRead && GetDefaultData()?.TryGetValue(sPropName, out var oDefVal) == true)
            if (p.PropertyType != typeof(IList<int>))
                Assert.AreEqual(oDefVal, testModel.GetProp(sPropName));
            else
                CollectionAssert.AreEqual((System.Collections.ICollection?)oDefVal, (System.Collections.ICollection?)testModel.GetProp(sPropName));
    }

    [TestMethod]
    public void MainSelectionTest()
    {
        // Arrange
        testModel.Additional1 = 1;
        testModel.Additional2 = 2;
        testModel.Additional3 = 3;
        var value = 5;
        // Act
        testModel.MainSelection = value;
        // Assert
        Assert.AreEqual(value, testModel.MainSelection);
        Assert.AreEqual(-1, testModel.Additional1);
        Assert.AreEqual(-1, testModel.Additional2);
        Assert.AreEqual(-1, testModel.Additional3);
    }

    [TestMethod]
    public void SubSelectionTest()
    {
        // Arrange
        testModel.Additional1 = 1;
        testModel.Additional2 = 2;
        testModel.Additional3 = 3;
        var value = 5;
        // Act
        testModel.SubSelection = value;
        // Assert
        Assert.AreEqual(value, testModel.SubSelection);
        Assert.AreEqual(-1, testModel.Additional1);
        Assert.AreEqual(-1, testModel.Additional2);
        Assert.AreEqual(-1, testModel.Additional3);
    }
}
