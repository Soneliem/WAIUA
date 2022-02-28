using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using RestSharp;
using WAIUA.Models;
using static WAIUA.Helpers.ValApi;
using static WAIUA.Helpers.Login;
using WAIUA.Properties;
using DataFormat = RestSharp.DataFormat;

namespace WAIUA.Helpers
{

	public class LiveMatch
	{
		internal string Server { get; set; }
		internal string Map { get; set; }
		internal Uri MapImage { get; set; }
		internal string GameMode { get; set; }

		private static Guid Matchid { get; set; }
		private static Guid CurrentSeason { get; set; }
		private static Guid PSeason { get; set; }
		private static Guid PPSeason { get; set; }
		private static Guid PPPSeason { get; set; }
		private static string[] PlayerList { get; } = new string[10];
		private static Guid[] PUUIDList { get; set; } = new Guid[10];
		private static Guid[] AgentUUIDList { get; set; } = new Guid[10];
        private static Uri[] AgentList { get; set; } = new Uri[10];
		private static Uri[] AgentPList { get; } = new Uri[10];
        private static int[] LevelList { get; set; } = new int[10];
		private static Uri[] RankList { get; } = new Uri[10];
		private static int[] MaxRRList { get; } = new int[10];
		private static Uri[] PRankList { get; } = new Uri[10];
		private static Uri[] PPRankList { get; } = new Uri[10];
		private static Uri[] PPPRankList { get; } = new Uri[10];
		private static string[] RankNameList { get; } = new string[10];
		private static string[] PRankNameList { get; } = new string[10];
		private static string[] PPRankNameList { get; } = new string[10];
		private static string[] PPPRankNameList { get; } = new string[10];
		private static long[] RankProgList { get; } = new long[10];
		private static string[] PGList { get; } = new string[10];
		private static string[] PPGList { get; } = new string[10];
		private static string[] PPPGList { get; } = new string[10];
		private static string[] PGColourList { get; } = new string[10];
		private static string[] PPGColourList { get; } = new string[10];
		private static string[] PPPGColourList { get; } = new string[10];
		private static bool[] IsIncognito { get; set; } = new bool[10];
		private static Uri[] TrackerUrlList { get; } = new Uri[10];
		private static string[] TrackerEnabledList { get; } = new string[10];
		private static string[] TrackerDisabledList { get; } = new string[10];
		private static Uri[] VandalList { get; } = new Uri[10];
		private static Uri[] PhantomList { get; } = new Uri[10];
		private static string[] VandalNameList { get; } = new string[10];
		private static string[] PhantomNameList { get; } = new string[10];
		private static Guid[] PartyList { get; } = new Guid[10];
		private static PresencesResponse Presences { get; set; }
		private static string[] BackgroundColour { get; } = new string[10];


        private static async Task<bool> LiveMatchIdAsync()
		{
            var client = new RestClient($"https://glz-{Constants.Shard}-1.{Constants.Region}.a.pvp.net/core-game/v1/players/{Constants.PPUUID}");
			var request = new RestRequest();
			request.AddHeader("X-Riot-Entitlements-JWT", Constants.EntitlementToken);
			request.AddHeader("Authorization", $"Bearer {Constants.AccessToken}");
			var response = await client.ExecuteGetAsync<LivePlayersResponse>(request).ConfigureAwait(false);
			if (!response.IsSuccessful) return false;
            Matchid = response.Data.MatchId;
			return true;
		}

