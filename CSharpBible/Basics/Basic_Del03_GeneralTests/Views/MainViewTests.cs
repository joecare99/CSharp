﻿// ***********************************************************************
// Assembly         : Basic_Del03_General_netTests
// Author           : Mir
// Created          : 05-14-2023
//
// Last Modified By : Mir
// Last Modified On : 05-14-2023
// ***********************************************************************
// <copyright file="MainWindowTests.cs" company="JC-Soft">
//     Copyright © JC-Soft 2023
// </copyright>
// <summary></summary>
// ***********************************************************************
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;
using BaseLib.Model.Tests;

/// <summary>
/// The Tests namespace.
/// </summary>
/// <autogeneratedoc />
namespace Basic_Del03_General.Views.Tests
{
    /// <summary>
    /// Defines test class MainWindowTests.
    /// </summary>
    /// <autogeneratedoc />
    [TestClass()]
    public class MainViewTests : BaseTest
    {
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
        /// <summary>
        /// The test view
        /// </summary>
        /// <autogeneratedoc />
        MainView testView;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        /// <autogeneratedoc />
        [TestInitialize]
        public void Init()
        {
            var t = new Thread(() => testView = new());
            t.SetApartmentState(ApartmentState.STA); //Set the thread to STA
            t.Start();
            t.Join(); //Wait for the thread to end
            testView.console = new TestConsole(DoLog);
            ClearLog();
        }

        /// <summary>
        /// Defines the test method MainWindowTest.
        /// </summary>
        /// <autogeneratedoc />
        [TestMethod()]
        public void MainWindowTest()
        {
            Assert.IsNotNull(testView);
            Assert.IsInstanceOfType(testView, typeof(MainView));    
        }

        /// <summary>
        /// Defines the test method MainWindowTest.
        /// </summary>
        /// <autogeneratedoc />
        [TestMethod()]
        public void CanExecuteTest()
        {
            Assert.IsTrue(testView.CanExecute(null!));
        }

        /// <summary>
        /// Defines the test method MainWindowTest.
        /// </summary>
        /// <autogeneratedoc />
        [TestMethod()]
        public void ExecuteTest()
        {
            testView.Execute(null!);
            Assert.AreEqual(@"WriteLine(Hello World !)
", DebugLog);
        }

    }
}
