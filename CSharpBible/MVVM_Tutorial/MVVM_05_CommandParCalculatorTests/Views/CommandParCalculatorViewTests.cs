﻿// ***********************************************************************
// Assembly         : MVVM_05_CommandParCalculator_netTests
// Author           : Mir
// Created          : 05-11-2023
//
// Last Modified By : Mir
// Last Modified On : 05-02-2023
// ***********************************************************************
// <copyright file="CommandParCalculatorViewTests.cs" company="JC-Soft">
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
/// Defines test class CommandParCalculatorViewTests.
/// </summary>
/// <autogeneratedoc />
[TestClass()]
public class CommandParCalculatorViewTests
{
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
    /// <summary>
    /// The test view
    /// </summary>
    /// <autogeneratedoc />
    CommandParCalculatorView testView;
    /// <summary>
    /// The vm
    /// </summary>
    /// <autogeneratedoc />
    private object vm;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.

    /// <summary>
    /// Initializes this instance.
    /// </summary>
    /// <autogeneratedoc />
    [TestInitialize()]
    public void Init()
    {
        Thread thread = new(() =>
        {
            testView = new();
            vm = testView.DataContext;
        });
        thread.SetApartmentState(ApartmentState.STA); //Set the thread to STA
        thread.Start();
        thread.Join(); //Wait for the thread to end
    }

    /// <summary>
    /// Defines the test method ValidationPageTest.
    /// </summary>
    /// <autogeneratedoc />
    [TestMethod()]
    public void ValidationPageTest()
    {
        Assert.IsNotNull(testView);
        Assert.IsNotNull(vm);
    }
}