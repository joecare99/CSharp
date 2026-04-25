using CommunityToolkit.Mvvm.ComponentModel;
using MSQBrowser.ViewModels.Interfaces;
using MSQBrowser.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using CommunityToolkit.Mvvm.Input;
using System.Threading;
using System.Threading.Tasks;

namespace MSQBrowser.ViewModels
{
    public partial class TableViewViewModel : ObservableValidator, ITableViewViewModel
    {
        [ObservableProperty]
        string _tableName = "<TableName>";
        [ObservableProperty]
        ObservableCollection<DBMetaData> _columns = new() {
            new(){ Name = "Column1", Kind = EKind.Column, Data = new List<string>() { "1", "2", "3" } },
            new(){ Name = "Column2", Kind = EKind.Column, Data = new List<string>() { "1", "2", "3" } },
            new(){ Name = "Column3", Kind = EKind.Column, Data = new List<string>() { "1", "2", "3" } },
        };

        [ObservableProperty]
        DataTable _tableData = new()
        {
            Columns = { "Column1", "Column2", "Column3" },
            Rows = { { "1", "2", "3" }, { "1", "2", "3" }, { "1", "2", "3" } }
        };

        private int _pageOffset;
        private bool _hasNextPage;
        private bool _isLoading;
        private string _pageStatus = "";

        public int PageSize { get; } = 100;
        public int PageOffset
        {
            get => _pageOffset;
            private set => SetProperty(ref _pageOffset, value);
        }

