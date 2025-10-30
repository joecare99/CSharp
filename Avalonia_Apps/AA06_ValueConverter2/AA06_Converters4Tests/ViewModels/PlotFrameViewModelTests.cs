using Microsoft.VisualStudio.TestTools.UnitTesting;
using Avln_BaseLib.ViewModel;
using System.Collections.Generic;
using System;
using AA06_Converters_4.Model;
using NSubstitute;

namespace AA06_Converters_4.ViewModels.Tests;

[TestClass()]
public class PlotFrameViewModelTests: BaseTestViewModel<PlotFrameViewModel>
{
    private IAGVModel? _model;

    [TestInitialize]
    public override void Init()
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

    protected override Dictionary<string, object?> GetDefaultData() 
        => base.GetDefaultData();
}