﻿// ***********************************************************************
// Assembly         : ListBindingTests
// Author           : Mir
// Created          : 06-17-2022
//
// Last Modified By : Mir
// Last Modified On : 06-17-2022
// ***********************************************************************
// <copyright file="PersonViewViewModelTests.cs" company="Hewlett-Packard Company">
//     Copyright © Hewlett-Packard Company 2022
// </copyright>
// <summary></summary>
// ***********************************************************************
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ListBinding.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListBinding.ViewModel.Tests
{
    /// <summary>
    /// Defines test class PersonViewViewModelTests.
    /// </summary>
    [TestClass()]
    public class PersonViewViewModelTests
    {
        /// <summary>
        /// Defines the test method PersonViewViewModelTest.
        /// </summary>
        [TestMethod()]
        public void PersonViewViewModelTest()
        {
            var model = new PersonViewViewModel();
            Assert.IsNotNull(model.NewPerson);
            Assert.IsNotNull(model.Persons);
            Assert.AreEqual(0, model.Persons.Count);
            Assert.IsTrue(model.NewPerson.IsEmpty);
        }
    }
}