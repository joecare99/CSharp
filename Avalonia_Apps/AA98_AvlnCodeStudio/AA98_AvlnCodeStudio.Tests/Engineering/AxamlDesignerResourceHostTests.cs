using AA98_AxamlDesigner.Host.ViewModels;
using AA98_AvlnCodeStudio.Model.Workspaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;

namespace AA98_AvlnCodeStudio.Tests.Engineering;

[TestClass]
public sealed class AxamlDesignerResourceHostTests
{
    [TestMethod]
    public void StandaloneHost_UsesOwningProjectAcrossSaveAsAndWorkspaceChangesWithoutLosingHistory()
    {
        string fixture = Path.Combine(Environment.CurrentDirectory, "host-resource-context-" + Guid.NewGuid().ToString("N"));
        AxamlDesignerHostViewModel host = new();
        try
        {
            string first = CreateProject(fixture, "First", "First caption");
            string second = CreateProject(fixture, "Second", "Second caption");
            string firstView = Path.Combine(first, "Views", "View.axaml");
            string secondView = Path.Combine(second, "Views", "Saved.axaml");
            string solution = Path.Combine(fixture, "Workspace.slnx");
            File.WriteAllText(solution, """
                <Solution><Project Path="First\Sample.csproj"/><Project Path="Second\Sample.csproj"/></Solution>
                """);
            const string markup = """
                <StackPanel xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' xmlns:p='clr-namespace:Sample.Properties'>
                  <Button Width='100' Content='{x:Static p:Resources.Caption}'/>
                </StackPanel>
                """;
            File.WriteAllText(firstView, markup);

            Assert.IsTrue(host.OpenDocument(firstView));
            Assert.AreEqual("{x:Static p:Resources.Caption}", host.Designer.PreviewRoot!.Children[0].Text);
            Assert.IsTrue(host.SetWorkspaceEntry(solution));
            Assert.AreEqual("First caption", host.Designer.PreviewRoot!.Children[0].Text);

            host.Designer.SelectedNode = host.Designer.PreviewRoot.Children[0];
            host.Designer.SelectedProperty = "Width";
            host.Designer.SelectedValue = "120";
            host.Designer.ApplySelectedValueCommand.Execute(null);
            host.Designer.SelectedValue = "140";
            host.Designer.ApplySelectedValueCommand.Execute(null);
            host.Designer.UndoCommand.Execute(null);
            Assert.IsTrue(host.Designer.CanUndo);
            Assert.IsTrue(host.Designer.CanRedo);

            Assert.IsTrue(host.SaveDocumentAs(secondView));
            Assert.AreEqual("Second caption", host.Designer.PreviewRoot!.Children[0].Text);
            Assert.AreEqual("120", host.Designer.SelectedValue);
            Assert.IsTrue(host.Designer.CanUndo);
            Assert.IsTrue(host.Designer.CanRedo);
            host.Designer.RedoCommand.Execute(null);
            Assert.AreEqual("140", host.Designer.SelectedValue);
            Assert.AreEqual("Second caption", host.Designer.PreviewRoot!.Children[0].Text);

            host.ClearWorkspaceContext();
            Assert.AreEqual("{x:Static p:Resources.Caption}", host.Designer.PreviewRoot!.Children[0].Text);
            Assert.IsTrue(host.Designer.CanUndo);
            Assert.IsTrue(host.SetWorkspaceEntry(solution, fixture));
            Assert.AreEqual("Second caption", host.Designer.PreviewRoot!.Children[0].Text);
            Assert.IsTrue(host.Designer.CanUndo);

            string outsideView = Path.Combine(fixture + "-outside", "Other.axaml");
            Directory.CreateDirectory(Path.GetDirectoryName(outsideView)!);
            try
            {
                Assert.IsTrue(host.SaveDocumentAs(outsideView));
                Assert.AreEqual(string.Empty, host.WorkspaceRootPath);
                Assert.AreEqual("{x:Static p:Resources.Caption}", host.Designer.PreviewRoot!.Children[0].Text);
                Assert.IsTrue(host.Designer.CanUndo);
            }
            finally
            {
                Directory.Delete(Path.GetDirectoryName(outsideView)!, true);
            }
        }
        finally
        {
            host.ClearWorkspaceContext();
            if (Directory.Exists(fixture))
            {
                Directory.Delete(fixture, true);
            }
        }
    }

    [TestMethod]
    public void StandaloneHost_AmbiguousProjectDirectoryDoesNotResolveResources()
    {
        string fixture = Path.Combine(Environment.CurrentDirectory, "host-resource-ambiguous-" + Guid.NewGuid().ToString("N"));
        AxamlDesignerHostViewModel host = new();
        try
        {
            string project = CreateProject(fixture, "Shared", "Must not resolve");
            File.WriteAllText(Path.Combine(project, "Other.csproj"), "<Project />");
            string solution = Path.Combine(fixture, "Workspace.slnx");
            File.WriteAllText(solution,
                "<Solution><Project Path='Shared\\Sample.csproj'/><Project Path='Shared\\Other.csproj'/></Solution>");
            string view = Path.Combine(project, "Views", "View.axaml");
            File.WriteAllText(view,
                "<Button xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' xmlns:p='clr-namespace:Sample.Properties' Content='{x:Static p:Resources.Caption}'/>");
            Assert.IsTrue(host.OpenDocument(view));
            Assert.IsTrue(host.SetWorkspaceEntry(solution));
            Assert.AreEqual("{x:Static p:Resources.Caption}", host.Designer.PreviewRoot!.Text);
        }
        finally
        {
            host.ClearWorkspaceContext();
            if (Directory.Exists(fixture))
            {
                Directory.Delete(fixture, true);
            }
        }
    }

