using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using AA98_AvlnCodeStudio.Model.Workspaces;
using AA98_AvlnCodeStudio.UI.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Project.Explorer;
using Project.Explorer.Avalonia.ViewModels;
using Project.Explorer.FileSystem;

namespace AA98_AvlnCodeStudio.Tests.Engineering;

[TestClass]
public sealed class ResourceDependencyProjectExplorerTests
{
    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public async Task LiteralMetadataAttribute_AndConflictingConditionalUpdate(bool conditionalOverride)
    {
        string root = Path.Combine(Directory.GetCurrentDirectory(), $"ResourceExplorer-{Guid.NewGuid():N}");
        Directory.CreateDirectory(root);
        try
        {
            string project = Path.Combine(root, "App.csproj");
            string resource = Path.Combine(root, "Resources.resx");
            string designer = Path.Combine(root, "Resources.Designer.cs");
            await File.WriteAllTextAsync(resource, "");
            await File.WriteAllTextAsync(designer, "");
            await File.WriteAllTextAsync(Path.Combine(root, "Alternate.resx"), "");
            ResourceDependencyProjectExplorerSource source = new(new FileSystemProjectExplorerSource());

            string xml = conditionalOverride ? """
                <Project Sdk="Microsoft.NET.Sdk"><ItemGroup>
                  <Compile Update="Resources.Designer.cs"><DependentUpon>Resources.resx</DependentUpon></Compile>
                  <Compile Update="Resources.Designer.cs" Condition="'$(Configuration)' == 'Release'">
                    <DependentUpon>Alternate.resx</DependentUpon>
                  </Compile>
                </ItemGroup></Project>
                """ : """
                <Project Sdk="Microsoft.NET.Sdk"><ItemGroup>
                  <Compile Update="Resources.Designer.cs" DependentUpon="Resources.resx" />
                </ItemGroup></Project>
                """;
            await File.WriteAllTextAsync(project, xml);
            ProjectExplorerItem tree = (await source.RefreshAsync(project)).RootItems.Single();
            ProjectExplorerItem parent = tree.Children.Single(i => i.Path == resource);
            if (conditionalOverride)
            {
                Assert.IsTrue(tree.Children.Any(i => i.Path == designer));
                Assert.AreEqual(0, parent.Children.Count);
            }
            else
            {
                Assert.AreEqual(designer, parent.Children.Single().Path);
                Assert.IsFalse(tree.Children.Any(i => i.Path == designer));
            }
        }
        finally
        {
            Directory.Delete(root, true);
        }
    }

    [TestMethod]
    [DataRow("<ItemGroup Condition=\"'$(Configuration)' == 'Release'\"><Compile Update=\"Designer.cs\" DependentUpon=\"Other.resx\" /></ItemGroup>")]
    [DataRow("<ItemGroup><Compile Update=\"Designer.cs\"><DependentUpon Condition=\"'$(X)' == '1'\">Other.resx</DependentUpon></Compile></ItemGroup>")]
    [DataRow("<ItemGroup><Compile Update=\"Designer.cs\" DependentUpon=\"$(Resource).resx\" /></ItemGroup>")]
    [DataRow("<ItemGroup><Compile Update=\"Designer.cs\" DependentUpon=\"Other.resx\"><DependentUpon>Resource.resx</DependentUpon></Compile></ItemGroup>")]
    [DataRow("<Choose><When Condition=\"'$(X)' == '1'\" /><Otherwise><ItemGroup><Compile Update=\"Designer.cs\" DependentUpon=\"Other.resx\" /></ItemGroup></Otherwise></Choose>")]
    [DataRow("<Target Name=\"ChangeMetadata\"><ItemGroup><Compile Update=\"Designer.cs\" DependentUpon=\"Other.resx\" /></ItemGroup></Target>")]
    public async Task UncertainMetadataAssignment_DoesNotLeaveAnEarlierDependency(string uncertainAssignment)
    {
        string root = Path.Combine(Directory.GetCurrentDirectory(), $"ResourceExplorer-{Guid.NewGuid():N}");
        Directory.CreateDirectory(root);
        try
        {
            string project = Path.Combine(root, "App.csproj");
            await File.WriteAllTextAsync(project,
                "<Project><ItemGroup><Compile Update=\"Designer.cs\" DependentUpon=\"Resource.resx\" /></ItemGroup>" +
                uncertainAssignment + "</Project>");
            await File.WriteAllTextAsync(Path.Combine(root, "Resource.resx"), "");
            await File.WriteAllTextAsync(Path.Combine(root, "Other.resx"), "");
            await File.WriteAllTextAsync(Path.Combine(root, "Designer.cs"), "");

            ProjectExplorerItem tree = (await new ResourceDependencyProjectExplorerSource(new FileSystemProjectExplorerSource())
                .RefreshAsync(project)).RootItems.Single();
            Assert.IsTrue(tree.Children.Any(i => i.Name == "Designer.cs"));
            Assert.AreEqual(0, tree.Children.Single(i => i.Name == "Resource.resx").Children.Count);
        }
        finally
        {
            Directory.Delete(root, true);
        }
    }

