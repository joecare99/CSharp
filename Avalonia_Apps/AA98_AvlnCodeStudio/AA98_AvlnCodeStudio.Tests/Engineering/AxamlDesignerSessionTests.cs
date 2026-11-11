using AA98_AvlnCodeStudio.Model.Workspaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.Tests.Engineering;

[TestClass]
public sealed class AxamlDesignerSessionTests
{
    [TestMethod]
    public void ApplySelectedValue_UpdatesLayoutAndSupportsUndoRedo()
    {
        AxamlDesignerSession session = new();
        session.Load("<StackPanel Orientation='Vertical'><TextBlock Text='Hello' /></StackPanel>");
        session.SelectNode(session.Root);
        session.SelectProperty("Orientation");

        Assert.IsTrue(session.ApplySelectedValue("Horizontal"));
        Assert.AreEqual("Horizontal", session.SelectedValue);
        Assert.IsTrue(session.CanUndo);
        Assert.IsTrue(session.Undo());
        Assert.AreEqual("Vertical", session.SelectedValue);
        Assert.IsTrue(session.CanRedo);
        Assert.IsTrue(session.Redo());
        Assert.AreEqual("Horizontal", session.SelectedValue);
        StringAssert.Contains(session.Markup, "Orientation='Horizontal'");
    }

    [TestMethod]
    public void Load_ExternalMarkupClearsHistoryAndRetainsOnlyMatchingSelection()
    {
        AxamlDesignerSession session = new();
        session.Load("<StackPanel><TextBlock Text='Hello' /></StackPanel>");
        session.SelectNode(session.Root!.Children[0]);
        session.SelectProperty("Text");
        session.ApplySelectedValue("Changed");
        Assert.IsTrue(session.CanUndo);

        session.Load("<StackPanel><TextBlock Text='External' /></StackPanel>");

        Assert.IsFalse(session.CanUndo);
        Assert.IsFalse(session.CanRedo);
        Assert.AreEqual("TextBlock", session.SelectedNode?.TypeName);
        Assert.AreEqual("External", session.SelectedValue);
    }

    [TestMethod]
    public void ResourceContextChange_PreservesUndoRedoSelectionAndUsesNewProjectForHistory()
    {
        string fixture = Path.Combine(Environment.CurrentDirectory, "designer-context-" + Guid.NewGuid().ToString("N"));
        try
        {
            string first = CreateResourceProject(fixture, "First", "First caption");
            string second = CreateResourceProject(fixture, "Second", "Second caption");
            string firstView = Path.Combine(first, "Views", "View.axaml");
            string secondView = Path.Combine(second, "Views", "View.axaml");
            const string markup = """
                <StackPanel xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' xmlns:p='clr-namespace:Sample.Properties'>
                  <Button Width='100' Content='{x:Static p:Resources.Caption}'/>
                </StackPanel>
                """;
            AxamlDesignerSession session = new();
            session.Load(markup);
            session.SelectNode(session.Root!.Children[0]);
            session.SelectProperty("Width");
            Assert.IsTrue(session.ApplySelectedValue("120"));
            Assert.IsTrue(session.ApplySelectedValue("140"));
            Assert.IsTrue(session.Undo());
            string editedMarkup = session.Markup;
            Assert.IsTrue(session.CanUndo);
            Assert.IsTrue(session.CanRedo);

            session.SetResourceContext(firstView, first);
            Assert.AreEqual(editedMarkup, session.Markup);
            Assert.AreEqual("First caption", session.SelectedNode!.Text);
            Assert.AreEqual("Width", session.SelectedProperty);
            Assert.AreEqual("120", session.SelectedValue);
            Assert.IsTrue(session.CanUndo);
            Assert.IsTrue(session.CanRedo);

            session.SetResourceContext(secondView, second);
            Assert.AreEqual("Second caption", session.SelectedNode!.Text);
            Assert.IsTrue(session.CanUndo);
            Assert.IsTrue(session.CanRedo);
            Assert.IsTrue(session.Undo());
            Assert.AreEqual("100", session.SelectedValue);
            Assert.AreEqual("Second caption", session.SelectedNode!.Text);
            Assert.IsTrue(session.Redo());
            Assert.AreEqual("120", session.SelectedValue);
            Assert.IsTrue(session.Redo());
            Assert.AreEqual("140", session.SelectedValue);
            Assert.AreEqual("Second caption", session.SelectedNode!.Text);

            session.SetResourceContext(null, null);
            Assert.AreEqual("{x:Static p:Resources.Caption}", session.SelectedNode!.Text);
            Assert.IsTrue(session.CanUndo);
        }
        finally
        {
            if (Directory.Exists(fixture))
            {
                Directory.Delete(fixture, true);
            }
        }
    }

    [TestMethod]
    public void ResourceContextChange_DoesNotSplitAnInProgressPropertyEdit()
    {
        AxamlDesignerSession session = new();
        session.Load("<StackPanel Width='100' />");
        session.SelectNode(session.Root);
        session.BeginPropertyEdit("Width");
        Assert.IsTrue(session.ApplySelectedValue("120"));
        session.SetResourceContext(null, "unavailable");
        Assert.IsTrue(session.ApplySelectedValue("140"));
        session.EndPropertyEdit();

        Assert.IsTrue(session.Undo());
        Assert.AreEqual("100", session.SelectedValue);
        Assert.IsFalse(session.CanUndo);
        Assert.IsTrue(session.Redo());
        Assert.AreEqual("140", session.SelectedValue);
    }

    private static string CreateResourceProject(string fixture, string folder, string caption)
    {
        string root = Path.Combine(fixture, folder);
        Directory.CreateDirectory(Path.Combine(root, "Properties"));
        Directory.CreateDirectory(Path.Combine(root, "Views"));
        File.WriteAllText(Path.Combine(root, "Sample.csproj"), "<Project />");
        File.WriteAllText(Path.Combine(root, "Views", "View.axaml"), "");
        File.WriteAllText(Path.Combine(root, "Properties", "Resources.resx"),
            $"<root><data name='Caption'><value>{caption}</value></data></root>");
        return root;
    }

    [TestMethod]
    public void ApplyValues_UpdatesMultipleLayoutPropertiesAsOneUndoableEdit()
    {
        AxamlDesignerSession session = new();
        session.Load("<StackPanel Width='120'><TextBlock Text='Hello' /></StackPanel>");
        session.SelectNode(session.Root);

        Assert.IsTrue(session.ApplyValues(
            new Dictionary<string, string>
            {
                ["Width"] = "240",
                ["Height"] = "80",
            },
            "Width"));

        StringAssert.Contains(session.Markup, "Width='240'");
        StringAssert.Contains(session.Markup, "Height=\"80\"");
        Assert.IsTrue(session.Undo());
        StringAssert.Contains(session.Markup, "Width='120'");
        Assert.IsFalse(session.Markup.Contains("Height=", System.StringComparison.Ordinal));
        Assert.IsTrue(session.Redo());
        StringAssert.Contains(session.Markup, "Width='240'");
        StringAssert.Contains(session.Markup, "Height=\"80\"");
    }

    [TestMethod]
    public void ApplyPropertyEdit_CoalescesDebouncedValuesIntoOneUndoStep()
    {
        AxamlDesignerSession session = new();
        session.Load("<StackPanel Width='100' />");
        session.SelectNode(session.Root);
        session.BeginPropertyEdit("Width");

        Assert.IsTrue(session.ApplySelectedValue("120"));
        Assert.IsTrue(session.ApplySelectedValue("140"));
        session.EndPropertyEdit();
        session.BeginPropertyEdit("Width");
        Assert.IsTrue(session.ApplySelectedValue("160"));
        session.EndPropertyEdit();

        StringAssert.Contains(session.Markup, "Width='160'");
        Assert.IsTrue(session.Undo());
        StringAssert.Contains(session.Markup, "Width='140'");
        Assert.IsTrue(session.Undo());
        StringAssert.Contains(session.Markup, "Width='100'");
        Assert.IsFalse(session.CanUndo);
        Assert.IsTrue(session.CanRedo);
    }

    [TestMethod]
    public void ApplySelectedValue_RejectsInvalidLiteralWithoutChangingMarkupOrHistory()
    {
        const string markup = "<StackPanel Width='100' />";
        AxamlDesignerSession session = new();
        session.Load(markup);
        session.SelectNode(session.Root);
        session.SelectProperty("Width");

        Assert.ThrowsExactly<System.InvalidOperationException>(() => session.ApplySelectedValue("-1"));

        Assert.AreEqual(markup, session.Markup);
        Assert.IsFalse(session.CanUndo);
    }

