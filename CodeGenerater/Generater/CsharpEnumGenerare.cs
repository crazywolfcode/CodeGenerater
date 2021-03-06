﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlDao;
namespace CodeGenerater
{
    class CsharpEnumGenerare:BaseGenerater
    {
        private List<DbSchema> mDbSchemas;
        private List<MysqlTabeSchema> mMysqlTabeSchemas;
        private List<SqliteTableSchema> mSqliteTableSchemas;
        public CsharpEnumGenerare(Connection connection)
        {       
            mConnection = connection;      
        }

        public string dbEnumGenerater()
        {
            getDbSchema();
            if (mDbSchemas.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("using System;");
                sb.AppendLine("using System.Collections.Generic;");
                sb.AppendLine("using System.Linq;");
                sb.AppendLine("using System.Text;");
                sb.AppendLine("using System.Threading.Tasks;");
                sb.AppendLine();
                sb.AppendLine(string.Format("namespace {0}", mConnection.nameSpace));
                sb.AppendLine("{");
                sb.AppendLine();
                sb.AppendLine(getcomment("数据库中的所有表名称"));
                sb.AppendLine(tab + " public enum DataTabeName{");
                for (int i = 0; i < mDbSchemas.Count; i++)
                {
                    DbSchema sc = mDbSchemas[i];
                    sb.AppendLine(tab + tab + sc.TableName + comma);
                }
                sb.AppendLine(tab + "}");

                for (int i = 0; i < mDbSchemas.Count; i++)
                {
                    DbSchema sc = mDbSchemas[i];
                    sb.AppendLine(tableEnumGeneraterNoNameSpace(sc.TableName));
                }

                sb.AppendLine("}");
                return sb.ToString();
            }
            return null;
        }

        public string tableEnumGenerater(LocalSchema schema)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.Linq;");
            sb.AppendLine("using System.Text;");
            sb.AppendLine("using System.Threading.Tasks;");
            sb.AppendLine();
            sb.AppendLine(string.Format("namespace {0}", mConnection.nameSpace));
            sb.AppendLine("{");
            if (!string.IsNullOrEmpty(schema.TableComment))
            {
                sb.AppendLine(getcomment(schema.TableComment));
            }
            string name = StringHelper.upperCaseFirstLetter(StringHelper.DBNamingToCamelCase(schema.TableName));
            if (!string.IsNullOrEmpty(mConnection.enumSuffi)) {
                name = name + mConnection.enumSuffi;
            }
            sb.AppendLine(tab + $"public enum {name}" + "{");
            if (mConnection.type == DbType.mysql.ToString()) {
                mMysqlTabeSchemas = new MySqlHelper(mConnection.connStr).GetTableSchema<MysqlTabeSchema>(schema.TableName);
                for (int i = 0; i < mMysqlTabeSchemas.Count; i++)
                {
                    sb.AppendLine(tab + tab + mMysqlTabeSchemas[i].Field + comma);
                }
            } else {
                mSqliteTableSchemas = new SQLiteHelper(mConnection.connStr).GetTableSchema<SqliteTableSchema>(schema.TableName);
                for (int i = 0; i < mSqliteTableSchemas.Count; i++)
                {
                    sb.AppendLine(tab + tab + mSqliteTableSchemas[i].Name + comma);
                }
            }           
            sb.AppendLine(tab + "}");
            sb.AppendLine("}");
            return sb.ToString();
        }

        private string tableEnumGeneraterNoNameSpace(string tableName, string commend = null)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine();
            if (!string.IsNullOrEmpty(commend))
            {
                sb.AppendLine(getcomment(commend));
            }
            string name = StringHelper.upperCaseFirstLetter(StringHelper.DBNamingToCamelCase(tableName));
            if (!string.IsNullOrEmpty(mConnection.enumSuffi))
            {
                ClassNmae = ClassNmae + StringHelper.upperCaseFirstLetter(mConnection.enumSuffi);
            }
            String suffix = string.Empty;
            if (!String.IsNullOrEmpty(mConnection.enumSuffi)) {
                name = name + mConnection.enumSuffi;
            }                    
            sb.AppendLine(tab + $"public enum {name}" + "{");
            if (mConnection.type == DbType.mysql.ToString())
            {
                mMysqlTabeSchemas = new MySqlHelper(mConnection.connStr).GetTableSchema<MysqlTabeSchema>(tableName);
                for (int i = 0; i < mMysqlTabeSchemas.Count; i++)
                {
                    sb.AppendLine(tab + tab + mMysqlTabeSchemas[i].Field + comma);
                }
            }
            else
            {
                mSqliteTableSchemas = new SQLiteHelper(mConnection.connStr).GetTableSchema<SqliteTableSchema>(tableName);
                for (int i = 0; i < mSqliteTableSchemas.Count; i++)
                {
                    sb.AppendLine(tab + tab + mSqliteTableSchemas[i].Name + comma);
                }
            }
            sb.AppendLine(tab + "}");          
            return sb.ToString();
        }

        private void getDbSchema()
        {
            if (mConnection.type == DbType.mysql.ToString())
            {
                mDbSchemas = MySqlHelperInstance.GetAllTableSchema<DbSchema>(mConnection.dbName);
            }
            else if (mConnection.type == DbType.sqlite.ToString())
            {
                mDbSchemas = SQLiteHelperInstance.GetAllTableSchema<DbSchema>();
            }
            else {
                throw new Exception("不支持的数据库类型：" +mConnection.type.ToString());
            }
        }

        private void GetTableSchema()
        {
            if (mConnection.type == DbType.mysql.ToString())
            {
                mDbSchemas = new MySqlHelper(mConnection.connStr).GetAllTableSchema<DbSchema>(mConnection.dbName);
            }
            else
            {
                mDbSchemas = new SQLiteHelper(mConnection.connStr).GetAllTableSchema<DbSchema>();
            }
        }
        public string getcomment(string comment)
        {           
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(tab+"/// <summary>");
            sb.AppendLine(tab + "///" + comment);
            sb.AppendLine(tab + "/// </summary>");
            return sb.ToString();
        }
    }
}
