using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Authentication;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using RestSharp;
using WAIUA.Objects;

namespace WAIUA.Helpers;

public static class Login
{ 
    public static async Task<bool> CheckLoginAsync()
    {
        // var client = new RestClient(new RestClientOptions("https://auth.riotgames.com/userinfo")
        // { RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true});
        //
        // var request = new RestRequest() {}
        //     .AddHeader("Authorization", $"Bearer {Constants.AccessToken}")
        // //.AddHeader("X-Riot-ClientPlatform", "ew0KCSJwbGF0Zm9ybVR5cGUiOiAiUEMiLA0KCSJwbGF0Zm9ybU9TIjogIldpbmRvd3MiLA0KCSJwbGF0Zm9ybU9TVmVyc2lvbiI6ICIxMC4wLjE5MDQyLjEuMjU2LjY0Yml0IiwNCgkicGxhdGZvcm1DaGlwc2V0IjogIlVua25vd24iDQp9")
        //     .AddHeader("X-Riot-ClientVersion", Constants.Version);
        // var response = await client.ExecutePostAsync<UserInfoResponse>(request).ConfigureAwait(false);
        // if (!response.IsSuccessful)
        // {
        //     if (Constants.AccessToken is null)
        //     {
        //         var isToken = false;
        //         Constants.Log.Error("CheckLoginAsync() failed. Response(Accesstoken {isToken}): {Response}", isToken, response.ErrorException);
        //     }
        //
        //     Constants.Log.Error("CheckLoginAsync() failed. Response: {Response}", response.ErrorException);
        //     return false;
        // }

        // TODO:  Actually check if the user is logged in.

        if (Constants.Ppuuid == Guid.Empty)
        {
            return false;
        }
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
        await using (var file = new FileStream(lockfileLocation, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            using var reader = new StreamReader(file, Encoding.UTF8);
            lockFileString = (string) reader.ReadToEnd().Clone();
            file.Close();
            reader.Close();
        }

        var parts = lockFileString.Split(":");
        Constants.Port = parts[2];
        Constants.LPassword = parts[3];
        return true;
    }

    public static async Task<bool> LocalLoginAsync()
    {
        await GetLatestVersionAsync().ConfigureAwait(false);
        var options = new RestClientOptions($"https://127.0.0.1:{Constants.Port}/entitlements/v1/token")
        {
            RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true
        };
        var client = new RestClient(options);
        var request = new RestRequest()
            .AddHeader("Authorization", $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes($"riot:{Constants.LPassword}"))}");
        // .AddHeader("X-Riot-ClientPlatform", "ew0KCSJwbGF0Zm9ybVR5cGUiOiAiUEMiLA0KCSJwbGF0Zm9ybU9TIjogIldpbmRvd3MiLA0KCSJwbGF0Zm9ybU9TVmVyc2lvbiI6ICIxMC4wLjE5MDQyLjEuMjU2LjY0Yml0IiwNCgkicGxhdGZvcm1DaGlwc2V0IjogIlVua25vd24iDQp9")
        // .AddHeader("X-Riot-ClientVersion", Constants.Version);
        var response = await client.ExecuteGetAsync<EntitlementsResponse>(request).ConfigureAwait(false);
        if (!response.IsSuccessful)
        {
            Constants.Log.Error("LocalLoginAsync Failed");
            return false;
        }

        Constants.AccessToken = response.Data.AccessToken;
        Constants.EntitlementToken = response.Data.Token;
        Constants.Ppuuid = response.Data.Subject;
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
        if (!response.IsSuccessful || response.Content == "{}")
        {
            Constants.Log.Error("LocalRegionAsync Failed: {e}", response.ErrorException);
            return;
        }

        var parts = response.Data.ExtensionData.First().Value.Deserialize<ExternalSessions>().LaunchConfiguration.Arguments[3].Split('=', '&');
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

    public static async Task<bool> CheckMatchAsync()
    {
        RestClient client = new RestClient($"https://glz-{Constants.Shard}-1.{Constants.Region}.a.pvp.net/core-game/v1/players/{Constants.Ppuuid}");
        var request = new RestRequest();
        request.AddHeader("X-Riot-Entitlements-JWT", Constants.EntitlementToken);
        request.AddHeader("Authorization", $"Bearer {Constants.AccessToken}");
        var response = await client.ExecuteGetAsync(request).ConfigureAwait(false);
        if (response.IsSuccessful) return true;

        client = new RestClient($"https://glz-{Constants.Shard}-1.{Constants.Region}.a.pvp.net/pregame/v1/players/{Constants.Ppuuid}");
        response = await client.ExecuteGetAsync(request).ConfigureAwait(false);
        if (response.IsSuccessful) return true;

        Constants.Log.Error("CheckMatchAsync Failed: {e}", response.ErrorException);
        return false;
    }

    public static async Task<string> GetNameServiceGetUsernameAsync(Guid puuid)
    {
        if (puuid == Guid.Empty) return null;
        var options = new RestClientOptions(new Uri($"https://pd.{Constants.Region}.a.pvp.net/name-service/v2/players"))
        {
            RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true
        };
        var client = new RestClient(options);
        RestRequest request = new()
        {
            RequestFormat = DataFormat.Json
        };

        string[] body = {puuid.ToString()};
        request.AddJsonBody(body);
        var response = await client.ExecutePutAsync(request).ConfigureAwait(false);
        if (response.IsSuccessful)
        {
            var incorrectContent = response.Content.Replace("[", string.Empty).Replace("]", string.Empty).Replace("\n", string.Empty);
            var content = JsonSerializer.Deserialize<NameServiceResponse>(incorrectContent);
            return content.GameName + "#" + content.TagLine;
        }

        Constants.Log.Error("GetNameServiceGetUsernameAsync Failed: {e}", response.ErrorException);
        return "";
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
            Constants.Log.Error("Request to {url} Failed: {e}", url, response.ErrorException);
            return response;
        }

        if (attemptCache) Constants.UrlToBody.TryAdd(url, response);
        return response;
    }
}