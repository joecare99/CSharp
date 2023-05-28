using CommunityToolkit.Mvvm.Input;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MVVM.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVM.ViewModel.Tests
{
    [TestClass()]
    public class DelegateCommandTests : BaseTestViewModel
    {
        DelegateCommand _testCommand1;
        DelegateCommand _testCommand3;
        DelegateCommand<TypeCode?> _testCommand2;

        [TestInitialize()]
        public void Init()
        {
            _testCommand1 = new((o) => DoLog($"Execute({nameof(_testCommand1)},{o})"));
            _testCommand3 = new((o) => DoLog($"Execute({nameof(_testCommand3)},{o})"),canCmd3);
            _testCommand2 = new(
                (o) => DoLog($"Execute({nameof(_testCommand2)},{o})"),
                canCmd2);
            _testCommand1.CanExecuteChanged += OnCanExecuteChanged;
            _testCommand2.CanExecuteChanged += OnCanExecuteChanged;
//            _testCommand3.CanExecuteChanged += OnCanExecuteChanged;
        }

        private void OnCanExecuteChanged(object sender, EventArgs e)
        {
            DoLog($"CanExecuteChanged({sender})");
        }

        private bool canCmd2(TypeCode? o)
            => o is not null and not (TypeCode.Object or TypeCode.Empty or TypeCode.DBNull);

        private bool canCmd3(object? o)
            => o is not null and not (TypeCode.Object or TypeCode.Empty or TypeCode.DBNull);


        [DataTestMethod()]
        [DataRow(0, null, false)]
        [DataRow(1,TypeCode.Boolean,true)]
        [DataRow(1, TypeCode.Empty, true)]
        [DataRow(1, TypeCode.String, true)]
        [DataRow(1, null, true)]
        [DataRow(2, TypeCode.Boolean, true)]
        [DataRow(2, TypeCode.Empty, false)]
        [DataRow(2, TypeCode.String, true)]
        [DataRow(2, null, false)]
        [DataRow(3, TypeCode.Boolean, true)]
        [DataRow(3, TypeCode.Empty, false)]
        [DataRow(3, TypeCode.String, true)]
        [DataRow(3, null, false)]
        public void CanExecuteTest(int iVal,TypeCode? tC,bool xExp)
        {
            var f = iVal switch { 
                1 => _testCommand1.CanExecute, 
                2 => _testCommand2.CanExecute,
                3 => _testCommand3.CanExecute,
                _ => (Func<object?,bool>)(o => false) };
            Assert.AreEqual(xExp,f(tC));
        }

        [DataTestMethod()]
        [DataRow(0, null, new[] { ""})]
        [DataRow(1, TypeCode.Boolean, new[] { "Execute(_testCommand1,Boolean)\r\n" })]
        [DataRow(1, TypeCode.Empty, new[] { "Execute(_testCommand1,Empty)\r\n" })]
        [DataRow(1, TypeCode.String, new[] { "Execute(_testCommand1,String)\r\n" })]
        [DataRow(1, null, new[] { "Execute(_testCommand1,)\r\n" })]
        [DataRow(2, TypeCode.Boolean, new[] { "Execute(_testCommand2,Boolean)\r\n" })]
        [DataRow(2, TypeCode.Empty, new[] { "Execute(_testCommand2,Empty)\r\n" })]
        [DataRow(2, TypeCode.String, new[] { "Execute(_testCommand2,String)\r\n" })]
        [DataRow(2, null, new[] { "Execute(_testCommand2,)\r\n" })]
        [DataRow(3, TypeCode.Boolean, new[] { "Execute(_testCommand3,Boolean)\r\n" })]
        [DataRow(3, TypeCode.Empty, new[] { "Execute(_testCommand3,Empty)\r\n" })]
        [DataRow(3, TypeCode.String, new[] { "Execute(_testCommand3,String)\r\n" })]
        [DataRow(3, null, new[] { "Execute(_testCommand3,)\r\n" })]
        public void ExecuteTest(int iVal, TypeCode? tC, string[] asExp)
        {
            (iVal switch
            {
                1 => _testCommand1.Execute,
                2 => _testCommand2.Execute,
                3 => _testCommand3.Execute,
                _ => (Action<object?>)(o => { })
            }).Invoke(tC);
            Assert.AreEqual(asExp[0], DebugLog);
        }

        [DataTestMethod()]
        [DataRow(0, new[] { "" })]
        [DataRow(1, new[] { "CanExecuteChanged(MVVM.ViewModel.DelegateCommand)\r\n" })]
        [DataRow(2, new[] { "CanExecuteChanged(MVVM.ViewModel.DelegateCommand`1[System.Nullable`1[System.TypeCode]])\r\n" })]
        [DataRow(3, new[] { "" })]
        public void NotifyCanExecuteChangedTest(int iVal, string[] asExp)
        {
            (iVal switch
            {
                1 => _testCommand1.NotifyCanExecuteChanged,
                2 => _testCommand2.NotifyCanExecuteChanged,
                3 => _testCommand3.NotifyCanExecuteChanged,
                _ => (Action)(() => { })
            }).Invoke();
            Assert.AreEqual(asExp[0],DebugLog);
        }
    }
}