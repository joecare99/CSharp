using System;
using System.Drawing;
using System.Windows.Forms;

namespace ADO_Test.Views
{
    partial class Form2
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            buttonPanel = new Panel();
            addNewRowButton = new Button();
            deleteRowButton = new Button();
            songsDataGridView = new DataGridView();
            dataGridViewTextBoxColumn1 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn2 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn3 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn4 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn5 = new DataGridViewTextBoxColumn();
            buttonPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)songsDataGridView).BeginInit();
            SuspendLayout();
            // 
            // buttonPanel
            // 
            buttonPanel.Controls.Add(addNewRowButton);
            buttonPanel.Controls.Add(deleteRowButton);
            buttonPanel.Dock = DockStyle.Bottom;
            buttonPanel.Location = new Point(0, 629);
            buttonPanel.Name = "buttonPanel";
            buttonPanel.Size = new Size(1001, 50);
            buttonPanel.TabIndex = 0;
            // 
            // addNewRowButton
            // 
            addNewRowButton.Location = new Point(10, 10);
            addNewRowButton.Name = "addNewRowButton";
            addNewRowButton.Size = new Size(75, 32);
            addNewRowButton.TabIndex = 0;
            addNewRowButton.Text = "Add Row";
            addNewRowButton.Click += addNewRowButton_Click;
            // 
            // deleteRowButton
            // 
            deleteRowButton.Location = new Point(100, 10);
            deleteRowButton.Name = "deleteRowButton";
            deleteRowButton.Size = new Size(75, 32);
            deleteRowButton.TabIndex = 1;
            deleteRowButton.Text = "Delete Row";
            deleteRowButton.Click += deleteRowButton_Click;
            // 
            // songsDataGridView
            // 
            songsDataGridView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCellsExceptHeaders;
            songsDataGridView.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = Color.Navy;
            dataGridViewCellStyle1.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            dataGridViewCellStyle1.ForeColor = Color.White;
            dataGridViewCellStyle1.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = DataGridViewTriState.True;
            songsDataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            songsDataGridView.ColumnHeadersHeight = 34;
            songsDataGridView.Columns.AddRange(new DataGridViewColumn[] { dataGridViewTextBoxColumn1, dataGridViewTextBoxColumn2, dataGridViewTextBoxColumn3, dataGridViewTextBoxColumn4, dataGridViewTextBoxColumn5 });
            songsDataGridView.Dock = DockStyle.Fill;
            songsDataGridView.GridColor = Color.Black;
            songsDataGridView.Location = new Point(0, 0);
            songsDataGridView.MultiSelect = false;
            songsDataGridView.Name = "songsDataGridView";
            songsDataGridView.RowHeadersVisible = false;
            songsDataGridView.RowHeadersWidth = 62;
            songsDataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            songsDataGridView.Size = new Size(1001, 629);
            songsDataGridView.TabIndex = 0;
            songsDataGridView.CellFormatting += songsDataGridView_CellFormatting;
            // 
            // dataGridViewTextBoxColumn1
            // 
            dataGridViewTextBoxColumn1.HeaderText = "Release Date";
            dataGridViewTextBoxColumn1.MinimumWidth = 8;
            dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            dataGridViewTextBoxColumn1.Width = 150;
            // 
            // dataGridViewTextBoxColumn2
            // 
            dataGridViewTextBoxColumn2.HeaderText = "Track";
            dataGridViewTextBoxColumn2.MinimumWidth = 8;
            dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            dataGridViewTextBoxColumn2.Width = 150;
            // 
            // dataGridViewTextBoxColumn3
            // 
            dataGridViewTextBoxColumn3.HeaderText = "Title";
            dataGridViewTextBoxColumn3.MinimumWidth = 8;
            dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            dataGridViewTextBoxColumn3.Width = 150;
            // 
            // dataGridViewTextBoxColumn4
            // 
            dataGridViewTextBoxColumn4.HeaderText = "Artist";
            dataGridViewTextBoxColumn4.MinimumWidth = 8;
            dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            dataGridViewTextBoxColumn4.Width = 150;
            // 
            // dataGridViewTextBoxColumn5
            // 
            dataGridViewTextBoxColumn5.HeaderText = "Album";
            dataGridViewTextBoxColumn5.MinimumWidth = 8;
            dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
            dataGridViewTextBoxColumn5.Width = 150;
            // 
            // Form2
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1001, 679);
            Controls.Add(songsDataGridView);
            Controls.Add(buttonPanel);
            Name = "Form2";
            Text = "Form2";
            Load += Form2_Load;
            buttonPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)songsDataGridView).EndInit();
            ResumeLayout(false);
        }

        private Panel buttonPanel;
        private DataGridView songsDataGridView;
        private Button addNewRowButton;
        private Button deleteRowButton;

        #endregion

        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
    }
}