    [TestMethod]
    public void StandaloneHost_UsesClosestExistingProjectAndDoesNotGuessMissingProjects()
    {
        string fixture = Path.Combine(Environment.CurrentDirectory, "host-resource-nested-" + Guid.NewGuid().ToString("N"));
        AxamlDesignerHostViewModel host = new();
        try
        {
            string parent = CreateProject(fixture, "Parent", "Parent");
            string nested = CreateProject(parent, "Nested", "Nested");
            string view = Path.Combine(nested, "Views", "View.axaml");
            File.WriteAllText(view,
                "<Button xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' xmlns:p='clr-namespace:Sample.Properties' Content='{x:Static p:Resources.Caption}'/>");
            string solution = Path.Combine(fixture, "Workspace.slnx");
            File.WriteAllText(solution, """
                <Solution><Project Path="Parent\Sample.csproj"/><Project Path="Parent\Nested\Sample.csproj"/></Solution>
                """);
            Assert.IsTrue(host.OpenDocument(view));
            Assert.IsTrue(host.SetWorkspaceEntry(solution));
            Assert.AreEqual("Nested", host.Designer.PreviewRoot!.Text);

            host.ClearWorkspaceContext();
            File.Delete(Path.Combine(nested, "Sample.csproj"));
            string missingOnly = Path.Combine(fixture, "Missing.slnx");
            File.WriteAllText(missingOnly, "<Solution><Project Path='Parent\\Nested\\Missing.csproj'/></Solution>");
            Assert.IsTrue(host.SetWorkspaceEntry(missingOnly));
            Assert.AreEqual("{x:Static p:Resources.Caption}", host.Designer.PreviewRoot!.Text);
        }
        finally
        {
            host.ClearWorkspaceContext();
            if (Directory.Exists(fixture))
            {
                Directory.Delete(fixture, true);
            }
        }
    }

    [TestMethod]
    public void StandaloneHost_DoesNotUseProjectsOutsideTheSelectedWorkspace()
    {
        string fixture = Path.Combine(Environment.CurrentDirectory, "host-resource-external-" + Guid.NewGuid().ToString("N"));
        AxamlDesignerHostViewModel host = new();
        try
        {
            Directory.CreateDirectory(Path.Combine(fixture, "Properties"));
            string workspace = Path.Combine(fixture, "Workspace");
            Directory.CreateDirectory(workspace);
            File.WriteAllText(Path.Combine(fixture, "Sample.csproj"), "<Project />");
            File.WriteAllText(Path.Combine(fixture, "Properties", "Resources.resx"),
                "<root><data name='Caption'><value>Outside</value></data></root>");
            string solution = Path.Combine(workspace, "Workspace.slnx");
            File.WriteAllText(solution, "<Solution><Project Path='..\\Sample.csproj'/></Solution>");
            string view = Path.Combine(workspace, "View.axaml");
            File.WriteAllText(view,
                "<Button xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' xmlns:p='clr-namespace:Sample.Properties' Content='{x:Static p:Resources.Caption}'/>");

            Assert.IsTrue(host.OpenDocument(view));
            Assert.IsTrue(host.SetWorkspaceEntry(solution));
            Assert.AreEqual("{x:Static p:Resources.Caption}", host.Designer.PreviewRoot!.Text);
        }
        finally
        {
            host.ClearWorkspaceContext();
            if (Directory.Exists(fixture))
            {
                Directory.Delete(fixture, true);
            }
        }
    }

    [TestMethod]
    public void StandaloneHost_ResolvesReportedCalculatorSample()
    {
        const string root = @"C:\Projekte\CSharp\Avalonia_Apps\AA05_CommandParCalc\AA05_CommandParCalc";
        string view = Path.Combine(root, "Views", "CommandParCalculatorView.axaml");
        if (!File.Exists(view))
        {
            Assert.Inconclusive("The external read-only calculator sample is not present.");
        }

        AxamlDesignerHostViewModel host = new();
        try
        {
            Assert.IsTrue(host.SetWorkspaceEntry(Path.Combine(root, "AA05_CommandParCalc.csproj")));
            Assert.IsTrue(host.OpenDocument(view));
            AxamlPreviewNode button = Descendants(host.Designer.PreviewRoot!).Single(node => node.StableName == "btn7");
            Assert.AreEqual("7", button.Text);
            Assert.AreEqual("-5", button.RenderProperties["Margin"]);
            Assert.AreEqual("10", button.RenderProperties["Padding"]);
        }
        finally
        {
            host.ClearWorkspaceContext();
        }
    }

    private static string CreateProject(string fixture, string folder, string caption)
    {
        string root = Path.Combine(fixture, folder);
        Directory.CreateDirectory(Path.Combine(root, "Properties"));
        Directory.CreateDirectory(Path.Combine(root, "Views"));
        File.WriteAllText(Path.Combine(root, "Sample.csproj"), "<Project />");
        File.WriteAllText(Path.Combine(root, "Properties", "Resources.resx"),
            $"<root><data name='Caption'><value>{caption}</value></data></root>");
        return root;
    }

    private static System.Collections.Generic.IEnumerable<AxamlPreviewNode> Descendants(AxamlPreviewNode node) =>
        new[] { node }.Concat(node.Children.SelectMany(Descendants));
}
