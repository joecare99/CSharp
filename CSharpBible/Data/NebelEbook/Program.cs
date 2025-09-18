using MigraDoc.DocumentObjectModel;
using PdfSharp.Drawing;
using PdfSharp.Drawing.Layout;
using PdfSharp.Fonts;
using PdfSharp.Pdf;
using PdfSharp.Pdf.Annotations;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using SkiaSharp;

namespace NebelEbook;

class Program
{
    static void Main()
    {
        GlobalFontSettings.UseWindowsFontsUnderWindows = true;

        var nodes = BuildStoryNodes();

        PdfDocument doc = new PdfDocument();
        doc.Info.Title = "Nebel über Bretten – Interaktives eBook";

        var fontTitle = new XFont("Arial", 18, XFontStyleEx.Bold);
        var fontBody = new XFont("Arial", 12, XFontStyleEx.Regular);
        var fontLink = new XFont("Arial", 12, XFontStyleEx.Underline);

        // Inhaltsverzeichnis
        var tocPage = doc.AddPage();
        var gfxToc = XGraphics.FromPdfPage(tocPage);
        gfxToc.DrawString("Inhaltsverzeichnis", fontTitle, XBrushes.Black, new XRect(0, 40, tocPage.Width, 20), XStringFormats.TopCenter);

        double tocY = 80;
        foreach (var node in nodes)
        {
            gfxToc.DrawString(node.Title, fontBody, XBrushes.Black, 40, tocY);
            var rect = new XRect(300, tocY - 12, 200, 20);
            gfxToc.DrawString("Gehe zu Kapitel", fontLink, XBrushes.Blue, rect, XStringFormats.TopLeft);

            var link = new PdfLinkAnnotation()
            {
                Rectangle = rect.ToPdfRect(),
            };
            tocPage.Annotations.Add(link);

            tocY += 25;
        }

        // Kapitel-Seiten
        var pageIndexMap = new Dictionary<string, PdfPage>();
        foreach (var node in nodes)
        {
            var page = doc.AddPage();
            pageIndexMap[node.Id] = page;
            var gfx = XGraphics.FromPdfPage(page);

            double y = 40;
            gfx.DrawString(node.Title, fontTitle, XBrushes.Black, 40, y);
            y += 30;

            foreach (var para in node.Paragraphs)
            {
                var tf = new XTextFormatter(gfx);
                tf.DrawString(para, fontBody, XBrushes.Black, new XRect(40, y, page.Width - 80, 100), XStringFormats.TopLeft);
                y += 60;
            }

            if (node.Choices != null && node.Choices.Count > 0)
            {
                y += 20;
                gfx.DrawString("Deine Wahl:", fontBody, XBrushes.Black, 40, y);
                y += 20;

                foreach (var choice in node.Choices)
                {
                    var rect = new XRect(60, y - 12, 400, 20);
                    gfx.DrawString("→ " + choice.Label, fontLink, XBrushes.Blue, rect, XStringFormats.TopLeft);

                    var link = new PdfLinkAnnotation()
                    {
                        Rectangle = rect.ToPdfRect(),
                    };
                    page.Annotations.Add(link);

                    y += 20;
                }
            }
        }

        // Links aktualisieren
        int tocIndex = 0;
        foreach (var node in nodes)
        {
            var tocLink = tocPage.Annotations[tocIndex] as PdfLinkAnnotation;
            tocIndex++;
        }

        // Kapitel-Links aktualisieren
        foreach (var node in nodes)
        {
            var page = pageIndexMap[node.Id];
            int choiceIndex = 0;
            foreach (var choice in node.Choices)
            {
                var link = page.Annotations[choiceIndex] as PdfLinkAnnotation;
                choiceIndex++;
            }
        }

        AddFlowchartPage(doc, nodes);

        doc.Save("Nebel_ueber_Bretten_Interaktiv.pdf");
        Console.WriteLine("PDF erstellt: Nebel_ueber_Bretten_Interaktiv.pdf");
    }
    static void AddFlowchartPage(PdfDocument pdf, List<StoryNode> nodes)
    {
        int width = 1200;
        int height = 1600;
        using var bitmap = new SKBitmap(width, height);
        using var canvas = new SKCanvas(bitmap);
        canvas.Clear(SKColors.White);

        var paintText = new SKPaint
        {
            Color = SKColors.Black,
            TextSize = 20,
            IsAntialias = true
        };
        var paintRect = new SKPaint
        {
            Color = SKColors.LightGray,
            IsAntialias = true,
            Style = SKPaintStyle.Fill
        };
        var paintLine = new SKPaint
        {
            Color = SKColors.DarkSlateGray,
            StrokeWidth = 2,
            IsAntialias = true
        };

        // Einfache Layout-Logik: Knoten untereinander
        int x = 100;
        int y = 80;
        int boxW = 300;
        int boxH = 60;
        int vSpacing = 100;

        var positions = new Dictionary<string, SKPoint>();

        foreach (var node in nodes)
        {
            var rect = new SKRect(x, y, x + boxW, y + boxH);
            canvas.DrawRect(rect, paintRect);
            canvas.DrawText(node.Title, x + 10, y + 35, paintText);
            positions[node.Id] = new SKPoint(x + boxW / 2, y + boxH);

            y += boxH + vSpacing;
        }

        // Pfeile zeichnen
        foreach (var node in nodes)
        {
            if (node.Choices == null) continue;
            foreach (var choice in node.Choices)
            {
                if (positions.ContainsKey(node.Id) && positions.ContainsKey(choice.TargetId))
                {
                    var start = positions[node.Id];
                    var end = positions[choice.TargetId];
                    canvas.DrawLine(start.X, start.Y, end.X, end.Y - boxH, paintLine);
                }
            }
        }

        // In MemoryStream speichern
        using var imgStream = new MemoryStream();
        using (var image = SKImage.FromBitmap(bitmap))
        using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
        {
            data.SaveTo(imgStream);
        }

        var page=  pdf.AddPage();
        
    }

