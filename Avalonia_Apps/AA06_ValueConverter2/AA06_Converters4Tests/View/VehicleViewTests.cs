// ***********************************************************************
// Assembly         : AA06_Converters_4Tests
// Author           : Mir
// Created          : 05-11-2023
//
// Last Modified By : Mir
// Last Modified On : 01-28-2024
// ***********************************************************************
// <copyright file="VehicleViewTests.cs" company="JC-Soft">
//     Copyright © JC-Soft 2023
// </copyright>
// <summary></summary>
// ***********************************************************************

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using AA06_Converters_4.Models.Interfaces;
using AA06_Converters_4.ViewModels;
using AA06_Converters_4.View;
using Avalonia.Headless;
using Avalonia.Headless.MSTest;

/// <summary>
/// The Tests namespace.
/// </summary>
namespace AA06_Converters_4.View.Tests;

/// <summary>
/// Defines test class VehicleViewTests.
/// </summary>
[TestClass]
public class VehicleViewTests
{
    /// <summary>
    /// Defines the test method VehicleViewTest.
    /// </summary>
    [AvaloniaTestMethod]
    public void VehicleViewTest()
    {
        // Arrange
        var model = Substitute.For<IAGVModel>();
        var viewModel = new VehicleViewModel(model);

        // Act
        var testView = new VehicleView1(viewModel)
        {
            Width = 800,
            Height = 600
        };

        // Assert
        Assert.IsNotNull(testView);
        Assert.IsInstanceOfType(testView, typeof(VehicleView1));
        Assert.AreSame(viewModel, testView.DataContext);
    }
}
