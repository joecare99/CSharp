// ***********************************************************************
// Assembly         : WfSystem
// Author           : Mir
// Created          : 12-19-2021
//
// Last Modified By : Mir
// Last Modified On : 10-07-2022
// ***********************************************************************
// <copyright file="TextBindingAttribute.cs" company="JC-Soft">
//     Copyright © JC-Soft 2020
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;

namespace Views;

[AttributeUsage(AttributeTargets.Field)]
public class CheckedBindingAttribute(string cmdName) : Attribute
{
    public string PropertyName { get; } = cmdName;

    public void Bind(object viewModel, Control field)
    {
        if (viewModel.GetType().GetProperty(PropertyName) is PropertyInfo pi )
        {
            if (field is CheckBox cb)
            {
                cb.Checked = (bool)pi.GetValue(viewModel)!;
                if (viewModel is INotifyPropertyChanged npc)
                    npc.PropertyChanged += (s, e) => { if (e.PropertyName == PropertyName) cb.Checked = (bool)s!.GetType().GetProperty(e.PropertyName)?.GetValue(s)!; };
                if (pi.CanWrite)
                {
                    cb.CheckedChanged += (s, e) => pi.SetValue(viewModel, cb.Checked);
                }
            }
            else if (field is RadioButton rb)
            {
                rb.Checked = (bool)pi.GetValue(viewModel)!;
                if (viewModel is INotifyPropertyChanged npc)
                    npc.PropertyChanged += (s, e) => { if (e.PropertyName == PropertyName) rb.Checked = (bool)s!.GetType().GetProperty(e.PropertyName)?.GetValue(s)!; };
                if (pi.CanWrite)
                {
                    rb.CheckedChanged += (s, e) => pi.SetValue(viewModel, rb.Checked);
                }
            }
        }
    }

    public static void Commit(object obj, object dataContext)
    {
        foreach (var field in obj.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance))
        {
            if (GetCustomAttribute(field, typeof(CheckedBindingAttribute)) is CheckedBindingAttribute attr
                && field.GetValue(obj) is Control ctrl)
            {
                attr.Bind(dataContext, ctrl);
            }
        }
    }

}