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
    /// WecomeW.xaml 的交互逻辑
    /// </summary>
    public partial class WelcomeW : Window
    {
        public WelcomeW()
        {
            InitializeComponent();
        }

        #region smover
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

        #endregion mover

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {

        }
        private void startBtn_Click(object sender, RoutedEventArgs e)
        {
            new MainWindow().Show();
            this.Close();
        }

        private void RewardBtn_Click(object sender, RoutedEventArgs e)
        {
            new RewardW().ShowDialog();
        }
    }
}
