// ***********************************************************************
// Assembly         : AA06_Converters_4Tests
// Author           : Mir
// Created          : 02-03-2024
//
// Last Modified By : Mir
// Last Modified On : 02-03-2024
// ***********************************************************************
// <copyright file="MainWindowTests.cs" company="JC-Soft">
//     Copyright © JC-Soft 2023
// </copyright>
// <summary></summary>
// ***********************************************************************
using Avalonia.Headless;
using Avalonia.Headless.MSTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using AA06_Converters_4.ViewModels;
using AA06_Converters_4.View;
using AA06_Converters_4.Models.Interfaces;

/// <summary>
/// The Tests namespace.
/// </summary>
namespace AA06_Converters_4.View.Tests;

/// <summary>
/// Defines test class MainWindowTests.
/// </summary>
[TestClass()]
public class MainWindowTests
{
    /// <summary>
    /// Defines the test method MainWindowTest.
    /// </summary>
    [AvaloniaTestMethod()]
    public void MainWindowTest()
    {
        // Arrange minimal model for ViewModels
        var model = Substitute.For<IAGVModel>();
        var vehicleVM = new VehicleViewModel(model);
        var plotVM = new PlotFrameViewModel(model);

        var vehicleView = new VehicleView1(vehicleVM);
        var plotFrame = new PlotFrame(plotVM);

        // Act
        var window = new MainWindow(vehicleView, plotFrame)
        {
            Width = 1024,
            Height = 768
        };
        window.Show();

        // Assert
        Assert.IsNotNull(window);
        Assert.IsInstanceOfType(window, typeof(MainWindow));
    }
}