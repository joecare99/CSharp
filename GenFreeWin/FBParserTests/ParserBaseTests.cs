using FBParser;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FBParserTests;

[TestClass]
public sealed class ParserBaseTests
{
    [TestMethod]
    public void Reset_ResetsTrackedPositionAndBuffer()
    {
        var sut = new TestParserBase();
        sut.SetState(5, 42, "buffer");

        sut.Reset();

        var position = sut.GetPos();
        Assert.AreEqual(1L, position.LineNo);
        Assert.AreEqual(0L, position.Offset);
        Assert.AreEqual(string.Empty, sut.Buffer);
    }

    [TestMethod]
    public void GetPos_ReturnsTrackedState()
    {
        var sut = new TestParserBase();
        sut.SetState(7, 12, "abc");

        var position = sut.GetPos();

        Assert.AreEqual(7L, position.LineNo);
        Assert.AreEqual(12L, position.Offset);
    }

    private sealed class TestParserBase : ParserBase
    {
        public string Buffer => FData;

        public void SetState(long lineNo, long offset, string data)
        {
            LineNo = lineNo;
            Offset = offset;
            FData = data;
        }

        public override void Feed(string text)
        {
        }

        public override void Error(object? sender, string message)
        {
        }

        public override void Warning(object? sender, string message)
        {
        }
    }
}
