using AA98_AvlnCodeStudio.Model.Workspaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;

namespace AA98_AvlnCodeStudio.Tests.Engineering;

[TestClass]
public sealed class AxamlPreviewServiceTests
{
    [TestMethod]
    public void Parse_BuildsSafeTreeForSupportedControlsAndLayout()
    {
        const string axaml = """
            <Window xmlns="https://github.com/avaloniaui"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                    x:Class="Sample.MainWindow">
              <Grid RowDefinitions="Auto,*" ColumnDefinitions="*">
                <StackPanel Grid.Row="1" Spacing="8">
                  <TextBlock Text="Hello" FontSize="20" />
                  <Button Content="Run" />
                </StackPanel>
              </Grid>
            </Window>
            """;

        AxamlPreviewNode root = new AxamlPreviewService().Parse(axaml);

        Assert.AreEqual("Grid", root.TypeName);
        Assert.AreEqual(1, root.Children.Count);
        Assert.AreEqual("StackPanel", root.Children[0].TypeName);
        Assert.AreEqual("8", root.Children[0].Properties["Spacing"]);
        Assert.AreEqual("Hello", root.Children[0].Children[0].Text);
        Assert.AreEqual("Run", root.Children[0].Children[1].Text);
        Assert.AreEqual("Auto", root.RowDefinitions[0]);
        Assert.AreEqual("*", root.RowDefinitions[1]);
        Assert.IsFalse(root.Properties.ContainsKey("Class"));
    }

