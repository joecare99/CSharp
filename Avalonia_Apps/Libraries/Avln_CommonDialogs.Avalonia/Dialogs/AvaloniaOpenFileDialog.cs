using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Avln_CommonDialogs.Base.Interfaces;

namespace Avln_CommonDialogs.Avalonia.Dialogs;

public sealed class AvaloniaOpenFileDialog : IOpenFileDialog
{
    private readonly Func<TopLevel?> _topLevelProvider;

    public AvaloniaOpenFileDialog(Func<TopLevel?> topLevelProvider)
    {
        _topLevelProvider = topLevelProvider;
    }

    public string? Title { get; set; }
    public string? InitialDirectory { get; set; }
    public bool RestoreDirectory { get; set; }
    public bool AddExtension { get; set; }
    public bool CheckFileExists { get; set; }
    public string? DefaultExtension { get; set; }

    public bool AllowMultiple { get; set; }

    public IList<FileTypeFilter> MutableFilters { get; } = new List<FileTypeFilter>();
    public IReadOnlyList<FileTypeFilter> Filters => MutableFilters;

    public ValueTask<IReadOnlyList<string>> ShowAsync(object owner)
        => ShowInternalAsync(TopLevelOwner.From(owner));

    public ValueTask<IReadOnlyList<string>> ShowAsync()
        => ShowInternalAsync(_topLevelProvider());

    private async ValueTask<IReadOnlyList<string>> ShowInternalAsync(TopLevel? topLevel)
    {
        if (topLevel?.StorageProvider is not { } sp)
            return Array.Empty<string>();

        var options = new FilePickerOpenOptions
        {
            Title = Title,
            AllowMultiple = AllowMultiple,
            FileTypeFilter = ConvertFilters(MutableFilters)
        };

        if (!string.IsNullOrWhiteSpace(InitialDirectory))
        {
            try
            {
                options.SuggestedStartLocation = await sp.TryGetFolderFromPathAsync(InitialDirectory).ConfigureAwait(false);
            }
            catch
            {
                // ignore invalid paths
            }
        }

        var files = await sp.OpenFilePickerAsync(options).ConfigureAwait(false);
        return files.Select(f => f.Path.LocalPath).ToArray();
    }

    private static IReadOnlyList<FilePickerFileType>? ConvertFilters(IEnumerable<FileTypeFilter> filters)
    {
        var list = new List<FilePickerFileType>();

        foreach (var f in filters)
        {
            var patterns = f.Patterns
                .Where(p => !string.IsNullOrWhiteSpace(p))
                .Select(NormalizePattern)
                .ToArray();

            list.Add(new FilePickerFileType(f.Name)
            {
                Patterns = patterns.Length == 0 ? null : patterns
            });
        }

        return list.Count == 0 ? null : list;
    }

    private static string NormalizePattern(string pattern)
    {
        pattern = pattern.Trim();
        if (pattern.StartsWith("*.", StringComparison.Ordinal) || pattern.StartsWith("*", StringComparison.Ordinal))
            return pattern;

        if (pattern.StartsWith(".", StringComparison.Ordinal))
            return "*" + pattern;

        return "*." + pattern;
    }
}
