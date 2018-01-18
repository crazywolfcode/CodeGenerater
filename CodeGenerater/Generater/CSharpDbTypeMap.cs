using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerater
{
    public sealed class CSharpDbTypeMap
    {
        private static Dictionary<string, Type> DbList = null;
        private static void GetSQLiteDbTypeMap()
        {
            if (DbList != null) return;
            DbList = new Dictionary<string, Type>
            {
                {"BIGINT", typeof(Int64)},
                {"BIGUINT", typeof(UInt64)},
                {"BINARY", typeof(byte)},
                {"BIT", typeof(bool)},
                {"BLOB", typeof(byte)},
                {"BOOL", typeof(bool)},
                {"BOOLEAN", typeof(bool)},
                {"CHAR", typeof(string)},
                {"CLOB", typeof(string)},
                {"COUNTER", typeof(Int64)},
                {"CURRENCY",typeof(decimal)},
                {"DATE", typeof(DateTime)},
                {"DATETIME", typeof(DateTime)},
                {"DECIMAL", typeof(decimal)},
                {"DOUBLE", typeof(double)},
                {"FLOAT", typeof(double)},
                {"GENERAL", typeof(Byte)},
                {"GUID", typeof(string)},
                {"IDENTITY",typeof(Int64)},
                {"IMAGE", typeof(byte[])},
                {"INT", typeof(int)},
                {"INT8", typeof(byte)},
                {"INT16", typeof(int)},
                {"INT32", typeof(int)},
                {"INT64", typeof(Int64)},
                {"INTEGER", typeof(Int64)},
                {"INTEGER8", typeof(byte)},
                {"INTEGER16", typeof(Int16)},
                {"INTEGER32", typeof(Int32)},
                {"INTEGER64", typeof(Int64)},
                {"LOGICAL", typeof(bool)},
                {"LONG", typeof(Int64)},
                {"LONGCHAR", typeof(String)},
                {"LONGTEXT", typeof(String)},
                {"LONGVARCHAR", typeof(String)},
                {"MEMO", typeof(String)},
                {"MONEY", typeof(Decimal)},
                {"NCHAR", typeof(String)},
                {"NOTE", typeof(String)},
                {"NTEXT", typeof(String)},
                {"NUMBER", typeof(Decimal)},
                {"NUMERIC", typeof(Decimal)},
                {"NVARCHAR", typeof(String)},
                {"OLEOBJECT", typeof(byte[])},
                {"RAW", typeof(byte[])},
                {"REAL", typeof(double)},
                {"SINGLE", typeof(Single)},
                {"SMALLDATE", typeof(DateTime)},
                {"SMALLINT", typeof(Int16)},
                {"SMALLUINT", typeof(UInt16)},
                {"STRING", typeof(String)},
                {"TEXT", typeof(String)},
                {"TIME", typeof(DateTime)},
                {"TIMESTAMP", typeof(DateTime)},
                {"TINYINT", typeof(Int16)},
                {"TINYSINT", typeof(byte)},
                {"UINT", typeof(Int64)},
                {"UINT8", typeof(byte)},
                {"UINT16", typeof(Int16)},
                {"UINT32",typeof(UInt32)},
                {"UINT64", typeof(UInt64)},
                {"ULONG", typeof(UInt64)},
                {"VARCHAR", typeof(string)},
                {"VARCHAR2", typeof(string)},
                {"YESNO", typeof(bool)}
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
            Type db = null;
            if (DbList.TryGetValue(typeName.ToUpper(), out db))
            {
                return db.Name;
            }
            return typeof(string).Name;
        }
    }
}