    [TestMethod]
    public void CanvasCoordinates_AreEditableOnlyForDirectSupportedChildren()
    {
        const string markup =
            "<av:Canvas xmlns:av='https://github.com/avaloniaui'><av:Button av:Canvas.Left='12.5' av:Canvas.Top='3' /></av:Canvas>";
        AxamlPreviewService service = new();
        AxamlPreviewNode canvas = service.Parse(markup);
        AxamlPreviewNode button = canvas.Children.Single();

        Assert.IsTrue(canvas.IsSupported);
        Assert.IsTrue(canvas.CanAppendChildren);
        Assert.AreEqual("Canvas", button.ParentTypeName);
        Assert.IsTrue(service.GetEditableProperties(button).Contains("Canvas.Left"));
        Assert.IsTrue(service.GetEditableProperties(button).Contains("Canvas.Top"));
        Assert.IsNull(AxamlPropertyValueValidator.Validate("Button", "Canvas.Left", "-100000"));
        Assert.IsNull(AxamlPropertyValueValidator.Validate("Button", "Canvas.Top", "1.25"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("Button", "Canvas.Left", "NaN"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("Button", "Canvas.Top", "100001"));

        string updated = service.UpdateAttribute(markup, button, "Canvas.Left", "-4.25");
        Assert.IsTrue(updated.Contains("av:Canvas.Left='-4.25'", StringComparison.Ordinal));
        Assert.IsTrue(updated.Contains("av:Canvas.Top='3'", StringComparison.Ordinal));

        AxamlPreviewNode nestedCanvas = service.Parse(
            "<Canvas><StackPanel><Button /></StackPanel></Canvas>").Children.Single().Children.Single();
        Assert.IsFalse(service.GetEditableProperties(nestedCanvas).Contains("Canvas.Left"));

        AxamlPreviewNode boundCanvas = service.Parse(
            "<Canvas><Button Canvas.Left='{Binding X}' /></Canvas>");
        Assert.IsFalse(boundCanvas.IsSupported);
        Assert.ThrowsExactly<InvalidOperationException>(
            () => service.UpdateAttribute(
                "<Canvas><Button Canvas.Left='{Binding X}' /></Canvas>",
                boundCanvas.Children.Single(),
                "Canvas.Left",
                "10"));
    }

    [TestMethod]
    public void DockPanelPlacement_IsEditableOnlyForDirectChildrenAndPreservesNamespace()
    {
        const string markup =
            "<av:DockPanel xmlns:av='https://github.com/avaloniaui'><av:Button av:DockPanel.Dock='Top' /></av:DockPanel>";
        AxamlPreviewService service = new();
        AxamlPreviewNode dockPanel = service.Parse(markup);
        AxamlPreviewNode button = dockPanel.Children.Single();
        AxamlPropertyDescriptor placement = AxamlPropertyValueValidator.Describe(button, "DockPanel.Dock");

        Assert.AreEqual("DockPanel", button.ParentTypeName);
        Assert.IsTrue(service.GetEditableProperties(button).Contains("DockPanel.Dock"));
        Assert.AreEqual(AxamlPropertyEditorKind.Enum, placement.EditorKind);
        Assert.AreEqual("Top", placement.Value);
        CollectionAssert.AreEqual(
            new[] { "Left", "Top", "Right", "Bottom" },
            placement.Options.ToArray());
        Assert.IsNull(placement.ValidationError);
        Assert.IsNull(AxamlPropertyValueValidator.Validate("Button", "DockPanel.Dock", "Bottom"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("Button", "DockPanel.Dock", "Client"));

        string updated = service.UpdateAttribute(markup, button, "DockPanel.Dock", "Bottom");
        Assert.IsTrue(updated.Contains("av:DockPanel.Dock='Bottom'", StringComparison.Ordinal));

        const string unsetMarkup =
            "<av:DockPanel xmlns:av='https://github.com/avaloniaui'><av:Button /></av:DockPanel>";
        string inserted = service.UpdateAttribute(
            unsetMarkup,
            service.Parse(unsetMarkup).Children.Single(),
            "DockPanel.Dock",
            "Right");
        Assert.IsTrue(inserted.Contains("av:DockPanel.Dock=\"Right\"", StringComparison.Ordinal));

        AxamlPreviewNode defaultButton = service.Parse("<DockPanel><Button /></DockPanel>").Children.Single();
        Assert.AreEqual("Left", AxamlPropertyValueValidator.Describe(defaultButton, "DockPanel.Dock").Value);

        AxamlPreviewNode nestedButton = service.Parse("<DockPanel><StackPanel><Button /></StackPanel></DockPanel>")
            .Children.Single().Children.Single();
        Assert.IsFalse(service.GetEditableProperties(nestedButton).Contains("DockPanel.Dock"));

        const string boundMarkup = "<DockPanel><Button DockPanel.Dock='{Binding Side}' /></DockPanel>";
        AxamlPreviewNode boundButton = service.Parse(boundMarkup).Children.Single();
        Assert.IsFalse(service.CanEditProperty(boundButton, "DockPanel.Dock"));
        Assert.ThrowsExactly<InvalidOperationException>(
            () => service.UpdateAttribute(boundMarkup, boundButton, "DockPanel.Dock", "Right"));

        const string foreignMarkup =
            "<DockPanel xmlns:custom='urn:custom'><Button custom:DockPanel.Dock='Right' /></DockPanel>";
        AxamlPreviewNode foreignButton = service.Parse(foreignMarkup).Children.Single();
        Assert.ThrowsExactly<InvalidOperationException>(
            () => service.UpdateAttribute(foreignMarkup, foreignButton, "DockPanel.Dock", "Right"));
    }

    [TestMethod]
    public void GridCellPlacement_IsEditableForSafeDirectChildrenAndRejectsCollisions()
    {
        const string markup =
            "<av:Grid xmlns:av='https://github.com/avaloniaui' RowDefinitions='*,*' ColumnDefinitions='*,*'>"
            + "<av:Button av:Grid.Row='0' av:Grid.Column='0' />"
            + "<av:TextBlock av:Grid.Row='1' av:Grid.Column='1' /></av:Grid>";
        AxamlPreviewService service = new();
        AxamlPreviewNode grid = service.Parse(markup);
        AxamlPreviewNode button = grid.Children[0];
        AxamlPropertyDescriptor row = AxamlPropertyValueValidator.Describe(button, "Grid.Row");

        Assert.IsTrue(grid.HasSafeGridLayout);
        Assert.IsTrue(button.ParentHasSafeGridLayout);
        Assert.IsTrue(service.GetEditableProperties(button).Contains("Grid.Row"));
        Assert.IsTrue(service.GetEditableProperties(button).Contains("Grid.Column"));
        Assert.AreEqual(AxamlPropertyEditorKind.Number, row.EditorKind);
        Assert.AreEqual("0", row.Value);
        Assert.IsNull(row.ValidationError);
        AxamlPropertyDescriptor columnSpan = AxamlPropertyValueValidator.Describe(button, "Grid.ColumnSpan");
        Assert.AreEqual(AxamlPropertyEditorKind.Number, columnSpan.EditorKind);
        Assert.AreEqual("1", columnSpan.Value);
        Assert.IsNull(AxamlPropertyValueValidator.Validate("Button", "Grid.Row", "100000"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("Button", "Grid.Row", "-1"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("Button", "Grid.Column", "100001"));
        Assert.IsNull(AxamlPropertyValueValidator.Validate("Button", "Grid.ColumnSpan", "2"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("Button", "Grid.ColumnSpan", "0"));

        string updated = service.UpdateAttribute(markup, button, "Grid.Row", "1");
        Assert.IsTrue(updated.Contains("av:Grid.Row='1'", StringComparison.Ordinal));
        AxamlPreviewNode movedButton = service.Parse(updated).Children[0];
        Assert.AreEqual("1", movedButton.Properties["Grid.Row"]);
        Assert.ThrowsExactly<InvalidOperationException>(
            () => service.UpdateAttribute(updated, movedButton, "Grid.Column", "1"));
        Assert.ThrowsExactly<InvalidOperationException>(
            () => service.UpdateAttribute(updated, movedButton, "Grid.ColumnSpan", "2"));

        AxamlPreviewNode nestedButton = service.Parse(
            "<Grid RowDefinitions='*,*'><StackPanel><Button /></StackPanel></Grid>")
            .Children.Single().Children.Single();
        Assert.IsFalse(service.GetEditableProperties(nestedButton).Contains("Grid.Row"));

        const string unsafeMarkup = "<Grid RowDefinitions='*,*'><Button Grid.Row='{Binding Row}' /></Grid>";
        AxamlPreviewNode boundButton = service.Parse(unsafeMarkup).Children.Single();
        Assert.IsFalse(boundButton.ParentHasSafeGridLayout);
        Assert.IsFalse(service.GetEditableProperties(boundButton).Contains("Grid.Row"));
    }

    [TestMethod]
    public void Parse_ExposesEditableToggleSwitchContentAndNumericValue()
    {
        AxamlPreviewService service = new();
        AxamlPreviewNode root = service.Parse(
            "<StackPanel xmlns='https://github.com/avaloniaui'><NumericUpDown Value='12.5' />"
            + "<ToggleSwitch Content='Notifications' IsChecked='True' OnContent='Enabled' OffContent='Disabled' /></StackPanel>");
        AxamlPreviewNode numericUpDown = root.Children[0];
        AxamlPreviewNode toggleSwitch = root.Children[1];

        AxamlPropertyDescriptor numericValue = AxamlPropertyValueValidator.Describe(numericUpDown, "Value");
        Assert.AreEqual(AxamlPropertyEditorKind.Number, numericValue.EditorKind);
        Assert.AreEqual("12.5", numericValue.Value);
        Assert.IsNull(numericValue.ValidationError);
        Assert.IsNull(AxamlPropertyValueValidator.Validate("NumericUpDown", "Value", "14.75"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("NumericUpDown", "Value", "14,75"));

        Assert.AreEqual("Notifications", toggleSwitch.Text);
        Assert.IsTrue(service.CanEditProperty(toggleSwitch, "Content"));
        Assert.AreEqual("Enabled", AxamlPropertyValueValidator.Describe(toggleSwitch, "OnContent").Value);
        Assert.AreEqual("Disabled", AxamlPropertyValueValidator.Describe(toggleSwitch, "OffContent").Value);
        Assert.IsNull(AxamlPropertyValueValidator.Validate("ToggleSwitch", "OnContent", string.Empty));
        Assert.IsNull(AxamlPropertyValueValidator.Validate("ToggleSwitch", "OffContent", "Not active"));
        AxamlPreviewNode boundNumericUpDown = service.Parse(
            "<NumericUpDown Value='{Binding Quantity}' />");
        Assert.IsFalse(service.CanEditProperty(boundNumericUpDown, "Value"));
        AxamlPreviewNode boundToggleContent = service.Parse(
            "<ToggleSwitch OnContent='{Binding EnabledLabel}' />");
        Assert.IsTrue(AxamlPropertyValueValidator.Describe(boundToggleContent, "OnContent").IsReadOnly);
    }

    [TestMethod]
    public void Parse_ExposesBooleanStateForSelectorControls()
    {
        AxamlPreviewService service = new();
        AxamlPreviewNode root = service.Parse(
            "<StackPanel xmlns='https://github.com/avaloniaui'><CheckBox /><RadioButton /><ToggleSwitch /></StackPanel>");

        foreach (AxamlPreviewNode node in root.Children)
        {
            AxamlPropertyDescriptor isChecked = AxamlPropertyValueValidator.Describe(node, "IsChecked");
            Assert.AreEqual(AxamlPropertyEditorKind.Boolean, isChecked.EditorKind);
            Assert.AreEqual("False", isChecked.Value);
            Assert.IsFalse(isChecked.IsReadOnly);
            Assert.IsNull(isChecked.ValidationError);
            Assert.IsNull(AxamlPropertyValueValidator.Validate(node.TypeName, "IsChecked", "True"));
            Assert.IsNotNull(AxamlPropertyValueValidator.Validate(node.TypeName, "IsChecked", string.Empty));

            AxamlPreviewNode bound = service.Parse($"<{node.TypeName} IsChecked='{{Binding Selected}}' />");
            Assert.IsTrue(AxamlPropertyValueValidator.Describe(bound, "IsChecked").IsReadOnly);
        }

        AxamlPreviewNode checkBox = root.Children[0];
        AxamlPropertyDescriptor isThreeState = AxamlPropertyValueValidator.Describe(checkBox, "IsThreeState");
        Assert.AreEqual(AxamlPropertyEditorKind.Boolean, isThreeState.EditorKind);
        Assert.AreEqual("False", isThreeState.Value);
        Assert.IsNull(isThreeState.ValidationError);
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("CheckBox", "IsThreeState", string.Empty));
        Assert.IsTrue(AxamlPropertyValueValidator.Describe(
            service.Parse("<CheckBox IsThreeState='{Binding SupportsNull}' />"),
            "IsThreeState").IsReadOnly);

        AxamlPreviewNode toggleSwitch = root.Children[2];
        AxamlPropertyDescriptor toggleThreeState = AxamlPropertyValueValidator.Describe(toggleSwitch, "IsThreeState");
        Assert.AreEqual(AxamlPropertyEditorKind.Boolean, toggleThreeState.EditorKind);
        Assert.AreEqual("False", toggleThreeState.Value);
        Assert.IsNull(toggleThreeState.ValidationError);
        Assert.IsNull(AxamlPropertyValueValidator.Validate("ToggleSwitch", "IsThreeState", "True"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("ToggleSwitch", "IsThreeState", string.Empty));
        Assert.IsTrue(AxamlPropertyValueValidator.Describe(
            service.Parse("<ToggleSwitch IsThreeState='{Binding SupportsNull}' />"),
            "IsThreeState").IsReadOnly);
    }

    [TestMethod]
    public void Parse_ExposesRadioButtonGroupNameAsLiteralText()
    {
        AxamlPreviewService service = new();
        AxamlPreviewNode radioButton = service.Parse("<RadioButton GroupName='Modes' />");
        AxamlPropertyDescriptor groupName = AxamlPropertyValueValidator.Describe(radioButton, "GroupName");

        Assert.AreEqual(AxamlPropertyEditorKind.Text, groupName.EditorKind);
        Assert.AreEqual("Modes", groupName.Value);
        Assert.IsNull(groupName.ValidationError);
        Assert.IsNull(AxamlPropertyValueValidator.Validate("RadioButton", "GroupName", string.Empty));
        Assert.IsTrue(service.CanEditProperty(radioButton, "GroupName"));

        AxamlPreviewNode boundGroupName = service.Parse("<RadioButton GroupName='{Binding ModeGroup}' />");
        Assert.IsTrue(AxamlPropertyValueValidator.Describe(boundGroupName, "GroupName").IsReadOnly);
    }

    [TestMethod]
    public void Parse_ExposesSharedSelectorAppearanceSafely()
    {
        AxamlPreviewService service = new();
        string[] controlTypes = ["CheckBox", "RadioButton", "ToggleSwitch"];
        string[] appearanceProperties =
        [
            "Background",
            "BorderBrush",
            "BorderThickness",
            "CornerRadius",
            "FontSize",
            "FontStyle",
            "FontWeight",
            "Foreground",
            "Padding",
        ];
        string appearance =
            "Background='White' BorderBrush='Black' BorderThickness='2' CornerRadius='1,2,3,4' "
            + "FontSize='16' FontStyle='Italic' FontWeight='Medium' Foreground='Blue' Padding='4'";

        foreach (string controlType in controlTypes)
        {
            AxamlPreviewNode node = service.Parse($"<{controlType} {appearance} />");
            foreach (string propertyName in appearanceProperties)
            {
                AxamlPropertyDescriptor property = AxamlPropertyValueValidator.Describe(node, propertyName);
                Assert.IsFalse(property.IsReadOnly, $"{controlType}.{propertyName}");
                Assert.IsNull(property.ValidationError, $"{controlType}.{propertyName}");
            }

            AxamlPreviewNode bound = service.Parse(
                $"<{controlType} Background='{{DynamicResource Surface}}' CornerRadius='{{Binding Radius}}' />");
            Assert.IsTrue(AxamlPropertyValueValidator.Describe(bound, "Background").IsReadOnly);
            Assert.IsTrue(AxamlPropertyValueValidator.Describe(bound, "CornerRadius").IsReadOnly);
        }
    }

    [TestMethod]
    public void Parse_ExposesTemplatedAppearanceForRangeAndComboControls()
    {
        AxamlPreviewService service = new();
        string[] controlTypes = ["ComboBox", "NumericUpDown", "ProgressBar", "Slider"];
        string[] appearanceProperties =
        [
            "Background",
            "BorderBrush",
            "BorderThickness",
            "CornerRadius",
            "FontSize",
            "FontStyle",
            "FontWeight",
            "Foreground",
            "Padding",
        ];
        string appearance =
            "Background='White' BorderBrush='Black' BorderThickness='2' CornerRadius='1,2,3,4' "
            + "FontSize='16' FontStyle='Italic' FontWeight='Medium' Foreground='Blue' Padding='4'";

        foreach (string controlType in controlTypes)
        {
            AxamlPreviewNode node = service.Parse($"<{controlType} {appearance} />");
            foreach (string propertyName in appearanceProperties)
            {
                AxamlPropertyDescriptor property = AxamlPropertyValueValidator.Describe(node, propertyName);
                Assert.IsFalse(property.IsReadOnly, $"{controlType}.{propertyName}");
                Assert.IsNull(property.ValidationError, $"{controlType}.{propertyName}");
            }

            AxamlPreviewNode bound = service.Parse(
                $"<{controlType} Background='{{DynamicResource Surface}}' CornerRadius='{{Binding Radius}}' />");
            Assert.IsTrue(AxamlPropertyValueValidator.Describe(bound, "Background").IsReadOnly);
            Assert.IsTrue(AxamlPropertyValueValidator.Describe(bound, "CornerRadius").IsReadOnly);
        }
    }

    [TestMethod]
    public void Parse_ExposesComboBoxPlaceholderAsSafeLiteralText()
    {
        AxamlPreviewService service = new();
        AxamlPreviewNode comboBox = service.Parse("<ComboBox PlaceholderText='Choose an option' />");
        AxamlPropertyDescriptor placeholder = AxamlPropertyValueValidator.Describe(comboBox, "PlaceholderText");

        Assert.IsTrue(service.CanEditProperty(comboBox, "PlaceholderText"));
        Assert.AreEqual(AxamlPropertyEditorKind.Text, placeholder.EditorKind);
        Assert.AreEqual("Choose an option", placeholder.Value);
        Assert.IsNull(placeholder.ValidationError);
        Assert.IsNull(AxamlPropertyValueValidator.Validate("ComboBox", "PlaceholderText", string.Empty));
        Assert.AreEqual(
            "ComboBox",
            AxamlPropertyValueValidator.Describe(service.Parse("<ComboBox />"), "PlaceholderText").Value);

        AxamlPreviewNode boundComboBox = service.Parse("<ComboBox PlaceholderText='{Binding Hint}' />");
        Assert.IsTrue(AxamlPropertyValueValidator.Describe(boundComboBox, "PlaceholderText").IsReadOnly);
    }

    [TestMethod]
    public void Parse_ExposesSelectorItemContentAndInlineText()
    {
        AxamlPreviewService service = new();
        AxamlPreviewNode comboBox = service.Parse(
            "<ComboBox><ComboBoxItem Content='First choice' /><ComboBoxItem>Second choice</ComboBoxItem><ComboBoxItem Content='{Binding Label}' /><ComboBoxItem Content='{StaticResource Label}' /><ComboBoxItem Content='{DynamicResource Label}' /><ComboBoxItem Content='' /><ComboBoxItem><ComboBoxItem.Content><TextBlock Text='Property content' /></ComboBoxItem.Content></ComboBoxItem></ComboBox>");

        Assert.AreEqual("First choice", comboBox.Children[0].Text);
        Assert.AreEqual("Second choice", comboBox.Children[1].Text);
        Assert.AreEqual("[Binding Label]", comboBox.Children[2].Text);
        Assert.AreEqual("[StaticResource Label]", comboBox.Children[3].Text);
        Assert.AreEqual("[DynamicResource Label]", comboBox.Children[4].Text);
        Assert.AreEqual(string.Empty, comboBox.Children[5].Text);
        Assert.AreEqual("ComboBoxItem", comboBox.Children[6].TypeName);
        Assert.AreEqual("Property content", comboBox.Children[6].Children[0].Text);
    }

    [TestMethod]
    public void Parse_ExposesTextBlockWrappingAndForegroundSafely()
    {
        AxamlPreviewService service = new();
        AxamlPreviewNode textBlock = service.Parse("<TextBlock TextWrapping='NoWrap' Foreground='Red' />");
        AxamlPropertyDescriptor wrapping = AxamlPropertyValueValidator.Describe(textBlock, "TextWrapping");
        AxamlPropertyDescriptor foreground = AxamlPropertyValueValidator.Describe(textBlock, "Foreground");

        Assert.AreEqual(AxamlPropertyEditorKind.Enum, wrapping.EditorKind);
        Assert.AreEqual("NoWrap", wrapping.Value);
        Assert.IsNull(wrapping.ValidationError);
        Assert.AreEqual(AxamlPropertyEditorKind.Text, foreground.EditorKind);
        Assert.AreEqual("Red", foreground.Value);
        Assert.IsNull(foreground.ValidationError);
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("TextBlock", "Foreground", "LinearGradientBrush(Red,Blue)"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("TextBlock", "Foreground", "NotAColorName"));
        Assert.AreEqual("Wrap", AxamlPropertyValueValidator.Describe(
            service.Parse("<TextBlock />"),
            "TextWrapping").Value);
        CollectionAssert.AreEqual(
            new[] { "NoWrap", "Wrap", "WrapWithOverflow" },
            wrapping.Options.ToArray());
        Assert.IsNull(AxamlPropertyValueValidator.Validate("TextBlock", "TextWrapping", "WrapWithOverflow"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("TextBlock", "TextWrapping", "Clip"));
        Assert.IsTrue(AxamlPropertyValueValidator.Describe(
            service.Parse("<TextBlock TextWrapping='{Binding Wrapping}' />"),
            "TextWrapping").IsReadOnly);
        Assert.IsTrue(AxamlPropertyValueValidator.Describe(
            service.Parse("<TextBlock Foreground='{DynamicResource TextBrush}' />"),
            "Foreground").IsReadOnly);
    }

    [TestMethod]
    public void Parse_ExposesTextBlockFontWeightOptions()
    {
        AxamlPreviewService service = new();
        AxamlPreviewNode textBlock = service.Parse("<TextBlock FontWeight='SemiBold' />");
        AxamlPropertyDescriptor fontWeight = AxamlPropertyValueValidator.Describe(textBlock, "FontWeight");

        Assert.AreEqual(AxamlPropertyEditorKind.Enum, fontWeight.EditorKind);
        Assert.AreEqual("SemiBold", fontWeight.Value);
        CollectionAssert.AreEqual(
            new[] { "Thin", "Light", "Normal", "Medium", "SemiBold", "Bold", "ExtraBold", "Black" },
            fontWeight.Options.ToArray());
        Assert.IsNull(fontWeight.ValidationError);
        Assert.IsNull(AxamlPropertyValueValidator.Validate("TextBlock", "FontWeight", "Medium"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("TextBlock", "FontWeight", "ExtraBlack"));
        Assert.AreEqual("Normal", AxamlPropertyValueValidator.Describe(
            service.Parse("<TextBlock />"),
            "FontWeight").Value);
        Assert.IsTrue(AxamlPropertyValueValidator.Describe(
            service.Parse("<TextBlock FontWeight='{Binding Weight}' />"),
            "FontWeight").IsReadOnly);
    }

    [TestMethod]
    public void Parse_ExposesTextBoxForegroundSafely()
    {
        AxamlPreviewService service = new();
        AxamlPreviewNode textBox = service.Parse("<TextBox Foreground='#FF0000FF' />");
        AxamlPropertyDescriptor foreground = AxamlPropertyValueValidator.Describe(textBox, "Foreground");

        Assert.AreEqual(AxamlPropertyEditorKind.Text, foreground.EditorKind);
        Assert.AreEqual("#FF0000FF", foreground.Value);
        Assert.IsNull(foreground.ValidationError);
        Assert.IsNull(AxamlPropertyValueValidator.Validate("TextBox", "Foreground", "Blue"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("TextBox", "Foreground", "NotAColorName"));
        Assert.IsTrue(AxamlPropertyValueValidator.Describe(
            service.Parse("<TextBox Foreground='{DynamicResource TextBrush}' />"),
            "Foreground").IsReadOnly);
    }

    [TestMethod]
    public void Parse_ExposesTextBoxBackgroundAndPaddingSafely()
    {
        AxamlPreviewService service = new();
        AxamlPreviewNode textBox = service.Parse("<TextBox Background='#FFF0F0F0' BorderBrush='Blue' Padding='8,4' />");
        AxamlPropertyDescriptor background = AxamlPropertyValueValidator.Describe(textBox, "Background");
        AxamlPropertyDescriptor borderBrush = AxamlPropertyValueValidator.Describe(textBox, "BorderBrush");
        AxamlPropertyDescriptor padding = AxamlPropertyValueValidator.Describe(textBox, "Padding");

        Assert.AreEqual(AxamlPropertyEditorKind.Text, background.EditorKind);
        Assert.AreEqual("#FFF0F0F0", background.Value);
        Assert.IsNull(background.ValidationError);
        Assert.AreEqual(AxamlPropertyEditorKind.Text, borderBrush.EditorKind);
        Assert.AreEqual("Blue", borderBrush.Value);
        Assert.IsNull(borderBrush.ValidationError);
        Assert.AreEqual(AxamlPropertyEditorKind.Text, padding.EditorKind);
        Assert.AreEqual("8,4", padding.Value);
        Assert.IsNull(padding.ValidationError);
        Assert.IsNull(AxamlPropertyValueValidator.Validate("TextBox", "Background", "White"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("TextBox", "Background", "NotAColorName"));
        Assert.IsNull(AxamlPropertyValueValidator.Validate("TextBox", "BorderBrush", "Red"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("TextBox", "BorderBrush", "NotAColorName"));
        Assert.IsNull(AxamlPropertyValueValidator.Validate("TextBox", "Padding", "8,4"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("TextBox", "Padding", "8,4,2"));
        Assert.IsTrue(AxamlPropertyValueValidator.Describe(
            service.Parse("<TextBox Background='{DynamicResource TextBrush}' />"),
            "Background").IsReadOnly);
        Assert.IsTrue(AxamlPropertyValueValidator.Describe(
            service.Parse("<TextBox BorderBrush='{DynamicResource BorderBrush}' />"),
            "BorderBrush").IsReadOnly);
        Assert.IsTrue(AxamlPropertyValueValidator.Describe(
            service.Parse("<TextBox Padding='{Binding InnerPadding}' />"),
            "Padding").IsReadOnly);
    }

    [TestMethod]
    public void Parse_ExposesButtonAppearanceSafely()
    {
        AxamlPreviewService service = new();
        AxamlPreviewNode button = service.Parse(
            "<Button Background='Blue' BorderBrush='Black' BorderThickness='2' CornerRadius='6,4,2,8' Foreground='White' Padding='8,4' />");

        Assert.AreEqual("Blue", AxamlPropertyValueValidator.Describe(button, "Background").Value);
        Assert.AreEqual("Black", AxamlPropertyValueValidator.Describe(button, "BorderBrush").Value);
        Assert.AreEqual("2", AxamlPropertyValueValidator.Describe(button, "BorderThickness").Value);
        Assert.AreEqual("6,4,2,8", AxamlPropertyValueValidator.Describe(button, "CornerRadius").Value);
        Assert.AreEqual("White", AxamlPropertyValueValidator.Describe(button, "Foreground").Value);
        Assert.AreEqual("8,4", AxamlPropertyValueValidator.Describe(button, "Padding").Value);
        Assert.IsNull(AxamlPropertyValueValidator.Validate("Button", "Background", "Red"));
        Assert.IsNull(AxamlPropertyValueValidator.Validate("Button", "BorderBrush", "Red"));
        Assert.IsNull(AxamlPropertyValueValidator.Validate("Button", "BorderThickness", "2,4"));
        Assert.IsNull(AxamlPropertyValueValidator.Validate("Button", "CornerRadius", "6"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("Button", "BorderThickness", "-1"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("Button", "CornerRadius", "-1"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("Button", "Padding", "-1"));
        Assert.IsNull(AxamlPropertyValueValidator.Validate("Button", "Margin", "-1"));
        Assert.IsNull(AxamlPropertyValueValidator.Validate("Button", "Foreground", "Red"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("Button", "Foreground", "NotAColorName"));
        Assert.IsTrue(AxamlPropertyValueValidator.Describe(
            service.Parse("<Button Background='{DynamicResource ButtonBackground}' />"),
            "Background").IsReadOnly);
        Assert.IsTrue(AxamlPropertyValueValidator.Describe(
            service.Parse("<Button Padding='{Binding ButtonPadding}' />"),
            "Padding").IsReadOnly);
        Assert.IsTrue(AxamlPropertyValueValidator.Describe(
            service.Parse("<Button BorderThickness='{Binding Outline}' />"),
            "BorderThickness").IsReadOnly);
        Assert.IsTrue(AxamlPropertyValueValidator.Describe(
            service.Parse("<Button CornerRadius='{Binding Radius}' />"),
            "CornerRadius").IsReadOnly);
    }

    [TestMethod]
    public void Parse_ExposesButtonTypographySafely()
    {
        AxamlPreviewService service = new();
        AxamlPreviewNode button = service.Parse("<Button FontSize='18' FontStyle='Italic' FontWeight='SemiBold' />");

        Assert.AreEqual("18", AxamlPropertyValueValidator.Describe(button, "FontSize").Value);
        Assert.AreEqual("Italic", AxamlPropertyValueValidator.Describe(button, "FontStyle").Value);
        Assert.AreEqual("SemiBold", AxamlPropertyValueValidator.Describe(button, "FontWeight").Value);
        Assert.IsNull(AxamlPropertyValueValidator.Validate("Button", "FontSize", "18.5"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("Button", "FontSize", "-1"));
        Assert.IsNull(AxamlPropertyValueValidator.Validate("Button", "FontStyle", "Oblique"));
        Assert.IsNull(AxamlPropertyValueValidator.Validate("Button", "FontWeight", "Medium"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("Button", "FontStyle", "Slanted"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("Button", "FontWeight", "ExtraBlack"));
        Assert.IsTrue(AxamlPropertyValueValidator.Describe(
            service.Parse("<Button FontSize='{Binding Size}' />"),
            "FontSize").IsReadOnly);
        Assert.IsTrue(AxamlPropertyValueValidator.Describe(
            service.Parse("<Button FontStyle='{Binding Style}' />"),
            "FontStyle").IsReadOnly);
        Assert.IsTrue(AxamlPropertyValueValidator.Describe(
            service.Parse("<Button FontWeight='{Binding Weight}' />"),
            "FontWeight").IsReadOnly);
    }

    [TestMethod]
    public void Parse_ExposesTextBoxFrameSafely()
    {
        AxamlPreviewService service = new();
        AxamlPreviewNode textBox = service.Parse("<TextBox BorderThickness='2,3' CornerRadius='1,2,3,4' />");

        Assert.AreEqual("2,3", AxamlPropertyValueValidator.Describe(textBox, "BorderThickness").Value);
        Assert.AreEqual("1,2,3,4", AxamlPropertyValueValidator.Describe(textBox, "CornerRadius").Value);
        Assert.IsNull(AxamlPropertyValueValidator.Validate("TextBox", "BorderThickness", "1,2,3,4"));
        Assert.IsNull(AxamlPropertyValueValidator.Validate("TextBox", "CornerRadius", "6"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("TextBox", "BorderThickness", "-1"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("TextBox", "CornerRadius", "-1"));
        Assert.IsTrue(AxamlPropertyValueValidator.Describe(
            service.Parse("<TextBox BorderThickness='{Binding Outline}' />"),
            "BorderThickness").IsReadOnly);
        Assert.IsTrue(AxamlPropertyValueValidator.Describe(
            service.Parse("<TextBox CornerRadius='{Binding Radius}' />"),
            "CornerRadius").IsReadOnly);
    }

    [TestMethod]
    public void Parse_ExposesBorderBrushAndCornerRadiusSafely()
    {
        AxamlPreviewService service = new();
        AxamlPreviewNode border = service.Parse(
            "<Border BorderBrush='Purple' CornerRadius='12,4,8,2' BorderThickness='2' />");
        AxamlPropertyDescriptor borderBrush = AxamlPropertyValueValidator.Describe(border, "BorderBrush");
        AxamlPropertyDescriptor cornerRadius = AxamlPropertyValueValidator.Describe(border, "CornerRadius");

        Assert.AreEqual("Purple", borderBrush.Value);
        Assert.IsNull(borderBrush.ValidationError);
        Assert.AreEqual(AxamlPropertyEditorKind.Text, cornerRadius.EditorKind);
        Assert.AreEqual("12,4,8,2", cornerRadius.Value);
        Assert.IsNull(cornerRadius.ValidationError);
        Assert.AreEqual("0", AxamlPropertyValueValidator.Describe(service.Parse("<Border />"), "CornerRadius").Value);
        Assert.IsNull(AxamlPropertyValueValidator.Validate("Border", "BorderBrush", "Red"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("Border", "BorderBrush", "NotAColorName"));
        Assert.IsNull(AxamlPropertyValueValidator.Validate("Border", "CornerRadius", "6"));
        Assert.IsNull(AxamlPropertyValueValidator.Validate("Border", "CornerRadius", "1,2,3,4"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("Border", "CornerRadius", "-1"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("Border", "CornerRadius", "1,2"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("Border", "CornerRadius", "NaN"));
        Assert.IsTrue(AxamlPropertyValueValidator.Describe(
            service.Parse("<Border BorderBrush='{DynamicResource BorderBrush}' />"),
            "BorderBrush").IsReadOnly);
        Assert.IsTrue(AxamlPropertyValueValidator.Describe(
            service.Parse("<Border CornerRadius='{Binding Radius}' />"),
            "CornerRadius").IsReadOnly);
    }

    [TestMethod]
    public void Parse_ExposesTextBoxFontStyleAndWeightSafely()
    {
        AxamlPreviewService service = new();
        AxamlPreviewNode textBox = service.Parse("<TextBox FontStyle='Italic' FontWeight='Medium' />");
        AxamlPropertyDescriptor fontStyle = AxamlPropertyValueValidator.Describe(textBox, "FontStyle");
        AxamlPropertyDescriptor fontWeight = AxamlPropertyValueValidator.Describe(textBox, "FontWeight");

        Assert.AreEqual(AxamlPropertyEditorKind.Enum, fontStyle.EditorKind);
        Assert.AreEqual("Italic", fontStyle.Value);
        CollectionAssert.AreEqual(new[] { "Normal", "Italic", "Oblique" }, fontStyle.Options.ToArray());
        Assert.AreEqual(AxamlPropertyEditorKind.Enum, fontWeight.EditorKind);
        Assert.AreEqual("Medium", fontWeight.Value);
        CollectionAssert.AreEqual(
            new[] { "Thin", "Light", "Normal", "Medium", "SemiBold", "Bold", "ExtraBold", "Black" },
            fontWeight.Options.ToArray());
        Assert.IsNull(AxamlPropertyValueValidator.Validate("TextBox", "FontStyle", "Oblique"));
        Assert.IsNull(AxamlPropertyValueValidator.Validate("TextBox", "FontWeight", "Black"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("TextBox", "FontStyle", "Slanted"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("TextBox", "FontWeight", "ExtraBlack"));

        AxamlPreviewNode defaults = service.Parse("<TextBox />");
        Assert.AreEqual("Normal", AxamlPropertyValueValidator.Describe(defaults, "FontStyle").Value);
        Assert.AreEqual("Normal", AxamlPropertyValueValidator.Describe(defaults, "FontWeight").Value);
        Assert.IsTrue(AxamlPropertyValueValidator.Describe(
            service.Parse("<TextBox FontStyle='{Binding Style}' />"),
            "FontStyle").IsReadOnly);
        Assert.IsTrue(AxamlPropertyValueValidator.Describe(
            service.Parse("<TextBox FontWeight='{Binding Weight}' />"),
            "FontWeight").IsReadOnly);
    }

    [TestMethod]
    public void Parse_ExposesTextBoxFontSizeAndControlAlignmentSafely()
    {
        AxamlPreviewService service = new();
        AxamlPreviewNode textBox = service.Parse(
            "<TextBox FontSize='18' HorizontalAlignment='Center' VerticalAlignment='Bottom' />");
        AxamlPropertyDescriptor fontSize = AxamlPropertyValueValidator.Describe(textBox, "FontSize");
        AxamlPropertyDescriptor horizontalAlignment = AxamlPropertyValueValidator.Describe(textBox, "HorizontalAlignment");
        AxamlPropertyDescriptor verticalAlignment = AxamlPropertyValueValidator.Describe(textBox, "VerticalAlignment");

        Assert.AreEqual(AxamlPropertyEditorKind.Number, fontSize.EditorKind);
        Assert.AreEqual("18", fontSize.Value);
        Assert.IsNull(fontSize.ValidationError);
        Assert.AreEqual("Center", horizontalAlignment.Value);
        Assert.AreEqual("Bottom", verticalAlignment.Value);
        CollectionAssert.AreEqual(new[] { "Left", "Center", "Right", "Stretch" }, horizontalAlignment.Options.ToArray());
        CollectionAssert.AreEqual(new[] { "Top", "Center", "Bottom", "Stretch" }, verticalAlignment.Options.ToArray());
        Assert.IsNull(AxamlPropertyValueValidator.Validate("TextBox", "FontSize", "24.5"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("TextBox", "FontSize", "-1"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("TextBox", "HorizontalAlignment", "Baseline"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("TextBox", "VerticalAlignment", "Baseline"));

        AxamlPreviewNode defaults = service.Parse("<TextBox />");
        Assert.AreEqual("Stretch", AxamlPropertyValueValidator.Describe(defaults, "HorizontalAlignment").Value);
        Assert.AreEqual("Stretch", AxamlPropertyValueValidator.Describe(defaults, "VerticalAlignment").Value);
        Assert.IsTrue(AxamlPropertyValueValidator.Describe(
            service.Parse("<TextBox FontSize='{Binding Size}' />"),
            "FontSize").IsReadOnly);
        Assert.IsTrue(AxamlPropertyValueValidator.Describe(
            service.Parse("<TextBox HorizontalAlignment='{Binding Alignment}' />"),
            "HorizontalAlignment").IsReadOnly);
        Assert.IsTrue(AxamlPropertyValueValidator.Describe(
            service.Parse("<TextBox VerticalAlignment='{Binding Alignment}' />"),
            "VerticalAlignment").IsReadOnly);
    }

    [TestMethod]
    public void Parse_ExposesTextOpacityWithinSafeRange()
    {
        AxamlPreviewService service = new();
        AxamlPreviewNode textBlock = service.Parse("<TextBlock Opacity='0.5' />");
        AxamlPreviewNode textBox = service.Parse("<TextBox Opacity='0.75' />");
        AxamlPreviewNode button = service.Parse("<Button Opacity='0.25' />");
        AxamlPropertyDescriptor textBlockOpacity = AxamlPropertyValueValidator.Describe(textBlock, "Opacity");
        AxamlPropertyDescriptor textBoxOpacity = AxamlPropertyValueValidator.Describe(textBox, "Opacity");

        Assert.AreEqual(AxamlPropertyEditorKind.Number, textBlockOpacity.EditorKind);
        Assert.AreEqual("0.5", textBlockOpacity.Value);
        Assert.AreEqual("0.75", textBoxOpacity.Value);
        Assert.AreEqual("0.25", AxamlPropertyValueValidator.Describe(button, "Opacity").Value);
        Assert.AreEqual("1", AxamlPropertyValueValidator.Describe(service.Parse("<TextBlock />"), "Opacity").Value);
        Assert.AreEqual("1", AxamlPropertyValueValidator.Describe(service.Parse("<TextBox />"), "Opacity").Value);
        Assert.IsNull(AxamlPropertyValueValidator.Validate("TextBlock", "Opacity", "0"));
        Assert.IsNull(AxamlPropertyValueValidator.Validate("TextBox", "Opacity", "1"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("TextBlock", "Opacity", "-0.01"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("TextBox", "Opacity", "1.01"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("TextBlock", "Opacity", "NaN"));
        Assert.IsTrue(AxamlPropertyValueValidator.Describe(
            service.Parse("<TextBlock Opacity='{Binding Alpha}' />"),
            "Opacity").IsReadOnly);
        Assert.IsTrue(AxamlPropertyValueValidator.Describe(
            service.Parse("<TextBox Opacity='{Binding Alpha}' />"),
            "Opacity").IsReadOnly);
        Assert.IsTrue(AxamlPropertyValueValidator.Describe(
            service.Parse("<Button Opacity='{Binding Alpha}' />"),
            "Opacity").IsReadOnly);
    }

    [TestMethod]
    public void Parse_ExposesTextBlockLineHeightAsPositiveNumber()
    {
        AxamlPreviewService service = new();
        AxamlPreviewNode textBlock = service.Parse("<TextBlock LineHeight='24.5' />");
        AxamlPropertyDescriptor lineHeight = AxamlPropertyValueValidator.Describe(textBlock, "LineHeight");

        Assert.AreEqual(AxamlPropertyEditorKind.Number, lineHeight.EditorKind);
        Assert.AreEqual("24.5", lineHeight.Value);
        Assert.IsNull(lineHeight.ValidationError);
        Assert.AreEqual(string.Empty, AxamlPropertyValueValidator.Describe(
            service.Parse("<TextBlock />"),
            "LineHeight").Value);
        Assert.IsNull(AxamlPropertyValueValidator.Validate("TextBlock", "LineHeight", "1"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("TextBlock", "LineHeight", "0"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("TextBlock", "LineHeight", "-1"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("TextBlock", "LineHeight", "NaN"));
        Assert.IsTrue(AxamlPropertyValueValidator.Describe(
            service.Parse("<TextBlock LineHeight='{Binding Height}' />"),
            "LineHeight").IsReadOnly);
    }

    [TestMethod]
    public void Parse_ExposesSizeConstraintsAsSafeNumbers()
    {
        AxamlPreviewService service = new();
        AxamlPreviewNode stackPanel = service.Parse(
            "<StackPanel MinWidth='40' MaxWidth='120' MinHeight='20' MaxHeight='80' />");

        foreach (string propertyName in new[] { "MinWidth", "MaxWidth", "MinHeight", "MaxHeight" })
        {
            AxamlPropertyDescriptor property = AxamlPropertyValueValidator.Describe(stackPanel, propertyName);
            Assert.AreEqual(AxamlPropertyEditorKind.Number, property.EditorKind, propertyName);
            Assert.IsNull(property.ValidationError, propertyName);
            Assert.IsNull(AxamlPropertyValueValidator.Validate("StackPanel", propertyName, "0"), propertyName);
            Assert.IsNotNull(AxamlPropertyValueValidator.Validate("StackPanel", propertyName, "-1"), propertyName);
            Assert.IsNotNull(AxamlPropertyValueValidator.Validate("StackPanel", propertyName, "NaN"), propertyName);
            Assert.IsNotNull(AxamlPropertyValueValidator.Validate("StackPanel", propertyName, "Infinity"), propertyName);
        }

        Assert.AreEqual("40", AxamlPropertyValueValidator.Describe(stackPanel, "MinWidth").Value);
        Assert.AreEqual("120", AxamlPropertyValueValidator.Describe(stackPanel, "MaxWidth").Value);
        Assert.AreEqual("20", AxamlPropertyValueValidator.Describe(stackPanel, "MinHeight").Value);
        Assert.AreEqual("80", AxamlPropertyValueValidator.Describe(stackPanel, "MaxHeight").Value);
        Assert.AreEqual(string.Empty, AxamlPropertyValueValidator.Describe(service.Parse("<StackPanel />"), "MaxWidth").Value);
        Assert.IsTrue(AxamlPropertyValueValidator.Describe(
            service.Parse("<StackPanel MinWidth='{Binding Minimum}' />"),
            "MinWidth").IsReadOnly);
        AxamlPreviewNode comboBox = service.Parse("<ComboBox MinWidth='40' />");
        AxamlPropertyDescriptor comboBoxMinWidth = AxamlPropertyValueValidator.Describe(comboBox, "MinWidth");
        Assert.IsTrue(service.CanEditProperty(comboBox, "MinWidth"));
        Assert.AreEqual(AxamlPropertyEditorKind.Number, comboBoxMinWidth.EditorKind);
        Assert.AreEqual("40", comboBoxMinWidth.Value);
        Assert.IsNull(comboBoxMinWidth.ValidationError);
    }

    [TestMethod]
    public void Parse_ExposesTextBoxInputConstraintsAsTypedProperties()
    {
        AxamlPreviewService service = new();
        AxamlPreviewNode textBox = service.Parse("<TextBox />");
        AxamlPropertyDescriptor acceptsReturn = AxamlPropertyValueValidator.Describe(textBox, "AcceptsReturn");
        AxamlPropertyDescriptor acceptsTab = AxamlPropertyValueValidator.Describe(textBox, "AcceptsTab");
        AxamlPropertyDescriptor maxLength = AxamlPropertyValueValidator.Describe(textBox, "MaxLength");

        Assert.AreEqual(AxamlPropertyEditorKind.Boolean, acceptsReturn.EditorKind);
        Assert.AreEqual("False", acceptsReturn.Value);
        Assert.IsNull(acceptsReturn.ValidationError);
        Assert.AreEqual(AxamlPropertyEditorKind.Boolean, acceptsTab.EditorKind);
        Assert.AreEqual("False", acceptsTab.Value);
        Assert.IsNull(acceptsTab.ValidationError);
        Assert.AreEqual(AxamlPropertyEditorKind.Number, maxLength.EditorKind);
        Assert.AreEqual("0", maxLength.Value);
        Assert.IsNull(maxLength.ValidationError);
        Assert.IsNull(AxamlPropertyValueValidator.Validate("TextBox", "AcceptsReturn", "True"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("TextBox", "AcceptsReturn", string.Empty));
        Assert.IsNull(AxamlPropertyValueValidator.Validate("TextBox", "AcceptsTab", "True"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("TextBox", "AcceptsTab", "yes"));
        Assert.IsNull(AxamlPropertyValueValidator.Validate("TextBox", "MaxLength", "100"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("TextBox", "MaxLength", "-1"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("TextBox", "MaxLength", "1.5"));

        AxamlPreviewNode boundMaxLength = service.Parse("<TextBox MaxLength='{Binding LengthLimit}' />");
        Assert.IsTrue(AxamlPropertyValueValidator.Describe(boundMaxLength, "MaxLength").IsReadOnly);
        Assert.IsTrue(AxamlPropertyValueValidator.Describe(
            service.Parse("<TextBox AcceptsTab='{Binding IndentWithTab}' />"),
            "AcceptsTab").IsReadOnly);
    }

    [TestMethod]
    public void Parse_ExposesSafeTextAlignmentAndTextBoxWrappingEnums()
    {
        AxamlPreviewService service = new();
        AxamlPreviewNode textBlock = service.Parse("<TextBlock />");
        AxamlPropertyDescriptor textBlockAlignment = AxamlPropertyValueValidator.Describe(textBlock, "TextAlignment");
        Assert.AreEqual(AxamlPropertyEditorKind.Enum, textBlockAlignment.EditorKind);
        Assert.AreEqual("Left", textBlockAlignment.Value);
        CollectionAssert.AreEqual(
            new[] { "Left", "Center", "Right", "Start", "End", "DetectFromContent", "Justify" },
            textBlockAlignment.Options.ToArray());
        Assert.IsNull(AxamlPropertyValueValidator.Validate("TextBlock", "TextAlignment", "Justify"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("TextBlock", "TextAlignment", "Baseline"));
        Assert.IsTrue(AxamlPropertyValueValidator.Describe(
            service.Parse("<TextBlock TextAlignment='{Binding Alignment}' />"),
            "TextAlignment").IsReadOnly);

        AxamlPreviewNode textBox = service.Parse("<TextBox />");
        AxamlPropertyDescriptor textBoxAlignment = AxamlPropertyValueValidator.Describe(textBox, "TextAlignment");
        AxamlPropertyDescriptor textBoxWrapping = AxamlPropertyValueValidator.Describe(textBox, "TextWrapping");
        Assert.AreEqual("Left", textBoxAlignment.Value);
        Assert.AreEqual(AxamlPropertyEditorKind.Enum, textBoxWrapping.EditorKind);
        Assert.AreEqual("NoWrap", textBoxWrapping.Value);
        CollectionAssert.AreEqual(
            new[] { "NoWrap", "Wrap", "WrapWithOverflow" },
            textBoxWrapping.Options.ToArray());
        Assert.IsNull(AxamlPropertyValueValidator.Validate("TextBox", "TextWrapping", "WrapWithOverflow"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("TextBox", "TextWrapping", "Clip"));
        Assert.IsTrue(AxamlPropertyValueValidator.Describe(
            service.Parse("<TextBox TextWrapping='{Binding Wrapping}' />"),
            "TextWrapping").IsReadOnly);
    }

    [TestMethod]
    public void Parse_ExposesButtonDefaultAndCancelAsSafeBooleanProperties()
    {
        AxamlPreviewService service = new();
        AxamlPreviewNode button = service.Parse("<Button />");
        AxamlPropertyDescriptor isDefault = AxamlPropertyValueValidator.Describe(button, "IsDefault");
        AxamlPropertyDescriptor isCancel = AxamlPropertyValueValidator.Describe(button, "IsCancel");

        Assert.AreEqual(AxamlPropertyEditorKind.Boolean, isDefault.EditorKind);
        Assert.AreEqual("False", isDefault.Value);
        Assert.AreEqual(AxamlPropertyEditorKind.Boolean, isCancel.EditorKind);
        Assert.AreEqual("False", isCancel.Value);
        Assert.IsNull(isDefault.ValidationError);
        Assert.IsNull(isCancel.ValidationError);
        Assert.IsNull(AxamlPropertyValueValidator.Validate("Button", "IsDefault", "True"));
        Assert.IsNull(AxamlPropertyValueValidator.Validate("Button", "IsCancel", "False"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("Button", "IsDefault", string.Empty));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("Button", "IsCancel", "yes"));

        Assert.IsTrue(AxamlPropertyValueValidator.Describe(
            service.Parse("<Button IsDefault='{Binding DefaultAction}' />"),
            "IsDefault").IsReadOnly);
        Assert.IsTrue(AxamlPropertyValueValidator.Describe(
            service.Parse("<Button IsCancel='{Binding CancelAction}' />"),
            "IsCancel").IsReadOnly);
    }

    [TestMethod]
    public void Parse_ExposesTextBlockFontStyleAsAValidatedEnum()
    {
        AxamlPreviewService service = new();
        AxamlPropertyDescriptor fontStyle = AxamlPropertyValueValidator.Describe(
            service.Parse("<TextBlock />"),
            "FontStyle");

        Assert.AreEqual(AxamlPropertyEditorKind.Enum, fontStyle.EditorKind);
        Assert.AreEqual("Normal", fontStyle.Value);
        CollectionAssert.AreEqual(new[] { "Normal", "Italic", "Oblique" }, fontStyle.Options.ToArray());
        Assert.IsNull(fontStyle.ValidationError);
        Assert.IsNull(AxamlPropertyValueValidator.Validate("TextBlock", "FontStyle", "Italic"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("TextBlock", "FontStyle", "Slanted"));
        Assert.IsTrue(AxamlPropertyValueValidator.Describe(
            service.Parse("<TextBlock FontStyle='{Binding Style}' />"),
            "FontStyle").IsReadOnly);
    }

    [TestMethod]
    public void Parse_ExposesTextBlockMaxLinesAsNonNegativeInteger()
    {
        AxamlPreviewService service = new();
        AxamlPropertyDescriptor maxLines = AxamlPropertyValueValidator.Describe(
            service.Parse("<TextBlock />"),
            "MaxLines");

        Assert.AreEqual(AxamlPropertyEditorKind.Number, maxLines.EditorKind);
        Assert.AreEqual("0", maxLines.Value);
        Assert.IsNull(maxLines.ValidationError);
        Assert.IsNull(AxamlPropertyValueValidator.Validate("TextBlock", "MaxLines", "3"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("TextBlock", "MaxLines", "-1"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("TextBlock", "MaxLines", "1.5"));
        Assert.IsTrue(AxamlPropertyValueValidator.Describe(
            service.Parse("<TextBlock MaxLines='{Binding LineLimit}' />"),
            "MaxLines").IsReadOnly);
    }

    [TestMethod]
    public void Parse_ExposesCommonTextTrimmingModesAsSafeChoices()
    {
        AxamlPreviewService service = new();
        AxamlPropertyDescriptor trimming = AxamlPropertyValueValidator.Describe(
            service.Parse("<TextBlock />"),
            "TextTrimming");

        Assert.AreEqual(AxamlPropertyEditorKind.Enum, trimming.EditorKind);
        Assert.AreEqual("None", trimming.Value);
        CollectionAssert.AreEqual(new[] { "None", "CharacterEllipsis", "WordEllipsis" }, trimming.Options.ToArray());
        Assert.IsNull(trimming.ValidationError);
        Assert.IsNull(AxamlPropertyValueValidator.Validate("TextBlock", "TextTrimming", "CharacterEllipsis"));
        Assert.IsNull(AxamlPropertyValueValidator.Validate("TextBlock", "TextTrimming", "WordEllipsis"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("TextBlock", "TextTrimming", "PathSegmentEllipsis"));
        Assert.IsTrue(AxamlPropertyValueValidator.Describe(
            service.Parse("<TextBlock TextTrimming='{Binding Trimming}' />"),
            "TextTrimming").IsReadOnly);
    }

    [TestMethod]
    public void Parse_ExposesFiniteProgressBarValueAsNumericProperty()
    {
        AxamlPropertyDescriptor value = AxamlPropertyValueValidator.Describe(
            new AxamlPreviewService().Parse("<ProgressBar />"),
            "Value");

        Assert.AreEqual(AxamlPropertyEditorKind.Number, value.EditorKind);
        Assert.AreEqual("50", value.Value);
        Assert.IsNull(value.ValidationError);
        Assert.IsNull(AxamlPropertyValueValidator.Validate("ProgressBar", "Value", "42.5"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("ProgressBar", "Value", "NaN"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("ProgressBar", "Value", "Infinity"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("ProgressBar", "Value", string.Empty));

        AxamlPreviewNode progressBar = new AxamlPreviewService()
            .Parse("<ProgressBar IsIndeterminate='True' />");
        AxamlPropertyDescriptor indeterminate = AxamlPropertyValueValidator.Describe(progressBar, "IsIndeterminate");
        Assert.AreEqual(AxamlPropertyEditorKind.Boolean, indeterminate.EditorKind);
        Assert.AreEqual("True", indeterminate.Value);
        Assert.IsNull(indeterminate.ValidationError);
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("ProgressBar", "IsIndeterminate", string.Empty));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("ProgressBar", "IsIndeterminate", "sometimes"));

        AxamlPreviewNode boundProgressBar = new AxamlPreviewService()
            .Parse("<ProgressBar Value='{Binding Progress}' />");
        Assert.IsTrue(AxamlPropertyValueValidator.Describe(boundProgressBar, "Value").IsReadOnly);
        AxamlPreviewNode boundIndeterminate = new AxamlPreviewService()
            .Parse("<ProgressBar IsIndeterminate='{Binding Busy}' />");
        Assert.IsTrue(AxamlPropertyValueValidator.Describe(boundIndeterminate, "IsIndeterminate").IsReadOnly);
    }

    [TestMethod]
    public void Parse_ExposesFiniteSliderValueAsNumericProperty()
    {
        AxamlPreviewService service = new();
        AxamlPreviewNode slider = service.Parse("<Slider />");
        AxamlPropertyDescriptor value = AxamlPropertyValueValidator.Describe(slider, "Value");

        Assert.IsTrue(service.CanEditProperty(slider, "Value"));
        Assert.AreEqual(AxamlPropertyEditorKind.Number, value.EditorKind);
        Assert.AreEqual("50", value.Value);
        Assert.IsNull(value.ValidationError);
        Assert.IsNull(AxamlPropertyValueValidator.Validate("Slider", "Value", "42.5"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("Slider", "Value", "NaN"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("Slider", "Value", "Infinity"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("Slider", "Value", string.Empty));

        AxamlPreviewNode boundSlider = service.Parse("<Slider Value='{Binding Progress}' />");
        Assert.IsTrue(AxamlPropertyValueValidator.Describe(boundSlider, "Value").IsReadOnly);
    }

    [TestMethod]
    public void Parse_ExposesRangeBoundsAsFiniteEditableNumbers()
    {
        AxamlPreviewService service = new();
        foreach (string controlType in new[] { "NumericUpDown", "ProgressBar", "Slider" })
        {
            AxamlPreviewNode node = service.Parse($"<{controlType} />");
            string defaultMinimum = controlType == "NumericUpDown" ? string.Empty : "0";
            string defaultMaximum = controlType == "NumericUpDown" ? string.Empty : "100";
            foreach ((string propertyName, string defaultValue) in new[]
            {
                ("Minimum", defaultMinimum),
                ("Maximum", defaultMaximum),
            })
            {
                AxamlPropertyDescriptor property = AxamlPropertyValueValidator.Describe(node, propertyName);
                Assert.AreEqual(AxamlPropertyEditorKind.Number, property.EditorKind, $"{controlType}.{propertyName}");
                Assert.AreEqual(defaultValue, property.Value, $"{controlType}.{propertyName}");
                Assert.IsFalse(property.IsReadOnly, $"{controlType}.{propertyName}");
                Assert.IsNull(property.ValidationError, $"{controlType}.{propertyName}");
                Assert.IsNull(AxamlPropertyValueValidator.Validate(controlType, propertyName, "12.5"));
                Assert.IsNotNull(AxamlPropertyValueValidator.Validate(controlType, propertyName, "NaN"));

                AxamlPreviewNode bound = service.Parse(
                    $"<{controlType} {propertyName}='{{Binding Bound}}' />");
                Assert.IsTrue(
                    AxamlPropertyValueValidator.Describe(bound, propertyName).IsReadOnly,
                    $"{controlType}.{propertyName} binding");
            }
        }
    }

    [TestMethod]
    public void Parse_ExposesPositiveNumericUpDownIncrementSafely()
    {
        AxamlPreviewService service = new();
        AxamlPreviewNode node = service.Parse("<NumericUpDown Increment='0.25' />");
        AxamlPropertyDescriptor increment = AxamlPropertyValueValidator.Describe(node, "Increment");

        Assert.IsTrue(service.CanEditProperty(node, "Increment"));
        Assert.AreEqual(AxamlPropertyEditorKind.Number, increment.EditorKind);
        Assert.AreEqual("0.25", increment.Value);
        Assert.IsNull(increment.ValidationError);
        Assert.IsNull(AxamlPropertyValueValidator.Validate("NumericUpDown", "Increment", "2.5"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("NumericUpDown", "Increment", "0"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("NumericUpDown", "Increment", "-0.5"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("NumericUpDown", "Increment", "NaN"));
        Assert.AreEqual(
            "1",
            AxamlPropertyValueValidator.Describe(service.Parse("<NumericUpDown />"), "Increment").Value);

        AxamlPreviewNode bound = service.Parse("<NumericUpDown Increment='{Binding Step}' />");
        Assert.IsTrue(AxamlPropertyValueValidator.Describe(bound, "Increment").IsReadOnly);
    }

    [TestMethod]
    public void Parse_ExposesNumericUpDownOptionsSafely()
    {
        AxamlPreviewService service = new();
        AxamlPreviewNode defaults = service.Parse("<NumericUpDown />");
        Assert.AreEqual("True", AxamlPropertyValueValidator.Describe(defaults, "AllowSpin").Value);
        Assert.AreEqual("True", AxamlPropertyValueValidator.Describe(defaults, "ShowButtonSpinner").Value);
        Assert.AreEqual("False", AxamlPropertyValueValidator.Describe(defaults, "ClipValueToMinMax").Value);
        AxamlPropertyDescriptor defaultLocation = AxamlPropertyValueValidator.Describe(defaults, "ButtonSpinnerLocation");
        Assert.AreEqual(AxamlPropertyEditorKind.Enum, defaultLocation.EditorKind);
        Assert.AreEqual("Right", defaultLocation.Value);
        CollectionAssert.AreEqual(new[] { "Left", "Right" }, defaultLocation.Options.ToArray());
        AxamlPropertyDescriptor defaultTextAlignment = AxamlPropertyValueValidator.Describe(defaults, "TextAlignment");
        Assert.AreEqual(AxamlPropertyEditorKind.Enum, defaultTextAlignment.EditorKind);
        Assert.AreEqual("Start", defaultTextAlignment.Value);
        AxamlPropertyDescriptor defaultParsingStyle = AxamlPropertyValueValidator.Describe(defaults, "ParsingNumberStyle");
        Assert.AreEqual(AxamlPropertyEditorKind.Enum, defaultParsingStyle.EditorKind);
        Assert.AreEqual("Any", defaultParsingStyle.Value);
        CollectionAssert.AreEqual(new[] { "Any", "Currency", "Float", "Integer", "None", "Number" }, defaultParsingStyle.Options.ToArray());

        AxamlPreviewNode configured = service.Parse(
            "<NumericUpDown AllowSpin='False' ShowButtonSpinner='False' ClipValueToMinMax='True' ButtonSpinnerLocation='Left' TextAlignment='Center' ParsingNumberStyle='Integer' />");
        foreach (string propertyName in new[] { "AllowSpin", "ShowButtonSpinner", "ClipValueToMinMax" })
        {
            Assert.AreEqual(AxamlPropertyEditorKind.Boolean,
                AxamlPropertyValueValidator.Describe(configured, propertyName).EditorKind,
                propertyName);
            Assert.IsNull(AxamlPropertyValueValidator.Validate("NumericUpDown", propertyName, "True"), propertyName);
            Assert.IsNotNull(AxamlPropertyValueValidator.Validate("NumericUpDown", propertyName, "enabled"), propertyName);
        }

        Assert.AreEqual("False", AxamlPropertyValueValidator.Describe(configured, "AllowSpin").Value);
        Assert.AreEqual("False", AxamlPropertyValueValidator.Describe(configured, "ShowButtonSpinner").Value);
        Assert.AreEqual("True", AxamlPropertyValueValidator.Describe(configured, "ClipValueToMinMax").Value);
        Assert.AreEqual("Left", AxamlPropertyValueValidator.Describe(configured, "ButtonSpinnerLocation").Value);
        Assert.AreEqual("Center", AxamlPropertyValueValidator.Describe(configured, "TextAlignment").Value);
        Assert.AreEqual("Integer", AxamlPropertyValueValidator.Describe(configured, "ParsingNumberStyle").Value);
        Assert.IsNull(AxamlPropertyValueValidator.Validate("NumericUpDown", "ButtonSpinnerLocation", "Right"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("NumericUpDown", "ButtonSpinnerLocation", "Top"));
        Assert.IsNull(AxamlPropertyValueValidator.Validate("NumericUpDown", "TextAlignment", "Right"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("NumericUpDown", "TextAlignment", "Baseline"));
        foreach (string style in new[] { "Any", "Currency", "Float", "Integer", "None", "Number" })
        {
            Assert.IsNull(AxamlPropertyValueValidator.Validate("NumericUpDown", "ParsingNumberStyle", style), style);
        }
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("NumericUpDown", "ParsingNumberStyle", "AllowHexSpecifier"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("NumericUpDown", "ParsingNumberStyle", "Integer, AllowHexSpecifier"));
        foreach (string propertyName in new[] { "AllowSpin", "ShowButtonSpinner", "ClipValueToMinMax" })
        {
            AxamlPreviewNode bound = service.Parse($"<NumericUpDown {propertyName}='{{Binding Option}}' />");
            Assert.IsTrue(AxamlPropertyValueValidator.Describe(bound, propertyName).IsReadOnly, propertyName);
        }
        Assert.IsTrue(AxamlPropertyValueValidator.Describe(
            service.Parse("<NumericUpDown ButtonSpinnerLocation='{Binding Location}' />"),
            "ButtonSpinnerLocation").IsReadOnly);
        Assert.IsTrue(AxamlPropertyValueValidator.Describe(
            service.Parse("<NumericUpDown TextAlignment='{Binding Alignment}' />"),
            "TextAlignment").IsReadOnly);
        Assert.IsTrue(AxamlPropertyValueValidator.Describe(
            service.Parse("<NumericUpDown ParsingNumberStyle='{Binding NumberStyle}' />"),
            "ParsingNumberStyle").IsReadOnly);
    }

    [TestMethod]
    public void Parse_ExposesSliderTickSettingsSafely()
    {
        AxamlPreviewService service = new();
        AxamlPreviewNode slider = service.Parse("<Slider TickFrequency='5' IsSnapToTickEnabled='True' TickPlacement='Outside' />");
        AxamlPropertyDescriptor frequency = AxamlPropertyValueValidator.Describe(slider, "TickFrequency");
        AxamlPropertyDescriptor snap = AxamlPropertyValueValidator.Describe(slider, "IsSnapToTickEnabled");
        AxamlPropertyDescriptor placement = AxamlPropertyValueValidator.Describe(slider, "TickPlacement");

        Assert.AreEqual(AxamlPropertyEditorKind.Number, frequency.EditorKind);
        Assert.AreEqual("5", frequency.Value);
        Assert.IsNull(frequency.ValidationError);
        Assert.IsNull(AxamlPropertyValueValidator.Validate("Slider", "TickFrequency", "0"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("Slider", "TickFrequency", "-1"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("Slider", "TickFrequency", "Infinity"));

        Assert.AreEqual(AxamlPropertyEditorKind.Boolean, snap.EditorKind);
        Assert.AreEqual("True", snap.Value);
        Assert.IsNull(snap.ValidationError);
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("Slider", "IsSnapToTickEnabled", "sometimes"));

        Assert.AreEqual(AxamlPropertyEditorKind.Enum, placement.EditorKind);
        Assert.AreEqual("Outside", placement.Value);
        CollectionAssert.AreEqual(new[] { "None", "TopLeft", "BottomRight", "Outside" }, placement.Options.ToArray());
        Assert.IsNull(placement.ValidationError);
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("Slider", "TickPlacement", "Both"));

        AxamlPreviewNode defaults = service.Parse("<Slider />");
        Assert.AreEqual("0", AxamlPropertyValueValidator.Describe(defaults, "TickFrequency").Value);
        Assert.AreEqual("False", AxamlPropertyValueValidator.Describe(defaults, "IsSnapToTickEnabled").Value);
        Assert.AreEqual("None", AxamlPropertyValueValidator.Describe(defaults, "TickPlacement").Value);
        foreach (string propertyName in new[] { "TickFrequency", "IsSnapToTickEnabled", "TickPlacement" })
        {
            AxamlPreviewNode bound = service.Parse($"<Slider {propertyName}='{{Binding Setting}}' />");
            Assert.IsTrue(AxamlPropertyValueValidator.Describe(bound, propertyName).IsReadOnly, propertyName);
        }
    }

    [TestMethod]
    public void Parse_ExposesStackAndWrapPanelLayoutOptionsSafely()
    {
        AxamlPreviewService service = new();
        AxamlPreviewNode defaultStackPanel = service.Parse("<StackPanel />");
        Assert.AreEqual("0", AxamlPropertyValueValidator.Describe(defaultStackPanel, "Spacing").Value);
        Assert.IsNull(AxamlPropertyValueValidator.Describe(defaultStackPanel, "Spacing").ValidationError);
        Assert.IsNull(AxamlPropertyValueValidator.Validate("StackPanel", "Spacing", "2.5"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("StackPanel", "Spacing", "-1"));
        AxamlPreviewNode boundStackPanel = service.Parse("<StackPanel Spacing='{Binding Gap}' />");
        Assert.IsTrue(AxamlPropertyValueValidator.Describe(boundStackPanel, "Spacing").IsReadOnly);

        AxamlPreviewNode defaults = service.Parse("<WrapPanel />");
        Assert.AreEqual("0", AxamlPropertyValueValidator.Describe(defaults, "ItemSpacing").Value);
        Assert.AreEqual("0", AxamlPropertyValueValidator.Describe(defaults, "LineSpacing").Value);
        Assert.AreEqual(string.Empty, AxamlPropertyValueValidator.Describe(defaults, "ItemWidth").Value);
        Assert.AreEqual(string.Empty, AxamlPropertyValueValidator.Describe(defaults, "ItemHeight").Value);
        AxamlPropertyDescriptor defaultAlignment = AxamlPropertyValueValidator.Describe(defaults, "ItemsAlignment");
        Assert.AreEqual(AxamlPropertyEditorKind.Enum, defaultAlignment.EditorKind);
        Assert.AreEqual("Start", defaultAlignment.Value);
        CollectionAssert.AreEqual(new[] { "Start", "Center", "End" }, defaultAlignment.Options.ToArray());

        AxamlPreviewNode configured = service.Parse(
            "<WrapPanel ItemSpacing='4.5' LineSpacing='6' ItemWidth='40' ItemHeight='20' ItemsAlignment='Center' />");
        foreach ((string property, string expected) in new[]
        {
            ("ItemSpacing", "4.5"),
            ("LineSpacing", "6"),
            ("ItemWidth", "40"),
            ("ItemHeight", "20"),
            ("ItemsAlignment", "Center"),
        })
        {
            AxamlPropertyDescriptor descriptor = AxamlPropertyValueValidator.Describe(configured, property);
            Assert.IsFalse(descriptor.IsReadOnly, property);
            Assert.IsNull(descriptor.ValidationError, property);
            Assert.AreEqual(expected, descriptor.Value, property);
        }

        foreach (string property in new[] { "ItemSpacing", "LineSpacing", "ItemWidth", "ItemHeight" })
        {
            Assert.IsNull(AxamlPropertyValueValidator.Validate("WrapPanel", property, "0"), property);
            Assert.IsNotNull(AxamlPropertyValueValidator.Validate("WrapPanel", property, "-1"), property);
            Assert.IsNotNull(AxamlPropertyValueValidator.Validate("WrapPanel", property, "NaN"), property);
        }

        Assert.IsNull(AxamlPropertyValueValidator.Validate("WrapPanel", "ItemsAlignment", "End"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("WrapPanel", "ItemsAlignment", "SpaceBetween"));
        AxamlPreviewNode bound = service.Parse(
            "<WrapPanel ItemSpacing='{Binding ItemGap}' ItemWidth='{Binding Width}' ItemsAlignment='{Binding Alignment}' />");
        foreach (string property in new[] { "ItemSpacing", "ItemWidth", "ItemsAlignment" })
        {
            Assert.IsTrue(AxamlPropertyValueValidator.Describe(bound, property).IsReadOnly, property);
        }
    }

    [TestMethod]
    public void Parse_ExposesDockPanelAndUniformGridLayoutOptionsSafely()
    {
        AxamlPreviewService service = new();
        AxamlPreviewNode defaultDockPanel = service.Parse("<DockPanel />");
        Assert.AreEqual("0", AxamlPropertyValueValidator.Describe(defaultDockPanel, "HorizontalSpacing").Value);
        Assert.AreEqual("0", AxamlPropertyValueValidator.Describe(defaultDockPanel, "VerticalSpacing").Value);
        AxamlPropertyDescriptor defaultLastChildFill = AxamlPropertyValueValidator.Describe(defaultDockPanel, "LastChildFill");
        Assert.AreEqual(AxamlPropertyEditorKind.Boolean, defaultLastChildFill.EditorKind);
        Assert.AreEqual("True", defaultLastChildFill.Value);

        AxamlPreviewNode dockPanel = service.Parse(
            "<DockPanel HorizontalSpacing='4.5' VerticalSpacing='6' LastChildFill='False' />");
        foreach ((string property, string expected) in new[]
        {
            ("HorizontalSpacing", "4.5"),
            ("VerticalSpacing", "6"),
            ("LastChildFill", "False"),
        })
        {
            AxamlPropertyDescriptor descriptor = AxamlPropertyValueValidator.Describe(dockPanel, property);
            Assert.IsFalse(descriptor.IsReadOnly, property);
            Assert.IsNull(descriptor.ValidationError, property);
            Assert.AreEqual(expected, descriptor.Value, property);
        }

        foreach (string property in new[] { "HorizontalSpacing", "VerticalSpacing" })
        {
            Assert.IsNull(AxamlPropertyValueValidator.Validate("DockPanel", property, "0"), property);
            Assert.IsNotNull(AxamlPropertyValueValidator.Validate("DockPanel", property, "-1"), property);
            Assert.IsNotNull(AxamlPropertyValueValidator.Validate("DockPanel", property, "NaN"), property);
        }
        Assert.IsNull(AxamlPropertyValueValidator.Validate("DockPanel", "LastChildFill", "False"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("DockPanel", "LastChildFill", "sometimes"));

        AxamlPreviewNode defaultUniformGrid = service.Parse("<UniformGrid />");
        Assert.AreEqual("0", AxamlPropertyValueValidator.Describe(defaultUniformGrid, "RowSpacing").Value);
        Assert.AreEqual("0", AxamlPropertyValueValidator.Describe(defaultUniformGrid, "ColumnSpacing").Value);
        AxamlPreviewNode uniformGrid = service.Parse(
            "<UniformGrid RowSpacing='3' ColumnSpacing='5.5' />");
        foreach ((string property, string expected) in new[]
        {
            ("RowSpacing", "3"),
            ("ColumnSpacing", "5.5"),
        })
        {
            AxamlPropertyDescriptor descriptor = AxamlPropertyValueValidator.Describe(uniformGrid, property);
            Assert.IsFalse(descriptor.IsReadOnly, property);
            Assert.IsNull(descriptor.ValidationError, property);
            Assert.AreEqual(expected, descriptor.Value, property);
            Assert.IsNotNull(AxamlPropertyValueValidator.Validate("UniformGrid", property, "-1"), property);
            Assert.IsNotNull(AxamlPropertyValueValidator.Validate("UniformGrid", property, "Infinity"), property);
        }

        AxamlPreviewNode defaultGrid = service.Parse("<Grid />");
        Assert.AreEqual("0", AxamlPropertyValueValidator.Describe(defaultGrid, "RowSpacing").Value);
        Assert.AreEqual("0", AxamlPropertyValueValidator.Describe(defaultGrid, "ColumnSpacing").Value);
        AxamlPropertyDescriptor defaultShowGridLines = AxamlPropertyValueValidator.Describe(defaultGrid, "ShowGridLines");
        Assert.AreEqual(AxamlPropertyEditorKind.Boolean, defaultShowGridLines.EditorKind);
        Assert.AreEqual("False", defaultShowGridLines.Value);
        AxamlPreviewNode grid = service.Parse("<Grid RowSpacing='2.5' ColumnSpacing='4' ShowGridLines='True' />");
        Assert.AreEqual("True", AxamlPropertyValueValidator.Describe(grid, "ShowGridLines").Value);
        Assert.IsNull(AxamlPropertyValueValidator.Validate("Grid", "ShowGridLines", "True"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("Grid", "ShowGridLines", "sometimes"));
        foreach ((string property, string expected) in new[]
        {
            ("RowSpacing", "2.5"),
            ("ColumnSpacing", "4"),
        })
        {
            AxamlPropertyDescriptor descriptor = AxamlPropertyValueValidator.Describe(grid, property);
            Assert.IsFalse(descriptor.IsReadOnly, property);
            Assert.IsNull(descriptor.ValidationError, property);
            Assert.AreEqual(expected, descriptor.Value, property);
            Assert.IsNotNull(AxamlPropertyValueValidator.Validate("Grid", property, "-1"), property);
            Assert.IsNotNull(AxamlPropertyValueValidator.Validate("Grid", property, "Infinity"), property);
        }

        AxamlPreviewNode bound = service.Parse(
            "<StackPanel><DockPanel HorizontalSpacing='{Binding Gap}' LastChildFill='{Binding Fill}' /><UniformGrid RowSpacing='{Binding Gap}' ColumnSpacing='{Binding ColumnGap}' /><Grid RowSpacing='{Binding Gap}' ColumnSpacing='{Binding ColumnGap}' ShowGridLines='{Binding ShowLines}' /></StackPanel>");
        foreach ((AxamlPreviewNode node, string property) in new[]
        {
            (bound.Children[0], "HorizontalSpacing"),
            (bound.Children[0], "LastChildFill"),
            (bound.Children[1], "RowSpacing"),
            (bound.Children[1], "ColumnSpacing"),
            (bound.Children[2], "RowSpacing"),
            (bound.Children[2], "ColumnSpacing"),
            (bound.Children[2], "ShowGridLines"),
        })
        {
            Assert.IsTrue(AxamlPropertyValueValidator.Describe(node, property).IsReadOnly, $"{node.TypeName}.{property}");
        }
    }

    [TestMethod]
    public void Parse_ExposesRangeControlDisplayFlagsSafely()
    {
        AxamlPreviewService service = new();
        foreach ((string controlType, string propertyName) in new[]
        {
            ("ProgressBar", "ShowProgressText"),
            ("Slider", "IsDirectionReversed"),
        })
        {
            AxamlPreviewNode node = service.Parse($"<{controlType} />");
            AxamlPropertyDescriptor property = AxamlPropertyValueValidator.Describe(node, propertyName);
            Assert.AreEqual(AxamlPropertyEditorKind.Boolean, property.EditorKind, $"{controlType}.{propertyName}");
            Assert.AreEqual("False", property.Value, $"{controlType}.{propertyName}");
            Assert.IsFalse(property.IsReadOnly, $"{controlType}.{propertyName}");
            Assert.IsNull(property.ValidationError, $"{controlType}.{propertyName}");
            Assert.IsNull(AxamlPropertyValueValidator.Validate(controlType, propertyName, "True"));
            Assert.IsNotNull(AxamlPropertyValueValidator.Validate(controlType, propertyName, "sometimes"));

            AxamlPreviewNode bound = service.Parse($"<{controlType} {propertyName}='{{Binding Flag}}' />");
            Assert.IsTrue(
                AxamlPropertyValueValidator.Describe(bound, propertyName).IsReadOnly,
                $"{controlType}.{propertyName} binding");
        }
    }

    [TestMethod]
    public void Parse_ExposesTextBoxReadOnlySourceValueAsBoolean()
    {
        AxamlPreviewService service = new();
        AxamlPreviewNode textBox = service.Parse("<TextBox IsReadOnly='True' />");
        AxamlPropertyDescriptor isReadOnly = AxamlPropertyValueValidator.Describe(textBox, "IsReadOnly");

        Assert.IsTrue(service.CanEditProperty(textBox, "IsReadOnly"));
        Assert.AreEqual(AxamlPropertyEditorKind.Boolean, isReadOnly.EditorKind);
        Assert.AreEqual("True", isReadOnly.Value);
        Assert.IsNull(isReadOnly.ValidationError);
        Assert.IsNull(AxamlPropertyValueValidator.Validate("TextBox", "IsReadOnly", "False"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("TextBox", "IsReadOnly", "sometimes"));
        Assert.AreEqual(
            "False",
            AxamlPropertyValueValidator.Describe(service.Parse("<TextBox />"), "IsReadOnly").Value);

        AxamlPreviewNode bound = service.Parse("<TextBox IsReadOnly='{Binding Locked}' />");
        Assert.IsTrue(AxamlPropertyValueValidator.Describe(bound, "IsReadOnly").IsReadOnly);
    }

    [TestMethod]
    public void Parse_ExposesSafeNumericAndProgressFormats()
    {
        AxamlPreviewService service = new();
        AxamlPreviewNode numericUpDown = service.Parse("<NumericUpDown FormatString='N2' />");
        AxamlPropertyDescriptor numericFormat = AxamlPropertyValueValidator.Describe(numericUpDown, "FormatString");
        Assert.AreEqual(AxamlPropertyEditorKind.Text, numericFormat.EditorKind);
        Assert.AreEqual("N2", numericFormat.Value);
        Assert.IsNull(numericFormat.ValidationError);
        Assert.IsNull(AxamlPropertyValueValidator.Validate("NumericUpDown", "FormatString", "0.00"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("NumericUpDown", "FormatString", "Q"));
        Assert.IsTrue(AxamlPropertyValueValidator.Describe(
            service.Parse("<NumericUpDown FormatString='{Binding NumberFormat}' />"),
            "FormatString").IsReadOnly);

        AxamlPreviewNode progressBar = service.Parse("<ProgressBar ProgressTextFormat='{}{1:0.0}%' />");
        AxamlPropertyDescriptor progressFormat =
            AxamlPropertyValueValidator.Describe(progressBar, "ProgressTextFormat");
        Assert.AreEqual(AxamlPropertyEditorKind.Text, progressFormat.EditorKind);
        Assert.AreEqual("{1:0.0}%", progressFormat.Value);
        Assert.IsNull(progressFormat.ValidationError);
        Assert.IsNull(AxamlPropertyValueValidator.Validate("ProgressBar", "ProgressTextFormat", "{1:0}%"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("ProgressBar", "ProgressTextFormat", "{2:0}%"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("ProgressBar", "ProgressTextFormat", "{0,999999999}%"));
        Assert.AreEqual(
            "{1:0}%",
            AxamlPropertyValueValidator.Describe(service.Parse("<ProgressBar />"), "ProgressTextFormat").Value);
        Assert.IsTrue(AxamlPropertyValueValidator.Describe(
            service.Parse("<ProgressBar ProgressTextFormat='{Binding TextFormat}' />"),
            "ProgressTextFormat").IsReadOnly);

        string updated = service.UpdateAttribute(
            "<ProgressBar />",
            service.Parse("<ProgressBar />"),
            "ProgressTextFormat",
            "{1:0.0}%");
        StringAssert.Contains(updated, "ProgressTextFormat=\"{}{1:0.0}%\"");
        Assert.AreEqual(
            "{1:0.0}%",
            AxamlPropertyValueValidator.Describe(service.Parse(updated), "ProgressTextFormat").Value);
    }

    [TestMethod]
    public void Parse_ExposesHorizontalOrientationForRangeControls()
    {
        AxamlPreviewService service = new();
        AxamlPreviewNode slider = service.Parse("<Slider />");
        AxamlPreviewNode progressBar = service.Parse("<ProgressBar />");

        foreach (AxamlPreviewNode node in new[] { slider, progressBar })
        {
            AxamlPropertyDescriptor orientation = AxamlPropertyValueValidator.Describe(node, "Orientation");
            Assert.AreEqual(AxamlPropertyEditorKind.Enum, orientation.EditorKind);
            Assert.AreEqual("Horizontal", orientation.Value);
            CollectionAssert.AreEqual(new[] { "Horizontal", "Vertical" }, orientation.Options.ToArray());
            Assert.IsNull(orientation.ValidationError);
            Assert.IsNull(AxamlPropertyValueValidator.Validate(node.TypeName, "Orientation", "Vertical"));
            Assert.IsNotNull(AxamlPropertyValueValidator.Validate(node.TypeName, "Orientation", "Diagonal"));
        }

        Assert.IsFalse(service.CanEditProperty(service.Parse("<Slider Orientation='{Binding Direction}' />"), "Orientation"));
        Assert.IsTrue(AxamlPropertyValueValidator.Describe(
            service.Parse("<ProgressBar Orientation='{Binding Direction}' />"),
            "Orientation").IsReadOnly);
    }

    [TestMethod]
    public void Parse_ExposesScrollViewerScrollbarVisibilitySafely()
    {
        AxamlPreviewService service = new();
        AxamlPreviewNode scrollViewer = service.Parse("<ScrollViewer />");
        foreach (string property in new[] { "HorizontalScrollBarVisibility", "VerticalScrollBarVisibility" })
        {
            AxamlPropertyDescriptor descriptor = AxamlPropertyValueValidator.Describe(scrollViewer, property);
            Assert.AreEqual(AxamlPropertyEditorKind.Enum, descriptor.EditorKind, property);
            Assert.AreEqual("Auto", descriptor.Value, property);
            CollectionAssert.AreEqual(new[] { "Auto", "Disabled", "Hidden", "Visible" }, descriptor.Options.ToArray());
            Assert.IsNull(descriptor.ValidationError, property);
        }

        AxamlPreviewNode configured = service.Parse(
            "<ScrollViewer HorizontalScrollBarVisibility='Hidden' VerticalScrollBarVisibility='Disabled' />");
        Assert.AreEqual("Hidden", AxamlPropertyValueValidator.Describe(configured, "HorizontalScrollBarVisibility").Value);
        Assert.AreEqual("Disabled", AxamlPropertyValueValidator.Describe(configured, "VerticalScrollBarVisibility").Value);
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("ScrollViewer", "HorizontalScrollBarVisibility", "Always"));
        Assert.IsTrue(AxamlPropertyValueValidator.Describe(
            service.Parse("<ScrollViewer HorizontalScrollBarVisibility='{Binding Mode}' />"),
            "HorizontalScrollBarVisibility").IsReadOnly);
    }

    [TestMethod]
    public void Parse_RepresentsExpanderHeaderAndSingleContentChild()
    {
        const string markup =
            "<Expander xmlns='https://github.com/avaloniaui' Header='Details' IsExpanded='True'><TextBlock Text='Content' /></Expander>";
        AxamlPreviewService service = new();
        AxamlPreviewNode root = service.Parse(markup);

        Assert.AreEqual("Expander", root.TypeName);
        Assert.IsTrue(root.IsSupported);
        Assert.AreEqual("Details", root.Text);
        Assert.AreEqual("Content", root.Children.Single().Text);
        Assert.IsFalse(root.CanAppendChildren);
        Assert.ThrowsExactly<System.InvalidOperationException>(() =>
            service.InsertChildControl(markup, root, "Button"));
    }

    [TestMethod]
    public void Parse_RepresentsGroupBoxHeaderAndSingleContentChild()
    {
        const string markup =
            "<GroupBox xmlns='https://github.com/avaloniaui' Header='Options'><TextBlock Text='Content' /></GroupBox>";
        AxamlPreviewService service = new();
        AxamlPreviewNode root = service.Parse(markup);

        Assert.AreEqual("GroupBox", root.TypeName);
        Assert.IsTrue(root.IsSupported);
        Assert.AreEqual("Options", root.Text);
        Assert.AreEqual("Content", root.Children.Single().Text);
        Assert.IsFalse(root.CanAppendChildren);
        Assert.IsTrue(service.CanEditProperty(root, "Header"));
        Assert.ThrowsExactly<System.InvalidOperationException>(() =>
            service.InsertChildControl(markup, root, "Button"));
    }

    [TestMethod]
    public void Parse_RepresentsCustomControlsAsNonExecutablePlaceholders()
    {
        AxamlPreviewNode root = new AxamlPreviewService().Parse(
            "<UserControl xmlns='https://github.com/avaloniaui'><MyCustomControl /></UserControl>");

        Assert.AreEqual("MyCustomControl", root.TypeName);
        Assert.IsFalse(root.IsSupported);
    }

    [TestMethod]
    public void Parse_DoesNotTreatCustomNamespaceControlsAsAvaloniaCapabilities()
    {
        const string axaml = """
            <Window xmlns="https://github.com/avaloniaui"
                    xmlns:custom="urn:sample">
              <custom:StackPanel>
                <custom:Button Content="Not Avalonia" />
              </custom:StackPanel>
            </Window>
            """;

        AxamlPreviewNode root = new AxamlPreviewService().Parse(axaml);
        AxamlPreviewNode customPanel = root;

        Assert.AreEqual("StackPanel", customPanel.TypeName);
        Assert.IsFalse(customPanel.IsSupported);
        Assert.IsFalse(customPanel.Children.Single().IsSupported);
        Assert.IsFalse(new AxamlPreviewService().CanEditProperty(customPanel.Children.Single(), "Content"));
        AxamlDesignerSession session = new();
        session.Load(axaml);
        session.SelectNode(customPanel);
        Assert.ThrowsExactly<System.InvalidOperationException>(() => session.AddChildControl("Button"));
    }

    [TestMethod]
    public void Parse_RejectsDtdAndOversizedInput()
    {
        AxamlPreviewService service = new();

        Assert.ThrowsExactly<System.Xml.XmlException>(() =>
            service.Parse("<!DOCTYPE x [<!ENTITY e 'expanded'>]><TextBlock Text='&e;' />"));
        Assert.ThrowsExactly<InvalidDataException>(() =>
            service.Parse(new string(' ', 1_000_001)));
    }

    [TestMethod]
    public void ParseForPreview_RecoversValidControlsAroundMalformedElement()
    {
        const string axaml =
            "<?xml version='1.0'?><StackPanel xmlns='https://github.com/avaloniaui'><TextBlock Text='Keep this' /><Button Content=broken /><Label Content='Keep this too' /></StackPanel>";

        AxamlPreviewParseResult result = new AxamlPreviewService().ParseForPreview(axaml);

        Assert.IsNotNull(result.Root);
        Assert.IsTrue(result.Root.IsRecoveryRoot);
        Assert.AreEqual(3, result.Root.Children.Count);
        Assert.AreEqual("Keep this", result.Root.Children[0].Text);
        Assert.IsFalse(result.Root.Children[1].IsSupported);
        StringAssert.Contains(result.Root.Children[1].Diagnostic, "Could not preview <Button>");
        Assert.AreEqual("Keep this too", result.Root.Children[2].Text);
        Assert.IsTrue(result.Root.Children.Where(static node => node.TypeName != "Invalid AXAML")
            .All(static node => node.SourceStartOffset < 0));
        StringAssert.Contains(result.Diagnostic, "Partial preview");
    }

    [TestMethod]
    public void Parse_RepresentsViewboxWithSingleChildAndStretchCapability()
    {
        const string markup =
            "<Viewbox xmlns='https://github.com/avaloniaui' Stretch='UniformToFill' StretchDirection='UpOnly'><TextBlock Text='Scale me' /></Viewbox>";
        AxamlPreviewService service = new();
        AxamlPreviewNode root = service.Parse(markup);
        AxamlPropertyDescriptor defaultDirection = AxamlPropertyValueValidator.Describe(
            service.Parse("<Viewbox />"),
            "StretchDirection");

        Assert.AreEqual("Viewbox", root.TypeName);
        Assert.IsTrue(root.IsSupported);
        Assert.AreEqual("Scale me", root.Children.Single().Text);
        Assert.IsTrue(service.CanEditProperty(root, "Stretch"));
        Assert.AreEqual(AxamlPropertyEditorKind.Enum, defaultDirection.EditorKind);
        Assert.AreEqual("Both", defaultDirection.Value);
        Assert.AreEqual("UpOnly", AxamlPropertyValueValidator.Describe(root, "StretchDirection").Value);
        Assert.IsNull(AxamlPropertyValueValidator.Validate("Viewbox", "StretchDirection", "DownOnly"));
        Assert.IsNotNull(AxamlPropertyValueValidator.Validate("Viewbox", "StretchDirection", "Sideways"));
        Assert.IsTrue(AxamlPropertyValueValidator.Describe(
            service.Parse("<Viewbox StretchDirection='{Binding Direction}' />"),
            "StretchDirection").IsReadOnly);
        Assert.IsFalse(root.CanAppendChildren);
        Assert.ThrowsExactly<System.InvalidOperationException>(() =>
            service.InsertChildControl(markup, root, "Button"));
    }

    [TestMethod]
    public void Parse_RepresentsUniformGridPropertiesAndSupportsSafeAppend()
    {
        const string markup =
            "<UniformGrid xmlns='https://github.com/avaloniaui' Rows='2' Columns='3' FirstColumn='1'><TextBlock Text='First' /></UniformGrid>";
        AxamlPreviewService service = new();
        AxamlPreviewNode root = service.Parse(markup);

        Assert.AreEqual("UniformGrid", root.TypeName);
        Assert.IsTrue(root.IsSupported);
        Assert.AreEqual("2", root.Properties["Rows"]);
        Assert.AreEqual("3", root.Properties["Columns"]);
        Assert.AreEqual("1", root.Properties["FirstColumn"]);
        Assert.IsTrue(root.CanAppendChildren);
        Assert.IsTrue(service.CanEditProperty(root, "Rows"));

        string updated = service.InsertChildControl(markup, root, "Button");
        AxamlPreviewNode updatedRoot = service.Parse(updated);
        Assert.AreEqual(2, updatedRoot.Children.Count);
        Assert.AreEqual("First", updatedRoot.Children[0].Text);
        Assert.AreEqual("Button", updatedRoot.Children[1].TypeName);
    }

    [TestMethod]
    public void Parse_RejectsExcessiveControlNesting()
    {
        string axaml = "<StackPanel>";
        for (int index = 0; index < 66; index++)
        {
            axaml += "<StackPanel>";
        }

        axaml += "<TextBlock Text='Too deep' />";
        for (int index = 0; index < 67; index++)
        {
            axaml += "</StackPanel>";
        }

        Assert.ThrowsExactly<InvalidDataException>(() => new AxamlPreviewService().Parse(axaml));
    }

    [TestMethod]
    public void UpdateTextAttribute_ChangesOnlySelectedAttributeAndEscapesMarkup()
    {
        const string axaml = """
            <Window xmlns="https://github.com/avaloniaui">
              <TextBlock Text='Before' Margin="4" />
            </Window>
            """;
        AxamlPreviewService service = new();
        AxamlPreviewNode root = service.Parse(axaml);

        string updated = service.UpdateTextAttribute(axaml, root, "After & \"quoted\"");

        StringAssert.Contains(updated, """Text='After &amp; &quot;quoted&quot;' Margin="4" """);
        Assert.AreEqual("After & \"quoted\"", service.Parse(updated).Text);
        Assert.IsTrue(service.CanEditText(root));
    }

    [TestMethod]
    public void UpdateTextAttribute_AddsMissingAttributeWithoutReformattingOtherMarkup()
    {
        const string axaml = "<StackPanel>\r\n  <TextBlock />\r\n</StackPanel>";
        AxamlPreviewService service = new();
        AxamlPreviewNode root = service.Parse(axaml);

        string updated = service.UpdateTextAttribute(axaml, root.Children[0], "New line\ntext");

        Assert.AreEqual("<StackPanel>\r\n  <TextBlock Text=\"New line&#xA;text\"/>\r\n</StackPanel>", updated);
        Assert.AreEqual("New line\ntext", service.Parse(updated).Children[0].Text);
    }

    [TestMethod]
    public void CanEditText_RejectsTextPropertyElementsAndStaleSourceNodes()
    {
        AxamlPreviewService service = new();
        AxamlPreviewNode propertyElementNode = service.Parse(
            "<TextBlock xmlns='https://github.com/avaloniaui'><TextBlock.Text>Inline text</TextBlock.Text></TextBlock>");
        AxamlPreviewNode inlineTextNode = service.Parse("<TextBlock>Inline text</TextBlock>");
        AxamlPreviewNode staleNode = service.Parse("<TextBlock Text='Old' />");
        AxamlPreviewNode labelNode = service.Parse("<Label Content='Caption' />");

        Assert.IsFalse(service.CanEditText(propertyElementNode));
        Assert.IsFalse(service.CanEditText(inlineTextNode));
        Assert.ThrowsExactly<InvalidDataException>(() =>
            service.UpdateTextAttribute("<TextBlock Text='Other' />", staleNode, "Wrong target"));
        Assert.AreEqual("Caption", labelNode.Text);
        Assert.AreEqual("Changed", service.Parse(service.UpdateTextAttribute(
            "<Label Content='Caption' />",
            labelNode,
            "Changed")).Text);
    }

    [TestMethod]
    public void CanEditProperty_RejectsBindingsAndResources()
    {
        AxamlPreviewService service = new();
        AxamlPreviewNode node = service.Parse(
            "<StackPanel xmlns='https://github.com/avaloniaui' Width='{Binding Width}' Height='{StaticResource PanelHeight}' />");

        Assert.IsFalse(service.CanEditProperty(node, "Width"));
        Assert.IsFalse(service.CanEditProperty(node, "Height"));
        Assert.ThrowsExactly<System.InvalidOperationException>(() =>
            service.UpdateAttribute("<StackPanel Width='{Binding Width}' />", node, "Width", "200"));
    }

    [TestMethod]
    public void UpdateAttribute_EmptyValueRemovesTheAttributeInsteadOfWritingAnInvalidLiteral()
    {
        const string markup = "<StackPanel   Width='120' Height=\"80\" />";
        AxamlPreviewService service = new();
        AxamlPreviewNode node = service.Parse(markup);

        string updated = service.UpdateAttribute(markup, node, "Width", string.Empty);

        Assert.AreEqual("<StackPanel    Height=\"80\" />", updated);
        AxamlPreviewNode withoutWidth = service.Parse(updated);
        Assert.AreEqual(updated, service.UpdateAttribute(updated, withoutWidth, "Width", string.Empty));
    }

    [TestMethod]
    public void UpdateBinding_OnlyWritesSimpleMemberPathsToCataloguedBindableProperties()
    {
        AxamlPreviewService service = new();
        const string markup = "<Button xmlns='https://github.com/avaloniaui' Command='{Binding OldCommand}' />";
        AxamlPreviewNode node = service.Parse(markup);

        string updated = service.UpdateBinding(markup, node, "Command", "SaveCommand");

        StringAssert.Contains(updated, "Command='{Binding SaveCommand}'");
        Assert.ThrowsExactly<InvalidOperationException>(() =>
            service.UpdateBinding(markup, node, "Command", "SaveCommand.Execute"));
        Assert.ThrowsExactly<InvalidOperationException>(() =>
            service.UpdateBinding(markup, node, "Width", "PanelWidth"));
        Assert.ThrowsExactly<InvalidOperationException>(() =>
            service.UpdateBinding(markup, node, "Command", "Bad Command"));
    }

    [TestMethod]
    public void InsertChildControl_AppendsToSupportedPanelsAndMatchesNestedClosingTags()
    {
        const string markup = "<StackPanel>\r\n  <StackPanel />\r\n</StackPanel>";
        AxamlPreviewService service = new();
        AxamlPreviewNode root = service.Parse(markup);

        string updated = service.InsertChildControl(markup, root, "Button");

        Assert.AreEqual(
            "<StackPanel>\r\n  <StackPanel />\r\n  <Button />\r\n</StackPanel>",
            updated);
        Assert.AreEqual(2, service.Parse(updated).Children.Count);
    }

    [TestMethod]
    public void InsertChildControl_RejectsStaleAttachedContentChildren()
    {
        const string original = "<Button><Button.ToolTip><ToolTip /></Button.ToolTip></Button>";
        const string changed = "<Button><Button.ToolTip><ToolTip><TextBlock /></ToolTip></Button.ToolTip></Button>";
        AxamlPreviewService service = new();
        AxamlPreviewNode toolTip = service.Parse(original).Children.Single();

        Assert.ThrowsExactly<InvalidOperationException>(
            () => service.InsertChildControl(changed, toolTip, "TextBlock"));
    }

    [TestMethod]
    public void InsertChildControl_AppendsToInlinePanelAndPreservesExistingMarkup()
    {
        const string markup = "<av:DockPanel xmlns:av='https://github.com/avaloniaui' LastChildFill='False'><av:TextBlock Text='keep' /></av:DockPanel>";
        AxamlPreviewService service = new();
        AxamlPreviewNode root = service.Parse(markup);

        string updated = service.InsertChildControl(markup, root, "Button", AxamlDockPanelPlacement.Bottom);

        StringAssert.Contains(
            updated,
            "<av:TextBlock Text='keep' />\n  <av:Button av:DockPanel.Dock=\"Bottom\" />\n</av:DockPanel>");
        AxamlPreviewNode updatedRoot = service.Parse(updated);
        Assert.AreEqual("keep", updatedRoot.Children[0].Text);
        Assert.AreEqual("Button", updatedRoot.Children[1].TypeName);
        Assert.AreEqual("Bottom", updatedRoot.Children[1].Properties["DockPanel.Dock"]);
    }

    [TestMethod]
    public void InsertChildControl_InsertsDockedChildrenBeforeTheClientChild()
    {
        AxamlPreviewService service = new();
        const string empty = "<DockPanel />";
        AxamlPreviewNode emptyPanel = service.Parse(empty);
        string withClient = service.InsertChildControl(
            empty,
            emptyPanel,
            "TextBlock",
            AxamlDockPanelPlacement.Client);
        AxamlPreviewNode clientPanel = service.Parse(withClient);

        string withTop = service.InsertChildControl(
            withClient,
            clientPanel,
            "Button",
            AxamlDockPanelPlacement.Top);
        AxamlPreviewNode topPanel = service.Parse(withTop);
        string withBottom = service.InsertChildControl(
            withTop,
            topPanel,
            "Separator",
            AxamlDockPanelPlacement.Bottom);

        StringAssert.Contains(
            withBottom,
            "<Button DockPanel.Dock=\"Top\" />\n  <Separator DockPanel.Dock=\"Bottom\" />\n  <TextBlock />");
        AxamlPreviewNode result = service.Parse(withBottom);
        Assert.AreEqual("Button", result.Children[0].TypeName);
        Assert.AreEqual("Top", result.Children[0].Properties["DockPanel.Dock"]);
        Assert.AreEqual("Bottom", result.Children[1].Properties["DockPanel.Dock"]);
        Assert.AreEqual("TextBlock", result.Children[2].TypeName);
        Assert.IsTrue(result.CanAppendChildren);
    }

    [TestMethod]
    public void InsertChildControl_RejectsInvalidClientPlacementRequests()
    {
        AxamlPreviewService service = new();
        const string empty = "<DockPanel />";
        AxamlPreviewNode emptyPanel = service.Parse(empty);
        Assert.ThrowsExactly<System.InvalidOperationException>(() =>
            service.InsertChildControl(empty, emptyPanel, "Button", AxamlDockPanelPlacement.Top));

        const string withClient = "<DockPanel><TextBlock /></DockPanel>";
        AxamlPreviewNode clientPanel = service.Parse(withClient);
        Assert.ThrowsExactly<System.InvalidOperationException>(() =>
            service.InsertChildControl(withClient, clientPanel, "Button", AxamlDockPanelPlacement.Client));
        Assert.ThrowsExactly<System.InvalidOperationException>(() =>
            service.InsertChildControl(withClient, clientPanel, "Button"));

        const string noFill = "<DockPanel LastChildFill='False'><TextBlock /></DockPanel>";
        AxamlPreviewNode noFillPanel = service.Parse(noFill);
        Assert.ThrowsExactly<System.InvalidOperationException>(() =>
            service.InsertChildControl(noFill, noFillPanel, "Button", AxamlDockPanelPlacement.Client));
        StringAssert.Contains(
            service.InsertChildControl(noFill, noFillPanel, "Button", AxamlDockPanelPlacement.Bottom),
            "<Button DockPanel.Dock=\"Bottom\" />");
    }

    [TestMethod]
    public void InsertChildControl_RejectsAmbiguousExistingContent()
    {
        AxamlPreviewService service = new();
        AxamlPreviewNode grid = service.Parse("<Grid><TextBlock /></Grid>");
        const string propertyMarkup =
            "<StackPanel><StackPanel.Resources><ResourceDictionary /></StackPanel.Resources></StackPanel>";
        AxamlPreviewNode panelWithPropertyElement = service.Parse(propertyMarkup);
        const string unknownMarkup = "<StackPanel><legacy /></StackPanel>";
        AxamlPreviewNode panelWithUnknownContent = service.Parse(unknownMarkup);
        const string inlineTextMarkup = "<StackPanel>unmodeled text<TextBlock /></StackPanel>";
        AxamlPreviewNode panelWithInlineText = service.Parse(inlineTextMarkup);
        const string customMarkup =
            "<StackPanel xmlns='https://github.com/avaloniaui' xmlns:custom='urn:sample'><custom:Button /></StackPanel>";
        AxamlPreviewNode panelWithCustomControl = service.Parse(customMarkup);

        Assert.ThrowsExactly<System.InvalidOperationException>(() =>
            service.InsertChildControl("<Grid><TextBlock /></Grid>", grid, "Button"));
        Assert.ThrowsExactly<System.InvalidOperationException>(() =>
            service.InsertChildControl(propertyMarkup, panelWithPropertyElement, "Button"));
        Assert.ThrowsExactly<System.InvalidOperationException>(() =>
            service.InsertChildControl(unknownMarkup, panelWithUnknownContent, "Button"));
        Assert.IsFalse(panelWithInlineText.CanAppendChildren);
        System.InvalidOperationException inlineTextException = Assert.ThrowsExactly<System.InvalidOperationException>(() =>
            service.InsertChildControl(inlineTextMarkup, panelWithInlineText, "Button"));
        StringAssert.Contains(inlineTextException.Message, "Correct invalid inline text");
        Assert.IsFalse(panelWithCustomControl.CanAppendChildren);
        Assert.ThrowsExactly<System.InvalidOperationException>(() =>
            service.InsertChildControl(customMarkup, panelWithCustomControl, "Button"));
    }

    [TestMethod]
    public void InsertChildControl_IgnoresCommentTextWhenFindingEmptyContainerClosingTag()
    {
        const string markup = "<StackPanel><!-- fake closing tag: </StackPanel> --></StackPanel>";
        AxamlPreviewService service = new();
        AxamlPreviewNode root = service.Parse(markup);

        string updated = service.InsertChildControl(markup, root, "Button");

        StringAssert.Contains(updated, "<!-- fake closing tag: </StackPanel> -->");
        Assert.AreEqual("Button", service.Parse(updated).Children.Single().TypeName);
    }

    [TestMethod]
    public void InsertChildControl_AddsGridChildToNextUnoccupiedExplicitCell()
    {
        const string markup = """
            <Grid RowDefinitions="Auto,*" ColumnDefinitions="*,*">
              <TextBlock Grid.Row="0" Grid.Column="0" />
              <TextBox Grid.Row="0" Grid.Column="1" />
            </Grid>
            """;
        AxamlPreviewService service = new();
        AxamlPreviewNode grid = service.Parse(markup);
        Assert.IsTrue(grid.CanAddGridChild);

        string updated = service.InsertChildControl(markup, grid, "Button");

        StringAssert.Contains(updated, "<Button Grid.Row=\"1\" Grid.Column=\"0\" />");
        AxamlPreviewNode result = service.Parse(updated);
        Assert.AreEqual(3, result.Children.Count);
        Assert.AreEqual("1", result.Children[2].Properties["Grid.Row"]);
        Assert.AreEqual("0", result.Children[2].Properties["Grid.Column"]);
    }

    [TestMethod]
    public void InsertChildControl_AddsGridChildWhenDefinitionsUsePropertyElements()
    {
        const string markup = """
            <Grid xmlns="https://github.com/avaloniaui">
              <Grid.RowDefinitions>
                <RowDefinition Height="Auto" />
                <RowDefinition Height="*" />
              </Grid.RowDefinitions>
              <Grid.ColumnDefinitions>
                <ColumnDefinition Width="*" />
                <ColumnDefinition Width="*" />
              </Grid.ColumnDefinitions>
              <TextBlock Grid.Row="0" Grid.Column="0" />
            </Grid>
            """;
        AxamlPreviewService service = new();
        AxamlPreviewNode grid = service.Parse(markup);
        Assert.IsTrue(grid.CanAddGridChild);

        string updated = service.InsertChildControl(markup, grid, "Button");

        StringAssert.Contains(updated, "<Button Grid.Row=\"0\" Grid.Column=\"1\" />");
        AxamlPreviewNode result = service.Parse(updated);
        Assert.AreEqual(2, result.RowDefinitions.Count);
        Assert.AreEqual(2, result.ColumnDefinitions.Count);
        Assert.AreEqual(2, result.Children.Count);
    }

    [TestMethod]
    public void InsertChildControl_AccountsForExistingGridSpans()
    {
        const string markup = """
            <Grid RowDefinitions="*,*" ColumnDefinitions="*,*">
              <TextBlock Grid.Row="0" Grid.Column="0" Grid.ColumnSpan="2" />
            </Grid>
            """;
        AxamlPreviewService service = new();
        AxamlPreviewNode grid = service.Parse(markup);

        Assert.IsTrue(grid.CanAddGridChild);
        string updated = service.InsertChildControl(markup, grid, "Button");

        StringAssert.Contains(updated, "<Button Grid.Row=\"1\" Grid.Column=\"0\" />");
        Assert.AreEqual(2, service.Parse(updated).Children.Count);
    }

    [TestMethod]
    public void InsertChildControl_RejectsStaleGridChildrenAndCollapsedContentRoots()
    {
        AxamlPreviewService service = new();
        const string original = "<Grid ColumnDefinitions='*,*'><Button /></Grid>";
        AxamlPreviewNode grid = service.Parse(original);
        const string changedChild = "<Grid ColumnDefinitions='*,*'><Button Grid.Column='1' /></Grid>";
        Assert.ThrowsExactly<System.IO.InvalidDataException>(() => service.InsertChildControl(changedChild, grid, "TextBlock"));

        const string empty = "<Grid ColumnDefinitions='*,*'></Grid>";
        AxamlPreviewNode emptyGrid = service.Parse(empty);
        const string changedEmpty = "<Grid ColumnDefinitions='*,*'><Button /></Grid>";
        Assert.ThrowsExactly<System.InvalidOperationException>(() => service.InsertChildControl(changedEmpty, emptyGrid, "TextBlock"));

        const string collapsed = "<Grid ColumnDefinitions='*,*'><UserControl Grid.Column='1'><Button /></UserControl></Grid>";
        AxamlPreviewNode collapsedGrid = service.Parse(collapsed);
        Assert.IsFalse(collapsedGrid.HasSafeGridLayout);
        Assert.IsFalse(collapsedGrid.CanAddGridChild);
        Assert.ThrowsExactly<System.InvalidOperationException>(() => service.InsertChildControl(collapsed, collapsedGrid, "TextBlock"));
    }

    [TestMethod]
    public void InsertChildControl_RejectsStaleSingleChildContainerContent()
    {
        AxamlPreviewService service = new();
        const string original = "<Border></Border>";
        AxamlPreviewNode border = service.Parse(original);

        Assert.ThrowsExactly<System.InvalidOperationException>(() =>
            service.InsertChildControl("<Border><Button /></Border>", border, "TextBlock"));
    }

    [TestMethod]
    public void InsertChildControl_RejectsAmbiguousOrFullGridPlacements()
    {
        AxamlPreviewService service = new();
        const string duplicateCell = "<Grid><TextBlock /><Button Grid.Row='0' Grid.Column='0' /></Grid>";
        const string spanningChild = "<Grid RowDefinitions='*,*'><TextBlock Grid.RowSpan='2' /></Grid>";
        const string outOfRangeChild = "<Grid RowDefinitions='Auto'><TextBlock Grid.Row='1' /></Grid>";
        const string duplicateDefinitionSources =
            "<Grid RowDefinitions='*,*'><Grid.RowDefinitions><RowDefinition /><RowDefinition /></Grid.RowDefinitions></Grid>";
        const string malformedDefinitionProperty =
            "<Grid><Grid.RowDefinitions><ColumnDefinition /></Grid.RowDefinitions></Grid>";
        const string emptyDefinitionSlot = "<Grid RowDefinitions='Auto,,*' />";
        const string dynamicDefinitionList = "<Grid RowDefinitions='{Binding Rows}' />";
        const string invalidGridLength = "<Grid ColumnDefinitions='Auto,unknown' />";
        string oversizedGrid =
            $"<Grid RowDefinitions='{string.Join(',', Enumerable.Repeat("Auto", 317))}' " +
            $"ColumnDefinitions='{string.Join(',', Enumerable.Repeat("Auto", 317))}' />";
        const string overlappingSpan = """
            <Grid RowDefinitions="*,*" ColumnDefinitions="*,*">
              <TextBlock Grid.Row="0" Grid.Column="0" Grid.ColumnSpan="2" />
              <Button Grid.Row="0" Grid.Column="1" />
            </Grid>
            """;
        const string dynamicPropertyDefinition = """
            <Grid>
              <Grid.RowDefinitions><RowDefinition Height="{Binding Height}" /></Grid.RowDefinitions>
            </Grid>
            """;
        const string negativePropertyDefinition = """
            <Grid>
              <Grid.ColumnDefinitions><ColumnDefinition Width="-1" /></Grid.ColumnDefinitions>
            </Grid>
            """;
        const string namespacedGridPlacement =
            "<Grid xmlns:custom='urn:custom'><TextBlock custom:Grid.Row='0' /></Grid>";
        const string namespacedGridDimensions =
            "<Grid xmlns:custom='urn:custom' custom:RowDefinitions='Auto,Auto' />";
        const string namespacedDefinitionLength = """
            <Grid xmlns:custom="urn:custom">
              <Grid.RowDefinitions><RowDefinition custom:Height="*" /></Grid.RowDefinitions>
            </Grid>
            """;

        foreach (string markup in new[]
        {
            duplicateCell,
            overlappingSpan,
            spanningChild,
            outOfRangeChild,
            duplicateDefinitionSources,
            malformedDefinitionProperty,
            emptyDefinitionSlot,
            dynamicDefinitionList,
            invalidGridLength,
            oversizedGrid,
            dynamicPropertyDefinition,
            negativePropertyDefinition,
            namespacedGridPlacement,
            namespacedGridDimensions,
            namespacedDefinitionLength,
        })
        {
            AxamlPreviewNode grid = service.Parse(markup);
            Assert.IsFalse(grid.CanAddGridChild, markup);
            Assert.ThrowsExactly<System.InvalidOperationException>(() =>
                service.InsertChildControl(markup, grid, "Button"));
        }

        const string fullGrid = "<Grid><TextBlock /></Grid>";
        AxamlPreviewNode occupiedGrid = service.Parse(fullGrid);
        Assert.IsFalse(occupiedGrid.CanAddGridChild);
        Assert.ThrowsExactly<System.InvalidOperationException>(() =>
            service.InsertChildControl(fullGrid, occupiedGrid, "Button"));
    }
}