    [TestMethod]
    public void ApplyBinding_UpdatesBindablePropertyAndSupportsUndoRedo()
    {
        AxamlDesignerSession session = new();
        session.Load("<TextBox Text='{Binding ExistingText}' />");
        session.SelectNode(session.Root);

        Assert.IsTrue(session.ApplyBinding("Text", "NewText"));
        StringAssert.Contains(session.Markup, "Text='{Binding NewText}'");
        Assert.IsTrue(session.ApplyBinding(
            "Text",
            "NewText",
            AxamlBindingMode.TwoWay,
            AxamlBindingUpdateSourceTrigger.PropertyChanged));
        StringAssert.Contains(
            session.Markup,
            "Text='{Binding NewText, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}'");
        Assert.IsTrue(session.Undo());
        StringAssert.Contains(session.Markup, "Text='{Binding NewText}'");
        Assert.IsTrue(session.Undo());
        StringAssert.Contains(session.Markup, "Text='{Binding ExistingText}'");
        Assert.IsTrue(session.Redo());
        StringAssert.Contains(session.Markup, "Text='{Binding NewText}'");
        Assert.IsTrue(session.Redo());
        StringAssert.Contains(
            session.Markup,
            "Text='{Binding NewText, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}'");
    }

    [TestMethod]
    public void ApplyValues_RejectsPropertyElementsWithoutChangingMarkup()
    {
        const string markup = "<StackPanel xmlns='https://github.com/avaloniaui'><StackPanel.Width>120</StackPanel.Width></StackPanel>";
        AxamlDesignerSession session = new();
        session.Load(markup);
        session.SelectNode(session.Root);

        Assert.ThrowsExactly<System.InvalidOperationException>(() => session.ApplyValues(
            new Dictionary<string, string>
            {
                ["Width"] = "240",
                ["Height"] = "80",
            },
            "Width"));

        Assert.AreEqual(markup, session.Markup);
        Assert.IsFalse(session.CanUndo);
    }

    [TestMethod]
    public void AddChildControl_UpdatesEmptyContainerAndSupportsUndoRedo()
    {
        AxamlDesignerSession session = new();
        session.Load("<StackPanel />");
        session.SelectNode(session.Root);

        Assert.IsTrue(session.AddChildControl("TextBlock"));
        StringAssert.Contains(session.Markup, "<StackPanel>");
        StringAssert.Contains(session.Markup, "<TextBlock />");
        StringAssert.Contains(session.Markup, "</StackPanel>");
        Assert.IsTrue(session.CanUndo);
        Assert.IsTrue(session.Undo());
        Assert.AreEqual("<StackPanel />", session.Markup);
        Assert.IsTrue(session.Redo());
        StringAssert.Contains(session.Markup, "<TextBlock />");
    }

    [TestMethod]
    public void AddChildControl_PreservesPrefixAndAppendsToEmptyExplicitContainer()
    {
        const string markup = "<av:Grid xmlns:av='https://github.com/avaloniaui'></av:Grid>";
        AxamlDesignerSession session = new();
        session.Load(markup);
        session.SelectNode(session.Root);

        Assert.IsTrue(session.AddChildControl("Button"));

        StringAssert.Contains(session.Markup, "<av:Button av:Grid.Row=\"0\" av:Grid.Column=\"0\" />");
        Assert.AreEqual("0", session.Root!.Children.Single().Properties["Grid.Row"]);
        Assert.AreEqual("0", session.Root.Children.Single().Properties["Grid.Column"]);
        Assert.IsTrue(session.Markup.StartsWith("<av:Grid", System.StringComparison.Ordinal));
        Assert.IsTrue(session.Markup.EndsWith("</av:Grid>", System.StringComparison.Ordinal));
    }

    [TestMethod]
    public void AddChildControl_CanvasPositionsChildrenAndSupportsUndoRedo()
    {
        AxamlDesignerSession session = new();
        session.Load("<av:Canvas xmlns:av='https://github.com/avaloniaui' />");
        session.SelectNode(session.Root);

        Assert.IsTrue(session.AddChildControl("Button"));
        Assert.IsTrue(session.AddChildControl("TextBlock"));

        StringAssert.Contains(session.Markup, "<av:Button av:Canvas.Left=\"0\" av:Canvas.Top=\"0\" />");
        StringAssert.Contains(session.Markup, "<av:TextBlock av:Canvas.Left=\"24\" av:Canvas.Top=\"24\" />");
        Assert.AreEqual("Canvas", session.Root!.Children[0].ParentTypeName);
        Assert.AreEqual("24", session.Root.Children[1].Properties["Canvas.Left"]);
        Assert.IsTrue(session.Undo());
        Assert.AreEqual(1, session.Root!.Children.Count);
        Assert.IsTrue(session.Undo());
        Assert.AreEqual("<av:Canvas xmlns:av='https://github.com/avaloniaui' />", session.Markup);
        Assert.IsTrue(session.Redo());
        Assert.IsTrue(session.Redo());
        Assert.AreEqual(2, session.Root!.Children.Count);
    }

    [TestMethod]
    public void AddChildControl_RejectsNonContainersExistingContentAndUnknownControls()
    {
        AxamlDesignerSession session = new();
        session.Load("<TextBlock />");
        session.SelectNode(session.Root);
        Assert.ThrowsExactly<System.InvalidOperationException>(() => session.AddChildControl("Button"));
        Assert.ThrowsExactly<System.InvalidOperationException>(() => session.AddChildControl("CustomControl"));

        session.Load("<StackPanel><TextBlock /></StackPanel>");
        session.SelectNode(session.Root);
        Assert.IsTrue(session.AddChildControl("Button"));
        Assert.AreEqual(2, session.Root!.Children.Count);
        Assert.IsTrue(session.Undo());
        Assert.AreEqual(1, session.Root!.Children.Count);
    }

    [TestMethod]
    public void AddChildControl_BuildsNestedContainersAsSeparateUndoUnits()
    {
        AxamlDesignerSession session = new();
        session.Load("<StackPanel />");
        session.SelectNode(session.Root);

        Assert.IsTrue(session.AddChildControl("DockPanel"));
        AxamlPreviewNode dockPanel = session.Root!.Children.Single();
        session.SelectNode(dockPanel);
        Assert.IsTrue(session.AddChildControl("Button", AxamlDockPanelPlacement.Client));

        Assert.AreEqual("Button", session.Root.Children.Single().Children.Single().TypeName);
        Assert.IsTrue(session.Undo());
        Assert.AreEqual("DockPanel", session.Root!.Children.Single().TypeName);
        Assert.AreEqual(0, session.Root.Children.Single().Children.Count);
        Assert.IsTrue(session.Undo());
        Assert.AreEqual(0, session.Root!.Children.Count);
    }

    [TestMethod]
    public void AddChildControl_AppendsAndUndoesAsOneSourcePreservingEdit()
    {
        const string markup = "<StackPanel>\r\n  <TextBlock Text='existing' />\r\n</StackPanel>";
        AxamlDesignerSession session = new();
        session.Load(markup);
        session.SelectNode(session.Root);

        Assert.IsTrue(session.AddChildControl("Button"));
        StringAssert.Contains(session.Markup, "<TextBlock Text='existing' />\r\n  <Button />");
        Assert.AreEqual("existing", session.Root!.Children[0].Text);
        Assert.IsTrue(session.Undo());
        Assert.AreEqual(markup, session.Markup);
        Assert.IsTrue(session.Redo());
        Assert.AreEqual(2, session.Root!.Children.Count);
    }

    [TestMethod]
    public void RemoveSelectedControl_PreservesSourceAndRestoresSelectionThroughUndoRedo()
    {
        const string markup = "<av:StackPanel xmlns:av='https://github.com/avaloniaui'>\r\n"
            + "  <av:TextBlock Text='Keep' />\r\n"
            + "  <av:Button Content='Remove' />\r\n"
            + "</av:StackPanel>";
        AxamlDesignerSession session = new();
        session.Load(markup);
        session.SelectNode(session.Root!.Children[1]);

        Assert.IsTrue(session.CanRemoveSelectedControl);
        Assert.IsTrue(session.RemoveSelectedControl());
        Assert.AreEqual(markup.Replace("  <av:Button Content='Remove' />\r\n", string.Empty), session.Markup);
        Assert.AreEqual("StackPanel", session.SelectedNode!.TypeName);
        Assert.IsTrue(session.Undo());
        Assert.AreEqual(markup, session.Markup);
        Assert.AreEqual("Button", session.SelectedNode!.TypeName);
        Assert.IsFalse(session.CanUndo);
        Assert.IsTrue(session.Redo());
        Assert.AreEqual("StackPanel", session.SelectedNode!.TypeName);
        Assert.AreEqual(1, session.Root!.Children.Count);
    }

