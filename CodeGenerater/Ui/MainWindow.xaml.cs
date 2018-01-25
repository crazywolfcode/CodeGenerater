using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using System.Xml;
using System.Windows.Markup;
using System.Windows.Threading;
namespace CodeGenerater
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Connection> mConnections = null;
        List<System.Threading.Thread> mThreads = new List<System.Threading.Thread>();
        private DispatcherTimer mDispatcherTimer;
        private bool needRefresh = false;
        public MainWindow()
        {
            InitializeComponent();
            new WindowBehavior(this).RepairWindowDefaultBehavior();
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            if (this.IsLoaded == true)
            {
                if (needRefresh == true)
                {
                    needRefresh = false;
                    mConnections = mConnections = CommonFunction.getListConn();
                    changedAlertIngo();
                    bindingElements();
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void userHeaderImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.userMenuPopup.IsOpen == false)
            {
                this.userMenuPopup.IsOpen = true;
            }
            else { this.userMenuPopup.IsOpen = false; }
        }

        #region max min close mover
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
        #endregion mover

        #region menu
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        { //setting update request support about quit
            MenuItem item = sender as MenuItem;
            switch (item.Tag.ToString())
            {
                case "setting":
                    MessageBox.Show("暂不支持！");
                    break;
                case "update":
                    MessageBox.Show("暂不支持！");
                    break;
                case "request":
                    new RequestW().ShowDialog();
                    break;
                case "support":
                    new HelpW().Show();
                    break;
                case "about":
                    new AboutW().ShowDialog();
                    break;
                case "reward":
                    new RewardW().ShowDialog();
                    break;
                case "quit":
                    this.Close();
                    break;
            }
            this.userMenuPopup.IsOpen = false;
        }
        #endregion

        private void addBtn_Click(object sender, RoutedEventArgs e)
        {
            new AddW().Show();
            needRefresh = true;
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            mConnections = CommonFunction.getListConn();
            changedAlertIngo();
            bindingElements();
            StartThreadGenerate();
        }

        private void StartThreadGenerate()
        {
            if (mConnections == null || mConnections.Count <= 0)
            {
                return;
            }
            mDispatcherTimer = new DispatcherTimer();
            mDispatcherTimer.Interval = new TimeSpan(0, 0, 60);
            mDispatcherTimer.Tick += delegate
            {
                for (int i = 0; i < mConnections.Count; i++)
                {
                    new AutoGenerater(mConnections[i]).generater();
                }
            };
            startDispatcherTime();
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

        private void bindingElements()
        {
            if (mConnections == null || mConnections.Count <= 0)
            {
                return;
            }
            this.mainBody.Children.Clear();
            string path = MyHelper.FileHelper.GetProjectRootPath() + "/Ui/connItem.xaml";
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
                Button autoRb = element.FindName("AutoBtn") as Button;
                Button deleteBtn = element.FindName("deleteBtn") as Button;
                if (mConnections[i].auto == Auto.yes.ToString())
                {
                    autoRb.Foreground = Brushes.Green;
                    autoRb.ToolTip = "自动生成中";
                }
                else
                {
                    autoRb.Foreground = Brushes.Black;
                    autoRb.ToolTip = "设置项目跟踪生成";
                }
                autoRb.Tag = i;
                autoRb.Click += AutoRb_Click;
                deleteBtn.Tag = i;
                connNameTb.Text = mConnections[i].name;
                connDesTb.Text = mConnections[i].description;
                connTypeTb.Text = mConnections[i].type;
                connStrTb.Text = mConnections[i].connStr;
                deleteBtn.Click += DeleteBtn_Click;
                this.mainBody.Children.Add(element);
            }
        }

        private void AutoRb_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            int index = Convert.ToInt32(btn.Tag.ToString());
            Connection conn = mConnections[index];
            SettingW setting = new SettingW(conn, index);
            setting.changeParent = new Action<Connection, int>(updateConns);
            setting.ShowDialog();
        }

        public void updateConns(Connection conn, int index)
        {
            if (index > -1 && conn != null)
            {
                mConnections.RemoveAt(index);
                mConnections.Insert(index, conn);
                saveConnections();
            }
        }
        private void Element_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Grid grid = sender as Grid;
            int index = Convert.ToInt32(grid.Tag.ToString());
            GeneraterW gw = new GeneraterW(mConnections[index]);
            this.Hide();
            gw.ShowDialog();
            needRefresh = true;
            this.Show();
        }

        private void Element_MouseLeave(object sender, MouseEventArgs e)
        {
            Grid grid = sender as Grid;
            Border mainBorder = grid.FindName("main") as Border;
            mainBorder.Background = (Brush)App.Current.Resources["F9"];
            Button deleteBtn = ((FrameworkElement)sender).FindName("deleteBtn") as Button;
            Button autoBtn = ((FrameworkElement)sender).FindName("AutoBtn") as Button;
            deleteBtn.Visibility = Visibility.Hidden;
            autoBtn.Visibility = Visibility.Hidden;
        }

        private void Element_MouseMove(object sender, MouseEventArgs e)
        {
            Grid grid = sender as Grid;
            Border mainBorder = grid.FindName("main") as Border;
            mainBorder.Background = Brushes.White;
            Button deleteBtn = ((FrameworkElement)sender).FindName("deleteBtn") as Button;
            Button autoBtn = ((FrameworkElement)sender).FindName("AutoBtn") as Button;
            deleteBtn.Visibility = Visibility.Visible;
            autoBtn.Visibility = Visibility.Visible;
        }
        private string deleteTag;
        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            deleteTag = btn.Tag.ToString();
            if (string.IsNullOrEmpty(deleteTag))
            {
                return;
            }
            Grid element = this.mainBody.Children[Convert.ToInt32(deleteTag)] as Grid;
            //Storyboard sb = getStoryboard(element);
            //sb.Completed += Sb_Completed;  
            DoubleAnimation da1 = new DoubleAnimation();
            da1.From = 1;
            da1.Duration = new Duration(TimeSpan.Parse("0:0:0.5"));
            da1.To = 0;
            da1.Completed += delegate
            {
                this.mainBody.Children.Remove(element);
                mConnections.RemoveAt(Convert.ToInt32(this.deleteTag));
                this.bindingElements();
                changedAlertIngo();
                new System.Threading.Thread(new System.Threading.ThreadStart(updateConnXmlFile)).Start();
            };
            element.BeginAnimation(OpacityProperty, da1);
        }
        /// <summary>
        /// update the connection xml file
        /// </summary>
        private void updateConnXmlFile()
        {
            string xml = MyHelper.XmlHelper.Serialize(typeof(List<Connection>), mConnections);
            try
            {
                MyHelper.FileHelper.Write(Constract.ConnFilePath, xml);
            }
            catch (Exception)
            {
                MyHelper.ConsoleHelper.writeLine("update the connection xml file fauilure");
                throw;
            }
        }

        private void changedAlertIngo()
        {
            string msg = "还没有数据库连接";
            if (this.mConnections == null)
            {
                return;
            }
            if (this.mConnections.Count > 0)
            {
                msg = string.Format("共有 {0} 条连接 ", this.mConnections.Count);
            }
            this.alertTb.Text = msg;
        }

        private Storyboard getStoryboard(FrameworkElement element)
        {
            Storyboard sb = new Storyboard();
            DoubleAnimation da1 = new DoubleAnimation();
            da1.From = element.ActualHeight;
            da1.Duration = new Duration(TimeSpan.Parse("0:0:0.5"));
            da1.To = 0;
            Storyboard.SetTarget(da1, element);
            Storyboard.SetTargetProperty(da1, new PropertyPath("Height"));
            DoubleAnimation da2 = new DoubleAnimation();
            da2.From = 1;
            da2.To = 0;
            da2.Duration = new Duration(TimeSpan.Parse("0:0:0.5"));
            Storyboard.SetTarget(da2, element);
            Storyboard.SetTargetProperty(da2, new PropertyPath("Opacity"));
            return sb;
        }

  

        private void refreshBtn_Click(object sender, RoutedEventArgs e)
        {
            mConnections = CommonFunction.getListConn();
            changedAlertIngo();
            bindingElements();
            needRefresh = false;
        }

        private void saveConnections()
        {
            if (mConnections != null)
            {
                string xml = MyHelper.XmlHelper.Serialize(typeof(List<Connection>), mConnections);
                try
                {
                    MyHelper.FileHelper.Write(Constract.ConnFilePath, xml);
                }
                catch (Exception)
                {
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.stopDispatcherTime();
            this.saveConnections();
        }

    }
}
