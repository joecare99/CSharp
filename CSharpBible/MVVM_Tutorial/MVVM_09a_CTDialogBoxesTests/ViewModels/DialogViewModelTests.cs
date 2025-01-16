// ***********************************************************************
// Assembly         : MVVM_09a_CTDialogBoxesTests
// Author           : Mir
// Created          : 06-16-2024
//
// Last Modified By : Mir
// Last Modified On : 06-16-2024
// ***********************************************************************
// <copyright file="DialogViewModelTests.cs" company="JC-Soft">
//     Copyright © JC-Soft 2023
// </copyright>
// <summary></summary>
// ***********************************************************************
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MVVM.ViewModel;
using System.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;

/// <summary>
/// The Tests namespace.
/// </summary>
namespace MVVM_09a_CTDialogBoxes.ViewModels.Tests;

/// <summary>
/// Defines test class DialogViewModelTests.
/// Implements the <see cref="BaseTestViewModel" />
/// </summary>
/// <seealso cref="BaseTestViewModel" />
[TestClass()]
public class DialogViewModelTests : BaseTestViewModel
{
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
    /// <summary>
    /// The test model
    /// </summary>
    DialogViewModel testModel;
    /// <summary>
    /// The mb result
    /// </summary>
    System.Windows.MessageBoxResult mbResult;
    /// <summary>
    /// The t result
    /// </summary>
    (string name, string email) tResult;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.

    /// <summary>
    /// Initializes this instance.
    /// </summary>
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

    /// <summary>
    /// Does the open message box test.
    /// </summary>
    /// <param name="title">The title.</param>
    /// <param name="name">The name.</param>
    /// <returns>System.Windows.MessageBoxResult.</returns>
    private System.Windows.MessageBoxResult DoOpenMessageBoxTest(string title, string name)
    {
        DoLog($"DoOpenMessageBox({title},{name})=>{mbResult}");
        return mbResult;
    }

    /// <summary>
    /// Does the open dialog test.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="email">The email.</param>
    /// <returns>System.ValueTuple&lt;System.String, System.String&gt;.</returns>
    private (string name, string email) DoOpenDialogTest(string name, string email)
    {
        DoLog($"DoOpenMessageBox({name},{email})=>{tResult}");
        return tResult;
    }

    /// <summary>
    /// Defines the test method SetupTest.
    /// </summary>
    [TestMethod()]
    public void SetupTest()
    {
        Assert.IsNotNull(testModel);
        Assert.IsInstanceOfType(testModel, typeof(DialogViewModel));
        Assert.IsInstanceOfType(testModel, typeof(BaseViewModelCT));
        Assert.IsInstanceOfType(testModel, typeof(INotifyPropertyChanged));
        Assert.IsNotNull(testModel.OpenMsgCommand);
        Assert.IsInstanceOfType(testModel.OpenMsgCommand, typeof(IRelayCommand));
        Assert.IsNotNull(testModel.OpenDialogCommand);
        Assert.IsInstanceOfType(testModel.OpenDialogCommand, typeof(IRelayCommand));
    }

