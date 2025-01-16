using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Avln_BaseLibTests.Properties;

namespace Avalonia.ViewModels.Tests;

[NotifyDataErrorInfo]
public partial class TestVM:BaseViewModelCT
{
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(DoSomethingCommand))]
    [Range(0, 10, ErrorMessageResourceName = "Err_Range", ErrorMessageResourceType = typeof(Resource))]
    private int _testInt;

    [ObservableProperty]
    [Required(ErrorMessageResourceName = "Err_Required", ErrorMessageResourceType = typeof(Resource))]
    private string _testStr ="<TestStr>";

    bool DSCanExecute() => TestInt > 5;

    [RelayCommand(CanExecute =nameof(DSCanExecute))]
    private void DoSomething() { }
}

[TestClass]
public class BaseTestViewModelTest : BaseTestViewModel
{
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
    TestVM testModel;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.

    [TestInitialize]
    public void Init()
    {
        testModel = new TestVM();
        testModel.PropertyChanged += OnVMPropertyChanged;
        testModel.PropertyChanging += OnVMPropertyChanging;
        testModel.ErrorsChanged += OnVMErrorsChanged;
        testModel.DoSomethingCommand.CanExecuteChanged += OnCanExChanged;
    }

    [DataTestMethod]
    [DataRow(0,0,new[] { "" })]
    [DataRow(-1,0,new[] { @"PropChgn(MVVM_BaseLibTests.ViewModel.TestVM,TestInt)=0
PropChg(MVVM_BaseLibTests.ViewModel.TestVM,HasErrors)=True
ErrorsChanged(MVVM_BaseLibTests.ViewModel.TestVM,TestInt)=The value is not in the specified range TestInt (0-10)
PropChg(MVVM_BaseLibTests.ViewModel.TestVM,TestInt)=-1
CanExChanged(CommunityToolkit.Mvvm.Input.RelayCommand)=False
" })]
    [DataRow(11, 0, new[] { @"PropChgn(MVVM_BaseLibTests.ViewModel.TestVM,TestInt)=0
PropChg(MVVM_BaseLibTests.ViewModel.TestVM,HasErrors)=True
ErrorsChanged(MVVM_BaseLibTests.ViewModel.TestVM,TestInt)=The value is not in the specified range TestInt (0-10)
PropChg(MVVM_BaseLibTests.ViewModel.TestVM,TestInt)=11
CanExChanged(CommunityToolkit.Mvvm.Input.RelayCommand)=True
" })]
    [DataRow(5, 5, new[] { @"PropChgn(MVVM_BaseLibTests.ViewModel.TestVM,TestInt)=0
PropChg(MVVM_BaseLibTests.ViewModel.TestVM,TestInt)=5
CanExChanged(CommunityToolkit.Mvvm.Input.RelayCommand)=False
" })]
    [DataRow(6, 6, new[] { @"PropChgn(MVVM_BaseLibTests.ViewModel.TestVM,TestInt)=0
PropChg(MVVM_BaseLibTests.ViewModel.TestVM,TestInt)=6
CanExChanged(CommunityToolkit.Mvvm.Input.RelayCommand)=True
" })]
    [DataRow(10, 10, new[] { @"PropChgn(MVVM_BaseLibTests.ViewModel.TestVM,TestInt)=0
PropChg(MVVM_BaseLibTests.ViewModel.TestVM,TestInt)=10
CanExChanged(CommunityToolkit.Mvvm.Input.RelayCommand)=True
" })]
    public void IntPropTest(int iAct,int _,string[] asExp)
    {
        // Act
        testModel.TestInt = iAct;

        // Assert
        Assert.AreEqual(iAct, testModel.TestInt);
        Assert.AreEqual(asExp[0], DebugLog);
    }
    [DataTestMethod]
    [DataRow("<TestStr>", new[] { "","<TestStr>" })]
    [DataRow(null, new[] { @"PropChgn(MVVM_BaseLibTests.ViewModel.TestVM,TestStr)=<TestStr>
PropChg(MVVM_BaseLibTests.ViewModel.TestVM,HasErrors)=True
ErrorsChanged(MVVM_BaseLibTests.ViewModel.TestVM,TestStr)=The value may not be empty TestStr
PropChg(MVVM_BaseLibTests.ViewModel.TestVM,TestStr)=
", "<TestStr>" })]
    [DataRow("", new[] { @"PropChgn(MVVM_BaseLibTests.ViewModel.TestVM,TestStr)=<TestStr>
PropChg(MVVM_BaseLibTests.ViewModel.TestVM,HasErrors)=True
ErrorsChanged(MVVM_BaseLibTests.ViewModel.TestVM,TestStr)=The value may not be empty TestStr
PropChg(MVVM_BaseLibTests.ViewModel.TestVM,TestStr)=
", "" })]
    [DataRow("Test", new[] { @"PropChgn(MVVM_BaseLibTests.ViewModel.TestVM,TestStr)=<TestStr>
PropChg(MVVM_BaseLibTests.ViewModel.TestVM,TestStr)=Test
", "Test" })]
    public void StringPropTest(string sAct, string[] asExp)
    {
        // Act
        testModel.TestStr = sAct;

        // Assert
        Assert.AreEqual(sAct, testModel.TestStr);
        Assert.AreEqual(asExp[0], DebugLog);
    }
}

[TestClass]
public class BaseTestViewModelTest_T : BaseTestViewModel<TestVM>
{

    [DataTestMethod]
    [DataRow(0, 0, new[] { "" })]
    [DataRow(-1, 0, new[] { @"PropChgn(MVVM_BaseLibTests.ViewModel.TestVM,TestInt)=0
PropChg(MVVM_BaseLibTests.ViewModel.TestVM,HasErrors)=True
ErrorsChanged(MVVM_BaseLibTests.ViewModel.TestVM,TestInt)=The value is not in the specified range TestInt (0-10)
PropChg(MVVM_BaseLibTests.ViewModel.TestVM,TestInt)=-1
CanExChanged(CommunityToolkit.Mvvm.Input.RelayCommand)=False
" })]
    [DataRow(11, 0, new[] { @"PropChgn(MVVM_BaseLibTests.ViewModel.TestVM,TestInt)=0
PropChg(MVVM_BaseLibTests.ViewModel.TestVM,HasErrors)=True
ErrorsChanged(MVVM_BaseLibTests.ViewModel.TestVM,TestInt)=The value is not in the specified range TestInt (0-10)
PropChg(MVVM_BaseLibTests.ViewModel.TestVM,TestInt)=11
CanExChanged(CommunityToolkit.Mvvm.Input.RelayCommand)=True
" })]
    [DataRow(5, 5, new[] { @"PropChgn(MVVM_BaseLibTests.ViewModel.TestVM,TestInt)=0
PropChg(MVVM_BaseLibTests.ViewModel.TestVM,TestInt)=5
CanExChanged(CommunityToolkit.Mvvm.Input.RelayCommand)=False
" })]
    [DataRow(6, 6, new[] { @"PropChgn(MVVM_BaseLibTests.ViewModel.TestVM,TestInt)=0
PropChg(MVVM_BaseLibTests.ViewModel.TestVM,TestInt)=6
CanExChanged(CommunityToolkit.Mvvm.Input.RelayCommand)=True
" })]
    [DataRow(10, 10, new[] { @"PropChgn(MVVM_BaseLibTests.ViewModel.TestVM,TestInt)=0
PropChg(MVVM_BaseLibTests.ViewModel.TestVM,TestInt)=10
CanExChanged(CommunityToolkit.Mvvm.Input.RelayCommand)=True
" })]
    public void IntPropTest(int iAct, int _, string[] asExp)
    {
        // Act
        testModel.TestInt = iAct;

        // Assert
        Assert.AreEqual(iAct, testModel.TestInt);
        Assert.AreEqual(asExp[0], DebugLog);
    }
    [DataTestMethod]
    [DataRow("<TestStr>", new[] { "", "<TestStr>" })]
    [DataRow(null, new[] { @"PropChgn(MVVM_BaseLibTests.ViewModel.TestVM,TestStr)=<TestStr>
PropChg(MVVM_BaseLibTests.ViewModel.TestVM,HasErrors)=True
ErrorsChanged(MVVM_BaseLibTests.ViewModel.TestVM,TestStr)=The value may not be empty TestStr
PropChg(MVVM_BaseLibTests.ViewModel.TestVM,TestStr)=
", "<TestStr>" })]
    [DataRow("", new[] { @"PropChgn(MVVM_BaseLibTests.ViewModel.TestVM,TestStr)=<TestStr>
PropChg(MVVM_BaseLibTests.ViewModel.TestVM,HasErrors)=True
ErrorsChanged(MVVM_BaseLibTests.ViewModel.TestVM,TestStr)=The value may not be empty TestStr
PropChg(MVVM_BaseLibTests.ViewModel.TestVM,TestStr)=
", "" })]
    [DataRow("Test", new[] { @"PropChgn(MVVM_BaseLibTests.ViewModel.TestVM,TestStr)=<TestStr>
PropChg(MVVM_BaseLibTests.ViewModel.TestVM,TestStr)=Test
", "Test" })]
    public void StringPropTest(string sAct, string[] asExp)
    {
        // Act
        testModel.TestStr = sAct;

        // Assert
        Assert.AreEqual(sAct, testModel.TestStr);
        Assert.AreEqual(asExp[0], DebugLog);
    }

    [TestMethod]
    public void DoSomethingTest()
    {
        testModel.TestInt = 5;
        ClearLog();
        // Act
        testModel.DoSomethingCommand.Execute(null);

        // Assert
        Assert.AreEqual(5, testModel.TestInt);
        Assert.AreEqual(@"", DebugLog);
    }

    [TestMethod]
    public void TestModelPropsTest()
    {
        // Act
        // Assert
        Assert.IsNotNull(TestModelProperies);
    }

    protected override Dictionary<string, object?> GetDefaultData() 
        => new() { { nameof(TestVM.HasErrors), false }, { nameof(TestVM.TestInt), 0 }, { nameof(TestVM.TestStr),"<TestStr>" } };
}
