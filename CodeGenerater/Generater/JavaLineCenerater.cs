
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
    class JavaLineGenerater : BaseGenerater
    {        
        private List<MysqlTableColumnSchema> mMysqlTableColumnSchema;
        private List<SqliteTableSchema> mSqliteTableSchemas;
        string PropertyT = "private {0} {1}; //{2}";
  
        public JavaLineGenerater(LocalSchema schema, Connection connection)
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

        private string GetProerty(string type, string field ,string comment = " ")
        {
            return string.Format(PropertyT, type, StringHelper.DBNamingToCamelCase(field),comment);
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
                    sb.AppendLine(tab + GetProerty(JavaDbTypeMap.FindType(schema.Type), schema.ColumnName,schema.CommentValue));                    
                }        
            }
            else
            {
                mSqliteTableSchemas = SQLiteHelperInstance.GetTableSchema<SqliteTableSchema>(mTableName);
                for (int i = 0; i < mSqliteTableSchemas.Count; i++)
                {
                    SqliteTableSchema schema = mSqliteTableSchemas[i];                 
                    sb.AppendLine(tab + GetProerty(JavaDbTypeMap.FindType(schema.Type), schema.Name));        
                }
           
            }
            sb.AppendLine("}");
            return sb.ToString();
        }
    }
}
