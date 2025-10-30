﻿// ***********************************************************************
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
using System;
using System.Threading;
using NSubstitute;
using BaseLib.Helper;
using AA06_Converters_4.Model;

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
    [TestMethod]
    public void VehicleViewTest()
    {
        IoC.GetReqSrv = (t)=>t switch
        {
            _ when t == typeof(IAGVModel) => Substitute.For<IAGVModel>(),
            _ => throw new NotImplementedException()
        };
        VehicleView1? testView = null;
        var t = new Thread(() => testView = new());
        t.SetApartmentState(ApartmentState.STA); //Set the thread to STA
        t.Start();
        t.Join(); //Wait for the thread to end
        Assert.IsNotNull(testView);
        Assert.IsInstanceOfType(testView, typeof(VehicleView1));
    }
}
