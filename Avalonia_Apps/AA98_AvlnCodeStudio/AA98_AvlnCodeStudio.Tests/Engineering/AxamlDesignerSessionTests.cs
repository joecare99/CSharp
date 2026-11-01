using AA98_AvlnCodeStudio.Model.Workspaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
}
