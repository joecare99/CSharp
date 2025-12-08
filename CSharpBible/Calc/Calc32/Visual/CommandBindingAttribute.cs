// ***********************************************************************
// Assembly         : Calc32
// Author           : Mir
// Created          : 12-19-2021
//
// Last Modified By : Mir
// Last Modified On : 10-07-2022
// ***********************************************************************
// <copyright file="FrmCalc32Main.Designer.cs" company="JC-Soft">
//     Copyright © JC-Soft 2020
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Input;

namespace Calc32.Visual;

[AttributeUsage(AttributeTargets.Field)]
public class CommandBindingAttribute(string cmdName) : Attribute
{
    public string CommandName { get; } = cmdName;

    public void Bind(object viewModel, Control field)
    {
        if (viewModel.GetType().GetProperty(CommandName)?.GetValue(viewModel) is ICommand cmd)
        {
            cmd.CanExecuteChanged += (s, e) => field.Enabled = cmd.CanExecute(field.Tag);
            field.Click += (s, e) => cmd.Execute(field.Tag);
            field.Enabled = cmd.CanExecute(field.Tag);
        }
    }

    public static void Commit(object obj, object dataContext)
    {
        foreach (var field in obj.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance))
        {
            if (GetCustomAttribute(field, typeof(CommandBindingAttribute)) is CommandBindingAttribute attr
                && field.GetValue(obj) is Control ctrl)
            {
                attr.Bind(dataContext, ctrl);
            }
        }
    }

}