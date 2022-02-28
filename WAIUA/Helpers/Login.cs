using System;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using WAIUA.Models;

namespace WAIUA.Helpers;

public static class Login
{
    public static async Task<bool> GetSetPpuuidAsync()
    {
        var client = new RestClient("https://auth.riotgames.com/userinfo");
        var request = new RestRequest()
            .AddHeader("Authorization", $"Bearer {Constants.AccessToken}")
            .AddBody("{}");
        var response = await client.ExecutePostAsync<UserInfoResponse>(request).ConfigureAwait(false);
        if (!response.IsSuccessful)
        {
            Constants.Log.Error("GetSetPpuuidAsync() failed. Response: {Response}", response.ErrorException);
            return false;
        }

        Constants.PPUUID = response.Data.Sub;
        return true;
    }

    public static async Task<bool> CheckLocalAsync()
    {
        var lockfileLocation =
            $@"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\Riot Games\Riot Client\Config\lockfile";

        if (!File.Exists(lockfileLocation))
        {
            Constants.Log.Error("Valorant Not detected");
            return false;
        }

        string lockFileString;
        using (var File = new FileStream(lockfileLocation, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            using (var Reader = new StreamReader(File, Encoding.UTF8))
            {
                lockFileString = (string) Reader.ReadToEnd().Clone();
                File.Close();
                Reader.Close();
            }
        }

        var parts = lockFileString.Split(":");
        Constants.Port = Convert.ToInt32(parts[1]);
        Constants.LPassword = Convert.ToInt32((parts[2]));
        return true;
    }

    public static async Task<bool> LocalLoginAsync()
    {
        await GetLatestVersionAsync().ConfigureAwait(false);
        // var options = new RestClientOptions(RemoteCertificateValidationCallback(sender, certificate, chain, sslPolicyErrors));
        var client = new RestClient($"https://127.0.0.1:{Constants.Port}/entitlements/v1/token");
        var request = new RestRequest()
            .AddHeader("Authorization", $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes($"riot:{Constants.LPassword}"))}");
        // .AddHeader("X-Riot-ClientPlatform", "ew0KCSJwbGF0Zm9ybVR5cGUiOiAiUEMiLA0KCSJwbGF0Zm9ybU9TIjogIldpbmRvd3MiLA0KCSJwbGF0Zm9ybU9TVmVyc2lvbiI6ICIxMC4wLjE5MDQyLjEuMjU2LjY0Yml0IiwNCgkicGxhdGZvcm1DaGlwc2V0IjogIlVua25vd24iDQp9")
        // .AddHeader("X-Riot-ClientVersion", Constants.Version);
        // client.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
        var response = await client.ExecuteGetAsync<EntitlementsResponse>(request).ConfigureAwait(false);
        if (!response.IsSuccessful)
        {
            Constants.Log.Error("LocalLoginAsync Failed");
            return false;
        }
        Constants.AccessToken = response.Data.AccessToken;
        Constants.EntitlementToken = response.Data.Token;
        return true;
    }

