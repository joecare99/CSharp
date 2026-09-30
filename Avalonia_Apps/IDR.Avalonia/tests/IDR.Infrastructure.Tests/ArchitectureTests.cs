using IDR.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IDR.Infrastructure.Tests;

[TestClass]
public sealed class ArchitectureTests
{
    [TestMethod]
    public void IcedDecoderIsAvailable()
    {
        ArchitectureMarker marker = new();
        Assert.AreEqual(32, marker.Decoder.Bitness);
    }
}
