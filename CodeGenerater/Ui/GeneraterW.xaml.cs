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
using ICSharpCode.AvalonEdit.Highlighting;
using Microsoft.Win32;
using SqlDao;

namespace CodeGenerater
{
    /// <summary>
    /// SettingW.xaml 的交互逻辑
    /// </summary>
    public partial class GeneraterW : Window
    {
        private LocalSchema currLocalschma;
        private Connection mConnection;
        private string[] mTables;
        private List<DbSchema> mDbSchemas;
        private Int32 CacheSecond = 60;//默认在缓存为60 s
        private Dictionary<String, Int32> timeCache = new Dictionary<string, int>();
        private Dictionary<String, String> CodeCache = new Dictionary<string, String>();
        private string sufix;
        public GeneraterW(Connection conn)
        {
            InitializeComponent();
            this.mConnection = conn;
            new WindowBehavior(this).RepairWindowDefaultBehavior();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Cursor = Cursors.Wait;            
            setScrollViewHeight();
            setCodeScrollViewHeight();
            setCurrentConnInfo();            
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            // logical start
            getDBschemas();
            setTableNameList();
            this.Cursor = Cursors.Arrow;
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
                var instance = (MySqlHelper)DatabaseOPtionHelper.GetInstance(mConnection.connStr);
                mDbSchemas = instance.GetAllTableSchema<DbSchema>(mConnection.dbName);

                if(mDbSchemas == null || mDbSchemas.Count <= 0)
                {
                    //mysql 可以是可能 是 8.0
                    mDbSchemas = instance.GetAllTableSchema8(mConnection.dbName);
                }
            }
            else
            {
                mDbSchemas = new SQLiteHelper(mConnection.connStr).GetAllTableSchema<DbSchema>();
            }
        }

        private void getTablelist()
        {
            if (mConnection.type == DbType.mysql.ToString())
            {
                mTables = DatabaseOPtionHelper.GetInstance(mConnection.connStr).GetAllTableName(mConnection.dbName);
            }
            else if (mConnection.type == DbType.sqlite.ToString())
            {
                mTables = new SQLiteHelper(mConnection.connStr).GetAllTableName();
            }
            else
            {
                MessageBox.Show("未知的数据库类型！");
            }

        }

