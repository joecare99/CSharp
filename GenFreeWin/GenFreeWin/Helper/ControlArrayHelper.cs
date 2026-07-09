using BaseLib.Helper;
using System;
using System.Windows.Forms;

namespace GenFree.Helper;

public static class ControlArrayHelper
{
    public static void AddKeyUp<T>(this ControlArray<T> caText, KeyEventHandler keh) where T : Control
    {
        foreach (var item in caText)
        {
            item.Value.KeyUp += keh;
        }
    }
    public static void RemoveKeyUp<T>(this ControlArray<T> caText, KeyEventHandler keh) where T : Control
    {
        foreach (var item in caText)
        {
            item.Value.KeyUp -= keh;
        }
    }
    public static void AddKeyPress<T>(this ControlArray<T> caText, KeyPressEventHandler keh) where T : Control
    {
        foreach (var item in caText)
        {
            item.Value.KeyPress += keh;
        }
    }
    public static void RemoveKeyPress<T>(this ControlArray<T> caText, KeyPressEventHandler keh) where T : Control
    {
        foreach (var item in caText)
        {
            item.Value.KeyPress -= keh;
        }
    }
    public static void AddKeyDown<T>(this ControlArray<T> caText, KeyEventHandler keh) where T : Control
    {
        foreach (var item in caText)
        {
            item.Value.KeyDown += keh;
        }
    }
    public static void RemoveKeyDown<T>(this ControlArray<T> caText, KeyEventHandler keh) where T : Control
    {
        foreach (var item in caText)
        {
            item.Value.KeyDown -= keh;
        }
    }

    public static void AddMouseDown<T>(this ControlArray<T> caText, MouseEventHandler keh) where T : Control
    {
        foreach (var item in caText)
        {
            item.Value.MouseDown += keh;
        }
    }
    public static void RemoveMouseDown<T>(this ControlArray<T> caText, MouseEventHandler keh) where T : Control
    {
        foreach (var item in caText)
        {
            item.Value.MouseDown -= keh;
        }
    }
    public static void AddMouseUp<T>(this ControlArray<T> caText, MouseEventHandler keh) where T : Control
    {
        foreach (var item in caText)
        {
            item.Value.MouseUp += keh;
        }
    }
    public static void RemoveMouseUp<T>(this ControlArray<T> caText, MouseEventHandler keh) where T : Control
    {
        foreach (var item in caText)
        {
            item.Value.MouseUp -= keh;
        }
    }

    public static void AddEnter<T>(this ControlArray<T> caText, EventHandler eh) where T : Control
    {
        foreach (var item in caText)
        {
            item.Value.Enter += eh;
        }
    }
    public static void RemoveEnter<T>(this ControlArray<T> caText, EventHandler eh) where T : Control
    {
        foreach (var item in caText)
        {
            item.Value.Enter -= eh;
        }
    }
    public static void AddTextChanged<T>(this ControlArray<T> caText, EventHandler eh) where T : Control
    {
        foreach (var item in caText)
        {
            item.Value.TextChanged += eh;
        }
    }
    public static void RemoveTextChanged<T>(this ControlArray<T> caText, EventHandler eh) where T : Control
    {
        foreach (var item in caText)
        {
            item.Value.TextChanged -= eh;
        }
    }
    public static void AddDoubleClick<T>(this ControlArray<T> caText, EventHandler eh) where T : Control
    {
        foreach (var item in caText)
        {
            item.Value.DoubleClick += eh;
        }
    }
    public static void RemoveDoubleClick<T>(this ControlArray<T> caText, EventHandler eh) where T : Control
    {
        foreach (var item in caText)
        {
            item.Value.DoubleClick -= eh;
        }
    }
    public static void AddClick<T>(this ControlArray<T> caText, EventHandler eh) where T : Control
    {
        foreach (var item in caText)
        {
            item.Value.Click += eh;
        }
    }
    public static void RemoveClick<T>(this ControlArray<T> caText, EventHandler eh) where T : Control
    {
        foreach (var item in caText)
        {
            item.Value.Click -= eh;
        }
    }

    public static void AddCheckedChanged<T>(this ControlArray<T> caText, EventHandler eh) where T : CheckBox
    {
        foreach (var item in caText)
        {
            item.Value.CheckedChanged += eh;
        }
    }
    public static void RemoveCheckedChanged<T>(this ControlArray<T> caText, EventHandler eh) where T : CheckBox
    {
        foreach (var item in caText)
        {
            item.Value.CheckedChanged -= eh;
        }
    }
      public static void AddCheckedChangedRB<T>(this ControlArray<T> caText, EventHandler eh) where T : RadioButton
    {
        foreach (var item in caText)
        {
            item.Value.CheckedChanged += eh;
        }
    }
    public static void RemoveCheckedChangedRB<T>(this ControlArray<T> caText, EventHandler eh) where T : RadioButton
    {
        foreach (var item in caText)
        {
            item.Value.CheckedChanged -= eh;
        }
    }

}
