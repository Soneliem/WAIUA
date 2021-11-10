using System;
using System.Collections.Generic;
using System.Diagnostics;
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
		private static List<Ulrs> _urls;

		private class Ulrs
		{
			public string Filepath { get; init; }
			public string Url { get; init; }
		}

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
			string content = response.Content;
			var responsevar = JsonConvert.DeserializeObject(content);
			JToken responseObj = JObject.FromObject(responsevar);
			return responseObj["data"]["version"].Value<string>();
		}

		private static async Task<string> GetLocalValApiVersion()
		{
			string content;
			using (StreamReader r = new StreamReader("ValAPI/version.json"))
			{
				content = await r.ReadToEndAsync();
			}

			var responsevar = JsonConvert.DeserializeObject(content);
			JToken responseObj = JObject.FromObject(responsevar);
			return responseObj["data"]["version"].Value<string>();
		}

		private static Task GetUrls()
        {
            String Language = ValApiLanguages.GetValueOrDefault(Properties.Settings.Default.Language);
			_urls = new List<Ulrs>
			{
				new Ulrs {Filepath = "ValApi/version.json", Url = "https://valorant-api.com/v1/version"},
				new Ulrs
				{
					Filepath = "ValApi/competitivetiers.json",
					Url = $"https://valorant-api.com/v1/competitivetiers?language={Language}"
				},
				new Ulrs {Filepath = "ValApi/playercards.json", Url = "https://valorant-api.com/v1/playercards"},
				new Ulrs
				{
					Filepath = "ValApi/skinchromas.json",
					Url = $"https://valorant-api.com/v1/weapons/skinchromas?language={Language}"
				},
				new Ulrs
				{
					Filepath = "ValApi/agents.json", Url = $"https://valorant-api.com/v1/agents?language={Language}"
				}
			};
            return Task.CompletedTask;
        }

        public static async Task UpdateJson()
		{
			await GetUrls();
			var tasks = _urls.Select(async url =>
			{
				RestClient client = new(new Uri(url.Url));
				RestRequest request = new(Method.GET);
				var response = await client.ExecuteGetAsync(request);
				string content = response.Content;
				await File.WriteAllTextAsync(url.Filepath, content);
			});
			await Task.WhenAll(tasks);
		}

		public static async Task<dynamic> LoadJsonFromFile(string filepath)
		{
			string content;
			using (StreamReader r = new StreamReader(filepath))
			{
				content = await r.ReadToEndAsync();
			}

			dynamic response = JsonConvert.DeserializeObject(content);
			return response;
		}

		public static async Task CheckAndUpdateJson()
		{
			if (await GetValApiVersion() != await GetLocalValApiVersion())
			{
				await UpdateJson();
			}
		}

		public static async Task VerifyJson()
		{
			if (await GetValApiVersion() != await GetLocalValApiVersion())
			{
				await UpdateJson();
			}
		}
	}
}