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
        /// <summary>
        /// Add SQL parameter for query. Required to finish adding parameters with EndParameter.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        void AddParameter(string name, object value);
        /// <summary>
        ///  Required to finish adding parameters with this method.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        void EndParameter(string name, object value);
        /// <summary>
        /// Insert new with parameters and return Guid output.
        /// </summary>
        /// <param name="TableName"></param>
        /// <param name="OutputQuery">Required Output from query</param>
        /// <returns></returns>
        Guid AddNewReturnGuid(string TableName, string OutputQuery);
        /// <summary>
        /// Insert new with parameters and return Int output.
        /// </summary>
        /// <param name="TableName"></param>
        /// <param name="OutputQuery"></param>
        /// <returns></returns>
        int AddNewReturnInt(string TableName, string OutputQuery = "");
        /// <summary>
        /// Insert new with parameters and return Long output.
        /// </summary>
        /// <param name="TableName"></param>
        /// <param name="OutputQuery"></param>
        /// <returns></returns>
        long AddNewReturnLong(string TableName, string OutputQuery = "");
        /// <summary>
        /// Create insert with parameters.
        /// </summary>
        /// <param name="TableName"></param>
        void Create(string TableName);
        /// <summary>
        ///  Update with parameters with where query.
        /// </summary>
        /// <param name="TableName"></param>
        /// <param name="WhereQuery"></param>
        void Update(string TableName, string WhereQuery);
        /// <summary>
        /// Delete with parameters with where query.
        /// </summary>
        /// <param name="TableName"></param>
        /// <param name="WhereQuery"></param>
        void Delete(string TableName, string WhereQuery);
        /// <summary>
        ///  Read data from database can include sql parameters, run raw query onto database
        /// </summary>
        /// <param name="Query"></param>
        /// <returns>DataTable of the query</returns>
        DataTable Read(string Query);
        /// <summary>
        /// Create Extend Property, depend of where via the parameters
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="Database"></param>
        /// <param name="TableName"></param>
        /// <param name="ColumnName"></param>
        void CreateExtendProperty(string name, string value, bool Database = false, string TableName = "", string ColumnName = "");
        /// <summary>
        /// Read Extend Property, depend of where via the parameters
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Database"></param>
        /// <param name="TableName"></param>
        /// <param name="ColumnName"></param>
        /// <returns></returns>
        string ReadExtendProperty(string Name, bool Database = false, string TableName = "", string ColumnName = "");
        /// <summary>
        /// Update Extend Property, depend of where via the parameters
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="Database"></param>
        /// <param name="TableName"></param>
        /// <param name="ColumnName"></param>
        void UpdateExtendProperty(string name, string value, bool Database = false, string TableName = "", string ColumnName = "");
        /// <summary>
        /// Delete Extend Property, depend of where via the parameters
        /// </summary>
        /// <param name="name"></param>
        /// <param name="Database"></param>
        /// <param name="TableName"></param>
        /// <param name="ColumnName"></param>
        void DeleteExtendProperty(string name, bool Database = false, string TableName = "", string ColumnName = "");
    }
}
