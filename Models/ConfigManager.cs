using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dBParser.Models
{
    //public class ConfigManager
    //{
    //    public string Token { get; set; }
    //    public string ConnectionString { get; set; }
    //}
    public class ConnectionStrings
    {
        public string DataBaseType { get; set; }
        public string DefaultConnection { get; set; }
    }

    public class ConfigManager
    {
        public ConnectionStrings ConnectionStrings { get; set; }
        public string token { get; set; }
    }

    public class RootObject
    {
        public ConfigManager ConfigManager { get; set; }
    }

     
   

}
