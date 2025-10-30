// ***********************************************************************
// Assembly   : Avln_AnimationTimingTests
// Author           : Mir
// Created       : 01-15-2025
//
// Last Modified By : Mir
// Last Modified On : 01-15-2025
// ***********************************************************************
// <copyright file="MainWindowViewModelTests.cs" company="JC-Soft">
//     Copyright © JC-Soft 2025
// </copyright>
// <summary></summary>
// ***********************************************************************
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel;
using MVVM.ViewModel;

namespace Avln_AnimationTiming.ViewModels.Tests
{
    /// <summary>
    /// Defines test class MainWindowViewModelTests.
    /// </summary>
    [TestClass()]
    public class MainWindowViewModelTests : BaseTestViewModel
    {
        /// <summary>
/// The test model
    /// </summary>
        MainWindowViewModel? testModel;

     /// <summary>
        /// Initializes this instance.
        /// </summary>
        [TestInitialize]
        public void Init()
   {
    testModel = new();
            testModel.PropertyChanged += OnVMPropertyChanged;
            if (testModel is INotifyPropertyChanging npchg)
        npchg.PropertyChanging += OnVMPropertyChanging;
    ClearLog();
    }

        /// <summary>
      /// Defines the test method SetupTest.
        /// </summary>
        [TestMethod()]
    public void SetupTest()
        {
        Assert.IsNotNull(testModel);
      Assert.IsInstanceOfType(testModel, typeof(MainWindowViewModel));
  Assert.IsInstanceOfType(testModel, typeof(BaseViewModelCT));
            Assert.IsInstanceOfType(testModel, typeof(INotifyPropertyChanged));
        }
    }
}
