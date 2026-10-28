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
    public void DetectPrefersRuntimeImportsOverEmbeddedMarkerText()
    {
        DelphiVersionDetection result = new DelphiVersionDetector().Detect(
            CreateImage(
                "vcl70.bpl",
                new PeImportModule("VCL180.BPL", [])));

        Assert.AreEqual(DelphiVersion.DelphiXE4, result.SelectedVersion);
        CollectionAssert.AreEqual(
            new[] { DelphiVersion.DelphiXE4 },
            result.Candidates.ToArray());
        Assert.IsFalse(result.IsAmbiguous);
    }

    [TestMethod]
    [DataRow("VCL30.BPL", DelphiVersion.Delphi3)]
    [DataRow("VCL40.DPL", DelphiVersion.Delphi4)]
    [DataRow("vcl50.bpl", DelphiVersion.Delphi5)]
    [DataRow("RTL60.BPL", DelphiVersion.Delphi6)]
    [DataRow("rtl70.dpl", DelphiVersion.Delphi7)]
    [DataRow("RTL80.BPL", DelphiVersion.Delphi8)]
    [DataRow("VCL90.BPL", DelphiVersion.Delphi2005)]
    [DataRow("rtl100.dpl", DelphiVersion.Delphi2006)]
    [DataRow("VCL110.BPL", DelphiVersion.Delphi2007)]
    [DataRow("RTL120.BPL", DelphiVersion.Delphi2009)]
    [DataRow("vcl140.bpl", DelphiVersion.Delphi2010)]
    [DataRow("RTL150.DPL", DelphiVersion.DelphiXE1)]
    [DataRow("vcl160.bpl", DelphiVersion.DelphiXE2)]
    [DataRow("RTL170.BPL", DelphiVersion.DelphiXE3)]
    [DataRow("vcl180.dpl", DelphiVersion.DelphiXE4)]
    public void DetectRecognizesCompilerVersionFromRuntimeImport(
        string moduleName,
        DelphiVersion expectedVersion)
    {
        DelphiVersionDetection result = new DelphiVersionDetector().Detect(
            CreateImage("unrelated", new PeImportModule(moduleName, [])));

        Assert.AreEqual(expectedVersion, result.SelectedVersion);
        CollectionAssert.AreEqual(
            new[] { expectedVersion },
            result.Candidates.ToArray());
        Assert.IsFalse(result.IsAmbiguous);
    }

    [TestMethod]
    public void DetectKeepsSameVersionRuntimeImportsUnambiguous()
    {
        DelphiVersionDetection result = new DelphiVersionDetector().Detect(
            CreateImage(
                "unrelated",
                new PeImportModule("VCL70.BPL", []),
                new PeImportModule("RTL70.BPL", [])));

        Assert.AreEqual(DelphiVersion.Delphi7, result.SelectedVersion);
        CollectionAssert.AreEqual(
            new[] { DelphiVersion.Delphi7 },
            result.Candidates.ToArray());
        Assert.IsFalse(result.IsAmbiguous);
    }

    [TestMethod]
    public void DetectReportsConflictingRuntimeImportsAsAmbiguous()
    {
        DelphiVersionDetection result = new DelphiVersionDetector().Detect(
            CreateImage(
                "unrelated",
                new PeImportModule("VCL70.BPL", []),
                new PeImportModule("RTL80.BPL", [])));

        Assert.AreEqual(DelphiVersion.Unknown, result.SelectedVersion);
        CollectionAssert.AreEqual(
            new[] { DelphiVersion.Delphi7, DelphiVersion.Delphi8 },
            result.Candidates.ToArray());
        Assert.IsTrue(result.IsAmbiguous);
    }

    [TestMethod]
    public void DetectDoesNotInferRuntimeVersionFromUnrelatedImportModuleName()
    {
        DelphiVersionDetection result = new DelphiVersionDetector().Detect(
            CreateImage("unrelated", new PeImportModule("myvcl70.bpl", [])));

        Assert.AreEqual(DelphiVersion.Unknown, result.SelectedVersion);
        Assert.AreEqual(16, result.Candidates.Count);
        Assert.IsTrue(result.IsAmbiguous);
    }

    [TestMethod]
    public void DetectReturnsCandidatesForUnknownImage()
    {
        DelphiVersionDetection result = new DelphiVersionDetector().Detect(CreateImage("unrelated"));

        Assert.AreEqual(DelphiVersion.Unknown, result.SelectedVersion);
        Assert.IsTrue(result.IsAmbiguous);
        Assert.AreEqual(16, result.Candidates.Count);
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
    public void DetectMapsVcl100ToDelphi2006Only()
    {
        DelphiVersionDetection result = new DelphiVersionDetector().Detect(
            CreateImage("vcl100.bpl"));

        CollectionAssert.AreEqual(
            new[] { DelphiVersion.Delphi2006 },
            result.Candidates.ToArray());
        Assert.AreEqual(DelphiVersion.Delphi2006, result.SelectedVersion);
        Assert.IsFalse(result.IsAmbiguous);
    }

    private static PeImage CreateImage(string content, params PeImportModule[] imports)
    {
        return new PeImage(
            "synthetic.exe",
            Encoding.ASCII.GetBytes(content),
            new List<PeSection>(),
            0)
        {
            Imports = imports
        };
    }
}
