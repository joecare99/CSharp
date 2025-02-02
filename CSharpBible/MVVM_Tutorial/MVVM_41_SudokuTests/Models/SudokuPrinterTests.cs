using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Xps.Packaging;
using Sudoku_Base.Models.Interfaces;
using Sudoku_Base.Models;
using NSubstitute;
using BaseLib.Interfaces;
using BaseLib.Models.Interfaces;

namespace MVVM_41_Sudoku.Models.Tests;
[TestClass]
public class SudokuPrinterTests
{
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Fügen Sie ggf. den „erforderlichen“ Modifizierer hinzu, oder deklarieren Sie den Modifizierer als NULL-Werte zulassend.
    ISudokuModel _model;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Fügen Sie ggf. den „erforderlichen“ Modifizierer hinzu, oder deklarieren Sie den Modifizierer als NULL-Werte zulassend.
    
    [TestInitialize]
    public void Init()
    {
        _model = new SudokuModel(Substitute.For<ISysTime>(),Substitute.For<ILog>());
    }


    [DataTestMethod()]
    [DataRow(new[] {1,0,3,0,0,0,7,0,0,
                    9,4,5,0,3,0,2,3,4,
                    8,0,3,0,0,0,7,0,0,
                    7,4,5,0,3,0,2,3,4,
                    6,0,3,0,0,0,7,0,0,
                    5,4,5,0,3,0,2,3,4,
                    4,0,3,0,0,0,7,0,0,
                    3,4,5,0,3,0,2,3,4,
                    2,1,9,8,7,6,5,4,3,})]
    public void PrintTest(int[] aiVal)
    {
        PagePrinter.Print("PDF", "Sudoku", aiVal.Select(i => i == 0 ? null : (int?)i).ToArray(), _model.DrawSudoku);
    }

    [TestMethod()]
    public void PrintTest2()
    {
        PagePrinter.Print("PDF", "TestPage", null, TestPage );
    }

    private void TestPage(string title, object? data, DrawingContext dc, Rect r)
    {
        Pen blackLinePen = new Pen(Brushes.Black, 0.5);

        dc.DrawText(new FormattedText(title, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Arial"), 24, Brushes.Black, 1.0), r.TopLeft);
        // Draw a diagonal cross 
        dc.DrawLine(blackLinePen, r.TopLeft, r.BottomRight);
        dc.DrawLine(blackLinePen, r.TopRight, r.BottomLeft);
        // Draw a rectangle
        dc.DrawRectangle(Brushes.Transparent, blackLinePen, r);
    }

    [TestMethod()]
    public void DrawSudokuTest()
    {
        DrawingVisual visual = new DrawingVisual();
        DrawingContext dc = visual.RenderOpen();
        _model.DrawSudoku("Test", new int?[] {1,0,3,0,0,0,7,0,0,
                    9,4,5,0,3,0,2,3,4,
                    8,0,3,0,0,0,7,0,0,
                    7,4,5,0,3,0,2,3,4,
                    6,0,3,0,0,0,7,0,0,
                    5,4,5,0,3,0,2,3,4,
                    4,0,3,0,0,0,7,0,0,
                    3,4,5,0,3,0,2,3,4,
                    2,1,9,8,7,6,5,4,3,}, dc, new Rect(30, 30, 700, 1000));
        dc.Close();
        Assert.AreEqual(0, visual.Children.Count) ;

        MemoryStream stream = new MemoryStream();
        var package = Package.Open(stream,FileMode.CreateNew);
        
        //var xpsDoc = new XpsDocument(Path.GetTempFileName(),FileAccess.ReadWrite);
        var xpsDoc = new XpsDocument(package,CompressionOption.Maximum);

   //     xpsDoc.Uri = new Uri(Path.GetTempFileName());
   //     xpsDoc.AddFixedDocumentSequence();

        var writer = XpsDocument.CreateXpsDocumentWriter(xpsDoc);

        writer.Write(visual);
        
        xpsDoc.Close(); 

        Assert.AreEqual(6,package.GetParts().Count());
        package.Close();
   //     Assert.AreEqual(74360,stream.Length);
   //     Assert.AreEqual(74360,stream.Position);
        stream.Seek(0, SeekOrigin.Begin);  
        using(var fs = new FileStream($"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\\TestSudoku.xps", FileMode.CreateNew))
        {
            stream.CopyTo(fs);
        }

    } 

    [TestMethod()]
    public void InternalDrawTest()
    {
        DrawingVisual visual = new DrawingVisual();
        DrawingContext dc = visual.RenderOpen();
        TestPage("Test2", null, dc, new Rect(30, 30, 700, 1000));
        dc.Close();
        Assert.AreEqual(0, visual.Children.Count) ;

        MemoryStream stream = new MemoryStream();
        var package = Package.Open(stream,FileMode.CreateNew);
        
        //var xpsDoc = new XpsDocument(Path.GetTempFileName(),FileAccess.ReadWrite);
        var xpsDoc = new XpsDocument(package,CompressionOption.Maximum);

   //     xpsDoc.Uri = new Uri(Path.GetTempFileName());
   //     xpsDoc.AddFixedDocumentSequence();

        var writer = XpsDocument.CreateXpsDocumentWriter(xpsDoc);

        writer.Write(visual);
        
        xpsDoc.Close(); 

        Assert.AreEqual(6,package.GetParts().Count());
        package.Close();
   //     Assert.AreEqual(74360,stream.Length);
   //     Assert.AreEqual(74360,stream.Position);
        stream.Seek(0, SeekOrigin.Begin);  
        using(var fs = new FileStream($"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\\TestInt.xps", FileMode.CreateNew))
        {
            stream.CopyTo(fs);
        }

    }
}

