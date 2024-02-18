using Microsoft.VisualStudio.TestTools.UnitTesting;
using MVVM_06_Converters_4.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVM_06_Converters_4.View.Tests
{
    [TestClass()]
    public class PlotFrameTests
    {
        PlotFrame testView = new PlotFrame();

        [TestInitialize]
        public void TestInitialize()
        {
            testView = new PlotFrame();
            testView.Visibility = System.Windows.Visibility.Visible;
        }

        [TestMethod()]
        public void PlotFrameTest()
        {
            Assert.IsNotNull(testView);
        }

        [TestMethod()]
        public void OnSizeChangeTest()
        {
            testView.Height = testView.Height+2;
        }

        [TestMethod()]
        public void OnVInitializedTest()
        {
           // testView.OnInitialized(;
        }
    }
}