    [TestMethod]
    public async Task Slnx_NestsLiteralResourceDependency_AndRefreshesWithoutDuplicates()
    {
        string root = Path.Combine(Directory.GetCurrentDirectory(), $"ResourceExplorer-{Guid.NewGuid():N}");
        Directory.CreateDirectory(Path.Combine(root, "App", "Properties"));
        try
        {
            string project = Path.Combine(root, "App", "App.csproj");
            string solution = Path.Combine(root, "App.slnx");
            await File.WriteAllTextAsync(solution, "<Solution><Folder Name=\"/Group/\"><Project Path=\"App/App.csproj\" /></Folder></Solution>");
            await File.WriteAllTextAsync(project, """
                <Project><ItemGroup>
                  <Compile Update="Properties\Resources.Designer.cs"><DependentUpon>Resources.resx</DependentUpon></Compile>
                </ItemGroup></Project>
                """);
            string properties = Path.Combine(root, "App", "Properties");
            await File.WriteAllTextAsync(Path.Combine(properties, "Resources.resx"), "");
            await File.WriteAllTextAsync(Path.Combine(properties, "Resources.Designer.cs"), "");
            ResourceDependencyProjectExplorerSource source = new(new FileSystemProjectExplorerSource());

            ProjectExplorerItem tree = (await source.RefreshAsync(solution)).RootItems.Single();
            ProjectExplorerItem folder = tree.Children.Single().Children.Single().Children.Single(i => i.Name == "Properties");
            ProjectExplorerItem resource = folder.Children.Single(i => i.Name == "Resources.resx");
            ProjectExplorerItem designer = resource.Children.Single();
            Assert.AreEqual("Resources.Designer.cs", designer.Name);
            Assert.IsFalse(folder.Children.Any(i => i.Name == designer.Name));

            await File.WriteAllTextAsync(project, "<Project />");
            tree = (await source.RefreshAsync(solution)).RootItems.Single();
            folder = tree.Children.Single().Children.Single().Children.Single(i => i.Name == "Properties");
            Assert.AreEqual(designer.Id, folder.Children.Single(i => i.Name == designer.Name).Id);
            Assert.AreEqual(0, folder.Children.Single(i => i.Name == "Resources.resx").Children.Count);
            File.Delete(Path.Combine(properties, "Resources.resx"));
            await File.WriteAllTextAsync(project, """
                <Project><ItemGroup><Compile Update="Properties\Resources.Designer.cs"><DependentUpon>Resources.resx</DependentUpon></Compile></ItemGroup></Project>
                """);
            tree = (await source.RefreshAsync(solution)).RootItems.Single();
            folder = tree.Children.Single().Children.Single().Children.Single(i => i.Name == "Properties");
            Assert.IsTrue(folder.Children.Any(i => i.Name == designer.Name));
        }
        finally
        {
            Directory.Delete(root, true);
        }
    }

