using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Property.Editor.Tests;

[TestClass]
public sealed class PropertyItemTests
{
    private static readonly PropertyCategory GeneralCategory = new("General", "General");

    [TestMethod]
    public void Constructor_WithScalarValue_ExposesScalarMetadata()
    {
        var item = new PropertyItem(GeneralCategory, "Title", "Title", typeof(string), "Document");

        Assert.AreEqual(PropertyEditorKind.Scalar, item.EditorKind);
        Assert.AreEqual("Document", item.Value);
        Assert.IsTrue(item.ValidationResult.IsValid);
    }

    [TestMethod]
    public void Constructor_WithNullableScalar_AcceptsNull()
    {
        var item = new PropertyItem(GeneralCategory, "Description", "Description", typeof(string), null, isNullable: true);

        Assert.IsNull(item.Value);
        Assert.IsTrue(item.TrySetValue("Details"));
        Assert.AreEqual("Details", item.Value);
    }

    [TestMethod]
    public void Constructor_WithBooleanAndEnum_InfersEditorKinds()
    {
        var booleanItem = new PropertyItem(GeneralCategory, "Enabled", "Enabled", typeof(bool), true);
        var enumItem = new PropertyItem(
            GeneralCategory,
            "Mode",
            "Mode",
            typeof(SampleMode),
            SampleMode.Basic,
            options: [new PropertyOption(SampleMode.Basic, "Basic"), new PropertyOption(SampleMode.Advanced, "Advanced")]);

        Assert.AreEqual(PropertyEditorKind.Boolean, booleanItem.EditorKind);
        Assert.AreEqual(PropertyEditorKind.Enum, enumItem.EditorKind);
        Assert.AreEqual(2, enumItem.Options.Count);
    }

    [TestMethod]
    public void TrySetValue_WithInvalidValue_PreservesLastValidValueAndReportsError()
    {
        var item = new PropertyItem(GeneralCategory, "Count", "Count", typeof(int), 3);

        var updated = item.TrySetValue("not a number");

        Assert.IsFalse(updated);
        Assert.AreEqual(3, item.Value);
        Assert.IsFalse(item.ValidationResult.IsValid);
        Assert.IsNotNull(item.ValidationResult.ErrorMessage);
    }

    [TestMethod]
    public void TrySetValue_OnReadOnlyProperty_DoesNotChangeValue()
    {
        var item = new PropertyItem(GeneralCategory, "Id", "Id", typeof(string), "42", isEditable: false);

        var updated = item.TrySetValue("43");

        Assert.IsFalse(updated);
        Assert.AreEqual("42", item.Value);
        Assert.IsFalse(item.ValidationResult.IsValid);
    }

    [TestMethod]
    public void Constructor_WithSensitiveValue_ExposesMaskingMetadata()
    {
        var item = new PropertyItem(GeneralCategory, "Password", "Password", typeof(string), "secret", isSensitive: true);

        Assert.IsTrue(item.IsSensitive);
    }

    [TestMethod]
    public void TrySetValue_WithEmptyOptions_AcceptsMatchingEnumValue()
    {
        var item = new PropertyItem(GeneralCategory, "Mode", "Mode", typeof(SampleMode), SampleMode.Basic);

        var updated = item.TrySetValue(SampleMode.Advanced);

        Assert.IsTrue(updated);
        Assert.AreEqual(SampleMode.Advanced, item.Value);
    }

    private enum SampleMode
    {
        Basic,
        Advanced,
    }
}
