using System;
using ConsoleLib.Cxaml.Designer.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Avalonia.Controls;
using Avalonia;
using Avalonia.Headless;
using System.IO;

namespace ConsoleLib.Cxaml.DesignerTests;

[TestClass]
public sealed class DesignerViewModelTests
{
    [TestMethod]
    public void PreviewCreatesLiveControlAndExposesEditableProperties()
    {
        var viewModel = new DesignerViewModel();

        Assert.IsNotNull(viewModel.PreviewControl);
        StringAssert.Contains(viewModel.Preview, "StackPanel");
        CollectionAssert.Contains(viewModel.InspectorProperties.ToArray(), "Width");
    }

    [TestMethod]
    public void ApplyingInspectorPropertyUpdatesLivePreview()
    {
        var viewModel = new DesignerViewModel
        {
            SelectedPropertyName = "Width",
            SelectedPropertyValue = "55"
        };

        viewModel.ApplySelectedPropertyCommand.Execute(null);

        Assert.AreEqual(55, viewModel.PreviewControl!.size.Width);
        Assert.AreEqual("Property applied.", viewModel.InspectorStatus);
    }

    [TestMethod]
    public void InvalidMarkupClearsPreviewAndReportsDiagnostics()
    {
        var viewModel = new DesignerViewModel
        {
            Markup = "<NotAControl />"
        };

        Assert.IsNull(viewModel.PreviewControl);
        Assert.AreEqual("Preview unavailable", viewModel.Preview);
        StringAssert.Contains(viewModel.InspectorStatus, "Preview validation failed");
        StringAssert.Contains(viewModel.Diagnostics, "Unsupported CXAML control");
    }

    [TestMethod]
    public void RenderedPreviewUsesStableSourceMappings()
    {
        var viewModel = new DesignerViewModel
        {
            Markup = "<StackPanel><Button Text=\"One\" /><Label Text=\"Two\" /></StackPanel>"
        };

        Assert.IsTrue(viewModel.PreviewState.IsAvailable);
        StringAssert.Contains(viewModel.VirtualAxaml, "<StackPanel");
        StringAssert.Contains(viewModel.InspectorStatus, "Preview rendered");
        CollectionAssert.AreEqual(
            new[] { "root", "root/0", "root/1" },
            viewModel.PreviewMappings.Select(mapping => mapping.Id).ToArray());
        Assert.IsInstanceOfType(viewModel.RenderedPreview, typeof(StackPanel));
        Assert.IsInstanceOfType(viewModel.PreviewMappings[1].PreviewControl, typeof(Button));
    }

    [TestMethod]
    public void PreviewSelectionUpdatesInspectorAndSource()
    {
        var viewModel = new DesignerViewModel
        {
            Markup = "<StackPanel><Button Name=\"save\" Text=\"Save\" /></StackPanel>"
        };

        Assert.IsTrue(viewModel.ActivatePreviewSelection("root/0"));
        Assert.AreEqual("root/0", viewModel.SelectedPreviewControlId);
        StringAssert.Contains(viewModel.SelectedElement, "Button");
        CollectionAssert.Contains(viewModel.InspectorProperties.ToArray(), "Text");

        viewModel.SelectedPropertyName = "Text";
        viewModel.SelectedPropertyValue = "Updated";
        viewModel.ApplySelectedPropertyCommand.Execute(null);

        StringAssert.Contains(viewModel.Markup, "Text=\"Updated\"");
        Assert.AreEqual("Updated", viewModel.PreviewControl!.Children.Single().Text);
    }

    [TestMethod]
    public void InvalidMarkupRemovesRenderedPreviewAndMappings()
    {
        var viewModel = new DesignerViewModel();

        viewModel.Markup = "<NotAControl />";

        Assert.IsNull(viewModel.RenderedPreview);
        Assert.AreEqual(0, viewModel.PreviewMappings.Count);
        Assert.IsFalse(viewModel.PreviewState.IsAvailable);
    }

    [TestMethod]
    public void RenderedPreviewCanBeHostedOnAvaloniaHeadlessPlatform()
    {
        AppBuilder.Configure<ConsoleLib.Cxaml.Designer.App>()
            .UseHeadless(new AvaloniaHeadlessPlatformOptions())
            .SetupWithoutStarting();

        var viewModel = new DesignerViewModel
        {
            Markup = "<StackPanel><Button Text=\"Preview\" /></StackPanel>"
        };

        Assert.IsNotNull(viewModel.RenderedPreview);
        Assert.IsInstanceOfType(viewModel.RenderedPreview, typeof(StackPanel));
    }

