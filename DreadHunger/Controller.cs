using Panuon.UI.Silver;
using Panuon.UI.Silver.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace DreadHunger
{

    public partial class MainWindow
    {
        private string GetUrlByChannelName(string Method)
        {
            switch (Method)
            {
                case "Update":
                    switch (client.ChannelName)
                    {
                        case Channel.Cui: return "https://dhs-down.moeyy.cn/client/update.exe";
                    }
                    break;
                case "get_version":
                    switch (client.ChannelName)
                    {
                        case Channel.Cui: return "https://dreadhunger-server-api.moeyy.cn/client/get_mod_client_version";
                    }
                    break;
                case "PatchBase": return "https://dhs-down.moeyy.cn/patches/Win64.zip";
                case "get_patch_list": return "https://dreadhunger-server-api.moeyy.cn/client/get_patch_list";
                case "get_mod_list_v2": return "https://dreadhunger-server-api.moeyy.cn/client/get_mod_list_v2";

            }
            return "";
        }

        public static List<Grid> FindTopLevelGrids(Panel container)
        {
            List<Grid> topLevelGrids = new List<Grid>();
            foreach (UIElement child in container.Children)
            {
                if (child is Grid grid)
                {
                    topLevelGrids.Add(grid);
                }
            }
            return topLevelGrids;
        }



        private void HiddenRightGrid()
        {
            List<Grid> topLevelGrids = FindTopLevelGrids(RightGrid);
            // 现在你可以处理这些顶层 Grid 控件了  
            foreach (Grid grid in topLevelGrids)
            {
                grid.Visibility = Visibility.Hidden;
            }
        }

  



        private List<CheckBox> FindAllCheckedCheckBoxes(DependencyObject reference)
        {
            List<CheckBox> checkedCheckBoxes = new List<CheckBox>();
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(reference); i++)
            {
                var child = VisualTreeHelper.GetChild(reference, i);
                if (child is CheckBox checkBox)
                {
                    if (checkBox.IsChecked == true)
                    {
                        checkedCheckBoxes.Add(checkBox);
                    }
                }
                else
                {
                    checkedCheckBoxes.AddRange(FindAllCheckedCheckBoxes(child));
                }
            }
            return checkedCheckBoxes;
        }


        private void FlushPatch()
        {
            client.PatchIsNeedUpdate = false;
            string fileExtension = ".js";
            string[] JsFile = new string[] { };
            if (!string.IsNullOrWhiteSpace(client.ShippingPath))
            {
                JsFile = Directory.GetFiles(client.ShippingPath, $"*{fileExtension}", SearchOption.AllDirectories);
            }

            var list = GetPatchList();

            if (list != null)
            {
                var PatchInfoList = new List<PatchInfo>();
                foreach (var patch in list)
                {
                    var patchInfo = new PatchInfo();
                    patchInfo.FileFullName = "";
                    patchInfo.IsInstall = false;
                    patchInfo.IsNeedUpdate = false;
                    patchInfo.Patch = patch;
                    foreach (var Fileitem in JsFile)
                    {
                        if (patch.PatchFileName == System.IO.Path.GetFileName(Fileitem))
                        {
                            patchInfo.IsInstall = true;
                            patchInfo.FileFullName = Fileitem;
                            string FileMd5 = CalculateMD5Checksum(Fileitem);
                            if (!string.IsNullOrWhiteSpace(FileMd5))
                            {
                                if (FileMd5 != patch.PatchJsMd5)
                                {
                                    patchInfo.IsNeedUpdate = true;
                                    client.PatchIsNeedUpdate = true;
                                    client.PatchNeedUpdateName = patch.PatchNameEn;
                                }
                            }
                        }
                    }
                    PatchInfoList.Add(patchInfo);
                }

                PatchInfoList.Sort((x, y) => y.IsInstall.CompareTo(x.IsInstall));
                PatchInfoList.Sort((x, y) => y.IsNeedUpdate.CompareTo(x.IsNeedUpdate));

                FlushPatchInfoList(PatchInfoList);

                if (client.PatchIsNeedUpdate)
                {
                    Dispatcher.Invoke(() =>
                    {
                        HiddenRightGrid();
                        PatchGrid.Visibility = Visibility.Visible;
                        MessageBoxX.Show("Please update the patch and restart the game", client.PatchNeedUpdateName + "-need to be updated", this, MessageBoxButton.OK, new MessageBoxXConfigurations() { WindowStartupLocation = WindowStartupLocation.CenterOwner });
                    });
                }
            }
        }




        private void FlushMod(object pender)
        {
            client.HasTestMod = false;
            client.IsDownload = false;

            var pending = pender as IPendingHandler;
            client.IsNeedUpdate = false;
            Dispatcher.Invoke(() =>
            {
                DownloadProcessBar.Visibility = Visibility.Collapsed;
                pending.UpdateMessage("Getting MOD list");
                wanfa.Children.Clear();
                chuanchang.Children.Clear();
                gongcheng.Children.Clear();
                qiangshou.Children.Clear();
                lieren.Children.Clear();
                yisheng.Children.Clear();
                chushi.Children.Clear();
                daohang.Children.Clear();
                mushi.Children.Clear();
                laoshu.Children.Clear();
                yetu.Children.Clear();
                haibao.Children.Clear();
                lang.Children.Clear();
                beijixiong.Children.Clear();
                shishigui.Children.Clear();
                gutou.Children.Clear();
                meitan.Children.Clear();
                mutou.Children.Clear();
                shitou.Children.Clear();
                modaoshi.Children.Clear();
                yaoshi.Children.Clear();
                shouqiang.Children.Clear();
                daqiang.Children.Clear();
                gong.Children.Clear();
                caidao.Children.Clear();
                jundao.Children.Clear();
                gudao.Children.Clear();
                chanzi.Children.Clear();
                fuzi.Children.Clear();
                binggao.Children.Clear();
                dunrou.Children.Clear();
                shengrou.Children.Clear();
                shurou.Children.Clear();
                zhifang.Children.Clear();
                cha.Children.Clear();
                yapianding.Children.Clear();
                duyao.Children.Clear();
                jieduji.Children.Clear();
                meitantong.Children.Clear();
                zhayaotong.Children.Clear();
                zhentong.Children.Clear();
                denglong.Children.Clear();
                wangyuanjing.Children.Clear();
                weifenlei.Children.Clear();
            });
            var ModList = GetModList();
            if (ModList != null)
            {
                string fileExtension = ".pak";
                List<string> pakFile = new List<string>();
                if (!string.IsNullOrWhiteSpace(client.GameModsPath) && Directory.Exists(client.GameModsPath))
                {
                    pakFile = Directory.GetFiles(client.GameModsPath, $"*{fileExtension}", SearchOption.AllDirectories).ToList<string>();
                }
                var ModInfoList = new List<ModInfo>();

                foreach (var mod in ModList)
                {
                    var modInfo = new ModInfo();
                    modInfo.FileFullName = "";
                    modInfo.IsInstall = false;
                    modInfo.IsNeedUpdate = false;
                    modInfo.Mod = mod;
                    foreach (var Fileitem in pakFile)
                    {
                        if (Fileitem.ToLower().Contains("test") || Fileitem.Contains("测试"))
                        {
                            client.HasTestMod = true;
                            client.TestModName = Fileitem;
                        }
                        if (mod.ModPakFileName == System.IO.Path.GetFileName(Fileitem))
                        {
                            if (System.IO.Path.GetFileName(Fileitem) == "Maxplayer_p.pak")
                            {
                                if (!File.Exists(client.GameIni) || File.ReadAllLines(client.GameIni).Count() != 2)
                                {
                                    if (!Directory.Exists(Path.GetDirectoryName(client.GameIni)))
                                    {
                                        Directory.CreateDirectory(Path.GetDirectoryName(client.GameIni));
                                    }
                                    File.WriteAllLines(client.GameIni, maxplayer);
                                }
                            }
                            modInfo.IsInstall = true;

                            modInfo.FileFullName = Fileitem;

                            string FileMd5 = CalculateMD5Checksum(Fileitem);

                            if (!string.IsNullOrWhiteSpace(mod.ModPakMd5))
                            {
                                if (!string.IsNullOrWhiteSpace(FileMd5))
                                {
                                    if (mod.ModPakMd5 != FileMd5)
                                    {
                                        modInfo.IsNeedUpdate = true;
                                        client.IsNeedUpdate = true;
                                        client.NeedUpdateName = mod.ModNameEn;
                                        client.NeedUpdateIndex = mod.ModType;
                                    }
                                }
                            }
                        }
                    }
                    ModInfoList.Add(modInfo);
                }

                ModInfoList.Sort((x, y) => y.IsNeedUpdate.CompareTo(x.IsNeedUpdate));

                ModInfoList.Sort((x, y) => y.IsInstall.CompareTo(x.IsInstall));

                FlushModInfoList(ModInfoList);

                Dispatcher.Invoke(() =>
                {
                    pending.Close();
                });

                string ModType = "";

                if (client.IsNeedUpdate)
                {
                    if (client.NeedUpdateIndex == 1)
                    {
                        ModType = "玩法";
                    }
                    else
                    {
                        switch (client.NeedUpdateIndex)
                        {
                            case 1: ModType = "玩法"; break;
                            case 10: ModType = "船长"; break;
                            case 11: ModType = "工程"; break;
                            case 12: ModType = "枪手"; break;
                            case 13: ModType = "猎人"; break;
                            case 14: ModType = "医生"; break;
                            case 15: ModType = "厨师"; break;
                            case 16: ModType = "导航"; break;
                            case 17: ModType = "牧师"; break;
                            case 18: ModType = "老鼠"; break;
                            case 19: ModType = "野兔"; break;
                            case 20: ModType = "海豹"; break;
                            case 21: ModType = "狼"; break;
                            case 22: ModType = "北极熊"; break;
                            case 23: ModType = "食尸鬼"; break;
                            case 24: ModType = "骨头"; break;
                            case 25: ModType = "煤炭"; break;
                            case 26: ModType = "木头"; break;
                            case 27: ModType = "石头"; break;
                            case 28: ModType = "磨刀石"; break;
                            case 29: ModType = "钥匙"; break;
                            case 30: ModType = "手枪"; break;
                            case 31: ModType = "大枪"; break;
                            case 32: ModType = "弓"; break;
                            case 33: ModType = "菜刀"; break;
                            case 34: ModType = "军刀"; break;
                            case 35: ModType = "骨刀"; break;
                            case 36: ModType = "铲子"; break;
                            case 37: ModType = "斧子"; break;
                            case 38: ModType = "冰镐"; break;
                            case 39: ModType = "炖肉"; break;
                            case 40: ModType = "生肉"; break;
                            case 41: ModType = "熟肉"; break;
                            case 42: ModType = "脂肪"; break;
                            case 43: ModType = "茶"; break;
                            case 44: ModType = "鸦片酊"; break;
                            case 45: ModType = "毒药"; break;
                            case 46: ModType = "解毒剂"; break;
                            case 47: ModType = "煤炭桶"; break;
                            case 48: ModType = "炸药桶"; break;
                            case 49: ModType = "针筒"; break;
                            case 50: ModType = "灯笼"; break;
                            case 51: ModType = "望远镜"; break;
                            case 52: ModType = "血液颜色"; break;
                            default:
                                ModType = "Other"; break;
                        }
                    }
                    Dispatcher.Invoke(() =>
                    {
                        MessageBoxX.Show("Please update the Mod", ModType + " : " + client.NeedUpdateName + "Mods need updating", this, MessageBoxButton.OK, new MessageBoxXConfigurations() { WindowStartupLocation = WindowStartupLocation.CenterOwner });
                    });
                }
                if (client.HasTestMod)
                {
                    Dispatcher.Invoke(() =>
                    {
                        var result = MessageBoxX.Show(Path.GetFileNameWithoutExtension(client.TestModName) + "\n"+ "Testers please ignore", "A beta version of the MOD has been detected. It may cause the game to crash. Do you want to delete it?", this, MessageBoxButton.OKCancel, new MessageBoxXConfigurations() { OKButton = "delete", CancelButton = "ignore", WindowStartupLocation = WindowStartupLocation.CenterOwner, ReverseButtonSequence = true, FontSize = 20 });
                        switch (result)
                        {
                            case MessageBoxResult.OK:
                                try
                                {
                                    File.Delete(client.TestModName);
                                    File.Delete(client.TestModName.Replace(".pak", ".sig"));                                   
                                }
                                catch (Exception)
                                {
                                  
                                }
                                var Deletepending = PendingBox.Show("Updating MOD list", "", false, this, new PendingBoxConfigurations() { WindowStartupLocation = WindowStartupLocation.CenterOwner });
                                Thread thth = new Thread(FlushMod);
                                thth.IsBackground = true;
                                thth.Start(Deletepending);
                                break;
                            case MessageBoxResult.Cancel:
                                break;
                        }
                    });
                }
            }
        }

        private void FirstFlushMod()
        {
            client.HasTestMod = false;

            client.IsNeedUpdate = false;
            Dispatcher.Invoke(() =>
            {
                wanfa.Children.Clear();
                chuanchang.Children.Clear();
                gongcheng.Children.Clear();
                qiangshou.Children.Clear();
                lieren.Children.Clear();
                yisheng.Children.Clear();
                chushi.Children.Clear();
                daohang.Children.Clear();
                mushi.Children.Clear();
                laoshu.Children.Clear();
                yetu.Children.Clear();
                haibao.Children.Clear();
                lang.Children.Clear();
                beijixiong.Children.Clear();
                shishigui.Children.Clear();
                gutou.Children.Clear();
                meitan.Children.Clear();
                mutou.Children.Clear();
                shitou.Children.Clear();
                modaoshi.Children.Clear();
                yaoshi.Children.Clear();
                shouqiang.Children.Clear();
                daqiang.Children.Clear();
                gong.Children.Clear();
                caidao.Children.Clear();
                jundao.Children.Clear();
                gudao.Children.Clear();
                chanzi.Children.Clear();
                fuzi.Children.Clear();
                binggao.Children.Clear();
                dunrou.Children.Clear();
                shengrou.Children.Clear();
                shurou.Children.Clear();
                zhifang.Children.Clear();
                cha.Children.Clear();
                yapianding.Children.Clear();
                duyao.Children.Clear();
                jieduji.Children.Clear();
                meitantong.Children.Clear();
                zhayaotong.Children.Clear();
                zhentong.Children.Clear();
                denglong.Children.Clear();
                wangyuanjing.Children.Clear();
                weifenlei.Children.Clear();
            });
            var ModList = GetModList();
            if (ModList != null)
            {
                string fileExtension = ".pak";
                List<string> pakFile = new List<string>();
                if (!string.IsNullOrWhiteSpace(client.GameModsPath) && Directory.Exists(client.GameModsPath))
                {
                    pakFile = Directory.GetFiles(client.GameModsPath, $"*{fileExtension}", SearchOption.AllDirectories).ToList<string>();
                }
                var ModInfoList = new List<ModInfo>();

                foreach (var mod in ModList)
                {
                    var modInfo = new ModInfo();
                    modInfo.FileFullName = "";
                    modInfo.IsInstall = false;
                    modInfo.IsNeedUpdate = false;
                    modInfo.Mod = mod;
                    foreach (var Fileitem in pakFile)
                    {
                        if (Fileitem.ToLower().Contains("test") || Fileitem.Contains("测试"))
                        {
                            client.HasTestMod = true;
                            client.TestModName = Fileitem;
                        }
                        if (mod.ModPakFileName == System.IO.Path.GetFileName(Fileitem))
                        {                           
                            if (System.IO.Path.GetFileName(Fileitem) == "Maxplayer_p.pak")
                            {
                                if (!File.Exists(client.GameIni) || File.ReadAllLines(client.GameIni).Count() != 2)
                                {
                                    if (!Directory.Exists(Path.GetDirectoryName(client.GameIni)))
                                    {
                                        Directory.CreateDirectory(Path.GetDirectoryName(client.GameIni));
                                    }
                                    File.WriteAllLines(client.GameIni, maxplayer);
                                }
                            }

                            modInfo.IsInstall = true;

                            modInfo.FileFullName = Fileitem;

                            string FileMd5 = CalculateMD5Checksum(Fileitem);

                            if (!string.IsNullOrWhiteSpace(mod.ModPakMd5))
                            {
                                if (!string.IsNullOrWhiteSpace(FileMd5))
                                {
                                    if (mod.ModPakMd5 != FileMd5)
                                    {
                                        modInfo.IsNeedUpdate = true;
                                        client.IsNeedUpdate = true;
                                        client.NeedUpdateName = mod.ModName;
                                        client.NeedUpdateIndex = mod.ModType;
                                    }
                                }
                            }
                        }
                    }
                    ModInfoList.Add(modInfo);
                }

                ModInfoList.Sort((x, y) => y.IsNeedUpdate.CompareTo(x.IsNeedUpdate));

                ModInfoList.Sort((x, y) => y.IsInstall.CompareTo(x.IsInstall));

                FlushModInfoList(ModInfoList);

                Dispatcher.Invoke(() =>
                {
                    foreach (var MainItem in MainTab.Items)
                    {
                        var MainTabControlItem = MainItem as TabItem;
                        var SubTabControl = MainTabControlItem.Content as TabControl;
                        foreach (var SubTabControlItem in SubTabControl.Items)
                        {
                            var SubTabControlSubItem = SubTabControlItem as TabItem;

                            if (SubTabControlSubItem.Visibility == Visibility.Visible)
                            {
                                SubTabControlSubItem.IsSelected = true;
                                break;
                            }
                        }
                    }

                    foreach (var MainItem in MainTab.Items)
                    {
                        var MainTabControlItem = MainItem as TabItem;
                        if (MainTabControlItem.Visibility == Visibility.Visible)
                        {
                            MainTabControlItem.IsSelected = true;
                            break;
                        }
                    }
                });


                string ModType = "";

                if (client.IsNeedUpdate)
                {
                    if (client.NeedUpdateIndex == 1)
                    {
                        ModType = "玩法";
                    }
                    else
                    {
                        switch (client.NeedUpdateIndex)
                        {
                            case 1: ModType = "玩法"; break;
                            case 10: ModType = "船长"; break;
                            case 11: ModType = "工程"; break;
                            case 12: ModType = "枪手"; break;
                            case 13: ModType = "猎人"; break;
                            case 14: ModType = "医生"; break;
                            case 15: ModType = "厨师"; break;
                            case 16: ModType = "导航"; break;
                            case 17: ModType = "牧师"; break;
                            case 18: ModType = "老鼠"; break;
                            case 19: ModType = "野兔"; break;
                            case 20: ModType = "海豹"; break;
                            case 21: ModType = "狼"; break;
                            case 22: ModType = "北极熊"; break;
                            case 23: ModType = "食尸鬼"; break;
                            case 24: ModType = "骨头"; break;
                            case 25: ModType = "煤炭"; break;
                            case 26: ModType = "木头"; break;
                            case 27: ModType = "石头"; break;
                            case 28: ModType = "磨刀石"; break;
                            case 29: ModType = "钥匙"; break;
                            case 30: ModType = "手枪"; break;
                            case 31: ModType = "大枪"; break;
                            case 32: ModType = "弓"; break;
                            case 33: ModType = "菜刀"; break;
                            case 34: ModType = "军刀"; break;
                            case 35: ModType = "骨刀"; break;
                            case 36: ModType = "铲子"; break;
                            case 37: ModType = "斧子"; break;
                            case 38: ModType = "冰镐"; break;
                            case 39: ModType = "炖肉"; break;
                            case 40: ModType = "生肉"; break;
                            case 41: ModType = "熟肉"; break;
                            case 42: ModType = "脂肪"; break;
                            case 43: ModType = "茶"; break;
                            case 44: ModType = "鸦片酊"; break;
                            case 45: ModType = "毒药"; break;
                            case 46: ModType = "解毒剂"; break;
                            case 47: ModType = "煤炭桶"; break;
                            case 48: ModType = "炸药桶"; break;
                            case 49: ModType = "针筒"; break;
                            case 50: ModType = "灯笼"; break;
                            case 51: ModType = "望远镜"; break;
                            case 52: ModType = "血液颜色"; break;
                            default:
                                ModType = "Other"; break;
                        }
                    }
                    Dispatcher.Invoke(() =>
                    {
                        MessageBoxX.Show("Please update the Mod", ModType + " : " + client.NeedUpdateName + "Mods need updating", this, MessageBoxButton.OK, new MessageBoxXConfigurations() { WindowStartupLocation = WindowStartupLocation.CenterOwner });
                    });
                }
                if (client.HasTestMod)
                {
                    Dispatcher.Invoke(() =>
                    {
                        var result = MessageBoxX.Show(Path.GetFileNameWithoutExtension(client.TestModName) + "\n" + "Testers please ignore", "A beta version of the MOD has been detected. It may cause the game to crash. Do you want to delete it?", this, MessageBoxButton.OKCancel, new MessageBoxXConfigurations() { OKButton = "delete", CancelButton = "ignore", WindowStartupLocation = WindowStartupLocation.CenterOwner, ReverseButtonSequence = true, FontSize = 20 });
                        switch (result)
                        {

                            case MessageBoxResult.OK:
                                try
                                {
                                    File.Delete(client.TestModName);
                                    File.Delete(client.TestModName.Replace(".pak", ".sig"));
                                }
                                catch (Exception)
                                {

                                }
                                var Deletepending = PendingBox.Show("Updating MOD list", "", false, this, new PendingBoxConfigurations() { WindowStartupLocation = WindowStartupLocation.CenterOwner });
                                Thread thth = new Thread(FlushMod);
                                thth.IsBackground = true;
                                thth.Start(Deletepending);
                                break;
                            case MessageBoxResult.Cancel:
                                break;
                        }
                    });
                }
            }
        }


        private void FlushModInfoList(List<ModInfo> ModInfoList)
        {
            foreach (var item in ModInfoList)
            {
                try
                {
                    Dispatcher.Invoke(() =>
                    {
                        Grid grid = new Grid();

                        grid.Height = 200;

                        SetOnlineImageAsBackground(item.Mod.ModImgUrl, grid);

                        if (!string.IsNullOrWhiteSpace(item.Mod.ModIntroduceEn))
                        {
                            grid.ToolTip = new ToolTip() { FontSize = 20, Content = item.Mod.ModIntroduceEn };
                        }
                        grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(0.2, GridUnitType.Star) });
                        grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(0.2, GridUnitType.Star) });
                        grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(0.3, GridUnitType.Star) });
                        grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(0.1, GridUnitType.Star) });
                        grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(0.2, GridUnitType.Star) });

                        // 为Grid添加列定义  
                        grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(0.33, GridUnitType.Star) });
                        grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(0.33, GridUnitType.Star) });
                        grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(0.33, GridUnitType.Star) });

                        //如果有视频标题 再 加载视频播放键
                        if (!string.IsNullOrWhiteSpace(item.Mod.ModVideoName))
                        {
                            var playerbutton = new Button();
                            playerbutton.Content = "▶";
                            playerbutton.Tag = Tag = new VideoFile() { ModIntroduce = item.Mod.ModIntroduceEn, ModVideoUrl = item.Mod.ModVideoUrl, ModVideoName = item.Mod.ModVideoName };
                            playerbutton.FontSize = 40;
                            ButtonHelper.SetCornerRadius(playerbutton, new CornerRadius(15));
                            ButtonHelper.SetHoverBrush(playerbutton, new SolidColorBrush(Colors.White) { Opacity = 0.6 });
                            playerbutton.FontFamily = new System.Windows.Media.FontFamily("Segoe UI Symbol");
                            playerbutton.Background = new SolidColorBrush(Colors.White) { Opacity = 0.3 };
                            playerbutton.BorderBrush = new SolidColorBrush(Colors.White) { Opacity = 1 };
                            playerbutton.Foreground = new SolidColorBrush(Colors.White) { Opacity = 0.8 };
                            Grid.SetColumn(playerbutton, 1);
                            Grid.SetRow(playerbutton, 2);
                            playerbutton.Click += PlayBtn_Click;
                            grid.Children.Add(playerbutton);
                        }
                        if (item.IsInstall)
                        {
                            var Installbutton = new Button();
                            Installbutton.Click += UninstallModBtn_Click;
                            Installbutton.Content = "Uninstall";
                            Installbutton.Tag = item.Mod;
                            Installbutton.FontSize = 15;
                            Installbutton.Background = new SolidColorBrush(Colors.White) { Opacity = 0.5 };
                            Grid.SetColumn(Installbutton, 1);
                            Grid.SetRow(Installbutton, 4);
                            grid.Children.Add(Installbutton);
                        }
                        else
                        {
                            if (!String.IsNullOrWhiteSpace(item.Mod.ModPakUrl))
                            {
                                var Installbutton = new Button();
                                Installbutton.Click += InstallModBtn_Click;
                                Installbutton.Content = "Install";
                                Installbutton.Tag = item.Mod;
                                Installbutton.FontSize = 15;
                                Installbutton.Background = new SolidColorBrush(Colors.White) { Opacity = 0.5 };
                                Grid.SetColumn(Installbutton, 1);
                                Grid.SetRow(Installbutton, 4);
                                grid.Children.Add(Installbutton);
                            }
                        }

                        if (item.IsNeedUpdate)
                        {
                            var UpdateButton = new Button();
                            UpdateButton.Content = "Update";
                            UpdateButton.Tag = item.Mod;
                            UpdateButton.FontSize = 15;
                            UpdateButton.Click += UpdateModBtn_Click;
                            UpdateButton.Background = new SolidColorBrush(Colors.White) { Opacity = 0.5 };
                            Grid.SetColumn(UpdateButton, 2);
                            Grid.SetRow(UpdateButton, 4);
                            grid.Children.Add(UpdateButton);
                        }
                        //Mod标题
                        Label ModNameLabel = new Label();
                        ModNameLabel.Content = item.Mod.ModNameEn;
                        ModNameLabel.Foreground = new SolidColorBrush(Colors.White) { Opacity = 0.8 };
                        ModNameLabel.FontSize = 20;
                        Grid.SetRow(ModNameLabel, 0);
                        Grid.SetColumnSpan(ModNameLabel, 3);
                        grid.Children.Add(ModNameLabel);

                        switch (item.Mod.ModType)
                        {
                            case 1: wanfa.Children.Add(grid); break;
                            case 10: chuanchang.Children.Add(grid); break;
                            case 11: gongcheng.Children.Add(grid); break;
                            case 12: qiangshou.Children.Add(grid); break;
                            case 13: lieren.Children.Add(grid); break;
                            case 14: yisheng.Children.Add(grid); break;
                            case 15: chushi.Children.Add(grid); break;
                            case 16: daohang.Children.Add(grid); break;
                            case 17: mushi.Children.Add(grid); break;
                            case 18: laoshu.Children.Add(grid); break;
                            case 19: yetu.Children.Add(grid); break;
                            case 20: haibao.Children.Add(grid); break;
                            case 21: lang.Children.Add(grid); break;
                            case 22: beijixiong.Children.Add(grid); break;
                            case 23: shishigui.Children.Add(grid); break;
                            case 24: gutou.Children.Add(grid); break;
                            case 25: meitan.Children.Add(grid); break;
                            case 26: mutou.Children.Add(grid); break;
                            case 27: shitou.Children.Add(grid); break;
                            case 28: modaoshi.Children.Add(grid); break;
                            case 29: yaoshi.Children.Add(grid); break;
                            case 30: shouqiang.Children.Add(grid); break;
                            case 31: daqiang.Children.Add(grid); break;
                            case 32: gong.Children.Add(grid); break;
                            case 33: caidao.Children.Add(grid); break;
                            case 34: jundao.Children.Add(grid); break;
                            case 35: gudao.Children.Add(grid); break;
                            case 36: chanzi.Children.Add(grid); break;
                            case 37: fuzi.Children.Add(grid); break;
                            case 38: binggao.Children.Add(grid); break;
                            case 39: dunrou.Children.Add(grid); break;
                            case 40: shengrou.Children.Add(grid); break;
                            case 41: shurou.Children.Add(grid); break;
                            case 42: zhifang.Children.Add(grid); break;
                            case 43: cha.Children.Add(grid); break;
                            case 44: yapianding.Children.Add(grid); break;
                            case 45: duyao.Children.Add(grid); break;
                            case 46: jieduji.Children.Add(grid); break;
                            case 47: meitantong.Children.Add(grid); break;
                            case 48: zhayaotong.Children.Add(grid); break;
                            case 49: zhentong.Children.Add(grid); break;
                            case 50: denglong.Children.Add(grid); break;
                            case 51: wangyuanjing.Children.Add(grid); break;
                            case 52: xueyeyanse.Children.Add(grid); break;
                            default:
                                weifenlei.Children.Add(grid);
                                break;
                        }
                    });
                }
                catch (Exception e)
                {
                    Dispatcher.Invoke(() =>
                    {
                        MessageBoxX.Show(e.Message, "Please contact Moeyy", this, MessageBoxButton.OK, new MessageBoxXConfigurations() { WindowStartupLocation = WindowStartupLocation.CenterOwner });
                    });
                }
            }

            Dispatcher.Invoke(() =>
            {
                foreach (var MainItem in MainTab.Items)
                {
                    int number = 0;
                    var MainTabControlItem = MainItem as TabItem;
                    var SubTabControl = MainTabControlItem.Content as TabControl;
                    foreach (var SubTabControlItem in SubTabControl.Items)
                    {
                        var SubTabControlSubItem = SubTabControlItem as TabItem;
                        var SubItemWaterfallViewer = SubTabControlSubItem.Content as WaterfallViewer;
                        if (SubItemWaterfallViewer.Children.Count == 0)
                        {
                            number++;
                            SubTabControlSubItem.Visibility = Visibility.Collapsed;
                        }
                        if (number == SubTabControl.Items.Count)
                        {
                            MainTabControlItem.Visibility = Visibility.Collapsed;
                        }
                    }
                }
            });
        }

        private void PlayBtn_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var videofile = (VideoFile)button.Tag;
            var window = new PlayerWindow();
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            window.Url = videofile.ModVideoUrl;
            window.DirectoryPath = client.DirectoryPath;
            window.Filename = videofile.ModVideoName;

            var introduce = videofile.ModIntroduce;
            if (string.IsNullOrWhiteSpace(introduce) || introduce.Any(c => !IsValidTitleCharacter(c)))
            {
                introduce = "Default Title";
            }

            window.Title = introduce;
            window.Owner = this;
            window.ShowDialog();
        }

        private bool IsValidTitleCharacter(char c)
        {
            return char.IsLetterOrDigit(c) || char.IsWhiteSpace(c) || char.IsPunctuation(c);
        }

        private void FlushPatchInfoList(List<PatchInfo> PatchInfoList)
        {
            Dispatcher.Invoke(() =>
            {
                Patch.Children.Clear();
            });

            foreach (var item in PatchInfoList)
            {
                Dispatcher.Invoke(() =>
                {
                    try
                    {
                        Grid grid = new Grid();
                        grid.ToolTip = new ToolTip() { FontSize = 20, Content = item.Patch.PatchIntroduceEn };
                        grid.Height = 200;
                        grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(0.2, GridUnitType.Star) });
                        grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(0.2, GridUnitType.Star) });
                        grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(0.3, GridUnitType.Star) });
                        grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(0.1, GridUnitType.Star) });
                        grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(0.2, GridUnitType.Star) });

                        // 为Grid添加列定义  
                        grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(0.33, GridUnitType.Star) });
                        grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(0.33, GridUnitType.Star) });
                        grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(0.33, GridUnitType.Star) });

                        SetOnlineImageAsBackground(item.Patch.PatchImgUrl, grid);

                        if (item.IsInstall)
                        {
                            var Installbutton = new Button();
                            Installbutton.Click += UnstallPatchBtnControl_Click;
                            Installbutton.Content = "Uninstall";
                            Installbutton.Tag = new PatchButtonTag() { ButtonType = Installbutton.Content.ToString(), PatchContent = item.Patch };
                            Installbutton.FontSize = 15;
                            Installbutton.Background = new SolidColorBrush(Colors.White) { Opacity = 0.5 };
                            Grid.SetColumn(Installbutton, 1);
                            Grid.SetRow(Installbutton, 4);
                            grid.Children.Add(Installbutton);
                        }
                        else
                        {
                            if (!String.IsNullOrWhiteSpace(item.Patch.PatchUrl))
                            {
                                var Installbutton = new Button();
                                Installbutton.Click += InstallPatchBtnControl_Click;
                                Installbutton.Content = "Install";
                                Installbutton.Tag = new PatchButtonTag() { ButtonType = Installbutton.Content.ToString(), PatchContent = item.Patch };
                                Installbutton.FontSize = 15;
                                Installbutton.Background = new SolidColorBrush(Colors.White) { Opacity = 0.5 };
                                Grid.SetColumn(Installbutton, 1);
                                Grid.SetRow(Installbutton, 4);
                                grid.Children.Add(Installbutton);
                            }
                        }
                        if (item.IsNeedUpdate)
                        {
                            var UpdateButton = new Button();
                            UpdateButton.Content = "Update";
                            UpdateButton.Tag = new PatchButtonTag() { ButtonType = UpdateButton.Content.ToString(), PatchContent = item.Patch };
                            UpdateButton.FontSize = 15;
                            UpdateButton.Click += UpdatePatchBtnControl_Click;
                            UpdateButton.Background = new SolidColorBrush(Colors.White) { Opacity = 0.5 };
                            Grid.SetColumn(UpdateButton, 2);
                            Grid.SetRow(UpdateButton, 4);
                            grid.Children.Add(UpdateButton);
                        }
                        //Mod标题
                        Label ModNameLabel = new Label();
                        ModNameLabel.Content = item.Patch.PatchNameEn;
                        ModNameLabel.Foreground = new SolidColorBrush(Colors.White) { Opacity = 0.8 };
                        ModNameLabel.FontSize = 20;
                        Grid.SetRow(ModNameLabel, 0);
                        Grid.SetColumnSpan(ModNameLabel, 3);
                        grid.Children.Add(ModNameLabel);

                        Patch.Children.Add(grid);
                    }
                    catch (Exception e)
                    {
                        MessageBoxX.Show(e.Message, "Please contact Moeyy", this, MessageBoxButton.OK, new MessageBoxXConfigurations() { WindowStartupLocation = WindowStartupLocation.CenterOwner });
                    }
                });
            }
        }



        public static Dictionary<string, BitmapImage> BitmapImageList = new Dictionary<string, BitmapImage>();

        private void SetOnlineImageAsBackground(string modImgUrl, object owner)
        {
            Dispatcher.Invoke(async () =>
            {
                var grid = (Grid)owner;
                try
                {
                    var bitmap = BitmapImageList.FirstOrDefault(x => x.Key == modImgUrl);
                    if (bitmap.Value == null)
                    {
                        using (var client = new HttpClient())
                        {
                            var imageStream = await client.GetStreamAsync(modImgUrl);
                            BitmapImage bitmapImage = new BitmapImage();
                            bitmapImage.BeginInit();
                            bitmapImage.StreamSource = imageStream;
                            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                            bitmapImage.EndInit();
                            BitmapImageList.Add(modImgUrl, bitmapImage);
                            ImageBrush imageBrush = new ImageBrush(bitmapImage);
                            imageBrush.Stretch = Stretch.Fill;
                            grid.Background = imageBrush;
                        }
                    }
                    else
                    {
                        ImageBrush imageBrush = new ImageBrush(bitmap.Value);
                        imageBrush.Stretch = Stretch.Fill;
                        grid.Background = imageBrush;
                    }
                }
                catch (Exception)
                {
                }
            });
        }


        public static string CalculateMD5Checksum(string filePath)
        {
            try
            {
                // 创建一个MD5算法实例  
                using (MD5 md5 = MD5.Create())
                {
                    // 打开文件  
                    using (FileStream stream = File.OpenRead(filePath))
                    {
                        // 计算文件的MD5校验和  
                        byte[] hash = md5.ComputeHash(stream);

                        // 将字节转换为十六进制字符串  
                        StringBuilder sb = new StringBuilder(hash.Length * 2);
                        foreach (byte b in hash)
                        {
                            sb.Append(b.ToString("x2"));
                        }
                        // 返回校验和字符串  
                        return sb.ToString();
                    }
                }
            }
            catch (Exception)
            {
                return "";
            }
        }

    

     

        public static void ExtractWithOverwrite(string zipFilePath, string destinationDirectory)
        {
            // 确保目标目录存在
            Directory.CreateDirectory(destinationDirectory);
            // 获取 zip 文件中的条目
            using (var zip = ZipFile.OpenRead(zipFilePath))
            {
                foreach (var entry in zip.Entries)
                {
                    // 获取目标文件路径
                    string destinationPath = Path.GetFullPath(Path.Combine(destinationDirectory, entry.FullName));
                    // 如果目标文件存在，则删除
                    if (File.Exists(destinationPath))
                    {
                        try
                        {
                            File.Delete(destinationPath);
                        }
                        catch (Exception)
                        {

                        }
                    }
                    // 提取文件
                    try
                    {
                        entry.ExtractToFile(destinationPath, true);
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }

        private void DownLoadPatchFile(object PatchInfo)
        {
            var moder = (DownloadPatchThread)PatchInfo;
            var pender = moder.PendingHandler;
            try
            {
                if (!Directory.Exists(client.ShippingPath + "\\Patches"))
                {
                    Directory.CreateDirectory(client.ShippingPath + "\\Patches");
                }

                using (WebClient client1 = new WebClient())
                {
                    client1.DownloadFile(moder.PatchContent.PatchUrl, client.ShippingPath + "\\Patches\\" + moder.PatchContent.PatchFileName);
                }
                Dispatcher.Invoke(() =>
                {
                    pender.Close();
                    MessageBoxX.Show("", "Installation successful Restart the game to take effect", this, MessageBoxButton.OK, new MessageBoxXConfigurations() { WindowStartupLocation = WindowStartupLocation.CenterOwner });

                });
            }
            catch (Exception)
            {
                Dispatcher.Invoke(() =>
                {
                    pender.Close();
                    MessageBoxX.Show("", "Download failed. Please try again.", this, MessageBoxButton.OK, new MessageBoxXConfigurations() { WindowStartupLocation = WindowStartupLocation.CenterOwner });
                });
            }
            Thread th = new Thread(FlushPatch);
            th.IsBackground = true;
            th.Start();
        }





        private void DownLoadFile(object ModInfo)
        {
            var moder = (ModContent)ModInfo;
            if (Directory.Exists(client.GameModsPath))
            {
                string modInstallPath = Path.Combine(client.GameModsPath, client.ModInstallPath);

                if (!Directory.Exists(modInstallPath))
                {
                    Directory.CreateDirectory(modInstallPath);
                }
                DownLoadFile(moder.ModPakUrl, Path.Combine(modInstallPath, moder.ModPakFileName));
                DownLoadFileWithOutProcess(moder.ModSigUrl, Path.Combine(modInstallPath, moder.ModSigFileName));
            }
        }





   
        private bool DeletePatch(PatchContent patchContent)
        {
            try
            {
                var patchfile = Directory.GetFiles(client.ShippingPath, patchContent.PatchFileName, SearchOption.AllDirectories);
                foreach (var item in patchfile)
                {
                    File.Delete(item);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


        private bool DeleteFile(ModContent modInfo)
        {
            try
            {
                var pakFile = Directory.GetFiles(client.GameModsPath, modInfo.ModPakFileName, SearchOption.AllDirectories);
                foreach (var item in pakFile)
                {
                    File.Delete(item);
                }
                var SigFile = Directory.GetFiles(client.GameModsPath, modInfo.ModSigFileName, SearchOption.AllDirectories);
                foreach (var item in SigFile)
                {
                    File.Delete(item);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }





    }
}