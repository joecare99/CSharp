using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Config.Service.Tests;

[TestClass]
public sealed class ConfigLayerResolverTests
{
    private static readonly ConfigPropertyKey s_key = new("Workbench", "AIModelId");

    [TestMethod]
    public void Resolve_UsesUserThenGroupThenSystemThenProductDefault()
    {
        ConfigLayerSnapshot system = Snapshot(ConfigLayer.System, "system");
        ConfigLayerSnapshot group = Snapshot(ConfigLayer.Group, "group");
        ConfigLayerSnapshot user = Snapshot(ConfigLayer.User, "user");

        ConfigValueResolution resolvedUser = ConfigLayerResolver.Resolve(s_key, typeof(string), "default", system, group, user);
        ConfigValueResolution resolvedGroup = ConfigLayerResolver.Resolve(s_key, typeof(string), "default", system, group);
        ConfigValueResolution resolvedSystem = ConfigLayerResolver.Resolve(s_key, typeof(string), "default", system);
        ConfigValueResolution resolvedDefault = ConfigLayerResolver.Resolve(s_key, typeof(string), "default");

        Assert.AreEqual("user", resolvedUser.Value);
        Assert.AreEqual(ConfigValueSource.User, resolvedUser.Source);
        Assert.AreEqual("group", resolvedGroup.Value);
        Assert.AreEqual(ConfigValueSource.Group, resolvedGroup.Source);
        Assert.AreEqual("system", resolvedSystem.Value);
        Assert.AreEqual(ConfigValueSource.System, resolvedSystem.Source);
        Assert.AreEqual("default", resolvedDefault.Value);
        Assert.AreEqual(ConfigValueSource.ProductDefault, resolvedDefault.Source);
    }

    [TestMethod]
    public void Resolve_EnforcedSystemValueOverridesUserAndGroupOnlyWhenSystemSourceIsTrusted()
    {
        ConfigLayerSnapshot system = Snapshot(ConfigLayer.System, "policy", isEnforced: true);
        ConfigLayerSnapshot group = Snapshot(ConfigLayer.Group, "group");
        ConfigLayerSnapshot user = Snapshot(ConfigLayer.User, "user");

        ConfigValueResolution untrusted = ConfigLayerResolver.Resolve(s_key, typeof(string), "default", system, group, user);
        ConfigValueResolution resolved = ConfigLayerResolver.Resolve(
            s_key,
            typeof(string),
            "default",
            system,
            group,
            user,
            trustEnforcedSystemValues: true);

        Assert.AreEqual("user", untrusted.Value);
        Assert.AreEqual(ConfigValueSource.User, untrusted.Source);
        Assert.IsFalse(untrusted.IsEnforced);
        Assert.AreEqual("policy", resolved.Value);
        Assert.AreEqual(ConfigValueSource.System, resolved.Source);
        Assert.IsTrue(resolved.IsEnforced);
    }

    [TestMethod]
    public void Resolve_PresentNullIsNotTreatedAsMissing()
    {
        ConfigLayerSnapshot user = Snapshot(ConfigLayer.User, null);

        ConfigValueResolution resolved = ConfigLayerResolver.Resolve(s_key, typeof(string), "default", user: user);

        Assert.IsNull(resolved.Value);
        Assert.AreEqual(ConfigValueSource.User, resolved.Source);
    }

    [TestMethod]
    public void Snapshot_OverrideDraftCanBeSetAndRemovedWithoutMutatingSource()
    {
        ConfigLayerSnapshot system = Snapshot(ConfigLayer.System, "system");
        ConfigLayerSnapshot user = new(ConfigLayer.User, [], storageVersion: "loaded-version");
        ConfigLayerSnapshot edited = user.WithValue(s_key, new ConfigLayerValue("user"));
        ConfigLayerSnapshot reset = edited.WithoutValue(s_key);

        ConfigValueResolution editedValue = ConfigLayerResolver.Resolve(s_key, typeof(string), "default", system, user: edited);
        ConfigValueResolution resetValue = ConfigLayerResolver.Resolve(s_key, typeof(string), "default", system, user: reset);

        Assert.AreEqual("user", editedValue.Value);
        Assert.AreEqual(ConfigValueSource.User, editedValue.Source);
        Assert.AreEqual("system", resetValue.Value);
        Assert.AreEqual(ConfigValueSource.System, resetValue.Source);
        Assert.AreEqual(0, user.Values.Count);
        Assert.AreEqual("loaded-version", edited.StorageVersion);
        Assert.AreEqual("loaded-version", reset.StorageVersion);
        Assert.AreSame(reset, reset.WithoutValue(s_key));
    }

