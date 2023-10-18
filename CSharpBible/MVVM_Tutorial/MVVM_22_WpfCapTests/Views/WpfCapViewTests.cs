﻿// ***********************************************************************
// Assembly         : MVVM_00a_CTTemplate_netTests
// Author           : Mir
// Created          : 05-14-2023
//
// Last Modified By : Mir
// Last Modified On : 05-14-2023
// ***********************************************************************
// <copyright file="TemplateViewTests.cs" company="JC-Soft">
//     Copyright © JC-Soft 2023
// </copyright>
// <summary></summary>
// ***********************************************************************
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MVVM.View.Extension;
using System;
using System.Threading;

/// <summary>
/// The Tests namespace.
/// </summary>
/// <autogeneratedoc />
namespace MVVM_22_WpfCap.View.Tests
{
    /// <summary>
    /// Defines test class TemplateViewTests.
    /// </summary>
    /// <autogeneratedoc />
    [TestClass()]
    public class WpfCapViewTests
    {
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
        /// <summary>
        /// The test view
        /// </summary>
        /// <autogeneratedoc />
        WpfCapView testView;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        /// <autogeneratedoc />
        [TestInitialize]
        public void Init()
        {
            IoC.GetReqSrv = GetMyReqSrv;
            var t = new Thread(() => testView = new());
            t.SetApartmentState(ApartmentState.STA); //Set the thread to STA
            t.Start();
            t.Join(); //Wait for the thread to end
        }

        private object GetMyReqSrv(Type type)
        {
            return null!;
        }

        /// <summary>
        /// Defines the test method MainWindowTest.
        /// </summary>
        /// <autogeneratedoc />
        [TestMethod()]
        public void WpfCapViewTest()
        {
            Assert.IsNotNull(testView);
            Assert.IsInstanceOfType(testView, typeof(WpfCapView));    
        }
    }
}
