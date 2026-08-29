using System;
using ConsoleLib;
using ConsoleLib.CommonControls;
using ConsoleLib.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConsoleLib.Tests;

[TestClass]
public class V2FocusManagerTests
{
    [TestMethod]
    public void FocusManager_TraversesVisibleEnabledControls()
    {
        var root = new Panel();
        var first = new Button { Text = "First" };
        var second = new Button { Text = "Second" };
        root.Add(first);
        root.Add(second);
        var manager = new FocusManager(root);

        Assert.IsTrue(manager.MoveNext());
        Assert.AreSame(first, manager.FocusedControl);
        Assert.IsTrue(manager.MoveNext());
        Assert.AreSame(second, manager.FocusedControl);
        Assert.IsTrue(manager.HandleKey(new KeyInput(ConsoleKey.Tab, '\t', KeyModifiers.Shift, true)));
        Assert.AreSame(first, manager.FocusedControl);
    }
}
