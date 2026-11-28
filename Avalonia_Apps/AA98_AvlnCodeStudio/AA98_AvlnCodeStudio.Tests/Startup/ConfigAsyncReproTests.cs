using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Config.Service;
using Config.UI.Avalonia.ViewModels;
using Config.UI.ConfigService;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Property.Editor.Avalonia;

namespace AA98_AvlnCodeStudio.Tests.Startup;

[TestClass]
public sealed class ConfigAsyncReproTests
{
    [TestMethod]
    public async Task EarlierLoadCompletingAfterReselectionMustNotEraseDraft()
    {
        var (config, sections, firstLoad) = Create();
        config.SelectedSection = sections[1];
        await WaitUntilAsync(() => config.PropertyEditor.Items.Count == 1);
        config.SelectedSection = sections[0];
        await WaitUntilAsync(() => config.PropertyEditor.Items.Count == 1);
        Assert.IsTrue(config.PropertyEditor.Items.Single().PropertyItem.TrySetValue("new draft"));
        Assert.IsTrue(config.HasUnsavedChanges);

        firstLoad.SetResult(new ConfigLayerSnapshot(ConfigLayer.System, []));
        await WaitUntilAsync(() => !config.IsBusy);

        Assert.AreEqual("new draft", config.PropertyEditor.Items.Single().PropertyItem.Value);
        Assert.IsTrue(config.HasUnsavedChanges);
    }

    [TestMethod]
    public async Task EarlierFailureMustNotAppearAsCurrentSectionStatus()
    {
        var (config, sections, firstLoad) = Create();
        config.SelectedSection = sections[1];
        await WaitUntilAsync(() => config.PropertyEditor.Items.Count == 1);
        firstLoad.SetException(new InvalidOperationException("old provider failure marker"));
        await WaitUntilAsync(() => !config.IsBusy);

        Assert.AreSame(sections[1], config.SelectedSection);
        Assert.AreEqual(string.Empty, config.OperationMessage);
    }

    private static (ConfigUiViewModel, System.Collections.Generic.IReadOnlyList<global::Config.UI.IConfigUiSection>, TaskCompletionSource<ConfigLayerSnapshot>) Create()
    {
        ConfigSectionRegistry registry = new();
        registry.AddSection(Provider("First", 0));
        registry.AddSection(Provider("Second", 1));
        IConfigLayerStore user = Substitute.For<IConfigLayerStore>();
        IConfigLayerStore system = Substitute.For<IConfigLayerStore>();
        user.Layer.Returns(ConfigLayer.User);
        system.Layer.Returns(ConfigLayer.System);
        user.LoadAsync(Arg.Any<CancellationToken>()).Returns(new ConfigLayerSnapshot(ConfigLayer.User, []));
        TaskCompletionSource<ConfigLayerSnapshot> firstLoad = new(TaskCreationOptions.RunContinuationsAsynchronously);
        int calls = 0;
        system.LoadAsync(Arg.Any<CancellationToken>()).Returns(_ =>
            Interlocked.Increment(ref calls) == 1
                ? firstLoad.Task
                : Task.FromResult(new ConfigLayerSnapshot(ConfigLayer.System, [])));
        ConfigLayeredSettingsResolver resolver = new(registry, [system, user]);
        ConfigLayeredUiSectionRegistry uiRegistry = new(registry, resolver, user, ConfigLayer.User);
        return (new ConfigUiViewModel(uiRegistry, new PropertyEditorViewModel()), uiRegistry.Sections, firstLoad);
    }

    private static IConfigSectionProvider Provider(string name, int order)
    {
        IConfigSectionProvider provider = Substitute.For<IConfigSectionProvider>();
        provider.Name.Returns(name);
        provider.DisplayName.Returns(name);
        provider.Order.Returns(order);
        provider.ModelType.Returns(typeof(Settings));
        provider.CreateModel().Returns(_ => new Settings());
        return provider;
    }

    private static async Task WaitUntilAsync(Func<bool> condition)
    {
        for (int i = 0; i < 500 && !condition(); i++)
        {
            await Task.Delay(10);
        }
        Assert.IsTrue(condition(), "Timed out waiting for the operation.");
    }

    public sealed class Settings
    {
        public string Name { get; set; } = "stored";
    }
}
