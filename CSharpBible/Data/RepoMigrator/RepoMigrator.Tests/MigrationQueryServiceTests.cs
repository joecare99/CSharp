using RepoMigrator.App.Logic.Services;
using RepoMigrator.Core;

namespace RepoMigrator.Tests;

[TestClass]
public sealed class MigrationQueryServiceTests
{
    [TestMethod]
    public void CreateQuery_ForSvnPrefersSelectedRevisionBoundary()
    {
        var queryService = new MigrationQueryService();
        var lstRevisions = CreateRevisions();

        var query = queryService.CreateQuery(RepoType.Svn, "100", null, 25, true, lstRevisions, "110", "120");

        Assert.AreEqual("100", query.FromExclusiveId);
        Assert.AreEqual("120", query.ToInclusiveId);
        Assert.AreEqual(25, query.MaxCount);
        Assert.IsTrue(query.OldestFirst);
    }

    [TestMethod]
    public void UpdateResumeAfterCommit_ForSvnReturnsNextRevision()
    {
        var queryService = new MigrationQueryService();
        var lstRevisions = CreateRevisions();

        var result = queryService.UpdateResumeAfterCommit(RepoType.Svn, "110", lstRevisions);

        Assert.AreEqual("110", result.FromExclusiveId);
        Assert.AreEqual("120", result.SelectedSvnFromRevisionId);
    }

    private static IReadOnlyList<RepositoryRevisionInfo> CreateRevisions()
        =>
        [
            new RepositoryRevisionInfo { Id = "100", AuthorName = "alice", Message = "Initial import", Timestamp = new DateTimeOffset(2024, 1, 1, 8, 0, 0, TimeSpan.Zero) },
            new RepositoryRevisionInfo { Id = "110", AuthorName = "bob", Message = "Feature A", Timestamp = new DateTimeOffset(2024, 1, 2, 8, 0, 0, TimeSpan.Zero) },
            new RepositoryRevisionInfo { Id = "120", AuthorName = "carol", Message = "Feature B", Timestamp = new DateTimeOffset(2024, 1, 3, 8, 0, 0, TimeSpan.Zero) }
        ];
}
