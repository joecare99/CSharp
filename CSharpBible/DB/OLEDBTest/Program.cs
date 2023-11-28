using System;
using System.Data;
using System.Data.OleDb;

class Program
{
    static void Main(string[] args)
    {
        OleDbEnumerator enumerator = new OleDbEnumerator();
        DataTable table = enumerator.GetElements();

        DisplayData(table);

        Console.WriteLine("Press any key to continue.");
        Console.ReadKey();
    }

    static void DisplayData(DataTable table)
    {
        foreach (DataRow row in table.Rows)
        {
            foreach (DataColumn col in table.Columns)
            {
                Console.WriteLine("{0} = {1}", col.ColumnName, row[col]);
            }
            Console.WriteLine("==================================");
        }
    }
}
