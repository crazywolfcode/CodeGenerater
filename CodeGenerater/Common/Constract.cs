using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerater
{
    class Constract
    {
        public static string ConnPath = System.IO.Path.Combine(MyHelper.FileHelper.GetRunTimeRootPath(), "temp");
        public static string ConnFilePath = ConnPath + "\\" + "connection.xml";
    }
}
