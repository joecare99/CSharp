using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace GenFreeBrowser.Places;

public interface ISearchHistoryService
{
    IReadOnlyList<string> Items { get; }
    void Register(string query);
    Task LoadAsync();
}

/// <summary>
/// Simple persistent MRU (most recently used) query history stored as JSON in AppData.
/// Thread-safe for simple use via lock.
/// </summary>
public sealed class SearchHistoryService : ISearchHistoryService
{
    private readonly object _gate = new();
    private readonly List<string> _items = new();
    private readonly int _maxItems;
    private readonly string _filePath;

    public IReadOnlyList<string> Items
    {
        get
        {
            lock (_gate) return _items.AsReadOnly();
        }
    }

    public SearchHistoryService(string? appFolder = null, string fileName = "searchhistory.json", int maxItems = 25)
    {
        _maxItems = maxItems <= 0 ? 25 : maxItems;
        var root = appFolder ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "GenFreeBrowser");
        Directory.CreateDirectory(root);
        _filePath = Path.Combine(root, fileName);
    }

    public async Task LoadAsync()
    {
        try
        {
            if (File.Exists(_filePath))
            {
                await using var fs = File.OpenRead(_filePath);
                var data = await JsonSerializer.DeserializeAsync<List<string>>(fs) ?? new();
                lock (_gate)
                {
                    _items.Clear();
                    foreach (var s in data)
                    {
                        if (!string.IsNullOrWhiteSpace(s)) _items.Add(s.Trim());
                    }
                }
            }
        }
        catch
        {
            // ignore (corrupt file etc.)
        }
    }

    public void Register(string query)
    {
        if (string.IsNullOrWhiteSpace(query)) return;
        var q = query.Trim();
        lock (_gate)
        {
            _items.RemoveAll(s => s.Equals(q, StringComparison.OrdinalIgnoreCase));
            _items.Insert(0, q);
            while (_items.Count > _maxItems) _items.RemoveAt(_items.Count - 1);
            PersistUnsafe();
        }
    }

    private void PersistUnsafe()
    {
        try
        {
            var json = JsonSerializer.Serialize(_items, new JsonSerializerOptions { WriteIndented = false });
            File.WriteAllText(_filePath, json);
        }
        catch
        {
            // ignore write errors
        }
    }
}
