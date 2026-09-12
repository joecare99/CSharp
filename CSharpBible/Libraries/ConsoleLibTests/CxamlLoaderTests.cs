using System;
using System.IO;
using ConsoleLib;
using ConsoleLib.CommonControls;
using ConsoleLib.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConsoleLib.Tests;

[TestClass]
public class CxamlLoaderTests
{
    [TestMethod]
    public void Loader_CreatesNestedControlsAndAppliesAttributes()
    {
        var markup = "<StackPanel Width=\"20\"><Button Text=\"Run\" /><CheckBox Text=\"Ready\" /></StackPanel>";

        var root = new CxamlLoader().Load(new StringReader(markup));

        Assert.IsInstanceOfType(root, typeof(StackPanel));
        Assert.AreEqual(20, root.size.Width);
        Assert.AreEqual(2, root.Children.Count);
        Assert.AreEqual("Run", root.Children[0].Text);
    }

    [TestMethod]
    public void Loader_AppliesColorsAndCheckBoxState()
    {
        var root = new CxamlLoader().Load(new StringReader(
            "<CheckBox BackColor=\"DarkBlue\" ForeColor=\"White\" IsChecked=\"true\" />"));

        var checkBox = (CheckBox)root;
        Assert.AreEqual(ConsoleColor.DarkBlue, checkBox.BackColor);
        Assert.AreEqual(ConsoleColor.White, checkBox.ForeColor);
        Assert.IsTrue(checkBox.IsChecked);
    }

    [TestMethod]
    public void Validator_ReportsUnsupportedElementsAndAttributes()
    {
        ICxamlValidator validator = new CxamlLoader();

        var diagnostics = validator.Validate(new StringReader(
            "<StackPanel Unknown=\"1\"><NotAControl /></StackPanel>"));

        Assert.AreEqual(2, diagnostics.Count);
        StringAssert.Contains(diagnostics[0].Message, "Unknown");
        StringAssert.Contains(diagnostics[1].Message, "NotAControl");
    }

    [TestMethod]
    public void Loader_RejectsInvalidColor()
    {
        var error = Assert.ThrowsExactly<CxamlParseException>(() =>
            new CxamlLoader().Load(new StringReader("<Label ForeColor=\"NotAColor\" />")));

        StringAssert.Contains(error.Message, "ForeColor");
    }

    [TestMethod]
    public void Loader_AppliesPositionAndRejectsInvalidScalarValues()
    {
        var root = new CxamlLoader().Load(new StringReader(
            "<Label X=\"3\" Y=\"4\" Width=\"10\" Height=\"2\" Visible=\"false\" Enabled=\"false\" />"));

        Assert.AreEqual(3, root.Position.X);
        Assert.AreEqual(4, root.Position.Y);
        Assert.AreEqual(10, root.size.Width);
        Assert.AreEqual(2, root.size.Height);
        Assert.IsFalse(root.Visible);
        Assert.IsFalse(root.Enabled);

        var error = Assert.ThrowsExactly<CxamlParseException>(() =>
            new CxamlLoader().Load(new StringReader("<Label Width=\"not-a-number\" />")));
        StringAssert.Contains(error.Message, "Width");
    }

    [TestMethod]
    public void Loader_ParsesDetailedGridDefinitionsAndAttachedPlacement()
    {
        var root = (Grid)new CxamlLoader().Load(new StringReader(
            "<Grid Width=\"20\" Height=\"10\">" +
            "<Grid.RowDefinitions><RowDefinition Height=\"Auto\" /><RowDefinition Height=\"2*\" /></Grid.RowDefinitions>" +
            "<Grid.ColumnDefinitions><ColumnDefinition Width=\"4\" /><ColumnDefinition Width=\"*\" /></Grid.ColumnDefinitions>" +
            "<Label Text=\"Cell\" Grid.Row=\"1\" Grid.Column=\"1\" Grid.RowSpan=\"1\" Grid.ColumnSpan=\"1\" />" +
            "</Grid>"));

        Assert.AreEqual(2, root.RowDefinitions.Count);
        Assert.AreEqual(GridUnitType.Auto, root.RowDefinitions[0].Height.GridUnitType);
        Assert.AreEqual(2d, root.RowDefinitions[1].Height.Value);
        Assert.AreEqual(2, root.ColumnDefinitions.Count);
        Assert.AreEqual(4d, root.ColumnDefinitions[0].Width.Value);
        Assert.AreEqual(GridUnitType.Star, root.ColumnDefinitions[1].Width.GridUnitType);
        Assert.AreEqual(1, Grid.GetRow(root.Children[0]));
        Assert.AreEqual(1, Grid.GetColumn(root.Children[0]));
    }

    [TestMethod]
    public void Loader_ParsesCompactGridDefinitions()
    {
        var root = (Grid)new CxamlLoader().Load(new StringReader(
            "<Grid RowDefinitions=\"Auto,24,2*\" ColumnDefinitions=\"*,3*\" />"));

        Assert.AreEqual(3, root.RowDefinitions.Count);
        Assert.AreEqual(GridUnitType.Auto, root.RowDefinitions[0].Height.GridUnitType);
        Assert.AreEqual(24d, root.RowDefinitions[1].Height.Value);
        Assert.AreEqual(2d, root.RowDefinitions[2].Height.Value);
        Assert.AreEqual(2, root.ColumnDefinitions.Count);
        Assert.AreEqual(1d, root.ColumnDefinitions[0].Width.Value);
        Assert.AreEqual(3d, root.ColumnDefinitions[1].Width.Value);
    }

