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
            Assert.IsTrue(capability.EditableProperties.Contains("ClipToBounds"), capability.TypeName);
            AxamlPropertyDescriptor clipToBounds = AxamlPropertyValueValidator.Describe(node, "ClipToBounds");
            Assert.AreEqual(AxamlPropertyEditorKind.Boolean, clipToBounds.EditorKind, capability.TypeName);
            Assert.AreEqual("False", clipToBounds.Value, capability.TypeName);
            Assert.IsNull(clipToBounds.ValidationError, capability.TypeName);
            Assert.IsTrue(capability.EditableProperties.Contains("IsVisible"), capability.TypeName);
            AxamlPropertyDescriptor isVisible = AxamlPropertyValueValidator.Describe(node, "IsVisible");
            Assert.AreEqual(AxamlPropertyEditorKind.Boolean, isVisible.EditorKind, capability.TypeName);
            Assert.AreEqual("True", isVisible.Value, capability.TypeName);
            Assert.IsNull(isVisible.ValidationError, capability.TypeName);
            Assert.IsTrue(capability.EditableProperties.Contains("Margin"), capability.TypeName);
            Assert.AreEqual("0", AxamlPropertyValueValidator.Describe(node, "Margin").Value, capability.TypeName);
            Assert.IsTrue(capability.EditableProperties.Contains("HorizontalAlignment"), capability.TypeName);
            Assert.AreEqual("Stretch", AxamlPropertyValueValidator.Describe(node, "HorizontalAlignment").Value, capability.TypeName);
            Assert.IsTrue(capability.EditableProperties.Contains("VerticalAlignment"), capability.TypeName);
            Assert.AreEqual("Stretch", AxamlPropertyValueValidator.Describe(node, "VerticalAlignment").Value, capability.TypeName);
            foreach (string property in new[] { "Width", "Height", "MinWidth", "MaxWidth", "MinHeight", "MaxHeight" })
            {
                AxamlPropertyDescriptor size = AxamlPropertyValueValidator.Describe(node, property);
                Assert.AreEqual(string.Empty, size.Value, $"{capability.TypeName}.{property} default");
                Assert.IsNull(size.ValidationError, $"{capability.TypeName}.{property} default");
            }
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
                $"<{capability.TypeName} xmlns='https://github.com/avaloniaui' Width='120' Height='80' MinWidth='40' MaxWidth='160' MinHeight='30' MaxHeight='100' Margin='4,2' HorizontalAlignment='Right' VerticalAlignment='Center' ClipToBounds='True' IsVisible='False' />");
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
            AxamlPropertyDescriptor clipToBounds = AxamlPropertyValueValidator.Describe(configured, "ClipToBounds");
            Assert.IsFalse(clipToBounds.IsReadOnly, capability.TypeName);
            Assert.AreEqual("True", clipToBounds.Value, capability.TypeName);
            Assert.IsNull(AxamlPropertyValueValidator.Validate(capability.TypeName, "ClipToBounds", "False"));
            Assert.IsNotNull(AxamlPropertyValueValidator.Validate(capability.TypeName, "ClipToBounds", "sometimes"));
            AxamlPropertyDescriptor isVisible = AxamlPropertyValueValidator.Describe(configured, "IsVisible");
            Assert.IsFalse(isVisible.IsReadOnly, capability.TypeName);
            Assert.AreEqual("False", isVisible.Value, capability.TypeName);
            Assert.IsNull(AxamlPropertyValueValidator.Validate(capability.TypeName, "IsVisible", "True"));
            Assert.IsNotNull(AxamlPropertyValueValidator.Validate(capability.TypeName, "IsVisible", "sometimes"));
            foreach ((string property, string expected) in new[]
            {
                ("Width", "120"),
                ("Height", "80"),
                ("MinWidth", "40"),
                ("MaxWidth", "160"),
                ("MinHeight", "30"),
                ("MaxHeight", "100"),
            })
            {
                AxamlPropertyDescriptor size = AxamlPropertyValueValidator.Describe(configured, property);
                Assert.IsFalse(size.IsReadOnly, $"{capability.TypeName}.{property}");
                Assert.AreEqual(AxamlPropertyEditorKind.Number, size.EditorKind, $"{capability.TypeName}.{property}");
                Assert.AreEqual(expected, size.Value, $"{capability.TypeName}.{property}");
                Assert.IsNull(size.ValidationError, $"{capability.TypeName}.{property}");
                Assert.IsNull(AxamlPropertyValueValidator.Validate(capability.TypeName, property, "12.5"));
                Assert.IsNotNull(AxamlPropertyValueValidator.Validate(capability.TypeName, property, "-1"));
                Assert.IsNotNull(AxamlPropertyValueValidator.Validate(capability.TypeName, property, "NaN"));
            }

            AxamlPreviewNode bound = service.Parse(
                $"<{capability.TypeName} xmlns='https://github.com/avaloniaui' Width='{{Binding ControlWidth}}' Height='{{Binding ControlHeight}}' MinWidth='{{Binding MinimumWidth}}' MaxWidth='{{Binding MaximumWidth}}' MinHeight='{{Binding MinimumHeight}}' MaxHeight='{{Binding MaximumHeight}}' Margin='{{Binding OuterMargin}}' HorizontalAlignment='{{Binding HAlign}}' VerticalAlignment='{{Binding VAlign}}' ClipToBounds='{{Binding Clipping}}' IsVisible='{{Binding Visibility}}' />");
            foreach (string property in new[]
            {
                "Width", "Height", "MinWidth", "MaxWidth", "MinHeight", "MaxHeight",
                "Margin", "HorizontalAlignment", "VerticalAlignment",
                "ClipToBounds",
                "IsVisible",
            })
            {
                Assert.IsTrue(
                    AxamlPropertyValueValidator.Describe(bound, property).IsReadOnly,
                    $"{capability.TypeName}.{property} binding");
            }
        }
    }

    [TestMethod]
    public void Catalog_ExposesSizeConstraintsForEveryControl()
    {
        string[] actualControls = AxamlControlCatalog.All
            .Where(static capability => capability.EditableProperties.Contains("MinWidth"))
            .Select(static capability => capability.TypeName)
            .ToArray();

        CollectionAssert.AreEqual(
            AxamlControlCatalog.All.Select(static capability => capability.TypeName).ToArray(),
            actualControls);
        foreach (AxamlControlCapability capability in AxamlControlCatalog.All.Where(
            static item => item.EditableProperties.Contains("MinWidth")))
        {
            foreach (string property in new[] { "Width", "Height", "MinWidth", "MaxWidth", "MinHeight", "MaxHeight" })
            {
                Assert.IsTrue(capability.EditableProperties.Contains(property), $"{capability.TypeName}.{property}");
            }
        }
    }

    [TestMethod]
    public void Catalog_ExposesContentAlignmentForContentControls()
    {
        string[] contentControls =
        [
            "Button", "CheckBox", "ComboBox", "ContentControl", "Expander", "GroupBox",
            "Label", "NumericUpDown", "RadioButton", "TextBox", "ToggleSwitch",
        ];
        AxamlPreviewService service = new();

        foreach (string typeName in contentControls)
        {
            AxamlControlCapability capability = AxamlControlCatalog.All.Single(item => item.TypeName == typeName);
            AxamlPreviewNode unset = service.Parse($"<{typeName} xmlns='https://github.com/avaloniaui' />");
            foreach ((string property, string[] options) in new[]
            {
                ("HorizontalContentAlignment", new[] { "Left", "Center", "Right", "Stretch" }),
                ("VerticalContentAlignment", new[] { "Top", "Center", "Bottom", "Stretch" }),
            })
            {
                Assert.IsTrue(capability.EditableProperties.Contains(property), $"{typeName}.{property}");
                AxamlPropertyDescriptor descriptor = AxamlPropertyValueValidator.Describe(unset, property);
                Assert.AreEqual(AxamlPropertyEditorKind.Enum, descriptor.EditorKind, $"{typeName}.{property}");
                Assert.AreEqual(string.Empty, descriptor.Value, $"{typeName}.{property} preserves framework default");
                CollectionAssert.AreEqual(options, descriptor.Options.ToArray(), $"{typeName}.{property}");
                Assert.IsNull(descriptor.ValidationError, $"{typeName}.{property}");
                Assert.IsNull(AxamlPropertyValueValidator.Validate(typeName, property, options[0]));
                Assert.IsNotNull(AxamlPropertyValueValidator.Validate(typeName, property, "Invalid"));
            }

            AxamlPreviewNode bound = service.Parse(
                $"<{typeName} xmlns='https://github.com/avaloniaui' HorizontalContentAlignment='{{Binding HorizontalAlignment}}' VerticalContentAlignment='{{Binding VerticalAlignment}}' />");
            Assert.IsTrue(AxamlPropertyValueValidator.Describe(bound, "HorizontalContentAlignment").IsReadOnly, $"{typeName} horizontal binding");
            Assert.IsTrue(AxamlPropertyValueValidator.Describe(bound, "VerticalContentAlignment").IsReadOnly, $"{typeName} vertical binding");
        }
    }

    [TestMethod]
    public void Label_ExposesSafeTextProperties()
    {
        AxamlControlCapability capability = AxamlControlCatalog.All.Single(static item => item.TypeName == "Label");
        AxamlPreviewService service = new();
        AxamlPreviewNode unset = service.Parse("<Label xmlns='https://github.com/avaloniaui' />");

        foreach ((string property, AxamlPropertyEditorKind editor, string validValue, string defaultValue) in new[]
        {
            ("FontSize", AxamlPropertyEditorKind.Number, "18", string.Empty),
            ("FontStyle", AxamlPropertyEditorKind.Enum, "Italic", string.Empty),
            ("FontWeight", AxamlPropertyEditorKind.Enum, "SemiBold", "Normal"),
            ("Foreground", AxamlPropertyEditorKind.Text, "Blue", string.Empty),
            ("TextAlignment", AxamlPropertyEditorKind.Enum, "Right", string.Empty),
            ("TextTrimming", AxamlPropertyEditorKind.Enum, "WordEllipsis", string.Empty),
            ("TextWrapping", AxamlPropertyEditorKind.Enum, "NoWrap", string.Empty),
        })
        {
            Assert.IsTrue(capability.EditableProperties.Contains(property), $"Label.{property}");
            AxamlPropertyDescriptor descriptor = AxamlPropertyValueValidator.Describe(unset, property);
            Assert.AreEqual(editor, descriptor.EditorKind, $"Label.{property}");
            Assert.AreEqual(defaultValue, descriptor.Value, $"Label.{property} preserves its default");
            Assert.IsNull(descriptor.ValidationError, $"Label.{property}");
            Assert.IsNull(AxamlPropertyValueValidator.Validate("Label", property, validValue), $"Label.{property}");
        }

        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("Label", "FontSize", "-1"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("Label", "FontStyle", "Bold"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("Label", "Foreground", "Gradient"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("Label", "TextAlignment", "Unknown"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("Label", "TextTrimming", "Unknown"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("Label", "TextWrapping", "Unknown"));

        AxamlPreviewNode bound = service.Parse(
            "<Label xmlns='https://github.com/avaloniaui' FontSize='{Binding Size}' FontStyle='{Binding Style}' FontWeight='{Binding Weight}' Foreground='{Binding Brush}' TextAlignment='{Binding Alignment}' TextTrimming='{Binding Trimming}' TextWrapping='{Binding Wrapping}' />");
        foreach (string property in new[] { "FontSize", "FontStyle", "FontWeight", "Foreground", "TextAlignment", "TextTrimming", "TextWrapping" })
        {
            Assert.IsTrue(AxamlPropertyValueValidator.Describe(bound, property).IsReadOnly, $"Label.{property} binding");
        }
    }

    [TestMethod]
    public void TextBoxLineLimits_AreNonNegativeIntegerProperties()
    {
        AxamlControlCapability capability = AxamlControlCatalog.All.Single(static item => item.TypeName == "TextBox");
        AxamlPreviewService service = new();
        AxamlPreviewNode unset = service.Parse("<TextBox xmlns='https://github.com/avaloniaui' />");

        foreach (string property in new[] { "MinLines", "MaxLines" })
        {
            Assert.IsTrue(capability.EditableProperties.Contains(property));
            AxamlPropertyDescriptor descriptor = AxamlPropertyValueValidator.Describe(unset, property);
            Assert.AreEqual(AxamlPropertyEditorKind.Number, descriptor.EditorKind);
            Assert.AreEqual(string.Empty, descriptor.Value);
            Assert.IsNull(descriptor.ValidationError);
            Assert.IsNull(AxamlPropertyValueValidator.Validate("TextBox", property, "0"));
            Assert.IsNull(AxamlPropertyValueValidator.Validate("TextBox", property, "3"));
            Assert.IsNotNull(AxamlPropertyValueValidator.Validate("TextBox", property, "-1"));
            Assert.IsNotNull(AxamlPropertyValueValidator.Validate("TextBox", property, "1.5"));
        }

        AxamlPreviewNode bound = service.Parse(
            "<TextBox xmlns='https://github.com/avaloniaui' MinLines='{Binding MinimumLines}' MaxLines='{Binding MaximumLines}' />");
        Assert.IsTrue(AxamlPropertyValueValidator.Describe(bound, "MinLines").IsReadOnly);
        Assert.IsTrue(AxamlPropertyValueValidator.Describe(bound, "MaxLines").IsReadOnly);
    }

    [TestMethod]
    public void TextBoxLineHeightAndFloatingPlaceholder_AreSafelyEditable()
    {
        AxamlControlCapability capability = AxamlControlCatalog.All.Single(static item => item.TypeName == "TextBox");
        AxamlPreviewService service = new();
        AxamlPreviewNode unset = service.Parse("<TextBox xmlns='https://github.com/avaloniaui' />");

        Assert.IsTrue(capability.EditableProperties.Contains("LineHeight"));
        AxamlPropertyDescriptor lineHeight = AxamlPropertyValueValidator.Describe(unset, "LineHeight");
        Assert.AreEqual(AxamlPropertyEditorKind.Number, lineHeight.EditorKind);
        Assert.AreEqual(string.Empty, lineHeight.Value);
        Assert.IsNull(lineHeight.ValidationError);
        Assert.IsNull(AxamlPropertyValueValidator.Validate("TextBox", "LineHeight", "18"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("TextBox", "LineHeight", "0"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("TextBox", "LineHeight", "NaN"));

        Assert.IsTrue(capability.EditableProperties.Contains("UseFloatingPlaceholder"));
        AxamlPropertyDescriptor floatingPlaceholder = AxamlPropertyValueValidator.Describe(unset, "UseFloatingPlaceholder");
        Assert.AreEqual(AxamlPropertyEditorKind.Boolean, floatingPlaceholder.EditorKind);
        Assert.AreEqual("False", floatingPlaceholder.Value);
        Assert.IsNull(floatingPlaceholder.ValidationError);
        Assert.IsNull(AxamlPropertyValueValidator.Validate("TextBox", "UseFloatingPlaceholder", "True"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("TextBox", "UseFloatingPlaceholder", "sometimes"));

        AxamlPreviewNode bound = service.Parse(
            "<TextBox xmlns='https://github.com/avaloniaui' LineHeight='{Binding TextLineHeight}' UseFloatingPlaceholder='{Binding FloatingPlaceholder}' />");
        Assert.IsTrue(AxamlPropertyValueValidator.Describe(bound, "LineHeight").IsReadOnly);
        Assert.IsTrue(AxamlPropertyValueValidator.Describe(bound, "UseFloatingPlaceholder").IsReadOnly);
    }

    [TestMethod]
    public void TextBoxPlaceholderForeground_AcceptsOnlySafeColorLiterals()
    {
        AxamlControlCapability capability = AxamlControlCatalog.All.Single(static item => item.TypeName == "TextBox");
        AxamlPreviewService service = new();
        AxamlPreviewNode unset = service.Parse("<TextBox xmlns='https://github.com/avaloniaui' />");
        Assert.IsTrue(capability.EditableProperties.Contains("PlaceholderForeground"));
        AxamlPropertyDescriptor descriptor = AxamlPropertyValueValidator.Describe(unset, "PlaceholderForeground");
        Assert.AreEqual(AxamlPropertyEditorKind.Text, descriptor.EditorKind);
        Assert.AreEqual(string.Empty, descriptor.Value);
        Assert.IsNull(descriptor.ValidationError);
        Assert.IsNull(AxamlPropertyValueValidator.Validate("TextBox", "PlaceholderForeground", "DarkGray"));
        Assert.IsNull(AxamlPropertyValueValidator.Validate("TextBox", "PlaceholderForeground", "#FF777777"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("TextBox", "PlaceholderForeground", "Gradient"));

        AxamlPreviewNode bound = service.Parse(
            "<TextBox xmlns='https://github.com/avaloniaui' PlaceholderForeground='{Binding PlaceholderBrush}' />");
        Assert.IsTrue(AxamlPropertyValueValidator.Describe(bound, "PlaceholderForeground").IsReadOnly);
    }

    [TestMethod]
    public void Catalog_FlowDirectionIsSafeAndEditableForEveryControl()
    {
        AxamlPreviewService service = new();
        foreach (AxamlControlCapability capability in AxamlControlCatalog.All)
        {
            Assert.IsTrue(capability.EditableProperties.Contains("FlowDirection"), capability.TypeName);
            AxamlPreviewNode unset = service.Parse($"<{capability.TypeName} xmlns='https://github.com/avaloniaui' />");
            AxamlPropertyDescriptor descriptor = AxamlPropertyValueValidator.Describe(unset, "FlowDirection");
            Assert.AreEqual(AxamlPropertyEditorKind.Enum, descriptor.EditorKind, capability.TypeName);
            Assert.AreEqual(string.Empty, descriptor.Value, $"{capability.TypeName} inherits its direction by default");
            CollectionAssert.AreEqual(new[] { "LeftToRight", "RightToLeft" }, descriptor.Options.ToArray(), capability.TypeName);
            Assert.IsNull(descriptor.ValidationError, capability.TypeName);
            Assert.IsNull(AxamlPropertyValueValidator.Validate(capability.TypeName, "FlowDirection", "RightToLeft"));
            Assert.IsNotNull(AxamlPropertyValueValidator.Validate(capability.TypeName, "FlowDirection", "Inherit"));

            AxamlPreviewNode configured = service.Parse(
                $"<{capability.TypeName} xmlns='https://github.com/avaloniaui' FlowDirection='RightToLeft' />");
            Assert.AreEqual("RightToLeft", AxamlPropertyValueValidator.Describe(configured, "FlowDirection").Value, capability.TypeName);
            AxamlPreviewNode bound = service.Parse(
                $"<{capability.TypeName} xmlns='https://github.com/avaloniaui' FlowDirection='{{Binding CurrentFlowDirection}}' />");
            Assert.IsTrue(AxamlPropertyValueValidator.Describe(bound, "FlowDirection").IsReadOnly, capability.TypeName);
        }
    }

    [TestMethod]
    public void Catalog_UseLayoutRoundingIsBooleanForEveryControl()
    {
        AxamlPreviewService service = new();
        foreach (AxamlControlCapability capability in AxamlControlCatalog.All)
        {
            Assert.IsTrue(capability.EditableProperties.Contains("UseLayoutRounding"), capability.TypeName);
            AxamlPreviewNode unset = service.Parse($"<{capability.TypeName} xmlns='https://github.com/avaloniaui' />");
            AxamlPropertyDescriptor descriptor = AxamlPropertyValueValidator.Describe(unset, "UseLayoutRounding");
            Assert.AreEqual(AxamlPropertyEditorKind.Boolean, descriptor.EditorKind, capability.TypeName);
            Assert.AreEqual(string.Empty, descriptor.Value, $"{capability.TypeName} keeps the Avalonia default");
            Assert.IsNull(descriptor.ValidationError, capability.TypeName);
            Assert.IsNull(AxamlPropertyValueValidator.Validate(capability.TypeName, "UseLayoutRounding", "True"));
            Assert.IsNull(AxamlPropertyValueValidator.Validate(capability.TypeName, "UseLayoutRounding", "False"));
            Assert.IsNotNull(AxamlPropertyValueValidator.Validate(capability.TypeName, "UseLayoutRounding", "sometimes"));

            AxamlPreviewNode bound = service.Parse(
                $"<{capability.TypeName} xmlns='https://github.com/avaloniaui' UseLayoutRounding='{{Binding LayoutRounding}}' />");
            Assert.IsTrue(AxamlPropertyValueValidator.Describe(bound, "UseLayoutRounding").IsReadOnly, capability.TypeName);
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
