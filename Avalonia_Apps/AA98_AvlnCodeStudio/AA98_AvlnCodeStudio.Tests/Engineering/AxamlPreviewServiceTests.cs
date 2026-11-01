using AA98_AvlnCodeStudio.Model.Workspaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace AA98_AvlnCodeStudio.Tests.Engineering;

[TestClass]
public sealed class AxamlPreviewServiceTests
{
    [TestMethod]
    public void Parse_BuildsSafeTreeForSupportedControlsAndLayout()
    {
        const string axaml = """
            <Window xmlns="https://github.com/avaloniaui"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                    x:Class="Sample.MainWindow">
              <Grid RowDefinitions="Auto,*" ColumnDefinitions="*">
                <StackPanel Grid.Row="1" Spacing="8">
                  <TextBlock Text="Hello" FontSize="20" />
                  <Button Content="Run" />
                </StackPanel>
              </Grid>
            </Window>
            """;

        AxamlPreviewNode root = new AxamlPreviewService().Parse(axaml);

        Assert.AreEqual("Grid", root.TypeName);
        Assert.AreEqual(1, root.Children.Count);
        Assert.AreEqual("StackPanel", root.Children[0].TypeName);
        Assert.AreEqual("8", root.Children[0].Properties["Spacing"]);
        Assert.AreEqual("Hello", root.Children[0].Children[0].Text);
        Assert.AreEqual("Run", root.Children[0].Children[1].Text);
        Assert.AreEqual("Auto", root.RowDefinitions[0]);
        Assert.AreEqual("*", root.RowDefinitions[1]);
        Assert.IsFalse(root.Properties.ContainsKey("Class"));
    }

    [TestMethod]
    public void Parse_RepresentsCustomControlsAsNonExecutablePlaceholders()
    {
        AxamlPreviewNode root = new AxamlPreviewService().Parse(
            "<UserControl xmlns='https://github.com/avaloniaui'><MyCustomControl /></UserControl>");

        Assert.AreEqual("MyCustomControl", root.TypeName);
        Assert.IsFalse(root.IsSupported);
    }

    [TestMethod]
    public void Parse_RejectsDtdAndOversizedInput()
    {
        AxamlPreviewService service = new();

        Assert.ThrowsExactly<System.Xml.XmlException>(() =>
            service.Parse("<!DOCTYPE x [<!ENTITY e 'expanded'>]><TextBlock Text='&e;' />"));
        Assert.ThrowsExactly<InvalidDataException>(() =>
            service.Parse(new string(' ', 1_000_001)));
    }

    [TestMethod]
    public void Parse_RejectsExcessiveControlNesting()
    {
        string axaml = "<StackPanel>";
        for (int index = 0; index < 66; index++)
        {
            axaml += "<StackPanel>";
        }

        axaml += "<TextBlock Text='Too deep' />";
        for (int index = 0; index < 67; index++)
        {
            axaml += "</StackPanel>";
        }

        Assert.ThrowsExactly<InvalidDataException>(() => new AxamlPreviewService().Parse(axaml));
    }

    [TestMethod]
    public void UpdateTextAttribute_ChangesOnlySelectedAttributeAndEscapesMarkup()
    {
        const string axaml = """
            <Window xmlns="https://github.com/avaloniaui">
              <TextBlock Text='Before' Margin="4" />
            </Window>
            """;
        AxamlPreviewService service = new();
        AxamlPreviewNode root = service.Parse(axaml);

        string updated = service.UpdateTextAttribute(axaml, root, "After & \"quoted\"");

        StringAssert.Contains(updated, """Text='After &amp; &quot;quoted&quot;' Margin="4" """);
        Assert.AreEqual("After & \"quoted\"", service.Parse(updated).Text);
        Assert.IsTrue(service.CanEditText(root));
    }

    [TestMethod]
    public void UpdateTextAttribute_AddsMissingAttributeWithoutReformattingOtherMarkup()
    {
        const string axaml = "<StackPanel>\r\n  <TextBlock />\r\n</StackPanel>";
        AxamlPreviewService service = new();
        AxamlPreviewNode root = service.Parse(axaml);

        string updated = service.UpdateTextAttribute(axaml, root.Children[0], "New line\ntext");

        Assert.AreEqual("<StackPanel>\r\n  <TextBlock Text=\"New line&#xA;text\"/>\r\n</StackPanel>", updated);
        Assert.AreEqual("New line\ntext", service.Parse(updated).Children[0].Text);
    }

    [TestMethod]
    public void CanEditText_RejectsTextPropertyElementsAndStaleSourceNodes()
    {
        AxamlPreviewService service = new();
        AxamlPreviewNode propertyElementNode = service.Parse(
            "<TextBlock xmlns='https://github.com/avaloniaui'><TextBlock.Text>Inline text</TextBlock.Text></TextBlock>");
        AxamlPreviewNode inlineTextNode = service.Parse("<TextBlock>Inline text</TextBlock>");
        AxamlPreviewNode staleNode = service.Parse("<TextBlock Text='Old' />");
        AxamlPreviewNode labelNode = service.Parse("<Label Content='Caption' />");

        Assert.IsFalse(service.CanEditText(propertyElementNode));
        Assert.IsFalse(service.CanEditText(inlineTextNode));
        Assert.ThrowsExactly<InvalidDataException>(() =>
            service.UpdateTextAttribute("<TextBlock Text='Other' />", staleNode, "Wrong target"));
        Assert.AreEqual("Caption", labelNode.Text);
        Assert.AreEqual("Changed", service.Parse(service.UpdateTextAttribute(
            "<Label Content='Caption' />",
            labelNode,
            "Changed")).Text);
    }

    [TestMethod]
    public void CanEditProperty_RejectsBindingsAndResources()
    {
        AxamlPreviewService service = new();
        AxamlPreviewNode node = service.Parse(
            "<StackPanel xmlns='https://github.com/avaloniaui' Width='{Binding Width}' Height='{StaticResource PanelHeight}' />");

        Assert.IsFalse(service.CanEditProperty(node, "Width"));
        Assert.IsFalse(service.CanEditProperty(node, "Height"));
        Assert.ThrowsExactly<System.InvalidOperationException>(() =>
            service.UpdateAttribute("<StackPanel Width='{Binding Width}' />", node, "Width", "200"));
    }
}
