using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Input;
using ConsoleLib;
using ConsoleLib.CommonControls;
using ConsoleLib.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConsoleLib.CoreTests;

[TestClass]
public sealed class CxamlBindingTests
{
    [TestMethod]
    public void LoadContext_ResolvesNamesTextListAndCommands()
    {
        var model = new BindingModel();
        var result = new CxamlLoader().Load(new StringReader(
            "<Panel Name=\"Root\"><Label Name=\"Message\" Text=\"{Binding Message}\" /><Button Name=\"Run\" Command=\"{Binding RunCommand}\" /><ListBox Name=\"Items\" ItemsSource=\"{Binding Items}\" /></Panel>"),
            new CxamlLoadContext(model));

        Assert.IsInstanceOfType(result.Root, typeof(Panel));
        Assert.AreEqual(4, result.NamedControls.Count);
        Assert.AreEqual("Initial", result.NamedControls["Message"].Text);
        Assert.IsInstanceOfType(result.NamedControls["Run"], typeof(Button));
        Assert.AreEqual(2, ((ListBox)result.NamedControls["Items"]).GetItemCount());
    }

    [TestMethod]
    public void LoadContext_RejectsMissingBindingTarget()
    {
        try
        {
            _ = new CxamlLoader().Load(
                new StringReader("<Label Text=\"{Binding Missing}\" />"),
                new CxamlLoadContext(new BindingModel()));
            Assert.Fail("A missing CXAML binding target must fail.");
        }
        catch (CxamlParseException)
        {
        }
    }

    [TestMethod]
    public void DesignLoadContext_PreservesUnresolvedBindingAsPlaceholder()
    {
        var root = new CxamlLoader().Load(
            new StringReader("<Panel><Button Text=\"{Binding Caption}\" Command=\"{Binding RunCommand}\" /></Panel>"),
            new CxamlLoadContext(new object(), allowUnresolvedBindings: true)).Root;

        Assert.AreEqual("[Caption]", root.Children[0].Text);
    }

    [TestMethod]
    public void Load_AppliesControlDimensionsAtomically()
    {
        var root = new CxamlLoader().Load(new StringReader(
            "<Panel><Terminal Width=\"56\" Height=\"8\" X=\"2\" Y=\"3\" /></Panel>"));

        var terminal = root.Children[0] as Terminal;

        Assert.IsNotNull(terminal);
        Assert.AreEqual(56, terminal.size.Width);
        Assert.AreEqual(8, terminal.size.Height);
        Assert.AreEqual(2, terminal.Position.X);
        Assert.AreEqual(3, terminal.Position.Y);
    }

    [TestMethod]
    public void LoadContext_TextBoxBindingUpdatesWritableProperty()
    {
        var model = new BindingModel();
        var result = new CxamlLoader().Load(
            new StringReader("<TextBox Name=\"Input\" Text=\"{Binding Input}\" />"),
            new CxamlLoadContext(model));

        result.NamedControls["Input"].Text = "Updated";

        Assert.AreEqual("Updated", model.Input);
    }

    [TestMethod]
    public void Load_AppliesInteractionAndBorderAttributes()
    {
        var root = new CxamlLoader().Load(new StringReader(
            "<Panel Tag=\"root-tag\" Accelerator=\"p\" Shadow=\"true\" BorderStyle=\"Double\" BorderColor=\"Green\">" +
            "<Button Tag=\"save\" Accelerator=\"s\" Shadow=\"true\" HLBackColor=\"Cyan\" />" +
            "</Panel>"));

        var panel = (Panel)root;
        var button = (Button)panel.Children[0];

        Assert.AreEqual("root-tag", panel.Tag);
        Assert.AreEqual('p', panel.Accelerator);
        Assert.IsTrue(panel.Shadow);
        Assert.AreEqual(BorderStyle.Double, panel.BorderStyle);
        Assert.AreEqual(ConsoleColor.Green, panel.BorderColor);
        Assert.AreEqual("save", button.Tag);
        Assert.AreEqual('s', button.Accelerator);
        Assert.IsTrue(button.Shadow);
        Assert.AreEqual(ConsoleColor.Cyan, button.HLBackColor);
    }

    private sealed class BindingModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public string Message { get; } = "Initial";
        public string Input { get; set; } = string.Empty;
        public System.Collections.IList Items { get; } = new[] { "One", "Two" };
        public ICommand RunCommand { get; } = new DelegateCommand();
    }

    private sealed class DelegateCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged;
        public bool CanExecute(object? parameter) => true;
        public void Execute(object? parameter) { }
    }
}
