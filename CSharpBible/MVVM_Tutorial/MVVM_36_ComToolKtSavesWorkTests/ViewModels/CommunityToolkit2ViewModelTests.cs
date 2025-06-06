﻿// ***********************************************************************
// Assembly         : MVVM_36_ComToolKtSavesWork_netTests
// Author           : Mir
// Created          : 05-14-2023
//
// Last Modified By : Mir
// Last Modified On : 05-14-2023
// ***********************************************************************
// <copyright file="CommunityToolkit2ViewModelTests.cs" company="JC-Soft">
//     Copyright © JC-Soft 2023
// </copyright>
// <summary></summary>
// ***********************************************************************
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel;
using MVVM.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using MVVM_36_ComToolKtSavesWork.Models;
using System;
using BaseLib.Helper;
using CommunityToolkit.Mvvm.Messaging;
using BaseLib.Helper.MVVM;
using MVVM.ViewModel.Tests;
using System.Collections.Generic;

/// <summary>
/// The Tests namespace.
/// </summary>
/// <autogeneratedoc />
namespace MVVM_36_ComToolKtSavesWork.ViewModels.Tests;

/// <summary>
/// Defines test class CommunityToolkit2ViewModelTests.
/// </summary>
/// <autogeneratedoc />
[TestClass()]
public class CommunityToolkit2ViewModelTests : BaseTestViewModel<CommunityToolkit2ViewModel>
{
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
    private IDebugLog _debugLog;
    private IGetResult _getResult;
    private Func<Type, object?> _gsold;
    private Func<Type, object> _grsold;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
    DateTime dtResult = new DateTime(2023, 04, 30);

    /// <summary>
    /// Initializes this instance.
    /// </summary>
    /// <autogeneratedoc />
    [TestInitialize]
    public override void Init()
    {
        _gsold = IoC.GetSrv;
        _grsold = IoC.GetReqSrv;
        var sp = new ServiceCollection()
                .AddSingleton<IUserRepository, TestUserRepository>()
                .AddSingleton<ICommunityToolkit2Model, TestUserModel>()
                .AddSingleton<IDebugLog, DebugLog>()
                .AddSingleton<IGetResult, GetResult>()
                .BuildServiceProvider();
        IoC.GetReqSrv = t => sp.GetRequiredService(t);
        IoC.GetSrv = t => sp.GetService(t);
        _debugLog = IoC.GetRequiredService<IDebugLog>();
        _getResult = IoC.GetRequiredService<IGetResult>();
        _getResult.Register("Now", o=>dtResult);
        base.Init();
    }

    [TestCleanup]
    public void Cleanup()
    {
        testModel.Dispose();
        testModel2.Dispose();
        GC.Collect();
        IoC.GetReqSrv = _grsold;
        IoC.GetSrv = _gsold;
    }

    /// <summary>
    /// Defines the test method SetupTest.
    /// </summary>
    /// <autogeneratedoc />
    [TestMethod()]
    public void SetupTest()
    {
        Assert.IsNotNull(testModel);
        Assert.IsNotNull(testModel2);
        Assert.IsInstanceOfType(testModel, typeof(CommunityToolkit2ViewModel));
        Assert.IsInstanceOfType(testModel, typeof(BaseViewModelCT));
        Assert.IsInstanceOfType(testModel, typeof(INotifyPropertyChanged));
        Assert.IsInstanceOfType(_debugLog, typeof(IDebugLog));
        Assert.IsInstanceOfType(_getResult, typeof(IGetResult));
    }

    [TestMethod]
    public void ShowLoginTest()
    {
        Assert.AreEqual(false, testModel.ShowLogin);
        if (testModel is IRecipient<ShowLoginMessage> rslm)
            rslm.Receive(new ShowLoginMessage());
        Assert.AreEqual(true, testModel.ShowLogin);
        Assert.AreEqual("PropChgn(MVVM_36_ComToolKtSavesWork.ViewModels.CommunityToolkit2ViewModel,ShowLogin)=False\r\nPropChg(MVVM_36_ComToolKtSavesWork.ViewModels.CommunityToolkit2ViewModel,ShowLogin)=True\r\n", DebugLog);
        ClearLog();
        testModel.Receive(new CommunityToolkit.Mvvm.Messaging.Messages.ValueChangedMessage<User>(new User()));
        Assert.AreEqual(false, testModel.ShowLogin);
        Assert.AreEqual("PropChgn(MVVM_36_ComToolKtSavesWork.ViewModels.CommunityToolkit2ViewModel,ShowLogin)=True\r\nPropChg(MVVM_36_ComToolKtSavesWork.ViewModels.CommunityToolkit2ViewModel,ShowLogin)=False\r\n", DebugLog);
    }

    [TestMethod]
    public void OnMPropertyChangedTest()
    {
        var m = IoC.GetService<ICommunityToolkit2Model>();
        if (m is IRaisePropChangedEvents irpce)
            irpce.RaisePropertyChanged("Now");
        Assert.AreEqual("PropChg(MVVM_36_ComToolKtSavesWork.ViewModels.CommunityToolkit2ViewModel,Now)=30.04.2023 00:00:00\r\n", DebugLog);
    }

    protected override Dictionary<string, object?> GetDefaultData() => new() {
        { nameof(CommunityToolkit2ViewModel.HasErrors),false},
        { nameof(CommunityToolkit2ViewModel.Now),dtResult},
        { nameof(CommunityToolkit2ViewModel.ShowLogin),false},
    };
}
