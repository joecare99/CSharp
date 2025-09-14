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
public class DblClickBindingAttribute(string cmdName) : Attribute
{
    public string CommandName { get; } = cmdName;

    public void Bind(object viewModel, Control field)
    {
        if (viewModel.GetType().GetProperty(CommandName)?.GetValue(viewModel) is ICommand cmd)
        {
            cmd.CanExecuteChanged += (s, e) => field.Enabled = cmd.CanExecute(field.Tag);
            field.DoubleClick += (s, e) => cmd.Execute(field.Tag);
            field.KeyPress += (s, e) =>
            {
                if (cmd.CanExecute(field.Tag) && (e.KeyChar == (char)Keys.Return || e.KeyChar == (char)Keys.Enter))
                {
                    cmd.Execute(field.Tag);
                    e.Handled = true; // Prevents the beep sound on Enter key press
                }
            };
            field.Enabled = cmd.CanExecute(field.Tag);
        }
    }

    public static void Commit(object obj, object dataContext)
    {
        foreach (var field in obj.GetType().GetFields(BindingFlags.NonPublic| BindingFlags.Public | BindingFlags.Instance))
        {
            if (GetCustomAttribute(field, typeof(DblClickBindingAttribute)) is DblClickBindingAttribute attr
                && field.GetValue(obj) is Control ctrl)
            {
                attr.Bind(dataContext, ctrl);
            }
        }
    }

}