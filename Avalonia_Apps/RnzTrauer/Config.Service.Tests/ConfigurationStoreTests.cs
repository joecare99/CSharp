using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Config.Service.Tests;

[TestClass]
public class ConfigurationStoreTests
{
    private static readonly string TestKey = "TestSection";
    private readonly IConfigSectionRegistry _mockRegistry = Substitute.For<IConfigSectionRegistry>();

    [TestInitialize]
    public void InitializeIsolatedStorage()
    {
        Environment.SetEnvironmentVariable("CONFIG_ROOT", CreateTestRoot());
    }

    [TestCleanup]
    public void CleanupIsolatedStorage()
    {
        var root = Environment.GetEnvironmentVariable("CONFIG_ROOT");
        Environment.SetEnvironmentVariable("CONFIG_ROOT", null);

        if (!string.IsNullOrWhiteSpace(root) && Directory.Exists(root))
        {
            Directory.Delete(root, true);
        }
    }

    private static string CreateTestRoot()
    {
        var root = Path.Combine(Path.GetTempPath(), "Config.Service.Tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        return root;
    }

    [TestMethod]
    public async Task RoundTripPersist_SavesAndLoadsModel()
    {
        // Arrange
        var store = new JsonConfigStore(_mockRegistry);
        var model = new DatabaseConfig
        {
            Server = "localhost",
            Port = 3306,
            Database = "testdb"
        };

        // Act & Assert - Save (creates file if missing)
        await store.SaveAsync(TestKey, model);

        // Reload
        var loaded = await store.LoadAsync<TestConfig>(TestKey, new TestConfig());

        Assert.AreEqual(model.Database, loaded.Database);
        Assert.AreEqual(model.Server, loaded.Server);
        Assert.AreEqual(model.Port, loaded.Port);
    }

    [TestMethod]
    public async Task MissingFile_ReturnsFallbackValue()
    {
        // Arrange
        var store = new JsonConfigStore(_mockRegistry);
        var fallback = new TestConfig
        {
            Server = "fallback",
            Port = 5432,
            Database = "fallbackdb"
        };

        // Act
        var loaded = await store.LoadAsync<TestConfig>(TestKey+"_", fallback);

        // Assert
        Assert.AreEqual(fallback, loaded);
    }

    [TestMethod]
    public async Task MultiSection_SupportsMultipleKeys()
    {
        // Arrange
        var section1 = new ConfigSectionProvider("Database", "Datenbank", 0);
        var section2 = new ConfigSectionProvider("Cache", "Cache-Einstellungen", 1);
        var store = new JsonConfigStore(_mockRegistry);

        // Act
        await store.SaveAsync("Database.KeyA", "value1");
        await store.SaveAsync("Database.KeyB", "value2");
        await store.SaveAsync("Cache.ValueX", "cache-value");

        // Assert
        var loadedValue1 = await store.LoadAsync<string>("Database.KeyA", null);
        Assert.AreEqual("value1", loadedValue1);
        var loadedValue2 = await store.LoadAsync<string>("Database.KeyB", null);
        Assert.AreEqual("value2", loadedValue2);
        var loadedValue3 = await store.LoadAsync<string>("Cache.ValueX", null);
        Assert.AreEqual("cache-value", loadedValue3);
    }

    [TestMethod]
    public async Task Sensitive_DetectsMasking()
    {
        // Arrange
        var section = new SensitiveSectionProvider();
        var store = new JsonConfigStore(_mockRegistry);
        var model = new TestModel
        {
            Server = "test",
            Port = 3306,
            Password = "secret123"
        };

        await store.SaveAsync("Sensitive.Test", model);

        // Act
        var loaded = await store.LoadAsync<TestModel>("Sensitive.Test", null);

        // Assert - Load works normally; masking is UI-side.
        Assert.AreEqual("secret123", loaded.Password);
    }

    [TestMethod]
    public async Task RegistryEvent_FiresOnChange()
    {
        // Arrange
        var registry = new ConfigSectionRegistry();
        var eventsReceived = 0;
        registry.SectionsChanged += () => { eventsReceived++; };

        // Act - Initial event (should be 1 if registered)
        await Task.Run(() =>
        {
            foreach (var s in (IList<ConfigSectionProvider>)[new ("Section1","1",1), new ("ToBeRemoved","2",2)])
            {
                registry.AddSection(s);
            }
        });

        // Remove one section
        var removed = registry.RemoveSection("ToBeRemoved");

        // Assert
        Assert.IsTrue(removed);
        Assert.IsTrue(eventsReceived >= 3);
    }

    [TestMethod]
    public async Task LookupStringAndEnumValues_WorksAcrossSections()
    {
        // Arrange
        var registry = new ConfigSectionRegistry();
        var store = new JsonConfigStore(registry, "Contoso", "DemoApp");
        var configService = new ConfigService(store, registry, "Contoso.DemoApp", "Contoso", "DemoApp");

        await configService.SaveAsync("General", new TestConfig
        {
            Server = "example.local",
            Port = 4242,
            Database = "lookupdb",
            Mode = TestMode.Manual
        });

        // Act
        var server = await configService.GetStringValueAsync<TestConfig>("General", nameof(TestConfig.Server), "fallback");
        var mode = await configService.GetEnumValueAsync<TestConfig, TestMode>("General", nameof(TestConfig.Mode), TestMode.Automatic);
        var valid = await configService.ValidateSectionAsync<TestConfig>("General");

        // Assert
        Assert.AreEqual("example.local", server);
        Assert.AreEqual(TestMode.Manual, mode);
        Assert.IsTrue(valid);
    }

    [TestMethod]
    public void RegistryCanResolveSectionsByName()
    {
        // Arrange
        var registry = new ConfigSectionRegistry();
        var section = new ConfigSectionProvider("Settings", "General settings", 1);

        // Act
        registry.AddSection(section);

        // Assert
        Assert.IsTrue(registry.TryGetSection("settings", out var resolved));
        Assert.AreEqual("Settings", resolved?.Name);
    }

    [TestMethod]
    public void RegistrationDescription_CanBeProvidedFromTheUiLayer()
    {
        // Arrange
        var provider = new ConfigSectionProvider("Settings", "General settings", 1);

        // Act
        var registration = new ConfigSectionRegistration(provider, "Localized explanation");

        // Assert
        Assert.AreEqual("Localized explanation", registration.Description);
        Assert.AreEqual("Settings", registration.Name);
    }

    [TestMethod]
    public void AddConfigService_ResolvesCoreServicesAsSingletons()
    {
        var services = new ServiceCollection();

        services.AddConfigService("Contoso", "CodeStudio");

        using var provider = services.BuildServiceProvider();
        var configService = provider.GetRequiredService<ConfigService>();
        var store = provider.GetRequiredService<IConfigStore>();
        var registry = provider.GetRequiredService<IConfigSectionRegistry>();

        Assert.AreSame(configService, provider.GetRequiredService<ConfigService>());
        Assert.AreSame(store, provider.GetRequiredService<IConfigStore>());
        Assert.AreSame(registry, provider.GetRequiredService<IConfigSectionRegistry>());
        Assert.AreEqual("Contoso.CodeStudio", configService.BaseKey);
        Assert.AreEqual("Contoso", configService.VendorName);
        Assert.AreEqual("CodeStudio", configService.ApplicationName);
    }

    [TestMethod]
    public void AddConfigSection_RegistersProvidersInRegistryOrder()
    {
        var services = new ServiceCollection();

        services
            .AddConfigService("Contoso", "CodeStudio")
            .AddConfigSection<TestConfig>(new ConfigSectionProvider("Later", "Later", 20))
            .AddConfigSection<TestConfig>(new ConfigSectionProvider("Earlier", "Earlier", 10));

        using var provider = services.BuildServiceProvider();
        var registry = provider.GetRequiredService<IConfigSectionRegistry>();
        var sections = new List<IConfigSectionProvider>(registry.Sections);

        Assert.AreEqual(2, sections.Count);
        Assert.AreEqual("Earlier", sections[0].Name);
        Assert.AreEqual("Later", sections[1].Name);
        Assert.AreEqual(2, provider.GetServices<IConfigSectionProvider>().Count());
    }

    [TestMethod]
    public void AddConfigSection_PreservesUiDescriptionOverride()
    {
        var section = new ConfigSectionProvider("General", "General", 0);
        var services = new ServiceCollection()
            .AddConfigService("Contoso", "CodeStudio")
            .AddConfigSection<TestConfig>(section, "Localized description");

        using var provider = services.BuildServiceProvider();
        var registration = provider.GetRequiredService<ConfigSectionRegistration>();

        Assert.AreEqual("Localized description", registration.Description);
        Assert.AreSame(section, registration.Provider);
    }

    [TestMethod]
    public void AddConfigService_RejectsInvalidArguments()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() =>
            ServiceCollectionExtensions.AddConfigService((IServiceCollection)null, "Vendor", "Application"));

