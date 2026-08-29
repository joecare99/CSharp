using System;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
#if NET5_0_OR_GREATER
using ConsoleLib.Posix;
#endif

namespace ConsoleLibTests;

[TestClass]
public sealed class Osc52ResponseParserTests
{
#if NET5_0_OR_GREATER
    [TestMethod]
    public void Parser_DecodesValidResponseAndRejectsInvalidPayload()
    {
        var parser = new Osc52ResponseParser(10);
        var encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes("ready"));

        Assert.AreEqual("ready", parser.Parse("\u001b]52;c;" + encoded + "\a"));
        Assert.IsNull(parser.Parse("\u001b]52;c;not-base64\a"));
    }
#endif
}
