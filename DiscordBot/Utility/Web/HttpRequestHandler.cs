using DiscordBot.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Utility.Web
{
    public class HttpRequestHandler : IHttpRequestHandler
    {
        public string UserAgent { get; set; }

        private readonly WebHeaderCollection defaultHeaderCollection;

        public HttpRequestHandler(IOptions<BotSettingsConfiguration> options)
        {
            UserAgent = options.Value.UserAgent;

            defaultHeaderCollection = new WebHeaderCollection();
            defaultHeaderCollection.Add("User-Agent", options.Value.UserAgent);
        }

        public virtual async Task<WebRequestResult> GetRequest(string url, Action<HttpWebRequest> webRequestBuilder = null)
        {
            try
            {
                HttpWebRequest httpWebRequest = WebRequest.CreateHttp(url);
                httpWebRequest.Method = "GET";
                httpWebRequest.Headers = defaultHeaderCollection;
                webRequestBuilder?.Invoke(httpWebRequest);

                WebResponse response = await httpWebRequest.GetResponseAsync();
                var httpResponse = response as HttpWebResponse;

                StreamReader reader = new StreamReader(response.GetResponseStream());
                string message = await reader.ReadToEndAsync();

                var result = new WebRequestResult()
                {
                    Data = message,
                    StatusCode = httpResponse.StatusCode,
                    StatusDescription = httpResponse.StatusDescription,
                    Headers = httpResponse.Headers
                };

                return result;
            }
            catch (UriFormatException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public virtual async Task<WebRequestResult> PostRequest(string url, byte[] data, Action<HttpWebRequest> webRequestBuilder = null)
        {
            try
            {
                HttpWebRequest httpWebRequest = WebRequest.CreateHttp(url);
                httpWebRequest.Method = "POST";
                httpWebRequest.Headers = defaultHeaderCollection;
                webRequestBuilder?.Invoke(httpWebRequest);

                httpWebRequest.GetRequestStream().Write(data, 0, data.Length);

                WebResponse response = await httpWebRequest.GetResponseAsync();
                var httpResponse = response as HttpWebResponse;

                StreamReader reader = new StreamReader(httpResponse.GetResponseStream());
                string message = await reader.ReadToEndAsync();

                var result = new WebRequestResult()
                {
                    Data = message,
                    StatusCode = httpResponse.StatusCode,
                    StatusDescription = httpResponse.StatusDescription,
                    Headers = httpResponse.Headers
                };

                return result;
            }
            catch (UriFormatException ex)
            {
                throw ex;
            }
            catch (WebException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public virtual Task<WebRequestResult> PostRequest(string url, string data, Action<HttpWebRequest> webRequestBuilder = null)
        {
            return PostRequest(url, Encoding.UTF8.GetBytes(data.ToString()), webRequestBuilder);
        }
    }
}
