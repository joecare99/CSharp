using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;

namespace MVVM_24a_CTUserControl.Views.Tests
{
    [TestClass()]
    public class MaxLengthTBUserControlTests
    {
        [TestMethod()]
        public void MaxLengthTBUserControlTest()
        {
			MaxLengthTextBoxUserControl? mw=null;
            string?[] asIO = {".", ".",".", "10", "20","30","","","" };
            var t = new Thread(() =>
            {
                mw = new();
                asIO[0] = mw.Text;
                asIO[1] = mw.Caption;
                asIO[2] = mw.TextHint; 
                mw.Text = asIO[3];
                mw.Caption = asIO[4];
                mw.TextHint = asIO[5];
                asIO[6] = mw.Text;
                asIO[7] = mw.Caption;
                asIO[8] = mw.TextHint;
                return;
            });
            t.SetApartmentState(ApartmentState.STA); //Set the thread to STA
            t.Start();
            t.Join(); //Wait for the thread to end
            Assert.IsNotNull(mw!);
            Assert.IsInstanceOfType(mw, typeof(MaxLengthTextBoxUserControl)); 
            Assert.AreEqual(0, mw.MaxLength);
            Assert.AreEqual(null, asIO[0]);
            Assert.AreEqual("", asIO[1]);
            Assert.AreEqual("", asIO[2]);
            Assert.AreEqual("10", asIO[6]);
            Assert.AreEqual("20", asIO[7]);
            Assert.AreEqual("30", asIO[8]);
        }
    }
}
