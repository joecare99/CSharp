﻿// ***********************************************************************
// Assembly         : MVVM_40_Wizzard_netTests
// Author           : Mir
// Created          : 05-14-2023
//
// Last Modified By : Mir
// Last Modified On : 05-14-2023
// ***********************************************************************
// <copyright file="WizzardViewModelTests.cs" company="JC-Soft">
//     Copyright © JC-Soft 2023
// </copyright>
// <summary></summary>
// ***********************************************************************
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel;
using MVVM.ViewModel;
using MVVM.View.Extension;
using System;
using NSubstitute;
using MVVM_40_Wizzard.Models;

/// <summary>
/// The Tests namespace.
/// </summary>
/// <autogeneratedoc />
namespace MVVM_40_Wizzard.ViewModels.Tests
{
    /// <summary>
    /// Defines test class WizzardViewModelTests.
    /// </summary>
    /// <autogeneratedoc />
    [TestClass()]
    public class WizzardViewModelTests : BaseTestViewModel<WizzardViewModel>
    {
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
            IoC.GetReqSrv = (t) => t switch
            {
                Type _t when _t == typeof(IWizzardModel) => _model ??= Substitute.For<IWizzardModel>(),
                _ => null
            };
            base.Init();
        }

        /// <summary>
        /// Defines the test method SetupTest.
        /// </summary>
        /// <autogeneratedoc />
        [TestMethod()]
        public void SetupTest()
        {
            Assert.IsNotNull(testModel);
            Assert.IsInstanceOfType(testModel, typeof(WizzardViewModel));
            Assert.IsInstanceOfType(testModel, typeof(BaseViewModelCT));
            Assert.IsInstanceOfType(testModel, typeof(INotifyPropertyChanged));
            Assert.IsNotNull(_model);
        }

        [DataTestMethod]
        [DataRow(-1, false, new[] { "" })]
        [DataRow(0, true, new[] { "" })]
        [DataRow(-1, false, new[] { "" })]
        [DataRow(0, true, new[] { "" })]
        [DataRow(1, true, new[] { "" })]
        public void Tab2EnabledTest(int iAct1, bool xExp, string[] asExp)
        {
            _model!.MainSelection = iAct1;
            Assert.AreEqual(xExp, testModel.Tab2Enabled);
            Assert.AreEqual(asExp[0], DebugLog);
        }

        [DataTestMethod]
        [DataRow(-1, -1, false, new[] { "" })]
        [DataRow(0, -1, false, new[] { "" })]
        [DataRow(-1, 0, false, new[] { "" })]
        [DataRow(0, 0, true, new[] { "" })]
        [DataRow(1, 3, true, new[] { "" })]
        public void Tab3EnabledTest(int iAct1, int iAct2, bool xExp, string[] asExp)
        {
            _model!.MainSelection = iAct1;
            _model.SubSelection = iAct2;
            Assert.AreEqual(xExp, testModel.Tab3Enabled);
            Assert.AreEqual(asExp[0], DebugLog);
        }

        [DataTestMethod]
        [DataRow(-1, -1, new[] { -1,-1,-1 }, false, new[] { "" })]
        [DataRow(0, -1, new[] { -1, -1, -1 }, false, new[] { "" })]
        [DataRow(-1, 0, new[] { -1, -1, -1 }, false, new[] { "" })]
        [DataRow(0, -1, new[] { 0, 1, 2 }, false, new[] { "" })]
        [DataRow(-1, 0, new[] { 0, 1, 2 }, false, new[] { "" })]
        [DataRow(0, 0, new[] { -1, -1, -1 }, false, new[] { "" })]
        [DataRow(1, 3, new[] {0, 1, -1 }, false, new[] { "" })]
        [DataRow(2, 4, new[] {0, -1, 2 }, false, new[] { "" })]
        [DataRow(3, 3, new[] {-1, 1, 2 }, false, new[] { "" })]
        [DataRow(4, 2, new[] {0, 1, 2 }, true, new[] { "" })]
        public void Tab4EnabledTest(int iAct1, int iAct2, int[] aiAct3, bool xExp, string[] asExp)
        {
            _model!.MainSelection = iAct1;
            _model.SubSelection = iAct2;
            _model.Additional1 = aiAct3[0];
            _model.Additional2 = aiAct3[1];
            _model.Additional3 = aiAct3[2];
            Assert.AreEqual(xExp, testModel.Tab4Enabled);
            Assert.AreEqual(asExp[0], DebugLog);
        }

