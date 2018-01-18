using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerater
{
    class CSharpCenerater : BaseGenerater
    {
        public CSharpCenerater(MyHelper.DbSchema schema, Connection connection)
        {
            mDbSchema = schema;
            mTableName = schema.TableName;
            mConnection = connection;
            ClassNmae = MyHelper.StringHelper.dbNameToClassName(MyHelper.StringHelper.DBNamingToCamelCase(mTableName));
        }
        private string getHeader()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Linq;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.Text;");
            sb.AppendLine("");
            sb.AppendLine(string.Format(" namespace {0}", mConnection.nameSpace));
            sb.AppendLine("{");
            sb.AppendLine("");
            sb.AppendLine(getClassComment());
            sb.AppendLine("");
            sb.AppendLine(tab + string.Format(" public  class {0}", ClassNmae));
            sb.AppendLine(tab + "{");
            return sb.ToString();
        }

        private string getProerty(string type, string field)
        {
            return tab + $"public {type} {MyHelper.StringHelper.DBNamingToCamelCase(field)}" + "{ get; set; }";
        }

        public string getcomment(string comment, string isNull, string defaultValue)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(tab + "/// <summary>");
            if (!string.IsNullOrWhiteSpace(comment))
            {
                sb.AppendLine(tab + "/// 注释:" + comment);
            }
            if (!string.IsNullOrWhiteSpace(isNull))
            {
                sb.AppendLine(tab + "/// 可空:" + isNull);
            }
            if (!string.IsNullOrWhiteSpace(defaultValue))
            {
                sb.AppendLine(tab + "///默认值:" + defaultValue);
            }
            sb.AppendLine(tab + "/// </summary>");
            return sb.ToString();
        }

        public string getClassComment()
        {
            StringBuilder sb = new StringBuilder();
            if (mDbSchema == null)
            {
                return null;
            }
            sb.AppendLine(tab + "/// <summary>");
            if (!string.IsNullOrWhiteSpace(mDbSchema.TableComment))
            {
                sb.AppendLine(tab + "/// " + mDbSchema.TableComment);
            }
            if (!string.IsNullOrWhiteSpace(mDbSchema.tableRows))
            {
                sb.AppendLine(tab + "/// 数据条数:" + mDbSchema.tableRows);
            }
            if (!string.IsNullOrWhiteSpace(mDbSchema.dataLength))
            {
                sb.AppendLine(tab + "/// 数据大小:" + MyHelper.FileHelper.ConverterFileSizeUnit(Convert.ToInt64(mDbSchema.dataLength)));
            }
            sb.AppendLine(tab + "/// </summary>");
            return sb.ToString();
        }
        public string CeneraterClass()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(getHeader());
            if (mConnection.type == DbType.mysql.ToString())
            {
                List<MyHelper.MysqlTableColumnSchema> schemas = new MyHelper.MySqlHelper(mConnection.connStr).getTableColumnSchema(mConnection.dbName, mTableName);
                for (int i = 0; i < schemas.Count; i++)
                {
                    MyHelper.MysqlTableColumnSchema schema = schemas[i];
                    sb.AppendLine(getcomment(schema.commentValue, schema.isNullable, schema.defaultValue));
                    sb.AppendLine(getProerty(CSharpDbTypeMap.FindType(schema.type), schema.columnName));
                    sb.AppendLine();
                }
            }
            else
            {
                List<MyHelper.SqliteTableSchema> schemas = new MyHelper.SQLiteHelper(mConnection.connStr).getTableSchema(mTableName);
                for (int i = 0; i < schemas.Count; i++)
                {
                    MyHelper.SqliteTableSchema schema = schemas[i];
                    sb.AppendLine(getcomment(null, schema.notnull, schema.dflt_value));
                    sb.AppendLine(getProerty(CSharpDbTypeMap.FindType(schema.type), schema.name));
                    sb.AppendLine();
                }
            }

            sb.AppendLine(tab + "}");
            sb.AppendLine("}");
            return sb.ToString();
        }
    }
}
