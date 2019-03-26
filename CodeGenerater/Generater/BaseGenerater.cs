using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerater
{
   public class BaseGenerater
    {
        public Connection mConnection;
        public LocalSchema mDbSchema;
        public string mNameSpace;
        public string tab = "\t ";
        public string comma = ",";     
        public string ClassNmae; 
        public string mTableName;
    }
}
