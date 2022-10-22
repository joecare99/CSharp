using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sokoban_Base.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sokoban_Base.Model.Tests
{
    /// <summary>
    /// Defines test class OffsetsTests.
    /// </summary>
    [TestClass()]
    public class OffsetsTests
    {
        /// <summary>
        /// Dirs the offset test.
        /// </summary>
        /// <param name="dir">The dir.</param>
        /// <param name="Koor">The koor.</param>
        [DataTestMethod()]
        [DataRow(Direction.North,new int[] { 0, -1 })]
        [DataRow(Direction.West, new int[] { -1, 0 })]
        [DataRow(Direction.South, new int[] { 0, 1 })]
        [DataRow(Direction.East, new int[] { 1, 0 })]
        public void DirOffsetTest(Direction dir,int[] Koor)
        {
            Assert.AreEqual(new Point(Koor[0], Koor[1]), Offsets.DirOffset(dir));
        }

        /// <summary>
        /// Dirs the offset test1.
        /// </summary>
        /// <param name="dir">The dir.</param>
        /// <param name="Koor">The koor.</param>
        /// <param name="Pnkt">The PNKT.</param>
        [DataTestMethod()]
        [DataRow(Direction.North, new int[] { 4, 6 }, new int[] { 4, 7 })]
        [DataRow(Direction.West, new int[] { 4, 8 }, new int[] { 5, 8 })]
        [DataRow(Direction.South, new int[] { 6, 10 }, new int[] { 6, 9 })]
        [DataRow(Direction.East, new int[] { 8, 10 }, new int[] { 7, 10 })]
        [DataRow(null, new int[] { 8, 12 }, new int[] { 8, 12 })]
        public void DirOffsetTest1(Direction? dir, int[] Koor, int[] Pnkt)
        {
            Assert.AreEqual(new Point(Koor[0], Koor[1]), Offsets.DirOffset(dir, new Point(Pnkt[0], Pnkt[1])));
        }
    }
}