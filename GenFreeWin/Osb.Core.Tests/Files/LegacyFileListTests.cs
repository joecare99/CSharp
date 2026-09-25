using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Osb.Core.Files;

namespace Osb.Core.Tests.Files;

[TestClass]
public sealed class LegacyFileListTests
{
    [TestMethod]
    public void EnumerateNames_FiltersToTopLevelAndSortsIgnoringCase()
    {
        var directory = CreateTemporaryDirectory();
        try
        {
            File.WriteAllText(Path.Combine(directory, "b.OSI"), string.Empty);
            File.WriteAllText(Path.Combine(directory, "A.osi"), string.Empty);
            File.WriteAllText(Path.Combine(directory, "ignore.txt"), string.Empty);
            Directory.CreateDirectory(Path.Combine(directory, "nested"));
            File.WriteAllText(Path.Combine(directory, "nested", "nested.OSI"), string.Empty);

            var actual = LegacyFileList.EnumerateNames(directory, "*.OSI");

            CollectionAssert.AreEqual(new[] { "A.osi", "b.OSI" }, actual.ToArray());
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [TestMethod]
    public void EnumerateNames_ReturnsEmptyForMissingDirectory()
    {
        var directory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));

        var actual = LegacyFileList.EnumerateNames(directory, "*.*");

        Assert.AreEqual(0, actual.Count);
    }

    [TestMethod]
    public void EnumerateNames_RejectsNullArguments()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() => LegacyFileList.EnumerateNames(null!, "*.*"));
        Assert.ThrowsExactly<ArgumentNullException>(() => LegacyFileList.EnumerateNames("directory", null!));
    }

    private static string CreateTemporaryDirectory()
    {
        var directory = Path.Combine(Path.GetTempPath(), "osb-file-list-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        return directory;
    }
}
