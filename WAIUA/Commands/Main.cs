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
using DataFormat = RestSharp.DataFormat;

namespace WAIUA.Commands
{
    public static class Dictionary
    {
        public static ConcurrentDictionary<string, string> url_to_body = new();
    }

    public class Main
    {
        public static string AccessToken { get; set; }  // To be moved to OOP
        public static string EntitlementToken { get; set; }
        public static string Region { get; set; }
        public static string Shard { get; set; }
        public static string Version { get; set; }
        public static string PPUUID { get; set; }
        public static string Matchid { get; set; }
        public static string Port { get; set; }
        public static string LPassword { get; set; }
        public static string Protocol { get; set; }
        public static string CurrentSeason { get; set; }
        public static string PSeason { get; set; }
        public static string PPSeason { get; set; }
        public static string PPPSeason { get; set; }
        public static sbyte[] PlayerNo { get; set; } = new sbyte[10];
        public static string[] PlayerList { get; set; } = new string[10];
        public static string[] PUUIDList { get; set; } = new string[10];
        public static string[] AgentList { get; set; } = new string[10];
        public static string[] AgentPList { get; set; } = new string[10];
        public static string[] CardList { get; set; } = new string[10];
        public static string[] LevelList { get; set; } = new string[10];
        public static string[] RankList { get; set; } = new string[10];
        public static string[] MaxRRList { get; set; } = new string[10];
        public static string[] PRankList { get; set; } = new string[10];
        public static string[] PPRankList { get; set; } = new string[10];
        public static string[] PPPRankList { get; set; } = new string[10];
        public static string[] RankNameList { get; set; } = new string[10];
        public static string[] PRankNameList { get; set; } = new string[10];
        public static string[] PPRankNameList { get; set; } = new string[10];
        public static string[] PPPRankNameList { get; set; } = new string[10];
        public static string[] RankProgList { get; set; } = new string[10];
        public static string[] PGList { get; set; } = new string[10];
        public static string[] PPGList { get; set; } = new string[10];
        public static string[] PPPGList { get; set; } = new string[10];
        public static bool[] IsIncognito { get; set; } = new bool[10];
        public static string[] TrackerUrlList { get; set; } = new string[10];
        public static string[] TrackerEnabledList { get; set; } = new string[10];
        public static string[] TrackerDisabledList { get; set; } = new string[10];
        public static string[] VandalList { get; set; } = new string[10];
        public static string[] PhantomList { get; set; } = new string[10];
        public static string[] VandalNameList { get; set; } = new string[10];
        public static string[] PhantomNameList { get; set; } = new string[10];

        public static string DoCachedRequest(Method method, string url, bool add_riot_auth, bool bypass_cache = false) // Thank you MitchC for this, I am always touched when random people go out of the way to help others even though they know that we would be clueless and need to ask alot of followup questions
        {
            var tsk = DoCachedRequestAsync(method, url, add_riot_auth, bypass_cache);
            return tsk.Result;
        }

        // TODO: Add dictonary for locales

        public static async Task<string> DoCachedRequestAsync(Method method, string url, bool add_riot_auth,
            bool bypass_cache = false)
        {
            var attempt_cache = method == Method.GET && !bypass_cache;
            if (attempt_cache)
            {
                if (Dictionary.url_to_body.TryGetValue(url, out var res))
                {
                    return res;
                }
            }
            RestClient client = new(url);
            RestRequest request = new(method);
            if (add_riot_auth)
            {
                request.AddHeader("X-Riot-Entitlements-JWT", $"{EntitlementToken}");
                request.AddHeader("Authorization", $"Bearer {AccessToken}");
                request.AddHeader("X-Riot-ClientPlatform", "ew0KCSJwbGF0Zm9ybVR5cGUiOiAiUEMiLA0KCSJwbGF0Zm9ybU9TIjogIldpbmRvd3MiLA0KCSJwbGF0Zm9ybU9TVmVyc2lvbiI6ICIxMC4wLjE5MDQyLjEuMjU2LjY0Yml0IiwNCgkicGxhdGZvcm1DaGlwc2V0IjogIlVua25vd24iDQp9");
                request.AddHeader("X-Riot-ClientVersion", $"{Version}");
            }
            var cont = (await client.ExecuteAsync(request)).Content;
            if (attempt_cache) Dictionary.url_to_body.TryAdd(url, cont);
            return cont;
        }

