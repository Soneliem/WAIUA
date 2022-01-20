using System;
using System.Globalization;
using System.IO;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using static WAIUA.Helpers.ValAPI;
using static WAIUA.Helpers.Login;
using WAIUA.Properties;
using DataFormat = RestSharp.DataFormat;

namespace WAIUA.Helpers
{

	public class LiveMatch
	{
		internal string Server { get; set; }
		internal string Map { get; set; }
		internal string MapImage { get; set; }
		internal string GameMode { get; set; }

		private static string Matchid { get; set; }
		private static string CurrentSeason { get; set; }
		private static string PSeason { get; set; }
		private static string PPSeason { get; set; }
		private static string PPPSeason { get; set; }
		private static string[] PlayerList { get; } = new string[10];
		private static string[] PUUIDList { get; set; } = new string[10];
		private static string[] AgentList { get; set; } = new string[10];
		private static string[] AgentPList { get; } = new string[10];
		private static string[] CardList { get; set; } = new string[10];
		private static string[] LevelList { get; set; } = new string[10];
		private static string[] RankList { get; } = new string[10];
		private static string[] MaxRRList { get; } = new string[10];
		private static string[] PRankList { get; } = new string[10];
		private static string[] PPRankList { get; } = new string[10];
		private static string[] PPPRankList { get; } = new string[10];
		private static string[] RankNameList { get; } = new string[10];
		private static string[] PRankNameList { get; } = new string[10];
		private static string[] PPRankNameList { get; } = new string[10];
		private static string[] PPPRankNameList { get; } = new string[10];
		private static string[] RankProgList { get; } = new string[10];
		private static string[] PGList { get; } = new string[10];
		private static string[] PPGList { get; } = new string[10];
		private static string[] PPPGList { get; } = new string[10];
		private static string[] PGColourList { get; } = new string[10];
		private static string[] PPGColourList { get; } = new string[10];
		private static string[] PPPGColourList { get; } = new string[10];
		private static bool[] IsIncognito { get; set; } = new bool[10];
		private static string[] TrackerUrlList { get; } = new string[10];
		private static string[] TrackerEnabledList { get; } = new string[10];
		private static string[] TrackerDisabledList { get; } = new string[10];
		private static string[] VandalList { get; } = new string[10];
		private static string[] PhantomList { get; } = new string[10];
		private static string[] VandalNameList { get; } = new string[10];
		private static string[] PhantomNameList { get; } = new string[10];
		private static string[] PartyList { get; } = new string[10];
		private static dynamic Presences { get; set; }
		private static string[] BackgroundColour { get; } = new string[10];

		

		public static async Task<bool> LiveMatchIDAsync()
		{
			var url =
				$"https://glz-{Constants.Shard}-1.{Constants.Region}.a.pvp.net/core-game/v1/players/{Constants.PPUUID}";
			RestClient client = new(url);
			RestRequest request = new(Method.GET);
			request.AddHeader("X-Riot-Entitlements-JWT", Constants.EntitlementToken);
			request.AddHeader("Authorization", $"Bearer {Constants.AccessToken}");
			var response = await client.ExecuteAsync(request).ConfigureAwait(false);
			if (!response.IsSuccessful) return false;
			var content = response.Content;
			var matchinfo = JsonConvert.DeserializeObject(content);
			JToken matchinfoObj = JObject.FromObject(matchinfo);
			Matchid = matchinfoObj["MatchID"].Value<string>();
			return true;
		}

