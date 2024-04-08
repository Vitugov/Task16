using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Windows.Controls;
using System.Data.Common;


namespace Task16.Other
{
    public class DBConnections
    {
        public static DBConnections Instance { get; }
        public SqlConnection SqlConnection { get; }
        public OleDbConnection OleDbConnection { get; }
        public Task<bool> IsSqlAccessible => IsAccessible(SqlConnection);
        public Task<bool> IsOleDbAccessible => IsAccessible(OleDbConnection);

        static DBConnections()
        {
            Instance = new DBConnections();
        }

        public DBConnections()
        {
            OleDbConnection = GetOleDbConnection();
            SqlConnection = GetSqlConnection();
        }
        public async Task<bool> IsAccessible<T>(T connection)
            where T : DbConnection
        {
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

        public static SqlConnection GetSqlConnection()
        {
            var connectionString = GetMsSqlConnectionString();
            return new SqlConnection(connectionString);
        }

        public static OleDbConnection GetOleDbConnection()
        {
            var connectionString = GetOleDbConnectionString();
            return new OleDbConnection(connectionString);
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
                Pooling = true,
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
