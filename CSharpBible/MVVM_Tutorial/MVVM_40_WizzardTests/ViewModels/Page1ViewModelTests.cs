﻿// ***********************************************************************
// Assembly         : MVVM_40_Wizzard_netTests
// Author           : Mir
// Created          : 05-14-2023
//
// Last Modified By : Mir
// Last Modified On : 05-14-2023
// ***********************************************************************
// <copyright file="Page1ViewModelTests.cs" company="JC-Soft">
//     Copyright © JC-Soft 2023
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using MVVM.ViewModel;
using BaseLib.Helper;
using MVVM_40_Wizzard.Models;
using MVVM_40_Wizzard.Properties;
using MVVM_40_Wizzard.Models.Interfaces;
using static BaseLib.Helper.TestHelper;

/// <summary>
/// The Tests namespace.
/// </summary>
/// <autogeneratedoc />
namespace MVVM_40_Wizzard.ViewModels.Tests;

/// <summary>
/// Defines test class Page1ViewModelTests.
/// </summary>
/// <autogeneratedoc />
[TestClass()]
public class Page1ViewModelTests:BaseTestViewModel<Page1ViewModel>
{
    private const string csMainSel = "MainSelection{0}";
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
    private IWizzardModel? _model;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.

    /// <summary>
    /// Initializes this instance.
    /// </summary>
    /// <autogeneratedoc />
    [TestInitialize]
    public override void Init()
    {
        IoC.GetReqSrv=(t)=>t switch
        {
            Type _t when _t == typeof(IWizzardModel) => _model ??= Substitute.For<IWizzardModel>(),
            _ => null
        };
        base.Init();
        _model!.MainOptions.Returns(new List<int> { 0, 1, 3 });
    }

    /// <summary>
    /// Defines the test method SetupTest.
    /// </summary>
    /// <autogeneratedoc />
    [TestMethod()]
    public void SetupTest()
    {
        Assert.IsNotNull(testModel);
        Assert.IsInstanceOfType(testModel, typeof(Page1ViewModel));
        Assert.IsInstanceOfType(testModel, typeof(BaseViewModelCT));
        Assert.IsInstanceOfType(testModel, typeof(INotifyPropertyChanged));
        Assert.IsNotNull(_model);
    }

    [TestMethod()]
    [DataRow(2, new[] { "" })]
    [DataRow(0, new[] { "" })]
    public void SetMainSelectionTest(int iAct, string[] asExp)
    {
        testModel.MainSelection = new ListEntry(iAct, "Test");
        Assert.AreEqual(iAct, _model.MainSelection);
        Assert.AreEqual(asExp[0], DebugLog);
    }

    [DataTestMethod()]
    [DataRow(new[] { 0, 1, 3 })]
    public void MainOptionsTest(int[] aiAct)
    {
        var lst = aiAct.Select(i=> new ListEntry(i, Resources.ResourceManager.GetString(csMainSel.Format(i)))).ToList();
        AssertAreEqual(lst, testModel.MainOptions);
    }

    [TestMethod()]
    [DataRow(2, new[] { "" })]
    [DataRow(0, new[] { "" })]
    public void ClearTest(int iAct, string[] asExp)
    {
        testModel.MainSelection = new ListEntry(iAct, "Test");
        testModel.ClearCommand.Execute(null);
        Assert.AreEqual(null, testModel.MainSelection);
        Assert.AreEqual(asExp[0], DebugLog);
    }

    [DataTestMethod]
    [DataRow(nameof(IWizzardModel.AdditOptions), new[] { "" })]
    [DataRow(nameof(IWizzardModel.Now), new[] { "" })]
    [DataRow(nameof(IWizzardModel.MainSelection), new[] { "PropChg(MVVM_40_Wizzard.ViewModels.Page1ViewModel,MainSelection)=1. Entry\r\n" })]
    [DataRow(nameof(IWizzardModel.MainOptions), new[] { "PropChg(MVVM_40_Wizzard.ViewModels.Page1ViewModel,MainOptions)=System.Collections.Generic.List`1[MVVM_40_Wizzard.Models.ListEntry]\r\n" })]
    [DataRow(nameof(IWizzardModel.SubSelection), new[] { "" })]
    [DataRow(nameof(IWizzardModel.Additional1), new[] { "" })]
    [DataRow(nameof(IWizzardModel.Additional2), new[] { "" })]
    public void OnMPChangedTest(string prop, string[] asExp)
    {
        _model.PropertyChanged += Raise.Event<PropertyChangedEventHandler>(_model, new PropertyChangedEventArgs(prop));
        Assert.AreEqual(asExp[0], DebugLog);
    }

}