    [TestMethod]
    public void MoveSelectedControl_PreservesNestedSourceAndTracksIdenticalSiblingTypes()
    {
        const string markup = "<StackPanel xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>\n"
            + "  <StackPanel><StackPanel><Button /></StackPanel></StackPanel>\n"
            + "  <TextBox x:Name='First' />\n"
            + "  <TextBox x:Name='Second' />\n"
            + "</StackPanel>";
        AxamlDesignerSession session = new();
        session.Load(markup);
        session.SelectNode(session.Root!.Children[2]);

        Assert.IsTrue(session.CanMoveSelectedControlEarlier);
        Assert.IsFalse(session.CanMoveSelectedControlLater);
        Assert.IsTrue(session.MoveSelectedControl(-1));
        Assert.AreEqual("Second", session.SelectedNode!.StableName);
        Assert.AreEqual("Second", session.Root!.Children[1].StableName);
        Assert.AreEqual("First", session.Root.Children[2].StableName);
        string moved = session.Markup;
        Assert.IsTrue(session.Undo());
        Assert.AreEqual(markup, session.Markup);
        Assert.AreEqual("Second", session.SelectedNode!.StableName);
        Assert.IsTrue(session.Redo());
        Assert.AreEqual(moved, session.Markup);
        Assert.AreEqual("Second", session.SelectedNode!.StableName);
        Assert.IsTrue(session.MoveSelectedControl(-1));
        Assert.AreEqual("Second", session.Root!.Children[0].StableName);
        Assert.IsTrue(session.MoveSelectedControl(1));
        Assert.AreEqual(moved, session.Markup);
        Assert.AreEqual("Second", session.SelectedNode!.StableName);
    }

    [TestMethod]
    public void StructureEditing_WorksInEachSupportedPanelAndPreservesNestedCommentsWhenMoving()
    {
        foreach (string panel in new[] { "StackPanel", "WrapPanel", "UniformGrid" })
        {
            string markup = $"<{panel}><StackPanel><!-- keep <StackPanel> -->"
                + $"<Button /></StackPanel><TextBlock /></{panel}>";
            AxamlDesignerSession session = new();
            session.Load(markup);
            session.SelectNode(session.Root!.Children[0]);

            Assert.IsTrue(session.MoveSelectedControl(1), panel);
            Assert.AreEqual("StackPanel", session.SelectedNode!.TypeName);
            StringAssert.Contains(session.Markup, "<!-- keep <StackPanel> -->");
            Assert.IsTrue(session.Undo());
            Assert.AreEqual(markup, session.Markup);
            session.SelectNode(session.Root!.Children[1]);
            Assert.IsTrue(session.RemoveSelectedControl(), panel);
            Assert.AreEqual($"<{panel}><StackPanel><!-- keep <StackPanel> -->"
                + $"<Button /></StackPanel></{panel}>", session.Markup);
            Assert.IsTrue(session.Undo());
            Assert.AreEqual(markup, session.Markup);
        }
    }

    [TestMethod]
    public void StructureEditing_WorksForCataloguedMenuAndTabContainers()
    {
        string[] markupCases =
        [
            "<Menu><MenuItem Header='First' /><Separator /><MenuItem Header='Second' /></Menu>",
            "<MenuItem Header='Root'><MenuItem Header='First' /><MenuItem Header='Second' /></MenuItem>",
            "<ContextMenu><MenuItem Header='First' /><Separator /></ContextMenu>",
            "<TabControl><TabItem Header='First' /><TabItem Header='Second' /></TabControl>",
        ];

        foreach (string markup in markupCases)
        {
            AxamlDesignerSession session = new();
            session.Load(markup);
            AxamlPreviewNode firstChild = session.Root!.Children[0];
            session.SelectNode(firstChild);

            Assert.IsTrue(session.CanMoveSelectedControlLater, markup);
            Assert.IsTrue(session.MoveSelectedControl(1), markup);
            Assert.AreEqual(firstChild.TypeName, session.SelectedNode!.TypeName, markup);
            Assert.IsTrue(session.Undo(), markup);
            Assert.IsTrue(session.CanRemoveSelectedControl, markup);
            Assert.IsTrue(session.RemoveSelectedControl(), markup);
            Assert.IsTrue(session.Undo(), markup);
            Assert.AreEqual(markup, session.Markup, markup);
        }
    }

    [TestMethod]
    public void StructureEditing_RemovesOnlySupportedDirectContentFromSingleChildContainers()
    {
        foreach (string container in new[] { "Border", "ContentControl", "Expander", "GroupBox", "ScrollViewer", "Viewbox" })
        {
            string markup = $"<{container}><Button /></{container}>";
            AxamlDesignerSession session = new();
            session.Load(markup);
            session.SelectNode(session.Root!.Children.Single());

            Assert.IsTrue(session.CanRemoveSelectedControl, container);
            Assert.IsFalse(session.CanMoveSelectedControlEarlier, container);
            Assert.IsFalse(session.CanMoveSelectedControlLater, container);
            Assert.IsTrue(session.RemoveSelectedControl(), container);
            Assert.AreEqual($"<{container}></{container}>", session.Markup, container);
            Assert.IsTrue(session.Undo(), container);
            Assert.AreEqual(markup, session.Markup, container);
            Assert.IsTrue(session.Redo(), container);
            Assert.AreEqual($"<{container}></{container}>", session.Markup, container);
        }

        AxamlDesignerSession propertyContent = new();
        propertyContent.Load("<Border><Border.Child><Button /></Border.Child></Border>");
        propertyContent.SelectNode(propertyContent.Root!.Children.Single());
        Assert.IsFalse(propertyContent.CanRemoveSelectedControl);
        Assert.ThrowsExactly<System.InvalidOperationException>(() => propertyContent.RemoveSelectedControl());

        AxamlDesignerSession collapsedContent = new();
        collapsedContent.Load("<Border><UserControl><Button /></UserControl></Border>");
        collapsedContent.SelectNode(collapsedContent.Root!.Children.Single());
        Assert.IsFalse(collapsedContent.CanRemoveSelectedControl);
        Assert.ThrowsExactly<System.InvalidOperationException>(() => collapsedContent.RemoveSelectedControl());
    }

    [TestMethod]
    public void ToolTipContent_CanBeAddedRemovedAndRestoredThroughUndoRedo()
    {
        const string markup = "<Window><Button><Button.ToolTip><ToolTip /></Button.ToolTip></Button></Window>";
        AxamlDesignerSession session = new();
        session.Load(markup);
        AxamlPreviewNode toolTip = session.Root!.Children.Single();

        Assert.IsTrue(toolTip.CanAppendChildren);
        session.SelectNode(toolTip);
        Assert.IsTrue(session.AddChildControl("TextBlock"));
        Assert.AreEqual("TextBlock", session.SelectedNode!.Children.Single().TypeName);
        Assert.IsTrue(session.Undo());
        Assert.AreEqual(markup, session.Markup);
        Assert.IsTrue(session.Redo());
        Assert.AreEqual("TextBlock", session.SelectedNode!.Children.Single().TypeName);

        AxamlPreviewNode textBlock = session.SelectedNode.Children.Single();
        session.SelectNode(textBlock);
        Assert.IsTrue(session.CanRemoveSelectedControl);
        Assert.IsTrue(session.RemoveSelectedControl());
        Assert.AreEqual("ToolTip", session.SelectedNode!.TypeName);
        Assert.AreEqual(0, session.SelectedNode.Children.Count);
        Assert.IsTrue(session.SelectedNode!.CanAppendChildren);
        Assert.IsTrue(session.Undo());
        Assert.AreEqual("TextBlock", session.SelectedNode!.TypeName);
        Assert.IsTrue(session.Redo());
        Assert.AreEqual("ToolTip", session.SelectedNode!.TypeName);
        Assert.AreEqual(0, session.SelectedNode!.Children.Count);

        Assert.IsTrue(session.AddChildControl("Border"));
        AxamlPreviewNode border = session.SelectedNode!.Children.Single();
        session.SelectNode(border);
        Assert.IsTrue(session.AddChildControl("StackPanel"));
        AxamlPreviewNode stackPanel = session.SelectedNode!.Children.Single();
        session.SelectNode(stackPanel);
        Assert.IsTrue(session.AddChildControl("TextBlock"));
        Assert.IsTrue(session.AddChildControl("TextBlock"));
        Assert.AreEqual(2, session.SelectedNode!.Children.Count);
        Assert.IsTrue(session.Root!.IsSupported);
        Assert.IsTrue(session.Undo());
        Assert.AreEqual(1, session.SelectedNode!.Children.Count);
        Assert.IsTrue(session.Undo());
        Assert.AreEqual(0, session.SelectedNode!.Children.Count);
        Assert.IsTrue(session.Undo());
        Assert.AreEqual("Border", session.SelectedNode!.TypeName);
        Assert.AreEqual(0, session.SelectedNode.Children.Count);
        Assert.IsTrue(session.Undo());
        Assert.AreEqual("ToolTip", session.SelectedNode!.TypeName);
        Assert.AreEqual(0, session.SelectedNode.Children.Count);
    }

