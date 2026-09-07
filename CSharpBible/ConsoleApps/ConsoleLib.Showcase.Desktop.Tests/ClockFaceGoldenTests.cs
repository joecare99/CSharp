using System;
using System.Collections.Generic;
using System.Linq;
using ConsoleLib.Showcase.Desktop.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConsoleLib.Showcase.Desktop.Tests;

/// <summary>Golden snapshots for the fixed-width clock face.</summary>
[TestClass]
public sealed class ClockFaceGoldenTests
{
    [TestMethod]
    [DataRow(0, 0, 0, "^", "^", "00:00:00", "00")]
    [DataRow(6, 15, 15, "v", ">", "06:15:15", "15")]
    [DataRow(12, 30, 30, "^", "v", "12:30:30", "30")]
    [DataRow(23, 59, 59, "^", "^", "23:59:59", "59")]
    public void FaceMatchesGoldenSnapshot(int hour, int minute, int second, string hourHand, string minuteHand, string timeText, string secondText)
    {
        using var clock = new ClockViewModel(new DateTime(2026, 9, 1, hour, minute, second));

        var actual = clock.Face.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
        var expected = ExpectedFace(hourHand, minuteHand, timeText, secondText).ToArray();
        Assert.AreEqual(9, actual.Length, string.Join("|", actual));
        Assert.AreEqual(9, expected.Length, string.Join("|", expected));

        Assert.AreEqual(string.Join(Environment.NewLine, expected), clock.Face);
    }

    [TestMethod]
    public void FaceGoldenContractUsesNineRowsAndThirtyTwoColumns()
    {
        using var clock = new ClockViewModel(new DateTime(2026, 9, 1, 10, 15, 45));

        var lines = clock.Face.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

        Assert.AreEqual(9, lines.Length);
        Assert.IsTrue(lines.All(line => line.Length == 32));
        StringAssert.Contains(lines[8], "H:\\  M:>  S:45");
    }

    private static IReadOnlyList<string> ExpectedFace(string hourHand, string minuteHand, string timeText, string second)
    {
        return new[]
        {
            "          .--------.            ",
            "       .-'     12     '-.       ",
            $"     .'    {timeText}    '.       ",
            "    /    9      +      3   \\    ",
            $"   |        H{hourHand}  +  M{minuteHand}        |     ",
            "    \\                    /      ",
            "     '.        6       .'       ",
            "       '-.          .-'         ",
            $"          H:{hourHand}  M:{minuteHand}  S:{second}"
        }.Select(Normalize).ToArray();
    }

    private static string Normalize(string line) =>
        line.Length >= 32 ? line[..32] : line.PadRight(32);
}
