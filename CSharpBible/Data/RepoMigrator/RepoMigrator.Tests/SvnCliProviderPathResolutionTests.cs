using System.Reflection;
using RepoMigrator.Core;
using RepoMigrator.Providers.SvnCli;

namespace RepoMigrator.Tests;

[TestClass]
public sealed class SvnCliProviderPathResolutionTests
{
    [TestMethod]
    [DataRow("https://svn.example.local/repos/demo", null, "https://svn.example.local/repos/demo")]
    [DataRow("https://svn.example.local/repos/demo", "", "https://svn.example.local/repos/demo")]
    [DataRow("https://svn.example.local/repos/demo", "trunk", "https://svn.example.local/repos/demo/trunk")]
    [DataRow("https://svn.example.local/repos/demo/", "/branches/release", "https://svn.example.local/repos/demo/branches/release")]
    [DataRow("svn://server/repo", "branches\\legacy", "svn://server/repo/branches/legacy")]
    public void ResolveRemoteEndpointPath_AppendsBranchOrTrunk_WhenProvided(string sUrlOrPath, string? sBranchOrTrunk, string sExpectedPath)
    {
        var endpoint = new RepositoryEndpoint
        {
            Type = RepoType.Svn,
            UrlOrPath = sUrlOrPath,
            BranchOrTrunk = sBranchOrTrunk
        };

        var sResolvedPath = InvokeResolveRemoteEndpointPath(endpoint);

        Assert.AreEqual(sExpectedPath, sResolvedPath);
    }

    private static string InvokeResolveRemoteEndpointPath(RepositoryEndpoint endpoint)
    {
        var method = typeof(SvnCliProvider).GetMethod("ResolveRemoteEndpointPath", BindingFlags.NonPublic | BindingFlags.Static);
        Assert.IsNotNull(method);
        return (string)method.Invoke(null, new object[] { endpoint })!;
    }
}
