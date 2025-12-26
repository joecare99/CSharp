using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TileSetAnimator.Models;
using TileSetAnimator.Services;

namespace TileSetAnimator.Tests.Services;

[TestClass]
public class TileSetServiceTests
{
    [TestMethod]
    public void SliceTiles_UsesGridSettingsToCreateExpectedTiles()
    {
        var service = new TileSetService();
        var bitmap = BitmapSource.Create(
            48,
            32,
            96,
            96,
            PixelFormats.Gray8,
            null,
            new byte[48 * 32],
            48);

        var settings = new TileGridSettings(16, 16, 0, 0);

        var tiles = service.SliceTiles(bitmap, settings);

        Assert.HasCount(6, tiles);
        Assert.IsTrue(tiles.All(t => t.Bounds.Width == 16 && t.Bounds.Height == 16));
        Assert.AreEqual(0, tiles[0].Column);
        Assert.AreEqual(2, tiles[2].Column);
        Assert.AreEqual(1, tiles[3].Row);
    }
}
