﻿// ***********************************************************************
// Assembly         : MVVM_40_Wizzard_netTests
// Author           : Mir
// Created          : 05-14-2023
//
// Last Modified By : Mir
// Last Modified On : 05-19-2023
// ***********************************************************************
// <copyright file="MainWindowViewModelTests.cs" company="JC-Soft">
//     Copyright © JC-Soft 2023
// </copyright>
// <summary></summary>
// ***********************************************************************
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel;
using MVVM.ViewModel;

/// <summary>
/// The Tests namespace.
/// </summary>
/// <autogeneratedoc />
namespace MVVM_40_Wizzard.ViewModels.Tests
{
    /// <summary>
    /// Defines test class MainWindowViewModelTests.
    /// </summary>
    /// <autogeneratedoc />
    [TestClass()]
    public class MainWindowViewModelTests : BaseTestViewModel<MainWindowViewModel>
    {

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        /// <autogeneratedoc />
        [TestInitialize]
        public override void Init()
        {
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
            Assert.IsInstanceOfType(testModel, typeof(MainWindowViewModel));
            Assert.IsInstanceOfType(testModel, typeof(BaseViewModelCT));
            Assert.IsInstanceOfType(testModel, typeof(INotifyPropertyChanged));
        }
    }
}
