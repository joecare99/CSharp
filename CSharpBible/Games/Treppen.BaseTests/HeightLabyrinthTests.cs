using System.Drawing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Treppen.Base;

namespace Treppen.BaseTests;

[TestClass]
public class HeightLabyrinthTests
{
    [TestMethod]
    public void Create_Default_IsEmpty()
    {
        var hl = new HeightLabyrinth { Dimension = new Rectangle(0, 0, 3, 3) };
        for (int x = 0; x < 3; x++) for (int y = 0; y < 3; y++) Assert.AreEqual(0, hl[x, y]);
    }

    [TestMethod]
    public void BaseLevel_Is_DiagonalGradient()
    {
        var hl = new HeightLabyrinth { Dimension = new Rectangle(0, 0, 3, 3) };
        Assert.AreEqual(1, hl.BaseLevel(0, 0));
        Assert.IsTrue(hl.BaseLevel(2, 2) > hl.BaseLevel(0, 0));
    }

    [TestMethod]
    public void Generate_Fills_All_Cells()
    {
        var hl = new HeightLabyrinth { Dimension = new Rectangle(0, 0, 8, 8) };
        hl.Generate();
        for (int x = 0; x < 8; x++) for (int y = 0; y < 8; y++) Assert.AreNotEqual(0, hl[x, y]);
    }

    [TestMethod]
    public void Generate_Raises_UpdateCell_Event()
    {
        var hl = new HeightLabyrinth { Dimension = new Rectangle(0, 0, 8, 8) };
        bool eventRaised = false;
        hl.UpdateCell += (s, p) => { eventRaised = true; };
        hl.Generate();
        Assert.IsTrue(eventRaised);
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void Save_And_Load_Maintains_Data(bool xBin)
    {
        var hl = new HeightLabyrinth { Dimension = new Rectangle(0, 0, 5, 5) };
        hl.Generate();
        using var ms = new System.IO.MemoryStream();
        hl.SaveToStream(ms,xBin);
        ms.Position = 0;
        var hl2 = new HeightLabyrinth { Dimension = new Rectangle(0, 0, 5, 5) };
        bool loaded = hl2.LoadFromStream(ms);
        Assert.IsTrue(loaded);
        for (int x = 0; x < 5; x++) for (int y = 0; y < 5; y++) Assert.AreEqual(hl[x, y], hl2[x, y]);
    }
}
