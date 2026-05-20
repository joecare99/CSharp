using Microsoft.Extensions.DependencyInjection;
using RepoMigrator.Core;
using RepoMigrator.Core.Abstractions;
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

        var createdProvider = factory.Create(RepoType.Git);

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

        var createdProvider = factory.Create(RepoType.Svn);

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
            _ = factory.Create(RepoType.Custom);
            Assert.Fail("Expected NotSupportedException.");
        }
        catch (NotSupportedException caughtEx)
        {
            ex = caughtEx;
        }

        Assert.IsNotNull(ex);
        StringAssert.Contains(ex.Message, RepoType.Custom.ToString());
    }
}
