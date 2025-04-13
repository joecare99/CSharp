﻿// ***********************************************************************
// Assembly         : MVVM_25_RichTextEditTests
// Author           : Mir
// Created          : 05-19-2023
//
// Last Modified By : Mir
// Last Modified On : 05-19-2023
// ***********************************************************************
// <copyright file="RichTextEditModelTests.cs" company="JC-Soft">
//     Copyright © JC-Soft 2023
// </copyright>
// <summary></summary>
// ***********************************************************************
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MVVM.ViewModel;
using NSubstitute;
using System.ComponentModel;
using BaseLib.Models.Interfaces;

/// <summary>
/// The Tests namespace.
/// </summary>
/// <autogeneratedoc />
namespace MVVM_25_RichTextEdit.Models.Tests;

/// <summary>
/// Defines test class RichTextEditModelTests.
/// Implements the <see cref="BaseTestViewModel" />
/// </summary>
/// <seealso cref="BaseTestViewModel" />
/// <autogeneratedoc />
[TestClass()]
public class RichTextEditModelTests : BaseTestViewModel
{
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
    /// <summary>
    /// The test model
    /// </summary>
    /// <autogeneratedoc />
    RichTextEditModel testModel;
    ISysTime _sysTime;
    ILog _log;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.

    /// <summary>
    /// Initializes this instance.
    /// </summary>
    [TestInitialize]
    public void Init()
    {
        _sysTime = Substitute.For<ISysTime>();
        _log = Substitute.For<ILog>();
        testModel = new(_sysTime,_log);
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
        Assert.IsInstanceOfType(testModel, typeof(RichTextEditModel));
        Assert.IsInstanceOfType(testModel, typeof(ObservableObject));
        Assert.IsInstanceOfType(testModel, typeof(INotifyPropertyChanged));
    }
}
