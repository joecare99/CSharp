using RepoMigrator.App.Logic.Services;
using RepoMigrator.Core;

namespace RepoMigrator.Tests;

[TestClass]
public sealed class MigrationEndpointFactoryTests
{
    [TestMethod]
    public void CreateSourceEndpoint_MapsAllInputValues()
    {
        var factory = new MigrationEndpointFactory();

        var endpoint = factory.CreateSourceEndpoint("svn", "svn://example/repo", "trunk", "alice", "secret");

        Assert.AreEqual("svn", endpoint.ProviderKey);
        Assert.AreEqual("svn://example/repo", endpoint.UrlOrPath);
        Assert.AreEqual("trunk", endpoint.BranchOrTrunk);
        Assert.IsNotNull(endpoint.Credentials);
        Assert.AreEqual("alice", endpoint.Credentials.Username);
        Assert.AreEqual("secret", endpoint.Credentials.Password);
    }

    [TestMethod]
    public void CreateTargetEndpoint_MapsAllInputValues()
    {
        var factory = new MigrationEndpointFactory();

        var endpoint = factory.CreateTargetEndpoint("git", "C:\\target", "main", "bob", "token");

        Assert.AreEqual("git", endpoint.ProviderKey);
        Assert.AreEqual("C:\\target", endpoint.UrlOrPath);
        Assert.AreEqual("main", endpoint.BranchOrTrunk);
        Assert.IsNotNull(endpoint.Credentials);
        Assert.AreEqual("bob", endpoint.Credentials.Username);
        Assert.AreEqual("token", endpoint.Credentials.Password);
    }
}
