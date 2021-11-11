using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static WAIUA.ValAPI.ValAPI;
using DataFormat = RestSharp.DataFormat;

namespace WAIUA.Commands
{
	public static class Dictionary
	{
		public static ConcurrentDictionary<string, string> UrlToBody = new();
	}

	public class Main
	{
		private static string AccessToken { get; set; } // To be moved to OOP
		private static string EntitlementToken { get; set; }
		private static string Region { get; set; }
		private static string Shard { get; set; }
		private static string Version { get; set; }
		private static string Language { get; set; }
		public static string PPUUID { get; private set; }
		private static string Matchid { get; set; }
		private static string Port { get; set; }
		private static string LPassword { get; set; }
		private static string CurrentSeason { get; set; }
		private static string PSeason { get; set; }
		private static string PPSeason { get; set; }
		private static string PPPSeason { get; set; }
		private static string[] PlayerList { get; set; } = new string[10];
		private static string[] PUUIDList { get; set; } = new string[10];
		private static string[] AgentList { get; set; } = new string[10];
		private static string[] AgentPList { get; set; } = new string[10];
		private static string[] CardList { get; set; } = new string[10];
		private static string[] LevelList { get; set; } = new string[10];
		private static string[] RankList { get; set; } = new string[10];
		private static string[] MaxRRList { get; set; } = new string[10];
		private static string[] PRankList { get; set; } = new string[10];
		private static string[] PPRankList { get; set; } = new string[10];
		private static string[] PPPRankList { get; set; } = new string[10];
		private static string[] RankNameList { get; set; } = new string[10];
		private static string[] PRankNameList { get; set; } = new string[10];
		private static string[] PPRankNameList { get; set; } = new string[10];
		private static string[] PPPRankNameList { get; set; } = new string[10];
		private static string[] RankProgList { get; set; } = new string[10];
		private static string[] PGList { get; set; } = new string[10];
		private static string[] PPGList { get; set; } = new string[10];
		private static string[] PPPGList { get; set; } = new string[10];
		private static bool[] IsIncognito { get; set; } = new bool[10];
		private static string[] TrackerUrlList { get; set; } = new string[10];
		private static string[] TrackerEnabledList { get; set; } = new string[10];
		private static string[] TrackerDisabledList { get; set; } = new string[10];
		private static string[] VandalList { get; set; } = new string[10];
		private static string[] PhantomList { get; set; } = new string[10];
		private static string[] VandalNameList { get; set; } = new string[10];
		private static string[] PhantomNameList { get; set; } = new string[10];

		private static string
			DoCachedRequest(Method method, string url, bool addRiotAuth, bool bypassCache = false) // Thank you MitchC for this, I am always touched when random people go out of the way to help others even though they know that we would be clueless and need to ask alot of followup questions
		{
			var tsk = DoCachedRequestAsync(method, url, addRiotAuth, bypassCache);
			return tsk.Result;
		}

		private static async Task<string> DoCachedRequestAsync(Method method, string url, bool addRiotAuth,
			bool bypassCache = false)
		{
			var attemptCache = method == Method.GET && !bypassCache;
			if (attemptCache)
			{
				if (Dictionary.UrlToBody.TryGetValue(url, out var res))
				{
					return res;
				}
			}

			RestClient client = new(url);
			RestRequest request = new(method);
			if (addRiotAuth)
			{
				request.AddHeader("X-Riot-Entitlements-JWT", $"{EntitlementToken}");
				request.AddHeader("Authorization", $"Bearer {AccessToken}");
				request.AddHeader("X-Riot-ClientPlatform",
					"ew0KCSJwbGF0Zm9ybVR5cGUiOiAiUEMiLA0KCSJwbGF0Zm9ybU9TIjogIldpbmRvd3MiLA0KCSJwbGF0Zm9ybU9TVmVyc2lvbiI6ICIxMC4wLjE5MDQyLjEuMjU2LjY0Yml0IiwNCgkicGxhdGZvcm1DaGlwc2V0IjogIlVua25vd24iDQp9");
				request.AddHeader("X-Riot-ClientVersion", $"{Version}");
			}

			var resp = await client.ExecuteAsync(request);
			if (resp.IsSuccessful && resp.RawBytes != null)
			{
				var cont = resp.Content;

				if (attemptCache) Dictionary.UrlToBody.TryAdd(url, cont);
				return cont;
			}
			else
			{
				return null;
			}
		}

