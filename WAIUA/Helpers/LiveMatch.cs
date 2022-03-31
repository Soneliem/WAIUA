using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using RestSharp;
using RestSharp.Serializers.Json;
using WAIUA.Objects;
using WAIUA.Properties;
using static WAIUA.Helpers.Login;

namespace WAIUA.Helpers;

public class Match
{
    public delegate void UpdateProgress(int percentage);

    public MatchDetails MatchInfo { get; set; } = new();
    public static Guid Matchid { get; set; }


    private static async Task<bool> CheckAndSetLiveMatchIdAsync()
    {
        var client = new RestClient($"https://glz-{Constants.Shard}-1.{Constants.Region}.a.pvp.net/core-game/v1/players/{Constants.Ppuuid}");
        var request = new RestRequest();
        request.AddHeader("X-Riot-Entitlements-JWT", Constants.EntitlementToken);
        request.AddHeader("Authorization", $"Bearer {Constants.AccessToken}");
        var response = await client.ExecuteGetAsync<LiveMatchIDResponse>(request).ConfigureAwait(false);
        if (!response.IsSuccessful)
        {
            Constants.Log.Error("CheckAndSetLiveMatchIdAsync() failed. Response: {Response}", response.ErrorException);
            return false;
        }

        Matchid = response.Data.MatchId;
        return true;
    }


    public static async Task<bool> LiveMatchChecksAsync(bool isSilent)
    {
        if (await GetSetPpuuidAsync().ConfigureAwait(false))
        {
            await LocalRegionAsync().ConfigureAwait(false);
            if (await CheckAndSetLiveMatchIdAsync().ConfigureAwait(false)) return true;

            if (!isSilent)
                MessageBox.Show(Resources.NoMatch, Resources.Error, MessageBoxButton.OK,
                    MessageBoxImage.Question, MessageBoxResult.OK);
            return false;
        }

        if (await CheckLocalAsync().ConfigureAwait(false))
        {
            await LocalLoginAsync().ConfigureAwait(false);
            await GetSetPpuuidAsync().ConfigureAwait(false);
            await LocalRegionAsync().ConfigureAwait(false);

            if (await CheckAndSetLiveMatchIdAsync().ConfigureAwait(false)) return true;

            if (!isSilent)
                MessageBox.Show(Resources.NoMatch, Resources.Error, MessageBoxButton.OK,
                    MessageBoxImage.Question, MessageBoxResult.OK);
            return false;
        }

        if (!isSilent)
            MessageBox.Show(Resources.NoValGame, Resources.Error, MessageBoxButton.OK,
                MessageBoxImage.Question, MessageBoxResult.OK);
        return false;
    }

    public static async Task<LiveMatchResponse> GetLiveMatchDetailsAsync()
    {
        var url = $"https://glz-{Constants.Shard}-1.{Constants.Region}.a.pvp.net/core-game/v1/matches/{Matchid}";
        RestClient client = new(url);
        RestRequest request = new();
        request.AddHeader("X-Riot-Entitlements-JWT", Constants.EntitlementToken);
        request.AddHeader("Authorization", $"Bearer {Constants.AccessToken}");
        var response = await client.ExecuteGetAsync<LiveMatchResponse>(request).ConfigureAwait(false);
        if (response.IsSuccessful) return response.Data;
        Constants.Log.Error("GetLiveMatchID() failed. Response: {Response}", response.ErrorException);
        return null;
    }

