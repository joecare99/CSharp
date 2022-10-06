﻿// ***********************************************************************
// Assembly         : CSV_Viewer
// Author           : Mir
// Created          : 12-19-2021
//
// Last Modified By : Mir
// Last Modified On : 04-06-2020
// ***********************************************************************
// <copyright file="FrmCSVViewerMain.cs" company="JC Soft">
//     Copyright (c) HP Inc.. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Data;
using System.Windows.Forms;

namespace CSharpBible.CSV_Viewer.Visual
{
    /// <summary>
    /// Class FrmCSVViewerMain.
    /// Implements the <see cref="Form" />
    /// </summary>
    /// <seealso cref="Form" />
    public partial class FrmCSVViewerMain : Form
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FrmCSVViewerMain"/> class.
        /// </summary>
        public FrmCSVViewerMain()
        {
            InitializeComponent();
            MakeDataTables();
        }

        // Put the next line into the Declarations section.
        /// <summary>
        /// The data set
        /// </summary>
        private DataSet dataSet;
        /// <summary>
        /// The data grid1
        /// </summary>
        private DataGrid dataGrid1;

        /// <summary>
        /// Makes the data tables.
        /// </summary>
        private void MakeDataTables()
        {
            // Run all of the functions. 
            MakeParentTable();
            MakeChildTable();
            MakeDataRelation();
            BindToDataGrid();
        }

        /// <summary>
        /// Makes the parent table.
        /// </summary>
        private void MakeParentTable()
        {
            // Create a new DataTable.
            System.Data.DataTable table = new DataTable("ParentTable");
            // Declare variables for DataColumn and DataRow objects.
            DataColumn column;
            DataRow row;

            // Create new DataColumn, set DataType, 
            // ColumnName and add to DataTable.    
            column = new DataColumn();
            column.DataType = System.Type.GetType("System.Int32");
            column.ColumnName = "id";
            column.ReadOnly = true;
            column.AutoIncrement = true;
            column.Unique = true;
            // Add the Column to the DataColumnCollection.
            table.Columns.Add(column);

            // Create second column.
            column = new DataColumn();
            column.DataType = System.Type.GetType("System.String");
            column.ColumnName = "ParentItem";
            column.AutoIncrement = false;
            column.Caption = "ParentItem";
            column.ReadOnly = false;
            column.Unique = false;
            // Add the column to the table.
            table.Columns.Add(column);

            // Make the ID column the primary key column.
            DataColumn[] PrimaryKeyColumns = new DataColumn[1];
            PrimaryKeyColumns[0] = table.Columns["id"];
            table.PrimaryKey = PrimaryKeyColumns;

            // Instantiate the DataSet variable.
            dataSet = new DataSet();
            // Add the new DataTable to the DataSet.
            dataSet.Tables.Add(table);

            // Create three new DataRow objects and add 
            // them to the DataTable
            for (int i = 0; i <= 2; i++)
            {
                row = table.NewRow();
                row["id"] = i;
                row["ParentItem"] = "ParentItem " + i;
                table.Rows.Add(row);
            }
        }

        /// <summary>
        /// Makes the child table.
        /// </summary>
        private void MakeChildTable()
        {
            // Create a new DataTable.
            DataTable table = new DataTable("childTable");
            DataColumn column;
            DataRow row;

            // Create first column and add to the DataTable.
            column = new DataColumn();
            column.DataType = System.Type.GetType("System.Int32");
            column.ColumnName = "ChildID";
            column.AutoIncrement = true;
            column.Caption = "ID";
            column.ReadOnly = true;
            column.Unique = true;

            // Add the column to the DataColumnCollection.
            table.Columns.Add(column);

            // Create second column.
            column = new DataColumn();
            column.DataType = System.Type.GetType("System.String");
            column.ColumnName = "ChildItem";
            column.AutoIncrement = false;
            column.Caption = "ChildItem";
            column.ReadOnly = false;
            column.Unique = false;
            table.Columns.Add(column);

            // Create third column.
            column = new DataColumn();
            column.DataType = System.Type.GetType("System.Int32");
            column.ColumnName = "ParentID";
            column.AutoIncrement = false;
            column.Caption = "ParentID";
            column.ReadOnly = false;
            column.Unique = false;
            table.Columns.Add(column);

            dataSet.Tables.Add(table);

            // Create three sets of DataRow objects, 
            // five rows each, and add to DataTable.
            for (int i = 0; i <= 4; i++)
            {
                row = table.NewRow();
                row["childID"] = i;
                row["ChildItem"] = "Item " + i;
                row["ParentID"] = 0;
                table.Rows.Add(row);
            }
            for (int i = 0; i <= 4; i++)
            {
                row = table.NewRow();
                row["childID"] = i + 5;
                row["ChildItem"] = "Item " + i;
                row["ParentID"] = 1;
                table.Rows.Add(row);
            }
            for (int i = 0; i <= 4; i++)
            {
                row = table.NewRow();
                row["childID"] = i + 10;
                row["ChildItem"] = "Item " + i;
                row["ParentID"] = 2;
                table.Rows.Add(row);
            }
        }

        /// <summary>
        /// Makes the data relation.
        /// </summary>
        private void MakeDataRelation()
        {
            // DataRelation requires two DataColumn 
            // (parent and child) and a name.
            DataColumn parentColumn =
                dataSet.Tables["ParentTable"].Columns["id"];
            DataColumn childColumn =
                dataSet.Tables["ChildTable"].Columns["ParentID"];
            DataRelation relation = new
                DataRelation("parent2Child", parentColumn, childColumn);
            dataSet.Tables["ChildTable"].ParentRelations.Add(relation);
        }

        /// <summary>
        /// Binds to data grid.
        /// </summary>
        private void BindToDataGrid()
        {
            // Instruct the DataGrid to bind to the DataSet, with the 
            // ParentTable as the topmost DataTable.
            dataGrid1 = new DataGrid();
            dataGrid1.SetDataBinding(dataSet, "ParentTable");
            dataGrid1.Parent = panel1;
            dataGrid1.Dock = DockStyle.Fill;
            bindingSource1.DataSource = dataSet.Tables["ParentTable"];
        }
    }
}
