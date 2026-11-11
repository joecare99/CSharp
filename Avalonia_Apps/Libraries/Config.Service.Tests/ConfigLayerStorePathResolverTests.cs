using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Config.Service.Tests;

[TestClass]
public sealed class ConfigLayerStorePathResolverTests
{
    [TestMethod]
    public void ResolveUserAndSystemPaths_UsesSeparateExplicitRoots()
    {
        string root = Path.Combine(Path.GetTempPath(), "ConfigLayerPathResolverTests", Guid.NewGuid().ToString("N"));
        string userRoot = Path.Combine(root, "user");
        string systemRoot = Path.Combine(root, "system");
        ConfigLayerStorePathResolver resolver = new(
            "Contoso",
            "Editor",
            userRoot: userRoot,
            systemRoot: systemRoot);

        Assert.AreEqual(
            Path.Combine(userRoot, "Contoso", "Editor", "config", "layers", "user.json"),
            resolver.ResolveUserPath());
        Assert.AreEqual(
            Path.Combine(systemRoot, "Contoso", "Editor", "config", "layers", "system.json"),
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

        string groupRoot = Path.Combine(Path.GetTempPath(), "ConfigLayerGroupPathTests", Guid.NewGuid().ToString("N"));
        ConfigLayerStorePathResolver withGroupRoot = new(
            "Contoso",
            "Editor",
            userRoot: Path.GetTempPath(),
            systemRoot: Path.GetTempPath(),
            groupRoot: groupRoot);
        Assert.AreEqual(
            Path.Combine(groupRoot, "Contoso", "Editor", "config", "layers", "Engineering.json"),
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

    [TestMethod]
    public void EvaluateWriteAccess_DoesNotCreateUnprovisionedDirectory()
    {
        string directory = Path.Combine(Path.GetTempPath(), "ConfigLayerAccessTests", Guid.NewGuid().ToString("N"));
        try
        {
            JsonConfigLayerStore store = new(ConfigLayer.System, Path.Combine(directory, "system.json"));

            ConfigLayerStoreAccessResult result = ConfigLayerStoreAccessEvaluator.Evaluate(store);

            Assert.IsFalse(result.CanWrite);
            StringAssert.Contains(result.ReadOnlyReason, "not been provisioned");
            Assert.IsFalse(Directory.Exists(directory));
        }
        finally
        {
            if (Directory.Exists(directory))
            {
                Directory.Delete(directory, recursive: true);
            }
        }
    }

    [TestMethod]
    public void EvaluateWriteAccess_UsesAndRemovesAnEphemeralSiblingProbe()
    {
        string directory = Path.Combine(Path.GetTempPath(), "ConfigLayerAccessTests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        try
        {
            string path = Path.Combine(directory, "system.json");
            JsonConfigLayerStore store = new(ConfigLayer.System, path);

            ConfigLayerStoreAccessResult result = ConfigLayerStoreAccessEvaluator.Evaluate(store);

            Assert.IsTrue(result.CanWrite, result.ReadOnlyReason);
            string[] files = Directory.GetFiles(directory);
            Assert.AreEqual(0, files.Length);
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [TestMethod]
    public void EvaluateWriteAccess_RefusesUnsupportedStoreTypes()
    {
        IConfigLayerStore store = Substitute.For<IConfigLayerStore>();
        store.Layer.Returns(ConfigLayer.Group);

        ConfigLayerStoreAccessResult result = ConfigLayerStoreAccessEvaluator.Evaluate(store);

        Assert.IsFalse(result.CanWrite);
        StringAssert.Contains(result.ReadOnlyReason, "cannot be verified");
    }
}
