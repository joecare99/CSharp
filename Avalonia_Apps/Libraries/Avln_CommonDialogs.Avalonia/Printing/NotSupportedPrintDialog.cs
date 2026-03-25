using Avln_CommonDialogs.Base.Interfaces;

namespace Avln_CommonDialogs.Avalonia.Printing;

public sealed class NotSupportedPrintDialog : IPrintDialog
{
    public string? Title { get; set; }

    public uint MinPage { get; set; } = 1;
    public uint MaxPage { get; set; } = 1;

    public bool AllowCurrentPage { get; set; }
    public bool AllowSelectedPages { get; set; }
    public bool AllowPageRange { get; set; } = true;

    public PrintPageRange? PageRange { get; set; }

    public ValueTask<IPrintSession?> ShowAsync(object owner)
        => new((IPrintSession?)null);

    public ValueTask<IPrintSession?> ShowAsync()
        => new((IPrintSession?)null);
}
