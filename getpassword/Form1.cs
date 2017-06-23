using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Win32;
using System.Diagnostics;

namespace getpassword
{
    public partial class Form1 : Form
    {
        private string urlref = "http://www.ishadowsocks.org/";
        /// <summary>
        /// 原表达式
        /// </summary>
        private string regexstrold = @"<div class=\""col-sm-4 text-center\"">([\s\S]*?)<font color=\""red\"">";
        /// <summary>
        /// 网页改版后的表达式
        /// </summary>
        private string regexstr = @"<div class=\""hover-bg\"">([\s\S]*?)<img src=";
        /// <summary>
        /// 正则之后二次表达式拆分
        /// </summary>
        private string regexstr2 = @">([\s\S]*?)<";
        HttpWebResponse response;
        Stream stream;
        StreamReader streamreader;
        HttpWebRequest requests;
        string pagestrings = "";
        
        /// <summary>
        ///  abccxzvcxvzxcv
        /// </summary>
        public Form1()
        {
            InitializeComponent();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            OpenDefaultBrowserUrl(urlref);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.richTextBox1.Text = "";
            requests = (HttpWebRequest)WebRequest.Create(urlref);
            requests.Method = "GET";
            response = (HttpWebResponse)requests.GetResponse();
            streamreader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            string stream2 = streamreader.ReadToEnd();

            MatchCollection TitleMatchs = getcollection(stream2, regexstr);
            foreach (Match NextMatch in TitleMatchs)
            {
                var TitleMatchs1 = getcollection(NextMatch.Groups[1].Value.Replace(" ", "").Replace("\r", ""), regexstr2);
                foreach (Match item in TitleMatchs1)
                {
                    var rictext = item.Groups[1].Value.Replace(" ", "").Replace("ClicktoviewQRCode", "");
                    this.richTextBox1.Text += rictext;
                }

                #region MyRegion
                //continue;
                //var matchstring = NextMatch.Groups[1].Value.Trim().Replace("</h4>", "").Replace("</font>", "").Replace("<font color=\"green\">", "").Replace("\n", "").Replace("<br/>", "").Replace(" ", "");
                //string[] str = { "<h4>" };
                //var stringlist = matchstring.Split(new[] { "<h4>" }, StringSplitOptions.None);
                //foreach (var items in stringlist)
                //{
                //    if (items != "")
                //    {
                //        richTextBox1.Text += items + "\r\n";
                //        if (items.Contains("正常"))
                //        {
                //            richTextBox1.Text += "\r\n";
                //        }
                //    }
                //} 
                #endregion
            }

            if (checkBox1.Checked)
            {
                downloadhtml();
            }
        }
        public string isaddtext(string text)
        {
            switch (text)
            {
                //    case "\n":
                ////        return "";
                //    case "\r\n":
                //        return "";
                default:
                    return text;//.Replace("\r","").Replace("\n","");
            }
        }

        public MatchCollection getcollection(string streams, string reg)
        {
            return Regex.Matches(streams, reg, RegexOptions.IgnoreCase | RegexOptions.Multiline);
        }

        public void downloadhtml()
        {
            HttpWebRequest reqeusts = (HttpWebRequest)WebRequest.Create(urlref);
            reqeusts.Method = "GET";
            response = (HttpWebResponse)reqeusts.GetResponse();
            stream = response.GetResponseStream();
            streamreader = new StreamReader(stream, Encoding.GetEncoding("gb2312"));
            pagestrings = streamreader.ReadToEnd();
            streamreader.Close();
            stream.Close();

            FileStream fs = new FileStream("d://a.html", FileMode.Create, FileAccess.ReadWrite);
            StreamWriter sw = new StreamWriter(fs, Encoding.GetEncoding("gb2312"));
            sw.WriteLine(pagestrings);
            sw.Flush();
            sw.Close();
        }


        #region 打开系统浏览器
        /// <summary>
        /// 调用系统浏览器打开网页
        /// http://m.jb51.net/article/44622.htm
        /// http://www.2cto.com/kf/201412/365633.html
        /// </summary>
        /// <param name="url">打开网页的链接</param>
        public static void OpenBrowserUrl(string url)
        {
            try
            {
                // 64位注册表路径
                var openKey = @"SOFTWARE\Wow6432Node\Google\Chrome";
                if (IntPtr.Size == 4)
                {
                    // 32位注册表路径
                    openKey = @"SOFTWARE\Google\Chrome";
                }
                RegistryKey appPath = Registry.LocalMachine.OpenSubKey(openKey);
                // 谷歌浏览器就用谷歌打开，没找到就用系统默认的浏览器
                // 谷歌卸载了，注册表还没有清空，程序会返回一个"系统找不到指定的文件。"的bug
                if (appPath != null)
                {
                    var result = Process.Start("chrome.exe", url);
                    if (result == null)
                    {
                        OpenIe(url);
                    }
                }
                else
                {
                    var result = Process.Start("chrome.exe", url);
                    if (result == null)
                    {
                        OpenDefaultBrowserUrl(url);
                    }
                }
            }
            catch
            {
                // 出错调用用户默认设置的浏览器，还不行就调用IE
                OpenDefaultBrowserUrl(url);
            }
        }

