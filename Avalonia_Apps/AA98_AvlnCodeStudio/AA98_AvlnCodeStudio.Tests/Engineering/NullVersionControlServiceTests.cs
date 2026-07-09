using System.IO;
using System.Threading.Tasks;
using AA98_AvlnCodeStudio.Base.Versioning.Models;
using AA98_AvlnCodeStudio.Base.Versioning.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AA98_AvlnCodeStudio.Tests.Engineering;

/// <summary>
/// Verifies conservative repository workflow behavior of the fallback version control service.
/// </summary>
[TestClass]
public class NullVersionControlServiceTests
{
    /// <summary>
    /// Verifies that repository context is preserved when no repository root is explicitly known.
    /// </summary>
    [TestMethod]
    public async Task GetStatusAsync_PreservesRepositoryContextWhenRootIsUnknown()
    {
        string repositoryContextPath = Path.Combine("C:", "src", "AA98");
        NullVersionControlService service = new();

        VersionControlStatus status = await service.GetStatusAsync(new VersionControlStatusRequest
        {
            RepositoryContextPath = repositoryContextPath,
        }).ConfigureAwait(false);

        Assert.AreEqual(repositoryContextPath, status.RepositoryRootPath);
        Assert.AreEqual("AA98", status.RepositoryName);
        Assert.IsFalse(status.IsRepositoryRootDiscovered);
        Assert.IsFalse(status.HasLocalChanges);
        CollectionAssert.AreEqual(
            new[]
            {
                VersionControlCapability.InspectStatus,
            },
            (System.Collections.ICollection)status.Capabilities);
    }

    /// <summary>
    /// Verifies that capability reporting can be suppressed for neutral fallback inspection.
    /// </summary>
    [TestMethod]
    public async Task GetStatusAsync_SuppressesCapabilitiesWhenDisabled()
    {
        NullVersionControlService service = new();

        VersionControlStatus status = await service.GetStatusAsync(new VersionControlStatusRequest
        {
            RepositoryRootPath = Path.Combine("C:", "src", "AA98"),
            IncludeCapabilities = false,
        }).ConfigureAwait(false);

        Assert.AreEqual(0, status.Capabilities.Count);
    }
}