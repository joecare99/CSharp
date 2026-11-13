using AA98_AvlnCodeStudio.Model.Workspaces;
using AA98_AvlnCodeStudio.AxamlDesigner.UI.Controls;
using AA98_AvlnCodeStudio.AxamlDesigner.UI.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;
using System.IO;
using System.Linq;

namespace AA98_AvlnCodeStudio.Tests.Engineering;

[TestClass]
public sealed class AxamlPreviewResourcesTests
{
    [TestMethod]
    public void LocalStylesAndResources_AreInertScopedAndDoNotChangeSourceProperties()
    {
        const string markup = """
            <UserControl xmlns='https://github.com/avaloniaui' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' xmlns:evil='clr-namespace:Evil'>
              <UserControl.Resources>
                <x:String x:Key='Caption'>Hello</x:String>
                <x:String x:Key='LoopA'>{StaticResource LoopB}</x:String>
                <x:String x:Key='LoopB'>{StaticResource LoopA}</x:String>
                <ResourceDictionary Source='https://example.invalid/resources.axaml' />
                <evil:Converter x:Key='Danger' />
              </UserControl.Resources>
              <UserControl.Styles><Style Selector='Button'><Setter Property='Margin' Value='-5'/><Setter Property='Padding' Value='10'/></Style>
                <Style Selector='Button.hot'><Setter Property='Margin' Value='2'/></Style>
                <Style Selector='Button:pointerover'><Setter Property='Padding' Value='99'/></Style>
                <StyleInclude Source='https://example.invalid/styles.axaml'/></UserControl.Styles>
              <UserControl.DataContext><evil:ViewModel/></UserControl.DataContext>
              <StackPanel><Button Classes='hot' Content='{StaticResource Caption}' Padding='3'/>
                <Button Content='{StaticResource LoopA}'/><Button Content='{StaticResource Danger}'/></StackPanel>
            </UserControl>
            """;
        AxamlPreviewNode root = new AxamlPreviewService().Parse(markup);
        Assert.AreEqual("StackPanel", root.TypeName);
        AxamlPreviewNode button = root.Children[0];
        Assert.AreEqual("Hello", button.Text);
        Assert.AreEqual("{StaticResource Caption}", button.Properties["Content"]);
        Assert.AreEqual("2", button.RenderProperties["Margin"]);
        Assert.AreEqual("3", button.RenderProperties["Padding"]);
        Assert.AreEqual("10", root.Children[1].RenderProperties["Padding"]);
        StringAssert.Contains(root.Children[1].Text!, "StaticResource LoopA");
        StringAssert.Contains(root.Children[2].Text!, "StaticResource Danger");
    }

    [TestMethod]
    public void Resx_UsesCultureFallbackAndIgnoresTypedDataExternalAndDtd()
    {
        string root = Path.Combine(Environment.CurrentDirectory, "preview-resources-" + Guid.NewGuid().ToString("N"));
        CultureInfo previous = CultureInfo.CurrentUICulture;
        try
        {
            Directory.CreateDirectory(Path.Combine(root, "Properties"));
            Directory.CreateDirectory(Path.Combine(root, "Views"));
            File.WriteAllText(Path.Combine(root, "Sample.csproj"), "<Project />");
            string document = Path.Combine(root, "Views", "View.axaml");
            File.WriteAllText(document, "");
            File.WriteAllText(Path.Combine(root, "Properties", "Resources.resx"),
                "<root><data name='Neutral'><value>neutral</value></data><data name='Typed' type='System.String'><value>unsafe</value></data><data name='Duplicate'><value>A</value></data><data name='Duplicate' type='System.String'><value>B</value></data></root>");
            File.WriteAllText(Path.Combine(root, "Properties", "Resources.de.resx"),
                "<root><data name='Greeting'><value>Hallo</value></data></root>");
            File.WriteAllText(Path.Combine(root, "Properties", "Resources.de-CH.resx"),
                "<!DOCTYPE root [<!ENTITY x SYSTEM 'https://example.invalid/evil'>]><root><data name='Greeting'><value>&x;</value></data></root>");
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("de-CH");
            const string markup = """
                <StackPanel xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' xmlns:p='clr-namespace:Sample.Properties'>
                  <Button Content='{x:Static p:Resources.Greeting}'/><Button Content='{x:Static p:Resources.Neutral}'/>
                  <Button Content='{x:Static p:Resources.Typed}'/><Button Content='{x:Static p:Resources.Duplicate}'/>
                  <Button Content='{x:Static p:Resources.Missing}'/>
                </StackPanel>
                """;
            AxamlPreviewNode preview = new AxamlPreviewService().Parse(markup, document, root);
            Assert.AreEqual("Hallo", preview.Children[0].Text);
            Assert.AreEqual("neutral", preview.Children[1].Text);
            Assert.AreEqual("{x:Static p:Resources.Typed}", preview.Children[2].Text);
            Assert.AreEqual("{x:Static p:Resources.Duplicate}", preview.Children[3].Text);
            Assert.AreEqual("{x:Static p:Resources.Missing}", preview.Children[4].Text);
            AxamlDesignerSession session = new();
            session.Load(markup);
            session.SetResourceContext(document, root);
            Assert.AreEqual("Hallo", session.Root!.Children[0].Text);
            session.SelectNode(session.Root.Children[0]);
            session.SelectProperty("Margin");
            Assert.IsTrue(session.ApplySelectedValue("4"));
            Assert.AreEqual("Hallo", session.Root!.Children[0].Text);
            AxamlPreviewNode noContext = new AxamlPreviewService().Parse(markup, document, Path.Combine(root, "Views"));
            Assert.AreEqual("{x:Static p:Resources.Greeting}", noContext.Children[0].Text);
            AxamlPreviewNode external = new AxamlPreviewService().Parse(markup.Replace("clr-namespace:Sample.Properties", "clr-namespace:Other.Properties"), document, root);
            Assert.AreEqual("{x:Static p:Resources.Greeting}", external.Children[0].Text);
        }
        finally
        {
            CultureInfo.CurrentUICulture = previous;
            if (Directory.Exists(root)) Directory.Delete(root, true);
        }
    }

