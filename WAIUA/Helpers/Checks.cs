using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using RestSharp;

namespace WAIUA.Helpers;

public class Checks
{
    public static async Task<bool> CheckLoginAsync()
    {
        if (Constants.Region == null || Constants.Ppuuid == Guid.Empty) return false;
        var client = new RestClient($"https://pd.{Constants.Region}.a.pvp.net/account-xp/v1/players/{Constants.Ppuuid}");

        var request = new RestRequest().AddHeader("Authorization", $"Bearer {Constants.AccessToken}")
            .AddHeader("X-Riot-Entitlements-JWT", Constants.EntitlementToken);
        var response = await client.ExecuteGetAsync(request).ConfigureAwait(false);
        if (response.IsSuccessful) return true;
        Constants.Log.Warning("CheckLoginAsync() failed. Response: {Response}", response.ErrorException);
        return false;
    }

    public static async Task<bool> CheckLocalAsync()
    {
        var lockfileLocation =
            $@"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\Riot Games\Riot Client\Config\lockfile";

        if (!File.Exists(lockfileLocation))
            // Constants.Log.Warning("Valorant Not detected");
            return false;

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

    public static async Task<bool> CheckMatchAsync()
    {
        var client = new RestClient($"https://glz-{Constants.Shard}-1.{Constants.Region}.a.pvp.net/core-game/v1/players/{Constants.Ppuuid}");
        var request = new RestRequest();
        request.AddHeader("X-Riot-Entitlements-JWT", Constants.EntitlementToken);
        request.AddHeader("Authorization", $"Bearer {Constants.AccessToken}");
        var response = await client.ExecuteGetAsync(request).ConfigureAwait(false);
        if (response.IsSuccessful) return true;

        client = new RestClient($"https://glz-{Constants.Shard}-1.{Constants.Region}.a.pvp.net/pregame/v1/players/{Constants.Ppuuid}");
        response = await client.ExecuteGetAsync(request).ConfigureAwait(false);
        if (response.IsSuccessful) return true;

        // Constants.Log.Error("CheckMatchAsync Failed: {e}", response.ErrorException);
        return false;
    }
}