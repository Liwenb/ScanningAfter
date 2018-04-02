using AForge.Video.DirectShow;
using Microsoft.Win32;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ScanningAfter
{
    /// <summary>
    /// InStore.xaml 的交互逻辑
    /// </summary>
    public partial class InStore : Window
    {
        private MainWindow mm;
        private Product pro;
        public InStore(MainWindow mw)
        {
            InitializeComponent();
            mm = mw;
            if (mm.isTog == true && mm.ishoudo == false)
            {
                tiaoma.Text = mm.producttiaoma.Text;
                zongliang.Text = mm.zongl.Text;
            }
            else if (mm.isTog == false && mm.ishoudo == false)
                tiaoma.Text = mm.Content.Text;
            else
            {
                tiaoma.Text = mm.tiaoma.Text;
                zongliang.Text = mm.shul.Text;
            }
                
        }
        private void getStr()
        {
            pro = new Product();
            pro.Pinpai = pinpai.Text;
            pro.Pinming = pinming.Text;
            pro.Chengben = chengben.Text;
            pro.Beizhu = beizhu.Text;
            pro.Laiyuan = laiyuan.Text;
            pro.Tiaoma = tiaoma.Text;
            pro.Wujia = shoujia.Text;
            pro.Xinghao = xinghao.Text;
            pro.Zongliang = zongliang.Text;
            pro.file = ReadImageFile(strPath + "\\test1.png");
            #region            
            //pro = new Product();
            //if (pinpai.Text == "")
            //    pro.Pinpai = "";
            //else
            //    pro.Pinpai = pinpai.Text;
            //if (pinming.Text == "")
            //    pro.Pinming = "";
            //else
            //    pro.Pinming = pinming.Text;
            //if (chengben.Text == "")
            //    pro.Chengben ="0";
            //else
            //    pro.Chengben =chengben.Text;
            //if (beizhu.Text == "")
            //    pro.Beizhu = "";
            //else
            //    pro.Beizhu = beizhu.Text;
            //if (laiyuan.Text == "")
            //    pro.Laiyuan = "";
            //else
            //    pro.Laiyuan = laiyuan.Text;
            //if (tiaoma.Text == "")
            //    pro.Tiaoma = "";
            //else
            //    pro.Tiaoma = tiaoma.Text;
            //if (shoujia.Text == "")
            //    pro.Wujia = "0";
            //else
            //    pro.Wujia = shoujia.Text;
            //if (xinghao.Text == "")
            //    pro.Xinghao = "";
            //else
            //    pro.Xinghao = xinghao.Text;
            //if (zongliang.Text == "")
            //    pro.Zongliang = null;
            //else
            //    pro.Zongliang = zongliang.Text;
            //pro.file = ReadImageFile(strPath + "\\test.png");
            #endregion
        }
        private static byte[] ReadImageFile(String img)
        {
            FileInfo fileinfo = new FileInfo(img);
            byte[] buf = new byte[fileinfo.Length];
            FileStream fs = new FileStream(img, FileMode.Open, FileAccess.Read);
            fs.Read(buf, 0, buf.Length);
            fs.Close();
            //fileInfo.Delete ();
            GC.ReRegisterForFinalize(fileinfo);
            GC.ReRegisterForFinalize(fs);
            return buf;
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            mm.xinjian = false;
            mm.isxinjian = 0;
            sourcePlayer.Stop();
            this.Close();            
        }

        private void zongliang_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox txt = sender as TextBox;

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
                    MessageBox.Show(this.Resources["Txt_InnerPage_ConnPointManage_TabMyConnPoint_AddMyCloudSeeNum_Prompt"].ToString(), this.Resources["Txt_InnerPage_ConnPointManage_TabMyConnPoint_AddMyCloudSeeNum_PromptTitle"].ToString(), MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void zongliang_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
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
        private FilterInfoCollection videoDevices;
        private VideoCaptureDevice div = null;
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            //isClickOne = true;
            // 设定初始视频设备
            videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            if (videoDevices.Count > 0)
            { // 默认设备
                div = new VideoCaptureDevice(videoDevices[0].MonikerString);
                sourcePlayer.VideoSource = div;
            }
            sourcePlayer.Start();
            cut.IsEnabled = true;
            shex.IsEnabled = false;
        }
        //private DataTable IsExist;
        private bool isClick = false;
        private void canfirm_Click(object sender, RoutedEventArgs e)
        {
            getStr();
            try
            {
                string filePath = strPath + "\\test1.png";
                string fileName = strPath + "\\test1.png";
                pro.Tiaoma = Regex.Replace(pro.Tiaoma, "\r\n", string.Empty);
                Uri uri = new Uri("http://153.37.217.112:60003/stock/interface/saveInfo.do?code=" + pro.Tiaoma + "&brand=" + pro.Pinpai + "&name=" + pro.Pinming + "&model=" + pro.Xinghao + "&inPrice=" + pro.Chengben + "&outPrice=" + pro.Wujia + "&count=" + pro.Zongliang + "&origin=" + pro.Laiyuan + "&comment=" + pro.Beizhu + "");
                HttpWebRequest req = HttpWebRequest.CreateHttp(uri);
                req.Method = "POST";
                if (isClick == true)
                {
                    var boundary = "---------------" + DateTime.Now.Ticks.ToString("x");
                    var beginBoundary = Encoding.ASCII.GetBytes("--" + boundary + "\r\n");
                    var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

                    // 最后的结束符  
                    var endBoundary = Encoding.ASCII.GetBytes("--" + boundary + "--\r\n");

                    // 文件参数头  
                    const string filePartHeader =
                        "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\n" +
                         "Content-Type: application/octet-stream\r\n\r\n";
                    var fileHeader = string.Format(filePartHeader, "files", fileName);
                    var fileHeaderBytes = Encoding.UTF8.GetBytes(fileHeader);

                    // 开始拼数据  
                    var memStream = new MemoryStream();
                    memStream.Write(beginBoundary, 0, beginBoundary.Length);

                    // 文件数据  
                    memStream.Write(fileHeaderBytes, 0, fileHeaderBytes.Length);
                    var buffer = new byte[1024];
                    int bytesRead; // =0  
                    while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                    {
                        memStream.Write(buffer, 0, bytesRead);
                    }
                    fileStream.Close();

                    // Key-Value数据  
                    var stringKeyHeader = "\r\n--" + boundary +
                                           "\r\nContent-Disposition: form-data; name=\"{0}\"" +
                                           "\r\n\r\n{1}\r\n";

                    Dictionary<string, string> stringDict = new Dictionary<string, string>();
                    stringDict.Add("len", "500");
                    stringDict.Add("wid", "300");
                    foreach (byte[] formitembytes in from string key in stringDict.Keys
                                                     select string.Format(stringKeyHeader, key, stringDict[key])
                                                         into formitem
                                                     select Encoding.UTF8.GetBytes(formitem))
                    {
                        memStream.Write(formitembytes, 0, formitembytes.Length);
                    }

                    // 写入最后的结束边界符  
                    memStream.Write(endBoundary, 0, endBoundary.Length);

                    //倒腾到tempBuffer?  
                    memStream.Position = 0;
                    var tempBuffer = new byte[memStream.Length];
                    memStream.Read(tempBuffer, 0, tempBuffer.Length);
                    memStream.Close();

                    req.Timeout = 100000;
                    req.ContentType = "multipart/form-data;boundary=" + boundary;
                    req.ContentLength = tempBuffer.Length;
                    var requestStream = req.GetRequestStream();
                    requestStream.Write(tempBuffer, 0, tempBuffer.Length);
                    requestStream.Close();
                }
                WebResponse resp = req.GetResponse();
                StreamReader sr = new StreamReader(resp.GetResponseStream(), Encoding.Default);
                string sReturn = sr.ReadToEnd().Trim();
                //MessageBox.Show(sReturn);
                //string bb = sReturn.Split(':')[1];
                string bb = sReturn.Split(':')[1];
                if (int.Parse(bb.Substring(0, 1)) == 0)
                {
                    mm.displayInfo.Text = "商品新建失败";
                    mm.displayInfo.Foreground = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#fc4a64"));
                    mm.player.Open(new Uri(strPath + "\\4.mp3", UriKind.Relative));
                    mm.player.Play();
                }
                //mm.displayInfo.Text = "商品新建失败";
                else if (int.Parse(bb.Substring(0, 1)) == 1)
                {
                    mm.displayInfo.Text = "商品新建成功";
                    mm.displayInfo.Foreground = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#0bd38a"));
                    mm.player.Open(new Uri(strPath + "\\9.mp3", UriKind.Relative));
                    mm.player.Play();
                }
                //mm.displayInfo.Text = "商品新建成功";
                else{
                    mm.displayInfo.Text = "商品已存在";
                    mm.displayInfo.Foreground = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#fc4a64"));
                }
                //mm.displayInfo.Text = "商品已存在";
                mm.xinjian = false;
                mm.isxinjian = 0;
                this.Close();
            }
            catch (Exception r)
            {
                MessageBox.Show(r.Message.ToString());
                //throw;
            }
            #region
            //try
            //{
            //    getStr();
            //    IsExist = MySqlHelper.ExecuteDataTable(string.Format("SELECT count(*) as cc from goods td where td.barcode='{0}'", pro.Tiaoma), new MySqlParameter[] { });
            //    if (int.Parse(IsExist.Rows[0]["cc"].ToString()) > 0)
            //    {
            //        mm.displayInfo.Text = "商品已存在";
            //        mm.displayInfo.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#fc4a64"));
            //        //this.Close();
            //    }
            //    else
            //    {
            //        int i = MySqlHelper.ExecuteDataSet(string.Format("INSERT INTO `goods`(`barcode`,`in_price`,`out_price`,`remain_count`,`brand`,`model`,`comment`,`name`,`origin`)VALUES('{0}',{1},{2},{3},'{4}','{5}','{6}','{7}','{8}')", pro.Tiaoma, pro.Chengben, pro.Wujia, pro.Zongliang, pro.Pinpai, pro.Xinghao, pro.Beizhu, pro.Pinming, pro.Laiyuan), new MySqlParameter[] { });
            //        if (i > 0)
            //        {
            //            mm.displayInfo.Text = "商品新建成功";
            //            mm.displayInfo.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0bd38a"));
            //        }
            //        else
            //        {
            //            mm.displayInfo.Text = "商品新建失败";
            //            mm.displayInfo.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#fc4a64"));
            //        }
            //    }
            //    mm.xinjian = false;
            //    mm.isxinjian = 0;
            //    sourcePlayer.Stop();
            //    this.Close();              
            //}
            //catch (Exception)
            //{
            //    return;
            //}
            #endregion
        }
        private string strPath = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
        private void cut_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Windows.Controls.Image image = new System.Windows.Controls.Image();
                myImage.Source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                sourcePlayer.GetCurrentVideoFrame().GetHbitmap(),
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
                string ProImgPath = strPath + "\\test.png";//要保存的图片的地址，包含文件名
                BitmapSource BS = (BitmapSource)myImage.Source;
                PngBitmapEncoder PBE = new PngBitmapEncoder();
                PBE.Frames.Add(BitmapFrame.Create(BS));
                //getThumImage(ProImgPath, 10, 5, strPath+ "\\test1.png");           
                using (Stream stream = File.Create(strPath + "\\test.png"))
                {
                    PBE.Save(stream);
                    isClick = true;
                }
                GetPicThumbnail(ProImgPath, strPath + "\\test1.png", 480, 640, 50);
            }
            catch (Exception)
            {
                MessageBox.Show("请连接摄像头");
            }
           
        }
        /// <summary>
        /// 压缩图片
        /// </summary>
        /// <param name="sFile">输入路径</param>
        /// <param name="dFile">输出路径</param>
        /// <param name="dHeight">照片高度</param>
        /// <param name="dWidth">照片宽度</param>
        /// <param name="flag">帧数</param>
        /// <returns></returns>
        public static bool GetPicThumbnail(string sFile, string dFile, int dHeight, int dWidth, int flag)
        {
            System.Drawing.Image iSource = System.Drawing.Image.FromFile(sFile);
            ImageFormat tFormat = iSource.RawFormat;
            int sW = 0, sH = 0;

            //按比例缩放 
            System.Drawing.Size tem_size = new System.Drawing.Size(iSource.Width, iSource.Height);

            if (tem_size.Width > dHeight || tem_size.Width > dWidth)
            {
                if ((tem_size.Width * dHeight) > (tem_size.Width * dWidth))
                {
                    sW = dWidth;
                    sH = (dWidth * tem_size.Height) / tem_size.Width;
                }
                else
                {
                    sH = dHeight;
                    sW = (tem_size.Width * dHeight) / tem_size.Height;
                }
            }
            else
            {
                sW = tem_size.Width;
                sH = tem_size.Height;
            }

            Bitmap ob = new Bitmap(dWidth, dHeight);
            Graphics g = Graphics.FromImage(ob);

            g.Clear(System.Drawing.Color.WhiteSmoke);
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            g.DrawImage(iSource, new System.Drawing.Rectangle((dWidth - sW) / 2, (dHeight - sH) / 2, sW, sH), 0, 0, iSource.Width, iSource.Height, GraphicsUnit.Pixel);

            g.Dispose();
            //以下代码为保存图片时，设置压缩质量    
            EncoderParameters ep = new EncoderParameters();
            long[] qy = new long[1];
            qy[0] = flag;//设置压缩的比例1-100    
            EncoderParameter eParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, qy);
            ep.Param[0] = eParam;
            try
            {
                ImageCodecInfo[] arrayICI = ImageCodecInfo.GetImageEncoders();
                ImageCodecInfo jpegICIinfo = null;
                for (int x = 0; x < arrayICI.Length; x++)
                {
                    if (arrayICI[x].FormatDescription.Equals("JPEG"))
                    {
                        jpegICIinfo = arrayICI[x];
                        break;
                    }
                }
                if (jpegICIinfo != null)
                {
                    ob.Save(dFile, jpegICIinfo, ep);//dFile是压缩后的新路径    
                }
                else
                {
                    ob.Save(dFile, tFormat);
                }
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                iSource.Dispose();
                ob.Dispose();
            }
        }
        /// <summary>
        /// 获取本地图片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void get_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            fd.Filter = "图片文件|*.png;*.jpg";
            fd.ShowDialog();
            if (fd.FileName == "")
            {
                return;
            }
            myImage.Source = new BitmapImage(new Uri(fd.FileName, UriKind.Absolute));
            GetPicThumbnail(fd.FileName, strPath + "\\test1.png", 480, 640, 50);
        }
    }
}
