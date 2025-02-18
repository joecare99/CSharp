// ***********************************************************************
// Assembly         : WfSystem
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
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Input;

namespace Views;

[AttributeUsage(AttributeTargets.Field)]
public class TextBindingAttribute(string cmdName) : Attribute
{
    public string PropertyName { get; } = cmdName;

    public void Bind(object viewModel, Control field)
    {
        if (viewModel.GetType().GetProperty(PropertyName) is PropertyInfo pi )
        {
            field.Text = pi.GetValue(viewModel)?.ToString();
            if (viewModel is INotifyPropertyChanged npc)
                npc.PropertyChanged += (s, e) => { if (e.PropertyName == PropertyName) field.Text = s!.GetType().GetProperty(e.PropertyName)?.GetValue(s)?.ToString(); };
        }
    }

    public static void Commit(object obj, object dataContext)
    {
        foreach (var field in obj.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance))
        {
            if (GetCustomAttribute(field, typeof(TextBindingAttribute)) is TextBindingAttribute attr
                && field.GetValue(obj) is Control ctrl)
            {
                attr.Bind(dataContext, ctrl);
            }
        }
    }

}