    static List<StoryNode> BuildStoryNodes()
    {
        var nodes = new List<StoryNode>();

        nodes.Add(new StoryNode(
            "kap1",
            "Kapitel 1 – Fund im Nebel",
            new[]
            {
            "Der Nebel hing schwer über der Melanchthonstraße...",
            "Im Salon: Friedrich von Hohenberg, tot im Sessel..."
            },
            new List<Choice>
            {
            new Choice("Clara sofort befragen", "kap2_clara"),
            new Choice("Tatort gründlich untersuchen", "kap2_tatort")
            }
        ));

        nodes.Add(new StoryNode(
            "kap2_clara",
            "Kapitel 2 – Gespräch mit Clara",
            new[]
            {
            "Clara öffnete die Tür mit verweinten Augen...",
            "»Wir haben gestritten«, sagte sie leise..."
            },
            new List<Choice>
            {
            new Choice("Nach Alibi fragen", "kap3_alibi"),
            new Choice("Skizzenblock untersuchen", "kap3_skizzen")
            }
        ));

        nodes.Add(new StoryNode(
            "kap2_tatort",
            "Kapitel 2 – Stille im Salon",
            new[]
            {
            "Der Salon roch nach kaltem Kaminrauch...",
            "Kein Kampf, keine umgestürzten Möbel..."
            },
            new List<Choice>
            {
            new Choice("Rotwein ins Labor", "kap3_labor"),
            new Choice("Fenster prüfen", "kap3_fenster")
            }
        ));

        // Weitere Kapitel hier ergänzen...

        return nodes;
    }
}

static class PdfExtensions
{
    public static PdfRectangle ToPdfRect(this XRect rect)
    {
        return new PdfRectangle(rect.TopLeft, rect.Size);
    }
}