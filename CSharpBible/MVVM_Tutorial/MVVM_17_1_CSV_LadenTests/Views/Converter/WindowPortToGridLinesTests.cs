using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using MVVM_17_1_CSV_Laden.ViewModels;

namespace MVVM_17_1_CSV_Laden.Views.Converter.Tests;

[TestClass()]
public class WindowPortToGridLinesTests
{
    WindowPortToGridLines testVC;
    SWindowPort wp;

    public static IEnumerable<object[]> ConvertTestData
    {
        get
        {
            yield return new object[] { new SWindowPort() { Parent = null!, port = new System.Drawing.RectangleF(-10, -10, 20, 20) } };
            yield return new object[] { new SWindowPort() { Parent = null!, port = new System.Drawing.RectangleF(-10, -10, 20, 20) } };
        }
    }

    [TestInitialize]
    public void TestInit()
    {
        testVC = new WindowPortToGridLines();
        testVC.WindowSize = new System.Windows.Size(200, 100);
        wp = new SWindowPort() { Parent = null!, port = new System.Drawing.RectangleF(-10, -10, 20, 20) };
    }

    [TestMethod()]
    public void WindowPortToGridLinesTest()
    {
        Assert.Fail();
    }

    [TestMethod()]
    [DynamicData(nameof(ConvertTestData))]
    public void ConvertTest(object o)
    {
        var test = testVC.Convert(o, null, null, null);
        Assert.Fail();
    }

    [TestMethod()]
    public void GetAdjustedRectTest()
    {
        var r2 = testVC.GetAdjustedRect(wp);
        System.Drawing.RectangleF rExp = new(-20, -10, 40, 20);
        Assert.AreEqual(rExp, r2);
    }

    [TestMethod()]
    public void ConvertBackTest()
    {
        Assert.ThrowsExactly<NotImplementedException>(() => testVC.ConvertBack(null!, null, null, null));
    }

    private static void RunSTA(Action a)
    {
        Exception ex = null;
        var t = new System.Threading.Thread(() =>
        {
            try { a(); }
            catch (Exception e) { ex = e; }
        });
        t.SetApartmentState(System.Threading.ApartmentState.STA);
        t.Start();
        t.Join();
        if (ex != null) throw ex;
    }

    [DataTestMethod]
    [DataRow(200, 100, -10f, -10f, 20f, 20f, -20f, -10f, 40f, 20f)]
    [DataRow(100, 200, -10f, -10f, 20f, 20f, -10f, -20f, 20f, 40f)]
    public void GetAdjustedRect_Various_ReturnsExpected(
        int winW, int winH,
        float pL, float pT, float pW, float pH,
        float eL, float eT, float eW, float eH)
    {
        RunSTA(() =>
        {
            var conv = new WindowPortToGridLines
            {
                WindowSize = new System.Windows.Size(winW, winH)
            };
            var wp = new SWindowPort
            {
                port = new System.Drawing.RectangleF(pL, pT, pW, pH),
                WindowSize = new System.Windows.Size(winW, winH),
                Parent = null!
            };
            var r2 = conv.GetAdjustedRect(wp);
            var expected = new System.Drawing.RectangleF(eL, eT, eW, eH);
            Assert.AreEqual(expected, r2);
        });
    }

