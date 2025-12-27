using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TileSetAnimator.Models;
using TileSetAnimator.Services;

namespace TileSetAnimator.Tests.Services;

[TestClass]
public class ContiguousAnimationDetectionServiceTests
{
    [TestMethod]
    public void DetectAnimations_GroupsContiguousTiles()
    {
        var tiles = new List<TileDefinition>
        {
            new(0, 0, 0, new Int32Rect(0, 0, 16, 16), "A", string.Empty, TileCategory.Unknown, string.Empty),
            new(1, 0, 1, new Int32Rect(16, 0, 16, 16), "B", string.Empty, TileCategory.Unknown, string.Empty),
            new(2, 0, 2, new Int32Rect(32, 0, 16, 16), "C", string.Empty, TileCategory.Unknown, string.Empty),
            new(3, 1, 0, new Int32Rect(0, 16, 16, 16), "D", string.Empty, TileCategory.Unknown, string.Empty),
        };

        var service = new ContiguousAnimationDetectionService();
        var animations = service.DetectAnimations(tiles, minimumFrameCount: 2, TimeSpan.FromMilliseconds(100));

        Assert.HasCount(1, animations);
        Assert.HasCount(3, animations[0].Frames);
        CollectionAssert.AreEqual(new[] { 0, 1, 2 }, animations[0].Frames.Select(f => f.Index).ToArray());
    }
}
