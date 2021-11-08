using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace WAIUA.ValAPI
{
    public class ValAPI
    {
        private static string Language { get; set; }
        private static List<ULRS> _urls;

        private class ULRS
        {
            public string filepath { get; set; }
            public string url { get; set; }
        }

        private static async Task<string> GetValApiVersion()
        {
            RestClient client = new(  new Uri("https://valorant-api.com/v1/version"));
            RestRequest request = new(Method.GET);
            var response = await client.ExecuteGetAsync(request);
            string content = response.Content;
            var responsevar = JsonConvert.DeserializeObject(content);
            JToken responseObj = JObject.FromObject(responsevar);
            return responseObj["data"]["version"].Value<string>();
        }

        private static async Task<string> GetLocalValApiVersion()
        {
            string content;
            using (StreamReader r = new StreamReader("ValApi/version.json"))
            {
                content = await r.ReadToEndAsync();
            }
            var responsevar = JsonConvert.DeserializeObject(content);
            JToken responseObj = JObject.FromObject(responsevar);
            return responseObj["data"]["version"].Value<string>();
            
        }

        private static async Task GetUrls()
        {
            Language = Models.LanguageConverter.ValApicomDictionary.GetValueOrDefault(Properties.Settings.Default.Language);
            _urls = new List<ULRS>
            {
                new ULRS {filepath = "ValApi/version.json", url = "https://valorant-api.com/v1/version"},
                new ULRS {filepath = "ValApi/competitivetiers.json", url = $"https://valorant-api.com/v1/competitivetiers?language={Language}"},
                new ULRS {filepath = "ValApi/playercards.json", url = "https://valorant-api.com/v1/playercards"},
                new ULRS {filepath = "ValApi/skinchromas.json", url = "https://valorant-api.com/v1/weapons/skinchromas"},
                new ULRS {filepath = "ValApi/agents.json", url = "https://valorant-api.com/v1/agents"}
            };
        }


        private static async Task UpdateJson()
        {
            await GetUrls();
            var tasks = _urls.Select(async url =>
            {
                RestClient client = new(  new Uri(url.url));
                RestRequest request = new(Method.GET);
                var response = await client.ExecuteGetAsync(request);
                string content = response.Content;
                await File.WriteAllTextAsync(url.filepath, content);
            });
            await Task.WhenAll(tasks);
        }

        public static async Task CheckAndUpdateJson()
        {
            if (await GetValApiVersion() != await GetLocalValApiVersion())
            {
                await UpdateJson();
            }


        }

    }
}
