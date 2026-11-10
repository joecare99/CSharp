using AA98_AvlnCodeStudio.AxamlDesigner.UI.ViewModels;
using AA98_AvlnCodeStudio.Model.Workspaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Property.Editor;
using Property.Editor.Avalonia;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.Tests.Engineering;

[TestClass]
public sealed class AxamlDesignerViewModelTests
{
    [TestMethod]
    public void DesignerViewModel_ApplyUndoRedo_PublishesUpdatedMarkup()
    {
        AxamlDesignerViewModel viewModel = new();
        string? publishedMarkup = null;
        viewModel.MarkupChanged += (_, args) => publishedMarkup = args.Markup;
        viewModel.LoadMarkup(
            "<UserControl xmlns='https://github.com/avaloniaui'><StackPanel><TextBlock Text='Before' /></StackPanel></UserControl>");
        viewModel.SelectedNode = viewModel.PreviewRoot?.Children[0];
        viewModel.SelectedProperty = "Text";
        viewModel.SelectedValue = "After & updated";

        viewModel.ApplySelectedValueCommand.Execute(null);

        Assert.IsNotNull(publishedMarkup);
        StringAssert.Contains(publishedMarkup, "Text='After &amp; updated'");
        Assert.IsTrue(viewModel.CanUndo);
        viewModel.UndoCommand.Execute(null);
        StringAssert.Contains(publishedMarkup, "Text='Before'");
        viewModel.RedoCommand.Execute(null);
        StringAssert.Contains(publishedMarkup, "Text='After &amp; updated'");
        Assert.IsFalse(string.IsNullOrWhiteSpace(viewModel.StatusText));
    }

    [TestMethod]
    public async Task NumericUpDownValueAndToggleSwitchContent_AreEditableAndUndoable()
    {
        const string markup = "<StackPanel xmlns='https://github.com/avaloniaui'>"
            + "<NumericUpDown Value='12.5' /><ToggleSwitch Content='Alerts' /></StackPanel>";
        AxamlDesignerViewModel viewModel = new();
        viewModel.LoadMarkup(markup);

        viewModel.SelectedNode = viewModel.PreviewRoot!.Children[0];
        var numericValue = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "Value");
        Assert.IsTrue(numericValue.TrySetValue("14.75"));
        await viewModel.PropertyProvider.ApplyAsync();
        Assert.AreEqual("14.75", viewModel.PreviewRoot!.Children[0].Properties["Value"]);

