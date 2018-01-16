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
    /// SettingW.xaml 的交互逻辑
    /// </summary>
    public partial class SettingW : Window
    {
        public SettingW()
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

        private void AddCameraBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            string sql = MyHelper.DbBaseHelper.getSelectSql("company", null, null);
            System.Data.DataTable dt = new MyHelper.MySqlHelper().select(sql);
            for (int i = 0; i < dt.Columns.Count; i++)
            {

                MyHelper.ConsoleHelper.writeLine("name:" + dt.Columns[i].ColumnName + " type:" + dt.Columns[i].DataType.ToString());
            }
        }
    }
}
