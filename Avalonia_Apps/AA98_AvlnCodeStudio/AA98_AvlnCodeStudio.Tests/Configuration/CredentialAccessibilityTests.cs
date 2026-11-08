using Avalonia.Automation;
using Avalonia.Automation.Peers;
using Avalonia.Automation.Provider;
using Avalonia.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace AA98_AvlnCodeStudio.Tests.Configuration;

[TestClass]
public sealed class CredentialAccessibilityTests
{
    [TestMethod]
    public void MaskedCredentialInput_DoesNotExposeTextThroughAutomationValuesOrEvents()
    {
        const string previous = "synthetic-previous";
        const string current = "synthetic-current";
        TextBox input = new() { PasswordChar = '*', Text = previous };
        AutomationPeer peer = ControlAutomationPeer.CreatePeerForElement(input);
        IValueProvider? valueProvider = peer.GetProvider<IValueProvider>();
        Assert.IsNotNull(valueProvider);

        List<(object? OldValue, object? NewValue)> changes = [];
        peer.PropertyChanged += (_, eventArgs) =>
        {
            if (eventArgs.Property == ValuePatternIdentifiers.ValueProperty)
            {
                changes.Add((eventArgs.OldValue, eventArgs.NewValue));
            }
        };

        input.Text = current;

        Assert.AreEqual(new string('*', current.Length), valueProvider.Value);
        Assert.AreEqual(1, changes.Count);
        Assert.AreEqual(new string('*', previous.Length), changes[0].OldValue);
        Assert.AreEqual(new string('*', current.Length), changes[0].NewValue);
    }
}
