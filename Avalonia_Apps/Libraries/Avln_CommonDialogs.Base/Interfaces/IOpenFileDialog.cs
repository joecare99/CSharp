namespace Avln_CommonDialogs.Base.Interfaces;

public interface IOpenFileDialog : IFileDialog
{
    bool AllowMultiple { get; set; }

    IList<FileTypeFilter> MutableFilters { get; }

    ValueTask<IReadOnlyList<string>> ShowAsync(object owner);
    ValueTask<IReadOnlyList<string>> ShowAsync();
}