    [TestMethod]
    public void TreeViewStructure_CanBeBuiltAndUndoneOneLevelAtATime()
    {
        AxamlDesignerSession session = new();
        session.Load("<TreeView />");
        AxamlPreviewNode tree = session.Root!;
        session.SelectNode(tree);

        Assert.IsTrue(session.AddChildControl("TreeViewItem"));
        AxamlPreviewNode project = session.Root!.Children.Single();
        session.SelectNode(project);
        Assert.IsTrue(session.AddChildControl("TreeViewItem"));
        Assert.AreEqual(1, session.Root!.Children.Single().Children.Count);
        Assert.IsTrue(session.Root!.IsSupported);
        Assert.ThrowsExactly<System.InvalidOperationException>(() => session.AddChildControl("Button"));

        Assert.IsTrue(session.Undo());
        Assert.AreEqual(0, session.Root!.Children.Single().Children.Count);
        Assert.IsTrue(session.Undo());
        Assert.AreEqual(0, session.Root!.Children.Count);
        Assert.AreEqual("<TreeView />", session.Markup);
        Assert.IsTrue(session.Redo());
        Assert.AreEqual(1, session.Root!.Children.Count);
    }

    [TestMethod]
    public void TreeViewItems_CanBeReorderedAndRemovedWithUndoRedo()
    {
        const string markup = "<TreeView><TreeViewItem Header='Project'>"
            + "<TreeViewItem Header='src' /><TreeViewItem Header='tests' />"
            + "</TreeViewItem></TreeView>";
        AxamlDesignerSession session = new();
        session.Load(markup);
        AxamlPreviewNode project = session.Root!.Children.Single();
        session.SelectNode(project.Children[1]);

        Assert.IsTrue(session.CanMoveSelectedControlEarlier);
        Assert.IsTrue(session.MoveSelectedControl(-1));
        CollectionAssert.AreEqual(
            new[] { "tests", "src" },
            session.Root!.Children.Single().Children.Select(
                static item => item.Properties["Header"]).ToArray());
        Assert.IsTrue(session.Undo());
        Assert.AreEqual(markup, session.Markup);
        Assert.IsTrue(session.Redo());

        session.SelectNode(session.Root!.Children.Single().Children[0]);
        Assert.IsTrue(session.CanRemoveSelectedControl);
        Assert.IsTrue(session.RemoveSelectedControl());
        Assert.AreEqual("src", session.Root!.Children.Single().Children.Single().Properties["Header"]);
        Assert.IsTrue(session.Undo());
        Assert.AreEqual("tests", session.Root!.Children.Single().Children[0].Properties["Header"]);
        Assert.IsTrue(session.Redo());
        Assert.AreEqual("src", session.Root!.Children.Single().Children.Single().Properties["Header"]);

        AxamlDesignerSession singleItem = new();
        singleItem.Load("<TreeView><TreeViewItem Header='Only' /></TreeView>");
        singleItem.SelectNode(singleItem.Root!.Children.Single());
        Assert.IsTrue(singleItem.CanRemoveSelectedControl);
        Assert.IsTrue(singleItem.RemoveSelectedControl());
        Assert.AreEqual(0, singleItem.Root!.Children.Count);
        Assert.IsTrue(singleItem.Undo());
        Assert.AreEqual("Only", singleItem.Root!.Children.Single().Properties["Header"]);
    }

    [TestMethod]
    public void SelectorItems_CanBeAddedEditedAndUndoneIndividually()
    {
        AxamlDesignerSession session = new();
        session.Load("<ComboBox />");
        session.SelectNode(session.Root);

        Assert.IsTrue(session.AddChildControl("ComboBoxItem"));
        AxamlPreviewNode item = session.Root!.Children.Single();
        Assert.IsTrue(item.IsSupported);
        session.SelectNode(item);
        session.SelectProperty("Content");
        Assert.IsTrue(session.ApplySelectedValue("Debug"));
        Assert.IsTrue(session.Markup.Contains("Content=\"Debug\"", StringComparison.Ordinal));

        Assert.IsTrue(session.Undo());
        Assert.AreEqual(0, session.Root!.Children.Single().Properties.Count);
        Assert.IsTrue(session.Undo());
        Assert.AreEqual(0, session.Root!.Children.Count);
        Assert.AreEqual("<ComboBox />", session.Markup);

        Assert.IsTrue(session.Redo());
        Assert.AreEqual("ComboBoxItem", session.Root!.Children.Single().TypeName);
        Assert.IsTrue(session.Redo());
        Assert.AreEqual("Debug", session.Root!.Children.Single().Properties["Content"]);

        session.SelectNode(session.Root.Children.Single());
        Assert.IsTrue(session.CanRemoveSelectedControl);
        Assert.IsTrue(session.RemoveSelectedControl());
        Assert.AreEqual(0, session.Root!.Children.Count);
        Assert.IsTrue(session.Undo());
        Assert.AreEqual("Debug", session.Root!.Children.Single().Properties["Content"]);
        Assert.IsTrue(session.Redo());
        Assert.AreEqual(0, session.Root!.Children.Count);
    }

    [TestMethod]
    public void SelectorItems_CanBeReorderedWithUndoRedo()
    {
        foreach ((string selectorType, string itemType) in new[]
        {
            ("ComboBox", "ComboBoxItem"),
            ("ListBox", "ListBoxItem"),
        })
        {
            string markup = $"<{selectorType}><{itemType} Content='First' /><{itemType} Content='Second' /></{selectorType}>";
            AxamlDesignerSession session = new();
            session.Load(markup);
            session.SelectNode(session.Root!.Children[1]);

            Assert.IsTrue(session.CanMoveSelectedControlEarlier, selectorType);
            Assert.IsTrue(session.MoveSelectedControl(-1), selectorType);
            CollectionAssert.AreEqual(
                new[] { "Second", "First" },
                session.Root!.Children.Select(static item => item.Text).ToArray(),
                selectorType);
            string reordered = session.Markup;
            Assert.IsTrue(session.Undo(), selectorType);
            Assert.AreEqual(markup, session.Markup, selectorType);
            Assert.IsTrue(session.Redo(), selectorType);
            Assert.AreEqual(reordered, session.Markup, selectorType);
        }
    }

    [TestMethod]
    public void ShapeControls_CanBeAddedToPanelsWithUndoRedo()
    {
        foreach (string shapeType in new[] { "Ellipse", "Line", "Polygon", "Polyline", "Rectangle" })
        {
            AxamlDesignerSession session = new();
            session.Load("<StackPanel />");
            session.SelectNode(session.Root);

            Assert.IsTrue(session.AddChildControl(shapeType), shapeType);
            Assert.AreEqual(shapeType, session.Root!.Children.Single().TypeName);
            string addedMarkup = session.Markup;
            Assert.IsTrue(session.Undo(), shapeType);
            Assert.AreEqual("<StackPanel />", session.Markup, shapeType);
            Assert.IsTrue(session.Redo(), shapeType);
            Assert.AreEqual(addedMarkup, session.Markup, shapeType);
            Assert.AreEqual(shapeType, session.Root!.Children.Single().TypeName, shapeType);
        }
    }

    [TestMethod]
    public void ItemsControlItems_CanBeAddedRemovedAndUndone()
    {
        AxamlDesignerSession session = new();
        session.Load("<ItemsControl />");
        session.SelectNode(session.Root);

        Assert.IsTrue(session.AddChildControl("TextBlock"));
        Assert.AreEqual("TextBlock", session.Root!.Children.Single().TypeName);
        session.SelectNode(session.Root.Children.Single());
        Assert.IsTrue(session.CanRemoveSelectedControl);
        Assert.IsTrue(session.RemoveSelectedControl());
        Assert.AreEqual(0, session.Root!.Children.Count);
        Assert.IsTrue(session.Undo());
        Assert.AreEqual("TextBlock", session.Root!.Children.Single().TypeName);
        Assert.IsTrue(session.Undo());
        Assert.AreEqual("<ItemsControl />", session.Markup);
        Assert.IsTrue(session.Redo());
        Assert.AreEqual("TextBlock", session.Root!.Children.Single().TypeName);
        Assert.IsTrue(session.Redo());
        Assert.AreEqual(0, session.Root!.Children.Count);

        const string reorderMarkup = "<ItemsControl><TextBlock Text='First' /><Label Content='Second' /></ItemsControl>";
        AxamlDesignerSession reordered = new();
        reordered.Load(reorderMarkup);
        reordered.SelectNode(reordered.Root!.Children[1]);
        Assert.IsTrue(reordered.CanMoveSelectedControlEarlier);
        Assert.IsTrue(reordered.MoveSelectedControl(-1));
        CollectionAssert.AreEqual(
            new[] { "Second", "First" },
            reordered.Root!.Children.Select(static item => item.Text).ToArray());
        Assert.IsTrue(reordered.Undo());
        Assert.AreEqual(reorderMarkup, reordered.Markup);
        Assert.IsTrue(reordered.Redo());
        CollectionAssert.AreEqual(
            new[] { "Second", "First" },
            reordered.Root!.Children.Select(static item => item.Text).ToArray());
    }

