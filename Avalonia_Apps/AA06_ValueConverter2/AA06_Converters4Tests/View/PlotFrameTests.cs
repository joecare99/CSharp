using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using AA06_Converters_4.Models.Interfaces;
using AA06_Converters_4.ViewModels;
using AA06_Converters_4.View;
using Avalonia.Headless;
using Avalonia.Headless.MSTest;

namespace AA06_Converters_4.View.Tests;

[TestClass()]
public class PlotFrameTests
{
    [AvaloniaTestMethod()]
    public void PlotFrameTest()
    {
        // Arrange
        var model = Substitute.For<IAGVModel>();
        var viewModel = new PlotFrameViewModel(model);

        // Act
        var view = new PlotFrame(viewModel)
        {
            Width = 800,
            Height = 600
        };

        // Assert
        Assert.IsNotNull(view);
        Assert.IsInstanceOfType(view, typeof(PlotFrame));
        Assert.AreSame(viewModel, view.DataContext);
    }

    [AvaloniaTestMethod()]
    public void OnSizeChangeTest()
    {
        var model = Substitute.For<IAGVModel>();
        var viewModel = new PlotFrameViewModel(model);
        var view = new PlotFrame(viewModel) { Height = 100 };

        var old = view.Height;
        view.Height = old + 2;

        Assert.AreEqual(old + 2, view.Height);
    }

    [AvaloniaTestMethod()]
    public void OnVInitializedTest()
    {
        var model = Substitute.For<IAGVModel>();
        var viewModel = new PlotFrameViewModel(model);
        var view = new PlotFrame(viewModel);

        Assert.IsNotNull(view.DataContext);
        Assert.IsInstanceOfType(view.DataContext, typeof(PlotFrameViewModel));
    }
}