    [TestMethod]
    public async Task AmbiguousConditionalAndInvalidDependencies_RemainUnnested()
    {
        string root = Path.Combine(Directory.GetCurrentDirectory(), $"ResourceExplorer-{Guid.NewGuid():N}");
        Directory.CreateDirectory(root);
        try
        {
            string project = Path.Combine(root, "App.csproj");
            await File.WriteAllTextAsync(project, """
                <Project><ItemGroup>
                  <Compile Update="Ambiguous.cs"><DependentUpon>A.resx</DependentUpon></Compile>
                  <Compile Update="Ambiguous.cs"><DependentUpon>B.resx</DependentUpon></Compile>
                  <Compile Update="Conditional.cs" Condition="'$(X)' == '1'"><DependentUpon>A.resx</DependentUpon></Compile>
                  <Compile Update="Cycle.cs"><DependentUpon>Cycle.cs</DependentUpon></Compile>
                  <Compile Update="Outside.cs"><DependentUpon>../A.resx</DependentUpon></Compile>
                  <Compile Update="Expression.cs"><DependentUpon>$(Name).resx</DependentUpon></Compile>
                </ItemGroup></Project>
                """);
            foreach (string file in new[] { "A.resx", "B.resx", "Ambiguous.cs", "Conditional.cs", "Cycle.cs", "Outside.cs", "Expression.cs" })
                await File.WriteAllTextAsync(Path.Combine(root, file), "");

            ProjectExplorerItem tree = (await new ResourceDependencyProjectExplorerSource(new FileSystemProjectExplorerSource())
                .RefreshAsync(project)).RootItems.Single();
            foreach (string file in new[] { "Ambiguous.cs", "Conditional.cs", "Cycle.cs", "Outside.cs", "Expression.cs" })
                Assert.IsTrue(tree.Children.Any(i => i.Name == file), file);
            Assert.AreEqual(0, tree.Children.Single(i => i.Name == "A.resx").Children.Count);
            Assert.AreEqual(0, tree.Children.Single(i => i.Name == "B.resx").Children.Count);
        }
        finally
        {
            Directory.Delete(root, true);
        }
    }

    [TestMethod]
    public async Task ProjectSwitch_DoesNotRetainDependenciesOrSelection()
    {
        string root = Path.Combine(Directory.GetCurrentDirectory(), $"ResourceExplorer-{Guid.NewGuid():N}");
        string first = Path.Combine(root, "first");
        string second = Path.Combine(root, "second");
        Directory.CreateDirectory(first);
        Directory.CreateDirectory(second);
        try
        {
            foreach (string directory in new[] { first, second })
            {
                await File.WriteAllTextAsync(Path.Combine(directory, "Resource.resx"), "");
                await File.WriteAllTextAsync(Path.Combine(directory, "Designer.cs"), "");
            }
            string firstProject = Path.Combine(first, "App.csproj");
            string secondProject = Path.Combine(second, "App.csproj");
            await File.WriteAllTextAsync(firstProject, """
                <Project><ItemGroup><Compile Update="Designer.cs"><DependentUpon>Resource.resx</DependentUpon></Compile></ItemGroup></Project>
                """);
            await File.WriteAllTextAsync(secondProject, "<Project />");
            using ProjectExplorerViewModel explorer = new(
                new ResourceDependencyProjectExplorerSource(new FileSystemProjectExplorerSource()),
                new ProjectExplorerState(), Substitute.For<IProjectExplorerItemOpener>());

            await explorer.LoadAsync(firstProject);
            ProjectExplorerItemViewModel resource = explorer.RootItems.Single().Children.Single(i => i.Name == "Resource.resx");
            Assert.AreEqual("Designer.cs", resource.Children.Single().Name);
            explorer.SelectedItem = resource;
            await explorer.LoadAsync(secondProject);
            Assert.IsNull(explorer.SelectedItem);
            Assert.AreEqual(secondProject, explorer.RootItems.Single().Item.Path);
            Assert.IsTrue(explorer.RootItems.Single().Children.Any(i => i.Name == "Designer.cs"));
            Assert.AreEqual(0, explorer.RootItems.Single().Children.Single(i => i.Name == "Resource.resx").Children.Count);

            using CancellationTokenSource cancellation = new();
            cancellation.Cancel();
            await Assert.ThrowsExactlyAsync<OperationCanceledException>(async () =>
                await new ResourceDependencyProjectExplorerSource(new FileSystemProjectExplorerSource())
                    .RefreshAsync(firstProject, cancellation.Token));
        }
        finally
        {
            Directory.Delete(root, true);
        }
    }