		public async Task<bool> LiveMatchChecksAsync(bool isSilent)
		{
			bool output;

			if (await GetSetPpuuidAsync().ConfigureAwait(false))
			{
				if (await LiveMatchIDAsync().ConfigureAwait(false))
				{
					await LiveMatchSetupAsync().ConfigureAwait(false);
					output = true;
				}
				else
				{
					if (!isSilent)
						MessageBox.Show(Resources.NoMatch, Resources.Error, MessageBoxButton.OK,
							MessageBoxImage.Question, MessageBoxResult.OK);
					output = false;
				}
			}
			else
			{
				if (await CheckLocalAsync().ConfigureAwait(false))
				{
					await LocalLoginAsync().ConfigureAwait(false);
					await GetSetPpuuidAsync().ConfigureAwait(false);
					await LocalRegionAsync().ConfigureAwait(false);

					if (await LiveMatchIDAsync().ConfigureAwait(false))
					{
						await LiveMatchSetupAsync().ConfigureAwait(false);
						output = true;
					}
					else
					{
						if (!isSilent)
							MessageBox.Show(Resources.NoMatch, Resources.Error, MessageBoxButton.OK,
								MessageBoxImage.Question, MessageBoxResult.OK);
						output = false;
					}
				}
				else
				{
					if (!isSilent)
						MessageBox.Show(Resources.NoValGame, Resources.Error, MessageBoxButton.OK,
							MessageBoxImage.Question, MessageBoxResult.OK);
					output = false;
				}
			}

			return output;
		}

		public async Task LiveMatchSetupAsync()
        {
            var getseason = GetSeasonsAsync();
            var getpresense = GetPresencesAsync();
            await Task.WhenAll(getseason, getpresense).ConfigureAwait(false);

			var url = $"https://glz-{Constants.Shard}-1.{Constants.Region}.a.pvp.net/core-game/v1/matches/{Matchid}";
			RestClient client = new(url);
			RestRequest request = new(Method.GET);
			request.AddHeader("X-Riot-Entitlements-JWT", Constants.EntitlementToken);
			request.AddHeader("Authorization", $"Bearer {Constants.AccessToken}");
			var content = (await client.ExecuteAsync(request).ConfigureAwait(false)).Content;
			dynamic matchinfo = JsonConvert.DeserializeObject(content);
			var playerno = new sbyte[10];
			var puuid = new string[10];
			var agent = new string[10];
			var card = new string[10];
			var level = new string[10];
			var incognito = new bool[10];
			sbyte index = 0;
			foreach (var entry in matchinfo.Players)
				if (entry.IsCoach == false)
				{
					playerno[index] = index;
					puuid[index] = entry.Subject;
					agent[index] = entry.CharacterID;
					card[index] = entry.PlayerIdentity.PlayerCardID;
					level[index] = entry.PlayerIdentity.AccountLevel;
					if (entry.PlayerIdentity.Incognito == true) incognito[index] = true;

					index++;
					if (index == 10) break;
				}

			PUUIDList = puuid;
			AgentList = agent;
			CardList = card;
			LevelList = level;
			IsIncognito = incognito;
			Constants.CurrentPath =
				Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\WAIUA";
			string GamePod = matchinfo.GamePodID;
			if (Constants.GamePodsDictionary.TryGetValue(GamePod, out var ServerName)) Server = ServerName;
		}

		public async Task<string[]> LiveMatchOutputAsync(sbyte playerno)
        {
            var one = GetIgcUsernameAsync(playerno);
            var two = GetAgentInfoAsync(AgentList[playerno], playerno);
            var three = GetCardInfoAsync(CardList[playerno], playerno);
            var four = GetSkinInfoAsync(playerno);
            var five = GetCompHistoryAsync(PUUIDList[playerno], playerno);
            var six = GetPlayerHistoryAsync(PUUIDList[playerno], playerno);
            var seven = GetPresenceInfoAsync(PUUIDList[playerno], playerno);


           await Task.WhenAll(one, two, three, four, five, six, seven).ConfigureAwait(false);

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
				VandalNameList[playerno],
				PGColourList[playerno],
				PPGColourList[playerno],
				PPPGColourList[playerno],
				PartyList[playerno],
				BackgroundColour[playerno]
			};
			return output;
		}

