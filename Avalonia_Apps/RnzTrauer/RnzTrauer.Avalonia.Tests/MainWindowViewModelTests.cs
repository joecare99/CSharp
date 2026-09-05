using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RnzTrauer.Avalonia.ViewModels;
using RnzTrauer.Core.Domain;
using RnzTrauer.Core.Services;
using RnzTrauer.Places;

namespace RnzTrauer.Avalonia.Tests;

[TestClass]
public sealed class MainWindowViewModelTests
{
    [TestMethod]
    public void SelectingNoticeSynchronizesCoordinatePlace()
    {
        var store = new FakeCoordinateStore();
        var viewModel = CreateViewModel(store, CoordinateSchemaStatus.Available);

        viewModel.SelectedNotice = NoticeProjection.FromDomain(
            new DeathNotice { Place = "Heidelberg" });
        viewModel.CoordinateLatitude = "49.3988";
        viewModel.CoordinateLongitude = "8.6724";
        viewModel.CoordinateSource = "fixture";
        viewModel.CoordinateIsApproximate = true;
        viewModel.SelectedNotice = NoticeProjection.FromDomain(
            new DeathNotice { Place = "Mannheim" });

        Assert.AreEqual("Mannheim", viewModel.CoordinatePlace);
        Assert.AreEqual(string.Empty, viewModel.CoordinateLatitude);
        Assert.AreEqual(string.Empty, viewModel.CoordinateLongitude);
        Assert.AreEqual(string.Empty, viewModel.CoordinateSource);
        Assert.IsFalse(viewModel.CoordinateIsApproximate);
        Assert.AreEqual("coordinate.place_selected", viewModel.CoordinateSchemaDiagnosticCode);
    }

    [TestMethod]
    public async Task SavingIsRejectedWhenSchemaIsUnavailable()
    {
        var store = new FakeCoordinateStore();
        var viewModel = CreateViewModel(store, CoordinateSchemaStatus.Missing);
        viewModel.CoordinatePlace = "Heidelberg";
        viewModel.CoordinateLatitude = "49.3988";
        viewModel.CoordinateLongitude = "8.6724";

        await viewModel.SaveCoordinateCommand.ExecuteAsync(null);

        Assert.AreEqual("coordinate.persistence_unavailable", viewModel.CoordinateSchemaDiagnosticCode);
        Assert.IsFalse(store.HasSavedCoordinate);
    }

    [TestMethod]
    public async Task InvalidCoordinateInputIsRejected()
    {
        var store = new FakeCoordinateStore();
        var viewModel = CreateViewModel(store, CoordinateSchemaStatus.Available);
        viewModel.CoordinatePlace = "Heidelberg";
        viewModel.CoordinateLatitude = "not-a-number";
        viewModel.CoordinateLongitude = "8.6724";

        await viewModel.SaveCoordinateCommand.ExecuteAsync(null);

        Assert.AreEqual("coordinate.invalid_input", viewModel.CoordinateSchemaDiagnosticCode);
        Assert.IsFalse(store.HasSavedCoordinate);
    }

    [TestMethod]
    public async Task SavingValidCoordinateUsesStoreWhenSchemaIsAvailable()
    {
        var store = new FakeCoordinateStore();
        var viewModel = CreateViewModel(store, CoordinateSchemaStatus.Available);
        viewModel.CoordinatePlace = "Heidelberg";
        viewModel.CoordinateLatitude = "49.3988";
        viewModel.CoordinateLongitude = "8.6724";
        viewModel.CoordinateSource = "fixture";
        viewModel.CoordinateIsApproximate = true;

        await viewModel.SaveCoordinateCommand.ExecuteAsync(null);

        Assert.IsTrue(store.HasSavedCoordinate);
        Assert.AreEqual("coordinate.saved_partial_metadata", viewModel.CoordinateSchemaDiagnosticCode);
    }

    [TestMethod]
    public async Task LoadingStoredCoordinatePopulatesEditor()
    {
        var store = new FakeCoordinateStore
        {
            Coordinate = new PlaceCoordinate("Heidelberg", 49.3988, 8.6724, "fixture", true),
        };
        var viewModel = CreateViewModel(store, CoordinateSchemaStatus.Available);
        viewModel.CoordinatePlace = "Heidelberg";

        await viewModel.LoadCoordinateCommand.ExecuteAsync(null);

        Assert.AreEqual("49.3988", viewModel.CoordinateLatitude);
        Assert.AreEqual("8.6724", viewModel.CoordinateLongitude);
        Assert.AreEqual("fixture", viewModel.CoordinateSource);
        Assert.IsTrue(viewModel.CoordinateIsApproximate);
        Assert.AreEqual("coordinate.loaded", viewModel.CoordinateSchemaDiagnosticCode);
    }

