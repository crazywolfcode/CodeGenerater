using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerater
{
    public class FileHelper
    {
        /// <summary>
        /// 
        /// 判定相应位置下的文件是否存在
        /// </summary>
        /// <param name="path"> 路径</param>
        /// <returns></returns>
        public static bool Exists(string path)
        {
            return File.Exists(path);
        }
        /// <summary>
        /// 判定相应位置下的文件夹是否存在
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool FolderExists(string path)
        {
            return Directory.Exists(path);
        }

        /// <summary>
        /// 判定相应位置下的文件夹是否存在
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool FolderExistsCreater(string path)
        {
            if (string.IsNullOrEmpty(path))
            { return false; }
            if (Directory.Exists(path) == true)
            {
                return true;
            }
            else
            {
                DirectoryInfo dir = new DirectoryInfo(path);
                try
                {
                    dir.Create();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// 写文件
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="content">内容</param>
        public static void Write(string path, string content)
        {
            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                fs.SetLength(0);
                using (StreamWriter writer = new StreamWriter(fs))
                {
                    writer.BaseStream.Seek(0, SeekOrigin.Begin);
                    writer.Write(content);
                    writer.Flush();
                }
            }
        }

        /// <summary>
        /// 写文件
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="content">内容</param>
        public static void WriteAppend(string path, string content)
        {
            using (FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write))
            {
                fs.Position = fs.Length;
                using (StreamWriter writer = new StreamWriter(fs))
                {
                    writer.BaseStream.Seek(0, SeekOrigin.End);
                    writer.Write(content);
                    writer.Flush();
                }
            }
        }
        /// <summary>
        /// 读文件 Encording gb2312
        /// </summary>
        /// <param name="path"> 路径</param>
        /// <returns> 文件内容</returns>
        public static string Reader(string path, Encoding encoding = null)
        {
            if (encoding == null)
            {
                encoding = Encoding.GetEncoding("gb2312");
            }

            if (File.Exists(path) == false)
            {
                return null;
            }
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                using (StreamReader sr = new StreamReader(fs, encoding))
                {
                    return sr.ReadToEnd();
                }
            }
        }
        /// <summary>
        /// 读取文件
        /// </summary>
        /// <param name="filePath">路径</param>
        /// <param name="encoding"> default gb2312</param>
        /// <returns></returns>
        public static List<string> ReadFileLines(string filePath, Encoding encoding = null)
        {
            var str = new List<string>();
            if (encoding == null)
            {
                encoding = Encoding.GetEncoding("gb2312");
            }
            using (var sr = new StreamReader(filePath, encoding))
            {
                String input;
                while ((input = sr.ReadLine()) != null)
                {
                    str.Add(input);
                }
            }
            return str;
        }
        /// <summary>
        /// 创建文件
        /// </summary>
        /// <param name="filename">路径包含文件名</param>
        /// <returns>true or false</returns>
        public static bool CreateFile(string filename)
        {
            if (File.Exists(filename) == false)
            {
                try
                {
                    FileStream fs = File.Create(filename);
                    fs.Dispose();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 获取项目的根路径
        /// </summary>
        /// <returns>根路径</returns>
        public static string GetProjectRootPath()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory;
            string rootpath = path.Substring(0, path.LastIndexOf("bin"));
            return rootpath;
        }
        /// <summary>
        /// 获取项目运行时的根路径
        /// </summary>
        /// <returns></returns>
        public static string GetRunTimeRootPath()
        {
            return AppDomain.CurrentDomain.BaseDirectory.ToString();
        }
        /// <summary>
        /// image file to base64
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string GetFileBase64(string filePath)
        {
            try
            {
                Bitmap bitmap = new Bitmap(filePath);
                MemoryStream ms = new MemoryStream();
                bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                byte[] arr = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(arr, 0, (int)ms.Length);
                ms.Close();
                String strbaser64 = Convert.ToBase64String(arr);
                return strbaser64;
            }
            catch (Exception)
            {
                return null;
            }

        }

        /// <summary>
        /// 将图片数据转换为Base64字符串
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static byte[] GetBytes(string imagePath)
        {
            System.Drawing.Image img = System.Drawing.Image.FromFile(imagePath);
            System.IO.MemoryStream mstream = new System.IO.MemoryStream();
            img.Save(mstream, System.Drawing.Imaging.ImageFormat.Jpeg);
            byte[] byData = new Byte[mstream.Length];
            mstream.Position = 0;
            mstream.Read(byData, 0, byData.Length);
            mstream.Close();
            return byData;
        }
        /// <summary>
        /// base64编码的文本 转为    图片
        /// </summary>
        /// <param name="base64"></param>
        /// <returns></returns>
        private Bitmap Base64StringToImage(string base64)
        {
            try
            {
                byte[] arr = Convert.FromBase64String(base64);
                MemoryStream ms = new MemoryStream(arr);
                Bitmap bmp = new Bitmap(ms);
                ms.Close();
                return bmp;
            }
            catch
            {
                return null;
            }
        }
        /// <summary>  
        ///     复制文件夹（及文件夹下所有子文件夹和文件）  
        /// </summary>  
        /// <param name="sourcePath">待复制的文件夹路径</param>  
        /// <param name="destinationPath">目标路径</param>  
        public static void CopyDirectory(String sourcePath, String destinationPath)
        {
            var info = new DirectoryInfo(sourcePath);
            Directory.CreateDirectory(destinationPath);
            foreach (FileSystemInfo fsi in info.GetFileSystemInfos())
            {
                String destName = Path.Combine(destinationPath, fsi.Name);

                if (fsi is FileInfo) //如果是文件，复制文件  
                    File.Copy(fsi.FullName, destName);
                else //如果是文件夹，新建文件夹，递归  
                {
                    Directory.CreateDirectory(destName);
                    CopyDirectory(fsi.FullName, destName);
                }
            }
        }

        /// <summary>  
        ///     删除文件夹（及文件夹下所有子文件夹和文件）  
        /// </summary>  
        /// <param name="directoryPath"></param>  
        public static void DeleteFolder(string directoryPath)
        {
            foreach (string d in Directory.GetFileSystemEntries(directoryPath))
            {
                if (File.Exists(d))
                {
                    var fi = new FileInfo(d);
                    if (fi.Attributes.ToString().IndexOf("ReadOnly", StringComparison.Ordinal) != -1)
                        fi.Attributes = FileAttributes.Normal;
                    File.Delete(d); //删除文件     
                }
                else
                    DeleteFolder(d); //删除文件夹  
            }
            Directory.Delete(directoryPath); //删除空文件夹  
        }

        /// <summary>  
        ///     清空文件夹（及文件夹下所有子文件夹和文件）  
        /// </summary>  
        /// <param name="directoryPath"></param>  
        public static void ClearFolder(string directoryPath)
        {
            foreach (string d in Directory.GetFileSystemEntries(directoryPath))
            {
                if (File.Exists(d))
                {
                    var fi = new FileInfo(d);
                    if (fi.Attributes.ToString().IndexOf("ReadOnly", StringComparison.Ordinal) != -1)
                        fi.Attributes = FileAttributes.Normal;
                    File.Delete(d); //删除文件     
                }
                else
                    DeleteFolder(d); //删除文件夹  
            }
        }

        /// <summary>  
        ///     取得文件大小，按适当单位转换  
        /// </summary>  
        /// <param name="filepath"></param>  
        /// <returns></returns>  
        public static string GetFileSize(string filepath)
        {
            string result = "0KB";
            if (File.Exists(filepath))
            {
                var size = new FileInfo(filepath).Length;
                int filelength = size.ToString().Length;
                if (filelength < 4)
                    result = size + "byte";
                else if (filelength < 7)
                    result = Math.Round(Convert.ToDouble(size / 1024d), 2) + "KB";
                else if (filelength < 10)
                    result = Math.Round(Convert.ToDouble(size / 1024d / 1024), 2) + "MB";
                else if (filelength < 13)
                    result = Math.Round(Convert.ToDouble(size / 1024d / 1024 / 1024), 2) + "GB";
                else
                    result = Math.Round(Convert.ToDouble(size / 1024d / 1024 / 1024 / 1024), 2) + "TB";
                return result;
            }
            return result;
        }

        /// <summary>  
        ///     取得文件大小，按适当单位转换  
        /// </summary>  
        /// <param name="size">file length</param>  
        /// <returns></returns>  
        public static string ConverterFileSizeUnit(long size)
        {
            string result = "0KB";
            if (size > 0)
            {
                int filelength = size.ToString().Length;
                if (filelength < 4)
                    result = size + "byte";
                else if (filelength < 7)
                    result = Math.Round(Convert.ToDouble(size / 1024d), 2) + "KB";
                else if (filelength < 10)
                    result = Math.Round(Convert.ToDouble(size / 1024d / 1024), 2) + "MB";
                else if (filelength < 13)
                    result = Math.Round(Convert.ToDouble(size / 1024d / 1024 / 1024), 2) + "GB";
                else
                    result = Math.Round(Convert.ToDouble(size / 1024d / 1024 / 1024 / 1024), 2) + "TB";
                return result;
            }
            return result;
        }
    }
}
