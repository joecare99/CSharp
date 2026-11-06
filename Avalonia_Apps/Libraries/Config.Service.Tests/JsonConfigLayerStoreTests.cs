using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Config.Service.Tests;

[TestClass]
public sealed class JsonConfigLayerStoreTests
{
    private string _root = string.Empty;

    [TestInitialize]
    public void CreateStoreDirectory()
    {
        _root = Path.Combine(Path.GetTempPath(), "ConfigLayerStoreTests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_root);
    }

    [TestCleanup]
    public void RemoveStoreDirectory()
    {
        if (Directory.Exists(_root))
        {
            Directory.Delete(_root, recursive: true);
        }
    }

    [TestMethod]
    public async Task LoadAsync_MissingFile_ReturnsEmptySnapshot()
    {
        JsonConfigLayerStore store = CreateStore(ConfigLayer.User);

        ConfigLayerSnapshot loaded = await store.LoadAsync();

        Assert.AreEqual(ConfigLayer.User, loaded.Layer);
        Assert.AreEqual(0, loaded.Values.Count);
    }

    [TestMethod]
    public async Task SaveAndLoadAsync_RoundTripsTypedValuesAndEnforcedPolicy()
    {
        ConfigPropertyKey nameKey = new("Workbench", "AIModelId");
        ConfigPropertyKey enabledKey = new("Workbench", "Enabled");
        ConfigLayerSnapshot snapshot = new(ConfigLayer.System,
        [
            new KeyValuePair<ConfigPropertyKey, ConfigLayerValue>(nameKey, new ConfigLayerValue("policy-model", IsEnforced: true)),
            new KeyValuePair<ConfigPropertyKey, ConfigLayerValue>(enabledKey, new ConfigLayerValue(true)),
        ]);
        JsonConfigLayerStore store = CreateStore(ConfigLayer.System);

        ConfigLayerSnapshot saved = await store.SaveAsync(snapshot);

        ConfigLayerSnapshot loaded = await store.LoadAsync();
        ConfigValueResolution name = ConfigLayerResolver.Resolve(
            nameKey,
            typeof(string),
            "default",
            system: loaded,
            user: new ConfigLayerSnapshot(ConfigLayer.User,
            [
                new KeyValuePair<ConfigPropertyKey, ConfigLayerValue>(nameKey, new ConfigLayerValue("user-model")),
            ]),
            trustEnforcedSystemValues: true);
        ConfigValueResolution enabled = ConfigLayerResolver.Resolve(enabledKey, typeof(bool), false, system: loaded);

        Assert.AreEqual("policy-model", name.Value);
        Assert.AreEqual(ConfigValueSource.System, name.Source);
        Assert.IsTrue(name.IsEnforced);
        Assert.AreEqual(true, enabled.Value);
        Assert.AreEqual(saved.StorageVersion, loaded.StorageVersion);
    }

    [TestMethod]
    public async Task SaveAndLoadAsync_RoundTripsNestedObjectProperties()
    {
        ConfigPropertyKey key = new("Workbench", "Profiles");
        JsonConfigLayerStore store = CreateStore(ConfigLayer.System);
        ConfigLayerSnapshot original = (await store.LoadAsync()).WithValue(
            key, new ConfigLayerValue(new List<Profile> { new() { DisplayName = "Local", Endpoint = "https://example.test" } }));
        await store.SaveAsync(original);

        ConfigValueResolution result = ConfigLayerResolver.Resolve(
            key, typeof(List<Profile>), new List<Profile>(), system: await store.LoadAsync());

        List<Profile> profiles = (List<Profile>)result.Value!;
        Assert.AreEqual(1, profiles.Count);
        Assert.AreEqual("Local", profiles[0].DisplayName);
        Assert.AreEqual("https://example.test", profiles[0].Endpoint);
    }

    public sealed class Profile
    {
        public string DisplayName { get; set; } = string.Empty;
        public string Endpoint { get; set; } = string.Empty;
    }

    [TestMethod]
    public async Task LoadAsync_CorruptOrWrongLayerFile_ReportsInvalidData()
    {
        JsonConfigLayerStore store = CreateStore(ConfigLayer.User);
        await File.WriteAllTextAsync(FilePath, "{not-json");

        await Assert.ThrowsExactlyAsync<InvalidDataException>(() => store.LoadAsync());

        await File.WriteAllTextAsync(FilePath, """
            {"schemaVersion":1,"layer":"Group","values":[]}
            """);
        await Assert.ThrowsExactlyAsync<InvalidDataException>(() => store.LoadAsync());
    }

    [TestMethod]
    public async Task SaveAsync_RejectsWrongLayerAndDoesNotCreateMissingDirectory()
    {
        JsonConfigLayerStore userStore = CreateStore(ConfigLayer.User);
        ConfigLayerSnapshot systemSnapshot = new(ConfigLayer.System, []);

        await Assert.ThrowsExactlyAsync<ArgumentException>(() => userStore.SaveAsync(systemSnapshot));
        JsonConfigLayerStore missingDirectoryStore = new(ConfigLayer.User, Path.Combine(_root, "missing", "user.json"));
        await Assert.ThrowsExactlyAsync<DirectoryNotFoundException>(() =>
            missingDirectoryStore.SaveAsync(new ConfigLayerSnapshot(ConfigLayer.User, [])));

        Assert.IsFalse(File.Exists(FilePath));
    }

    [TestMethod]
    public async Task SaveAsync_RejectsStaleSnapshotAndPreservesConcurrentUpdate()
    {
        JsonConfigLayerStore firstStore = CreateStore(ConfigLayer.User);
        JsonConfigLayerStore secondStore = CreateStore(ConfigLayer.User);
        ConfigLayerSnapshot firstRead = await firstStore.LoadAsync();
        ConfigLayerSnapshot staleRead = await secondStore.LoadAsync();
        ConfigPropertyKey key = new("Workbench", "AIModelId");
        ConfigLayerSnapshot firstChange = new(ConfigLayer.User,
        [
            new KeyValuePair<ConfigPropertyKey, ConfigLayerValue>(key, new ConfigLayerValue("first")),
        ], firstRead.StorageVersion);
        ConfigLayerSnapshot concurrentChange = new(ConfigLayer.User,
        [
            new KeyValuePair<ConfigPropertyKey, ConfigLayerValue>(key, new ConfigLayerValue("concurrent")),
        ], staleRead.StorageVersion);

        await firstStore.SaveAsync(firstChange);
        await Assert.ThrowsExactlyAsync<IOException>(() => secondStore.SaveAsync(concurrentChange));

        ConfigLayerSnapshot final = await firstStore.LoadAsync();
        ConfigValueResolution finalValue = ConfigLayerResolver.Resolve(key, typeof(string), "default", user: final);
        Assert.AreEqual("first", finalValue.Value);
    }

    private string FilePath => Path.Combine(_root, "user.json");

    private JsonConfigLayerStore CreateStore(ConfigLayer layer) => new(layer, FilePath);
}
