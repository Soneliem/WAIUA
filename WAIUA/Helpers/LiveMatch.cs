using System;
using System.Collections.Generic;
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

public class LiveMatch
{
    public delegate void UpdateProgress(int percentage);

    public MatchDetails MatchInfo { get; } = new();
    private static Guid Matchid { get; set; }
    private static Guid Partyid { get; set; }
    private static string Stage { get; set; }
    public string QueueId { get; set; }
    public string Status { get; set; }

    private static async Task<bool> CheckAndSetLiveMatchIdAsync()
    {
        var client = new RestClient($"https://glz-{Constants.Shard}-1.{Constants.Region}.a.pvp.net/core-game/v1/players/{Constants.Ppuuid}");
        var request = new RestRequest();
        request.AddHeader("X-Riot-Entitlements-JWT", Constants.EntitlementToken);
        request.AddHeader("Authorization", $"Bearer {Constants.AccessToken}");             
        var response = await client.ExecuteGetAsync<MatchIDResponse>(request).ConfigureAwait(false);
        if (response.IsSuccessful)
        {
            Matchid = response.Data.MatchId;
            Stage = "core";
            return true;
        }

        client = new RestClient($"https://glz-{Constants.Shard}-1.{Constants.Region}.a.pvp.net/pregame/v1/players/{Constants.Ppuuid}");
        response = await client.ExecuteGetAsync<MatchIDResponse>(request).ConfigureAwait(false);
        if (response.IsSuccessful)
        {
            Matchid = response.Data.MatchId;
            Stage = "pre";
            return true;
        }

        Constants.Log.Error("CheckAndSetLiveMatchIdAsync() failed. Response: {Response}", response.ErrorException);
        return false;
    }

    public async Task<bool> CheckAndSetPartyIdAsync()
    {
        var client = new RestClient($"https://glz-{Constants.Shard}-1.{Constants.Region}.a.pvp.net/parties/v1/players/{Constants.Ppuuid}");
        var request = new RestRequest();
        request.AddHeader("X-Riot-Entitlements-JWT", Constants.EntitlementToken);
        request.AddHeader("Authorization", $"Bearer {Constants.AccessToken}");
        request.AddHeader("X-Riot-ClientVersion", Constants.Version);
        var response = await client.ExecuteGetAsync<PartyIdResponse>(request).ConfigureAwait(false);
        if (!response.IsSuccessful) return false;
        Partyid = response.Data.CurrentPartyId;
        return true;
    }


    public static async Task<bool> LiveMatchChecksAsync()
    {
        if (await Checks.CheckLoginAsync().ConfigureAwait(false))
        {
            await LocalRegionAsync().ConfigureAwait(false);
            return await CheckAndSetLiveMatchIdAsync().ConfigureAwait(false);
        }

        if (!await Checks.CheckLocalAsync().ConfigureAwait(false)) return false;
        await LocalLoginAsync().ConfigureAwait(false);
        await Checks.CheckLoginAsync().ConfigureAwait(false);
        await LocalRegionAsync().ConfigureAwait(false);

        return await CheckAndSetLiveMatchIdAsync().ConfigureAwait(false);
    }

    private static async Task<LiveMatchResponse> GetLiveMatchDetailsAsync()
    {
        RestClient client = new($"https://glz-{Constants.Shard}-1.{Constants.Region}.a.pvp.net/core-game/v1/matches/{Matchid}");
        RestRequest request = new();
        request.AddHeader("X-Riot-Entitlements-JWT", Constants.EntitlementToken);
        request.AddHeader("Authorization", $"Bearer {Constants.AccessToken}");
        var response = await client.ExecuteGetAsync<LiveMatchResponse>(request).ConfigureAwait(false);
        if (response.IsSuccessful) return response.Data;
        Constants.Log.Error("GetLiveMatchDetailsAsync() failed. Response: {Response}", response.ErrorException);
        return null;
    }

