using Config.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Config.UI.ConfigService.Tests;

/// <summary>
/// Verifies Config.Service persistence adaptation without product model types.
/// </summary>
[TestClass]
public sealed class ConfigServiceUiSectionTests
{
    [TestMethod]
    public async Task LoadAsync_ValidModel_ExposesTypedEditableProperties()
    {
        IConfigStore store = Substitute.For<IConfigStore>();
        TestConfig model = new() { Server = "database", Port = 3306, Mode = TestMode.Automatic };
        store.LoadAsync("TestHost.General", Arg.Any<TestConfig>()).Returns(Task.FromResult(model));
        ConfigServiceUiSection section = new(new global::Config.Service.ConfigService(store, "TestHost"), new TestSectionProvider());

        await section.LoadAsync();

        Assert.AreEqual(ConfigUiSectionState.Ready, section.State);
        Assert.AreEqual(4, section.Properties.Count);
        Assert.AreEqual("database", section.Properties.Single(static property => property.Name == "Server").Value);
        Assert.AreEqual(2, section.Properties.Single(static property => property.Name == "Mode").Options.Count);
        Assert.IsTrue(section.Properties.Single(static property => property.Name == "Password").IsSensitive);
    }

    [TestMethod]
    public async Task SaveAsync_ChangedProperty_PersistsUpdatedModel()
    {
        IConfigStore store = Substitute.For<IConfigStore>();
        TestConfig model = new() { Server = "database" };
        store.LoadAsync("TestHost.General", Arg.Any<TestConfig>()).Returns(Task.FromResult(model));
        ConfigServiceUiSection section = new(new global::Config.Service.ConfigService(store, "TestHost"), new TestSectionProvider());
        await section.LoadAsync();

        Assert.IsTrue(section.Properties.Single(static property => property.Name == "Server").TrySetValue("updated"));
        await section.SaveAsync();

        await store.Received(1).SaveAsync("TestHost.General", Arg.Is<TestConfig>(saved => saved.Server == "updated"));
        Assert.AreEqual(ConfigUiSectionState.Ready, section.State);
    }

    [TestMethod]
    public async Task ResetAsync_ReadOnlySection_DoesNotMutateStore()
    {
        IConfigStore store = Substitute.For<IConfigStore>();
        ConfigServiceUiSection section = new(new global::Config.Service.ConfigService(store, "TestHost"), new TestSectionProvider(), isReadOnly: true);

        await section.ResetAsync();

        await store.DidNotReceive().ResetAsync(Arg.Any<string>());
        Assert.AreEqual(ConfigUiSectionState.Failed, section.State);
        Assert.IsNotNull(section.ErrorMessage);
    }

    [TestMethod]
    public async Task SaveAsync_InvalidProperty_DoesNotPersist()
    {
        IConfigStore store = Substitute.For<IConfigStore>();
        TestConfig model = new() { Port = 3306 };
        store.LoadAsync("TestHost.General", Arg.Any<TestConfig>()).Returns(Task.FromResult(model));
        ConfigServiceUiSection section = new(new global::Config.Service.ConfigService(store, "TestHost"), new TestSectionProvider());
        await section.LoadAsync();

        Assert.IsFalse(section.Properties.Single(static property => property.Name == "Port").TrySetValue("invalid"));
        await section.SaveAsync();

        await store.DidNotReceive().SaveAsync(Arg.Any<string>(), Arg.Any<TestConfig>());
        Assert.AreEqual(ConfigUiSectionState.Failed, section.State);
    }

    private sealed class TestSectionProvider : IConfigSectionProvider
    {
        public string Name => "General";
        public string DisplayName => "General settings";
        public string? Description => null;
        public int Order => 0;
        public Type ModelType => typeof(TestConfig);
        public object CreateModel() => new TestConfig();
    }

    private sealed class TestConfig
    {
        public string? Server { get; set; }
        public int Port { get; set; } = 1;
        public TestMode Mode { get; set; }
        [SensitiveConfigProperty]
        public string? Password { get; set; }
        [ConfigIgnore]
        public string Ignored => "ignored";
    }

    private enum TestMode
    {
        Manual,
        Automatic,
    }
}
