﻿// ***********************************************************************
// Assembly         : MVVM_40_Wizzard_netTests
// Author           : Mir
// Created          : 05-14-2023
//
// Last Modified By : Mir
// Last Modified On : 05-14-2023
// ***********************************************************************
// <copyright file="Page3ViewTests.cs" company="JC-Soft">
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
namespace MVVM_40_Wizzard.Views.Tests;

/// <summary>
/// Defines test class Page3ViewTests.
/// </summary>
/// <autogeneratedoc />
[TestClass()]
public class Page3ViewTests
{
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
    /// <summary>
    /// The test view
    /// </summary>
    /// <autogeneratedoc />
    Page3View testView;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.

    /// <summary>
    /// Initializes this instance.
    /// </summary>
    /// <autogeneratedoc />
    [TestInitialize]
    public void Init()
    {
        var t = new Thread(() => testView = new());
        t.SetApartmentState(ApartmentState.STA); //Set the thread to STA
        t.Start();
        t.Join(); //Wait for the thread to end
    }

    /// <summary>
    /// Defines the test method MainWindowTest.
    /// </summary>
    /// <autogeneratedoc />
    [TestMethod()]
    public void MainWindowTest()
    {
        Assert.IsNotNull(testView);
        Assert.IsInstanceOfType(testView, typeof(Page3View));    
    }
}
