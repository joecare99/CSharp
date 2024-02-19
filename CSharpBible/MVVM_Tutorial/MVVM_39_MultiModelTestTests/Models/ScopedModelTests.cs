﻿// ***********************************************************************
// Assembly         : MVVM_39_MultiModelTestTests
// Author           : Mir
// Created          : 05-19-2023
//
// Last Modified By : Mir
// Last Modified On : 05-19-2023
// ***********************************************************************
// <copyright file="ScopedModelTests.cs" company="JC-Soft">
//     Copyright © JC-Soft 2023
// </copyright>
// <summary></summary>
// ***********************************************************************
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MVVM.ViewModel;
using NSubstitute;
using System.Collections.Generic;
using System.ComponentModel;

/// <summary>
/// The Tests namespace.
/// </summary>
/// <autogeneratedoc />
namespace MVVM_39_MultiModelTest.Models.Tests
{
    /// <summary>
    /// Defines test class ScopedModelTests.
    /// Implements the <see cref="BaseTestViewModel" />
    /// </summary>
    /// <seealso cref="BaseTestViewModel" />
    /// <autogeneratedoc />
    [TestClass()]
    public class ScopedModelTests : BaseTestViewModel<ScopedModel>
    {
        private ISystemModel systemModel;

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        [TestInitialize]
        public override void Init()
        {
            systemModel = Substitute.For<ISystemModel>();
            systemModel.ScModels.Returns(new System.Collections.Generic.List<IScopedModel>());
            base.Init();
            testModel.parent = systemModel;
            ClearLog();
        }

        /// <summary>
        /// Defines the test method SetupTest.
        /// </summary>
        [TestMethod()]
        public void SetupTest()
        {
            Assert.IsNotNull(testModel);
            Assert.IsInstanceOfType(testModel, typeof(ScopedModel));
            Assert.IsInstanceOfType(testModel, typeof(ObservableObject));
            Assert.IsInstanceOfType(testModel, typeof(INotifyPropertyChanged));
        }

        protected override Dictionary<string, object?> GetDefaultData() => new() {
                { nameof(ScopedModel.Name),"ScopedModel_0" },
                { nameof(ScopedModel.Description),"ScopedModel_0 Description" },
                { nameof(ScopedModel.ICommonValue),0 },
                { nameof(ScopedModel.parent),systemModel },
        };
    }
}