    [TestMethod]
    public async Task ProjectXml_WithDtdOrExcessiveSize_DoesNotApplyDependencies()
    {
        string root = Path.Combine(Directory.GetCurrentDirectory(), $"ResourceExplorer-{Guid.NewGuid():N}");
        Directory.CreateDirectory(root);
        try
        {
            string project = Path.Combine(root, "App.csproj");
            string resource = Path.Combine(root, "Resource.resx");
            string designer = Path.Combine(root, "Designer.cs");
            ProjectExplorerItem[] files =
            [
                new("resource", "Resource.resx", resource, ProjectExplorerItemKind.File),
                new("designer", "Designer.cs", designer, ProjectExplorerItemKind.File),
            ];
            ProjectExplorerItem projectItem = new("project", "App", project, ProjectExplorerItemKind.Project, children: files);
            IProjectExplorerSource inner = Substitute.For<IProjectExplorerSource>();
            inner.RefreshAsync(project, Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(new ProjectExplorerSnapshot(project, [projectItem])));
            ResourceDependencyProjectExplorerSource source = new(inner);
            const string body = "<Project><ItemGroup><Compile Update=\"Designer.cs\"><DependentUpon>Resource.resx</DependentUpon></Compile></ItemGroup></Project>";

            foreach (string xml in new[]
            {
                "<!DOCTYPE Project [<!ENTITY x \"ignored\">]>" + body,
                "<!--" + new string('x', 1_000_000) + "-->" + body,
                "<Project><ItemGroup>",
            })
            {
                await File.WriteAllTextAsync(project, xml);
                ProjectExplorerItem result = (await source.RefreshAsync(project)).RootItems.Single();
                Assert.AreEqual(2, result.Children.Count);
                Assert.AreEqual(0, result.Children.Single(i => i.Name == "Resource.resx").Children.Count);
            }
        }
        finally
        {
            Directory.Delete(root, true);
        }
    }

    [TestMethod]
    public async Task WorkspaceAndExplorer_RejectDtdAndOversizedXmlBeforeProjection()
    {
        string root = Path.Combine(Directory.GetCurrentDirectory(), $"ResourceExplorer-{Guid.NewGuid():N}");
        Directory.CreateDirectory(root);
        try
        {
            string project = Path.Combine(root, "App.csproj");
            string solution = Path.Combine(root, "App.slnx");
            FileSystemProjectExplorerSource source = new();
            foreach (string prefix in new[]
            {
                "<!DOCTYPE Solution [<!ENTITY x \"ignored\">]>",
                "<!--" + new string('x', 1_000_000) + "-->",
            })
            {
                await File.WriteAllTextAsync(solution, prefix + "<Solution><Project Path=\"App.csproj\" /></Solution>");
                await File.WriteAllTextAsync(project, "<Project />");
                Assert.ThrowsExactly<XmlException>(() => new CSharpWorkspaceLoader().Load(solution));
                await Assert.ThrowsExactlyAsync<XmlException>(() => source.RefreshAsync(solution));
            }

            await File.WriteAllTextAsync(solution, "<Solution><Project Path=\"App.csproj\" /></Solution>");
            foreach (string prefix in new[]
            {
                "<!DOCTYPE Project [<!ENTITY x \"ignored\">]>",
                "<!--" + new string('x', 1_000_000) + "-->",
            })
            {
                await File.WriteAllTextAsync(project, prefix + "<Project />");
                await Assert.ThrowsExactlyAsync<XmlException>(() => source.RefreshAsync(solution));
                await Assert.ThrowsExactlyAsync<XmlException>(() => source.RefreshAsync(project));
            }
        }
        finally
        {
            Directory.Delete(root, true);
        }
    }

    [TestMethod]
    public async Task AA05_Slnx_NestsDesignerWhenFixtureIsProvided()
    {
        string? solution = Environment.GetEnvironmentVariable("AA05_SOLUTION");
        if (string.IsNullOrEmpty(solution))
            Assert.Inconclusive("Set AA05_SOLUTION to the read-only AA05 .slnx fixture.");

        ProjectExplorerItem tree = (await new ResourceDependencyProjectExplorerSource(new FileSystemProjectExplorerSource())
            .RefreshAsync(solution)).RootItems.Single();
        ProjectExplorerItem project = tree.Children.Single(i => i.Kind == ProjectExplorerItemKind.Project && i.Name == "AA05_CommandParCalc");
        ProjectExplorerItem properties = project.Children.Single(i => i.Name == "Properties");
        ProjectExplorerItem resource = properties.Children.Single(i => i.Name == "Resources.resx");
        Assert.AreEqual("Resources.Designer.cs", resource.Children.Single(i => i.Name == "Resources.Designer.cs").Name);
        Assert.IsFalse(properties.Children.Any(i => i.Name == "Resources.Designer.cs"));
    }
}
