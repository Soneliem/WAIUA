using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WAIUA.Properties;
using static WAIUA.Helpers.ValAPI;

namespace WAIUA.Helpers
{
    public static class ValAPI
    {
        private static List<Urls> _urlsJson;
        private static List<Urls> _urlsTxt;

        public static readonly Dictionary<string, string> ValApiLanguages = new()
        {
            { "ar", "ar-AE" },
            { "de", "de-DE" },
            { "en", "en-US" },
            { "es", "es-ES" },
            { "fr", "fr-FR" },
            { "id", "id-ID" },
            { "it", "it-IT" },
            { "ja", "ja-JP" },
            { "ko", "ko-KR" },
            { "pl", "pl-PL" },
            { "pt", "pt-BR" },
            { "ru", "ru-RU" },
            { "th", "th-TH" },
            { "tr", "tr-TR" },
            { "vi", "vi-VN" },
            { "zh", "zh-CN" },
            { "hi", "en-US" }
        };

        private static async Task<string> GetValApiVersion()
        {
            RestClient client = new(new Uri("https://valorant-api.com/v1/version"));
            RestRequest request = new(Method.GET);
            var response = await client.ExecuteGetAsync(request);
            var content = response.Content;
            var responsevar = JsonConvert.DeserializeObject(content);
            if (responsevar == null) return "";
            JToken responseObj = JObject.FromObject(responsevar);
            return responseObj["data"]["version"].Value<string>();
        }

        private static async Task<string> GetLocalValApiVersion()
        {
            string content;
            using (var r = new StreamReader(Constants.CurrentPath + "\\ValAPI\\version.json"))
            {
                content = await r.ReadToEndAsync();
            }
            var responsevar = JsonConvert.DeserializeObject(content);
            if (responsevar == null) return "";
            JToken responseObj = JObject.FromObject(responsevar);
            return responseObj["data"]["version"].Value<string>();
        }

        private static Task GetUrls()
        {
            var language = ValApiLanguages.GetValueOrDefault(Settings.Default.Language);
            _urlsJson = new List<Urls>
            {
                new()
                {
                    Name = "Version",
                    Filepath = Constants.CurrentPath + "\\ValAPI\\version.json",
                    Url = "https://valorant-api.com/v1/version"
                },
                new()
                {
                    Name = "Ranks",
                    Filepath = Constants.CurrentPath + "\\ValAPI\\competitivetiers.json",
                    Url = $"https://valorant-api.com/v1/competitivetiers?language={language}"
                },
                new()
                {
                    Name = "Skins",
                    Filepath = Constants.CurrentPath + "\\ValAPI\\skinchromas.json",
                    Url = $"https://valorant-api.com/v1/weapons/skinchromas?language={language}"
                }

            };
            _urlsTxt = new List<Urls>
            {
                new()
                {
                    Name = "Agents",
                    Filepath = Constants.CurrentPath + "\\ValAPI\\agents.txt",
                    Url = $"https://valorant-api.com/v1/agents?language={language}"
                },
                new()
                {
                    Name = "Maps",
                    Filepath = Constants.CurrentPath + "\\ValAPI\\maps.txt",
                    Url = $"https://valorant-api.com/v1/maps?language={language}"
                },
                new()
                {
                Name = "PlayerCards",
                Filepath = Constants.CurrentPath + "\\ValAPI\\cards.txt",
                Url = "https://valorant-api.com/v1/playercards"
            }
            };
            return Task.CompletedTask;
        }

