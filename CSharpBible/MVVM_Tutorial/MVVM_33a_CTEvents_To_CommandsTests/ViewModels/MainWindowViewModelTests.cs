// ***********************************************************************
// Assembly         : MVVM_33a_CTEvents_To_CommandsTests
// Author           : Mir
// Created          : 05-22-2023
//
// Last Modified By : Mir
// Last Modified On : 05-22-2023
// ***********************************************************************
// <copyright file="MainWindowViewModelTests.cs" company="JC-Soft">
//     Copyright � JC-Soft 2023
// </copyright>
// <summary></summary>
// ***********************************************************************
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel;
using MVVM.ViewModel;

/// <summary>
/// The Tests namespace.
/// </summary>
/// <autogeneratedoc />
namespace MVVM_33a_CTEvents_To_Commands.ViewModels.Tests;

/// <summary>
/// Defines test class MainWindowViewModelTests.
/// </summary>
/// <autogeneratedoc />
[TestClass()]
public class MainWindowViewModelTests
{
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erw�gen Sie die Deklaration als Nullable.
    /// <summary>
    /// The test model
    /// </summary>
    /// <autogeneratedoc />
    MainWindowViewModel testModel;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erw�gen Sie die Deklaration als Nullable.

    /// <summary>
    /// Initializes this instance.
    /// </summary>
    /// <autogeneratedoc />
    [TestInitialize]
    public void Init()
    {
        testModel = new();
    }

    /// <summary>
    /// Defines the test method SetupTest.
    /// </summary>
    /// <autogeneratedoc />
    [TestMethod()]
    public void SetupTest()
    {
        Assert.IsNotNull(testModel);
        Assert.IsInstanceOfType(testModel, typeof(MainWindowViewModel));
        Assert.IsInstanceOfType(testModel, typeof(BaseViewModelCT));
        Assert.IsInstanceOfType(testModel, typeof(INotifyPropertyChanged));
    }
}
