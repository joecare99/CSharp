using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;

namespace MVVM_24c_CTUserControl.Views.Tests
{
    [TestClass()]
    public class MaxLengthTBUserControlTests
    {
        [TestMethod()]
        [STAThread]
        public void MaxLengthTBUserControlTest()
        {
			MaxLengthTextBoxUserControl? mw=null;
            string?[] asIO = {"", "", "10", "20","","" };
            var t = new Thread(() =>
            {
                mw = new();
                asIO[0] = mw.Text;
                asIO[1] = mw.Caption;
                mw.Text = asIO[2];
                mw.Caption = asIO[3];
                asIO[4] = mw.Text;
                asIO[5] = mw.Caption;
                return;
            });
            t.SetApartmentState(ApartmentState.STA); //Set the thread to STA
            t.Start();
            t.Join(); //Wait for the thread to end
            Assert.IsNotNull(mw!);
            Assert.IsInstanceOfType(mw, typeof(MaxLengthTextBoxUserControl)); 
            Assert.AreEqual(0, mw.MaxLength);
            Assert.AreEqual(null, asIO[0]);
            Assert.AreEqual(null, asIO[1]);
            Assert.AreEqual("10", asIO[4]);
            Assert.AreEqual("20", asIO[5]);
        }
    }
}
