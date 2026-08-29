using System;
using ConsoleLib.CommonControls;
using ConsoleLib.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConsoleLib.Tests;

[TestClass]
public class CheckBoxTests
{
    [TestMethod]
    public void CheckBox_TogglesWithSpace()
    {
        var checkBox = new CheckBox { Active = true };
        var clicked = false;
        checkBox.OnClick += (_, _) => clicked = true;

        checkBox.HandlePressKeyEvents(new TestKeyEvent(' ', true));

        Assert.IsTrue(checkBox.IsChecked);
        Assert.IsTrue(clicked);
    }

    private sealed class TestKeyEvent : IKeyEvent
    {
        public TestKeyEvent(char keyChar, bool keyDown) { KeyChar = keyChar; bKeyDown = keyDown; }
        public bool bKeyDown { get; }
        public char KeyChar { get; }
        public ushort usKeyCode => 0;
        public ushort usScanCode => 0;
        public uint dwControlKeyState => 0;
        public bool Handled { get; set; }
    }
}
