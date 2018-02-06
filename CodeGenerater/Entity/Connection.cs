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
        public string classSuffix { get; set; }
        public string enumSuffi { get; set; }
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
        public string pakeage { get; set; }
        public string javaClassPath { get; set; }
        public string javaEnumPath { get; set; }
        public string nameSpace { get; set; }
        public string cSharpClassPath { get; set; }
        public string cSharpEnumPath { get; set; }
        /// <summary>
        /// 将整个数据库的表和表的列，保存到一个C#文件中
        /// </summary>
        public string cSharpEnumAllPath { get; set; }
        public string sqlPath { get; set; }
        public string auto { get; set; }
    }
}
