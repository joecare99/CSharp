﻿// ***********************************************************************
// Assembly         : MVVM_99_SomeIssue_netTests
// Author           : Mir
// Created          : 05-14-2023
//
// Last Modified By : Mir
// Last Modified On : 05-14-2023
// ***********************************************************************
// <copyright file="SomeIssueViewModelTests.cs" company="JC-Soft">
//     Copyright © JC-Soft 2023
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.ComponentModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using MVVM.ViewModel;
using BaseLib.Helper;
using MVVM_99_SomeIssue.Models;

/// <summary>
/// The Tests namespace.
/// </summary>
/// <autogeneratedoc />
namespace MVVM_99_SomeIssue.ViewModels.Tests;

/// <summary>
/// Defines test class SomeIssueViewModelTests.
/// </summary>
/// <autogeneratedoc />
[TestClass()]
public class SomeIssueViewModelTests : BaseTestViewModel<SomeIssueViewModel>
{
    /// <summary>The model</summary>
    private ISomeIssueModel? _model;

    /// <summary>
    /// Initializes this instance.
    /// </summary>
    /// <autogeneratedoc />
    [TestInitialize]
    public override void Init()
    {
        IoC.GetReqSrv = (t) => t switch
        {
            Type _t when _t == typeof(ISomeIssueModel) => _model ??= Substitute.For<ISomeIssueModel>(),
            _ => throw new System.NotImplementedException($"No code for {t}")
        };
        base.Init();
        _model.Now.Returns(new DateTime(2022, 08, 24, 12, 0, 0));
    }

    /// <summary>
    /// Defines the test method SetupTest.
    /// </summary>
    /// <autogeneratedoc />
    [TestMethod()]
    public void SetupTest()
    {
        Assert.IsNotNull(testModel);
        Assert.IsInstanceOfType(testModel, typeof(SomeIssueViewModel));
        Assert.IsInstanceOfType(testModel, typeof(BaseViewModelCT));
        Assert.IsInstanceOfType(testModel, typeof(INotifyPropertyChanged));
        Assert.IsNotNull(_model);
    }

    [DataTestMethod()]
    [DataRow(nameof(ISomeIssueModel.Now), new[] { "PropChg(MVVM_99_SomeIssue.ViewModels.SomeIssueViewModel,Now)=24.08.2022 12:00:00\r\n" })]
    [DataRow("HasErrors", new[] { "PropChg(MVVM_99_SomeIssue.ViewModels.SomeIssueViewModel,HasErrors)=False\r\n" })]
    [DataRow("Dummy", new[] { "PropChg(MVVM_99_SomeIssue.ViewModels.SomeIssueViewModel,Dummy)=\r\n" })]
    public void PropertyChangedTest(string prop, string[] asExp)
    {
        _model.PropertyChanged += Raise.Event<PropertyChangedEventHandler>(_model, new PropertyChangedEventArgs(prop));
        if (prop == nameof(ISomeIssueModel.Now))
            _ = _model.Received(2).Now;
        Assert.AreEqual(asExp[0], DebugLog);
    }
}
