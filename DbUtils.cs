using Microsoft.SqlServer.Server;
using System;
using System.Data;
using System.IO;
using System.Runtime.Caching;
using System.Text.RegularExpressions;

namespace dBParser
{
    public static class DbUtils
    {
        /// <summary>
        /// Used to intialise the db type
        /// </summary>
        /// <returns></returns>
        public static IDBParser GetDBType()
        {
            ConfigManager.ConfigHelper config = new ConfigManager.ConfigHelper();
            switch (config.DataBaseType())
            {
                case "sql":
                    return new MSSQLParser();
                case "sqlite":
                    return new SQLiteParser();
                case "oracle":
                    return new OracleParser();
                case "mysql":
                    return new MySQLParser();
                default:
                    return new MSSQLParser();
            }

        }
        /// <summary>
        /// test
        /// </summary>
        /// <returns></returns>
        public static string ReturnDir()
        {

            return  $"{Path.GetFullPath(Path.Combine(System.AppContext.BaseDirectory, @"..\..\..\"))}appsettings.json";
        }
        /// <summary>
        /// Delete Cache via unique name
        /// </summary>
        /// <param name="Name"></param>
        public static void DeleteCache(string Name)
        {
            MemoryCache.Default.Remove(Name);
        }
        /// <summary>
        /// Update Extend PropCache
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Value"></param>
        /// <param name="TableName"></param>
        /// <param name="ColumnName"></param>
        /// <param name="Minutes">How long data should be cached for</param>
        public static void UpdateExtendPropCache(string Name, string Value, string TableName = "", string ColumnName = "", double Minutes = 15)
        {
            string AdvName = $"{Name}{TableName}{ColumnName}";
            DeleteCache(AdvName);
            MemoryCache.Default.Add(AdvName, Value, DateTime.Now.AddMinutes(Minutes));
        }
        /// <summary>
        ///  Read Extended Prop cache, found via method parameters
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Database"></param>
        /// <param name="TableName"></param>
        /// <param name="ColumnName"></param>
        /// <param name="Minutes">How long data should be cached for</param>
        /// <returns></returns>
        public static string ReadExtendedPropCache(string Name, bool Database = false, string TableName = "", string ColumnName = "", double Minutes = 15)
        {
            IDBParser dB = GetDBType();
            string AdvName = $"{Name}{TableName}{ColumnName}";
            string value = (string)MemoryCache.Default[AdvName];

            if (string.IsNullOrEmpty(value))
            {
                value = dB.ReadExtendProperty(Name, Database, TableName, ColumnName);
                MemoryCache.Default.Add(AdvName, value, DateTime.Now.AddMinutes(Minutes));
            }
            return value;
        }
        /// <summary>
        /// Delete Extended Prop Cache, via unique name
        /// </summary>
        /// <param name="PropName"></param>
        public static void DeleteExtendedPropCache(string PropName)
        {
            MemoryCache.Default.Remove(PropName);
        }
        /// <summary>
        /// Insert data row to cache via tablename
        /// </summary>
        /// <param name="TableName"></param>
        /// <param name="dr"></param>
        /// <param name="Minutes">How long data should be cached for</param>
        public static void InsertCache(string TableName, DataRow dr, double Minutes = 15)
        {
            DataTable dt = (DataTable)MemoryCache.Default[TableName];
            if (dt != null)
            {
                dt.Rows.Add(dr);
                MemoryCache.Default.Add(TableName, dt, DateTime.Now.AddMinutes(Minutes));
            }
        }
        /// <summary>
        /// Updata datarow in cached table via name and id
        /// </summary>
        /// <param name="TableName"></param>
        /// <param name="Id"></param>
        /// <param name="UpdateDr"></param>
        public static void UpdateCache(string TableName, string Id, DataRow UpdateDr, double Minutes = 15)
        {
            DataTable dt = (DataTable)MemoryCache.Default[TableName];
            if (dt != null)
            {
                DataRow dr = dt.Rows.Find(Id);
                dr = UpdateDr;
                dt.AcceptChanges();
                MemoryCache.Default.Add(TableName, dt, DateTime.Now.AddMinutes(Minutes));
            }
        }
        /// <summary>
        /// Read from cached datatable
        /// </summary>
        /// <param name="TableName"></param>
        /// <param name="Query"></param>
        /// <param name="Minutes">How long data should be cached for</param>
        /// <returns></returns>
        public static DataTable ReadCache(string TableName, string Query = "", double Minutes = 15)
        {
            IDBParser dB = GetDBType();
            DataTable dt = (DataTable)MemoryCache.Default[TableName];

            if (dt == null)
            {
                dt = dB.Read(Query);
                MemoryCache.Default.Add(TableName, dt, DateTime.Now.AddMinutes(Minutes));
            }

            return dt;
        }

        /// <summary>
        /// Update Database Table From DataTable - The DataTable Must Be Eqv To DataBase Table.
        /// Remove All Identity Specification ID's.
        /// </summary>
        /// <param name="config"></param>
        [Obsolete("Idea to update table that is held in cache.")]
        private static void AMXEditModeUpdateTable(EditModeConfig config)
        {
            DataTable dt = config.Data.Copy();
            dt.Columns.Remove("EditMode");
            if (config.Data != null && config.Data.Rows.Count > 0)
            {
                IDBParser dB = DbUtils.GetDBType();
                foreach (DataRow dr in config.Data.Rows)
                {
                    switch (dr["editmode"])
                    {
                        case -1:
                            dB.EndParameter(config.PrimaryKey, dr[config.PrimaryKey]);
                            dB.Delete(config.TableName, config.WhereQuery);
                            break;
                        case 1:
                            RowParameters(dt, dr, dB);
                            dB.Update(config.TableName, config.WhereQuery);
                            break;
                        case 2:
                            //Removing Primary Key.
                            dt.Columns.Remove(config.PrimaryKey);
                            DataRow dataRow = dt.Rows[dt.Rows.IndexOf(dr)];
                            RowParameters(dt, dataRow, dB);
                            dB.Create(config.TableName);
                            break;
                        default:
                            break;
                    }
                }

            }
        }
        public class EditModeConfig
        {
            public string WhereQuery { get; set; }
            public string PrimaryKey { get; set; }
            public string TableName { get; set; }
            public DataTable Data { get; set; }
        }
        private static void RowParameters(DataTable dt, DataRow dr, IDBParser dB)
        {
            //Loop for all except last.
            string ColumnName = string.Empty;
            int LastIndex = dt.Columns.Count - 1;
            for (int i = 0; i < LastIndex - 1; i++)
            {
                ColumnName = dt.Columns[i].ColumnName;
                dB.AddParameter(ColumnName, dr[ColumnName]);
            }
            //Identify the last
            ColumnName = dt.Columns[LastIndex].ColumnName;
            dB.EndParameter(ColumnName, dr[ColumnName]);
        }
    }
}