        public static async Task UpdateFiles()
        {
            try
            {
                RestClient client = new RestClient();

                await GetUrls();
                if (!Directory.Exists(Constants.CurrentPath + "\\ValAPI"))
                    Directory.CreateDirectory(Constants.CurrentPath + "\\ValAPI");

                var jsonTasks = _urlsJson.Select(async url =>
                {
                    client.BaseUrl = new Uri(url.Url);
                    RestRequest request = new(Method.GET);
                    var response = await client.ExecuteGetAsync(request);
                    if (response.IsSuccessful)
                    {
                        await File.WriteAllTextAsync(url.Filepath, response.Content);
                    }

                });
                var txtTasks = _urlsTxt.Select(async url =>
                {
                    client.BaseUrl = new Uri(url.Url);
                    RestRequest request = new(Method.GET);
                    var response = await client.ExecuteGetAsync(request);
                    if (response.IsSuccessful)
                    {

                        ConcurrentDictionary<string, string[]> valuesConcurrentDictionary = new();
                        dynamic content = JsonConvert.DeserializeObject(response.Content);
                        if (content != null)
                        {
                            foreach (var entry in content.data)
                            {
                                switch (url.Name)
                                {
                                    case "Maps":
                                        valuesConcurrentDictionary.TryAdd((string)entry.mapUrl, new[] { (string)entry.displayName, (string)entry.listViewIcon });
                                        break;
                                    case "Agents":
                                        valuesConcurrentDictionary.TryAdd((string)entry.uuid, new[] { (string)entry.displayName, (string)entry.killfeedPortrait });
                                        break;
                                    case "PlayerCards":
                                        valuesConcurrentDictionary.TryAdd((string)entry.uuid, new[] { (string)entry.smallArt });
                                        break;
                                }

                            }
                            await File.WriteAllTextAsync(url.Filepath, JsonConvert.SerializeObject(valuesConcurrentDictionary));
                        }


                    }

                });

                await Task.WhenAll(jsonTasks);
                await Task.WhenAll(txtTasks);

                var content = await LoadJsonFromFile("\\ValAPI\\competitivetiers.json");
                var content2 = await LoadDictionaryFromFile("\\ValAPI\\agents.txt");

                if (!Directory.Exists(Constants.CurrentPath + "\\ValAPI\\agentsimg"))
                    Directory.CreateDirectory(Constants.CurrentPath + "\\ValAPI\\agentsimg");
                foreach (var agent in content2)
                {
                    client.BaseUrl = new Uri(agent.Value[1]);
                    RestRequest request = new(Method.GET);
                    var fileName = Constants.CurrentPath + $"\\ValAPI\\agentsimg\\{agent.Key}.png";
                    byte[] response = client.DownloadData(request);
                    if (response.Length > 0)
                    {
                        await File.WriteAllBytesAsync(fileName, response);
                    }
                }

                if (!Directory.Exists(Constants.CurrentPath + "\\ValAPI\\ranksimg"))
                    Directory.CreateDirectory(Constants.CurrentPath + "\\ValAPI\\ranksimg");
                foreach (var rank in content.data[3].tiers)
                {
                    int currentrank = rank.tier;
                    string url2;

                    if (currentrank == 0)
                        url2 = rank.smallIcon;
                    else if (currentrank is 1 or 2)
                        continue;
                    else
                        url2 = rank.largeIcon;

                    client.BaseUrl = new(url2);
                    var fileName = Constants.CurrentPath + $"\\ValAPI\\ranksimg\\{currentrank}.png";

                    RestRequest request = new(Method.GET);
                    byte[] response = client.DownloadData(request);
                    if (response.Length > 0)
                    {
                        await File.WriteAllBytesAsync(fileName, response);
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        public static async Task<dynamic> LoadJsonFromFile(string filepath)
        {

            string content;
            using (var r = new StreamReader(Constants.CurrentPath + filepath))
            {
                content = await r.ReadToEndAsync();
            }

            dynamic response = JsonConvert.DeserializeObject(content);
            return response;
        }

        public static async Task<ConcurrentDictionary<string, string[]>> LoadDictionaryFromFile(string filepath)
        {
            var currentPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\WAIUA";
            var dictionary = JsonConvert.DeserializeObject<ConcurrentDictionary<string, string[]>>
                (await File.ReadAllTextAsync(currentPath + filepath));
            return dictionary;
        }

        public static async Task CheckAndUpdateJson()
        {
            try
            {
                await GetUrls();
                foreach (var path in _urlsJson)
                    if (!File.Exists(path.Filepath))
                    {
                        await UpdateFiles();
                        break;
                    }
                foreach (var path in _urlsTxt)
                    if (!File.Exists(path.Filepath))
                    {
                        await UpdateFiles();
                        break;
                    }


                if (await GetValApiVersion() != await GetLocalValApiVersion()) await UpdateFiles();
            }
            catch (Exception)
            {
            }
        }

        private class Urls
        {
            public string Name { get; init; }
            public string Filepath { get; init; }
            public string Url { get; init; }
        }
    }
}