        /// <summary>
        /// 用IE打开浏览器
        /// </summary>
        /// <param name="url"></param>
        public static void OpenIe(string url)
        {
            try
            {
                Process.Start("iexplore.exe", url);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                // IE浏览器路径安装：C:\Program Files\Internet Explorer
                // at System.Diagnostics.process.StartWithshellExecuteEx(ProcessStartInfo startInfo)注意这个错误
                try
                {
                    if (File.Exists(@"C:\Program Files\Internet Explorer\iexplore.exe"))
                    {
                        ProcessStartInfo processStartInfo = new ProcessStartInfo
                        {
                            FileName = @"C:\Program Files\Internet Explorer\iexplore.exe",
                            Arguments = url,
                            UseShellExecute = false,
                            CreateNoWindow = true
                        };
                        Process.Start(processStartInfo);
                    }
                    else
                    {
                        if (File.Exists(@"C:\Program Files (x86)\Internet Explorer\iexplore.exe"))
                        {
                            ProcessStartInfo processStartInfo = new ProcessStartInfo
                            {
                                FileName = @"C:\Program Files (x86)\Internet Explorer\iexplore.exe",
                                Arguments = url,
                                UseShellExecute = false,
                                CreateNoWindow = true
                            };
                            Process.Start(processStartInfo);
                        }
                        else
                        {
                            if (MessageBox.Show(@"系统未安装IE浏览器，是否下载安装？", null, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question) == DialogResult.Yes)
                            {
                                // 打开下载链接，从微软官网下载
                                OpenDefaultBrowserUrl("http://windows.microsoft.com/zh-cn/internet-explorer/download-ie");
                            }
                        }
                    }
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message);
                }
            }
        }

        /// <summary>
        /// 打开系统默认浏览器（用户自己设置了默认浏览器）
        /// </summary>
        /// <param name="url"></param>
        public static void OpenDefaultBrowserUrl(string url)
        {
            try
            {
                // 方法1
                //从注册表中读取默认浏览器可执行文件路径
                RegistryKey key = Registry.ClassesRoot.OpenSubKey(@"http\shell\open\command\");
                if (key != null)
                {
                    string s = key.GetValue("").ToString();
                    //s就是你的默认浏览器，不过后面带了参数，把它截去，不过需要注意的是：不同的浏览器后面的参数不一样！
                    //"D:\Program Files (x86)\Google\Chrome\Application\chrome.exe" -- "%1"
                    var lastIndex = s.IndexOf(".exe", StringComparison.Ordinal);
                    if (lastIndex == -1)
                    {
                        lastIndex = s.IndexOf(".EXE", StringComparison.Ordinal);
                    }
                    var path = s.Substring(1, lastIndex + 3);
                    var result = Process.Start(path, url);
                    if (result == null)
                    {
                        // 方法2
                        // 调用系统默认的浏览器 
                        var result1 = Process.Start("explorer.exe", url);
                        if (result1 == null)
                        {
                            // 方法3
                            Process.Start(url);
                        }
                    }
                }
                else
                {
                    // 方法2
                    // 调用系统默认的浏览器 
                    var result1 = Process.Start("explorer.exe", url);
                    if (result1 == null)
                    {
                        // 方法3
                        Process.Start(url);
                    }
                }
            }
            catch
            {
                OpenIe(url);
            }
        }

        /// <summary>
        /// 火狐浏览器打开网页
        /// </summary>
        /// <param name="url"></param>
        public static void OpenFireFox(string url)
        {
            try
            {
                // 64位注册表路径
                var openKey = @"SOFTWARE\Wow6432Node\Mozilla\Mozilla Firefox";
                if (IntPtr.Size == 4)
                {
                    // 32位注册表路径
                    openKey = @"SOFTWARE\Mozilla\Mozilla Firefox";
                }
                RegistryKey appPath = Registry.LocalMachine.OpenSubKey(openKey);
                if (appPath != null)
                {
                    var result = Process.Start("firefox.exe", url);
                    if (result == null)
                    {
                        OpenIe(url);
                    }
                }
                else
                {
                    var result = Process.Start("firefox.exe", url);
                    if (result == null)
                    {
                        OpenDefaultBrowserUrl(url);
                    }
                }
            }
            catch
            {
                OpenDefaultBrowserUrl(url);
            }
        }
        #endregion

        private void Form1_HelpButtonClicked(object sender, CancelEventArgs e)
        {
            MessageBox.Show("*********  制作人：高强   *******\r\n*********  制作时间：2016-10-13  *******\r\n*********  联系qq：270669482  *******");
        }
    }
}
