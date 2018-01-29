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

namespace CodeGenerater
{
    /// <summary>
    /// DataMoveDesWxaml.xaml 的交互逻辑
    /// </summary>
    public partial class DataMoveW : Window
    {
        private List<Connection> mConnections;
        private Connection mConnection;
        private MyHelper.DbSchema CurrSchema;
        private string currCreateSql;
        public string DataMoverType = string.Empty;
        public DataMoveW(Connection conn, MyHelper.DbSchema schema, string moveType, string createSql = null)
        {
            InitializeComponent();
            mConnection = conn;
            CurrSchema = schema;
            currCreateSql = createSql;
            DataMoverType = moveType;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            mConnections = CommonFunction.getListConn();
            handleConn();
        }

        private void handleConn()
        {
            int count = mConnections.Count;
            List<Connection> list = new List<Connection>();
            foreach (Connection conn in mConnections)
            {
                if (conn.id != mConnection.id && conn.type == mConnection.type)
                {
                    list.Add(conn);
                }              
            }
            mConnections = list;       
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            bindingElement();
        }

        private void bindingElement()
        {

            if (mConnections == null || mConnections.Count <= 0)
            {
                MessageBox.Show("没有可以移动的连接！");
                TextBlock tb = new TextBlock();
                tb.HorizontalAlignment = HorizontalAlignment.Center;
                tb.VerticalAlignment = VerticalAlignment.Center;
                tb.Text = " 没有可以移动的连接！";
                return;
            }
            this.mainBody.Children.Clear();
            string path = string.Empty;
            if (App.DEBUG == true)
            {
                path = MyHelper.FileHelper.GetProjectRootPath() + "/Ui/connMoveItem.xaml";
            }
            else
            {
                path = MyHelper.FileHelper.GetRunTimeRootPath() + "/connMoveItem.xaml";
            }
            for (int i = 0; i < mConnections.Count; i++)
            {
                Grid element = (Grid)CommonFunction.getFrameworkElementFromXaml(path);
                element.MouseMove += Element_MouseMove;
                element.MouseLeave += Element_MouseLeave;
                element.Tag = i;
                element.MouseLeftButtonUp += Element_MouseLeftButtonUp;
                TextBlock connNameTb = element.FindName("connName") as TextBlock;
                TextBlock connDesTb = element.FindName("connDes") as TextBlock;
                TextBlock connTypeTb = element.FindName("conntype") as TextBlock;
                TextBlock connStrTb = element.FindName("connstr") as TextBlock;
                connNameTb.Text = mConnections[i].name;
                connDesTb.Text = mConnections[i].description;
                connTypeTb.Text = mConnections[i].type;
                connStrTb.Text = mConnections[i].connStr;
                this.mainBody.Children.Add(element);
            }
        }

        private void Element_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Grid grid = sender as Grid;
            if (grid != null && grid.Tag != null)
            {
                int index = Convert.ToInt32(grid.Tag.ToString());
                Connection to = mConnections[index];
                if (to != null)
                {
                    new DataMoveDialog(mConnection, to, CurrSchema, DataMoverType, currCreateSql).ShowDialog();
                    this.Close();
                }
            }
        }

        private void Element_MouseLeave(object sender, MouseEventArgs e)
        {
            Grid grid = sender as Grid;
            Border b = grid.FindName("main") as Border;
            b.Background = (Brush)App.Current.Resources["F9"];

        }

        private void Element_MouseMove(object sender, MouseEventArgs e)
        {
            Grid grid = sender as Grid;
            Border b = grid.FindName("main") as Border;
            b.Background = (Brush)App.Current.Resources["E4"];
        }

        #region windows event(move and close)
        private void headerBorder_MouseMove(object sender, MouseEventArgs e)
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
        #endregion

    }
}
