using CommunityToolkit.Mvvm.ComponentModel;
using MdbBrowser.ViewModels.Interfaces;
using MdbBrowser.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;

namespace MdbBrowser.ViewModels
{
    public partial class TableViewViewModel : ObservableValidator
    {
        [ObservableProperty]
        string _tableName = "";
        [ObservableProperty]
        ObservableCollection<DBMetaData> _columns = new() {
            new(){ Name = "Column1", Kind = EKind.Column, Data = new List<string>() { "1", "2", "3" } },
            new(){ Name = "Column2", Kind = EKind.Column, Data = new List<string>() { "1", "2", "3" } },
            new(){ Name = "Column3", Kind = EKind.Column, Data = new List<string>() { "1", "2", "3" } },
        };

        [ObservableProperty]
        DataTable _tableData = new() { 
            Columns = { "Column1", "Column2", "Column3" },
            Rows = { { "1", "2", "3" }, { "1", "2", "3" }, { "1", "2", "3" } }
        };

        private IDBViewViewModel? _parent;

        public TableViewViewModel()
        {
            _parent = DBViewViewModel.This;
            if (_parent != null)
            {
                TableName = _parent.SelectedEntry?.Name ?? "";
                _parent.PropertyChanged += (sender, e) =>
                {
                    if (e.PropertyName == "SelectedEntry"
                    && _parent.SelectedEntry?.Kind == EKind.Table)
                    {
                        TableName = _parent.SelectedEntry?.Name ?? "";
                    }
                };
            }
        }

        partial void OnTableNameChanged(string value)
        {
            Columns.Clear();
            foreach(var d in _parent?.dBModel?.QueryTable(value) ?? new List<DBMetaData>())
                Columns.Add(d);
        //    OnPropertyChanged(nameof(Columns));
            TableData = _parent?.dBModel?.QueryTableData(value) ?? new DataTable();
        }

    }
}
