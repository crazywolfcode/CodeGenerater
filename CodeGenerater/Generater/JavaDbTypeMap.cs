using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerater
{
   public class JavaDbTypeMap
    {
        private static Dictionary<string, string> DbList = null;
        private static void GetSQLiteDbTypeMap()
        {
            if (DbList != null) return;
            DbList = new Dictionary<string, string>
            {
                {"VARCHAR", "String"},
                {"CHAR", "String"},
                {"TEXT", "String"},
                {"BLOB", " byte[]"},            
                {"TINYINT", "Integer"},
                {"SMALLINT", "Integer"},
                {"MEDIUMINT", "Integer"},               
                {"INT", "Integer"},
                {"FLOAT", "Float"},
                {"DOUBLE", "Double"},
                {"BIGINT", "java.math.BigIntege"},
                {"DECIMAL", "java.math.BigDecimal"},
                {"BOOLEAN", "Integer"},
                {"INTEGER", "Long"},
                {"BIT", "Boolean"},
                {"DATE", "Date"},
                {"DATETIME", "Date"},
                {"TIME", "Date"},
                {"YEAR", "Date"},
            };
        }

        public static string FindType(string typeName)
        {
            GetSQLiteDbTypeMap();
            int index = typeName.IndexOf("(", StringComparison.InvariantCultureIgnoreCase);
            if (index > 0)
            {
                typeName = typeName.Substring(0, index);
            }
            string t = null;
            if (DbList.TryGetValue(typeName.ToUpper(), out t))
            {
                return t;
            }
            return "String";
        }
    }
}
