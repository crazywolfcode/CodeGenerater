using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlDao;
namespace CodeGenerater
{
   public class BaseGenerater
    {
        public Connection mConnection;
        public LocalSchema mDbLocalSchema;
        public string mNameSpace;
        public string tab = "\t ";
        public string comma = ",";     
        public string ClassNmae; 
        public string mTableName;

        protected string GetClassComment(String des = "")
        {
            StringBuilder sb = new StringBuilder();
            if (mDbLocalSchema == null)
            {
                return null;
            }
            sb.AppendLine("/**");
            if (!string.IsNullOrWhiteSpace(mDbLocalSchema.TableComment))
            {
                if (des == "")
                {
                    sb.AppendLine("* " + mDbLocalSchema.TableComment);
                }
                else
                {
                    sb.AppendLine("* " + mDbLocalSchema.TableComment + " " + des);
                }
            }
            sb.AppendLine("* @Email: 443055589@qq.com");
            sb.AppendLine("* @Author: 软友科技 Code Generater");
            sb.AppendLine("*/ ");
            return sb.ToString();
        }

        protected string GetCustomerClassComment(String des = "")
        {
            StringBuilder sb = new StringBuilder();           
            sb.AppendLine("/**");
            sb.AppendLine($"* @description:{ des}");
            sb.AppendLine("* @email: 443055589@qq.com");
            sb.AppendLine("* @author: 软友科技 Code Generater");
            sb.AppendLine("*/ ");
            return sb.ToString();
        }

        private MySqlHelper mySqlHelper;
        public MySqlHelper MySqlHelperInstance { 
            get {
                if (mySqlHelper == null) {
                    mySqlHelper = new MySqlHelper(mConnection.connStr);
                }
                return mySqlHelper;
            }            
        }

        private SQLiteHelper sqliteHelper;
        public SQLiteHelper SQLiteHelperInstance
        {
            get
            {
                if (sqliteHelper == null)
                {
                    sqliteHelper = new SQLiteHelper(mConnection.connStr);
                }
                return sqliteHelper;
            }
        }
    }

   
}