    public async Task<List<Player>> LiveMatchOutputAsync(UpdateProgress updateProgress)
    {
        var playerList = new List<Player>();
        var playerTasks = new List<Task<Player>>();
        var seasonData = new SeasonData();
        var presencesResponse = new PresencesResponse();

        var MatchIDInfo = await GetLiveMatchDetailsAsync().ConfigureAwait(false);

        if (MatchIDInfo != null)
        {
            Task sTask = Task.Run(async () => seasonData = await GetSeasonsAsync().ConfigureAwait(false));
            Task pTask = Task.Run(async () => presencesResponse = await GetPresencesAsync().ConfigureAwait(false));
            await Task.WhenAll(sTask, pTask).ConfigureAwait(false);
            sbyte index = 0;

            foreach (var riotPlayer in MatchIDInfo.Players)
            {
                if (!riotPlayer.IsCoach)
                {
                    async Task<Player> GetPlayerInfo()
                    {
                        var progress = 10;
                        // updateProgress(90 / 10 + progress);
                        // progress += 90 / 10;
                        Player player = new();
                        Debug.WriteLine(index);

                        var t1 = GetAgentInfoAsync(riotPlayer.CharacterId);
                        var t3 = GetCompHistoryAsync(riotPlayer.Subject);
                        var t4 = GetPlayerHistoryAsync(riotPlayer.Subject, seasonData);
                        var t5 = GetSkinInfoAsync(index);
                        var t6 = GetPresenceInfoAsync(riotPlayer.Subject, presencesResponse);

                        await Task.WhenAll(t1, t3, t4, t5, t6).ConfigureAwait(false);

                        player.AgentData = await t1.ConfigureAwait(false);
                        player.MatchHistoryData = await t3.ConfigureAwait(false);
                        player.RankData = await t4;
                        player.SkinData = await t5;
                        player.PlayerUiData = await t6;
                        player.IgnData = await GetIgcUsernameAsync(riotPlayer.Subject, riotPlayer.PlayerIdentity.Incognito, player.PlayerUiData.PartyUuid).ConfigureAwait(false);
                        player.AccountLevel = riotPlayer.PlayerIdentity.AccountLevel;
                        player.Active = Visibility.Visible;
                        return player;
                    }

                    playerTasks.Add(GetPlayerInfo());
                }

                index++;
            }

            var gamePod = MatchIDInfo.GamePodId;
            if (Constants.GamePodsDictionary.TryGetValue(gamePod, out var serverName)) MatchInfo.Server = "🌍 " + serverName;
            var results = await Task.WhenAll(playerTasks).ConfigureAwait(false);
            playerList.AddRange(results);
        }

        var colours = new List<string>
            {"Red", "#32e2b2", "DarkOrange", "White", "DeepSkyBlue", "MediumPurple", "SaddleBrown"};

        List<string> newArray = new();
        newArray.AddRange(Enumerable.Repeat("Transparent", playerList.Count));

        try
        {
            for (var i = 0; i < playerList.Count; i++)
            {
                if (playerList[i].PlayerUiData is null) continue;
                var colourused = false;
                var id = playerList[i].PlayerUiData.PartyUuid;
                for (var j = i + 1; j < playerList.Count; j++)
                {
                    if (playerList[j].PlayerUiData is null) continue;
                    if (playerList[j].PlayerUiData?.PartyUuid != id || id == Guid.Empty) continue;
                    newArray[i] = newArray[j] = colours[0];
                    colourused = true;
                }

                if (colourused) colours.RemoveAt(0);
            }

            for (var i = 0; i < playerList.Count; i++) playerList[i].PartyColour = newArray[i];
        }
        catch (Exception)
        {
            Constants.Log.Error("LiveMatchOutputAsync() party colour  failed.");
        }


        return playerList;
    }

    private static async Task<IgnData> GetIgcUsernameAsync(Guid puuid, bool isIncognito, Guid partyId)
    {
        IgnData ignData = new();
        if (isIncognito && partyId != Constants.PPartyId)
        {
            ignData.Username = "----";
            ignData.TrackerEnabled = Visibility.Hidden;
            ignData.TrackerDisabled = Visibility.Visible;
        }
        else
        {
            ignData.Username = await GetNameServiceGetUsernameAsync(puuid).ConfigureAwait(false);
            var trackerUri = await TrackerAsync(ignData.Username).ConfigureAwait(false);
            if (trackerUri != null)
            {
                ignData.TrackerEnabled = Visibility.Visible;
                ignData.TrackerDisabled = Visibility.Collapsed;
                ignData.TrackerUri = trackerUri;
            }
            else
            {
                ignData.TrackerEnabled = Visibility.Hidden;
                ignData.TrackerDisabled = Visibility.Visible;
            }
        }

        return ignData;
    }

