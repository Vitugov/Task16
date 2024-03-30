using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Windows.Controls;


namespace Task16.Other
{
    public static class DBConnection
    {
        public static async Task<bool> IsMSSQLConnectionAccessible()
        {
            var connectionString = GetMsSqlConnectionString();
            var connection = new SqlConnection(connectionString);
            try
            {
                connection.Open();
                connection.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static async Task<bool> IsOleDbConnectionAccessible()
        {
            var connectionString = GetOleDbConnectionString();
            var connection = new OleDbConnection(connectionString);
            try
            {
                connection.Open();
                connection.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static string GetMsSqlConnectionString()
        {
            var sqlConnectionBuilder = new SqlConnectionStringBuilder()
            {
                DataSource = "MAXASUS",
                InitialCatalog = "MSSQLDB",
                IntegratedSecurity = false,
                UserID = "Admin",
                Password = "Qwe123",
                ConnectTimeout = 1
            };
            return sqlConnectionBuilder.ConnectionString;
        }

        public static string GetOleDbConnectionString()
        {
            var connectionStringBuilder = new OleDbConnectionStringBuilder()
            {
                Provider = "Microsoft.ACE.OLEDB.12.0",
                DataSource = @"MSAccessDB.accdb",
                PersistSecurityInfo = true,
            };
            connectionStringBuilder["Jet OLEDB:Database Password"] = "Qwe123";
            return connectionStringBuilder.ConnectionString;
    }
    }
}