    [TestMethod]
    public async Task ReadOnlyModeDisablesAllWriteCommands()
    {
        var store = new FakeCoordinateStore();
        var viewModel = CreateViewModel(store, CoordinateSchemaStatus.Available, readOnly: true);
        viewModel.SelectedNotice = NoticeProjection.FromDomain(
            new DeathNotice { Text = "legacy text" });
        viewModel.CoordinatePlace = "Heidelberg";
        viewModel.CoordinateLatitude = "49.3988";
        viewModel.CoordinateLongitude = "8.6724";

        Assert.IsFalse(viewModel.SaveCommand.CanExecute(null));
        Assert.IsFalse(viewModel.ParseSelectedCommand.CanExecute(null));
        await viewModel.SaveCoordinateCommand.ExecuteAsync(null);

        Assert.IsFalse(store.HasSavedCoordinate);
    }

    [TestMethod]
    public async Task LoadingSelectedDetailUsesImmutableProjection()
    {
        var store = new FakeCoordinateStore();
        var detailService = new FakeDetailService();
        var viewModel = CreateViewModel(
            store, CoordinateSchemaStatus.Available, detailService: detailService);
        viewModel.SelectedNotice = NoticeProjection.FromDomain(
            new DeathNotice { Id = 42, Place = "Heidelberg" });

        await viewModel.LoadSelectedDetailCommand.ExecuteAsync(null);

        Assert.AreEqual(42, detailService.RequestedNoticeId);
        Assert.AreEqual("Heidelberg", viewModel.SelectedNoticePlace);
        Assert.AreEqual("PNG available", viewModel.SelectedNoticeMediaSummary);
        Assert.AreEqual(1, viewModel.LinkCandidates.Count);
    }

    private static MainWindowViewModel CreateViewModel(
        FakeCoordinateStore store,
        CoordinateSchemaStatus status,
        bool readOnly = false,
        INoticeDetailService? detailService = null)
    {
        var repository = new FakeNoticeRepository();
        var viewModel = new MainWindowViewModel(
            repository,
            new NoticeSearchService(repository),
            new FakeNoticeTextParser(),
            new FakeExportService(),
            new FakeSchemaProbe(status),
            store,
            detailService: detailService,
            readOnly: readOnly);
        viewModel.CoordinatePersistenceAvailable = status == CoordinateSchemaStatus.Available;
        return viewModel;
    }

    private sealed class FakeCoordinateStore : IPlaceCoordinateStore
    {
        public PlaceCoordinate? Coordinate { get; init; }
        public bool HasSavedCoordinate { get; private set; }

        public Task<PlaceCoordinate?> GetAsync(string place, CancellationToken cancellationToken = default) =>
            Task.FromResult(Coordinate?.Place == place ? Coordinate : null);

        public Task SaveAsync(PlaceCoordinate coordinate, CancellationToken cancellationToken = default)
        {
            HasSavedCoordinate = true;
            return Task.CompletedTask;
        }
    }

    private sealed class FakeSchemaProbe : ICoordinateSchemaProbe
    {
        private readonly CoordinateSchemaStatus _status;

        public FakeSchemaProbe(CoordinateSchemaStatus status) => _status = status;

        public Task<CoordinateSchemaReport> ProbeAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult(CoordinateSchemaReport.Create(_status));
    }

    private sealed class FakeNoticeRepository : INoticeRepository
    {
        public Task<IReadOnlyList<DeathNotice>> FindAsync(NoticeFilter filter, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<DeathNotice>>(new List<DeathNotice>());

        public Task SaveAsync(DeathNotice notice, CancellationToken cancellationToken = default) =>
            Task.CompletedTask;

        public Task<bool> UpsertImportedAsync(DeathNotice notice, CancellationToken cancellationToken = default) =>
            Task.FromResult(true);

        public Task<IReadOnlyList<string>> GetPlaceNamesAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<string>>(new List<string>());

        public Task<IReadOnlyList<DeathNotice>> GetLinkCandidatesAsync(long noticeId, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<DeathNotice>>(new List<DeathNotice>());
    }

    private sealed class FakeDetailService : INoticeDetailService
    {
        public long RequestedNoticeId { get; private set; }

        public Task<NoticeDetailResult> LoadAsync(
            NoticeDetailRequest request,
            CancellationToken cancellationToken = default)
        {
            RequestedNoticeId = request.Notice.Id;
            var candidate = NoticeProjection.FromDomain(new DeathNotice { Id = 43 });
            return Task.FromResult(new NoticeDetailResult(
                NoticeProjection.FromDomain(request.Notice),
                new[] { candidate },
                request.Notice.Place,
                new NoticeMediaState(false, true, false)));
        }
    }

    private sealed class FakeNoticeTextParser : INoticeTextParser
    {
        public ParsedNoticeFacts Parse(DeathNotice notice, string text, IReadOnlyCollection<string> placeNames) =>
            new(null, null, null, null, null, null, null);
    }

    private sealed class FakeExportService : IExportService
    {
        public Task ExportCsvAsync(string fileName, IReadOnlyCollection<DeathNotice> notices, CancellationToken cancellationToken = default) =>
            Task.CompletedTask;

        public Task ExportGedcomAsync(string fileName, IReadOnlyCollection<DeathNotice> notices, CancellationToken cancellationToken = default) =>
            Task.CompletedTask;
    }
}