    private static async Task<AgentData> GetAgentInfoAsync(Guid agentid)
    {
        AgentData agentData = new();
        if (agentid != Guid.Empty)
        {
            agentData.Image = new Uri(Constants.LocalAppDataPath + $"\\ValAPI\\agentsimg\\{agentid}.png");
            var agents = JsonSerializer.Deserialize<Dictionary<Guid, string>>(await File.ReadAllTextAsync(Constants.LocalAppDataPath + "\\ValAPI\\agents.txt").ConfigureAwait(false));
            agents.TryGetValue(agentid, out var agentName);
            agentData.Name = agentName;
        }
        else
        {
            Constants.Log.Error("GetAgentInfoAsync Failed: AgentID is empty");
            agentData.Image = null;
            agentData.Name = null;
        }

        return agentData;
    }

    private static async Task<SkinData> GetSkinInfoAsync(sbyte playerno)
    {
        SkinData skinData = new();
        var response = await DoCachedRequestAsync(Method.Get,
            $"https://glz-{Constants.Shard}-1.{Constants.Region}.a.pvp.net/core-game/v1/matches/{Matchid}/loadouts",
            true).ConfigureAwait(false);
        if (response.IsSuccessful)
        {
            var content = JsonSerializer.Deserialize<MatchLoadoutsResponse>(response.Content);
            var vandalchroma = content.Loadouts[playerno].Loadout
                .Items["9c82e19d-4575-0200-1a81-3eacf00cf872"].Sockets["3ad1b2b2-acdb-4524-852f-954a76ddae0a"]
                .Item.Id;
            var phantomchroma = content.Loadouts[playerno].Loadout
                .Items["ee8e8d15-496b-07ac-e5f6-8fae5d4c7b1a"].Sockets["3ad1b2b2-acdb-4524-852f-954a76ddae0a"]
                .Item.Id;

            var skins = JsonSerializer.Deserialize<Dictionary<Guid, ValSkin>>(await File.ReadAllTextAsync(Constants.LocalAppDataPath + "\\ValAPI\\skinchromas.txt").ConfigureAwait(false));

            skins.TryGetValue(phantomchroma, out var phantom);
            skins.TryGetValue(vandalchroma, out var vandal);

            skinData.PhantomName = phantom.Name;
            skinData.VandalName = vandal.Name;

            skinData.VandalImage = vandalchroma == new Guid("19629ae1-4996-ae98-7742-24a240d41f99") ? new Uri("pack://application:,,,/Assets/vandal.png") : vandal.Image;

            skinData.PhantomImage = phantomchroma == new Guid("52221ba2-4e4c-ec76-8c81-3483506d5242") ? new Uri("pack://application:,,,/Assets/phantom.png") : phantom.Image;
        }
        else
        {
            Constants.Log.Error("GetSkinInfoAsync Failed: {e}", response.ErrorException);
        }

        return skinData;
    }

    private static async Task<MatchHistoryData> GetCompHistoryAsync(Guid puuid)
    {
        MatchHistoryData history = new();
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
                    return history;
                }

                var options = new JsonSerializerOptions
                {
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
                };
                var content = JsonSerializer.Deserialize<CompetitiveUpdatesResponse>(response.Content, options);

                if (content?.Matches.Length > 0)
                {
                    history.RankProgress = content.Matches[0].RankedRatingAfterUpdate;
                    var pmatch = content.Matches[0].RankedRatingEarned;
                    history.PreviousGame = pmatch;
                    history.PreviousGameColour = pmatch switch
                    {
                        > 0 => "#32e2b2",
                        < 0 => "#ff4654",
                        _ => "#7f7f7f"
                    };
                }

