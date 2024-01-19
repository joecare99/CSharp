using Microsoft.VisualStudio.TestTools.UnitTesting;
using MdbBrowser.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace MdbBrowser.Views.Tests
{
    [TestClass()]
    public class TableViewTests
    {
        [TestMethod()]
        public void TableViewTest()
        {
            TableView? mw = null;
            var t = new Thread(() => mw = new());
            t.SetApartmentState(ApartmentState.STA); //Set the thread to STA
            t.Start();
            t.Join(); //Wait for the thread to end
            Assert.IsNotNull(mw);
            Assert.IsInstanceOfType(mw, typeof(TableView));
        }
    }
}