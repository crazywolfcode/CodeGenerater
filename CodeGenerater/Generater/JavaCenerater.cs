
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlDao;

namespace CodeGenerater
{
    class JavaGenerater : BaseGenerater
    {        
        private List<MysqlTableColumnSchema> mMysqlTableColumnSchema;
        private List<SqliteTableSchema> mSqliteTableSchemas;
        string PropertyT = "private {0} {1};";
        private string getGet(string type, string property)
        {
            StringBuilder sb = new StringBuilder();
            //sb.AppendLine(tab + string.Format("public {0} get{1}() {", type, StringHelper.upperCaseFirstLetter(property)));
            sb.AppendLine(tab + $"public {type} get{StringHelper.upperCaseFirstLetter(property)}()" + "{");
            sb.AppendLine(tab + tab + $"return {property};");
            sb.AppendLine(tab + "}");
            return sb.ToString();
        }

        private string getSet(string property, string type)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(tab + $"public void set{StringHelper.upperCaseFirstLetter(property)}({type} {property})" + "{");
            sb.AppendLine(tab + tab + $" this.{property} = {property};");
            sb.AppendLine(tab + "}");
            return sb.ToString();
        }

        public JavaGenerater(LocalSchema schema, Connection connection)
        {
            mDbLocalSchema = schema;
            mTableName = schema.TableName;
            mConnection = connection;
            ClassNmae = StringHelper.dbNameToClassName(StringHelper.DBNamingToCamelCase(mTableName));
        }

        private string getHeader()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Format("package {0}.entity;", mConnection.pakeage));
            sb.AppendLine();
            sb.AppendLine("import java.util.Date;");
            sb.AppendLine("");
            sb.AppendLine(GetClassComment());
            if (!string.IsNullOrEmpty(mConnection.classSuffix))
            {
                ClassNmae = ClassNmae + StringHelper.upperCaseFirstLetter(mConnection.classSuffix);
            }
            sb.AppendLine(string.Format("public class {0}", ClassNmae) + "{");
            sb.AppendLine("");
            return sb.ToString();
        }

        private string GetProerty(string type, string field)
        {
            return string.Format(PropertyT, type, StringHelper.DBNamingToCamelCase(field));
        }
        private string GetJavaComment(string comment, string isNull, string defaultValue)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(tab + "/**");
            if (!string.IsNullOrWhiteSpace(comment))
            {
                sb.AppendLine(tab + "* " + comment);
            }
            if (!string.IsNullOrWhiteSpace(isNull))
            {
                sb.AppendLine(tab + "* 可空:" + isNull);
            }
            if (!string.IsNullOrWhiteSpace(defaultValue))
            {
                sb.AppendLine(tab + "* 默认值:" + defaultValue);
            }
            sb.AppendLine(tab + "*/");
            return sb.ToString();
        }

        private string GetPropertyComment(string comment, string isNull, string defaultValue)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(tab + "//");
            if (!string.IsNullOrWhiteSpace(comment))
            {
                sb.Append(" " + comment);
            }
            if (!string.IsNullOrWhiteSpace(isNull))
            {
                sb.Append(" 可空:" + isNull);
            }
            if (!string.IsNullOrWhiteSpace(defaultValue))
            {
                sb.Append(" 默认值:" + defaultValue);
            }
            return sb.ToString();
        }
        private string getMethodComment(string comment)
        {
            if (string.IsNullOrEmpty(comment))
                return null;
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(tab + "/**");
            if (!string.IsNullOrWhiteSpace(comment))
            {
                sb.AppendLine(tab + "* " + comment);
            }
            sb.AppendLine(tab + "*/");
            return sb.ToString();
        }

        public string CeneraterClass()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(getHeader());
            if (mConnection.type == DbType.mysql.ToString())
            {
                mMysqlTableColumnSchema = MySqlHelperInstance.GetTableColumnSchema(mConnection.dbName, mTableName);
                for (int i = 0; i < mMysqlTableColumnSchema.Count; i++)
                {
                    MysqlTableColumnSchema schema = mMysqlTableColumnSchema[i];
                    sb.AppendLine(GetPropertyComment(schema.CommentValue, schema.IsNullable, schema.DefaultValue));
                    sb.AppendLine(tab + GetProerty(JavaDbTypeMap.FindType(schema.Type), schema.ColumnName));
                    sb.AppendLine();
                }
                for (int i = 0; i < mMysqlTableColumnSchema.Count; i++)
                {
                    MysqlTableColumnSchema schema = mMysqlTableColumnSchema[i];
                    string metodstr = getMethodComment(schema.CommentValue);
                    sb.AppendLine(metodstr);
                    sb.AppendLine(getGet(JavaDbTypeMap.FindType(schema.Type), StringHelper.DBNamingToCamelCase(schema.ColumnName)));
                    //sb.AppendLine();
                    sb.AppendLine(metodstr);
                    sb.AppendLine(getSet(StringHelper.DBNamingToCamelCase(schema.ColumnName), JavaDbTypeMap.FindType(schema.Type)));
                }
            }
            else
            {
                mSqliteTableSchemas = SQLiteHelperInstance.GetTableSchema<SqliteTableSchema>(mTableName);
                for (int i = 0; i < mSqliteTableSchemas.Count; i++)
                {
                    SqliteTableSchema schema = mSqliteTableSchemas[i];
                    sb.AppendLine(GetJavaComment(null, schema.Notnull, schema.Dflt_value));
                    sb.AppendLine(tab + GetProerty(JavaDbTypeMap.FindType(schema.Type), schema.Name));
                    sb.AppendLine();
                }
                for (int i = 0; i < mSqliteTableSchemas.Count; i++)
                {
                    SqliteTableSchema schema = mSqliteTableSchemas[i];
                    sb.AppendLine(getGet(JavaDbTypeMap.FindType(schema.Type), StringHelper.DBNamingToCamelCase(schema.Name)));
                    sb.AppendLine();
                    sb.AppendLine(getSet(StringHelper.DBNamingToCamelCase(schema.Name), JavaDbTypeMap.FindType(schema.Type)));
                }
            }
            sb.AppendLine("}");
            return sb.ToString();
        }
    }
}
