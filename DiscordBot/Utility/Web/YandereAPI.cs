using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using RestSharp;
using System.Net.Security;
using System.Runtime.InteropServices;
using System.Diagnostics;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DiscordBot.Utility.Web
{
    public class YandereAPIClient
    {
        private const string YANDERE_BASE_URL = @"https://yande.re/";
        private const string YANDERE_POOL_LIST_URL = @"https://yande.re/pool";
        private const string YANDERE_POOL_URL = @"https://yande.re/pool/show/";
        private const string YANDERE_POST_URL = @"https://yande.re/post/show/";

        private readonly WebHeaderCollection defaultHeaderCollection = new WebHeaderCollection();

        private readonly HttpRequestHandler httpRequestHandler;
        private readonly ILogger Logger;

        //TODO: Add concurrent request limits
        public YandereAPIClient(HttpRequestHandler webRequestHandler, ILogger logger)
        {
            defaultHeaderCollection.Add("User-Agent", "Discord(null, 0.1)");
            httpRequestHandler = webRequestHandler;
            Logger = logger;
        }

        public async Task<List<Pool>> GetPools(string query, string orderBy = null, string format = "json")
        {
            try
            {
                string url = $"{YANDERE_POOL_LIST_URL}.{format}?";
                if (query != null) { url += $"query={query}"; }
                if (orderBy != null) { url += $"&order={orderBy}"; }

                var response = await httpRequestHandler.GetRequest(url);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var pools = JsonConvert.DeserializeObject<Pool[]>(response.Data);

                    return new List<Pool>(pools);
                }
                else
                {
                    Logger.LogWarning($"GetPools request failed: {response.StatusCode} - {response.StatusDescription}\n{response.Data}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, $"GetPools {query}");
                return null;
            }
        }

        public async Task<Pool> GetPool(string poolId)
        {
            try
            {
                string url = $"{YANDERE_POOL_URL}{poolId}.json";

                var response = await httpRequestHandler.GetRequest(url);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    //JObject jObject = JObject.Parse(response.Message);
                    var pool = JsonConvert.DeserializeObject<Pool>(response.Data);
                    return pool;
                }
                else
                {
                    Logger.LogWarning($"GetPool request failed: {response.StatusCode} - {response.StatusDescription}\n{response.Data}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, $"GetPool {poolId}");
                return null;
            }
        }

        public class Pool
        {
            public string ID { get; set; }
            public string Name { get; set; }

            [JsonProperty("post_count")]
            public int PostCount { get; set; }

            [JsonProperty("created_at")]
            public string CreatedAt { get; set; }

            [JsonProperty("updated_at")]
            public string UpdatedAt { get; set; }

            public List<Post> Posts { get; set; }

            [JsonIgnore]
            public string Url => $"{YANDERE_POOL_URL}{ID}";
        }

        public class Post
        {
            public string ID { get; set; }
            public string Tags { get; set; }

            [JsonProperty("file_url")]
            public string FileUrl { get; set; }

            // Ratings are character codes (s - Safe, q - Questionable, e - Explicit?)
            public string Rating { get; set; }

            [JsonIgnore]
            public string Url => $"{YANDERE_POST_URL}{ID}";
        }
    }
}
