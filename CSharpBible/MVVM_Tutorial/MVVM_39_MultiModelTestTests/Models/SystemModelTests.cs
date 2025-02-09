﻿// ***********************************************************************
// Assembly         : MVVM_39_MultiModelTestTests
// Author           : Mir
// Created          : 05-19-2023
//
// Last Modified By : Mir
// Last Modified On : 05-19-2023
// ***********************************************************************
// <copyright file="SystemModelTests.cs" company="JC-Soft">
//     Copyright © JC-Soft 2023
// </copyright>
// <summary></summary>
// ***********************************************************************
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MVVM.ViewModel;
using System.ComponentModel;

/// <summary>
/// The Tests namespace.
/// </summary>
/// <autogeneratedoc />
namespace MVVM_39_MultiModelTest.Models.Tests;

/// <summary>
/// Defines test class SystemModelTests.
/// Implements the <see cref="BaseTestViewModel" />
/// </summary>
/// <seealso cref="BaseTestViewModel" />
/// <autogeneratedoc />
[TestClass()]
public class SystemModelTests : BaseTestViewModel<SystemModel>
{

    /// <summary>
    /// Initializes this instance.
    /// </summary>
    [TestInitialize]
    public override void Init()
    {

        base.Init();
        ClearLog();
    }

    /// <summary>
    /// Defines the test method SetupTest.
    /// </summary>
    [TestMethod()]
    public void SetupTest()
    {
        Assert.IsNotNull(testModel);
        Assert.IsInstanceOfType(testModel, typeof(SystemModel));
        Assert.IsInstanceOfType(testModel, typeof(ObservableObject));
        Assert.IsInstanceOfType(testModel, typeof(INotifyPropertyChanged));
    }
}
