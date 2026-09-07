using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RnzTrauer.Core.Domain;
using RnzTrauer.Core.Services;

namespace RnzTrauer.Core.Tests;

[TestClass]
public sealed class NoticeUpdateServiceTests
{
    [TestMethod]
    public async Task UpdateAsyncDelegatesValidNotice()
    {
        var notice = new DeathNotice { Id = 7, OrderNumber = "A-7", Category = AdvertisementCategory.DeathNotice };
        var repository = new FakeNoticeRepository();

        var result = await new NoticeUpdateService(repository).UpdateAsync(new NoticeUpdateRequest(notice));

        Assert.AreEqual(NoticeUpdateStatus.Succeeded, result.Status);
        Assert.AreSame(notice, repository.SavedNotice);
    }

    [TestMethod]
    [DataRow(0, "notice.id_required")]
    [DataRow(7, "notice.order_required")]
    public async Task UpdateAsyncRejectsInvalidNotice(long id, string errorCode)
    {
        var notice = new DeathNotice { Id = id, OrderNumber = id == 0 ? "A-7" : string.Empty };
        var repository = new FakeNoticeRepository();

        var result = await new NoticeUpdateService(repository).UpdateAsync(new NoticeUpdateRequest(notice));

        Assert.AreEqual(NoticeUpdateStatus.InvalidRequest, result.Status);
        Assert.AreEqual(errorCode, result.ErrorCode);
        Assert.IsNull(repository.SavedNotice);
    }

    [TestMethod]
    public async Task UpdateAsyncReturnsRepositoryFailureAndPropagatesCancellation()
    {
        var repository = new FakeNoticeRepository { Exception = new InvalidOperationException("conflict") };
        var notice = new DeathNotice
        {
            Id = 7,
            OrderNumber = "A-7",
            Category = AdvertisementCategory.DeathNotice,
        };

        var result = await new NoticeUpdateService(repository).UpdateAsync(new NoticeUpdateRequest(notice));
        Assert.AreEqual(NoticeUpdateStatus.Failed, result.Status);
        Assert.AreEqual("notice.update_failed", result.ErrorCode);

        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();
        try
        {
            await new NoticeUpdateService(new FakeNoticeRepository())
                .UpdateAsync(new NoticeUpdateRequest(notice), cancellation.Token);
            Assert.Fail("Cancellation should be propagated.");
        }
        catch (OperationCanceledException)
        {
            // Expected.
        }
    }

    private sealed class FakeNoticeRepository : INoticeRepository
    {
        public DeathNotice? SavedNotice { get; private set; }
        public Exception? Exception { get; init; }

        public Task<IReadOnlyList<DeathNotice>> FindAsync(NoticeFilter filter, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<DeathNotice>>(Array.Empty<DeathNotice>());

        public Task SaveAsync(DeathNotice notice, CancellationToken cancellationToken = default)
        {
            if (Exception is not null)
                throw Exception;
            SavedNotice = notice;
            return Task.CompletedTask;
        }

        public Task<bool> UpsertImportedAsync(DeathNotice notice, CancellationToken cancellationToken = default) => Task.FromResult(true);
        public Task<IReadOnlyList<string>> GetPlaceNamesAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<string>>(Array.Empty<string>());
        public Task<IReadOnlyList<DeathNotice>> GetLinkCandidatesAsync(long noticeId, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<DeathNotice>>(Array.Empty<DeathNotice>());
    }
}
