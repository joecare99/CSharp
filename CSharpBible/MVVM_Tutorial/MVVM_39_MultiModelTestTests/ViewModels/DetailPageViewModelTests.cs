// ***********************************************************************
// Assembly         : MVVM_39_MultiModelTest_netTests
// Author           : Mir
// Created          : 02-18-2024
//
// Last Modified By : Mir
// Last Modified On : 02-18-2024
// ***********************************************************************
// <copyright file="DetailPageViewModelTests.cs" company="JC-Soft">
//     Copyright © JC-Soft 2023
// </copyright>
// <summary></summary>
// ***********************************************************************
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MVVM.ViewModel;
using MVVM.View.Extension;
using MVVM_39_MultiModelTest.Models;
using System;
using NSubstitute;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// The Tests namespace.
/// </summary>
namespace MVVM_39_MultiModelTest.ViewModels.Tests
{
    /// <summary>
    /// Defines test class DetailPageViewModelTests.
    /// Implements the <see cref="MVVM.ViewModel.BaseTestViewModel{MVVM_39_MultiModelTest.ViewModels.DetailPageViewModel}" />
    /// </summary>
    /// <seealso cref="MVVM.ViewModel.BaseTestViewModel{MVVM_39_MultiModelTest.ViewModels.DetailPageViewModel}" />
    [TestClass()]
    public class DetailPageViewModelTests:BaseTestViewModel<DetailPageViewModel>
    {
        /// <summary>
        /// The scoped model
        /// </summary>
        private IScopedModel scopedModel;

        /// <summary>
        /// Initializes the test-models for this instance.
        /// </summary>
        [TestInitialize]
        public override void Init()
        {
            scopedModel = Substitute.For<IScopedModel>();
            IoC.GetReqSrv = (t) => t switch
            {
                _ when t == typeof(IScopedModel) => scopedModel,
                _ => throw new System.NotImplementedException()
            };
            base.Init();
        }

        /// <summary>
        /// Defines the test method SetupTest.
        /// </summary>
        [TestMethod()]
        public void SetupTest()
        {
            Assert.IsNotNull(testModel);
            Assert.IsNotNull(testModel2);
            Assert.IsInstanceOfType(testModel, typeof(DetailPageViewModel));
            Assert.IsInstanceOfType(testModel, typeof(INotifyPropertyChanged));
            Assert.IsInstanceOfType(testModel, typeof(INotifyPropertyChanging));
        }
    }
}