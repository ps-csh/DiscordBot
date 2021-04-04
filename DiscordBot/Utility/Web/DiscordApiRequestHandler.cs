using DiscordBot.Configuration;
using DiscordBot.DiscordAPI;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace DiscordBot.Utility.Web
{
    public class DiscordApiRequestHandler : HttpRequestHandler
    {
        /**
         *  < HTTP/1.1 429 TOO MANY REQUESTS
            < Content-Type: application/json
            < Retry-After: 65
            < X-RateLimit-Limit: 10
            < X-RateLimit-Remaining: 0
            < X-RateLimit-Reset: 1470173023.123
            < X-RateLimit-Reset-After: 64.57
            < X-RateLimit-Bucket: abcd1234
            {
              "message": "You are being rate limited.",
              "retry_after": 64.57,
              "global": false
            }
         */

        private const string RATE_BUCKET_HEADER = "X-RateLimit-Bucket";
        private const string RATE_LIMIT_HEADER = "X-RateLimit-Limit";
        private const string RATE_REMAINING_HEADER = "X-RateLimit-Remaining";
        private const string RATE_RESET_HEADER = "X-RateLimit-Reset";
        private const string RATE_RESET_AFTER_HEADER = "X-RateLimit-Reset-After";

        private const string CHANNEL_ENDPOINT_REGEX = @"^/api/channel/(?<channelId>\d+)/$";

        private const int MAX_SEMAPHORES = 10;

        public int RateLimit { get; set; }
        public int RateReset { get; set; }

        private Timer resetTimer;
        private SemaphoreSlim Semaphore { get; set; }
        public List<RateBucket> RateBuckets { get; private set; }

        private bool globalRateLimit;
        private int globalRateReset;
        private Timer globalResetTimer;

        public DiscordApiRequestHandler(IOptions<BotSettingsConfiguration> options) : base(options)
        {
            Semaphore = new SemaphoreSlim(5, MAX_SEMAPHORES);
            RateBuckets = new List<RateBucket>();
        }

        public override async Task<WebRequestResult> GetRequest(string url, Action<HttpWebRequest> webRequestBuilder = null)
        {
            try
            {
                string endpoint = GetEndpoint(url);
                var bucket = GetRateBucket(endpoint);
                if (bucket?.Semaphore != null)
                {
                    await bucket.Semaphore.WaitAsync();
                }

                try
                {
                    var response = await base.GetRequest(url, webRequestBuilder);

                    if (response.StatusCode == HttpStatusCode.TooManyRequests)
                    {
                        int retryAfterOld = (int)(float.Parse(response.Headers[RATE_RESET_AFTER_HEADER]) * 1000);

                        //resetTimer = new Timer(ResetRateBucket, Semaphore, retryAfterOld, Timeout.Infinite);

                        //TODO: Resend after time expires
                    }

                    // Creates a bucket if one didn't exist for this endpoint
                    SetRateBucket(response.Headers, endpoint);

                    return response;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public override async Task<WebRequestResult> PostRequest(string url, byte[] data, Action<HttpWebRequest> webRequestBuilder = null)
        {
            try
            {
                string endpoint = GetEndpoint(url);
                var bucket = GetRateBucket(endpoint);
                if (bucket?.Semaphore != null)
                {
                    await bucket.Semaphore.WaitAsync();
                }

                try
                {
                    var response = await base.PostRequest(url, data, webRequestBuilder);

                    if (response.StatusCode == HttpStatusCode.TooManyRequests)
                    {
                        int retryAfter = (int)(float.Parse(response.Headers[RATE_RESET_AFTER_HEADER]) * 1000);

                        //resetTimer = new Timer(ResetRateBucket, Semaphore, retryAfterOld, Timeout.Infinite);
                        await Task.Delay(retryAfter);

                        //TODO: Resend after time expires
                    }

                    //int resetAfter = (int)(float.Parse(response.Headers[RATE_RESET_AFTER_HEADER]) * 1000);
                    //int requestsRemaining = int.Parse(response.Headers[RATE_REMAINING_HEADER]);
                    // Creates a bucket if one didn't exist for this endpoint
                    SetRateBucket(response.Headers, endpoint);

                    return response;
                }
                catch (WebException ex)
                {
                    //ex.Response
                    throw ex;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string GetEndpoint(string url)
        {
            if (url.StartsWith(DiscordEndpoints.CHANNEL))
            {
                var channelRegex = Regex.Match(url, CHANNEL_ENDPOINT_REGEX);
                if (channelRegex.Success)
                {
                    return DiscordEndpoints.CHANNEL + channelRegex.Groups["channelId"];
                }
            }

            return null;
        }

        private RateBucket GetRateBucket(string endpoint)
        {
            //TEMP: Change back to FirstOrDefault or ensure that buckets get updated
            // if another bucket with different Id but same endpoint is created
            var bucket = RateBuckets.SingleOrDefault(r => r.Endpoint == endpoint);
            return bucket;
        }

        private RateBucket SetRateBucket(WebHeaderCollection headers, string endpoint)
        {
            try
            {
                string id = headers[RATE_BUCKET_HEADER];
                int requestLimit = int.Parse(headers[RATE_LIMIT_HEADER]);
                int requestsRemaining = int.Parse(headers[RATE_REMAINING_HEADER]);
                int resetAfter = int.Parse(headers[RATE_RESET_AFTER_HEADER]);

                var bucket = RateBuckets.FirstOrDefault(r => r.Id == id);
                if (bucket == null)
                {
                    bucket = new RateBucket(id, endpoint, requestLimit, requestsRemaining, resetAfter);
                    RateBuckets.Add(bucket);
                }
                else
                {
                    bucket.UpdateRates(requestsRemaining, resetAfter);
                }

                return bucket;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public class RateBucket
        {
            public string Id { get; set; }
            public string Endpoint { get; set; }
            public SemaphoreSlim Semaphore { get; set; }
            public Timer ResetTimer { get; set; }
            public int RequestLimit { get; set; }
            public int RequestsRemaining { get; set; }

            public RateBucket(string id, string endpoint, int requestLimit, int requestsRemaining, int resetAfter)
            {
                Id = id;
                Endpoint = endpoint;
                RequestLimit = requestLimit;
                RequestsRemaining = requestsRemaining;
                Semaphore = new SemaphoreSlim(RequestsRemaining, RequestLimit);
                ResetTimer = new Timer(ReleaseSemaphores, Semaphore, resetAfter, Timeout.Infinite);
            }

            public void UpdateRates(int requestsRemaining, int resetAfter)
            {
                if (ResetTimer != null)
                {
                    RequestsRemaining = requestsRemaining;
                }
                else
                {
                    ResetTimer = new Timer(ReleaseSemaphores, Semaphore, resetAfter, Timeout.Infinite);
                }
            }

            private void ReleaseSemaphores(object? state)
            {
                try
                {
                    var semaphore = (SemaphoreSlim)state;
                    semaphore.Release(RequestLimit - semaphore.CurrentCount);

                    RequestsRemaining = semaphore.CurrentCount;
                    ResetTimer?.Dispose();
                    ResetTimer = null;
                }  
                catch(Exception)
                {
                    //TODO: Is there any error handling to do here?
                    // assuming it's called by an asynchronous Timer
                }
            }
        }
    }
}
