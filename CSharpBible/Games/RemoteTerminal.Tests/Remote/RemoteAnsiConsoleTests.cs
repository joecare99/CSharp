using Microsoft.VisualStudio.TestTools.UnitTesting;
using RemoteTerminal.Remote;
using System.IO;

namespace RemoteTerminal.Tests.Remote;

[TestClass]
public sealed class RemoteAnsiConsoleTests
{
    [TestMethod]
    public void Ctor_WritesHandshake()
    {
        using var stream = new MemoryStream();
        _ = new RemoteAnsiConsole(stream);

        var bytes = stream.ToArray();
        Assert.IsTrue(bytes.Length > 0);
    }
}
