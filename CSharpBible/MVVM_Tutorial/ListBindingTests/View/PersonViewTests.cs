﻿// ***********************************************************************
// Assembly         : ListBinding_netTests
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
namespace ListBinding.View.Tests
{
    /// <summary>
    /// Defines test class PersonViewTests.
    /// </summary>
    /// <autogeneratedoc />
    [TestClass()]
    public class PersonViewTests
    {
        /// <summary>
        /// Defines the test method PersonViewTests.
        /// </summary>
        /// <autogeneratedoc />
        [TestMethod()]
        public void PersonViewTest()
        {
            PersonView? testView = null;
            var t = new Thread(() => testView = new());
            t.SetApartmentState(ApartmentState.STA); //Set the thread to STA
            t.Start();
            t.Join(); //Wait for the thread to end
            Assert.IsNotNull(testView);
            Assert.IsInstanceOfType(testView, typeof(PersonView));
        }
    }
}