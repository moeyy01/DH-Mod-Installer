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
using System.Drawing;
using System.Globalization;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using static DreadHunger.MainWindow;

namespace DreadHunger
{
    public class Client : INotifyPropertyChanged
    {
        public Channel ChannelName { get; set; }
        public string DirectoryPath { get; set; }
        public string Updatefilemd5 { get; set; }
        public string ShippingPath { get; set; }
        public string LogFilePath { get; set; }
        public string Cardfile { get; set; }
        public string Dhcfile { get; set; }
        public string Dhcdll { get; set; }
        public string Updatefile { get; set; }
        public string ModConfigFile { get; set; }
        public string GameSavePath { get; set; }
        public string ModInstallPath { get; set; }
        public string LastChecked { get; set; }
        public string IPConfigFile { get; set; }
        public string OfflineGamePath { get; set; }
        public string GameModsPath { get; set; }
        public string GameServerIP { get; set; }
        public string Card { get; set; }

        public bool PatchIsNeedUpdate = false;
        public string PatchNeedUpdateName { get; set; }
        public string Ipandport { get; set; }
        public bool CardStatus { get; set; }
        public bool ShowIpStatus = true;
        public string ShareLink { get; set; }
        public int RemainingTime { get; set; }

        public bool IsNeedUpdate { get; set; }
        public string NeedUpdateName { get; set; }

        public bool IsDownload { get; set; }
        public int NeedUpdateIndex { get; set; }
        public string Version { get; set; }
        public LastCheckBox CheckLog { get; set; }

        public string GameIni { get; set; }

        public bool HasTestMod { get; set; }

        public string TestModName { get; set; }



        private string _loginStatus;
        public string LoginStatus
        {
            get { return _loginStatus; }
            set
            {
                _loginStatus = value;
                OnPropertyChanged(nameof(LoginStatus));
            }
        }




        private string _announce;
        public string Announce
        {
            get { return _announce; }
            set
            {
                _announce = value;
                OnPropertyChanged(nameof(Announce));
            }
        }

        private string _serverLocation;
        public string ServerLocation
        {
            get { return _serverLocation; }
            set
            {
                _serverLocation = value;
                OnPropertyChanged(nameof(ServerLocation));
            }
        }

        private string _remainingTimeShow;
        public string RemainingTimeShow
        {
            get { return _remainingTimeShow; }
            set
            {
                _remainingTimeShow = value;
                OnPropertyChanged(nameof(RemainingTimeShow));
            }
        }


        private string _map;
        public string Map
        {
            get { return _map; }
            set
            {
                _map = value;
                OnPropertyChanged(nameof(Map));
            }
        }

        private string _roomId;
        public string RoomId
        {
            get { return _roomId; }
            set
            {
                _roomId = value;
                OnPropertyChanged(nameof(RoomId));
            }
        }

        private string _serverPing;
        public string ServerPing
        {
            get { return _serverPing; }
            set
            {
                _serverPing = value;
                OnPropertyChanged(nameof(ServerPing));
            }
        }


