using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Osb.Core.Imaging;

namespace Osb.Core.Tests.Imaging;

[TestClass]
public sealed class ImageLayoutCalculatorTests
{
    [TestMethod]
    [DataRow(800, 600, 400, 400, false, 400, 300)]
    [DataRow(600, 800, 400, 400, false, 300, 400)]
    [DataRow(400, 400, 400, 400, false, 400, 400)]
    [DataRow(100, 50, 400, 400, false, 100, 50)]
    [DataRow(100, 50, 400, 400, true, 400, 200)]
    public void CalculateContainedSize_PreservesLegacyAspectRatioBehavior(
        int sourceWidth,
        int sourceHeight,
        int maximumWidth,
        int maximumHeight,
        bool stretch,
        int expectedWidth,
        int expectedHeight)
    {
        var actual = ImageLayoutCalculator.CalculateContainedSize(
            new ImageSize(sourceWidth, sourceHeight),
            maximumWidth,
            maximumHeight,
            stretch);

        Assert.AreEqual(new ImageSize(expectedWidth, expectedHeight), actual);
    }

    [TestMethod]
    [DataRow(800, 600, 300, 400, 300)]
    [DataRow(600, 800, 300, 225, 300)]
    public void CalculateSizeForHeight_PreservesLegacyAspectRatioBehavior(
        int sourceWidth,
        int sourceHeight,
        int targetHeight,
        int expectedWidth,
        int expectedHeight)
    {
        var actual = ImageLayoutCalculator.CalculateSizeForHeight(
            new ImageSize(sourceWidth, sourceHeight),
            targetHeight);

        Assert.AreEqual(new ImageSize(expectedWidth, expectedHeight), actual);
    }

    [TestMethod]
    [DataRow(0, 1)]
    [DataRow(1, 0)]
    [DataRow(-1, 1)]
    public void ImageSize_RejectsNonPositiveDimensions(int width, int height)
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ImageSize(width, height));
    }

    [TestMethod]
    [DataRow(0, 100)]
    [DataRow(100, 0)]
    [DataRow(-1, 100)]
    public void CalculateContainedSize_RejectsNonPositiveMaximumDimensions(
        int maximumWidth,
        int maximumHeight)
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
            ImageLayoutCalculator.CalculateContainedSize(new ImageSize(100, 100), maximumWidth, maximumHeight));
    }
}
