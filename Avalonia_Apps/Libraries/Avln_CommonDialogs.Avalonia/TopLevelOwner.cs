using Avalonia.Controls;

namespace Avln_CommonDialogs.Avalonia;

public static class TopLevelOwner
{
    public static TopLevel? From(object? owner)
    {
        if (owner is null)
            return null;

        if (owner is TopLevel tl)
            return tl;

        if (owner is Control c)
            return TopLevel.GetTopLevel(c);

        return null;
    }
}
