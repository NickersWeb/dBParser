using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

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
            
            var builder = new ConfigurationBuilder()
                 .SetBasePath(Directory.GetCurrentDirectory())
                 .AddJsonFile(@".\appsettings.json");
            Configuration = builder.Build();
            
        }


    }
}
