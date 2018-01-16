using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerater
{
    /// <summary>
    /// 
    /// </summary>
    public class Connection
    {
        public string id { get; set; }
        public string name { get; set; }
        public string dbName { get; set; }
        public string description { get; set; }
        public string type { get; set; }
        public string filePath { get; set; }
        public string ipAddress { get; set; }
        public string port { get; set; }
        public string uaerName { get; set; }
        public string password { get; set; }
        public string addTime { get; set; }
        public string connStr { get; set; }
    }
}
