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
            Assert.IsTrue(capability.EditableProperties.Contains("Opacity"), capability.TypeName);
            Assert.AreEqual("1", AxamlPropertyValueValidator.Describe(node, "Opacity").Value, capability.TypeName);
            Assert.IsTrue(capability.EditableProperties.Contains("Margin"), capability.TypeName);
            Assert.AreEqual("0", AxamlPropertyValueValidator.Describe(node, "Margin").Value, capability.TypeName);
            Assert.IsTrue(capability.EditableProperties.Contains("HorizontalAlignment"), capability.TypeName);
            Assert.AreEqual("Stretch", AxamlPropertyValueValidator.Describe(node, "HorizontalAlignment").Value, capability.TypeName);
            Assert.IsTrue(capability.EditableProperties.Contains("VerticalAlignment"), capability.TypeName);
            Assert.AreEqual("Stretch", AxamlPropertyValueValidator.Describe(node, "VerticalAlignment").Value, capability.TypeName);
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
    public void Catalog_ExposesSafeLayoutPropertiesForEveryControl()
    {
        AxamlPreviewService service = new();
        foreach (AxamlControlCapability capability in AxamlControlCatalog.All)
        {
            AxamlPreviewNode configured = service.Parse(
                $"<{capability.TypeName} xmlns='https://github.com/avaloniaui' Margin='4,2' HorizontalAlignment='Right' VerticalAlignment='Center' />");
            AxamlPropertyDescriptor margin = AxamlPropertyValueValidator.Describe(configured, "Margin");
            Assert.IsFalse(margin.IsReadOnly, capability.TypeName);
            Assert.AreEqual(AxamlPropertyEditorKind.Text, margin.EditorKind, capability.TypeName);
            Assert.AreEqual("4,2", margin.Value, capability.TypeName);
            Assert.IsNull(margin.ValidationError, capability.TypeName);
            Assert.IsNull(AxamlPropertyValueValidator.Validate(capability.TypeName, "Margin", "-1,2"));
            Assert.IsNotNull(AxamlPropertyValueValidator.Validate(capability.TypeName, "Margin", "1,2,3"));
            AxamlPropertyDescriptor horizontal = AxamlPropertyValueValidator.Describe(configured, "HorizontalAlignment");
            AxamlPropertyDescriptor vertical = AxamlPropertyValueValidator.Describe(configured, "VerticalAlignment");
            Assert.AreEqual("Right", horizontal.Value, capability.TypeName);
            Assert.AreEqual("Center", vertical.Value, capability.TypeName);
            Assert.IsNull(AxamlPropertyValueValidator.Validate(capability.TypeName, "HorizontalAlignment", "Right"));
            Assert.IsNull(AxamlPropertyValueValidator.Validate(capability.TypeName, "VerticalAlignment", "Center"));
            Assert.IsNotNull(AxamlPropertyValueValidator.Validate(capability.TypeName, "HorizontalAlignment", "Bottom"));
            Assert.IsNotNull(AxamlPropertyValueValidator.Validate(capability.TypeName, "VerticalAlignment", "Left"));

            AxamlPreviewNode bound = service.Parse(
                $"<{capability.TypeName} xmlns='https://github.com/avaloniaui' Margin='{{Binding OuterMargin}}' HorizontalAlignment='{{Binding HAlign}}' VerticalAlignment='{{Binding VAlign}}' />");
            foreach (string property in new[] { "Margin", "HorizontalAlignment", "VerticalAlignment" })
            {
                Assert.IsTrue(
                    AxamlPropertyValueValidator.Describe(bound, property).IsReadOnly,
                    $"{capability.TypeName}.{property} binding");
            }
        }
    }

    [TestMethod]
    public void Catalog_ExposesSizeConstraintsOnlyWhereSizeEditingIsSupported()
    {
        string[] expectedControls =
        [
            "Border",
            "Button",
            "CheckBox",
            "Grid",
            "GroupBox",
            "Label",
            "RadioButton",
            "StackPanel",
            "TextBlock",
            "TextBox",
            "UniformGrid",
            "Viewbox",
            "WrapPanel",
        ];
        string[] actualControls = AxamlControlCatalog.All
            .Where(static capability => capability.EditableProperties.Contains("MinWidth"))
            .Select(static capability => capability.TypeName)
            .ToArray();

        CollectionAssert.AreEqual(expectedControls, actualControls);
        foreach (AxamlControlCapability capability in AxamlControlCatalog.All.Where(
            static item => item.EditableProperties.Contains("MinWidth")))
        {
            foreach (string property in new[] { "MinWidth", "MaxWidth", "MinHeight", "MaxHeight" })
            {
                Assert.IsTrue(capability.EditableProperties.Contains(property), $"{capability.TypeName}.{property}");
            }
        }
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

    [TestMethod]
    public void Catalog_OnlyEnablesSafeRemovalForSupportedSingleChildContainers()
    {
        string[] actual = AxamlControlCatalog.All
            .Where(static capability => capability.AllowsSafeChildRemoval)
            .Select(static capability => capability.TypeName)
            .OrderBy(static typeName => typeName, StringComparer.Ordinal)
            .ToArray();

        CollectionAssert.AreEqual(
            new[] { "Border", "ContentControl", "Expander", "GroupBox", "ScrollViewer", "Viewbox" },
            actual);
    }
}
