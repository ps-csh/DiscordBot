using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Utility.Web
{
    interface IHttpRequestHandler
    {
        public Task<WebRequestResult> GetRequest(string url, Action<HttpWebRequest> webRequestBuilder = null);
        public Task<WebRequestResult> PostRequest(string url, byte[] data, Action<HttpWebRequest> webRequestBuilder = null);
        public Task<WebRequestResult> PostRequest(string url, string data, Action<HttpWebRequest> webRequestBuilder = null);
    }

    public class WebRequestResult
    {
        public string Data { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public string StatusDescription { get; set; }
        public WebHeaderCollection Headers { get; set; }
    }
}
