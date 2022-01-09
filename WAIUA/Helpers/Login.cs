using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace WAIUA.Helpers;

public static class Login
{
	public static bool GetSetPPUUID()
	{
		RestClient client = new(new Uri("https://auth.riotgames.com/userinfo"));
		RestRequest request = new(Method.POST);
		request.AddHeader("Authorization", $"Bearer {Constants.AccessToken}");
		request.AddJsonBody("{}");
		var response = client.Execute(request);
		var statusCode = response.StatusCode;
		var numericStatusCode = (short)statusCode;
		if (numericStatusCode != 200) return false;

		var content = response.Content;
		var PlayerInfo = JsonConvert.DeserializeObject(content);
		JToken PUUIDObj = JObject.FromObject(PlayerInfo);
		Constants.PPUUID = PUUIDObj["sub"].Value<string>();
		return true;
	}

	public static bool CheckLocal()
	{
		var lockfileLocation =
			$@"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\Riot Games\Riot Client\Config\lockfile";

		if (File.Exists(lockfileLocation))
		{
			using FileStream fileStream = new(lockfileLocation, FileMode.Open, FileAccess.ReadWrite,
				FileShare.ReadWrite);
			using StreamReader sr = new(fileStream);
			var parts = sr.ReadToEnd().Split(":");
			Constants.Port = parts[2];
			Constants.LPassword = parts[3];

			return true;
		}

		return false;
	}

	public static void LocalLogin()
	{
		GetLatestVersion();
		RestClient client = new(new Uri($"https://127.0.0.1:{Constants.Port}/entitlements/v1/token"));
		RestRequest request = new(Method.GET);
		client.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
		request.AddHeader("Authorization",
			$"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes($"riot:{Constants.LPassword}"))}");
		request.AddHeader("X-Riot-ClientPlatform",
			"ew0KCSJwbGF0Zm9ybVR5cGUiOiAiUEMiLA0KCSJwbGF0Zm9ybU9TIjogIldpbmRvd3MiLA0KCSJwbGF0Zm9ybU9TVmVyc2lvbiI6ICIxMC4wLjE5MDQyLjEuMjU2LjY0Yml0IiwNCgkicGxhdGZvcm1DaGlwc2V0IjogIlVua25vd24iDQp9");
		request.AddHeader("X-Riot-ClientVersion", Constants.Version);
		request.RequestFormat = DataFormat.Json;
		var response = client.Get(request);
		var content = client.Execute(request).Content;
		var responsevar = JsonConvert.DeserializeObject(content);
		JToken responseObj = JObject.FromObject(responsevar);
		Constants.AccessToken = responseObj["accessToken"].Value<string>();
		Constants.EntitlementToken = responseObj["token"].Value<string>();
	}

	public static void LocalRegion()
	{
		RestClient client = new(new Uri($"https://127.0.0.1:{Constants.Port}/product-session/v1/external-sessions"));
		RestRequest request = new(Method.GET);
		client.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
		request.AddHeader("Authorization",
			$"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes($"riot:{Constants.LPassword}"))}");
		request.AddHeader("X-Riot-ClientPlatform",
			"ew0KCSJwbGF0Zm9ybVR5cGUiOiAiUEMiLA0KCSJwbGF0Zm9ybU9TIjogIldpbmRvd3MiLA0KCSJwbGF0Zm9ybU9TVmVyc2lvbiI6ICIxMC4wLjE5MDQyLjEuMjU2LjY0Yml0IiwNCgkicGxhdGZvcm1DaGlwc2V0IjogIlVua25vd24iDQp9");
		request.AddHeader("X-Riot-ClientVersion", Constants.Version);
		request.RequestFormat = DataFormat.Json;
		var response = client.Get(request);
		var content = client.Execute(request).Content;
		var root = JObject.Parse(content);
		var property = (JProperty)root.First;
		var fullstring = property.Value["launchConfiguration"]["arguments"][3];
		var parts = fullstring.ToString().Split('=', '&');
		var output = parts[1];
		if (output == "latam")
		{
			Constants.Region = "na";
			Constants.Shard = "latam";
		}
		else if (output == "br")
		{
			Constants.Region = "na";
			Constants.Shard = "br";
		}
		else
		{
			Constants.Region = output;
			Constants.Shard = output;
		}
	}

    public static bool CheckMatchID()
    {
        var url =
            $"https://glz-{Constants.Shard}-1.{Constants.Region}.a.pvp.net/core-game/v1/players/{Constants.PPUUID}";
        RestClient client = new(url);
        RestRequest request = new(Method.GET);
        request.AddHeader("X-Riot-Entitlements-JWT", Constants.EntitlementToken);
        request.AddHeader("Authorization", $"Bearer {Constants.AccessToken}");
        var response = client.Execute(request);
        if (!response.IsSuccessful) return false;
        return true;
    }

