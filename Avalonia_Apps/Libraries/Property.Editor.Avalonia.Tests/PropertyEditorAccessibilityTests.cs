using Avalonia.Automation;
using Avalonia.Automation.Peers;
using Avalonia.Automation.Provider;
using Avalonia.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Property.Editor.Avalonia.Tests;

[TestClass]
public sealed class PropertyEditorAccessibilityTests
{
    [TestMethod]
    public void SensitiveScalarEditor_MasksAutomationValuesAndChangeEvents()
    {
        const string previous = "synthetic-previous";
        const string current = "synthetic-current";
        PropertyItem property = new(
            new PropertyCategory("Test", "Test"), "Password", "Password",
            typeof(string), previous, isSensitive: true);
        PropertyEditorItemViewModel editor = new(property);
        TextBox input = new() { PasswordChar = editor.PasswordChar, Text = editor.ScalarValue };
        AutomationPeer peer = ControlAutomationPeer.CreatePeerForElement(input);
        IValueProvider? valueProvider = peer.GetProvider<IValueProvider>();
        Assert.IsNotNull(valueProvider);

        List<(object? OldValue, object? NewValue)> changes = [];
        peer.PropertyChanged += (_, args) =>
        {
            if (args.Property == ValuePatternIdentifiers.ValueProperty)
            {
                changes.Add((args.OldValue, args.NewValue));
            }
        };
        input.Text = current;

        Assert.AreEqual(new string('*', current.Length), valueProvider.Value);
        Assert.AreEqual(1, changes.Count);
        Assert.AreEqual(new string('*', previous.Length), changes[0].OldValue);
        Assert.AreEqual(new string('*', current.Length), changes[0].NewValue);
    }
}