    [DataTestMethod]
    [DataRow(200, 100)]
    public void Convert_WindowPort_GeneratesGridAndLabels(int winW, int winH)
    {
        RunSTA(() =>
        {
            var conv = new WindowPortToGridLines
            {
                WindowSize = new System.Windows.Size(winW, winH)
            };
            var wp = new SWindowPort
            {
                port = new System.Drawing.RectangleF(-10, -10, 20, 20),
                WindowSize = new System.Windows.Size(winW, winH),
                Parent = null!
            };

            var result = conv.Convert(wp, null, null, System.Globalization.CultureInfo.InvariantCulture)
                as System.Collections.ObjectModel.ObservableCollection<System.Windows.FrameworkElement>;
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0, "Es wurden keine Elemente erzeugt.");

            int axisCount = 0, majorCount = 0, minorCount = 0, labelZeroCount = 0, ellipseCount = 0;
            foreach (var fe in result)
            {
                if (fe is System.Windows.Shapes.Line l)
                {
                    var th = l.StrokeThickness;
                    if (Math.Abs(th - 1.5d) < 1e-6) axisCount++;
                    else if (Math.Abs(th - 0.8d) < 1e-6) majorCount++;
                    else if (Math.Abs(th - 0.3d) < 1e-6) minorCount++;
                }
                else if (fe is System.Windows.Controls.Label lab)
                {
                    if (string.Equals(lab.Content?.ToString(), "0", StringComparison.Ordinal)) labelZeroCount++;
                }
                else if (fe is System.Windows.Shapes.Ellipse)
                {
                    ellipseCount++;
                }
            }

            Assert.IsTrue(axisCount >= 2, "Achsenlinien (Stärke 1.5) fehlen.");
            Assert.IsTrue(majorCount > 0, "Haupt-Gitternetzlinien (Stärke 0.8) fehlen.");
            Assert.IsTrue(minorCount > 0, "Neben-Gitternetzlinien (Stärke 0.3) fehlen.");
            Assert.IsTrue(labelZeroCount >= 1, "Mindestens eine '0'-Beschriftung wird erwartet.");
            Assert.AreEqual(1, ellipseCount, "Der Nullpunkt (Ellipse) wird erwartet.");
        });
    }

    [DataTestMethod]
    [DataRow(200, 100, -10.0, -10.0, 10.0, 10.0, 1)]
    [DataRow(200, 100, 0.0, 0.0, 1000.0, 1000.0, 1)]
    [DataRow(200, 100, -1000.0, -1000.0, 1000.0, 1000.0, 0)]
    public void Convert_DataPoints_FiltersByViewport(int winW, int winH,
        double x1, double y1, double x2, double y2, int expectedLines)
    {
        RunSTA(() =>
        {
            var conv = new WindowPortToGridLines
            {
                WindowSize = new System.Windows.Size(winW, winH)
            };

            // Erst Viewport konvertieren, um actPort zu setzen
            var wp = new SWindowPort
            {
                port = new System.Drawing.RectangleF(-10, -10, 20, 20),
                WindowSize = new System.Windows.Size(winW, winH),
                Parent = null!
            };
            _ = conv.Convert(wp, null, null, System.Globalization.CultureInfo.InvariantCulture);

            var ds = new System.Collections.ObjectModel.ObservableCollection<Model.DataPoint>
            {
                    new Model.DataPoint { X = x1, Y = y1 },
                    new Model.DataPoint { X = x2, Y = y2 }
            };

            var res = conv.Convert(ds, null, null, System.Globalization.CultureInfo.InvariantCulture)
                as System.Collections.ObjectModel.ObservableCollection<System.Windows.FrameworkElement>;
            Assert.IsNotNull(res);

            int lineCount = 0;
            foreach (var fe in res)
                if (fe is System.Windows.Shapes.Line) lineCount++;

            Assert.AreEqual(expectedLines, lineCount, "Anzahl der erzeugten Datenpunkt-Linien entspricht nicht der Erwartung.");
        });
    }

    [TestMethod]
    public void Convert_DataPointArray_ReturnsEmpty()
    {
        RunSTA(() =>
        {
            var conv = new WindowPortToGridLines
            {
                WindowSize = new System.Windows.Size(200, 100)
            };

            // actPort initialisieren
            var wp = new SWindowPort
            {
                port = new System.Drawing.RectangleF(-10, -10, 20, 20),
                WindowSize = new System.Windows.Size(200, 100),
                Parent = null!
            };
            _ = conv.Convert(wp, null, null, System.Globalization.CultureInfo.InvariantCulture);

            var arr = new Model.DataPoint[0];
            var res = conv.Convert(arr, null, null, System.Globalization.CultureInfo.InvariantCulture)
                as System.Collections.ObjectModel.ObservableCollection<System.Windows.FrameworkElement>;
            Assert.IsNotNull(res);
            Assert.AreEqual(0, res.Count);
        });
    }

    [TestMethod]
    public void NSubstitute_DummyUsage_ForLibraryPresence()
    {
        var d = NSubstitute.Substitute.For<IDisposable>();
        d.Dispose();
        Assert.IsNotNull(d);
    }
}
