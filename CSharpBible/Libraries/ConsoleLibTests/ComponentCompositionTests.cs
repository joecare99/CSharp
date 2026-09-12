using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using ConsoleLib.CommonControls;
using ConsoleLib.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConsoleLib.Tests;

[TestClass]
public sealed class ComponentCompositionTests
{
    [TestMethod]
    public void LoaderLoadsTypedPageAndUserControlRoots()
    {
        var loader = new CxamlLoader();
        var context = new CxamlLoadContext(new object());

        var page = loader.LoadPage(new StringReader("<Page><Label Text=\"Page\" /></Page>"), context);
        var userControl = loader.LoadUserControl(
            new StringReader("<UserControl><Label Text=\"Reusable\" /></UserControl>"),
            context);

        Assert.IsInstanceOfType(page.Root, typeof(Page));
        Assert.IsInstanceOfType(userControl.Root, typeof(UserControl));
        Assert.AreEqual("Reusable", userControl.Root.Children[0].Text);
    }

    [TestMethod]
    public void LoaderRejectsWrongTypedRoot()
    {
        var loader = new CxamlLoader();

        var error = Assert.ThrowsExactly<CxamlParseException>(() =>
            loader.LoadDialog(
                new StringReader("<Panel />"),
                new CxamlLoadContext(new object())));

        StringAssert.Contains(error.Message, "Dialog");
    }

    [TestMethod]
    public void FrameReplacesContentAndSynchronizesSize()
    {
        var frame = new Frame { size = new Size(20, 5) };
        var first = new Page { size = new Size(2, 2) };
        var second = new Page();

        frame.SetContent(first);
        Assert.AreSame(first, frame.Content);
        Assert.AreSame(frame, first.Parent);
        Assert.AreEqual(frame.size, first.size);

        frame.SetContent(second);

        Assert.AreSame(second, frame.Content);
        Assert.IsNull(first.Parent);
        Assert.AreSame(frame, second.Parent);
        Assert.AreEqual(frame.size, second.size);
    }

    [TestMethod]
    public void DialogManagerTracksActivationAndOwnerCleanup()
    {
        var host = new Panel();
        var owner = new Page();
        host.Add(owner);
        var manager = new DialogManager();
        var first = new Dialog();
        var second = new Dialog();

        var firstSession = manager.Open(first, owner, modal: false);
        var secondSession = manager.Open(second, owner, modal: false);

        host.Add(first);
        host.Add(second);
        manager.Activate(firstSession);
        Assert.IsTrue(first.Active);
        Assert.IsTrue(second.Visible);
        Assert.IsTrue(firstSession.ZIndex > secondSession.ZIndex);

        manager.CloseOwnedBy(owner);

        Assert.AreEqual(0, manager.Sessions.Count);
        Assert.IsFalse(first.Visible);
        Assert.IsFalse(second.Visible);
    }

    [TestMethod]
    public void DialogManagerAttachesUnparentedDialogToOwner()
    {
        var owner = new Panel();
        var dialog = new Dialog();
        var manager = new DialogManager();

        manager.Open(dialog, owner, modal: false);

        Assert.AreSame(owner, dialog.Parent);
        Assert.IsTrue(dialog.Visible);
    }

    [TestMethod]
    public void DialogManagerAllowsOnlyOneModalSession()
    {
        var manager = new DialogManager();
        manager.Open(new Dialog(), modal: true);

        Assert.ThrowsExactly<InvalidOperationException>(() =>
            manager.Open(new Dialog(), modal: true));
    }

    [TestMethod]
    public void DialogResourceLoaderCreatesFreshDataContextBoundInstances()
    {
        var loader = new DialogResourceLoader();
        var first = loader.Load("<Dialog><Label Text=\"Shared\" /></Dialog>", new object());
        var second = loader.Load("<Dialog><Label Text=\"Shared\" /></Dialog>", new object());

        Assert.AreNotSame(first, second);
        Assert.AreEqual("Shared", first.Children[0].Text);
        Assert.AreEqual("Shared", second.Children[0].Text);
    }

    [TestMethod]
    public async Task FrameNavigationCreatesNamedCxamlPages()
    {
        var services = new EmptyServiceProvider();
        var frame = new Frame();
        var navigation = new FrameNavigationService(services);
        navigation.Register("main", frame);
        navigation.RegisterPage(PageDescriptor.FromCxaml("details", "<Page><Label Text=\"Details\" /></Page>"));

        await navigation.NavigateAsync("main", new NavigationRequest("details"));

        Assert.IsInstanceOfType(frame.Content, typeof(Page));
        Assert.AreEqual("Details", frame.Content!.Children[0].Text);
    }

    [TestMethod]
    public void LoaderUsesExplicitRegisteredPrefixedComponentFactory()
    {
        var registry = new CxamlComponentRegistry();
        registry.Register("forms:AddressEditor", (_, _) =>
        {
            var control = new UserControl();
            control.Add(new Label { Text = "Address" });
            return control;
        });

        var result = new CxamlLoader(registry).Load(
            new StringReader("<Panel xmlns:forms=\"clr-namespace:Forms\"><forms:AddressEditor /></Panel>"),
            new CxamlLoadContext(new object()));

        Assert.IsInstanceOfType(result.Root.Children[0], typeof(UserControl));
        Assert.AreEqual("Address", result.Root.Children[0].Children[0].Text);
    }

    private sealed class EmptyServiceProvider : IServiceProvider
    {
        public object? GetService(Type serviceType) => null;
    }
}
