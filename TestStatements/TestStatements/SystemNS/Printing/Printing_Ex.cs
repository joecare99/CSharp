using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if !NET5_0_OR_GREATER
using System.Drawing.Printing;
using System.Drawing;
#else
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
#endif
    }
}
