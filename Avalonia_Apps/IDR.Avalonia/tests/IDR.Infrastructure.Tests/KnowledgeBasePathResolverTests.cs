using IDR.Core.Services;
using IDR.Infrastructure.KnowledgeBase;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace IDR.Infrastructure.Tests;

[TestClass]
public sealed class KnowledgeBasePathResolverTests
{
    [TestMethod]
    public void ResolvePathUsesVersionSpecificFileName()
    {
        string directory = Path.GetFullPath(Path.Combine("test-data", "knowledge-bases"));
        KnowledgeBasePathResolver resolver = new(directory);

        string result = resolver.ResolvePath(DelphiVersion.Delphi2009);

        Assert.AreEqual(Path.Combine(directory, "kb2009.bin"), result);
    }

    [TestMethod]
    public void ResolvePathRejectsUnknownVersion()
    {
        KnowledgeBasePathResolver resolver = new(Path.GetTempPath());

        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => resolver.ResolvePath(DelphiVersion.Unknown));
    }
}
