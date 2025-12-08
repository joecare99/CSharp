using Microsoft.VisualStudio.TestTools.UnitTesting;
using AA15_Labyrinth.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AA15_Labyrinth.Model.Tests
{
    [TestClass()]
    public class LabyrinthGeneratorTests
    {
        [TestMethod()]
        [DataRow(0, 0, 2, 0, new long[] { 0 , 4294967296L, 8589934592L})]
        [DataRow(1, 1, 3, 2, new long[] { 4294967297L, 8589934593L, 8589934594L, 12884901890L })]
        [DataRow(1, 1, 3, 0, new long[] { 4294967297L, 8589934592L, 8589934593L, 12884901888L })]
        public void AddEdgeToBucketsTest(int ax, int ay, int bx, int by, long[] expected)
        {
            // Arrange
            Dictionary<long, List<int>> buckets = new Dictionary<long, List<int>>();
            int edgeIndex = 0;

            // Act
            LabyrinthGenerator.AddEdgeToBuckets(edgeIndex, ax, ay, bx, by, buckets);

            // Assert
            Assert.AreEqual(expected.Length, buckets.Count);
            var keys = buckets.Keys.OrderBy(k => k).ToArray();
            CollectionAssert.AreEqual(expected, keys);
        }

        [DataTestMethod]
        // Proper crossing
        [DataRow(true, 0, 0, 4, 4, 0, 4, 4, 0)]
        // Disjoint separate
        [DataRow(false, 0, 0, 1, 0, 2, 0, 3, 0)]
        // Touch at endpoint
        [DataRow(true, 0, 0, 4, 0, 4, 0, 4, 4)]
        // Collinear overlapping
        [DataRow(true, 0, 0, 4, 0, 2, 0, 6, 0)]
        // Collinear touching at endpoint
        [DataRow(true, 0, 0, 2, 0, 2, 0, 5, 0)]
        // Collinear disjoint
        [DataRow(false, 0, 0, 2, 0, 3, 0, 5, 0)]
        // Parallel non-collinear
        [DataRow(false, 0, 0, 0, 4, 1, 0, 1, 4)]
        // T-junction (internal point on other segment)
        [DataRow(true, 0, 0, 4, 0, 2, -2, 2, 2)]
        // Shared start point
        [DataRow(true, 1, 1, 3, 3, 1, 1, 3, 1)]
        // Degenerate: point on segment
        [DataRow(true, 1, 1, 1, 1, 0, 1, 3, 1)]
        // Degenerate: point away from segment
        [DataRow(false, 1, 1, 1, 1, 2, 2, 3, 3)]
        // Large coordinates crossing (tests overflow safety)
        [DataRow(true, 0, 0, 1000000000, 0, 500000000, -1, 500000000, 1)]
        public void SegmentsIntersect_Cases(
    bool expected,
    int ax, int ay, int bx, int by,
    int cx, int cy, int dx, int dy)
        {
            bool actual = LabyrinthGenerator.SegmentsIntersect(ax, ay, bx, by, cx, cy, dx, dy);
            Assert.AreEqual(expected, actual,
                $"Erwartet={expected} für AB=({ax},{ay})->({bx},{by}) und CD=({cx},{cy})->({dx},{dy}).");
        }

        [DataTestMethod]
        // Reuse a subset of above cases to validate symmetry and endpoint reversal invariants
        [DataRow(true, 0, 0, 4, 4, 0, 4, 4, 0)]     // proper crossing
        [DataRow(false, 0, 0, 2, 0, 3, 0, 5, 0)]     // collinear disjoint
        [DataRow(true, 0, 0, 4, 0, 4, 0, 4, 4)]     // touch at endpoint
        [DataRow(true, 0, 0, 4, 0, 2, -2, 2, 2)]     // T-junction
        public void SegmentsIntersect_Invariance_SwapAndReverse(
            bool expected,
            int ax, int ay, int bx, int by,
            int cx, int cy, int dx, int dy)
        {
            bool ab_cd = LabyrinthGenerator.SegmentsIntersect(ax, ay, bx, by, cx, cy, dx, dy);
            bool cd_ab = LabyrinthGenerator.SegmentsIntersect(cx, cy, dx, dy, ax, ay, bx, by);
            bool ba_cd = LabyrinthGenerator.SegmentsIntersect(bx, by, ax, ay, cx, cy, dx, dy);
            bool ab_dc = LabyrinthGenerator.SegmentsIntersect(ax, ay, bx, by, dx, dy, cx, cy);

            Assert.AreEqual(expected, ab_cd, "Asymmetrie: ursprüngliche Reihenfolge abweicht.");
            Assert.AreEqual(ab_cd, cd_ab, "Asymmetrie: Vertauschung der Segmente liefert anderen Wert.");
            Assert.AreEqual(ab_cd, ba_cd, "Asymmetrie: Umkehrung von AB liefert anderen Wert.");
            Assert.AreEqual(ab_cd, ab_dc, "Asymmetrie: Umkehrung von CD liefert anderen Wert.");
        }

    }
}