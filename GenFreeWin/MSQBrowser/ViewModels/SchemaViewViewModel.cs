﻿using CommunityToolkit.Mvvm.ComponentModel;
using MSQBrowser.ViewModels.Interfaces;
using MSQBrowser.Models;
using MVVM.View.Extension;
using System;
using System.Data;
using BaseLib.Helper;

namespace MSQBrowser.ViewModels
{
    public partial class SchemaViewViewModel : ObservableValidator, ISchemaViewViewModel
    {
        [ObservableProperty]
        string _tableName = "<TableName>";
        [ObservableProperty]
        DataTable _tableData = new()
        {
            Columns = { "Column1", "Column2", "Column3" },
            Rows = { { "1", "2", "3" }, { "1", "2", "3" }, { "1", "2", "3" } }
        };

        private IDBViewViewModel? _parent;
        public SchemaViewViewModel() : this(IoC.GetRequiredService<IDBViewViewModel>()) { }
        public SchemaViewViewModel(IDBViewViewModel parent)
        {
            _parent = parent;
            if (_parent != null)
            {
                TableName = GetTableName();
                _parent.PropertyChanged += (sender, e) => TableName = GetTableName(() => e.PropertyName == "SelectedEntry");
            }

            string GetTableName(Func<bool>? predicate = null)
            {
                if ((predicate?.Invoke() ?? true)
                    && _parent.SelectedEntry is DBMetaData md
                    && md.Kind == EKind.Schema)
                {
                    return md.Name;
                }
                else
                    return "";
            }
        }

        partial void OnTableNameChanged(string value)
        {
            if (value != "")
            {
                TableData = _parent?.dBModel?.QuerySchema(value) ?? new DataTable();
            }
            else
                TableData = new DataTable();
        }
    }
}
