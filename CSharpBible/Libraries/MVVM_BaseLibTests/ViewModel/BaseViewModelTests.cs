using CommunityToolkit.Mvvm.Input;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.ComponentModel;

namespace MVVM.ViewModel.Tests;

[TestClass()]
public class BaseViewModelTests : BaseViewModel
{
    private int property1=3;
    private int property2=5;
    private string DebugResult="";

    [TestInitialize()]
    public void Init()
    {
        CommandCanExecuteBindingClear();
        KnownParams.Clear();
        PropertyChanged -= OnPropertyChanged;
        PropertyChanged += OnPropertyChanged;
        doSomething = new DelegateCommand<int>((i) => DebugResult += $"doSomething({i})",(i)=>Prop1IsGreaterThen1Prop2());
        doSomething.CanExecuteChanged += OnCanExChanged;    
    }

    private void OnCanExChanged(object? sender, EventArgs e)
        => DebugResult += $"OnCanExChanged: o:{sender}{Environment.NewLine}";

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        => DebugResult += $"OnPropChanged: o:{sender}, p:{e.PropertyName}:{sender?.GetType().GetProperty(e.PropertyName??"")?.GetValue(sender)}{Environment.NewLine}";

    public int Property1 { get => property1; set => SetProperty(ref property1 , value); }
    public int Property2 { get => property2; set => SetProperty(ref property2, value); }
    public int Property3 { get => property1 + property2; set => Property1 = value -Property2; }
    public int Property4 { get => property2*property2; set => Property2 = (int)Math.Sqrt(value); }

    public bool Prop1IsGreaterThen1Prop2() => property1 > property2;
    public bool Prop2IsGreater(int i) => i > property2;
    public bool IsGreater(int i,int i2) => i > i2;

    public IRelayCommand? doSomething { get; set; }

