using AA98_AvlnCodeStudio.Model.Workspaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Collections.Generic;

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

        StringAssert.Contains(session.Markup, "<av:Button Grid.Row=\"0\" Grid.Column=\"0\" />");
        Assert.IsTrue(session.Markup.StartsWith("<av:Grid", System.StringComparison.Ordinal));
        Assert.IsTrue(session.Markup.EndsWith("</av:Grid>", System.StringComparison.Ordinal));
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
        Assert.IsTrue(session.AddChildControl("Button"));

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
}
