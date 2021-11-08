using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace WAIUA.ValAPI
{
    public class ValAPI
    {

        public static async Task<string> GetValApiVersion()
        {
            RestClient client = new(new Uri("https://valorant-api.com/v1/version"));
            RestRequest request = new(Method.GET);
            var response = await client.ExecuteGetAsync(request);
            string content = response.Content;
            var responsevar = JsonConvert.DeserializeObject(content);
            JToken responseObj = JObject.FromObject(responsevar);
            return responseObj["data"]["version"].Value<string>();
        }

        public static async Task<string> GetLocalValApiVersion()
        {
            string content = null;
            using (StreamReader r = new StreamReader("version.json"))
            {
                content = r.ReadToEnd();
            }
            var responsevar = JsonConvert.DeserializeObject(content);
            JToken responseObj = JObject.FromObject(responsevar);
            return responseObj["data"]["version"].Value<string>();
        }

        public static async Task<string> CheckAndUpdateJson()
        {
            if (await GetValApiVersion() != "223")
            {

            }
        }

    }
}