        Assert.ThrowsExactly<ArgumentException>(() =>
            new ServiceCollection().AddConfigService(string.Empty, "Application"));

        Assert.ThrowsExactly<ArgumentException>(() =>
            new ServiceCollection().AddConfigService("Vendor", string.Empty));

        Assert.ThrowsExactly<ArgumentException>(() =>
            new ServiceCollection().AddConfigService("."));
    }

    [TestMethod]
    public void AddConfigSection_RejectsInvalidArguments()
    {
        var services = new ServiceCollection();

        Assert.ThrowsExactly<ArgumentNullException>(() =>
            ServiceCollectionExtensions.AddConfigSection<TestConfig>((IServiceCollection)null, new ConfigSectionProvider("General", "General", 0)));

        Assert.ThrowsExactly<ArgumentNullException>(() =>
            services.AddConfigSection<TestConfig>((IConfigSectionProvider)null));
    }

    [TestMethod]
    public async Task ResetAsync_RemovesOnlyRequestedSection()
    {
        var store = new JsonConfigStore(_mockRegistry, "Contoso", "PersistenceTests");
        var first = new TestConfig { Server = "first" };
        var second = new TestConfig { Server = "second" };

        await store.SaveAsync("First", first);
        await store.SaveAsync("Second", second);
        await store.ResetAsync("First");

        var resetValue = await store.LoadAsync("First", new TestConfig { Server = "fallback" });
        var preservedValue = await store.LoadAsync("Second", new TestConfig { Server = "fallback" });

        Assert.AreEqual("fallback", resetValue.Server);
        Assert.AreEqual("second", preservedValue.Server);
    }

    [TestMethod]
    public async Task ConfigRootOverride_UsesVendorApplicationAndConfigSegments()
    {
        var root = Environment.GetEnvironmentVariable("CONFIG_ROOT");
        var store = new JsonConfigStore(_mockRegistry, "Contoso", "PersistenceTests");

        await store.SaveAsync("General", new TestConfig { Server = "override" });

        var expectedFile = Path.Combine(root!, "Contoso", "PersistenceTests", "config", "General.json");
        Assert.IsTrue(File.Exists(expectedFile));
    }

    [TestMethod]
    public async Task InvalidJson_ReturnsFallbackValue()
    {
        var root = Environment.GetEnvironmentVariable("CONFIG_ROOT");
        var store = new JsonConfigStore(_mockRegistry, "Contoso", "PersistenceTests");
        var path = Path.Combine(root!, "Contoso", "PersistenceTests", "config", "Invalid.json");
        await File.WriteAllTextAsync(path, "{ invalid json");

        var fallback = new TestConfig { Server = "fallback" };
        var loaded = await store.LoadAsync("Invalid", fallback);

        Assert.AreSame(fallback, loaded);
    }

    [TestMethod]
    public async Task MissingProperties_UseModelDefaults()
    {
        var root = Environment.GetEnvironmentVariable("CONFIG_ROOT");
        var store = new JsonConfigStore(_mockRegistry, "Contoso", "PersistenceTests");
        var path = Path.Combine(root!, "Contoso", "PersistenceTests", "config", "Defaults.json");
        await File.WriteAllTextAsync(path, "{\"server\":\"configured\"}");

        var loaded = await store.LoadAsync("Defaults", new TestConfig());

        Assert.AreEqual("configured", loaded.Server);
        Assert.AreEqual(5432, loaded.Port);
        Assert.AreEqual("testdb", loaded.Database);
    }

    [TestMethod]
    public async Task UnsupportedEnumValue_UsesLookupFallback()
    {
        var registry = new ConfigSectionRegistry();
        var store = new JsonConfigStore(registry, "Contoso", "PersistenceTests");
        var configService = new ConfigService(store, registry, "Contoso.PersistenceTests", "Contoso", "PersistenceTests");
        var root = Environment.GetEnvironmentVariable("CONFIG_ROOT");
        var path = Path.Combine(root!, "Contoso", "PersistenceTests", "config", "Contoso.PersistenceTests.General.json");
        await File.WriteAllTextAsync(path, "{\"mode\":\"Unsupported\"}");

        var mode = await configService.GetEnumValueAsync<TestConfig, TestMode>("General", "Mode", TestMode.Manual);

        Assert.AreEqual(TestMode.Manual, mode);
    }

    [TestMethod]
    public async Task LookupFallbacks_ReturnFallbackForMissingProperties()
    {
        var registry = new ConfigSectionRegistry();
        var store = new JsonConfigStore(registry, "Contoso", "PersistenceTests");
        var configService = new ConfigService(store, registry, "Contoso.PersistenceTests", "Contoso", "PersistenceTests");

        var stringValue = await configService.GetStringValueAsync<TestConfig>("Missing", "Server", "fallback");
        var enumValue = await configService.GetEnumValueAsync<TestConfig, TestMode>("Missing", "Mode", TestMode.Manual);

        Assert.AreEqual("fallback", stringValue);
        Assert.AreEqual(TestMode.Manual, enumValue);
    }
}
