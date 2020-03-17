using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Data;
using System.Windows.Threading;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using SqlDao;
namespace CodeGenerater
{
    /// <summary>
    /// DataMoveDesWxaml.xaml 的交互逻辑
    /// </summary>
    public partial class DataMoveDialog : Window
    {
        private Connection mFromConnection;
        private Connection mToConnection;
        private LocalSchema CurrSchema;
        private DispatcherTimer mDispatcherTimer;
        private string currCreateSql;
        public string DataMoverType = string.Empty;  
        private string loadingMsgStr = "执行中";
        private string successMsgStr = "执行成功";
        private string errorMsgStr = "执行失败";
        public DataMoveDialog(Connection fromConn, Connection toConn, LocalSchema schema, string moveType, string createSql = null)
        {
            InitializeComponent();
            mFromConnection = fromConn;
            mToConnection = toConn;
            CurrSchema = schema;
            currCreateSql = createSql;
            DataMoverType = moveType;

            mDispatcherTimer = new DispatcherTimer();
            mDispatcherTimer.Interval = TimeSpan.FromMilliseconds(5000);
            mDispatcherTimer.Tick += MDispatcherTimer_Tick;
        }

        private void MDispatcherTimer_Tick(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string sql = string.Empty;
            if (DataMoverType == DataMoveType.Schema.ToString())
            {
                sql = currCreateSql;

            }
            else if (DataMoverType == DataMoveType.Data.ToString())
            {
                sql = getDataSql();
            }
            else {
                sql = getDataSql();
            }
          
            if (string.IsNullOrEmpty(sql))
            {
                setIcon(-1);
                setMsg(this.errorMsgStr);
            }
            else
            {
                setIcon(0);
                setMsg(this.loadingMsgStr);
                try
                {
                    if (DataMoverType == DataMoveType.SchemaAndData.ToString())
                    {
                        if (excuted(currCreateSql))
                        {
                            setIcon(1);
                            setMsg(this.successMsgStr);
                        }
                        if (excuted(sql))
                        {
                            setIcon(1);
                            setMsg(this.successMsgStr);
                        }
                    }
                    else
                    {
                        if (excuted(sql))
                        {
                            setIcon(1);
                            setMsg(this.successMsgStr);
                        }
                    }
                }
                catch (Exception ex)
                {
                    setIcon(-1);
                    setMsg(ex.Message);
                }

            }
           startDispatcherTime();
        }

        private string getDataSql()
        {
            string sql = string.Empty;
            string tempsql = string.Empty;
            DataTable dt = null;
            tempsql = $"Select * from {CurrSchema.TableName}; ";
            if (mFromConnection.type == DbType.mysql.ToString())
            {
                dt = new MySqlHelper(mFromConnection.connStr).ExcuteDataTable(tempsql);
            }
            else
            {
                dt = new SQLiteHelper(mFromConnection.connStr).ExcuteDataTable(tempsql);
            }
            //INSERT INTO `｛｝` (｛｝) VALUES (｛｝);
            string columSpilt = "`";
            string valueSpilt = "'";
            string colums = string.Empty;
            string values = string.Empty;
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    if (string.IsNullOrEmpty(colums))
                    {
                        colums = columSpilt + dt.Columns[i].ColumnName + columSpilt;
                    }
                    else
                    {
                        colums += "," + columSpilt + dt.Columns[i].ColumnName + columSpilt;
                    }

                }
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    values = string.Empty;
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        string value = string.Empty;
                        if (dt.Rows[i][j] != null && !string.IsNullOrEmpty(dt.Rows[i][j].ToString()))
                        {
                            value = dt.Rows[i][j].ToString();
                            
                        }
                        else
                        {
                            value = "";
                        }

                        if (string.IsNullOrEmpty(values))
                        {
                        values = valueSpilt + value + valueSpilt;
                        }
                        else
                        {
                            values += "," + valueSpilt + value + valueSpilt;
                        }
                    }
                    if (string.IsNullOrEmpty(sql))
                    {
                        sql = $"INSERT INTO `{CurrSchema.TableName}` ({colums}) VALUES({values});";
                    }
                    else
                    {
                        sql += "\r\n" + $"INSERT INTO `{CurrSchema.TableName}` ({colums}) VALUES({values});";
                    }
                }
            }
            else
            {
                sql = string.Empty;
            }
            return sql;
        }

        private bool excuted(string sql)
        {
            if (mToConnection.type == DbType.mysql.ToString())
            {
                MySqlHelper tohelper = new MySqlHelper(mToConnection.connStr);
                return (tohelper.ExecuteNonQuery(sql,null) > 0);
            }
            else if (mFromConnection.type == DbType.sqlite.ToString())
            {
                SQLiteHelper tohelper = new SQLiteHelper(mToConnection.connStr);
                return (tohelper.ExcuteNoQuery(sql) > 0);
            }
            else
            {
                return false;
            }
        }

        private void setIcon(int i)
        {
            switch (i)
            {
                case -1:
                    loadIconTb.Visibility = Visibility.Collapsed;
                    sucessIconTb.Visibility = Visibility.Collapsed;
                    errorIconTb.Visibility = Visibility.Visible;
                    break;
                case 0:
                    loadIconTb.Visibility = Visibility.Visible;
                    sucessIconTb.Visibility = Visibility.Collapsed;
                    errorIconTb.Visibility = Visibility.Collapsed;
                    break;
                case 1:
                    loadIconTb.Visibility = Visibility.Collapsed;
                    sucessIconTb.Visibility = Visibility.Visible;
                    errorIconTb.Visibility = Visibility.Collapsed;
                    break;
            }

        }

        private void setMsg(string msg)
        {
            this.msgTb.Text = msg;
        }

        private void startDispatcherTime()
        {
            if (mDispatcherTimer != null)
            {
                mDispatcherTimer.Start();
            }
        }
        private void stopDispatcherTime()
        {
            if (mDispatcherTimer != null)
            {
                mDispatcherTimer.Stop();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.stopDispatcherTime();
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
