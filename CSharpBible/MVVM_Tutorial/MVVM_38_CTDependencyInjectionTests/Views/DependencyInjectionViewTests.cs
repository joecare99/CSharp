﻿// ***********************************************************************
// Assembly         : MVVM_38_CTDependencyInjection_netTests
// Author           : Mir
// Created          : 05-14-2023
//
// Last Modified By : Mir
// Last Modified On : 05-14-2023
// ***********************************************************************
// <copyright file="TemplateViewTests.cs" company="JC-Soft">
//     Copyright © JC-Soft 2023
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using BaseLib.Helper;
using MVVM_38_CTDependencyInjection.Models.Interfaces;

/// <summary>
/// The Tests namespace.
/// </summary>
/// <autogeneratedoc />
namespace MVVM_38_CTDependencyInjection.Views.Tests;

/// <summary>
/// Defines test class TemplateViewTests.
/// </summary>
/// <autogeneratedoc />
[TestClass()]
public class DependencyInjectionViewTests
{
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
    /// <summary>
    /// The test view
    /// </summary>
    /// <autogeneratedoc />
    DependencyInjectionView testView;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.

    /// <summary>
    /// Initializes this instance.
    /// </summary>
    /// <autogeneratedoc />
    [TestInitialize]
    public void Init()
    {
        IoC.GetReqSrv = (t) => t switch {
            _ when t == typeof(ITemplateModel) => Substitute.For<ITemplateModel>(),
            _ => throw new ArgumentException()
        };
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
    public void DependencyInjectionViewTest()
    {
        Assert.IsNotNull(testView);
        Assert.IsInstanceOfType(testView, typeof(DependencyInjectionView));    
    }
}