        public static bool GetSetPPUUID()
        {
            bool output = false;
            try
            {
                RestClient client = new(new Uri("https://auth.riotgames.com/userinfo"));
                RestRequest request = new(Method.POST);

                request.AddHeader("Authorization", $"Bearer {AccessToken}");
                request.AddJsonBody("{}");

                var response = client.Execute(request);
                HttpStatusCode statusCode = response.StatusCode;
                short numericStatusCode = (short)statusCode;
                if (numericStatusCode == 200)
                {
                    output = true;
                }
                string content = response.Content;
                var PlayerInfo = JsonConvert.DeserializeObject(content);
                JToken PUUIDObj = JObject.FromObject(PlayerInfo);
                PPUUID = PUUIDObj["sub"].Value<string>();
            }
            catch (Exception)
            {
            }
            return output;
        }

        public static bool CheckLocal()
        {
            var lockfileLocation = $@"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\Riot Games\Riot Client\Config\lockfile";

            if (File.Exists(lockfileLocation))
            {
                using (FileStream fileStream = new(lockfileLocation, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                using (StreamReader sr = new(fileStream))
                {
                    string[] parts = sr.ReadToEnd().Split(":");
                    Port = parts[2];
                    LPassword = parts[3];
                    Protocol = parts[4];
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void LocalLogin()
        {
            GetLatestVersion();
            RestClient client = new(new Uri($"https://127.0.0.1:{Port}/entitlements/v1/token"));
            RestRequest request = new(Method.GET);
            client.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
            request.AddHeader("Authorization", $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes($"riot:{LPassword}"))}");
            request.AddHeader("X-Riot-ClientPlatform", "ew0KCSJwbGF0Zm9ybVR5cGUiOiAiUEMiLA0KCSJwbGF0Zm9ybU9TIjogIldpbmRvd3MiLA0KCSJwbGF0Zm9ybU9TVmVyc2lvbiI6ICIxMC4wLjE5MDQyLjEuMjU2LjY0Yml0IiwNCgkicGxhdGZvcm1DaGlwc2V0IjogIlVua25vd24iDQp9");
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
            request.AddHeader("Authorization", $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes($"riot:{LPassword}"))}");
            request.AddHeader("X-Riot-ClientPlatform", "ew0KCSJwbGF0Zm9ybVR5cGUiOiAiUEMiLA0KCSJwbGF0Zm9ybU9TIjogIldpbmRvd3MiLA0KCSJwbGF0Zm9ybU9TVmVyc2lvbiI6ICIxMC4wLjE5MDQyLjEuMjU2LjY0Yml0IiwNCgkicGxhdGZvcm1DaGlwc2V0IjogIlVua25vd24iDQp9");
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
            RestClient client = new(new Uri("https://valorant-api.com/v1/version"));
            RestRequest request = new(Method.GET);
            var response = client.Get(request);
            string content = response.Content;
            //string content = DoCachedRequest(Method.GET, $"https://valorant-api.com/v1/version", false);
            var responsevar = JsonConvert.DeserializeObject(content);
            JToken responseObj = JObject.FromObject(responsevar);
            Version = responseObj["data"]["riotClientVersion"].Value<string>();
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
                //string response = DoCachedRequest(Method.GET, $"https://glz-{Shard}-1.{Region}.a.pvp.net/core-game/v1/players/{PUUID}", true, jar, true);
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
                    MessageBox.Show(Properties.Resources.NoMatch, "Error", MessageBoxButton.OK, MessageBoxImage.Question, MessageBoxResult.OK);
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
                        MessageBox.Show(Properties.Resources.NoMatch, "Error", MessageBoxButton.OK, MessageBoxImage.Question, MessageBoxResult.OK);
                        output = false;
                    }
                }
                else
                {
                    MessageBox.Show(Properties.Resources.NoValGame, "Error", MessageBoxButton.OK,
                        MessageBoxImage.Question, MessageBoxResult.OK);
                    output = false;
                }
            }
            return output;
        }

        private void LiveMatchSetup()
        {
            Parallel.Invoke(GetSeasons, GetLatestVersion);
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
                    if (index == 10) { break; }
                }
            }
            PlayerNo = playerno;
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

