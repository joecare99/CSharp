// ***********************************************************************
// Assembly : Avln_AnimationTimingTests
// Author    : Mir
// Created      : 01-15-2025
//
// Last Modified By : Mir
// Last Modified On : 01-15-2025
// ***********************************************************************
// <copyright file="TemplateViewModelTests.cs" company="JC-Soft">
//   Copyright © JC-Soft 2025
// </copyright>
// <summary></summary>
// ***********************************************************************
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel;
using MVVM.ViewModel;

namespace Avln_AnimationTiming.ViewModels.Tests
{
    /// <summary>
  /// Defines test class TemplateViewModelTests.
    /// </summary>
    [TestClass()]
    public class TemplateViewModelTests : BaseTestViewModel
    {
   /// <summary>
        /// The test model
    /// </summary>
        TemplateViewModel? testModel;

        /// <summary>
        /// Initializes this instance.
     /// </summary>
   [TestInitialize]
   public void Init()
     {
     testModel = new();
  testModel.PropertyChanged += OnVMPropertyChanged;
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
   Assert.IsInstanceOfType(testModel, typeof(TemplateViewModel));
            Assert.IsInstanceOfType(testModel, typeof(BaseViewModelCT));
       Assert.IsInstanceOfType(testModel, typeof(INotifyPropertyChanged));
        }
    }
}
