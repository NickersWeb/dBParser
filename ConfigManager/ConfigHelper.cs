using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace dBParser.ConfigManager
{
    public class ConfigHelper
    {
        public static IConfiguration Configuration { get; set; }

        public string ConnectionString()
        {
            return Configuration["ConfigManager:ConnectionStrings:DefaultConnection"];
        }
        public string DataBaseType()
        {
            return Configuration["ConfigManager:ConnectionStrings:DataBaseType"];

        }
        public string Token()
        {
            return Configuration["ConfigManager:token"];
        }
        public char[] TokenId()
        {
            return Configuration["ConfigManager:tokenId"].ToCharArray();
        }

        public ConfigHelper() {
            //pull from cache dbutils.readcache
            string dir = $"{Path.GetFullPath(Path.Combine(System.AppContext.BaseDirectory, @"..\..\..\"))}";
            var builder = new ConfigurationBuilder();
            builder.SetBasePath(dir).AddJsonFile("appsettings.json");
            Configuration = builder.Build();
            
        }


    }
}