    /// <summary>
    /// Opens the MSG command test.
    /// </summary>
    /// <param name="oAct">if set to <c>true</c> [o act].</param>
    /// <param name="mrExp">The mr exp.</param>
    /// <param name="asExp">As exp.</param>
    [DataTestMethod()]
    [DataRow(true,MessageBoxResult.Yes, new[] { @"DoOpenMessageBox(Frage,Willst Du Das ?)=>Yes
PropChgn(MVVM_09a_CTDialogBoxes.ViewModels.DialogViewModel,Name)=TestNameTrue
PropChgn(MVVM_09a_CTDialogBoxes.ViewModels.DialogViewModel,Cnt)=2
PropChg(MVVM_09a_CTDialogBoxes.ViewModels.DialogViewModel,Cnt)=3
PropChg(MVVM_09a_CTDialogBoxes.ViewModels.DialogViewModel,Name)=42 Entwickler
", "42 Entwickler" })]
    [DataRow(false, MessageBoxResult.OK, new[] { @"PropChgn(MVVM_09a_CTDialogBoxes.ViewModels.DialogViewModel,Name)=TestNameFalse
PropChgn(MVVM_09a_CTDialogBoxes.ViewModels.DialogViewModel,Cnt)=2
PropChg(MVVM_09a_CTDialogBoxes.ViewModels.DialogViewModel,Cnt)=3
PropChg(MVVM_09a_CTDialogBoxes.ViewModels.DialogViewModel,Name)=Nö
", "Nö" })]
    [DataRow(null, MessageBoxResult.No, new[] { @"DoOpenMessageBox(Frage,Willst Du Das ?)=>No
PropChgn(MVVM_09a_CTDialogBoxes.ViewModels.DialogViewModel,Name)=TestName
PropChgn(MVVM_09a_CTDialogBoxes.ViewModels.DialogViewModel,Cnt)=2
PropChg(MVVM_09a_CTDialogBoxes.ViewModels.DialogViewModel,Cnt)=3
PropChg(MVVM_09a_CTDialogBoxes.ViewModels.DialogViewModel,Name)=Nö
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

    /// <summary>
    /// Opens the dialog command test.
    /// </summary>
    /// <param name="oAct">if set to <c>true</c> [o act].</param>
    /// <param name="sAct">The s act.</param>
    /// <param name="asExp">As exp.</param>
    [DataTestMethod()]
    [DataRow(true,"0", new[] { @"DoOpenMessageBox(TestNameTrue,TestEmailTrue)=>(TestName0, TestMail0)
PropChgn(MVVM_09a_CTDialogBoxes.ViewModels.DialogViewModel,Name)=TestNameTrue
PropChgn(MVVM_09a_CTDialogBoxes.ViewModels.DialogViewModel,Cnt)=2
PropChg(MVVM_09a_CTDialogBoxes.ViewModels.DialogViewModel,Cnt)=3
PropChg(MVVM_09a_CTDialogBoxes.ViewModels.DialogViewModel,Name)=TestName0
PropChgn(MVVM_09a_CTDialogBoxes.ViewModels.DialogViewModel,Email)=TestEmailTrue
PropChg(MVVM_09a_CTDialogBoxes.ViewModels.DialogViewModel,Email)=TestMail0
", "TestName0", "TestMail0" })]
    [DataRow(false, "1", new[] { @"PropChgn(MVVM_09a_CTDialogBoxes.ViewModels.DialogViewModel,Name)=TestNameFalse
PropChgn(MVVM_09a_CTDialogBoxes.ViewModels.DialogViewModel,Cnt)=2
PropChg(MVVM_09a_CTDialogBoxes.ViewModels.DialogViewModel,Cnt)=3
PropChg(MVVM_09a_CTDialogBoxes.ViewModels.DialogViewModel,Name)=
PropChgn(MVVM_09a_CTDialogBoxes.ViewModels.DialogViewModel,Email)=TestEmailFalse
PropChg(MVVM_09a_CTDialogBoxes.ViewModels.DialogViewModel,Email)=
", "","" })]
    [DataRow(null, "2", new[] { @"DoOpenMessageBox(TestName,TestEmail)=>(TestName2, TestMail2)
PropChgn(MVVM_09a_CTDialogBoxes.ViewModels.DialogViewModel,Name)=TestName
PropChgn(MVVM_09a_CTDialogBoxes.ViewModels.DialogViewModel,Cnt)=2
PropChg(MVVM_09a_CTDialogBoxes.ViewModels.DialogViewModel,Cnt)=3
PropChg(MVVM_09a_CTDialogBoxes.ViewModels.DialogViewModel,Name)=TestName2
PropChgn(MVVM_09a_CTDialogBoxes.ViewModels.DialogViewModel,Email)=TestEmail
PropChg(MVVM_09a_CTDialogBoxes.ViewModels.DialogViewModel,Email)=TestMail2
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
