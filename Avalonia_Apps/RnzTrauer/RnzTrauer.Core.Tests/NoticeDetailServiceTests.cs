using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RnzTrauer.Core.Domain;
using RnzTrauer.Core.Services;

namespace RnzTrauer.Core.Tests;

[TestClass]
public sealed class NoticeDetailServiceTests
{
    [TestMethod]
    public async Task LoadAsyncBuildsPlaceMediaAndLinkProjection()
    {
        var notice = new DeathNotice
        {
            Id = 42,
            Place = "Heidelberg",
            PdfFile = "notice.pdf",
            ProfileImage = "profile.png",
        };
        var candidate = new DeathNotice { Id = 43 };
        var repository = new FakeNoticeRepository { LinkCandidates = new[] { candidate } };

        var result = await new NoticeDetailService(repository).LoadAsync(
            new NoticeDetailRequest(NoticeProjection.FromDomain(notice)));

        Assert.AreEqual("Heidelberg", result.PlaceName);
        Assert.AreEqual(candidate.Id, result.LinkCandidates[0].Id);
        Assert.IsFalse(ReferenceEquals(candidate, result.LinkCandidates[0]));
        Assert.IsTrue(result.Media.HasPdf);
        Assert.IsFalse(result.Media.HasPng);
        Assert.IsTrue(result.Media.HasProfileImage);
        Assert.AreEqual("Profile image available", result.Media.Summary);
        Assert.AreEqual(42, repository.RequestedNoticeId);
    }

    [TestMethod]
    public async Task LoadAsyncDoesNotQueryCandidatesForUnsavedNotice()
    {
        var result = await new NoticeDetailService(new FakeNoticeRepository()).LoadAsync(
            new NoticeDetailRequest(
                NoticeProjection.FromDomain(new DeathNotice { Place = "Mannheim" })));

        Assert.AreEqual(0, result.LinkCandidates.Count);
    }

    [TestMethod]
    public async Task LoadAsyncReturnsTheImmutableRequestProjectionWithoutAliasingDomainObjects()
    {
        var domainNotice = new DeathNotice
        {
            Id = 7,
            Place = "Heidelberg",
            PdfFile = "notice.pdf",
        };
        var projection = NoticeProjection.FromDomain(domainNotice);
        var result = await new NoticeDetailService(new FakeNoticeRepository()).LoadAsync(
            new NoticeDetailRequest(projection));

        Assert.AreSame(projection, result.Notice);
        Assert.AreEqual("Heidelberg", result.PlaceName);
        Assert.AreEqual("notice.pdf", result.Notice.PdfFile);
        Assert.IsFalse(ReferenceEquals(domainNotice, result.Notice));
    }

    [TestMethod]
    public async Task LoadAsyncPropagatesCancellationBeforeRepositoryAccess()
    {
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();

        try
        {
            await new NoticeDetailService(new FakeNoticeRepository()).LoadAsync(
                new NoticeDetailRequest(
                    NoticeProjection.FromDomain(new DeathNotice { Id = 42 })),
                cancellation.Token);
            Assert.Fail("Expected cancellation.");
        }
        catch (OperationCanceledException)
        {
        }
    }

    private sealed class FakeNoticeRepository : INoticeRepository
    {
        public IReadOnlyList<DeathNotice> LinkCandidates { get; init; } = Array.Empty<DeathNotice>();
        public long RequestedNoticeId { get; private set; }

        public Task<IReadOnlyList<DeathNotice>> FindAsync(NoticeFilter filter, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<DeathNotice>>(Array.Empty<DeathNotice>());

        public Task SaveAsync(DeathNotice notice, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<bool> UpsertImportedAsync(DeathNotice notice, CancellationToken cancellationToken = default) => Task.FromResult(true);
        public Task<IReadOnlyList<string>> GetPlaceNamesAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<string>>(Array.Empty<string>());

        public Task<IReadOnlyList<DeathNotice>> GetLinkCandidatesAsync(
            long noticeId,
            CancellationToken cancellationToken = default)
        {
            RequestedNoticeId = noticeId;
            return Task.FromResult(LinkCandidates);
        }
    }
}