        viewModel.SelectedNode = viewModel.PreviewRoot.Children[1];
        var toggleContent = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "Content");
        Assert.IsTrue(toggleContent.TrySetValue("Notifications"));
        await viewModel.PropertyProvider.ApplyAsync();
        Assert.AreEqual("Notifications", viewModel.PreviewRoot.Children[1].Text);

        viewModel.UndoCommand.Execute(null);
        Assert.AreEqual("Alerts", viewModel.PreviewRoot!.Children[1].Text);
        viewModel.UndoCommand.Execute(null);
        Assert.AreEqual("12.5", viewModel.PreviewRoot!.Children[0].Properties["Value"]);
        viewModel.RedoCommand.Execute(null);
        Assert.AreEqual("14.75", viewModel.PreviewRoot!.Children[0].Properties["Value"]);
        viewModel.RedoCommand.Execute(null);
        Assert.AreEqual("Notifications", viewModel.PreviewRoot!.Children[1].Text);
    }

    [TestMethod]
    public async Task NumericUpDownIncrement_RequiresPositiveValueAndSupportsUndoRedo()
    {
        AxamlDesignerViewModel viewModel = new();
        viewModel.LoadMarkup("<NumericUpDown xmlns='https://github.com/avaloniaui' Increment='0.5' />");
        viewModel.SelectedNode = viewModel.PreviewRoot;
        var increment = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "Increment");

        Assert.IsFalse(increment.TrySetValue("0"));
        Assert.IsTrue(viewModel.HasInvalidPropertyDraft);
        Assert.IsTrue(increment.TrySetValue("2.5"));
        await viewModel.PropertyProvider.ApplyAsync();
        Assert.AreEqual("2.5", viewModel.PreviewRoot!.Properties["Increment"]);
        viewModel.UndoCommand.Execute(null);
        Assert.AreEqual("0.5", viewModel.PreviewRoot!.Properties["Increment"]);
        viewModel.RedoCommand.Execute(null);
        Assert.AreEqual("2.5", viewModel.PreviewRoot!.Properties["Increment"]);
    }

    [TestMethod]
    public async Task NumericUpDownOptions_AreEditableAndUndoable()
    {
        AxamlDesignerViewModel viewModel = new();
        viewModel.LoadMarkup("<NumericUpDown xmlns='https://github.com/avaloniaui' />");
        viewModel.SelectedNode = viewModel.PreviewRoot;

        foreach ((string propertyName, string value, string invalidValue) in new[]
        {
            ("AllowSpin", "False", "enabled"),
            ("ShowButtonSpinner", "False", "enabled"),
            ("ClipValueToMinMax", "True", "enabled"),
            ("ButtonSpinnerLocation", "Left", "Top"),
            ("TextAlignment", "Center", "Baseline"),
            ("ParsingNumberStyle", "Integer", "AllowHexSpecifier"),
        })
        {
            var property = viewModel.PropertyProvider.Properties.Single(row => row.Name == propertyName);
            Assert.IsFalse(property.TrySetValue(invalidValue), propertyName);
            Assert.IsTrue(property.TrySetValue(value), propertyName);
            await viewModel.PropertyProvider.ApplyAsync();
        }

        Assert.AreEqual("False", viewModel.PreviewRoot!.Properties["AllowSpin"]);
        Assert.AreEqual("False", viewModel.PreviewRoot.Properties["ShowButtonSpinner"]);
        Assert.AreEqual("True", viewModel.PreviewRoot.Properties["ClipValueToMinMax"]);
        Assert.AreEqual("Left", viewModel.PreviewRoot.Properties["ButtonSpinnerLocation"]);
        Assert.AreEqual("Center", viewModel.PreviewRoot.Properties["TextAlignment"]);
        Assert.AreEqual("Integer", viewModel.PreviewRoot.Properties["ParsingNumberStyle"]);
        for (int index = 0; index < 6; index++)
        {
            viewModel.UndoCommand.Execute(null);
        }

        Assert.IsFalse(viewModel.PreviewRoot!.Properties.ContainsKey("AllowSpin"));
        Assert.IsFalse(viewModel.PreviewRoot.Properties.ContainsKey("ShowButtonSpinner"));
        Assert.IsFalse(viewModel.PreviewRoot.Properties.ContainsKey("ClipValueToMinMax"));
        Assert.IsFalse(viewModel.PreviewRoot.Properties.ContainsKey("ButtonSpinnerLocation"));
        Assert.IsFalse(viewModel.PreviewRoot.Properties.ContainsKey("TextAlignment"));
        Assert.IsFalse(viewModel.PreviewRoot.Properties.ContainsKey("ParsingNumberStyle"));
        for (int index = 0; index < 6; index++)
        {
            viewModel.RedoCommand.Execute(null);
        }

        Assert.AreEqual("False", viewModel.PreviewRoot!.Properties["AllowSpin"]);
        Assert.AreEqual("False", viewModel.PreviewRoot.Properties["ShowButtonSpinner"]);
        Assert.AreEqual("True", viewModel.PreviewRoot.Properties["ClipValueToMinMax"]);
        Assert.AreEqual("Left", viewModel.PreviewRoot.Properties["ButtonSpinnerLocation"]);
        Assert.AreEqual("Center", viewModel.PreviewRoot.Properties["TextAlignment"]);
        Assert.AreEqual("Integer", viewModel.PreviewRoot.Properties["ParsingNumberStyle"]);
    }

    [TestMethod]
    public async Task NumericUpDownMargin_IsEditableAndUndoable()
    {
        AxamlDesignerViewModel viewModel = new();
        viewModel.LoadMarkup("<NumericUpDown xmlns='https://github.com/avaloniaui' />");
        viewModel.SelectedNode = viewModel.PreviewRoot;
        var margin = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "Margin");

        Assert.IsTrue(margin.TrySetValue("3,4"));
        await viewModel.PropertyProvider.ApplyAsync();
        Assert.AreEqual("3,4", viewModel.PreviewRoot!.Properties["Margin"]);
        viewModel.UndoCommand.Execute(null);
        Assert.IsFalse(viewModel.PreviewRoot!.Properties.ContainsKey("Margin"));
        viewModel.RedoCommand.Execute(null);
        Assert.AreEqual("3,4", viewModel.PreviewRoot!.Properties["Margin"]);
    }

    [TestMethod]
    public async Task NumericUpDownAlignment_IsEditableAndUndoable()
    {
        AxamlDesignerViewModel viewModel = new();
        viewModel.LoadMarkup("<NumericUpDown xmlns='https://github.com/avaloniaui' />");
        viewModel.SelectedNode = viewModel.PreviewRoot;
        var horizontal = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "HorizontalAlignment");
        var vertical = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "VerticalAlignment");

        Assert.IsTrue(horizontal.TrySetValue("Right"));
        await viewModel.PropertyProvider.ApplyAsync();
        Assert.IsTrue(vertical.TrySetValue("Center"));
        await viewModel.PropertyProvider.ApplyAsync();
        Assert.AreEqual("Right", viewModel.PreviewRoot!.Properties["HorizontalAlignment"]);
        Assert.AreEqual("Center", viewModel.PreviewRoot.Properties["VerticalAlignment"]);

        viewModel.UndoCommand.Execute(null);
        Assert.IsFalse(viewModel.PreviewRoot!.Properties.ContainsKey("VerticalAlignment"));
        viewModel.UndoCommand.Execute(null);
        Assert.IsFalse(viewModel.PreviewRoot.Properties.ContainsKey("HorizontalAlignment"));
        viewModel.RedoCommand.Execute(null);
        viewModel.RedoCommand.Execute(null);
        Assert.AreEqual("Right", viewModel.PreviewRoot!.Properties["HorizontalAlignment"]);
        Assert.AreEqual("Center", viewModel.PreviewRoot.Properties["VerticalAlignment"]);
    }

    [TestMethod]
    public async Task SliderTickSettings_AreEditableAndUndoable()
    {
        AxamlDesignerViewModel viewModel = new();
        viewModel.LoadMarkup("<Slider xmlns='https://github.com/avaloniaui' />");
        viewModel.SelectedNode = viewModel.PreviewRoot;

        var frequency = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "TickFrequency");
        var snap = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "IsSnapToTickEnabled");
        var placement = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "TickPlacement");

        Assert.IsTrue(frequency.TrySetValue("5"));
        await viewModel.PropertyProvider.ApplyAsync();
        Assert.IsTrue(snap.TrySetValue(true));
        await viewModel.PropertyProvider.ApplyAsync();
        Assert.IsTrue(placement.TrySetValue("Outside"));
        await viewModel.PropertyProvider.ApplyAsync();

        Assert.AreEqual("5", viewModel.PreviewRoot!.Properties["TickFrequency"]);
        Assert.AreEqual("True", viewModel.PreviewRoot.Properties["IsSnapToTickEnabled"]);
        Assert.AreEqual("Outside", viewModel.PreviewRoot.Properties["TickPlacement"]);
        viewModel.UndoCommand.Execute(null);
        Assert.IsFalse(viewModel.PreviewRoot!.Properties.ContainsKey("TickPlacement"));
        viewModel.UndoCommand.Execute(null);
        Assert.IsFalse(viewModel.PreviewRoot!.Properties.ContainsKey("IsSnapToTickEnabled"));
        viewModel.UndoCommand.Execute(null);
        Assert.IsFalse(viewModel.PreviewRoot!.Properties.ContainsKey("TickFrequency"));
        viewModel.RedoCommand.Execute(null);
        viewModel.RedoCommand.Execute(null);
        viewModel.RedoCommand.Execute(null);
        Assert.AreEqual("Outside", viewModel.PreviewRoot!.Properties["TickPlacement"]);
    }

    [TestMethod]
    public async Task RangeControlDisplayFlags_AreEditableAndUndoable()
    {
        AxamlDesignerViewModel viewModel = new();
        viewModel.LoadMarkup(
            "<StackPanel xmlns='https://github.com/avaloniaui'><ProgressBar /><Slider /></StackPanel>");

        viewModel.SelectedNode = viewModel.PreviewRoot!.Children[0];
        var showProgressText = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "ShowProgressText");
        Assert.IsTrue(showProgressText.TrySetValue(true));
        await viewModel.PropertyProvider.ApplyAsync();

        viewModel.SelectedNode = viewModel.PreviewRoot.Children[1];
        var reverseDirection = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "IsDirectionReversed");
        Assert.IsTrue(reverseDirection.TrySetValue(true));
        await viewModel.PropertyProvider.ApplyAsync();

        Assert.AreEqual("True", viewModel.PreviewRoot.Children[0].Properties["ShowProgressText"]);
        Assert.AreEqual("True", viewModel.PreviewRoot.Children[1].Properties["IsDirectionReversed"]);
        viewModel.UndoCommand.Execute(null);
        Assert.IsFalse(viewModel.PreviewRoot.Children[1].Properties.ContainsKey("IsDirectionReversed"));
        viewModel.UndoCommand.Execute(null);
        Assert.IsFalse(viewModel.PreviewRoot.Children[0].Properties.ContainsKey("ShowProgressText"));
        viewModel.RedoCommand.Execute(null);
        viewModel.RedoCommand.Execute(null);
        Assert.AreEqual("True", viewModel.PreviewRoot.Children[1].Properties["IsDirectionReversed"]);
    }

    [TestMethod]
    public async Task TextBoxIsReadOnlySourceValue_IsEditableAndUndoable()
    {
        AxamlDesignerViewModel viewModel = new();
        viewModel.LoadMarkup("<TextBox xmlns='https://github.com/avaloniaui' IsReadOnly='True' />");
        viewModel.SelectedNode = viewModel.PreviewRoot;
        var isReadOnly = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "IsReadOnly");

        Assert.IsTrue(isReadOnly.TrySetValue(false));
        await viewModel.PropertyProvider.ApplyAsync();
        Assert.AreEqual("False", viewModel.PreviewRoot!.Properties["IsReadOnly"]);
        viewModel.UndoCommand.Execute(null);
        Assert.AreEqual("True", viewModel.PreviewRoot!.Properties["IsReadOnly"]);
        viewModel.RedoCommand.Execute(null);
        Assert.AreEqual("False", viewModel.PreviewRoot!.Properties["IsReadOnly"]);
    }

    [TestMethod]
    public async Task NumericAndProgressFormats_AreEditableAndUndoable()
    {
        AxamlDesignerViewModel viewModel = new();
        viewModel.LoadMarkup(
            "<StackPanel xmlns='https://github.com/avaloniaui'><NumericUpDown FormatString='N2' /><ProgressBar /></StackPanel>");

        viewModel.SelectedNode = viewModel.PreviewRoot!.Children[0];
        var numericFormat = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "FormatString");
        Assert.IsTrue(numericFormat.TrySetValue("F1"));
        await viewModel.PropertyProvider.ApplyAsync();

        viewModel.SelectedNode = viewModel.PreviewRoot.Children[1];
        var progressFormat = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "ProgressTextFormat");
        Assert.IsTrue(progressFormat.TrySetValue("{1:0.0}%"));
        await viewModel.PropertyProvider.ApplyAsync();

        Assert.AreEqual("F1", viewModel.PreviewRoot.Children[0].Properties["FormatString"]);
        Assert.AreEqual("{}{1:0.0}%", viewModel.PreviewRoot.Children[1].Properties["ProgressTextFormat"]);
        viewModel.UndoCommand.Execute(null);
        Assert.IsFalse(viewModel.PreviewRoot.Children[1].Properties.ContainsKey("ProgressTextFormat"));
        viewModel.UndoCommand.Execute(null);
        Assert.AreEqual("N2", viewModel.PreviewRoot.Children[0].Properties["FormatString"]);
        viewModel.RedoCommand.Execute(null);
        viewModel.RedoCommand.Execute(null);
        Assert.AreEqual("{}{1:0.0}%", viewModel.PreviewRoot.Children[1].Properties["ProgressTextFormat"]);
    }

    [TestMethod]
    public async Task ComboBoxPlaceholder_IsEditableAndUndoable()
    {
        AxamlDesignerViewModel viewModel = new();
        viewModel.LoadMarkup("<ComboBox xmlns='https://github.com/avaloniaui' PlaceholderText='Choose an option' />");
        viewModel.SelectedNode = viewModel.PreviewRoot;
        var placeholder = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "PlaceholderText");

        Assert.IsTrue(placeholder.TrySetValue("Select a value"));
        await viewModel.PropertyProvider.ApplyAsync();
        Assert.AreEqual("Select a value", viewModel.PreviewRoot!.Properties["PlaceholderText"]);
        viewModel.UndoCommand.Execute(null);
        Assert.AreEqual("Choose an option", viewModel.PreviewRoot!.Properties["PlaceholderText"]);
        viewModel.RedoCommand.Execute(null);
        Assert.AreEqual("Select a value", viewModel.PreviewRoot!.Properties["PlaceholderText"]);
    }

    [TestMethod]
    public async Task ProgressBarValue_RejectsNonFiniteDraftAndSupportsUndoRedo()
    {
        AxamlDesignerViewModel viewModel = new();
        viewModel.LoadMarkup("<ProgressBar xmlns='https://github.com/avaloniaui' Value='25.5' />");
        viewModel.SelectedNode = viewModel.PreviewRoot;
        var value = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "Value");

        Assert.IsFalse(value.TrySetValue("NaN"));
        Assert.IsTrue(viewModel.HasInvalidPropertyDraft);
        Assert.IsTrue(value.TrySetValue("42.5"));
        await viewModel.PropertyProvider.ApplyAsync();

        Assert.AreEqual("42.5", viewModel.PreviewRoot!.Properties["Value"]);
        viewModel.UndoCommand.Execute(null);
        Assert.AreEqual("25.5", viewModel.PreviewRoot!.Properties["Value"]);
        viewModel.RedoCommand.Execute(null);
        Assert.AreEqual("42.5", viewModel.PreviewRoot!.Properties["Value"]);
    }

    [TestMethod]
    public async Task SliderValue_RejectsNonFiniteDraftAndSupportsUndoRedo()
    {
        AxamlDesignerViewModel viewModel = new();
        viewModel.LoadMarkup("<Slider xmlns='https://github.com/avaloniaui' Value='25.5' />");
        viewModel.SelectedNode = viewModel.PreviewRoot;
        var value = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "Value");

        Assert.IsFalse(value.TrySetValue("Infinity"));
        Assert.IsTrue(viewModel.HasInvalidPropertyDraft);
        Assert.IsTrue(value.TrySetValue("42.5"));
        await viewModel.PropertyProvider.ApplyAsync();

        Assert.AreEqual("42.5", viewModel.PreviewRoot!.Properties["Value"]);
        viewModel.UndoCommand.Execute(null);
        Assert.AreEqual("25.5", viewModel.PreviewRoot!.Properties["Value"]);
        viewModel.RedoCommand.Execute(null);
        Assert.AreEqual("42.5", viewModel.PreviewRoot!.Properties["Value"]);
    }

    [TestMethod]
    public async Task ProgressBarIndeterminate_RequiresBooleanAndSupportsUndoRedo()
    {
        AxamlDesignerViewModel viewModel = new();
        viewModel.LoadMarkup("<ProgressBar xmlns='https://github.com/avaloniaui' IsIndeterminate='False' />");
        viewModel.SelectedNode = viewModel.PreviewRoot;
        var indeterminate = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "IsIndeterminate");

        Assert.IsFalse(indeterminate.TrySetValue("sometimes"));
        Assert.IsTrue(viewModel.HasInvalidPropertyDraft);
        Assert.IsTrue(indeterminate.TrySetValue("True"));
        await viewModel.PropertyProvider.ApplyAsync();

        Assert.AreEqual("True", viewModel.PreviewRoot!.Properties["IsIndeterminate"]);
        viewModel.UndoCommand.Execute(null);
        Assert.AreEqual("False", viewModel.PreviewRoot!.Properties["IsIndeterminate"]);
        viewModel.RedoCommand.Execute(null);
        Assert.AreEqual("True", viewModel.PreviewRoot!.Properties["IsIndeterminate"]);
    }

    [TestMethod]
    public async Task RangeControlOrientation_AppliesAndSupportsUndoRedo()
    {
        AxamlDesignerViewModel viewModel = new();
        viewModel.LoadMarkup("<StackPanel xmlns='https://github.com/avaloniaui'><Slider /><ProgressBar /></StackPanel>");

        viewModel.SelectedNode = viewModel.PreviewRoot!.Children[0];
        var sliderOrientation = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "Orientation");
        Assert.IsTrue(sliderOrientation.TrySetValue("Vertical"));
        await viewModel.PropertyProvider.ApplyAsync();

        viewModel.SelectedNode = viewModel.PreviewRoot.Children[1];
        var progressOrientation = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "Orientation");
        Assert.IsTrue(progressOrientation.TrySetValue("Vertical"));
        await viewModel.PropertyProvider.ApplyAsync();

        Assert.AreEqual("Vertical", viewModel.PreviewRoot.Children[0].Properties["Orientation"]);
        Assert.AreEqual("Vertical", viewModel.PreviewRoot.Children[1].Properties["Orientation"]);
        viewModel.UndoCommand.Execute(null);
        Assert.IsFalse(viewModel.PreviewRoot.Children[1].Properties.ContainsKey("Orientation"));
        viewModel.UndoCommand.Execute(null);
        Assert.IsFalse(viewModel.PreviewRoot.Children[0].Properties.ContainsKey("Orientation"));
        viewModel.RedoCommand.Execute(null);
        Assert.AreEqual("Vertical", viewModel.PreviewRoot.Children[0].Properties["Orientation"]);
    }

    [TestMethod]
    public async Task RangeControlBounds_AreEditableAndUndoable()
    {
        foreach (string controlType in new[] { "NumericUpDown", "ProgressBar", "Slider" })
        {
            AxamlDesignerViewModel viewModel = new();
            viewModel.LoadMarkup($"<{controlType} xmlns='https://github.com/avaloniaui' Minimum='10' Maximum='80' />");
            viewModel.SelectedNode = viewModel.PreviewRoot;

            var minimum = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "Minimum");
            var maximum = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "Maximum");
            Assert.IsTrue(minimum.TrySetValue("12.5"), $"{controlType}.Minimum");
            await viewModel.PropertyProvider.ApplyAsync();
            Assert.IsTrue(maximum.TrySetValue("90.5"), $"{controlType}.Maximum");
            await viewModel.PropertyProvider.ApplyAsync();

            Assert.AreEqual("12.5", viewModel.PreviewRoot!.Properties["Minimum"], $"{controlType}.Minimum");
            Assert.AreEqual("90.5", viewModel.PreviewRoot.Properties["Maximum"], $"{controlType}.Maximum");
            viewModel.UndoCommand.Execute(null);
            Assert.AreEqual("80", viewModel.PreviewRoot!.Properties["Maximum"], $"{controlType}.Maximum undo");
            viewModel.UndoCommand.Execute(null);
            Assert.AreEqual("10", viewModel.PreviewRoot!.Properties["Minimum"], $"{controlType}.Minimum undo");
            viewModel.RedoCommand.Execute(null);
            viewModel.RedoCommand.Execute(null);
            Assert.AreEqual("12.5", viewModel.PreviewRoot!.Properties["Minimum"], $"{controlType}.Minimum redo");
            Assert.AreEqual("90.5", viewModel.PreviewRoot.Properties["Maximum"], $"{controlType}.Maximum redo");
        }
    }

    [TestMethod]
    public async Task SelectorControlCheckedStates_AreEditableAndUndoable()
    {
        AxamlDesignerViewModel viewModel = new();
        viewModel.LoadMarkup(
            "<StackPanel xmlns='https://github.com/avaloniaui'><CheckBox /><RadioButton /><ToggleSwitch /></StackPanel>");

        for (int index = 0; index < viewModel.PreviewRoot!.Children.Count; index++)
        {
            viewModel.SelectedNode = viewModel.PreviewRoot.Children[index];
            var isChecked = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "IsChecked");
            Assert.IsTrue(isChecked.TrySetValue(true));
            await viewModel.PropertyProvider.ApplyAsync();
            Assert.AreEqual("True", viewModel.PreviewRoot.Children[index].Properties["IsChecked"]);
        }

        for (int index = viewModel.PreviewRoot.Children.Count - 1; index >= 0; index--)
        {
            viewModel.UndoCommand.Execute(null);
            Assert.IsFalse(viewModel.PreviewRoot.Children[index].Properties.ContainsKey("IsChecked"));
        }

        viewModel.RedoCommand.Execute(null);
        Assert.AreEqual("True", viewModel.PreviewRoot.Children[0].Properties["IsChecked"]);
    }

    [TestMethod]
    public async Task SelectorControlAppearance_IsEditableAndUndoable()
    {
        foreach (string controlType in new[] { "CheckBox", "RadioButton", "ToggleSwitch" })
        {
            AxamlDesignerViewModel viewModel = new();
            viewModel.LoadMarkup(
                $"<{controlType} xmlns='https://github.com/avaloniaui' Background='Red' CornerRadius='2' />");
            viewModel.SelectedNode = viewModel.PreviewRoot;
            var background = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "Background");
            var cornerRadius = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "CornerRadius");

            Assert.IsTrue(background.TrySetValue("Blue"));
            await viewModel.PropertyProvider.ApplyAsync();
            Assert.IsTrue(cornerRadius.TrySetValue("6,4,2,8"));
            await viewModel.PropertyProvider.ApplyAsync();

            Assert.AreEqual("Blue", viewModel.PreviewRoot!.Properties["Background"], controlType);
            Assert.AreEqual("6,4,2,8", viewModel.PreviewRoot.Properties["CornerRadius"], controlType);
            viewModel.UndoCommand.Execute(null);
            Assert.AreEqual("2", viewModel.PreviewRoot!.Properties["CornerRadius"], controlType);
            viewModel.UndoCommand.Execute(null);
            Assert.AreEqual("Red", viewModel.PreviewRoot!.Properties["Background"], controlType);
            viewModel.RedoCommand.Execute(null);
            viewModel.RedoCommand.Execute(null);
            Assert.AreEqual("Blue", viewModel.PreviewRoot!.Properties["Background"], controlType);
            Assert.AreEqual("6,4,2,8", viewModel.PreviewRoot.Properties["CornerRadius"], controlType);
        }
    }

    [TestMethod]
    public async Task RangeAndComboAppearance_IsEditableAndUndoable()
    {
        foreach (string controlType in new[] { "ComboBox", "NumericUpDown", "ProgressBar", "Slider" })
        {
            AxamlDesignerViewModel viewModel = new();
            viewModel.LoadMarkup(
                $"<{controlType} xmlns='https://github.com/avaloniaui' Background='Red' CornerRadius='2' />");
            viewModel.SelectedNode = viewModel.PreviewRoot;
            var background = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "Background");
            var cornerRadius = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "CornerRadius");

            Assert.IsTrue(background.TrySetValue("Blue"));
            await viewModel.PropertyProvider.ApplyAsync();
            Assert.IsTrue(cornerRadius.TrySetValue("6,4,2,8"));
            await viewModel.PropertyProvider.ApplyAsync();

            Assert.AreEqual("Blue", viewModel.PreviewRoot!.Properties["Background"], controlType);
            Assert.AreEqual("6,4,2,8", viewModel.PreviewRoot.Properties["CornerRadius"], controlType);
            viewModel.UndoCommand.Execute(null);
            Assert.AreEqual("2", viewModel.PreviewRoot!.Properties["CornerRadius"], controlType);
            viewModel.UndoCommand.Execute(null);
            Assert.AreEqual("Red", viewModel.PreviewRoot!.Properties["Background"], controlType);
            viewModel.RedoCommand.Execute(null);
            viewModel.RedoCommand.Execute(null);
            Assert.AreEqual("Blue", viewModel.PreviewRoot!.Properties["Background"], controlType);
            Assert.AreEqual("6,4,2,8", viewModel.PreviewRoot.Properties["CornerRadius"], controlType);
        }
    }

    [TestMethod]
    public async Task CheckBoxThreeState_IsBooleanEditableAndUndoable()
    {
        AxamlDesignerViewModel viewModel = new();
        viewModel.LoadMarkup("<CheckBox xmlns='https://github.com/avaloniaui' />");
        viewModel.SelectedNode = viewModel.PreviewRoot;
        var threeState = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "IsThreeState");

        Assert.IsTrue(threeState.TrySetValue(true));
        await viewModel.PropertyProvider.ApplyAsync();

        Assert.AreEqual("True", viewModel.PreviewRoot!.Properties["IsThreeState"]);
        viewModel.UndoCommand.Execute(null);
        Assert.IsFalse(viewModel.PreviewRoot!.Properties.ContainsKey("IsThreeState"));
        viewModel.RedoCommand.Execute(null);
        Assert.AreEqual("True", viewModel.PreviewRoot!.Properties["IsThreeState"]);
    }

    [TestMethod]
    public async Task ToggleSwitchThreeState_IsBooleanEditableAndUndoable()
    {
        AxamlDesignerViewModel viewModel = new();
        viewModel.LoadMarkup("<ToggleSwitch xmlns='https://github.com/avaloniaui' IsThreeState='False' />");
        viewModel.SelectedNode = viewModel.PreviewRoot;
        var threeState = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "IsThreeState");

        Assert.IsTrue(threeState.TrySetValue(true));
        await viewModel.PropertyProvider.ApplyAsync();
        Assert.AreEqual("True", viewModel.PreviewRoot!.Properties["IsThreeState"]);
        viewModel.UndoCommand.Execute(null);
        Assert.AreEqual("False", viewModel.PreviewRoot.Properties["IsThreeState"]);
        viewModel.RedoCommand.Execute(null);
        Assert.AreEqual("True", viewModel.PreviewRoot.Properties["IsThreeState"]);
    }

    [TestMethod]
    public async Task ToggleSwitchOnOffContent_IsEditableAndUndoable()
    {
        AxamlDesignerViewModel viewModel = new();
        viewModel.LoadMarkup("<ToggleSwitch xmlns='https://github.com/avaloniaui' />");
        viewModel.SelectedNode = viewModel.PreviewRoot;

        var onContent = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "OnContent");
        Assert.IsTrue(onContent.TrySetValue("Available"));
        await viewModel.PropertyProvider.ApplyAsync();
        var offContent = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "OffContent");
        Assert.IsTrue(offContent.TrySetValue("Unavailable"));
        await viewModel.PropertyProvider.ApplyAsync();

        Assert.AreEqual("Available", viewModel.PreviewRoot!.Properties["OnContent"]);
        Assert.AreEqual("Unavailable", viewModel.PreviewRoot.Properties["OffContent"]);
        viewModel.UndoCommand.Execute(null);
        Assert.IsFalse(viewModel.PreviewRoot.Properties.ContainsKey("OffContent"));
        viewModel.UndoCommand.Execute(null);
        Assert.IsFalse(viewModel.PreviewRoot.Properties.ContainsKey("OnContent"));
        viewModel.RedoCommand.Execute(null);
        Assert.AreEqual("Available", viewModel.PreviewRoot.Properties["OnContent"]);
    }

    [TestMethod]
    public async Task RadioButtonGroupName_IsEditableAndUndoable()
    {
        AxamlDesignerViewModel viewModel = new();
        viewModel.LoadMarkup("<RadioButton xmlns='https://github.com/avaloniaui' GroupName='Before' />");
        viewModel.SelectedNode = viewModel.PreviewRoot;
        var groupName = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "GroupName");

        Assert.IsTrue(groupName.TrySetValue("Options"));
        await viewModel.PropertyProvider.ApplyAsync();

        Assert.AreEqual("Options", viewModel.PreviewRoot!.Properties["GroupName"]);
        viewModel.UndoCommand.Execute(null);
        Assert.AreEqual("Before", viewModel.PreviewRoot!.Properties["GroupName"]);
        viewModel.RedoCommand.Execute(null);
        Assert.AreEqual("Options", viewModel.PreviewRoot!.Properties["GroupName"]);
    }

    [TestMethod]
    public async Task TextBoxInputConstraints_AreEditableAndUndoable()
    {
        AxamlDesignerViewModel viewModel = new();
        viewModel.LoadMarkup("<TextBox xmlns='https://github.com/avaloniaui' MaxLength='40' AcceptsReturn='False' TextAlignment='Left' TextWrapping='NoWrap' />");
        viewModel.SelectedNode = viewModel.PreviewRoot;

        var maxLength = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "MaxLength");
        Assert.IsTrue(maxLength.TrySetValue("120"));
        await viewModel.PropertyProvider.ApplyAsync();
        var acceptsReturn = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "AcceptsReturn");
        Assert.IsTrue(acceptsReturn.TrySetValue(true));
        await viewModel.PropertyProvider.ApplyAsync();
        var alignment = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "TextAlignment");
        Assert.IsTrue(alignment.TrySetValue("Center"));
        await viewModel.PropertyProvider.ApplyAsync();
        var wrapping = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "TextWrapping");
        Assert.IsTrue(wrapping.TrySetValue("Wrap"));
        await viewModel.PropertyProvider.ApplyAsync();

        Assert.AreEqual("120", viewModel.PreviewRoot!.Properties["MaxLength"]);
        Assert.AreEqual("True", viewModel.PreviewRoot.Properties["AcceptsReturn"]);
        Assert.AreEqual("Center", viewModel.PreviewRoot.Properties["TextAlignment"]);
        Assert.AreEqual("Wrap", viewModel.PreviewRoot.Properties["TextWrapping"]);
        viewModel.UndoCommand.Execute(null);
        Assert.AreEqual("NoWrap", viewModel.PreviewRoot.Properties["TextWrapping"]);
        viewModel.UndoCommand.Execute(null);
        Assert.AreEqual("Left", viewModel.PreviewRoot.Properties["TextAlignment"]);
        viewModel.UndoCommand.Execute(null);
        Assert.AreEqual("False", viewModel.PreviewRoot.Properties["AcceptsReturn"]);
        viewModel.UndoCommand.Execute(null);
        Assert.AreEqual("40", viewModel.PreviewRoot.Properties["MaxLength"]);
        viewModel.RedoCommand.Execute(null);
        Assert.AreEqual("120", viewModel.PreviewRoot.Properties["MaxLength"]);
        viewModel.RedoCommand.Execute(null);
        viewModel.RedoCommand.Execute(null);
        Assert.AreEqual("Center", viewModel.PreviewRoot.Properties["TextAlignment"]);
        viewModel.RedoCommand.Execute(null);
        Assert.AreEqual("Wrap", viewModel.PreviewRoot.Properties["TextWrapping"]);
    }

    [TestMethod]
    public async Task TextBlockTextAlignment_IsEditableAndUndoable()
    {
        AxamlDesignerViewModel viewModel = new();
        viewModel.LoadMarkup("<TextBlock xmlns='https://github.com/avaloniaui' Text='Align me' />");
        viewModel.SelectedNode = viewModel.PreviewRoot;
        var alignment = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "TextAlignment");

        Assert.IsTrue(alignment.TrySetValue("Right"));
        await viewModel.PropertyProvider.ApplyAsync();
        Assert.AreEqual("Right", viewModel.PreviewRoot!.Properties["TextAlignment"]);
        viewModel.UndoCommand.Execute(null);
        Assert.IsFalse(viewModel.PreviewRoot.Properties.ContainsKey("TextAlignment"));
        viewModel.RedoCommand.Execute(null);
        Assert.AreEqual("Right", viewModel.PreviewRoot.Properties["TextAlignment"]);
    }

    [TestMethod]
    public async Task ButtonDefaultAndCancel_AreEditableAndUndoable()
    {
        AxamlDesignerViewModel viewModel = new();
        viewModel.LoadMarkup("<Button xmlns='https://github.com/avaloniaui' Content='Dialog action' IsDefault='False' IsCancel='False' />");
        viewModel.SelectedNode = viewModel.PreviewRoot;

        var isDefault = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "IsDefault");
        Assert.IsTrue(isDefault.TrySetValue(true));
        await viewModel.PropertyProvider.ApplyAsync();
        var isCancel = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "IsCancel");
        Assert.IsTrue(isCancel.TrySetValue(true));
        await viewModel.PropertyProvider.ApplyAsync();

        Assert.AreEqual("True", viewModel.PreviewRoot!.Properties["IsDefault"]);
        Assert.AreEqual("True", viewModel.PreviewRoot.Properties["IsCancel"]);
        viewModel.UndoCommand.Execute(null);
        Assert.AreEqual("False", viewModel.PreviewRoot.Properties["IsCancel"]);
        viewModel.UndoCommand.Execute(null);
        Assert.AreEqual("False", viewModel.PreviewRoot.Properties["IsDefault"]);
        viewModel.RedoCommand.Execute(null);
        Assert.AreEqual("True", viewModel.PreviewRoot.Properties["IsDefault"]);
        viewModel.RedoCommand.Execute(null);
        Assert.AreEqual("True", viewModel.PreviewRoot.Properties["IsCancel"]);
    }

    [TestMethod]
    public async Task ButtonAppearance_IsEditableAndUndoable()
    {
        AxamlDesignerViewModel viewModel = new();
        viewModel.LoadMarkup(
            "<Button xmlns='https://github.com/avaloniaui' Background='Red' BorderBrush='Black' BorderThickness='1' CornerRadius='2' Foreground='White' Padding='2' />");
        viewModel.SelectedNode = viewModel.PreviewRoot;
        var background = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "Background");
        var borderBrush = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "BorderBrush");
        var borderThickness = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "BorderThickness");
        var cornerRadius = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "CornerRadius");
        var foreground = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "Foreground");
        var padding = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "Padding");

        Assert.IsTrue(background.TrySetValue("Blue"));
        await viewModel.PropertyProvider.ApplyAsync();
        Assert.IsTrue(borderBrush.TrySetValue("Green"));
        await viewModel.PropertyProvider.ApplyAsync();
        Assert.IsTrue(foreground.TrySetValue("Yellow"));
        await viewModel.PropertyProvider.ApplyAsync();
        Assert.IsTrue(padding.TrySetValue("8,4"));
        await viewModel.PropertyProvider.ApplyAsync();
        Assert.IsTrue(borderThickness.TrySetValue("3"));
        await viewModel.PropertyProvider.ApplyAsync();
        Assert.IsTrue(cornerRadius.TrySetValue("6,4,2,8"));
        await viewModel.PropertyProvider.ApplyAsync();

        Assert.AreEqual("Blue", viewModel.PreviewRoot!.Properties["Background"]);
        Assert.AreEqual("Green", viewModel.PreviewRoot.Properties["BorderBrush"]);
        Assert.AreEqual("Yellow", viewModel.PreviewRoot.Properties["Foreground"]);
        Assert.AreEqual("8,4", viewModel.PreviewRoot.Properties["Padding"]);
        Assert.AreEqual("3", viewModel.PreviewRoot.Properties["BorderThickness"]);
        Assert.AreEqual("6,4,2,8", viewModel.PreviewRoot.Properties["CornerRadius"]);
        viewModel.UndoCommand.Execute(null);
        Assert.AreEqual("2", viewModel.PreviewRoot!.Properties["CornerRadius"]);
        viewModel.UndoCommand.Execute(null);
        Assert.AreEqual("1", viewModel.PreviewRoot!.Properties["BorderThickness"]);
        viewModel.UndoCommand.Execute(null);
        Assert.AreEqual("2", viewModel.PreviewRoot!.Properties["Padding"]);
        viewModel.UndoCommand.Execute(null);
        Assert.AreEqual("White", viewModel.PreviewRoot!.Properties["Foreground"]);
        viewModel.UndoCommand.Execute(null);
        Assert.AreEqual("Black", viewModel.PreviewRoot!.Properties["BorderBrush"]);
        viewModel.UndoCommand.Execute(null);
        Assert.AreEqual("Red", viewModel.PreviewRoot!.Properties["Background"]);
        viewModel.RedoCommand.Execute(null);
        viewModel.RedoCommand.Execute(null);
        viewModel.RedoCommand.Execute(null);
        viewModel.RedoCommand.Execute(null);
        viewModel.RedoCommand.Execute(null);
        viewModel.RedoCommand.Execute(null);
        Assert.AreEqual("Blue", viewModel.PreviewRoot!.Properties["Background"]);
        Assert.AreEqual("Green", viewModel.PreviewRoot.Properties["BorderBrush"]);
        Assert.AreEqual("Yellow", viewModel.PreviewRoot.Properties["Foreground"]);
        Assert.AreEqual("8,4", viewModel.PreviewRoot.Properties["Padding"]);
        Assert.AreEqual("3", viewModel.PreviewRoot.Properties["BorderThickness"]);
        Assert.AreEqual("6,4,2,8", viewModel.PreviewRoot.Properties["CornerRadius"]);
    }

    [TestMethod]
    public async Task ButtonTypography_IsEditableAndUndoable()
    {
        AxamlDesignerViewModel viewModel = new();
        viewModel.LoadMarkup(
            "<Button xmlns='https://github.com/avaloniaui' FontSize='12' FontStyle='Normal' FontWeight='Normal' />");
        viewModel.SelectedNode = viewModel.PreviewRoot;
        var fontSize = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "FontSize");
        var fontStyle = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "FontStyle");
        var fontWeight = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "FontWeight");

        Assert.IsTrue(fontSize.TrySetValue("18"));
        await viewModel.PropertyProvider.ApplyAsync();
        Assert.IsTrue(fontStyle.TrySetValue("Italic"));
        await viewModel.PropertyProvider.ApplyAsync();
        Assert.IsTrue(fontWeight.TrySetValue("SemiBold"));
        await viewModel.PropertyProvider.ApplyAsync();

        Assert.AreEqual("18", viewModel.PreviewRoot!.Properties["FontSize"]);
        Assert.AreEqual("Italic", viewModel.PreviewRoot.Properties["FontStyle"]);
        Assert.AreEqual("SemiBold", viewModel.PreviewRoot.Properties["FontWeight"]);
        viewModel.UndoCommand.Execute(null);
        Assert.AreEqual("Normal", viewModel.PreviewRoot!.Properties["FontWeight"]);
        viewModel.UndoCommand.Execute(null);
        Assert.AreEqual("Normal", viewModel.PreviewRoot!.Properties["FontStyle"]);
        viewModel.UndoCommand.Execute(null);
        Assert.AreEqual("12", viewModel.PreviewRoot!.Properties["FontSize"]);
        viewModel.RedoCommand.Execute(null);
        viewModel.RedoCommand.Execute(null);
        viewModel.RedoCommand.Execute(null);
        Assert.AreEqual("18", viewModel.PreviewRoot!.Properties["FontSize"]);
        Assert.AreEqual("Italic", viewModel.PreviewRoot.Properties["FontStyle"]);
        Assert.AreEqual("SemiBold", viewModel.PreviewRoot.Properties["FontWeight"]);
    }

    [TestMethod]
    public async Task LabelTypography_IsEditableAndUndoable()
    {
        AxamlDesignerViewModel viewModel = new();
        viewModel.LoadMarkup(
            "<Label xmlns='https://github.com/avaloniaui' FontSize='12' FontStyle='Normal' FontWeight='Normal' Foreground='Black' />");
        viewModel.SelectedNode = viewModel.PreviewRoot;
        var fontSize = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "FontSize");
        var fontStyle = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "FontStyle");
        var fontWeight = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "FontWeight");
        var foreground = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "Foreground");

        Assert.IsTrue(fontSize.TrySetValue("18"));
        await viewModel.PropertyProvider.ApplyAsync();
        Assert.IsTrue(fontStyle.TrySetValue("Italic"));
        await viewModel.PropertyProvider.ApplyAsync();
        Assert.IsTrue(fontWeight.TrySetValue("SemiBold"));
        await viewModel.PropertyProvider.ApplyAsync();
        Assert.IsTrue(foreground.TrySetValue("Blue"));
        await viewModel.PropertyProvider.ApplyAsync();

        Assert.AreEqual("18", viewModel.PreviewRoot!.Properties["FontSize"]);
        Assert.AreEqual("Italic", viewModel.PreviewRoot.Properties["FontStyle"]);
        Assert.AreEqual("SemiBold", viewModel.PreviewRoot.Properties["FontWeight"]);
        Assert.AreEqual("Blue", viewModel.PreviewRoot.Properties["Foreground"]);
        viewModel.UndoCommand.Execute(null);
        Assert.AreEqual("Black", viewModel.PreviewRoot!.Properties["Foreground"]);
        viewModel.UndoCommand.Execute(null);
        Assert.AreEqual("Normal", viewModel.PreviewRoot!.Properties["FontWeight"]);
        viewModel.UndoCommand.Execute(null);
        Assert.AreEqual("Normal", viewModel.PreviewRoot!.Properties["FontStyle"]);
        viewModel.UndoCommand.Execute(null);
        Assert.AreEqual("12", viewModel.PreviewRoot!.Properties["FontSize"]);
        viewModel.RedoCommand.Execute(null);
        viewModel.RedoCommand.Execute(null);
        viewModel.RedoCommand.Execute(null);
        viewModel.RedoCommand.Execute(null);
        Assert.AreEqual("18", viewModel.PreviewRoot!.Properties["FontSize"]);
        Assert.AreEqual("Italic", viewModel.PreviewRoot.Properties["FontStyle"]);
        Assert.AreEqual("SemiBold", viewModel.PreviewRoot.Properties["FontWeight"]);
        Assert.AreEqual("Blue", viewModel.PreviewRoot.Properties["Foreground"]);
    }

    [TestMethod]
    public async Task LabelTextLayout_IsEditableAndUndoable()
    {
        AxamlDesignerViewModel viewModel = new();
        viewModel.LoadMarkup(
            "<Label xmlns='https://github.com/avaloniaui' LineHeight='12' MaxLines='1' TextAlignment='Left' TextTrimming='None' TextWrapping='Wrap' />");
        viewModel.SelectedNode = viewModel.PreviewRoot;
        var lineHeight = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "LineHeight");
        var maxLines = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "MaxLines");
        var alignment = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "TextAlignment");
        var trimming = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "TextTrimming");
        var wrapping = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "TextWrapping");

        Assert.IsTrue(lineHeight.TrySetValue("24"));
        await viewModel.PropertyProvider.ApplyAsync();
        Assert.IsTrue(maxLines.TrySetValue("3"));
        await viewModel.PropertyProvider.ApplyAsync();
        Assert.IsTrue(alignment.TrySetValue("Right"));
        await viewModel.PropertyProvider.ApplyAsync();
        Assert.IsTrue(trimming.TrySetValue("CharacterEllipsis"));
        await viewModel.PropertyProvider.ApplyAsync();
        Assert.IsTrue(wrapping.TrySetValue("NoWrap"));
        await viewModel.PropertyProvider.ApplyAsync();

        Assert.AreEqual("24", viewModel.PreviewRoot!.Properties["LineHeight"]);
        Assert.AreEqual("3", viewModel.PreviewRoot.Properties["MaxLines"]);
        Assert.AreEqual("Right", viewModel.PreviewRoot!.Properties["TextAlignment"]);
        Assert.AreEqual("CharacterEllipsis", viewModel.PreviewRoot.Properties["TextTrimming"]);
        Assert.AreEqual("NoWrap", viewModel.PreviewRoot.Properties["TextWrapping"]);
        viewModel.UndoCommand.Execute(null);
        Assert.AreEqual("Wrap", viewModel.PreviewRoot!.Properties["TextWrapping"]);
        viewModel.UndoCommand.Execute(null);
        Assert.AreEqual("None", viewModel.PreviewRoot!.Properties["TextTrimming"]);
        viewModel.UndoCommand.Execute(null);
        Assert.AreEqual("Left", viewModel.PreviewRoot!.Properties["TextAlignment"]);
        viewModel.UndoCommand.Execute(null);
        Assert.AreEqual("1", viewModel.PreviewRoot!.Properties["MaxLines"]);
        viewModel.UndoCommand.Execute(null);
        Assert.AreEqual("12", viewModel.PreviewRoot!.Properties["LineHeight"]);
        viewModel.RedoCommand.Execute(null);
        viewModel.RedoCommand.Execute(null);
        viewModel.RedoCommand.Execute(null);
        viewModel.RedoCommand.Execute(null);
        viewModel.RedoCommand.Execute(null);
        Assert.AreEqual("24", viewModel.PreviewRoot!.Properties["LineHeight"]);
        Assert.AreEqual("3", viewModel.PreviewRoot.Properties["MaxLines"]);
        Assert.AreEqual("Right", viewModel.PreviewRoot!.Properties["TextAlignment"]);
        Assert.AreEqual("CharacterEllipsis", viewModel.PreviewRoot.Properties["TextTrimming"]);
        Assert.AreEqual("NoWrap", viewModel.PreviewRoot.Properties["TextWrapping"]);
    }

    [TestMethod]
    public async Task ContentControlAppearance_IsEditableAndUndoable()
    {
        AxamlDesignerViewModel viewModel = new();
        viewModel.LoadMarkup(
            "<ContentControl xmlns='https://github.com/avaloniaui' Background='White' BorderBrush='Black' BorderThickness='1' CornerRadius='2' FontSize='12' FontStyle='Normal' FontWeight='Normal' Foreground='White' Padding='3' />");
        viewModel.SelectedNode = viewModel.PreviewRoot;
        (string Name, string Value, string InitialValue)[] edits =
        [
            ("Background", "AliceBlue", "White"),
            ("BorderBrush", "Blue", "Black"),
            ("BorderThickness", "1,2,3,4", "1"),
            ("CornerRadius", "4,2,4,2", "2"),
            ("FontSize", "18", "12"),
            ("FontStyle", "Italic", "Normal"),
            ("FontWeight", "SemiBold", "Normal"),
            ("Foreground", "Yellow", "White"),
            ("Padding", "6,3", "3"),
        ];

        foreach (var (name, value, _) in edits)
        {
            var property = viewModel.PropertyProvider.Properties.Single(item => item.Name == name);
            Assert.IsTrue(property.TrySetValue(value), name);
            await viewModel.PropertyProvider.ApplyAsync();
        }

        foreach (var (name, value, _) in edits)
        {
            Assert.AreEqual(value, viewModel.PreviewRoot!.Properties[name]);
        }

        for (int index = edits.Length - 1; index >= 0; index--)
        {
            viewModel.UndoCommand.Execute(null);
            Assert.AreEqual(edits[index].InitialValue, viewModel.PreviewRoot!.Properties[edits[index].Name]);
        }

        foreach (var (name, value, _) in edits)
        {
            viewModel.RedoCommand.Execute(null);
            Assert.AreEqual(value, viewModel.PreviewRoot!.Properties[name]);
        }
    }

    [TestMethod]
    public async Task ListBoxSelectedIndex_IsEditableAndUndoable()
    {
        AxamlDesignerViewModel viewModel = new();
        viewModel.LoadMarkup(
            "<ListBox xmlns='https://github.com/avaloniaui' SelectedIndex='0'><TextBlock Text='First' /><TextBlock Text='Second' /></ListBox>");
        viewModel.SelectedNode = viewModel.PreviewRoot;
        var selectedIndex = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "SelectedIndex");

        Assert.IsTrue(selectedIndex.TrySetValue("1"));
        await viewModel.PropertyProvider.ApplyAsync();
        Assert.AreEqual("1", viewModel.PreviewRoot!.Properties["SelectedIndex"]);
        viewModel.UndoCommand.Execute(null);
        Assert.AreEqual("0", viewModel.PreviewRoot.Properties["SelectedIndex"]);
        viewModel.RedoCommand.Execute(null);
        Assert.AreEqual("1", viewModel.PreviewRoot.Properties["SelectedIndex"]);
    }

    [TestMethod]
    public async Task ComboBoxSelectedIndex_IsEditableAndUndoable()
    {
        AxamlDesignerViewModel viewModel = new();
        viewModel.LoadMarkup(
            "<ComboBox xmlns='https://github.com/avaloniaui' SelectedIndex='0'><TextBlock Text='First' /><TextBlock Text='Second' /></ComboBox>");
        viewModel.SelectedNode = viewModel.PreviewRoot;
        var selectedIndex = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "SelectedIndex");

        Assert.IsTrue(selectedIndex.TrySetValue("1"));
        await viewModel.PropertyProvider.ApplyAsync();
        Assert.AreEqual("1", viewModel.PreviewRoot!.Properties["SelectedIndex"]);
        viewModel.UndoCommand.Execute(null);
        Assert.AreEqual("0", viewModel.PreviewRoot.Properties["SelectedIndex"]);
        viewModel.RedoCommand.Execute(null);
        Assert.AreEqual("1", viewModel.PreviewRoot.Properties["SelectedIndex"]);
    }

    [TestMethod]
    public async Task DatePickerSelectedDate_IsEditableAndUndoable()
    {
        AxamlDesignerViewModel viewModel = new();
        viewModel.LoadMarkup("<DatePicker xmlns='https://github.com/avaloniaui' SelectedDate='2024-02-28' />");
        viewModel.SelectedNode = viewModel.PreviewRoot;
        var selectedDate = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "SelectedDate");

        Assert.IsTrue(selectedDate.TrySetValue("2024-02-29"));
        await viewModel.PropertyProvider.ApplyAsync();
        Assert.AreEqual("2024-02-29", viewModel.PreviewRoot!.Properties["SelectedDate"]);
        viewModel.UndoCommand.Execute(null);
        Assert.AreEqual("2024-02-28", viewModel.PreviewRoot.Properties["SelectedDate"]);
        viewModel.RedoCommand.Execute(null);
        Assert.AreEqual("2024-02-29", viewModel.PreviewRoot.Properties["SelectedDate"]);
    }

    [TestMethod]
    public async Task DatePickerVisibility_IsEditableAndUndoable()
    {
        AxamlDesignerViewModel viewModel = new();
        viewModel.LoadMarkup("<DatePicker xmlns='https://github.com/avaloniaui' DayVisible='True' />");
        viewModel.SelectedNode = viewModel.PreviewRoot;
        var dayVisible = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "DayVisible");

        Assert.IsTrue(dayVisible.TrySetValue("False"));
        await viewModel.PropertyProvider.ApplyAsync();
        Assert.AreEqual("False", viewModel.PreviewRoot!.Properties["DayVisible"]);
        viewModel.UndoCommand.Execute(null);
        Assert.AreEqual("True", viewModel.PreviewRoot.Properties["DayVisible"]);
        viewModel.RedoCommand.Execute(null);
        Assert.AreEqual("False", viewModel.PreviewRoot.Properties["DayVisible"]);
    }

    [TestMethod]
    public async Task DatePickerFormats_AreEditableAndUndoable()
    {
        AxamlDesignerViewModel viewModel = new();
        viewModel.LoadMarkup("<DatePicker xmlns='https://github.com/avaloniaui' DayFormat='d' />");
        viewModel.SelectedNode = viewModel.PreviewRoot;
        var dayFormat = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "DayFormat");

        Assert.IsTrue(dayFormat.TrySetValue("dd"));
        await viewModel.PropertyProvider.ApplyAsync();
        Assert.AreEqual("dd", viewModel.PreviewRoot!.Properties["DayFormat"]);
        viewModel.UndoCommand.Execute(null);
        Assert.AreEqual("d", viewModel.PreviewRoot.Properties["DayFormat"]);
        viewModel.RedoCommand.Execute(null);
        Assert.AreEqual("dd", viewModel.PreviewRoot.Properties["DayFormat"]);
    }

    [TestMethod]
    public async Task DatePickerYearRange_IsEditableAndUndoable()
    {
        AxamlDesignerViewModel viewModel = new();
        viewModel.LoadMarkup("<DatePicker xmlns='https://github.com/avaloniaui' MinYear='2020-01-01' MaxYear='2030-12-31' />");
        viewModel.SelectedNode = viewModel.PreviewRoot;
        var minYear = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "MinYear");

        Assert.IsTrue(minYear.TrySetValue("2021-01-01"));
        await viewModel.PropertyProvider.ApplyAsync();
        Assert.AreEqual("2021-01-01", viewModel.PreviewRoot!.Properties["MinYear"]);
        viewModel.UndoCommand.Execute(null);
        Assert.AreEqual("2020-01-01", viewModel.PreviewRoot.Properties["MinYear"]);
        viewModel.RedoCommand.Execute(null);
        Assert.AreEqual("2021-01-01", viewModel.PreviewRoot.Properties["MinYear"]);
    }

    [TestMethod]
    public async Task TimePickerSelectedTime_IsEditableAndUndoable()
    {
        AxamlDesignerViewModel viewModel = new();
        viewModel.LoadMarkup("<TimePicker xmlns='https://github.com/avaloniaui' SelectedTime='09:15' />");
        viewModel.SelectedNode = viewModel.PreviewRoot;
        var selectedTime = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "SelectedTime");

        Assert.IsTrue(selectedTime.TrySetValue("09:30:45"));
        await viewModel.PropertyProvider.ApplyAsync();
        Assert.AreEqual("09:30:45", viewModel.PreviewRoot!.Properties["SelectedTime"]);
        viewModel.UndoCommand.Execute(null);
        Assert.AreEqual("09:15", viewModel.PreviewRoot.Properties["SelectedTime"]);
        viewModel.RedoCommand.Execute(null);
        Assert.AreEqual("09:30:45", viewModel.PreviewRoot.Properties["SelectedTime"]);
    }

    [TestMethod]
    public async Task CalendarSelectedDate_IsEditableAndUndoable()
    {
        AxamlDesignerViewModel viewModel = new();
        viewModel.LoadMarkup("<Calendar xmlns='https://github.com/avaloniaui' SelectedDate='2024-02-28' />");
        viewModel.SelectedNode = viewModel.PreviewRoot;
        var selectedDate = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "SelectedDate");

        Assert.IsTrue(selectedDate.TrySetValue("2024-02-29"));
        await viewModel.PropertyProvider.ApplyAsync();
        Assert.AreEqual("2024-02-29", viewModel.PreviewRoot!.Properties["SelectedDate"]);
        viewModel.UndoCommand.Execute(null);
        Assert.AreEqual("2024-02-28", viewModel.PreviewRoot.Properties["SelectedDate"]);
        viewModel.RedoCommand.Execute(null);
        Assert.AreEqual("2024-02-29", viewModel.PreviewRoot.Properties["SelectedDate"]);
    }

    [TestMethod]
    public async Task ColorPickerColor_IsEditableAndUndoable()
    {
        AxamlDesignerViewModel viewModel = new();
        viewModel.LoadMarkup("<ColorPicker xmlns='https://github.com/avaloniaui' Color='#FF112233' />");
        viewModel.SelectedNode = viewModel.PreviewRoot;
        var color = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "Color");

        Assert.IsTrue(color.TrySetValue("#80445566"));
        await viewModel.PropertyProvider.ApplyAsync();
        Assert.AreEqual("#80445566", viewModel.PreviewRoot!.Properties["Color"]);
        viewModel.UndoCommand.Execute(null);
        Assert.AreEqual("#FF112233", viewModel.PreviewRoot.Properties["Color"]);
        viewModel.RedoCommand.Execute(null);
        Assert.AreEqual("#80445566", viewModel.PreviewRoot.Properties["Color"]);
    }

    [TestMethod]
    public async Task AutoCompleteBoxText_IsEditableAndUndoable()
    {
        AxamlDesignerViewModel viewModel = new();
        viewModel.LoadMarkup("<AutoCompleteBox xmlns='https://github.com/avaloniaui' Text='Par' />");
        viewModel.SelectedNode = viewModel.PreviewRoot;
        var text = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "Text");

        Assert.IsTrue(text.TrySetValue("Paris"));
        await viewModel.PropertyProvider.ApplyAsync();
        Assert.AreEqual("Paris", viewModel.PreviewRoot!.Properties["Text"]);
        viewModel.UndoCommand.Execute(null);
        Assert.AreEqual("Par", viewModel.PreviewRoot.Properties["Text"]);
        viewModel.RedoCommand.Execute(null);
        Assert.AreEqual("Paris", viewModel.PreviewRoot.Properties["Text"]);
    }

    [TestMethod]
    public async Task TabControlSelection_IsEditableAndUndoable()
    {
        AxamlDesignerViewModel viewModel = new();
        viewModel.LoadMarkup(
            "<TabControl xmlns='https://github.com/avaloniaui' SelectedIndex='0'><TabItem Header='General' /><TabItem Header='Advanced' /></TabControl>");
        viewModel.SelectedNode = viewModel.PreviewRoot;
        var selection = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "SelectedIndex");

        Assert.IsTrue(selection.TrySetValue("1"));
        await viewModel.PropertyProvider.ApplyAsync();
        Assert.AreEqual("1", viewModel.PreviewRoot!.Properties["SelectedIndex"]);
        viewModel.UndoCommand.Execute(null);
        Assert.AreEqual("0", viewModel.PreviewRoot.Properties["SelectedIndex"]);
        viewModel.RedoCommand.Execute(null);
        Assert.AreEqual("1", viewModel.PreviewRoot.Properties["SelectedIndex"]);
    }

    [TestMethod]
    public async Task TabItemHeader_IsEditableAndUndoable()
    {
        AxamlDesignerViewModel viewModel = new();
        viewModel.LoadMarkup(
            "<TabControl xmlns='https://github.com/avaloniaui'><TabItem Header='General' /></TabControl>");
        viewModel.SelectedNode = viewModel.PreviewRoot!.Children.Single();
        AxamlPropertyRowViewModel header = viewModel.PropertyRows.Single(static row => row.Name == "Header");

        Assert.IsTrue(header.TrySetValue("Configuration"));
        await viewModel.PropertyProvider.ApplyAsync();
        Assert.AreEqual("Configuration", viewModel.SelectedNode!.Properties["Header"]);
        viewModel.UndoCommand.Execute(null);
        Assert.AreEqual("General", viewModel.SelectedNode.Properties["Header"]);
        viewModel.RedoCommand.Execute(null);
        Assert.AreEqual("Configuration", viewModel.SelectedNode.Properties["Header"]);
    }

    [TestMethod]
    public async Task MenuItemHeader_IsEditableAndUndoable()
    {
        AxamlDesignerViewModel viewModel = new();
        viewModel.LoadMarkup(
            "<Menu xmlns='https://github.com/avaloniaui'><MenuItem Header='File' /></Menu>");
        viewModel.SelectedNode = viewModel.PreviewRoot!.Children.Single();
        AxamlPropertyRowViewModel header = viewModel.PropertyRows.Single(static row => row.Name == "Header");

        Assert.IsTrue(header.TrySetValue("Project"));
        await viewModel.PropertyProvider.ApplyAsync();
        Assert.AreEqual("Project", viewModel.SelectedNode!.Properties["Header"]);
        viewModel.UndoCommand.Execute(null);
        Assert.AreEqual("File", viewModel.SelectedNode.Properties["Header"]);
        viewModel.RedoCommand.Execute(null);
        Assert.AreEqual("Project", viewModel.SelectedNode.Properties["Header"]);
    }

    [TestMethod]
    public async Task BorderAppearance_IsEditableAndUndoable()
    {
        AxamlDesignerViewModel viewModel = new();
        viewModel.LoadMarkup(
            "<Border xmlns='https://github.com/avaloniaui' BorderBrush='Black' CornerRadius='2' />");
        viewModel.SelectedNode = viewModel.PreviewRoot;
        var borderBrush = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "BorderBrush");
        var cornerRadius = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "CornerRadius");

        Assert.IsTrue(borderBrush.TrySetValue("Purple"));
        await viewModel.PropertyProvider.ApplyAsync();
        Assert.IsTrue(cornerRadius.TrySetValue("12,4,8,2"));
        await viewModel.PropertyProvider.ApplyAsync();

        Assert.AreEqual("Purple", viewModel.PreviewRoot!.Properties["BorderBrush"]);
        Assert.AreEqual("12,4,8,2", viewModel.PreviewRoot.Properties["CornerRadius"]);
        viewModel.UndoCommand.Execute(null);
        Assert.AreEqual("2", viewModel.PreviewRoot!.Properties["CornerRadius"]);
        viewModel.UndoCommand.Execute(null);
        Assert.AreEqual("Black", viewModel.PreviewRoot.Properties["BorderBrush"]);
        viewModel.RedoCommand.Execute(null);
        viewModel.RedoCommand.Execute(null);
        Assert.AreEqual("Purple", viewModel.PreviewRoot!.Properties["BorderBrush"]);
        Assert.AreEqual("12,4,8,2", viewModel.PreviewRoot.Properties["CornerRadius"]);
    }

    [TestMethod]
    public async Task TextBlockFontStyle_IsEditableAndUndoable()
    {
        AxamlDesignerViewModel viewModel = new();
        viewModel.LoadMarkup("<TextBlock xmlns='https://github.com/avaloniaui' Text='Style me' FontStyle='Normal' />");
        viewModel.SelectedNode = viewModel.PreviewRoot;
        var fontStyle = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "FontStyle");

        Assert.IsTrue(fontStyle.TrySetValue("Italic"));
        await viewModel.PropertyProvider.ApplyAsync();
        Assert.AreEqual("Italic", viewModel.PreviewRoot!.Properties["FontStyle"]);
        viewModel.UndoCommand.Execute(null);
        Assert.AreEqual("Normal", viewModel.PreviewRoot.Properties["FontStyle"]);
        viewModel.RedoCommand.Execute(null);
        Assert.AreEqual("Italic", viewModel.PreviewRoot.Properties["FontStyle"]);
    }

    [TestMethod]
    public async Task TextBlockFontWeight_IsEditableAndUndoable()
    {
        AxamlDesignerViewModel viewModel = new();
        viewModel.LoadMarkup("<TextBlock xmlns='https://github.com/avaloniaui' Text='Weight me' FontWeight='Normal' />");
        viewModel.SelectedNode = viewModel.PreviewRoot;
        var fontWeight = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "FontWeight");

        Assert.IsTrue(fontWeight.TrySetValue("SemiBold"));
        await viewModel.PropertyProvider.ApplyAsync();
        Assert.AreEqual("SemiBold", viewModel.PreviewRoot!.Properties["FontWeight"]);
        viewModel.UndoCommand.Execute(null);
        Assert.AreEqual("Normal", viewModel.PreviewRoot.Properties["FontWeight"]);
        viewModel.RedoCommand.Execute(null);
        Assert.AreEqual("SemiBold", viewModel.PreviewRoot.Properties["FontWeight"]);
    }

    [TestMethod]
    public async Task TextBlockWrappingAndForeground_AreEditableAndUndoable()
    {
        AxamlDesignerViewModel viewModel = new();
        viewModel.LoadMarkup("<TextBlock xmlns='https://github.com/avaloniaui' Text='Style me' TextWrapping='Wrap' Foreground='Red' />");
        viewModel.SelectedNode = viewModel.PreviewRoot;

        var wrapping = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "TextWrapping");
        Assert.IsTrue(wrapping.TrySetValue("NoWrap"));
        await viewModel.PropertyProvider.ApplyAsync();
        var foreground = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "Foreground");
        Assert.IsTrue(foreground.TrySetValue("Blue"));
        await viewModel.PropertyProvider.ApplyAsync();

        Assert.AreEqual("NoWrap", viewModel.PreviewRoot!.Properties["TextWrapping"]);
        Assert.AreEqual("Blue", viewModel.PreviewRoot.Properties["Foreground"]);
        viewModel.UndoCommand.Execute(null);
        Assert.AreEqual("Red", viewModel.PreviewRoot.Properties["Foreground"]);
        viewModel.UndoCommand.Execute(null);
        Assert.AreEqual("Wrap", viewModel.PreviewRoot.Properties["TextWrapping"]);
        viewModel.RedoCommand.Execute(null);
        Assert.AreEqual("NoWrap", viewModel.PreviewRoot.Properties["TextWrapping"]);
        viewModel.RedoCommand.Execute(null);
        Assert.AreEqual("Blue", viewModel.PreviewRoot.Properties["Foreground"]);
    }

    [TestMethod]
    public async Task TextBoxForeground_IsEditableAndUndoable()
    {
        AxamlDesignerViewModel viewModel = new();
        viewModel.LoadMarkup("<TextBox xmlns='https://github.com/avaloniaui' Text='Read-only preview' Foreground='Red' />");
        viewModel.SelectedNode = viewModel.PreviewRoot;
        var foreground = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "Foreground");

        Assert.IsTrue(foreground.TrySetValue("Blue"));
        await viewModel.PropertyProvider.ApplyAsync();
        Assert.AreEqual("Blue", viewModel.PreviewRoot!.Properties["Foreground"]);
        viewModel.UndoCommand.Execute(null);
        Assert.AreEqual("Red", viewModel.PreviewRoot.Properties["Foreground"]);
        viewModel.RedoCommand.Execute(null);
        Assert.AreEqual("Blue", viewModel.PreviewRoot.Properties["Foreground"]);
    }

    [TestMethod]
    public async Task TextBoxBackgroundAndPadding_AreEditableAndUndoable()
    {
        AxamlDesignerViewModel viewModel = new();
        viewModel.LoadMarkup("<TextBox xmlns='https://github.com/avaloniaui' Background='Red' BorderBrush='Black' Padding='2' />");
        viewModel.SelectedNode = viewModel.PreviewRoot;
        var background = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "Background");
        var borderBrush = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "BorderBrush");
        var padding = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "Padding");

        Assert.IsTrue(background.TrySetValue("Blue"));
        await viewModel.PropertyProvider.ApplyAsync();
        Assert.IsTrue(borderBrush.TrySetValue("Green"));
        await viewModel.PropertyProvider.ApplyAsync();
        Assert.IsTrue(padding.TrySetValue("8,4"));
        await viewModel.PropertyProvider.ApplyAsync();

        Assert.AreEqual("Blue", viewModel.PreviewRoot!.Properties["Background"]);
        Assert.AreEqual("Green", viewModel.PreviewRoot.Properties["BorderBrush"]);
        Assert.AreEqual("8,4", viewModel.PreviewRoot.Properties["Padding"]);
        viewModel.UndoCommand.Execute(null);
        Assert.AreEqual("2", viewModel.PreviewRoot!.Properties["Padding"]);
        viewModel.UndoCommand.Execute(null);
        Assert.AreEqual("Black", viewModel.PreviewRoot!.Properties["BorderBrush"]);
        viewModel.UndoCommand.Execute(null);
        Assert.AreEqual("Red", viewModel.PreviewRoot!.Properties["Background"]);
        viewModel.RedoCommand.Execute(null);
        viewModel.RedoCommand.Execute(null);
        viewModel.RedoCommand.Execute(null);
        Assert.AreEqual("Blue", viewModel.PreviewRoot!.Properties["Background"]);
        Assert.AreEqual("Green", viewModel.PreviewRoot.Properties["BorderBrush"]);
        Assert.AreEqual("8,4", viewModel.PreviewRoot.Properties["Padding"]);
    }

    [TestMethod]
    public async Task TextBoxFrame_IsEditableAndUndoable()
    {
        AxamlDesignerViewModel viewModel = new();
        viewModel.LoadMarkup(
            "<TextBox xmlns='https://github.com/avaloniaui' BorderThickness='1' CornerRadius='2' />");
        viewModel.SelectedNode = viewModel.PreviewRoot;
        var borderThickness = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "BorderThickness");
        var cornerRadius = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "CornerRadius");

        Assert.IsTrue(borderThickness.TrySetValue("2,3"));
        await viewModel.PropertyProvider.ApplyAsync();
        Assert.IsTrue(cornerRadius.TrySetValue("1,2,3,4"));
        await viewModel.PropertyProvider.ApplyAsync();

        Assert.AreEqual("2,3", viewModel.PreviewRoot!.Properties["BorderThickness"]);
        Assert.AreEqual("1,2,3,4", viewModel.PreviewRoot.Properties["CornerRadius"]);
        viewModel.UndoCommand.Execute(null);
        Assert.AreEqual("2", viewModel.PreviewRoot!.Properties["CornerRadius"]);
        viewModel.UndoCommand.Execute(null);
        Assert.AreEqual("1", viewModel.PreviewRoot!.Properties["BorderThickness"]);
        viewModel.RedoCommand.Execute(null);
        viewModel.RedoCommand.Execute(null);
        Assert.AreEqual("2,3", viewModel.PreviewRoot!.Properties["BorderThickness"]);
        Assert.AreEqual("1,2,3,4", viewModel.PreviewRoot.Properties["CornerRadius"]);
    }

    [TestMethod]
    public async Task TextBoxFontStyleAndWeight_AreEditableAndUndoable()
    {
        AxamlDesignerViewModel viewModel = new();
        viewModel.LoadMarkup("<TextBox xmlns='https://github.com/avaloniaui' FontStyle='Normal' FontWeight='Normal' />");
        viewModel.SelectedNode = viewModel.PreviewRoot;
        var fontStyle = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "FontStyle");
        var fontWeight = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "FontWeight");

        Assert.IsTrue(fontStyle.TrySetValue("Italic"));
        await viewModel.PropertyProvider.ApplyAsync();
        Assert.IsTrue(fontWeight.TrySetValue("SemiBold"));
        await viewModel.PropertyProvider.ApplyAsync();

        Assert.AreEqual("Italic", viewModel.PreviewRoot!.Properties["FontStyle"]);
        Assert.AreEqual("SemiBold", viewModel.PreviewRoot.Properties["FontWeight"]);
        viewModel.UndoCommand.Execute(null);
        Assert.AreEqual("Normal", viewModel.PreviewRoot.Properties["FontWeight"]);
        viewModel.UndoCommand.Execute(null);
        Assert.AreEqual("Normal", viewModel.PreviewRoot.Properties["FontStyle"]);
        viewModel.RedoCommand.Execute(null);
        viewModel.RedoCommand.Execute(null);
        Assert.AreEqual("Italic", viewModel.PreviewRoot.Properties["FontStyle"]);
        Assert.AreEqual("SemiBold", viewModel.PreviewRoot.Properties["FontWeight"]);
    }

    [TestMethod]
    public async Task TextBoxFontSizeAndControlAlignment_AreEditableAndUndoable()
    {
        AxamlDesignerViewModel viewModel = new();
        viewModel.LoadMarkup("<TextBox xmlns='https://github.com/avaloniaui' FontSize='14' HorizontalAlignment='Stretch' VerticalAlignment='Stretch' />");
        viewModel.SelectedNode = viewModel.PreviewRoot;
        var fontSize = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "FontSize");
        var horizontalAlignment = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "HorizontalAlignment");
        var verticalAlignment = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "VerticalAlignment");

        Assert.IsTrue(fontSize.TrySetValue("18"));
        await viewModel.PropertyProvider.ApplyAsync();
        Assert.IsTrue(horizontalAlignment.TrySetValue("Right"));
        await viewModel.PropertyProvider.ApplyAsync();
        Assert.IsTrue(verticalAlignment.TrySetValue("Bottom"));
        await viewModel.PropertyProvider.ApplyAsync();

        Assert.AreEqual("18", viewModel.PreviewRoot!.Properties["FontSize"]);
        Assert.AreEqual("Right", viewModel.PreviewRoot.Properties["HorizontalAlignment"]);
        Assert.AreEqual("Bottom", viewModel.PreviewRoot.Properties["VerticalAlignment"]);
        viewModel.UndoCommand.Execute(null);
        Assert.AreEqual("Stretch", viewModel.PreviewRoot.Properties["VerticalAlignment"]);
        viewModel.UndoCommand.Execute(null);
        Assert.AreEqual("Stretch", viewModel.PreviewRoot.Properties["HorizontalAlignment"]);
        viewModel.UndoCommand.Execute(null);
        Assert.AreEqual("14", viewModel.PreviewRoot.Properties["FontSize"]);
        viewModel.RedoCommand.Execute(null);
        viewModel.RedoCommand.Execute(null);
        viewModel.RedoCommand.Execute(null);
        Assert.AreEqual("18", viewModel.PreviewRoot.Properties["FontSize"]);
        Assert.AreEqual("Right", viewModel.PreviewRoot.Properties["HorizontalAlignment"]);
        Assert.AreEqual("Bottom", viewModel.PreviewRoot.Properties["VerticalAlignment"]);
    }

    [TestMethod]
    public async Task ControlSizeProperties_AreEditableAndUndoable()
    {
        AxamlDesignerViewModel viewModel = new();
        viewModel.LoadMarkup("<NumericUpDown xmlns='https://github.com/avaloniaui' />");
        viewModel.SelectedNode = viewModel.PreviewRoot;

        foreach ((string propertyName, string value) in new[]
        {
            ("Width", "200"),
            ("Height", "60"),
            ("MinWidth", "40"),
            ("MaxWidth", "120"),
            ("MinHeight", "20"),
            ("MaxHeight", "80"),
        })
        {
            var property = viewModel.PropertyProvider.Properties.Single(row => row.Name == propertyName);
            Assert.IsTrue(property.TrySetValue(value), propertyName);
            await viewModel.PropertyProvider.ApplyAsync();
        }

        Assert.AreEqual("200", viewModel.PreviewRoot!.Properties["Width"]);
        Assert.AreEqual("60", viewModel.PreviewRoot.Properties["Height"]);
        Assert.AreEqual("40", viewModel.PreviewRoot!.Properties["MinWidth"]);
        Assert.AreEqual("120", viewModel.PreviewRoot.Properties["MaxWidth"]);
        Assert.AreEqual("20", viewModel.PreviewRoot.Properties["MinHeight"]);
        Assert.AreEqual("80", viewModel.PreviewRoot.Properties["MaxHeight"]);

        for (int index = 0; index < 6; index++)
        {
            viewModel.UndoCommand.Execute(null);
        }

        Assert.IsFalse(viewModel.PreviewRoot!.Properties.ContainsKey("Width"));
        Assert.IsFalse(viewModel.PreviewRoot.Properties.ContainsKey("Height"));
        Assert.IsFalse(viewModel.PreviewRoot!.Properties.ContainsKey("MinWidth"));
        Assert.IsFalse(viewModel.PreviewRoot.Properties.ContainsKey("MaxWidth"));
        Assert.IsFalse(viewModel.PreviewRoot.Properties.ContainsKey("MinHeight"));
        Assert.IsFalse(viewModel.PreviewRoot.Properties.ContainsKey("MaxHeight"));

        for (int index = 0; index < 6; index++)
        {
            viewModel.RedoCommand.Execute(null);
        }

        Assert.AreEqual("200", viewModel.PreviewRoot!.Properties["Width"]);
        Assert.AreEqual("60", viewModel.PreviewRoot.Properties["Height"]);
        Assert.AreEqual("40", viewModel.PreviewRoot!.Properties["MinWidth"]);
        Assert.AreEqual("120", viewModel.PreviewRoot.Properties["MaxWidth"]);
        Assert.AreEqual("20", viewModel.PreviewRoot.Properties["MinHeight"]);
        Assert.AreEqual("80", viewModel.PreviewRoot.Properties["MaxHeight"]);
    }

    [TestMethod]
    public async Task WrapPanelLayoutOptions_AreEditableAndUndoable()
    {
        AxamlDesignerViewModel viewModel = new();
        viewModel.LoadMarkup("<WrapPanel xmlns='https://github.com/avaloniaui' />");
        viewModel.SelectedNode = viewModel.PreviewRoot;

        foreach ((string propertyName, string value) in new[]
        {
            ("ItemSpacing", "4"),
            ("LineSpacing", "6"),
            ("ItemWidth", "40"),
            ("ItemHeight", "20"),
            ("ItemsAlignment", "Center"),
        })
        {
            var property = viewModel.PropertyProvider.Properties.Single(row => row.Name == propertyName);
            Assert.IsTrue(property.TrySetValue(value), propertyName);
            await viewModel.PropertyProvider.ApplyAsync();
        }

        foreach ((string property, string expected) in new[]
        {
            ("ItemSpacing", "4"),
            ("LineSpacing", "6"),
            ("ItemWidth", "40"),
            ("ItemHeight", "20"),
            ("ItemsAlignment", "Center"),
        })
        {
            Assert.AreEqual(expected, viewModel.PreviewRoot!.Properties[property], property);
        }

        for (int index = 0; index < 5; index++)
        {
            viewModel.UndoCommand.Execute(null);
        }

        foreach (string property in new[] { "ItemSpacing", "LineSpacing", "ItemWidth", "ItemHeight", "ItemsAlignment" })
        {
            Assert.IsFalse(viewModel.PreviewRoot!.Properties.ContainsKey(property), property);
        }

        for (int index = 0; index < 5; index++)
        {
            viewModel.RedoCommand.Execute(null);
        }

        Assert.AreEqual("Center", viewModel.PreviewRoot!.Properties["ItemsAlignment"]);
        Assert.AreEqual("4", viewModel.PreviewRoot.Properties["ItemSpacing"]);
    }

    [TestMethod]
    public async Task StackPanelSpacing_IsEditableAndUndoable()
    {
        AxamlDesignerViewModel viewModel = new();
        viewModel.LoadMarkup("<StackPanel xmlns='https://github.com/avaloniaui' Spacing='2' />");
        viewModel.SelectedNode = viewModel.PreviewRoot;
        var spacing = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "Spacing");

        Assert.IsFalse(spacing.TrySetValue("-1"));
        Assert.IsTrue(spacing.TrySetValue("8"));
        await viewModel.PropertyProvider.ApplyAsync();
        Assert.AreEqual("8", viewModel.PreviewRoot!.Properties["Spacing"]);
        viewModel.UndoCommand.Execute(null);
        Assert.AreEqual("2", viewModel.PreviewRoot!.Properties["Spacing"]);
        viewModel.RedoCommand.Execute(null);
        Assert.AreEqual("8", viewModel.PreviewRoot!.Properties["Spacing"]);
    }

    [TestMethod]
    public async Task DockPanelLayoutOptions_AreEditableAndUndoable()
    {
        AxamlDesignerViewModel viewModel = new();
        viewModel.LoadMarkup("<DockPanel xmlns='https://github.com/avaloniaui' />");
        viewModel.SelectedNode = viewModel.PreviewRoot;

        foreach ((string propertyName, string value) in new[]
        {
            ("HorizontalSpacing", "4"),
            ("VerticalSpacing", "6"),
            ("LastChildFill", "False"),
        })
        {
            var property = viewModel.PropertyProvider.Properties.Single(row => row.Name == propertyName);
            Assert.IsTrue(property.TrySetValue(value), propertyName);
            await viewModel.PropertyProvider.ApplyAsync();
        }

        Assert.AreEqual("4", viewModel.PreviewRoot!.Properties["HorizontalSpacing"]);
        Assert.AreEqual("6", viewModel.PreviewRoot.Properties["VerticalSpacing"]);
        Assert.AreEqual("False", viewModel.PreviewRoot.Properties["LastChildFill"]);
        for (int index = 0; index < 3; index++)
        {
            viewModel.UndoCommand.Execute(null);
        }
        Assert.IsFalse(viewModel.PreviewRoot!.Properties.ContainsKey("HorizontalSpacing"));
        Assert.IsFalse(viewModel.PreviewRoot.Properties.ContainsKey("VerticalSpacing"));
        Assert.IsFalse(viewModel.PreviewRoot.Properties.ContainsKey("LastChildFill"));
        for (int index = 0; index < 3; index++)
        {
            viewModel.RedoCommand.Execute(null);
        }
        Assert.AreEqual("False", viewModel.PreviewRoot!.Properties["LastChildFill"]);
    }

    [TestMethod]
    public async Task UniformGridSpacing_IsEditableAndUndoable()
    {
        AxamlDesignerViewModel viewModel = new();
        viewModel.LoadMarkup("<UniformGrid xmlns='https://github.com/avaloniaui' />");
        viewModel.SelectedNode = viewModel.PreviewRoot;

        foreach ((string propertyName, string value) in new[]
        {
            ("RowSpacing", "3"),
            ("ColumnSpacing", "5"),
        })
        {
            var property = viewModel.PropertyProvider.Properties.Single(row => row.Name == propertyName);
            Assert.IsTrue(property.TrySetValue(value), propertyName);
            await viewModel.PropertyProvider.ApplyAsync();
        }

        Assert.AreEqual("3", viewModel.PreviewRoot!.Properties["RowSpacing"]);
        Assert.AreEqual("5", viewModel.PreviewRoot.Properties["ColumnSpacing"]);
        viewModel.UndoCommand.Execute(null);
        viewModel.UndoCommand.Execute(null);
        Assert.IsFalse(viewModel.PreviewRoot!.Properties.ContainsKey("RowSpacing"));
        Assert.IsFalse(viewModel.PreviewRoot.Properties.ContainsKey("ColumnSpacing"));
        viewModel.RedoCommand.Execute(null);
        viewModel.RedoCommand.Execute(null);
        Assert.AreEqual("3", viewModel.PreviewRoot!.Properties["RowSpacing"]);
        Assert.AreEqual("5", viewModel.PreviewRoot.Properties["ColumnSpacing"]);
    }

    [TestMethod]
    public async Task GridSpacing_IsEditableAndUndoable()
    {
        AxamlDesignerViewModel viewModel = new();
        viewModel.LoadMarkup("<Grid xmlns='https://github.com/avaloniaui' />");
        viewModel.SelectedNode = viewModel.PreviewRoot;

        foreach ((string propertyName, string value) in new[]
        {
            ("RowSpacing", "2"),
            ("ColumnSpacing", "4"),
            ("ShowGridLines", "True"),
        })
        {
            var property = viewModel.PropertyProvider.Properties.Single(row => row.Name == propertyName);
            Assert.IsTrue(property.TrySetValue(value), propertyName);
            await viewModel.PropertyProvider.ApplyAsync();
        }

        Assert.AreEqual("2", viewModel.PreviewRoot!.Properties["RowSpacing"]);
        Assert.AreEqual("4", viewModel.PreviewRoot.Properties["ColumnSpacing"]);
        Assert.AreEqual("True", viewModel.PreviewRoot.Properties["ShowGridLines"]);
        for (int index = 0; index < 3; index++)
        {
            viewModel.UndoCommand.Execute(null);
        }
        Assert.IsFalse(viewModel.PreviewRoot!.Properties.ContainsKey("RowSpacing"));
        Assert.IsFalse(viewModel.PreviewRoot.Properties.ContainsKey("ColumnSpacing"));
        Assert.IsFalse(viewModel.PreviewRoot.Properties.ContainsKey("ShowGridLines"));
        for (int index = 0; index < 3; index++)
        {
            viewModel.RedoCommand.Execute(null);
        }
        Assert.AreEqual("2", viewModel.PreviewRoot!.Properties["RowSpacing"]);
        Assert.AreEqual("4", viewModel.PreviewRoot.Properties["ColumnSpacing"]);
        Assert.AreEqual("True", viewModel.PreviewRoot.Properties["ShowGridLines"]);
    }

    [TestMethod]
    public async Task ClipToBounds_IsEditableAndUndoable()
    {
        AxamlDesignerViewModel viewModel = new();
        viewModel.LoadMarkup("<Button xmlns='https://github.com/avaloniaui' Content='Clipped' />");
        viewModel.SelectedNode = viewModel.PreviewRoot;
        var clipToBounds = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "ClipToBounds");
        Assert.IsTrue(clipToBounds.TrySetValue("True"));
        await viewModel.PropertyProvider.ApplyAsync();

        Assert.AreEqual("True", viewModel.PreviewRoot!.Properties["ClipToBounds"]);
        viewModel.UndoCommand.Execute(null);
        Assert.IsFalse(viewModel.PreviewRoot!.Properties.ContainsKey("ClipToBounds"));
        viewModel.RedoCommand.Execute(null);
        Assert.AreEqual("True", viewModel.PreviewRoot!.Properties["ClipToBounds"]);
    }

    [TestMethod]
    public async Task IsVisible_IsEditableAndUndoable()
    {
        AxamlDesignerViewModel viewModel = new();
        viewModel.LoadMarkup("<Button xmlns='https://github.com/avaloniaui' Content='Visibility' />");
        viewModel.SelectedNode = viewModel.PreviewRoot;
        var isVisible = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "IsVisible");
        Assert.IsTrue(isVisible.TrySetValue("False"));
        await viewModel.PropertyProvider.ApplyAsync();

        Assert.AreEqual("False", viewModel.PreviewRoot!.Properties["IsVisible"]);
        viewModel.UndoCommand.Execute(null);
        Assert.IsFalse(viewModel.PreviewRoot!.Properties.ContainsKey("IsVisible"));
        viewModel.RedoCommand.Execute(null);
        Assert.AreEqual("False", viewModel.PreviewRoot!.Properties["IsVisible"]);
    }

    [TestMethod]
    public async Task ButtonContentAlignment_IsEditableAndUndoable()
    {
        AxamlDesignerViewModel viewModel = new();
        viewModel.LoadMarkup("<Button xmlns='https://github.com/avaloniaui' Content='Aligned' />");
        viewModel.SelectedNode = viewModel.PreviewRoot;
        var horizontal = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "HorizontalContentAlignment");
        var vertical = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "VerticalContentAlignment");

        Assert.IsTrue(horizontal.TrySetValue("Right"));
        await viewModel.PropertyProvider.ApplyAsync();
        Assert.IsTrue(vertical.TrySetValue("Top"));
        await viewModel.PropertyProvider.ApplyAsync();
        Assert.AreEqual("Right", viewModel.PreviewRoot!.Properties["HorizontalContentAlignment"]);
        Assert.AreEqual("Top", viewModel.PreviewRoot.Properties["VerticalContentAlignment"]);

        viewModel.UndoCommand.Execute(null);
        Assert.IsFalse(viewModel.PreviewRoot!.Properties.ContainsKey("VerticalContentAlignment"));
        viewModel.UndoCommand.Execute(null);
        Assert.IsFalse(viewModel.PreviewRoot.Properties.ContainsKey("HorizontalContentAlignment"));
        viewModel.RedoCommand.Execute(null);
        viewModel.RedoCommand.Execute(null);
        Assert.AreEqual("Right", viewModel.PreviewRoot!.Properties["HorizontalContentAlignment"]);
        Assert.AreEqual("Top", viewModel.PreviewRoot.Properties["VerticalContentAlignment"]);
    }

    [TestMethod]
    public async Task TextBoxLineLimits_AreEditableAndUndoable()
    {
        AxamlDesignerViewModel viewModel = new();
        viewModel.LoadMarkup("<TextBox xmlns='https://github.com/avaloniaui' Text='Multiline' />");
        viewModel.SelectedNode = viewModel.PreviewRoot;
        var minLines = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "MinLines");
        var maxLines = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "MaxLines");

        Assert.IsTrue(minLines.TrySetValue("2"));
        await viewModel.PropertyProvider.ApplyAsync();
        Assert.IsTrue(maxLines.TrySetValue("5"));
        await viewModel.PropertyProvider.ApplyAsync();
        Assert.AreEqual("2", viewModel.PreviewRoot!.Properties["MinLines"]);
        Assert.AreEqual("5", viewModel.PreviewRoot.Properties["MaxLines"]);

        viewModel.UndoCommand.Execute(null);
        Assert.IsFalse(viewModel.PreviewRoot!.Properties.ContainsKey("MaxLines"));
        viewModel.UndoCommand.Execute(null);
        Assert.IsFalse(viewModel.PreviewRoot.Properties.ContainsKey("MinLines"));
        viewModel.RedoCommand.Execute(null);
        viewModel.RedoCommand.Execute(null);
        Assert.AreEqual("2", viewModel.PreviewRoot!.Properties["MinLines"]);
        Assert.AreEqual("5", viewModel.PreviewRoot.Properties["MaxLines"]);
    }

    [TestMethod]
    public async Task TextBoxLineHeightAndFloatingPlaceholder_AreEditableAndUndoable()
    {
        AxamlDesignerViewModel viewModel = new();
        viewModel.LoadMarkup("<TextBox xmlns='https://github.com/avaloniaui' Text='Placeholder' />");
        viewModel.SelectedNode = viewModel.PreviewRoot;
        var lineHeight = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "LineHeight");
        var floatingPlaceholder = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "UseFloatingPlaceholder");

        Assert.IsTrue(lineHeight.TrySetValue("22"));
        await viewModel.PropertyProvider.ApplyAsync();
        Assert.IsTrue(floatingPlaceholder.TrySetValue("True"));
        await viewModel.PropertyProvider.ApplyAsync();
        Assert.AreEqual("22", viewModel.PreviewRoot!.Properties["LineHeight"]);
        Assert.AreEqual("True", viewModel.PreviewRoot.Properties["UseFloatingPlaceholder"]);

        viewModel.UndoCommand.Execute(null);
        Assert.IsFalse(viewModel.PreviewRoot!.Properties.ContainsKey("UseFloatingPlaceholder"));
        viewModel.UndoCommand.Execute(null);
        Assert.IsFalse(viewModel.PreviewRoot.Properties.ContainsKey("LineHeight"));
        viewModel.RedoCommand.Execute(null);
        viewModel.RedoCommand.Execute(null);
        Assert.AreEqual("22", viewModel.PreviewRoot!.Properties["LineHeight"]);
        Assert.AreEqual("True", viewModel.PreviewRoot.Properties["UseFloatingPlaceholder"]);
    }

    [TestMethod]
    public async Task TextBoxPlaceholderForeground_IsEditableAndUndoable()
    {
        AxamlDesignerViewModel viewModel = new();
        viewModel.LoadMarkup("<TextBox xmlns='https://github.com/avaloniaui' PlaceholderText='Enter a value' />");
        viewModel.SelectedNode = viewModel.PreviewRoot;
        var placeholderForeground = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "PlaceholderForeground");

        Assert.IsTrue(placeholderForeground.TrySetValue("DarkGray"));
        await viewModel.PropertyProvider.ApplyAsync();
        Assert.AreEqual("DarkGray", viewModel.PreviewRoot!.Properties["PlaceholderForeground"]);
        viewModel.UndoCommand.Execute(null);
        Assert.IsFalse(viewModel.PreviewRoot!.Properties.ContainsKey("PlaceholderForeground"));
        viewModel.RedoCommand.Execute(null);
        Assert.AreEqual("DarkGray", viewModel.PreviewRoot!.Properties["PlaceholderForeground"]);
    }

    [TestMethod]
    public async Task FlowDirection_IsEditableAndUndoable()
    {
        AxamlDesignerViewModel viewModel = new();
        viewModel.LoadMarkup("<TextBlock xmlns='https://github.com/avaloniaui' Text='Direction' />");
        viewModel.SelectedNode = viewModel.PreviewRoot;
        var flowDirection = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "FlowDirection");

        Assert.IsTrue(flowDirection.TrySetValue("RightToLeft"));
        await viewModel.PropertyProvider.ApplyAsync();
        Assert.AreEqual("RightToLeft", viewModel.PreviewRoot!.Properties["FlowDirection"]);
        viewModel.UndoCommand.Execute(null);
        Assert.IsFalse(viewModel.PreviewRoot!.Properties.ContainsKey("FlowDirection"));
        viewModel.RedoCommand.Execute(null);
        Assert.AreEqual("RightToLeft", viewModel.PreviewRoot!.Properties["FlowDirection"]);
    }

    [TestMethod]
    public async Task UseLayoutRounding_IsEditableAndUndoable()
    {
        AxamlDesignerViewModel viewModel = new();
        viewModel.LoadMarkup("<Button xmlns='https://github.com/avaloniaui' Content='Pixel aligned' />");
        viewModel.SelectedNode = viewModel.PreviewRoot;
        var useLayoutRounding = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "UseLayoutRounding");

        Assert.IsTrue(useLayoutRounding.TrySetValue("True"));
        await viewModel.PropertyProvider.ApplyAsync();
        Assert.AreEqual("True", viewModel.PreviewRoot!.Properties["UseLayoutRounding"]);
        viewModel.UndoCommand.Execute(null);
        Assert.IsFalse(viewModel.PreviewRoot!.Properties.ContainsKey("UseLayoutRounding"));
        viewModel.RedoCommand.Execute(null);
        Assert.AreEqual("True", viewModel.PreviewRoot!.Properties["UseLayoutRounding"]);
    }

    [TestMethod]
    public async Task ScrollViewerVisibility_IsEditableAndUndoable()
    {
        AxamlDesignerViewModel viewModel = new();
        viewModel.LoadMarkup("<ScrollViewer xmlns='https://github.com/avaloniaui' />");
        viewModel.SelectedNode = viewModel.PreviewRoot;
        foreach ((string propertyName, string value) in new[]
        {
            ("HorizontalScrollBarVisibility", "Hidden"),
            ("VerticalScrollBarVisibility", "Disabled"),
        })
        {
            var property = viewModel.PropertyProvider.Properties.Single(row => row.Name == propertyName);
            Assert.IsTrue(property.TrySetValue(value), propertyName);
            await viewModel.PropertyProvider.ApplyAsync();
        }

        Assert.AreEqual("Hidden", viewModel.PreviewRoot!.Properties["HorizontalScrollBarVisibility"]);
        Assert.AreEqual("Disabled", viewModel.PreviewRoot.Properties["VerticalScrollBarVisibility"]);
        viewModel.UndoCommand.Execute(null);
        viewModel.UndoCommand.Execute(null);
        Assert.IsFalse(viewModel.PreviewRoot!.Properties.ContainsKey("HorizontalScrollBarVisibility"));
        Assert.IsFalse(viewModel.PreviewRoot.Properties.ContainsKey("VerticalScrollBarVisibility"));
        viewModel.RedoCommand.Execute(null);
        viewModel.RedoCommand.Execute(null);
        Assert.AreEqual("Hidden", viewModel.PreviewRoot!.Properties["HorizontalScrollBarVisibility"]);
        Assert.AreEqual("Disabled", viewModel.PreviewRoot.Properties["VerticalScrollBarVisibility"]);
    }

    [TestMethod]
    public async Task ViewboxStretchDirection_IsEditableAndUndoable()
    {
        AxamlDesignerViewModel viewModel = new();
        viewModel.LoadMarkup("<Viewbox xmlns='https://github.com/avaloniaui'><TextBlock Text='Scale me' /></Viewbox>");
        viewModel.SelectedNode = viewModel.PreviewRoot;
        var direction = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "StretchDirection");
        Assert.IsTrue(direction.TrySetValue("DownOnly"));
        await viewModel.PropertyProvider.ApplyAsync();

        Assert.AreEqual("DownOnly", viewModel.PreviewRoot!.Properties["StretchDirection"]);
        viewModel.UndoCommand.Execute(null);
        Assert.IsFalse(viewModel.PreviewRoot!.Properties.ContainsKey("StretchDirection"));
        viewModel.RedoCommand.Execute(null);
        Assert.AreEqual("DownOnly", viewModel.PreviewRoot!.Properties["StretchDirection"]);
    }

    [TestMethod]
    public async Task TextBlockAndTextBoxOpacity_AreEditableAndUndoable()
    {
        AxamlDesignerViewModel viewModel = new();
        viewModel.LoadMarkup(
            "<StackPanel xmlns='https://github.com/avaloniaui'><TextBlock Opacity='1' /><TextBox Opacity='1' /></StackPanel>");

        viewModel.SelectedNode = viewModel.PreviewRoot!.Children[0];
        var textBlockOpacity = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "Opacity");
        Assert.IsTrue(textBlockOpacity.TrySetValue("0.5"));
        await viewModel.PropertyProvider.ApplyAsync();

        viewModel.SelectedNode = viewModel.PreviewRoot!.Children[1];
        var textBoxOpacity = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "Opacity");
        Assert.IsTrue(textBoxOpacity.TrySetValue("0.75"));
        await viewModel.PropertyProvider.ApplyAsync();

        Assert.AreEqual("0.5", viewModel.PreviewRoot!.Children[0].Properties["Opacity"]);
        Assert.AreEqual("0.75", viewModel.PreviewRoot.Children[1].Properties["Opacity"]);
        viewModel.UndoCommand.Execute(null);
        Assert.AreEqual("1", viewModel.PreviewRoot!.Children[1].Properties["Opacity"]);
        viewModel.UndoCommand.Execute(null);
        Assert.AreEqual("1", viewModel.PreviewRoot!.Children[0].Properties["Opacity"]);
        viewModel.RedoCommand.Execute(null);
        viewModel.RedoCommand.Execute(null);
        Assert.AreEqual("0.5", viewModel.PreviewRoot!.Children[0].Properties["Opacity"]);
        Assert.AreEqual("0.75", viewModel.PreviewRoot.Children[1].Properties["Opacity"]);
    }

    [TestMethod]
    public async Task TextBlockMaxLines_IsEditableAndUndoable()
    {
        AxamlDesignerViewModel viewModel = new();
        viewModel.LoadMarkup("<TextBlock xmlns='https://github.com/avaloniaui' Text='Two lines' MaxLines='0' />");
        viewModel.SelectedNode = viewModel.PreviewRoot;
        var maxLines = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "MaxLines");

        Assert.IsTrue(maxLines.TrySetValue("2"));
        await viewModel.PropertyProvider.ApplyAsync();
        Assert.AreEqual("2", viewModel.PreviewRoot!.Properties["MaxLines"]);
        viewModel.UndoCommand.Execute(null);
        Assert.AreEqual("0", viewModel.PreviewRoot.Properties["MaxLines"]);
        viewModel.RedoCommand.Execute(null);
        Assert.AreEqual("2", viewModel.PreviewRoot.Properties["MaxLines"]);
    }

    [TestMethod]
    public async Task TextBlockLineHeight_IsEditableAndUndoable()
    {
        AxamlDesignerViewModel viewModel = new();
        viewModel.LoadMarkup("<TextBlock xmlns='https://github.com/avaloniaui' LineHeight='20' />");
        viewModel.SelectedNode = viewModel.PreviewRoot;
        var lineHeight = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "LineHeight");

        Assert.IsTrue(lineHeight.TrySetValue("28"));
        await viewModel.PropertyProvider.ApplyAsync();
        Assert.AreEqual("28", viewModel.PreviewRoot!.Properties["LineHeight"]);
        viewModel.UndoCommand.Execute(null);
        Assert.AreEqual("20", viewModel.PreviewRoot!.Properties["LineHeight"]);
        viewModel.RedoCommand.Execute(null);
        Assert.AreEqual("28", viewModel.PreviewRoot!.Properties["LineHeight"]);
    }

    [TestMethod]
    public async Task TextBlockTextTrimming_IsEditableAndUndoable()
    {
        AxamlDesignerViewModel viewModel = new();
        viewModel.LoadMarkup("<TextBlock xmlns='https://github.com/avaloniaui' Text='Truncate me' TextTrimming='None' />");
        viewModel.SelectedNode = viewModel.PreviewRoot;
        var trimming = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "TextTrimming");

        Assert.IsTrue(trimming.TrySetValue("CharacterEllipsis"));
        await viewModel.PropertyProvider.ApplyAsync();
        Assert.AreEqual("CharacterEllipsis", viewModel.PreviewRoot!.Properties["TextTrimming"]);
        viewModel.UndoCommand.Execute(null);
        Assert.AreEqual("None", viewModel.PreviewRoot.Properties["TextTrimming"]);
        viewModel.RedoCommand.Execute(null);
        Assert.AreEqual("CharacterEllipsis", viewModel.PreviewRoot.Properties["TextTrimming"]);
    }

    [TestMethod]
    public async Task TextBoxAcceptsTab_IsEditableAndUndoable()
    {
        AxamlDesignerViewModel viewModel = new();
        viewModel.LoadMarkup("<TextBox xmlns='https://github.com/avaloniaui' AcceptsTab='False' />");
        viewModel.SelectedNode = viewModel.PreviewRoot;
        var acceptsTab = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "AcceptsTab");

        Assert.IsTrue(acceptsTab.TrySetValue(true));
        await viewModel.PropertyProvider.ApplyAsync();
        Assert.AreEqual("True", viewModel.PreviewRoot!.Properties["AcceptsTab"]);
        viewModel.UndoCommand.Execute(null);
        Assert.AreEqual("False", viewModel.PreviewRoot.Properties["AcceptsTab"]);
        viewModel.RedoCommand.Execute(null);
        Assert.AreEqual("True", viewModel.PreviewRoot.Properties["AcceptsTab"]);
    }

    [TestMethod]
    public void StructureCommands_UpdateTheSharedMarkupSelectionAndUndo()
    {
        const string markup = "<StackPanel xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>"
            + "<TextBox x:Name='First' /><Button x:Name='Second' /></StackPanel>";
        AxamlDesignerViewModel viewModel = new();
        string? published = null;
        viewModel.MarkupChanged += (_, args) => published = args.Markup;
        viewModel.LoadMarkup(markup);
        Assert.AreSame(viewModel.PreviewRoot, viewModel.StructureRoots.Single());
        viewModel.SelectedNode = viewModel.PreviewRoot!.Children[1];

        Assert.IsTrue(viewModel.MoveStructureEarlierCommand.CanExecute(null));
        viewModel.MoveStructureEarlierCommand.Execute(null);
        Assert.AreEqual("Second", viewModel.SelectedNode!.StableName);
        Assert.AreEqual("Second", viewModel.PreviewRoot!.Children[0].StableName);
        Assert.IsNotNull(published);
        viewModel.RemoveStructureCommand.Execute(null);
        Assert.AreEqual("StackPanel", viewModel.SelectedNode!.TypeName);
        Assert.AreEqual("First", viewModel.PreviewRoot!.Children.Single().StableName);
        viewModel.UndoCommand.Execute(null);
        Assert.AreEqual("Second", viewModel.SelectedNode!.StableName);
        viewModel.UndoCommand.Execute(null);
        Assert.AreEqual(markup, published);
        Assert.AreEqual("Second", viewModel.SelectedNode!.StableName);
        viewModel.RedoCommand.Execute(null);
        Assert.AreEqual("Second", viewModel.PreviewRoot!.Children[0].StableName);
    }

    [TestMethod]
    public void TabPaletteCommandsBuildAValidTabHierarchy()
    {
        AxamlDesignerViewModel viewModel = new();
        viewModel.LoadMarkup("<StackPanel />");
        viewModel.SelectedNode = viewModel.PreviewRoot;

        viewModel.AddTabControlCommand.Execute(null);
        AxamlPreviewNode tabControl = viewModel.PreviewRoot!.Children.Single();
        Assert.AreEqual("TabControl", tabControl.TypeName);
        viewModel.SelectedNode = tabControl;
        viewModel.AddTabItemCommand.Execute(null);
        AxamlPreviewNode tabItem = viewModel.PreviewRoot.Children.Single().Children.Single();
        Assert.AreEqual("TabItem", tabItem.TypeName);
        viewModel.SelectedNode = tabItem;
        viewModel.AddTextBlockCommand.Execute(null);

        AxamlPreviewNode content = viewModel.PreviewRoot.Children.Single().Children.Single();
        Assert.AreEqual("TextBlock", content.Children.Single().TypeName);
        Assert.IsTrue(viewModel.PreviewRoot.IsSupported);
        viewModel.UndoCommand.Execute(null);
        Assert.AreEqual(0, viewModel.PreviewRoot.Children.Single().Children.Single().Children.Count);
    }

    [TestMethod]
    public void MenuPaletteCommandsBuildSafeNestedItems()
    {
        AxamlDesignerViewModel viewModel = new();
        viewModel.LoadMarkup("<StackPanel />");
        viewModel.SelectedNode = viewModel.PreviewRoot;

        viewModel.AddMenuCommand.Execute(null);
        AxamlPreviewNode menu = viewModel.PreviewRoot!.Children.Single();
        viewModel.SelectedNode = menu;
        viewModel.AddMenuItemCommand.Execute(null);
        AxamlPreviewNode menuItem = viewModel.PreviewRoot.Children.Single().Children.Single();
        viewModel.SelectedNode = menuItem;
        viewModel.AddMenuItemCommand.Execute(null);
        viewModel.AddSeparatorCommand.Execute(null);

        AxamlPreviewNode selectedMenuItem = viewModel.PreviewRoot.Children.Single().Children.Single();
        Assert.AreEqual(2, selectedMenuItem.Children.Count);
        Assert.AreEqual("MenuItem", selectedMenuItem.Children[0].TypeName);
        Assert.AreEqual("Separator", selectedMenuItem.Children[1].TypeName);
        Assert.IsTrue(viewModel.PreviewRoot.IsSupported);
        viewModel.UndoCommand.Execute(null);
        Assert.AreEqual(1, viewModel.PreviewRoot.Children.Single().Children.Single().Children.Count);
    }

    [TestMethod]
    public void ContextMenuPaletteCommand_AddsMenuItemsAndUndoRestoresTheSource()
    {
        const string markup = "<StackPanel><Button Content='Save' /></StackPanel>";
        AxamlDesignerViewModel viewModel = new();
        string? publishedMarkup = null;
        viewModel.MarkupChanged += (_, args) => publishedMarkup = args.Markup;
        viewModel.LoadMarkup(markup);
        viewModel.SelectedNode = viewModel.PreviewRoot!.Children.Single();

        viewModel.AddContextMenuCommand.Execute(null);

        AxamlPreviewNode button = viewModel.PreviewRoot!.Children.Single();
        AxamlPreviewNode contextMenu = button.Children.Single();
        Assert.AreEqual("ContextMenu", contextMenu.TypeName);
        Assert.IsTrue(contextMenu.IsSupported);
        Assert.AreEqual("ContextMenu", viewModel.SelectedNode!.TypeName);
        Assert.IsTrue(viewModel.SelectedNode.CanAppendChildren);
        StringAssert.Contains(publishedMarkup!, "<Button.ContextMenu>");

        viewModel.AddMenuItemCommand.Execute(null);
        Assert.AreEqual("MenuItem", viewModel.SelectedNode!.Children.Single().TypeName);
        Assert.IsTrue(viewModel.PreviewRoot.IsSupported);

        viewModel.UndoCommand.Execute(null);
        Assert.AreEqual(0, viewModel.PreviewRoot!.Children.Single().Children.Single().Children.Count);
        viewModel.UndoCommand.Execute(null);
        Assert.AreEqual(markup, publishedMarkup);
        viewModel.RedoCommand.Execute(null);
        viewModel.RedoCommand.Execute(null);
        Assert.AreEqual(
            "MenuItem",
            viewModel.PreviewRoot!.Children.Single().Children.Single().Children.Single().TypeName);

        const string textBoxMarkup = "<StackPanel><TextBox Text='Search' /></StackPanel>";
        viewModel.LoadMarkup(textBoxMarkup);
        viewModel.SelectedNode = viewModel.PreviewRoot!.Children.Single();
        viewModel.AddContextMenuCommand.Execute(null);
        AxamlPreviewNode textBox = viewModel.PreviewRoot!.Children.Single();
        Assert.AreEqual("TextBox", textBox.TypeName);
        Assert.AreEqual("ContextMenu", textBox.Children.Single().TypeName);
        StringAssert.Contains(publishedMarkup!, "<TextBox.ContextMenu>");
        viewModel.UndoCommand.Execute(null);
        Assert.AreEqual(textBoxMarkup, publishedMarkup);

        const string tooltipMarkup = "<StackPanel><Button Content='Save' /></StackPanel>";
        viewModel.LoadMarkup(tooltipMarkup);
        viewModel.SelectedNode = viewModel.PreviewRoot!.Children.Single();
        viewModel.AddToolTipCommand.Execute(null);
        AxamlPreviewNode tooltip = viewModel.PreviewRoot!.Children.Single().Children.Single();
        Assert.AreEqual("ToolTip", tooltip.TypeName);
        Assert.AreEqual("TextBlock", tooltip.Children.Single().TypeName);
        StringAssert.Contains(publishedMarkup!, "<Button.ToolTip>");
        viewModel.UndoCommand.Execute(null);
        Assert.AreEqual(tooltipMarkup, publishedMarkup);

        const string boundToolTipMarkup = "<Button ToolTip='{Binding Help}' />";
        viewModel.LoadMarkup(boundToolTipMarkup);
        publishedMarkup = null;
        viewModel.SelectedNode = viewModel.PreviewRoot;
        viewModel.AddToolTipCommand.Execute(null);
        Assert.IsNull(publishedMarkup);
        StringAssert.Contains(viewModel.StatusText, "supported control");

        const string boundMarkup = "<Button ContextMenu='{Binding Actions}' />";
        viewModel.LoadMarkup(boundMarkup);
        publishedMarkup = null;
        viewModel.SelectedNode = viewModel.PreviewRoot;
        viewModel.AddContextMenuCommand.Execute(null);
        Assert.IsNull(publishedMarkup);
        StringAssert.Contains(viewModel.StatusText, "supported control");
    }

    [TestMethod]
    public void ToolTipPaletteCommandsAllowSafeContentReplacement()
    {
        AxamlDesignerViewModel viewModel = new();
        viewModel.LoadMarkup("<StackPanel><Button Content='Save' /></StackPanel>");
        viewModel.SelectedNode = viewModel.PreviewRoot!.Children.Single();
        viewModel.AddToolTipCommand.Execute(null);

        AxamlPreviewNode toolTip = viewModel.SelectedNode!;
        AxamlPreviewNode textBlock = toolTip.Children.Single();
        viewModel.SelectedNode = textBlock;
        Assert.IsTrue(viewModel.RemoveStructureCommand.CanExecute(null));
        viewModel.RemoveStructureCommand.Execute(null);

        Assert.AreEqual("ToolTip", viewModel.SelectedNode!.TypeName);
        Assert.IsTrue(viewModel.SelectedNode.CanAppendChildren);
        viewModel.AddStackPanelCommand.Execute(null);
        AxamlPreviewNode stackPanel = viewModel.SelectedNode!.Children.Single();
        viewModel.SelectedNode = stackPanel;
        viewModel.AddTextBlockCommand.Execute(null);
        viewModel.AddTextBlockCommand.Execute(null);
        Assert.AreEqual(2, viewModel.PreviewRoot!.Children.Single().Children.Single().Children.Single().Children.Count);

        viewModel.UndoCommand.Execute(null);
        Assert.AreEqual(1, viewModel.PreviewRoot!.Children.Single().Children.Single().Children.Single().Children.Count);
        viewModel.RedoCommand.Execute(null);
        Assert.AreEqual(
            2,
            viewModel.PreviewRoot!.Children.Single().Children.Single().Children.Single().Children.Count);
    }

    [TestMethod]
    public void StructureCommands_BlockDraftsFromOtherTargetsAndProtectUndo()
    {
        const string markup = "<StackPanel><TextBox Width='100' /><Button /></StackPanel>";
        AxamlDesignerViewModel viewModel = new();
        string? published = null;
        viewModel.MarkupChanged += (_, args) => published = args.Markup;
        viewModel.LoadMarkup(markup);
        viewModel.SelectedNode = viewModel.PreviewRoot;
        viewModel.AddTextBlockCommand.Execute(null);
        string beforeDraft = published!;
        viewModel.SelectedNode = viewModel.PreviewRoot!.Children[0];
        AxamlPropertyRowViewModel width = viewModel.PropertyRows.Single(row => row.Name == "Width");
        Assert.IsFalse(width.TrySetValue("-2"));
        viewModel.SelectedNode = viewModel.PreviewRoot.Children[1];

        Assert.AreEqual(1, viewModel.UnappliedDrafts.Count);
        Assert.IsFalse(viewModel.RemoveStructureCommand.CanExecute(null));
        Assert.IsFalse(viewModel.MoveStructureEarlierCommand.CanExecute(null));
        Assert.IsFalse(viewModel.UndoCommand.CanExecute(null));
        viewModel.RemoveStructureCommand.Execute(null);
        viewModel.MoveStructureEarlierCommand.Execute(null);
        viewModel.UndoCommand.Execute(null);
        Assert.AreEqual(beforeDraft, published);
        Assert.AreEqual("-2", viewModel.UnappliedDrafts.Single().Value);
        StringAssert.Contains(viewModel.StructureAvailabilityText, "all property drafts");

        viewModel.SelectedNode = viewModel.PreviewRoot;
        viewModel.AddButtonCommand.Execute(null);
        Assert.AreEqual(beforeDraft, published);
        viewModel.DiscardStoredDraft(viewModel.UnappliedDrafts.Single());
        Assert.IsTrue(viewModel.UndoCommand.CanExecute(null));
        viewModel.UndoCommand.Execute(null);
        Assert.AreEqual(markup, published);
    }

    [TestMethod]
    public void StructureCommands_ExplainProtectedCommentsWithoutChangingMarkup()
    {
        const string markup = "<StackPanel><!-- keep --><Button /><TextBlock /></StackPanel>";
        AxamlDesignerViewModel viewModel = new();
        int changes = 0;
        viewModel.MarkupChanged += (_, _) => changes++;
        viewModel.LoadMarkup(markup);
        viewModel.SelectedNode = viewModel.PreviewRoot!.Children[0];

        viewModel.RemoveStructureCommand.Execute(null);

        Assert.AreEqual(0, changes);
        Assert.IsFalse(viewModel.CanUndo);
        StringAssert.Contains(viewModel.StatusText, "comments");
    }

    [TestMethod]
    public void GridCommands_MoveSpansAndBlockDraftsFromAnyTarget()
    {
        const string markup = "<Grid RowDefinitions='*,*' ColumnDefinitions='*,*,*'>"
            + "<TextBox Width='100' Grid.RowSpan='2' /><Button Grid.Column='2' /></Grid>";
        AxamlDesignerViewModel viewModel = new();
        string? published = null;
        viewModel.MarkupChanged += (_, args) => published = args.Markup;
        viewModel.LoadMarkup(markup);
        viewModel.SelectedNode = viewModel.PreviewRoot!.Children[0];

        Assert.IsTrue(viewModel.IsSelectedGridChild);
        Assert.IsTrue(viewModel.MoveGridColumnLaterCommand.CanExecute(null));
        Assert.IsFalse(viewModel.MoveGridRowLaterCommand.CanExecute(null));
        Assert.IsFalse(viewModel.MoveStructureLaterCommand.CanExecute(null));
        viewModel.MoveGridColumnLaterCommand.Execute(null);
        Assert.AreEqual("1", viewModel.SelectedNode!.Properties["Grid.Column"]);
        Assert.AreEqual("2", viewModel.SelectedNode.Properties["Grid.RowSpan"]);
        Assert.IsFalse(viewModel.MoveGridColumnLaterCommand.CanExecute(null));
        Assert.IsTrue(viewModel.MoveGridColumnEarlierCommand.CanExecute(null));
        string moved = published!;

        viewModel.SelectedNode = viewModel.PreviewRoot!.Children[1];
        AxamlPropertyRowViewModel width = viewModel.PropertyRows.Single(row => row.Name == "Width");
        Assert.IsFalse(width.TrySetValue("-2"));
        Assert.IsFalse(viewModel.RemoveStructureCommand.CanExecute(null));
        viewModel.SelectedNode = viewModel.PreviewRoot.Children[0];
        Assert.AreEqual(1, viewModel.UnappliedDrafts.Count);
        Assert.IsFalse(viewModel.MoveGridColumnEarlierCommand.CanExecute(null));
        Assert.IsFalse(viewModel.RemoveStructureCommand.CanExecute(null));
        Assert.IsFalse(viewModel.UndoCommand.CanExecute(null));
        viewModel.MoveGridColumnEarlierCommand.Execute(null);
        viewModel.RemoveStructureCommand.Execute(null);
        viewModel.UndoCommand.Execute(null);
        Assert.AreEqual(moved, published);
        Assert.AreEqual("-2", viewModel.UnappliedDrafts.Single().Value);

        viewModel.DiscardStoredDraft(viewModel.UnappliedDrafts.Single());
        Assert.IsTrue(viewModel.UndoCommand.CanExecute(null));
        viewModel.UndoCommand.Execute(null);
        Assert.AreEqual(markup, published);
        Assert.IsTrue(viewModel.RedoCommand.CanExecute(null));
        viewModel.RedoCommand.Execute(null);
        Assert.AreEqual(moved, published);
    }

    [TestMethod]
    public void PropertyRows_DebouncedEditsAreUndoableAndDraftFollowsItsOwnTarget()
    {
        AxamlDesignerViewModel viewModel = new();
        viewModel.LoadMarkup("<StackPanel Width='100'><TextBlock Text='Hello' /></StackPanel>");
        viewModel.SelectedNode = viewModel.PreviewRoot;
        AxamlPropertyRowViewModel width = viewModel.PropertyRows.Single(static row => row.Name == "Width");
        Assert.IsFalse(viewModel.PropertyRows.Single(static row => row.Name == "Height").HasValidationError);

        width.Value = "120";
        viewModel.BeginPropertyDraft(width, hasChanged: true);
        viewModel.ApplyPropertyRow(width, finalize: false);
        width.Value = "140";
        viewModel.BeginPropertyDraft(width, hasChanged: true);
        viewModel.ApplyPropertyRow(width, finalize: true);

        StringAssert.Contains(viewModel.PreviewRoot!.Properties["Width"], "140");
        viewModel.UndoCommand.Execute(null);
        Assert.AreEqual("100", viewModel.PreviewRoot!.Properties["Width"]);

        width = viewModel.PropertyRows.Single(static row => row.Name == "Width");
        width.Value = "-2";
        viewModel.BeginPropertyDraft(width, hasChanged: true);
        Assert.IsTrue(width.HasValidationError);
        AxamlPreviewNode selectedNode = viewModel.SelectedNode!;
        viewModel.SelectedNode = viewModel.PreviewRoot!.Children[0];
        Assert.AreNotSame(selectedNode, viewModel.SelectedNode);
        Assert.AreEqual(1, viewModel.UnappliedDrafts.Count);
        Assert.AreEqual("-2", viewModel.UnappliedDrafts[0].Value);
        viewModel.SelectedNode = selectedNode;
        width = viewModel.PropertyRows.Single(static row => row.Name == "Width");
        Assert.AreEqual("-2", width.Value);
        Assert.IsTrue(viewModel.HasInvalidPropertyDraft);

        viewModel.DiscardPropertyDraft(width);
        Assert.IsFalse(viewModel.HasInvalidPropertyDraft);
        Assert.AreEqual("100", width.Value);
    }

    [TestMethod]
    public void PropertyDrafts_ChangedOrMissingTargetRequiresExplicitRecovery()
    {
        const string original = """
            <StackPanel xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
              <TextBox x:Name="First" Width="100" />
              <TextBox x:Name="Second" Width="300" />
            </StackPanel>
            """;
        AxamlDesignerViewModel viewModel = new();
        viewModel.LoadMarkup(original);
        viewModel.SelectedNode = viewModel.PreviewRoot!.Children[0];
        PropertyEditorItemViewModel width = viewModel.PropertyEditor.Items.Single(item => item.PropertyItem.Name == "Width");
        width.ScalarValue = "-2";
        Assert.IsTrue(width.HasValidationError);

        viewModel.SelectedNode = viewModel.PreviewRoot.Children[1];
        Assert.AreEqual("300", viewModel.PropertyRows.Single(row => row.Name == "Width").Value);
        string changed = original.Replace("Width=\"100\"", "Width=\"200\"", StringComparison.Ordinal);
        viewModel.LoadMarkup(changed);
        viewModel.SelectedNode = viewModel.PreviewRoot!.Children[0];
        AxamlPropertyDraft draft = viewModel.UnappliedDrafts.Single();
        Assert.IsTrue(draft.CanRestore);
        Assert.AreEqual("200", viewModel.PropertyRows.Single(row => row.Name == "Width").Value);
        Assert.IsTrue(viewModel.RestoreDraft(draft));
        Assert.AreEqual("-2", viewModel.PropertyRows.Single(row => row.Name == "Width").Value);
        Assert.AreEqual("200", viewModel.PreviewRoot.Children[0].Properties["Width"]);

        viewModel.SelectedNode = viewModel.PreviewRoot.Children[1];
        viewModel.LoadMarkup("<StackPanel><TextBox Width='300' /></StackPanel>");
        draft = viewModel.UnappliedDrafts.Single();
        Assert.IsFalse(draft.CanRestore);
        Assert.AreEqual("-2", draft.Value);
        Assert.IsTrue(viewModel.HasUnappliedPropertyDrafts);
        viewModel.DiscardStoredDraft(draft);
        Assert.IsFalse(viewModel.HasUnappliedPropertyDrafts);
    }

    [TestMethod]
    public async Task PropertyProvider_UsesSharedPropertyContractAndPreservesUndo()
    {
        AxamlDesignerViewModel viewModel = new();
        viewModel.LoadMarkup("<StackPanel Width='100' />");
        viewModel.SelectedNode = viewModel.PreviewRoot;
        var width = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "Width");
        PropertyEditorViewModel sharedEditor = new();
        sharedEditor.SetItems(viewModel.PropertyProvider.Properties);

        Assert.AreEqual(PropertyScope.Session, viewModel.PropertyProvider.Context.Scope);
        Assert.AreEqual(1, sharedEditor.Items.Count(static item => item.PropertyItem.Name == "Width"));
        Assert.IsTrue(width.TrySetValue("120"));
        await viewModel.PropertyProvider.ApplyAsync();

        Assert.AreEqual("120", viewModel.PreviewRoot!.Properties["Width"]);
        Assert.IsTrue(viewModel.CanUndo);
        viewModel.UndoCommand.Execute(null);
        Assert.AreEqual("100", viewModel.PreviewRoot!.Properties["Width"]);
    }

    [TestMethod]
    public void SharedPropertyEditor_EditCompletionAppliesValueAsUndoableDesignerChange()
    {
        AxamlDesignerViewModel viewModel = new();
        viewModel.LoadMarkup("<StackPanel Width='100' />");
        viewModel.SelectedNode = viewModel.PreviewRoot;
        var width = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "Width");
        viewModel.PropertyEditor.SetItems(viewModel.PropertyProvider.Properties);
        viewModel.PropertyEditor.NotifyEditStarted(width);

        Assert.IsTrue(width.TrySetValue("120"));
        viewModel.PropertyEditor.NotifyEditCompleted(width);

        Assert.AreEqual("120", viewModel.PreviewRoot!.Properties["Width"]);
        viewModel.UndoCommand.Execute(null);
        Assert.AreEqual("100", viewModel.PreviewRoot!.Properties["Width"]);
    }

    [TestMethod]
    public async Task PropertyProvider_InvalidDraftBlocksApplyAndCanBeDiscarded()
    {
        AxamlDesignerViewModel viewModel = new();
        viewModel.LoadMarkup("<StackPanel Width='100' />");
        viewModel.SelectedNode = viewModel.PreviewRoot;
        var width = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "Width");

        Assert.IsFalse(width.TrySetValue("-2"));
        Assert.IsTrue(viewModel.HasInvalidPropertyDraft);
        await Assert.ThrowsExactlyAsync<InvalidOperationException>(
            () => viewModel.PropertyProvider.ApplyAsync());

        await viewModel.PropertyProvider.DiscardAsync();
        Assert.IsFalse(viewModel.HasInvalidPropertyDraft);
        Assert.AreEqual("100", viewModel.PropertyRows.Single(static row => row.Name == "Width").Value);
    }

    [TestMethod]
    public void PropertyRows_ChangingSelectedControlDoesNotDuplicateSharedProperties()
    {
        AxamlDesignerViewModel viewModel = new();
        viewModel.LoadMarkup(
            "<StackPanel xmlns='https://github.com/avaloniaui'><TextBox /></StackPanel>");
        AxamlPreviewNode stackPanel = viewModel.PreviewRoot!;
        AxamlPreviewNode textBox = stackPanel.Children[0];
        viewModel.SelectedNode = stackPanel;
        viewModel.SelectedNode = textBox;

        Assert.AreEqual(
            viewModel.PropertyRows.Count,
            viewModel.PropertyRows.Select(static row => row.Name).Distinct(StringComparer.Ordinal).Count());
        Assert.IsTrue(viewModel.PropertyRows.All(static row => row.ControlType == "TextBox"));
        Assert.AreEqual(1, viewModel.PropertyRows.Count(static row => row.Name == "Height"));
        Assert.AreEqual(1, viewModel.PropertyRows.Count(static row => row.Name == "Width"));

        AxamlBindingRowViewModel binding = viewModel.BindingRows.Single();
        binding.NewMemberName = "Caption";
        viewModel.ApplyBindingRow(binding);

        StringAssert.Contains(viewModel.SelectedNode!.Properties["Text"], "Binding Caption");
    }

    [TestMethod]
    public void BindingRows_OfferCompatibleMembersAndApplyExplicitNewBinding()
    {
        AxamlDesignerViewModel viewModel = new();
        viewModel.LoadMarkup("<Button Command='{Binding OldCommand}' />");
        viewModel.SelectedNode = viewModel.PreviewRoot;
        viewModel.SetBindingContext(
        [
            new AxamlMvvmSemanticMember("SaveCommand", "ICommand", IsCommand: true, IsWritable: false),
            new AxamlMvvmSemanticMember("Caption", "string", IsCommand: false, IsWritable: true),
        ]);

        AxamlBindingRowViewModel command = viewModel.BindingRows.Single();
        Assert.AreEqual("Command (ICommand)", command.MemberKindLabel);
        Assert.AreEqual("New command member name", command.NewMemberPlaceholder);
        CollectionAssert.AreEqual(new[] { "SaveCommand" }, command.AvailableMembers.ToArray());
        Assert.IsTrue(command.IsCreatingNewMember);
        Assert.IsFalse(command.IsSelectingExistingMember);
        command.NewMemberName = string.Empty;
        Assert.IsNull(command.GetRequestedMember());
        command.SelectExistingMemberSource();
        Assert.IsTrue(command.IsSelectingExistingMember);
        command.SelectedMember = "SaveCommand";
        Assert.AreEqual("SaveCommand", command.GetRequestedMember());
        command.NewMemberName = "DeleteCommand";
        Assert.IsTrue(command.IsCreatingNewMember);
        viewModel.ApplyBindingRow(command);

        StringAssert.Contains(viewModel.PreviewRoot!.Properties["Command"], "DeleteCommand");
        Assert.IsTrue(viewModel.CanUndo);
        Assert.IsFalse(viewModel.BindingRows.Single().HasDiagnostic);

        AxamlDesignerViewModel modeViewModel = new();
        modeViewModel.LoadMarkup("<TextBox Text='{Binding ExistingText, Mode=OneWay}' />");
        modeViewModel.SelectedNode = modeViewModel.PreviewRoot;
        modeViewModel.SetBindingContext(
        [
            new AxamlMvvmSemanticMember("ExistingText", "string", IsCommand: false, IsWritable: true),
        ]);
        AxamlBindingRowViewModel textBinding = modeViewModel.BindingRows.Single();
        Assert.AreEqual(AxamlBindingMode.OneWay, textBinding.BindingMode);
        textBinding.BindingMode = AxamlBindingMode.TwoWay;
        textBinding.UpdateSourceTrigger = AxamlBindingUpdateSourceTrigger.PropertyChanged;
        modeViewModel.ApplyBindingRow(textBinding);
        StringAssert.Contains(modeViewModel.PreviewRoot!.Properties["Text"], "Mode=TwoWay");
        StringAssert.Contains(modeViewModel.PreviewRoot!.Properties["Text"], "UpdateSourceTrigger=PropertyChanged");
        Assert.AreEqual(AxamlBindingMode.TwoWay, modeViewModel.BindingRows.Single().BindingMode);
        Assert.AreEqual(
            AxamlBindingUpdateSourceTrigger.PropertyChanged,
            modeViewModel.BindingRows.Single().UpdateSourceTrigger);
        Assert.IsTrue(modeViewModel.UndoCommand.CanExecute(null));
        modeViewModel.UndoCommand.Execute(null);
        StringAssert.Contains(modeViewModel.PreviewRoot!.Properties["Text"], "Mode=OneWay");
        Assert.IsFalse(modeViewModel.PreviewRoot!.Properties["Text"].Contains(
            "UpdateSourceTrigger",
            StringComparison.Ordinal));

        AxamlDesignerViewModel advancedViewModel = new();
        advancedViewModel.LoadMarkup("<TextBox Text='{Binding User.Name, Mode=TwoWay}' />");
        advancedViewModel.SelectedNode = advancedViewModel.PreviewRoot;
        AxamlBindingRowViewModel advancedBinding = advancedViewModel.BindingRows.Single();
        Assert.IsFalse(advancedBinding.CanEditBinding);
        Assert.IsTrue(advancedBinding.HasUnsupportedBindingExpression);
        Assert.IsNotNull(advancedBinding.BindingExpressionDiagnostic);
        Assert.IsNull(AxamlBindingRowViewModel.ReadBindingPath(
            "{Binding UserName, Mode=OneWay, Mode=TwoWay}"));
        Assert.IsNull(AxamlBindingRowViewModel.ReadBindingPath(
            "{Binding UserName, Converter=Example}"));
        Assert.AreEqual(
            "UserName",
            AxamlBindingRowViewModel.ReadBindingPath("{Binding Path=UserName}", out AxamlBindingMode simplePathMode));
        Assert.AreEqual(AxamlBindingMode.Default, simplePathMode);
        Assert.AreEqual(
            "UserName",
            AxamlBindingRowViewModel.ReadBindingPath(
                "{Binding, Path=UserName, Mode=TwoWay}",
                out AxamlBindingMode explicitPathMode));
        Assert.AreEqual(AxamlBindingMode.TwoWay, explicitPathMode);
        Assert.AreEqual(
            "UserName",
            AxamlBindingRowViewModel.ReadBindingPath(
                "{Binding UserName, Mode=Default}",
                out AxamlBindingMode explicitDefaultMode));
        Assert.AreEqual(AxamlBindingMode.Default, explicitDefaultMode);
        Assert.AreEqual(
            "UserName",
            AxamlBindingRowViewModel.ReadBindingPath(
                "{Binding UserName, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}",
                out AxamlBindingMode updateMode,
                out AxamlBindingUpdateSourceTrigger updateTrigger));
        Assert.AreEqual(AxamlBindingMode.TwoWay, updateMode);
        Assert.AreEqual(AxamlBindingUpdateSourceTrigger.PropertyChanged, updateTrigger);
        Assert.AreEqual(
            "UserName",
            AxamlBindingRowViewModel.ReadBindingPath(
                "{Binding UserName, UpdateSourceTrigger=DataValidation}",
                out _,
                out AxamlBindingUpdateSourceTrigger validationTrigger));
        Assert.AreEqual(AxamlBindingUpdateSourceTrigger.DataValidation, validationTrigger);
        Assert.IsNull(AxamlBindingRowViewModel.ReadBindingPath(
            "{Binding UserName, UpdateSourceTrigger=PropertyChanged, UpdateSourceTrigger=LostFocus}"));

        viewModel.SetBindingContext(
        [
            new AxamlMvvmSemanticMember("SaveCommand", "ICommand", IsCommand: true, IsWritable: false),
            new AxamlMvvmSemanticMember("Caption", "string", IsCommand: false, IsWritable: true),
        ]);
        command = viewModel.BindingRows.Single();
        command.NewMemberName = "Caption";
        viewModel.ApplyBindingRow(command);

        StringAssert.Contains(viewModel.PreviewRoot!.Properties["Command"], "DeleteCommand");
        StringAssert.Contains(command.Diagnostic, "incompatible");

        AxamlDesignerViewModel propertyViewModel = new();
        propertyViewModel.LoadMarkup("<TextBox />");
        propertyViewModel.SelectedNode = propertyViewModel.PreviewRoot;
        propertyViewModel.SetBindingContext([]);
        AxamlBindingRowViewModel property = propertyViewModel.BindingRows.Single();
        Assert.AreEqual("Property (string)", property.MemberKindLabel);
        Assert.AreEqual("New string property name", property.NewMemberPlaceholder);
        CollectionAssert.AreEqual(new[] { "Create new member" }, property.MemberSources.ToArray());
        Assert.IsTrue(property.IsCreatingNewMember);
        Assert.AreEqual(AxamlBindingMode.TwoWay, property.BindingMode);
        Assert.IsTrue(property.SupportsUpdateSourceTrigger);

        AxamlDesignerViewModel displayViewModel = new();
        displayViewModel.LoadMarkup("<TextBlock />");
        displayViewModel.SelectedNode = displayViewModel.PreviewRoot;
        Assert.AreEqual(AxamlBindingMode.OneWay, displayViewModel.BindingRows.Single().BindingMode);
        Assert.IsFalse(displayViewModel.BindingRows.Single().SupportsUpdateSourceTrigger);
    }

    [TestMethod]
    public async Task BindingRows_SendMissingMemberToWorkspaceReviewHandler()
    {
        AxamlDesignerViewModel viewModel = new();
        viewModel.LoadMarkup("<TextBox Text='unsaved draft' />");
        viewModel.SelectedNode = viewModel.PreviewRoot;
        viewModel.SetBindingContext([]);
        AxamlBindingRowViewModel row = viewModel.BindingRows.Single();
        row.NewMemberName = "Caption";
        bool invoked = false;
        viewModel.ApplyMissingBindingAsync = requestedRow =>
        {
            invoked = ReferenceEquals(requestedRow, row);
            return Task.FromResult(false);
        };

        await viewModel.ApplyBindingRowAsync(row);

        Assert.IsTrue(invoked);
        Assert.AreEqual("unsaved draft", viewModel.PreviewRoot!.Properties["Text"]);
        Assert.IsFalse(viewModel.CanUndo);
    }

    [TestMethod]
    public void DesignerViewModel_InvalidMarkupSurfacesErrorAndCanRecover()
    {
        AxamlDesignerViewModel viewModel = new();
        viewModel.LoadMarkup("<StackPanel>");

        Assert.IsNull(viewModel.PreviewRoot);
        Assert.IsTrue(viewModel.HasPreview);
        Assert.IsFalse(string.IsNullOrWhiteSpace(viewModel.ErrorText));

        viewModel.LoadMarkup("<StackPanel />");

        Assert.IsNotNull(viewModel.PreviewRoot);
        Assert.IsTrue(viewModel.HasPreview);
        Assert.AreEqual(string.Empty, viewModel.ErrorText);
    }

    [TestMethod]
    public void DesignerViewModel_PreservesValidPreviewElementsWhenMarkupIsMalformed()
    {
        AxamlDesignerViewModel viewModel = new();
        viewModel.LoadMarkup(
            "<StackPanel xmlns='https://github.com/avaloniaui'><TextBlock Text='Visible' /><Button Content=broken /><Label Content='Also visible' /></StackPanel>");

        Assert.IsNotNull(viewModel.PreviewRoot);
        Assert.IsTrue(viewModel.PreviewRoot.IsRecoveryRoot);
        Assert.AreEqual(3, viewModel.PreviewRoot.Children.Count);
        Assert.IsFalse(viewModel.PreviewRoot.Children[1].IsSupported);
        Assert.IsTrue(viewModel.HasPreview);
        StringAssert.Contains(viewModel.ErrorText, "Partial preview");
        Assert.IsFalse(viewModel.CanUndo);
    }

    [TestMethod]
    public void DesignerViewModel_ReportsCapabilitiesForSupportedAndUnknownControls()
    {
        AxamlDesignerViewModel viewModel = new();
        viewModel.LoadMarkup(
            "<Window xmlns='https://github.com/avaloniaui' xmlns:custom='using:Sample'><TextBox /><custom:SpecialEditor /></Window>");

        viewModel.SelectedNode = viewModel.PreviewRoot!.Children[0];
        StringAssert.Contains(viewModel.SelectedCapabilityText, "Editable: AcceptsReturn, AcceptsTab, Background, BorderBrush, BorderThickness, ClipToBounds, CornerRadius, FlowDirection, FontSize, FontStyle, FontWeight, Foreground, Height, HorizontalAlignment, HorizontalContentAlignment, IsReadOnly, IsVisible, LineHeight, Margin, MaxHeight, MaxLength, MaxLines, MaxWidth, MinHeight, MinLines, MinWidth, Opacity, Padding, PlaceholderForeground, PlaceholderText, Text, TextAlignment, TextWrapping, UseFloatingPlaceholder, UseLayoutRounding, VerticalAlignment, VerticalContentAlignment, Width");
        StringAssert.Contains(viewModel.SelectedCapabilityText, "Binding analysis: Text");
        StringAssert.Contains(viewModel.SelectedCapabilityText, "MVVM generation: supported for named controls");

        viewModel.SelectedNode = viewModel.PreviewRoot.Children[1];
        StringAssert.Contains(viewModel.SelectedCapabilityText, "Safe placeholder preview only");
        StringAssert.Contains(viewModel.SelectedCapabilityText, "MVVM generation are unavailable");

        viewModel.LoadMarkup(
            "<Window xmlns='https://github.com/avaloniaui' xmlns:custom='urn:sample'><custom:Button /></Window>");
        viewModel.SelectedNode = viewModel.PreviewRoot;
        StringAssert.Contains(viewModel.SelectedCapabilityText, "Safe placeholder preview only");

        viewModel.LoadMarkup("<StackPanel><TextBlock /></StackPanel>");
        viewModel.SelectedNode = viewModel.PreviewRoot;
        StringAssert.Contains(viewModel.SelectedCapabilityText, "appending supported");

        viewModel.LoadMarkup("<StackPanel>unmodeled text<TextBlock /></StackPanel>");
        viewModel.SelectedNode = viewModel.PreviewRoot;
        StringAssert.Contains(viewModel.SelectedCapabilityText, "correct invalid or unsupported markup");
        viewModel.AddButtonCommand.Execute(null);
        StringAssert.Contains(viewModel.StatusText, "Correct invalid inline text");

        viewModel.LoadMarkup("<DockPanel><TextBlock /></DockPanel>");
        viewModel.SelectedNode = viewModel.PreviewRoot;
        StringAssert.Contains(viewModel.SelectedCapabilityText, "docked children stay before Client");
        Assert.IsTrue(viewModel.IsDockPanelSelected);

        viewModel.LoadMarkup("<DockPanel LastChildFill='False'><TextBlock /></DockPanel>");
        viewModel.SelectedNode = viewModel.PreviewRoot;
        StringAssert.Contains(viewModel.SelectedCapabilityText, "LastChildFill is disabled");

        viewModel.LoadMarkup("<Grid />");
        viewModel.SelectedNode = viewModel.PreviewRoot;
        StringAssert.Contains(viewModel.SelectedCapabilityText, "next unoccupied cell");

        viewModel.LoadMarkup("<Grid RowDefinitions='Auto,,*' />");
        viewModel.SelectedNode = viewModel.PreviewRoot;
        StringAssert.Contains(viewModel.SelectedCapabilityText, "correct invalid or unsupported markup");
    }

    [TestMethod]
    public void DesignerViewModel_DockPanelPaletteKeepsClientLastAndAddsDockedChildren()
    {
        AxamlDesignerViewModel viewModel = new();
        viewModel.LoadMarkup("<DockPanel />");
        viewModel.SelectedNode = viewModel.PreviewRoot;

        Assert.IsTrue(viewModel.IsDockPanelSelected);
        Assert.AreEqual(AxamlDockPanelPlacement.Client, viewModel.SelectedDockPanelPlacement);
        viewModel.AddTextBlockCommand.Execute(null);

        viewModel.SelectedDockPanelPlacement = AxamlDockPanelPlacement.Top;
        viewModel.AddButtonCommand.Execute(null);
        viewModel.SelectedDockPanelPlacement = AxamlDockPanelPlacement.Bottom;
        viewModel.AddSeparatorCommand.Execute(null);

        Assert.AreEqual("Button", viewModel.PreviewRoot?.Children[0].TypeName);
        Assert.AreEqual("Top", viewModel.PreviewRoot?.Children[0].Properties["DockPanel.Dock"]);
        Assert.AreEqual("Separator", viewModel.PreviewRoot?.Children[1].TypeName);
        Assert.AreEqual("Bottom", viewModel.PreviewRoot?.Children[1].Properties["DockPanel.Dock"]);
        Assert.AreEqual("TextBlock", viewModel.PreviewRoot?.Children[2].TypeName);
        Assert.IsTrue(viewModel.CanUndo);
        viewModel.UndoCommand.Execute(null);
        Assert.AreEqual(2, viewModel.PreviewRoot?.Children.Count);
    }

    [TestMethod]
    public void DesignerViewModel_AddControlPublishesMarkupAndReportsInvalidContainer()
    {
        AxamlDesignerViewModel viewModel = new();
        string? publishedMarkup = null;
        viewModel.MarkupChanged += (_, args) => publishedMarkup = args.Markup;
        viewModel.LoadMarkup("<StackPanel />");
        viewModel.SelectedNode = viewModel.PreviewRoot;

        viewModel.AddTextBlockCommand.Execute(null);

        StringAssert.Contains(publishedMarkup, "<TextBlock />");
        StringAssert.Contains(viewModel.StatusText, "inserted");
        Assert.IsTrue(viewModel.CanUndo);
        viewModel.UndoCommand.Execute(null);
        Assert.AreEqual("<StackPanel />", publishedMarkup);

        viewModel.LoadMarkup("<TextBlock />");
        viewModel.SelectedNode = viewModel.PreviewRoot;
        viewModel.AddButtonCommand.Execute(null);
        StringAssert.Contains(viewModel.StatusText, "cannot contain another child");
    }

    [TestMethod]
    public void DesignerViewModel_RemovesSingleChildContentAndUndoRestoresIt()
    {
        AxamlDesignerViewModel viewModel = new();
        string? publishedMarkup = null;
        viewModel.MarkupChanged += (_, args) => publishedMarkup = args.Markup;
        viewModel.LoadMarkup("<Border><Button /></Border>");
        viewModel.SelectedNode = viewModel.PreviewRoot;
        StringAssert.Contains(viewModel.SelectedCapabilityText, "safe removal");
        viewModel.SelectedNode = viewModel.PreviewRoot!.Children.Single();

        Assert.IsTrue(viewModel.RemoveStructureCommand.CanExecute(null));
        viewModel.RemoveStructureCommand.Execute(null);

        Assert.AreEqual("<Border></Border>", publishedMarkup);
        Assert.IsTrue(viewModel.CanUndo);
        viewModel.UndoCommand.Execute(null);
        Assert.AreEqual("<Border><Button /></Border>", publishedMarkup);
    }

    [TestMethod]
    public void DesignerViewModel_PaletteAddsCatalogInputAndLayoutControls()
    {
        (string Markup, Action<AxamlDesignerViewModel> Execute)[] paletteActions =
        [
            ("<Label />", viewModel => viewModel.AddLabelCommand.Execute(null)),
            ("<CheckBox />", viewModel => viewModel.AddCheckBoxCommand.Execute(null)),
            ("<ComboBox />", viewModel => viewModel.AddComboBoxCommand.Execute(null)),
            ("<NumericUpDown />", viewModel => viewModel.AddNumericUpDownCommand.Execute(null)),
            ("<RadioButton />", viewModel => viewModel.AddRadioButtonCommand.Execute(null)),
            ("<Slider />", viewModel => viewModel.AddSliderCommand.Execute(null)),
            ("<ToggleSwitch />", viewModel => viewModel.AddToggleSwitchCommand.Execute(null)),
            ("<Border />", viewModel => viewModel.AddBorderCommand.Execute(null)),
            ("<ContentControl />", viewModel => viewModel.AddContentControlCommand.Execute(null)),
            ("<DockPanel />", viewModel => viewModel.AddDockPanelCommand.Execute(null)),
            ("<Expander />", viewModel => viewModel.AddExpanderCommand.Execute(null)),
            ("<GroupBox />", viewModel => viewModel.AddGroupBoxCommand.Execute(null)),
            ("<Grid />", viewModel => viewModel.AddGridCommand.Execute(null)),
            ("<ProgressBar />", viewModel => viewModel.AddProgressBarCommand.Execute(null)),
            ("<ScrollViewer />", viewModel => viewModel.AddScrollViewerCommand.Execute(null)),
            ("<Separator />", viewModel => viewModel.AddSeparatorCommand.Execute(null)),
            ("<StackPanel />", viewModel => viewModel.AddStackPanelCommand.Execute(null)),
            ("<WrapPanel />", viewModel => viewModel.AddWrapPanelCommand.Execute(null)),
            ("<Viewbox />", viewModel => viewModel.AddViewboxCommand.Execute(null)),
            ("<UniformGrid />", viewModel => viewModel.AddUniformGridCommand.Execute(null)),
        ];

        foreach ((string expectedControl, Action<AxamlDesignerViewModel> execute) in paletteActions)
        {
            AxamlDesignerViewModel viewModel = new();
            viewModel.LoadMarkup("<StackPanel />");
            viewModel.SelectedNode = viewModel.PreviewRoot;

            execute(viewModel);

            StringAssert.Contains(viewModel.PreviewRoot?.Children.Single().SourceStartTag, expectedControl);
            Assert.IsTrue(viewModel.CanUndo);
            Assert.IsFalse(string.IsNullOrWhiteSpace(viewModel.StatusText));
        }
    }

    [TestMethod]
    public void DesignerViewModel_ResizeElementPublishesOneUndoableLayoutEdit()
    {
        AxamlDesignerViewModel viewModel = new();
        string? publishedMarkup = null;
        viewModel.MarkupChanged += (_, args) => publishedMarkup = args.Markup;
        viewModel.LoadMarkup("<StackPanel><TextBlock Text='Resize me' /></StackPanel>");
        AxamlPreviewNode node = viewModel.PreviewRoot!;

        Assert.IsTrue(viewModel.ResizeElement(node, 180.5, 72.5));
        StringAssert.Contains(publishedMarkup, "Width=\"180.5\"");
        StringAssert.Contains(publishedMarkup, "Height=\"72.5\"");
        Assert.AreEqual("Width", viewModel.SelectedProperty);

        viewModel.UndoCommand.Execute(null);
        Assert.AreEqual("<StackPanel><TextBlock Text='Resize me' /></StackPanel>", publishedMarkup);
        viewModel.RedoCommand.Execute(null);
        StringAssert.Contains(publishedMarkup, "Width=\"180.5\"");
        StringAssert.Contains(publishedMarkup, "Height=\"72.5\"");
    }

    [TestMethod]
    public void DesignerViewModel_ResizeElementClampsDimensionsAndRejectsUnsupportedMarkup()
    {
        AxamlDesignerViewModel viewModel = new();
        viewModel.LoadMarkup("<StackPanel />");

        Assert.IsTrue(viewModel.ResizeElement(viewModel.PreviewRoot!, 9000, 0));
        StringAssert.Contains(viewModel.PreviewRoot!.SourceStartTag, "Width=\"4096\"");
        StringAssert.Contains(viewModel.PreviewRoot.SourceStartTag, "Height=\"16\"");

        viewModel.LoadMarkup("<StackPanel xmlns='https://github.com/avaloniaui'><StackPanel.Width>120</StackPanel.Width></StackPanel>");
        string originalMarkup = viewModel.PreviewRoot!.SourceStartTag!;
        Assert.IsFalse(viewModel.ResizeElement(viewModel.PreviewRoot, 200, 80));
        Assert.IsFalse(viewModel.CanUndo);
        Assert.AreEqual(originalMarkup, viewModel.PreviewRoot.SourceStartTag);
        StringAssert.Contains(viewModel.StatusText, "resize failed");
    }
}
