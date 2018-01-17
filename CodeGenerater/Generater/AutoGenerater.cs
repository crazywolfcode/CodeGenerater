using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerater
{
   public class AutoGenerater
    {
        private Connection mConnection;
        public delegate void showMsg(string msg);
        private List<MyHelper.DbSchema> mHostoryDbSchemas = null;
        private List<MyHelper.DbSchema> mNowSchemas = null;
        private List<MyHelper.DbSchema> temps = new List<MyHelper.DbSchema>();
        private showMsg DelegateShowMsg;
        public AutoGenerater(Connection conn, List<MyHelper.DbSchema> schemas) {
            mConnection = conn;
            mNowSchemas = schemas;
        }

    
        public void showmessage(string msg)
        {
            MyHelper.ConsoleHelper.writeLine(msg);
        }

        public void generater()
        {
           
            DelegateShowMsg = new showMsg(showmessage);
            getHostoryDbSchemas();
            if (mHostoryDbSchemas != null && mHostoryDbSchemas.Count > 0)
            {
                //marger
                mHostoryDbSchemas.Distinct();
                temps = mNowSchemas.Except(mHostoryDbSchemas).ToList();
            }
            else
            {
                temps = mNowSchemas;
            }
            generaterCode(temps);
        }

        private void generaterCode(List<MyHelper.DbSchema> list)
        {
            if (list == null || list.Count <= 0) { return; }
            string fileName = string.Empty;
            for (int i = 0; i < temps.Count; i++)
            {
                //java class              
                MyHelper.DbSchema schema = temps[i];
                if (mConnection.javaClassPath != null) {
                    if (MyHelper.FileHelper.FolderExistsCreater(mConnection.javaClassPath)) {
                        string javaClass = new JavaGenerater(schema.TableComment, schema.TableName, mConnection).CeneraterClass();
                        fileName = "\\" + MyHelper.StringHelper.upperCaseFirstLetter(MyHelper.StringHelper.DBNamingToCamelCase(schema.TableName)) + ".java";
                        MyHelper.FileHelper.Write(mConnection.javaClassPath + fileName, javaClass);
                    }
                }

                //java enum
                if (mConnection.javaEnumPath != null)
                {
                    if (MyHelper.FileHelper.FolderExistsCreater(mConnection.javaEnumPath))
                    {
                        string javaEnum = new JavaEnumGenerare(mConnection).tableEnumGenerater(schema.TableName,schema.TableComment);
                        fileName = "\\" + MyHelper.StringHelper.upperCaseFirstLetter(MyHelper.StringHelper.DBNamingToCamelCase(schema.TableName))+".java";
                        MyHelper.FileHelper.Write(mConnection.javaEnumPath +  fileName, javaEnum);
                    }
                }

                //Csharp class
                if (mConnection.cSharpClassPath != null)
                {
                    if (MyHelper.FileHelper.FolderExistsCreater(mConnection.cSharpClassPath))
                    {
                        string csharpClass = new CSharpCenerater(schema.TableComment,schema.TableName, mConnection).CeneraterClass();
                        fileName = "\\" + MyHelper.StringHelper.upperCaseFirstLetter(MyHelper.StringHelper.DBNamingToCamelCase(schema.TableName)) + ".cs";
                        MyHelper.FileHelper.Write(mConnection.cSharpClassPath + fileName, csharpClass);
                    }
                }

                //Csharp enum
                if (mConnection.cSharpEnumPath != null)
                {
                    if (MyHelper.FileHelper.FolderExistsCreater(mConnection.cSharpEnumPath))
                    {
                        string csharpEnum = new CsharpEnumGenerare(mConnection).tableEnumGenerater(schema.TableName,schema.TableComment);
                        fileName = "\\" + MyHelper.StringHelper.upperCaseFirstLetter(MyHelper.StringHelper.DBNamingToCamelCase(schema.TableName)) + ".cs";
                        MyHelper.FileHelper.Write(mConnection.cSharpEnumPath + fileName, csharpEnum);
                    }
                }

                //crete sql 
                if (mConnection.sqlPath != null)
                {
                    if (MyHelper.FileHelper.FolderExistsCreater(mConnection.sqlPath))
                    {
                        string createSQl = new CsharpEnumGenerare(mConnection).tableEnumGenerater(schema.TableName, schema.TableComment);
                        fileName = "\\" + schema.TableName+ ".sql";
                        MyHelper.FileHelper.Write(mConnection.sqlPath + fileName, createSQl);
                    }
                }
            }
        }
        private void getHostoryDbSchemas()
        {
            if (MyHelper.FileHelper.FolderExistsCreater(Constract.dBschemasPath))
            {
                if (!MyHelper.FileHelper.Exists(Constract.dBschemasFileName))
                {
                    MyHelper.FileHelper.createFile(Constract.dBschemasFileName);
                    return;
                }
                else
                {
                    string xml = MyHelper.FileHelper.Reader(Constract.dBschemasFileName, Encoding.UTF8);
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
                    if (!MyHelper.FileHelper.Exists(Constract.dBschemasFileName))
                    {
                        if (!MyHelper.FileHelper.createFile(Constract.dBschemasFileName))
                        {
                            DelegateShowMsg("创建文件失败：" + Constract.dBschemasFileName);
                        }
                    }
                    string xml = MyHelper.XmlHelper.Serialize(typeof(List<MyHelper.DbSchema>), mNowSchemas);
                    try
                    {
                        MyHelper.FileHelper.Write(Constract.dBschemasFileName, xml);
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