    public static async Task LocalRegionAsync()
    {
        var options = new RestClientOptions(new Uri($"https://127.0.0.1:{Constants.Port}/product-session/v1/external-sessions"))
        {
            RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true
        };
            
        var client = new RestClient(options);
        var request = new RestRequest().AddHeader("Authorization", $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes($"riot:{Constants.LPassword}"))}")
            .AddHeader("X-Riot-ClientPlatform", "ew0KCSJwbGF0Zm9ybVR5cGUiOiAiUEMiLA0KCSJwbGF0Zm9ybU9TIjogIldpbmRvd3MiLA0KCSJwbGF0Zm9ybU9TVmVyc2lvbiI6ICIxMC4wLjE5MDQyLjEuMjU2LjY0Yml0IiwNCgkicGxhdGZvcm1DaGlwc2V0IjogIlVua25vd24iDQp9")
            .AddHeader("X-Riot-ClientVersion", Constants.Version);
        // client.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
        var response = await client.ExecuteGetAsync<ExternalSessionsResponse>(request).ConfigureAwait(false);
        if (!response.IsSuccessful)
        {
            Constants.Log.Error("LocalRegionAsync Failed: {e}", response.ErrorException);
            return;
        }
        var parts = response.Data.randString.First().Value.LaunchConfiguration.Arguments[3].Split('=', '&');
        switch (parts[1])
        {
            case "latam":
                Constants.Region = "na";
                Constants.Shard = "latam";
                break;
            case "br":
                Constants.Region = "na";
                Constants.Shard = "br";
                break;
            default:
                Constants.Region = parts[1];
                Constants.Shard = parts[1];
                break;
        }
    }

    public static async Task<bool> CheckMatchIdAsync()
    {
        var url =
            $"https://glz-{Constants.Shard}-1.{Constants.Region}.a.pvp.net/core-game/v1/players/{Constants.PPUUID}";
        RestClient client = new(url);
        var request = new RestRequest();
        request.AddHeader("X-Riot-Entitlements-JWT", Constants.EntitlementToken);
        request.AddHeader("Authorization", $"Bearer {Constants.AccessToken}");
        var response = await client.ExecuteGetAsync(request).ConfigureAwait(false);
        if (response.IsSuccessful) return true;
        Constants.Log.Error("CheckMatchIdAsync Failed: {e}", response.ErrorException);
        return false;
    }

    public static async Task<string> GetIgUsernameAsync(Guid puuid)
    {
        var client = new RestClient($"https://pd.{Constants.Region}.a.pvp.net/name-service/v2/players");
        RestRequest request = new()
        {
            RequestFormat = DataFormat.Json
        };

        string[] body = {puuid.ToString()};
        request.AddJsonBody(body);

        var response = await client.ExecutePutAsync<NameServiceResponse>(request).ConfigureAwait(false);
        if (response.IsSuccessful)
        {
            return response.Data.DisplayName + "#" + response.Data.TagLine;
        }
        else
        {
            Constants.Log.Error("GetIgUsernameAsync Failed: {e}", response.ErrorException);
            return "";
            
        }

        // content = content.Replace("[", "");
        // content = content.Replace("]", "");
    }


    private static async Task GetLatestVersionAsync()
    {
        var lines = await File.ReadAllLinesAsync(Constants.LocalAppDataPath + "\\ValAPI\\version.txt").ConfigureAwait(false);
        Constants.Version = lines[0];
    }

    public static async Task<RestResponse> DoCachedRequestAsync(Method method, string url, bool addRiotAuth,
        bool bypassCache = false)
    {
        var attemptCache = method == Method.Get && !bypassCache;
        if (attemptCache)
            if (Constants.UrlToBody.TryGetValue(url, out var res))
                return res;
        var client = new RestClient(url);
        var request = new RestRequest();
        if (addRiotAuth)
        {
            request.AddHeader("X-Riot-Entitlements-JWT", Constants.EntitlementToken);
            request.AddHeader("Authorization", $"Bearer {Constants.AccessToken}");
            request.AddHeader("X-Riot-ClientPlatform",
                "ew0KCSJwbGF0Zm9ybVR5cGUiOiAiUEMiLA0KCSJwbGF0Zm9ybU9TIjogIldpbmRvd3MiLA0KCSJwbGF0Zm9ybU9TVmVyc2lvbiI6ICIxMC4wLjE5MDQyLjEuMjU2LjY0Yml0IiwNCgkicGxhdGZvcm1DaGlwc2V0IjogIlVua25vd24iDQp9");
            request.AddHeader("X-Riot-ClientVersion", Constants.Version);
        }
        var response = await client.ExecuteAsync(request, method).ConfigureAwait(false);
        if (!response.IsSuccessful)
        {
            Constants.Log.Error("Request to {url} Failed: {e}",url, response.ErrorException);
            return response;
        }

        if (attemptCache) Constants.UrlToBody.TryAdd(url, response);
        return response;
    }
}