using System;
using System.Drawing;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Osb.Core.Media;

namespace Osb.Core.Tests.Media;

[TestClass]
public sealed class BildausTests
{
    [TestMethod]
    public void PathResolver_ResolvesHashRelativePath()
    {
        var entry = new BildausEntry { Path = "#subdir", FileName = "pic.jpg" };
        var baseDir = Path.Combine(Path.GetTempPath(), "baseDir");
        var expected = Path.GetFullPath(Path.Combine(baseDir, "subdir", "pic.jpg"));

        var actual = BildausPathResolver.ResolvePath(entry, baseDir);

        Assert.AreEqual(expected, Path.GetFullPath(actual));
    }

    [TestMethod]
    public void Filter_PortraitUsesIncludeDescriptions()
    {
        var entry = new BildausEntry { Description = "Personenbild" };
        var optionsYes = new BildausOptions(IncludePictures: false, IncludeDescriptions: true, TargetHeight: 0);
        var optionsNo = new BildausOptions(IncludePictures: true, IncludeDescriptions: false, TargetHeight: 0);

        Assert.IsTrue(BildausFilter.MatchesOptions(entry, optionsYes));
        Assert.IsFalse(BildausFilter.MatchesOptions(entry, optionsNo));
    }

    [TestMethod]
    public void CaptionBuilder_PrependsNewlineWhenPresent()
    {
        var entry = new BildausEntry { Description = "MyDesc", Remark = "MyRemark" };
        var (caption, remark) = BildausCaptionBuilder.Build(entry);

        Assert.AreEqual("\nMyDesc", caption);
        Assert.AreEqual("\nMyRemark", remark);
    }

    [TestMethod]
    public void Processor_LoadsAndResizesImage_WhenFileExists()
    {
        // create temporary image file
        var tempDir = Path.Combine(Path.GetTempPath(), "BildausTests", Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        var filePath = Path.Combine(tempDir, "test.png");

        using (var bmp = new Bitmap(100, 80))
        using (var g = Graphics.FromImage(bmp))
        {
            g.Clear(Color.AliceBlue);
            bmp.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);
        }

        try
        {
            var entry = new BildausEntry { Path = tempDir, FileName = "test.png", Description = "D" };
            var repository = new TestRepository(entry);
            var processor = new BildausProcessor(repository, pictureBaseDirectory: tempDir);

            var options = new BildausOptions(IncludePictures: true, IncludeDescriptions: true, TargetHeight: 40);

            foreach (var result in processor.Process(BildausKind.Person, ownerNumber: 1, options))
            {
                Assert.IsNotNull(result.Image, "Expected image to be loaded and returned.");
                Assert.AreEqual(40, result.Image!.Height);
            }
        }
        finally
        {
            try { File.Delete(filePath); } catch { }
            try { Directory.Delete(tempDir, recursive: true); } catch { }
        }
    }

    private sealed class TestRepository : IBildausRepository
    {
        private readonly BildausEntry _entry;

        public TestRepository(BildausEntry entry) => _entry = entry;

        public System.Collections.Generic.IEnumerable<BildausEntry> EnumerateEntries(BildausKind kind, int ownerNumber)
        {
            yield return _entry;
        }
    }
}
