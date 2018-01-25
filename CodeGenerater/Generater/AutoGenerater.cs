using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeGenerater
{
    public class AutoGenerater
    {
        private Connection mConnection;
        public delegate void showMsg(string msg);
        private List<MyHelper.DbSchema> mHostoryDbSchemas = null;
        private List<MyHelper.DbSchema> mNowSchemas = null;
        private List<MyHelper.DbSchema> needThreadGeneraters = new List<MyHelper.DbSchema>();
        private showMsg DelegateShowMsg;
        public AutoGenerater(Connection conn)
        {
            mConnection = conn;
        }

        public void showmessage(string msg)
        {
            MyHelper.ConsoleHelper.writeLine(msg);
        }

        public void generater()
        {
            DelegateShowMsg = new showMsg(showmessage);
            getNowDbSchemas();
            if (mNowSchemas == null || mNowSchemas.Count <= 0)
            {
                return;
            }
            getHostoryDbSchemas();
            if (mHostoryDbSchemas != null && mHostoryDbSchemas.Count > 0)
            {
                //except identical
                mHostoryDbSchemas.Distinct();
                mNowSchemas.Distinct();
                exceptIdentical();
            }
            else
            {
                needThreadGeneraters = mNowSchemas;
            }
            generaterCode(needThreadGeneraters);
            // save the new to History ;
            saveDbSchemasToFile();
        }

        private void generaterCode(List<MyHelper.DbSchema> list)
        {
            if (list == null || list.Count <= 0) { return; }
            string fileName = string.Empty;
            for (int i = 0; i < needThreadGeneraters.Count; i++)
            {
                //java class              
                MyHelper.DbSchema schema = needThreadGeneraters[i];
                if (!string.IsNullOrEmpty(mConnection.javaClassPath))
                {
                    if (MyHelper.FileHelper.FolderExistsCreater(mConnection.javaClassPath))
                    {
                        string javaClass = new JavaGenerater(schema, mConnection).CeneraterClass();
                        fileName = "\\" + MyHelper.StringHelper.upperCaseFirstLetter(MyHelper.StringHelper.DBNamingToCamelCase(schema.TableName)) + ".java";
                        MyHelper.FileHelper.Write(mConnection.javaClassPath + fileName, javaClass);
                    }
                }

                //java enum
                if (!string.IsNullOrEmpty(mConnection.javaEnumPath))
                {
                    if (MyHelper.FileHelper.FolderExistsCreater(mConnection.javaEnumPath))
                    {
                        string javaEnum = new JavaEnumGenerare(mConnection).tableEnumGenerater(schema);
                        fileName = "\\" + MyHelper.StringHelper.upperCaseFirstLetter(MyHelper.StringHelper.DBNamingToCamelCase(schema.TableName)) + ".java";
                        MyHelper.FileHelper.Write(mConnection.javaEnumPath + fileName, javaEnum);
                    }
                }

                //Csharp class
                if (!string.IsNullOrEmpty(mConnection.cSharpClassPath))
                {
                    if (MyHelper.FileHelper.FolderExistsCreater(mConnection.cSharpClassPath))
                    {
                        string csharpClass = new CSharpCenerater(schema, mConnection).CeneraterClass();
                        fileName = "\\" + MyHelper.StringHelper.upperCaseFirstLetter(MyHelper.StringHelper.DBNamingToCamelCase(schema.TableName)) + ".cs";
                        MyHelper.FileHelper.Write(mConnection.cSharpClassPath + fileName, csharpClass);
                    }
                }

                //Csharp enum
                if (!string.IsNullOrEmpty(mConnection.cSharpEnumPath))
                {
                    if (MyHelper.FileHelper.FolderExistsCreater(mConnection.cSharpEnumPath))
                    {
                        string csharpEnum = new CsharpEnumGenerare(mConnection).tableEnumGenerater(schema);
                        fileName = "\\" + MyHelper.StringHelper.upperCaseFirstLetter(MyHelper.StringHelper.DBNamingToCamelCase(schema.TableName)) + ".cs";
                        MyHelper.FileHelper.Write(mConnection.cSharpEnumPath + fileName, csharpEnum);
                    }
                }
                //Csharp all enum
                if (!string.IsNullOrEmpty(mConnection.cSharpEnumAllPath))
                {
                    if (MyHelper.FileHelper.FolderExistsCreater(mConnection.cSharpEnumPath))
                    {
                        string csharpEnum = new CsharpEnumGenerare(mConnection).dbEnumGenerater();
                        fileName = "\\" + MyHelper.StringHelper.upperCaseFirstLetter(MyHelper.StringHelper.DBNamingToCamelCase(mConnection.dbName)) + ".cs";
                        MyHelper.FileHelper.Write(mConnection.cSharpEnumAllPath + fileName, csharpEnum);
                    }
                }
                //crete sql 
                if (!string.IsNullOrEmpty(mConnection.sqlPath))
                {
                    if (MyHelper.FileHelper.FolderExistsCreater(mConnection.sqlPath))
                    {
                        string createSQl = new CsharpEnumGenerare(mConnection).tableEnumGenerater(schema);
                        fileName = "\\" + schema.TableName + ".sql";
                        MyHelper.FileHelper.Write(mConnection.sqlPath + fileName, createSQl);
                    }
                }
            }
        }

        private void exceptIdentical()
        {
            for (int i = 0; i < mNowSchemas.Count; i++)
            {
                MyHelper.DbSchema schema = mNowSchemas[i];
                bool isContains = false;
                for (int j = 0; j < mHostoryDbSchemas.Count; j++)
                {
                    MyHelper.DbSchema historySchema = mHostoryDbSchemas[j];
                    if (schema.TableName == historySchema.TableName)
                    {
                        isContains = true;
                        if (schema.updateTime == null)
                        {
                            needThreadGeneraters.Add(schema);
                        }
                        else if (historySchema.updateTime != schema.updateTime)
                        {
                            needThreadGeneraters.Add(schema);
                        }
                    }
                }

                if (isContains == false)
                {
                    needThreadGeneraters.Add(schema);
                }
            }
        }

        private void getNowDbSchemas()
        {
            if (mConnection != null)
            {
                if (mConnection.type == DbType.mysql.ToString())
                {
                    mNowSchemas = new MyHelper.MySqlHelper(mConnection.connStr).getAllTableSchema(mConnection.dbName);
                }
                else if (mConnection.type == DbType.sqlite.ToString())
                {
                    mNowSchemas = new MyHelper.SQLiteHelper(mConnection.connStr).getAllTableSchema();
                }
            }
        }
        private void getHostoryDbSchemas()
        {
            if (MyHelper.FileHelper.FolderExistsCreater(Constract.dBschemasPath))
            {
                if (!MyHelper.FileHelper.Exists(Constract.getDbdBschemasPath(mConnection.id)))
                {
                    MyHelper.FileHelper.createFile(Constract.getDbdBschemasPath(mConnection.id));
                    return;
                }
                else
                {
                    string xml = MyHelper.FileHelper.Reader(Constract.getDbdBschemasPath(mConnection.id), Encoding.UTF8);
                    if (string.IsNullOrEmpty(xml))
                    {
                        return;
                    }
                    try
                    {
                        mHostoryDbSchemas = (List<MyHelper.DbSchema>)MyHelper.XmlHelper.Deserialize(typeof(List<MyHelper.DbSchema>), xml);
                    }
                    catch (Exception)
                    {
                        DelegateShowMsg("Xml  文件中有错误！");
                        return;
                    }
                }
            }
            else
            {
                mHostoryDbSchemas = null;
            }
        }
        private void saveDbSchemasToFile()
        {
            if (mNowSchemas != null && mNowSchemas.Count > 0)
            {
                if (MyHelper.FileHelper.FolderExistsCreater(Constract.dBschemasPath))
                {
                    if (!MyHelper.FileHelper.Exists(Constract.getDbdBschemasPath(mConnection.id)))
                    {
                        if (!MyHelper.FileHelper.createFile(Constract.getDbdBschemasPath(mConnection.id)))
                        {
                            DelegateShowMsg("创建文件失败：" + Constract.getDbdBschemasPath(mConnection.id));
                        }
                    }
                    string xml = MyHelper.XmlHelper.Serialize(typeof(List<MyHelper.DbSchema>), mNowSchemas);
                    try
                    {
                        MyHelper.FileHelper.Write(Constract.getDbdBschemasPath(mConnection.id), xml);
                    }
                    catch (Exception)
                    {
                        DelegateShowMsg("写入文件失败");
                    }
                }
                else
                {
                    DelegateShowMsg("创建文件夹失败：" + Constract.dBschemasPath);
                }

            }
        }
    }
}
