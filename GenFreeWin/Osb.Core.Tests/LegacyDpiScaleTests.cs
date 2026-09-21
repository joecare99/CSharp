using Microsoft.VisualStudio.TestTools.UnitTesting;
using Osb.Core;

namespace Osb.Core.Tests;

[TestClass]
public sealed class LegacyDpiScaleTests
{
    [TestMethod]
    public void TwipsPerPixel_DefaultsTo96DpiLegacyBehavior()
    {
        Assert.AreEqual(15.0, LegacyDpiScale.TwipsPerPixel(), 0.0001);
    }

    [TestMethod]
    public void TwipsToPixels_UsesCurrentDeviceDpi()
    {
        Assert.AreEqual(192.0, LegacyDpiScale.TwipsToPixels(1440.0, 192), 0.0001);
    }

    [TestMethod]
    public void PixelsToTwips_UsesCurrentDeviceDpi()
    {
        Assert.AreEqual(7500.0, LegacyDpiScale.PixelsToTwips(1000.0, 192), 0.0001);
    }
}
