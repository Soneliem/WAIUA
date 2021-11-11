using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace WAIUA.ValAPI
{
	public class ValAPI
	{
		private static List<Urls> _urls;

		private class Urls
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
			_urls = new List<Urls>
			{
				new Urls {Filepath = "ValApi/version.json", Url = "https://valorant-api.com/v1/version"},
				new Urls
				{
					Filepath = "ValApi/competitivetiers.json",
					Url = $"https://valorant-api.com/v1/competitivetiers?language={Language}"
				},
				new Urls {Filepath = "ValApi/playercards.json", Url = "https://valorant-api.com/v1/playercards"},
				new Urls
				{
					Filepath = "ValApi/skinchromas.json",
					Url = $"https://valorant-api.com/v1/weapons/skinchromas?language={Language}"
				},
				new Urls
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

			var task1 = LoadJsonFromFile("ValAPI/competitivetiers.json");
			var task2 = LoadJsonFromFile("ValAPI/agents.json");
			await Task.WhenAll(task1, task2);
			dynamic content = task1.Result;
			dynamic content2 = task2.Result;

			foreach (var agent in content2.data)
			{
				string uuid = agent.uuid;
				WebClient client = new WebClient();
				string url = agent.killfeedPortrait;
				Uri uri = new(url);
				string fileName = string.Format("ValAPI/agentsimg/{0}.png", uuid);
				client.DownloadFile(uri, fileName);
			}

			foreach (var rank in content.data[3].tiers)
			{
				int currentrank = rank.tier;
				if (currentrank == 1 || currentrank == 2) { continue; }
				WebClient client2 = new WebClient();
				string url2 = rank.largeIcon;
				Uri uri2 = new(url2);
				string fileName = string.Format("ValAPI/ranksimg/{0}.png", currentrank);
				client2.DownloadFile(uri2, fileName);
			}
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
			await GetUrls();
			foreach (Urls path in _urls)
			{
				if (!File.Exists(path.Filepath))
				{
					await UpdateJson();
				}
			}
			if (await GetValApiVersion() != await GetLocalValApiVersion())
			{
				await UpdateJson();
			}
		}
	}
}