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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace Views;

[AttributeUsage(AttributeTargets.Field)]
public class TextBindingAttribute(string cmdName) : Attribute
{
    private static readonly Dictionary<Control, bool> _validationState = new();
    private static readonly HashSet<Control> _hookedParents = new();

    public string PropertyName { get; } = cmdName;

    public void Bind(object viewModel, Control field)
    {
        if (viewModel.GetType().GetProperty(PropertyName) is PropertyInfo pi)
        {
            field.Text = pi.GetValue(viewModel)?.ToString();
            if (viewModel is INotifyPropertyChanged npc)
                npc.PropertyChanged += (s, e) => { if (e.PropertyName == PropertyName) field.Text = s!.GetType().GetProperty(e.PropertyName)?.GetValue(s)?.ToString(); };
            if (pi.CanWrite && pi.PropertyType == typeof(string))
            {
                HookParentPaint(field);
                field.TextChanged += (s, e) => UpdateModelAndValidation(viewModel, field, pi);
            }
        }
    }

    private void UpdateModelAndValidation(object viewModel, Control field, PropertyInfo pi)
    {
        string? error = null;
        try
        {
            pi.SetValue(viewModel, field.Text);
            error = GetValidationError(viewModel, pi, field.Text);
        }
        catch (Exception ex)
        {
            error = ex.InnerException?.Message ?? ex.Message;
        }

        SetValidationState(field, !string.IsNullOrWhiteSpace(error));
    }

    private string? GetValidationError(object viewModel, PropertyInfo pi, string value)
    {
        if (viewModel is IDataErrorInfo dataErrorInfo)
        {
            var error = dataErrorInfo[PropertyName];
            if (!string.IsNullOrWhiteSpace(error))
                return error;
        }

        if (viewModel is INotifyDataErrorInfo notifyDataErrorInfo)
        {
            var errors = notifyDataErrorInfo.GetErrors(PropertyName) as IEnumerable;
            if (errors is not null)
            {
                var firstError = errors.Cast<object?>().Select(e => e?.ToString()).FirstOrDefault(e => !string.IsNullOrWhiteSpace(e));
                if (!string.IsNullOrWhiteSpace(firstError))
                    return firstError;
            }
        }

        return null;
    }

    private static void HookParentPaint(Control field)
    {
        if (field.Parent is not Control parent)
            return;

        if (_hookedParents.Add(parent))
            parent.Paint += Parent_Paint;
    }

    private static void Parent_Paint(object sender, PaintEventArgs e)
    {
        if (sender is not Control parent)
            return;

        foreach (var entry in _validationState.ToArray())
        {
            var control = entry.Key;
            var hasError = entry.Value;
            if (!hasError || control.Parent != parent || !control.Visible)
                continue;

            var rect = control.Bounds;
            rect.Inflate(1, 1);
            ControlPaint.DrawBorder(e.Graphics, rect, Color.Red, ButtonBorderStyle.Solid);
        }
    }

    private static void SetValidationState(Control field, bool hasError)
    {
        _validationState[field] = hasError;
        if (field.Parent is not null)
        {
            var rect = field.Bounds;
            rect.Inflate(2, 2);
            field.Parent.Invalidate(rect);
        }
    }

    public static void Commit(object obj, object dataContext)
    {
        foreach (var field in obj.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance))
        {
            if (GetCustomAttribute(field, typeof(TextBindingAttribute)) is TextBindingAttribute attr
                && field.GetValue(obj) is Control ctrl)
            {
                attr.Bind(dataContext, ctrl);
            }
        }
    }

}
