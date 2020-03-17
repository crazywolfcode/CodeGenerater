using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerater
{
    class Constract
    {
        public static string DateTimeFormart = "yyyy - MM - dd HH:mm:ss";
        public static string ConnPath = System.IO.Path.Combine(FileHelper.GetRunTimeRootPath(), "temp");
        public static string ConnFilePath = ConnPath + "\\" + "connection.xml";
        public static string dBschemasPath= System.IO.Path.Combine(FileHelper.GetRunTimeRootPath(), "temp");       
        public static string getDbdBschemasPath(string guid)
        {
            return dBschemasPath + "\\" + guid.Replace("-","") + ".xml";
        }
  
    }
}
