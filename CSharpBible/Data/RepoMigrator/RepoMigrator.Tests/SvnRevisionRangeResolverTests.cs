using RepoMigrator.Core;

namespace RepoMigrator.Tests;

[TestClass]
public sealed class SvnRevisionRangeResolverTests
{
    [TestMethod]
    [DataRow(null, null)]
    [DataRow("100", null)]
    [DataRow("110", "100")]
    [DataRow("120", "110")]
    public void ResolveFromExclusiveId_ReturnsExpectedPreviousRevision(string? sSelectedRevisionId, string? sExpectedExclusiveId)
    {
        var lstRevisions = CreateRevisions();

        var sResolvedRevisionId = SvnRevisionRangeResolver.ResolveFromExclusiveId(lstRevisions, sSelectedRevisionId);

        Assert.AreEqual(sExpectedExclusiveId, sResolvedRevisionId);
    }

    [TestMethod]
    public void GetSuggestedFromRevisionId_ReturnsFirstRevision()
    {
        var lstRevisions = CreateRevisions();

        var sSuggestedRevisionId = SvnRevisionRangeResolver.GetSuggestedFromRevisionId(lstRevisions);

        Assert.AreEqual("100", sSuggestedRevisionId);
    }

    [TestMethod]
    [DataRow(null, null)]
    [DataRow("100", "110")]
    [DataRow("110", "120")]
    [DataRow("120", null)]
    public void GetNextRevisionId_ReturnsExpectedValue(string? sCurrentRevisionId, string? sExpectedNextRevisionId)
    {
        var lstRevisions = CreateRevisions();

        var sResolvedRevisionId = SvnRevisionRangeResolver.GetNextRevisionId(lstRevisions, sCurrentRevisionId);

        Assert.AreEqual(sExpectedNextRevisionId, sResolvedRevisionId);
    }

    [TestMethod]
    [DataRow(null, null)]
    [DataRow("", null)]
    [DataRow("120", "120")]
    public void ResolveToInclusiveId_ReturnsExpectedValue(string? sSelectedRevisionId, string? sExpectedRevisionId)
    {
        var sResolvedRevisionId = SvnRevisionRangeResolver.ResolveToInclusiveId(sSelectedRevisionId);

        Assert.AreEqual(sExpectedRevisionId, sResolvedRevisionId);
    }

    private static IReadOnlyList<RepositoryRevisionInfo> CreateRevisions()
        =>
        [
            new RepositoryRevisionInfo { Id = "100", AuthorName = "alice", Message = "Initial import", Timestamp = new DateTimeOffset(2024, 1, 1, 8, 0, 0, TimeSpan.Zero) },
            new RepositoryRevisionInfo { Id = "110", AuthorName = "bob", Message = "Feature A", Timestamp = new DateTimeOffset(2024, 1, 2, 8, 0, 0, TimeSpan.Zero) },
            new RepositoryRevisionInfo { Id = "120", AuthorName = "carol", Message = "Feature B", Timestamp = new DateTimeOffset(2024, 1, 3, 8, 0, 0, TimeSpan.Zero) }
        ];
}
