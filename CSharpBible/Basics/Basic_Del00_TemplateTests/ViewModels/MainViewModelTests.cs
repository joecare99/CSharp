﻿// ***********************************************************************
// Assembly         : Basic_Del00_Template_netTests
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

/// <summary>
/// The Tests namespace.
/// </summary>
/// <autogeneratedoc />
namespace Basic_Del00_Template.ViewModels.Tests
{
    /// <summary>
    /// Defines test class MainWindowViewModelTests.
    /// </summary>
    /// <autogeneratedoc />
    [TestClass()]
    public class MainViewModelTests //: BaseTestViewModel
    {
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
        /// <summary>
        /// The test model
        /// </summary>
        /// <autogeneratedoc />
        MainViewModel testModel;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        /// <autogeneratedoc />
        [TestInitialize]
        public void Init()
        {
            testModel = new();
        }

        /// <summary>
        /// Defines the test method SetupTest.
        /// </summary>
        /// <autogeneratedoc />
        [TestMethod()]
        public void SetupTest()
        {
            Assert.IsNotNull(testModel);
            Assert.IsInstanceOfType(testModel, typeof(MainViewModel));
//            Assert.IsInstanceOfType(testModel, typeof(BaseViewModel));
//            Assert.IsInstanceOfType(testModel, typeof(INotifyPropertyChanged));
        }
    }
}
