// ***********************************************************************
// Assembly         : WfSystem
// Author           : Mir
// Created          : 12-19-2021
//
// Last Modified By : Mir
// Last Modified On : 10-07-2022
// ***********************************************************************
// <copyright file="ListBindingAttribute.cs" company="JC-Soft">
//     Copyright © JC-Soft 2020
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace Views;

[AttributeUsage(AttributeTargets.Field)]
public class ListBindingAttribute(string itmsName, string selItemName) : Attribute
{
    public string PropertyName { get; } = itmsName;
    public string Property2Name { get; } = selItemName;

    public void Bind(object viewModel, ListControl field)
    {
        if (viewModel.GetType().GetProperty(PropertyName) is PropertyInfo pi)
        {
            ApplyItems(field, pi.GetValue(viewModel) as IEnumerable<object>);
            if (pi.GetValue(viewModel) is INotifyCollectionChanged npc)
            {
                npc.CollectionChanged += (s, e) => ApplyItems(field, (s as IEnumerable<object>) ?? []);
            }
        }

        if (viewModel.GetType().GetProperty(Property2Name) is PropertyInfo pi2)
        {
            ApplySelectedItem(field, pi2.GetValue(viewModel));
            if (pi2.CanWrite)
            {
                field.SelectedValueChanged += (s, e) => pi2.SetValue(viewModel, GetSelectedItem(field));
            }

            if (viewModel is INotifyPropertyChanged inpc)
            {
                inpc.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == Property2Name)
                    {
                        ApplySelectedItem(field, pi2.GetValue(viewModel));
                    }
                };
            }
        }
    }

    private static object? GetSelectedItem(ListControl field)
        => field switch
        {
            ComboBox cbx => cbx.SelectedItem,
            ListBox lbx => lbx.SelectedItem,
            _ => field.SelectedValue,
        };

    private static void ApplySelectedItem(ListControl field, object? selectedItem)
    {
        switch (field)
        {
            case ComboBox cbx when !Equals(cbx.SelectedItem, selectedItem):
                cbx.SelectedItem = selectedItem;
                break;
            case ListBox lbx when !Equals(lbx.SelectedItem, selectedItem):
                lbx.SelectedItem = selectedItem;
                break;
        }
    }

    private static void ApplyItems(ListControl field, IEnumerable<object>? items)
    {
        object[] values = items?.ToArray() ?? [];
        switch (field)
        {
            case ComboBox cbx:
                cbx.Items.Clear();
                cbx.Items.AddRange(values);
                break;
            case ListBox lbx:
                lbx.Items.Clear();
                lbx.Items.AddRange(values);
                break;
        }
    }

    public static void Commit(object obj, object dataContext)
    {
        foreach (var field in obj.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance))
        {
            if (GetCustomAttribute(field, typeof(ListBindingAttribute)) is ListBindingAttribute attr
                && field.GetValue(obj) is ListControl ctrl)
            {
                attr.Bind(dataContext, ctrl);
            }
        }
    }

}
