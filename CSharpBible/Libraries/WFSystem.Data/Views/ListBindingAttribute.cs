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
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace Views;

[AttributeUsage(AttributeTargets.Field)]
public class ListBindingAttribute(string itmsName,string selItemName) : Attribute
{
    public string PropertyName { get; } = itmsName;
    public string Property2Name { get; } = selItemName;

    public void Bind(object viewModel, ListControl field)
    {
        if (viewModel.GetType().GetProperty(PropertyName) is PropertyInfo pi )
        {
            if (field is ComboBox cbx)
            {
                cbx.Items.AddRange((pi.GetValue(viewModel) as IEnumerable<object>).ToArray());
                if (pi.GetValue(viewModel) is INotifyCollectionChanged npc)
                    npc.CollectionChanged += (s, e) => { cbx.Items.Clear();cbx.Items.AddRange((s as IEnumerable<object>).ToArray()); };
            }
            else if (field is ListBox lbx)
            {
                lbx.Items.AddRange((pi.GetValue(viewModel) as IEnumerable<object>).ToArray());
                if (pi.GetValue(viewModel) is INotifyCollectionChanged npc)
                    npc.CollectionChanged += (s, e) => { lbx.Items.Clear(); lbx.Items.AddRange((s as IEnumerable<object>).ToArray()); };
            }
        }
        if (viewModel.GetType().GetProperty(Property2Name) is PropertyInfo pi2)
        {
            if (pi2.CanWrite)
            {
                field.SelectedValueChanged += (s, e) => pi2.SetValue(viewModel, field.SelectedValue);
            }
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