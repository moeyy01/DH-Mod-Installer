using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace DreadHunger
{
    public partial class MainWindow
    {

        private VersionResponse GetVersion()
        {
            HttpHelper http = new HttpHelper();
            HttpItem item = new HttpItem()
            {
                URL = GetUrlByChannelName("get_version"),
                Method = "GET",
                Encoding = Encoding.UTF8,
                PostEncoding = Encoding.UTF8,
                ContentType = "application/x-www-form-urlencoded"
            };
            HttpResult result = http.GetHtml(item);
            try
            {
                return JsonConvert.DeserializeObject<VersionResponse>(result.Html);
            }
            catch (Exception)
            {
                return null;
            }
        }


        private List<PatchContent> GetPatchList()
        {
            HttpHelper http = new HttpHelper();
            HttpItem item = new HttpItem()
            {
                URL = GetUrlByChannelName("get_patch_list"),
                Method = "GET",
                Encoding = Encoding.UTF8,
                PostEncoding = Encoding.UTF8,
                ContentType = "application/x-www-form-urlencoded"
            };
            HttpResult result = http.GetHtml(item);
            try
            {
                return JsonConvert.DeserializeObject<List<PatchContent>>(result.Html);
            }
            catch (Exception)
            {
                return null;
            }
        }
               

        public static string ModResponse = null;
        private List<ModContent> GetModList()
        {
            if (string.IsNullOrWhiteSpace(ModResponse))
            {
                HttpHelper http = new HttpHelper();
                HttpItem item = new HttpItem()
                {
                    URL = GetUrlByChannelName("get_mod_list_v2"),
                    Method = "GET",
                    Encoding = Encoding.UTF8,
                    PostEncoding = Encoding.UTF8,
                    ContentType = "application/x-www-form-urlencoded"
                };
                HttpResult result = http.GetHtml(item);
                try
                {
                    var res = JsonConvert.DeserializeObject<List<ModContent>>(result.Html);
                    ModResponse = result.Html;
                    return res;
                }
                catch (Exception)
                {
                    return null;
                }
            }
            else
            {
                return JsonConvert.DeserializeObject<List<ModContent>>(ModResponse);
            }
        }
}
}
