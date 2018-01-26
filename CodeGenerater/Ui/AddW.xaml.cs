using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Win32;
namespace CodeGenerater
{
    /// <summary>
    /// SettingW.xaml 的交互逻辑
    /// </summary>
    public partial class AddW : Window
    {
        public AddW()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            new WindowBehavior(this).RepairWindowDefaultBehavior();

        }

        public void InitializingEvent()
        {

        }

        private void WindowTitle_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ConfigItemPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void updateBtn_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            MessageBox.Show(btn.Tag.ToString());
        }

        private void deleteBtn_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            MessageBox.Show(btn.Tag.ToString());
        }

        private void choseImgBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == true)
            {
                this.filePathTB.Text = dialog.FileName;
            }
        }

        private static string connStrTemplate = " Data Source={0};Version=3;Pooling=False;Max Pool Size=100;";
        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            Connection conn = new Connection();
            conn.nameSpace = "你的名称空间";
            conn.pakeage = "你的包名";
            string connName = string.Empty, connDes = string.Empty;
            if (mysqlBtn.IsChecked == true)
            {
                connName = this.mysql_connNameTB.Text.Trim();
                connDes = this.mysql_connDesTB.Text.Trim();
                if (string.IsNullOrEmpty(connName))
                {
                    MessageBox.Show("连接名称必需填写！");
                    this.sqlite_connNameTB.Focus();
                    return;
                }
                string dbname = this.dbNameTb.Text.Trim();
                string address = this.IpTb.Text.Trim();
                string userid = this.usernameTb.Text.Trim();
                string port = this.portTb.Text.Trim();
                string pwd = this.pwdTb.Text.Trim();
                string connstr = MyHelper.MySqlHelper.buildConnectionString(address, dbname, userid, pwd, port);
                if (MyHelper.MySqlHelper.CheckConn(connstr))
                {
                    conn.name = connName;
                    conn.description = connDes;
                    conn.type = DbType.mysql.ToString();
                    conn.ipAddress = address;
                    conn.dbName = dbname;
                    conn.port = port;
                    conn.uaerName = userid;
                    conn.password = pwd;
                    conn.connStr = connstr;
                    conn.addTime = MyHelper.DateTimeHelper.getCurrentDateTime();
                    //保存
                    if (saveToFile(conn))
                    {
                        MessageBox.Show("保存成功！");
                        this.Close();
                    }
                }
                else
                {
                    MessageBox.Show("连接不成功！无法保存！");
                    return;
                }
            }
            else if (sqliteBtn.IsChecked == true)
            {
                conn.type = DbType.sqlite.ToString();
                if (filePathTB.Text.Trim().Length <= 0)
                {
                    MessageBox.Show("数据库文件路径不正确！");
                    return;
                }
                conn.connStr = string.Format(connStrTemplate, filePathTB.Text.Trim());
                conn.description = sqlite_connDesTB.Text.Trim();
                conn.name = sqlite_connNameTB.Text.Trim();
                if (MyHelper.SQLiteHelper.CheckConn(conn.connStr))
                {
                    conn.addTime = MyHelper.DateTimeHelper.getCurrentDateTime();
                    conn.auto = Auto.no.ToString();
                  
                    //保存
                    if (saveToFile(conn))
                    {
                        MessageBox.Show("保存成功！");
                        this.Close();
                    }
                }
                else
                {
                    MessageBox.Show("连接不成功！无法保存！");
                    return;
                }
            }
            else
            {

                MessageBox.Show("数据库类型不对！");
            }
        }
        private bool saveToFile(Connection conn)
        {
            conn.id = Guid.NewGuid().ToString();
            List<Connection> conns;
            string path = Constract.ConnPath;
            string filePath = Constract.ConnFilePath;
            if (MyHelper.FileHelper.FolderExistsCreater(path))
            {
                if (!MyHelper.FileHelper.Exists(filePath))
                {
                    try
                    {
                        MyHelper.FileHelper.createFile(filePath);
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show("保存失败,文件创建失败：" + filePath+" msg:"+ex.Message);
                        return false;
                    }
                }
                string xml = MyHelper.FileHelper.Reader(filePath, Encoding.UTF8);
                conns = (List<Connection>)MyHelper.XmlHelper.Deserialize(typeof(List<Connection>), xml);
                if (conns == null)
                {
                    conns = new List<Connection>();
                    conns.Add(conn);
                }
                else
                {
                    conns.Add(conn);
                }
                string connStrings = MyHelper.XmlHelper.Serialize(typeof(List<Connection>), conns);

                try
                {
                    MyHelper.FileHelper.Write(filePath, connStrings);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("保存连接失败"+"msg:"+ex.Message);
                    return false;
                }
            }
            else
            {
                MessageBox.Show("保存失败！路径创建失败：" + path);
                return false;
            }
            return true;
        }

    }
}