    public static string GetIGUsername(string puuid)
    {
        var IGN = "";
        try
        {
            var url = $"https://pd.{Constants.Region}.a.pvp.net/name-service/v2/players";
            RestClient client = new(url);
            RestRequest request = new(Method.PUT)
            {
                RequestFormat = DataFormat.Json
            };

            string[] body = { puuid };
            request.AddJsonBody(body);

            var response = client.Put(request);
            var content = client.Execute(request).Content;

            content = content.Replace("[", "");
            content = content.Replace("]", "");

            var uinfo = JsonConvert.DeserializeObject(content);
            JToken uinfoObj = JObject.FromObject(uinfo);
            IGN = uinfoObj["GameName"].Value<string>() + "#" + uinfoObj["TagLine"].Value<string>();
        }
        catch (Exception)
        {
            // ignored
        }

        return IGN;
    }

	private static void GetLatestVersion()
	{
		var content = Task.Run(() => ValAPI.LoadJsonFromFile("\\ValAPI\\version.json")).Result;
		Constants.Version = content.data.riotClientVersion;
	}

    //private static string DoCachedRequest(Method method, string url, bool addRiotAuth, bool bypassCache = false) // Thank you MitchC for this, I am always touched when random people go out of the way to help others even though they know that we would be clueless and need to ask alot of followup questions
    //{
    //	var tsk = DoCachedRequestAsync(method, url, addRiotAuth, bypassCache);
    //	return tsk.Result;
    //}

    public static string DoCachedRequest(Method method, string url, bool addRiotAuth, bool bypassCache = false)
    {
        var attemptCache = method == Method.GET && !bypassCache;
        if (attemptCache)
            if (Constants.UrlToBody.TryGetValue(url, out var res))
                return res;

        RestClient client = new(url);
        RestRequest request = new(method);
        if (addRiotAuth)
        {
            request.AddHeader("X-Riot-Entitlements-JWT", Constants.EntitlementToken);
            request.AddHeader("Authorization", $"Bearer {Constants.AccessToken}");
            request.AddHeader("X-Riot-ClientPlatform",
                "ew0KCSJwbGF0Zm9ybVR5cGUiOiAiUEMiLA0KCSJwbGF0Zm9ybU9TIjogIldpbmRvd3MiLA0KCSJwbGF0Zm9ybU9TVmVyc2lvbiI6ICIxMC4wLjE5MDQyLjEuMjU2LjY0Yml0IiwNCgkicGxhdGZvcm1DaGlwc2V0IjogIlVua25vd24iDQp9");
            request.AddHeader("X-Riot-ClientVersion", Constants.Version);
        }

        var resp = client.Execute(request);
        if (!resp.IsSuccessful) return null;
        var cont = resp.Content;

        if (attemptCache) Constants.UrlToBody.TryAdd(url, cont);
        return cont;
    }

    private static async Task<string> DoCachedRequestAsync(Method method, string url, bool addRiotAuth,
        bool bypassCache = false)
    {
        var attemptCache = method == Method.GET && !bypassCache;
        if (attemptCache)
            if (Constants.UrlToBody.TryGetValue(url, out var res))
                return res;

        RestClient client = new(url);
        RestRequest request = new(method);
        if (addRiotAuth)
        {
            request.AddHeader("X-Riot-Entitlements-JWT", Constants.EntitlementToken);
            request.AddHeader("Authorization", $"Bearer {Constants.AccessToken}");
            request.AddHeader("X-Riot-ClientPlatform",
                "ew0KCSJwbGF0Zm9ybVR5cGUiOiAiUEMiLA0KCSJwbGF0Zm9ybU9TIjogIldpbmRvd3MiLA0KCSJwbGF0Zm9ybU9TVmVyc2lvbiI6ICIxMC4wLjE5MDQyLjEuMjU2LjY0Yml0IiwNCgkicGxhdGZvcm1DaGlwc2V0IjogIlVua25vd24iDQp9");
            request.AddHeader("X-Riot-ClientVersion", Constants.Version);
        }

        var resp = await client.ExecuteAsync(request);
        if (resp.IsSuccessful && resp.RawBytes != null)
        {
            var cont = resp.Content;

            if (attemptCache) Constants.UrlToBody.TryAdd(url, cont);
            return cont;
        }

        return null;
    }

}