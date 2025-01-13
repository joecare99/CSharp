using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;

namespace VBUnObfusicator.Views.Tests
{
    [TestClass()]
    public class CodeUnObFusViewTests
    {
        [TestMethod()]
        public void CodeUnObFusViewTest()
        {
            CodeUnObFusView? mw=null;
            var t = new Thread(()=> mw = new());
            t.SetApartmentState(ApartmentState.STA); //Set the thread to STA
            t.Start();
            t.Join(); //Wait for the thread to end
            Assert.IsNotNull(mw);
            Assert.IsInstanceOfType(mw, typeof(CodeUnObFusView));    
        }
    }
}