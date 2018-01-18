using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerater
{
    class JavaGenerater:BaseGenerater
    {    
        private List<MyHelper.MysqlTableColumnSchema> mMysqlTableColumnSchema;
        private List<MyHelper.SqliteTableSchema> mSqliteTableSchemas;
        string PropertyT = "public {0} {1};";     
        public string getGet(string type, string property)
        {
            StringBuilder sb = new StringBuilder();
            //sb.AppendLine(tab + string.Format("public {0} get{1}() {", type, MyHelper.StringHelper.upperCaseFirstLetter(property)));
            sb.AppendLine(tab + $"public {type} get{MyHelper.StringHelper.upperCaseFirstLetter(property)}()" + "{");
            sb.AppendLine(tab + tab + $"return {property};");
            sb.AppendLine(tab + "}");
            return sb.ToString();
        }

        public string getSet(string property, string type)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(tab + $"public void set{MyHelper.StringHelper.upperCaseFirstLetter(property)}({type} {property})" + "{");
            sb.AppendLine(tab + tab + $" this.{property} = {property};");
            sb.AppendLine(tab + "}");
            return sb.ToString();
        }

        public JavaGenerater(MyHelper.DbSchema schema, Connection connection)
        {          
            mDbSchema = schema;
            mTableName = schema.TableName;
            mConnection = connection;
            ClassNmae = MyHelper.StringHelper.dbNameToClassName(MyHelper.StringHelper.DBNamingToCamelCase(mTableName));

        }
        private string getHeader()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Format("package {0};", mConnection.pakeage));
            sb.AppendLine();
            sb.AppendLine("import java.util.Date;");
            sb.AppendLine("");
            sb.AppendLine(getClassComment());
            sb.AppendLine(string.Format("public class {0}", ClassNmae) + "{");
            sb.AppendLine("");
            return sb.ToString();
        }

        private string getProerty(string type, string field)
        {
            return string.Format(PropertyT, type, MyHelper.StringHelper.DBNamingToCamelCase(field));
        }

        public string getcomment(string comment, string isNull, string defaultValue)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(tab + "/**");
            if (!string.IsNullOrWhiteSpace(comment))
            {
                sb.AppendLine(tab + "* " + comment);
            }
            if (!string.IsNullOrWhiteSpace(isNull))
            {
                sb.AppendLine(tab + "* @可空:" + isNull);
            }
            if (!string.IsNullOrWhiteSpace(defaultValue))
            {
                sb.AppendLine(tab + "* @默认值:" + defaultValue);
            }
            sb.AppendLine(tab + "*/");
            return sb.ToString();
        }

        public string getMethodComment(string comment)
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

        public string getClassComment()
        {
            StringBuilder sb = new StringBuilder();
            if (mDbSchema == null)
            {
                return null;
            }
            sb.AppendLine("/**");
            if (!string.IsNullOrWhiteSpace(mDbSchema.TableComment))
            {
                sb.AppendLine("* " + mDbSchema.TableComment);
            }
            if (!string.IsNullOrWhiteSpace(mDbSchema.tableRows))
            {
                sb.AppendLine("* 数据条数:" + mDbSchema.tableRows);
            }
            if (!string.IsNullOrWhiteSpace(mDbSchema.dataLength))
            {
                sb.AppendLine("* 数据大小:" + MyHelper.FileHelper.ConverterFileSizeUnit(Convert.ToInt64(mDbSchema.dataLength)));
            }
            sb.AppendLine("*/ ");
            return sb.ToString();
        }
        public string CeneraterClass()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(getHeader());
            if (mConnection.type == DbType.mysql.ToString())
            {
                mMysqlTableColumnSchema = new MyHelper.MySqlHelper(mConnection.connStr).getTableColumnSchema(mConnection.dbName, mTableName);
                for (int i = 0; i < mMysqlTableColumnSchema.Count; i++)
                {
                    MyHelper.MysqlTableColumnSchema schema = mMysqlTableColumnSchema[i];
                    sb.AppendLine(getcomment(null, schema.isNullable, schema.defaultValue));
                    sb.AppendLine(tab + getProerty(JavaDbTypeMap.FindType(schema.type), schema.columnName));
                    sb.AppendLine();
                }
                for (int i = 0; i < mMysqlTableColumnSchema.Count; i++)
                {
                    MyHelper.MysqlTableColumnSchema schema = mMysqlTableColumnSchema[i];
                    string metodstr = getMethodComment(schema.commentValue);
                    sb.AppendLine(metodstr);
                    sb.AppendLine(getGet(JavaDbTypeMap.FindType(schema.type), MyHelper.StringHelper.DBNamingToCamelCase(schema.columnName)));
                    sb.AppendLine();
                    sb.AppendLine(metodstr);
                    sb.AppendLine(getSet(MyHelper.StringHelper.DBNamingToCamelCase(schema.columnName), JavaDbTypeMap.FindType(schema.type)));
                }
            }
            else
            {
                mSqliteTableSchemas = new MyHelper.SQLiteHelper(mConnection.connStr).getTableSchema(mTableName);
                for (int i = 0; i < mSqliteTableSchemas.Count; i++)
                {
                    MyHelper.SqliteTableSchema schema = mSqliteTableSchemas[i];
                    sb.AppendLine(getcomment(null, schema.notnull, schema.dflt_value));
                    sb.AppendLine(tab + getProerty(JavaDbTypeMap.FindType(schema.type), schema.name));
                    sb.AppendLine();
                }
                for (int i = 0; i < mSqliteTableSchemas.Count; i++)
                {
                    MyHelper.SqliteTableSchema schema = mSqliteTableSchemas[i];
                    sb.AppendLine(getGet(JavaDbTypeMap.FindType(schema.type), MyHelper.StringHelper.DBNamingToCamelCase(schema.name)));
                    sb.AppendLine();
                    sb.AppendLine(getSet(MyHelper.StringHelper.DBNamingToCamelCase(schema.name), JavaDbTypeMap.FindType(schema.type)));
                }
            }
            sb.AppendLine("}");
            return sb.ToString();
        }


    }
}
