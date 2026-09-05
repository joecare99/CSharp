using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RnzTrauer.Core.Domain;
using RnzTrauer.Core.Services;

namespace RnzTrauer.Core.Tests;

[TestClass]
public sealed class NoticeSearchServiceTests
{
    [TestMethod]
    public async Task SearchAsyncReturnsRepositoryResults()
    {
        var repository = new FakeNoticeRepository
        {
            Notices = new[] { new DeathNotice { Id = 12 } },
        };
        var service = new NoticeSearchService(repository);

        var result = await service.SearchAsync(
            new NoticeSearchRequest(new NoticeFilter("A-1")));

        Assert.AreEqual(NoticeSearchStatus.Succeeded, result.Status);
        Assert.AreEqual(12, result.Notices[0].Id);
        Assert.IsFalse(ReferenceEquals(repository.Notices[0], result.Notices[0]));
        Assert.AreEqual(repository.Notices[0].OrderNumber, result.Notices[0].OrderNumber);
        Assert.AreEqual("A-1", repository.Filter?.OrderNumberPrefix);
    }

    [TestMethod]
    public async Task SearchAsyncReturnsInvalidResultForControlCharacters()
    {
        var repository = new FakeNoticeRepository();
        var result = await new NoticeSearchService(repository).SearchAsync(
            new NoticeSearchRequest(new NoticeFilter(KeywordContains: "bad\nvalue")));

        Assert.AreEqual(NoticeSearchStatus.InvalidRequest, result.Status);
        Assert.AreEqual("search.invalid_text", result.ErrorCode);
        Assert.IsNull(repository.Filter);
    }

    [TestMethod]
    public async Task SearchAsyncReturnsFailureWithoutSwallowingCancellation()
    {
        var repository = new FakeNoticeRepository
        {
            Exception = new InvalidOperationException("offline"),
        };
        var result = await new NoticeSearchService(repository).SearchAsync(
            new NoticeSearchRequest(new NoticeFilter()));

        Assert.AreEqual(NoticeSearchStatus.Failed, result.Status);
        Assert.AreEqual("search.failed", result.ErrorCode);
        StringAssert.Contains(result.ErrorMessage, "offline");

        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();
        try
        {
            await new NoticeSearchService(new FakeNoticeRepository()).SearchAsync(
                new NoticeSearchRequest(new NoticeFilter()), cancellation.Token);
            Assert.Fail("Cancellation should be propagated.");
        }
        catch (OperationCanceledException)
        {
            // Expected: cancellation is not converted into a failed result.
        }
    }

    private sealed class FakeNoticeRepository : INoticeRepository
    {
        public IReadOnlyList<DeathNotice> Notices { get; init; } = Array.Empty<DeathNotice>();
        public NoticeFilter? Filter { get; private set; }
        public Exception? Exception { get; init; }

        public Task<IReadOnlyList<DeathNotice>> FindAsync(NoticeFilter filter, CancellationToken cancellationToken = default)
        {
            Filter = filter;
            if (Exception is not null)
                throw Exception;
            return Task.FromResult(Notices);
        }

        public Task SaveAsync(DeathNotice notice, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<bool> UpsertImportedAsync(DeathNotice notice, CancellationToken cancellationToken = default) => Task.FromResult(true);
        public Task<IReadOnlyList<string>> GetPlaceNamesAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<string>>(Array.Empty<string>());
        public Task<IReadOnlyList<DeathNotice>> GetLinkCandidatesAsync(long noticeId, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<DeathNotice>>(Array.Empty<DeathNotice>());
    }
}
