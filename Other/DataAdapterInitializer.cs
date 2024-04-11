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
using System.ComponentModel.DataAnnotations;

namespace Task16.Other
{
    public class DataAdapterInitializer
    {        
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
            var oleDbStringCommands = new StringCommands();
            oleDbStringCommands.Select =
                @"SELECT *
                  FROM Orders
                  ORDER BY Id;";
            oleDbStringCommands.Insert =
                @"INSERT INTO Orders (Email, ProductId, ProductName)
                  VALUES (@Email, @ProductId, @ProductName)";
            oleDbStringCommands.Update =
                @"UPDATE Orders SET
                    Email = @Email,
                    ProductId = @ProductId,
                    ProductName = @ProductName
                  WHERE Id = @Id;";
            oleDbStringCommands.Delete =
                @"DELETE FROM Orders
                  WHERE Id = @Id;";
            return oleDbStringCommands;
        }

        private Dictionary<string, SqlParameter> GetSqlParameters()
        {
            var idParameter = new SqlParameter("@Id", SqlDbType.Int, 4, "Id");
            idParameter.Direction = ParameterDirection.Output;
            return new Dictionary<string, SqlParameter>
            {
                ["IdOut"] = idParameter,
                ["Id"] = new SqlParameter("@Id", SqlDbType.Int, 4, "Id"),
                ["Surname"] = new SqlParameter("@Surname", SqlDbType.NVarChar, 25, "Surname"),
                ["FirstName"] = new SqlParameter("@FirstName", SqlDbType.NVarChar, 25, "FirstName"),
                ["Patronymic"] = new SqlParameter("@Patronymic", SqlDbType.NVarChar, 25, "Patronymic"),
                ["TelephoneNumber"] = new SqlParameter("@TelephoneNumber", SqlDbType.NVarChar, 25, "TelephoneNumber"),
                ["Email"] = new SqlParameter("@Email", SqlDbType.NVarChar, 50, "Email")
            };
        }

        private Dictionary<string,OleDbParameter> GetOleDbParameters()
        {
            return new Dictionary<string, OleDbParameter>
            {
                ["Email"] = new OleDbParameter("@Email", OleDbType.WChar, 50, "Email"),
                ["ProductId"] = new OleDbParameter("@ProductId", OleDbType.Integer, 0, "ProductId"),
                ["ProductName"] = new OleDbParameter("@ProductName", OleDbType.WChar, 50, "ProductName"),
                ["Id"] = new OleDbParameter("@Id", OleDbType.Integer, 0, "Id")
            };
        }

        private TOut FillOutDataAdapter<TIn1, TIn2, TOut>(StringCommands stringCommands, Dictionary<string,TIn1> dbParameters, TIn2 connection, TOut dataAdapter)
            where TIn1 : DbParameter
            where TIn2 : DbConnection
            where TOut : DbDataAdapter, IDbDataAdapter
        {
            var commandsDb = new CommandsDb(stringCommands, connection);
            
            dataAdapter.SelectCommand = commandsDb.SelectCommand;
            dataAdapter.InsertCommand = commandsDb.InsertCommand;
            dataAdapter.UpdateCommand = commandsDb.UpdateCommand;
            dataAdapter.DeleteCommand = commandsDb.DeleteCommand;

            if (dataAdapter.GetType() == typeof(SqlDataAdapter))
            {
                dataAdapter.InsertCommand.Parameters.AddMany(dbParameters["Surname"], dbParameters["FirstName"],
                    dbParameters["Patronymic"], dbParameters["TelephoneNumber"], dbParameters["Email"], dbParameters["IdOut"]);
                dataAdapter.UpdateCommand.Parameters.AddMany(dbParameters["Surname"], dbParameters["FirstName"],
                    dbParameters["Patronymic"], dbParameters["TelephoneNumber"], dbParameters["Email"], dbParameters["Id"]);
                dataAdapter.DeleteCommand.Parameters.AddMany(dbParameters["Id"]);
            }
            else
            {
                dataAdapter.InsertCommand.Parameters.AddMany(dbParameters["Email"], dbParameters["ProductId"],
                    dbParameters["ProductName"]);
                dataAdapter.UpdateCommand.Parameters.AddMany(dbParameters["Email"], dbParameters["ProductId"],
                    dbParameters["ProductName"], dbParameters["Id"]);
                dataAdapter.DeleteCommand.Parameters.Add(dbParameters["Id"]);
            }
            //dataAdapter.InsertCommand.Parameters.AddMany(dbParameters.Take(3).ToArray());
            //dataAdapter.UpdateCommand.Parameters.AddMany(dbParameters);
            //dataAdapter.DeleteCommand.Parameters.Add(dbParameters[3]);

            return dataAdapter;
        }
    }
}
