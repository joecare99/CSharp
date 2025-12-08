// ***********************************************************************
// Assembly   : Avln_Hello_World_Tests
// Author   : Mir
// Created   : 01-15-2025
//
// Last Modified By : Mir
// Last Modified On : 01-15-2025
// ***********************************************************************
// <copyright file="HelloWorldViewModelTests.cs" company="JC-Soft">
//Copyright © JC-Soft 2025
// </copyright>
// <summary></summary>
// ***********************************************************************
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel;
using Avalonia.ViewModels;
using Avalonia.Test;

/// <summary>
/// The Tests namespace.
/// </summary>
namespace Avln_Hello_World.ViewModels.Tests;

/// <summary>
/// Defines test class HelloWorldViewModelTests.
/// </summary>
[TestClass()]
public class HelloWorldViewModelTests : BaseTestViewModel
{
    /// <summary>
    /// The test model
    /// </summary>
    private HelloWorldViewModel testModel = null!;

    /// <summary>
    /// Initializes this instance.
    /// </summary>
    [TestInitialize]
    public void Init()
    {
        testModel = new();
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
        Assert.IsInstanceOfType(testModel, typeof(HelloWorldViewModel));
        Assert.IsInstanceOfType(testModel, typeof(BaseViewModelCT));
        Assert.IsInstanceOfType(testModel, typeof(INotifyPropertyChanged));
    }
}
