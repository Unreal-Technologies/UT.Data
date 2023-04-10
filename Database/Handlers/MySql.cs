using MySql.Data.MySqlClient;
using System.Diagnostics;

namespace UT.Data.Database.Handlers
{
    internal class MySql : DbManager
    {
        #region Enums
        private enum CommandStates
        {
            Data, NoResult
        }
        #endregion //Enums

        #region Public Methods
        public override Tuple<int, Dictionary<string, object?>[]?>? Execute(string command)
        {
            return this.Execute(command, Filetypes.Main);
        }

        public override bool Close()
        {
            return this.Close(Filetypes.Main);
        }

        public override bool Open()
        {
            return this.Open(Filetypes.Main);
        }
        #endregion //Public Methods

        #region Internal Methods
        internal override Tuple<int, Dictionary<string, object?>[]?>? Execute(string command, Filetypes filetype)
        {
            MySqlConnection? con = (MySqlConnection?)this.Connections.Where(x => x.Item1 == filetype).FirstOrDefault()?.Item3;
            if (con == null)
            {
                return null;
            }

            MySqlCommand cmd = con.CreateCommand();
            cmd.CommandText = command;
            CommandStates state = CheckCommandState(cmd);
            switch(state)
            {
                case CommandStates.Data:
                    MySqlDataReader reader = cmd.ExecuteReader();
                    int fields = reader.FieldCount;
                    List<Dictionary<string, object?>> rows = new();
                    while (reader.Read())
                    {
                        Dictionary<string, object?> row = new();
                        for(int i=0;i<fields;i++)
                        {
                            object val = reader.GetValue(i);
                            bool isNull = val == null || val.GetType() == typeof(DBNull);
                            string name = reader.GetName(i);

                            row.Add(name, isNull ? null : val);
                        }
                        rows.Add(row);
                    }
                    reader.Close();
                    return new Tuple<int, Dictionary<string, object?>[]?>(rows.Count, rows.ToArray());
                case CommandStates.NoResult:
                    return new Tuple<int, Dictionary<string, object?>[]?>(cmd.ExecuteNonQuery(), null);
            }
            return null;
        }

        internal override bool Close(Filetypes filetype)
        {
            MySqlConnection? con = (MySqlConnection?)this.Connections.Where(x => x.Item1 == filetype).FirstOrDefault()?.Item3;
            if (con == null)
            {
                return false;
            }
            con.Close();
            return true;
        }

        internal override void Connection(string connection, FileInfo? file, Filetypes filetype)
        {
            this.Connections.Add(new Tuple<Filetypes, FileInfo?, object>(filetype, file, new MySqlConnection(connection)));
        }

        internal override bool Open(Filetypes filetype)
        {
            MySqlConnection? con = (MySqlConnection?)this.Connections.Where(x => x.Item1 == filetype).FirstOrDefault()?.Item3;
            if(con == null)
            {
                return false;
            }
            try
            {
                con.Open();
            }
            catch(MySqlException ex)
            {
                Exception? inner = ex.InnerException;
                if(inner != null && inner.Message.StartsWith("Unknown database"))
                {
                    if(CreateEmptyDatabase(con))
                    {
                        return this.Open();
                    }
                }
                throw ex;
            }
            return true;
        }
        #endregion //Internal Methods

        #region Private Methods
        private static CommandStates CheckCommandState(MySqlCommand cmd)
        {
            string command = cmd.CommandText.ToLower();
            if (command.StartsWith("select") || command.StartsWith("describe"))
            {
                return CommandStates.Data;
            }

            return CommandStates.NoResult;
        }

        private static bool CreateEmptyDatabase(MySqlConnection con)
        {
            string cs = con.ConnectionString.Replace("database=" + con.Database + ";", "");
            MySqlConnection master = new(cs);
            master.Open();

            MySqlCommand cmd = master.CreateCommand();
            cmd.CommandText = "create database `" + con.Database + "`;";
            cmd.ExecuteNonQuery();

            master.Close();
            return true;
        }
        #endregion //Private Methods
    }
}
