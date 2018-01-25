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
    public partial class DataMoveDesW : Window
    {
        private Connection mConnection;
        private MyHelper.DbSchema CurrSchema;
        private string currCreateSql;
        public string type = string.Empty;
        public DataMoveDesW(Connection conn, MyHelper.DbSchema schema, string createSql = null)
        {
            InitializeComponent();
            mConnection = conn;
            CurrSchema = schema;
            currCreateSql = createSql;
        }
        private void schemaRB_Checked(object sender, RoutedEventArgs e)
        {
            type = DataMoveType.Schema.ToString();
        }

        private void dataRB_Checked(object sender, RoutedEventArgs e)
        {
            type = DataMoveType.Data.ToString();
        }

        private void schemaDataRB_Checked(object sender, RoutedEventArgs e)
        {
            type = DataMoveType.SchemaAndData.ToString();
        }


       

        private void startBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(type)) {
                new DataMoveW(mConnection, CurrSchema,type,currCreateSql).ShowDialog();
                this.Close();
            }
        }

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
    }
}
