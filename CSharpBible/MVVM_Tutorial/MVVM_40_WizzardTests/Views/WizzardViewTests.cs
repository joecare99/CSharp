﻿// ***********************************************************************
// Assembly         : MVVM_40_Wizzard_netTests
// Author           : Mir
// Created          : 05-14-2023
//
// Last Modified By : Mir
// Last Modified On : 05-14-2023
// ***********************************************************************
// <copyright file="WizzardViewTests.cs" company="JC-Soft">
//     Copyright © JC-Soft 2023
// </copyright>
// <summary></summary>
// ***********************************************************************
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BaseLib.Helper;
using MVVM_40_Wizzard.Models.Interfaces;
using NSubstitute;
using System;
using System.Threading;

/// <summary>
/// The Tests namespace.
/// </summary>
/// <autogeneratedoc />
namespace MVVM_40_Wizzard.Views.Tests;

/// <summary>
/// Defines test class WizzardViewTests.
/// </summary>
/// <autogeneratedoc />
[TestClass()]
public class WizzardViewTests
{
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
    /// <summary>
    /// The test view
    /// </summary>
    /// <autogeneratedoc />
    WizzardView testView;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.

    /// <summary>
    /// Initializes this instance.
    /// </summary>
    /// <autogeneratedoc />
    [TestInitialize]
    public void Init()
    {
       IoC.GetReqSrv = (t) => t switch {
           _ when t == typeof(IWizzardModel) => Substitute.For<IWizzardModel>(),
           _ when t == typeof(IMessenger) => Substitute.For<IMessenger>(),
           _ => throw new ArgumentException($"No implementation for {t}")};
    }

    /// <summary>
    /// Defines the test method WizzardViewTest.
    /// </summary>
    /// <autogeneratedoc />
    [TestMethod()]
    public void WizzardViewTest()
    {
        var t = new Thread(() =>
        {
            testView = new();
            Assert.IsNotNull(testView);
            Assert.IsInstanceOfType(testView, typeof(WizzardView));
        });
        t.SetApartmentState(ApartmentState.STA); //Set the thread to STA
        t.Start();
        t.Join(); //Wait for the thread to end
    }
}
