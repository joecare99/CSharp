using System.Text;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RnzTrauer.Core.Services;
using RnzTrauer.Import.Services;

namespace RnzTrauer.Core.Tests;

[TestClass]
public sealed class HtmlEncodingDecoderTests
{
    [TestMethod]
    public void Decode_RecognizesUtf8Bom()
    {
        var decoder = new HtmlEncodingDecoder();
        var encoding = new UTF8Encoding(false);
        var bytes = new UTF8Encoding(true).GetPreamble()
            .Concat(encoding.GetBytes("Müller"))
            .ToArray();

        var result = decoder.Decode(bytes);

        Assert.AreEqual("Müller", result.Text);
        Assert.AreEqual("UTF-8 BOM", result.EncodingName);
    }

    [TestMethod]
    public void Decode_FallsBackToWindows1252ForInvalidUtf8()
    {
        var decoder = new HtmlEncodingDecoder();
        var bytes = new byte[] { (byte)'M', 0xFC, (byte)'l', (byte)'l', (byte)'e', (byte)'r' };

        var result = decoder.Decode(bytes);

        Assert.AreEqual("Müller", result.Text);
        Assert.AreEqual("Windows-1252 fallback", result.EncodingName);
    }
}
