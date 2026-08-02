using AppKomponentBaseLib.Commands;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace AA98_AvlnCodeStudio.Tests.Components.Commands;

/// <summary>
/// Verifies the shared tool-capable command contracts.
/// </summary>
[TestClass]
public sealed class ToolCommandContractTests
{
    /// <summary>
    /// Verifies that descriptors normalize metadata and preserve consent information.
    /// </summary>
    [TestMethod]
    public void DescriptorNormalizesMetadataAndDefaults()
    {
        var descriptor = new ToolCommandDescriptor(
            "Planning.Read",
            " Read Planning ",
            new[]
            {
                new ToolCommandParameterDescriptor("path", " Path ", "Planning file path", true, " ./planning.md ", "string")
            },
            new[]
            {
                new ToolCommandResultDescriptor("items", " Items ", "Planning items", "array")
            },
            new[] { "Planning", " planning ", string.Empty, "Editor" },
            "Reads planning content.",
            requiresConsent: true,
            safetyLevel: ToolCommandSafetyLevel.Medium);

        Assert.AreEqual("Planning.Read", descriptor.CommandId);
        Assert.AreEqual("Read Planning", descriptor.DisplayTitle);
        Assert.AreEqual("Reads planning content.", descriptor.Description);
        Assert.IsTrue(descriptor.RequiresConsent);
        Assert.AreEqual(ToolCommandSafetyLevel.Medium, descriptor.SafetyLevel);
        Assert.AreEqual(1, descriptor.Parameters.Count);
        Assert.AreEqual("path", descriptor.Parameters[0].ParameterName);
        Assert.AreEqual("Path", descriptor.Parameters[0].DisplayName);
        Assert.IsTrue(descriptor.Parameters[0].IsRequired);
        Assert.AreEqual("./planning.md", descriptor.Parameters[0].DefaultValue);
        Assert.AreEqual("string", descriptor.Parameters[0].ValueKind);
        Assert.AreEqual(1, descriptor.Results.Count);
        Assert.AreEqual("items", descriptor.Results[0].ResultName);
        Assert.AreEqual("Items", descriptor.Results[0].DisplayName);
        Assert.AreEqual("array", descriptor.Results[0].ValueKind);
        CollectionAssert.AreEqual(new[] { "Planning", "Editor" }, (System.Collections.ICollection)descriptor.RequiredContextKinds);
    }

    /// <summary>
    /// Verifies that descriptors reject missing identifiers and titles.
    /// </summary>
    [TestMethod]
    public void DescriptorRejectsMissingIdentity()
    {
        Assert.ThrowsExactly<ArgumentException>(() => new ToolCommandDescriptor(string.Empty, "Title"));
        Assert.ThrowsExactly<ArgumentException>(() => new ToolCommandDescriptor("Planning.Read", string.Empty));
    }

    /// <summary>
    /// Verifies that parameter descriptors reject missing identifiers and names.
    /// </summary>
    [TestMethod]
    public void ParameterDescriptorRejectsMissingIdentity()
    {
        Assert.ThrowsExactly<ArgumentException>(() => new ToolCommandParameterDescriptor(string.Empty, "Name"));
        Assert.ThrowsExactly<ArgumentException>(() => new ToolCommandParameterDescriptor("path", string.Empty));
    }

    /// <summary>
    /// Verifies that result descriptors reject missing identifiers and names.
    /// </summary>
    [TestMethod]
    public void ResultDescriptorRejectsMissingIdentity()
    {
        Assert.ThrowsExactly<ArgumentException>(() => new ToolCommandResultDescriptor(string.Empty, "Name"));
        Assert.ThrowsExactly<ArgumentException>(() => new ToolCommandResultDescriptor("result", string.Empty));
    }
}
