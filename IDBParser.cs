using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dBParser
{
    public interface IDBParser
    {
        void AddParameter(string name, object value);
        void EndParameter(string name, object value);
        Guid AddNewReturnGuid(string TableName, string OutputQuery);
        int AddNewReturnInt(string TableName, string OutputQuery = "");
        long AddNewReturnLong(string TableName, string OutputQuery = "");
        void Create(string TableName);
        void Update(string TableName, string WhereQuery);
        void Delete(string TableName, string WhereQuery);
        DataTable Read(string Query);
        void CreateExtendProperty(string name, string value, bool Database = false, string TableName = "", string ColumnName = "");
        string ReadExtendProperty(string Name, bool Database = false, string TableName = "", string ColumnName = "");
        void UpdateExtendProperty(string name, string value, bool Database = false, string TableName = "", string ColumnName = "");
        void DeleteExtendProperty(string name, bool Database = false, string TableName = "", string ColumnName = "");
    }
}
