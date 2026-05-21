using NSubstitute;
using RepoMigrator.App.Logic.Services;
using RepoMigrator.Core;
using RepoMigrator.Core.Abstractions;

namespace RepoMigrator.Tests;

[TestClass]
public sealed class RepositorySelectionServiceTests
{
    [TestMethod]
    public async Task LoadSourceSelectionAsync_ReturnsEmptySelection_WhenProviderDoesNotSupportSelection()
    {
        var providerFactory = Substitute.For<IProviderFactory>();
        var provider = Substitute.For<IVersionControlProvider>();
        providerFactory.Create("git").Returns(provider);
        provider.GetCapabilitiesAsync(Arg.Any<RepositoryEndpoint>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new RepositoryCapabilities()));
        provider.DisposeAsync().Returns(ValueTask.CompletedTask);

        var service = new RepositorySelectionService(providerFactory);

        var result = await service.LoadSourceSelectionAsync(new RepositoryEndpoint { ProviderKey = "git", UrlOrPath = "/repo" }, CancellationToken.None);

        Assert.IsNotNull(result);
        Assert.IsNotNull(result.Capabilities);
        Assert.AreEqual(0, result.Branches.Count);
        Assert.AreEqual(0, result.Tags.Count);
        Assert.AreEqual(0, result.Revisions.Count);
        Assert.IsNull(result.DefaultBranch);
        Assert.IsNull(result.SuggestedFromRevisionId);
        Assert.IsNull(result.SuggestedToRevisionId);
        await provider.DidNotReceive().GetSelectionDataAsync(Arg.Any<RepositoryEndpoint>(), Arg.Any<CancellationToken>());
    }

    [TestMethod]
    public async Task LoadSourceSelectionAsync_ReturnsProviderSelection_WhenCapabilitiesAreSupported()
    {
        var providerFactory = Substitute.For<IProviderFactory>();
        var provider = Substitute.For<IVersionControlProvider>();
        providerFactory.Create("svn").Returns(provider);

        var capabilities = new RepositoryCapabilities
        {
            SupportsBranchSelection = true,
            SupportsTagSelection = true,
            SupportsRevisionSelection = true
        };

        var selectionData = new RepositorySelectionData
        {
            DefaultBranch = "main",
            SuggestedFromRevisionId = "100",
            SuggestedToRevisionId = "120",
            Branches = [new RepositoryReferenceInfo { Name = "main" }],
            Tags = [new RepositoryReferenceInfo { Name = "v1.0" }],
            Revisions = [new RepositoryRevisionInfo { Id = "120", AuthorName = "alice", Message = "msg", Timestamp = DateTimeOffset.UtcNow }]
        };

        provider.GetCapabilitiesAsync(Arg.Any<RepositoryEndpoint>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(capabilities));
        provider.GetSelectionDataAsync(Arg.Any<RepositoryEndpoint>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(selectionData));
        provider.DisposeAsync().Returns(ValueTask.CompletedTask);

        var service = new RepositorySelectionService(providerFactory);

        var result = await service.LoadSourceSelectionAsync(new RepositoryEndpoint { ProviderKey = "svn", UrlOrPath = "svn://repo" }, CancellationToken.None);

        Assert.IsTrue(result.Capabilities.SupportsBranchSelection);
        Assert.IsTrue(result.Capabilities.SupportsTagSelection);
        Assert.IsTrue(result.Capabilities.SupportsRevisionSelection);
        Assert.AreEqual("main", result.DefaultBranch);
        Assert.AreEqual("100", result.SuggestedFromRevisionId);
        Assert.AreEqual("120", result.SuggestedToRevisionId);
        Assert.AreEqual(1, result.Branches.Count);
        Assert.AreEqual("main", result.Branches[0].Name);
        Assert.AreEqual(1, result.Tags.Count);
        Assert.AreEqual("v1.0", result.Tags[0].Name);
        Assert.AreEqual(1, result.Revisions.Count);
        Assert.AreEqual("120", result.Revisions[0].Id);
    }

    [TestMethod]
    public async Task LoadTargetSelectionAsync_ReturnsDefault_WhenEndpointIsNotGit()
    {
        var providerFactory = Substitute.For<IProviderFactory>();
        var provider = Substitute.For<IVersionControlProvider>();
        providerFactory.Create("svn").Returns(provider);
        provider.GetCapabilitiesAsync(Arg.Any<RepositoryEndpoint>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new RepositoryCapabilities()));
        provider.DisposeAsync().Returns(ValueTask.CompletedTask);
        var service = new RepositorySelectionService(providerFactory);

        var result = await service.LoadTargetSelectionAsync(new RepositoryEndpoint { ProviderKey = "svn", UrlOrPath = "svn://repo" }, CancellationToken.None);

        Assert.IsNotNull(result);
        Assert.IsNotNull(result.Capabilities);
        Assert.AreEqual(0, result.Branches.Count);
        Assert.IsNull(result.DefaultBranch);
        providerFactory.Received(1).Create("svn");
    }

    [TestMethod]
    public async Task LoadTargetSelectionAsync_DeduplicatesBranches_IgnoringCase()
    {
        var providerFactory = Substitute.For<IProviderFactory>();
        var provider = Substitute.For<IVersionControlProvider>();
        providerFactory.Create("git").Returns(provider);
        provider.GetCapabilitiesAsync(Arg.Any<RepositoryEndpoint>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new RepositoryCapabilities { SupportsBranchSelection = true }));

        provider.GetSelectionDataAsync(Arg.Any<RepositoryEndpoint>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new RepositorySelectionData
            {
                DefaultBranch = "main",
                Branches =
                [
                    new RepositoryReferenceInfo { Name = "Main" },
                    new RepositoryReferenceInfo { Name = "main" },
                    new RepositoryReferenceInfo { Name = "release" }
                ]
            }));
        provider.DisposeAsync().Returns(ValueTask.CompletedTask);

        var service = new RepositorySelectionService(providerFactory);

        var result = await service.LoadTargetSelectionAsync(new RepositoryEndpoint { ProviderKey = "git", UrlOrPath = "/repo" }, CancellationToken.None);

        Assert.IsTrue(result.Capabilities.SupportsBranchSelection);
        Assert.AreEqual("main", result.DefaultBranch);
        CollectionAssert.AreEquivalent(new[] { "Main", "release" }, result.Branches.ToArray());
    }
}