    private static async Task<PreMatchResponse> GetPreMatchDetailsAsync()
    {
        RestClient client = new($"https://glz-{Constants.Shard}-1.{Constants.Region}.a.pvp.net/pregame/v1/matches/{Matchid}");
        RestRequest request = new();
        request.AddHeader("X-Riot-Entitlements-JWT", Constants.EntitlementToken);
        request.AddHeader("Authorization", $"Bearer {Constants.AccessToken}");
        var response = await client.ExecuteGetAsync<PreMatchResponse>(request).ConfigureAwait(false);
        if (response.IsSuccessful) return response.Data;
        Constants.Log.Error("GetPreMatchDetailsAsync() failed. Response: {Response}", response.ErrorException);
        return null;
    }

    private static async Task<PartyResponse> GetPartyDetailsAsync()
    {
        RestClient client = new($"https://glz-{Constants.Shard}-1.{Constants.Region}.a.pvp.net/parties/v1/parties/{Partyid}");
        RestRequest request = new();
        request.AddHeader("X-Riot-Entitlements-JWT", Constants.EntitlementToken);
        request.AddHeader("Authorization", $"Bearer {Constants.AccessToken}");
        var response = await client.ExecuteGetAsync<PartyResponse>(request).ConfigureAwait(false);
        if (response.IsSuccessful) return response.Data;
        Constants.Log.Error("GetPreMatchDetailsAsync() failed. Response: {Response}", response.ErrorException);
        return null;
    }

    public async Task<List<Player>> LiveMatchOutputAsync(UpdateProgress updateProgress)
    {
        var playerList = new List<Player>();
        var playerTasks = new List<Task<Player>>();
        var seasonData = new SeasonData();
        var presencesResponse = new PresencesResponse();

        if (Stage == "pre")
        {
            var matchIdInfo = await GetPreMatchDetailsAsync().ConfigureAwait(false);
            updateProgress(10);

            if (matchIdInfo != null)
            {
                Task sTask = Task.Run(async () => seasonData = await GetSeasonsAsync().ConfigureAwait(false));
                Task pTask = Task.Run(async () => presencesResponse = await GetPresencesAsync().ConfigureAwait(false));
                await Task.WhenAll(sTask, pTask).ConfigureAwait(false);
                sbyte index = 0;

                foreach (var riotPlayer in matchIdInfo.AllyTeam.Players)
                {
                    async Task<Player> GetPlayerInfo()
                    {
                        Player player = new();

                        var t1 = GetCardAsync(riotPlayer.PlayerIdentity.PlayerCardId, index);
                        var t3 = GetMatchHistoryAsync(riotPlayer.Subject);
                        var t4 = GetPlayerHistoryAsync(riotPlayer.Subject, seasonData);
                        var t5 = GetPreSkinInfoAsync(index);
                        var t6 = GetPresenceInfoAsync(riotPlayer.Subject, presencesResponse);

                        await Task.WhenAll(t1, t3, t4, t5, t6).ConfigureAwait(false);
                        // await Task.WhenAll(t1, t3, t5, t6).ConfigureAwait(false);

                        player.IdentityData = t1.Result;
                        player.MatchHistoryData = t3.Result;
                        player.RankData = t4.Result;
                        player.SkinData = t5.Result;
                        player.PlayerUiData = t6.Result;
                        player.IgnData = await GetIgcUsernameAsync(riotPlayer.Subject, riotPlayer.PlayerIdentity.Incognito, player.PlayerUiData.PartyUuid).ConfigureAwait(false);
                        player.AccountLevel = !riotPlayer.PlayerIdentity.HideAccountLevel ? riotPlayer.PlayerIdentity.AccountLevel.ToString() : "-";
                        player.TeamId = "Blue";
                        player.Active = Visibility.Visible;
                        return player;
                    }

                    playerTasks.Add(GetPlayerInfo());

                    index++;
                }

                var gamePodId = matchIdInfo.GamePodId;
                if (Constants.GamePodsDictionary.TryGetValue(gamePodId, out var serverName)) MatchInfo.Server = "🌍 " + serverName;
            }
        }
        else
        {
            var matchIdInfo = await GetLiveMatchDetailsAsync().ConfigureAwait(false);
            updateProgress(10);

            if (matchIdInfo != null)
            {
                Task sTask = Task.Run(async () => seasonData = await GetSeasonsAsync().ConfigureAwait(false));
                Task pTask = Task.Run(async () => presencesResponse = await GetPresencesAsync().ConfigureAwait(false));
                await Task.WhenAll(sTask, pTask).ConfigureAwait(false);
                sbyte index = 0;

                foreach (var riotPlayer in matchIdInfo.Players)
                {
                    if (!riotPlayer.IsCoach)
                    {
                        async Task<Player> GetPlayerInfo()
                        {
                            Player player = new();

                            var t1 = GetAgentInfoAsync(riotPlayer.CharacterId);
                            // var t2 = GetCompHistoryAsync(riotPlayer.Subject);
                            var t3 = GetPlayerHistoryAsync(riotPlayer.Subject, seasonData);
                            var t4 = GetMatchSkinInfoAsync(index);
                            var t5 = GetPresenceInfoAsync(riotPlayer.Subject, presencesResponse);

                            await Task.WhenAll(t1, t3, t4, t5).ConfigureAwait(false);
                            // await Task.WhenAll(t1, t2, t3, t4, t5).ConfigureAwait(false);

                            player.IdentityData = t1.Result;
                            // player.MatchHistoryData = t2.Result;
                            player.RankData = t3.Result;
                            player.SkinData = t4.Result;
                            player.PlayerUiData = t5.Result;
                            player.IgnData = await GetIgcUsernameAsync(riotPlayer.Subject, riotPlayer.PlayerIdentity.Incognito, player.PlayerUiData.PartyUuid).ConfigureAwait(false);
                            // player.RankData = await GetPlayerHistoryAsync(riotPlayer.Subject, seasonData).ConfigureAwait(false);
                            player.AccountLevel = !riotPlayer.PlayerIdentity.HideAccountLevel ? riotPlayer.PlayerIdentity.AccountLevel.ToString() : "-";
                            player.TeamId = riotPlayer.TeamId;
                            player.Active = Visibility.Visible;
                            return player;
                        }

                        playerTasks.Add(GetPlayerInfo());
                    }

                    index++;
                }

                var gamePodId = matchIdInfo.GamePodId;
                if (Constants.GamePodsDictionary.TryGetValue(gamePodId, out var serverName)) MatchInfo.Server = "🌍 " + serverName;
            }
        }

        playerList.AddRange(await Task.WhenAll(playerTasks).ConfigureAwait(false));
        updateProgress(75);

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
                for (var j = i; j < playerList.Count; j++)
                {
                    if (newArray[i] != "Transparent" || playerList[i] == playerList[j] || 
                        playerList[j].PlayerUiData?.PartyUuid != id || id == Guid.Empty) continue;
                    newArray[j] = colours[0];
                    colourused = true;
                }
                if (!colourused) continue;
                newArray[i] = colours[0];
                colours.RemoveAt(0);

            }

