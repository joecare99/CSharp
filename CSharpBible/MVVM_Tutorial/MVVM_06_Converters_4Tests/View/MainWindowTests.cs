// ***********************************************************************
// Assembly         : MVVM_06_Converters_4Tests
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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;

/// <summary>
/// The Tests namespace.
/// </summary>
namespace MVVM_06_Converters_4.View.Tests;

/// <summary>
/// Defines test class MainWindowTests.
/// </summary>
[TestClass()]
public class MainWindowTests
{
    /// <summary>
    /// Defines the test method MainWindowTest.
    /// </summary>
    [TestMethod()]
    public void MainWindowTest()
    {
        MainWindow? mw=null;
        var t = new Thread(()=> mw = new());
        t.SetApartmentState(ApartmentState.STA); //Set the thread to STA
        t.Start();
        t.Join(); //Wait for the thread to end
        Assert.IsNotNull(mw);
        Assert.IsInstanceOfType(mw, typeof(MainWindow));    
    }
}