		private async Task GetIgcUsernameAsync(sbyte playerno)
		{
			if (IsIncognito[playerno] && PartyList[playerno] != Constants.PPartyID)
			{
				PlayerList[playerno] = "----";
				TrackerEnabledList[playerno] = "Hidden";
				TrackerDisabledList[playerno] = "Visible";
			}
			else
			{
				PlayerList[playerno] = await GetIgUsernameAsync(PUUIDList[playerno]).ConfigureAwait(false);
				if (await TrackerAsync(PlayerList[playerno], playerno).ConfigureAwait(false))
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

		private async Task GetAgentInfoAsync(string agentid, sbyte playerno)
		{
			if (!string.IsNullOrEmpty(agentid))
			{
				AgentPList[playerno] = Constants.CurrentPath + $"\\ValAPI\\agentsimg\\{agentid}.png";
				var agents = await LoadDictionaryFromFileAsync("\\ValAPI\\agents.txt").ConfigureAwait(false);
				agents.TryGetValue(agentid, out var agent);
				AgentList[playerno] = agent?[0];
			}
			else
			{
				AgentPList[playerno] = AgentList[playerno] = "";
			}
		}

		private async Task GetCardInfoAsync(string cardid, sbyte playerno)
		{
			if (!string.IsNullOrEmpty(cardid))
			{
				var Cards =
					await Task.Run(() => LoadDictionaryFromFileAsync("\\ValAPI\\cards.txt")).ConfigureAwait(false);
				Cards.TryGetValue(cardid, out var card);
				CardList[playerno] = card?[0];
			}
			else
			{
				CardList[playerno] = "";
			}
		}

		private async Task GetSkinInfoAsync(sbyte playerno)
		{
			try
			{
				VandalList[playerno] = null;
				VandalNameList[playerno] = "";
				PhantomList[playerno] = null;
				PhantomNameList[playerno] = "";
				if (!string.IsNullOrEmpty(PUUIDList[playerno]))
				{
					var response = await DoCachedRequestAsync(Method.GET,
                        $"https://glz-{Constants.Shard}-1.{Constants.Region}.a.pvp.net/core-game/v1/matches/{Matchid}/loadouts",
                        true).ConfigureAwait(false);
					dynamic content = JsonConvert.DeserializeObject(response);
					string vandalchroma = content.Loadouts[playerno].Loadout
						.Items["9c82e19d-4575-0200-1a81-3eacf00cf872"].Sockets["3ad1b2b2-acdb-4524-852f-954a76ddae0a"]
						.Item.ID;
					string phantomchroma = content.Loadouts[playerno].Loadout
						.Items["ee8e8d15-496b-07ac-e5f6-8fae5d4c7b1a"].Sockets["3ad1b2b2-acdb-4524-852f-954a76ddae0a"]
						.Item.ID;

					var Skins = await LoadDictionaryFromFileAsync("\\ValAPI\\skinchromas.txt").ConfigureAwait(false);
					Skins.TryGetValue(phantomchroma, out var phantomskin);
					PhantomList[playerno] = phantomskin?[1];
					PhantomNameList[playerno] = phantomskin?[0];

					Skins.TryGetValue(phantomchroma, out var vandalskin);
					VandalList[playerno] = vandalskin?[1];
					VandalNameList[playerno] = vandalskin?[0];

					if (vandalchroma == "19629ae1-4996-ae98-7742-24a240d41f99")
						VandalList[playerno] = "/Assets/vandal.png";
					if (phantomchroma == "52221ba2-4e4c-ec76-8c81-3483506d5242")
						PhantomList[playerno] = "/Assets/phantom.png";
				}
			}
			catch (Exception)
			{
				// ignored
			}
		}

		private async Task GetCompHistoryAsync(string puuid, sbyte playerno)
		{
			try
			{
				RankProgList[playerno] = PGList[playerno] = PPGList[playerno] = PPPGList[playerno] = "";
				PGColourList[playerno] = PPGColourList[playerno] = PPPGColourList[playerno] = "Transparent";
				if (!string.IsNullOrEmpty(puuid))
				{
					var content = await DoCachedRequestAsync(Method.GET,
                        $"https://pd.{Constants.Region}.a.pvp.net/mmr/v1/players/{puuid}/competitiveupdates?queue=competitive",
                        true, true).ConfigureAwait(false);
					var historyinfo = JsonConvert.DeserializeObject(content);
					JToken historyinfoObj = JObject.FromObject(historyinfo);

					if (historyinfoObj["Matches"][0]["RankedRatingEarned"] != null)
					{
						RankProgList[playerno] =
							historyinfoObj["Matches"][0]["RankedRatingAfterUpdate"].Value<string>();
						var pmatch = historyinfoObj["Matches"][0]["RankedRatingEarned"].Value<int>();
						PGList[playerno] = pmatch.ToString("+#;-#;0");
						if (pmatch > 0)
							PGColourList[playerno] = "#32e2b2";
						else if (pmatch < 0)
							PGColourList[playerno] = "#ff4654";
						else
							PGColourList[playerno] = "#7f7f7f";
					}

					if (historyinfoObj["Matches"][1]["RankedRatingEarned"] != null)
					{
						var ppmatch = historyinfoObj["Matches"][1]["RankedRatingEarned"].Value<int>();
						PPGList[playerno] = ppmatch.ToString("+#;-#;0");
						if (ppmatch > 0)
							PPGColourList[playerno] = "#32e2b2";
						else if (ppmatch < 0)
							PPGColourList[playerno] = "#ff4654";
						else
							PPGColourList[playerno] = "#7f7f7f";
					}

					if (historyinfoObj["Matches"][2]["RankedRatingEarned"] != null)
					{
						var pppmatch = historyinfoObj["Matches"][2]["RankedRatingEarned"].Value<int>();
						PPPGList[playerno] = pppmatch.ToString("+#;-#;0");
						if (pppmatch > 0)
							PPPGColourList[playerno] = "#32e2b2";
						else if (pppmatch < 0)
							PPPGColourList[playerno] = "#ff4654";
						else
							PPPGColourList[playerno] = "#7f7f7f";
					}
				}
			}
			catch (Exception)
			{
				// ignored
			}
		}

		private async Task GetPlayerHistoryAsync(string puuid, sbyte playerno)
		{
			RankList[playerno] = PRankList[playerno] = PPRankList[playerno] = PPPRankList[playerno] = null;
			RankNameList[playerno] = PRankNameList[playerno] =
				PPRankNameList[playerno] = PPPRankNameList[playerno] = "";
			if (!string.IsNullOrEmpty(puuid))
			{
				string rank, prank, pprank, ppprank;
				var content = await DoCachedRequestAsync(Method.GET,
                    $"https://pd.{Constants.Region}.a.pvp.net/mmr/v1/players/{puuid}",
                    true,
                    true).ConfigureAwait(false);

				if (content != null)
				{
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
						var content2 = await DoCachedRequestAsync(Method.GET,
                            $"https://pd.{Constants.Shard}.a.pvp.net/mmr/v1/leaderboards/affinity/{Constants.Region}/queue/competitive/season/{CurrentSeason}?startIndex=0&size=0",
                            true).ConfigureAwait(false);
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

					await GetRankStuffAsync(playerno, rank, prank, pprank, ppprank).ConfigureAwait(false);
				}
			}
		}

		private async Task GetSeasonsAsync()
		{
			try
			{
				var url = $"https://shared.{Constants.Region}.a.pvp.net/content-service/v3/content";
				RestClient client = new(url);
				RestRequest request = new(Method.GET);
				request.AddHeader("X-Riot-Entitlements-JWT", Constants.EntitlementToken);
				request.AddHeader("Authorization", $"Bearer {Constants.AccessToken}");
				request.AddHeader("X-Riot-ClientPlatform",
					"ew0KCSJwbGF0Zm9ybVR5cGUiOiAiUEMiLA0KCSJwbGF0Zm9ybU9TIjogIldpbmRvd3MiLA0KCSJwbGF0Zm9ybU9TVmVyc2lvbiI6ICIxMC4wLjE5MDQyLjEuMjU2LjY0Yml0IiwNCgkicGxhdGZvcm1DaGlwc2V0IjogIlVua25vd24iDQp9");
				request.AddHeader("X-Riot-ClientVersion", Constants.Version);
				var response = (await client.ExecuteAsync(request).ConfigureAwait(false)).Content;
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
			catch (Exception)
			{
				// ignored
			}
		}

		private async Task GetRankStuffAsync(sbyte playerno, string rank, string prank, string pprank, string ppprank)
		{
			var ranks = await LoadDictionaryFromFileAsync("\\ValAPI\\competitivetiers.txt").ConfigureAwait(false);

            ranks.TryGetValue(rank, out var rank0);
            RankList[playerno] = rank0?[1];
            RankNameList[playerno] = rank0?[0];

            ranks.TryGetValue(prank, out var rank1);
            PRankList[playerno] = rank1?[1];
            PRankNameList[playerno] = rank1?[0];

            ranks.TryGetValue(pprank, out var rank2);
            PPRankList[playerno] = rank2?[1];
            PPRankNameList[playerno] = rank2?[0];

            ranks.TryGetValue(ppprank, out var rank3);
            PPPRankList[playerno] = rank3?[1];
            PPPRankNameList[playerno] = rank3?[0];

		}

		private async Task<bool> TrackerAsync(string username, sbyte playerno)
		{
			var output = false;
			try
			{
				TrackerUrlList[playerno] = "";
				if (!string.IsNullOrEmpty(username))
				{
					var encodedUsername = Uri.EscapeDataString(username);
					var url = "https://tracker.gg/valorant/profile/riot/" + encodedUsername;

					RestClient client = new(url);
					RestRequest request = new();
					var response = await client.ExecuteAsync(request).ConfigureAwait(false);
					var statusCode = response.StatusCode;
					var numericStatusCode = (short) statusCode;

					if (numericStatusCode == 200)
					{
						TrackerUrlList[playerno] = url;
						output = true;
					}
				}
			}
			catch (Exception)
			{
				// ignored
			}

			return output;
		}

		private Task GetPresencesAsync()
		{
			RestClient client = new(new Uri($"https://127.0.0.1:{Constants.Port}/chat/v4/presences"));
			RestRequest request = new(Method.GET);
			client.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
			request.AddHeader("Authorization",
				$"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes($"riot:{Constants.LPassword}"))}");
			request.AddHeader("X-Riot-ClientPlatform",
				"ew0KCSJwbGF0Zm9ybVR5cGUiOiAiUEMiLA0KCSJwbGF0Zm9ybU9TIjogIldpbmRvd3MiLA0KCSJwbGF0Zm9ybU9TVmVyc2lvbiI6ICIxMC4wLjE5MDQyLjEuMjU2LjY0Yml0IiwNCgkicGxhdGZvcm1DaGlwc2V0IjogIlVua25vd24iDQp9");
			request.AddHeader("X-Riot-ClientVersion", Constants.Version);
			request.RequestFormat = DataFormat.Json;
			var response = client.Execute(request);
			if (response.IsSuccessful) Presences = JsonConvert.DeserializeObject(response.Content);
            return Task.CompletedTask;
        }

		private async Task GetPresenceInfoAsync(string puuid, int playerno)
		{
			try
			{
				PartyList[playerno] = null;
				foreach (var friend in Presences.presences)
					if (friend.puuid == puuid)
					{
						string base64 = friend["private"];
						var blob = Convert.FromBase64String(base64);
						var json = Encoding.UTF8.GetString(blob);
						dynamic content = JsonConvert.DeserializeObject(json);
						PartyList[playerno] = content.partyId;
						if (puuid == Constants.PPUUID)
						{
							var Maps = await LoadDictionaryFromFileAsync("\\ValAPI\\maps.txt").ConfigureAwait(false);
							Maps.TryGetValue((string) content.matchMap, out var map);
							Map = map[0];
							MapImage = map[1];
							BackgroundColour[playerno] = "#181E34";
							Constants.PPartyID = content.partyId;

							if (content?.provisioningFlowID == "customGame")
							{
								GameMode = "Custom";
							}
							else
							{
								var textInfo = new CultureInfo("en-US", false).TextInfo;
								GameMode = (string) content?.queueId switch
								{
									"ggteam" => "Escalation",
									"spikerush" => "Spike Rush",
									"newmap" => "New Map",
									"onefa" => "Replication",
									"snowball" => "Replication",
									_ => textInfo.ToTitleCase(content.queueId)
								};
							}
						}
						else
						{
							BackgroundColour[playerno] = "#252A40";
						}

						break;
					}
			}
			catch (Exception)
			{
				// ignored
			}
		}
	}
}