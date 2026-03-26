using Microsoft.VisualStudio.TestTools.UnitTesting;
using JCAMS.Core.Components.Coloring;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace JCAMS.Core.Components.Coloring.Tests
{
    [TestClass()]
    public class CColorCubeTests
    {
        [TestMethod()]
        public void CColorCubeTest()
        {
            var colorCube = new CColorCube();
            Assert.IsNotNull(colorCube);
            var colorCube2 = CColorCube.Empty;
            Assert.IsNotNull(colorCube2);
            var colorCube3 = CColorCube.Create();
            Assert.IsNotNull(colorCube3);
            var colorCube4 = CColorCube.Create(7,"SomeCube", Color.Transparent, Color.Red, Color.Lime, Color.Blue);
            Assert.IsNotNull(colorCube3);

            var colorCube5 = new CColorCube(5);
            Assert.IsNotNull(colorCube5);
            var colorCube6 = new CColorCube(6,"Cube6");
            Assert.IsNotNull(colorCube6);

        }

        [TestMethod()]
        public void CColorCubeTest1()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void CColorCubeTest2()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void CColorCubeTest3()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void CreateTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void CreateTest1()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void SaveConfigurationTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void LoadConfigurationTest()
        {
            Assert.Fail();
        }
    }
}