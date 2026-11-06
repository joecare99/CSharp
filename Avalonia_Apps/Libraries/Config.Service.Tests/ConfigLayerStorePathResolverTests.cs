using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Config.Service.Tests;

[TestClass]
public sealed class ConfigLayerStorePathResolverTests
{
    [TestMethod]
    public void ResolveUserAndSystemPaths_UsesSeparateExplicitRoots()
    {
        ConfigLayerStorePathResolver resolver = new(
            "Contoso",
            "Editor",
            userRoot: @"C:\user-data",
            systemRoot: @"C:\shared-data");

        Assert.AreEqual(
            Path.GetFullPath(@"C:\user-data\Contoso\Editor\config\layers\user.json"),
            resolver.ResolveUserPath());
        Assert.AreEqual(
            Path.GetFullPath(@"C:\shared-data\Contoso\Editor\config\layers\system.json"),
            resolver.ResolveSystemPath());
    }

    [TestMethod]
    public async Task CreateUserStore_CreatesOnlyUserLayerDirectory()
    {
        string root = Path.Combine(Path.GetTempPath(), "ConfigLayerPathTests", Guid.NewGuid().ToString("N"));
        try
        {
            ConfigLayerStorePathResolver resolver = new(
                "Contoso",
                "Editor",
                userRoot: Path.Combine(root, "user"),
                systemRoot: Path.Combine(root, "system"));

            JsonConfigLayerStore store = resolver.CreateUserStore();

            Assert.AreEqual(ConfigLayer.User, store.Layer);
            Assert.IsTrue(Directory.Exists(Path.GetDirectoryName(resolver.ResolveUserPath())));
            Assert.IsFalse(Directory.Exists(Path.Combine(root, "system")));
            ConfigLayerSnapshot loaded = await store.LoadAsync();
            Assert.AreEqual(ConfigLayer.User, loaded.Layer);
            Assert.AreEqual(0, loaded.Values.Count);
        }
        finally
        {
            if (Directory.Exists(root))
            {
                Directory.Delete(root, recursive: true);
            }
        }
    }

    [TestMethod]
    public void ResolveGroupPath_RequiresConfiguredRootAndSafeGroupIdentity()
    {
        ConfigLayerStorePathResolver noGroupRoot = new(
            "Contoso",
            "Editor",
            userRoot: Path.GetTempPath(),
            systemRoot: Path.GetTempPath());
        Assert.ThrowsExactly<InvalidOperationException>(() => noGroupRoot.ResolveGroupPath("Engineering"));

        ConfigLayerStorePathResolver withGroupRoot = new(
            "Contoso",
            "Editor",
            userRoot: Path.GetTempPath(),
            systemRoot: Path.GetTempPath(),
            groupRoot: @"C:\managed-groups");
        Assert.AreEqual(
            Path.GetFullPath(@"C:\managed-groups\Contoso\Editor\config\layers\Engineering.json"),
            withGroupRoot.ResolveGroupPath("Engineering"));
        Assert.ThrowsExactly<ArgumentException>(() => withGroupRoot.ResolveGroupPath(@"..\OtherGroup"));
        Assert.ThrowsExactly<ArgumentException>(() => withGroupRoot.ResolveGroupPath("DOMAIN:Group"));
    }

    [TestMethod]
    public void Constructor_RejectsUnsafeProductPathSegments()
    {
        Assert.ThrowsExactly<ArgumentException>(() => new ConfigLayerStorePathResolver(
            "..",
            "Editor",
            userRoot: Path.GetTempPath(),
            systemRoot: Path.GetTempPath()));
        Assert.ThrowsExactly<ArgumentException>(() => new ConfigLayerStorePathResolver(
            "Contoso",
            @"Nested\Editor",
            userRoot: Path.GetTempPath(),
            systemRoot: Path.GetTempPath()));
    }
}