            string[] output = {
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
                    string content = DoCachedRequest(Method.GET, $"https://valorant-api.com/v1/agents/{agent}", false);
                    var agentinfo = JsonConvert.DeserializeObject(content);
                    JToken agentinfoObj = JObject.FromObject(agentinfo);
                    AgentPList[playerno] = agentinfoObj["data"]["killfeedPortrait"].Value<string>();
                    AgentList[playerno] = agentinfoObj["data"]["displayName"].Value<string>();
                }
                else
                {
                    AgentPList[playerno] = null;
                    AgentList[playerno] = null;
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
            }
        }

        private static void GetCardInfo(string card, sbyte playerno)
        {
            try
            {
                string content = DoCachedRequest(Method.GET, $"https://valorant-api.com/v1/playercards/{card}", false);
                var agentinfo = JsonConvert.DeserializeObject(content);
                JToken agentinfoObj = JObject.FromObject(agentinfo);
                CardList[playerno] = agentinfoObj["data"]["smallArt"].Value<string>();
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
            }
        }

        private static void GetSkinInfo(sbyte playerno)
        {
            try
            {
                if (!String.IsNullOrEmpty(PUUIDList[playerno]))
                {
                    string response = DoCachedRequest(Method.GET, $"https://glz-{Shard}-1.{Region}.a.pvp.net/core-game/v1/matches/{Matchid}/loadouts", true);
                    dynamic content = JsonConvert.DeserializeObject(response);
                    string vandalchroma = content.Loadouts[playerno].Loadout.Items["9c82e19d-4575-0200-1a81-3eacf00cf872"].Sockets["3ad1b2b2-acdb-4524-852f-954a76ddae0a"].Item.ID;
                    string phantomchroma = content.Loadouts[playerno].Loadout.Items["ee8e8d15-496b-07ac-e5f6-8fae5d4c7b1a"].Sockets["3ad1b2b2-acdb-4524-852f-954a76ddae0a"].Item.ID;

                    if (vandalchroma == "19629ae1-4996-ae98-7742-24a240d41f99")
                    {
                        VandalList[playerno] = "/Assets/vandal.png";
                        VandalNameList[playerno] = "Default Vandal";
                    }
                    else
                    {
                        string vandalcontent = DoCachedRequest(Method.GET, $"https://valorant-api.com/v1/weapons/skinchromas/{vandalchroma}", false);
                        var vandalinfo = JsonConvert.DeserializeObject(vandalcontent);
                        JToken vandalinfoObj = JObject.FromObject(vandalinfo);
                        VandalList[playerno] = vandalinfoObj["data"]["displayIcon"].Value<string>();
                        VandalNameList[playerno] = vandalinfoObj["data"]["displayName"].Value<string>();
                    }
                    if (phantomchroma == "52221ba2-4e4c-ec76-8c81-3483506d5242")
                    {
                        PhantomList[playerno] = "/Assets/phantom.png";
                        PhantomNameList[playerno] = "Default Phantom";
                    }
                    else
                    {
                        string phantomcontent = DoCachedRequest(Method.GET, $"https://valorant-api.com/v1/weapons/skinchromas/{phantomchroma}", false);
                        var phantominfo = JsonConvert.DeserializeObject(phantomcontent);
                        JToken phantominfoObj = JObject.FromObject(phantominfo);
                        PhantomList[playerno] = phantominfoObj["data"]["displayIcon"].Value<string>();
                        PhantomNameList[playerno] = phantominfoObj["data"]["displayName"].Value<string>();
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
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
            }
        }

        private static void GetCompHistory(string puuid, sbyte playerno)
        {
            try
            {
                if (!String.IsNullOrEmpty(puuid))
                {
                    string content = DoCachedRequest(Method.GET, $"https://pd.{Region}.a.pvp.net/mmr/v1/players/{puuid}/competitiveupdates?queue=competitive", true, true);
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
                    RankProgList[playerno] = null;
                    PGList[playerno] = null;
                    PPGList[playerno] = null;
                    PPPGList[playerno] = null;
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
            }
        }

        private static void GetPlayerHistory(string puuid, sbyte playerno)
        {
            try
            {
                if (!String.IsNullOrEmpty(puuid))
                {
                    string rank, prank, pprank, ppprank;
                    string content = DoCachedRequest(Method.GET, $"https://pd.{Region}.a.pvp.net/mmr/v1/players/{puuid}", true, true);
                    dynamic contentobj = JObject.Parse(content);

                    try
                    {
                        rank = contentobj.QueueSkills.competitive.SeasonalInfoBySeasonID[$"{CurrentSeason}"].CompetitiveTier;
                    }
                    catch (Exception)
                    {
                        rank = "0";
                    }
                    try
                    {
                        prank = contentobj.QueueSkills.competitive.SeasonalInfoBySeasonID[$"{PSeason}"].CompetitiveTier;
                    }
                    catch (Exception)
                    {
                        prank = "0";
                    }
                    try
                    {
                        pprank = contentobj.QueueSkills.competitive.SeasonalInfoBySeasonID[$"{PPSeason}"].CompetitiveTier;
                    }
                    catch (Exception)
                    {
                        pprank = "0";
                    }
                    try
                    {
                        ppprank = contentobj.QueueSkills.competitive.SeasonalInfoBySeasonID[$"{PPPSeason}"].CompetitiveTier;
                    }
                    catch (Exception)
                    {
                        ppprank = "0";
                    }

                    if (rank is "21" or "22" or "23")
                    {
                        string content2 = DoCachedRequest(Method.GET, $"https://pd.{Shard}.a.pvp.net/mmr/v1/leaderboards/affinity/{Region}/queue/competitive/season/{CurrentSeason}?startIndex=0&size=0", true);
                        dynamic contentobj2 = JObject.Parse(content2);
                        switch (rank)
                        {
                            case "21":
                                MaxRRList[playerno] = contentobj2.tierDetails[22].rankedRatingThreshold;
                                break;

                            case "22":
                                MaxRRList[playerno] = contentobj2.tierDetails[23].rankedRatingThreshold;
                                break;

                            case "23":
                                MaxRRList[playerno] = contentobj2.tierDetails[24].rankedRatingThreshold;
                                break;
                        }
                    }
                    else
                    {
                        MaxRRList[playerno] = "100";
                    }

                    Parallel.Invoke(
                        () => RankList[playerno] = GetLRankIcon(rank),
                        () => PRankList[playerno] = GetRankIcon(prank),
                        () => PPRankList[playerno] = GetRankIcon(pprank),
                        () => PPPRankList[playerno] = GetRankIcon(ppprank),
                        () => RankNameList[playerno] = GetRankName(rank),
                        () => PRankNameList[playerno] = GetRankName(prank),
                        () => PPRankNameList[playerno] = GetRankName(pprank),
                        () => PPPRankNameList[playerno] = GetRankName(ppprank));
                }
                else
                {
                    Parallel.Invoke(
                        () => RankList[playerno] = null,
                        () => PRankList[playerno] = null,
                        () => PPRankList[playerno] = null,
                        () => PPPRankList[playerno] = null,
                        () => RankNameList[playerno] = null,
                        () => PRankNameList[playerno] = null,
                        () => PPRankNameList[playerno] = null,
                        () => PPPRankNameList[playerno] = null);
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
            }
        }

        private static void GetSeasons()
        {
            try
            {
                string url = $"https://shared.{Region}.a.pvp.net/content-service/v2/content";
                RestClient client = new(url);
                RestRequest request = new(Method.GET);
                request.AddHeader("X-Riot-Entitlements-JWT", $"{EntitlementToken}");
                request.AddHeader("Authorization", $"Bearer {AccessToken}");
                request.AddHeader("X-Riot-ClientPlatform", "ew0KCSJwbGF0Zm9ybVR5cGUiOiAiUEMiLA0KCSJwbGF0Zm9ybU9TIjogIldpbmRvd3MiLA0KCSJwbGF0Zm9ybU9TVmVyc2lvbiI6ICIxMC4wLjE5MDQyLjEuMjU2LjY0Yml0IiwNCgkicGxhdGZvcm1DaGlwc2V0IjogIlVua25vd24iDQp9");
                request.AddHeader("X-Riot-ClientVersion", $"{Version}");
                string response = client.Execute(request).Content;
                //string response = DoCachedRequest(Method.GET, $"https://shared.{Region}.a.pvp.net/content-service/v2/content", true);
                //System.Diagnostics.Debug.WriteLine(response);
                dynamic content = JsonConvert.DeserializeObject(response);
                sbyte index = 0;
                sbyte currentindex = 0;

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

        public static string GetLRankIcon(string rank)
        {
            string content = DoCachedRequest(Method.GET, "https://valorant-api.com/v1/competitivetiers", false);
            dynamic agentinfo = JsonConvert.DeserializeObject(content);
            string output = "";
            foreach (var tiers in agentinfo.data[2].tiers)
            {
                if (tiers.tier == rank)
                {
                    output = tiers.largeIcon;
                    break;
                }
            }
            return output;
        }

        public static string GetRankIcon(string rank)
        {
            string content = DoCachedRequest(Method.GET, "https://valorant-api.com/v1/competitivetiers", false);
            dynamic agentinfo = JsonConvert.DeserializeObject(content);
            string output = "";
            foreach (var tiers in agentinfo.data[2].tiers)
            {
                if (tiers.tier == rank)
                {
                    output = tiers.smallIcon;
                    break;
                }
            }
            return output;
        }

        public static string GetRankName(string rank)
        {
            string content = DoCachedRequest(Method.GET, "https://valorant-api.com/v1/competitivetiers", false);
            dynamic agentinfo = JsonConvert.DeserializeObject(content);
            string name = null;
            foreach (var tiers in agentinfo.data[2].tiers)
            {
                if (tiers.tier == rank)
                {
                    name = tiers.tierName;
                    break;
                }
            }
            return name;
        }

        public static bool Tracker(string username, sbyte playerno)
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
            }
            catch (Exception)
            {
            }
            return output;
        }
    }
}