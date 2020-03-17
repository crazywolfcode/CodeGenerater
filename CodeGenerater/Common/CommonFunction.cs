using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Windows;
using System.Windows.Markup;
using System.Threading.Tasks;

namespace CodeGenerater
{
    class CommonFunction
    {
        public static List<Connection> getListConn()
        {
            List<Connection> mConnections = null;
            string path = Constract.ConnPath;
            string filePath = Constract.ConnFilePath;
            if (FileHelper.Exists(filePath))
            {
                mConnections = (List<Connection>)XmlHelper.Deserialize(typeof(List<Connection>), FileHelper.Reader(filePath, Encoding.UTF8));
            }
            else
            {
                mConnections = new List<Connection>();
            }
            return mConnections;
        }
        public static FrameworkElement getFrameworkElementFromXaml(string path)
        {
            XmlTextReader reader = new XmlTextReader(path);
            FrameworkElement element = XamlReader.Load(reader) as FrameworkElement;
            return element;
        }
    }
}
