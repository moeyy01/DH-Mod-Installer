using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Panuon.UI.Silver;
using Panuon.UI.Silver.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using static DreadHunger.MainWindow;

namespace DreadHunger
{
    public partial class MainWindow
    {
        /// <summary>
        ///下载MOD pak专用 带显示进度 带回调 非阻塞
        /// </summary>
        /// <param name="url">下载链接</param>
        /// <param name="filename">完整路径文件名</param>
        public void DownLoadFile(string url, string filename)
        {
            client.IsDownload = true;
            Dispatcher.BeginInvoke(new Action(() =>
            {
                DownloadProcessBar.Visibility = Visibility.Visible;
            }));
            try
            {
                WebClient client = new WebClient();
                Uri uri = new Uri(url);

                client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(DownloadProgressCallback);
                client.DownloadFileCompleted += new AsyncCompletedEventHandler(DownloadFileCallback);
                client.DownloadFileAsync(uri, filename);
            }
            catch (Exception e)
            {
                client.Announce = e.Message;
            }
        }

        /// <summary>
        ///下载MOD Sig专用 不带显示进度 不带回调 非阻塞
        /// </summary>
        /// <param name="url">下载链接</param>
        /// <param name="filename">完整路径文件名</param>
        public void DownLoadFileWithOutProcess(string url, string filename)
        {
            try
            {
                using (WebClient Webclient = new WebClient())
                {
                    Uri uri = new Uri(url);
                    Webclient.DownloadFileAsync(uri, filename);
                }              
            }
            catch (Exception e)
            {
                client.Announce = e.Message;
            }
        }

        /// <summary>
        ///  显示MOD 下载进度
        /// </summary>
        private void DownloadProgressCallback(object sender, DownloadProgressChangedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                client.Announce = "Downloading：" + e.ProgressPercentage + "%";
                ProgressBarHelper.SetAnimateTo(DownloadProcessBar, e.ProgressPercentage);
            }));
        }

        /// <summary>
        /// MODPak下载完成 刷新UI
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DownloadFileCallback(object sender, AsyncCompletedEventArgs e)
        {
            client.IsDownload = false;
            client.Announce = "Download completed";

            Dispatcher.BeginInvoke(new Action(() =>
            {
                DownloadProcessBar.Visibility = Visibility.Collapsed;

                ProgressBarHelper.SetAnimateTo(DownloadProcessBar, 0);

                var pending = PendingBox.Show("Updating MOD list", "", false, this, new PendingBoxConfigurations() { WindowStartupLocation = WindowStartupLocation.CenterOwner });
                Thread thth = new Thread(FlushMod);
                thth.IsBackground = true;
                thth.Start(pending);
            })); 
        }



        public void InstallPatchBase()
        {
            client.IsDownload = true;
            Dispatcher.BeginInvoke(new Action(() =>
            {
                DownloadProcessBar.Visibility = Visibility.Visible;
            }));

            try
            {
                using (WebClient Webclient = new WebClient())
                {
                    Uri uri = new Uri(GetUrlByChannelName("PatchBase"));
                    Webclient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(InstallPatchBaseProgressCallback);
                    Webclient.DownloadFileCompleted += new AsyncCompletedEventHandler(InstallPatchBaseCallback);
                    Webclient.DownloadFileAsync(uri, Path.Combine(client.DirectoryPath, "Win64.zip"));
                }
            }
            catch (Exception e)
            {
                client.Announce = e.Message + "Please try again";
            }
        }



        private void InstallPatchBaseProgressCallback(object sender, DownloadProgressChangedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                client.Announce = "Downloading：" + e.ProgressPercentage + "%";
                ProgressBarHelper.SetAnimateTo(DownloadProcessBar, e.ProgressPercentage);
            }));
        }


        private void InstallPatchBaseCallback(object sender, AsyncCompletedEventArgs e)
        {
            client.IsDownload = false;
            Dispatcher.BeginInvoke(new Action(() =>
            {
                DownloadProcessBar.Visibility = Visibility.Collapsed;

                ProgressBarHelper.SetAnimateTo(DownloadProcessBar, 0);
            }));
            try
            {
                ExtractWithOverwrite(Path.Combine(client.DirectoryPath, "Win64.zip"), client.ShippingPath);
                File.Delete(Path.Combine(client.DirectoryPath, "Win64.zip"));
            }
            catch (Exception ee)
            {
                client.Announce = ee.Message + "Please try again";
            }
        }
    }
}
