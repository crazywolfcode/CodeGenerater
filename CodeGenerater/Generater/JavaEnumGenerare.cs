using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerater
{
    class JavaEnumGenerare : BaseGenerater { 
        private List<MyHelper.DbSchema> mDbSchemas;
        public JavaEnumGenerare(Connection connection)
        {           
            mConnection = connection;
        }

        public string dbEnumGenerater()
        {
            if (mDbSchemas.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
            }
            return null;
        }

        public string tableEnumGenerater(MyHelper.DbSchema schema)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine();
            if (!string.IsNullOrEmpty(schema.TableComment))
            {
                sb.AppendLine(getcomment(schema.TableComment));
            }
            string name = MyHelper.StringHelper.upperCaseFirstLetter(MyHelper.StringHelper.DBNamingToCamelCase(schema.TableName));
            if (!string.IsNullOrEmpty(mConnection.enumSuffi))
            {
                name = name + MyHelper.StringHelper.upperCaseFirstLetter(mConnection.enumSuffi);
            }
            sb.AppendLine(tab + $"public enum {name}" + "{");
            if (mConnection.type == DbType.mysql.ToString())
            {
                List<MyHelper.MysqlTabeSchema> myschemas = new MyHelper.MySqlHelper(mConnection.connStr).getTableSchema(schema.TableName);
                for (int i = 0; i < myschemas.Count; i++)
                {
                    sb.AppendLine(tab + tab + myschemas[i].Field + comma);
                }
            }
            else
            {
                List<MyHelper.SqliteTableSchema> myschemas = new MyHelper.SQLiteHelper(mConnection.connStr).getTableSchema(schema.TableName);
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
            mDbSchemas = new MyHelper.MySqlHelper(mConnection.connStr).getAllTableSchema(mConnection.dbName);
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
