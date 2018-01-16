using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                res = new MyHelper.MySqlHelper(connstr).getCreateSql(mTableName);
            }
            else
            {
                res = new MyHelper.SQLiteHelper(connstr).getCreateSql(mTableName);
            }
            return res;
        }

    }
}
