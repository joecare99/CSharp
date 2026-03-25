namespace Avln_CommonDialogs.Base.Interfaces;

public interface IColorDialog
{
    bool AllowAlpha { get; set; }
    object? Color { get; set; }

    ValueTask<bool?> ShowAsync(object owner);
    ValueTask<bool?> ShowAsync();
}
