using CommunityToolkit.Mvvm.DependencyInjection;
using CSharpBible.Calc32;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Calc32.Tests
{
    class testFrm : System.Windows.Forms.Form
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Close();
        }
    }
    [TestClass]
    public class ProgramTests
    {
        [TestMethod]
        public void MainTest()
        {
            var f = Program.Init;
            Program.Init = static () =>
            {
                var sp = new ServiceCollection()
                 .AddTransient<System.Windows.Forms.Form, testFrm>()
                 .BuildServiceProvider();
                Ioc.Default.ConfigureServices(sp);
            };
            Program.Main();
        }
    }
}
