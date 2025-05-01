using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;

namespace WindowsFormsApp1.src
{
    internal class SQLHandler
    {
        private string sqliteConnectionString;
        private string sqlServerConnectionString;

        // Constructor to initialize connection strings
        public SQLHandler(string sqliteConnectionString, string sqlServerConnectionString)
        {
            this.sqliteConnectionString = sqliteConnectionString;
            this.sqlServerConnectionString = sqlServerConnectionString;
        }

        // Method to execute non-query operations (INSERT, UPDATE, DELETE) for SQLite
        public void ExecuteSQLiteNonQuery(string query)
        {
            using (SQLiteConnection connection = new SQLiteConnection(sqliteConnectionString))
            {
                try
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("SQLite Error: " + ex.Message);
                }
            }
        }

        // Method to execute non-query operations (INSERT, UPDATE, DELETE) for SQL Server
        public void ExecuteSQLServerNonQuery(string query)
        {
            using (SqlConnection connection = new SqlConnection(sqlServerConnectionString))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("SQL Server Error: " + ex.Message);
                }
            }
        }

        // Method to retrieve data from SQLite
        public DataTable GetSQLiteData(string query)
        {
            DataTable dataTable = new DataTable();
            using (SQLiteConnection connection = new SQLiteConnection(sqliteConnectionString))
            {
                try
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(command))
                    {
                        adapter.Fill(dataTable);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("SQLite Error: " + ex.Message);
                }
            }
            return dataTable;
        }

        // Method to retrieve data from SQL Server
        public DataTable GetSQLServerData(string query)
        {
            DataTable dataTable = new DataTable();
            using (SqlConnection connection = new SqlConnection(sqlServerConnectionString))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(dataTable);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("SQL Server Error: " + ex.Message);
                }
            }
            return dataTable;
        }
    }
}
