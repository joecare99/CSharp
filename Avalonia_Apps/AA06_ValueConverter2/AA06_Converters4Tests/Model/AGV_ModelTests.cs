using Microsoft.VisualStudio.TestTools.UnitTesting;
using CommunityToolkit.Mvvm.ComponentModel;
using MathLibrary.TwoDim;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace AA06_Converters_4.Models.Tests;

[TestClass()]
public class AGV_ModelTests
{
    private AGV_Model? testModel;
    private AGV_Model? testModel2;

    [TestInitialize]
    public void Init()
    {
        ResetSettings();
        testModel = new AGV_Model();
        testModel2 = new AGV_Model();
    }

    [TestCleanup]
    public void Cleanup()
    {
        ResetSettings();
    }

    private static void ResetSettings()
    {
        var settingsType = typeof(AGV_Model).Assembly.GetType("AA06_Converters_4.Properties.Settings")!;
        var settings = settingsType.GetProperty("Default", BindingFlags.Public | BindingFlags.Static)!.GetValue(null)!;
        settingsType.GetMethod("Reset")!.Invoke(settings, null);
        settingsType.GetMethod("Save")!.Invoke(settings, null);
    }

    [TestMethod()]
    public void AGV_ModelTest()
    {
        Assert.IsNotNull(testModel);
        Assert.IsNotNull(testModel2);
        Assert.IsInstanceOfType(testModel, typeof(AGV_Model));
        Assert.IsInstanceOfType(testModel, typeof(ObservableObject));
    }

    [TestMethod]
    public void Constructor_InitializesConfiguredValues()
    {
        Assert.AreEqual(2000d, testModel!.VehicleDim.x);
        Assert.AreEqual(1200d, testModel.VehicleDim.y);
        Assert.AreEqual(800d, testModel.SwivelKoor.x);
        Assert.AreEqual(-200d, testModel.SwivelKoor.y);
        Assert.AreEqual(400d, testModel.AxisOffset);
        Assert.AreEqual(0d, testModel.Swivel1Angle);
        Assert.AreEqual(0d, testModel.Swivel2Angle);
        Assert.AreEqual(0d, testModel.Wheel1Velocity);
        Assert.AreEqual(0d, testModel.Wheel2Velocity);
        Assert.AreEqual(0d, testModel.Wheel3Velocity);
        Assert.AreEqual(0d, testModel.Wheel4Velocity);
        Assert.IsFalse(testModel.IsDirty);
    }

    [TestMethod]
    public void DerivedWheelValues_ReturnExpectedResults()
    {
        testModel!.Wheel1Velocity = 10d;
        testModel.Wheel2Velocity = 30d;
        testModel.Wheel3Velocity = -8d;
        testModel.Wheel4Velocity = 18d;

        Assert.AreEqual(20d, testModel.Swivel1Velocity, 1e-10);
        Assert.AreEqual(5d, testModel.Swivel2Velocity, 1e-10);
        Assert.AreEqual(0.05d, testModel.Swivel1Rot, 1e-10);
        Assert.AreEqual(0.065d, testModel.Swivel2Rot, 1e-10);
    }

    [TestMethod]
    public void AGVVelocity_ReturnsAverageOfSwivelVectors()
    {
        testModel!.Wheel1Velocity = 10d;
        testModel.Wheel2Velocity = 30d;
        testModel.Wheel3Velocity = 20d;
        testModel.Wheel4Velocity = 40d;
        testModel.Swivel1Angle = 0d;
        testModel.Swivel2Angle = Math2d.pi / 2d;

        Assert.AreEqual(10d, testModel.AGVVelocity.x, 1e-10);
        Assert.AreEqual(15d, testModel.AGVVelocity.y, 1e-10);
    }

    [TestMethod]
    public void VehicleRotation_ReturnsExpectedResult()
    {
        testModel!.Wheel1Velocity = 10d;
        testModel.Wheel2Velocity = 30d;
        testModel.Wheel3Velocity = 20d;
        testModel.Wheel4Velocity = 40d;
        testModel.Swivel1Angle = 0d;
        testModel.Swivel2Angle = Math2d.pi / 2d;

        Assert.AreEqual(-20000d / 1360000d, testModel.VehicleRotation, 1e-12);
    }

    [TestMethod]
    public void PropertyChange_RaisesNotificationAndSetsDirty()
    {
        var propertyNames = new List<string?>();
        testModel!.PropertyChanged += (_, args) => propertyNames.Add(args.PropertyName);

        testModel.Wheel1Velocity = 12d;

        CollectionAssert.Contains(propertyNames, nameof(AGV_Model.Wheel1Velocity));
        CollectionAssert.Contains(propertyNames, nameof(AGV_Model.Swivel1Velocity));
        CollectionAssert.Contains(propertyNames, nameof(AGV_Model.Swivel1Rot));
        CollectionAssert.Contains(propertyNames, nameof(AGV_Model.AGVVelocity));
        CollectionAssert.Contains(propertyNames, nameof(AGV_Model.VehicleRotation));
        Assert.IsTrue(testModel.IsDirty);
    }

    [TestMethod]
    public void Dependencies_ContainsDeclaredSwivelVelocityDependency()
    {
        var dependencies = new List<(string Dest, string Src)>(testModel!.Dependencies);

        CollectionAssert.Contains(dependencies, (nameof(AGV_Model.Swivel1Velocity), nameof(AGV_Model.Wheel1Velocity)));
        Assert.AreEqual(1, dependencies.Count);
    }

    [TestMethod()]
    public void SaveTest()
    {
        testModel!.Wheel1Velocity = 12d;

        testModel.Save();

        Assert.IsFalse(testModel.IsDirty);
    }
}