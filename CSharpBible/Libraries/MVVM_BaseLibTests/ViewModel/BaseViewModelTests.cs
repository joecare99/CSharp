using CommunityToolkit.Mvvm.Input;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MVVM.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVM.ViewModel.Tests
{
    [TestClass()]
    public class BaseViewModelTests : BaseViewModel
    {
        private int property1=3;
        private int property2=5;
        private string DebugResult="";

        [TestInitialize()]
        public void Init()
        {
            CommandCanExecuteBinding.Clear();
            KnownParams.Clear();
            PropertyChanged -= OnPropertyChanged;
            PropertyChanged += OnPropertyChanged;
            doSomething = new DelegateCommand<int>((i) => DebugResult += $"doSomething({i})",(i)=>Prop1IsGreaterthan1Prop2());
            doSomething.CanExecuteChanged += OnCanExChanged;    
        }

        private void OnCanExChanged(object sender, EventArgs e)
            => DebugResult += $"OnCanExChanged: o:{sender}{Environment.NewLine}";

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
            => DebugResult += $"OnPropChanged: o:{sender}, p:{e.PropertyName}:{sender?.GetType().GetProperty(e.PropertyName)?.GetValue(sender)}{Environment.NewLine}";

        public int Property1 { get => property1; set => SetProperty(ref property1 , value); }
        public int Property2 { get => property2; set => SetProperty(ref property2, value); }
        public int Property3 { get => property1 + property2; set => Property1 = value -Property2; }
        public int Property4 { get => property2*property2; set => Property2 = (int)Math.Sqrt(value); }

        public bool Prop1IsGreaterthan1Prop2() => property1 > property2;
        public bool Prop2IsGreater(int i) => i > property2;

        public IRelayCommand doSomething { get; set; }

        [DataTestMethod()]
        [DataRow("0 - 1 =>  2",1,2,new string[] {
            @"OnPropChanged: o:MVVM.ViewModel.Tests.BaseViewModelTests, p:Property3:7
OnCanExChanged: o:MVVM.ViewModel.DelegateCommand`1[System.Int32]
OnPropChanged: o:MVVM.ViewModel.Tests.BaseViewModelTests, p:Prop1IsGreaterthan1Prop2:
OnPropChanged: o:MVVM.ViewModel.Tests.BaseViewModelTests, p:Property1:2
" })]
        [DataRow("1 - 1 =>  6", 1, 6, new string[] {
            @"OnPropChanged: o:MVVM.ViewModel.Tests.BaseViewModelTests, p:Property3:11
OnCanExChanged: o:MVVM.ViewModel.DelegateCommand`1[System.Int32]
OnPropChanged: o:MVVM.ViewModel.Tests.BaseViewModelTests, p:Prop1IsGreaterthan1Prop2:
OnPropChanged: o:MVVM.ViewModel.Tests.BaseViewModelTests, p:Property1:6
" })]
        [DataRow("2 - 2 =>  2", 2, 2, new string[] {
            @"OnPropChanged: o:MVVM.ViewModel.Tests.BaseViewModelTests, p:Property3:5
OnPropChanged: o:MVVM.ViewModel.Tests.BaseViewModelTests, p:Property4:4
OnCanExChanged: o:MVVM.ViewModel.DelegateCommand`1[System.Int32]
OnPropChanged: o:MVVM.ViewModel.Tests.BaseViewModelTests, p:Prop1IsGreaterthan1Prop2:
OnPropChanged: o:MVVM.ViewModel.Tests.BaseViewModelTests, p:Prop2IsGreater:
OnPropChanged: o:MVVM.ViewModel.Tests.BaseViewModelTests, p:Prop2IsGreater:
OnPropChanged: o:MVVM.ViewModel.Tests.BaseViewModelTests, p:Prop2IsGreater:
OnPropChanged: o:MVVM.ViewModel.Tests.BaseViewModelTests, p:Property2:2
" })]
        [DataRow("3 - 2 =>  6", 2, 6, new string[] {
            @"OnPropChanged: o:MVVM.ViewModel.Tests.BaseViewModelTests, p:Property3:9
OnPropChanged: o:MVVM.ViewModel.Tests.BaseViewModelTests, p:Property4:36
OnCanExChanged: o:MVVM.ViewModel.DelegateCommand`1[System.Int32]
OnPropChanged: o:MVVM.ViewModel.Tests.BaseViewModelTests, p:Prop1IsGreaterthan1Prop2:
OnPropChanged: o:MVVM.ViewModel.Tests.BaseViewModelTests, p:Prop2IsGreater:
OnPropChanged: o:MVVM.ViewModel.Tests.BaseViewModelTests, p:Prop2IsGreater:
OnPropChanged: o:MVVM.ViewModel.Tests.BaseViewModelTests, p:Prop2IsGreater:
OnPropChanged: o:MVVM.ViewModel.Tests.BaseViewModelTests, p:Property2:6
" })]
        [DataRow("4 - 3 =>  9", 3, 9, new string[] {
            @"OnPropChanged: o:MVVM.ViewModel.Tests.BaseViewModelTests, p:Property3:9
OnCanExChanged: o:MVVM.ViewModel.DelegateCommand`1[System.Int32]
OnPropChanged: o:MVVM.ViewModel.Tests.BaseViewModelTests, p:Prop1IsGreaterthan1Prop2:
OnPropChanged: o:MVVM.ViewModel.Tests.BaseViewModelTests, p:Property1:4
" })]
        [DataRow("5 - 3 =>  7", 3, 7, new string[] {
            @"OnPropChanged: o:MVVM.ViewModel.Tests.BaseViewModelTests, p:Property3:7
OnCanExChanged: o:MVVM.ViewModel.DelegateCommand`1[System.Int32]
OnPropChanged: o:MVVM.ViewModel.Tests.BaseViewModelTests, p:Prop1IsGreaterthan1Prop2:
OnPropChanged: o:MVVM.ViewModel.Tests.BaseViewModelTests, p:Property1:2
" })]
        [DataRow("6 - 4 =>  9", 4, 9, new string[] {
            @"OnPropChanged: o:MVVM.ViewModel.Tests.BaseViewModelTests, p:Property3:6
OnPropChanged: o:MVVM.ViewModel.Tests.BaseViewModelTests, p:Property4:9
OnCanExChanged: o:MVVM.ViewModel.DelegateCommand`1[System.Int32]
OnPropChanged: o:MVVM.ViewModel.Tests.BaseViewModelTests, p:Prop1IsGreaterthan1Prop2:
OnPropChanged: o:MVVM.ViewModel.Tests.BaseViewModelTests, p:Prop2IsGreater:
OnPropChanged: o:MVVM.ViewModel.Tests.BaseViewModelTests, p:Prop2IsGreater:
OnPropChanged: o:MVVM.ViewModel.Tests.BaseViewModelTests, p:Prop2IsGreater:
OnPropChanged: o:MVVM.ViewModel.Tests.BaseViewModelTests, p:Property2:3
" })]
        [DataRow("7 - 4 =>  16", 4, 16, new string[] {
            @"OnPropChanged: o:MVVM.ViewModel.Tests.BaseViewModelTests, p:Property3:7
OnPropChanged: o:MVVM.ViewModel.Tests.BaseViewModelTests, p:Property4:16
OnCanExChanged: o:MVVM.ViewModel.DelegateCommand`1[System.Int32]
OnPropChanged: o:MVVM.ViewModel.Tests.BaseViewModelTests, p:Prop1IsGreaterthan1Prop2:
OnPropChanged: o:MVVM.ViewModel.Tests.BaseViewModelTests, p:Prop2IsGreater:
OnPropChanged: o:MVVM.ViewModel.Tests.BaseViewModelTests, p:Prop2IsGreater:
OnPropChanged: o:MVVM.ViewModel.Tests.BaseViewModelTests, p:Prop2IsGreater:
OnPropChanged: o:MVVM.ViewModel.Tests.BaseViewModelTests, p:Property2:4
" })]
        public void BaseViewModelTest(string name, int i,int ival, string[] aExp )
        {
            CommandCanExecuteBinding.Add((nameof(Property1), nameof(Property3)));
            CommandCanExecuteBinding.Add((nameof(Property2), nameof(Property3)));
            CommandCanExecuteBinding.Add((nameof(Property2), nameof(Property4)));
            CommandCanExecuteBinding.Add((nameof(Property1), nameof(Prop1IsGreaterthan1Prop2)));
            CommandCanExecuteBinding.Add((nameof(Property2), nameof(Prop1IsGreaterthan1Prop2)));
            CommandCanExecuteBinding.Add((nameof(Property2), nameof(Prop2IsGreater)));
            CommandCanExecuteBinding.Add((nameof(Prop1IsGreaterthan1Prop2),nameof(doSomething)));
            AppendKnownParams(1, nameof(Prop2IsGreater));
            AppendKnownParams(5, nameof(Prop2IsGreater));
            AppendKnownParams(7, nameof(Prop2IsGreater));
            AppendKnownParams(7, "");
            switch (i)
            {
                case 1:Property1 = ival; break;
                case 2: Property2 = ival; break;
                case 3: Property3 = ival; break;
                case 4: Property4 = ival; break;
            }
            Assert.AreEqual( aExp[0], DebugResult, name);

        }
    }
}