        public void setTableNameList()
        {

            if (mConnection.type == DbType.mysql.ToString())
            {
                if (mDbSchemas.Count > 0)
                {
                    this.tablePanel.Children.Clear();
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
            else
            {
                if (mDbSchemas.Count > 0)
                {
                    this.tablePanel.Children.Clear();
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
        }

        private UIElement getRadioButton(DbSchema schema)
        {
            RadioButton tb = new RadioButton();
            tb.Style = App.Current.Resources["menuRadioButtonStyle"] as Style;
            tb.Height = 28;
            tb.Width = this.tablePanel.ActualWidth;
            Foreground = App.Current.Resources["69"] as Brush;
            tb.Tag = schema;
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
            DbSchema schema = (DbSchema)tabbtn.Tag;
            currLocalschma = new LocalSchema()
            {
                createTime = schema.CreateTime,
                updateTime = schema.UpdateTime,
                dataLength = schema.DataLength,
                TableComment = schema.TableComment,
                TableName = schema.TableName,
                tableRows = schema.TableRows
            };
            startCeneraterCode();
        }
        #endregion

        #region start Cenerater Code
        private void startCeneraterCode()
        {
            // 设置文件名称
            SetCodeFileName();


            string doc = string.Empty;
            if (beanRb.IsChecked == true)
            {
                var key = currLocalschma.TableName + "java_bean";
                doc = GetCodeFromCache(key);

                if (string.IsNullOrEmpty(doc))
                {
                    doc = new JavaGenerater(currLocalschma, mConnection).CeneraterClass();
                    Cache(key, doc);
                }
                this.CodeTb.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("Java");
            }
            else if (csRb.IsChecked == true)
            {
                var key = currLocalschma.TableName + "cs";
                doc = GetCodeFromCache(key);

                if (string.IsNullOrEmpty(doc))
                {
                    CSharpCenerater generater = new CSharpCenerater(currLocalschma, mConnection);
                    doc = generater.CeneraterClass();
                    Cache(key, doc);
                }
                this.CodeTb.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("C#");
            }
            else if (beanEnumRb.IsChecked == true)
            {
                var key = currLocalschma.TableName + "java_enum";
                doc = GetCodeFromCache(key);
                if (string.IsNullOrEmpty(doc))
                {
                    JavaEnumGenerare generater = new JavaEnumGenerare(mConnection);
                    doc = generater.tableEnumGenerater(currLocalschma);
                    Cache(key, doc);
                }
                this.CodeTb.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("Java");

            }
            else if (enumcsRb.IsChecked == true)
            {
                var key = currLocalschma.TableName + "cs_enum";
                doc = GetCodeFromCache(key);
                if (string.IsNullOrEmpty(doc))
                {
                    CsharpEnumGenerare generater = new CsharpEnumGenerare(mConnection);
                    doc = generater.tableEnumGenerater(currLocalschma);
                    Cache(key, doc);
                }
                this.CodeTb.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("C#");

            }
            else if (sqlRb.IsChecked == true)
            {
                var key = currLocalschma.TableName + "sql";
                doc = GetCodeFromCache(key);
                if (string.IsNullOrEmpty(doc))
                {
                    SqlGenerater generater = new SqlGenerater(currLocalschma.TableName, mConnection.connStr, mConnection.type);
                    doc = generater.GeneraterSql();
                    Cache(key, doc);
                }
                this.CodeTb.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("Sql");

            }
            else if (conntrollerRb.IsChecked == true)
            {
                var key = currLocalschma.TableName + "controller";
                doc = GetCodeFromCache(key);
                if (string.IsNullOrEmpty(doc))
                {
                    doc = new ControllerGenerater(currLocalschma, mConnection).GeneraterCode();
                    Cache(key, doc);
                }
                this.CodeTb.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("Java");

            }
            else if (serviceRb.IsChecked == true)
            {
                var key = currLocalschma.TableName + "service";
                doc = GetCodeFromCache(key);
                if (string.IsNullOrEmpty(doc))
                {
                    doc = new ServiceGenerater(currLocalschma, mConnection).GeneraterCode();
                    Cache(key, doc);
                }
                this.CodeTb.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("Java");

            }
            else if (serviceImplRb.IsChecked == true)
            {
                var key = currLocalschma.TableName + "impl";
                doc = GetCodeFromCache(key);
                if (string.IsNullOrEmpty(doc))
                {
                    doc = new ServiceImplGenerater(currLocalschma, mConnection).GeneraterCode();
                    Cache(key, doc);
                }
                this.CodeTb.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("Java");

            }
            else if (mapperRb.IsChecked == true)
            {
                doc = new MapperGenerater(currLocalschma, mConnection).GeneraterCode();
            }
            else if (xmlRb.IsChecked == true)
            {
                var key = currLocalschma.TableName + "xml";
                doc = GetCodeFromCache(key);
                if (string.IsNullOrEmpty(doc))
                {
                    doc = new MapperXmlGenerater(currLocalschma, mConnection).GeneraterCode();
                    Cache(key, doc);
                }
                this.CodeTb.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("XML");

            }
            else if (lineBeanRb.IsChecked == true) {
                var key = currLocalschma.TableName + "java_line_bean";
                doc = GetCodeFromCache(key);

                if (string.IsNullOrEmpty(doc))
                {
                    doc = new JavaLineGenerater(currLocalschma, mConnection).CeneraterClass();
                    Cache(key, doc);
                }
                this.CodeTb.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("Java");

            }
            else if (showDocRb.IsChecked == true)
            {
                var key = currLocalschma.TableName + "show_doc";
                doc = GetCodeFromCache(key);

                if (string.IsNullOrEmpty(doc))
                {
                    doc = new ShowDocCenerater(currLocalschma, mConnection).Generater();
                    Cache(key, doc);
                }
            }
            else
            {
                doc = "\r\n 请点击左侧的表名或者上面的文件名 \r\n ";
            }
            this.CodeTb.Text = doc;
        }

        //设置文件名称
        private void SetCodeFileName()
        {
            if (currLocalschma == null)
                return;
            var tableNmae = StringHelper.DBNamingToCamelCase(currLocalschma.TableName);
            tableNmae = StringHelper.dbNameToClassName(tableNmae);
            this.csRb.Content = tableNmae + this.csRb.Tag.ToString();
            this.enumcsRb.Content = tableNmae + this.enumcsRb.Tag.ToString();
            // this.conntrollerRb.Content = tableNmae + this.conntrollerRb.Tag.ToString();
            //this.serviceRb.Content = tableNmae + this.serviceRb.Tag.ToString();
            //this.serviceImplRb.Content = tableNmae + this.serviceImplRb.Tag.ToString();
            // this.mapperRb.Content = tableNmae + this.mapperRb.Tag.ToString();
            // this.xmlRb.Content = tableNmae + this.xmlRb.Tag.ToString();
            this.beanRb.Content = tableNmae + this.beanRb.Tag.ToString();
            this.beanEnumRb.Content = tableNmae + this.beanEnumRb.Tag.ToString();
            this.sqlRb.Content = tableNmae + this.sqlRb.Tag.ToString();
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

            string doc = this.CodeTb.Text;
            SaveFileDialog sfd = new SaveFileDialog();
            var fileName = "";
            if (sufix != ".sql")
            {
                fileName = StringHelper.upperCaseFirstLetter(StringHelper.DBNamingToCamelCase(currLocalschma.TableName));
            }
            else
            {
                fileName = currLocalschma.TableName;
            }
            sfd.FileName = fileName + sufix.Substring(0, sufix.LastIndexOf("."));
            sfd.DefaultExt = sufix.Substring(sufix.LastIndexOf(".") + 1);

            if (sfd.ShowDialog() == true)
            {
                try
                {
                    FileHelper.Write(sfd.FileName, doc);
                    MessageBox.Show("保存成功！");
                }
                catch (Exception exc)
                {
                    MessageBox.Show("保存失败!原因：" + exc.Message);
                }

            }
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

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            var rb = sender as RadioButton;
            sufix = rb.Tag.ToString();
            if (currLocalschma != null)
            {
                this.startCeneraterCode();
            }
        }
        #endregion

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void DataMoveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (currLocalschma != null)
            {
                new DataMoveDesW(mConnection, currLocalschma, this.CodeTb.Text).ShowDialog();
            }
        }

        private String GetCodeFromCache(String key)
        {
            var seconds = DateTime.Now.Second;
            if (timeCache.ContainsKey(key))
            {
                var cacheTime = timeCache[key];
                if (cacheTime > seconds)
                {
                    seconds += 60;
                }
                if (seconds - cacheTime > cacheTime - 5)
                {
                    if (CodeCache.ContainsKey(key))
                        return CodeCache[key];
                }
                return String.Empty;
            }
            else
            {
                return String.Empty;
            }
        }
        private void Cache(String key, String doc)
        {
            var seconds = DateTime.Now.Second;
            if (timeCache.ContainsKey(key))
            {
                timeCache[key] = seconds;
            }
            else
            {
                timeCache.Add(key, seconds);
            }
            if (CodeCache.ContainsKey(key))
            {
                CodeCache[key] = doc;
            }
            else
            {
                CodeCache.Add(key, doc);
            }
        }

        private void HorizontalScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var offset = Math.Min(Math.Max(0, HorizontalScrollViewer.HorizontalOffset - e.Delta), HorizontalScrollViewer.ScrollableWidth);
            HorizontalScrollViewer.ScrollToHorizontalOffset(offset);
        }

        private void CodeScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var offset = Math.Min(Math.Max(0, CodeScrollViewer.VerticalOffset - e.Delta), CodeScrollViewer.ScrollableHeight);
            CodeScrollViewer.ScrollToVerticalOffset(offset);
        }
    }
}
