using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using RestSharp;
using WAIUA.Objects;

namespace WAIUA.Helpers;

public static class Login
{
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
        var response = await client.ExecuteGetAsync<EntitlementsResponse>(request).ConfigureAwait(false);
        if (!response.IsSuccessful)
        {
            Constants.Log.Error("LocalLoginAsync Failed");
            return false;
        }

        Constants.AccessToken = response.Data.AccessToken;
        Constants.EntitlementToken = response.Data.Token;
        Constants.Ppuuid = response.Data.Subject;
        Constants.Log.Information("Logged in as {Ppuuid}", Constants.Ppuuid);
        return true;
    }

    public static async Task LocalRegionAsync()
    {
        var path = $@"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\VALORANT\Saved\Logs\ShooterGame.log";

        if (!File.Exists(path))
        {
            Constants.Log.Error("LocalRegionAsync() Failed: Log file not found");
            return;
        }

        await using var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using var sr = new StreamReader(fs, Encoding.UTF8);

        while (await sr.ReadLineAsync().ConfigureAwait(false) is { } line)
        {
            if (!line.Contains("https://glz-")) continue;
            var text = line.Split("https://glz-")[1].Split('.', '-');
            Constants.Region = text[0];
            Constants.Shard = text[2];
            Constants.Log.Information("Region Detected: {region} Shard: {shard}", text[0], text[2]);
            fs.Close();
            sr.Close();
            return;
        }

        fs.Close();
        sr.Close();
        Constants.Log.Error("LocalRegionAsync() Failed: https://glz- could not be found in logs");
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
            try
            {
                var incorrectContent = response.Content.Replace("[", string.Empty).Replace("]", string.Empty).Replace("\n", string.Empty);
                var content = JsonSerializer.Deserialize<NameServiceResponse>(incorrectContent);
                return content.GameName + "#" + content.TagLine;
            }
            catch (Exception e)
            {
                Constants.Log.Error("GetNameServiceGetUsernameAsync Failed: {e}", e);
                return "";
            }

        Constants.Log.Error("GetNameServiceGetUsernameAsync Failed: {e}", response.ErrorException);
        return "";
    }


    private static async Task GetLatestVersionAsync()
    {
        if (!File.Exists(Constants.LocalAppDataPath + "\\ValAPI\\version.txt"))
        {
            Constants.Log.Error("GetLatestVersionAsync() Failed: version.txt not found");
            await ValApi.UpdateFilesAsync().ConfigureAwait(false);
            return;
        }

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