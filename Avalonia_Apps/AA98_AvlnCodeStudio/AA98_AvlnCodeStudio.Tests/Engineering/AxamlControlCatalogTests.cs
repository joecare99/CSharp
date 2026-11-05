using AA98_AvlnCodeStudio.Model.Workspaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace AA98_AvlnCodeStudio.Tests.Engineering;

[TestClass]
public sealed class AxamlControlCatalogTests
{
    [TestMethod]
    public void Catalog_MatchesPreviewEditingAndBindingCapabilities()
    {
        string controls = string.Join(
            string.Empty,
            AxamlControlCatalog.All.Select(static capability => $"<{capability.TypeName} />"));
        AxamlPreviewNode root = new AxamlPreviewService().Parse(
            $"<Window xmlns='https://github.com/avaloniaui'>{controls}</Window>");
        Assert.AreEqual(AxamlControlCatalog.All.Count, root.Children.Count);

        foreach (AxamlControlCapability capability in AxamlControlCatalog.All)
        {
            Assert.IsTrue(AxamlControlCatalog.TryGet(capability.TypeName, out AxamlControlCapability found));
            Assert.AreSame(capability, found);
            AxamlPreviewNode node = capability.TypeName == root.TypeName
                ? root
                : root.Children.Single(child => child.TypeName == capability.TypeName);
            Assert.IsTrue(node.IsSupported, capability.TypeName);
            CollectionAssert.AreEqual(
                capability.EditableProperties.ToArray(),
                new AxamlPreviewService().GetEditableProperties(node).ToArray(),
                capability.TypeName);
        }

        string bindings = string.Join(
            string.Empty,
            AxamlControlCatalog.All.SelectMany((capability, controlIndex) =>
                capability.BindableProperties.Select((property, propertyIndex) =>
                    $"<{capability.TypeName} {property.Name}='{{Binding Member{controlIndex}_{propertyIndex}}}' />")));
        AxamlMvvmMemberRequirement[] requirements = new AxamlMvvmBindingAnalyzer()
            .Analyze($"<Window xmlns='https://github.com/avaloniaui'>{bindings}</Window>")
            .ToArray();
        int expectedBindingCount = AxamlControlCatalog.All.Sum(static capability =>
            capability.BindableProperties.Count);
        Assert.AreEqual(expectedBindingCount, requirements.Length);
    }

    [TestMethod]
    public void TryGet_RejectsUnknownControlNames()
    {
        Assert.IsFalse(AxamlControlCatalog.TryGet("MyCustomControl", out _));
    }

    [TestMethod]
    public void Catalog_ListsCurrentNamedControlMvvmGeneratorSupport()
    {
        string[] expected =
        [
            "Button",
            "CheckBox",
            "ComboBox",
            "NumericUpDown",
            "RadioButton",
            "Slider",
            "TextBlock",
            "TextBox",
            "ToggleSwitch",
        ];
        string[] actual = AxamlControlCatalog.All
            .Where(static capability => capability.SupportsMvvmGeneration)
            .Select(static capability => capability.TypeName)
            .OrderBy(static typeName => typeName, StringComparer.Ordinal)
            .ToArray();

        CollectionAssert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void Catalog_OnlyEnablesAppendingForExplicitlySupportedLayoutPanels()
    {
        string[] actual = AxamlControlCatalog.All
            .Where(static capability => capability.AllowsAppendingChildren)
            .Select(static capability => capability.TypeName)
            .OrderBy(static typeName => typeName, StringComparer.Ordinal)
            .ToArray();

        CollectionAssert.AreEqual(new[] { "DockPanel", "StackPanel", "UniformGrid", "WrapPanel" }, actual);
    }
}
