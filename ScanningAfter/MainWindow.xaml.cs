using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ScanningAfter
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private DataTable IsBatch;
        private DispatcherTimer MonitorTime;
        private InStore instore;
        public MediaPlayer player = new MediaPlayer();
        private string strPath = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
        private bool Iscome { get; set; }
        private bool Isgo { get; set; }
        KeyboardHook BarCode = new KeyboardHook();
        public MainWindow()
        {
            InitializeComponent();
            BarCode.KeyDownEvent += BarCode_KeyDownEvent;
        }
        private StringBuilder inputKey = new StringBuilder();
        private DateTime previewTime = DateTime.Now;
        private void BarCode_KeyDownEvent(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            string temp = string.Empty;
            DateTime nowTime = DateTime.Now;
            //string temp = string.Empty;
            //判断是不是数字的键盘值
            if ((e.KeyData >= Keys.D0 && e.KeyData <= Keys.D9) || (e.KeyData >= Keys.NumPad0 && e.KeyData <= Keys.NumPad9))
            {
                temp = (e.KeyData.ToString()).Last().ToString();
                //Content.Text = (e.KeyData.ToString()).Last().ToString();
            }
            else
            {
                temp = e.KeyData.ToString();
            }
            if ((nowTime - previewTime).Milliseconds < 20)//通过判断键盘输入的间隔来确定是扫描枪还是通过键盘输入的
            {
                if (e.KeyValue == (int)Keys.Return && inputKey.Length > 2)
                {
                    previewTime = DateTime.Now;
                    return;
                }
                inputKey.Append(temp);
                //Console.WriteLine(inputKey);
                string tempContent = inputKey.ToString();
                if (isTog == false && ishoudo == false)
                {
                    if (tempContent.Contains("Space"))
                    {
                        string s = tempContent.Substring(tempContent.LastIndexOf("Space")+5);
                        Content.Text = s;
                    }
                    else
                        Content.Text = tempContent;
                }
                else if (isTog == true && ishoudo == false)
                {
                    if (tempContent.Contains("Space"))
                    {
                        string s = tempContent.Substring(tempContent.LastIndexOf("Space") + 5);
                        producttiaoma.Text = s;
                    }
                    else
                        producttiaoma.Text = tempContent;
                    
                    //producttiaoma.Text = e.KeyData.ToString();
                }
                else
                {
                    Content.Text = "";
                }
            }
            else
            {
                inputKey = new StringBuilder(temp);
            }          
            previewTime = DateTime.Now;
        }
        private void InitTimer()
        {
            MonitorTime = new DispatcherTimer();
            MonitorTime.Interval = TimeSpan.FromMilliseconds(500);
            MonitorTime.Tick += (e, f) =>
            {
                //Console.WriteLine(inputKey);
                if (inputKey.ToString() != "")
                {
                    //Console.WriteLine(inputKey);
                    InitGetData();
                    //inputKey = new StringBuilder();                    
                }
                else
                    return;

            };
            MonitorTime.Start();
        }
        public bool isTog { get; set; }
        public string sql { get; set; }
        public string sqlHistory { get; set; }
        private void InitHistory(int goods_id, string create_time, int type, int count)
        {
            sqlHistory = string.Format("INSERT INTO `history`(`goods_id`,`create_time`,`type`,`count`) VALUES ({0},'{1}',{2},{3})", goods_id, create_time, type, count);
        }
        private void Initsql(string isgocome, int count, string barcode)
        {
            sql = string.Format("UPDATE `goods` SET `remain_count`=`remain_count`{0}{1} WHERE `barcode`='{2}'", isgocome, count, barcode);
        }
        public bool xinjian { get; set; }
        public int isxinjian = 0;
        private string content { get; set; }
        private void InitGetData()
        {
            try
            {
                if (Content.Text != "" || producttiaoma.Text != "")
                {
                    if (isTog == false)
                    {
                        if (Iscome == true)
                        {
                            InitIsexist(1, "入库", "+", Content.Text);
                        }
                        else if (Isgo == true)
                        {
                            InitIsexist(1, "出库", "-", Content.Text);
                        }
                        else
                        {
                            return;
                        }
                    }
                    else
                    {
                        IsBatch = MySqlHelper.ExecuteDataTable(string.Format("SELECT * from goods td where td.barcode='{0}'", producttiaoma.Text), new MySqlParameter[] { });
                        if (IsBatch.Rows.Count > 0)
                        {
                            productname.Text = IsBatch.Rows[0]["brand"].ToString();
                            zongl.Text = IsBatch.Rows[0]["remain_count"].ToString();
                        }
                        else
                        {
                            productname.Text = "";
                            zongl.Text = "";
                            name.Text = 1.ToString();
                        }
                        if (producttiaoma.Text != "")
                        {
                            name.Focusable = true;
                            mybtn.IsEnabled = true;
                        }
                        else
                            mybtn.IsEnabled = false;
                    }
                    Content.Text = "";
                }
                else                 
                    return;
            }
            catch (Exception)
            {
                return;
            }

        }
        private void submit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (isTog == false && tiaoma.Text != "")
                {
                    //this.Content.Text = (string)iData.GetData(DataFormats.Text);
                    if (Iscome == true)
                    {
                        InitIsexist(int.Parse(shul.Text), "入库", "+", tiaoma.Text);
                    }
                    else if (Isgo == true)
                    {
                        InitIsexist(int.Parse(shul.Text), "出库", "-", tiaoma.Text);
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                }
            }
            catch (Exception)
            {
                return;
            }

        }

        private void zongliang_TextChanged(object sender, TextChangedEventArgs e)
        {
            System.Windows.Controls.TextBox textBox = sender as System.Windows.Controls.TextBox;
            TextChange[] change = new TextChange[e.Changes.Count];
            e.Changes.CopyTo(change, 0);
            int offset = change[0].Offset;
            if (change[0].AddedLength > 0)
            {
                double num = 0;
                if (!Double.TryParse(textBox.Text, out num))
                {
                    textBox.Text = textBox.Text.Remove(offset, change[0].AddedLength);
                    textBox.Select(offset, 0);
                }
            }
        }
        private void zongliang_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            System.Windows.Controls.TextBox txt = sender as System.Windows.Controls.TextBox;

            //屏蔽非法按键
            if ((e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) || e.Key == Key.Decimal || e.Key.ToString() == "Tab")
            {
                if (txt.Text.Contains(".") && e.Key == Key.Decimal)
                {
                    e.Handled = true;
                    return;
                }
                e.Handled = false;
            }
            else if (((e.Key >= Key.D0 && e.Key <= Key.D9) || e.Key == Key.OemPeriod) && e.KeyboardDevice.Modifiers != ModifierKeys.Shift)
            {
                if (txt.Text.Contains(".") && e.Key == Key.OemPeriod)
                {
                    e.Handled = true;
                    return;
                }
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
                if (e.Key.ToString() != "RightCtrl")
                {
                    name.Text = "1";
                    //zongl.Text = "1";
                    //System.Windows.MessageBox.Show(this.Resources["Txt_InnerPage_ConnPointManage_TabMyConnPoint_AddMyCloudSeeNum_Prompt"].ToString(), this.Resources["Txt_InnerPage_ConnPointManage_TabMyConnPoint_AddMyCloudSeeNum_PromptTitle"].ToString(), MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }
        public bool ishoudo = false;
        private void isShou_Click(object sender, RoutedEventArgs e)
        {
            if (ishoudo == false)
            {
                GridShoudo.Visibility = System.Windows.Visibility.Visible;
                isShou.Content = "切换为自动输入";
                MonitorTime.Stop();
                Content.Visibility = System.Windows.Visibility.Collapsed;                
                Sptog.Visibility = System.Windows.Visibility.Collapsed;
                if (isTog == true)
                    stp.Visibility = System.Windows.Visibility.Collapsed;
                ishoudo = true;
            }
            else
            {
                if (isTog == true)
                {
                    stp.Visibility = System.Windows.Visibility.Visible;
                    Content.Visibility = System.Windows.Visibility.Collapsed;
                }
                else
                {
                    stp.Visibility = System.Windows.Visibility.Collapsed;
                    Content.Visibility = System.Windows.Visibility.Visible;
                }                                        
                GridShoudo.Visibility = System.Windows.Visibility.Collapsed;               
                MonitorTime.Start();
                ishoudo = false;
                Sptog.Visibility = System.Windows.Visibility.Visible;
                isShou.Content = "切换为手动输入";
            }
        }

        private void come_Click(object sender, RoutedEventArgs e)
        {

            come.Background = new SolidColorBrush(Colors.LightSkyBlue);
            go.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#474955"));
            Iscome = true;
            Isgo = false;
        }

        private void go_Click(object sender, RoutedEventArgs e)
        {
            go.Background = new SolidColorBrush(Colors.LightSkyBlue);
            come.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#474955"));
            Isgo = true;
            Iscome = false;
        }

        private void mybtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int count = int.Parse(name.Text);
                if (Iscome == true)
                {
                    InitIsexist(count, "入库", "+", producttiaoma.Text);
                }
                else if (Isgo == true)
                {
                    InitIsexist(count, "出库", "-", producttiaoma.Text);
                }
                productname.Text = "";
                producttiaoma.Text = "";
                zongl.Text = "";
                name.Text = 1.ToString();
                mybtn.IsEnabled = false;
                name.Focusable = false;
            }
            catch (Exception)
            {
                return;
            }

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            BarCode.Start();
            InitTimer();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            BarCode.Stop();
            System.Windows.Application.Current.Shutdown();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                this.DragMove();
            }
            catch (Exception)
            {
                return;
            }
        }
        private void InitIsexist(int count, string ruchu, string jiajian, string content)
        {
            IsBatch = MySqlHelper.ExecuteDataTable(string.Format("SELECT * from goods td where td.barcode='{0}'", content), new MySqlParameter[] { });
            if (IsBatch.Rows.Count > 0)
            {
                int kucun = int.Parse(IsBatch.Rows[0]["remain_count"].ToString());
                if (ruchu == "出库" && kucun < count)
                {
                    displayInfo.Text = ruchu + "失败";
                    displayInfo.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#fc4a64"));
                    player.Open(new Uri(strPath + "\\4.mp3", UriKind.Relative));
                    player.Play();
                    return;
                }
                if (ruchu == "入库")
                {
                    InitHistory(int.Parse(IsBatch.Rows[0]["goods_id"].ToString()), DateTime.Now.ToString(), 1, count);
                    MySqlHelper.ExecuteDataSet(sqlHistory, new MySqlParameter[] { });
                }
                else if (ruchu == "出库")
                {
                    InitHistory(int.Parse(IsBatch.Rows[0]["goods_id"].ToString()), DateTime.Now.ToString(), 2, count);
                    MySqlHelper.ExecuteDataSet(sqlHistory, new MySqlParameter[] { });
                }
                Initsql(jiajian, count, content);
                int i = MySqlHelper.ExecuteDataSet(sql, new MySqlParameter[] { });
                if (i > 0)
                {
                    IsBatch = MySqlHelper.ExecuteDataTable(string.Format("SELECT * from goods td where td.barcode='{0}'", content), new MySqlParameter[] { });
                    displayInfo.Text = IsBatch.Rows[0]["barcode"] + "(" + IsBatch.Rows[0]["brand"] + ")" + "/" + ruchu + "成功/剩余数量" + IsBatch.Rows[0]["remain_count"];
                    displayInfo.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0bd38a"));
                    player.Open(new Uri(strPath + "\\9.mp3", UriKind.Relative));
                    player.Play();
                }
                else
                {
                    displayInfo.Text = ruchu + "失败";
                    displayInfo.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#fc4a64"));
                    player.Open(new Uri(strPath + "\\4.mp3", UriKind.Relative));
                    player.Play();
                }

            }
            else
            {
                isxinjian++;
                if (xinjian == false && isxinjian == 1)
                {
                    instore = new InStore(this);
                    instore.ShowDialog();
                }
                else
                {
                    System.Windows.MessageBox.Show("商品正在新建，请等待");
                }
            }
        }
        private void togbutton_Click_1(object sender, RoutedEventArgs e)
        {
            if (isTog == false)
            {
                stp.Visibility = System.Windows.Visibility.Visible;
                Content.Visibility = System.Windows.Visibility.Collapsed;
                isTog = true;
            }
            else
            {
                stp.Visibility = System.Windows.Visibility.Collapsed;
                Content.Visibility = System.Windows.Visibility.Visible;
                productname.Text = "";
                producttiaoma.Text = "";
                zongl.Text = "";
                name.Text = 1.ToString();
                isTog = false;
            }
            //togbutton.Focusable = false;
        }
        private void Content_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Content.Text.Length > 0)
            {
                //MessageBox.Show("条码长度：" + Content.Text.Length + "\n条码内容：" + Content.Text, "系统提示");
            }
        }

        private void togbutton_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {            
            System.Windows.MessageBox.Show("asd");
            if (e.Key == Key.Enter||e.Key==Key.Space)
            { // MoveFocus takes a TraveralReqest as its argument.
                e.Handled = true;
            }
            base.OnKeyDown(e);
        }
    }
}
