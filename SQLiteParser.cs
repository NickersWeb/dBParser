using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dBParser
{
    public class SQLiteParser : IDBParser
    {
        List<SQLiteParameter> ParameterList = new List<SQLiteParameter>();
        List<string> UnParameterList = new List<string>();
        List<string> ParameterisedList = new List<string>();

        private string _connectionstring { get; set; }
        public SQLiteParser()
        {
            ConfigManager.ConfigHelper config = new ConfigManager.ConfigHelper();
            _connectionstring = config.ConnectionString();
        }

        public void AddParameter(string name, object value)
        {
            ParameterList.Add(new SQLiteParameter(name, value ?? string.Empty));
            UnParameterList.Append($"{name}, ");
            ParameterisedList.Append($"@{name}, ");
        }
        public void EndParameter(string name, object value)
        {
            ParameterList.Add(new SQLiteParameter(name, value ?? string.Empty));
            UnParameterList.Append($"{name}");
            ParameterisedList.Append($"@{name}");
        }
        public Guid AddNewReturnGuid(string TableName, string OutputQuery)
        {
            string query = $"INSERT INTO {TableName} ({UnParameterList.ToString()}) OUTPUT {OutputQuery} VALUES ({ParameterisedList.ToString()})";
            Guid output = Guid.Empty;
            using (var connection = new SQLiteConnection(_connectionstring))
            {
                connection.Open();
                using (SQLiteCommand sqlData = new SQLiteCommand(query, connection))
                {
                    sqlData.Parameters.AddRange(ParameterList.ToArray());
                    output = (Guid)sqlData.ExecuteScalar();

                    UnParameterList = new List<string>();
                    ParameterisedList = new List<string>();
                    ParameterList = new List<SQLiteParameter>();
                }
                connection.Close();
            }
            return output;
        }
        public long AddNewReturnLong(string TableName, string OutputQuery = "")
        {
            StringBuilder str = new StringBuilder();
            str.Append($"INSERT INTO {TableName} ({string.Join("", UnParameterList.ToArray())}) ");
            if (!string.IsNullOrEmpty(OutputQuery))
            {
                str.Append($"OUTPUT {OutputQuery} ");
            }
            str.Append($"VALUES ({string.Join("", ParameterisedList.ToArray())})");
            long output = 0;
            using (var connection = new SqlConnection(_connectionstring))
            {
                connection.Open();
                using (SqlDataAdapter sqlData = new SqlDataAdapter(str.ToString(), connection))
                {
                    sqlData.SelectCommand.Parameters.AddRange(ParameterList.ToArray());
                    output = (long)sqlData.SelectCommand.ExecuteScalar();
                    //sqlData.SelectCommand.CommandType = CommandType.StoredProcedure;
                    UnParameterList = new List<string>();
                    ParameterisedList = new List<string>();
                    ParameterList = new List<SQLiteParameter>();
                }
                connection.Close();
            }
            return output;
        }
        public int AddNewReturnInt(string TableName, string OutputQuery)
        {
            string query = $"INSERT INTO {TableName} ({UnParameterList.ToString()}) OUTPUT {OutputQuery} VALUES ({ParameterisedList.ToString()})";
            int output = 0;
            using (var connection = new SQLiteConnection(_connectionstring))
            {
                connection.Open();
                using (SQLiteCommand sqlData = new SQLiteCommand(connection))
                {
                    sqlData.Parameters.AddRange(ParameterList.ToArray());
                    output = (int)sqlData.ExecuteScalar();

                    UnParameterList = new List<string>();
                    ParameterisedList = new List<string>();
                    ParameterList = new List<SQLiteParameter>();
                }
                connection.Close();
            }
            return output;
        }
        public void Create(string TableName)
        {
            string query = $"INSERT INTO {TableName} ({UnParameterList.ToString()}) VALUES ({ParameterisedList.ToString()})";

            using (var connection = new SQLiteConnection(_connectionstring))
            {
                connection.Open();
                using (SQLiteCommand sqlCom = new SQLiteCommand(query, connection))
                {
                    sqlCom.Parameters.AddRange(ParameterList.ToArray());
                    sqlCom.ExecuteNonQuery();
                    UnParameterList = new List<string>();
                    ParameterisedList = new List<string>();
                    ParameterList = new List<SQLiteParameter>();
                }
                connection.Close();
            }
        }
        public void Update(string TableName, string WhereClause)
        {
            string query = $"UPDATE {TableName} SET ({ParameterisedList.ToString()}) WHERE {WhereClause}";
            using (var connection = new SQLiteConnection(_connectionstring))
            {
                connection.Open();
                using (SQLiteCommand sqlCom = new SQLiteCommand(query, connection))
                {
                    sqlCom.Parameters.AddRange(ParameterList.ToArray());
                    sqlCom.ExecuteNonQuery();
                    UnParameterList = new List<string>();
                    ParameterisedList = new List<string>();
                    ParameterList = new List<SQLiteParameter>();
                }
                connection.Close();
            }
        }
        public void Delete(string TableName, string WhereClause)
        {
            string query = $"DELETE FROM {TableName} WHERE {WhereClause}";
            using (var connection = new SQLiteConnection(_connectionstring))
            {
                connection.Open();
                using (SQLiteCommand sqlCom = new SQLiteCommand(query, connection))
                {
                    sqlCom.Parameters.AddRange(ParameterList.ToArray());
                    sqlCom.ExecuteNonQuery();
                    UnParameterList = new List<string>();
                    ParameterisedList = new List<string>();
                    ParameterList = new List<SQLiteParameter>();
                }
                connection.Close();
            }
        }
        public DataTable Read(string Query)
        {
            DataTable dt = new DataTable();

            using (var connection = new SqlConnection(_connectionstring))
            {
                connection.Open();
                using (SQLiteDataAdapter sqliteData = new SQLiteDataAdapter(Query, _connectionstring))
                {
                    sqliteData.SelectCommand.Parameters.AddRange(ParameterList.ToArray());
                    sqliteData.Fill(dt);
                    UnParameterList = new List<string>();
                    ParameterisedList = new List<string>();
                    ParameterList = new List<SQLiteParameter>();
                }
                connection.Close();
            }
            return dt;
        }
        public void CreateExtendPropertyFromTable(string table, string name, string value)
        {

        }
        public DataTable ReadExtendPropertyFromTable(string table)
        {
            StringBuilder str = new StringBuilder();
            str.Append("SELECT EP.name as name, EP.value as value from sys.extended_properties as EP ");
            str.Append("LEFT JOIN sys.tables TBL ON TBL.object_id = EP.major_id ");
            str.Append("LEFT JOIN sys.schemas SCH ON SCH.schema_id = TBL.schema_id ");
            str.Append("LEFT JOIN sys.indexes IND ON IND.object_id = TBL.object_id ");
            str.Append("AND IND.index_id = EP.minor_id ");
            str.Append("where TBL.name = @tablename");
            EndParameter("tablename", table);
            return Read(str.ToString());
        }
        public void UpdateExtendProperty(string table, string name, string value)
        {

        }
        public void DeleteExtendProperty(string table, string name)
        {

        }
        public void CreateExtendProperty(string name, string value, bool Database = false, string TableName = "", string ColumnName = "") { }
        public string ReadExtendProperty(string Name, bool Database = false, string TableName = "", string ColumnName = "") { return string.Empty; }
        public void UpdateExtendProperty(string name, string value, bool Database = false, string TableName = "", string ColumnName = "") { }
        public void DeleteExtendProperty(string name, bool Database = false, string TableName = "", string ColumnName = "") { }
    }

}

