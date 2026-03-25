namespace Avln_CommonDialogs.Base.Interfaces;

public interface ISaveFileDialog : IFileDialog
{
    string? InitialFileName { get; set; }
    bool OverwritePrompt { get; set; }

    IList<FileTypeFilter> MutableFilters { get; }

    ValueTask<string?> ShowAsync(object owner);
    ValueTask<string?> ShowAsync();
}