		public static bool GetSetPPUUID()
		{
			RestClient client = new(new Uri("https://auth.riotgames.com/userinfo"));
			RestRequest request = new(Method.POST);
			request.AddHeader("Authorization", $"Bearer {AccessToken}");
			request.AddJsonBody("{}");
			var response = client.Execute(request);
			HttpStatusCode statusCode = response.StatusCode;
			short numericStatusCode = (short)statusCode;
			if (numericStatusCode != 200)
			{
				return false;
			}

			string content = response.Content;
			var PlayerInfo = JsonConvert.DeserializeObject(content);
			JToken PUUIDObj = JObject.FromObject(PlayerInfo);
			PPUUID = PUUIDObj["sub"].Value<string>();
			return true;
		}

		public static bool CheckLocal()
		{
			var lockfileLocation =
				$@"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\Riot Games\Riot Client\Config\lockfile";

			if (File.Exists(lockfileLocation))
			{
				using (FileStream fileStream = new(lockfileLocation, FileMode.Open, FileAccess.ReadWrite,
					FileShare.ReadWrite))
				using (StreamReader sr = new(fileStream))
				{
					string[] parts = sr.ReadToEnd().Split(":");
					Port = parts[2];
					LPassword = parts[3];
				}

				return true;
			}

			return false;
		}

		public static void LocalLogin()
		{
			GetLatestVersion();
			RestClient client = new(new Uri($"https://127.0.0.1:{Port}/entitlements/v1/token"));
			RestRequest request = new(Method.GET);
			client.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
			request.AddHeader("Authorization",
				$"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes($"riot:{LPassword}"))}");
			request.AddHeader("X-Riot-ClientPlatform",
				"ew0KCSJwbGF0Zm9ybVR5cGUiOiAiUEMiLA0KCSJwbGF0Zm9ybU9TIjogIldpbmRvd3MiLA0KCSJwbGF0Zm9ybU9TVmVyc2lvbiI6ICIxMC4wLjE5MDQyLjEuMjU2LjY0Yml0IiwNCgkicGxhdGZvcm1DaGlwc2V0IjogIlVua25vd24iDQp9");
			request.AddHeader("X-Riot-ClientVersion", $"{Version}");
			request.RequestFormat = DataFormat.Json;
			var response = client.Get(request);
			string content = client.Execute(request).Content;
			var responsevar = JsonConvert.DeserializeObject(content);
			JToken responseObj = JObject.FromObject(responsevar);
			AccessToken = responseObj["accessToken"].Value<string>();
			EntitlementToken = responseObj["token"].Value<string>();
		}

		public static void LocalRegion()
		{
			RestClient client = new(new Uri($"https://127.0.0.1:{Port}/product-session/v1/external-sessions"));
			RestRequest request = new(Method.GET);
			client.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
			request.AddHeader("Authorization",
				$"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes($"riot:{LPassword}"))}");
			request.AddHeader("X-Riot-ClientPlatform",
				"ew0KCSJwbGF0Zm9ybVR5cGUiOiAiUEMiLA0KCSJwbGF0Zm9ybU9TIjogIldpbmRvd3MiLA0KCSJwbGF0Zm9ybU9TVmVyc2lvbiI6ICIxMC4wLjE5MDQyLjEuMjU2LjY0Yml0IiwNCgkicGxhdGZvcm1DaGlwc2V0IjogIlVua25vd24iDQp9");
			request.AddHeader("X-Riot-ClientVersion", $"{Version}");
			request.RequestFormat = DataFormat.Json;
			var response = client.Get(request);
			string content = client.Execute(request).Content;
			JObject root = JObject.Parse(content);
			JProperty property = (JProperty)root.First;
			var fullstring = (property.Value["launchConfiguration"]["arguments"][3]);
			string[] parts = fullstring.ToString().Split(new char[] { '=', '&' });
			string output = parts[1];
			if (output == "latam")
			{
				Region = "na";
				Shard = "latam";
			}
			else if (output == "br")
			{
				Region = "na";
				Shard = "br";
			}
			else
			{
				Region = output;
				Shard = output;
			}
		}

		private static void GetLatestVersion()
		{
			dynamic content = Task.Run(() => { return LoadJsonFromFile("ValAPI/version.json"); }).Result;
			Version = content.data.riotClientVersion;
		}

