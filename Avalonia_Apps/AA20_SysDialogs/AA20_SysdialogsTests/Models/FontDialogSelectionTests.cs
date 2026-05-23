using Avln_CommonDialogs.Base.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AA20_SysdialogsTests.Models;

[TestClass]
public class FontDialogSelectionTests
{
    [TestMethod]
    public void Clone_CreatesIndependentCopy()
    {
        var selection = new FontDialogSelection
        {
            FamilyName = "Arial",
            Size = 14,
            IsBold = true,
            IsUnderline = true,
            ArgbColor = 0xFF112233
        };

        var clone = selection.Clone();
        clone.FamilyName = "Calibri";
        clone.Size = 20;

        Assert.AreEqual("Arial", selection.FamilyName);
        Assert.AreEqual(14d, selection.Size);
        Assert.AreEqual("Calibri", clone.FamilyName);
        Assert.AreEqual(20d, clone.Size);
    }

    [TestMethod]
    public void ToString_IncludesSelectedValues()
    {
        var selection = new FontDialogSelection
        {
            FamilyName = "Consolas",
            Size = 12.5,
            IsBold = true,
            IsItalic = true,
            IsUnderline = true,
            ArgbColor = 0xFFAABBCC
        };

        var result = selection.ToString();

        StringAssert.Contains(result, "Consolas");
        StringAssert.Contains(result, "pt");
        StringAssert.Contains(result, "Bold");
        StringAssert.Contains(result, "Italic");
        StringAssert.Contains(result, "Underline");
        StringAssert.Contains(result, "#FFAABBCC");
    }
}
