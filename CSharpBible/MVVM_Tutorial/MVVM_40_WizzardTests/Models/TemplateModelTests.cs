﻿// ***********************************************************************
// Assembly         : MVVM_40_WizzardTests
// Author           : Mir
// Created          : 05-19-2023
//
// Last Modified By : Mir
// Last Modified On : 05-19-2023
// ***********************************************************************
// <copyright file="TemplateModelTests.cs" company="JC-Soft">
//     Copyright © JC-Soft 2023
// </copyright>
// <summary></summary>
// ***********************************************************************
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MVVM.ViewModel;
using System.ComponentModel;

/// <summary>
/// The Tests namespace.
/// </summary>
/// <autogeneratedoc />
namespace MVVM_40_Wizzard.Models.Tests
{
    /// <summary>
    /// Defines test class TemplateModelTests.
    /// Implements the <see cref="BaseTestViewModel" />
    /// </summary>
    /// <seealso cref="BaseTestViewModel" />
    /// <autogeneratedoc />
    [TestClass()]
    public class TemplateModelTests : BaseTestViewModel
    {
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
        /// <summary>
        /// The test model
        /// </summary>
        /// <autogeneratedoc />
        WizzardModel testModel;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        [TestInitialize]
        public void Init()
        {
            testModel = new();
            testModel.PropertyChanged += OnVMPropertyChanged;
            if (testModel is INotifyPropertyChanging npchgn)
                npchgn.PropertyChanging += OnVMPropertyChanging;
            ClearLog();
        }

        /// <summary>
        /// Defines the test method SetupTest.
        /// </summary>
        [TestMethod()]
        public void SetupTest()
        {
            Assert.IsNotNull(testModel);
            Assert.IsInstanceOfType(testModel, typeof(WizzardModel));
            Assert.IsInstanceOfType(testModel, typeof(ObservableObject));
            Assert.IsInstanceOfType(testModel, typeof(INotifyPropertyChanged));
        }
    }
}
