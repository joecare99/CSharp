namespace Avln_CommonDialogs.Base.Interfaces;

public interface IFontDialog
{
    object? Font { get; set; }

    ValueTask<bool?> ShowAsync(object owner);
    ValueTask<bool?> ShowAsync();
}
