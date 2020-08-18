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
        public Object InsertUpdate(string TableName, string WhereQuery, string OutputQuery = "")
        {
            Object output = new { };
            //StringBuilder str = new StringBuilder();
            //StringBuilder u = new StringBuilder();
            //bool IsOutPutQuery = !string.IsNullOrEmpty(OutputQuery);
            //if (ParameterList.Count > 1)
            //{
            //    for (int i = 0; i < ParameterList.Count - 1; i++)
            //    {
            //        u.Append($"{ParameterList[i].ParameterName} = {ParameterisedList[i]}");
            //    }
            //}
            //u.Append($"{ParameterList[ParameterList.Count - 1].ParameterName} = {ParameterisedList[ParameterisedList.Count - 1]}");

            //str.Append($"IF NOT EXISTS(SELECT @@IDENTITY FROM {TableName} WHERE {WhereQuery}) ");
            //str.Append($"INSERT INTO [{TableName}] ");
            //if (IsOutPutQuery)
            //{
            //    str.Append($"OUTPUT {OutputQuery} ");
            //}
            //str.Append($"VALUES ({string.Join("", ParameterisedList.ToArray())}) ");
            //str.Append("ELSE ");
            //str.Append($"UPDATE [{TableName}] SET ");
            //str.Append($"{u} ");
            //if (IsOutPutQuery)
            //{
            //    str.Append($"OUTPUT {OutputQuery} ");
            //}
            //str.Append($"FROM [{TableName}]");
            //str.Append($" WHERE {WhereQuery} ");

            //using (var connection = new SqlConnection(_connectionstring))
            //{
            //    connection.Open();
            //    using (SqlDataAdapter sqlData = new SqlDataAdapter(str.ToString(), connection))
            //    {
            //        sqlData.SelectCommand.Parameters.AddRange(ParameterList.ToArray());
            //        output = sqlData.SelectCommand.ExecuteScalar();
            //        UnParameterList = new List<string>();
            //        ParameterisedList = new List<string>();
            //        ParameterList = new List<SqlParameter>();
            //    }
            //    connection.Close();
            //}
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

        public void CreateExtendProperty(string name, string value, bool Database = false, string TableName = "", string ColumnName = "") {
            StringBuilder str = new StringBuilder();

            str.Append("EXEC sp_addextendedproperty ");
            str.Append($"@name = N'{name}', ");
            str.Append($"@value = N'{value}', ");
            if (Database && string.IsNullOrEmpty(TableName) && string.IsNullOrEmpty(ColumnName))
            {
                str.Append($"@level0type = N'Schema', @level0name = 'dbo'; ");
            }
            else
            {
                if (!string.IsNullOrEmpty(TableName) && string.IsNullOrEmpty(ColumnName))
                {
                    str.Append($"@level0type = N'Schema', @level0name = 'dbo', ");
                    str.Append($"@level1type = N'Table',  @level1name = @tablename; ");
                    EndParameter("tablename", TableName);
                }
                else if (!string.IsNullOrEmpty(TableName) && !string.IsNullOrEmpty(ColumnName))
                {
                    str.Append($"@level0type = N'Schema', @level0name = 'dbo', ");
                    str.Append($"@level1type = N'Table',  @level1name = @tablename, ");
                    str.Append($"@level2type = N'Column', @level2name = @columnname; ");
                    AddParameter("tablename", TableName);
                    EndParameter("columnname", ColumnName);
                }

            }
            Read(str.ToString());

         }
        public string ReadExtendProperty(string Name, bool Database = false, string TableName = "", string ColumnName = "")
        {
            StringBuilder str = new StringBuilder();
            string value = "";
            if (Database && string.IsNullOrEmpty(TableName) && string.IsNullOrEmpty(ColumnName))
            {
                EndParameter("name", Name);
                str.Append("SELECT value as [ExtendedPropertyValue] FROM sys.extended_properties where name = @name");
            }
            else
            {
                if (!string.IsNullOrEmpty(TableName) && string.IsNullOrEmpty(ColumnName))
                {
                    str.Append("SELECT ");
                    str.Append("SCHEMA_NAME(tbl.schema_id) AS SchemaName, ");
                    str.Append("tbl.name AS TableName,  ");
                    str.Append("p.name AS ExtendedPropertyName, ");
                    str.Append("CAST(p.value AS sql_variant) AS ExtendedPropertyValue ");
                    str.Append("FROM ");
                    str.Append("sys.tables AS tbl ");
                    str.Append("INNER JOIN sys.extended_properties AS p ON p.major_id = tbl.object_id AND p.minor_id = 0 AND p.class=1 ");
                    str.Append("WHERE tbl.name = @tablename AND p.name = @name");
                    AddParameter("tablename", TableName);
                    EndParameter("name", Name);
                }
                else if (!string.IsNullOrEmpty(TableName) && !string.IsNullOrEmpty(ColumnName))
                {
                    str.Append("SELECT ");
                    str.Append("SCHEMA_NAME(tbl.schema_id) AS SchemaName, ");
                    str.Append("tbl.name AS TableName,  ");
                    str.Append("clmns.name AS ColumnName, ");
                    str.Append("p.name AS ExtendedPropertyName, ");
                    str.Append("CAST(p.value AS sql_variant) AS ExtendedPropertyValue ");
                    str.Append("FROM ");
                    str.Append("sys.tables AS tbl ");
                    str.Append("INNER JOIN sys.all_columns AS clmns ON clmns.object_id = tbl.object_id ");
                    str.Append("INNER JOIN sys.extended_properties AS p ON p.major_id = tbl.object_id AND p.minor_id = clmns.column_id AND p.class=1 ");
                    str.Append("WHERE tbl.name = @TableName AND clmns.name = @columnname AND  p.name = @name ");
                    AddParameter("tablename", TableName);
                    AddParameter("columnname", ColumnName);
                    EndParameter("name", Name);
                }

            }
            DataTable dt = Read(str.ToString());
            if (dt != null && dt.Rows.Count > 0)
            {
                value = dt.AsEnumerable().First()["ExtendedPropertyValue"].ToString();
            }
            return value;

        }
        public void UpdateExtendProperty(string name, string value, bool Database = false, string TableName = "", string ColumnName = "")
        {
            StringBuilder str = new StringBuilder();

            str.Append("EXEC sp_updateextendedproperty ");
            str.Append($"@name = N'{name}', ");
            str.Append($"@value = N'{value}', ");
            if (Database && string.IsNullOrEmpty(TableName) && string.IsNullOrEmpty(ColumnName))
            {
                str.Append($"@level0type = N'Schema', @level0name = 'dbo'; ");
            }
            else
            {
                if (!string.IsNullOrEmpty(TableName) && string.IsNullOrEmpty(ColumnName))
                {
                    str.Append($"@level0type = N'Schema', @level0name = 'dbo', ");
                    str.Append($"@level1type = N'Table',  @level1name = @tablename; ");
                    EndParameter("tablename", TableName);
                }
                else if (!string.IsNullOrEmpty(TableName) && !string.IsNullOrEmpty(ColumnName))
                {
                    str.Append($"@level0type = N'Schema', @level0name = 'dbo', ");
                    str.Append($"@level1type = N'Table',  @level1name = @tablename, ");
                    str.Append($"@level2type = N'Column', @level2name = @columnname; ");
                    AddParameter("tablename", TableName);
                    EndParameter("columnname", ColumnName);
                }

            }
            Read(str.ToString());
        }
        public void DeleteExtendProperty(string name, bool Database = false, string TableName = "", string ColumnName = "")
        {
            StringBuilder str = new StringBuilder();

            str.Append("EXEC sp_dropextendedproperty  ");
            str.Append($"@name = N'{name}', ");
            if (Database && string.IsNullOrEmpty(TableName) && string.IsNullOrEmpty(ColumnName))
            {
                str.Append($"@level0type = N'Schema', @level0name = 'dbo'; ");
            }
            else
            {
                if (!string.IsNullOrEmpty(TableName) && string.IsNullOrEmpty(ColumnName))
                {
                    str.Append($"@level0type = N'Schema', @level0name = 'dbo', ");
                    str.Append($"@level1type = N'Table',  @level1name = @tablename; ");
                    EndParameter("tablename", TableName);
                }
                else if (!string.IsNullOrEmpty(TableName) && !string.IsNullOrEmpty(ColumnName))
                {
                    str.Append($"@level0type = N'Schema', @level0name = 'dbo', ");
                    str.Append($"@level1type = N'Table',  @level1name = @tablename, ");
                    str.Append($"@level2type = N'Column', @level2name = @columnname; ");
                    AddParameter("tablename", TableName);
                    EndParameter("columnname", ColumnName);
                }

            }
            Read(str.ToString());

        }
    }

}

