using System;
using System.Linq;
using System.Windows.Media;
using System.Windows;
using System.Globalization;


#if !NET5_0_OR_GREATER
using System.Drawing.Printing;
using System.Drawing;
#else
using System.Printing;
using System.Windows.Xps;
#endif
namespace TestStatements.SystemNS.Printing;

public static class Printing_Ex
{
    public static void PrintDocument()
    {
#if !NET5_0_OR_GREATER
        PrintDocument pd = new PrintDocument();
        pd.DocumentName = "Hello, Printer! Document";
        pd.PrinterSettings.PrinterName = "Microsoft Print to PDF";
        pd.PrintPage += (sender, e) =>
        {
            
            e.Graphics.DrawString("Hello, Printer!", new Font("Arial", 12), Brushes.Black, e.MarginBounds.Left, e.MarginBounds.Top);
            e.Graphics.DrawLine(new Pen(Brushes.Black), e.MarginBounds.Left, e.MarginBounds.Top, e.MarginBounds.Right, e.MarginBounds.Bottom); 
            e.Graphics.DrawLine(new Pen(Brushes.Black), e.MarginBounds.Right, e.MarginBounds.Top, e.MarginBounds.Left, e.MarginBounds.Bottom); 
            e.Graphics.DrawLine(new Pen(Brushes.Black), e.MarginBounds.Left, e.MarginBounds.Top, e.MarginBounds.Left, e.MarginBounds.Bottom); 
            e.Graphics.DrawLine(new Pen(Brushes.Black), e.MarginBounds.Right, e.MarginBounds.Top, e.MarginBounds.Right, e.MarginBounds.Bottom); 
            
        };
        pd.Print();
#else
        PrintQueue printQueue = new LocalPrintServer().GetPrintQueues().FirstOrDefault(p => p.Name.Contains("PDF"));        
        printQueue.Comment = "Hello, Printer! Document";
        var ticket = printQueue.UserPrintTicket;
        ticket.PageMediaSize = new PageMediaSize(PageMediaSizeName.ISOA4);
        ticket.PageOrientation = PageOrientation.Landscape;
        ticket.PageResolution = new PageResolution(600, 600);
        ticket.Duplexing = Duplexing.TwoSidedShortEdge;
        XpsDocumentWriter xpsDocumentWriter = PrintQueue.CreateXpsDocumentWriter(printQueue);

        const int iScreenPPI = 92;
        const double fmmPerInch = 25.4;

        Rect r = new Rect(10.0 * iScreenPPI / fmmPerInch, 10.0 * iScreenPPI / fmmPerInch,
            (297.0-10) * iScreenPPI / fmmPerInch  , (210.0-10) * iScreenPPI / fmmPerInch );

        var visual = new DrawingVisual();
        DrawingContext dc = visual.RenderOpen();                
        dc.DrawText(new FormattedText("Hello, Printer!", CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Arial"), 24, Brushes.Black), r.TopLeft );
        dc.DrawLine(new Pen(Brushes.Black, 0.5), r.TopLeft, r.BottomRight);
        dc.DrawLine(new Pen(Brushes.Black, 0.5), r.TopRight, r.BottomLeft);
        dc.Close();

        xpsDocumentWriter.Write(visual,ticket);
        
#endif
    }
}
