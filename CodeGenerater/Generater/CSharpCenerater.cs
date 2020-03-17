using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlDao;
namespace CodeGenerater
{
    class CSharpCenerater : BaseGenerater
    {
        public CSharpCenerater(LocalSchema schema, Connection connection)
        {
            mDbLocalSchema = schema;
            mTableName = schema.TableName;
            mConnection = connection;
            ClassNmae = StringHelper.dbNameToClassName(StringHelper.DBNamingToCamelCase(mTableName));
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
            if (!string.IsNullOrEmpty(mConnection.classSuffix))
            {
                ClassNmae = ClassNmae + StringHelper.upperCaseFirstLetter(mConnection.classSuffix);
            }
            sb.AppendLine(tab + string.Format(" public  class {0}", ClassNmae));
            sb.AppendLine(tab + "{");
            return sb.ToString();
        }

        private string getProerty(string type, string field)
        {
            string fieldName = StringHelper.DBNamingToCamelCase(field);
            return tab + $"public {type} {StringHelper.upperCaseFirstLetter(fieldName)}" + "{ get; set; }";
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
            if (mDbLocalSchema == null)
            {
                return null;
            }
            sb.AppendLine(tab + "/// <summary>");
            if (!string.IsNullOrWhiteSpace(mDbLocalSchema.TableComment))
            {
                sb.AppendLine(tab + "/// " + mDbLocalSchema.TableComment);
            }
            if (!string.IsNullOrWhiteSpace(mDbLocalSchema.tableRows))
            {
                sb.AppendLine(tab + "/// 数据条数:" + mDbLocalSchema.tableRows);
            }
            if (!string.IsNullOrWhiteSpace(mDbLocalSchema.dataLength))
            {
                sb.AppendLine(tab + "/// 数据大小:" + FileHelper.ConverterFileSizeUnit(Convert.ToInt64(mDbLocalSchema.dataLength)));
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
                List<MysqlTableColumnSchema> schemas = MySqlHelperInstance.GetTableColumnSchema(mConnection.dbName, mTableName);
                for (int i = 0; i < schemas.Count; i++)
                {
                    MysqlTableColumnSchema schema = schemas[i];
                    sb.AppendLine(getcomment(schema.CommentValue, schema.IsNullable, schema.DefaultValue));
                    sb.AppendLine(getProerty(CSharpDbTypeMap.FindType(schema.Type), schema.ColumnName));
                    sb.AppendLine();
                }
            }
            else
            {
                List<SqliteTableSchema> schemas = SQLiteHelperInstance.GetTableSchema<SqliteTableSchema>(mTableName);
                for (int i = 0; i < schemas.Count; i++)
                {
                    SqliteTableSchema schema = schemas[i];
                    sb.AppendLine(getcomment(null, schema.Notnull, schema.Dflt_value));
                    sb.AppendLine(getProerty(CSharpDbTypeMap.FindType(schema.Type), schema.Name));
                    sb.AppendLine();
                }
            }

            sb.AppendLine(tab + "}");
            sb.AppendLine("}");
            return sb.ToString();
        }
    }
}
