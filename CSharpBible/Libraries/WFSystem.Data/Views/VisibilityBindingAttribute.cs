﻿// ***********************************************************************
// Assembly         : WfSystem
// Author           : Mir
// Created          : 12-19-2021
//
// Last Modified By : Mir
// Last Modified On : 10-07-2022
// ***********************************************************************
// <copyright file="VisibilityBindingAttribute.cs" company="JC-Soft">
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
public class VisibilityBindingAttribute(string cmdName) : Attribute
{
    public string PropertyName { get; } = cmdName;

    public void Bind(object viewModel, Control field)
    {
        if (viewModel.GetType().GetProperty(PropertyName) is PropertyInfo pi && pi.PropertyType==typeof(bool) )
        {
            field.Visible = (bool)pi.GetValue(viewModel)!;
            if (viewModel is INotifyPropertyChanged npc)
                npc.PropertyChanged += (s, e) => { if (e.PropertyName == PropertyName) field.Visible = (bool)s!.GetType().GetProperty(e.PropertyName)?.GetValue(s)!; };
        }
    }

    public static void Commit(object obj, object dataContext)
    {
        foreach (var field in obj.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance))
        {
            if (GetCustomAttribute(field, typeof(VisibilityBindingAttribute)) is VisibilityBindingAttribute attr
                && field.GetValue(obj) is Control ctrl)
            {
                attr.Bind(dataContext, ctrl);
            }
        }
    }

}