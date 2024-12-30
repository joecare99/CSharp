using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;

namespace MVVM_34_BindingEventArgs.Views.Tests
{
    [TestClass()]
    public class EventBindingViewTests
    {
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
        EventBindingView testView;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.

        [TestInitialize]
        public void Init()
        {
            var t = new Thread(() => testView = new());
            t.SetApartmentState(ApartmentState.STA); //Set the thread to STA
            t.Start();
            t.Join(); //Wait for the thread to end
        }

        [TestMethod()]
        public void MainWindowTest()
        {
            Assert.IsNotNull(testView);
            Assert.IsInstanceOfType(testView, typeof(EventBindingView));    
        }
    }
}