using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using WAIUA.Properties;

namespace WAIUA.ValAPI
{
    public class ValAPI
    {
        private static List<Urls> _urls;

        public static readonly Dictionary<string, string> ValApiLanguages = new()
        {
            {"ar", "ar-AE"},
            {"de", "de-DE"},
            {"en", "en-US"},
            {"es", "es-ES"},
            {"fr", "fr-FR"},
            {"id", "id-ID"},
            {"it", "it-IT"},
            {"ja", "ja-JP"},
            {"ko", "ko-KR"},
            {"pl", "pl-PL"},
            {"pt", "pt-BR"},
            {"ru", "ru-RU"},
            {"th", "th-TH"},
            {"tr", "tr-TR"},
            {"vi", "vi-VN"},
            {"zh", "zh-CN"},
            {"hi", "en-US"}
        };

        private static async Task<string> GetValApiVersion()
        {
            RestClient client = new(new Uri("https://valorant-api.com/v1/version"));
            RestRequest request = new(Method.GET);
            var response = await client.ExecuteGetAsync(request);
            var content = response.Content;
            var responsevar = JsonConvert.DeserializeObject(content);
            JToken responseObj = JObject.FromObject(responsevar);
            return responseObj["data"]["version"].Value<string>();
        }

        private static async Task<string> GetLocalValApiVersion(string currentPath)
        {
            string content;
            using (var r = new StreamReader(currentPath + "\\ValAPI\\version.json"))
            {
                content = await r.ReadToEndAsync();
            }

            var responsevar = JsonConvert.DeserializeObject(content);
            JToken responseObj = JObject.FromObject(responsevar);
            if (responseObj != null) return responseObj["data"]["version"].Value<string>();
            return "";
        }

        private static Task GetUrls(string currentPath)
        {
            var language = ValApiLanguages.GetValueOrDefault(Settings.Default.Language);
            _urls = new List<Urls>
            {
                new()
                {
                    Filepath = currentPath + "\\ValAPI\\version.json",
                    Url = "https://valorant-api.com/v1/version"
                },
                new()
                {
                    Filepath = currentPath + "\\ValAPI\\competitivetiers.json",
                    Url = $"https://valorant-api.com/v1/competitivetiers?language={language}"
                },
                new()
                {
                    Filepath = currentPath + "\\ValAPI\\playercards.json",
                    Url = "https://valorant-api.com/v1/playercards"
                },
                new()
                {
                    Filepath = currentPath + "\\ValAPI\\skinchromas.json",
                    Url = $"https://valorant-api.com/v1/weapons/skinchromas?language={language}"
                },
                new()
                {
                    Filepath = currentPath + "\\ValAPI\\agents.json",
                    Url = $"https://valorant-api.com/v1/agents?language={language}"
                }
            };
            return Task.CompletedTask;
        }

        public static async Task UpdateJson(string currentPath)
        {
            try
            {
                await GetUrls(currentPath);
                if (!Directory.Exists(currentPath + "\\ValAPI"))
                    Directory.CreateDirectory(currentPath + "\\ValAPI");

                var tasks = _urls.Select(async url =>
                {
                    RestClient client = new(new Uri(url.Url));
                    RestRequest request = new(Method.GET);
                    var response = await client.ExecuteGetAsync(request);
                    var content = response.Content;
                    await File.WriteAllTextAsync(url.Filepath, content);
                });
                await Task.WhenAll(tasks);

                var content = await LoadJsonFromFile("\\ValAPI\\competitivetiers.json");
                var content2 = await LoadJsonFromFile("\\ValAPI\\agents.json");

                if (!Directory.Exists(currentPath + "\\ValAPI\\agentsimg"))
                    Directory.CreateDirectory(currentPath + "\\ValAPI\\agentsimg");
                foreach (var agent in content2.data)
                {
                    string uuid = agent.uuid;
                    var client = new WebClient();
                    string url = agent.killfeedPortrait;
                    Uri uri = new(url);
                    var fileName = currentPath + $"\\ValAPI\\agentsimg\\{uuid}.png";
                    client.DownloadFile(uri, fileName);
                }

                if (!Directory.Exists(currentPath + "\\ValAPI\\ranksimg"))
                    Directory.CreateDirectory(currentPath + "\\ValAPI\\ranksimg");
                foreach (var rank in content.data[3].tiers)
                {
                    int currentrank = rank.tier;
                    string url2;
                    var client2 = new WebClient();
                    if (currentrank == 0)
                        url2 = rank.smallIcon;
                    else if (currentrank is 1 or 2)
                        continue;
                    else
                        url2 = rank.largeIcon;
                    Uri uri2 = new(url2);
                    var fileName = currentPath + $"\\ValAPI\\ranksimg\\{currentrank}.png";
                    client2.DownloadFile(uri2, fileName);
                }
            }
            catch (Exception)
            {
            }
        }

        public static async Task<dynamic> LoadJsonFromFile(string filepath)
        {
            var currentPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\WAIUA";
            string content;
            using (var r = new StreamReader(currentPath + filepath))
            {
                content = await r.ReadToEndAsync();
            }

            dynamic response = JsonConvert.DeserializeObject(content);
            return response;
        }

        public static async Task CheckAndUpdateJson()
        {
            try
            {
                var currentPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\WAIUA";
                await GetUrls(currentPath);
                foreach (var path in _urls)
                    if (!File.Exists(path.Filepath))
                        await UpdateJson(currentPath);
                if (await GetValApiVersion() != await GetLocalValApiVersion(currentPath)) await UpdateJson(currentPath);
            }
            catch (Exception)
            {
            }
        }

        private class Urls
        {
            public string Filepath { get; init; }
            public string Url { get; init; }
        }
    }
}