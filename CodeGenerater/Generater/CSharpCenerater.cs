using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerater
{
    class CSharpCenerater
    {
        string codeNameSpace;
        string ClassNmae;
        string ClassComment;
        string mConnstring;
        string mTableName;
        string mDbType;
        string mDbName;
        string tab = "\t";

        public CSharpCenerater(string nspace, string comment, string dbname, string tableName, string conn, string dbtype)
        {
            codeNameSpace = nspace;
            ClassComment = comment;
            mConnstring = conn;
            mTableName = tableName;
            mDbType = dbtype;
            mDbName = dbname;
            ClassNmae = MyHelper.StringHelper.dbNameToClassName(MyHelper.StringHelper.DBNamingToCamelCase(tableName));
        }
        private string getHeader()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Linq;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.Text;");
            sb.AppendLine("");
            sb.AppendLine(string.Format(" namespace {0}", codeNameSpace));
            sb.AppendLine("{");
            sb.AppendLine("");
            sb.AppendLine(getClassComment(ClassComment));
            sb.AppendLine("");
            sb.AppendLine(tab + string.Format(" public  class {0}", ClassNmae));
            sb.AppendLine(tab + "{");
            return sb.ToString();
        }

        private string getProerty(string type, string field)
        {           
            return tab+$"public {type} {MyHelper.StringHelper.DBNamingToCamelCase(field)}"+ "{ get; set; }";
        }

        public string getcomment(string comment, string isNull, string defaultValue)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(tab+"/// <summary>");
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

        public string getClassComment(string comment)
        {
            StringBuilder sb = new StringBuilder();
            if (string.IsNullOrEmpty(comment))
            {
                return null;
            }
            sb.AppendLine(tab+"/// <summary>");
            if (!string.IsNullOrWhiteSpace(comment))
            {
                sb.AppendLine(tab + "/// " + comment);
            }
            sb.AppendLine(tab + "/// </summary>");
            return sb.ToString();
        }
        public string CeneraterClass()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(getHeader());
            if (mDbType == DbType.mysql.ToString()) {
                List<MyHelper.MysqlTableColumnSchema> schemas = new MyHelper.MySqlHelper(mConnstring).getTableColumnSchema(mDbName,mTableName);
                for (int i = 0; i < schemas.Count; i++)
                {
                    MyHelper.MysqlTableColumnSchema schema = schemas[i];
                    sb.AppendLine(getcomment(null, schema.isNullable, schema.defaultValue));
                    sb.AppendLine(getProerty(CSharpDbTypeMap.FindType(schema.type), schema.columnName));
                    sb.AppendLine();
                }
            } else {
                List<MyHelper.SqliteTableSchema> schemas = new MyHelper.SQLiteHelper(mConnstring).getTableSchema(mTableName);
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
