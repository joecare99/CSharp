using Microsoft.VisualStudio.TestTools.UnitTesting;
using RnzTrauer.Core.Services;
using RnzTrauer.Import.Services;

namespace RnzTrauer.Core.Tests;

[TestClass]
public sealed class HtmlCallbackTokenizerTests
{
    [TestMethod]
    public void Feed_PreservesTagSplitAcrossChunks()
    {
        var tokenizer = new HtmlCallbackTokenizer();

        var first = tokenizer.Feed("<a href=\"/rnz/");
        var second = tokenizer.Feed("notice.pdf\">Text</a>");

        Assert.AreEqual(0, first.Count);
        Assert.AreEqual(HtmlCallbackKind.StartTag, second[0].Kind);
        Assert.AreEqual("a", second[0].Value);
        Assert.AreEqual(HtmlCallbackKind.StandardText, second[2].Kind);
        Assert.AreEqual(HtmlCallbackKind.EndTag, second[3].Kind);
    }

    [TestMethod]
    public void Feed_PreservesSplitCommentAndScriptCallbacks()
    {
        var tokenizer = new HtmlCallbackTokenizer();

        tokenizer.Feed("<!-- split");
        var comment = tokenizer.Feed(" comment --><script>var x=1;</script>");

        Assert.AreEqual(HtmlCallbackKind.Comment, comment[0].Kind);
        Assert.AreEqual(HtmlCallbackKind.StartTag, comment[1].Kind);
        Assert.AreEqual(HtmlCallbackKind.Script, comment[2].Kind);
        Assert.AreEqual("var x=1;", comment[2].Value);
        Assert.AreEqual(HtmlCallbackKind.EndTag, comment[3].Kind);
    }

    [TestMethod]
    public void Feed_TracksNestedTagPathsAndModifierOwner()
    {
        var tokenizer = new HtmlCallbackTokenizer();

        var callbacks = tokenizer.Feed("<div><a href='/notice'>Text</a></div>");

        Assert.AreEqual(string.Empty, callbacks[0].TagPath);
        Assert.AreEqual("DIV", callbacks[1].TagPath);
        Assert.AreEqual("A", callbacks[2].TagName);
        Assert.AreEqual("DIV\\A", callbacks[2].TagPath);
        Assert.AreEqual("DIV\\A", callbacks[4].TagPath);
        Assert.AreEqual("DIV", callbacks[5].TagPath);
    }

    [TestMethod]
    public void Feed_RecoversPathWhenClosingAnOuterTag()
    {
        var tokenizer = new HtmlCallbackTokenizer();

        var callbacks = tokenizer.Feed("<div><span></div><p>");

        Assert.AreEqual("DIV\\SPAN", callbacks[2].TagPath);
        Assert.AreEqual(string.Empty, callbacks[3].TagPath);
    }

    [TestMethod]
    public void Feed_KeepsQuotedModifierWithSpacesTogether()
    {
        var tokenizer = new HtmlCallbackTokenizer();

        var callbacks = tokenizer.Feed("<a title='death notice card' href='/notice'>");

        Assert.AreEqual(3, callbacks.Count);
        Assert.AreEqual("title='death notice card'", callbacks[1].Value);
        Assert.AreEqual("href='/notice'", callbacks[2].Value);
    }

    [TestMethod]
    public void Feed_RecognizesDoctypeAsSingletonTag()
    {
        var callbacks = new HtmlCallbackTokenizer().Feed("<!DOCTYPE HTML>");

        Assert.IsTrue(callbacks.Count >= 1);
        Assert.AreEqual(HtmlCallbackKind.StartTag, callbacks[0].Kind);
        Assert.AreEqual("!DOCTYPE", callbacks[0].Value);
        Assert.AreEqual(string.Empty, callbacks[0].TagPath);
    }

    [TestMethod]
    public void Complete_DoesNotInventCallbackForUnterminatedTag()
    {
        var tokenizer = new HtmlCallbackTokenizer();

        var callbacks = tokenizer.Feed("before <div");
        var completed = tokenizer.Complete();

        Assert.AreEqual(HtmlCallbackKind.StandardText, callbacks[0].Kind);
        Assert.AreEqual("before ", callbacks[0].Value);
        Assert.AreEqual(0, completed.Count);
    }
}
