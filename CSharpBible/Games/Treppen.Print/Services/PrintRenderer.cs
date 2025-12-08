using BaseLib.Helper;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Treppen.Base;
using Treppen.Export.Models;
using Treppen.Export.Services.Drawing;
using Treppen.Export.Services.Interfaces;

namespace Treppen.Print.Services;

public class PrintRenderer : IPrintRenderer
{
    public Task RenderAsync(IHeightLabyrinth labyrinth, PrintOptions options, Stream output)
    {
        var threeDVisualSrv = IoC.GetRequiredService<ILabyrinth3dDrawer>();
        var printDialog = new PrintDialog();
        if (printDialog.ShowDialog() != true)
        {
            return Task.CompletedTask;
        }

        DrawingVisual? threeDVisual = null;

        if (threeDVisualSrv is not null && labyrinth is not null)
        {
            threeDVisual = new DrawingVisual();
            using (var dc = threeDVisual.RenderOpen())
            {
                printDialog.PrintTicket.PageOrientation = System.Printing.PageOrientation.Landscape;
                var printableSize = new Size(printDialog.PrintableAreaWidth, printDialog.PrintableAreaHeight);
                // Baue Kommandos und rendere in den WPF-DrawingContext
                threeDVisualSrv.Build(labyrinth, printableSize, IoC.GetKeyedRequiredService<IDrawCommandFactory>("dc")).Render(dc);
            }

            printDialog.PrintVisual(threeDVisual, "Labyrinth 3D");

        }
        return Task.CompletedTask;
    }
}
