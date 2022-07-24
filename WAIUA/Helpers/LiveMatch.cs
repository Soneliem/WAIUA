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
                            try
                            {
                                var t1 = GetCardAsync(riotPlayer.PlayerIdentity.PlayerCardId, index);
                                var t4 = GetPlayerHistoryAsync(riotPlayer.Subject, seasonData);
                                var t6 = GetPresenceInfoAsync(riotPlayer.Subject, presencesResponse);

                                await Task.WhenAll(t1, t4, t6).ConfigureAwait(false);

                                player.IdentityData = t1.Result;
                                player.RankData = t4.Result;
                                player.PlayerUiData = t6.Result;
                                player.IgnData = await GetIgcUsernameAsync(riotPlayer.Subject, riotPlayer.PlayerIdentity.Incognito, player.PlayerUiData.PartyUuid).ConfigureAwait(false);
                                player.AccountLevel = !riotPlayer.PlayerIdentity.HideAccountLevel ? riotPlayer.PlayerIdentity.AccountLevel.ToString() : "-";
                                player.TeamId = "Blue";
                                player.Active = Visibility.Visible;
                            }
                            catch(Exception e)
                            {
                                Constants.Log.Error("GetPlayerInfo() (PRE) failed for player {index}: {e}", index, e);
                            }
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
                    
                        if (riotPlayer.IsCoach) continue;

                        async Task<Player> GetPlayerInfo()
                        {
                            Player player = new();
                            try
                            {
                                var t1 = GetAgentInfoAsync(riotPlayer.CharacterId);
                                var t3 = GetPlayerHistoryAsync(riotPlayer.Subject, seasonData);
                                var t4 = GetMatchSkinInfoAsync(index, riotPlayer.PlayerIdentity.PlayerCardId);
                                var t5 = GetPresenceInfoAsync(riotPlayer.Subject, presencesResponse);

                                await Task.WhenAll(t1, t3, t4, t5).ConfigureAwait(false);

                                player.IdentityData = t1.Result;
                                player.RankData = t3.Result;
                                player.SkinData = t4.Result;
                                player.PlayerUiData = t5.Result;
                                player.IgnData = await GetIgcUsernameAsync(riotPlayer.Subject, riotPlayer.PlayerIdentity.Incognito, player.PlayerUiData.PartyUuid).ConfigureAwait(false);
                                player.AccountLevel = !riotPlayer.PlayerIdentity.HideAccountLevel ? riotPlayer.PlayerIdentity.AccountLevel.ToString() : "-";
                                player.TeamId = riotPlayer.TeamId;
                                player.Active = Visibility.Visible;
                            }
                            catch (Exception e)
                            {
                                Constants.Log.Error("GetPlayerInfo() (CORE) failed for player {index}: {e}", index, e);
                            }
                            return player;
                        }

                        playerTasks.Add(GetPlayerInfo());
                    

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
        catch (Exception e)
        {
            Constants.Log.Error("LiveMatchOutputAsync() party colour  failed: {e}", e);
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
        if (cardid != Guid.Empty)
        {
            var cards = JsonSerializer.Deserialize<Dictionary<Guid, ValNameImage>>(await File.ReadAllTextAsync(Constants.LocalAppDataPath + "\\ValAPI\\cards.txt").ConfigureAwait(false));
            cards.TryGetValue(cardid, out var card);
            return new IdentityData()
            {
                Image = card.Image, Name = Resources.Player + " " + (index + 1)
            };
        }
        Constants.Log.Error("GetCardAsync Failed: CardID is empty");
        return new IdentityData();
    }

    private static async Task<SkinData> GetMatchSkinInfoAsync(sbyte playerno, Guid cardid)
    {
        var response = await DoCachedRequestAsync(Method.Get,
            $"https://glz-{Constants.Shard}-1.{Constants.Region}.a.pvp.net/core-game/v1/matches/{Matchid}/loadouts",
            true).ConfigureAwait(false);
        if (response.IsSuccessful)
        {
            var content = JsonSerializer.Deserialize<MatchLoadoutsResponse>(response.Content);
            return await GetSkinInfoAsync(content.Loadouts[playerno].Loadout, cardid);
        }

        Constants.Log.Error("GetMatchSkinInfoAsync Failed: {e}", response.ErrorException);
        return new SkinData();
    }

    private static async Task<SkinData> GetPreSkinInfoAsync(sbyte playerno, Guid cardid)
    {
        var response = await DoCachedRequestAsync(Method.Get,
            $"https://glz-{Constants.Shard}-1.{Constants.Region}.a.pvp.net/pregame/v1/matches/{Matchid}/loadouts",
            true).ConfigureAwait(false);
        if (response.IsSuccessful)
            try
            {
                var content = JsonSerializer.Deserialize<PreMatchLoadoutsResponse>(response.Content);
                return await GetSkinInfoAsync(content.Loadouts[playerno], cardid);
            }
            catch
            {
                // ignored
            }

        Constants.Log.Error("GetPreSkinInfoAsync Failed: {e}", response.ErrorException);
        return new SkinData();
    }

    private static async Task<SkinData> GetSkinInfoAsync(LoadoutLoadout loadout, Guid cardid)
    {
        var skins = JsonSerializer.Deserialize<Dictionary<Guid, ValNameImage>>(await File.ReadAllTextAsync(Constants.LocalAppDataPath + "\\ValAPI\\skinchromas.txt").ConfigureAwait(false));
        var cards = JsonSerializer.Deserialize<Dictionary<Guid, ValNameImage>>(await File.ReadAllTextAsync(Constants.LocalAppDataPath + "\\ValAPI\\cards.txt").ConfigureAwait(false));
        var sprays = JsonSerializer.Deserialize<Dictionary<Guid, ValNameImage>>(await File.ReadAllTextAsync(Constants.LocalAppDataPath + "\\ValAPI\\sprays.txt").ConfigureAwait(false));

        return new SkinData()
        {
            CardImage = cards[cardid].Image,
            CardName = cards[cardid].Name,
            Spray1Image = sprays[loadout.Sprays.SpraySelections[0].SprayId].Image,
            Spray1Name = sprays[loadout.Sprays.SpraySelections[0].SprayId].Name,
            Spray2Image = sprays[loadout.Sprays.SpraySelections[1].SprayId].Image,
            Spray2Name = sprays[loadout.Sprays.SpraySelections[1].SprayId].Name,
            Spray3Image = sprays[loadout.Sprays.SpraySelections[2].SprayId].Image,
            Spray3Name = sprays[loadout.Sprays.SpraySelections[2].SprayId].Name,
            
            ClassicImage = skins[loadout.Items["29a0cfab-485b-f5d5-779a-b59f85e204a8"].Sockets["3ad1b2b2-acdb-4524-852f-954a76ddae0a"]
                .Item.Id].Image,
            ClassicName = skins[loadout.Items["29a0cfab-485b-f5d5-779a-b59f85e204a8"].Sockets["3ad1b2b2-acdb-4524-852f-954a76ddae0a"]
                .Item.Id].Name,
            ShortyImage = skins[loadout.Items["42da8ccc-40d5-affc-beec-15aa47b42eda"].Sockets["3ad1b2b2-acdb-4524-852f-954a76ddae0a"]
                .Item.Id].Image,
            ShortyName = skins[loadout.Items["42da8ccc-40d5-affc-beec-15aa47b42eda"].Sockets["3ad1b2b2-acdb-4524-852f-954a76ddae0a"]
                .Item.Id].Name,
            FrenzyImage = skins[loadout.Items["44d4e95c-4157-0037-81b2-17841bf2e8e3"].Sockets["3ad1b2b2-acdb-4524-852f-954a76ddae0a"]
                .Item.Id].Image,
            FrenzyName = skins[loadout.Items["44d4e95c-4157-0037-81b2-17841bf2e8e3"].Sockets["3ad1b2b2-acdb-4524-852f-954a76ddae0a"]
                .Item.Id].Name,
            GhostImage = skins[loadout.Items["1baa85b4-4c70-1284-64bb-6481dfc3bb4e"].Sockets["3ad1b2b2-acdb-4524-852f-954a76ddae0a"]
                .Item.Id].Image,
            GhostName = skins[loadout.Items["1baa85b4-4c70-1284-64bb-6481dfc3bb4e"].Sockets["3ad1b2b2-acdb-4524-852f-954a76ddae0a"]
                .Item.Id].Name,
            SheriffImage = skins[loadout.Items["e336c6b8-418d-9340-d77f-7a9e4cfe0702"].Sockets["3ad1b2b2-acdb-4524-852f-954a76ddae0a"]
                .Item.Id].Image,
            SheriffName = skins[loadout.Items["e336c6b8-418d-9340-d77f-7a9e4cfe0702"].Sockets["3ad1b2b2-acdb-4524-852f-954a76ddae0a"]
                .Item.Id].Name,  
            
            StingerImage = skins[loadout.Items["f7e1b454-4ad4-1063-ec0a-159e56b58941"].Sockets["3ad1b2b2-acdb-4524-852f-954a76ddae0a"]
                .Item.Id].Image,
            StingerName = skins[loadout.Items["f7e1b454-4ad4-1063-ec0a-159e56b58941"].Sockets["3ad1b2b2-acdb-4524-852f-954a76ddae0a"]
                .Item.Id].Name,
            SpectreImage = skins[loadout.Items["462080d1-4035-2937-7c09-27aa2a5c27a7"].Sockets["3ad1b2b2-acdb-4524-852f-954a76ddae0a"]
                .Item.Id].Image,
            SpectreName = skins[loadout.Items["462080d1-4035-2937-7c09-27aa2a5c27a7"].Sockets["3ad1b2b2-acdb-4524-852f-954a76ddae0a"]
                .Item.Id].Name,
            BuckyImage = skins[loadout.Items["910be174-449b-c412-ab22-d0873436b21b"].Sockets["3ad1b2b2-acdb-4524-852f-954a76ddae0a"]
                .Item.Id].Image,
            BuckyName = skins[loadout.Items["910be174-449b-c412-ab22-d0873436b21b"].Sockets["3ad1b2b2-acdb-4524-852f-954a76ddae0a"]
                .Item.Id].Name,      
            JudgeImage = skins[loadout.Items["ec845bf4-4f79-ddda-a3da-0db3774b2794"].Sockets["3ad1b2b2-acdb-4524-852f-954a76ddae0a"]
                .Item.Id].Image,
            JudgeName = skins[loadout.Items["ec845bf4-4f79-ddda-a3da-0db3774b2794"].Sockets["3ad1b2b2-acdb-4524-852f-954a76ddae0a"]
                .Item.Id].Name, 

            BulldogImage = skins[loadout.Items["ae3de142-4d85-2547-dd26-4e90bed35cf7"].Sockets["3ad1b2b2-acdb-4524-852f-954a76ddae0a"]
                .Item.Id].Image,
            BulldogName = skins[loadout.Items["ae3de142-4d85-2547-dd26-4e90bed35cf7"].Sockets["3ad1b2b2-acdb-4524-852f-954a76ddae0a"]
                .Item.Id].Name, 
            GuardianImage = skins[loadout.Items["4ade7faa-4cf1-8376-95ef-39884480959b"].Sockets["3ad1b2b2-acdb-4524-852f-954a76ddae0a"]
                .Item.Id].Image,
            GuardianName = skins[loadout.Items["4ade7faa-4cf1-8376-95ef-39884480959b"].Sockets["3ad1b2b2-acdb-4524-852f-954a76ddae0a"]
                .Item.Id].Name, 
            PhantomImage = skins[loadout.Items["ee8e8d15-496b-07ac-e5f6-8fae5d4c7b1a"].Sockets["3ad1b2b2-acdb-4524-852f-954a76ddae0a"]
                .Item.Id].Image,
            PhantomName = skins[loadout.Items["ee8e8d15-496b-07ac-e5f6-8fae5d4c7b1a"].Sockets["3ad1b2b2-acdb-4524-852f-954a76ddae0a"]
                .Item.Id].Name,
            VandalImage = skins[loadout.Items["9c82e19d-4575-0200-1a81-3eacf00cf872"].Sockets["3ad1b2b2-acdb-4524-852f-954a76ddae0a"]
                .Item.Id].Image,
            VandalName = skins[loadout.Items["9c82e19d-4575-0200-1a81-3eacf00cf872"].Sockets["3ad1b2b2-acdb-4524-852f-954a76ddae0a"]
                .Item.Id].Name, 
            
            MarshalImage = skins[loadout.Items["c4883e50-4494-202c-3ec3-6b8a9284f00b"].Sockets["3ad1b2b2-acdb-4524-852f-954a76ddae0a"]
                .Item.Id].Image,
            MarshalName = skins[loadout.Items["c4883e50-4494-202c-3ec3-6b8a9284f00b"].Sockets["3ad1b2b2-acdb-4524-852f-954a76ddae0a"]
                .Item.Id].Name, 
            OperatorImage = skins[loadout.Items["a03b24d3-4319-996d-0f8c-94bbfba1dfc7"].Sockets["3ad1b2b2-acdb-4524-852f-954a76ddae0a"]
                .Item.Id].Image,
            OperatorName = skins[loadout.Items["a03b24d3-4319-996d-0f8c-94bbfba1dfc7"].Sockets["3ad1b2b2-acdb-4524-852f-954a76ddae0a"]
                .Item.Id].Name, 
            AresImage = skins[loadout.Items["55d8a0f4-4274-ca67-fe2c-06ab45efdf58"].Sockets["3ad1b2b2-acdb-4524-852f-954a76ddae0a"]
                .Item.Id].Image,
            AresName = skins[loadout.Items["55d8a0f4-4274-ca67-fe2c-06ab45efdf58"].Sockets["3ad1b2b2-acdb-4524-852f-954a76ddae0a"]
                .Item.Id].Name, 
            OdinImage = skins[loadout.Items["63e6c2b6-4a8e-869c-3d4c-e38355226584"].Sockets["3ad1b2b2-acdb-4524-852f-954a76ddae0a"]
                .Item.Id].Image,
            OdinName = skins[loadout.Items["63e6c2b6-4a8e-869c-3d4c-e38355226584"].Sockets["3ad1b2b2-acdb-4524-852f-954a76ddae0a"]
                .Item.Id].Name, 
            MeleeImage = skins[loadout.Items["2f59173c-4bed-b6c3-2191-dea9b58be9c7"].Sockets["3ad1b2b2-acdb-4524-852f-954a76ddae0a"]
                .Item.Id].Image,
            MeleeName = skins[loadout.Items["2f59173c-4bed-b6c3-2191-dea9b58be9c7"].Sockets["3ad1b2b2-acdb-4524-852f-954a76ddae0a"]
                .Item.Id].Name,
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
                    true).ConfigureAwait(false);
                if (!response.IsSuccessful)
                {
                    Constants.Log.Error("GetMatchHistoryAsync request failed: {e}", response.ErrorException);
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

            if (rank is 24 or 25 or 26 or 27)
            {
                var leaderboardResponse = await DoCachedRequestAsync(Method.Get,
                    $"https://pd.{Constants.Shard}.a.pvp.net/mmr/v1/leaderboards/affinity/{Constants.Region}/queue/competitive/season/{seasonData.CurrentSeason}?startIndex=0&size=0",
                    true).ConfigureAwait(false);
                if (leaderboardResponse.Content != null && leaderboardResponse.IsSuccessful)
                {
                    var leaderboardcontent = JsonSerializer.Deserialize<LeaderboardsResponse>(leaderboardResponse.Content);
                    try
                    {
                        rankData.MaxRr = leaderboardcontent.TierDetails[rank.ToString()].RankedRatingThreshold;
                    }
                    catch (Exception e)
                    {
                        Constants.Log.Error("GetPlayerHistoryAsync Failed; leaderboardcontent error: {e}", e);
                    }
                }
                else
                {
                    Constants.Log.Error("GetPlayerHistoryAsync Failed; leaderboardResponse error: {e}", leaderboardResponse.ErrorException);
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