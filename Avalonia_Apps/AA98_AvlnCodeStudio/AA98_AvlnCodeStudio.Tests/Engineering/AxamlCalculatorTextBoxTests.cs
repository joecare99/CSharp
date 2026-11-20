using AA98_AvlnCodeStudio.AxamlDesigner.UI.Controls;
using AA98_AvlnCodeStudio.AxamlDesigner.UI.ViewModels;
using AA98_AvlnCodeStudio.Model.Workspaces;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.Tests.Engineering;

[TestClass]
public sealed class AxamlCalculatorTextBoxTests
{
    [TestMethod]
    public async Task RealCalculator_TextBoxesPreserveBindingsAndHistory()
    {
        string root = Environment.GetEnvironmentVariable("AA98_CALCULATOR_SAMPLE_ROOT")
            ?? @"C:\Projekte\CSharp\Avalonia_Apps\AA05_CommandParCalc\AA05_CommandParCalc";
        string path = Path.Combine(root, "Views", "CommandParCalculatorView.axaml");
        if (!File.Exists(path))
        {
            Assert.Inconclusive("The external read-only calculator sample is not present.");
        }

        string markup = File.ReadAllText(path);
        foreach (string member in new[] { "Memory", "Accumulator" })
        {
            string binding = $"{{Binding {member}, ConverterParameter='', Converter={{StaticResource DoubleValueConverter}}, Mode=OneWay}}";
            AxamlDesignerViewModel designer = new();
            string? published = null;
            designer.MarkupChanged += (_, args) => published = args.Markup;
            designer.LoadMarkup(markup);
            int offset = markup.IndexOf(binding, StringComparison.Ordinal);
            Assert.IsTrue(offset >= 0, member);
            designer.SelectAtCaret(markup, offset);
            AxamlPreviewNode node = designer.SelectedNode!;
            Assert.AreEqual("TextBox", node.TypeName);
            Assert.AreEqual(binding, node.Properties["Text"]);
            Assert.AreEqual("Right", node.Properties["TextAlignment"]);
            Assert.AreEqual("Wrap", node.Properties["TextWrapping"]);

            AxamlPreviewControl preview = new() { PreviewNode = designer.PreviewRoot };
            Grid wrapper = (Grid)preview.GetLogicalDescendants().OfType<Border>()
                .Single(border => ReferenceEquals(border.Tag, node)).Child!;
            TextBox rendered = (TextBox)wrapper.Children[0];
            Assert.AreEqual(TextAlignment.Right, rendered.TextAlignment);
            Assert.AreEqual(TextWrapping.Wrap, rendered.TextWrapping);

            var alignment = designer.PropertyProvider.Properties.Single(property => property.Name == "TextAlignment");
            Assert.IsTrue(alignment.TrySetValue("Center"));
            await designer.PropertyProvider.ApplyAsync();
            Assert.AreEqual(binding, designer.SelectedNode!.Properties["Text"]);
            Assert.AreEqual("Center", designer.SelectedNode.Properties["TextAlignment"]);
            string edited = published!;
            int alignmentOffset = markup.IndexOf("TextAlignment=\"Right\"", offset, StringComparison.Ordinal);
            Assert.IsTrue(alignmentOffset > offset);
            string expected = markup.Remove(alignmentOffset, "TextAlignment=\"Right\"".Length)
                .Insert(alignmentOffset, "TextAlignment=\"Center\"");
            Assert.AreEqual(expected, edited);
            var wrapping = designer.PropertyProvider.Properties.Single(property => property.Name == "TextWrapping");
            Assert.IsTrue(wrapping.TrySetValue("NoWrap"));
            await designer.PropertyProvider.ApplyAsync();
            int wrappingOffset = edited.IndexOf("TextWrapping=\"Wrap\"", offset, StringComparison.Ordinal);
            Assert.IsTrue(wrappingOffset > offset);
            string bothEdited = edited.Remove(wrappingOffset, "TextWrapping=\"Wrap\"".Length)
                .Insert(wrappingOffset, "TextWrapping=\"NoWrap\"");
            Assert.AreEqual(bothEdited, published);
            Assert.AreEqual(binding, designer.SelectedNode!.Properties["Text"]);
            designer.UndoCommand.Execute(null);
            Assert.AreEqual(edited, published);
            designer.UndoCommand.Execute(null);
            Assert.AreEqual(markup, published);
            Assert.AreEqual(binding, designer.SelectedNode!.Properties["Text"]);
            designer.RedoCommand.Execute(null);
            Assert.AreEqual(edited, published);
            designer.RedoCommand.Execute(null);
            Assert.AreEqual(bothEdited, published);
            Assert.AreEqual(binding, designer.SelectedNode!.Properties["Text"]);
        }
    }
}
