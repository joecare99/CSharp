using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Osb.Core.Selection;

namespace Osb.Core.Tests.Selection;

[TestClass]
public sealed class PlaceSelectionTextCodecTests
{
    [TestMethod]
    public void Serialize_PreservesOrderAndUsesDeterministicCrLf()
    {
        var actual = PlaceSelectionTextCodec.Serialize(
            new[] { "Musterort                                        42", "Nebenort                                         8" });

        Assert.AreEqual(
            "Musterort                                        42\r\nNebenort                                         8",
            actual);
    }

    [TestMethod]
    public void Deserialize_AcceptsLegacyLineEndingsAndDropsTrailingLines()
    {
        var actual = PlaceSelectionTextCodec.Deserialize("Ort A\r\nOrt B\nOrt C\r\n\r\n");

        CollectionAssert.AreEqual(new[] { "Ort A", "Ort B", "Ort C" }, actual.ToArray());
    }

    [TestMethod]
    public void SerializeAndDeserialize_PreservesUnicodeSelectionEntries()
    {
        var expected = new[] { "München", "Łódź", "São Paulo" };

        var serialized = PlaceSelectionTextCodec.Serialize(expected);
        var actual = PlaceSelectionTextCodec.Deserialize(serialized);

        CollectionAssert.AreEqual(expected, actual.ToArray());
    }

    [TestMethod]
    public void Serialize_RejectsInvalidEntries()
    {
        Assert.ThrowsExactly<ArgumentException>(() => PlaceSelectionTextCodec.Serialize(new[] { string.Empty }));
        Assert.ThrowsExactly<ArgumentException>(() => PlaceSelectionTextCodec.Serialize(new[] { "Ort A\nOrt B" }));
    }

    [TestMethod]
    public void Deserialize_RejectsEmptyInteriorLines()
    {
        Assert.ThrowsExactly<ArgumentException>(() => PlaceSelectionTextCodec.Deserialize("Ort A\r\n\r\nOrt B"));
    }
}
