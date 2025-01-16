﻿using CommunityToolkit.Mvvm.Input;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MVVM.ViewModel;
using System;
using System.ComponentModel;

namespace MVVM_09a_CTDialogBoxes.ViewModels.Tests;

[TestClass()]
public class DialogWindowViewModelTests : BaseTestViewModel
{
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
    /// <summary>
    /// The test model
    /// </summary>
    /// <autogeneratedoc />
    DialogWindowViewModel testModel;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.

    /// <summary>
    /// Initializes this instance.
    /// </summary>
    /// <autogeneratedoc />
    [TestInitialize]
    public void Init()
    {
        testModel = new();
        testModel.PropertyChanged += OnVMPropertyChanged;
        if (testModel is INotifyPropertyChanging npchgn)
            npchgn.PropertyChanging += OnVMPropertyChanging;
        testModel.DoCancel += DoCancelTest;
        testModel.DoOK += DoOKTest;
        ClearLog();
    }

    private void DoOKTest(object o, EventArgs e)
    {
       DoLog($"DoOK({o},{e})"); 
    }

    private void DoCancelTest(object o, EventArgs e)
    {
        DoLog($"DoCancel({o},{e})");
    }

    /// <summary>
    /// Defines the test method SetupTest.
    /// </summary>
    /// <autogeneratedoc />
    [TestMethod()]
    public void SetupTest()
    {
        Assert.IsNotNull(testModel);
        Assert.IsInstanceOfType(testModel, typeof(DialogWindowViewModel));
        Assert.IsInstanceOfType(testModel, typeof(BaseViewModelCT));
        Assert.IsInstanceOfType(testModel, typeof(INotifyPropertyChanged));
        Assert.IsNotNull(testModel.CancelCommand);
        Assert.IsInstanceOfType(testModel.CancelCommand, typeof(IRelayCommand));
        Assert.IsNotNull(testModel.OKCommand);
        Assert.IsInstanceOfType(testModel.OKCommand, typeof(IRelayCommand));
    }

    [DataTestMethod()]
    [DataRow(true, new[] { @"DoCancel(MVVM_09a_CTDialogBoxes.ViewModels.DialogWindowViewModel,System.EventArgs)
" })]
    [DataRow(false, new[] { @"" })]
    [DataRow(null, new[] { @"DoCancel(MVVM_09a_CTDialogBoxes.ViewModels.DialogWindowViewModel,System.EventArgs)
" })]
    public void CancelCommandTest(bool? oAct, string[] asExp)
    {
        Assert.IsTrue(testModel.CancelCommand.CanExecute(oAct));
        if (oAct == false) testModel.DoCancel -= DoCancelTest; 
        testModel.CancelCommand.Execute(oAct);
        Assert.AreEqual(asExp[0], DebugLog);
    }

    [DataTestMethod()]
    [DataRow(true, new[] { @"DoOK(MVVM_09a_CTDialogBoxes.ViewModels.DialogWindowViewModel,System.EventArgs)
" })]
    [DataRow(false, new[] { @"" })]
    [DataRow(null, new[] { @"DoOK(MVVM_09a_CTDialogBoxes.ViewModels.DialogWindowViewModel,System.EventArgs)
" })]
    public void OKCommandTest(bool? oAct, string[] asExp)
    {
        Assert.IsTrue(testModel.OKCommand.CanExecute(oAct));
        if (oAct == false) testModel.DoOK -= DoOKTest;
        testModel.OKCommand.Execute(oAct);
        Assert.AreEqual(asExp[0], DebugLog);
    }
}