		public async Task<bool> LiveMatchChecksAsync(bool isSilent)
		{
			bool output;

			if (await GetSetPpuuidAsync().ConfigureAwait(false))
            {
                await LocalRegionAsync().ConfigureAwait(false);
				if (await LiveMatchIdAsync().ConfigureAwait(false))
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

					if (await LiveMatchIdAsync().ConfigureAwait(false))
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
			RestRequest request = new();
			request.AddHeader("X-Riot-Entitlements-JWT", Constants.EntitlementToken);
			request.AddHeader("Authorization", $"Bearer {Constants.AccessToken}");
			var response = await client.ExecuteGetAsync<LiveMatchResponse>(request).ConfigureAwait(false);
            var playerno = new sbyte[10];
			var puuid = new Guid[10];
			var agent = new Guid[10];
            var level = new int[10];
			var incognito = new bool[10];
			sbyte index = 0;
			foreach (var entry in response.Data.Players)
				if (entry.IsCoach == false)
				{
					playerno[index] = index;
					puuid[index] = entry.Subject;
					agent[index] = entry.CharacterId;
                    level[index] = entry.PlayerIdentity.AccountLevel;
					if (entry.PlayerIdentity.Incognito == true) incognito[index] = true;

					index++;
					if (index == 10) break;
				}

			PUUIDList = puuid;
			AgentUUIDList = agent;
            LevelList = level;
			IsIncognito = incognito;
			Constants.LocalAppDataPath =
				Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\WAIUA";
			string gamePod = response.Data.GamePodId;
			if (Constants.GamePodsDictionary.TryGetValue(gamePod, out var ServerName)) Server = ServerName;
		}

		public async Task<object[]> LiveMatchOutputAsync(sbyte playerno)
        {
            var one = GetIgcUsernameAsync(playerno);
            var two = GetAgentInfoAsync(AgentUUIDList[playerno], playerno);
            var four = GetSkinInfoAsync(playerno);
            var five = GetCompHistoryAsync(PUUIDList[playerno], playerno);
            var six = GetPlayerHistoryAsync(PUUIDList[playerno], playerno);
            var seven = GetPresenceInfoAsync(PUUIDList[playerno], playerno);


           await Task.WhenAll(one, two, four, five, six, seven).ConfigureAwait(false);

			object[] output =
			{
				AgentList[playerno],
				AgentPList[playerno],
                AgentPList[playerno],
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

		private async Task GetAgentInfoAsync(Guid agentid, sbyte playerno)
		{
			if (agentid != Guid.Empty && agentid != null)
			{
				AgentPList[playerno] = new Uri(Constants.LocalAppDataPath + $"\\ValAPI\\agentsimg\\{agentid}.png");
				var agents = JsonSerializer.Deserialize<Dictionary<Guid, Uri>>(await File.ReadAllTextAsync(Constants.LocalAppDataPath + "\\ValAPI\\agents.txt").ConfigureAwait(false));
				agents.TryGetValue(agentid, out AgentList[playerno]);
            }
			else
            {
                AgentPList[playerno] = new Uri("");
                AgentList[playerno] = null;
			}
		}

        private async Task GetSkinInfoAsync(sbyte playerno)
		{
			try
			{
				
				if (PUUIDList[playerno] == Guid.Empty)
				{
					var response = await DoCachedRequestAsync(Method.Get,
                        $"https://glz-{Constants.Shard}-1.{Constants.Region}.a.pvp.net/core-game/v1/matches/{Matchid}/loadouts",
                        true).ConfigureAwait(false);
                    if (response.IsSuccessful)
                    {
                        var content = JsonSerializer.Deserialize<MatchLoadoutsResponse>(response.Content);
                        Guid vandalchroma = content.Loadouts[playerno].Loadout
                            .Items["9c82e19d-4575-0200-1a81-3eacf00cf872"].Sockets["3ad1b2b2-acdb-4524-852f-954a76ddae0a"]
                            .Item.Id;
                        Guid phantomchroma = content.Loadouts[playerno].Loadout
                            .Items["ee8e8d15-496b-07ac-e5f6-8fae5d4c7b1a"].Sockets["3ad1b2b2-acdb-4524-852f-954a76ddae0a"]
                            .Item.Id;

                        var Skins = JsonSerializer.Deserialize<Dictionary<Guid, Tuple<string, Uri>>>(await File.ReadAllTextAsync(Constants.LocalAppDataPath + "\\ValAPI\\skinchromas.txt").ConfigureAwait(false));
						
                        Skins.TryGetValue(phantomchroma, out var phantomTuple);
                        Skins.TryGetValue(vandalchroma, out var vandalTuple);

                        PhantomNameList[playerno] = phantomTuple.Item1;
                        VandalNameList[playerno] = vandalTuple.Item1;

						if (vandalchroma == new Guid("19629ae1-4996-ae98-7742-24a240d41f99"))
                            VandalList[playerno] = new Uri("/Assets/vandal.png");
						else
                            VandalList[playerno] = vandalTuple.Item2;

						if (phantomchroma == new Guid("52221ba2-4e4c-ec76-8c81-3483506d5242"))
                            PhantomList[playerno] = new Uri("/Assets/phantom.png");
                        else
                            PhantomList[playerno] = phantomTuple.Item2;
                    }
                    else
                    {
						VandalList[playerno] = null;
                        VandalNameList[playerno] = "";
                        PhantomList[playerno] = null;
                        PhantomNameList[playerno] = "";
					}
                   
				}
			}
			catch (Exception)
			{
				// ignored
			}
		}

		private async Task GetCompHistoryAsync(Guid puuid, sbyte playerno)
		{
			try
            {
                if (puuid != Guid.Empty)
				{
					var response = await DoCachedRequestAsync(Method.Get,
                        $"https://pd.{Constants.Region}.a.pvp.net/mmr/v1/players/{puuid}/competitiveupdates?queue=competitive",
                        true, true).ConfigureAwait(false);
                    if (!response.IsSuccessful)
                    {
						Constants.Log.Error("GetCompHistoryAsync request failed: {e}", response.ErrorException);
                        return;
                    }
                    var content = JsonSerializer.Deserialize<CompetitiveUpdatesResponse>(response.Content);

					if (0 < content?.Matches.Length)
                    {
                        RankProgList[playerno] = content.Matches[0].RankedRatingAfterUpdate;
                        var pmatch = content.Matches[0].RankedRatingEarned;
                        PGList[playerno] = pmatch.ToString("+#;-#;0");
                        PGColourList[playerno] = pmatch switch
                        {
                            > 0 => "#32e2b2",
                            < 0 => "#ff4654",
                            _ => "#7f7f7f"
                        };
                    }

                    if (1 < content?.Matches.Length)
					{
						var ppmatch = content.Matches[1].RankedRatingEarned;
						PPGList[playerno] = ppmatch.ToString("+#;-#;0");
                        PPGColourList[playerno] = ppmatch switch
                        {
                            > 0 => "#32e2b2",
                            < 0 => "#ff4654",
                            _ => "#7f7f7f"
                        };
                    }

                    if (2 < content?.Matches.Length)
					{
						var pppmatch = content.Matches[2].RankedRatingEarned;
						PPPGList[playerno] = pppmatch.ToString("+#;-#;0");
                        PPPGColourList[playerno] = pppmatch switch
                        {
                            > 0 => "#32e2b2",
                            < 0 => "#ff4654",
                            _ => "#7f7f7f"
                        };
                    }
				}
                else
                {
                    RankProgList[playerno] = 0;
                    PGList[playerno] = PPGList[playerno] = PPPGList[playerno] = "";
                    PGColourList[playerno] = PPGColourList[playerno] = PPPGColourList[playerno] = "Transparent";
                    Constants.Log.Error("GetCompHistoryAsync Puuid is null");
                }
			}
			catch (Exception e)
			{
                Constants.Log.Error("GetCompHistoryAsync failed: {e}", e);
			}
		}

		private async Task GetPlayerHistoryAsync(Guid puuid, sbyte playerno)
        {
            RankList[playerno] = PRankList[playerno] = PPRankList[playerno] = PPPRankList[playerno] = null;
			RankNameList[playerno] = PRankNameList[playerno] =
				PPRankNameList[playerno] = PPPRankNameList[playerno] = "";
			if (puuid != Guid.Empty)
			{
				int rank, prank, pprank, ppprank;
                var response = await DoCachedRequestAsync(Method.Get,
                    $"https://pd.{Constants.Region}.a.pvp.net/mmr/v1/players/{puuid}",
                    true,
                    true).ConfigureAwait(false);

				if (!response.IsSuccessful)
				{
                    var content = JsonSerializer.Deserialize<MmrResponse>(response.Content);
					try
                    {
                        content.QueueSkills.Competitive.SeasonalInfoBySeasonId.Act.TryGetValue(CurrentSeason, out ActInfo CurrentAct);
						rank = CurrentAct.CompetitiveTier;
						if (rank is 1 or 2) rank = 0;
					}
					catch (Exception)
					{
						rank = 0;
					}

					try
					{
                        content.QueueSkills.Competitive.SeasonalInfoBySeasonId.Act.TryGetValue(PSeason, out ActInfo PAct);
						prank = PAct.CompetitiveTier;
						if (prank is 1 or 2) prank = 0;
					}
					catch (Exception)
					{
						prank = 0;
					}

					try
					{
                        content.QueueSkills.Competitive.SeasonalInfoBySeasonId.Act.TryGetValue(PPSeason, out ActInfo PPAct);
						pprank = PPAct.CompetitiveTier;
						if (pprank is 1 or 2) pprank = 0;
					}
					catch (Exception)
					{
						pprank = 0;
					}

					try
					{
                        content.QueueSkills.Competitive.SeasonalInfoBySeasonId.Act.TryGetValue(PPPSeason, out ActInfo PPPAct);
						ppprank = PPPAct.CompetitiveTier;
						if (ppprank is 1 or 2) ppprank = 0;
					}
					catch (Exception)
					{
						ppprank = 0;
					}

					if (rank is 21 or 22 or 23)
					{
						var response2 = await DoCachedRequestAsync(Method.Get,
                            $"https://pd.{Constants.Shard}.a.pvp.net/mmr/v1/leaderboards/affinity/{Constants.Region}/queue/competitive/season/{CurrentSeason}?startIndex=0&size=0",
                            true).ConfigureAwait(false);
                        var content2 = JsonSerializer.Deserialize<LeaderboardsResponse>(response2.Content);
						MaxRRList[playerno] = rank switch
						{
							21 => content2.TierDetails["22"].RankedRatingThreshold,
							22 => content2.TierDetails["23"].RankedRatingThreshold,
							23 => content2.TierDetails["24"].RankedRatingThreshold,
							_ => MaxRRList[playerno]
						};
					}
					else
                    {
                        MaxRRList[playerno] = 100;
                    }

					await GetRankStuffAsync(playerno, rank, prank, pprank, ppprank).ConfigureAwait(false);
				}
			}
		}

		private async Task GetSeasonsAsync()
		{
			try
			{
                RestClient client = new($"https://shared.{Constants.Region}.a.pvp.net/content-service/v3/content");
				var request = new RestRequest().AddHeader("X-Riot-Entitlements-JWT", Constants.EntitlementToken)
                    .AddHeader("Authorization", $"Bearer {Constants.AccessToken}")
                    .AddHeader("X-Riot-ClientPlatform",
					"ew0KCSJwbGF0Zm9ybVR5cGUiOiAiUEMiLA0KCSJwbGF0Zm9ybU9TIjogIldpbmRvd3MiLA0KCSJwbGF0Zm9ybU9TVmVyc2lvbiI6ICIxMC4wLjE5MDQyLjEuMjU2LjY0Yml0IiwNCgkicGxhdGZvcm1DaGlwc2V0IjogIlVua25vd24iDQp9")
                    .AddHeader("X-Riot-ClientVersion", Constants.Version);
				var response = await client.ExecuteGetAsync<ContentResponse>(request).ConfigureAwait(false);
                sbyte index = 0;
				sbyte currentindex = 0;

				if (!response.IsSuccessful) return;
				foreach (var season in response.Data.Seasons)
				{
					if ((season.IsActive) & (season.Type == TypeEnum.Act))
					{
						CurrentSeason = season.Id;
						currentindex = index;
						break;
					}

					index++;
				}

				currentindex--;
				if (response.Data.Seasons[currentindex].Type == TypeEnum.Act)
				{
					PSeason = response.Data.Seasons[currentindex].Id;
				}
				else
				{
					currentindex--;
					PSeason = response.Data.Seasons[currentindex].Id;
				}

				currentindex--;
				if (response.Data.Seasons[currentindex].Type == TypeEnum.Act)
				{
					PPSeason = response.Data.Seasons[currentindex].Id;
				}
				else
				{
					currentindex--;
					PPSeason = response.Data.Seasons[currentindex].Id;
				}

				currentindex--;
				if (response.Data.Seasons[currentindex].Type == TypeEnum.Act)
				{
					PPPSeason = response.Data.Seasons[currentindex].Id;
				}
				else
				{
					currentindex--;
					PPPSeason = response.Data.Seasons[currentindex].Id;
				}
			}
			catch (Exception)
			{
				// ignored
			}
		}

		private async Task GetRankStuffAsync(sbyte playerno, int rank, int prank, int pprank, int ppprank)
		{
            var ranks = JsonSerializer.Deserialize<Dictionary<int, string>>(await File.ReadAllTextAsync(Constants.LocalAppDataPath + "\\ValAPI\\competitivetiers.txt").ConfigureAwait(false));

            ranks.TryGetValue(rank, out var rank0);
            RankList[playerno] = new Uri(Constants.LocalAppDataPath + $"\\ValAPI\\agentsimg\\{rank}.png");
			RankNameList[playerno] = rank0;

            ranks.TryGetValue(prank, out var rank1);
            PRankList[playerno] = new Uri(Constants.LocalAppDataPath + $"\\ValAPI\\agentsimg\\{prank}.png");
			PRankNameList[playerno] = rank1;

            ranks.TryGetValue(pprank, out var rank2);
            PPRankList[playerno] = new Uri(Constants.LocalAppDataPath + $"\\ValAPI\\agentsimg\\{pprank}.png");
			PPRankNameList[playerno] = rank2;

            ranks.TryGetValue(ppprank, out var rank3);
            PPPRankList[playerno] = new Uri(Constants.LocalAppDataPath + $"\\ValAPI\\agentsimg\\{ppprank}.png");
			PPPRankNameList[playerno] = rank3;

		}

		private async Task<bool> TrackerAsync(string username, sbyte playerno)
		{
			var output = false;
			try
			{
				TrackerUrlList[playerno] = null;
				if (!string.IsNullOrEmpty(username))
				{
					var encodedUsername = Uri.EscapeDataString(username);
					var url = new Uri("https://tracker.gg/valorant/profile/riot/" + encodedUsername);

					RestClient client = new(url);
					RestRequest request = new();
					var response = await client.ExecuteAsync(request).ConfigureAwait(false);
                    var numericStatusCode = (short)response.StatusCode;

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

		private async Task GetPresencesAsync()
		{
			var client = new RestClient($"https://127.0.0.1:{Constants.Port}/chat/v4/presences");

			// client.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
            var request = new RestRequest().AddHeader("Authorization",
				$"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes($"riot:{Constants.LPassword}"))}")
			.AddHeader("X-Riot-ClientPlatform",
				"ew0KCSJwbGF0Zm9ybVR5cGUiOiAiUEMiLA0KCSJwbGF0Zm9ybU9TIjogIldpbmRvd3MiLA0KCSJwbGF0Zm9ybU9TVmVyc2lvbiI6ICIxMC4wLjE5MDQyLjEuMjU2LjY0Yml0IiwNCgkicGxhdGZvcm1DaGlwc2V0IjogIlVua25vd24iDQp9")
			.AddHeader("X-Riot-ClientVersion", Constants.Version);
			var response = await client.ExecuteGetAsync<PresencesResponse>(request).ConfigureAwait(false);
			if (response.IsSuccessful) Presences = response.Data;
        }

		private async Task GetPresenceInfoAsync(Guid puuid, int playerno)
		{
			try
			{
				PartyList[playerno] = Guid.Empty;
				foreach (var friend in Presences.Presences)
					if (friend.Puuid == puuid)
					{
                        var json = Encoding.UTF8.GetString(Convert.FromBase64String(friend.Private));
						var content = JsonSerializer.Deserialize<PresencesPrivate>(json);
                        PartyList[playerno] = content.PartyId;
						if (puuid == Constants.PPUUID)
						{
							var Maps = JsonSerializer.Deserialize<Dictionary<string, string>>(await File.ReadAllTextAsync(Constants.LocalAppDataPath + "\\ValAPI\\maps.txt").ConfigureAwait(false));

							Maps.TryGetValue((string) content.MatchMap, out var mapName);
							Map = mapName;
							MapImage = new Uri(Constants.LocalAppDataPath + $"\\ValAPI\\agentsimg\\{content.MatchMap}.png");
							BackgroundColour[playerno] = "#181E34";
							Constants.PPartyID = content.PartyId;

							if (content?.ProvisioningFlow == "customGame")
							{
								GameMode = "Custom";
							}
							else
							{
								var textInfo = new CultureInfo("en-US", false).TextInfo;
								GameMode = (string) content?.QueueId switch
								{
									"ggteam" => "Escalation",
									"spikerush" => "Spike Rush",
									"newmap" => "New Map",
									"onefa" => "Replication",
									"snowball" => "Replication",
									_ => textInfo.ToTitleCase(content.QueueId)
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