    [TestMethod]
    public void Snapshot_CopiesInputAndRejectsEnforcementOutsideSystemLayer()
    {
        Dictionary<ConfigPropertyKey, ConfigLayerValue> values = new()
        {
            [s_key] = new ConfigLayerValue("before"),
        };
        ConfigLayerSnapshot snapshot = new(ConfigLayer.User, values);
        values[s_key] = new ConfigLayerValue("after");

        Assert.IsTrue(snapshot.TryGetValue(s_key, out ConfigLayerValue? retained));
        Assert.AreEqual("before", retained.Value);
        Assert.ThrowsExactly<ArgumentException>(() =>
            new ConfigLayerSnapshot(ConfigLayer.Group,
            [
                new KeyValuePair<ConfigPropertyKey, ConfigLayerValue>(s_key, new ConfigLayerValue("policy", IsEnforced: true)),
            ]));
    }

    [TestMethod]
    public void Resolve_RejectsWrongLayerAndIncompatibleValueType()
    {
        ConfigLayerSnapshot wrongLayer = Snapshot(ConfigLayer.Group, "group");
        Assert.ThrowsExactly<ArgumentException>(() =>
            ConfigLayerResolver.Resolve(s_key, typeof(string), "default", system: wrongLayer));

        ConfigLayerSnapshot invalidValue = Snapshot(ConfigLayer.User, 42);
        Assert.ThrowsExactly<ArgumentException>(() =>
            ConfigLayerResolver.Resolve(s_key, typeof(string), "default", user: invalidValue));
    }

    [TestMethod]
    public async Task LayeredSettingsResolver_UsesRegisteredProductDefaultAndExplicitStores()
    {
        string root = Path.Combine(Path.GetTempPath(), "ConfigLayeredResolverTests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        try
        {
            ConfigSectionRegistry registry = new();
            registry.AddSection(new ResolverSettingsProvider());
            JsonConfigLayerStore userStore = new(ConfigLayer.User, Path.Combine(root, "user.json"));
            ConfigLayeredSettingsResolver resolver = new(registry, [userStore]);

            ConfigValueResolution initial = await resolver.ResolveAsync("General", nameof(ResolverSettings.Name));
            ConfigLayerSnapshot empty = await userStore.LoadAsync();
            ConfigLayerSnapshot draft = empty.WithValue(
                new ConfigPropertyKey("General", nameof(ResolverSettings.Name)),
                new ConfigLayerValue("configured"));
            await userStore.SaveAsync(draft);

            ConfigValueResolution overridden = await resolver.ResolveAsync("General", nameof(ResolverSettings.Name));

            Assert.AreEqual("product-default", initial.Value);
            Assert.AreEqual(ConfigValueSource.ProductDefault, initial.Source);
            Assert.AreEqual("configured", overridden.Value);
            Assert.AreEqual(ConfigValueSource.User, overridden.Source);
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
    public async Task LayeredSettingsResolver_FailsOnMissingSectionOrProperty()
    {
        ConfigSectionRegistry registry = new();
        ConfigLayeredSettingsResolver resolver = new(registry, []);

        await Assert.ThrowsExactlyAsync<KeyNotFoundException>(
            () => resolver.ResolveAsync("Missing", "Name"));

        registry.AddSection(new ResolverSettingsProvider());
        await Assert.ThrowsExactlyAsync<KeyNotFoundException>(
            () => resolver.ResolveAsync("General", "Missing"));
    }

    private static ConfigLayerSnapshot Snapshot(ConfigLayer layer, object? value, bool isEnforced = false)
    {
        return new ConfigLayerSnapshot(layer,
        [
            new KeyValuePair<ConfigPropertyKey, ConfigLayerValue>(s_key, new ConfigLayerValue(value, isEnforced)),
        ]);
    }

    private sealed class ResolverSettings
    {
        public string Name { get; set; } = "product-default";
    }

    private sealed class ResolverSettingsProvider : IConfigSectionProvider
    {
        public string Name => "General";
        public string DisplayName => "General";
        public string? Description => null;
        public int Order => 0;
        public Type ModelType => typeof(ResolverSettings);
        public object CreateModel() => new ResolverSettings();
    }
}
