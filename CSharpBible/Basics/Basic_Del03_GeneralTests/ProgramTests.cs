using BaseLib.Model.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Basic_Del03_General;
using System;
using System.Windows.Input;

namespace Basic_Del03_GeneralTests
{
    [TestClass]
    public class ProgramTests :BaseTest,ICommand
    {
        private Action<string[]>? _OldRun;
        private Func<string[],bool>? _oldInit;
        private bool xInitResult = false;

        public event EventHandler? CanExecuteChanged;
        [TestInitialize]
        public void Init() {
            _OldRun=Program.Run;
            _oldInit=Program.Init;
            try { _oldInit(Array.Empty<string>()); } catch { }
            Program.Init = MyInit;
            Program.Run = MyRun;
        }
        public bool CanExecute(object? parameter) => MyInit(parameter as string[]);
        public void Execute(object? parameter)=> MyRun(parameter as string[]);

        private void MyRun(string[] obj)
        {
            DoLog($"MyRun({string.Join(", ", obj)})");
        }

        private bool MyInit(string[] arg)
        {
            DoLog($"MyInit({string.Join(", ", arg)})={xInitResult}");
            return xInitResult;
        }

        [TestCleanup]
        public void CleanUp()
        {
            Program.Init = _oldInit;
            Program.Run = _OldRun;
        }

        [DataTestMethod]
        [DataRow(new[] { "" }, false, new[] { @"MyInit()=False
" })]
        [DataRow(new[] { "" }, true, new[] { @"MyInit()=True
MyRun()
" })]
        [DataRow(new[] { "Hello","World" }, false, new[] { @"MyInit(Hello, World)=False
" })]
        [DataRow(new[] { "Stormy", "weather", "!" }, true, new[] { @"MyInit(Stormy, weather, !)=True
MyRun(Stormy, weather, !)
" })]
        public void MainTest(string[] args,bool xVal, string[] asExp) {
            xInitResult = xVal;
            Program.Main(args);
            Assert.AreEqual(asExp[0],DebugLog);
        }

        [DataTestMethod]
        [DataRow(new[] { "" },true,  new[] { @"MyRun()
" })]
        [DataRow(new[] { "" }, false,  new[] { @"" })]
        [DataRow(new[] { "Hello","World" }, true, new[] { @"MyRun(Hello, World)
" })]
        [DataRow(new[] { "Stormy","weather","!" }, false, new[] { @"" })]
        public void RunTest(string[] args, bool xSet, string[] asExp)
        {
            Program.SetView(xSet?this:null!);
            _OldRun(args);
            Assert.AreEqual(asExp[0], DebugLog);
        }


    }
}
