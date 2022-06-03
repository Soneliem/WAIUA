using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using RestSharp;
using WAIUA.Objects;
using WAIUA.Properties;

namespace WAIUA.Helpers;

public static class ValApi
{
    private static readonly RestClient Client;
    private static readonly RestClient MediaClient;

    private static Urls _mapsInfo;
    private static Urls _agentsInfo;
    private static Urls _ranksInfo;
    private static Urls _versionInfo;
    private static Urls _skinsInfo;
    private static Urls _cardsInfo;
    private static Urls _gamemodeInfo;
    private static List<Urls> _allInfo;

    private static readonly Dictionary<string, string> ValApiLanguages = new()
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
        {"zh", "zh-CN"}
    };

    static ValApi()
    {
        Client = new RestClient("https://valorant-api.com/v1");
        MediaClient = new RestClient();
    }

    private static async Task<string> GetValApiVersionAsync()
    {
        var request = new RestRequest("/version");
        var response = await Client.ExecuteGetAsync<VapiVersionResponse>(request).ConfigureAwait(false);
        return !response.IsSuccessful ? null : response.Data.Data.BuildDate;
    }

    private static async Task<string> GetLocalValApiVersionAsync()
    {
        if (File.Exists(Constants.LocalAppDataPath + "\\ValAPI\\version.txt"))
        {
            var lines = await File.ReadAllLinesAsync(Constants.LocalAppDataPath + "\\ValAPI\\version.txt")
                .ConfigureAwait(false);
            return lines[1];
        }

        return null;
    }

    private static Task GetUrlsAsync()
    {
        var language = ValApiLanguages.GetValueOrDefault(Settings.Default.Language, "en-US");
        _mapsInfo = new Urls
        {
            Name = "Maps",
            Filepath = Constants.LocalAppDataPath + "\\ValAPI\\maps.txt",
            Url = $"/maps?language={language}"
        };
        _agentsInfo = new Urls
        {
            Name = "Agents",
            Filepath = Constants.LocalAppDataPath + "\\ValAPI\\agents.txt",
            Url = $"/agents?language={language}"
        };
        _skinsInfo = new Urls
        {
            Name = "Skins",
            Filepath = Constants.LocalAppDataPath + "\\ValAPI\\skinchromas.txt",
            Url = $"/weapons/skinchromas?language={language}"
        };
        _cardsInfo = new Urls
        {
            Name = "Cards",
            Filepath = Constants.LocalAppDataPath + "\\ValAPI\\cards.txt",
            Url = "/playercards"
        };
        _ranksInfo = new Urls
        {
            Name = "Ranks",
            Filepath = Constants.LocalAppDataPath + "\\ValAPI\\competitivetiers.txt",
            Url = $"/competitivetiers?language={language}"
        };
        _versionInfo = new Urls
        {
            Name = "Version",
            Filepath = Constants.LocalAppDataPath + "\\ValAPI\\version.txt",
            Url = "/version"
        };
        _gamemodeInfo = new Urls
        {
            Name = "Gamemode",
            Filepath = Constants.LocalAppDataPath + "\\ValAPI\\gamemode.txt",
            Url = "/gamemodes"
        };
        _allInfo = new List<Urls> {_mapsInfo, _agentsInfo, _ranksInfo, _versionInfo, _skinsInfo, _cardsInfo, _gamemodeInfo};
        return Task.CompletedTask;
    }

    public static async Task UpdateFilesAsync()
    {
        try
        {
            await GetUrlsAsync().ConfigureAwait(false);
            if (!Directory.Exists(Constants.LocalAppDataPath + "\\ValAPI"))
                Directory.CreateDirectory(Constants.LocalAppDataPath + "\\ValAPI");

            async Task UpdateVersion()
            {
                var versionRequest = new RestRequest(_versionInfo.Url);
                var versionResponse =
                    await Client.ExecuteGetAsync<VapiVersionResponse>(versionRequest).ConfigureAwait(false);
                if (versionResponse.IsSuccessful)
                {
                    string[] lines =
                    {
                        versionResponse.Data.Data.RiotClientVersion, versionResponse.Data.Data.BuildDate
                    };
                    await File.WriteAllLinesAsync(_versionInfo.Filepath, lines).ConfigureAwait(false);
                }
                else
                {
                    Constants.Log.Error("updateVersion Failed, Response:{error}", versionResponse.ErrorException);
                }
            }

            async Task UpdateMapsDictionary()
            {
                var mapsRequest = new RestRequest(_mapsInfo.Url);
                var mapsResponse = await Client.ExecuteGetAsync<ValApiMapsResponse>(mapsRequest).ConfigureAwait(false);
                if (mapsResponse.IsSuccessful)
                {
                    Dictionary<string, ValMap> mapsDictionary = new();
                    if (!Directory.Exists(Constants.LocalAppDataPath + "\\ValAPI\\mapsimg"))
                        Directory.CreateDirectory(Constants.LocalAppDataPath + "\\ValAPI\\mapsimg");
                    foreach (var map in mapsResponse.Data.Data)
                    {
                        mapsDictionary.TryAdd(map.MapUrl, new ValMap
                        {
                            Name = map.DisplayName,
                            UUID = map.Uuid
                        });
                        var fileName = Constants.LocalAppDataPath + $"\\ValAPI\\mapsimg\\{map.Uuid}.png";
                        var request = new RestRequest(map.ListViewIcon);
                        var response = await MediaClient.DownloadDataAsync(request).ConfigureAwait(false);
                        if (response != null)
                            await File.WriteAllBytesAsync(fileName, response).ConfigureAwait(false);
                    }

                    await File.WriteAllTextAsync(_mapsInfo.Filepath, JsonSerializer.Serialize(mapsDictionary)).ConfigureAwait(false);
                }
                else
                {
                    Constants.Log.Error("updateMapsDictionary Failed, Response:{error}", mapsResponse.ErrorException);
                }
            }

            async Task UpdateAgentsDictionary()
            {
                var agentsRequest = new RestRequest(_agentsInfo.Url);
                var agentsResponse = await Client.ExecuteGetAsync<ValApiAgentsResponse>(agentsRequest).ConfigureAwait(false);
                if (agentsResponse.IsSuccessful)
                {
                    Dictionary<Guid, string> agentsDictionary = new();
                    if (!Directory.Exists(Constants.LocalAppDataPath + "\\ValAPI\\agentsimg"))
                        Directory.CreateDirectory(Constants.LocalAppDataPath + "\\ValAPI\\agentsimg");
                    foreach (var agent in agentsResponse.Data.Data)
                    {
                        agentsDictionary.TryAdd(agent.Uuid, agent.DisplayName);

                        var fileName = Constants.LocalAppDataPath + $"\\ValAPI\\agentsimg\\{agent.Uuid}.png";
                        var request = new RestRequest(agent.DisplayIcon);
                        var response = await MediaClient.DownloadDataAsync(request).ConfigureAwait(false);
                        if (response != null)
                            await File.WriteAllBytesAsync(fileName, response)
                                .ConfigureAwait(false);
                    }

                    await File.WriteAllTextAsync(_agentsInfo.Filepath, JsonSerializer.Serialize(agentsDictionary)).ConfigureAwait(false);
                }
                else
                {
                    Constants.Log.Error("updateAgentsDictionary Failed, Response:{error}", agentsResponse.ErrorException);
                }
            }

            async Task UpdateSkinsDictionary()
            {
                var skinsRequest = new RestRequest(_skinsInfo.Url);
                var skinsResponse = await Client.ExecuteGetAsync<ValApiSkinsResponse>(skinsRequest).ConfigureAwait(false);
                if (skinsResponse.IsSuccessful)
                {
                    Dictionary<Guid, ValSkin> skinsDictionary = new();
                    foreach (var skin in skinsResponse.Data.Data) skinsDictionary.TryAdd(skin.Uuid, new ValSkin {Name = skin.DisplayName, Image = skin.FullRender});
                    await File.WriteAllTextAsync(_skinsInfo.Filepath, JsonSerializer.Serialize(skinsDictionary)).ConfigureAwait(false);
                }
                else
                {
                    Constants.Log.Error("updateSkinsDictionary Failed, Response:{error}", skinsResponse.ErrorException);
                }
            }

            async Task UpdateCardsDictionary()
            {
                var cardsRequest = new RestRequest(_cardsInfo.Url);
                var cardsResponse = await Client.ExecuteGetAsync<ValApiCardsResponse>(cardsRequest).ConfigureAwait(false);
                if (cardsResponse.IsSuccessful)
                {
                    Dictionary<Guid, Uri> cardsDictionary = new();
                    foreach (var card in cardsResponse.Data.Data) cardsDictionary.TryAdd(card.Uuid, card.DisplayIcon);
                    await File.WriteAllTextAsync(_cardsInfo.Filepath, JsonSerializer.Serialize(cardsDictionary)).ConfigureAwait(false);
                }
                else
                {
                    Constants.Log.Error("updateCardsDictionary Failed, Response:{error}", cardsResponse.ErrorException);
                }
            }

            async Task UpdateRanksDictionary()
            {
                var ranksRequest = new RestRequest(_ranksInfo.Url);
                var ranksResponse =
                    await Client.ExecuteGetAsync<ValApiRanksResponse>(ranksRequest).ConfigureAwait(false);
                if (ranksResponse.IsSuccessful)
                {
                    Dictionary<int, string> ranksDictionary = new();
                    if (!Directory.Exists(Constants.LocalAppDataPath + "\\ValAPI\\ranksimg"))
                        Directory.CreateDirectory(Constants.LocalAppDataPath + "\\ValAPI\\ranksimg");
                    foreach (var rank in ranksResponse.Data.Data.Last().Tiers)
                    {
                        var tier = rank.TierTier;
                        ranksDictionary.TryAdd(tier, rank.TierName);

                        if (tier == 0)
                        {
                            File.Copy(Directory.GetCurrentDirectory() + "\\Assets\\0.png",
                                Constants.LocalAppDataPath + "\\ValAPI\\ranksimg\\0.png", true);
                            continue;
                        }

                        if (tier is 1 or 2)
                            continue;

                        var fileName = Constants.LocalAppDataPath + $"\\ValAPI\\ranksimg\\{tier}.png";

                        var request = new RestRequest(rank.LargeIcon);
                        var response = await MediaClient.DownloadDataAsync(request).ConfigureAwait(false);

                        // if (response.IsCompletedSuccessfully)
                        if (response != null)
                            await File.WriteAllBytesAsync(fileName, response)
                                .ConfigureAwait(false);
                    }

                    await File.WriteAllTextAsync(_ranksInfo.Filepath, JsonSerializer.Serialize(ranksDictionary)).ConfigureAwait(false);
                }
                else
                {
                    Constants.Log.Error("updateRanksDictionary Failed, Response:{error}", ranksResponse.ErrorException);
                }
            }

            async Task UpdateGamemodeDictionary()
            {
                var gamemodeRequest = new RestRequest(_gamemodeInfo.Url);
                var gamemodeResponse = await Client.ExecuteGetAsync<ValApiGamemodeResponse>(gamemodeRequest).ConfigureAwait(false);
                if (gamemodeResponse.IsSuccessful)
                {
                    Dictionary<Guid, string> gamemodeDictionary = new();
                    if (!Directory.Exists(Constants.LocalAppDataPath + "\\ValAPI\\gamemodeimg"))
                        Directory.CreateDirectory(Constants.LocalAppDataPath + "\\ValAPI\\gamemodeimg");
                    foreach (var gamemode in gamemodeResponse.Data.Data)
                    {
                        if (gamemode.DisplayIcon == null) continue;
                        gamemodeDictionary.TryAdd(gamemode.Uuid, gamemode.DisplayName);

                        var fileName = Constants.LocalAppDataPath + $"\\ValAPI\\gamemodeimg\\{gamemode.Uuid}.png";
                        var request = new RestRequest(gamemode.DisplayIcon);
                        var response = await MediaClient.DownloadDataAsync(request).ConfigureAwait(false);
                        if (response != null)
                            await File.WriteAllBytesAsync(fileName, response)
                                .ConfigureAwait(false);
                    }

                    await File.WriteAllTextAsync(_gamemodeInfo.Filepath, JsonSerializer.Serialize(gamemodeDictionary)).ConfigureAwait(false);
                }
                else
                {
                    Constants.Log.Error("updateGamemodeDictionary Failed, Response:{error}", gamemodeResponse.ErrorException);
                }
            }

            await Task.WhenAll(UpdateVersion(), UpdateRanksDictionary(), UpdateAgentsDictionary(), UpdateMapsDictionary(), UpdateSkinsDictionary(), UpdateCardsDictionary(), UpdateGamemodeDictionary()).ConfigureAwait(false);
        }
        catch (Exception e)
        {
            Constants.Log.Error("UpdateFilesAsync Failed, Response:{error}", e);
        }
    }

    public static async Task CheckAndUpdateJsonAsync()
    {
        try
        {
            await GetUrlsAsync().ConfigureAwait(false);

            if (await GetValApiVersionAsync().ConfigureAwait(false) !=
                await GetLocalValApiVersionAsync().ConfigureAwait(false))
            {
                await UpdateFilesAsync().ConfigureAwait(false);
                return;
            }

            if (_allInfo.Any(url => !File.Exists(url.Filepath))) await UpdateFilesAsync().ConfigureAwait(false);
        }
        catch (Exception)
        {
        }
    }
}