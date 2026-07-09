using AppKomponentBaseLib.Components;
using AppKomponentBaseLib.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace AA98_AvlnCodeStudio.Tests.Components.Configuration;

/// <summary>
/// Verifies the shared application-component configuration contracts.
/// </summary>
[TestClass]
public sealed class AppComponentContractTests
{
    /// <summary>
    /// Verifies that component descriptors preserve normalized metadata.
    /// </summary>
    [TestMethod]
    public void ComponentDescriptorStoresMetadataAndCapabilities()
    {
        var descriptor = new AppComponentDescriptor(
            "ImageEditor",
            "Image Editor",
            new[] { "documents", "documents", " tools ", "" },
            "Provides image editing features.");

        Assert.AreEqual("ImageEditor", descriptor.ComponentId);
        Assert.AreEqual("Image Editor", descriptor.DisplayName);
        Assert.AreEqual("Provides image editing features.", descriptor.Description);
        CollectionAssert.AreEqual(new[] { "documents", "tools" }, (System.Collections.ICollection)descriptor.Capabilities);
    }

    /// <summary>
    /// Verifies that component descriptors reject missing identifiers and names.
    /// </summary>
    [TestMethod]
    public void ComponentDescriptorRejectsMissingIdentity()
    {
        Assert.ThrowsExactly<ArgumentException>(() => new AppComponentDescriptor(string.Empty, "Component"));
        Assert.ThrowsExactly<ArgumentException>(() => new AppComponentDescriptor("Component", string.Empty));
    }

    /// <summary>
    /// Verifies that setting descriptors preserve key metadata.
    /// </summary>
    [TestMethod]
    public void SettingDescriptorStoresMetadata()
    {
        var descriptor = new AppSettingDescriptor(
            "Editor.ZoomStep",
            "Zoom Step",
            "Editor",
            AppSettingScope.User,
            "Controls the zoom increment.",
            10);

        Assert.AreEqual("Editor.ZoomStep", descriptor.SettingKey);
        Assert.AreEqual("Zoom Step", descriptor.DisplayName);
        Assert.AreEqual("Editor", descriptor.SectionKey);
        Assert.AreEqual(AppSettingScope.User, descriptor.Scope);
        Assert.AreEqual("Controls the zoom increment.", descriptor.Description);
        Assert.AreEqual(10, descriptor.DefaultValue);
    }

    /// <summary>
    /// Verifies that setting descriptors reject incomplete metadata.
    /// </summary>
    [TestMethod]
    public void SettingDescriptorRejectsMissingIdentity()
    {
        Assert.ThrowsExactly<ArgumentException>(() => new AppSettingDescriptor(string.Empty, "Name", "Section", AppSettingScope.Application));
        Assert.ThrowsExactly<ArgumentException>(() => new AppSettingDescriptor("Key", string.Empty, "Section", AppSettingScope.Application));
        Assert.ThrowsExactly<ArgumentException>(() => new AppSettingDescriptor("Key", "Name", string.Empty, AppSettingScope.Application));
    }

    /// <summary>
    /// Verifies that settings contributors can expose component metadata and registrations together.
    /// </summary>
    [TestMethod]
    public void SettingsContributorCombinesRegistrationAndSettings()
    {
        var contributor = new TestSettingsContributor();
        var services = new ServiceCollection();

        contributor.Register(services);

        Assert.AreEqual("Test.Component", contributor.Descriptor.ComponentId);
        Assert.AreEqual(1, contributor.Settings.Count);
        Assert.AreEqual("Test.Component", contributor.Settings[0].ComponentId);
        Assert.AreEqual(1, services.Count);
    }

    private sealed class TestSettingsContributor : IAppSettingsContributor
    {
        public AppComponentDescriptor Descriptor { get; } = new("Test.Component", "Test Component");

        public IReadOnlyList<IAppSettingContribution> Settings { get; } = new IAppSettingContribution[]
        {
            new TestSettingContribution()
        };

        public void Register(IServiceCollection services)
        {
            services.AddSingleton(this);
        }
    }

    private sealed class TestSettingContribution : IAppSettingContribution
    {
        public string ComponentId => "Test.Component";

        public AppSettingDescriptor Descriptor { get; } = new(
            "Test.Component.Enabled",
            "Enabled",
            "General",
            AppSettingScope.User,
            "Controls whether the component is enabled.",
            true);
    }
}