		public static string GetIGUsername(string puuid)
		{
			string IGN = null;
			try
			{
				string gameName = "";
				string gameTag = "";
				string url = $"https://pd.{Region}.a.pvp.net/name-service/v2/players";
				RestClient client = new(url);
				RestRequest request = new(Method.PUT)
				{
					RequestFormat = DataFormat.Json
				};

				string[] body = new String[] { puuid };
				request.AddJsonBody(body);

				var response = client.Put(request);
				string content = client.Execute(request).Content;

				content = content.Replace("[", "");
				content = content.Replace("]", "");

				var uinfo = JsonConvert.DeserializeObject(content);
				JToken uinfoObj = JObject.FromObject(uinfo);
				gameName = uinfoObj["GameName"].Value<string>();
				gameTag = uinfoObj["TagLine"].Value<string>();
				IGN = gameName + "#" + gameTag;
			}
			catch (Exception)
			{
				IGN = null;
			}

			return IGN;
		}

		private static bool LiveMatchID()
		{
			try
			{
				string url = $"https://glz-{Shard}-1.{Region}.a.pvp.net/core-game/v1/players/{PPUUID}";
				RestClient client = new(url);
				RestRequest request = new(Method.GET);
				request.AddHeader("X-Riot-Entitlements-JWT", $"{EntitlementToken}");
				request.AddHeader("Authorization", $"Bearer {AccessToken}");
				string response = client.Execute(request).Content;
				var matchinfo = JsonConvert.DeserializeObject(response);
				JToken matchinfoObj = JObject.FromObject(matchinfo);
				Matchid = matchinfoObj["MatchID"].Value<string>();
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		public bool LiveMatchChecks()
		{
			bool output;

			if (GetSetPPUUID())
			{
				if (LiveMatchID())
				{
					LiveMatchSetup();
					output = true;
				}
				else
				{
					MessageBox.Show(Properties.Resources.NoMatch, Properties.Resources.Error, MessageBoxButton.OK,
						MessageBoxImage.Question, MessageBoxResult.OK);
					output = false;
				}
			}
			else
			{
				if (CheckLocal())
				{
					LocalLogin();
					GetSetPPUUID();
					LocalRegion();

					if (LiveMatchID())
					{
						LiveMatchSetup();
						output = true;
					}
					else
					{
						MessageBox.Show(Properties.Resources.NoMatch, Properties.Resources.Error, MessageBoxButton.OK,
							MessageBoxImage.Question, MessageBoxResult.OK);
						output = false;
					}
				}
				else
				{
					MessageBox.Show(Properties.Resources.NoValGame, Properties.Resources.Error, MessageBoxButton.OK,
						MessageBoxImage.Question, MessageBoxResult.OK);
					output = false;
				}
			}

			return output;
		}

		private static void LiveMatchSetup()
		{
			Parallel.Invoke(GetSeasons, GetLatestVersion, GetLanguage);
			string url = $"https://glz-{Shard}-1.{Region}.a.pvp.net/core-game/v1/matches/{Matchid}";
			RestClient client = new(url);
			RestRequest request = new(Method.GET);
			request.AddHeader("X-Riot-Entitlements-JWT", $"{EntitlementToken}");
			request.AddHeader("Authorization", $"Bearer {AccessToken}");
			string content = client.Execute(request).Content;
			dynamic matchinfo = JsonConvert.DeserializeObject(content);
			sbyte[] playerno = new sbyte[10];
			string[] puuid = new string[10];
			string[] agent = new string[10];
			string[] card = new string[10];
			string[] level = new string[10];
			bool[] incognito = new bool[10];
			sbyte index = 0;
			foreach (var entry in matchinfo.Players)
			{
				if (entry.IsCoach == false)
				{
					playerno[index] = index;
					puuid[index] = entry.Subject;
					agent[index] = entry.CharacterID;
					card[index] = entry.PlayerIdentity.PlayerCardID;
					level[index] = entry.PlayerIdentity.AccountLevel;
					if (entry.PlayerIdentity.Incognito == true)
					{
						incognito[index] = true;
					}

					index++;
					if (index == 10)
					{
						break;
					}
				}
			}

			PUUIDList = puuid;
			AgentList = agent;
			CardList = card;
			LevelList = level;
			IsIncognito = incognito;
		}

		public string[] LiveMatchOutput(sbyte playerno)
		{
			Parallel.Invoke(
				() => GetIGCUsername(playerno),
				() => GetAgentInfo(AgentList[playerno], playerno),
				() => GetCardInfo(CardList[playerno], playerno),
				() => GetSkinInfo(playerno),
				() => GetCompHistory(PUUIDList[playerno], playerno),
				() => GetPlayerHistory(PUUIDList[playerno], playerno));

			string[] output =
			{
				AgentList[playerno],
				AgentPList[playerno],
				CardList[playerno],
				PlayerList[playerno],
				LevelList[playerno],
				MaxRRList[playerno],
				PGList[playerno],
				PPGList[playerno],
				PPPGList[playerno],
				PPPRankList[playerno],
				PPPRankNameList[playerno],
				PPRankList[playerno],
				PPRankNameList[playerno],
				PRankList[playerno],
				PRankNameList[playerno],
				PhantomList[playerno],
				PhantomNameList[playerno],
				RankList[playerno],
				RankNameList[playerno],
				RankProgList[playerno],
				TrackerDisabledList[playerno],
				TrackerEnabledList[playerno],
				TrackerUrlList[playerno],
				VandalList[playerno],
				VandalNameList[playerno]
			};
			return output;
		}

		private static void GetLanguage()
		{
			Language = "ru-RU";
		}

		private static void GetIGCUsername(sbyte playerno)
		{
			if (IsIncognito[playerno])
			{
				PlayerList[playerno] = "----";
				TrackerEnabledList[playerno] = "Hidden";
				TrackerDisabledList[playerno] = "Visible";
			}
			else
			{
				PlayerList[playerno] = GetIGUsername(PUUIDList[playerno]);
				if (Tracker(PlayerList[playerno], playerno))
				{
					TrackerEnabledList[playerno] = "Visible";
					TrackerDisabledList[playerno] = "Collapsed";
				}
				else
				{
					TrackerEnabledList[playerno] = "Hidden";
					TrackerDisabledList[playerno] = "Visible";
				}
			}
		}

		private static void GetAgentInfo(string agent, sbyte playerno)
		{
			try
			{
				if (!String.IsNullOrEmpty(agent))
				{
					dynamic content = Task.Run(() => LoadJsonFromFile("ValAPI/agents.json")).Result;
					foreach (var agentEntry in content.data)
					{
						string uuid = agentEntry.uuid;
						if (uuid != agent) continue;
						AgentPList[playerno] = String.Format("/ValAPI/agentsimg/{0}.png", uuid);
						AgentList[playerno] = agentEntry.displayName;
						break;
					}
				}
				else
				{
					AgentPList[playerno] = AgentList[playerno] = null;
				}
			}
			finally
			{
			}
		}

		private static void GetCardInfo(string card, sbyte playerno)
		{
			try
			{
				dynamic content = Task.Run(() => LoadJsonFromFile("ValAPI/playercards.json")).Result;

				foreach (var cardItem in content.data)
				{
					if (cardItem.uuid != card) continue;
					CardList[playerno] = cardItem.smallArt;
					break;
				}
			}
			finally
			{
			}
		}

		private static void GetSkinInfo(sbyte playerno)
		{
			try
			{
				if (!String.IsNullOrEmpty(PUUIDList[playerno]))
				{
					string response = DoCachedRequest(Method.GET,
						$"https://glz-{Shard}-1.{Region}.a.pvp.net/core-game/v1/matches/{Matchid}/loadouts", true);
					dynamic content = JsonConvert.DeserializeObject(response);
					string vandalchroma = content.Loadouts[playerno].Loadout
						.Items["9c82e19d-4575-0200-1a81-3eacf00cf872"].Sockets["3ad1b2b2-acdb-4524-852f-954a76ddae0a"]
						.Item.ID;
					string phantomchroma = content.Loadouts[playerno].Loadout
						.Items["ee8e8d15-496b-07ac-e5f6-8fae5d4c7b1a"].Sockets["3ad1b2b2-acdb-4524-852f-954a76ddae0a"]
						.Item.ID;

					dynamic content2 = Task.Run(() => LoadJsonFromFile("ValAPI/skinchromas.json")).Result;
					foreach (var skin in content2.data)
					{
						if (skin.uuid == phantomchroma)
						{
							PhantomList[playerno] = skin.displayIcon;
							PhantomNameList[playerno] = skin.displayName;
						}
						else if (skin.uuid == vandalchroma)
						{
							VandalList[playerno] = skin.displayIcon;
							VandalNameList[playerno] = skin.displayName;
						}
					}

					if (vandalchroma == "19629ae1-4996-ae98-7742-24a240d41f99")
					{
						VandalList[playerno] = "/Assets/vandal.png";
					}
					if (phantomchroma == "52221ba2-4e4c-ec76-8c81-3483506d5242")
					{
						PhantomList[playerno] = "/Assets/phantom.png";
					}
				}
				else
				{
					VandalList[playerno] = null;
					VandalNameList[playerno] = null;
					PhantomList[playerno] = null;
					PhantomNameList[playerno] = null;
				}
			}
			finally
			{
			}
		}

		private static void GetCompHistory(string puuid, sbyte playerno)
		{
			try
			{
				if (!String.IsNullOrEmpty(puuid))
				{
					string content = DoCachedRequest(Method.GET,
						$"https://pd.{Region}.a.pvp.net/mmr/v1/players/{puuid}/competitiveupdates?queue=competitive",
						true, true);
					var historyinfo = JsonConvert.DeserializeObject(content);
					JToken historyinfoObj = JObject.FromObject(historyinfo);
					RankProgList[playerno] = historyinfoObj["Matches"][0]["RankedRatingAfterUpdate"].Value<string>();
					if (historyinfoObj["Matches"][0]["RankedRatingEarned"].Value<int>() >= 0)
					{
						PGList[playerno] = "/Assets/win.png";
					}
					else
					{
						PGList[playerno] = "/Assets/loss.png";
					}

					if (historyinfoObj["Matches"][1]["RankedRatingEarned"].Value<int>() >= 0)
					{
						PPGList[playerno] = "/Assets/win.png";
					}
					else
					{
						PPGList[playerno] = "/Assets/loss.png";
					}

					if (historyinfoObj["Matches"][2]["RankedRatingEarned"].Value<int>() >= 0)
					{
						PPPGList[playerno] = "/Assets/win.png";
					}
					else
					{
						PPPGList[playerno] = "/Assets/loss.png";
					}
				}
				else
				{
					RankProgList[playerno] = PGList[playerno] = PPGList[playerno] = PPPGList[playerno] = null;
				}
			}
			finally
			{
			}
		}

		private static void GetPlayerHistory(string puuid, sbyte playerno)
		{
			try
			{
				if (!String.IsNullOrEmpty(puuid))
				{
					string rank, prank, pprank, ppprank;
					string content = DoCachedRequest(Method.GET,
						$"https://pd.{Region}.a.pvp.net/mmr/v1/players/{puuid}", true, true);
					dynamic contentobj = JObject.Parse(content);

					try
					{
						rank = contentobj.QueueSkills.competitive.SeasonalInfoBySeasonID[$"{CurrentSeason}"]
							.CompetitiveTier;
						if (rank is "1" or "2") rank = "0";
					}
					catch (Exception)
					{
						rank = "0";
					}

					try
					{
						prank = contentobj.QueueSkills.competitive.SeasonalInfoBySeasonID[$"{PSeason}"].CompetitiveTier;
						if (prank is "1" or "2") prank = "0";
					}
					catch (Exception)
					{
						prank = "0";
					}

					try
					{
						pprank = contentobj.QueueSkills.competitive.SeasonalInfoBySeasonID[$"{PPSeason}"]
							.CompetitiveTier;
						if (pprank is "1" or "2") pprank = "0";
					}
					catch (Exception)
					{
						pprank = "0";
					}

					try
					{
						ppprank = contentobj.QueueSkills.competitive.SeasonalInfoBySeasonID[$"{PPPSeason}"]
							.CompetitiveTier;
						if (ppprank is "1" or "2") ppprank = "0";
					}
					catch (Exception)
					{
						ppprank = "0";
					}

					if (rank is "21" or "22" or "23")
					{
						string content2 = DoCachedRequest(Method.GET,
							$"https://pd.{Shard}.a.pvp.net/mmr/v1/leaderboards/affinity/{Region}/queue/competitive/season/{CurrentSeason}?startIndex=0&size=0",
							true);
						dynamic contentobj2 = JObject.Parse(content2);
						MaxRRList[playerno] = rank switch
						{
							"21" => contentobj2.tierDetails["22"].rankedRatingThreshold,
							"22" => contentobj2.tierDetails["23"].rankedRatingThreshold,
							"23" => contentobj2.tierDetails["24"].rankedRatingThreshold,
							_ => MaxRRList[playerno]
						};
					}
					else
					{
						MaxRRList[playerno] = "100";
					}

					GetRankStuff(playerno, rank, prank, pprank, ppprank);
				}
				else
				{
					RankList[playerno] = PRankList[playerno] = PPRankList[playerno] = PPPRankList[playerno] =
						RankNameList[playerno] = PRankNameList[playerno] =
							PPRankNameList[playerno] = PPPRankNameList[playerno] = null;
				}
			}
			finally
			{
			}
		}

		private static void GetSeasons()
		{
			try
			{
				string url = $"https://shared.{Region}.a.pvp.net/content-service/v3/content";
				RestClient client = new(url);
				RestRequest request = new(Method.GET);
				request.AddHeader("X-Riot-Entitlements-JWT", $"{EntitlementToken}");
				request.AddHeader("Authorization", $"Bearer {AccessToken}");
				request.AddHeader("X-Riot-ClientPlatform",
					"ew0KCSJwbGF0Zm9ybVR5cGUiOiAiUEMiLA0KCSJwbGF0Zm9ybU9TIjogIldpbmRvd3MiLA0KCSJwbGF0Zm9ybU9TVmVyc2lvbiI6ICIxMC4wLjE5MDQyLjEuMjU2LjY0Yml0IiwNCgkicGxhdGZvcm1DaGlwc2V0IjogIlVua25vd24iDQp9");
				request.AddHeader("X-Riot-ClientVersion", $"{Version}");
				string response = client.Execute(request).Content;
				dynamic content = JsonConvert.DeserializeObject(response);
				sbyte index = 0;
				sbyte currentindex = 0;

				if (content == null) return;
				foreach (var season in content.Seasons)
				{
					if ((season.IsActive == true) & (season.Type == "act"))
					{
						CurrentSeason = season.ID;
						currentindex = index;
						break;
					}

					index++;
				}

				currentindex--;
				if (content.Seasons[currentindex].Type == "act")
				{
					PSeason = content.Seasons[currentindex].ID;
				}
				else
				{
					currentindex--;
					PSeason = content.Seasons[currentindex].ID;
				}

				currentindex--;
				if (content.Seasons[currentindex].Type == "act")
				{
					PPSeason = content.Seasons[currentindex].ID;
				}
				else
				{
					currentindex--;
					PPSeason = content.Seasons[currentindex].ID;
				}

				currentindex--;
				if (content.Seasons[currentindex].Type == "act")
				{
					PPPSeason = content.Seasons[currentindex].ID;
				}
				else
				{
					currentindex--;
					PPPSeason = content.Seasons[currentindex].ID;
				}
			}
			catch (Exception e)
			{
				System.Diagnostics.Debug.WriteLine(e);
			}
		}

		private static void GetRankStuff(sbyte playerno, string rank, string prank, string pprank, string ppprank)
		{
			dynamic content = Task.Run(() => LoadJsonFromFile("ValAPI/competitivetiers.json")).Result;
			foreach (var tiers in content.data[3].tiers)
			{
				string tier = tiers.tier;
				if (rank == tier)
				{
					RankList[playerno] = String.Format("/ValAPI/ranksimg/{0}.png", tier);
					RankNameList[playerno] = tiers.tierName;
				}
				if (prank == tier)
				{
					PRankList[playerno] = String.Format("/ValAPI/ranksimg/{0}.png", tier);
					PRankNameList[playerno] = tiers.tierName;
				}
				if (tier == pprank)
				{
					PPRankList[playerno] = String.Format("/ValAPI/ranksimg/{0}.png", tier);
					PPRankNameList[playerno] = tiers.tierName;
				}
				if (tier == ppprank)
				{
					PPPRankList[playerno] = String.Format("/ValAPI/ranksimg/{0}.png", tier);
					PPPRankNameList[playerno] = tiers.tierName;
				}
			}
		}

		private static bool Tracker(string username, sbyte playerno)
		{
			bool output = false;
			try
			{
				if (!String.IsNullOrEmpty(username))
				{
					string encodedUsername = Uri.EscapeDataString(username);
					string url = "https://tracker.gg/valorant/profile/riot/" + encodedUsername;

					RestClient client = new(url);
					RestRequest request = new();
					var response = client.Execute(request);
					HttpStatusCode statusCode = response.StatusCode;
					short numericStatusCode = (short)statusCode;

					if (numericStatusCode == 200)
					{
						TrackerUrlList[playerno] = url;
						output = true;
					}
				}
				else
				{
					TrackerUrlList[playerno] = null;
				}

				return output;
			}
			finally
			{
			}
		}
	}
}