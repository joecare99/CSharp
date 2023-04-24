using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace DBTest1.Model
{
    public class BasicExample
    {
        public static async Task DoExampleAsync(params object?[] args)
        {
            var connString = "Server={0};User ID={1};Password={2};Database={3}";

            using var conn = new MySqlConnection(String.Format(connString, args));
            conn.StateChange += Conn_StateChange;
            try
            {
                await conn.OpenAsync();
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
                cmd.CommandText = "INSERT INTO data (some_field) VALUES (@p)";
                cmd.Parameters.AddWithValue("p", "Hello world");
                await cmd.ExecuteNonQueryAsync();
            }

            // Retrieve all rows
            using (var cmd = new MySqlCommand("SELECT some_field FROM data", conn))
            using (var reader = await cmd.ExecuteReaderAsync())
                while (await reader.ReadAsync())
                    Console.WriteLine(reader.GetString(0));
        }

        private static void Conn_StateChange(object sender, System.Data.StateChangeEventArgs e)
        {
            Console.WriteLine($"{sender}:{e.CurrentState}");
        }
    }
}
