using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Document.Axaml;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AA25_RichTextEdit.Tests.Document;

[TestClass]
public class FlowDocumentAxamlConverterTests
{
    [TestMethod]
    public void ExtractPlainTextReadsRunsAndLineBreaksTest()
    {
        var converter = new FlowDocumentAxamlConverter();
        const string flowDocumentXaml = "<FlowDocument xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"><Paragraph><Run FontWeight=\"Bold\">Hello</Run><LineBreak /><Run>World</Run></Paragraph></FlowDocument>";

        var result = converter.ExtractPlainText(flowDocumentXaml);

        StringAssert.Contains(result, "Hello");
        StringAssert.Contains(result, "World");
    }

    [TestMethod]
    public void CreatePreviewControlBuildsAvaloniaTextPreviewTest()
    {
        var converter = new FlowDocumentAxamlConverter();
        const string flowDocumentXaml = "<FlowDocument xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"><Paragraph TextAlignment=\"Justify\"><Run FontWeight=\"Bold\">Preview</Run></Paragraph></FlowDocument>";

        var control = converter.CreatePreviewControl(flowDocumentXaml);

        Assert.IsInstanceOfType<StackPanel>(control);
        var firstTextBlock = ((StackPanel)control).Children.OfType<TextBlock>().FirstOrDefault();
        Assert.IsNotNull(firstTextBlock);
        Assert.AreEqual(TextAlignment.Justify, firstTextBlock.TextAlignment);
    }
}
