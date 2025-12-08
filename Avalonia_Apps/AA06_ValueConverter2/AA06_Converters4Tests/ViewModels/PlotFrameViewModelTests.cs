using Microsoft.VisualStudio.TestTools.UnitTesting;
using Avalonia.ViewModels; // BaseTestViewModel
using System.Collections.Generic;
using System;
using NSubstitute;
using AA06_Converters_4.Models.Interfaces;

namespace AA06_Converters_4.ViewModels.Tests;

[TestClass()]
public class PlotFrameViewModelTests : BaseTestViewModel
{
#pragma warning disable CS8618
    private IAGVModel _model;
    protected PlotFrameViewModel testModel;
    protected PlotFrameViewModel testModel2;
#pragma warning restore CS8618

    [TestInitialize]
    public void Init()
    {
        _model = Substitute.For<IAGVModel>();
        _model.SwivelKoor.Returns(new MathLibrary.TwoDim.Math2d.Vector());
        _model.AGVVelocity.Returns(new MathLibrary.TwoDim.Math2d.Vector());
        _model.VehicleDim.Returns(new MathLibrary.TwoDim.Math2d.Vector());

        testModel = new PlotFrameViewModel(_model);
        testModel2 = new PlotFrameViewModel(_model);
    }

    [TestMethod()]
    public void PlotFrameViewModelTest()
    {
        Assert.IsNotNull(testModel);
        Assert.IsNotNull(_model);
    }
}