        private string _taobaoLbl;
        public string TaobaoLbl
    {
            get { return _taobaoLbl; }
            set
            {
                _taobaoLbl = value;
                OnPropertyChanged(nameof(TaobaoLbl));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public Client(Channel channelName,string version)
        {
            Version = version;
            ChannelName = channelName;
            switch (channelName)
            {
                case Channel.Cui:
                    DirectoryPath = "C:\\DreadHungerCui\\";
                    TaobaoLbl = "";
                    Updatefilemd5 = "c6fb8da686889a71ebfa951c01d764d8";
                    ModInstallPath = "cuimods";
                    break;
                default:
                    DirectoryPath = "C:\\DreadHungerClient\\";
                    break;
            }
            Updatefile = Path.Combine(DirectoryPath, "Update.exe");
            ModConfigFile = Path.Combine(DirectoryPath, "modconfig");
            LastChecked = Path.Combine(DirectoryPath, "LastChecked");
            GameIni = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DreadHunger\\Saved\\Config\\WindowsNoEditor\\Game.ini");
        }
    }

    public enum Channel
    {
        Cui,
        Samo,
        Chengyu
    }

    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : WindowX
    {      
        public string PatchedShipping = "bad66f223e69d682e7b23b9f8a91b635";
        public string[] maxplayer = { "[/Script/Engine.GameSession]", "MaxPlayers=64" };
        Client client = new Client(Channel.Cui,"1.0.0");


        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = client;            
        }

        private void WindowX_Loaded(object sender, RoutedEventArgs e)
        {
            // 创建目录
            if (!Directory.Exists(client.DirectoryPath))
            {
                try
                {
                    Directory.CreateDirectory(client.DirectoryPath);
                    Directory.CreateDirectory(client.LogFilePath);
                }
                catch (Exception ee)
                {
                    client.Announce = ee.Message;
                }
            }
            else
            {
                if (!Directory.Exists(client.LogFilePath))
                {
                    try
                    {
                        Directory.CreateDirectory(client.LogFilePath);
                    }
                    catch (Exception ee)
                    {
                        client.Announce = ee.Message;
                    }
                }
                if (File.Exists(client.GameSavePath))
                {
                    string GameInstallPath = File.ReadAllText(client.GameSavePath);
                    if (File.Exists(Path.Combine(GameInstallPath, "DreadHunger", "Binaries", "Win64", "DreadHunger-Win64-Shipping.exe")))
                    {
                        client.OfflineGamePath = GameInstallPath;
                        client.ShippingPath = Path.Combine(GameInstallPath, "DreadHunger", "Binaries", "Win64");
                        client.GameModsPath = Path.Combine(GameInstallPath, "DreadHunger", "Content", "Paks");                      
                    }
                    else
                    {
                        File.Delete(client.GameSavePath);
                    }
                }
                else if (File.Exists(client.ModConfigFile))
                {
                    string ModsPath = File.ReadAllText(client.ModConfigFile);
                    string shippingPath = ModsPath.Replace("\\Content\\Paks", "\\Binaries\\Win64");
                    if (Directory.Exists(ModsPath) && Directory.Exists(shippingPath))
                    {
                        client.GameModsPath = ModsPath;
                        client.ShippingPath = shippingPath;
                    }
                    else
                    {
                        File.Delete(client.ModConfigFile);
                    }
                }
                else
                {
                    Process[] Shipping = Process.GetProcessesByName("DreadHunger-Win64-Shipping");
                    if (Shipping.Count() > 0)
                    {
                        client.ShippingPath = System.IO.Path.GetDirectoryName(Shipping[0].MainModule.FileName);
                        client.GameModsPath = client.ShippingPath.Replace("\\Binaries\\Win64", "\\Content\\Paks");

                        File.WriteAllText(client.ModConfigFile, client.GameModsPath);

                    }
                }
            }

            Thread CheckDirectory = new Thread(new ThreadStart(CheckDirectoryPath));
            CheckDirectory.IsBackground = true;
            CheckDirectory.Start();

            PlayModGrid.Visibility = Visibility.Visible;

        }


        private void CheckDirectoryPath()
        {
            try
            {
                if (!File.Exists(client.Updatefile) || CalculateMD5Checksum(client.Updatefile) != client.Updatefilemd5)
                {
                    using (WebClient Webclient = new WebClient())
                    {
                        Uri uri = new Uri(GetUrlByChannelName("Update"));
                        Webclient.DownloadFile(uri, client.Updatefile);
                    }
                }
                var lastVersion = GetVersion();
                if (lastVersion != null)
                {
                    client.ShareLink = lastVersion.Download;
                    if (lastVersion.Version != client.Version)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            MessageBoxX.Show("", "Version update detected. New version will be downloaded soon.", this, MessageBoxButton.OK, new MessageBoxXConfigurations() { WindowStartupLocation = WindowStartupLocation.CenterOwner });
                        });
                        try
                        {
                            var info = new UpdateInfo();
                            info.Id = Process.GetCurrentProcess().Id;
                            info.ExePath = this.GetType().Assembly.Location;
                            info.Download = lastVersion.Download;
                            info.Version = lastVersion.Version;
                            var para = JsonConvert.SerializeObject(info);
                            ProcessStartInfo versionUpdatePrp = new ProcessStartInfo(client.Updatefile, AesEncryption.EncryptString(para));
                            Process newProcess = new Process();
                            newProcess.StartInfo = versionUpdatePrp;
                            newProcess.Start();
                        }
                        catch (Exception)
                        {

                        }
                    }
                    else
                    {

                        Thread th = new Thread(new ThreadStart(FirstFlushMod));
                        th.IsBackground = true;
                        th.Start();

                        Thread patchth = new Thread(FlushPatch);
                        patchth.IsBackground = true;
                        patchth.Start();

                    }
                }

            }
            catch (Exception e)
            {
                Dispatcher.Invoke(() =>
                {
                    MessageBoxX.Show("", e.Message, this, MessageBoxButton.OK, new MessageBoxXConfigurations() { WindowStartupLocation = WindowStartupLocation.CenterOwner });
                });

            }
        }

        private void TopMostBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!this.Topmost)
            {
                int text = 0xe66a;
                char unicodeChar = (char)text;
                TopMostBtn.Content = unicodeChar.ToString();
                TopMostBtn.ToolTip = "取消置顶";
                this.Topmost = true;
            }
            else
            {
                int text = 0xe634;
                char unicodeChar = (char)text;
                TopMostBtn.Content = unicodeChar.ToString();
                TopMostBtn.ToolTip = "置顶";
                this.Topmost = false;
            }
        }

        private void GetModPath_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Please select the DreadHunger.exe file";
            openFileDialog.Filter = "DreadHunger.exe (*.exe)|DreadHunger.exe";
            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;

                string Fullpath = Path.GetDirectoryName(filePath);

                string NowPath = Fullpath + "\\DreadHunger\\Content\\Paks";

                string ShippingPath = NowPath.Replace("\\Content\\Paks", "\\Binaries\\Win64");

                if (Directory.Exists(NowPath) && Directory.Exists(ShippingPath))
                {
                    client.GameModsPath = NowPath;
                    client.ShippingPath = ShippingPath;

                    MessageBoxX.Show("", "Directory binding successful", this, MessageBoxButton.OK, new MessageBoxXConfigurations() { WindowStartupLocation = WindowStartupLocation.CenterOwner });

                    Thread th = new Thread(new ThreadStart(FirstFlushMod));
                    th.IsBackground = true;
                    th.Start();

                    File.WriteAllText(client.ModConfigFile, client.GameModsPath);

                }
                else
                {
                    MessageBoxX.Show("", "Directory binding failed. Please reselect.", this, MessageBoxButton.OK, new MessageBoxXConfigurations() { WindowStartupLocation = WindowStartupLocation.CenterOwner });

                }
            }
        }

        private void OpenModPathBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(client.GameModsPath) && Directory.Exists(client.GameModsPath))
            {
                Process.Start(client.GameModsPath);
            }
            else
            {
                MessageBoxX.Show("", "Directory does not exist", this, MessageBoxButton.OK, new MessageBoxXConfigurations() { WindowStartupLocation = WindowStartupLocation.CenterOwner });
            }
        }


        private void InstallPatchBtnControl_Click(object sender, RoutedEventArgs e)
        {
            if (!client.IsDownload)
            {
                var button = sender as Button;
                var buttontype = (PatchButtonTag)button.Tag;
                var modInfo = buttontype.PatchContent;

                if (!string.IsNullOrWhiteSpace(client.ShippingPath))
                {
                    if (Directory.Exists(client.ShippingPath))
                    {
                        if (!Directory.Exists((Path.Combine(client.ShippingPath, "Patches"))))
                        {
                            Directory.CreateDirectory(Path.Combine(client.ShippingPath, "Patches"));
                        }
                        if (CalculateMD5Checksum(Path.Combine(client.ShippingPath, "DreadHunger-Win64-Shipping.exe")) != PatchedShipping)
                        {
                            var result = MessageBoxX.Show("", "Whether to install the patch frida", this, MessageBoxButton.YesNo, new MessageBoxXConfigurations() { YesButton = "Install", NoButton = "Cancel", WindowStartupLocation = WindowStartupLocation.CenterOwner, ReverseButtonSequence = true });
                            switch (result)
                            {
                                case MessageBoxResult.Yes:
                                    Process[] Shipping = Process.GetProcessesByName("DreadHunger-Win64-Shipping");
                                    if (Shipping.Count() > 0)
                                    {
                                        MessageBoxX.Show("", "Please close the game first", this, MessageBoxButton.OK, new MessageBoxXConfigurations() { WindowStartupLocation = WindowStartupLocation.CenterOwner });
                                    }
                                    else
                                    {
                                        Thread DownLoadBaseThread = new Thread(InstallPatchBase);
                                        DownLoadBaseThread.IsBackground = true;
                                        DownLoadBaseThread.Start();
                                    }
                                    break;
                                case MessageBoxResult.No:
                                    break;
                            }
                        }
                        else
                        {
                            Thread DownLoadFileThread = new Thread(new ParameterizedThreadStart(DownLoadPatchFile));
                            var pending = PendingBox.Show("Patch：" + modInfo.PatchName + "downloading", "Please wait", false, this, new PendingBoxConfigurations() { WindowStartupLocation = WindowStartupLocation.CenterOwner });
                            DownLoadFileThread.IsBackground = true;
                            DownLoadFileThread.Start(new DownloadPatchThread() { PatchContent = modInfo, PendingHandler = pending });
                        }
                    }
                }
                else
                {
                    MessageBoxX.Show("", "Patch folder not found. Please enter settings to bind.", this, MessageBoxButton.OK, new MessageBoxXConfigurations() { WindowStartupLocation = WindowStartupLocation.CenterOwner });
                }
            }

        }

        private void UnstallPatchBtnControl_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var buttontype = (PatchButtonTag)button.Tag;
            var modInfo = buttontype.PatchContent;

            if (DeletePatch(modInfo))
            {
                Thread th = new Thread(FlushPatch);
                th.IsBackground = true;
                th.Start();
            }
            else
            {
                MessageBoxX.Show("", "Deletion failed. Please close the game and try again.", this, MessageBoxButton.OK, new MessageBoxXConfigurations() { WindowStartupLocation = WindowStartupLocation.CenterOwner });
            }

        }

        private void UpdatePatchBtnControl_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var buttontype = (PatchButtonTag)button.Tag;
            var modInfo = buttontype.PatchContent;

            if (DeletePatch(modInfo))
            {
                Thread DownLoadFileThread = new Thread(new ParameterizedThreadStart(DownLoadPatchFile));
                var pending = PendingBox.Show("Patch：" + modInfo.PatchName + "downloading", "Please wait", false, this, new PendingBoxConfigurations() { WindowStartupLocation = WindowStartupLocation.CenterOwner });
                DownLoadFileThread.IsBackground = true;
                DownLoadFileThread.Start(new DownloadPatchThread() { PatchContent = modInfo, PendingHandler = pending });
            }
            else
            {
                MessageBoxX.Show("", "Deletion failed. Please close the game and try again.", this, MessageBoxButton.OK, new MessageBoxXConfigurations() { WindowStartupLocation = WindowStartupLocation.CenterOwner });
            }

        }

        private void InstallModBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!client.IsDownload)
            {
                Process[] Shipping = Process.GetProcessesByName("DreadHunger-Win64-Shipping");
                if (string.IsNullOrWhiteSpace(client.GameModsPath) && Shipping.Count() > 0)
                {
                    client.ShippingPath = System.IO.Path.GetDirectoryName(Shipping[0].MainModule.FileName);
                    client.GameModsPath = client.ShippingPath.Replace("\\Binaries\\Win64", "\\Content\\Paks");
                    MessageBoxX.Show("", "Directory binding is successful. Please click the installation button again.", this, MessageBoxButton.OK, new MessageBoxXConfigurations() { WindowStartupLocation = WindowStartupLocation.CenterOwner });

                    Thread th = new Thread(new ThreadStart(FirstFlushMod));
                    th.IsBackground = true;
                    th.Start();

                    File.WriteAllText(client.ModConfigFile, client.GameModsPath);

                }
                else
                {
                    var button = sender as Button;
                    var modInfo = button.Tag as ModContent;
                    if (!string.IsNullOrWhiteSpace(client.GameModsPath) && Directory.Exists(client.GameModsPath))
                    {
                        Thread DownLoadFileThread = new Thread(new ParameterizedThreadStart(DownLoadFile));
                        DownLoadFileThread.IsBackground = true;
                        DownLoadFileThread.Start(modInfo);
                    }
                    else
                    {
                        var response = MessageBoxX.Show("Please click Manual Binding and select the game installation directory [DreadHunger.exe] file", "Please run the game and click Install or select Bind Game", this, MessageBoxButton.OKCancel, new MessageBoxXConfigurations() { OKButton = "Manual Bind", WindowStartupLocation = WindowStartupLocation.CenterOwner, ReverseButtonSequence = true });
                        switch (response)
                        {
                            case MessageBoxResult.Cancel:
                                break;
                            case MessageBoxResult.OK:
                                OpenFileDialog openFileDialog = new OpenFileDialog();
                                openFileDialog.Title = "Please select the DreadHunger.exe file";
                                openFileDialog.Filter = "DreadHunger.exe (*.exe)|DreadHunger.exe";
                                if (openFileDialog.ShowDialog() == true)
                                {
                                    string filePath = openFileDialog.FileName;
                                    string Fullpath = System.IO.Path.GetDirectoryName(filePath);
                                    string NowPath = Fullpath + "\\DreadHunger\\Content\\Paks";
                                    client.GameModsPath = NowPath;
                                    client.ShippingPath = client.GameModsPath.Replace("\\Content\\Paks", "\\Binaries\\Win64");
                                    MessageBoxX.Show("", "Directory bind successful", this, MessageBoxButton.OK, new MessageBoxXConfigurations() { WindowStartupLocation = WindowStartupLocation.CenterOwner });

                                    Thread th = new Thread(new ThreadStart(FirstFlushMod));
                                    th.IsBackground = true;
                                    th.Start();

                                    File.WriteAllText(client.ModConfigFile, client.GameModsPath);
                                }
                                break;
                            default:
                                break;
                        }


                    }
                }
            }
        }

        private void UninstallModBtn_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var modInfo = button.Tag as ModContent;
            if (DeleteFile(modInfo))
            {
                if (modInfo.ModPakFileName == "Maxplayer_p.pak")
                {
                    if (File.Exists(client.GameIni))
                    {
                        File.Delete(client.GameIni);
                    }
                }
                Dispatcher.Invoke(() =>
                {
                    var pending = PendingBox.Show("Updating MOD list", "", false, this, new PendingBoxConfigurations() { WindowStartupLocation = WindowStartupLocation.CenterOwner });
                    Thread thth = new Thread(FlushMod);
                    thth.IsBackground = true;
                    thth.Start(pending);
                });
            }
            else
            {
                MessageBoxX.Show("", "Deletion failed. Please try again.", this, MessageBoxButton.OK, new MessageBoxXConfigurations() { WindowStartupLocation = WindowStartupLocation.CenterOwner });
            }
        }


        private void UpdateModBtn_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var modInfo = button.Tag as ModContent;
            if (DeleteFile(modInfo))
            {
                Thread UbdateFileThread = new Thread(new ParameterizedThreadStart(DownLoadFile));
                UbdateFileThread.IsBackground = true;
                UbdateFileThread.Start(modInfo);
            }
            else
            {
                MessageBoxX.Show("", "Deletion failed. Please try again.", this, MessageBoxButton.OK, new MessageBoxXConfigurations() { WindowStartupLocation = WindowStartupLocation.CenterOwner });
            }
        }

        private void LeftTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var send = this.LeftTree.SelectedItem as TreeViewItem;
            if (send != null)
            {
                switch ((string)send.Header)
                {                  
                    case "Play or Maps":
                        HiddenRightGrid();
                        PlayModGrid.Visibility = Visibility.Visible;
                        break;
                    case "Skin":
                        HiddenRightGrid();
                        QualityModGrid.Visibility = Visibility.Visible;
                        break;
                    case "Patches":
                        HiddenRightGrid();
                        PatchGrid.Visibility = Visibility.Visible;
                        break;
                    case "Settings":
                        HiddenRightGrid();
                        ManageGrid.Visibility = Visibility.Visible;
                        break;    

                }
            }
        }

    }
}