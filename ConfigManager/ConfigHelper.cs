using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
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
            var pathRegex = new Regex(@"\\bin(\\x86|\\x64)?\\(Debug|Release)$", RegexOptions.Compiled);
            var directory = pathRegex.Replace(Directory.GetCurrentDirectory(), String.Empty);
            var builder = new ConfigurationBuilder()
            .SetBasePath(directory)
                 .AddJsonFile(@"appsettings.json");
            Configuration = builder.Build();
            
        }


    }
}
