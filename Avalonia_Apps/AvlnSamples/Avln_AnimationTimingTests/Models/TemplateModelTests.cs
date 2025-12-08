// ***********************************************************************
// Assembly      : Avln_AnimationTimingTests
// Author           : Mir
// Created  : 01-15-2025
//
// Last Modified By : Mir
// Last Modified On : 01-15-2025
// ***********************************************************************
// <copyright file="TemplateModelTests.cs" company="JC-Soft">
//     Copyright © JC-Soft 2025
// </copyright>
// <summary></summary>
// ***********************************************************************
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MVVM.ViewModel;
using System.ComponentModel;

namespace Avln_AnimationTiming.Models.Tests
{
    /// <summary>
    /// Defines test class TemplateModelTests.
   /// Implements the <see cref="BaseTestViewModel" />
    /// </summary>
/// <seealso cref="BaseTestViewModel" />
  [TestClass()]
    public class TemplateModelTests : BaseTestViewModel
    {
        /// <summary>
   /// The test model
        /// </summary>
     TemplateModel? testModel;

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
      Assert.IsInstanceOfType(testModel, typeof(TemplateModel));
   Assert.IsInstanceOfType(testModel, typeof(ObservableObject));
  Assert.IsInstanceOfType(testModel, typeof(INotifyPropertyChanged));
        }
    }
}
