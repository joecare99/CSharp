using MySql.Data.MySqlClient;
using System;

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

            // Retrieve all rows
            using (var cmd = new MySqlCommand("SELECT * FROM Testtable", conn))
            using (var reader = cmd.ExecuteReader())
                while (reader.Read()) { 
                    for (int i = 0; i < reader.FieldCount; i++)
                        Console.Write(reader.GetString(i) + "\t");
                    Console.WriteLine();
                }
        }

        private static void Conn_StateChange(object sender, System.Data.StateChangeEventArgs e)
        {
            Console.WriteLine($"{sender}:{e.CurrentState}");
        }
    }
}
