using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace Property.Editor.Avalonia.Tests;

/// <summary>
/// Verifies type-specific behavior of the reusable Property.Editor view models.
/// </summary>
[TestClass]
public sealed class PropertyEditorItemViewModelTests
{
    private static readonly PropertyCategory TestCategory = new("Test", "Test");

    [TestMethod]
    public void ScalarValue_ValidTypedText_AppliesConvertedValue()
    {
        PropertyItem item = new(TestCategory, "Count", "Count", typeof(int), 1);
        PropertyEditorItemViewModel viewModel = new(item);

        viewModel.ScalarValue = "42";

        Assert.AreEqual(42, item.Value);
        Assert.IsFalse(viewModel.HasValidationError);
    }

    [TestMethod]
    public void ScalarValue_InvalidTypedText_PreservesPriorValueAndShowsError()
    {
        PropertyItem item = new(TestCategory, "Count", "Count", typeof(int), 1);
        PropertyEditorItemViewModel viewModel = new(item);

        viewModel.ScalarValue = "not a number";

        Assert.AreEqual(1, item.Value);
        Assert.IsTrue(viewModel.HasValidationError);
    }

    [TestMethod]
    public void ScalarValue_EmptyNullableValue_AppliesNull()
    {
        PropertyItem item = new(TestCategory, "Count", "Count", typeof(int), 1, isNullable: true);
        PropertyEditorItemViewModel viewModel = new(item);

        viewModel.ScalarValue = string.Empty;

        Assert.IsNull(item.Value);
        Assert.IsFalse(viewModel.HasValidationError);
    }

    [TestMethod]
    public void BooleanValue_Changed_AppliesBooleanValue()
    {
        PropertyItem item = new(TestCategory, "Enabled", "Enabled", typeof(bool), false);
        PropertyEditorItemViewModel viewModel = new(item);

        viewModel.BooleanValue = true;

        Assert.AreEqual(true, item.Value);
    }

    [TestMethod]
    public void SelectedOption_ValidEnumOption_AppliesSelectedValue()
    {
        PropertyOption[] options =
        [
            new PropertyOption(TestValue.First, "First"),
            new PropertyOption(TestValue.Second, "Second"),
        ];
        PropertyItem item = new(TestCategory, "Value", "Value", typeof(TestValue), TestValue.First, options: options);
        PropertyEditorItemViewModel viewModel = new(item);

        viewModel.SelectedOption = options[1];

        Assert.AreEqual(TestValue.Second, item.Value);
    }

    [TestMethod]
    public void ReadOnlyItem_EditAttempt_PreservesValueAndShowsValidationError()
    {
        PropertyItem item = new(TestCategory, "Name", "Name", typeof(string), "Original", isEditable: false);
        PropertyEditorItemViewModel viewModel = new(item);

        viewModel.ScalarValue = "Changed";

        Assert.AreEqual("Original", item.Value);
        Assert.IsTrue(viewModel.HasValidationError);
        Assert.IsTrue(viewModel.ShowReadOnlyValue);
        Assert.IsFalse(viewModel.ShowScalarEditor);
    }

    [TestMethod]
    public void SensitiveItem_ExposesMaskingCharacter()
    {
        PropertyItem item = new(TestCategory, "Password", "Password", typeof(string), "secret", isSensitive: true);
        PropertyEditorItemViewModel viewModel = new(item);

        Assert.IsTrue(viewModel.IsSensitive);
        Assert.AreEqual('*', viewModel.PasswordChar);
    }

    [TestMethod]
    public void SetItems_OrdersItemsByNeutralCategoryAndPropertyOrdering()
    {
        PropertyCategory laterCategory = new("Later", "Later", 2);
        PropertyItem later = new(laterCategory, "Z", "Z", typeof(string), "z");
        PropertyItem first = new(TestCategory, "B", "B", typeof(string), "b", sortOrder: 2);
        PropertyItem second = new(TestCategory, "A", "A", typeof(string), "a", sortOrder: 1);
        PropertyEditorViewModel viewModel = new();

        viewModel.SetItems([later, first, second]);

        CollectionAssert.AreEqual(
            new[] { "A", "B", "Z" },
            viewModel.Items.Select(static item => item.PropertyItem.Name).ToArray());
    }

    private enum TestValue
    {
        First,
        Second,
    }
}
