﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using MVVM.ViewModel;
using System.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;

namespace MVVM_09_DialogBoxes.ViewModels.Tests;

[TestClass()]
public class DialogViewModelTests : BaseTestViewModel
{
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
    /// <summary>
    /// The test model
    /// </summary>
    /// <autogeneratedoc />
    DialogViewModel testModel;
    System.Windows.MessageBoxResult mbResult;
    (string name, string email) tResult;
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
        testModel.DoOpenDialog += DoOpenDialogTest;
        testModel.DoOpenMessageBox += DoOpenMessageBoxTest;
        ClearLog();
    }

    private System.Windows.MessageBoxResult DoOpenMessageBoxTest(string title, string name)
    {
        DoLog($"DoOpenMessageBox({title},{name})=>{mbResult}");
        return mbResult;
    }

    private (string name, string email) DoOpenDialogTest(string name, string email)
    {
        DoLog($"DoOpenMessageBox({name},{email})=>{tResult}");
        return tResult;
    }

    /// <summary>
    /// Defines the test method SetupTest.
    /// </summary>
    /// <autogeneratedoc />
    [TestMethod()]
    public void SetupTest()
    {
        Assert.IsNotNull(testModel);
        Assert.IsInstanceOfType(testModel, typeof(DialogViewModel));
        Assert.IsInstanceOfType(testModel, typeof(BaseViewModel));
        Assert.IsInstanceOfType(testModel, typeof(INotifyPropertyChanged));
        Assert.IsNotNull(testModel.OpenMsgCommand);
        Assert.IsInstanceOfType(testModel.OpenMsgCommand, typeof(IRelayCommand));
        Assert.IsNotNull(testModel.OpenDialogCommand);
        Assert.IsInstanceOfType(testModel.OpenDialogCommand, typeof(IRelayCommand));
    }

    [DataTestMethod()]
    [DataRow(true,MessageBoxResult.Yes, new[] { @"DoOpenMessageBox(Frage,Willst Du Das ?)=>Yes
PropChg(MVVM_09_DialogBoxes.ViewModels.DialogViewModel,Name)=42 Entwickler
", "42 Entwickler" })]
    [DataRow(false, MessageBoxResult.OK, new[] { @"PropChg(MVVM_09_DialogBoxes.ViewModels.DialogViewModel,Name)=Nö
", "Nö" })]
    [DataRow(null, MessageBoxResult.No, new[] { @"DoOpenMessageBox(Frage,Willst Du Das ?)=>No
PropChg(MVVM_09_DialogBoxes.ViewModels.DialogViewModel,Name)=Nö
", "Nö" })]
    public void OpenMsgCommandTest(bool? oAct,MessageBoxResult mrExp, string[] asExp)
    {
        // Arrange
        testModel.Name = $"TestName{oAct}";
        if (oAct == false) testModel.DoOpenMessageBox -= DoOpenMessageBoxTest;
        mbResult = mrExp;
        ClearLog();

        // Act
        Assert.IsTrue(testModel.OpenMsgCommand.CanExecute(oAct));
        testModel.OpenMsgCommand.Execute(oAct);

        // Assert
        Assert.AreEqual(asExp[1], testModel.Name);
        Assert.AreEqual(asExp[0], DebugLog);
    }

    [DataTestMethod()]
    [DataRow(true,"0", new[] { @"DoOpenMessageBox(TestNameTrue,TestEmailTrue)=>(TestName0, TestMail0)
PropChg(MVVM_09_DialogBoxes.ViewModels.DialogViewModel,Name)=TestName0
PropChg(MVVM_09_DialogBoxes.ViewModels.DialogViewModel,Email)=TestMail0
", "TestName0", "TestMail0" })]
    [DataRow(false, "1", new[] { @"PropChg(MVVM_09_DialogBoxes.ViewModels.DialogViewModel,Name)=
PropChg(MVVM_09_DialogBoxes.ViewModels.DialogViewModel,Email)=
", "","" })]
    [DataRow(null, "2", new[] { @"DoOpenMessageBox(TestName,TestEmail)=>(TestName2, TestMail2)
PropChg(MVVM_09_DialogBoxes.ViewModels.DialogViewModel,Name)=TestName2
PropChg(MVVM_09_DialogBoxes.ViewModels.DialogViewModel,Email)=TestMail2
", "TestName2", "TestMail2" })]
    public void OpenDialogCommandTest(bool? oAct,string sAct, string[] asExp)
    {
        // Arrange
        testModel.Name = $"TestName{oAct}";
        testModel.Email = $"TestEmail{oAct}";
        tResult = ($"TestName{sAct}", $"TestMail{sAct}");
        if (oAct == false) testModel.DoOpenDialog -= DoOpenDialogTest;
        ClearLog();

        // Act    
        Assert.IsTrue(testModel.OpenDialogCommand.CanExecute(oAct));
        testModel.OpenDialogCommand.Execute(oAct);
        
        // Assert
        Assert.AreEqual(asExp[1], testModel.Name);
        Assert.AreEqual(asExp[2], testModel.Email);
        Assert.AreEqual(asExp[0], DebugLog);
    }
}
