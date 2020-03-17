using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlDao;
namespace CodeGenerater
{
    public class SqlGenerater
    {
        private string connstr;
        private string mDbType;
        private string mTableName;

        public SqlGenerater(string tableName, string conn = null, string dbtype = null)
        {
            this.connstr = conn;
            this.mDbType = dbtype;
            this.mTableName = tableName;
        }

        public string GeneraterSql()
        {
            string res = string.Empty;
            if (mDbType == DbType.mysql.ToString())
            {
                res = new MySqlHelper(connstr).GetCreateSql(mTableName);
            }
            else
            {
                res = new SQLiteHelper(connstr).GetCreateSql(mTableName);
            }
            return res;
        }

    }
}
