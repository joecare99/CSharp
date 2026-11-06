using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Config.Service.Tests;

[TestClass]
public sealed class ConfigLayerLegacyJsonMigratorTests
{
    private string _root = string.Empty;

    [TestInitialize]
    public void CreateDirectory()
    {
        _root = Path.Combine(Path.GetTempPath(), "ConfigLayerLegacyMigrationTests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_root);
    }

    [TestCleanup]
    public void RemoveDirectory()
    {
        if (Directory.Exists(_root))
        {
            Directory.Delete(_root, recursive: true);
        }
    }

    [TestMethod]
    public async Task MigrateSectionToUserLayerAsync_PreservesSourceAndExistingLayerValues()
    {
        string legacyPath = Path.Combine(_root, "Workbench.json");
        const string legacyJson = """
            {
              "aiEndpoint": "https://example.test/chat",
              "aiModelId": "legacy-model",
              "workspacePath": "C:\\legacy",
              "unknownLegacyValue": { "enabled": true }
            }
            """;
        await File.WriteAllTextAsync(legacyPath, legacyJson);
        ConfigSectionRegistry registry = new();
        registry.AddSection(new LegacySettingsProvider());
        JsonConfigLayerStore store = new(ConfigLayer.User, Path.Combine(_root, "user.json"));
        ConfigLayerSnapshot initial = await store.LoadAsync();
        ConfigLayerSnapshot existing = initial.WithValue(
            new ConfigPropertyKey("Workbench", nameof(LegacySettings.AIModelId)),
            new ConfigLayerValue("already-configured"));
        await store.SaveAsync(existing);
        ConfigLayerLegacyJsonMigrator migrator = new(registry);

        ConfigLayerLegacyMigrationResult result = await migrator.MigrateSectionToUserLayerAsync(
            legacyPath,
            "Workbench",
            store);

        ConfigLayerSnapshot migrated = await store.LoadAsync();
        Assert.IsTrue(result.SourceFound);
        Assert.IsTrue(result.DestinationChanged);
        Assert.AreEqual(3, result.ImportedPropertyCount);
        Assert.AreEqual(1, result.ExistingDestinationPropertyCount);
        Assert.AreEqual(1, result.UnmappedPropertyCount);
        Assert.AreEqual("already-configured", migrated.Values[new ConfigPropertyKey("Workbench", nameof(LegacySettings.AIModelId))].Value?.ToString());
        Assert.AreEqual("https://example.test/chat", ((JsonElement)migrated.Values[new ConfigPropertyKey("Workbench", nameof(LegacySettings.AIEndpoint))].Value!).GetString());
        Assert.AreEqual("C:\\legacy", ((JsonElement)migrated.Values[new ConfigPropertyKey("Workbench", nameof(LegacySettings.WorkspacePath))].Value!).GetString());
        Assert.AreEqual(true, ((JsonElement)migrated.Values[new ConfigPropertyKey("Workbench", "unknownLegacyValue")].Value!).GetProperty("enabled").GetBoolean());
        Assert.AreEqual(legacyJson, await File.ReadAllTextAsync(legacyPath));

        ConfigLayerLegacyMigrationResult repeated = await migrator.MigrateSectionToUserLayerAsync(
            legacyPath,
            "Workbench",
            store);

        Assert.IsFalse(repeated.DestinationChanged);
        Assert.AreEqual(0, repeated.ImportedPropertyCount);
        Assert.AreEqual(0, repeated.ExistingDestinationPropertyCount);
    }

    [TestMethod]
    public async Task MigrateSectionToUserLayerAsync_MissingSourceIsNoOp()
    {
        ConfigSectionRegistry registry = new();
        registry.AddSection(new LegacySettingsProvider());
        JsonConfigLayerStore store = new(ConfigLayer.User, Path.Combine(_root, "user.json"));
        ConfigLayerLegacyMigrationResult result = await new ConfigLayerLegacyJsonMigrator(registry)
            .MigrateSectionToUserLayerAsync(Path.Combine(_root, "missing.json"), "Workbench", store);

        Assert.IsFalse(result.SourceFound);
        Assert.IsFalse(result.DestinationChanged);
        Assert.IsFalse(File.Exists(Path.Combine(_root, "user.json")));
    }

    [TestMethod]
    public async Task MigrateSectionToUserLayerAsync_ResetOverrideDoesNotReimportLegacyValue()
    {
        string legacyPath = Path.Combine(_root, "Workbench.json");
        await File.WriteAllTextAsync(legacyPath, """{"aiModelId":"legacy-model"}""");
        ConfigSectionRegistry registry = new();
        registry.AddSection(new LegacySettingsProvider());
        JsonConfigLayerStore store = new(ConfigLayer.User, Path.Combine(_root, "user.json"));
        ConfigLayerLegacyJsonMigrator migrator = new(registry);

        await migrator.MigrateSectionToUserLayerAsync(legacyPath, "Workbench", store);
        ConfigLayerSnapshot migrated = await store.LoadAsync();
        Dictionary<ConfigPropertyKey, ConfigLayerValue> resetValues = new(migrated.Values);
        resetValues.Remove(new ConfigPropertyKey("Workbench", nameof(LegacySettings.AIModelId)));
        await store.SaveAsync(new ConfigLayerSnapshot(ConfigLayer.User, resetValues, migrated.StorageVersion));

        ConfigLayerLegacyMigrationResult repeated =
            await migrator.MigrateSectionToUserLayerAsync(legacyPath, "Workbench", store);
        ConfigLayerSnapshot final = await store.LoadAsync();

        Assert.IsTrue(repeated.SourceFound);
        Assert.IsFalse(repeated.DestinationChanged);
        Assert.IsFalse(final.TryGetValue(
            new ConfigPropertyKey("Workbench", nameof(LegacySettings.AIModelId)),
            out _));
    }

    [TestMethod]
    public async Task MigrateSectionToUserLayerAsync_InvalidSourceDoesNotChangeDestination()
    {
        string legacyPath = Path.Combine(_root, "Workbench.json");
        await File.WriteAllTextAsync(legacyPath, "{broken");
        ConfigSectionRegistry registry = new();
        registry.AddSection(new LegacySettingsProvider());
        JsonConfigLayerStore store = new(ConfigLayer.User, Path.Combine(_root, "user.json"));
        ConfigLayerSnapshot initial = await store.LoadAsync();
        await store.SaveAsync(initial.WithValue(
            new ConfigPropertyKey("Workbench", nameof(LegacySettings.AIModelId)),
            new ConfigLayerValue("retained")));
        string before = await File.ReadAllTextAsync(Path.Combine(_root, "user.json"));

        await Assert.ThrowsExactlyAsync<InvalidDataException>(() =>
            new ConfigLayerLegacyJsonMigrator(registry).MigrateSectionToUserLayerAsync(
                legacyPath,
                "Workbench",
                store));

        Assert.AreEqual(before, await File.ReadAllTextAsync(Path.Combine(_root, "user.json")));
    }

    [TestMethod]
    public async Task MigrateSectionToUserLayerAsync_RejectsNonUserStore()
    {
        ConfigSectionRegistry registry = new();
        registry.AddSection(new LegacySettingsProvider());
        JsonConfigLayerStore store = new(ConfigLayer.Group, Path.Combine(_root, "group.json"));

        await Assert.ThrowsExactlyAsync<ArgumentException>(() =>
            new ConfigLayerLegacyJsonMigrator(registry).MigrateSectionToUserLayerAsync(
                Path.Combine(_root, "Workbench.json"),
                "Workbench",
                store));
    }

    private sealed class LegacySettings
    {
        public string AIEndpoint { get; set; } = string.Empty;
        public string AIModelId { get; set; } = string.Empty;
        public string? WorkspacePath { get; set; }
    }

    private sealed class LegacySettingsProvider : IConfigSectionProvider
    {
        public string Name => "Workbench";
        public string DisplayName => "Workbench";
        public string? Description => null;
        public int Order => 0;
        public Type ModelType => typeof(LegacySettings);
        public object CreateModel() => new LegacySettings();
    }
}
