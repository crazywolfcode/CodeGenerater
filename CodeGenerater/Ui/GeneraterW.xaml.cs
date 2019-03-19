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
    public partial class GeneraterW : Window
    {
        private MyHelper.DbSchema currDbschema;
        private Connection mConnection;
        private string[] mTables = new string[] { };
        private List<MyHelper.DbSchema> mDbSchemas;
        private string sufix;
        public GeneraterW(Connection conn)
        {
            InitializeComponent();
            this.mConnection = conn;
            new WindowBehavior(this).RepairWindowDefaultBehavior();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            setScrollViewHeight();
            setCodeScrollViewHeight();
            setCurrentConnInfo();
            // logical start
            getDBschemas();
            setTableNameList();
        }
        private void setCurrentConnInfo()
        {
            if (mConnection != null)
            {
                if (mConnection.name != null)
                {
                    this.connNameTb.Text = mConnection.name;
                }
                if (mConnection.type != null)
                {
                    this.conntypeTb.Text = mConnection.type;
                }
                if (mConnection.description != null)
                {
                    this.topHeader.ToolTip = mConnection.description;
                }
            }
        }

        #region logical code

        private void getDBschemas()
        {
            if (mConnection.type == DbType.mysql.ToString())
            {
                mDbSchemas = new MyHelper.MySqlHelper(mConnection.connStr).getAllTableSchema(mConnection.dbName);
            }
            else
            {
                mDbSchemas = new MyHelper.SQLiteHelper(mConnection.connStr).getAllTableSchema();
            }

        }

        private void getTablelist()
        {
            if (mConnection.type == DbType.mysql.ToString())
            {
                mTables = new MyHelper.MySqlHelper(mConnection.connStr).getAllTableName(mConnection.dbName);
            }
            else if (mConnection.type == DbType.sqlite.ToString())
            {
                mTables = new MyHelper.SQLiteHelper(mConnection.connStr).getAllTableName();
            }
            else
            {
                MessageBox.Show("未知的数据库类型！");
            }


        }

        public void setTableNameList()
        {
            if (mDbSchemas.Count > 0)
            {
                this.tablePanel.Children.Clear();
                for (int i = 0; i < mDbSchemas.Count; i++)
                {
                    this.tablePanel.Children.Add(getRadioButton(mDbSchemas[i]));
                }
            }
            else
            {
                MessageBox.Show("没有获取任何数据表！");
            }

        }

        public RadioButton getRadioButton(MyHelper.DbSchema schema)
        {
            RadioButton tb = new RadioButton();
            tb.Style = App.Current.Resources["menuRadioButtonStyle"] as Style;
            tb.Height = 28;
            tb.Width = this.tablePanel.ActualWidth;
            Foreground = App.Current.Resources["69"] as Brush;
            tb.Tag = schema;
            tb.Content = schema.TableName;
            if (!string.IsNullOrEmpty(schema.TableComment))
            {
                tb.ToolTip = schema.TableComment;
            }
            tb.Margin = new Thickness(0, 10, 0, 0);
            tb.Click += TabButton_Click;
            return tb;
        }

        private void TabButton_Click(object sender, RoutedEventArgs e)
        {
            RadioButton tabbtn = sender as RadioButton;
            currDbschema = (MyHelper.DbSchema)tabbtn.Tag;

            startCeneraterCode();
        }
        #endregion

        #region start Cenerater Code
        private void startCeneraterCode()
        {
            string doc = string.Empty;
            if (javaRb.IsChecked == true)
            {
                JavaGenerater jg = new JavaGenerater(currDbschema, mConnection);
                doc = jg.CeneraterClass();
            }
            else if (csharpRb.IsChecked == true)
            {
                CSharpCenerater generater = new CSharpCenerater(currDbschema, mConnection);
                doc = generater.CeneraterClass();
            }
            else if (javaEnumRb.IsChecked == true)
            {
                JavaEnumGenerare generater = new JavaEnumGenerare(mConnection);
                doc = generater.tableEnumGenerater(currDbschema);
            }
            else if (csharpEnumRb.IsChecked == true)
            {
                CsharpEnumGenerare generater = new CsharpEnumGenerare(mConnection);
                doc = generater.tableEnumGenerater(currDbschema);
            }
            else if (createSqlb.IsChecked == true)
            {
                SqlGenerater generater = new SqlGenerater(currDbschema.TableName, mConnection.connStr, mConnection.type);
                doc = generater.GeneraterSql();
            }
            else
            {
                doc = "\r\n 请告诉我你需要什么代码？\r\n ";
            }
            this.CodeTb.Text = doc;
        }
        #endregion

        #region max min close mover
        private void WindowTitle_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
        private void ConfigItemPanel_MouseMove(object sender, MouseEventArgs e)
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

        private void MaxBtn_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
            }
            else
            {
                this.WindowState = WindowState.Maximized;
            }
        }

        private void MinBtn_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        #endregion

        /// <summary>
        /// save code to file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {

            string doc = this.CodeTb.Text;
            SaveFileDialog sfd = new SaveFileDialog();
            if (sufix != ".sql")
            {
                sfd.FileName = MyHelper.StringHelper.upperCaseFirstLetter(MyHelper.StringHelper.DBNamingToCamelCase(currDbschema.TableName));
            }
            else
            {
                sfd.FileName = currDbschema.TableName;
            }
            sfd.DefaultExt = sufix;

            if (sfd.ShowDialog() == true)
            {
                try
                {
                    MyHelper.FileHelper.Write(sfd.FileName, doc);
                    MessageBox.Show("保存成功！");
                }
                catch (Exception exc)
                {
                    MessageBox.Show("保存失败!原因：" + exc.Message);
                }

            }
        }

        #region set scrollviewer height
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.setScrollViewHeight();
            this.setCodeScrollViewHeight();
        }
        private void setScrollViewHeight()
        {
            this.TableSv.Height = this.ConfigItemPanel.ActualHeight - this.topHeader.ActualHeight;

        }
        private void setCodeScrollViewHeight()
        {
            this.CodeScrollViewer.Height = this.codeMainPanel.ActualHeight;

        }
        #endregion

        #region Code tyoe Change

        private void javaRb_Checked(object sender, RoutedEventArgs e)
        {
            sufix = ".java";
            changed();
        }

        private void csharpRb_Checked(object sender, RoutedEventArgs e)
        {
            sufix = ".cs";
            changed();
        }
        private void javaEnumRb_Checked(object sender, RoutedEventArgs e)
        {
            sufix = ".java";
            changed();
        }

        private void csharpEnumRb_Checked(object sender, RoutedEventArgs e)
        {
            sufix = ".cs";
            changed();
        }
        private void createSqlb_Checked(object sender, RoutedEventArgs e)
        {
            sufix = ".sql";
            changed();
        }
        private void changed()
        {
            if (currDbschema != null)
            {
                this.startCeneraterCode();
            }
        }
        #endregion

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void DataMoveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (currDbschema != null)
            {
                new DataMoveDesW(mConnection, currDbschema, this.CodeTb.Text).ShowDialog();
            }            
        }
    }
}