    [TestMethod]
    public void StructureEditing_RejectsAmbiguousPanelsAndProtectedContent()
    {
        string[] protectedMarkup =
        [
            "<StackPanel><!-- keep --><Button /><TextBlock /></StackPanel>",
            "<StackPanel><StackPanel><!-- keep --><Button /></StackPanel><TextBlock /></StackPanel>",
            "<StackPanel><StackPanel><StackPanel.Resources /></StackPanel><TextBlock /></StackPanel>",
            "<StackPanel xmlns:custom='using:Example'><StackPanel><custom:Button /></StackPanel><TextBlock /></StackPanel>",
            "<StackPanel><UserControl><TextBlock /></UserControl><Button /></StackPanel>",
            "<StackPanel><Button><![CDATA[keep]]></Button><TextBlock /></StackPanel>",
        ];
        foreach (string markup in protectedMarkup)
        {
            AxamlDesignerSession session = new();
            session.Load(markup);
            session.SelectNode(session.Root!.Children[0]);
            Assert.ThrowsExactly<System.InvalidOperationException>(() => session.RemoveSelectedControl(), markup);
            Assert.AreEqual(markup, session.Markup);
            Assert.IsFalse(session.CanUndo);
        }

        AxamlDesignerSession commented = new();
        commented.Load(protectedMarkup[0]);
        commented.SelectNode(commented.Root!.Children[0]);
        Assert.ThrowsExactly<System.InvalidOperationException>(() => commented.MoveSelectedControl(1));
        Assert.AreEqual(protectedMarkup[0], commented.Markup);
        Assert.IsFalse(commented.CanUndo);
    }

    [TestMethod]
    public void StructureEditing_RejectsRootUnknownSiblingsAndLayoutSensitivePanels()
    {
        string[] markupCases =
        [
            "<Grid><Button /><TextBlock /></Grid>",
            "<DockPanel><Button DockPanel.Dock='{Binding Side}' /><TextBlock /></DockPanel>",
            "<av:DockPanel xmlns:av='https://github.com/avaloniaui'><av:Button DockPanel.Dock='Top' /><av:TextBlock /></av:DockPanel>",
            "<StackPanel><Button /><UnknownControl /></StackPanel>",
            "<StackPanel>unexpected<Button /></StackPanel>",
        ];
        foreach (string markup in markupCases)
        {
            AxamlDesignerSession session = new();
            session.Load(markup);
            session.SelectNode(session.Root);
            Assert.IsFalse(session.CanRemoveSelectedControl);
            Assert.ThrowsExactly<System.InvalidOperationException>(() => session.RemoveSelectedControl());
            session.SelectNode(session.Root!.Children[0]);
            Assert.IsFalse(session.CanRemoveSelectedControl);
            Assert.IsFalse(session.CanMoveSelectedControlLater);
            Assert.ThrowsExactly<System.InvalidOperationException>(() => session.RemoveSelectedControl());
            Assert.ThrowsExactly<System.InvalidOperationException>(() => session.MoveSelectedControl(1));
            Assert.AreEqual(markup, session.Markup);
            Assert.IsFalse(session.CanUndo);
        }
    }

    [TestMethod]
    public void DockPanelStructure_RemovesDirectChildrenWithUndoRedo()
    {
        const string original = "<DockPanel><Button DockPanel.Dock='Top' /><TextBlock /></DockPanel>";
        const string removedDockedChild = "<DockPanel><TextBlock /></DockPanel>";
        AxamlDesignerSession session = new();
        session.Load(original);
        session.SelectNode(session.Root!.Children[0]);

        Assert.IsTrue(session.CanRemoveSelectedControl);
        Assert.IsTrue(session.RemoveSelectedControl());
        Assert.AreEqual(removedDockedChild, session.Markup);
        Assert.IsTrue(session.Undo());
        Assert.AreEqual(original, session.Markup);
        Assert.IsTrue(session.Redo());
        Assert.AreEqual(removedDockedChild, session.Markup);

        session.SelectNode(session.Root!.Children[0]);
        Assert.IsTrue(session.RemoveSelectedControl());
        Assert.AreEqual("<DockPanel></DockPanel>", session.Markup);
    }

    [TestMethod]
    public void DockPanelStructure_ReordersDockedChildrenWithoutMovingLastChildFillClient()
    {
        const string original =
            "<DockPanel><Button DockPanel.Dock='Top' /><Separator DockPanel.Dock='Bottom' /><TextBlock /></DockPanel>";
        AxamlDesignerSession session = new();
        session.Load(original);
        session.SelectNode(session.Root!.Children[1]);

        Assert.IsTrue(session.CanMoveSelectedControlEarlier);
        Assert.IsFalse(session.CanMoveSelectedControlLater);
        Assert.IsTrue(session.MoveSelectedControl(-1));
        CollectionAssert.AreEqual(
            new[] { "Separator", "Button", "TextBlock" },
            session.Root!.Children.Select(static child => child.TypeName).ToArray());
        Assert.AreEqual("TextBlock", session.Root.Children[^1].TypeName);
        Assert.IsTrue(session.Undo());
        Assert.AreEqual(original, session.Markup);
        Assert.IsTrue(session.Redo());
        Assert.AreEqual("TextBlock", session.Root!.Children[^1].TypeName);

        session.SelectNode(session.Root.Children[^1]);
        Assert.IsFalse(session.CanMoveSelectedControlEarlier);
        Assert.IsFalse(session.CanMoveSelectedControlLater);
        Assert.ThrowsExactly<System.InvalidOperationException>(() => session.MoveSelectedControl(-1));
    }

    [TestMethod]
    public void DockPanelStructure_ReordersChildrenWhenLastChildFillIsDisabled()
    {
        const string original =
            "<DockPanel LastChildFill='False'><Button DockPanel.Dock='Top' /><TextBlock DockPanel.Dock='Left' /></DockPanel>";
        AxamlDesignerSession session = new();
        session.Load(original);
        session.SelectNode(session.Root!.Children[0]);

        Assert.IsTrue(session.CanMoveSelectedControlLater);
        Assert.IsTrue(session.MoveSelectedControl(1));
        CollectionAssert.AreEqual(
            new[] { "TextBlock", "Button" },
            session.Root!.Children.Select(static child => child.TypeName).ToArray());
        Assert.IsTrue(session.Undo());
        Assert.AreEqual(original, session.Markup);
    }

    [TestMethod]
    public void GridPlacement_MovesSpanningChildOnlyIntoFreeCellsAndPreservesSourceAndHistory()
    {
        const string markup = """
            <av:Grid xmlns:av='https://github.com/avaloniaui' xmlns:layout='https://github.com/avaloniaui'
                     xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
              <av:Grid.RowDefinitions><av:RowDefinition Height='*' /><av:RowDefinition Height='*' /></av:Grid.RowDefinitions>
              <av:Grid.ColumnDefinitions><av:ColumnDefinition /><av:ColumnDefinition /><av:ColumnDefinition /></av:Grid.ColumnDefinitions>
              <av:Button x:Name='Moving' layout:Grid.RowSpan='2' layout:Grid.Column='0' />
              <av:TextBlock x:Name='Occupied' av:Grid.Column='2' />
            </av:Grid>
            """;
        AxamlDesignerSession session = new();
        session.Load(markup);
        session.SelectNode(session.Root!.Children[0]);

        Assert.IsTrue(session.IsSelectedGridChild);
        Assert.IsTrue(session.CanEditSelectedGridLayout);
        Assert.IsTrue(session.CanMoveSelectedGridControl(0, 1));
        Assert.IsFalse(session.CanMoveSelectedGridControl(1, 0));
        Assert.IsFalse(session.CanMoveSelectedControlLater);
        Assert.IsTrue(session.MoveSelectedGridControl(0, 1));
        string moved = markup.Replace("layout:Grid.Column='0'", "layout:Grid.Column='1'");
        Assert.AreEqual(moved, session.Markup);
        Assert.AreEqual("Moving", session.SelectedNode!.StableName);
        Assert.IsFalse(session.CanMoveSelectedGridControl(0, 1));
        Assert.IsTrue(session.CanMoveSelectedGridControl(0, -1));
        Assert.IsTrue(session.Undo());
        Assert.AreEqual(markup, session.Markup);
        Assert.AreEqual("Moving", session.SelectedNode!.StableName);
        Assert.IsTrue(session.Redo());
        Assert.AreEqual(moved, session.Markup);
        Assert.AreEqual("Moving", session.SelectedNode!.StableName);
    }

    [TestMethod]
    public void GridPlacement_AddsMissingCoordinatesAsSeparateUndoUnits()
    {
        const string markup = "<Grid RowDefinitions='*,*' ColumnDefinitions='*,*'><Button /></Grid>";
        AxamlDesignerSession session = new();
        session.Load(markup);
        session.SelectNode(session.Root!.Children[0]);

        Assert.ThrowsExactly<System.ArgumentOutOfRangeException>(() => session.MoveSelectedGridControl(1, 1));
        Assert.IsTrue(session.MoveSelectedGridControl(1, 0));
        StringAssert.Contains(session.Markup, "Grid.Row=\"1\"");
        Assert.IsTrue(session.MoveSelectedGridControl(0, 1));
        StringAssert.Contains(session.Markup, "Grid.Column=\"1\"");
        Assert.IsTrue(session.Undo());
        StringAssert.Contains(session.Markup, "Grid.Row=\"1\"");
        Assert.IsFalse(session.Markup.Contains("Grid.Column="));
        Assert.IsTrue(session.Undo());
        Assert.AreEqual(markup, session.Markup);
        Assert.IsTrue(session.Redo());
        Assert.AreEqual("1", session.SelectedNode!.Properties["Grid.Row"]);
    }

