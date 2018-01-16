using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerater
{
    class JavaEnumGenerare
    {
        private string mpacename;
        private string connstr;
        private string mDbtype;
        private string tab = "\t";
        private string comma = ",";
        private List<MyHelper.DbSchema> mDbSchemas;
        public JavaEnumGenerare(string pacename, string mysqlconn = null, string dbtype = null)
        {
            mpacename = pacename;
            connstr = mysqlconn;
            mDbtype = dbtype;
        }

        public string dbEnumGenerater()
        {
            if (mDbSchemas.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
            }
            return null;
        }

        public string tableEnumGenerater(string tableName, string commend = null)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine();
            if (!string.IsNullOrEmpty(commend))
            {
                sb.AppendLine(getcomment(commend));
            }
            string name = MyHelper.StringHelper.upperCaseFirstLetter(MyHelper.StringHelper.DBNamingToCamelCase(tableName));
            sb.AppendLine(tab + $"public enum {name}" + "{");
            if (mDbtype == DbType.mysql.ToString())
            {
                List<MyHelper.MysqlTabeSchema> myschemas = new MyHelper.MySqlHelper(connstr).getTableSchema(tableName);
                for (int i = 0; i < myschemas.Count; i++)
                {
                    sb.AppendLine(tab + tab + myschemas[i].Field + comma);
                }
            }
            else
            {
                List<MyHelper.SqliteTableSchema> myschemas = new MyHelper.SQLiteHelper(connstr).getTableSchema(tableName);
                for (int i = 0; i < myschemas.Count; i++)
                {
                    sb.AppendLine(tab + tab + myschemas[i].name + comma);
                }
            }
            sb.AppendLine(tab + "}");
            return sb.ToString();
        }

        private void getDbSchema()
        {
            mDbSchemas = new MyHelper.MySqlHelper(connstr).getAllTableSchema();
        }
        public string getcomment(string comment)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(tab + "/**");
            sb.AppendLine(tab + "*" + comment);
            sb.AppendLine(tab + "*/");
            return sb.ToString();
        }
    }
}
