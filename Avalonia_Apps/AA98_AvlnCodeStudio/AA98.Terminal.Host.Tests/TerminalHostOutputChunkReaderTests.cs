using AA98.Terminal.Host.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Threading.Tasks;

namespace AA98.Terminal.Host.Tests;

[TestClass]
public class TerminalHostOutputChunkReaderTests
{
    [TestMethod]
    public async Task OutputChunkReader_ShouldFlushPartialOutputAfterInactivity()
    {
        using var reader = new StringReader("prompt> ");
        var partialOutput = string.Empty;
        var sut = new OutputChunkReader(
            reader,
            TimeSpan.FromMilliseconds(20),
            _ => Assert.Fail("No complete line expected."),
            text => partialOutput += text);

        await sut.RunAsync(default);

        Assert.AreEqual("prompt> ", partialOutput);
    }

    [TestMethod]
    public async Task OutputChunkReader_ShouldEmitCompleteLineWithoutPartialFlush()
    {
        using var reader = new StringReader("hello\n");
        var lineOutput = string.Empty;
        var partialOutput = string.Empty;
        var sut = new OutputChunkReader(
            reader,
            TimeSpan.FromMilliseconds(20),
            text => lineOutput += text,
            text => partialOutput += text);

        await sut.RunAsync(default);

        Assert.AreEqual("hello", lineOutput);
        Assert.AreEqual(string.Empty, partialOutput);
    }
}