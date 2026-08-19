using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Windows.Forms;

namespace ADO_Test.Views
{
    public partial class Form3 : Form
    {
        OleDbConnection database;
        OleDbConnectionStringBuilder connectionStringBuilder = new();
        OleDbDataAdapter dataAdapter;
        OleDbCommand SQLQuery = new();
        DataTable data = new();

        public Form3()
        {
            InitializeComponent();
            // initiate DB connection
            connectionStringBuilder.Provider = "Microsoft.ACE.OLEDB.16.0";
            connectionStringBuilder.DataSource = "Resource\\Gen_Plusdaten.mdb";
            connectionStringBuilder.PersistSecurityInfo = false;
            //connectionStringBuilder.Add("Jet OLEDB:Database Password", "");

            try
            {
                database = new OleDbConnection(connectionStringBuilder.ConnectionString);
                database.Open();               

                System.Diagnostics.Debug.WriteLine($"Connection State: {database.State}");
                System.Diagnostics.Debug.WriteLine($"Connection Database:  {database.Database}");
                System.Diagnostics.Debug.WriteLine($"Connection DataSource:  {database.DataSource}");
                System.Diagnostics.Debug.WriteLine($"Connection Provider:  {database.Provider}");
                System.Diagnostics.Debug.WriteLine($"Connection Version:  {database.ServerVersion}");
                System.Diagnostics.Debug.WriteLine($"Connection Site:  {database.Site}");

                DisplayData(database.GetSchema(), new() {"CollectionName" });
                DisplayData(database.GetSchema("DataTypes"), new() { "TypeName","DataType" });
                DisplayData(database.GetSchema("Tables"), new() { "TABLE_NAME" });
                DisplayData(database.GetSchema("Indexes"), new() { "TABLE_NAME" ,"INDEX_NAME", "ORDINAL_POSITION", "COLUMN_NAME" });
                //SQL query to list movies
                string queryString = "Personen";
                loadDataGrid(queryString);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }
        static void DisplayData(DataTable table,List<string>? filter=null)
        {
            foreach (DataRow row in table.Rows)
            {
                foreach (DataColumn col in table.Columns)
                    if (filter == null || filter.Contains(col.ColumnName))
                {
                    System.Diagnostics.Debug.WriteLine("{0} = {1}", col.ColumnName, row[col]);
                }
                if (filter == null || filter.Count>1)
                System.Diagnostics.Debug.WriteLine("==================================");
            }
            if (filter != null && filter.Count == 1)
                System.Diagnostics.Debug.WriteLine("==================================");
        }

        public void loadDataGrid(string sqlQueryString)
        {

           dataGridView1.DataSource = null;
            SQLQuery.Connection = null;
            dataGridView1.Columns.Clear(); // <-- clear columns

            SQLQuery.CommandText = sqlQueryString;
            SQLQuery.Connection = database;
            SQLQuery.CommandType = CommandType.TableDirect;
            SQLQuery.UpdatedRowSource = UpdateRowSource.FirstReturnedRecord;
            SQLQuery.Prepare();

            dataAdapter = new OleDbDataAdapter(SQLQuery);
            dataAdapter.UpdateCommand = new OleDbCommandBuilder(dataAdapter).GetUpdateCommand();
            dataAdapter.DeleteCommand = new OleDbCommandBuilder(dataAdapter).GetDeleteCommand();
            dataAdapter.InsertCommand = new OleDbCommandBuilder(dataAdapter).GetInsertCommand();
            dataAdapter.FillLoadOption = LoadOption.OverwriteChanges;
            dataAdapter.Fill(data);
            data.RowChanged += Row_Changed;
            data.RowDeleted += Row_Deleted;

            dataGridView1.DataSource = data;

            dataGridView1.AllowUserToAddRows = false; // <-- remove the null line
            dataGridView1.ReadOnly = false;          // <-- so the user cannot type 

            // following code defines column sizes
            dataGridView1.Columns[0].Visible = false;
            dataGridView1.Columns[1].Width = 340;
            dataGridView1.Columns[3].Width = 55;
            dataGridView1.Columns[4].Width = 50;
            dataGridView1.Columns[5].Width = 80;

            // insert edit button into datagridview
            editButton = new DataGridViewButtonColumn();
            editButton.HeaderText = "Edit";
            editButton.Text = "Edit";
            editButton.UseColumnTextForButtonValue = true;
            editButton.Width = 80;
            dataGridView1.Columns.Add(editButton);

            // insert delete button to datagridview
            deleteButton = new DataGridViewButtonColumn();
            deleteButton.HeaderText = "Delete";
            deleteButton.Text = "Delete";
            deleteButton.UseColumnTextForButtonValue = true;
            deleteButton.Width = 80;
            dataGridView1.Columns.Add(deleteButton);
        }

        private void Row_Deleted(object sender, DataRowChangeEventArgs e)
        {
            if (sender is DataTable dt)
                dataAdapter.Update(new[] { e.Row });
        }

        private void _DataAdapter_RowUpdated(object sender, OleDbRowUpdatedEventArgs e)
        {
            if (sender is OleDbDataAdapter dt)
                ;
        }

        private void Row_Changed(object sender, DataRowChangeEventArgs e)
        {
            if (sender is DataTable dt)
                dataAdapter.Update(new[] { e.Row });
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            string queryString = "SELECT movieID, Title, Publisher,  Previewed, Year, Type FROM movie, movieType WHERE movietype.typeID = movie.typeID";

            int currentRow = e.RowIndex;
            try
            {
                string movieIDString = dataGridView1[7, currentRow].Value.ToString();
                int movieIDInt = int.Parse(movieIDString);
            }
            catch (Exception ex) { }
            // edit button
            if (dataGridView1.Columns[e.ColumnIndex] == editButton && currentRow >= 0)
            {
                string title = dataGridView1[1, currentRow].Value.ToString();
                string publisher = dataGridView1[2, currentRow].Value.ToString();
                string previewed = dataGridView1[3, currentRow].Value.ToString();
                string year = dataGridView1[4, currentRow].Value.ToString();
                string type = dataGridView1[5, currentRow].Value.ToString();
                /*
                Form2 f2 = new Form2();
                f2.title = title;
                f2.publisher = publisher;
                f2.previewed = previewed;
                f2.year = year;
                f2.type = type;
                f2.movieID = movieIDInt;
                f2.Show();
                dataGridView1.Update();
                dataGridView1.Refresh();
                */
            }
        }
    }
}
