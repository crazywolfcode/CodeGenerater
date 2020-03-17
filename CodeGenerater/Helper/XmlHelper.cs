using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace CodeGenerater
{
    public class XmlHelper
    {
        /// <summary>
        /// 反序列化 Xml To Object
        /// </summary>
        /// <param name="type"> 类型</param>
        /// <param name="xml"> xml 字符串</param>
        /// <returns></returns>
        public static object Deserialize(Type type, string xml)
        {
            try
            {
                using (StringReader reader = new StringReader(xml))
                {
                    XmlSerializer serialize = new XmlSerializer(type);
                    return serialize.Deserialize(reader);
                }
            }
            catch (Exception )
            {
                return null;
            }
        }


        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="type"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static object Deserialize(Type type, Stream stream)
        {
            XmlSerializer xmldes = new XmlSerializer(type);
            return xmldes.Deserialize(stream);
        }

        /// <summary>
        /// 对像到XML
        /// </summary>
        /// <param name="type"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string Serialize(Type type, Object obj)
        {

            using (MemoryStream ms = new MemoryStream())
            {

                XmlSerializer xs = new XmlSerializer(type);
                try
                {
                    xs.Serialize(ms, obj);
                }
                catch (Exception )
                {                 
                    throw;
                }
                ms.Position = 0;
                StreamReader sr = new StreamReader(ms);
                string str = string.Empty;
                try
                {
                    str = sr.ReadToEnd();
                }
                catch (Exception )
                {                
                    throw;
                }
                finally
                {
                    sr.Close();
                    sr.Dispose();
                }
                return str;
            }
        }

        /// <summary>
        /// 更新XML文件中节点的值。如果节点不存在，请创建它，并保存到XML文件中。 
        /// update the value of the node in the xml file.if the node not  Exist then create it ,and save to xml file
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="nodeName">节点名称</param>
        /// <param name="value">值</param>        
        public static void UpdateNode(string filePath, string nodeName, string value)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(filePath);
            //XmlElement element = doc.GetElementById(nodeName);
            XmlNodeList list = doc.GetElementsByTagName(nodeName);
            if (list.Count <= 0)
            {
                XmlElement e = doc.CreateElement(nodeName);
                e.InnerText = value;
                doc.AppendChild(e);
            }
            else
            {
                list[0].InnerText = value;
            }
            doc.Save(filePath);
        }

        /// <summary>
        /// get the value of the node in the xml file.
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="nodeName"></param>
        /// <returns></returns>
        public static string getValueByNodeName(string filePath, string nodeName)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(filePath);
            XmlNodeList list = doc.GetElementsByTagName(nodeName);
            if (list.Count > 0)
            {
                return list[0].InnerText;
            }
            else
            {
                return null;
            }
        }
    }
}
