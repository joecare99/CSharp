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
using Avalonia.ViewModels;
using System.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using AA09_DialogBoxes.Messages;
using System.Threading.Tasks;

/// <summary>
/// The Tests namespace.
/// </summary>
namespace AA09_DialogBoxes.ViewModels.Tests;

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
    MsgBoxResult mbResult;
    /// <summary>
    /// The t result
    /// </summary>
    (bool accepted, (string name, string email) values) tResult;
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
        WeakReferenceMessenger.Default.Register<DialogViewModelTests, MessageBoxRequestMessage>(this, static (r, m) =>
        {
            r.DoLog($"DoOpenMessageBox({m.Title},{m.Content})=>{r.mbResult}");
            m.Reply(r.mbResult);
        });
        WeakReferenceMessenger.Default.Register<DialogViewModelTests, EditDialogRequestMessage>(this, static (r, m) =>
        {
            r.DoLog($"DoOpenDialog({m.Name},{m.Email})=>{r.tResult}");
            m.Reply(r.tResult);
        });
        ClearLog();
    }

    [TestCleanup]
    public void Cleanup()
    {
        WeakReferenceMessenger.Default.UnregisterAll(this);
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
        Assert.IsInstanceOfType(testModel.OpenMsgCommand, typeof(IAsyncRelayCommand));
        Assert.IsNotNull(testModel.OpenDialogCommand);
        Assert.IsInstanceOfType(testModel.OpenDialogCommand, typeof(IAsyncRelayCommand));
    }

    /// <summary>
    /// Opens the MSG command test.
    /// </summary>
    /// <param name="mrAct">The message-box response.</param>
    /// <param name="asExp">Expected values and log.</param>
    [TestMethod()]
    [DataRow(MsgBoxResult.Yes, new[] { @"DoOpenMessageBox(Frage,Willst Du Das ?)=>Yes
PropChgn(DialogViewModel,Name)=TestName
PropChgn(DialogViewModel,Cnt)=2
PropChg(DialogViewModel,Cnt)=3
PropChg(DialogViewModel,Name)=42 Entwickler
", "42 Entwickler" })]
    [DataRow(MsgBoxResult.No, new[] { @"DoOpenMessageBox(Frage,Willst Du Das ?)=>No
PropChgn(DialogViewModel,Name)=TestName
PropChgn(DialogViewModel,Cnt)=2
PropChg(DialogViewModel,Cnt)=3
PropChg(DialogViewModel,Name)=Nö
", "Nö" })]
    public async Task OpenMsgCommandTest(MsgBoxResult mrAct, string[] asExp)
    {
        // Arrange
        testModel.Name = "TestName";
        mbResult = mrAct;
        ClearLog();

        // Act
        Assert.IsTrue(testModel.OpenMsgCommand.CanExecute(null));
        await testModel.OpenMsgCommand.ExecuteAsync(null);

        // Assert
        Assert.AreEqual(asExp[1], testModel.Name);
        Assert.AreEqual(asExp[0], DebugLog);
    }

    /// <summary>
    /// Opens the dialog command test.
    /// </summary>
    /// <param name="xAccepted">Should dialog result be accepted.</param>
    /// <param name="sAct">Suffix used for returned values.</param>
    /// <param name="asExp">Expected values and log.</param>
    [TestMethod()]
    [DataRow(true,"0", new[] { @"DoOpenDialog(TestName,TestEmail)=>(True, (TestName0, TestMail0))
PropChgn(DialogViewModel,Name)=TestName
PropChgn(DialogViewModel,Cnt)=2
PropChg(DialogViewModel,Cnt)=3
PropChg(DialogViewModel,Name)=TestName0
PropChgn(DialogViewModel,Email)=TestEmail
PropChg(DialogViewModel,Email)=TestMail0
", "TestName0", "TestMail0" })]
    [DataRow(false, "1", new[] { @"DoOpenDialog(TestName,TestEmail)=>(False, (TestName1, TestMail1))
", "TestName", "TestEmail" })]
    public async Task OpenDialogCommandTest(bool xAccepted,string sAct, string[] asExp)
    {
        // Arrange
        testModel.Name = "TestName";
        testModel.Email = "TestEmail";
        tResult = (xAccepted, ($"TestName{sAct}", $"TestMail{sAct}"));
        ClearLog();

        // Act    
        Assert.IsTrue(testModel.OpenDialogCommand.CanExecute(null));
        await testModel.OpenDialogCommand.ExecuteAsync(null);
        
        // Assert
        Assert.AreEqual(asExp[1], testModel.Name);
        Assert.AreEqual(asExp[2], testModel.Email);
        Assert.AreEqual(asExp[0], DebugLog);
    }
}
