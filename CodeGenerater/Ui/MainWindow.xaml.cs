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
namespace CodeGenerater
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Connection> conns = null;
        public MainWindow()
        {
            InitializeComponent();
            new WindowBehavior(this).RepairWindowDefaultBehavior();
        }



        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void getListConn()
        {
            string path = Constract.ConnPath;
            string filePath = Constract.ConnFilePath;
            if (MyHelper.FileHelper.Exists(filePath))
            {
                conns = (List<Connection>)MyHelper.XmlHelper.Deserialize(typeof(List<Connection>), MyHelper.FileHelper.Reader(filePath, Encoding.UTF8));
            }
            else
            {
                conns = new List<Connection>();
            }
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
                    MessageBox.Show("暂不支持！");
                    break;
                case "about":
                    new AboutW().ShowDialog();
                    break;
                case "quit":
                    this.Close();
                    break;
            }
        }



        #endregion

        private void nextBtn_Click(object sender, RoutedEventArgs e)
        {
            new AddW().Show();
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            getListConn();
            changedAlertIngo();
            bindingElements();
        }

        private void bindingElements()
        {
            if (conns == null || conns.Count <= 0)
            {
                return;
            }
            this.mainBody.Children.Clear();
            string path = MyHelper.FileHelper.GetProjectRootPath() + "/Ui/connItem.xaml";
            for (int i = 0; i < conns.Count; i++)
            {
                FrameworkElement element = getFrameworkElementFromXaml(path);
                element.MouseMove += Element_MouseMove;
                element.MouseLeave += Element_MouseLeave;
                element.Tag = i;
                element.MouseLeftButtonUp += Element_MouseLeftButtonUp;
                TextBlock connNameTb = element.FindName("connName") as TextBlock;
                TextBlock connDesTb = element.FindName("connDes") as TextBlock;
                TextBlock connTypeTb = element.FindName("conntype") as TextBlock;
                TextBlock connStrTb = element.FindName("connstr") as TextBlock;
                Button deleteBtn = element.FindName("deleteBtn") as Button;
                deleteBtn.Tag = i;
                connNameTb.Text = conns[i].name;
                connDesTb.Text = conns[i].description;
                connTypeTb.Text = conns[i].type;
                connStrTb.Text = conns[i].connStr;
                deleteBtn.Click += DeleteBtn_Click;
                this.mainBody.Children.Add(element);
            }
        }

        private void Element_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Grid grid = sender as Grid;
            int index = Convert.ToInt32(grid.Tag.ToString());
            GeneraterW gw = new GeneraterW(conns[index]);
            this.Hide();
            gw.Show();
        }

        private void Element_MouseLeave(object sender, MouseEventArgs e)
        {
            Grid grid = sender as Grid;
            Border mainBorder = grid.FindName("main") as Border;
            mainBorder.Background = (Brush)App.Current.Resources["F9"];
            Button deleteBtn = ((FrameworkElement)sender).FindName("deleteBtn") as Button;
            deleteBtn.Visibility = Visibility.Hidden;
        }

        private void Element_MouseMove(object sender, MouseEventArgs e)
        {
            Grid grid = sender as Grid;
            Border mainBorder = grid.FindName("main") as Border;
            mainBorder.Background = Brushes.White;
            Button deleteBtn = ((FrameworkElement)sender).FindName("deleteBtn") as Button;
            deleteBtn.Visibility = Visibility.Visible;
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
                conns.RemoveAt(Convert.ToInt32(this.deleteTag));
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
            string xml = MyHelper.XmlHelper.Serialize(typeof(List<Connection>), conns);
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
            if (this.conns == null)
            {
                return;
            }
            if (this.conns.Count > 0)
            {
                msg = string.Format("共有 {0} 条连接 ", this.conns.Count);
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

        public static FrameworkElement getFrameworkElementFromXaml(string path)
        {
            XmlTextReader reader = new XmlTextReader(path);
            FrameworkElement element = XamlReader.Load(reader) as FrameworkElement;
            return element;
        }


        private void refreshBtn_Click(object sender, RoutedEventArgs e)
        {
            getListConn();
            changedAlertIngo();
            bindingElements();
        }
    }
}
