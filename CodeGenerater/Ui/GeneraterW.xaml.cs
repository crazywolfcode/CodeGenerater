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
        private string currentTableName = string.Empty;
        private string currentCommend = string.Empty;
        private Connection mConnection;
        private string[] mTables = new string[] { };
        private List<MyHelper.DbSchema> mDbSchemas;
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
                mDbSchemas = new MyHelper.MySqlHelper(mConnection.connStr).getAllTableSchema();
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
                mTables = new MyHelper.MySqlHelper(mConnection.connStr).getAllTableName();
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
            Foreground = App.Current.Resources["69"] as Brush;
            tb.Tag = schema.TableName;
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
            currentTableName = tabbtn.Tag.ToString();
            if (tabbtn.ToolTip != null && !string.IsNullOrEmpty(tabbtn.ToolTip.ToString()))
            {
                currentCommend = tabbtn.ToolTip.ToString();
            }
            else
            {
                currentCommend = string.Empty;
            }
            startCeneraterCode(currentTableName, currentCommend);
        }
        #endregion

        #region start Cenerater Code
        private void startCeneraterCode(string tablename, string commend)
        {
            string doc = string.Empty;
            if (javaRb.IsChecked == true)
            {
                JavaGenerater jg = new JavaGenerater("com.main.txmy", commend, mConnection.dbName, tablename, mConnection.connStr, mConnection.type);
                doc = jg.CeneraterClass();
            }
            else if (csharpRb.IsChecked == true)
            {
                CSharpCenerater generater = new CSharpCenerater("codeGenerater", commend, mConnection.dbName, tablename, mConnection.connStr, mConnection.type);
                doc = generater.CeneraterClass();
            }
            else if (javaEnumRb.IsChecked == true)
            {
                JavaEnumGenerare generater = new JavaEnumGenerare("com.main.txmy", mConnection.connStr, mConnection.type);
                doc = generater.tableEnumGenerater(currentTableName, currentCommend);
            }
            else if (csharpEnumRb.IsChecked == true)
            {
                CsharpEnumGenerare generater = new CsharpEnumGenerare("codeGenerater", mConnection.connStr, mConnection.type);
                doc = generater.tableEnumGenerater(currentTableName, currentCommend);
            }
            else if (createSqlb.IsChecked == true)
            {
                SqlGenerater generater = new SqlGenerater(currentTableName, mConnection.connStr, mConnection.type);
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
            changed();
        }

        private void csharpRb_Checked(object sender, RoutedEventArgs e)
        {
            changed();
        }
        private void javaEnumRb_Checked(object sender, RoutedEventArgs e)
        {
            changed();
        }

        private void csharpEnumRb_Checked(object sender, RoutedEventArgs e)
        {
            changed();
        }
        private void createSqlb_Checked(object sender, RoutedEventArgs e)
        {
            changed();
        }
        private void changed()
        {
            if (!string.IsNullOrEmpty(currentTableName))
            {
                this.startCeneraterCode(currentTableName, currentCommend);
            }
        }
        #endregion

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            App.showMainWindow();
        }


    }
}
