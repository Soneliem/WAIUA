using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using WAIUA.Properties;

namespace WAIUA.Helpers;

public static class ValAPI
{
	private static List<Urls> _urlsTxt;

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

	private static async Task<string> GetValApiVersionAsync()
	{
		RestClient client = new(new Uri("https://valorant-api.com/v1/version"));
		RestRequest request = new(Method.GET);
		var response = await client.ExecuteGetAsync(request).ConfigureAwait(false);
		var content = response.Content;
		var responsevar = JsonConvert.DeserializeObject(content);
		if (responsevar == null) return "";
		JToken responseObj = JObject.FromObject(responsevar);
		return responseObj["data"]["buildDate"].Value<string>();
	}

	private static async Task<string> GetLocalValApiVersionAsync()
	{
        string[] lines = await File.ReadAllLinesAsync(Constants.CurrentPath + "\\ValAPI\\version.txt").ConfigureAwait(false);
		return lines[1];
	}

	private static Task GetUrlsAsync()
	{
		var language = ValApiLanguages.GetValueOrDefault(Settings.Default.Language);
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
			},
			new()
			{
				Name = "Skins",
				Filepath = Constants.CurrentPath + "\\ValAPI\\skinchromas.txt",
				Url = $"https://valorant-api.com/v1/weapons/skinchromas?language={language}"
			},
			new()
			{
				Name = "Ranks",
				Filepath = Constants.CurrentPath + "\\ValAPI\\competitivetiers.txt",
				Url = $"https://valorant-api.com/v1/competitivetiers?language={language}"
			},
			new()
			{
				Name = "Version",
				Filepath = Constants.CurrentPath + "\\ValAPI\\version.txt",
				Url = "https://valorant-api.com/v1/version"
			}
		};
		return Task.CompletedTask;
	}

	public static async Task UpdateFilesAsync()
	{
		try
		{
			var client = new RestClient();

			await GetUrlsAsync().ConfigureAwait(false);
			if (!Directory.Exists(Constants.CurrentPath + "\\ValAPI"))
				Directory.CreateDirectory(Constants.CurrentPath + "\\ValAPI");

			var txtTasks = _urlsTxt.Select(async url =>
			{
				client.BaseUrl = new Uri(url.Url);
				RestRequest request = new(Method.GET);
				var response = await client.ExecuteGetAsync(request).ConfigureAwait(false);
				if (response.IsSuccessful)
				{
					Dictionary<string, string[]> valuesConcurrentDictionary = new();
					dynamic content = JsonConvert.DeserializeObject(response.Content);
					if (content != null)
					{
                        if (url.Name != "Version")
                        {
                            foreach (var entry in content.data)
                                switch (url.Name)
                                {
                                    case "Maps":
                                        valuesConcurrentDictionary.TryAdd((string) entry.mapUrl,
                                            new[] {(string) entry.displayName, (string) entry.listViewIcon});
                                        break;
                                    case "Agents":
                                        valuesConcurrentDictionary.TryAdd((string) entry.uuid,
                                            new[] {(string) entry.displayName, (string) entry.killfeedPortrait});
                                        break;
                                    case "PlayerCards":
                                        valuesConcurrentDictionary.TryAdd((string) entry.uuid,
                                            new[] {(string) entry.smallArt});
                                        break;
                                    case "Skins":
                                        valuesConcurrentDictionary.TryAdd((string) entry.uuid,
                                            new[] {(string) entry.displayName, (string) entry.displayIcon});
                                        break;
                                }

                            if (url.Name == "Ranks")
                                foreach (var entry in content.data[3].tiers)
                                {
                                    valuesConcurrentDictionary.TryAdd((string) entry.tier, new[] {(string) entry.tierName, (string) entry.largeIcon});
                                }

                            await File.WriteAllTextAsync(url.Filepath,
                                JsonConvert.SerializeObject(valuesConcurrentDictionary)).ConfigureAwait(false);
                        }
                        else
                        {
                            string[] lines =
                            {
                                (string)content["data"]["riotClientVersion"], (string)content["data"]["buildDate"]
                            };
							await File.WriteAllLinesAsync(url.Filepath, lines).ConfigureAwait(false);
                        }
                    }
				}
			});
            await Task.WhenAll(txtTasks).ConfigureAwait(false);

			var content = await LoadDictionaryFromFileAsync("\\ValAPI\\competitivetiers.txt").ConfigureAwait(false);
			var content2 = await LoadDictionaryFromFileAsync("\\ValAPI\\agents.txt").ConfigureAwait(false);

			if (!Directory.Exists(Constants.CurrentPath + "\\ValAPI\\agentsimg"))
				Directory.CreateDirectory(Constants.CurrentPath + "\\ValAPI\\agentsimg");
			foreach (var agent in content2)
			{
				client.BaseUrl = new Uri(agent.Value[1]);
				RestRequest request = new(Method.GET);
				var fileName = Constants.CurrentPath + $"\\ValAPI\\agentsimg\\{agent.Key}.png";
				var response = client.DownloadData(request);
				if (response.Length > 0) await File.WriteAllBytesAsync(fileName, response).ConfigureAwait(false);
			}

			if (!Directory.Exists(Constants.CurrentPath + "\\ValAPI\\ranksimg"))
				Directory.CreateDirectory(Constants.CurrentPath + "\\ValAPI\\ranksimg");
			foreach (var rank in content)
			{
				string url2;

				if (rank.Key == "0")
				{
					File.Copy(Directory.GetCurrentDirectory() +"\\Assets\\0.png", Constants.CurrentPath + "\\ValAPI\\ranksimg\\0.png");
					continue;
				}

				if (rank.Key is "1" or "2")
					continue;
				url2 = rank.Value[1];

				client.BaseUrl = new Uri(url2);
				var fileName = Constants.CurrentPath + $"\\ValAPI\\ranksimg\\{rank.Key}.png";

				RestRequest request = new(Method.GET);
				var response = client.DownloadData(request);
				if (response.Length > 0) await File.WriteAllBytesAsync(fileName, response).ConfigureAwait(false);
			}
		}
		catch (Exception)
		{
		}
	}

    public static async Task<Dictionary<string, string[]>> LoadDictionaryFromFileAsync(string filepath)
	{
		var currentPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\WAIUA";
		var dictionary = JsonConvert.DeserializeObject<Dictionary<string, string[]>>
			(await File.ReadAllTextAsync(currentPath + filepath).ConfigureAwait(false));
		return dictionary;
	}

	public static async Task CheckAndUpdateJsonAsync()
	{
		try
		{
			await GetUrlsAsync().ConfigureAwait(false);

			if (await GetValApiVersionAsync().ConfigureAwait(false) != await GetLocalValApiVersionAsync().ConfigureAwait(false))
			{
				await UpdateFilesAsync().ConfigureAwait(false);
				return;
			}

			foreach (var path in _urlsTxt)
				if (!File.Exists(path.Filepath))
				{
					await UpdateFilesAsync().ConfigureAwait(false);
					break;
				}
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