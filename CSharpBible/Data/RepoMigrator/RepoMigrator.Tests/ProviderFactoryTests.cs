using Microsoft.Extensions.DependencyInjection;
using RepoMigrator.App.Wpf;
using RepoMigrator.Core;
using RepoMigrator.Core.Abstractions;
using RepoMigrator.Providers.Patch.Services;
using RepoMigrator.Providers.Git;
using RepoMigrator.Providers.Svn;

namespace RepoMigrator.Tests;

[TestClass]
public sealed class ProviderFactoryTests
{
    [TestMethod]
    public async Task Create_WhenTypeIsGit_ReturnsRegisteredGitProvider()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IVersionControlProvider, GitProvider>();
        await using var provider = services.BuildServiceProvider();
        var factory = new ProviderFactory(provider);

        var createdProvider = factory.Create("git");

        Assert.IsInstanceOfType<GitProvider>(createdProvider);
    }

    [TestMethod]
    public async Task Create_WhenTypeIsSvn_ReturnsFirstRegisteredSvnProvider()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IVersionControlProvider, GitProvider>();
        services.AddSingleton<IVersionControlProvider, SvnProvider>();
        await using var provider = services.BuildServiceProvider();
        var factory = new ProviderFactory(provider);

        var createdProvider = factory.Create("svn");

        Assert.IsInstanceOfType<SvnProvider>(createdProvider);
    }

    [TestMethod]
    public void Create_WhenTypeUnsupported_ThrowsNotSupportedException()
    {
        var services = new ServiceCollection();
        using var provider = services.BuildServiceProvider();
        var factory = new ProviderFactory(provider);

        NotSupportedException? ex = null;
        try
        {
            _ = factory.Create("custom");
            Assert.Fail("Expected NotSupportedException.");
        }
        catch (NotSupportedException caughtEx)
        {
            ex = caughtEx;
        }

        Assert.IsNotNull(ex);
        StringAssert.Contains(ex.Message, "custom");
    }

    [TestMethod]
    public async Task Bootstrap_RegistersDirectoryPatchChangeSetSource()
    {
        await using var provider = Bootstrap.Create();

        var changeSetSources = provider.GetServices<IMigrationChangeSetSource>().ToArray();

        Assert.AreEqual(1, changeSetSources.Length);
        Assert.IsInstanceOfType<DirectoryPatchChangeSetSource>(changeSetSources[0]);
    }

    [TestMethod]
    public async Task Bootstrap_RegistersMigrationChangeSetSourceFactory()
    {
        await using var provider = Bootstrap.Create();

        var factory = provider.GetRequiredService<IMigrationChangeSetSourceFactory>();

        Assert.IsNotNull(factory);
    }

    [TestMethod]
    public async Task Bootstrap_RegistersGitStructuredChangeSetSink()
    {
        await using var provider = Bootstrap.Create();

        var sinks = provider.GetServices<IMigrationChangeSetSink>().ToArray();

        Assert.AreEqual(1, sinks.Length);
        Assert.IsInstanceOfType<GitStructuredChangeSetSink>(sinks[0]);
    }

    [TestMethod]
    public async Task Bootstrap_RegistersMigrationChangeSetSinkFactory()
    {
        await using var provider = Bootstrap.Create();

        var factory = provider.GetRequiredService<IMigrationChangeSetSinkFactory>();

        Assert.IsNotNull(factory);
    }

    [TestMethod]
    public async Task Bootstrap_RegistersStructuredMigrationPlanner()
    {
        await using var provider = Bootstrap.Create();

        var planner = provider.GetRequiredService<IStructuredMigrationPlanner>();

        Assert.IsNotNull(planner);
    }
}
