﻿// ***********************************************************************
// Assembly         : MVVM_36_ComToolKtSavesWork_netTests
// Author           : Mir
// Created          : 05-14-2023
//
// Last Modified By : Mir
// Last Modified On : 05-14-2023
// ***********************************************************************
// <copyright file="CommunityToolkit2ViewTests.cs" company="JC-Soft">
//     Copyright © JC-Soft 2023
// </copyright>
// <summary></summary>
// ***********************************************************************
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MVVM.View.Extension;
using MVVM_36_ComToolKtSavesWork.Models;
using MVVM_36_ComToolKtSavesWork.ViewModels;
using System;
using System.Threading;
using NSubstitute;
using CommunityToolkit.Mvvm.Messaging;

/// <summary>
/// The Tests namespace.
/// </summary>
/// <autogeneratedoc />
namespace MVVM_36_ComToolKtSavesWork.Views.Tests;

/// <summary>
/// Defines test class CommunityToolkit2ViewTests.
/// </summary>
/// <autogeneratedoc />
[TestClass()]
public class CommunityToolkit2ViewTests
{
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
    /// <summary>
    /// The test view
    /// </summary>
    /// <autogeneratedoc />
    CommunityToolkit2View testView;

    private Func<Type, object?> _gsold;
    private Func<Type, object> _grsold;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
    private DateTime dtResult = new DateTime(2023,05,01);

    /// <summary>
    /// Initializes this instance.
    /// </summary>
    /// <autogeneratedoc />
    [TestInitialize]
    public void Init()
    {
        _gsold = IoC.GetSrv;
        _grsold = IoC.GetReqSrv;
        var sp = new ServiceCollection()
                .AddSingleton<CommunityToolkit2ViewModel>()
                .AddSingleton<UserInfoViewModel>()
                .AddSingleton<LoginViewModel>()
                .AddSingleton(Substitute.For<IMessenger>())
                .AddSingleton(Substitute.For<ICommunityToolkit2Model>())
                .BuildServiceProvider();
        IoC.GetReqSrv = sp.GetRequiredService;
//           testView = new();
        var t = new Thread(() => testView = new());
        t.SetApartmentState(ApartmentState.STA); //Set the thread to STA
        t.Start();
        t.Join(); //Wait for the thread to end
    }
    [TestCleanup]
    public void CleanUp()
    {
        IoC.GetSrv=_gsold;
        IoC.GetReqSrv=_grsold;
    }
    /// <summary>
    /// Defines the test method MainWindowTest.
    /// </summary>
    /// <autogeneratedoc />
    [TestMethod()]
    public void CommunityToolkit2ViewTest()
    {
        Assert.IsNotNull(testView);
        Assert.IsInstanceOfType(testView, typeof(CommunityToolkit2View));    
    }
}
