using System;

namespace Osb.ModernHost.Models;

public sealed class PageNavigationRequestedEventArgs : EventArgs
{
    public PageNavigationRequestedEventArgs(ModernHostPage page)
    {
        Page = page;
    }

    public ModernHostPage Page { get; }
}
