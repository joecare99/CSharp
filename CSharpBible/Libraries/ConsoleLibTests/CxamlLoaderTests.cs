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
    public void Generator_ProducesDeterministicFactoryForValidMarkup()
    {
        var result = new CxamlCodeGenerator().Generate(
            "<Label Text=\"Say &quot;hi&quot;\" />", "GeneratedView", "Demo.Views");

        Assert.IsTrue(result.Succeeded);
        StringAssert.Contains(result.GeneratedCode, "namespace Demo.Views;");
        StringAssert.Contains(result.GeneratedCode, "public static class GeneratedView");
        StringAssert.Contains(result.GeneratedCode, "&quot;hi&quot;");
        StringAssert.Contains(result.GeneratedCode, "new ConsoleLib.CxamlLoader().Load");
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
