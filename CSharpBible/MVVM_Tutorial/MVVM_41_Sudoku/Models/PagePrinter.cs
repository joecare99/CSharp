using System;
using System.Linq;
using System.Printing;
using System.Windows.Media;
using System.Windows;

namespace MVVM_41_Sudoku.Models;

public static class PagePrinter
{
    const int cPPI = 92;
    const double cMmPerInch = 25.4;
    const double cDpi = cPPI / cMmPerInch;
    private const int cA4Width = 210; //[mm]
    private const int cA4Height = 297; //[mm]
    private const int cBoarder = 10; //[mm]

    public static void Print(string printerName, string title, object? data, Action<string, object, DrawingContext, Rect> drawPage)
    {

        PrintQueue? printQueue = GetPrintQueue(printerName);
        if (printQueue == null)
        {
            return;
        }


        PrintTicket ticket = GetTicket(printQueue);

        Print(printQueue, ticket, title, data, drawPage);
    }

    public static void Print(PrintQueue? printQueue, PrintTicket ticket, string title, object? data, Action<string, object, DrawingContext, Rect> drawPage)
    {
        if (printQueue == null)
        {
            return;
        }
        printQueue.Comment = $"Comment: {title}";
        printQueue.CurrentJobSettings.Description = $"Desc: {title}";
        var writer = PrintQueue.CreateXpsDocumentWriter(printQueue);

        Rect boundingRect =
             //*/ ticket.PageMediaSize.Width != null && ticket.PageMediaSize.Height != null  /*
         //*/     ? new(cBoarder * cDpi, cBoarder * cDpi, ticket.PageMediaSize.Width.Value - cBoarder * cDpi, ticket.PageMediaSize.Height.Value - cBoarder * cDpi) :
             new(cBoarder * cDpi, cBoarder * cDpi, (cA4Width - cBoarder) * cDpi, (cA4Height - cBoarder) * cDpi);
        var visual = new DrawingVisual();
        using (DrawingContext dc = visual.RenderOpen())
        {
            drawPage(title, data, dc, boundingRect);
            dc.Close();

        }
        writer.Write(visual, ticket);
    }

    private static PrintTicket GetTicket(PrintQueue? printQueue)
    {
        var ticket = printQueue?.UserPrintTicket;
        if (ticket == null)
        {
            return new PrintTicket();
        }
        ticket.PageMediaSize = new PageMediaSize(PageMediaSizeName.ISOA4);
        ticket.PageOrientation = PageOrientation.Portrait;
        ticket.PageResolution = new PageResolution(300, 300);
        return ticket;
    }

    private static PrintQueue? GetPrintQueue(string printerName)
    {
        return new LocalPrintServer()?.GetPrintQueues().FirstOrDefault(p => p.Name.Contains(printerName));
    }
}