    [TestMethod]
    public void Validator_AcceptsDetailedGridDefinitions()
    {
        var diagnostics = new CxamlLoader().Validate(new StringReader(
            "<Grid><Grid.RowDefinitions><RowDefinition Height=\"1*\" /></Grid.RowDefinitions>" +
            "<Grid.ColumnDefinitions><ColumnDefinition Width=\"Auto\" /></Grid.ColumnDefinitions>" +
            "<Button Grid.Row=\"0\" Grid.Column=\"0\" /></Grid>"));

        Assert.AreEqual(0, diagnostics.Count);
    }

    [TestMethod]
    public void Validator_ReportsUnboundNamespacePrefix()
    {
        var diagnostics = new CxamlLoader().Validate(new StringReader("<local:Button />"));

        Assert.AreEqual(1, diagnostics.Count);
        Assert.AreEqual(CxamlDiagnosticSeverity.Error, diagnostics[0].Severity);
        StringAssert.Contains(diagnostics[0].Message, "local");
    }

    [TestMethod]
    public void Validator_ReportsUnrecognizedNamespace()
    {
        var diagnostics = new CxamlLoader().Validate(new StringReader(
            "<local:Button xmlns:local=\"http://example.com/unknown\" />"));

        Assert.AreEqual(1, diagnostics.Count);
        Assert.AreEqual(CxamlDiagnosticSeverity.Warning, diagnostics[0].Severity);
        StringAssert.Contains(diagnostics[0].Message, "http://example.com/unknown");
    }

    [TestMethod]
    public void Validator_AcceptsRecognizedCoreNamespace()
    {
        var diagnostics = new CxamlLoader().Validate(new StringReader(
            "<local:Button xmlns:local=\"" + CxamlNamespaces.Core + "\" />"));

        Assert.AreEqual(0, diagnostics.Count);
    }

    [TestMethod]
    public void Validator_WarnsFrameRootWithoutSinglePage()
    {
        var emptyFrame = new CxamlLoader().Validate(new StringReader("<Frame />"));
        Assert.AreEqual(1, emptyFrame.Count);
        Assert.AreEqual(CxamlDiagnosticSeverity.Warning, emptyFrame[0].Severity);
        StringAssert.Contains(emptyFrame[0].Message, "Page");

        var labelChild = new CxamlLoader().Validate(new StringReader("<Frame><Label Text=\"Hi\" /></Frame>"));
        Assert.AreEqual(1, labelChild.Count);
        StringAssert.Contains(labelChild[0].Message, "Page");
    }

    [TestMethod]
    public void Validator_AcceptsFrameRootWithSinglePage()
    {
        var diagnostics = new CxamlLoader().Validate(new StringReader(
            "<Frame Width=\"40\" Height=\"10\"><Page Width=\"40\" Height=\"10\"><Label Text=\"Page\" /></Page></Frame>"));

        Assert.AreEqual(0, diagnostics.Count);
    }

    [TestMethod]
    public void Loader_RejectsInvalidGridLength()
    {
        var error = Assert.ThrowsExactly<CxamlParseException>(() =>
            new CxamlLoader().Load(new StringReader("<Grid RowDefinitions=\"Auto,invalid\" />")));

        StringAssert.Contains(error.Message, "Grid");
    }

    [TestMethod]
    public void Loader_RejectsMalformedAndMultipleRootMarkup()
    {
        var malformed = Assert.ThrowsExactly<CxamlParseException>(() =>
            new CxamlLoader().Load(new StringReader("<Label>")));
        Assert.IsNotNull(malformed.InnerException);

        var multipleRoots = Assert.ThrowsExactly<CxamlParseException>(() =>
            new CxamlLoader().Load(new StringReader("<Label /><Label />")));
        StringAssert.Contains(multipleRoots.Message, "Invalid CXAML markup");
    }

    [TestMethod]
    public void Generator_ProducesDeterministicDirectFactoryForValidMarkup()
    {
        var result = new CxamlCodeGenerator().Generate(
            "<Label Text=\"Say &quot;hi&quot;\" />", "GeneratedView", "Demo.Views");

        Assert.IsTrue(result.Succeeded);
        StringAssert.Contains(result.GeneratedCode, "namespace Demo.Views;");
        StringAssert.Contains(result.GeneratedCode, "public static class GeneratedView");
        StringAssert.Contains(result.GeneratedCode, "new ConsoleLib.CommonControls.Label()");
        StringAssert.Contains(result.GeneratedCode, "control0.Text = \"Say \\\"hi\\\"\";");
        Assert.IsFalse(result.GeneratedCode.Contains("CxamlLoader", StringComparison.Ordinal));
    }

    [TestMethod]
    public void Generator_ReturnsRuntimeEquivalentDiagnosticsWithoutGeneratedCode()
    {
        var result = new CxamlCodeGenerator().Generate(
            "<Label ForeColor=\"Invalid\" />", "GeneratedView", "Demo.Views");

        Assert.IsFalse(result.Succeeded);
        Assert.AreEqual(string.Empty, result.GeneratedCode);
        Assert.AreEqual(1, result.Diagnostics.Count);
        StringAssert.Contains(result.Diagnostics[0].Message, "ForeColor");
    }
}
