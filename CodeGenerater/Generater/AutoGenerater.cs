using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SqlDao;
namespace CodeGenerater
{
    public class AutoGenerater
    {
        private Connection mConnection;
        public delegate void showMsg(string msg);
        private List<LocalSchema> mHostoryDbSchemas = null;
        private List<LocalSchema> mNowSchemas = new List<LocalSchema>();
        private List<LocalSchema> needThreadGeneraters = new List<LocalSchema>();
        private showMsg DelegateShowMsg;
        public AutoGenerater(Connection conn)
        {
            mConnection = conn;
        }

        public void showmessage(string msg)
        {
            Console.WriteLine(msg);
        }

        public void generater()
        {
            DelegateShowMsg = new showMsg(showmessage);
            GetNowDbSchemas();
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

        private void generaterCode(List<LocalSchema> list)
        {
            if (list == null || list.Count <= 0) { return; }
            string fileName = string.Empty;
            for (int i = 0; i < needThreadGeneraters.Count; i++)
            {
                //java class              
                LocalSchema schema = needThreadGeneraters[i];
                if (!string.IsNullOrEmpty(mConnection.javaClassPath))
                {
                    if (FileHelper.FolderExistsCreater(mConnection.javaClassPath))
                    {
                        string javaClass = new JavaGenerater(schema, mConnection).CeneraterClass();
                        String suffix = string.Empty;
                        if (!String.IsNullOrEmpty(mConnection.classSuffix)) {
                            suffix = StringHelper.upperCaseFirstLetter(mConnection.classSuffix);
                        }
                        fileName = "\\" + StringHelper.upperCaseFirstLetter(StringHelper.DBNamingToCamelCase(schema.TableName)) +suffix+ ".java";
                        FileHelper.Write(mConnection.javaClassPath + fileName, javaClass);
                    }
                }

                //java enum
                if (!string.IsNullOrEmpty(mConnection.javaEnumPath))
                {
                    if (FileHelper.FolderExistsCreater(mConnection.javaEnumPath))
                    {
                        string javaEnum = new JavaEnumGenerare(mConnection).tableEnumGenerater(schema);
                        String suffix = string.Empty;
                        if (!String.IsNullOrEmpty(mConnection.enumSuffi))
                        {
                            suffix = StringHelper.upperCaseFirstLetter(mConnection.enumSuffi);
                        }
                        fileName = "\\" + StringHelper.upperCaseFirstLetter(StringHelper.DBNamingToCamelCase(schema.TableName)) + suffix + ".java";
                        FileHelper.Write(mConnection.javaEnumPath + fileName, javaEnum);
                    }
                }

                //Csharp class
                if (!string.IsNullOrEmpty(mConnection.cSharpClassPath))
                {
                    if (FileHelper.FolderExistsCreater(mConnection.cSharpClassPath))
                    {
                        string csharpClass = new CSharpCenerater(schema, mConnection).CeneraterClass();
                        String suffix = string.Empty;
                        if (!String.IsNullOrEmpty(mConnection.classSuffix))
                        {
                            suffix = StringHelper.upperCaseFirstLetter(mConnection.classSuffix);
                        }
                        fileName = "\\" + StringHelper.upperCaseFirstLetter(StringHelper.DBNamingToCamelCase(schema.TableName)) + suffix + ".cs";
                        FileHelper.Write(mConnection.cSharpClassPath + fileName, csharpClass);
                    }
                }

                //Csharp enum
                if (!string.IsNullOrEmpty(mConnection.cSharpEnumPath))
                {
                    if (FileHelper.FolderExistsCreater(mConnection.cSharpEnumPath))
                    {
                        string csharpEnum = new CsharpEnumGenerare(mConnection).tableEnumGenerater(schema);
                        String suffix = string.Empty;
                        if (!String.IsNullOrEmpty(mConnection.enumSuffi))
                        {
                            suffix = StringHelper.upperCaseFirstLetter(mConnection.enumSuffi);
                        }
                        fileName = "\\" + StringHelper.upperCaseFirstLetter(StringHelper.DBNamingToCamelCase(schema.TableName)) + suffix + ".cs";                       
                        FileHelper.Write(mConnection.cSharpEnumPath + fileName, csharpEnum);
                    }
                }
                //Csharp all enum
                if (!string.IsNullOrEmpty(mConnection.cSharpEnumAllPath))
                {
                    if (FileHelper.FolderExistsCreater(mConnection.cSharpEnumAllPath))
                    {
                        string csharpEnum = new CsharpEnumGenerare(mConnection).dbEnumGenerater();
                        fileName = "\\" + StringHelper.upperCaseFirstLetter(StringHelper.DBNamingToCamelCase(mConnection.dbName)) + ".cs";
                        FileHelper.Write(mConnection.cSharpEnumAllPath + fileName, csharpEnum);
                    }
                }
                //crete sql 
                if (!string.IsNullOrEmpty(mConnection.sqlPath))
                {
                    if (FileHelper.FolderExistsCreater(mConnection.sqlPath))
                    {
                        string createSQl = new CsharpEnumGenerare(mConnection).tableEnumGenerater(schema);
                        fileName = "\\" + schema.TableName + ".sql";
                        FileHelper.Write(mConnection.sqlPath + fileName, createSQl);
                    }
                }
            }
        }

        private void exceptIdentical()
        {
            for (int i = 0; i < mNowSchemas.Count; i++)
            {
                LocalSchema schema = mNowSchemas[i];
                bool isContains = false;
                for (int j = 0; j < mHostoryDbSchemas.Count; j++)
                {
                    LocalSchema historySchema = mHostoryDbSchemas[j];
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

        private void GetNowDbSchemas()
        {
            if (mConnection != null)
            {
                List<DbSchema> tempSchemas = new List<DbSchema>();
                if (mConnection.type == DbType.mysql.ToString())
                {
                   tempSchemas= DatabaseOPtionHelper.GetInstance(mConnection.connStr).GetAllTableSchema<DbSchema>(mConnection.dbName);
                }
                else if (mConnection.type == DbType.sqlite.ToString())
                {
                    tempSchemas = new SQLiteHelper(mConnection.connStr).GetAllTableSchema<DbSchema>();
                }
                foreach (DbSchema schema in tempSchemas)
                {
                    mNowSchemas.Add(new LocalSchema()
                    {
                        createTime = schema.CreateTime,
                        updateTime = schema.UpdateTime,
                        dataLength = schema.DataLength,
                        TableComment = schema.TableComment,
                        TableName = schema.TableName,
                        tableRows = schema.TableRows
                    });
                }
            }
        }
        private void getHostoryDbSchemas()
        {
            if (FileHelper.FolderExistsCreater(Constract.dBschemasPath))
            {
                if (!FileHelper.Exists(Constract.getDbdBschemasPath(mConnection.id)))
                {
                    FileHelper.CreateFile(Constract.getDbdBschemasPath(mConnection.id));
                    return;
                }
                else
                {
                    string xml = FileHelper.Reader(Constract.getDbdBschemasPath(mConnection.id), Encoding.UTF8);
                    if (string.IsNullOrEmpty(xml))
                    {
                        return;
                    }
                    try
                    {
                        mHostoryDbSchemas = (List<LocalSchema>)XmlHelper.Deserialize(typeof(List<LocalSchema>), xml);
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
                if (FileHelper.FolderExistsCreater(Constract.dBschemasPath))
                {
                    if (!FileHelper.Exists(Constract.getDbdBschemasPath(mConnection.id)))
                    {
                        if (!FileHelper.CreateFile(Constract.getDbdBschemasPath(mConnection.id)))
                        {
                            DelegateShowMsg("创建文件失败：" + Constract.getDbdBschemasPath(mConnection.id));
                        }
                    }
                    string xml = XmlHelper.Serialize(typeof(List<LocalSchema>), mNowSchemas);
                    try
                    {
                        FileHelper.Write(Constract.getDbdBschemasPath(mConnection.id), xml);
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
