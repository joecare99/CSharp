//using MySql.Data.MySqlClient;
using MySqlConnector;
using System;
using System.Data;

namespace DBTest1.Model
{
    public class BasicExample
    {
        public static void DoExample(params string[] args)
        {
            var builder = new MySqlConnectionStringBuilder
            {
                Server = args[0],               
                UserID = args[1],
                Password = args[2],
                Database = args[3],
                CharacterSet ="UTF8"             
            };

            using var conn = new MySqlConnection(builder.ConnectionString);
            conn.StateChange += Conn_StateChange;
            try
            {
                conn.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                throw;
            }

            // Insert some data
            using (var cmd = new MySqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "INSERT INTO Testtable (Description) VALUES (@d)";
                cmd.Parameters.AddWithValue("d", "Héllo wörld");
                cmd.ExecuteNonQuery();
            }

           var s= conn.GetSchema();
            foreach (var col in s.Columns)
                Console.Write($"{col}\t");
            Console.WriteLine();
            Console.WriteLine("=======================");
            foreach (DataRow row in s.Rows)
            {
               foreach(var col in row.ItemArray)
                    Console.Write($"{col}\t");
                Console.WriteLine();
            }


            s = conn.GetSchema("Tables");
            foreach (var col in s.Columns)
                Console.Write($"{col}\t");
            Console.WriteLine();
            Console.WriteLine("=======================");
            foreach (DataRow row in s.Rows)
            {
                foreach (var col in row.ItemArray)
                    Console.Write($"{col}\t");
                Console.WriteLine();
            }


            // Retrieve all rows
            var xFirst = true;
            using (var cmd = new MySqlCommand("SELECT * FROM Testtable", conn))
            using (var reader = cmd.ExecuteReader())                                
                while (reader.Read())
                {
                    if ( xFirst)
                    {
                        for (int i = 0; i < reader.FieldCount; i++)
                            Console.Write($"{reader.GetName(i)}\t"); 
                        Console.WriteLine();
                        for (int i = 0; i < reader.FieldCount; i++)
                            Console.Write($"{new string('=',reader.GetName(i).Length)}\t");
                        Console.WriteLine();
                        xFirst = false;
                    }

                    for (int i = 0; i < reader.FieldCount; i++)
                        switch (reader.GetDataTypeName(i))
                        {
                            case "VARCHAR": Console.Write($"{reader.GetString(i)}\t"); break;
                            case "INT": Console.Write($"{reader.GetInt32(i)}\t"); break;
                        }
                    Console.WriteLine();
                }            
        }

        private static void Conn_StateChange(object sender, System.Data.StateChangeEventArgs e)
        {
            Console.WriteLine($"{sender}:{e.CurrentState}");
        }
    }
}
