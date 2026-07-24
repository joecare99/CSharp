// ***********************************************************************
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
using GenFreeWin;
using GenFree.Interfaces.UI;
using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;

namespace GenFreeWin.Attributes;

[AttributeUsage(AttributeTargets.Field)]
public class ApplTextBindingAttribute(string? cmdName) : Attribute
{
    public ApplTextBindingAttribute(EUserText cmdText) : this(null)
    {
        PropertyText = cmdText;
    }

    public string PropertyName { get; } = cmdName;
    public EUserText? PropertyText { get; }

    public void Bind(INotifyPropertyChanged viewModel, Control field, IApplUserTexts strings)
    {
        if (PropertyText != null)
        {
            field.Text = strings[PropertyText.Value];
            strings.TextsChanged += (s, e) => field.Text = strings[PropertyText.Value];
        }
        else
            if (PropertyName != null 
            && viewModel.GetType().GetProperty(PropertyName) is PropertyInfo pi 
            && pi.PropertyType == typeof(EUserText))
        {
            field.Text = strings[pi.GetValue(viewModel)!];
            strings.TextsChanged += (s, e) => { field.Text = strings[pi.GetValue(viewModel)!]; };
            if (viewModel is INotifyPropertyChanged npc)
                npc.PropertyChanged += (s, e) => { if (e.PropertyName == PropertyName) field.Text = strings[s!.GetType().GetProperty(e.PropertyName)?.GetValue(s)!]; };
            }
    }

    public static void Commit(IComponent obj, INotifyPropertyChanged dataContext, IApplUserTexts strings)
    {
        foreach (var field in obj.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance))
        {
            if (GetCustomAttribute(field, typeof(ApplTextBindingAttribute)) is ApplTextBindingAttribute attr
                && field.GetValue(obj) is Control ctrl)
            {
                attr.Bind(dataContext, ctrl, strings);
            }
        }
    }

}