            for (var i = 0; i < playerList.Count; i++) playerList[i].PlayerUiData.PartyColour = newArray[i];
            updateProgress(100);
        }
        catch (Exception)
        {
            Constants.Log.Error("LiveMatchOutputAsync() party colour  failed.");
        }

        return playerList;
    }

    public async Task<List<Player>> PartyOutputAsync()
    {
        var playerList = new List<Player>();
        var playerTasks = new List<Task<Player>>();
        var partyInfo = await GetPartyDetailsAsync().ConfigureAwait(false);

        if (partyInfo != null)
        {
            var seasonData = await GetSeasonsAsync().ConfigureAwait(false);
            sbyte index = 0;

            foreach (var riotPlayer in partyInfo.Members)
            {
                async Task<Player> GetPlayerInfo()
                {
                    Player player = new();

                    var t1 = GetCardAsync(riotPlayer.PlayerIdentity.PlayerCardId, index);
                    var t3 = GetMatchHistoryAsync(riotPlayer.Subject);
                    var t4 = GetPlayerHistoryAsync(riotPlayer.Subject, seasonData);

                    await Task.WhenAll(t1, t3, t4).ConfigureAwait(false);

                    player.IdentityData = t1.Result;
                    player.MatchHistoryData = t3.Result;
                    player.RankData = t4.Result;
                    player.PlayerUiData = new PlayerUIData
                    {
                        BackgroundColour = "#252A40",
                        PartyUuid = Partyid,
                        PartyColour = "Transparent",
                        Puuid = riotPlayer.PlayerIdentity.Subject
                    };
                    player.IgnData = await GetIgcUsernameAsync(riotPlayer.Subject, false, player.PlayerUiData.PartyUuid).ConfigureAwait(false);
                    player.AccountLevel = !riotPlayer.PlayerIdentity.HideAccountLevel ? riotPlayer.PlayerIdentity.AccountLevel.ToString() : "-";
                    player.TeamId = "Blue";
                    player.Active = Visibility.Visible;
                    return player;
                }

                playerTasks.Add(GetPlayerInfo());
                index++;
            }
        }

        playerList.AddRange(await Task.WhenAll(playerTasks).ConfigureAwait(false));

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
                return new IgnData
                {
                    TrackerEnabled = Visibility.Visible,
                    TrackerDisabled = Visibility.Collapsed,
                    TrackerUri = trackerUri,
                    Username = ignData.Username + " 🔗"
                };
            ignData.TrackerEnabled = Visibility.Hidden;
            ignData.TrackerDisabled = Visibility.Visible;
        }

        return ignData;
    }

    private static async Task<IdentityData> GetAgentInfoAsync(Guid agentid)
    {
        IdentityData identityData = new();
        if (agentid != Guid.Empty)
        {
            identityData.Image = new Uri(Constants.LocalAppDataPath + $"\\ValAPI\\agentsimg\\{agentid}.png");
            var agents = JsonSerializer.Deserialize<Dictionary<Guid, string>>(await File.ReadAllTextAsync(Constants.LocalAppDataPath + "\\ValAPI\\agents.txt").ConfigureAwait(false));
            agents.TryGetValue(agentid, out var agentName);
            identityData.Name = agentName;
        }
        else
        {
            Constants.Log.Error("GetAgentInfoAsync Failed: AgentID is empty");
            identityData.Image = null;
            identityData.Name = "";
        }

        return identityData;
    }

    private static async Task<IdentityData> GetCardAsync(Guid cardid, sbyte index)
    {
        IdentityData identityData = new();
        if (cardid != Guid.Empty)
        {
            var cards = JsonSerializer.Deserialize<Dictionary<Guid, Uri>>(await File.ReadAllTextAsync(Constants.LocalAppDataPath + "\\ValAPI\\cards.txt").ConfigureAwait(false));
            cards.TryGetValue(cardid, out var card);
            identityData.Image = card;
            identityData.Name = Resources.Player + " " + (index + 1);
        }
        else
        {
            Constants.Log.Error("GetCardAsync Failed: CardID is empty");
            identityData.Image = null;
            identityData.Name = "";
        }

        return identityData;
    }

    private static async Task<SkinData> GetMatchSkinInfoAsync(sbyte playerno)
    {
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

            return await GetSkinInfoAsync(phantomchroma, vandalchroma);
        }

        Constants.Log.Error("GetMatchSkinInfoAsync Failed: {e}", response.ErrorException);
        return new SkinData();
    }

    private static async Task<SkinData> GetPreSkinInfoAsync(sbyte playerno)
    {
        var response = await DoCachedRequestAsync(Method.Get,
            $"https://glz-{Constants.Shard}-1.{Constants.Region}.a.pvp.net/pregame/v1/matches/{Matchid}/loadouts",
            true).ConfigureAwait(false);
        if (response.IsSuccessful)
            try
            {
                var content = JsonSerializer.Deserialize<PreMatchLoadoutsResponse>(response.Content);
                var vandalchroma = content.Loadouts[playerno]
                    .Items["9c82e19d-4575-0200-1a81-3eacf00cf872"].Sockets["3ad1b2b2-acdb-4524-852f-954a76ddae0a"]
                    .Item.Id;
                var phantomchroma = content.Loadouts[playerno]
                    .Items["ee8e8d15-496b-07ac-e5f6-8fae5d4c7b1a"].Sockets["3ad1b2b2-acdb-4524-852f-954a76ddae0a"]
                    .Item.Id;

                return await GetSkinInfoAsync(phantomchroma, vandalchroma);
            }
            catch
            {
                // ignored
            }

        Constants.Log.Error("GetPreSkinInfoAsync Failed: {e}", response.ErrorException);
        return new SkinData();
    }

    private static async Task<SkinData> GetSkinInfoAsync(Guid phantomchroma, Guid vandalchroma)
    {
        var skins = JsonSerializer.Deserialize<Dictionary<Guid, ValSkin>>(await File.ReadAllTextAsync(Constants.LocalAppDataPath + "\\ValAPI\\skinchromas.txt").ConfigureAwait(false));

        skins.TryGetValue(phantomchroma, out var phantom);
        skins.TryGetValue(vandalchroma, out var vandal);

        return new SkinData
        {
            PhantomImage = phantomchroma == new Guid("52221ba2-4e4c-ec76-8c81-3483506d5242") ? new Uri("pack://application:,,,/Assets/phantom.png") : phantom.Image,
            PhantomName = phantom?.Name,
            VandalImage = vandalchroma == new Guid("19629ae1-4996-ae98-7742-24a240d41f99") ? new Uri("pack://application:,,,/Assets/vandal.png") : vandal.Image,
            VandalName = vandal?.Name
        };
    }


    public static async Task<MatchHistoryData> GetMatchHistoryAsync(Guid puuid)
    {
        MatchHistoryData history = new()
        {
            PreviousGameColour = "#7f7f7f",
            PreviouspreviousGameColour = "#7f7f7f",
            PreviouspreviouspreviousGameColour = "#7f7f7f"
        };

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
                    history.PreviousGame = Math.Abs(pmatch);
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
                    history.PreviouspreviousGame = Math.Abs(ppmatch);
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
                    history.PreviouspreviouspreviousGame = Math.Abs(pppmatch);
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
                Constants.Log.Error("GetMatchHistoryAsync: Puuid is null");
            }
        }
        catch (Exception e)
        {
            Constants.Log.Error("GetMatchHistoryAsync failed: {e}", e);
        }

        return history;
    }

    private static async Task<RankData> GetPlayerHistoryAsync(Guid puuid, SeasonData seasonData)
    {
        var rankData = new RankData();
        if (puuid != Guid.Empty)
        {
            int rank = 0, prank = 0, pprank = 0, ppprank = 0;
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
                var currentAct = currentActJsonElement.Deserialize<ActInfo>();
                rank = currentAct.CompetitiveTier;
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
                switch (PAct.CompetitiveTier)
                {
                    case 1 or 2:
                        prank = 0;
                        break;
                    case > 20:
                    {
                        if (Constants.BeforeAscendantSeasons.Contains(seasonData.PreviousSeason))
                        {
                            prank = PAct.CompetitiveTier + 3;
                        }
                        else
                        {
                            prank = PAct.CompetitiveTier;
                        }
                        break;
                    }
                    default:
                        prank = PAct.CompetitiveTier;
                        break;
                }
            }
            catch (Exception)
            {
                prank = 0;
            }

            try
            {
                content.QueueSkills.Competitive.SeasonalInfoBySeasonId.Act.TryGetValue(seasonData.PreviouspreviousSeason.ToString(), out var ppActJsonElement);
                var PPAct = ppActJsonElement.Deserialize<ActInfo>();
                switch (PPAct.CompetitiveTier)
                {
                    case 1 or 2:
                        pprank = 0;
                        break;
                    case > 20:
                    {
                        if (Constants.BeforeAscendantSeasons.Contains(seasonData.PreviouspreviousSeason))
                        {
                            pprank = PPAct.CompetitiveTier + 3;
                        }
                        else
                        {
                            pprank  = PPAct.CompetitiveTier;
                        }
                        break;
                    }
                    default:
                        pprank = PPAct.CompetitiveTier;
                        break;
                }
            }
            catch (Exception)
            {
                pprank = 0;
            }

            try
            {
                content.QueueSkills.Competitive.SeasonalInfoBySeasonId.Act.TryGetValue(seasonData.PreviouspreviouspreviousSeason.ToString(), out var pppActJsonElement);
                var PPPAct = pppActJsonElement.Deserialize<ActInfo>();
                switch (PPPAct.CompetitiveTier)
                {
                    case 1 or 2:
                        ppprank = 0;
                        break;
                    case > 20:
                    {
                        if (Constants.BeforeAscendantSeasons.Contains(seasonData.PreviouspreviouspreviousSeason))
                        {
                            ppprank = PPPAct.CompetitiveTier + 3;
                        }
                        else
                        {
                            ppprank = PPPAct.CompetitiveTier;
                        }
                        break;
                    }
                    default:
                        ppprank = PPPAct.CompetitiveTier;
                        break;
                }
            }
            catch (Exception)
            {
                ppprank = 0;
            }

            if (rank is 24 or 25 or 26)
            {
                var leaderboardResponse = await DoCachedRequestAsync(Method.Get,
                    $"https://pd.{Constants.Shard}.a.pvp.net/mmr/v1/leaderboards/affinity/{Constants.Region}/queue/competitive/season/{seasonData.CurrentSeason}?startIndex=0&size=0",
                    true).ConfigureAwait(false);
                if (leaderboardResponse.Content != null)
                {
                    var leaderboardcontent = JsonSerializer.Deserialize<LeaderboardsResponse>(leaderboardResponse.Content);
                    rankData.MaxRr = rank switch
                    {
                        24 => leaderboardcontent.TierDetails["22"].RankedRatingThreshold,
                        25 => leaderboardcontent.TierDetails["23"].RankedRatingThreshold,
                        26 => leaderboardcontent.TierDetails["24"].RankedRatingThreshold,
                        _ => 100
                    };
                }
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
        PlayerUIData playerUiData = new()
        {
            BackgroundColour = "#252A40",
            Puuid = puuid
        };
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
                            MatchInfo.GameMode = "Custom";
                            MatchInfo.GameModeImage = new Uri(Constants.LocalAppDataPath + "\\ValAPI\\gamemodeimg\\96bd3920-4f36-d026-2b28-c683eb0bcac5.png");
                        }
                        else
                        {
                            var textInfo = new CultureInfo("en-US", false).TextInfo;

                            var gameModeName = "";
                            var gameModeId = Guid.Parse("96bd3920-4f36-d026-2b28-c683eb0bcac5");
                            QueueId = content?.QueueId;
                            Status = content?.SessionLoopState;

                            switch (content?.QueueId)
                            {
                                case "competitive":
                                    gameModeName = "Competitive";
                                    break;
                                case "unrated":
                                    gameModeName = "Unrated";
                                    break;
                                case "deathmatch":
                                    gameModeId = Guid.Parse("a8790ec5-4237-f2f0-e93b-08a8e89865b2");
                                    break;
                                case "spikerush":
                                    gameModeId = Guid.Parse("e921d1e6-416b-c31f-1291-74930c330b7b");
                                    break;
                                case "ggteam":
                                    gameModeId = Guid.Parse("a4ed6518-4741-6dcb-35bd-f884aecdc859");
                                    break;
                                case "newmap":
                                    gameModeName = "New Map";
                                    break;
                                case "onefa":
                                    gameModeId = Guid.Parse("96bd3920-4f36-d026-2b28-c683eb0bcac5");
                                    break;
                                case "snowball":
                                    gameModeId = Guid.Parse("57038d6d-49b1-3a74-c5ef-3395d9f23a97");
                                    break;
                                default:
                                    gameModeName = textInfo.ToTitleCase(content.QueueId);
                                    break;
                            }


                            if (gameModeName == "")
                            {
                                var gamemodes = JsonSerializer.Deserialize<Dictionary<Guid, string>>(await File.ReadAllTextAsync(Constants.LocalAppDataPath + "\\ValAPI\\gamemode.txt").ConfigureAwait(false));
                                gamemodes.TryGetValue(gameModeId, out var gamemode);
                                MatchInfo.GameMode = gamemode;
                            }
                            else
                            {
                                MatchInfo.GameMode = gameModeName;
                            }

                            MatchInfo.GameModeImage = new Uri(Constants.LocalAppDataPath + $"\\ValAPI\\gamemodeimg\\{gameModeId}.png");
                        }
                    }

                    break;
                }
        }
        catch (Exception e)
        {
            Constants.Log.Error("GetPresenceInfoAsync Failed: {Exception}", e);
        }

        return playerUiData;
    }
}