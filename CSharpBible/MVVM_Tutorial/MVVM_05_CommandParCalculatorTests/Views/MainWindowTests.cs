﻿// ***********************************************************************
// Assembly         : MVVM_05_CommandParCalculator_netTests
// Author           : Mir
// Created          : 05-11-2023
//
// Last Modified By : Mir
// Last Modified On : 05-02-2023
// ***********************************************************************
// <copyright file="MainWindowTests.cs" company="JC-Soft">
//     Copyright © JC-Soft 2023
// </copyright>
// <summary></summary>
// ***********************************************************************
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;

/// <summary>
/// The Tests namespace.
/// </summary>
/// <autogeneratedoc />
namespace MVVM_05_CommandParCalculator.Views.Tests;

/// <summary>
/// Defines test class MainWindowTests.
/// </summary>
/// <autogeneratedoc />
[TestClass()]
public class MainWindowTests
{
    /// <summary>
    /// Defines the test method MainWindowTest.
    /// </summary>
    /// <autogeneratedoc />
    [TestMethod()]
    public void MainWindowTest()
    {
        MainWindow? mw=null;
        var t = new Thread(()=> mw = new());
        t.SetApartmentState(ApartmentState.STA); //Set the thread to STA
        t.Start();
        t.Join(); //Wait for the thread to end
        Assert.IsNotNull(mw);
        Assert.IsInstanceOfType(mw, typeof(MainWindow));    
    }
}