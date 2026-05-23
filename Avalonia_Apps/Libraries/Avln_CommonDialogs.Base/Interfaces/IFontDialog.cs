using Avln_CommonDialogs.Base.Models;

namespace Avln_CommonDialogs.Base.Interfaces;

public interface IFontDialog
{
    object? Font { get; set; }

    object? Selection { get; set; }

    string? PreviewText { get; set; }

    FontDialogPresentationMode PresentationMode { get; set; }

    ValueTask<bool?> ShowAsync(object owner);
    ValueTask<bool?> ShowAsync();
}