    [DataTestMethod()]
    [DataRow("0 - 1 =>  2",1,2,new string[] {
        @"OnPropChanged: o:MVVM.ViewModel.Tests.BaseViewModelTests, p:Property3:7
OnCanExChanged: o:MVVM.ViewModel.DelegateCommand`1[System.Int32]
OnPropChanged: o:MVVM.ViewModel.Tests.BaseViewModelTests, p:Prop1IsGreaterThen1Prop2:
OnPropChanged: o:MVVM.ViewModel.Tests.BaseViewModelTests, p:Property1:2
" })]
    [DataRow("1 - 1 =>  6", 1, 6, new string[] {
        @"OnPropChanged: o:MVVM.ViewModel.Tests.BaseViewModelTests, p:Property3:11
OnCanExChanged: o:MVVM.ViewModel.DelegateCommand`1[System.Int32]
OnPropChanged: o:MVVM.ViewModel.Tests.BaseViewModelTests, p:Prop1IsGreaterThen1Prop2:
OnPropChanged: o:MVVM.ViewModel.Tests.BaseViewModelTests, p:Property1:6
" })]
    [DataRow("2 - 2 =>  2", 2, 2, new string[] {
        @"OnPropChanged: o:MVVM.ViewModel.Tests.BaseViewModelTests, p:Property3:5
OnPropChanged: o:MVVM.ViewModel.Tests.BaseViewModelTests, p:Property4:4
OnCanExChanged: o:MVVM.ViewModel.DelegateCommand`1[System.Int32]
OnPropChanged: o:MVVM.ViewModel.Tests.BaseViewModelTests, p:Prop1IsGreaterThen1Prop2:
OnPropChanged: o:MVVM.ViewModel.Tests.BaseViewModelTests, p:Prop2IsGreater:
OnPropChanged: o:MVVM.ViewModel.Tests.BaseViewModelTests, p:Prop2IsGreater:
OnPropChanged: o:MVVM.ViewModel.Tests.BaseViewModelTests, p:Prop2IsGreater:
OnPropChanged: o:MVVM.ViewModel.Tests.BaseViewModelTests, p:Property2:2
" })]
    [DataRow("3 - 2 =>  6", 2, 6, new string[] {
        @"OnPropChanged: o:MVVM.ViewModel.Tests.BaseViewModelTests, p:Property3:9
OnPropChanged: o:MVVM.ViewModel.Tests.BaseViewModelTests, p:Property4:36
OnCanExChanged: o:MVVM.ViewModel.DelegateCommand`1[System.Int32]
OnPropChanged: o:MVVM.ViewModel.Tests.BaseViewModelTests, p:Prop1IsGreaterThen1Prop2:
OnPropChanged: o:MVVM.ViewModel.Tests.BaseViewModelTests, p:Prop2IsGreater:
OnPropChanged: o:MVVM.ViewModel.Tests.BaseViewModelTests, p:Prop2IsGreater:
OnPropChanged: o:MVVM.ViewModel.Tests.BaseViewModelTests, p:Prop2IsGreater:
OnPropChanged: o:MVVM.ViewModel.Tests.BaseViewModelTests, p:Property2:6
" })]
    [DataRow("4 - 3 =>  9", 3, 9, new string[] {
        @"OnPropChanged: o:MVVM.ViewModel.Tests.BaseViewModelTests, p:Property3:9
OnCanExChanged: o:MVVM.ViewModel.DelegateCommand`1[System.Int32]
OnPropChanged: o:MVVM.ViewModel.Tests.BaseViewModelTests, p:Prop1IsGreaterThen1Prop2:
OnPropChanged: o:MVVM.ViewModel.Tests.BaseViewModelTests, p:Property1:4
" })]
    [DataRow("5 - 3 =>  7", 3, 7, new string[] {
        @"OnPropChanged: o:MVVM.ViewModel.Tests.BaseViewModelTests, p:Property3:7
OnCanExChanged: o:MVVM.ViewModel.DelegateCommand`1[System.Int32]
OnPropChanged: o:MVVM.ViewModel.Tests.BaseViewModelTests, p:Prop1IsGreaterThen1Prop2:
OnPropChanged: o:MVVM.ViewModel.Tests.BaseViewModelTests, p:Property1:2
" })]
    [DataRow("6 - 4 =>  9", 4, 9, new string[] {
        @"OnPropChanged: o:MVVM.ViewModel.Tests.BaseViewModelTests, p:Property3:6
OnPropChanged: o:MVVM.ViewModel.Tests.BaseViewModelTests, p:Property4:9
OnCanExChanged: o:MVVM.ViewModel.DelegateCommand`1[System.Int32]
OnPropChanged: o:MVVM.ViewModel.Tests.BaseViewModelTests, p:Prop1IsGreaterThen1Prop2:
OnPropChanged: o:MVVM.ViewModel.Tests.BaseViewModelTests, p:Prop2IsGreater:
OnPropChanged: o:MVVM.ViewModel.Tests.BaseViewModelTests, p:Prop2IsGreater:
OnPropChanged: o:MVVM.ViewModel.Tests.BaseViewModelTests, p:Prop2IsGreater:
OnPropChanged: o:MVVM.ViewModel.Tests.BaseViewModelTests, p:Property2:3
" })]
    [DataRow("7 - 4 =>  16", 4, 16, new string[] {
        @"OnPropChanged: o:MVVM.ViewModel.Tests.BaseViewModelTests, p:Property3:7
OnPropChanged: o:MVVM.ViewModel.Tests.BaseViewModelTests, p:Property4:16
OnCanExChanged: o:MVVM.ViewModel.DelegateCommand`1[System.Int32]
OnPropChanged: o:MVVM.ViewModel.Tests.BaseViewModelTests, p:Prop1IsGreaterThen1Prop2:
OnPropChanged: o:MVVM.ViewModel.Tests.BaseViewModelTests, p:Prop2IsGreater:
OnPropChanged: o:MVVM.ViewModel.Tests.BaseViewModelTests, p:Prop2IsGreater:
OnPropChanged: o:MVVM.ViewModel.Tests.BaseViewModelTests, p:Prop2IsGreater:
OnPropChanged: o:MVVM.ViewModel.Tests.BaseViewModelTests, p:Property2:4
" })]
    public void BaseViewModelTest(string name, int i,int iVal, string[] aExp )
    {
        AddPropertyDependency(nameof(Property3), nameof(Property1));
        AddPropertyDependency(nameof(Property3), nameof(Property2));
        AddPropertyDependency(nameof(Property4), nameof(Property2));
        AddPropertyDependency(nameof(Prop1IsGreaterThen1Prop2), nameof(Property1));
        AddPropertyDependency(nameof(Prop1IsGreaterThen1Prop2), nameof(Property2));
        AddPropertyDependency(nameof(Prop2IsGreater), nameof(Property2));
        AddPropertyDependency(nameof(doSomething), nameof(Prop1IsGreaterThen1Prop2));
        AddPropertyDependency(nameof(IsGreater), nameof(Property2));
        AddPropertyDependency("XX", nameof(Property2));
        AppendKnownParams(1, nameof(Prop2IsGreater));
        AppendKnownParams(5, nameof(Prop2IsGreater));
        AppendKnownParams(7, nameof(Prop2IsGreater));
        AppendKnownParams(7, "");
        switch (i)
        {
            case 1:Property1 = iVal; break;
            case 2: Property2 = iVal; break;
            case 3: Property3 = iVal; break;
            case 4: Property4 = iVal; break;
        }
        Assert.AreEqual( aExp[0], DebugResult, name);

    }

    [TestMethod]
    public void RemovePropertyDependencyTest()
    {
        Assert.ThrowsException<NotImplementedException>(() => RemovePropertyDependency("1", "3"));
    }

    [DataTestMethod]
    [DataRow(1,false)]
    [DataRow(2, false)]
    [DataRow(0, false)]
    public void FuncProxyTest (int dVal,bool xExp)
    {
        Assert.AreEqual (xExp,FuncProxy(dVal, Prop2IsGreater));
    }

    [DataTestMethod]
    [DataRow(1, true)]
    [DataRow(2, true)]
    [DataRow(0.5, false)]
    public void FuncProxy2Test(double dVal, bool xExp)
    {
        Assert.AreEqual(xExp, FuncProxy(dVal, LocalFunc));
    }

    [DataTestMethod]
    [DataRow(1,1, false)]
    [DataRow(3,2, true)]
    [DataRow(0,1, false)]
    public void IsGreaterTest(int dVal1,int dVal2, bool xExp)
    {
        Assert.AreEqual(xExp, IsGreater(dVal1,dVal2));
    }
    private bool LocalFunc(double arg)
    {
        return arg == Math.Floor(arg);
    }
}