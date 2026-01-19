namespace Avln_CommonDialogs.Base.Interfaces;

public interface IPrintDialog
{
    string? Title { get; set; }

    uint MinPage { get; set; }
    uint MaxPage { get; set; }

    bool AllowCurrentPage { get; set; }
    bool AllowSelectedPages { get; set; }
    bool AllowPageRange { get; set; }

    PrintPageRange? PageRange { get; set; }

    ValueTask<IPrintSession?> ShowAsync(object owner);
    ValueTask<IPrintSession?> ShowAsync();
}
