using Calc32WPF.Views;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;

namespace Calc32WPF.Tests
{
    [TestClass()]
    public class MainWindowTests
    {
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Fügen Sie ggf. den „erforderlichen“ Modifizierer hinzu, oder deklarieren Sie den Modifizierer als NULL-Werte zulassend.
        MainWindow testView;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Fügen Sie ggf. den „erforderlichen“ Modifizierer hinzu, oder deklarieren Sie den Modifizierer als NULL-Werte zulassend.

        [TestInitialize]
        public void Init()
        {
            var t=new Thread(()=>
            testView = new MainWindow());
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            t.Join();
        }

        [TestMethod()]
        public void MainWindowTest()
        {
            Assert.IsNotNull(testView);
        }
    }
}