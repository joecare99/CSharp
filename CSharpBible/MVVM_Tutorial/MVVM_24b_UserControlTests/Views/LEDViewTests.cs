using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using System.Windows.Media;

namespace MVVM_24b_UserControl.Views.Tests
{
    [TestClass()]
    public class LEDViewTests
	{
        [TestMethod()]
        public void LEDViewTest()
        {
			LEDView? mw=null;
            Brush?[] asIO = new Brush[3];
            asIO[1] = Brushes.SteelBlue;
            var t = new Thread(() =>
            {
                mw = new();
                asIO[0] = mw.LEDBrush;
                mw.LEDBrush = asIO[1]!;
                asIO[2] = mw.LEDBrush;
                return;
            });
            t.SetApartmentState(ApartmentState.STA); //Set the thread to STA
            t.Start();
            t.Join(); //Wait for the thread to end
            Assert.IsNotNull(mw!);
            Assert.IsInstanceOfType(mw, typeof(LEDView)); 
            Assert.AreEqual(Brushes.Pink, asIO[0]);
            Assert.AreEqual(Brushes.SteelBlue, asIO[2]);
        }
    }
}