    [TestMethod]
    public void PanelRootIsRenderedWithPositionedChildren()
    {
        var viewModel = new DesignerViewModel
        {
            Markup = "<Panel Width=\"20\" Height=\"5\"><Button Text=\"Preview\" X=\"3\" Y=\"2\" /></Panel>"
        };

        Assert.IsNotNull(viewModel.RenderedPreview);
        Assert.IsInstanceOfType(viewModel.RenderedPreview, typeof(Border));
        StringAssert.Contains(viewModel.VirtualAxaml, "<Panel");
        StringAssert.Contains(viewModel.VirtualAxaml, "Width=\"20\"");
        StringAssert.Contains(viewModel.VirtualAxaml, "BackColor=\"Black\"");
        StringAssert.Contains(viewModel.VirtualAxaml, "ForeColor=\"Black\"");
        Assert.AreEqual(24d, viewModel.PreviewMappings[1].PreviewControl.GetValue(Canvas.LeftProperty));
        Assert.AreEqual(56d, viewModel.PreviewMappings[1].PreviewControl.GetValue(Canvas.TopProperty));
    }

    [TestMethod]
    public void PreviewRendersViewsWithUnresolvedBindings()
    {
        var viewModel = new DesignerViewModel
        {
            Markup = "<Panel><Button Text=\"{Binding Caption}\" Command=\"{Binding RunCommand}\" /></Panel>"
        };

        Assert.IsTrue(viewModel.PreviewState.IsAvailable);
        Assert.AreEqual("[Caption]", viewModel.PreviewMappings[1].ConsoleControl.Text);
        Assert.AreEqual("[Caption]", ((Button)viewModel.PreviewMappings[1].PreviewControl).Content);
        StringAssert.Contains(viewModel.InspectorStatus, "Preview rendered");
    }

    [TestMethod]
    public void PanelPreviewKeepsLowerControlsInsideScrollableCanvas()
    {
        var viewModel = new DesignerViewModel
        {
            Markup = "<Panel Width=\"100\" Height=\"30\"><TextBox Text=\"Prompt\" Width=\"65\" Height=\"1\" X=\"0\" Y=\"25\" /><Button Text=\"Send\" Width=\"9\" Height=\"1\" X=\"66\" Y=\"25\" /></Panel>"
        };

        var textBox = viewModel.PreviewMappings[1].PreviewControl;
        var button = viewModel.PreviewMappings[2].PreviewControl;

        Assert.AreEqual(700d, textBox.GetValue(Canvas.TopProperty));
        Assert.AreEqual(528d, button.GetValue(Canvas.LeftProperty));
        Assert.IsTrue(textBox.Width >= 520);
        Assert.IsTrue(button.Width >= 72);
    }

    [TestMethod]
    public void PanelRootWithoutDimensionsReceivesVisiblePreviewLayout()
    {
        var viewModel = new DesignerViewModel
        {
            Markup = "<Panel><Button Text=\"Preview\" X=\"3\" Y=\"2\" /></Panel>"
        };

        var rendered = viewModel.RenderedPreview!;
        rendered.Measure(new Size(400, 300));
        rendered.Arrange(new Rect(0, 0, 400, 300));

        Assert.IsInstanceOfType(rendered, typeof(Border));
        Assert.IsTrue(rendered.Bounds.Width >= 640);
        Assert.IsTrue(rendered.Bounds.Height >= 450);
        var panelCanvas = ((Border)rendered).Child as Canvas;
        Assert.IsNotNull(panelCanvas);
        Assert.AreEqual(1, panelCanvas.Children.Count);
        Assert.IsTrue(viewModel.PreviewMappings[1].PreviewControl.Bounds.Width > 0);
        Assert.IsTrue(viewModel.PreviewMappings[1].PreviewControl.Bounds.Height > 0);
    }

    [TestMethod]
    public void LoadAndSaveCommandsRoundTripCxamlFile()
    {
        var path = Path.Combine(Path.GetTempPath(), "consolelib-designer-" + Guid.NewGuid().ToString("N") + ".cxaml");
        try
        {
            var viewModel = new DesignerViewModel { FilePath = path, Markup = "<Label Text=\"Saved\" />" };
            viewModel.SaveFileCommand.Execute(null);
            viewModel.Markup = "<Label Text=\"Changed\" />";
            viewModel.LoadFileCommand.Execute(null);

            Assert.AreEqual("<Label Text=\"Saved\" />", viewModel.Markup);
            Assert.AreEqual("CXAML file loaded.", viewModel.InspectorStatus);
        }
        finally
        {
            if (File.Exists(path))
                File.Delete(path);
        }
    }
}
