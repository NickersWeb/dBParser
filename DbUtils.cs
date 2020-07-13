using Microsoft.SqlServer.Server;
using System;
using System.Data;
using System.Runtime.Caching;

namespace dBParser
{
    public static class DbUtils
    {
        public static IDBParser GetDBType() {
            ConfigManager.ConfigHelper config = new ConfigManager.ConfigHelper();
            switch (config.DataBaseType())
            {
                case "sql":
                    return new MSSQLParser();
                case "sqlite":
                    return new SQLiteParser();
                default:
                    return new MSSQLParser();
            }
          
        }
        public static void DeleteCache(string Name)
        {
            MemoryCache.Default.Remove(Name);
        }
        public static void UpdateExtendPropCache(string Name, string Value, string TableName = "", string ColumnName = "")
        {
            string AdvName = $"{Name}{TableName}{ColumnName}";
            DeleteCache(AdvName);
            MemoryCache.Default.Add(AdvName, Value, DateTime.Now.AddMinutes(15));
        }
        public static string ReadExtendedPropCache(string Name, bool Database = false, string TableName = "", string ColumnName = "")
        {
            IDBParser dB = GetDBType();
            string AdvName = $"{Name}{TableName}{ColumnName}";
            string value = (string)MemoryCache.Default[AdvName];

            if (string.IsNullOrEmpty(value))
            {
                value = dB.ReadExtendProperty(Name, Database, TableName, ColumnName);
                MemoryCache.Default.Add(AdvName, value, DateTime.Now.AddMinutes(15));
            }
            return value;
        }
        public static void DeleteExtendedPropCache(string PropName)
        {
            MemoryCache.Default.Remove(PropName);
        }
        public static void InsertCache(string TableName, DataRow dr)
        {
            DataTable dt = (DataTable)MemoryCache.Default[TableName];
            if (dt != null)
            {
                dt.Rows.Add(dr);
                MemoryCache.Default.Add(TableName, dt, DateTime.Now.AddMinutes(15));
            }
        }
        public static void UpdateCache(string TableName, string Id, DataRow UpdateDr)
        {
            DataTable dt = (DataTable)MemoryCache.Default[TableName];
            if (dt != null)
            {
                DataRow dr = dt.Rows.Find(Id);
                dr = UpdateDr;
                dt.AcceptChanges();
                MemoryCache.Default.Add(TableName, dt, DateTime.Now.AddMinutes(15));
            }
        }
        public static DataTable ReadCache(string TableName, string Query = "")
        {
            IDBParser dB = GetDBType();
            DataTable dt = (DataTable)MemoryCache.Default[TableName];
            
            if (dt == null)
            {
                dt = dB.Read(Query);
                MemoryCache.Default.Add(TableName, dt, DateTime.Now.AddMinutes(15));
            }
            
            return dt;
        }

        /// <summary>
        /// Update Database Table From DataTable - The DataTable Must Be Eqv To DataBase Table.
        /// Remove All Identity Specification ID's.
        /// </summary>
        /// <param name="config"></param>
        public static void AMXEditModeUpdateTable(EditModeConfig config)
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
