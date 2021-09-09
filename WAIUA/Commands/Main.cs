using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace WAIUA.Commands
{
    public static class Main
    {
        public static string AccessToken { get; set; }
        public static string EntitlementToken { get; set; }
        public static string Region { get; set; }
        public static string Version { get; set; }
        public static string PPUUID { get; set; }
        public static string PUUID { get; set; }
        public static string Matchid { get; set; }
        public static string IGN { get; set; }
        public static string Port { get; set; }
        public static string LPassword { get; set; }
        public static string Protocol { get; set; }
        public static string CurrentSeason { get; set; }
        public static string PSeason { get; set; }
        public static string PPSeason { get; set; }
        public static string PPPSeason { get; set; }
        public static int[] PlayerNo { get; set; } = new int[10];
        public static string[] PlayerList { get; set; } = new string[10];
        public static string[] PUUIDList { get; set; } = new string[10];
        public static string[] AgentList { get; set; } = new string[10];
        public static string[] AgentPList { get; set; } = new string[10];
        public static string[] CardList { get; set; } = new string[10];
        public static string[] LevelList { get; set; } = new string[10];
        public static string[] RankList { get; set; } = new string[10];
        public static string[] PRankList { get; set; } = new string[10];
        public static string[] PPRankList { get; set; } = new string[10];
        public static string[] PPPRankList { get; set; } = new string[10];
        public static string[] RankNameList { get; set; } = new string[10];
        public static string[] PRankNameList { get; set; } = new string[10];
        public static string[] PpRankNameList { get; set; } = new string[10];
        public static string[] PppRankNameList { get; set; } = new string[10];
        public static string[] RankProgList { get; set; } = new string[10];
        public static string[] PGList { get; set; } = new string[10];
        public static string[] PPGList { get; set; } = new string[10];
        public static string[] PPPGList { get; set; } = new string[10];
        public static string[] TitleList { get; set; } = new string[10];
        public static bool[] IsIncognito { get; set; } = new bool[10];
        private static ConcurrentDictionary<string, string> url_to_body = new();

        public static string DoCachedRequest(Method method, String url, bool add_riot_auth, CookieContainer cookie_container = null, bool bypass_cache = false) // Thank you MitchC for this, I am always touched when random people go out of the way to help others even though they know that we would be clueless and need to ask alot of followup questions
        {
            var tsk = DoCachedRequestAsync(method, url, add_riot_auth, cookie_container, bypass_cache);
            return tsk.Result;
        }

        public static async Task<string> DoCachedRequestAsync(Method method, String url, bool add_riot_auth, CookieContainer cookie_container = null, bool bypass_cache = false)
        {
            var attempt_cache = method == Method.GET && !bypass_cache;
            if (attempt_cache)
            {
                if (url_to_body.TryGetValue(url, out var res))
                {
                    return res;
                }
            }
            RestClient client = new RestClient(url);
            if (cookie_container != null)
                client.CookieContainer = cookie_container;
            RestRequest request = new RestRequest(method);
            if (add_riot_auth)
            {
                request.AddHeader("X-Riot-Entitlements-JWT", $"{EntitlementToken}");
                request.AddHeader("Authorization", $"Bearer {AccessToken}");
                request.AddHeader("X-Riot-ClientPlatform", "ew0KCSJwbGF0Zm9ybVR5cGUiOiAiUEMiLA0KCSJwbGF0Zm9ybU9TIjogIldpbmRvd3MiLA0KCSJwbGF0Zm9ybU9TVmVyc2lvbiI6ICIxMC4wLjE5MDQyLjEuMjU2LjY0Yml0IiwNCgkicGxhdGZvcm1DaGlwc2V0IjogIlVua25vd24iDQp9");
                request.AddHeader("X-Riot-ClientVersion", $"{Version}");
            }
            var cont = (await client.ExecuteAsync(request)).Content;
            if (attempt_cache)
                url_to_body.TryAdd(url, cont);
            return cont;
        }

        public static void Login(CookieContainer cookie, string username, string password)
        {
            try
            {
                Main.GetAuthorization(cookie);
                var authJson = JsonConvert.DeserializeObject(Main.Authenticate(cookie, username, password));
                JToken authObj = JObject.FromObject(authJson);

                string authURL = authObj["response"]["parameters"]["uri"].Value<string>();
                var accessTokenVar = Regex.Match(authURL, @"access_token=(.+?)&scope=").Groups[1].Value;
                AccessToken = $"{accessTokenVar}";

                RestClient client = new RestClient(new Uri("https://entitlements.auth.riotgames.com/api/token/v1"));
                RestRequest request = new RestRequest(Method.POST);

                request.AddHeader("Authorization", $"Bearer {AccessToken}");
                request.AddJsonBody("{}");

                string response = client.Execute(request).Content;
                var entitlement_token = JsonConvert.DeserializeObject(response);
                JToken entitlement_tokenObj = JObject.FromObject(entitlement_token);

                EntitlementToken = entitlement_tokenObj["entitlements_token"].Value<string>();

                GetPPUUID();
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
            }
        }

        public static void GetPPUUID()
        {
            RestClient client = new RestClient(new Uri("https://auth.riotgames.com/userinfo"));
            RestRequest request = new RestRequest(Method.POST);

            request.AddHeader("Authorization", $"Bearer {AccessToken}");
            request.AddJsonBody("{}");

            string response = client.Execute(request).Content;
            var PlayerInfo = JsonConvert.DeserializeObject(response);
            JToken PUUIDObj = JObject.FromObject(PlayerInfo);
            PPUUID = PUUIDObj["sub"].Value<string>();
        }

        public static void GetAuthorization(CookieContainer jar)
        {
            string url = "https://auth.riotgames.com/api/v1/authorization";
            RestClient client = new RestClient(url);

            client.CookieContainer = jar;

            RestRequest request = new RestRequest(Method.POST);
            string body = "{\"client_id\":\"play-valorant-web-prod\",\"nonce\":\"1\",\"redirect_uri\":\"https://playvalorant.com/opt_in" + "\",\"response_type\":\"token id_token\",\"scope\":\"account openid\"}";
            request.AddJsonBody(body);
            client.Execute(request);
        }

        public static string Authenticate(CookieContainer cookie, string user, string pass)
        {
            string url = "https://auth.riotgames.com/api/v1/authorization";
            RestClient client = new RestClient(url);

            client.CookieContainer = cookie;

            RestRequest request = new RestRequest(Method.PUT);
            string body = "{\"type\":\"auth\",\"username\":\"" + user + "\",\"password\":\"" + pass + "\",\"remember\": true, \"language\": \"en_US\"}";
            request.AddJsonBody(body);

            return client.Execute(request).Content;
        }

        public static bool CheckLocal()
        {
            var lockfileLocation = $@"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\Riot Games\Riot Client\Config\lockfile";

            if (File.Exists(lockfileLocation))
            {
                using (FileStream fileStream = new FileStream(lockfileLocation, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                using (StreamReader sr = new StreamReader(fileStream))
                {
                    string[] parts = sr.ReadToEnd().Split(":");
                    Port = parts[2];
                    LPassword = parts[3];
                    Protocol = parts[4];
                    return true;
                }
            }
            return false;
        }

        public static void LocalLogin()
        {
            GetLatestVersion();
            RestClient client = new RestClient(new Uri($"https://127.0.0.1:{Port}/entitlements/v1/token"));
            RestRequest request = new RestRequest(Method.GET);
            client.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
            request.AddHeader("Authorization", $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes($"riot:{LPassword}"))}");
            request.AddHeader("X-Riot-ClientPlatform", "ew0KCSJwbGF0Zm9ybVR5cGUiOiAiUEMiLA0KCSJwbGF0Zm9ybU9TIjogIldpbmRvd3MiLA0KCSJwbGF0Zm9ybU9TVmVyc2lvbiI6ICIxMC4wLjE5MDQyLjEuMjU2LjY0Yml0IiwNCgkicGxhdGZvcm1DaGlwc2V0IjogIlVua25vd24iDQp9");
            request.AddHeader("X-Riot-ClientVersion", $"{Version}");
            request.RequestFormat = RestSharp.DataFormat.Json;
            var response = client.Get(request);
            string content = client.Execute(request).Content;
            var responsevar = JsonConvert.DeserializeObject(content);
            JToken responseObj = JObject.FromObject(responsevar);
            AccessToken = responseObj["accessToken"].Value<string>();
            EntitlementToken = responseObj["token"].Value<string>();
            GetPPUUID();
        }

        public static void LocalRegion()
        {
            RestClient client = new RestClient(new Uri($"https://127.0.0.1:{Port}/product-session/v1/external-sessions"));
            RestRequest request = new RestRequest(Method.GET);
            client.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
            request.AddHeader("Authorization", $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes($"riot:{LPassword}"))}");
            request.AddHeader("X-Riot-ClientPlatform", "ew0KCSJwbGF0Zm9ybVR5cGUiOiAiUEMiLA0KCSJwbGF0Zm9ybU9TIjogIldpbmRvd3MiLA0KCSJwbGF0Zm9ybU9TVmVyc2lvbiI6ICIxMC4wLjE5MDQyLjEuMjU2LjY0Yml0IiwNCgkicGxhdGZvcm1DaGlwc2V0IjogIlVua25vd24iDQp9");
            request.AddHeader("X-Riot-ClientVersion", $"{Version}");
            request.RequestFormat = RestSharp.DataFormat.Json;
            var response = client.Get(request);
            string content = client.Execute(request).Content;
            JObject root = JObject.Parse(content);
            JProperty property = (JProperty)root.First;
            var fullstring = (property.Value["launchConfiguration"]["arguments"][3]);
            string[] parts = fullstring.ToString().Split(new char[] { '=', '&' });
            string output = parts[1];
            Region = output;
        }

        public static void GetLatestVersion()
        {
            RestClient client = new RestClient(new Uri("https://valorant-api.com/v1/version"));
            RestRequest request = new RestRequest(Method.GET);
            var response = client.Get(request);
            string content = response.Content;
            //string content = DoCachedRequest(Method.GET, $"https://valorant-api.com/v1/version", false);
            var responsevar = JsonConvert.DeserializeObject(content);
            JToken responseObj = JObject.FromObject(responsevar);
            Version = responseObj["data"]["riotClientVersion"].Value<string>();
        }

        public static string GetIGUsername(CookieContainer cookie, string puuid)
        {
            try
            {
                string gameName = "";
                string gameTag = "";
                string url = $"https://pd.{Region}.a.pvp.net/name-service/v2/players";
                RestClient client = new RestClient(url);
                client.CookieContainer = cookie;
                RestRequest request = new RestRequest(Method.PUT);

                request.RequestFormat = RestSharp.DataFormat.Json;
                request.AddHeader("X-Riot-Entitlements-JWT", $"{EntitlementToken}");
                request.AddHeader("Authorization", $"Bearer {AccessToken}");

                string[] body = new String[1] { puuid };
                request.AddJsonBody(body);

                var response = client.Put(request);
                string content = client.Execute(request).Content;

                content = content.Replace("[", "");
                content = content.Replace("]", "");

                string errorMessage = response.ErrorMessage;

                var uinfo = JsonConvert.DeserializeObject(content);
                JToken uinfoObj = JObject.FromObject(uinfo);
                gameName = uinfoObj["GameName"].Value<string>();
                gameTag = uinfoObj["TagLine"].Value<string>();
                IGN = gameName + "#" + gameTag;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
            }
            return IGN;
        }

        public static Boolean LiveMatchID(CookieContainer jar)
        {
            try
            {
                string url = $"https://glz-{Region}-1.{Region}.a.pvp.net/core-game/v1/players/{PPUUID}";
                RestClient client = new RestClient(url);
                client.CookieContainer = jar;
                RestRequest request = new RestRequest(Method.GET);
                request.AddHeader("X-Riot-Entitlements-JWT", $"{EntitlementToken}");
                request.AddHeader("Authorization", $"Bearer {AccessToken}");
                string response = client.Execute(request).Content;
                //string response = DoCachedRequest(Method.GET, $"https://glz-{Region}-1.{Region}.a.pvp.net/core-game/v1/players/{PPUUID}", true, jar, true);
                var matchinfo = JsonConvert.DeserializeObject(response);
                JToken matchinfoObj = JObject.FromObject(matchinfo);
                Matchid = matchinfoObj["MatchID"].Value<string>();
                return true;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
                return false;
            }
        }

        public static bool LiveMatchSetup()
        {
            bool output = false;
            CookieContainer cookie = new CookieContainer();
            if (String.IsNullOrEmpty(Main.GetIGUsername(cookie, PPUUID)))
            {
                if (CheckLocal())
                {
                    LocalLogin();
                    LocalRegion();
                    output = true;
                }
                else
                {
                    output =  false;
                }
            }
            if (LiveMatchID(cookie))
            {
                Parallel.Invoke(
                    () => GetSeasons(),
                    () => GetLatestVersion());
                string url = $"https://glz-{Region}-1.{Region}.a.pvp.net/core-game/v1/matches/{Matchid}";
                RestClient client = new RestClient(url);
                RestRequest request = new RestRequest(Method.GET);
                request.AddHeader("X-Riot-Entitlements-JWT", $"{EntitlementToken}");
                request.AddHeader("Authorization", $"Bearer {AccessToken}");
                string content = client.Execute(request).Content;
                //string content = DoCachedRequest(Method.GET, $"https://glz-{Region}-1.{Region}.a.pvp.net/core-game/v1/matches/{Matchid}", true, null, true);
                dynamic matchinfo = JsonConvert.DeserializeObject(content);
                int[] playerno = new int[10];
                string[] puuid = new string[10];
                string[] agent = new string[10];
                string[] card = new string[10];
                string[] level = new string[10];
                string[] title = new string[10];
                bool[] incognito = new bool[10];
                int index = 0;
                foreach (var entry in matchinfo.Players)
                {
                    if (entry.IsCoach == false)
                    {
                        playerno[index] = index;
                        puuid[index] = entry.Subject;
                        agent[index] = entry.CharacterID;
                        card[index] = entry.PlayerIdentity.PlayerCardID;
                        level[index] = entry.PlayerIdentity.AccountLevel;
                        title[index] = entry.PlayerIdentity.PlayerTitleID;
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
                TitleList = title;
                IsIncognito = incognito;
                output = true;
            }

            return output;
        }

        public static string[] LiveMatchOutput(int playerno)
        {
            Parallel.Invoke(
                () => GetIGCUsername(playerno),
                () => GetAgentInfo(AgentList[playerno], playerno),
                () => GetCardInfo(CardList[playerno], playerno),
                () => GetTitleInfo(TitleList[playerno], playerno),
                () => GetCompHistory(PUUIDList[playerno], playerno),
                () => GetPlayerHistory(PUUIDList[playerno], playerno));

            string[] output = new string[]{
                PlayerList[playerno],
                AgentList[playerno],
                AgentPList[playerno],
                CardList[playerno],
                LevelList[playerno],
                PGList[playerno],
                PPGList[playerno],
                PPPGList[playerno],
                RankProgList[playerno],
                RankList[playerno],
                PRankList[playerno],
                PPRankList[playerno],
                PPPRankList[playerno],
                TitleList[playerno],
                RankNameList[playerno],
                PRankNameList[playerno],
                PpRankNameList[playerno],
                PppRankNameList[playerno]
            };
            return output;
        }

        public static void GetIGCUsername(int playerno)
        {
            CookieContainer cookie = new CookieContainer();
            if (IsIncognito[playerno])
            {
                PlayerList[playerno] = "----";
            }
            else
            {
                PlayerList[playerno] = GetIGUsername(cookie, PUUIDList[playerno]);
            }
        }

        public static void GetAgentInfo(string agent, int playerno)
        {
            try
            {
                string content = DoCachedRequest(Method.GET, $"https://valorant-api.com/v1/agents/{agent}", false);
                var agentinfo = JsonConvert.DeserializeObject(content);
                JToken agentinfoObj = JObject.FromObject(agentinfo);
                AgentPList[playerno] = agentinfoObj["data"]["killfeedPortrait"].Value<string>();
                AgentList[playerno] = agentinfoObj["data"]["displayName"].Value<string>();
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
            }
        }

        public static void GetCardInfo(string card, int playerno)
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

        public static void GetTitleInfo(string title, int playerno)
        {
            try
            {
                string content = DoCachedRequest(Method.GET, $"https://valorant-api.com/v1/playertitles/{title}", false);
                var agentinfo = JsonConvert.DeserializeObject(content);
                JToken agentinfoObj = JObject.FromObject(agentinfo);
                TitleList[playerno] = agentinfoObj["data"]["titleText"].Value<string>();
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
            }
        }

        public static void GetCompHistory(string puuid, int playerno)
        {
            try
            {
                string content = DoCachedRequest(Method.GET, $"https://pd.{Region}.a.pvp.net/mmr/v1/players/{puuid}/competitiveupdates?queue=competitive", true, null, true);
                var historyinfo = JsonConvert.DeserializeObject(content);
                JToken historyinfoObj = JObject.FromObject(historyinfo);
                RankProgList[playerno] = historyinfoObj["Matches"][0]["RankedRatingAfterUpdate"].Value<string>();
                if (historyinfoObj["Matches"][0]["RankedRatingEarned"].Value<int>() >= 0)
                {
                    PGList[playerno] = "/Assets/rankup.png";
                }
                else
                {
                    PGList[playerno] = "/Assets/rankdown.png";
                }
                if (historyinfoObj["Matches"][1]["RankedRatingEarned"].Value<int>() >= 0)
                {
                    PPGList[playerno] = "/Assets/rankup.png";
                }
                else
                {
                    PPGList[playerno] = "/Assets/rankdown.png";
                }
                if (historyinfoObj["Matches"][2]["RankedRatingEarned"].Value<int>() >= 0)
                {
                    PPPGList[playerno] = "/Assets/rankup.png";
                }
                else
                {
                    PPPGList[playerno] = "/Assets/rankdown.png";
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
            }
        }

        public static void GetPlayerHistory(string puuid, int playerno)
        {
            try
            {
                string rank, prank, pprank, ppprank = "";
                string content = DoCachedRequest(Method.GET, $"https://pd.{Region}.a.pvp.net/mmr/v1/players/{puuid}", true, null, true);
                dynamic contentobj = JObject.Parse(content);

                /*try
                {*/
                rank = contentobj.QueueSkills.competitive.SeasonalInfoBySeasonID[$"{CurrentSeason}"].CompetitiveTier;
                /*}
                catch (Exception)
                {
                    rank = "0";
                }*/
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

                Parallel.Invoke(
                    () => RankList[playerno] = GetLRankIcon(rank),
                    () => PRankList[playerno] = GetRankIcon(prank),
                    () => PPRankList[playerno] = GetRankIcon(pprank),
                    () => PPPRankList[playerno] = GetRankIcon(ppprank),
                    () => RankNameList[playerno] = GetRankName(rank),
                    () => PRankNameList[playerno] = GetRankName(prank),
                    () => PpRankNameList[playerno] = GetRankName(pprank),
                    () => PppRankNameList[playerno] = GetRankName(ppprank));
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
                RestClient client = new RestClient(url);
                RestRequest request = new RestRequest(Method.GET);
                request.AddHeader("X-Riot-Entitlements-JWT", $"{EntitlementToken}");
                request.AddHeader("Authorization", $"Bearer {AccessToken}");
                request.AddHeader("X-Riot-ClientPlatform", "ew0KCSJwbGF0Zm9ybVR5cGUiOiAiUEMiLA0KCSJwbGF0Zm9ybU9TIjogIldpbmRvd3MiLA0KCSJwbGF0Zm9ybU9TVmVyc2lvbiI6ICIxMC4wLjE5MDQyLjEuMjU2LjY0Yml0IiwNCgkicGxhdGZvcm1DaGlwc2V0IjogIlVua25vd24iDQp9");
                request.AddHeader("X-Riot-ClientVersion", $"{Version}");
                string response = client.Execute(request).Content;
                //string response = DoCachedRequest(Method.GET, $"https://shared.{Region}.a.pvp.net/content-service/v2/content", true);
                //System.Diagnostics.Debug.WriteLine(response);
                dynamic content = JsonConvert.DeserializeObject(response);
                int index = 0;
                int currentindex = 0;

                foreach (var season in content.Seasons)
                {
                    if ((season.IsActive == true) & (season.Type == "act"))
                    {
                        CurrentSeason = season.ID;
                        currentindex = index;
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
                }
            }
            return output;
        }

        public static string GetRankName(string rank)
        {
            string content = DoCachedRequest(Method.GET, "https://valorant-api.com/v1/competitivetiers", false);
            dynamic agentinfo = JsonConvert.DeserializeObject(content);
            string name = "";
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

        public static string TrackerUrl(string username)
        {
            string output = null;
            output = HttpUtility.UrlEncode(username);
            return output;
        }
    }
}