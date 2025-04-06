﻿using CommunityToolkit.Mvvm.ComponentModel;
using MdbBrowser.ViewModels.Interfaces;
using MdbBrowser.Models;
using MVVM.View.Extension;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using BaseLib.Helper;

namespace MdbBrowser.ViewModels
{
    public partial class TableViewViewModel : ObservableValidator, ITableViewViewModel
    {
        [ObservableProperty]
        string _tableName = "<TableName>";
        [ObservableProperty]
        ObservableCollection<DBMetaData> _columns = new() {
            new(){ Name = "Column1", Kind = EKind.Column, Data = new List<string>() { "1", "2", "3" } },
            new(){ Name = "Column2", Kind = EKind.Column, Data = new List<string>() { "1", "2", "3" } },
            new(){ Name = "Column3", Kind = EKind.Column, Data = new List<string>() { "1", "2", "3" } },
        };

        [ObservableProperty]
        DataTable _tableData = new()
        {
            Columns = { "Column1", "Column2", "Column3" },
            Rows = { { "1", "2", "3" }, { "1", "2", "3" }, { "1", "2", "3" } }
        };

        private IDBViewViewModel? _parent;

        public TableViewViewModel() : this(IoC.GetRequiredService<IDBViewViewModel>())
        {
        }
        public TableViewViewModel(IDBViewViewModel parent)
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
                    && md.Kind == EKind.Table)
                {
                    return md.Name;
                }
                else
                    return "";
            }
        }

        partial void OnTableNameChanged(string value)
        {
            Columns.Clear();
            if (value != "")
            {
                foreach (var d in _parent?.dBModel?.QueryTable(value) ?? new List<DBMetaData>())
                    Columns.Add(d);
                //    OnPropertyChanged(nameof(Columns));
                TableData = _parent?.dBModel?.QueryTableData(value) ?? new DataTable();
            }
            else
                TableData = new DataTable();

        }

    }
}
