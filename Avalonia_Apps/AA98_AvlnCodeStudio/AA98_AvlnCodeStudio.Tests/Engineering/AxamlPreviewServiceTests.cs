using AA98_AvlnCodeStudio.Model.Workspaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;

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
    public void Parse_RepresentsExpanderHeaderAndSingleContentChild()
    {
        const string markup =
            "<Expander xmlns='https://github.com/avaloniaui' Header='Details' IsExpanded='True'><TextBlock Text='Content' /></Expander>";
        AxamlPreviewService service = new();
        AxamlPreviewNode root = service.Parse(markup);

        Assert.AreEqual("Expander", root.TypeName);
        Assert.IsTrue(root.IsSupported);
        Assert.AreEqual("Details", root.Text);
        Assert.AreEqual("Content", root.Children.Single().Text);
        Assert.IsFalse(root.CanAppendChildren);
        Assert.ThrowsExactly<System.InvalidOperationException>(() =>
            service.InsertChildControl(markup, root, "Button"));
    }

    [TestMethod]
    public void Parse_RepresentsGroupBoxHeaderAndSingleContentChild()
    {
        const string markup =
            "<GroupBox xmlns='https://github.com/avaloniaui' Header='Options'><TextBlock Text='Content' /></GroupBox>";
        AxamlPreviewService service = new();
        AxamlPreviewNode root = service.Parse(markup);

        Assert.AreEqual("GroupBox", root.TypeName);
        Assert.IsTrue(root.IsSupported);
        Assert.AreEqual("Options", root.Text);
        Assert.AreEqual("Content", root.Children.Single().Text);
        Assert.IsFalse(root.CanAppendChildren);
        Assert.IsTrue(service.CanEditProperty(root, "Header"));
        Assert.ThrowsExactly<System.InvalidOperationException>(() =>
            service.InsertChildControl(markup, root, "Button"));
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
    public void Parse_DoesNotTreatCustomNamespaceControlsAsAvaloniaCapabilities()
    {
        const string axaml = """
            <Window xmlns="https://github.com/avaloniaui"
                    xmlns:custom="urn:sample">
              <custom:StackPanel>
                <custom:Button Content="Not Avalonia" />
              </custom:StackPanel>
            </Window>
            """;

        AxamlPreviewNode root = new AxamlPreviewService().Parse(axaml);
        AxamlPreviewNode customPanel = root;

        Assert.AreEqual("StackPanel", customPanel.TypeName);
        Assert.IsFalse(customPanel.IsSupported);
        Assert.IsFalse(customPanel.Children.Single().IsSupported);
        Assert.IsFalse(new AxamlPreviewService().CanEditProperty(customPanel.Children.Single(), "Content"));
        AxamlDesignerSession session = new();
        session.Load(axaml);
        session.SelectNode(customPanel);
        Assert.ThrowsExactly<System.InvalidOperationException>(() => session.AddChildControl("Button"));
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
    public void ParseForPreview_RecoversValidControlsAroundMalformedElement()
    {
        const string axaml =
            "<?xml version='1.0'?><StackPanel xmlns='https://github.com/avaloniaui'><TextBlock Text='Keep this' /><Button Content=broken /><Label Content='Keep this too' /></StackPanel>";

        AxamlPreviewParseResult result = new AxamlPreviewService().ParseForPreview(axaml);

        Assert.IsNotNull(result.Root);
        Assert.IsTrue(result.Root.IsRecoveryRoot);
        Assert.AreEqual(3, result.Root.Children.Count);
        Assert.AreEqual("Keep this", result.Root.Children[0].Text);
        Assert.IsFalse(result.Root.Children[1].IsSupported);
        StringAssert.Contains(result.Root.Children[1].Diagnostic, "Could not preview <Button>");
        Assert.AreEqual("Keep this too", result.Root.Children[2].Text);
        Assert.IsTrue(result.Root.Children.Where(static node => node.TypeName != "Invalid AXAML")
            .All(static node => node.SourceStartOffset < 0));
        StringAssert.Contains(result.Diagnostic, "Partial preview");
    }

    [TestMethod]
    public void Parse_RepresentsViewboxWithSingleChildAndStretchCapability()
    {
        const string markup =
            "<Viewbox xmlns='https://github.com/avaloniaui' Stretch='UniformToFill'><TextBlock Text='Scale me' /></Viewbox>";
        AxamlPreviewService service = new();
        AxamlPreviewNode root = service.Parse(markup);

        Assert.AreEqual("Viewbox", root.TypeName);
        Assert.IsTrue(root.IsSupported);
        Assert.AreEqual("Scale me", root.Children.Single().Text);
        Assert.IsTrue(service.CanEditProperty(root, "Stretch"));
        Assert.IsFalse(root.CanAppendChildren);
        Assert.ThrowsExactly<System.InvalidOperationException>(() =>
            service.InsertChildControl(markup, root, "Button"));
    }

    [TestMethod]
    public void Parse_RepresentsUniformGridPropertiesAndSupportsSafeAppend()
    {
        const string markup =
            "<UniformGrid xmlns='https://github.com/avaloniaui' Rows='2' Columns='3' FirstColumn='1'><TextBlock Text='First' /></UniformGrid>";
        AxamlPreviewService service = new();
        AxamlPreviewNode root = service.Parse(markup);

        Assert.AreEqual("UniformGrid", root.TypeName);
        Assert.IsTrue(root.IsSupported);
        Assert.AreEqual("2", root.Properties["Rows"]);
        Assert.AreEqual("3", root.Properties["Columns"]);
        Assert.AreEqual("1", root.Properties["FirstColumn"]);
        Assert.IsTrue(root.CanAppendChildren);
        Assert.IsTrue(service.CanEditProperty(root, "Rows"));

        string updated = service.InsertChildControl(markup, root, "Button");
        AxamlPreviewNode updatedRoot = service.Parse(updated);
        Assert.AreEqual(2, updatedRoot.Children.Count);
        Assert.AreEqual("First", updatedRoot.Children[0].Text);
        Assert.AreEqual("Button", updatedRoot.Children[1].TypeName);
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

    [TestMethod]
    public void UpdateAttribute_EmptyValueRemovesTheAttributeInsteadOfWritingAnInvalidLiteral()
    {
        const string markup = "<StackPanel   Width='120' Height=\"80\" />";
        AxamlPreviewService service = new();
        AxamlPreviewNode node = service.Parse(markup);

        string updated = service.UpdateAttribute(markup, node, "Width", string.Empty);

        Assert.AreEqual("<StackPanel    Height=\"80\" />", updated);
        AxamlPreviewNode withoutWidth = service.Parse(updated);
        Assert.AreEqual(updated, service.UpdateAttribute(updated, withoutWidth, "Width", string.Empty));
    }

    [TestMethod]
    public void UpdateBinding_OnlyWritesSimpleMemberPathsToCataloguedBindableProperties()
    {
        AxamlPreviewService service = new();
        const string markup = "<Button xmlns='https://github.com/avaloniaui' Command='{Binding OldCommand}' />";
        AxamlPreviewNode node = service.Parse(markup);

        string updated = service.UpdateBinding(markup, node, "Command", "SaveCommand");

        StringAssert.Contains(updated, "Command='{Binding SaveCommand}'");
        Assert.ThrowsExactly<InvalidOperationException>(() =>
            service.UpdateBinding(markup, node, "Command", "SaveCommand.Execute"));
        Assert.ThrowsExactly<InvalidOperationException>(() =>
            service.UpdateBinding(markup, node, "Width", "PanelWidth"));
        Assert.ThrowsExactly<InvalidOperationException>(() =>
            service.UpdateBinding(markup, node, "Command", "Bad Command"));
    }

    [TestMethod]
    public void InsertChildControl_AppendsToSupportedPanelsAndMatchesNestedClosingTags()
    {
        const string markup = "<StackPanel>\r\n  <StackPanel />\r\n</StackPanel>";
        AxamlPreviewService service = new();
        AxamlPreviewNode root = service.Parse(markup);

        string updated = service.InsertChildControl(markup, root, "Button");

        Assert.AreEqual(
            "<StackPanel>\r\n  <StackPanel />\r\n  <Button />\r\n</StackPanel>",
            updated);
        Assert.AreEqual(2, service.Parse(updated).Children.Count);
    }

    [TestMethod]
    public void InsertChildControl_AppendsToInlinePanelAndPreservesExistingMarkup()
    {
        const string markup = "<av:DockPanel xmlns:av='https://github.com/avaloniaui' LastChildFill='False'><av:TextBlock Text='keep' /></av:DockPanel>";
        AxamlPreviewService service = new();
        AxamlPreviewNode root = service.Parse(markup);

        string updated = service.InsertChildControl(markup, root, "Button", AxamlDockPanelPlacement.Bottom);

        StringAssert.Contains(
            updated,
            "<av:TextBlock Text='keep' />\n  <av:Button DockPanel.Dock=\"Bottom\" />\n</av:DockPanel>");
        AxamlPreviewNode updatedRoot = service.Parse(updated);
        Assert.AreEqual("keep", updatedRoot.Children[0].Text);
        Assert.AreEqual("Button", updatedRoot.Children[1].TypeName);
    }

    [TestMethod]
    public void InsertChildControl_InsertsDockedChildrenBeforeTheClientChild()
    {
        AxamlPreviewService service = new();
        const string empty = "<DockPanel />";
        AxamlPreviewNode emptyPanel = service.Parse(empty);
        string withClient = service.InsertChildControl(
            empty,
            emptyPanel,
            "TextBlock",
            AxamlDockPanelPlacement.Client);
        AxamlPreviewNode clientPanel = service.Parse(withClient);

        string withTop = service.InsertChildControl(
            withClient,
            clientPanel,
            "Button",
            AxamlDockPanelPlacement.Top);
        AxamlPreviewNode topPanel = service.Parse(withTop);
        string withBottom = service.InsertChildControl(
            withTop,
            topPanel,
            "Separator",
            AxamlDockPanelPlacement.Bottom);

        StringAssert.Contains(
            withBottom,
            "<Button DockPanel.Dock=\"Top\" />\n  <Separator DockPanel.Dock=\"Bottom\" />\n  <TextBlock />");
        AxamlPreviewNode result = service.Parse(withBottom);
        Assert.AreEqual("Button", result.Children[0].TypeName);
        Assert.AreEqual("Top", result.Children[0].Properties["DockPanel.Dock"]);
        Assert.AreEqual("Bottom", result.Children[1].Properties["DockPanel.Dock"]);
        Assert.AreEqual("TextBlock", result.Children[2].TypeName);
        Assert.IsTrue(result.CanAppendChildren);
    }

    [TestMethod]
    public void InsertChildControl_RejectsInvalidClientPlacementRequests()
    {
        AxamlPreviewService service = new();
        const string empty = "<DockPanel />";
        AxamlPreviewNode emptyPanel = service.Parse(empty);
        Assert.ThrowsExactly<System.InvalidOperationException>(() =>
            service.InsertChildControl(empty, emptyPanel, "Button", AxamlDockPanelPlacement.Top));

        const string withClient = "<DockPanel><TextBlock /></DockPanel>";
        AxamlPreviewNode clientPanel = service.Parse(withClient);
        Assert.ThrowsExactly<System.InvalidOperationException>(() =>
            service.InsertChildControl(withClient, clientPanel, "Button", AxamlDockPanelPlacement.Client));
        Assert.ThrowsExactly<System.InvalidOperationException>(() =>
            service.InsertChildControl(withClient, clientPanel, "Button"));

        const string noFill = "<DockPanel LastChildFill='False'><TextBlock /></DockPanel>";
        AxamlPreviewNode noFillPanel = service.Parse(noFill);
        Assert.ThrowsExactly<System.InvalidOperationException>(() =>
            service.InsertChildControl(noFill, noFillPanel, "Button", AxamlDockPanelPlacement.Client));
        StringAssert.Contains(
            service.InsertChildControl(noFill, noFillPanel, "Button", AxamlDockPanelPlacement.Bottom),
            "<Button DockPanel.Dock=\"Bottom\" />");
    }

    [TestMethod]
    public void InsertChildControl_RejectsAmbiguousExistingContent()
    {
        AxamlPreviewService service = new();
        AxamlPreviewNode grid = service.Parse("<Grid><TextBlock /></Grid>");
        const string propertyMarkup =
            "<StackPanel><StackPanel.Resources><ResourceDictionary /></StackPanel.Resources></StackPanel>";
        AxamlPreviewNode panelWithPropertyElement = service.Parse(propertyMarkup);
        const string unknownMarkup = "<StackPanel><legacy /></StackPanel>";
        AxamlPreviewNode panelWithUnknownContent = service.Parse(unknownMarkup);
        const string inlineTextMarkup = "<StackPanel>unmodeled text<TextBlock /></StackPanel>";
        AxamlPreviewNode panelWithInlineText = service.Parse(inlineTextMarkup);
        const string customMarkup =
            "<StackPanel xmlns='https://github.com/avaloniaui' xmlns:custom='urn:sample'><custom:Button /></StackPanel>";
        AxamlPreviewNode panelWithCustomControl = service.Parse(customMarkup);

        Assert.ThrowsExactly<System.InvalidOperationException>(() =>
            service.InsertChildControl("<Grid><TextBlock /></Grid>", grid, "Button"));
        Assert.ThrowsExactly<System.InvalidOperationException>(() =>
            service.InsertChildControl(propertyMarkup, panelWithPropertyElement, "Button"));
        Assert.ThrowsExactly<System.InvalidOperationException>(() =>
            service.InsertChildControl(unknownMarkup, panelWithUnknownContent, "Button"));
        Assert.IsFalse(panelWithInlineText.CanAppendChildren);
        System.InvalidOperationException inlineTextException = Assert.ThrowsExactly<System.InvalidOperationException>(() =>
            service.InsertChildControl(inlineTextMarkup, panelWithInlineText, "Button"));
        StringAssert.Contains(inlineTextException.Message, "Correct invalid inline text");
        Assert.IsFalse(panelWithCustomControl.CanAppendChildren);
        Assert.ThrowsExactly<System.InvalidOperationException>(() =>
            service.InsertChildControl(customMarkup, panelWithCustomControl, "Button"));
    }

    [TestMethod]
    public void InsertChildControl_IgnoresCommentTextWhenFindingEmptyContainerClosingTag()
    {
        const string markup = "<StackPanel><!-- fake closing tag: </StackPanel> --></StackPanel>";
        AxamlPreviewService service = new();
        AxamlPreviewNode root = service.Parse(markup);

        string updated = service.InsertChildControl(markup, root, "Button");

        StringAssert.Contains(updated, "<!-- fake closing tag: </StackPanel> -->");
        Assert.AreEqual("Button", service.Parse(updated).Children.Single().TypeName);
    }

    [TestMethod]
    public void InsertChildControl_AddsGridChildToNextUnoccupiedExplicitCell()
    {
        const string markup = """
            <Grid RowDefinitions="Auto,*" ColumnDefinitions="*,*">
              <TextBlock Grid.Row="0" Grid.Column="0" />
              <TextBox Grid.Row="0" Grid.Column="1" />
            </Grid>
            """;
        AxamlPreviewService service = new();
        AxamlPreviewNode grid = service.Parse(markup);
        Assert.IsTrue(grid.CanAddGridChild);

        string updated = service.InsertChildControl(markup, grid, "Button");

        StringAssert.Contains(updated, "<Button Grid.Row=\"1\" Grid.Column=\"0\" />");
        AxamlPreviewNode result = service.Parse(updated);
        Assert.AreEqual(3, result.Children.Count);
        Assert.AreEqual("1", result.Children[2].Properties["Grid.Row"]);
        Assert.AreEqual("0", result.Children[2].Properties["Grid.Column"]);
    }

    [TestMethod]
    public void InsertChildControl_AddsGridChildWhenDefinitionsUsePropertyElements()
    {
        const string markup = """
            <Grid xmlns="https://github.com/avaloniaui">
              <Grid.RowDefinitions>
                <RowDefinition Height="Auto" />
                <RowDefinition Height="*" />
              </Grid.RowDefinitions>
              <Grid.ColumnDefinitions>
                <ColumnDefinition Width="*" />
                <ColumnDefinition Width="*" />
              </Grid.ColumnDefinitions>
              <TextBlock Grid.Row="0" Grid.Column="0" />
            </Grid>
            """;
        AxamlPreviewService service = new();
        AxamlPreviewNode grid = service.Parse(markup);
        Assert.IsTrue(grid.CanAddGridChild);

        string updated = service.InsertChildControl(markup, grid, "Button");

        StringAssert.Contains(updated, "<Button Grid.Row=\"0\" Grid.Column=\"1\" />");
        AxamlPreviewNode result = service.Parse(updated);
        Assert.AreEqual(2, result.RowDefinitions.Count);
        Assert.AreEqual(2, result.ColumnDefinitions.Count);
        Assert.AreEqual(2, result.Children.Count);
    }

    [TestMethod]
    public void InsertChildControl_AccountsForExistingGridSpans()
    {
        const string markup = """
            <Grid RowDefinitions="*,*" ColumnDefinitions="*,*">
              <TextBlock Grid.Row="0" Grid.Column="0" Grid.ColumnSpan="2" />
            </Grid>
            """;
        AxamlPreviewService service = new();
        AxamlPreviewNode grid = service.Parse(markup);

        Assert.IsTrue(grid.CanAddGridChild);
        string updated = service.InsertChildControl(markup, grid, "Button");

        StringAssert.Contains(updated, "<Button Grid.Row=\"1\" Grid.Column=\"0\" />");
        Assert.AreEqual(2, service.Parse(updated).Children.Count);
    }

    [TestMethod]
    public void InsertChildControl_RejectsAmbiguousOrFullGridPlacements()
    {
        AxamlPreviewService service = new();
        const string duplicateCell = "<Grid><TextBlock /><Button Grid.Row='0' Grid.Column='0' /></Grid>";
        const string spanningChild = "<Grid RowDefinitions='*,*'><TextBlock Grid.RowSpan='2' /></Grid>";
        const string outOfRangeChild = "<Grid RowDefinitions='Auto'><TextBlock Grid.Row='1' /></Grid>";
        const string duplicateDefinitionSources =
            "<Grid RowDefinitions='*,*'><Grid.RowDefinitions><RowDefinition /><RowDefinition /></Grid.RowDefinitions></Grid>";
        const string malformedDefinitionProperty =
            "<Grid><Grid.RowDefinitions><ColumnDefinition /></Grid.RowDefinitions></Grid>";
        const string emptyDefinitionSlot = "<Grid RowDefinitions='Auto,,*' />";
        const string dynamicDefinitionList = "<Grid RowDefinitions='{Binding Rows}' />";
        const string invalidGridLength = "<Grid ColumnDefinitions='Auto,unknown' />";
        string oversizedGrid =
            $"<Grid RowDefinitions='{string.Join(',', Enumerable.Repeat("Auto", 317))}' " +
            $"ColumnDefinitions='{string.Join(',', Enumerable.Repeat("Auto", 317))}' />";
        const string overlappingSpan = """
            <Grid RowDefinitions="*,*" ColumnDefinitions="*,*">
              <TextBlock Grid.Row="0" Grid.Column="0" Grid.ColumnSpan="2" />
              <Button Grid.Row="0" Grid.Column="1" />
            </Grid>
            """;
        const string dynamicPropertyDefinition = """
            <Grid>
              <Grid.RowDefinitions><RowDefinition Height="{Binding Height}" /></Grid.RowDefinitions>
            </Grid>
            """;
        const string negativePropertyDefinition = """
            <Grid>
              <Grid.ColumnDefinitions><ColumnDefinition Width="-1" /></Grid.ColumnDefinitions>
            </Grid>
            """;
        const string namespacedGridPlacement =
            "<Grid xmlns:custom='urn:custom'><TextBlock custom:Grid.Row='0' /></Grid>";
        const string namespacedGridDimensions =
            "<Grid xmlns:custom='urn:custom' custom:RowDefinitions='Auto,Auto' />";
        const string namespacedDefinitionLength = """
            <Grid xmlns:custom="urn:custom">
              <Grid.RowDefinitions><RowDefinition custom:Height="*" /></Grid.RowDefinitions>
            </Grid>
            """;

        foreach (string markup in new[]
        {
            duplicateCell,
            overlappingSpan,
            spanningChild,
            outOfRangeChild,
            duplicateDefinitionSources,
            malformedDefinitionProperty,
            emptyDefinitionSlot,
            dynamicDefinitionList,
            invalidGridLength,
            oversizedGrid,
            dynamicPropertyDefinition,
            negativePropertyDefinition,
            namespacedGridPlacement,
            namespacedGridDimensions,
            namespacedDefinitionLength,
        })
        {
            AxamlPreviewNode grid = service.Parse(markup);
            Assert.IsFalse(grid.CanAddGridChild, markup);
            Assert.ThrowsExactly<System.InvalidOperationException>(() =>
                service.InsertChildControl(markup, grid, "Button"));
        }

        const string fullGrid = "<Grid><TextBlock /></Grid>";
        AxamlPreviewNode occupiedGrid = service.Parse(fullGrid);
        Assert.IsFalse(occupiedGrid.CanAddGridChild);
        Assert.ThrowsExactly<System.InvalidOperationException>(() =>
            service.InsertChildControl(fullGrid, occupiedGrid, "Button"));
    }
}
