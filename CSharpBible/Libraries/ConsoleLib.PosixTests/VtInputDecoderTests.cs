using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ConsoleLib.Interfaces;
#if NET5_0_OR_GREATER
using ConsoleLib.Posix;
#endif

namespace ConsoleLibTests;

[TestClass]
public sealed class VtInputDecoderTests
{
#if NET5_0_OR_GREATER

    [TestMethod]
    public void DecodesCommonVtInput()
    {
        var decoder = new VtInputDecoder();
        var keys = decoder.Decode("\u001b[A\u001b[B\u001b[C\u001b[D\r\u001b\t\bZ").ToArray();

        Assert.AreEqual(9, keys.Length);
        Assert.AreEqual(ConsoleKey.UpArrow, keys[0].Key);
        Assert.AreEqual(ConsoleKey.DownArrow, keys[1].Key);
        Assert.AreEqual(ConsoleKey.RightArrow, keys[2].Key);
        Assert.AreEqual(ConsoleKey.LeftArrow, keys[3].Key);
        Assert.AreEqual(ConsoleKey.Enter, keys[4].Key);
        Assert.AreEqual(ConsoleKey.Escape, keys[5].Key);
        Assert.AreEqual(ConsoleKey.Tab, keys[6].Key);
        Assert.AreEqual(ConsoleKey.Backspace, keys[7].Key);
        Assert.AreEqual('Z', keys[8].KeyChar);
        Assert.IsTrue(keys.All(static key => key.IsKeyDown));
    }

    [TestMethod]
    public void HandlesSequencesSplitAcrossReadsAndFlushesEscape()
    {
        var decoder = new VtInputDecoder();

        Assert.AreEqual(0, decoder.Decode("\u001b").Count);
        Assert.AreEqual(ConsoleKey.UpArrow, decoder.Decode("[A").Single().Key);
        Assert.AreEqual(ConsoleKey.Escape, decoder.Decode("\u001b").Count == 0
            ? decoder.Flush().Single().Key
            : ConsoleKey.NoName);
    }
#endif
}
