using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task16.Other
{
    public class CommandsDb
    {
        private DbConnection Connection {  get; }
        private StringCommands StringCommands { get; }
        
        public DbCommand SelectCommand { get; }
        public DbCommand InsertCommand { get; }
        public DbCommand UpdateCommand { get; }
        public DbCommand DeleteCommand { get; }

        public CommandsDb(StringCommands stringCommands, DbConnection connection)
        {
            Connection = connection;
            StringCommands = stringCommands;
            SelectCommand = stringCommands.Select.MakeDbCommand(connection);
            InsertCommand = stringCommands.Insert.MakeDbCommand(connection);
            UpdateCommand = stringCommands.Update.MakeDbCommand(connection);
            DeleteCommand = stringCommands.Delete.MakeDbCommand(connection);
        }
    }
}
