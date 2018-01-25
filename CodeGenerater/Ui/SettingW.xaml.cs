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
    public partial class SettingW : Window
    {
        private Connection mConnection;
        private int mIndex;
        public Action<Connection,int> changeParent { get; set; }
        public SettingW(Connection conn,int index=-1)
        {
            InitializeComponent();
            mConnection = conn;
            mIndex = index;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            BindingValue();
        }

        private void BindingValue() {
            if (mConnection == null) {
                return;
            }
            this.connNameTb.Text = mConnection.name;
            this.conntypeTb.Text = mConnection.type;
            this.desTb.Text = mConnection.description;
            this.ipTb.Text = mConnection.ipAddress;
            this.portTb.Text = mConnection.port;
            this.dbNmaeTb.Text = mConnection.dbName;
            this.userIdTb.Text = mConnection.uaerName;
            this.pwdTb.Text = mConnection.password;

            if (mConnection.auto == Auto.yes.ToString()) {
                onRb.IsChecked = true;
            }
            else
            {
                offRb.IsChecked = true;
            }
            this.nameSpaceTbox.Text = mConnection.nameSpace;
            this.CsharpClasssSaveTb.Text = mConnection.cSharpClassPath;
            this.CsharpEnumSaveTb.Text = mConnection.cSharpEnumPath;
            this.CsharpEnumAllSaveTb.Text = mConnection.cSharpEnumAllPath;
            this.javaPakeageTbox.Text = mConnection.pakeage;
            this.JavaClasssSaveTb.Text = mConnection.javaClassPath;
            this.JavaEnumSaveTb.Text = mConnection.javaEnumPath;
            this.SqlSaveTb.Text = mConnection.sqlPath;
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
            this.changeParent(mConnection, mIndex);
            this.Close();
        }

        private void ConfigItemPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
          
        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            this.changeParent(mConnection, mIndex);
        }

        #region select path dialog

        private void CsharpClassBtn_Click(object sender, RoutedEventArgs e)
        {
          System.Windows.Forms.FolderBrowserDialog fd = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = fd.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK || result == System.Windows.Forms.DialogResult.Yes) {
                string fileName = fd.SelectedPath;
                if (!string.IsNullOrEmpty(fileName))
                {
                    this.CsharpClasssSaveTb.Text = fileName;
                }
            }
        }

        private void CsharpenumBtn_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog fd = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = fd.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK || result == System.Windows.Forms.DialogResult.Yes)
            {
                string fileName = fd.SelectedPath;
                if (!string.IsNullOrEmpty(fileName))
                {
                    this.CsharpEnumSaveTb.Text = fileName;
                }
            }
        }

        private void CsharpenumAllBtn_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog fd = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = fd.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK || result == System.Windows.Forms.DialogResult.Yes)
            {
                string fileName = fd.SelectedPath;
                if (!string.IsNullOrEmpty(fileName))
                {
                    this.CsharpEnumAllSaveTb.Text = fileName;
                }
            }
        }

        private void JavaClassBtn_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog fd = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = fd.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK || result == System.Windows.Forms.DialogResult.Yes)
            {
                string fileName = fd.SelectedPath;
                if (!string.IsNullOrEmpty(fileName))
                {
                    this.JavaClasssSaveTb.Text = fileName;
                }
            }
        }

        private void JavaenumBtn_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog fd = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = fd.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK || result == System.Windows.Forms.DialogResult.Yes)
            {
                string fileName = fd.SelectedPath;
                if (!string.IsNullOrEmpty(fileName))
                {
                    this.JavaEnumSaveTb.Text = fileName;
                }
            }
        }

        private void SqlBtn_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog fd = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = fd.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK || result == System.Windows.Forms.DialogResult.Yes)
            {
                string fileName = fd.SelectedPath;
                if (!string.IsNullOrEmpty(fileName))
                {
                    this.SqlSaveTb.Text = fileName;
                }
            }
        }
        #endregion

        
        #region update connection after the Text Changed 
        private void nameSpaceTbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string temp = this.nameSpaceTbox.Text.Trim();
            if (temp.Length > 0) {
                mConnection.nameSpace = temp;
            }       
        }

        private void CsharpClasssSaveTb_TextChanged(object sender, TextChangedEventArgs e)
        {
            mConnection.cSharpClassPath = this.CsharpClasssSaveTb.Text.Trim();
        }

        private void CsharpEnumSaveTb_TextChanged(object sender, TextChangedEventArgs e)
        {
            mConnection.cSharpEnumPath = this.CsharpEnumSaveTb.Text.Trim();
        }

        private void CsharpEnumAllSaveTb_TextChanged(object sender, TextChangedEventArgs e)
        {
            mConnection.cSharpEnumAllPath = this.CsharpEnumAllSaveTb.Text.Trim();
        }

        private void javaPakeageTbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(this.javaPakeageTbox.Text.Length > 0)
            {
                mConnection.pakeage = this.javaPakeageTbox.Text.Trim();
            }            
        }

        private void JavaClasssSaveTb_TextChanged(object sender, TextChangedEventArgs e)
        {
            mConnection.javaClassPath = this.JavaClasssSaveTb.Text.Trim();
        }

        private void JavaEnumSaveTb_TextChanged(object sender, TextChangedEventArgs e)
        {
            mConnection.javaEnumPath = this.JavaEnumSaveTb.Text.Trim();
        }

        private void SqlSaveTb_TextChanged(object sender, TextChangedEventArgs e)
        {
            mConnection.sqlPath = this.SqlSaveTb.Text.Trim();
        }
        private void onRb_Checked(object sender, RoutedEventArgs e)
        {
            mConnection.auto = Auto.yes.ToString();
        }

        private void offRb_Checked(object sender, RoutedEventArgs e)
        {
            mConnection.auto = Auto.no.ToString();
        }


        #endregion

        private void helpBtn_Click(object sender, RoutedEventArgs e)
        {
            new HelpW().Show();
        }
    }
}