                if (content?.Matches.Length > 1)
                {
                    var ppmatch = content.Matches[1].RankedRatingEarned;
                    history.PreviouspreviousGame = ppmatch;
                    history.PreviouspreviousGameColour = ppmatch switch
                    {
                        > 0 => "#32e2b2",
                        < 0 => "#ff4654",
                        _ => "#7f7f7f"
                    };
                }

                if (content?.Matches.Length > 2)
                {
                    var pppmatch = content.Matches[2].RankedRatingEarned;
                    history.PreviouspreviouspreviousGame = pppmatch;
                    history.PreviouspreviouspreviousGameColour = pppmatch switch
                    {
                        > 0 => "#32e2b2",
                        < 0 => "#ff4654",
                        _ => "#7f7f7f"
                    };
                }
            }
            else
            {
                Constants.Log.Error("GetCompHistoryAsync Puuid is null");
            }
        }
        catch (Exception e)
        {
            Constants.Log.Error("GetCompHistoryAsync failed: {e}", e);
        }

        return history;
    }

    private static async Task<RankData> GetPlayerHistoryAsync(Guid puuid, SeasonData seasonData)
    {
        Debug.WriteLine(puuid);
        var rankData = new RankData();
        if (puuid != Guid.Empty)
        {
            int rank, prank, pprank, ppprank;
            var response = await DoCachedRequestAsync(Method.Get,
                $"https://pd.{Constants.Region}.a.pvp.net/mmr/v1/players/{puuid}",
                true,
                true).ConfigureAwait(false);

            if (!response.IsSuccessful)
            {
                Constants.Log.Error("GetPlayerHistoryAsync Failed: {e}", response.ErrorException);
                return rankData;
            }

            var options = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            };
            var content = JsonSerializer.Deserialize<MmrResponse>(response.Content, options);
            // var content = JsonSerializer.Deserialize<Dictionary<Guid, ActInfo>>(allcontent.QueueSkills.Competitive.SeasonalInfoBySeasonId.Act.Keys);
            try
            {
                content.QueueSkills.Competitive.SeasonalInfoBySeasonId.Act.TryGetValue(seasonData.CurrentSeason.ToString(), out var currentActJsonElement);
                var CurrentAct = currentActJsonElement.Deserialize<ActInfo>();
                rank = CurrentAct.CompetitiveTier;
                if (rank is 1 or 2) rank = 0;
            }
            catch (Exception)
            {
                rank = 0;
            }

            try
            {
                content.QueueSkills.Competitive.SeasonalInfoBySeasonId.Act.TryGetValue(seasonData.PreviousSeason.ToString(), out var pActJsonElement);
                var PAct = pActJsonElement.Deserialize<ActInfo>();
                prank = PAct.CompetitiveTier;
                if (prank is 1 or 2) prank = 0;
            }
            catch (Exception)
            {
                prank = 0;
            }

            try
            {
                content.QueueSkills.Competitive.SeasonalInfoBySeasonId.Act.TryGetValue(seasonData.PreviouspreviousSeason.ToString(), out var ppActJsonElement);
                var PPAct = ppActJsonElement.Deserialize<ActInfo>();
                pprank = PPAct.CompetitiveTier;
                if (pprank is 1 or 2) pprank = 0;
            }
            catch (Exception)
            {
                pprank = 0;
            }

            try
            {
                content.QueueSkills.Competitive.SeasonalInfoBySeasonId.Act.TryGetValue(seasonData.PreviouspreviouspreviousSeason.ToString(), out var pppActJsonElement);
                var PPPAct = pppActJsonElement.Deserialize<ActInfo>();
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
                    $"https://pd.{Constants.Shard}.a.pvp.net/mmr/v1/leaderboards/affinity/{Constants.Region}/queue/competitive/season/{seasonData.CurrentSeason}?startIndex=0&size=0",
                    true).ConfigureAwait(false);
                var content2 = JsonSerializer.Deserialize<LeaderboardsResponse>(response2.Content);
                rankData.MaxRr = rank switch
                {
                    21 => content2.TierDetails["22"].RankedRatingThreshold,
                    22 => content2.TierDetails["23"].RankedRatingThreshold,
                    23 => content2.TierDetails["24"].RankedRatingThreshold,
                    _ => 100
                };
            }

            var ranks = JsonSerializer.Deserialize<Dictionary<int, string>>(await File.ReadAllTextAsync(Constants.LocalAppDataPath + "\\ValAPI\\competitivetiers.txt").ConfigureAwait(false));

            ranks.TryGetValue(rank, out var rank0);
            rankData.RankImage = new Uri(Constants.LocalAppDataPath + $"\\ValAPI\\ranksimg\\{rank}.png");
            rankData.RankName = rank0;

            ranks.TryGetValue(prank, out var rank1);
            rankData.PreviousrankImage = new Uri(Constants.LocalAppDataPath + $"\\ValAPI\\ranksimg\\{prank}.png");
            rankData.PreviousrankName = rank1;

            ranks.TryGetValue(pprank, out var rank2);
            rankData.PreviouspreviousrankImage = new Uri(Constants.LocalAppDataPath + $"\\ValAPI\\ranksimg\\{pprank}.png");
            rankData.PreviouspreviousrankName = rank2;

            ranks.TryGetValue(ppprank, out var rank3);
            rankData.PreviouspreviouspreviousrankImage = new Uri(Constants.LocalAppDataPath + $"\\ValAPI\\ranksimg\\{ppprank}.png");
            rankData.PreviouspreviouspreviousrankName = rank3;
        }
        else
        {
            Constants.Log.Error("GetPlayerHistoryAsync Failed: PUUID is empty");
        }

        return rankData;
    }

    private static async Task<SeasonData> GetSeasonsAsync()
    {
        var seasonData = new SeasonData();
        try
        {
            RestClient client = new($"https://shared.{Constants.Region}.a.pvp.net/content-service/v3/content");
            var request = new RestRequest().AddHeader("X-Riot-Entitlements-JWT", Constants.EntitlementToken)
                .AddHeader("Authorization", $"Bearer {Constants.AccessToken}")
                .AddHeader("X-Riot-ClientPlatform",
                    "ew0KCSJwbGF0Zm9ybVR5cGUiOiAiUEMiLA0KCSJwbGF0Zm9ybU9TIjogIldpbmRvd3MiLA0KCSJwbGF0Zm9ybU9TVmVyc2lvbiI6ICIxMC4wLjE5MDQyLjEuMjU2LjY0Yml0IiwNCgkicGxhdGZvcm1DaGlwc2V0IjogIlVua25vd24iDQp9")
                .AddHeader("X-Riot-ClientVersion", Constants.Version);
            client.UseSystemTextJson(new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            });
            var response = await client.ExecuteGetAsync<ContentResponse>(request).ConfigureAwait(false);
            sbyte index = 0;
            sbyte currentindex = 0;

            if (!response.IsSuccessful)
            {
                Constants.Log.Error("GetSeasonsAsync Failed: {e}", response.ErrorException);
                return seasonData;
            }

            foreach (var season in response.Data.Seasons)
            {
                if (season.IsActive & (season.Type == "act"))
                {
                    seasonData.CurrentSeason = season.Id;
                    currentindex = index;
                    break;
                }

                index++;
            }

            currentindex--;
            if (response.Data.Seasons[currentindex].Type == "act")
            {
                seasonData.PreviousSeason = response.Data.Seasons[currentindex].Id;
            }
            else
            {
                currentindex--;
                seasonData.PreviousSeason = response.Data.Seasons[currentindex].Id;
            }

            currentindex--;
            if (response.Data.Seasons[currentindex].Type == "act")
            {
                seasonData.PreviouspreviousSeason = response.Data.Seasons[currentindex].Id;
            }
            else
            {
                currentindex--;
                seasonData.PreviouspreviousSeason = response.Data.Seasons[currentindex].Id;
            }

            currentindex--;
            if (response.Data.Seasons[currentindex].Type == "act")
            {
                seasonData.PreviouspreviouspreviousSeason = response.Data.Seasons[currentindex].Id;
            }
            else
            {
                currentindex--;
                seasonData.PreviouspreviouspreviousSeason = response.Data.Seasons[currentindex].Id;
            }
        }
        catch (Exception e)
        {
            Constants.Log.Error("GetSeasonsAsync Failed: {Exception}", e);
        }

        return seasonData;
    }


    private static async Task<Uri> TrackerAsync(string username)
    {
        try
        {
            if (!string.IsNullOrEmpty(username))
            {
                var encodedUsername = Uri.EscapeDataString(username);
                var url = new Uri("https://tracker.gg/valorant/profile/riot/" + encodedUsername);

                RestClient client = new(url);
                RestRequest request = new();
                var response = await client.ExecuteAsync(request).ConfigureAwait(false);
                var numericStatusCode = (short) response.StatusCode;

                if (numericStatusCode == 200) return url;
            }
        }
        catch (Exception e)
        {
            Constants.Log.Error("TrackerAsync Failed: {Exception}", e);
        }

        return null;
    }


    private static async Task<PresencesResponse> GetPresencesAsync()
    {
        var options = new RestClientOptions($"https://127.0.0.1:{Constants.Port}/chat/v4/presences")
        {
            RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true
        };
        var client = new RestClient(options);

        var request = new RestRequest().AddHeader("Authorization",
                $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes($"riot:{Constants.LPassword}"))}")
            .AddHeader("X-Riot-ClientPlatform",
                "ew0KCSJwbGF0Zm9ybVR5cGUiOiAiUEMiLA0KCSJwbGF0Zm9ybU9TIjogIldpbmRvd3MiLA0KCSJwbGF0Zm9ybU9TVmVyc2lvbiI6ICIxMC4wLjE5MDQyLjEuMjU2LjY0Yml0IiwNCgkicGxhdGZvcm1DaGlwc2V0IjogIlVua25vd24iDQp9")
            .AddHeader("X-Riot-ClientVersion", Constants.Version);
        client.UseSystemTextJson(new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
        });
        var response = await client.ExecuteGetAsync<PresencesResponse>(request).ConfigureAwait(false);
        if (response.IsSuccessful)
            return response.Data;
        Constants.Log.Error("GetPresencesAsync Failed: {e}", response.ErrorException);
        return null;
    }

    private async Task<PlayerUIData> GetPresenceInfoAsync(Guid puuid, PresencesResponse presences)
    {
        PlayerUIData playerUiData = new();
        try
        {
            foreach (var friend in presences.Presences)
                if (friend.Puuid == puuid)
                {
                    var json = Encoding.UTF8.GetString(Convert.FromBase64String(friend.Private));
                    var content = JsonSerializer.Deserialize<PresencesPrivate>(json);
                    playerUiData.PartyUuid = content.PartyId;
                    if (puuid == Constants.Ppuuid)
                    {
                        var maps = JsonSerializer.Deserialize<Dictionary<string, ValMap>>(await File.ReadAllTextAsync(Constants.LocalAppDataPath + "\\ValAPI\\maps.txt").ConfigureAwait(false));

                        maps.TryGetValue(content.MatchMap, out var map);
                        MatchInfo.Map = map.Name;
                        MatchInfo.MapImage = new Uri(Constants.LocalAppDataPath + $"\\ValAPI\\mapsimg\\{map.UUID}.png");
                        playerUiData.BackgroundColour = "#181E34";
                        Constants.PPartyId = content.PartyId;

                        if (content?.ProvisioningFlow == "CustomGame")
                        {
                            MatchInfo.GameMode = "🏅 Custom";
                        }
                        else
                        {
                            var textInfo = new CultureInfo("en-US", false).TextInfo;
                            MatchInfo.GameMode = "🏅 " + content?.QueueId switch
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
                        playerUiData.BackgroundColour = "#252A40";
                    }

                    break;
                }
        }
        catch (Exception e)
        {
            Debugger.Break();
            Constants.Log.Error("GetPresenceInfoAsync Failed: {Exception}", e);
        }

        return playerUiData;
    }
}