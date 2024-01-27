﻿// ***********************************************************************
// Assembly         : ItemsControlTut3_netTests
// Author           : Mir
// Created          : 05-13-2023
//
// Last Modified By : Mir
// Last Modified On : 05-13-2023
// ***********************************************************************
// <copyright file="MainWindowTests.cs" company="JC-Soft">
//     Copyright © JC-Soft 2023
// </copyright>
// <summary></summary>
// ***********************************************************************
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;

/// <summary>
/// The Tests namespace.
/// </summary>
/// <autogeneratedoc />
namespace ItemsControlTut3.View.Tests
{
    /// <summary>
    /// Defines test class MainWindowTests.
    /// </summary>
    /// <autogeneratedoc />
    [TestClass()]
    public class MainWindowTests
    {
        /// <summary>
        /// Defines the test method MainWindowTest.
        /// </summary>
        /// <autogeneratedoc />
        [TestMethod()]
        public void MainWindowTest()
        {
            MainWindow? testView = null;
            var t = new Thread(() => testView = new());
            t.SetApartmentState(ApartmentState.STA); //Set the thread to STA
            t.Start();
            t.Join(); //Wait for the thread to end
            Assert.IsNotNull(testView);
            Assert.IsInstanceOfType(testView, typeof(MainWindow));
        }
    }
}