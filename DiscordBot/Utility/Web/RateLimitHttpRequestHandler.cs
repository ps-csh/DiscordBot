using DiscordBot.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DiscordBot.Utility.Web
{
    public class RateLimitHttpRequestHandler : HttpRequestHandler
    {
        private const int MAX_SEMAPHORES = 10;

        public int RateLimit { get; set; }
        public int RateReset { get; set; }

        private SemaphoreSlim semaphore;

        public RateLimitHttpRequestHandler(IOptions<BotSettingsConfiguration> options) : base(options)
        {
            semaphore = new SemaphoreSlim(5, MAX_SEMAPHORES);
        }

        public override Task<WebRequestResult> GetRequest(string url, Action<HttpWebRequest> webRequestBuilder)
        {
            try
            {
                semaphore.WaitAsync();

                try
                {
                    var response = base.GetRequest(url);
                    response.ContinueWith(async t => await Task.Delay(RateReset));

                    return response;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    semaphore.Release();
                }
            }
            catch (SemaphoreFullException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public override Task<WebRequestResult> PostRequest(string url, byte[] data, Action<HttpWebRequest> webRequestBuilder)
        {
            return base.PostRequest(url, data);
        }
    }
}