        [DataTestMethod]
        [DataRow(nameof(IWizzardModel.AdditOptions), new[] { "" })]
        [DataRow(nameof(IWizzardModel.Now), new[] { "PropChg(MVVM_40_Wizzard.ViewModels.WizzardViewModel,Now)=01.01.0001 00:00:00\r\n" })]
        [DataRow(nameof(IWizzardModel.MainSelection), new[] { "PropChg(MVVM_40_Wizzard.ViewModels.WizzardViewModel,Tab2Enabled)=True\r\nPropChg(MVVM_40_Wizzard.ViewModels.WizzardViewModel,Tab3Enabled)=True\r\nCanExChanged(CommunityToolkit.Mvvm.Input.RelayCommand)=True\r\n" })]
        [DataRow(nameof(IWizzardModel.SubSelection), new[] { "PropChg(MVVM_40_Wizzard.ViewModels.WizzardViewModel,Tab3Enabled)=True\r\nCanExChanged(CommunityToolkit.Mvvm.Input.RelayCommand)=True\r\n" })]
        [DataRow(nameof(IWizzardModel.Additional1), new[] { "PropChg(MVVM_40_Wizzard.ViewModels.WizzardViewModel,Tab4Enabled)=True\r\nCanExChanged(CommunityToolkit.Mvvm.Input.RelayCommand)=True\r\n" })]
        [DataRow(nameof(IWizzardModel.Additional2), new[] { "PropChg(MVVM_40_Wizzard.ViewModels.WizzardViewModel,Tab4Enabled)=True\r\nCanExChanged(CommunityToolkit.Mvvm.Input.RelayCommand)=True\r\n" })]
        [DataRow(nameof(IWizzardModel.Additional3), new[] { "PropChg(MVVM_40_Wizzard.ViewModels.WizzardViewModel,Tab4Enabled)=True\r\nCanExChanged(CommunityToolkit.Mvvm.Input.RelayCommand)=True\r\n" })]
        public void OnMPChangedTest(string prop, string[] asExp)
        {
            _model.PropertyChanged += Raise.Event<PropertyChangedEventHandler>(_model, new PropertyChangedEventArgs(prop));
            Assert.AreEqual(asExp[0],DebugLog);
        }

        [DataTestMethod]
        public void CanPrevTabTest()
        {
            Assert.IsFalse(testModel.PrevTabCommand.CanExecute(null));
            testModel.SelectedTab = 1;
            Assert.IsTrue(testModel.PrevTabCommand.CanExecute(null));
        }

        [DataTestMethod]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        public void CanNextTabTest(int i)
        {
            _model.MainSelection = 0;
            _model.SubSelection = -1;
            Assert.IsTrue(testModel.NextTabCommand.CanExecute(null));
            testModel.SelectedTab = i;
            Assert.IsFalse(testModel.NextTabCommand.CanExecute(null));
        }

        [DataTestMethod]
        [DataRow(0,1)]
        [DataRow(1,2)]
        [DataRow(2,3)]
        public void NextTabTest(int iAct, int iExp)
        {
            testModel.SelectedTab = iAct;
            testModel.NextTabCommand.Execute(null);
            Assert.AreEqual(iExp, testModel.SelectedTab);
        }

        [DataTestMethod]
        [DataRow(0, 1)]
        [DataRow(1, 2)]
        [DataRow(2, 3)]
        public void PrevTabTest(int iExp, int iAct)
        {
            testModel.SelectedTab = iAct;
            testModel.PrevTabCommand.Execute(null);
            Assert.AreEqual(iExp, testModel.SelectedTab);
        }
    }
}
