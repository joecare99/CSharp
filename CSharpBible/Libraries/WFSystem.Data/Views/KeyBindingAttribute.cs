// ***********************************************************************
// Assembly         : WfSystem
// Author           : Mir
// Created          : 12-19-2021
//
// Last Modified By : Mir
// Last Modified On : 10-07-2022
// ***********************************************************************
// <copyright file="CommandBindingAttribute.cs" company="JC-Soft">
//     Copyright © JC-Soft 2020
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Input;

namespace Views;

[AttributeUsage(AttributeTargets.Field)]
public class KeyBindingAttribute(Char key, string cmdName) : Attribute
{
    public Char Key { get; } = key;
    public string CommandName { get; } = cmdName;

    public void Bind(object viewModel, Control field)
    {
        if (viewModel.GetType().GetProperty(CommandName)?.GetValue(viewModel) is ICommand cmd)
        {
            if (Key == '\x8')
                field.KeyDown += (s, e) =>
                {
                    if (e.KeyCode == Keys.Delete && cmd.CanExecute(field.Tag))
                    {
                        cmd.Execute(field.Tag);
                        e.Handled = true; // Prevent further processing of the key press
                    }
                };
            else
                field.KeyPress += (s, e) =>
                {
                    if (e.KeyChar == key && cmd.CanExecute(field.Tag))
                    {
                        cmd.Execute(field.Tag);
                        e.Handled = true; // Prevent further processing of the key press
                    }
                };
        }
    }

    public static void Commit(object obj, object dataContext)
    {
        foreach (var field in obj.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance))
        {
            if (GetCustomAttribute(field, typeof(KeyBindingAttribute)) is KeyBindingAttribute attr
                && field.GetValue(obj) is Control ctrl)
            {
                attr.Bind(dataContext, ctrl);
            }
        }
    }

}