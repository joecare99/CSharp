using Microsoft.VisualStudio.TestTools.UnitTesting;
using RnzTrauer.Core.Services;
using RnzTrauer.Import.Services;

namespace RnzTrauer.Core.Tests;

[TestClass]
public sealed class HtmlTextNormalizerTests
{
    [TestMethod]
    public void Normalize_RemovesMarkupDecodesEntitiesAndCollapsesWhitespace()
    {
        var normalizer = new HtmlTextNormalizer();

        var result = normalizer.Normalize(
            "<div>Maria&nbsp;Müller</div><p>Abschied &amp; Beisetzung</p>");

        Assert.AreEqual("Maria Müller Abschied & Beisetzung", result);
    }

    [TestMethod]
    public void Normalize_ExcludesScriptAndStyleContent()
    {
        var normalizer = new HtmlTextNormalizer();

        var result = normalizer.Normalize(
            "<style>.hidden { display:none; }</style><script>alert('x');</script>" +
            "<span>Visible</span>");

        Assert.AreEqual("Visible", result);
    }
}
