using IDR.Core.Services;
using IDR.Core.Models;
using IDR.Infrastructure.Pe;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IDR.Infrastructure.Tests;

[TestClass]
public sealed class DelphiVersionDetectorTests
{
    [TestMethod]
    public void DetectRecognizesUniqueRuntimeMarker()
    {
        DelphiVersionDetection result = new DelphiVersionDetector().Detect(CreateImage("vcl120.bpl"));

        Assert.AreEqual(DelphiVersion.Delphi2009, result.SelectedVersion);
        CollectionAssert.AreEqual(
            new[] { DelphiVersion.Delphi2009 },
            result.Candidates.ToArray());
        Assert.IsFalse(result.IsAmbiguous);
    }

    [TestMethod]
    public void DetectReturnsCandidatesForUnknownImage()
    {
        DelphiVersionDetection result = new DelphiVersionDetector().Detect(CreateImage("unrelated"));

        Assert.AreEqual(DelphiVersion.Unknown, result.SelectedVersion);
        Assert.IsTrue(result.IsAmbiguous);
        Assert.AreEqual(15, result.Candidates.Count);
    }

    [TestMethod]
    public void DetectReportsAmbiguousRuntimeMarkers()
    {
        DelphiVersionDetection result = new DelphiVersionDetector()
            .Detect(CreateImage("vcl60.bpl\0vcl70.bpl"));

        Assert.AreEqual(DelphiVersion.Unknown, result.SelectedVersion);
        CollectionAssert.AreEqual(
            new[] { DelphiVersion.Delphi6, DelphiVersion.Delphi7 },
            result.Candidates.ToArray());
        Assert.IsTrue(result.IsAmbiguous);
    }

    [TestMethod]
    public void DetectKeepsSharedDelphi2006And2007RuntimeAmbiguous()
    {
        DelphiVersionDetection result = new DelphiVersionDetector().Detect(CreateImage("vcl100.bpl"));

        CollectionAssert.AreEqual(
            new[] { DelphiVersion.Delphi2006, DelphiVersion.Delphi2007 },
            result.Candidates.ToArray());
        Assert.AreEqual(DelphiVersion.Unknown, result.SelectedVersion);
        Assert.IsTrue(result.IsAmbiguous);
    }

    private static PeImage CreateImage(string content)
    {
        return new PeImage(
            "synthetic.exe",
            Encoding.ASCII.GetBytes(content),
            new List<PeSection>(),
            0);
    }
}
