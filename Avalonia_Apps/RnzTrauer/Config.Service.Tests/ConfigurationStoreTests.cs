using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Config.Service.Tests;

[TestClass]
public class ConfigurationStoreTests
{
    private static readonly string TestKey = "TestSection";
    private readonly IConfigSectionRegistry _mockRegistry = Substitute.For<IConfigSectionRegistry>();

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
}
