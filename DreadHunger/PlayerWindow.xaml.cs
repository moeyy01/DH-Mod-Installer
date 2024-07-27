using Panuon.UI.Silver;
using Panuon.UI.Silver.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
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

namespace DreadHunger
{
    /// <summary>
    /// PlayerWindow.xaml 的交互逻辑
    /// </summary>
    public partial class PlayerWindow : WindowX
    {
        public string Url { get; set; }
        public string Filename { get; set; }
        public string VideoFile { get; set; }
        public string DirectoryPath { get; set; }

        public PlayerWindow()
        {
            InitializeComponent();
        }

        private void WindowX_Loaded(object sender, RoutedEventArgs e)
        {

            VideoFile = DirectoryPath + "\\" + Filename;

            if (File.Exists(VideoFile))
            {
                Loading.Visibility = Visibility.Hidden;
                mediaPlayer.Visibility = Visibility.Visible;
                mediaPlayer.Source = new Uri(VideoFile, UriKind.Absolute);
                mediaPlayer.Play();
            }
            else
            {
                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += new DoWorkEventHandler(backgroundDownLoadFile);
                worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundDownLoadFileCompleted);
                worker.RunWorkerAsync();
            }


        }

        private void backgroundDownLoadFileCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                bool result = (bool)e.Result;
                if (result)
                {
                    Loading.Visibility = Visibility.Hidden;
                    mediaPlayer.Visibility = Visibility.Visible;
                    mediaPlayer.Source = new Uri(VideoFile, UriKind.Absolute);
                    mediaPlayer.Play();
                }
                else
                {
                    MessageBoxX.Show("Video download failed", "error", this, MessageBoxButton.OK, new MessageBoxXConfigurations() { WindowStartupLocation = WindowStartupLocation.CenterOwner });
                    this.Close();
                }
            }
            catch (Exception)
            {
            }


        }

        private void backgroundDownLoadFile(object sender, DoWorkEventArgs e)
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                   client.DownloadFile(Url, DirectoryPath + "\\" + Filename);
                }
                e.Result = true; // 设置结果为true  
            }
            catch (Exception)
            {
                e.Result = false; // 设置结果为false  
            }
        }

        private void MediaPlayer_MediaEnded(object sender, RoutedEventArgs e)
        {
            try
            {
                mediaPlayer.Position = TimeSpan.Zero; // 将播放位置重置为开头  
                mediaPlayer.Play(); // 开始播放
            }
            catch (Exception)
            {

            }

        }

        private void WindowX_Closed(object sender, EventArgs e)
        {
            try
            {
                mediaPlayer.Stop();
                mediaPlayer.Source = null;
            }
            catch (Exception)
            {
            }

        }

    }
}