    [TestMethod]
    public void DesignerContextChange_RefreshesPreviewAndPreservesEditHistory()
    {
        string root = Path.Combine(Environment.CurrentDirectory, "preview-context-" + Guid.NewGuid().ToString("N"));
        try
        {
            Directory.CreateDirectory(Path.Combine(root, "Properties"));
            Directory.CreateDirectory(Path.Combine(root, "Views"));
            File.WriteAllText(Path.Combine(root, "Sample.csproj"), "<Project />");
            string document = Path.Combine(root, "Views", "View.axaml");
            File.WriteAllText(document, "");
            File.WriteAllText(Path.Combine(root, "Properties", "Resources.resx"),
                "<root><data name='Caption'><value>Localized</value></data></root>");
            string markup = "<StackPanel xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' "
                + "xmlns:p='clr-namespace:Sample.Properties'>"
                + "<Button Width='100' Content='{x:Static p:Resources.Caption}'/></StackPanel>";
            AxamlDesignerViewModel designer = new();
            designer.MarkupChanged += (_, args) => markup = args.Markup;
            designer.LoadMarkup(markup);
            designer.SelectedNode = designer.PreviewRoot!.Children[0];
            designer.SelectedProperty = "Width";
            designer.SelectedValue = "120";
            designer.ApplySelectedValueCommand.Execute(null);
            Assert.IsTrue(designer.CanUndo);

            designer.SetResourceContext(document, root);
            Assert.AreEqual("Localized", designer.PreviewRoot!.Children[0].Text);
            Assert.AreEqual("120", designer.SelectedValue);
            Assert.IsTrue(designer.CanUndo);
            designer.UndoCommand.Execute(null);
            Assert.AreEqual("100", designer.SelectedValue);
            Assert.AreEqual("Localized", designer.PreviewRoot!.Children[0].Text);
            Assert.IsTrue(designer.CanRedo);
            designer.RedoCommand.Execute(null);
            Assert.AreEqual("120", designer.SelectedValue);

            designer.SetResourceContext(null, null);
            Assert.AreEqual("{x:Static p:Resources.Caption}", designer.PreviewRoot!.Children[0].Text);
            Assert.IsTrue(designer.CanUndo);
        }
        finally
        {
            if (Directory.Exists(root))
            {
                Directory.Delete(root, true);
            }
        }
    }

    [TestMethod]
    public void ReportedCalculator_ResolvesRealNeutralResxAndLocalStyle()
    {
        const string root = @"C:\Projekte\CSharp\Avalonia_Apps\AA05_CommandParCalc\AA05_CommandParCalc";
        string view = Path.Combine(root, "Views", "CommandParCalculatorView.axaml");
        if (!File.Exists(view))
        {
            Assert.Inconclusive("The external read-only calculator sample is not present.");
        }

        CultureInfo previous = CultureInfo.CurrentUICulture;
        try
        {
            CultureInfo.CurrentUICulture = CultureInfo.InvariantCulture;
            AxamlPreviewNode preview = new AxamlPreviewService().Parse(File.ReadAllText(view), view, root);
            AxamlPreviewNode button = Descendants(preview).Single(node => node.StableName == "btn7");
            Assert.AreEqual("7", button.Text);
            Assert.AreEqual("-5", button.RenderProperties["Margin"]);
            Assert.AreEqual("10", button.RenderProperties["Padding"]);
            AxamlPreviewControl control = new() { PreviewNode = preview };
            Button renderedButton = control.GetLogicalDescendants().OfType<Button>()
                .Single(item => Equals(item.Content, "7"));
            Assert.AreEqual(new Thickness(-5), renderedButton.Margin);
            Assert.AreEqual(new Thickness(10), renderedButton.Padding);
            Assert.AreEqual("C/CE", Descendants(preview).Single(node => node.StableName == "btnC_CE").Text);
            Assert.AreEqual(".", Descendants(preview).Single(node => node.StableName == "btnDecp").Text);
            Assert.IsFalse(Descendants(preview).Any(node => node.TypeName is "DoubleValueConverter" or "CommandParCalculatorViewModel"));
            Assert.AreEqual(6, preview.RowDefinitions.Count);
            Assert.AreEqual(10, preview.ColumnDefinitions.Count);
            Assert.IsTrue(preview.RowDefinitions.All(value => value == "*"));
            Assert.IsTrue(preview.ColumnDefinitions.All(value => value == "*"));
        }
        finally
        {
            CultureInfo.CurrentUICulture = previous;
        }
    }

    private static System.Collections.Generic.IEnumerable<AxamlPreviewNode> Descendants(AxamlPreviewNode node) =>
        new[] { node }.Concat(node.Children.SelectMany(Descendants));
}
