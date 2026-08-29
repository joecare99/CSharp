using ConsoleLib.CommonControls;
using ConsoleLib.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace ConsoleLib.Tests;

[TestClass]
public class FormWidgetTests
{
    [TestMethod]
    public void RadioButtons_SelectExclusively()
    {
        var panel = new Panel();
        var first = new RadioButton();
        var second = new RadioButton();
        panel.Add(first);
        panel.Add(second);

        first.Select();
        second.Select();

        Assert.IsFalse(first.IsChecked);
        Assert.IsTrue(second.IsChecked);
    }

    [TestMethod]
    public void ComboBox_TraversesItems()
    {
        var combo = new ComboBox();
        combo.Items.Add("A");
        combo.Items.Add("B");

        Assert.IsTrue(combo.SelectNext());
        Assert.AreEqual("A", combo.SelectedItem);
        Assert.IsTrue(combo.SelectNext());
        Assert.AreEqual("B", combo.SelectedItem);
    }

    [TestMethod]
    public void FormControls_UseOptionalRenderer()
    {
        var widgetSet = Substitute.For<IWidgetSet, IFormWidgetRenderer>();
        var renderer = (IFormWidgetRenderer)widgetSet;
        widgetSet.ClipRect.Returns(System.Drawing.Rectangle.Empty);
        var app = new Application(widgetSet);
        app.Visible = true;

        var checkBox = new CheckBox { Parent = app };
        var comboBox = new ComboBox { Parent = app };
        var progressBar = new ProgressBar { Parent = app };
        var statusBar = new StatusBar { Parent = app };
        var tabControl = new TabControl { Parent = app };

        checkBox.Draw();
        comboBox.Draw();
        progressBar.Draw();
        statusBar.Draw();
        tabControl.Draw();

        renderer.Received(1).DrawCheckBox(checkBox);
        renderer.Received(1).DrawComboBox(comboBox);
        renderer.Received(1).DrawProgressBar(progressBar);
        renderer.Received(1).DrawStatusBar(statusBar);
        renderer.Received(1).DrawTabControl(tabControl);
    }
}
