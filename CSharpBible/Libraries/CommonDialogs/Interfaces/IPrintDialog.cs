using System.Printing;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace CommonDialogs.Interfaces
{
    public interface IPrintDialog
    {
       
        bool CurrentPageEnabled { get; set; }
        //
        // Zusammenfassung:
        //     Gets or sets the highest page number that is allowed in page ranges.
        //
        // Rückgabewerte:
        //     A System.UInt32 that represents the highest page number that can be used in a
        //     page range in the Print dialog box.
        //
        // Ausnahmen:
        //   T:System.ArgumentException:
        //     The property is being set to less than 1.
        uint MaxPage { get; set; }
        //
        // Zusammenfassung:
        //     Gets or sets the lowest page number that is allowed in page ranges.
        //
        // Rückgabewerte:
        //     A System.UInt32 that represents the lowest page number that can be used in a
        //     page range in the Print dialog box.
        //
        // Ausnahmen:
        //   T:System.ArgumentException:
        //     The property is being set to less than 1.
        uint MinPage { get; set; }
        //
        // Zusammenfassung:
        //     Gets or sets the range of pages to print when System.Windows.Controls.PrintDialog.PageRangeSelection
        //     is set to System.Windows.Controls.PageRangeSelection.UserPages.
        //
        // Rückgabewerte:
        //     A System.Windows.Controls.PageRange that represents the range of pages that are
        //     printed.
        //
        // Ausnahmen:
        //   T:System.ArgumentException:
        //     The System.Windows.Controls.PageRange object that is being used to set the property
        //     has either the beginning of the range or the end of the range set to a value
        //     that is less than 1.
        PageRange PageRange { get; set; }
        //
        // Zusammenfassung:
        //     Gets or sets the System.Windows.Controls.PageRangeSelection for this instance
        //     of System.Windows.Controls.PrintDialog.
        //
        // Rückgabewerte:
        //     The System.Windows.Controls.PageRangeSelection value that represents the type
        //     of page range to print.
        PageRangeSelection PageRangeSelection { get; set; }
        //
        // Zusammenfassung:
        //     Gets the height of the printable area of the page.
        //
        // Rückgabewerte:
        //     A System.Double that represents the height of the printable page area.
        double PrintableAreaHeight { get; }
        //
        // Zusammenfassung:
        //     Gets the width of the printable area of the page.
        //
        // Rückgabewerte:
        //     A System.Double that represents the width of the printable page area.
         double PrintableAreaWidth { get; }
        //
        // Zusammenfassung:
        //     Gets or sets a System.Printing.PrintQueue that represents the printer that is
        //     selected.
        //
        // Rückgabewerte:
        //     The System.Printing.PrintQueue that the user selected.
        PrintQueue PrintQueue { get; set; }
        //
        // Zusammenfassung:
        //     Gets or sets the System.Printing.PrintTicket that is used by the System.Windows.Controls.PrintDialog
        //     when the user clicks Print for the current print job.
        //
        // Rückgabewerte:
        //     A System.Printing.PrintTicket that is used the next time the Print button in
        //     the dialog box is clicked. Setting this System.Windows.Controls.PrintDialog.PrintTicket
        //     property does not validate or modify the specified System.Printing.PrintTicket
        //     for a particular System.Printing.PrintQueue. If needed, use the System.Printing.PrintQueue.MergeAndValidatePrintTicket(System.Printing.PrintTicket,System.Printing.PrintTicket)
        //     method to create a System.Printing.PrintQueue-specific System.Printing.PrintTicket
        //     that is valid for a given printer.
        PrintTicket PrintTicket { get; set; }
        //
        // Zusammenfassung:
        //     Gets or sets a value that indicates whether the option to print the selected
        //     pages is enabled.
        //
        // Rückgabewerte:
        //     true if the option to print the selected pages is enabled; otherwise, false.
        bool SelectedPagesEnabled { get; set; }
        //
        // Zusammenfassung:
        //     Gets or sets a value that indicates whether users of the Print dialog box have
        //     the option to specify ranges of pages to print.
        //
        // Rückgabewerte:
        //     true if the option is available; otherwise, false.
        bool UserPageRangeEnabled { get; set; }

        //
        // Zusammenfassung:
        //     Prints a System.Windows.Documents.DocumentPaginator object to the System.Printing.PrintQueue
        //     that is currently selected.
        //
        // Parameter:
        //   documentPaginator:
        //     The System.Windows.Documents.DocumentPaginator object to print.
        //
        //   description:
        //     A description of the job that is to be printed. This text appears in the user
        //     interface (UI) of the printer.
        //
        // Ausnahmen:
        //   T:System.ArgumentNullException:
        //     documentPaginator is null.
        void PrintDocument(DocumentPaginator documentPaginator, string description);
        //
        // Zusammenfassung:
        //     Prints a visual (non-text) object, which is derived from the System.Windows.Media.Visual
        //     class, to the System.Printing.PrintQueue that is currently selected.
        //
        // Parameter:
        //   visual:
        //     The System.Windows.Media.Visual to print.
        //
        //   description:
        //     A description of the job that is to be printed. This text appears in the user
        //     interface (UI) of the printer.
        //
        // Ausnahmen:
        //   T:System.ArgumentNullException:
        //     visual is null.
        void PrintVisual(Visual visual, string description);
        //
        // Zusammenfassung:
        //     Invokes the System.Windows.Controls.PrintDialog as a modal dialog box.
        //
        // Rückgabewerte:
        //     true if a user clicks Print; false if a user clicks Cancel; or null if a user
        //     closes the dialog box without clicking Print or Cancel.
        public bool? ShowDialog();
    }
}