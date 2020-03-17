using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlDao;
namespace CodeGenerater
{
    public class DatabaseOPtionHelper
    {

        private static DbHelper Instance;


        public static DbHelper GetInstance(string conn)
        {         
            Instance = new SqlDao.MySqlHelper(conn);
            App.Current.Dispatcher.BeginInvoke(new Action(delegate
            {
                if (Instance == null)
                {
                    Instance = new SqlDao.MySqlHelper(conn);
                }
            }));
            return Instance;
        }

    }
}