    [TestMethod]
    public void GridDefinitions_AddRowsAndColumnsAndPreserveAttributeQuotesWithUndoRedo()
    {
        const string original = "<Grid RowDefinitions='*' ColumnDefinitions='*'><Button /></Grid>";
        AxamlDesignerSession session = new();
        session.Load(original);
        session.SelectNode(session.Root);

        Assert.IsTrue(session.CanAddGridRowDefinition);
        Assert.IsTrue(session.AddGridRowDefinition());
        StringAssert.Contains(session.Markup, "RowDefinitions='*,Auto'");
        CollectionAssert.AreEqual(new[] { "*", "Auto" }, session.Root!.RowDefinitions.ToArray());
        Assert.AreEqual("0", session.Root.Children.Single().Properties.GetValueOrDefault("Grid.Row", "0"));

        Assert.IsTrue(session.CanAddGridColumnDefinition);
        Assert.IsTrue(session.AddGridColumnDefinition());
        StringAssert.Contains(session.Markup, "ColumnDefinitions='*,Auto'");
        CollectionAssert.AreEqual(new[] { "*", "Auto" }, session.Root!.ColumnDefinitions.ToArray());
        Assert.IsTrue(session.Undo());
        Assert.AreEqual("*", session.Root!.ColumnDefinitions.Single());
        Assert.IsTrue(session.Undo());
        Assert.AreEqual(original, session.Markup);
        Assert.IsTrue(session.Redo());
        Assert.IsTrue(session.Redo());
        CollectionAssert.AreEqual(new[] { "*", "Auto" }, session.Root!.RowDefinitions.ToArray());
        CollectionAssert.AreEqual(new[] { "*", "Auto" }, session.Root.ColumnDefinitions.ToArray());
    }

    [TestMethod]
    public void GridDefinitions_AddToPropertyElementsAndPreservePrefixesAndImplicitStarTracks()
    {
        const string prefixedMarkup = """
            <av:Grid xmlns:av='https://github.com/avaloniaui'>
              <av:Grid.RowDefinitions>
                <av:RowDefinition Height='*' />
              </av:Grid.RowDefinitions>
              <av:Button />
            </av:Grid>
            """;
        AxamlDesignerSession prefixed = new();
        prefixed.Load(prefixedMarkup);
        prefixed.SelectNode(prefixed.Root);

        Assert.IsTrue(prefixed.AddGridRowDefinition());
        StringAssert.Contains(prefixed.Markup, "<av:RowDefinition Height=\"Auto\" />");
        Assert.IsTrue(prefixed.AddGridColumnDefinition());
        StringAssert.Contains(prefixed.Markup, "<av:Grid.ColumnDefinitions>");
        StringAssert.Contains(prefixed.Markup, "<av:ColumnDefinition Width=\"*\" />");
        StringAssert.Contains(prefixed.Markup, "<av:ColumnDefinition Width=\"Auto\" />");
        Assert.AreEqual(2, prefixed.Root!.RowDefinitions.Count);
        Assert.AreEqual(2, prefixed.Root.ColumnDefinitions.Count);
        Assert.IsTrue(prefixed.Root.HasSafeGridLayout);

        AxamlDesignerSession implicitTracks = new();
        implicitTracks.Load("<Grid><Button /></Grid>");
        implicitTracks.SelectNode(implicitTracks.Root);
        Assert.IsTrue(implicitTracks.AddGridRowDefinition());
        StringAssert.Contains(implicitTracks.Markup, "<RowDefinition Height=\"*\" />");
        StringAssert.Contains(implicitTracks.Markup, "<RowDefinition Height=\"Auto\" />");
        Assert.AreEqual(2, implicitTracks.Root!.RowDefinitions.Count);
        Assert.IsTrue(implicitTracks.Root.HasSafeGridLayout);
    }

    [TestMethod]
    public void GridDefinitions_RejectUnsafeLayoutsAndCellLimit()
    {
        AxamlDesignerSession unsafeGrid = new();
        unsafeGrid.Load("<Grid RowDefinitions='{Binding Rows}' />");
        unsafeGrid.SelectNode(unsafeGrid.Root);
        Assert.IsFalse(unsafeGrid.CanAddGridRowDefinition);
        Assert.ThrowsExactly<System.InvalidOperationException>(() => unsafeGrid.AddGridRowDefinition());
        Assert.AreEqual("<Grid RowDefinitions='{Binding Rows}' />", unsafeGrid.Markup);

        string rows = string.Join(',', Enumerable.Repeat("Auto", 1000));
        string columns = string.Join(',', Enumerable.Repeat("Auto", 100));
        AxamlDesignerSession fullGrid = new();
        fullGrid.Load($"<Grid RowDefinitions='{rows}' ColumnDefinitions='{columns}' />");
        fullGrid.SelectNode(fullGrid.Root);
        Assert.IsFalse(fullGrid.CanAddGridRowDefinition);
        Assert.IsFalse(fullGrid.CanAddGridColumnDefinition);
        Assert.ThrowsExactly<System.InvalidOperationException>(() => fullGrid.AddGridColumnDefinition());
        Assert.IsFalse(fullGrid.CanUndo);
    }

    [TestMethod]
    public void GridDefinitions_RemoveOnlyUnusedEndTracksAndSupportUndoRedo()
    {
        const string attributeMarkup =
            "<Grid RowDefinitions='*,Auto' ColumnDefinitions=\"*,Auto\"><Button /></Grid>";
        AxamlDesignerSession attributes = new();
        attributes.Load(attributeMarkup);
        attributes.SelectNode(attributes.Root);

        Assert.IsTrue(attributes.CanRemoveGridRowDefinition);
        Assert.IsTrue(attributes.CanRemoveGridColumnDefinition);
        Assert.IsTrue(attributes.RemoveGridRowDefinition());
        StringAssert.Contains(attributes.Markup, "RowDefinitions='*'");
        Assert.IsTrue(attributes.RemoveGridColumnDefinition());
        StringAssert.Contains(attributes.Markup, "ColumnDefinitions=\"*\"");
        Assert.AreEqual(1, attributes.Root!.RowDefinitions.Count);
        Assert.AreEqual(1, attributes.Root.ColumnDefinitions.Count);
        Assert.IsTrue(attributes.Undo());
        Assert.AreEqual(2, attributes.Root!.ColumnDefinitions.Count);
        Assert.IsTrue(attributes.Undo());
        Assert.AreEqual(attributeMarkup, attributes.Markup);
        Assert.IsTrue(attributes.Redo());
        Assert.IsTrue(attributes.Redo());
        Assert.AreEqual(1, attributes.Root!.RowDefinitions.Count);
        Assert.AreEqual(1, attributes.Root.ColumnDefinitions.Count);

        const string propertyMarkup = """
            <av:Grid xmlns:av='https://github.com/avaloniaui'>
              <av:Grid.RowDefinitions>
                <av:RowDefinition Height='*' />
                <av:RowDefinition Height='Auto' />
              </av:Grid.RowDefinitions>
              <av:Grid.ColumnDefinitions>
                <av:ColumnDefinition Width='*' />
                <av:ColumnDefinition Width='Auto' />
              </av:Grid.ColumnDefinitions>
              <av:Button />
            </av:Grid>
            """;
        AxamlDesignerSession propertyElements = new();
        propertyElements.Load(propertyMarkup);
        propertyElements.SelectNode(propertyElements.Root);

        Assert.IsTrue(propertyElements.RemoveGridRowDefinition());
        Assert.IsTrue(propertyElements.RemoveGridColumnDefinition());
        Assert.AreEqual(1, propertyElements.Root!.RowDefinitions.Count);
        Assert.AreEqual(1, propertyElements.Root.ColumnDefinitions.Count);
        StringAssert.Contains(propertyElements.Markup, "<av:RowDefinition Height='*' />");
        StringAssert.Contains(propertyElements.Markup, "<av:ColumnDefinition Width='*' />");
        Assert.IsFalse(propertyElements.Markup.Contains("Height='Auto'", StringComparison.Ordinal));
        Assert.IsFalse(propertyElements.Markup.Contains("Width='Auto'", StringComparison.Ordinal));
    }

