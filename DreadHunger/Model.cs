using Panuon.UI.Silver.Core;
using System;
using System.Collections.Generic;

namespace DreadHunger
{
    public class LastCheckBox
    {
        public string MapName { get; set; }
        public string PlayName { get; set; }
        public List<String> CheckBoxName { get; set; }
    }

    public class WriteFileInfo
    {
        public string FileName { get; set; }
        public string Content { get; set; }
        public bool IsEncrypt { get; set; }
    }


    public class UpdateInfo
    {
        public int Id { get; set; }
        public string Version { get; set; }
        public string ExePath { get; set; }
        public string Download { get; set; }
    }
    public class VersionResponse
    {
        public string Version { get; set; }
        public string Download { get; set; }
    }

    class Response
    {
        public int Code { get; set; }
        public string Message { get; set; }
    }

    class SetCardResponse
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public int RemainingTime { get; set; }
    }

    public class PatchContent
    {
        public string PatchName { get; set; }   //补丁中文名字
        public string PatchNameEn { get; set; }
        public int PatchType { get; set; }
        public string PatchVersion { get; set; }
        public string PatchIntroduce { get; set; } //补丁中文介绍
        public string PatchIntroduceEn { get; set; }
        public string PatchVideoUrl { get; set; } 
        public string PatchImgUrl { get; set; }    //一张正方形图片
        public string PatchFileName { get; set; }  //补丁文件名                     
        public string PatchUrl { get; set; }   //补丁文件
        public string PatchJsMd5 { get; set; }
    }

    class PatchInfo
    {
        public string FileFullName { get; set; }
        public bool IsInstall { get; set; }
        public bool IsNeedUpdate { get; set; }
        public PatchContent Patch { get; set; }
    }

    public class ModContent
    {
        public string ModName { get; set; }   //Mod中文名字
        public string ModNameEn { get; set; }
        public int ModType { get; set; }      //Mod类型  玩法，材质包
        public string ModVersion { get; set; }
        public string ModIntroduce { get; set; } //Mod中文介绍
        public string ModIntroduceEn { get; set; }
        public string ModImgUrl { get; set; }    //一张正方形图片
        public string ModVideoUrl { get; set; }  //视频素材 15秒左右
        public string ModSigFileName { get; set; }
        public string ModPakFileName { get; set; }
        public string ModVideoName { get; set; }
        public string ModPakMd5 { get; set; }
        public string ModPakUrl { get; set; }   //Mod文件
        public string ModSigUrl { get; set; }
    }


    class ModInfo
    {
        public string FileFullName { get; set; }
        public bool IsInstall { get; set; }
        public bool IsNeedUpdate { get; set; }
        public ModContent Mod { get; set; }
    }

    class PatchButtonTag
    {
        public string ButtonType { get; set; }
        public PatchContent PatchContent { get; set; }
    }

    class JoinGameInfo
    {
        public IPendingHandler Pender { get; set; }
        public String IpAndPort { get; set; }
    }


    class VideoFile
    {
        public string ModIntroduce { get; set; }
        public string ModVideoName { get; set; }
        public string ModVideoUrl { get; set; }
    }


    class DownloadPatchThread
    {
        public IPendingHandler PendingHandler { get; set; }
        public PatchContent PatchContent { get; set; }
    }

    class BindingSteamClass
    {
        public string SteamId { get; set; }
        public IPendingHandler PendingHandler { get; set; }
    }

    public class CreatMessage
    {
        public bool IsNewVersion { get; set; }
        public string Card { get; set; }
        public string Map { get; set; }
        public bool IsSingle { get; set; }
    }

    public class Createthread
    {
        public string Json { get; set; }
        public IPendingHandler Pending { get; set; }
    }






}
