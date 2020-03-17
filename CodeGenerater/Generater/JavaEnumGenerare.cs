using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlDao;
namespace CodeGenerater
{
    class JavaEnumGenerare : BaseGenerater { 
        private List<DbSchema> mDbSchemas;
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

        public string tableEnumGenerater(LocalSchema schema)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine();
            if (!string.IsNullOrEmpty(schema.TableComment))
            {
                sb.AppendLine(getcomment(schema.TableComment));
            }
            string name = StringHelper.upperCaseFirstLetter(StringHelper.DBNamingToCamelCase(schema.TableName));
            if (!string.IsNullOrEmpty(mConnection.enumSuffi))
            {
                name = name + StringHelper.upperCaseFirstLetter(mConnection.enumSuffi);
            }
            sb.AppendLine(tab + $"public enum {name}" + "{");
            if (mConnection.type == DbType.mysql.ToString())
            {
                List<MysqlTabeSchema> myschemas = MySqlHelperInstance.GetTableSchema<MysqlTabeSchema>(schema.TableName);
                for (int i = 0; i < myschemas.Count; i++)
                {
                    sb.AppendLine(tab + tab + myschemas[i].Field + comma);
                }
            }
            else
            {
                List<SqliteTableSchema> myschemas = SQLiteHelperInstance.GetTableSchema<SqliteTableSchema>(schema.TableName);
                for (int i = 0; i < myschemas.Count; i++)
                {
                    sb.AppendLine(tab + tab + myschemas[i].Name + comma);
                }
            }
            sb.AppendLine(tab + "}");
            return sb.ToString();
        }

        private void getDbSchema()
        {
            mDbSchemas = new MySqlHelper(mConnection.connStr).GetAllTableSchema<DbSchema>(mConnection.dbName);
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
