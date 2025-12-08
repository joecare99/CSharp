// ***********************************************************************
// Assembly         : MVVM_39_MultiModelTest_netTests
// Author           : Mir
// Created          : 02-18-2024
//
// Last Modified By : Mir
// Last Modified On : 02-18-2024
// ***********************************************************************
// <copyright file="DetailPageViewModelTests.cs" company="JC-Soft">
//     Copyright © JC-Soft 2023
// </copyright>
// <summary></summary>
// ***********************************************************************
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MVVM.ViewModel;
using BaseLib.Helper;
using MVVM_39_MultiModelTest.Models;
using NSubstitute;
using System.ComponentModel;

/// <summary>
/// The Tests namespace.
/// </summary>
namespace MVVM_39_MultiModelTest.ViewModels.Tests;

/// <summary>
/// Defines test class DetailPageViewModelTests.
/// Implements the <see cref="MVVM.ViewModel.BaseTestViewModel{MVVM_39_MultiModelTest.ViewModels.DetailPageViewModel}" />
/// </summary>
/// <seealso cref="MVVM.ViewModel.BaseTestViewModel{MVVM_39_MultiModelTest.ViewModels.DetailPageViewModel}" />
[TestClass()]
public class DetailPageViewModelTests:BaseTestViewModel<DetailPageViewModel>
{
    /// <summary>
    /// The scoped model
    /// </summary>
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
    private IScopedModel scopedModel;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.

    /// <summary>
    /// Initializes the test-models for this instance.
    /// </summary>
    [TestInitialize]
    public override void Init()
    {
        scopedModel = Substitute.For<IScopedModel>();
        IoC.GetReqSrv = (t) => t switch
        {
            _ when t == typeof(IScopedModel) => scopedModel,
            _ => throw new System.NotImplementedException()
        };
        base.Init();
    }

    /// <summary>
    /// Defines the test method SetupTest.
    /// </summary>
    [TestMethod()]
    public void SetupTest()
    {
        Assert.IsNotNull(testModel);
        Assert.IsNotNull(testModel2);
        Assert.IsInstanceOfType(testModel, typeof(DetailPageViewModel));
        Assert.IsInstanceOfType(testModel, typeof(INotifyPropertyChanged));
        Assert.IsInstanceOfType(testModel, typeof(INotifyPropertyChanging));
    }
}