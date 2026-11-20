using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AA98_AvlnCodeStudio.Base.Testing.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AA98_AvlnCodeStudio.Tests.Engineering;

[TestClass]
public sealed class VSTestAdapterPathTests
{
    [TestMethod]
    public void PathComparer_UsesPlatformPathCaseRules()
    {
        Assert.AreEqual(OperatingSystem.IsWindows(),
            VSTestAdapterResolver.PathComparer.Equals("Adapters/Sample.TestAdapter.dll", "adapters/sample.TestAdapter.dll"));
    }

    [TestMethod]
    public async Task Resolve_HandlesCaseDistinctAdaptersOnCurrentFilesystem()
    {
        string root = Path.Combine(Environment.CurrentDirectory, $"adapter-path-test-{Guid.NewGuid():N}");
        string upper = Path.Combine(root, "Adapters");
        string lower = Path.Combine(root, "adapters");
        Directory.CreateDirectory(upper);
        try
        {
            bool caseSensitive = !Directory.Exists(lower);
            Directory.CreateDirectory(lower);
            string first = Path.Combine(upper, "Sample.TestAdapter.dll");
            string second = Path.Combine(lower, "sample.TestAdapter.dll");
            await File.WriteAllBytesAsync(first, []);
            if (caseSensitive)
            {
                await File.WriteAllBytesAsync(second, []);
            }

            await File.WriteAllTextAsync(Path.Combine(root, "vstest-adapters.json"),
                JsonSerializer.Serialize(new { adapterPaths = new[] { "Adapters/Sample.TestAdapter.dll", "adapters/sample.TestAdapter.dll" } }));
            string[] adapters = await VSTestAdapterResolver.ResolveAsync(
                Path.Combine(upper, "Sample.Tests.dll"), root, CancellationToken.None);

            int expected = OperatingSystem.IsWindows() ? 1 : 2;
            Assert.AreEqual(expected, adapters.Length, $"Filesystem case-sensitive: {caseSensitive}");
            Assert.IsTrue(adapters.Contains(first, StringComparer.Ordinal));
            if (expected == 2)
            {
                Assert.IsTrue(adapters.Contains(second, StringComparer.Ordinal));
            }
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [TestMethod]
    public async Task Resolve_RejectsSymbolicLinkInConfiguredPath()
    {
        if (OperatingSystem.IsWindows())
        {
            Assert.Inconclusive("Creating symbolic links on Windows may require elevated privileges.");
        }

        string root = Path.Combine(Environment.CurrentDirectory, $"adapter-link-test-{Guid.NewGuid():N}");
        string real = Path.Combine(root, "real");
        Directory.CreateDirectory(real);
        try
        {
            await File.WriteAllBytesAsync(Path.Combine(real, "Sample.TestAdapter.dll"), []);
            Directory.CreateSymbolicLink(Path.Combine(root, "link"), real);
            await File.WriteAllTextAsync(Path.Combine(root, "vstest-adapters.json"),
                """{"adapterPaths":["link/Sample.TestAdapter.dll"]}""");

            await Assert.ThrowsExactlyAsync<InvalidDataException>(() =>
                VSTestAdapterResolver.ResolveAsync(
                    Path.Combine(root, "output", "Sample.Tests.dll"), root, CancellationToken.None));
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }
}
