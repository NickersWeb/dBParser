using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dBParser
{
    public class MSSQLParser : IDBParser
    {
        List<SqlParameter> ParameterList = new List<SqlParameter>();
        List<string> UnParameterList = new List<string>();
        List<string> ParameterisedList = new List<string>();
        private string _connectionstring { get; set; }
        public MSSQLParser() {
            ConfigManager.ConfigHelper configHelper = new ConfigManager.ConfigHelper();
            _connectionstring = configHelper.ConnectionString();
        }

        public void AddParameter(string name, object value)
        {
            ParameterList.Add(new SqlParameter(name, value ?? string.Empty));
            UnParameterList.Add($"{name}, ");
            ParameterisedList.Add($"@{name}, ");
        }
        public void EndParameter(string name, object value)
        {
            ParameterList.Add(new SqlParameter(name, value ?? string.Empty));
            UnParameterList.Add($"{name}");
            ParameterisedList.Add($"@{name}");
        }
        public Guid AddNewReturnGuid(string TableName, string OutputQuery)
        {
            string query = $"INSERT INTO {TableName} ({string.Join("", UnParameterList.ToArray())}) OUTPUT {OutputQuery} VALUES ({string.Join("", ParameterisedList.ToArray())})";
            Guid output = Guid.Empty;
            using (var connection = new SqlConnection(_connectionstring))
            {
                connection.Open();
                using (SqlDataAdapter sqlData = new SqlDataAdapter(query, connection))
                {
                    sqlData.SelectCommand.Parameters.AddRange(ParameterList.ToArray());
                    output = (Guid)sqlData.SelectCommand.ExecuteScalar();
                    //sqlData.SelectCommand.CommandType = CommandType.StoredProcedure;
                    UnParameterList = new List<string>();
                    ParameterisedList = new List<string>();
                    ParameterList = new List<SqlParameter>();
                }
                connection.Close();
            }
            return output;
        }
        public long AddNewReturnLong(string TableName, string OutputQuery = "")
        {
             StringBuilder str = new StringBuilder();
            str.Append($"INSERT INTO {TableName} ({string.Join("", UnParameterList.ToArray())}) ");
            bool isOutPutQuery = string.IsNullOrEmpty(OutputQuery);
            if(!isOutPutQuery){
                str.Append($"OUTPUT {OutputQuery} ");
            }
            str.Append($"VALUES ({string.Join("", ParameterisedList.ToArray())}); ");
            if(isOutPutQuery){
                str.Append($"SELECT CAST(scope_identity() AS bigint);");
            }
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
                    ParameterList = new List<SqlParameter>();
                }
                connection.Close();
            }
            return output;
        }
        public int AddNewReturnInt(string TableName, string OutputQuery)
        {
            StringBuilder str = new StringBuilder();
            str.Append($"INSERT INTO {TableName} ({string.Join("", UnParameterList.ToArray())}) ");
            bool isOutPutQuery = string.IsNullOrEmpty(OutputQuery);
            if(!isOutPutQuery){
                str.Append($"OUTPUT {OutputQuery} ");
            }
            str.Append($"VALUES ({string.Join("", ParameterisedList.ToArray())}); ");
            if(isOutPutQuery){
                str.Append($"SELECT CAST(scope_identity() AS int);");
            }
            int output = 0;
            using (var connection = new SqlConnection(_connectionstring))
            {
                connection.Open();
                using (SqlDataAdapter sqlData = new SqlDataAdapter(str.ToString(), connection))
                {
                    sqlData.SelectCommand.Parameters.AddRange(ParameterList.ToArray());
                    output = (int)sqlData.SelectCommand.ExecuteScalar();
                    //sqlData.SelectCommand.CommandType = CommandType.StoredProcedure;
                    UnParameterList = new List<string>();
                    ParameterisedList = new List<string>();
                    ParameterList = new List<SqlParameter>();
                }
                connection.Close();
            }
            return output;
        }
        public void Create(string TableName)
        {
            string query = $"INSERT INTO {TableName} ({string.Join("", UnParameterList.ToArray())}) VALUES ({string.Join("", ParameterisedList.ToArray())})";

            using (var connection = new SqlConnection(_connectionstring))
            {
                connection.Open();
                using (SqlCommand sqlCom = new SqlCommand(query, connection))
                {
                    sqlCom.Parameters.AddRange(ParameterList.ToArray());
                    sqlCom.ExecuteNonQuery();
                    UnParameterList = new List<string>();
                    ParameterisedList = new List<string>();
                    ParameterList = new List<SqlParameter>();
                }
                connection.Close();
            }
        }
        public void Update(string TableName, string WhereQuery)
        {
            StringBuilder updatableColumns = new StringBuilder();
            if (ParameterList.Count > 1)
            {
                for (int i = 0; i < ParameterList.Count - 1; i++)
                {
                    updatableColumns.Append($"{ParameterList[i].ParameterName} = {ParameterisedList[i]}");
                }
            }
            updatableColumns.Append($"{ParameterList[ParameterList.Count - 1].ParameterName} = {ParameterisedList[ParameterisedList.Count - 1]}");
            string query = $"UPDATE {TableName} SET {updatableColumns.ToString()} WHERE {WhereQuery}";
            using (var connection = new SqlConnection(_connectionstring))
            {
                connection.Open();
                using (SqlCommand sqlCom = new SqlCommand(query, connection))
                {
                    sqlCom.Parameters.AddRange(ParameterList.ToArray());
                    sqlCom.ExecuteNonQuery();
                    UnParameterList = new List<string>();
                    ParameterisedList = new List<string>();
                    ParameterList = new List<SqlParameter>();
                }
                connection.Close();
            }
        }
        public void Delete(string TableName, string WhereQuery)
        {
            string query = $"DELETE FROM {TableName} WHERE {WhereQuery}";
            using (var connection = new SqlConnection(_connectionstring))
            {
                connection.Open();
                using (SqlCommand sqlCom = new SqlCommand(query, connection))
                {
                    sqlCom.Parameters.AddRange(ParameterList.ToArray());
                    sqlCom.ExecuteNonQuery();
                    UnParameterList = new List<string>();
                    ParameterisedList = new List<string>();
                    ParameterList = new List<SqlParameter>();
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
                using (SqlDataAdapter sqlData = new SqlDataAdapter(Query, connection))
                {
                    sqlData.SelectCommand.Parameters.AddRange(ParameterList.ToArray());
                    //sqlData.MissingSchemaAction = MissingSchemaAction.AddWithKey;
                    sqlData.Fill(dt);
                    
                    UnParameterList = new List<string>();
                    ParameterisedList = new List<string>();
                    ParameterList = new List<SqlParameter>();
                }
                connection.Close();
            }
            return dt;
        }
        public void CreateExtendProperty(string name, string value, bool Database = false, string TableName = "", string ColumnName = "")
        {
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
