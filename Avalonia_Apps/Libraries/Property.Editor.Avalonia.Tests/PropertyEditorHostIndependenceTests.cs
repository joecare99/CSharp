using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.Generic;

namespace Property.Editor.Avalonia.Tests;

/// <summary>
/// Verifies that UI consumers require only the public neutral property contract.
/// </summary>
[TestClass]
public sealed class PropertyEditorHostIndependenceTests
{
    [TestMethod]
    public void ScalarFixture_ValidChange_UsesOnlyNeutralContract()
    {
        IPropertyItem propertyItem = CreateScalarFixture();
        propertyItem.TrySetValue("Updated").Returns(true);
        PropertyEditorItemViewModel viewModel = new(propertyItem);

        viewModel.ScalarValue = "Updated";

        propertyItem.Received(1).TrySetValue("Updated");
        Assert.IsFalse(viewModel.HasValidationError);
    }

    [TestMethod]
    public void ScalarFixture_InvalidChange_ShowsContractValidationMessage()
    {
        IPropertyItem propertyItem = CreateScalarFixture();
        propertyItem.TrySetValue("Invalid").Returns(false);
        propertyItem.ValidationResult.Returns(PropertyValidationResult.Invalid("Localized validation message."));
        PropertyEditorItemViewModel viewModel = new(propertyItem);

        viewModel.ScalarValue = "Invalid";

        propertyItem.Received(1).TrySetValue("Invalid");
        Assert.AreEqual("Localized validation message.", viewModel.ValidationErrorText);
    }

    [TestMethod]
    public void SetItems_SameProperties_PreservesEditorInstancesAndEditLifecycle()
    {
        IPropertyItem propertyItem = CreateScalarFixture();
        PropertyEditorViewModel editor = new();
        editor.SetItems([propertyItem]);
        PropertyEditorItemViewModel firstItem = editor.Items[0];
        int startedCount = 0;
        int completedCount = 0;
        editor.EditStarted += (_, args) =>
        {
            Assert.AreSame(propertyItem, args.PropertyItem);
            startedCount++;
        };
        editor.EditCompleted += (_, args) =>
        {
            Assert.AreSame(propertyItem, args.PropertyItem);
            completedCount++;
        };

        editor.SetItems([propertyItem]);
        editor.NotifyEditStarted(propertyItem);
        editor.NotifyEditCompleted(propertyItem);

        Assert.AreSame(firstItem, editor.Items[0]);
        Assert.AreEqual(1, startedCount);
        Assert.AreEqual(1, completedCount);
    }

    private static IPropertyItem CreateScalarFixture()
    {
        IPropertyItem propertyItem = Substitute.For<IPropertyItem>();
        propertyItem.Category.Returns(new PropertyCategory("Fixture", "Fixture"));
        propertyItem.Name.Returns("Name");
        propertyItem.DisplayName.Returns("Name");
        propertyItem.ValueType.Returns(typeof(string));
        propertyItem.EditorKind.Returns(PropertyEditorKind.Scalar);
        propertyItem.IsNullable.Returns(false);
        propertyItem.IsEditable.Returns(true);
        propertyItem.Value.Returns("Initial");
        propertyItem.Options.Returns((IReadOnlyList<PropertyOption>)Array.Empty<PropertyOption>());
        propertyItem.ValidationResult.Returns(PropertyValidationResult.Valid);
        return propertyItem;
    }
}