    [TestMethod]
    public void GridDefinitions_KeepOccupiedSpansAndImplicitOrSingleTracks()
    {
        AxamlDesignerSession occupiedBySpans = new();
        occupiedBySpans.Load(
            "<Grid RowDefinitions='*,Auto' ColumnDefinitions='*,Auto'><Button Grid.RowSpan='2' Grid.ColumnSpan='2' /></Grid>");
        occupiedBySpans.SelectNode(occupiedBySpans.Root);
        Assert.IsFalse(occupiedBySpans.CanRemoveGridRowDefinition);
        Assert.IsFalse(occupiedBySpans.CanRemoveGridColumnDefinition);
        Assert.ThrowsExactly<System.InvalidOperationException>(() => occupiedBySpans.RemoveGridRowDefinition());

        AxamlDesignerSession singleTracks = new();
        singleTracks.Load("<Grid RowDefinitions='Auto' ColumnDefinitions='Auto'><Button /></Grid>");
        singleTracks.SelectNode(singleTracks.Root);
        Assert.IsFalse(singleTracks.CanRemoveGridRowDefinition);
        Assert.IsFalse(singleTracks.CanRemoveGridColumnDefinition);

        AxamlDesignerSession implicitTracks = new();
        implicitTracks.Load("<Grid><Button /></Grid>");
        implicitTracks.SelectNode(implicitTracks.Root);
        Assert.IsFalse(implicitTracks.CanRemoveGridRowDefinition);
        Assert.IsFalse(implicitTracks.CanRemoveGridColumnDefinition);
    }

    [TestMethod]
    public void GridDefinitions_ResizeExplicitTracksAndPreserveBothSourceForms()
    {
        const string attributeMarkup =
            "<Grid RowDefinitions='* , Auto' ColumnDefinitions=\"Auto, 2*\"><Button /></Grid>";
        AxamlDesignerSession attributes = new();
        attributes.Load(attributeMarkup);
        attributes.SelectNode(attributes.Root);

        Assert.IsTrue(attributes.CanSetGridDefinitionSize(isRow: true, index: 1, "120"));
        Assert.IsFalse(attributes.CanSetGridDefinitionSize(isRow: true, index: 1, "-1"));
        Assert.IsFalse(attributes.CanSetGridDefinitionSize(isRow: true, index: 2, "120"));
        Assert.IsFalse(attributes.SetGridDefinitionSize(isRow: true, index: 0, "*"));
        Assert.IsTrue(attributes.SetGridDefinitionSize(isRow: true, index: 1, "120"));
        StringAssert.Contains(attributes.Markup, "RowDefinitions='* , 120'");
        Assert.IsTrue(attributes.SetGridDefinitionSize(isRow: false, index: 1, "3*"));
        StringAssert.Contains(attributes.Markup, "ColumnDefinitions=\"Auto, 3*\"");
        CollectionAssert.AreEqual(new[] { "*", "120" }, attributes.Root!.RowDefinitions.ToArray());
        CollectionAssert.AreEqual(new[] { "Auto", "3*" }, attributes.Root.ColumnDefinitions.ToArray());
        Assert.IsTrue(attributes.Undo());
        Assert.IsTrue(attributes.Undo());
        Assert.AreEqual(attributeMarkup, attributes.Markup);
        Assert.IsTrue(attributes.Redo());
        Assert.IsTrue(attributes.Redo());
        Assert.AreEqual("120", attributes.Root!.RowDefinitions[1]);
        Assert.AreEqual("3*", attributes.Root.ColumnDefinitions[1]);

        const string propertyMarkup = """
            <av:Grid xmlns:av='https://github.com/avaloniaui'>
              <av:Grid.RowDefinitions>
                <av:RowDefinition Height='*' />
                <av:RowDefinition />
              </av:Grid.RowDefinitions>
              <av:Button />
            </av:Grid>
            """;
        AxamlDesignerSession propertyElements = new();
        propertyElements.Load(propertyMarkup);
        propertyElements.SelectNode(propertyElements.Root);

        Assert.IsTrue(propertyElements.SetGridDefinitionSize(isRow: true, index: 0, "2*"));
        StringAssert.Contains(propertyElements.Markup, "<av:RowDefinition Height='2*' />");
        Assert.IsTrue(propertyElements.SetGridDefinitionSize(isRow: true, index: 1, "96"));
        StringAssert.Contains(propertyElements.Markup, "<av:RowDefinition Height=\"96\" />");
        CollectionAssert.AreEqual(new[] { "2*", "96" }, propertyElements.Root!.RowDefinitions.ToArray());
        Assert.IsTrue(propertyElements.Root.HasSafeGridLayout);
    }

    [TestMethod]
    public void GridDefinitions_RejectUnsafeTrackSizeSourcesAndValues()
    {
        AxamlDesignerSession session = new();
        session.Load("<Grid><Button /></Grid>");
        session.SelectNode(session.Root);
        Assert.IsFalse(session.CanSetGridDefinitionSize(isRow: true, index: 0, "*"));

        session.Load("<Grid RowDefinitions='Auto' ColumnDefinitions='*'><Button /></Grid>");
        session.SelectNode(session.Root);
        foreach (string value in new[] { "{Binding Size}", "-1", "NaN", "Infinity", "1e9999", "" })
        {
            Assert.IsFalse(session.CanSetGridDefinitionSize(isRow: true, index: 0, value), value);
        }

        Assert.IsFalse(session.CanUndo);
        Assert.ThrowsExactly<System.InvalidOperationException>(
            () => session.SetGridDefinitionSize(isRow: true, index: 0, "{Binding Size}"));
        Assert.AreEqual("<Grid RowDefinitions='Auto' ColumnDefinitions='*'><Button /></Grid>", session.Markup);
        Assert.IsTrue(session.Root!.HasSafeGridLayout);
    }

    [TestMethod]
    public void GridPlacement_QualifiesMissingCoordinatesAndPreservesNamespaceDeclarations()
    {
        const string markup = """
            <av:Grid xmlns:av='https://github.com/avaloniaui' xmlns='urn:other' ColumnDefinitions='*,*'>
              <av:Button xmlns:Grid.Column='urn:unused' />
            </av:Grid>
            """;
        AxamlDesignerSession session = new();
        session.Load(markup);
        session.SelectNode(session.Root!.Children[0]);

        Assert.IsTrue(session.CanMoveSelectedGridControl(0, 1));
        Assert.IsTrue(session.MoveSelectedGridControl(0, 1));
        StringAssert.Contains(session.Markup, "xmlns:Grid.Column='urn:unused'");
        StringAssert.Contains(session.Markup, "av:Grid.Column=\"1\"");
        Assert.AreEqual("1", session.SelectedNode!.Properties["Grid.Column"]);
        Assert.IsTrue(session.MoveSelectedGridControl(0, -1));
        StringAssert.Contains(session.Markup, "av:Grid.Column=\"0\"");
        Assert.IsTrue(session.Undo());
        Assert.IsTrue(session.Undo());
        Assert.AreEqual(markup, session.Markup);
    }

    [TestMethod]
    public void GridPlacement_PreservesExistingQualifiedAttributeWithDefaultNamespace()
    {
        const string markup = """
            <Grid xmlns='https://github.com/avaloniaui' xmlns:g='https://github.com/avaloniaui' ColumnDefinitions='*,*'>
              <Button g:Grid.Column='0' />
            </Grid>
            """;
        AxamlDesignerSession session = new();
        session.Load(markup);
        session.SelectNode(session.Root!.Children[0]);

        Assert.IsTrue(session.MoveSelectedGridControl(0, 1));
        Assert.AreEqual(markup.Replace("g:Grid.Column='0'", "g:Grid.Column='1'"), session.Markup);
        Assert.AreEqual("1", session.SelectedNode!.Properties["Grid.Column"]);
    }

