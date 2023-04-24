using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Drawing;

namespace Sokoban_Base.Model.Tests
{
    [TestClass()]
    public class LabDefsTests
    {
        [TestMethod()]
        public void GetLevelTest()
        {
            (var fd, var s) = LabDefs.GetLevel(1);
            Assert.AreEqual(new Size(14,10),s);
            Assert.AreEqual(140, fd.Length);
            Assert.AreEqual(FieldDef.Wall, fd[0]);
        }

        [TestMethod()]
        public void CountTest()
        {
            Assert.AreEqual(90,LabDefs.Count);
        }

    }
}