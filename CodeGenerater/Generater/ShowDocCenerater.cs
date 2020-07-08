
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlDao;

/// <summary>
/// 
///java bean 注解添加到后面
/// 
/// </summary>
namespace CodeGenerater
{
    class ShowDocCenerater : BaseGenerater
    {
        private List<MysqlTableColumnSchema> mMysqlTableColumnSchema;
        private List<SqliteTableSchema> mSqliteTableSchemas;

        private string ResponseHeader1 = @"|参数名|类型|说明|";
        private string ResponseHeader2 = @"|:-----  |:-----|----- |";
        private string ResponseTemplate = "|{0}|{1}|{2}|";

        private string ParamHeader1 = @"|参数名|必选|类型|说明|";
        private string ParamHeader2 = "|:----    |:---|:----- |-----   |";
        private string ParamTemplate = "|{0}|{1}|{2}|{3}|";

        public ShowDocCenerater(LocalSchema schema, Connection connection)
        {
            mDbLocalSchema = schema;
            mTableName = schema.TableName;
            mConnection = connection;
            ClassNmae = StringHelper.dbNameToClassName(StringHelper.DBNamingToCamelCase(mTableName));
        }
        //type 1 param 2 response
        private string getHeader(int type)
        {
            StringBuilder sb = new StringBuilder();

            if (type == 1)
            {
                sb.AppendLine(ParamHeader1);
                sb.AppendLine(ParamHeader2);
            }
            else
            {
                sb.AppendLine(ResponseHeader1);
                sb.AppendLine(ResponseHeader2);
            }
            return sb.ToString();
        }

        private string GetParamProerty(string name, String need, string type = " ", string comment = "")
        {
            if (need == "NO")
            {
                need = "Y";
            }
            else
            {
                need = "N";
            }

            if (type.StartsWith("int") || type.StartsWith("INT"))
            {
                type = "int";
            }
            else if (type.ToUpper() == "DECIMAL" || type.ToUpper() == "DOUBLE" || type.ToUpper() == "FLOAT")
            {
                type = "double";
            }
            else
            {
                type = "string";
            }

            return string.Format(ParamTemplate, StringHelper.DBNamingToCamelCase(name), need, type, comment);
        }

        private string GetResponseProerty(string name, string type = " ", string comment = "")
        {
            return string.Format(ResponseTemplate, StringHelper.DBNamingToCamelCase(name), type, comment);
        }

        public string Generater()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine();
            sb.AppendLine(" **参数说明** ");
            sb.AppendLine();
            sb.AppendLine(ParamHeader1);
            sb.AppendLine(ParamHeader2);
            if (mConnection.type == DbType.mysql.ToString())
            {
                mMysqlTableColumnSchema = MySqlHelperInstance.GetTableColumnSchema(mConnection.dbName, mTableName);
                for (int i = 0; i < mMysqlTableColumnSchema.Count; i++)
                {
                    MysqlTableColumnSchema schema = mMysqlTableColumnSchema[i];
                    sb.AppendLine(GetParamProerty(schema.ColumnName, schema.IsNullable, schema.Type, schema.CommentValue));
                }
            }
            else
            {
                mSqliteTableSchemas = SQLiteHelperInstance.GetTableSchema<SqliteTableSchema>(mTableName);
                for (int i = 0; i < mSqliteTableSchemas.Count; i++)
                {
                    SqliteTableSchema schema = mSqliteTableSchemas[i];
                    sb.AppendLine(GetParamProerty(schema.Name, schema.Notnull, schema.Type));
                }

            }
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine(" **返回参数说明** ");
            sb.AppendLine();
            sb.AppendLine(ResponseHeader1);
            sb.AppendLine(ResponseHeader2);
            if (mConnection.type == DbType.mysql.ToString())
            {
                mMysqlTableColumnSchema = MySqlHelperInstance.GetTableColumnSchema(mConnection.dbName, mTableName);
                for (int i = 0; i < mMysqlTableColumnSchema.Count; i++)
                {
                    MysqlTableColumnSchema schema = mMysqlTableColumnSchema[i];
                    sb.AppendLine(GetResponseProerty(schema.ColumnName, JavaDbTypeMap.FindType(schema.Type),  schema.CommentValue));
                }
            }
            else
            {
                mSqliteTableSchemas = SQLiteHelperInstance.GetTableSchema<SqliteTableSchema>(mTableName);
                for (int i = 0; i < mSqliteTableSchemas.Count; i++)
                {
                    SqliteTableSchema schema = mSqliteTableSchemas[i];
                    sb.AppendLine( GetResponseProerty(schema.Name,JavaDbTypeMap.FindType(schema.Type)));
                }

            }
            sb.AppendLine();
            sb.AppendLine();
            return sb.ToString();
        }
    }
}
