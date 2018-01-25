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
    public partial class HelpW : Window
    {
        private int mIndex;
        public HelpW(int index = -1)
        {
            InitializeComponent();
            mIndex = index;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.mainBody.Content = new MainFunctionP();
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

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {

            System.Windows.Controls.RadioButton rb = (System.Windows.Controls.RadioButton)sender;
            if (rb == null)
                return;
            string tag = rb.Tag.ToString();
            if (tag == "intruder") {
                this.mainBody.Content = new MainFunctionP();
            } else if (tag == "use") {
                this.mainBody.Content = new FunctionUse();
            } else if (tag == "quest") {
                this.mainBody.Content = new QuestionP();
            }
        }
    }
}