        public bool HasNextPage
        {
            get => _hasNextPage;
            private set => SetProperty(ref _hasNextPage, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            private set => SetProperty(ref _isLoading, value);
        }

        public string PageStatus
        {
            get => _pageStatus;
            private set => SetProperty(ref _pageStatus, value);
        }

        public bool CanLoadPreviousPage => !IsLoading && PageOffset > 0;
        public bool CanLoadNextPage => !IsLoading && HasNextPage;
        public bool CanRefreshPage => !IsLoading && !string.IsNullOrWhiteSpace(TableName);
        public IAsyncRelayCommand LoadPreviousPageCommand { get; }
        public IAsyncRelayCommand LoadNextPageCommand { get; }
        public IAsyncRelayCommand RefreshPageCommand { get; }

        private IDBViewViewModel? _parent;
        private readonly List<DBMetaData> _columnDefinitions = new();
        private CancellationTokenSource? _loadCancellationTokenSource;
        private const int AutoLoadThreshold = 10;

        public TableViewViewModel() : this(DBViewViewModel.This ?? new DBViewViewModel())
        {
        }
        public TableViewViewModel(IDBViewViewModel parent)
        {
            _parent = parent;
            LoadPreviousPageCommand = new AsyncRelayCommand(() => LoadPageAsync(Math.Max(PageOffset - PageSize, 0)), () => CanLoadPreviousPage);
            LoadNextPageCommand = new AsyncRelayCommand(LoadMoreRowsAsync, () => CanLoadNextPage);
            RefreshPageCommand = new AsyncRelayCommand(() => LoadPageAsync(0), () => CanRefreshPage);

            if (_parent != null)
            {
                TableName = GetTableName();
                _parent.PropertyChanged += (sender, e) => TableName = GetTableName(() => e.PropertyName == "SelectedEntry");
            }

            string GetTableName(Func<bool>? predicate = null)
            {
                if ((predicate?.Invoke() ?? true)
                    && _parent.SelectedEntry is DBMetaData md
                    && md.Kind == EKind.Table)
                {
                    return md.Name;
                }
                else
                    return "";
            }
        }

        partial void OnTableNameChanged(string value)
        {
            Columns.Clear();
            _columnDefinitions.Clear();
            CancelPendingLoad();
            HasNextPage = false;
            PageOffset = 0;

            if (value != "")
            {
                foreach (var d in _parent?.dBModel?.QueryTable(value) ?? new List<DBMetaData>())
                {
                    Columns.Add(d);
                    _columnDefinitions.Add(d);
                }

                _ = LoadPageAsync(0);
            }
            else
            {
                TableData = new DataTable();
                PageStatus = "";
            }

            NotifyPagingCommandsChanged();
        }

        public Task EnsureVisibleRowsLoadedAsync(int visibleRowIndex)
        {
            if (visibleRowIndex < 0
                || IsLoading
                || !HasNextPage
                || visibleRowIndex < TableData.Rows.Count - AutoLoadThreshold)
            {
                return Task.CompletedTask;
            }

            return LoadMoreRowsAsync();
        }

        public Task LoadMoreRowsAsync()
        {
            return HasNextPage && !IsLoading
                ? LoadPageAsync(TableData.Rows.Count, true)
                : Task.CompletedTask;
        }

        private async Task LoadPageAsync(int offset, bool append = false)
        {
            if (string.IsNullOrWhiteSpace(TableName) || _parent?.dBModel == null)
            {
                TableData = new DataTable();
                PageStatus = "";
                return;
            }

            CancelPendingLoad();
            var cancellationTokenSource = new CancellationTokenSource();
            _loadCancellationTokenSource = cancellationTokenSource;

            IsLoading = true;
            PageStatus = append
                ? $"Lade weitere Zeilen ab {offset + 1} …"
                : $"Lade Zeilen {offset + 1}-{offset + PageSize} …";
            NotifyPagingCommandsChanged();

            try
            {
                var page = await _parent.dBModel.QueryTableDataPageAsync(TableName, _columnDefinitions, offset, PageSize, cancellationTokenSource.Token);
                if (_loadCancellationTokenSource != cancellationTokenSource)
                {
                    return;
                }

                if (append && TableData.Columns.Count > 0)
                {
                    AppendRows(page.Data);
                }
                else
                {
                    TableData = page.Data;
                    PageOffset = page.Offset;
                }

                HasNextPage = page.HasMoreRows;
                PageStatus = BuildPageStatus(page.Offset, page.Data.Rows.Count, page.HasMoreRows, append);
            }
            catch (OperationCanceledException)
            {
            }
            catch
            {
                TableData = new DataTable();
                HasNextPage = false;
                PageStatus = "Fehler beim Laden der Daten";
            }
            finally
            {
                if (_loadCancellationTokenSource == cancellationTokenSource)
                {
                    _loadCancellationTokenSource = null;
                }

                IsLoading = false;
                NotifyPagingCommandsChanged();
                cancellationTokenSource.Dispose();
            }
        }

        private string BuildPageStatus(int offset, int rowCount, bool hasMoreRows, bool append)
        {
            if (rowCount <= 0)
            {
                return offset == 0 ? "Keine Zeilen gefunden" : "Keine weiteren Zeilen gefunden";
            }

            if (append)
            {
                var loaded = TableData.Rows.Count;
                return hasMoreRows ? $"{loaded} Zeilen geladen, weitere folgen beim Scrollen" : $"{loaded} Zeilen geladen (Ende erreicht)";
            }

            var from = offset + 1;
            var to = offset + rowCount;
            return hasMoreRows ? $"Zeilen {from}-{to} geladen, weitere folgen beim Scrollen" : $"Zeilen {from}-{to} geladen (Ende erreicht)";
        }

        private void AppendRows(DataTable source)
        {
            foreach (DataRow row in source.Rows)
            {
                var targetRow = TableData.NewRow();
                targetRow.ItemArray = (object[])row.ItemArray.Clone();
                TableData.Rows.Add(targetRow);
            }
        }

        private void CancelPendingLoad()
        {
            _loadCancellationTokenSource?.Cancel();
            _loadCancellationTokenSource?.Dispose();
            _loadCancellationTokenSource = null;
        }

        private void NotifyPagingCommandsChanged()
        {
            OnPropertyChanged(nameof(CanLoadPreviousPage));
            OnPropertyChanged(nameof(CanLoadNextPage));
            OnPropertyChanged(nameof(CanRefreshPage));
            LoadPreviousPageCommand.NotifyCanExecuteChanged();
            LoadNextPageCommand.NotifyCanExecuteChanged();
            RefreshPageCommand.NotifyCanExecuteChanged();
        }

    }
}