    [TestMethod]
    public async Task GridPlacement_PrefixOnlyGeneratedMarkupBuildsWithAvalonia()
    {
        string directory = Directory.CreateTempSubdirectory("AxamlPrefixGrid-").FullName;
        try
        {
            const string markup = """
                <av:UserControl xmlns:av="https://github.com/avaloniaui"
                                xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                                x:Class="GridFixture.View">
                  <av:StackPanel>
                    <av:Grid RowDefinitions="*,*" ColumnDefinitions="*,*"><av:Button /></av:Grid>
                    <av:DockPanel LastChildFill="False"><av:TextBlock /></av:DockPanel>
                    <av:Grid><av:TextBlock /></av:Grid>
                  </av:StackPanel>
                </av:UserControl>
                """;
            AxamlDesignerSession session = new();
            session.Load(markup);
            session.SelectNode(session.Root!.Children[0].Children[0]);
            Assert.IsTrue(session.MoveSelectedGridControl(1, 0));
            session.SelectNode(session.Root!.Children[0]);
            Assert.IsTrue(session.AddChildControl("Label"));
            session.SelectNode(session.Root!.Children[1]);
            Assert.IsTrue(session.AddChildControl("Button", AxamlDockPanelPlacement.Bottom));
            session.SelectNode(session.Root!.Children[2]);
            Assert.IsTrue(session.AddGridRowDefinition());
            Assert.IsTrue(session.AddGridColumnDefinition());
            StringAssert.Contains(session.Markup, "<av:Grid.RowDefinitions>");
            StringAssert.Contains(session.Markup, "<av:ColumnDefinition Width=\"Auto\" />");

            string projectPath = Path.Combine(directory, "GridFixture.csproj");
            File.WriteAllText(projectPath, """
                <Project Sdk="Microsoft.NET.Sdk">
                  <PropertyGroup><TargetFramework>net10.0</TargetFramework><NuGetAudit>false</NuGetAudit></PropertyGroup>
                  <ItemGroup><PackageReference Include="Avalonia" Version="12.1.5" /></ItemGroup>
                </Project>
                """);
            File.WriteAllText(Path.Combine(directory, "View.axaml"), session.Markup);
            File.WriteAllText(Path.Combine(directory, "View.axaml.cs"),
                "namespace GridFixture; public partial class View : Avalonia.Controls.UserControl { }");
            ProcessStartInfo startInfo = new("dotnet")
            {
                WorkingDirectory = directory,
                ArgumentList = { "build", projectPath, "-nr:false", "-p:UseSharedCompilation=false", "-p:RestoreIgnoreFailedSources=true", "--verbosity", "quiet" },
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
            };
            foreach (string variable in new[]
            {
                "MSBuildSDKsPath", "MSBuildExtensionsPath", "MSBuildExtensionsPath32", "MSBuildToolsPath",
                "DOTNET_MSBUILD_SDK_RESOLVER_SDKS_DIR", "DOTNET_MSBUILD_SDK_RESOLVER_CLI_DIR",
            })
            {
                startInfo.Environment.Remove(variable);
            }

            using Process process = Process.Start(startInfo)
                ?? throw new InvalidOperationException("Could not start the prefixed AXAML fixture build.");
            Task<string> output = process.StandardOutput.ReadToEndAsync();
            Task<string> error = process.StandardError.ReadToEndAsync();
            await process.WaitForExitAsync();
            await Task.WhenAll(output, error);
            Assert.AreEqual(0, process.ExitCode, $"{output.Result}{error.Result}");
        }
        finally
        {
            for (int attempt = 0; ; attempt++)
            {
                try
                {
                    Directory.Delete(directory, recursive: true);
                    break;
                }
                catch (IOException) when (attempt < 9)
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(200));
                }
            }
        }
    }

    [TestMethod]
    public void GridRemoval_WorksInFullGridAndPreservesDefinitionElements()
    {
        const string markup = """
            <Grid xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
              <Grid.ColumnDefinitions><ColumnDefinition /><ColumnDefinition /></Grid.ColumnDefinitions>
              <Button x:Name='First' />
              <TextBlock x:Name='Second' Grid.Column='1' />
            </Grid>
            """;
        AxamlDesignerSession session = new();
        session.Load(markup);
        Assert.IsTrue(session.Root!.HasSafeGridLayout);
        Assert.IsFalse(session.Root.CanAddGridChild);
        session.SelectNode(session.Root.Children[0]);

        Assert.IsTrue(session.CanRemoveSelectedControl);
        Assert.IsFalse(session.CanMoveSelectedControlLater);
        Assert.IsFalse(session.CanMoveSelectedGridControl(0, 1));
        Assert.IsTrue(session.RemoveSelectedControl());
        string newline = markup.Contains("\r\n") ? "\r\n" : "\n";
        Assert.AreEqual(markup.Replace($"  <Button x:Name='First' />{newline}", string.Empty), session.Markup);
        Assert.AreEqual("Grid", session.SelectedNode!.TypeName);
        Assert.IsTrue(session.Root!.CanAddGridChild);
        Assert.IsTrue(session.Undo());
        Assert.AreEqual(markup, session.Markup);
        Assert.AreEqual("First", session.SelectedNode!.StableName);
        Assert.IsTrue(session.Redo());
        Assert.AreEqual("Grid", session.SelectedNode!.TypeName);
    }

    [TestMethod]
    public void GridEditing_RejectsAmbiguousOrDynamicLayoutsAndPreservesComments()
    {
        string[] invalid =
        [
            "<Grid RowDefinitions='{Binding Rows}'><Button /></Grid>",
            "<Grid ColumnDefinitions='*,*'><Button /><TextBlock /></Grid>",
            "<Grid RowDefinitions='*,*'><Button Grid.RowSpan='3' /></Grid>",
            "<Grid ColumnDefinitions='*,*'><Button /><UnknownControl Grid.Column='1' /></Grid>",
            "<Grid xmlns:c='urn:custom' ColumnDefinitions='*,*'><Button c:Grid.Column='0' /></Grid>",
            "<Grid ColumnDefinitions='*,*'><Button><Grid.Column>0</Grid.Column></Button></Grid>",
            "<av:Grid xmlns:av='https://github.com/avaloniaui' ColumnDefinitions='*,*'><av:Button Grid.Column='0' /></av:Grid>",
            "<Grid xmlns='https://github.com/avaloniaui' xmlns:av='https://github.com/avaloniaui' ColumnDefinitions='*,*'><Button Grid.Column='0' av:Grid.Column='1' /></Grid>",
            "<av:Grid xmlns:av='https://github.com/avaloniaui' ColumnDefinitions='*,*'><av:Button av:Grid.Column='{Binding Position}' /></av:Grid>",
            "<av:Grid xmlns:av='https://github.com/avaloniaui' ColumnDefinitions='*,*'><Button /></av:Grid>",
            "<Grid ColumnDefinitions='*,*'><?note keep?><Button /></Grid>",
        ];
        foreach (string markup in invalid)
        {
            AxamlDesignerSession session = new();
            session.Load(markup);
            session.SelectNode(session.Root!.Children[0]);
            Assert.IsTrue(session.IsSelectedGridChild, markup);
            Assert.IsFalse(session.CanEditSelectedGridLayout, markup);
            Assert.IsFalse(session.CanRemoveSelectedControl, markup);
            Assert.IsFalse(session.CanMoveSelectedGridControl(0, 1), markup);
            Assert.ThrowsExactly<System.InvalidOperationException>(() => session.RemoveSelectedControl(), markup);
            Assert.ThrowsExactly<System.InvalidOperationException>(() => session.MoveSelectedGridControl(0, 1), markup);
            Assert.AreEqual(markup, session.Markup);
            Assert.IsFalse(session.CanUndo);
        }

        const string commented = "<Grid ColumnDefinitions='*,*'><!-- keep --><Button /></Grid>";
        AxamlDesignerSession withComment = new();
        withComment.Load(commented);
        withComment.SelectNode(withComment.Root!.Children[0]);
        Assert.IsFalse(withComment.CanMoveSelectedGridControl(0, 1));
        Assert.IsFalse(withComment.CanRemoveSelectedControl);
        Assert.IsFalse(withComment.Root.CanAddGridChild);
        Assert.ThrowsExactly<System.InvalidOperationException>(() => withComment.MoveSelectedGridControl(0, 1));
        Assert.ThrowsExactly<System.InvalidOperationException>(() => withComment.RemoveSelectedControl());
        withComment.SelectNode(withComment.Root);
        Assert.ThrowsExactly<System.InvalidOperationException>(() => withComment.AddChildControl("TextBlock"));
        Assert.AreEqual(commented, withComment.Markup);
        Assert.IsFalse(withComment.CanUndo);
    }

    [TestMethod]
    public void AddChildControl_AddsNextGridCellAsOneUndoableEdit()
    {
        const string markup = "<Grid RowDefinitions='*,*' ColumnDefinitions='*,*'><TextBlock Grid.Row='0' Grid.Column='0' /></Grid>";
        AxamlDesignerSession session = new();
        session.Load(markup);
        session.SelectNode(session.Root);

        Assert.IsTrue(session.AddChildControl("Button"));
        Assert.AreEqual("0", session.Root!.Children[1].Properties["Grid.Row"]);
        Assert.AreEqual("1", session.Root.Children[1].Properties["Grid.Column"]);
        Assert.IsTrue(session.Undo());
        Assert.AreEqual(markup, session.Markup);
        Assert.IsTrue(session.Redo());
        Assert.AreEqual(2, session.Root!.Children.Count);
    }

    [TestMethod]
    public void AddChildControl_KeepsDockPanelClientLastAndSupportsUndoRedo()
    {
        const string original = "<DockPanel />";
        AxamlDesignerSession session = new();
        session.Load(original);
        session.SelectNode(session.Root);

        Assert.IsTrue(session.AddChildControl("TextBlock", AxamlDockPanelPlacement.Client));
        Assert.IsTrue(session.AddChildControl("Button", AxamlDockPanelPlacement.Top));
        Assert.IsTrue(session.AddChildControl("Separator", AxamlDockPanelPlacement.Bottom));
        Assert.AreEqual("Button", session.Root!.Children[0].TypeName);
        Assert.AreEqual("Top", session.Root.Children[0].Properties["DockPanel.Dock"]);
        Assert.AreEqual("Separator", session.Root.Children[1].TypeName);
        Assert.AreEqual("Bottom", session.Root.Children[1].Properties["DockPanel.Dock"]);
        Assert.AreEqual("TextBlock", session.Root.Children[2].TypeName);

        Assert.IsTrue(session.Undo());
        Assert.AreEqual(2, session.Root!.Children.Count);
        Assert.IsTrue(session.Redo());
        Assert.AreEqual(3, session.Root!.Children.Count);
        Assert.AreEqual("TextBlock", session.Root.Children[2].TypeName);
    }
}
