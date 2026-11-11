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
            Assert.IsTrue(capability.EditableProperties.Contains("IsEnabled"), capability.TypeName);
            AxamlPropertyDescriptor isEnabled = AxamlPropertyValueValidator.Describe(node, "IsEnabled");
            Assert.AreEqual(AxamlPropertyEditorKind.Boolean, isEnabled.EditorKind, capability.TypeName);
            Assert.AreEqual("True", isEnabled.Value, capability.TypeName);
            Assert.IsNull(isEnabled.ValidationError, capability.TypeName);
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
                $"<{capability.TypeName} xmlns='https://github.com/avaloniaui' Width='120' Height='80' MinWidth='40' MaxWidth='160' MinHeight='30' MaxHeight='100' Margin='4,2' HorizontalAlignment='Right' VerticalAlignment='Center' ClipToBounds='True' IsEnabled='False' IsVisible='False' />");
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
            AxamlPropertyDescriptor isEnabled = AxamlPropertyValueValidator.Describe(configured, "IsEnabled");
            Assert.IsFalse(isEnabled.IsReadOnly, capability.TypeName);
            Assert.AreEqual(AxamlPropertyEditorKind.Boolean, isEnabled.EditorKind, capability.TypeName);
            Assert.AreEqual("False", isEnabled.Value, capability.TypeName);
            Assert.IsNull(AxamlPropertyValueValidator.Validate(capability.TypeName, "IsEnabled", "True"));
            Assert.IsNotNull(AxamlPropertyValueValidator.Validate(capability.TypeName, "IsEnabled", "sometimes"));
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
                $"<{capability.TypeName} xmlns='https://github.com/avaloniaui' Width='{{Binding ControlWidth}}' Height='{{Binding ControlHeight}}' MinWidth='{{Binding MinimumWidth}}' MaxWidth='{{Binding MaximumWidth}}' MinHeight='{{Binding MinimumHeight}}' MaxHeight='{{Binding MaximumHeight}}' Margin='{{Binding OuterMargin}}' HorizontalAlignment='{{Binding HAlign}}' VerticalAlignment='{{Binding VAlign}}' ClipToBounds='{{Binding Clipping}}' IsEnabled='{{Binding Enabled}}' IsVisible='{{Binding Visibility}}' />");
            foreach (string property in new[]
            {
                "Width", "Height", "MinWidth", "MaxWidth", "MinHeight", "MaxHeight",
                "Margin", "HorizontalAlignment", "VerticalAlignment",
                "ClipToBounds",
                "IsEnabled",
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
    public void Catalog_ExposesSafeAppearanceForTemplatedControls()
    {
        string[] templatedControls =
        [
            "ContentControl", "Expander", "GroupBox", "Label", "ListBox", "Menu", "MenuItem",
            "TabControl", "TabItem", "TreeView", "TreeViewItem",
        ];
        (string Property, string Value, string InvalidValue, AxamlPropertyEditorKind Editor, string DefaultValue)[] properties =
        [
            ("Background", "AliceBlue", "Gradient", AxamlPropertyEditorKind.Text, string.Empty),
            ("BorderBrush", "#FF000000", "Gradient", AxamlPropertyEditorKind.Text, string.Empty),
            ("BorderThickness", "1,2,3,4", "-1", AxamlPropertyEditorKind.Text, string.Empty),
            ("CornerRadius", "4,2,4,2", "-1", AxamlPropertyEditorKind.Text, string.Empty),
            ("FontFamily", "DejaVu Sans", "Not/Allowed", AxamlPropertyEditorKind.Text, string.Empty),
            ("FontSize", "18", "-1", AxamlPropertyEditorKind.Number, string.Empty),
            ("FontStyle", "Italic", "Bold", AxamlPropertyEditorKind.Enum, string.Empty),
            ("FontWeight", "SemiBold", "Unknown", AxamlPropertyEditorKind.Enum, "Normal"),
            ("Foreground", "Blue", "Gradient", AxamlPropertyEditorKind.Text, string.Empty),
            ("Padding", "6,3", "-1", AxamlPropertyEditorKind.Text, string.Empty),
        ];
        AxamlPreviewService service = new();

        foreach (string typeName in templatedControls)
        {
            AxamlControlCapability capability = AxamlControlCatalog.All.Single(item => item.TypeName == typeName);
            AxamlPreviewNode unset = service.Parse($"<{typeName} xmlns='https://github.com/avaloniaui' />");
            foreach ((string property, string value, string invalidValue, AxamlPropertyEditorKind editor, string defaultValue) in properties)
            {
                Assert.IsTrue(capability.EditableProperties.Contains(property), $"{typeName}.{property}");
                AxamlPropertyDescriptor descriptor = AxamlPropertyValueValidator.Describe(unset, property);
                Assert.AreEqual(editor, descriptor.EditorKind, $"{typeName}.{property}");
                Assert.AreEqual(defaultValue, descriptor.Value, $"{typeName}.{property} preserves its default");
                Assert.IsNull(descriptor.ValidationError, $"{typeName}.{property}");
                Assert.IsNull(AxamlPropertyValueValidator.Validate(typeName, property, value), $"{typeName}.{property}");
                Assert.IsNotNull(AxamlPropertyValueValidator.Validate(typeName, property, invalidValue), $"{typeName}.{property}");
            }

            AxamlPreviewNode bound = service.Parse(
                $"<{typeName} xmlns='https://github.com/avaloniaui' Background='{{Binding Background}}' BorderBrush='{{Binding BorderBrush}}' BorderThickness='{{Binding BorderThickness}}' CornerRadius='{{Binding CornerRadius}}' FontFamily='{{Binding FontFamily}}' FontSize='{{Binding FontSize}}' FontStyle='{{Binding FontStyle}}' FontWeight='{{Binding FontWeight}}' Foreground='{{Binding Foreground}}' Padding='{{Binding Padding}}' />");
            foreach (var (property, _, _, _, _) in properties)
            {
                Assert.IsTrue(AxamlPropertyValueValidator.Describe(bound, property).IsReadOnly, $"{typeName}.{property} binding");
            }
        }
    }

    [TestMethod]
    public void TemplatedItemContainers_ExposeSafeCornerRadius()
    {
        string[] controlTypes = ["Menu", "MenuItem", "TabControl", "TreeView", "TreeViewItem"];
        AxamlPreviewService service = new();

        foreach (string controlType in controlTypes)
        {
            AxamlControlCapability capability = AxamlControlCatalog.All.Single(item => item.TypeName == controlType);
            AxamlPreviewNode unset = service.Parse($"<{controlType} />");
            AxamlPropertyDescriptor defaultValue = AxamlPropertyValueValidator.Describe(unset, "CornerRadius");
            Assert.IsTrue(capability.EditableProperties.Contains("CornerRadius"), controlType);
            Assert.AreEqual(AxamlPropertyEditorKind.Text, defaultValue.EditorKind, controlType);
            Assert.AreEqual(string.Empty, defaultValue.Value, controlType);
            Assert.IsNull(AxamlPropertyValueValidator.Validate(controlType, "CornerRadius", "4,2,4,2"), controlType);
            Assert.IsNotNull(AxamlPropertyValueValidator.Validate(controlType, "CornerRadius", "-1"), controlType);

            AxamlPreviewNode configured = service.Parse($"<{controlType} CornerRadius='4,2,4,2' />");
            Assert.AreEqual("4,2,4,2", AxamlPropertyValueValidator.Describe(configured, "CornerRadius").Value, controlType);

            AxamlPreviewNode bound = service.Parse($"<{controlType} CornerRadius='{{Binding Radius}}' />");
            Assert.IsTrue(AxamlPropertyValueValidator.Describe(bound, "CornerRadius").IsReadOnly, controlType);
        }
    }

    [TestMethod]
    public void SelectorSelectedIndex_IsSafeAndDefaultsToNoSelection()
    {
        AxamlPreviewService service = new();
        foreach (string typeName in new[] { "ComboBox", "ListBox", "TabControl" })
        {
            AxamlControlCapability capability = AxamlControlCatalog.All.Single(item => item.TypeName == typeName);
            AxamlPreviewNode unset = service.Parse($"<{typeName} xmlns='https://github.com/avaloniaui' />");
            AxamlPropertyDescriptor descriptor = AxamlPropertyValueValidator.Describe(unset, "SelectedIndex");

            Assert.IsTrue(capability.EditableProperties.Contains("SelectedIndex"), $"{typeName}.SelectedIndex");
            Assert.AreEqual(AxamlPropertyEditorKind.Number, descriptor.EditorKind, $"{typeName}.SelectedIndex");
            Assert.AreEqual("-1", descriptor.Value, $"{typeName}.SelectedIndex default");
            Assert.IsNull(descriptor.ValidationError, $"{typeName}.SelectedIndex");
            Assert.IsNull(AxamlPropertyValueValidator.Validate(typeName, "SelectedIndex", "-1"));
            Assert.IsNull(AxamlPropertyValueValidator.Validate(typeName, "SelectedIndex", "0"));
            Assert.IsNull(AxamlPropertyValueValidator.Validate(typeName, "SelectedIndex", "3"));
            Assert.IsNotNull(AxamlPropertyValueValidator.Validate(typeName, "SelectedIndex", "-2"));
            Assert.IsNotNull(AxamlPropertyValueValidator.Validate(typeName, "SelectedIndex", "1.5"));

            AxamlPreviewNode bound = service.Parse(
                $"<{typeName} xmlns='https://github.com/avaloniaui' SelectedIndex='{{Binding Selection}}' />");
            Assert.IsTrue(AxamlPropertyValueValidator.Describe(bound, "SelectedIndex").IsReadOnly, $"{typeName}.SelectedIndex binding");
        }
    }

    [TestMethod]
    public void Selectors_ExposeDropdownStateButKeepBindingsReadOnly()
    {
        AxamlPreviewService service = new();
        foreach (string typeName in new[] { "AutoCompleteBox", "ComboBox" })
        {
            AxamlControlCapability capability = AxamlControlCatalog.All.Single(item => item.TypeName == typeName);
            AxamlPreviewNode unset = service.Parse($"<{typeName} />");
            AxamlPropertyDescriptor dropdown = AxamlPropertyValueValidator.Describe(unset, "IsDropDownOpen");

            Assert.IsTrue(capability.EditableProperties.Contains("IsDropDownOpen"), typeName);
            Assert.AreEqual(AxamlPropertyEditorKind.Boolean, dropdown.EditorKind, typeName);
            Assert.AreEqual("False", dropdown.Value, typeName);
            Assert.IsNull(dropdown.ValidationError, typeName);
            Assert.IsNull(AxamlPropertyValueValidator.Validate(typeName, "IsDropDownOpen", "True"), typeName);
            Assert.IsNull(AxamlPropertyValueValidator.Validate(typeName, "IsDropDownOpen", "False"), typeName);
            Assert.IsNotNull(AxamlPropertyValueValidator.Validate(typeName, "IsDropDownOpen", "sometimes"), typeName);
            Assert.IsTrue(AxamlPropertyValueValidator.Describe(
                service.Parse($"<{typeName} IsDropDownOpen='{{Binding Open}}' />"),
                "IsDropDownOpen").IsReadOnly, typeName);
        }
    }

    [TestMethod]
    public void Selectors_ExposeStaticItemsAndRejectDynamicSources()
    {
        AxamlPreviewService service = new();
        foreach ((string selectorType, string itemType) in new[]
        {
            ("ComboBox", "ComboBoxItem"),
            ("ListBox", "ListBoxItem"),
        })
        {
            string markup = $"<{selectorType} xmlns='https://github.com/avaloniaui'><{itemType} Content='Local choice' /></{selectorType}>";
            AxamlPreviewNode selector = service.Parse(markup);
            AxamlPreviewNode item = selector.Children.Single();
            AxamlControlCapability capability = AxamlControlCatalog.All.Single(
                control => control.TypeName == selectorType);
            AxamlControlCapability itemCapability = AxamlControlCatalog.All.Single(
                control => control.TypeName == itemType);

            Assert.IsTrue(selector.IsSupported, selectorType);
            Assert.IsTrue(selector.CanAppendChildren, selectorType);
            Assert.IsTrue(capability.AllowsSafeChildRemoval, selectorType);
            Assert.IsTrue(capability.AllowedChildTypes.Contains(itemType), selectorType);
            Assert.IsTrue(itemCapability.EditableProperties.Contains("Content"), itemType);
            Assert.AreEqual("Local choice", AxamlPropertyValueValidator.Describe(item, "Content").Value, itemType);
            Assert.IsFalse(AxamlPropertyValueValidator.Describe(item, "Content").IsReadOnly, itemType);

            string withAddedItem = service.InsertChildControl(
                $"<{selectorType} />",
                service.Parse($"<{selectorType} />"),
                itemType);
            Assert.IsTrue(service.Parse(withAddedItem).IsSupported, selectorType);
            Assert.AreEqual(itemType, service.Parse(withAddedItem).Children.Single().TypeName);
        }

        string[] unsupportedSelectors =
        [
            "<ComboBox ItemsSource='{Binding Choices}' />",
            "<ComboBox ItemTemplate='{StaticResource ChoiceTemplate}' />",
            "<ComboBox SelectedItem='{Binding Choice}' />",
            "<ComboBox><ComboBox.Items><ComboBoxItem Content='Choice' /></ComboBox.Items></ComboBox>",
            "<ComboBox><ListBoxItem Content='Wrong item type' /></ComboBox>",
            "<ListBox DisplayMemberBinding='{Binding Name}' />",
            "<ListBox><ComboBoxItem Content='Wrong item type' /></ListBox>",
            "<ListBox><Button Content='Interactive item' /></ListBox>",
        ];
        foreach (string unsupported in unsupportedSelectors)
        {
            Assert.IsFalse(service.Parse(unsupported).IsSupported, unsupported);
        }
    }

    [TestMethod]
    public void ItemsControl_ExposesStaticTextItemsAndRejectsDynamicContent()
    {
        const string markup = """
            <ItemsControl xmlns="https://github.com/avaloniaui">
              <TextBlock Text="First" />
              <Label Content="Second" />
            </ItemsControl>
            """;
        AxamlPreviewService service = new();
        AxamlPreviewNode items = service.Parse(markup);
        AxamlControlCapability capability = AxamlControlCatalog.All.Single(
            static control => control.TypeName == "ItemsControl");

        Assert.IsTrue(items.IsSupported);
        Assert.IsTrue(items.CanAppendChildren);
        Assert.IsTrue(capability.AllowsSafeChildRemoval);
        CollectionAssert.AreEqual(new[] { "Label", "TextBlock" }, capability.AllowedChildTypes.OrderBy(
            static typeName => typeName, StringComparer.Ordinal).ToArray());
        CollectionAssert.AreEqual(new[] { "First", "Second" }, items.Children.Select(
            static child => child.Text).ToArray());

        string withAddedItem = service.InsertChildControl(
            "<ItemsControl />",
            service.Parse("<ItemsControl />"),
            "TextBlock");
        Assert.IsTrue(service.Parse(withAddedItem).IsSupported);
        Assert.AreEqual("TextBlock", service.Parse(withAddedItem).Children.Single().TypeName);

        string[] unsupportedItems =
        [
            "<ItemsControl ItemsSource='{Binding Entries}' />",
            "<ItemsControl ItemTemplate='{StaticResource EntryTemplate}' />",
            "<ItemsControl><ItemsControl.Items><TextBlock Text='Choice' /></ItemsControl.Items></ItemsControl>",
            "<ItemsControl><Button Content='Interactive item' /></ItemsControl>",
            "<ItemsControl><ListBoxItem Content='Wrong item type' /></ItemsControl>",
        ];
        foreach (string unsupported in unsupportedItems)
        {
            Assert.IsFalse(service.Parse(unsupported).IsSupported, unsupported);
        }
    }

    [TestMethod]
    public void TabControl_ExposesSafeTabsAndRestrictsChildTypes()
    {
        const string markup = """
            <TabControl xmlns="https://github.com/avaloniaui" SelectedIndex="1">
              <TabItem Header="General"><TextBlock Text="Settings" /></TabItem>
              <TabItem Header="Advanced" />
            </TabControl>
            """;
        AxamlPreviewService service = new();
        AxamlPreviewNode tabs = service.Parse(markup);
        AxamlPreviewNode firstTab = tabs.Children[0];
        AxamlControlCapability capability = AxamlControlCatalog.All.Single(
            static item => item.TypeName == "TabControl");
        AxamlPropertyDescriptor header = AxamlPropertyValueValidator.Describe(firstTab, "Header");

        Assert.IsTrue(tabs.IsSupported);
        Assert.IsTrue(tabs.CanAppendChildren);
        Assert.AreEqual("TabItem", capability.AllowedChildTypes.Single());
        Assert.AreEqual(AxamlPropertyEditorKind.Number,
            AxamlPropertyValueValidator.Describe(tabs, "SelectedIndex").EditorKind);
        Assert.AreEqual("Settings", firstTab.Children.Single().Text);
        Assert.AreEqual("General", header.Value);
        Assert.IsFalse(header.IsReadOnly);
        Assert.IsNull(header.ValidationError);

        string withNewTab = service.InsertChildControl(markup, tabs, "TabItem");
        Assert.AreEqual(3, service.Parse(withNewTab).Children.Count);
        Assert.ThrowsExactly<InvalidOperationException>(
            () => service.InsertChildControl(markup, tabs, "Button"));
        Assert.IsFalse(service.Parse("<TabControl><TextBlock /></TabControl>").IsSupported);
    }

    [TestMethod]
    public void TreeView_ExposesStaticHierarchyAndRejectsDynamicItems()
    {
        const string markup = """
            <TreeView xmlns="https://github.com/avaloniaui">
              <TreeViewItem Header="Project" IsExpanded="True">
                <TreeViewItem Header="src">
                  <TreeViewItem Header="Main.cs" />
                </TreeViewItem>
              </TreeViewItem>
            </TreeView>
            """;
        AxamlPreviewService service = new();
        AxamlPreviewNode tree = service.Parse(markup);
        AxamlPreviewNode project = tree.Children.Single();
        AxamlPreviewNode sourceFolder = project.Children.Single();
        AxamlControlCapability treeCapability = AxamlControlCatalog.All.Single(
            static item => item.TypeName == "TreeView");
        AxamlControlCapability itemCapability = AxamlControlCatalog.All.Single(
            static item => item.TypeName == "TreeViewItem");

        Assert.IsTrue(tree.IsSupported);
        Assert.IsTrue(tree.CanAppendChildren);
        Assert.IsTrue(treeCapability.AllowsSafeChildRemoval);
        Assert.IsTrue(project.CanAppendChildren);
        Assert.IsTrue(itemCapability.AllowsSafeChildRemoval);
        Assert.AreEqual("TreeViewItem", treeCapability.AllowedChildTypes.Single());
        Assert.AreEqual("TreeViewItem", itemCapability.AllowedChildTypes.Single());
        Assert.AreEqual("Project", AxamlPropertyValueValidator.Describe(project, "Header").Value);
        Assert.AreEqual("True", AxamlPropertyValueValidator.Describe(project, "IsExpanded").Value);
        Assert.AreEqual("False", AxamlPropertyValueValidator.Describe(
            service.Parse("<TreeViewItem />"),
            "IsExpanded").Value);

        string withRootItem = service.InsertChildControl("<TreeView />", service.Parse("<TreeView />"), "TreeViewItem");
        AxamlPreviewNode parsedWithRootItem = service.Parse(withRootItem);
        Assert.AreEqual(1, parsedWithRootItem.Children.Count);
        string withNestedItem = service.InsertChildControl(withRootItem, parsedWithRootItem.Children.Single(), "TreeViewItem");
        Assert.AreEqual(1, service.Parse(withNestedItem).Children.Single().Children.Count);
        Assert.ThrowsExactly<InvalidOperationException>(
            () => service.InsertChildControl(markup, tree, "Button"));
        Assert.IsFalse(service.Parse("<TreeView ItemsSource='{Binding Nodes}' />").IsSupported);
        Assert.IsFalse(service.Parse("<TreeViewItem ItemsSource='{Binding Children}' />").IsSupported);
        Assert.IsFalse(service.Parse("<TreeView><Button /></TreeView>").IsSupported);
        Assert.IsFalse(service.Parse("<TreeViewItem><TextBlock /></TreeViewItem>").IsSupported);
        Assert.IsFalse(service.Parse("<TreeView><!-- keep --><TreeViewItem Header='Project' /></TreeView>").IsSupported);
        Assert.IsFalse(service.Parse(
            "<TreeView><TreeView.Items><TreeViewItem Header='Project' /></TreeView.Items></TreeView>").IsSupported);
    }

    [TestMethod]
    public void Menu_ExposesSafeNestedItemsAndRestrictsChildTypes()
    {
        const string markup = """
            <Menu xmlns="https://github.com/avaloniaui">
              <MenuItem Header="File" Icon="F" ToggleType="CheckBox" IsChecked="True" IsSubMenuOpen="True" StaysOpenOnClick="True">
                <MenuItem Header="Open" InputGesture="Ctrl+O" HotKey="Ctrl+H" ToggleType="Radio" GroupName="EditorMode" IsChecked="True" />
                <Separator />
                <MenuItem Header="Exit" HotKey="Ctrl+Q" />
              </MenuItem>
            </Menu>
            """;
        AxamlPreviewService service = new();
        AxamlPreviewNode menu = service.Parse(markup);
        AxamlPreviewNode fileMenuItem = menu.Children.Single();
        AxamlControlCapability menuCapability = AxamlControlCatalog.All.Single(
            static item => item.TypeName == "Menu");
        AxamlControlCapability itemCapability = AxamlControlCatalog.All.Single(
            static item => item.TypeName == "MenuItem");

        Assert.IsTrue(menu.IsSupported);
        Assert.IsTrue(menu.CanAppendChildren);
        CollectionAssert.AreEquivalent(new[] { "MenuItem", "Separator" }, menuCapability.AllowedChildTypes.ToArray());
        CollectionAssert.AreEquivalent(new[] { "MenuItem", "Separator" }, itemCapability.AllowedChildTypes.ToArray());
        Assert.AreEqual("File", AxamlPropertyValueValidator.Describe(fileMenuItem, "Header").Value);
        Assert.IsFalse(AxamlPropertyValueValidator.Describe(fileMenuItem, "Header").IsReadOnly);
        AxamlPropertyDescriptor icon = AxamlPropertyValueValidator.Describe(fileMenuItem, "Icon");
        Assert.IsTrue(itemCapability.EditableProperties.Contains("Icon"));
        Assert.AreEqual(AxamlPropertyEditorKind.Text, icon.EditorKind);
        Assert.AreEqual("F", icon.Value);
        Assert.IsFalse(icon.IsReadOnly);
        Assert.IsNull(icon.ValidationError);
        AxamlPropertyDescriptor isChecked = AxamlPropertyValueValidator.Describe(fileMenuItem, "IsChecked");
        AxamlPropertyDescriptor isSubMenuOpen = AxamlPropertyValueValidator.Describe(fileMenuItem, "IsSubMenuOpen");
        AxamlPropertyDescriptor staysOpenOnClick = AxamlPropertyValueValidator.Describe(fileMenuItem, "StaysOpenOnClick");
        AxamlPropertyDescriptor groupName = AxamlPropertyValueValidator.Describe(fileMenuItem.Children[0], "GroupName");
        AxamlPropertyDescriptor toggleType = AxamlPropertyValueValidator.Describe(fileMenuItem, "ToggleType");
        Assert.IsTrue(itemCapability.EditableProperties.Contains("ToggleType"));
        Assert.IsTrue(itemCapability.EditableProperties.Contains("IsChecked"));
        Assert.IsTrue(itemCapability.EditableProperties.Contains("IsSubMenuOpen"));
        Assert.IsTrue(itemCapability.EditableProperties.Contains("StaysOpenOnClick"));
        Assert.IsTrue(itemCapability.EditableProperties.Contains("GroupName"));
        Assert.AreEqual(AxamlPropertyEditorKind.Enum, toggleType.EditorKind);
        Assert.AreEqual("CheckBox", toggleType.Value);
        Assert.IsNull(toggleType.ValidationError);
        AxamlPreviewNode boundIcon = service.Parse("<Menu xmlns='https://github.com/avaloniaui'><MenuItem Icon='{Binding FileIcon}' /></Menu>")
            .Children.Single();
        Assert.IsTrue(AxamlPropertyValueValidator.Describe(boundIcon, "Icon").IsReadOnly);
        Assert.AreEqual(AxamlPropertyEditorKind.Boolean, isChecked.EditorKind);
        Assert.AreEqual("True", isChecked.Value);
        Assert.IsNull(isChecked.ValidationError);
        Assert.AreEqual(AxamlPropertyEditorKind.Boolean, isSubMenuOpen.EditorKind);
        Assert.AreEqual("True", isSubMenuOpen.Value);
        Assert.IsNull(isSubMenuOpen.ValidationError);
        Assert.AreEqual(AxamlPropertyEditorKind.Boolean, staysOpenOnClick.EditorKind);
        Assert.AreEqual("True", staysOpenOnClick.Value);
        Assert.IsNull(staysOpenOnClick.ValidationError);
        Assert.IsNull(AxamlPropertyValueValidator.Validate("MenuItem", "IsChecked", "False"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("MenuItem", "IsChecked", "sometimes"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("MenuItem", "IsChecked", string.Empty));
        Assert.IsNull(AxamlPropertyValueValidator.Validate("MenuItem", "ToggleType", "Radio"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("MenuItem", "ToggleType", "Unknown"));
        Assert.IsNull(AxamlPropertyValueValidator.Validate("MenuItem", "IsSubMenuOpen", "False"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("MenuItem", "IsSubMenuOpen", "sometimes"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("MenuItem", "IsSubMenuOpen", string.Empty));
        Assert.IsNull(AxamlPropertyValueValidator.Validate("MenuItem", "StaysOpenOnClick", "False"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("MenuItem", "StaysOpenOnClick", "sometimes"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("MenuItem", "StaysOpenOnClick", string.Empty));
        Assert.IsNull(AxamlPropertyValueValidator.Validate("MenuItem", "GroupName", "EditorMode"));
        Assert.IsNull(AxamlPropertyValueValidator.Validate("MenuItem", "GroupName", string.Empty));
        Assert.AreEqual("EditorMode", groupName.Value);
        Assert.IsNull(groupName.ValidationError);
        Assert.AreEqual("None", AxamlPropertyValueValidator.Describe(
            service.Parse("<MenuItem />"),
            "ToggleType").Value);
        AxamlPreviewNode unsetMenuItem = service.Parse("<MenuItem />");
        Assert.AreEqual("False", AxamlPropertyValueValidator.Describe(unsetMenuItem, "IsChecked").Value);
        Assert.AreEqual("False", AxamlPropertyValueValidator.Describe(unsetMenuItem, "IsSubMenuOpen").Value);
        Assert.AreEqual("False", AxamlPropertyValueValidator.Describe(unsetMenuItem, "StaysOpenOnClick").Value);
        Assert.AreEqual(string.Empty, AxamlPropertyValueValidator.Describe(unsetMenuItem, "GroupName").Value);
        AxamlPreviewNode boundMenuItem = service.Parse(
            "<MenuItem ToggleType='{Binding ToggleType}' IsChecked='{Binding Checked}' GroupName='{Binding Group}' IsSubMenuOpen='{Binding Open}' StaysOpenOnClick='{Binding StayOpen}' />");
        Assert.IsTrue(AxamlPropertyValueValidator.Describe(boundMenuItem, "ToggleType").IsReadOnly);
        Assert.IsTrue(AxamlPropertyValueValidator.Describe(boundMenuItem, "IsChecked").IsReadOnly);
        Assert.IsTrue(AxamlPropertyValueValidator.Describe(boundMenuItem, "GroupName").IsReadOnly);
        Assert.IsTrue(AxamlPropertyValueValidator.Describe(boundMenuItem, "IsSubMenuOpen").IsReadOnly);
        Assert.IsTrue(AxamlPropertyValueValidator.Describe(boundMenuItem, "StaysOpenOnClick").IsReadOnly);
        Assert.AreEqual("MenuItem", fileMenuItem.Children[0].TypeName);
        Assert.AreEqual("Separator", fileMenuItem.Children[1].TypeName);
        Assert.AreEqual("Exit", fileMenuItem.Children[2].Text);
        AxamlPropertyDescriptor inputGesture = AxamlPropertyValueValidator.Describe(
            fileMenuItem.Children[0],
            "InputGesture");
        AxamlPropertyDescriptor hotKey = AxamlPropertyValueValidator.Describe(
            fileMenuItem.Children[2],
            "HotKey");
        Assert.IsTrue(itemCapability.EditableProperties.Contains("InputGesture"));
        Assert.IsTrue(itemCapability.EditableProperties.Contains("HotKey"));
        Assert.AreEqual(AxamlPropertyEditorKind.Text, inputGesture.EditorKind);
        Assert.AreEqual("Ctrl+O", inputGesture.Value);
        Assert.IsNull(inputGesture.ValidationError);
        Assert.AreEqual(AxamlPropertyEditorKind.Text, hotKey.EditorKind);
        Assert.AreEqual("Ctrl+Q", hotKey.Value);
        Assert.IsNull(hotKey.ValidationError);
        Assert.IsNull(AxamlPropertyValueValidator.Validate("MenuItem", "HotKey", "Ctrl+S"));
        Assert.IsNull(AxamlPropertyValueValidator.Validate("MenuItem", "HotKey", string.Empty));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("MenuItem", "HotKey", "NotAKey"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("MenuItem", "HotKey", "Ctrl"));
        Assert.IsNull(AxamlPropertyValueValidator.Validate("MenuItem", "InputGesture", "Ctrl+S"));
        Assert.IsNull(AxamlPropertyValueValidator.Validate("MenuItem", "InputGesture", string.Empty));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("MenuItem", "InputGesture", "NotAKey"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("MenuItem", "InputGesture", "Ctrl"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("MenuItem", "InputGesture", "Ctrl+S+T"));

        AxamlPreviewNode boundGesture = service.Parse(
            "<MenuItem InputGesture='{Binding SaveGesture}' HotKey='{Binding SaveHotKey}' />");
        Assert.IsTrue(AxamlPropertyValueValidator.Describe(boundGesture, "InputGesture").IsReadOnly);
        Assert.IsTrue(AxamlPropertyValueValidator.Describe(boundGesture, "HotKey").IsReadOnly);

        string withNewItem = service.InsertChildControl(markup, menu, "MenuItem");
        Assert.AreEqual(2, service.Parse(withNewItem).Children.Count);
        Assert.ThrowsExactly<InvalidOperationException>(
            () => service.InsertChildControl(markup, menu, "Button"));
        Assert.IsFalse(service.Parse("<Menu><TextBlock /></Menu>").IsSupported);
        Assert.IsFalse(service.Parse("<MenuItem><Button /></MenuItem>").IsSupported);
    }

    [TestMethod]
    public void Image_ExposesLiteralSourceAndStretchWithoutLoadingOrInterpretingSource()
    {
        AxamlPreviewService service = new();
        const string literalSource = "avares://Sample/Assets/logo.png";
        AxamlPreviewNode image = service.Parse(
            $"<Image xmlns='https://github.com/avaloniaui' Source='{literalSource}' Stretch='UniformToFill' StretchDirection='DownOnly' />");
        AxamlControlCapability capability = AxamlControlCatalog.All.Single(
            static item => item.TypeName == "Image");

        Assert.IsTrue(image.IsSupported);
        Assert.IsFalse(capability.SupportsMvvmGeneration);
        Assert.IsTrue(capability.EditableProperties.Contains("Source"));
        Assert.IsFalse(image.CanAppendChildren);
        AxamlPropertyDescriptor source = AxamlPropertyValueValidator.Describe(image, "Source");
        Assert.AreEqual(AxamlPropertyEditorKind.Text, source.EditorKind);
        Assert.AreEqual(literalSource, source.Value);
        Assert.IsFalse(source.IsReadOnly);
        Assert.IsNull(source.ValidationError);
        Assert.AreEqual(AxamlPropertyEditorKind.Enum, AxamlPropertyValueValidator.Describe(image, "Stretch").EditorKind);
        Assert.AreEqual("UniformToFill", AxamlPropertyValueValidator.Describe(image, "Stretch").Value);
        Assert.AreEqual("DownOnly", AxamlPropertyValueValidator.Describe(image, "StretchDirection").Value);
        Assert.AreEqual("Uniform", AxamlPropertyValueValidator.Describe(service.Parse("<Image />"), "Stretch").Value);
        Assert.IsNull(AxamlPropertyValueValidator.Validate("Image", "Source", "https://example.invalid/image.png"));
        Assert.IsNull(AxamlPropertyValueValidator.Validate("Image", "Source", string.Empty));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("Image", "Source", new string('a', 257)));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("Image", "Source", "bad\u0001source"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("Image", "Source", "{StaticResource Logo}"));
        Assert.IsTrue(AxamlPropertyValueValidator.Describe(
            service.Parse("<Image Source='{Binding Logo}' />"),
            "Source").IsReadOnly);
        Assert.IsFalse(service.Parse("<Image><Button /></Image>").IsSupported);
        Assert.IsFalse(service.Parse("<Image><Image.Source>source.png</Image.Source></Image>").IsSupported);
        Assert.IsTrue(service.Parse(
            "<Image><Image.ToolTip><ToolTip><TextBlock Text='Logo' /></ToolTip></Image.ToolTip></Image>").IsSupported);
    }

    [TestMethod]
    public void CommandParameter_IsLiteralTextForCommandCapableControls()
    {
        AxamlPreviewService service = new();
        string[] commandControls = ["Button", "CheckBox", "MenuItem", "RadioButton", "ToggleSwitch"];
        foreach (string controlType in commandControls)
        {
            AxamlControlCapability capability = AxamlControlCatalog.All.Single(item => item.TypeName == controlType);
            AxamlBindablePropertyCapability command = capability.BindableProperties.Single(
                static property => property.Name == "Command");
            AxamlPreviewNode control = service.Parse(
                $"<{controlType} Command='{{Binding RunCommand}}' CommandParameter='DocumentId' />");
            AxamlPropertyDescriptor parameter = AxamlPropertyValueValidator.Describe(control, "CommandParameter");

            Assert.IsTrue(command.IsCommand, controlType);
            Assert.IsTrue(capability.EditableProperties.Contains("CommandParameter"), controlType);
            Assert.AreEqual("DocumentId", parameter.Value, controlType);
            Assert.IsFalse(parameter.IsReadOnly, controlType);
            Assert.AreEqual(AxamlPropertyEditorKind.Text, parameter.EditorKind, controlType);

            AxamlPreviewNode boundControl = service.Parse(
                $"<{controlType} Command='{{Binding RunCommand}}' CommandParameter='{{Binding DocumentId}}' />");
            Assert.IsTrue(AxamlPropertyValueValidator.Describe(boundControl, "CommandParameter").IsReadOnly, controlType);
        }

        Assert.IsNull(AxamlPropertyValueValidator.Validate("Button", "CommandParameter", string.Empty));
        Assert.IsNull(AxamlPropertyValueValidator.Validate("MenuItem", "CommandParameter", "DocumentId"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("Button", "CommandParameter", "{Binding DocumentId}"));
    }

    [TestMethod]
    public void ControlContextMenu_IsLimitedToOneSafePropertyElementAndCanBeInserted()
    {
        const string prefixMarkup = "<ui:Button xmlns:ui='https://github.com/avaloniaui' Content='Open' />";
        const string existingMarkup = """
            <Button xmlns="https://github.com/avaloniaui">
              <Button.ContextMenu>
                <ContextMenu>
                  <MenuItem Header="Copy" />
                  <Separator />
                </ContextMenu>
              </Button.ContextMenu>
            </Button>
            """;
        AxamlPreviewService service = new();
        AxamlPreviewNode button = service.Parse(prefixMarkup);
        string inserted = service.InsertContextMenu(prefixMarkup, button);
        AxamlPreviewNode parsedButton = service.Parse(inserted);
        AxamlPreviewNode contextMenu = parsedButton.Children.Single();

        StringAssert.Contains(inserted, "<ui:Button.ContextMenu>");
        StringAssert.Contains(inserted, "<ui:ContextMenu />");
        Assert.IsTrue(parsedButton.IsSupported);
        Assert.IsTrue(contextMenu.IsSupported);
        Assert.IsTrue(contextMenu.CanAppendChildren);
        CollectionAssert.AreEquivalent(
            new[] { "MenuItem", "Separator" },
            AxamlControlCatalog.All.Single(static capability => capability.TypeName == "ContextMenu")
                .AllowedChildTypes.ToArray());
        Assert.ThrowsExactly<InvalidOperationException>(() => service.InsertContextMenu(inserted, parsedButton));

        const string textBoxMarkup =
            "<TextBox xmlns='https://github.com/avaloniaui' Text='Search' />";
        AxamlPreviewNode textBox = service.Parse(textBoxMarkup);
        string textBoxWithContextMenu = service.InsertContextMenu(textBoxMarkup, textBox);
        AxamlPreviewNode parsedTextBox = service.Parse(textBoxWithContextMenu);
        Assert.IsTrue(parsedTextBox.IsSupported);
        Assert.AreEqual("ContextMenu", parsedTextBox.Children.Single().TypeName);
        StringAssert.Contains(textBoxWithContextMenu, "<TextBox.ContextMenu>");

        const string emptyContextMarkup =
            "<Button><Button.ContextMenu><ContextMenu>\n</ContextMenu></Button.ContextMenu></Button>";
        AxamlPreviewNode staleContextMenu = service.Parse(emptyContextMarkup).Children.Single();
        string changedContextMarkup = emptyContextMarkup.Replace(
            "<ContextMenu>\n",
            "<ContextMenu>\n<MenuItem Header='Open' />\n",
            StringComparison.Ordinal);
        Assert.ThrowsExactly<InvalidOperationException>(
            () => service.InsertChildControl(changedContextMarkup, staleContextMenu, "MenuItem"));

        AxamlPreviewNode existingContext = service.Parse(existingMarkup);
        Assert.IsTrue(existingContext.IsSupported);
        Assert.AreEqual("ContextMenu", existingContext.Children.Single().TypeName);
        Assert.AreEqual("MenuItem", existingContext.Children.Single().Children[0].TypeName);
        Assert.AreEqual("Separator", existingContext.Children.Single().Children[1].TypeName);

        const string expandedButtonMarkup = "<Button Content='Save'></Button>";
        AxamlPreviewNode expandedButton = service.Parse(expandedButtonMarkup);
        string expandedButtonWithContextMenu = service.InsertContextMenu(expandedButtonMarkup, expandedButton);
        StringAssert.Contains(expandedButtonWithContextMenu, "Content='Save'");
        Assert.IsTrue(service.Parse(expandedButtonWithContextMenu).IsSupported);
        StringAssert.Contains(expandedButtonWithContextMenu, "<Button.ContextMenu>");

        Assert.IsFalse(service.Parse("<Button ContextMenu='{Binding Actions}' />").IsSupported);
        Assert.IsFalse(service.Parse("<TextBox ContextMenu='{Binding Actions}' />").IsSupported);
        const string separatorMarkup = "<Separator />";
        string separatorWithContextMenu = service.InsertContextMenu(separatorMarkup, service.Parse(separatorMarkup));
        Assert.IsTrue(service.Parse(separatorWithContextMenu).IsSupported);
        StringAssert.Contains(separatorWithContextMenu, "<Separator.ContextMenu>");
        Assert.IsFalse(service.Parse(
            "<Button><Button.ContextMenu><ContextMenu /></Button.ContextMenu><Button.ContextMenu><ContextMenu /></Button.ContextMenu></Button>")
            .IsSupported);
        Assert.IsFalse(service.Parse(
            "<TextBox><TextBox.ContextMenu><ContextMenu /></TextBox.ContextMenu><TextBox.ContextMenu><ContextMenu /></TextBox.ContextMenu></TextBox>")
            .IsSupported);
        Assert.IsTrue(service.Parse(
            "<MenuItem Header='More'><MenuItem.ContextMenu><ContextMenu><MenuItem Header='Nested' /></ContextMenu></MenuItem.ContextMenu></MenuItem>")
            .IsSupported);
        Assert.IsFalse(service.Parse(
            "<Button><Button.ContextMenu><ContextMenu><TextBlock /></ContextMenu></Button.ContextMenu></Button>")
            .IsSupported);
        Assert.IsFalse(service.Parse(
            "<Button><Button.ContextMenu><ContextMenu ItemsSource='{Binding Actions}' /></Button.ContextMenu></Button>")
            .IsSupported);
    }

    [TestMethod]
    public void ToolTip_IsLimitedToSafeTextAndCanBeInserted()
    {
        const string prefixMarkup = "<ui:TextBox xmlns:ui='https://github.com/avaloniaui' />";
        AxamlPreviewService service = new();
        string inserted = service.InsertToolTip(prefixMarkup, service.Parse(prefixMarkup));
        AxamlPreviewNode parsed = service.Parse(inserted);

        Assert.IsTrue(parsed.IsSupported);
        Assert.AreEqual("ToolTip", parsed.Children.Single().TypeName);
        Assert.AreEqual("TextBlock", parsed.Children.Single().Children.Single().TypeName);
        StringAssert.Contains(inserted, "<ui:TextBox.ToolTip>");
        StringAssert.Contains(inserted, "<ui:ToolTip>");
        StringAssert.Contains(inserted, "Text=\"Helpful text\"");
        Assert.ThrowsExactly<InvalidOperationException>(() => service.InsertToolTip(inserted, parsed));

        Assert.IsTrue(service.Parse("<ToolTip />").IsSupported);
        Assert.IsFalse(service.Parse("<Button ToolTip='{Binding Help}' />").IsSupported);
        Assert.IsFalse(service.Parse(
            "<Button><Button.ToolTip><ToolTip><Button /></ToolTip></Button.ToolTip></Button>").IsSupported);
        Assert.IsFalse(service.Parse(
            "<Button><Button.ToolTip><ToolTip><TextBlock Text='{Binding Help}' /></ToolTip></Button.ToolTip></Button>")
            .IsSupported);
        Assert.IsFalse(service.Parse(
            "<Button><Button.ToolTip><ToolTip><TextBlock Text='Help' /><TextBlock Text='More' /></ToolTip></Button.ToolTip></Button>")
            .IsSupported);
        AxamlPreviewNode richToolTip = service.Parse(
            "<Button><Button.ToolTip><ToolTip><StackPanel Spacing='4'><TextBlock Text='Title' /><TextBlock Text='Help' /></StackPanel></ToolTip></Button.ToolTip></Button>");
        Assert.IsTrue(richToolTip.IsSupported);
        Assert.AreEqual("StackPanel", richToolTip.Children.Single().Children.Single().TypeName);
        Assert.AreEqual(2, richToolTip.Children.Single().Children.Single().Children.Count);
        Assert.IsFalse(service.Parse(
            "<Button><Button.ToolTip><ToolTip><StackPanel><Button /></StackPanel></ToolTip></Button.ToolTip></Button>")
            .IsSupported);
        Assert.IsFalse(service.Parse(
            "<Button><Button.ToolTip><ToolTip><StackPanel Spacing='{Binding Spacing}'><TextBlock /></StackPanel></ToolTip></Button.ToolTip></Button>")
            .IsSupported);
        Assert.IsFalse(service.Parse(
            "<Button><Button.ToolTip><ToolTip><StackPanel><TextBlock Text='{Binding Help}' /></StackPanel></ToolTip></Button.ToolTip></Button>")
            .IsSupported);
        AxamlPreviewNode borderedToolTip = service.Parse(
            "<Button><Button.ToolTip><ToolTip><Border Padding='4' BorderBrush='Blue' CornerRadius='2'><StackPanel Spacing='4'><TextBlock Text='Title' /><TextBlock Text='Help' /></StackPanel></Border></ToolTip></Button.ToolTip></Button>");
        Assert.IsTrue(borderedToolTip.IsSupported);
        Assert.AreEqual("Border", borderedToolTip.Children.Single().Children.Single().TypeName);
        Assert.AreEqual(
            2,
            borderedToolTip.Children.Single().Children.Single().Children.Single().Children.Count);
        Assert.IsFalse(service.Parse(
            "<Button><Button.ToolTip><ToolTip><Border Padding='{Binding Padding}'><TextBlock Text='Help' /></Border></ToolTip></Button.ToolTip></Button>")
            .IsSupported);
        Assert.IsFalse(service.Parse(
            "<Button><Button.ToolTip><ToolTip><Border><Button /></Border></ToolTip></Button.ToolTip></Button>")
            .IsSupported);
        Assert.IsFalse(service.Parse(
            "<Button><Button.ToolTip><ToolTip><Border><TextBlock /><TextBlock /></Border></ToolTip></Button.ToolTip></Button>")
            .IsSupported);
        Assert.ThrowsExactly<InvalidOperationException>(
            () => service.InsertToolTip("<ToolTip />", service.Parse("<ToolTip />")));
    }

    [TestMethod]
    public void DatePickerSelectedDate_UsesStrictInvariantDateLiterals()
    {
        AxamlControlCapability capability = AxamlControlCatalog.All.Single(static item => item.TypeName == "DatePicker");
        AxamlPreviewService service = new();
        AxamlPreviewNode selected = service.Parse(
            "<DatePicker xmlns='https://github.com/avaloniaui' SelectedDate='2024-02-29' />");
        AxamlPropertyDescriptor descriptor = AxamlPropertyValueValidator.Describe(selected, "SelectedDate");

        Assert.IsTrue(capability.EditableProperties.Contains("SelectedDate"));
        Assert.AreEqual("2024-02-29", descriptor.Value);
        Assert.AreEqual(AxamlPropertyEditorKind.Text, descriptor.EditorKind);
        Assert.IsNull(descriptor.ValidationError);
        Assert.IsNull(AxamlPropertyValueValidator.Validate("DatePicker", "SelectedDate", "2024-02-29"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("DatePicker", "SelectedDate", "2023-02-29"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("DatePicker", "SelectedDate", "02/29/2024"));
        Assert.IsNull(AxamlPropertyValueValidator.Validate("DatePicker", "SelectedDate", string.Empty));
        Assert.AreEqual(
            string.Empty,
            AxamlPropertyValueValidator.Describe(service.Parse("<DatePicker />"), "SelectedDate").Value);

        AxamlPreviewNode bound = service.Parse(
            "<DatePicker xmlns='https://github.com/avaloniaui' SelectedDate='{Binding StartDate}' />");
        Assert.IsTrue(AxamlPropertyValueValidator.Describe(bound, "SelectedDate").IsReadOnly);
    }

    [TestMethod]
    public void CalendarDatePicker_ExposesSafeDateAndDropdownProperties()
    {
        AxamlControlCapability capability = AxamlControlCatalog.All.Single(
            static item => item.TypeName == "CalendarDatePicker");
        AxamlPreviewService service = new();
        AxamlPreviewNode unset = service.Parse("<CalendarDatePicker />");
        Assert.IsTrue(unset.IsSupported);
        Assert.IsTrue(capability.SupportsMvvmGeneration);
        Assert.AreEqual("DateTime?", capability.BindableProperties.Single().TypeName);
        Assert.AreEqual(AxamlBindingMode.TwoWay, capability.BindableProperties.Single().DefaultBindingMode);

        AxamlPropertyDescriptor selectedDate = AxamlPropertyValueValidator.Describe(
            service.Parse("<CalendarDatePicker SelectedDate='2024-02-29' />"),
            "SelectedDate");
        Assert.IsTrue(capability.EditableProperties.Contains("SelectedDate"));
        Assert.AreEqual(AxamlPropertyEditorKind.Text, selectedDate.EditorKind);
        Assert.AreEqual("2024-02-29", selectedDate.Value);
        Assert.IsNull(selectedDate.ValidationError);
        Assert.IsNull(AxamlPropertyValueValidator.Validate("CalendarDatePicker", "SelectedDate", "2024-02-29"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("CalendarDatePicker", "SelectedDate", "2023-02-29"));
        Assert.IsTrue(AxamlPropertyValueValidator.Describe(
            service.Parse("<CalendarDatePicker SelectedDate='{Binding StartDate}' />"),
            "SelectedDate").IsReadOnly);

        AxamlPropertyDescriptor placeholder = AxamlPropertyValueValidator.Describe(unset, "PlaceholderText");
        Assert.AreEqual("Choose a date", placeholder.Value);
        Assert.IsNull(AxamlPropertyValueValidator.Validate("CalendarDatePicker", "PlaceholderText", string.Empty));

        AxamlPropertyDescriptor floatingPlaceholder = AxamlPropertyValueValidator.Describe(unset, "UseFloatingPlaceholder");
        Assert.AreEqual(AxamlPropertyEditorKind.Boolean, floatingPlaceholder.EditorKind);
        Assert.AreEqual("False", floatingPlaceholder.Value);
        Assert.IsNull(AxamlPropertyValueValidator.Validate("CalendarDatePicker", "UseFloatingPlaceholder", "True"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("CalendarDatePicker", "UseFloatingPlaceholder", "sometimes"));
        Assert.IsTrue(AxamlPropertyValueValidator.Describe(
            service.Parse("<CalendarDatePicker UseFloatingPlaceholder='{Binding FloatingPlaceholder}' />"),
            "UseFloatingPlaceholder").IsReadOnly);

        AxamlPropertyDescriptor placeholderForeground = AxamlPropertyValueValidator.Describe(unset, "PlaceholderForeground");
        Assert.AreEqual(AxamlPropertyEditorKind.Text, placeholderForeground.EditorKind);
        Assert.AreEqual(string.Empty, placeholderForeground.Value);
        Assert.IsNull(AxamlPropertyValueValidator.Validate("CalendarDatePicker", "PlaceholderForeground", "DarkGray"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("CalendarDatePicker", "PlaceholderForeground", "Gradient"));
        Assert.IsTrue(AxamlPropertyValueValidator.Describe(
            service.Parse("<CalendarDatePicker PlaceholderForeground='{Binding PlaceholderBrush}' />"),
            "PlaceholderForeground").IsReadOnly);

        AxamlPropertyDescriptor dropdown = AxamlPropertyValueValidator.Describe(unset, "IsDropDownOpen");
        Assert.AreEqual(AxamlPropertyEditorKind.Boolean, dropdown.EditorKind);
        Assert.AreEqual("False", dropdown.Value);
        Assert.IsNull(AxamlPropertyValueValidator.Validate("CalendarDatePicker", "IsDropDownOpen", "True"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("CalendarDatePicker", "IsDropDownOpen", "sometimes"));
        Assert.IsTrue(AxamlPropertyValueValidator.Describe(
            service.Parse("<CalendarDatePicker IsDropDownOpen='{Binding PopupOpen}' />"),
            "IsDropDownOpen").IsReadOnly);
    }

    [TestMethod]
    public void Calendar_ExposesSafeSelectionModesAndNavigation()
    {
        AxamlControlCapability capability = AxamlControlCatalog.All.Single(static item => item.TypeName == "Calendar");
        AxamlControlCapability datePickerCapability = AxamlControlCatalog.All.Single(static item => item.TypeName == "DatePicker");
        AxamlPreviewService service = new();
        AxamlPreviewNode unset = service.Parse("<Calendar />");
        Assert.IsTrue(capability.SupportsMvvmGeneration);
        Assert.AreEqual("DateTime?", capability.BindableProperties.Single().TypeName);
        Assert.AreEqual(AxamlBindingMode.TwoWay, capability.BindableProperties.Single().DefaultBindingMode);

        foreach (string propertyName in new[] { "SelectedDate", "DisplayDate", "DisplayDateStart", "DisplayDateEnd" })
        {
        AxamlPropertyDescriptor date = AxamlPropertyValueValidator.Describe(unset, propertyName);
        Assert.IsTrue(capability.EditableProperties.Contains(propertyName), propertyName);
        Assert.AreEqual(AxamlPropertyEditorKind.Text, date.EditorKind, propertyName);
        Assert.AreEqual(string.Empty, date.Value, propertyName);
        Assert.IsNull(AxamlPropertyValueValidator.Validate("Calendar", propertyName, "2024-02-29"), propertyName);
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("Calendar", propertyName, "2024-02-30"), propertyName);
        Assert.IsNull(AxamlPropertyValueValidator.Validate("Calendar", propertyName, string.Empty), propertyName);
        Assert.IsTrue(AxamlPropertyValueValidator.Describe(
            service.Parse($"<Calendar {propertyName}='{{Binding CalendarDate}}' />"),
            propertyName).IsReadOnly, propertyName);
        }

        AxamlPropertyDescriptor selectionMode = AxamlPropertyValueValidator.Describe(unset, "SelectionMode");
        Assert.AreEqual("SingleDate", selectionMode.Value);
        CollectionAssert.AreEqual(
            new[] { "None", "SingleDate", "SingleRange", "MultipleRange" },
            selectionMode.Options.ToArray());
        foreach (string value in selectionMode.Options)
        {
            Assert.IsNull(AxamlPropertyValueValidator.Validate("Calendar", "SelectionMode", value), value);
        }
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("Calendar", "SelectionMode", "Multiple"));

        AxamlPropertyDescriptor displayMode = AxamlPropertyValueValidator.Describe(unset, "DisplayMode");
        Assert.AreEqual("Month", displayMode.Value);
        CollectionAssert.AreEqual(new[] { "Month", "Year", "Decade" }, displayMode.Options.ToArray());
        foreach (string value in displayMode.Options)
        {
            Assert.IsNull(AxamlPropertyValueValidator.Validate("Calendar", "DisplayMode", value), value);
        }
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("Calendar", "DisplayMode", "Century"));
        Assert.IsTrue(AxamlPropertyValueValidator.Describe(
            service.Parse("<Calendar DisplayMode='{Binding ViewMode}' />"),
            "DisplayMode").IsReadOnly);

        AxamlPropertyDescriptor firstDayOfWeek = AxamlPropertyValueValidator.Describe(unset, "FirstDayOfWeek");
        Assert.AreEqual("Monday", firstDayOfWeek.Value);
        CollectionAssert.AreEqual(
        new[] { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" },
        firstDayOfWeek.Options.ToArray());
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("Calendar", "FirstDayOfWeek", "Funday"));

        AxamlPropertyDescriptor todayHighlighted = AxamlPropertyValueValidator.Describe(unset, "IsTodayHighlighted");
        Assert.AreEqual(AxamlPropertyEditorKind.Boolean, todayHighlighted.EditorKind);
        Assert.AreEqual("True", todayHighlighted.Value);
        Assert.IsNull(AxamlPropertyValueValidator.Validate("Calendar", "IsTodayHighlighted", "False"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("Calendar", "IsTodayHighlighted", "sometimes"));
        Assert.AreEqual(AxamlBindingMode.TwoWay, capability.BindableProperties.Single().DefaultBindingMode);

        foreach (string propertyName in new[] { "DayVisible", "MonthVisible", "YearVisible" })
        {
            AxamlPropertyDescriptor visibility = AxamlPropertyValueValidator.Describe(
                service.Parse("<DatePicker />"),
                propertyName);
            Assert.IsTrue(datePickerCapability.EditableProperties.Contains(propertyName));
            Assert.AreEqual(AxamlPropertyEditorKind.Boolean, visibility.EditorKind, propertyName);
            Assert.AreEqual("True", visibility.Value, propertyName);
            Assert.IsNull(visibility.ValidationError, propertyName);
            Assert.IsNull(AxamlPropertyValueValidator.Validate("DatePicker", propertyName, "False"), propertyName);
            Assert.IsNotNull(AxamlPropertyValueValidator.Validate("DatePicker", propertyName, "sometimes"), propertyName);
            Assert.IsNotNull(AxamlPropertyValueValidator.Validate("DatePicker", propertyName, string.Empty), propertyName);

            AxamlPreviewNode boundVisibility = service.Parse(
                $"<DatePicker {propertyName}='{{Binding ShowPart}}' />");
            Assert.IsTrue(AxamlPropertyValueValidator.Describe(boundVisibility, propertyName).IsReadOnly, propertyName);
        }

        foreach (string propertyName in new[] { "DayFormat", "MonthFormat", "YearFormat" })
        {
            AxamlPropertyDescriptor format = AxamlPropertyValueValidator.Describe(
                service.Parse($"<DatePicker {propertyName}='yyyy' />"),
                propertyName);
            Assert.IsTrue(datePickerCapability.EditableProperties.Contains(propertyName));
            Assert.AreEqual(AxamlPropertyEditorKind.Text, format.EditorKind, propertyName);
            Assert.AreEqual("yyyy", format.Value, propertyName);
            Assert.IsNull(format.ValidationError, propertyName);
            Assert.IsNull(AxamlPropertyValueValidator.Validate("DatePicker", propertyName, "MMM"), propertyName);
            Assert.IsNotNull(AxamlPropertyValueValidator.Validate("DatePicker", propertyName, "%"), propertyName);
            Assert.IsNotNull(AxamlPropertyValueValidator.Validate("DatePicker", propertyName, new string('y', 33)), propertyName);
            Assert.IsNull(AxamlPropertyValueValidator.Validate("DatePicker", propertyName, string.Empty), propertyName);

            AxamlPreviewNode boundFormat = service.Parse(
                $"<DatePicker {propertyName}='{{Binding DateFormat}}' />");
            Assert.IsTrue(AxamlPropertyValueValidator.Describe(boundFormat, propertyName).IsReadOnly, propertyName);
        }

        foreach (string propertyName in new[] { "MinYear", "MaxYear" })
        {
            AxamlPropertyDescriptor year = AxamlPropertyValueValidator.Describe(
                service.Parse($"<DatePicker {propertyName}='2024-01-01' />"),
                propertyName);
            Assert.IsTrue(datePickerCapability.EditableProperties.Contains(propertyName));
            Assert.AreEqual(AxamlPropertyEditorKind.Text, year.EditorKind, propertyName);
            Assert.AreEqual("2024-01-01", year.Value, propertyName);
            Assert.IsNull(year.ValidationError, propertyName);
            Assert.IsNull(AxamlPropertyValueValidator.Validate("DatePicker", propertyName, "1900-01-01"), propertyName);
            Assert.IsNull(AxamlPropertyValueValidator.Validate("DatePicker", propertyName, "2100-12-31"), propertyName);
            Assert.IsNotNull(AxamlPropertyValueValidator.Validate("DatePicker", propertyName, "2024"), propertyName);
            Assert.IsNotNull(AxamlPropertyValueValidator.Validate("DatePicker", propertyName, "2024-02-30"), propertyName);
            Assert.IsNull(AxamlPropertyValueValidator.Validate("DatePicker", propertyName, string.Empty), propertyName);

            AxamlPreviewNode boundYear = service.Parse(
                $"<DatePicker {propertyName}='{{Binding SelectedYear}}' />");
            Assert.IsTrue(AxamlPropertyValueValidator.Describe(boundYear, propertyName).IsReadOnly, propertyName);
        }
    }

    [TestMethod]
    public void AutoCompleteBox_ExposesSafeTextAndFilterOptions()
    {
        AxamlControlCapability capability = AxamlControlCatalog.All.Single(static item => item.TypeName == "AutoCompleteBox");
        AxamlPreviewService service = new();
        AxamlPreviewNode unset = service.Parse("<AutoCompleteBox />");
        Assert.IsTrue(capability.SupportsMvvmGeneration);
        Assert.AreEqual("string?", capability.BindableProperties.Single().TypeName);
        Assert.AreEqual(AxamlBindingMode.TwoWay, capability.BindableProperties.Single().DefaultBindingMode);

        AxamlPropertyDescriptor text = AxamlPropertyValueValidator.Describe(
            service.Parse("<AutoCompleteBox Text='Ada' />"),
            "Text");
        Assert.IsTrue(capability.EditableProperties.Contains("Text"));
        Assert.AreEqual(AxamlPropertyEditorKind.Text, text.EditorKind);
        Assert.AreEqual("Ada", text.Value);
        Assert.IsTrue(AxamlPropertyValueValidator.Describe(
            service.Parse("<AutoCompleteBox Text='{Binding SearchText}' />"),
            "Text").IsReadOnly);

        AxamlPropertyDescriptor filterMode = AxamlPropertyValueValidator.Describe(unset, "FilterMode");
        Assert.AreEqual(AxamlPropertyEditorKind.Enum, filterMode.EditorKind);
        Assert.AreEqual("StartsWith", filterMode.Value);
        CollectionAssert.AreEqual(
            new[]
            {
                "None",
                "StartsWith",
                "StartsWithCaseSensitive",
                "StartsWithOrdinal",
                "StartsWithOrdinalCaseSensitive",
                "Contains",
                "ContainsCaseSensitive",
                "ContainsOrdinal",
                "ContainsOrdinalCaseSensitive",
                "Equals",
                "EqualsCaseSensitive",
                "EqualsOrdinal",
                "EqualsOrdinalCaseSensitive",
            },
            filterMode.Options.ToArray());
        Assert.IsNull(AxamlPropertyValueValidator.Validate("AutoCompleteBox", "FilterMode", "ContainsOrdinal"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("AutoCompleteBox", "FilterMode", "Custom"));

        AxamlPropertyDescriptor prefixLength = AxamlPropertyValueValidator.Describe(unset, "MinimumPrefixLength");
        Assert.AreEqual(AxamlPropertyEditorKind.Number, prefixLength.EditorKind);
        Assert.AreEqual("1", prefixLength.Value);
        Assert.IsNull(AxamlPropertyValueValidator.Validate("AutoCompleteBox", "MinimumPrefixLength", "0"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("AutoCompleteBox", "MinimumPrefixLength", "-1"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("AutoCompleteBox", "MinimumPrefixLength", "1.5"));

        AxamlPropertyDescriptor completion = AxamlPropertyValueValidator.Describe(unset, "IsTextCompletionEnabled");
        Assert.AreEqual(AxamlPropertyEditorKind.Boolean, completion.EditorKind);
        Assert.AreEqual("False", completion.Value);
        Assert.IsNull(AxamlPropertyValueValidator.Validate("AutoCompleteBox", "IsTextCompletionEnabled", "True"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("AutoCompleteBox", "IsTextCompletionEnabled", "maybe"));
    }

    [TestMethod]
    public void SelectorAndNumericPlaceholderForeground_AcceptsOnlySafeColorLiterals()
    {
        AxamlPreviewService service = new();
        foreach (string typeName in new[] { "AutoCompleteBox", "ComboBox", "NumericUpDown" })
        {
            AxamlControlCapability capability = AxamlControlCatalog.All.Single(
                item => item.TypeName == typeName);
            AxamlPreviewNode unset = service.Parse($"<{typeName} />");
            AxamlPropertyDescriptor placeholderForeground = AxamlPropertyValueValidator.Describe(
                unset,
                "PlaceholderForeground");

            Assert.IsTrue(capability.EditableProperties.Contains("PlaceholderForeground"), typeName);
            Assert.AreEqual(AxamlPropertyEditorKind.Text, placeholderForeground.EditorKind, typeName);
            Assert.AreEqual(string.Empty, placeholderForeground.Value, typeName);
            Assert.IsNull(AxamlPropertyValueValidator.Validate(typeName, "PlaceholderForeground", "DarkGray"), typeName);
            Assert.IsNotNull(AxamlPropertyValueValidator.Validate(typeName, "PlaceholderForeground", "Gradient"), typeName);
            Assert.IsTrue(AxamlPropertyValueValidator.Describe(
                service.Parse($"<{typeName} PlaceholderForeground='{{Binding PlaceholderBrush}}' />"),
                "PlaceholderForeground").IsReadOnly, typeName);
        }
    }

    [TestMethod]
    public void ColorPicker_ExposesSafeColorModelAndFeatureOptions()
    {
        AxamlControlCapability capability = AxamlControlCatalog.All.Single(static item => item.TypeName == "ColorPicker");
        AxamlPreviewService service = new();
        AxamlPreviewNode unset = service.Parse("<ColorPicker />");
        Assert.IsTrue(capability.SupportsMvvmGeneration);
        Assert.AreEqual("global::Avalonia.Media.Color", capability.BindableProperties.Single().TypeName);
        Assert.AreEqual(AxamlBindingMode.TwoWay, capability.BindableProperties.Single().DefaultBindingMode);

        AxamlPropertyDescriptor color = AxamlPropertyValueValidator.Describe(unset, "Color");
        Assert.IsTrue(capability.EditableProperties.Contains("Color"));
        Assert.AreEqual(AxamlPropertyEditorKind.Text, color.EditorKind);
        Assert.AreEqual("White", color.Value);
        Assert.IsNull(color.ValidationError);
        Assert.IsNull(AxamlPropertyValueValidator.Validate("ColorPicker", "Color", "#FF123456"));
        Assert.IsNull(AxamlPropertyValueValidator.Validate("ColorPicker", "Color", "#ABC"));
        Assert.IsNull(AxamlPropertyValueValidator.Validate("ColorPicker", "Color", "Red"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("ColorPicker", "Color", "NotAColor"));
        Assert.IsTrue(AxamlPropertyValueValidator.Describe(
            service.Parse("<ColorPicker Color='{Binding Accent}' />"),
            "Color").IsReadOnly);

        AxamlPropertyDescriptor colorModel = AxamlPropertyValueValidator.Describe(unset, "ColorModel");
        Assert.AreEqual(AxamlPropertyEditorKind.Enum, colorModel.EditorKind);
        Assert.AreEqual("Rgba", colorModel.Value);
        CollectionAssert.AreEqual(new[] { "Hsva", "Rgba" }, colorModel.Options.ToArray());
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("ColorPicker", "ColorModel", "Unknown"));

        foreach (string propertyName in new[]
        {
            "IsAccentColorsVisible", "IsAlphaEnabled", "IsAlphaVisible", "IsColorComponentsVisible",
            "IsColorModelVisible", "IsColorPaletteVisible", "IsColorPreviewVisible",
            "IsColorSpectrumSliderVisible", "IsColorSpectrumVisible", "IsComponentSliderVisible",
            "IsComponentTextInputVisible", "IsHexInputVisible",
        })
        {
            AxamlPropertyDescriptor option = AxamlPropertyValueValidator.Describe(unset, propertyName);
            Assert.IsTrue(capability.EditableProperties.Contains(propertyName), propertyName);
            Assert.AreEqual(AxamlPropertyEditorKind.Boolean, option.EditorKind, propertyName);
            Assert.AreEqual("True", option.Value, propertyName);
            Assert.IsNull(AxamlPropertyValueValidator.Validate("ColorPicker", propertyName, "False"), propertyName);
            Assert.IsNotNull(AxamlPropertyValueValidator.Validate("ColorPicker", propertyName, "maybe"), propertyName);
        }
    }

    [TestMethod]
    public void TimePicker_ExposesSafeSelectionAndClockOptions()
    {
        AxamlControlCapability capability = AxamlControlCatalog.All.Single(static item => item.TypeName == "TimePicker");
        AxamlPreviewService service = new();
        AxamlPreviewNode unset = service.Parse("<TimePicker />");
        Assert.IsTrue(capability.SupportsMvvmGeneration);
        Assert.AreEqual("TimeSpan?", capability.BindableProperties.Single().TypeName);
        Assert.AreEqual(AxamlBindingMode.TwoWay, capability.BindableProperties.Single().DefaultBindingMode);
        Assert.AreEqual(AxamlPropertyEditorKind.Text, AxamlPropertyValueValidator.Describe(unset, "SelectedTime").EditorKind);
        Assert.AreEqual(string.Empty, AxamlPropertyValueValidator.Describe(unset, "SelectedTime").Value);
        Assert.IsNull(AxamlPropertyValueValidator.Validate("TimePicker", "SelectedTime", string.Empty));
        Assert.IsNull(AxamlPropertyValueValidator.Validate("TimePicker", "SelectedTime", "00:00"));
        Assert.IsNull(AxamlPropertyValueValidator.Validate("TimePicker", "SelectedTime", "23:59:59"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("TimePicker", "SelectedTime", "24:00"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("TimePicker", "SelectedTime", "12:60"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("TimePicker", "SelectedTime", "12:30:61"));

        AxamlPropertyDescriptor clock = AxamlPropertyValueValidator.Describe(unset, "ClockIdentifier");
        Assert.AreEqual(AxamlPropertyEditorKind.Enum, clock.EditorKind);
        Assert.AreEqual("24HourClock", clock.Value);
        CollectionAssert.AreEqual(new[] { "12HourClock", "24HourClock" }, clock.Options.ToArray());
        Assert.IsNull(AxamlPropertyValueValidator.Validate("TimePicker", "ClockIdentifier", "12HourClock"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("TimePicker", "ClockIdentifier", "TwentyFourHourClock"));

        foreach (string propertyName in new[] { "MinuteIncrement", "SecondIncrement" })
        {
            AxamlPropertyDescriptor increment = AxamlPropertyValueValidator.Describe(unset, propertyName);
            Assert.AreEqual(AxamlPropertyEditorKind.Number, increment.EditorKind, propertyName);
            Assert.AreEqual("1", increment.Value, propertyName);
            Assert.IsNull(AxamlPropertyValueValidator.Validate("TimePicker", propertyName, "59"), propertyName);
            Assert.IsNotNull(AxamlPropertyValueValidator.Validate("TimePicker", propertyName, "0"), propertyName);
            Assert.IsNotNull(AxamlPropertyValueValidator.Validate("TimePicker", propertyName, "60"), propertyName);
            Assert.IsTrue(AxamlPropertyValueValidator.Describe(
                service.Parse($"<TimePicker {propertyName}='{{Binding Increment}}' />"),
                propertyName).IsReadOnly, propertyName);
        }

        Assert.AreEqual("False", AxamlPropertyValueValidator.Describe(unset, "UseSeconds").Value);
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("TimePicker", "UseSeconds", "sometimes"));
        Assert.IsTrue(AxamlPropertyValueValidator.Describe(
            service.Parse("<TimePicker SelectedTime='{Binding StartTime}' />"),
            "SelectedTime").IsReadOnly);
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
            ("LineHeight", AxamlPropertyEditorKind.Number, "22", string.Empty),
            ("MaxLines", AxamlPropertyEditorKind.Number, "2", string.Empty),
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
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("Label", "LineHeight", "0"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("Label", "MaxLines", "-1"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("Label", "TextAlignment", "Unknown"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("Label", "TextTrimming", "Unknown"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("Label", "TextWrapping", "Unknown"));

        AxamlPreviewNode bound = service.Parse(
            "<Label xmlns='https://github.com/avaloniaui' FontSize='{Binding Size}' FontStyle='{Binding Style}' FontWeight='{Binding Weight}' Foreground='{Binding Brush}' LineHeight='{Binding Height}' MaxLines='{Binding Count}' TextAlignment='{Binding Alignment}' TextTrimming='{Binding Trimming}' TextWrapping='{Binding Wrapping}' />");
        foreach (string property in new[] { "FontSize", "FontStyle", "FontWeight", "Foreground", "LineHeight", "MaxLines", "TextAlignment", "TextTrimming", "TextWrapping" })
        {
            Assert.IsTrue(AxamlPropertyValueValidator.Describe(bound, property).IsReadOnly, $"Label.{property} binding");
        }
    }

    [TestMethod]
    public void TextControls_ExposeSafeLiteralFontFamily()
    {
        AxamlPreviewService service = new();
        foreach (AxamlControlCapability capability in AxamlControlCatalog.All.Where(
            static item => item.EditableProperties.Contains("FontSize")))
        {
            string controlType = capability.TypeName;
            AxamlPreviewNode unset = service.Parse($"<{controlType} xmlns='https://github.com/avaloniaui' />");
            AxamlPropertyDescriptor fontFamily = AxamlPropertyValueValidator.Describe(unset, "FontFamily");

            Assert.IsTrue(capability.EditableProperties.Contains("FontFamily"), controlType);
            Assert.AreEqual(AxamlPropertyEditorKind.Text, fontFamily.EditorKind, controlType);
            Assert.AreEqual(string.Empty, fontFamily.Value, controlType);
            Assert.IsNull(fontFamily.ValidationError, controlType);
            Assert.IsNull(AxamlPropertyValueValidator.Validate(controlType, "FontFamily", "DejaVu Sans"), controlType);
            Assert.IsNotNull(AxamlPropertyValueValidator.Validate(controlType, "FontFamily", "avares://Assets#Injected"), controlType);
            Assert.IsNotNull(AxamlPropertyValueValidator.Validate(controlType, "FontFamily", "{DynamicResource Font}"), controlType);

            AxamlPreviewNode bound = service.Parse(
                $"<{controlType} xmlns='https://github.com/avaloniaui' FontFamily='{{Binding Family}}' />");
            Assert.IsTrue(AxamlPropertyValueValidator.Describe(bound, "FontFamily").IsReadOnly, controlType);
        }
    }

    [TestMethod]
    public void ShapeControls_ExposeSafeLiteralFillAndStroke()
    {
        AxamlPreviewService service = new();
        foreach (string shapeType in new[] { "Ellipse", "Rectangle" })
        {
            AxamlControlCapability capability = AxamlControlCatalog.All.Single(
                item => item.TypeName == shapeType);
            AxamlPreviewNode shape = service.Parse(
                $"<{shapeType} xmlns='https://github.com/avaloniaui' Fill='CornflowerBlue' Stroke='DarkBlue' StrokeThickness='2' />");

            Assert.IsTrue(shape.IsSupported, shapeType);
            Assert.IsTrue(capability.EditableProperties.Contains("Fill"), shapeType);
            Assert.IsTrue(capability.EditableProperties.Contains("Stroke"), shapeType);
            Assert.IsTrue(capability.EditableProperties.Contains("StrokeThickness"), shapeType);
            Assert.AreEqual(AxamlPropertyEditorKind.Text, AxamlPropertyValueValidator.Describe(shape, "Fill").EditorKind);
            Assert.AreEqual("CornflowerBlue", AxamlPropertyValueValidator.Describe(shape, "Fill").Value);
            Assert.AreEqual("DarkBlue", AxamlPropertyValueValidator.Describe(shape, "Stroke").Value);
            Assert.AreEqual(AxamlPropertyEditorKind.Number, AxamlPropertyValueValidator.Describe(shape, "StrokeThickness").EditorKind);
            Assert.AreEqual("2", AxamlPropertyValueValidator.Describe(shape, "StrokeThickness").Value);
            Assert.IsNull(AxamlPropertyValueValidator.Validate(shapeType, "Fill", "#FF123456"));
            Assert.IsNull(AxamlPropertyValueValidator.Validate(shapeType, "Stroke", "DarkBlue"));
            Assert.IsNull(AxamlPropertyValueValidator.Validate(shapeType, "StrokeThickness", "0"));
            Assert.IsNotNull(AxamlPropertyValueValidator.Validate(shapeType, "Fill", "Gradient"));
            Assert.IsNotNull(AxamlPropertyValueValidator.Validate(shapeType, "StrokeThickness", "-1"));
            Assert.IsNotNull(AxamlPropertyValueValidator.Validate(shapeType, "StrokeThickness", "NaN"));

            AxamlPreviewNode bound = service.Parse(
                $"<{shapeType} xmlns='https://github.com/avaloniaui' Fill='{{Binding Fill}}' Stroke='{{Binding Stroke}}' StrokeThickness='{{Binding Thickness}}' />");
            foreach (string property in new[] { "Fill", "Stroke", "StrokeThickness" })
            {
                Assert.IsTrue(AxamlPropertyValueValidator.Describe(bound, property).IsReadOnly, $"{shapeType}.{property}");
            }
        }
    }

    [TestMethod]
    public void Line_ExposesBoundedLiteralEndpointsAndStroke()
    {
        AxamlPreviewService service = new();
        AxamlControlCapability capability = AxamlControlCatalog.All.Single(
            static item => item.TypeName == "Line");
        AxamlPreviewNode line = service.Parse(
            "<Line xmlns='https://github.com/avaloniaui' StartPoint='-25.5,10' EndPoint='80,40' Stroke='DarkGreen' StrokeThickness='3' />");

        Assert.IsTrue(line.IsSupported);
        foreach (string property in new[] { "StartPoint", "EndPoint", "Stroke", "StrokeThickness" })
        {
            Assert.IsTrue(capability.EditableProperties.Contains(property), property);
        }

        Assert.AreEqual("-25.5,10", AxamlPropertyValueValidator.Describe(line, "StartPoint").Value);
        Assert.AreEqual("80,40", AxamlPropertyValueValidator.Describe(line, "EndPoint").Value);
        Assert.IsNull(AxamlPropertyValueValidator.Validate("Line", "StartPoint", "0,0"));
        Assert.IsNull(AxamlPropertyValueValidator.Validate("Line", "EndPoint", "10000,-10000"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("Line", "StartPoint", "NaN,0"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("Line", "EndPoint", "10001,0"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("Line", "EndPoint", "0"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("Line", "EndPoint", string.Empty));

        AxamlPreviewNode bound = service.Parse(
            "<Line xmlns='https://github.com/avaloniaui' StartPoint='{Binding From}' EndPoint='{Binding To}' />");
        Assert.IsTrue(AxamlPropertyValueValidator.Describe(bound, "StartPoint").IsReadOnly);
        Assert.IsTrue(AxamlPropertyValueValidator.Describe(bound, "EndPoint").IsReadOnly);
    }

    [TestMethod]
    public void PolylineAndPolygon_ExposeBoundedLiteralPointLists()
    {
        AxamlPreviewService service = new();
        string oversized = string.Join(' ', Enumerable.Repeat("0,0", 129));
        foreach (string shapeType in new[] { "Polyline", "Polygon" })
        {
            AxamlControlCapability capability = AxamlControlCatalog.All.Single(
                item => item.TypeName == shapeType);
            AxamlPreviewNode shape = service.Parse(
                $"<{shapeType} xmlns='https://github.com/avaloniaui' Points='0,0 80,0 40,40' Stroke='DarkBlue' StrokeThickness='2' />");

            Assert.IsTrue(shape.IsSupported, shapeType);
            Assert.IsTrue(capability.EditableProperties.Contains("Points"), shapeType);
            Assert.AreEqual(
                "0,0 80,0 40,40",
                AxamlPropertyValueValidator.Describe(shape, "Points").Value,
                shapeType);
            if (shapeType == "Polyline")
            {
                Assert.IsNull(AxamlPropertyValueValidator.Validate(shapeType, "Points", "0,0 80,40"), shapeType);
            }

            Assert.IsNull(AxamlPropertyValueValidator.Validate(shapeType, "Points", "0,0 80,0 40,40"), shapeType);
            Assert.IsNotNull(AxamlPropertyValueValidator.Validate(shapeType, "Points", "0,0"), shapeType);
            Assert.IsNotNull(AxamlPropertyValueValidator.Validate(shapeType, "Points", "NaN,0 80,40"), shapeType);
            Assert.IsNotNull(AxamlPropertyValueValidator.Validate(shapeType, "Points", "10001,0 80,40"), shapeType);
            Assert.IsNotNull(AxamlPropertyValueValidator.Validate(shapeType, "Points", oversized), shapeType);
            Assert.IsNotNull(AxamlPropertyValueValidator.Validate(shapeType, "Points", string.Empty), shapeType);

            AxamlPreviewNode bound = service.Parse(
                $"<{shapeType} xmlns='https://github.com/avaloniaui' Points='{{Binding Vertices}}' />");
            Assert.IsTrue(AxamlPropertyValueValidator.Describe(bound, "Points").IsReadOnly, shapeType);
        }

        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("Polygon", "Points", "0,0 80,0"));
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
            "AutoCompleteBox",
            "Button",
            "Calendar",
            "CalendarDatePicker",
            "CheckBox",
            "ColorPicker",
            "ComboBox",
            "DatePicker",
            "MenuItem",
            "NumericUpDown",
            "RadioButton",
            "Slider",
            "TabControl",
            "TextBlock",
            "TextBox",
            "TimePicker",
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
    public void Catalog_OnlyEnablesAppendingForExplicitlySupportedContainers()
    {
        string[] actual = AxamlControlCatalog.All
            .Where(static capability => capability.AllowsAppendingChildren)
            .Select(static capability => capability.TypeName)
            .OrderBy(static typeName => typeName, StringComparer.Ordinal)
            .ToArray();

        CollectionAssert.AreEqual(
            new[] { "Canvas", "ComboBox", "ContextMenu", "DockPanel", "ItemsControl", "ListBox", "Menu", "MenuItem", "StackPanel", "TabControl", "TabItem", "ToolTip", "TreeView", "TreeViewItem", "UniformGrid", "WrapPanel" },
            actual);
    }

    [TestMethod]
    public void Catalog_OnlyEnablesSafeRemovalForSupportedContainers()
    {
        string[] actual = AxamlControlCatalog.All
            .Where(static capability => capability.AllowsSafeChildRemoval)
            .Select(static capability => capability.TypeName)
            .OrderBy(static typeName => typeName, StringComparer.Ordinal)
            .ToArray();

        CollectionAssert.AreEqual(
            new[] { "Border", "Canvas", "ComboBox", "ContentControl", "DockPanel", "Expander", "GroupBox", "ItemsControl", "ListBox", "ScrollViewer", "TabItem", "ToolTip", "TreeView", "TreeViewItem", "Viewbox" },
            actual);
    }
}
