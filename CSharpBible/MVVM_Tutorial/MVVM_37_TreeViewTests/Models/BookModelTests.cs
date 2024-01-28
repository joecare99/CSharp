﻿// ***********************************************************************
// Assembly         : MVVM_37_TreeViewTests
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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MVVM.ViewModel;
using System;
using System.ComponentModel;

/// <summary>
/// The Tests namespace.
/// </summary>
/// <autogeneratedoc />
namespace MVVM_37_TreeView.Models.Tests
{
    /// <summary>
    /// Defines test class TemplateModelTests.
    /// Implements the <see cref="BaseTestViewModel" />
    /// </summary>
    /// <seealso cref="BaseTestViewModel" />
    /// <autogeneratedoc />
    [TestClass()]
    public class BookModelTests : BaseTestViewModel
    {
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
        /// <summary>
        /// The test model
        /// </summary>
        /// <autogeneratedoc />
        Book testModel;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        [TestInitialize]
        public void Init()
        {
            testModel = new("","","",Array.Empty<int>());
            if (testModel is INotifyPropertyChanged npchgd)
                npchgd.PropertyChanged += OnVMPropertyChanged;
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
            Assert.IsInstanceOfType(testModel, typeof(Book));
        }
    }
}
