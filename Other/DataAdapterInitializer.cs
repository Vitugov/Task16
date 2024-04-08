using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime;
using System.Data.Common;

namespace Task16.Other
{
    public class DataAdapterInitializer
    {
        //public delegate TOut DataAdapterBuilderM<TOut, TIn1, TIn2>
        //    (StringCommands stringCommands, TIn1[] dbParameters, TIn2 connection, TOut dataAdapter)
        //    where TIn1 : DbParameter
        //    where TIn2 : DbConnection
        //    where TOut : DbDataAdapter, IDbDataAdapter;
        
        public static DataAdapterInitializer Instance { get; }

        public SqlDataAdapter SqlDataAdapter { get; }
        public OleDbDataAdapter OleDbDataAdapter { get; }

        private SqlConnection sqlConnection => DBConnections.Instance.SqlConnection;
        private OleDbConnection oleDbConnection => DBConnections.Instance.OleDbConnection;
        
        static DataAdapterInitializer()
        {
            Instance = new DataAdapterInitializer();
        }

        public DataAdapterInitializer()
        {
            var sqlParameters = GetSqlParameters();
            var sqlStringCommands = GetSqlStringCommands();
            SqlDataAdapter = FillOutDataAdapter(sqlStringCommands, sqlParameters,
                sqlConnection, new SqlDataAdapter());

            var oleDbParameters = GetOleDbParameters();
            var OleDbStringCommands = GetOleDbStringCommands();
            OleDbDataAdapter = FillOutDataAdapter(OleDbStringCommands, oleDbParameters,
                oleDbConnection, new OleDbDataAdapter());
        }

        private StringCommands GetSqlStringCommands()
        {
            var sqlStringCommands = new StringCommands();
            sqlStringCommands.Select =
                @"SELECT *
                  FROM Clients
                  ORDER BY Id;";
            sqlStringCommands.Insert =
                @"INSERT INTO Clients (Surname, FirstName, Patronymic, TelephoneNumber, Email)
                  VALUES (@Surname, @FirstName, @Patronymic, @TelephoneNumber, @Email);
                  SET @Id = @@iDENTITY;";
            sqlStringCommands.Update =
                @"UPDATE Clients SET
                    Surname = @Surname,
                    FirstName = @FirstName,
                    Patronymic = @Patronymic,
                    TelephoneNumber = @TelephoneNumber,
                    Email = @Email
                  WHERE Id = @Id";
            sqlStringCommands.Delete =
                @"DELETE FROM Clients
                  WHERE Id = @Id";
            return sqlStringCommands;
        }

        private StringCommands GetOleDbStringCommands()
        {
            var sqlStringCommands = new StringCommands();
            sqlStringCommands.Select =
                @"SELECT *
                  FROM Orders
                  ORDER BY Id;";
            sqlStringCommands.Insert =
                @"INSERT INTO Orders (Email, ProductId, ProductName)
                  VALUES (@Email, @ProductId, @ProductName);
                  SET @Id = @@iDENTITY;";
            sqlStringCommands.Update =
                @"UPDATE Orders SET
                    Email = @Email,
                    ProductId = @ProductId,
                    ProductName = @ProductName,
                  WHERE Id = @Id;";
            sqlStringCommands.Delete =
                @"DELETE FROM Orders
                  WHERE Id = @Id;";
            return sqlStringCommands;
        }

        private SqlParameter[] GetSqlParameters()
        {
            return
            [
                new SqlParameter("@Id", SqlDbType.Int, 4, "Id"),
                new SqlParameter("@Surname", SqlDbType.NVarChar, 25, "Surname"),
                new SqlParameter("@FirstName", SqlDbType.NVarChar, 25, "FirstName"),
                new SqlParameter("@Patronymic", SqlDbType.NVarChar, 25, "Patronymic"),
                new SqlParameter("@TelephoneNumber", SqlDbType.NVarChar, 25, "TelephoneNumber"),
                new SqlParameter("@Email", SqlDbType.NVarChar, 50, "Email")
            ];
        }

        private OleDbParameter[] GetOleDbParameters()
        {
            return
            [
                new OleDbParameter("@Id", OleDbType.Integer, 4, "Id"),
                new OleDbParameter("@Email", OleDbType.WChar, 50, "Email"),
                new OleDbParameter("@ProductId", OleDbType.SmallInt, 4, "ProductId"),
                new OleDbParameter("ProductName", OleDbType.WChar, 50, "ProductName")
            ];
        }

        private TOut FillOutDataAdapter<TIn1, TIn2, TOut>(StringCommands stringCommands, TIn1[] dbParameters, TIn2 connection, TOut dataAdapter)
            where TIn1 : DbParameter
            where TIn2 : DbConnection
            where TOut : DbDataAdapter, IDbDataAdapter
        {
            var commandsDb = new CommandsDb(stringCommands, connection);
            
            dataAdapter.SelectCommand = commandsDb.SelectCommand;
            dataAdapter.InsertCommand = commandsDb.InsertCommand;
            dataAdapter.UpdateCommand = commandsDb.UpdateCommand;
            dataAdapter.DeleteCommand = commandsDb.DeleteCommand;

            dataAdapter.InsertCommand.Parameters.AddMany(dbParameters);
            dataAdapter.UpdateCommand.Parameters.AddMany(dbParameters);
            dataAdapter.DeleteCommand.Parameters.Add(dbParameters[0]);

            return dataAdapter;
        }
    }
}
