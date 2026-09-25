using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Osb.Core.Tests;

[TestClass]
public sealed class CompatibilityStructuralCheckTests
{
    [TestMethod]
    public void OsbProductionFiles_DoNotReferenceLegacyVisualBasicCompatibilityNamespace()
    {
        var osbProductionRoot = Path.GetFullPath(Path.Combine(
            AppContext.BaseDirectory,
            "..",
            "..",
            "..",
            "..",
            "Osb",
            "Osb"));

        Assert.IsTrue(Directory.Exists(osbProductionRoot), $"Expected OSB production folder at '{osbProductionRoot}'.");

        var violations = new List<string>();

        foreach (var file in Directory.EnumerateFiles(osbProductionRoot, "*.cs", SearchOption.AllDirectories))
        {
            if (Path.GetFileName(file).Equals("LegacyVbCompatibility.cs", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var content = File.ReadAllText(file);
            if (content.Contains("Microsoft.VisualBasic.Compatibility.VB6", StringComparison.OrdinalIgnoreCase) ||
                content.Contains("Compatibility.VB6", StringComparison.OrdinalIgnoreCase))
            {
                violations.Add(Path.GetRelativePath(osbProductionRoot, file));
            }
        }

        Assert.AreEqual(0, violations.Count, string.Join(Environment.NewLine, violations));
    }
}
