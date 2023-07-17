using DiscordBot.Configuration;
using DiscordBot.DiscordAPI.Structures;
using DiscordBot.Utility;
using DiscordBot.Utility.Web;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.DiscordAPI
{
    /// <summary>
    /// Handles HTTP messages to the Discord Web API
    /// </summary>
    public class DiscordApiClient
    {
        const int MESSAGE_CHARACTER_LIMIT = 2000;

        private readonly WebHeaderCollection defaultHeaderCollection = new WebHeaderCollection();

        private readonly WebHeaderCollection multipartHeaderCollection = new WebHeaderCollection();

        private readonly ILogger Logger;

        DiscordApiRequestHandler HttpRequestHandler { get; set; }

        public DiscordApiClient(IOptions<BotSecretsConfiguration> secrets, IOptions<BotSettingsConfiguration> options, 
            DiscordApiRequestHandler requestHandler, ILogger logger)
        {
            //defaultHeaderCollection = new WebHeaderCollection(); 
            string authorizationHeader = $"{secrets.Value.TokenType} {secrets.Value.Token}";
            string userAgentHeader = options.Value.UserAgent;

            Logger = logger;
            HttpRequestHandler = requestHandler;

            defaultHeaderCollection.Add("Authorization", authorizationHeader);
            defaultHeaderCollection.Add("User-Agent", userAgentHeader);
            defaultHeaderCollection.Add("Content-Type", "application/json");

            multipartHeaderCollection.Add("Authorization", authorizationHeader);
            multipartHeaderCollection.Add("User-Agent", userAgentHeader);
            multipartHeaderCollection.Add("Content-Type", "multipart/form-data");
        }

        public async Task<WebRequestResult> PostMessage(string message, string channel)
        {
            try
            {
                //TODO: Try to string format this using the DiscordEndpoints class
                string channelUrl = DiscordEndpoints.CHANNEL + channel + "/messages";

                if (message.Length > MESSAGE_CHARACTER_LIMIT)
                {
                    List<string> separatedMessages = SplitMessage(message, MESSAGE_CHARACTER_LIMIT);

                    var response = await SendMultiple(separatedMessages, channelUrl);

                    return response;
                }
                else
                {
                    JObject postMessage = new JObject
                    {
                        ["content"] = message,
                        ["tts"] = false
                    };

                    var response = await PostRequest(postMessage.ToString(), channelUrl);

                    return response;
                }      
            }
            catch (WebException ex)
            {
                //TODO: Notify if it was a bad request
                Logger.LogException(ex, $"Received WebException {ex.Status}\r\nWhen sending: {message} to {channel}");
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, $"When sending: {message} to {channel}");
            }

            return null;
        }

        public async Task<WebRequestResult> PostMessage(SendMessage message, string channel)
        {
            try
            {
                string channelUrl = DiscordEndpoints.CHANNEL + channel + "/messages";

                //TODO: If SendMessage contains attached file, use multipart headers instead

                // If the SendMessage contains an embed it will be split differently
                if (!string.IsNullOrEmpty(message.Content) && message.Content.Length > MESSAGE_CHARACTER_LIMIT)
                {
                    List<string> separatedMessages = SplitMessage(message.Content, MESSAGE_CHARACTER_LIMIT);
                    var sendMessages = separatedMessages.Select(s => new SendMessage(s)
                    {
                        Tts = message.Tts,
                        AllowedMentions = null, //TODO: Create class for AllowedMentions structure, avoid phantom pings
                    });

                    var response = await SendMultiple(separatedMessages, channelUrl);

                    if (message.Embed != null)
                    {
                        var embeds = SplitEmbed(message.Embed);
                        response = await SendMultiple(embeds, channelUrl);
                    }

                    return response;
                }
                else if (message.Embed?.IsValidRichEmbed == false)
                {
                    if (!string.IsNullOrEmpty(message.Content))
                    {
                        SendMessage sendMessage = new SendMessage()
                        {
                            Content = message.Content,
                            Tts = message.Tts,
                            AllowedMentions = message.AllowedMentions,
                        };
                        await PostRequest(sendMessage.ToJson(), channelUrl);
                    }

                    var embeds = SplitEmbed(message.Embed); //TODO: Set AllowedMentions if embeds need them
                    var response = await SendMultiple(embeds, channelUrl);
                    return response;
                }
                else
                {
                    var response = await PostRequest(message.ToJson(), channelUrl);

                    //TEMP:
                    var headerCollection = Enumerable.Range(0, response.Headers.Count)
                        .SelectMany(i => response.Headers.GetValues(i)
                        .Select(v => Tuple.Create(response.Headers.GetKey(i), v)));
                    foreach (var header in headerCollection)
                    {
                        Logger.LogInfo($"{header.Item1}: {header.Item2}");
                    }

                    //IAsyncResult result = httpWebRequest.BeginGetResponse(RequestCallback, null);
                    Console.WriteLine(response.ToString());

                    return response;
                }
            }
            catch (WebException ex)
            {
                //TODO: Notify if it was a bad request
                var response = ((HttpWebResponse)ex.Response);
                Logger.LogException(ex, $"Received {response.StatusCode}: {response.StatusDescription}\n" +
                    $"Message: " + message.Content);
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "When sending message");
            }

            return null;
        }

        public Task<WebRequestResult> PostMessage(Embed message, string channel)
        {
            return PostMessage(new SendMessage(message), channel);
        }

        public Task<WebRequestResult> PostMessage(Embed message, Channel channel)
        {
            return PostMessage(message, channel.ID);
        }

        public Task<WebRequestResult> PostMessage(string message, Channel channel)
        {
            return PostMessage(message, channel.ID);
        }

        public Task<WebRequestResult> PostMessage(SendMessage message, Channel channel)
        {
            return PostMessage(message, channel.ID);
        }

        public async Task<WebRequestResult> PostMessage(IEnumerable<SendMessage> messages, string channel)
        {
            WebRequestResult response = null;
            foreach (var message in messages)
            {
                response = await PostMessage(message, channel);
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    break;
                }
            }

            return response;
        }

        public Task<WebRequestResult> PostMessage(IEnumerable<SendMessage> messages, Channel channel)
        {
            return PostMessage(messages, channel.ID);
        }

        private async Task<WebRequestResult> PostRequest(byte[] data, string url)
        {
            var response = await HttpRequestHandler.PostRequest(url, data, (httpWebRequest) => {
                httpWebRequest.AutomaticDecompression = DecompressionMethods.GZip;
                httpWebRequest.Headers = defaultHeaderCollection;
            });

            return response;
        }

        private async Task<WebRequestResult> PostRequest(string message, string url)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);

            return await PostRequest(data, url);
        }

        private Task<WebRequestResult> SendMultiple(IEnumerable<string> messages, string channelUrl)
        {
            var sendMessages = messages.Select((m) => new SendMessage(m));
            return SendMultiple(sendMessages, channelUrl);
        }

        private Task<WebRequestResult> SendMultiple(IEnumerable<Embed> embeds, string channelUrl)
        {
            var sendMessages = embeds.Select((e) => new SendMessage(e)
            {
                Tts = false,
                AllowedMentions = null, //TODO:
            });
            return SendMultiple(sendMessages, channelUrl);
        }

        private async Task<WebRequestResult> SendMultiple(IEnumerable<SendMessage> messages, string channelUrl)
        {
            WebRequestResult lastResult = null;
            foreach (var message in messages)
            {
                message.SetNonce();

                lastResult = await PostRequest(message.ToJson(), channelUrl);
                if (lastResult.StatusCode == HttpStatusCode.OK)
                {
                    try
                    {
                        var responseMessage = JObject.Parse(lastResult.Data).ToObject<Message>();
                        if (responseMessage.Nonce != message.Nonce)
                        {
                            Logger.LogWarning($"Response contained a different Nonce value\n" +
                                $"{responseMessage.ToJson()}");
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogException(ex);
                    }
                }
                else
                {
                    Logger.LogWarning($"Did not receive Http response 200 (OK) when sending separated messages\n" +
                        $"{lastResult.StatusCode}: {lastResult.Data}");
                    return lastResult;
                }

                Logger.LogInfo(lastResult.Data);
            }

            return lastResult;
        }

        private void SetHttpRequestOptions(HttpWebRequest httpWebRequest)
        {
            httpWebRequest.Headers = defaultHeaderCollection;
            httpWebRequest.AutomaticDecompression = DecompressionMethods.GZip;
        }

        private List<string> SplitMessage(string message, int maxCharacters)
        {
            List<string> separatedMessages = new List<string>();
            while (message.Length > maxCharacters)
            {
                string currentMessage = message.Substring(0, maxCharacters);
                // Attempt to split the message from the last found space or newline in the substring
                int lastSpaceIndex = currentMessage.LastIndexOfAny(new[] { '\n', ' ' });
                if (lastSpaceIndex > 0)
                {
                    currentMessage = currentMessage.Substring(0, lastSpaceIndex);
                }
                separatedMessages.Add(currentMessage);
                message = message.Remove(0, currentMessage.Length);
            }
            // Add the remaining string. Check for zero in case the message was split at the exact maxCharacters limit
            if (message.Length > 0) { separatedMessages.Add(message); }

            return separatedMessages;
        }

        private List<Embed> SplitEmbed(Embed embed)
        {
            List<Embed> embeds = new List<Embed>();

            Embed firstEmbed = new Embed(embed.Title, description: embed.Description).SetFooter("...");
            for (int i = 0; i < Embed.MAX_FIELD_COUNT && embed.Fields.Count > 0; ++i)
            {
                var field = embed.Fields.First();
                // Break from this loop if the Field would cause the embed to exceed the character limit
                if (firstEmbed.TotalRichEmbedCharacters + field.Name.Length +
                    field.Value.Length > Embed.MAX_RICH_EMBED_CHARACTERS)
                {
                    break;
                }
                firstEmbed.AddField(field.Name, field.Value, field.Inline);
                embed.Fields.RemoveAt(0);
            }
            embeds.Add(firstEmbed);

            while (!embed.IsValidRichEmbed)
            {
                var tempEmbed = new Embed(embed.Title, description: "...continued").SetFooter("...");
                for (int i = 0; i < Embed.MAX_FIELD_COUNT; ++i)
                {
                    var field = embed.Fields.First();
                    // Break from this loop if the Field would cause the embed to exceed the character limit
                    if (tempEmbed.TotalRichEmbedCharacters + field.Name.Length +
                        field.Value.Length > Embed.MAX_RICH_EMBED_CHARACTERS)
                    {
                        break;
                    }
                    tempEmbed.AddField(field.Name, field.Value, field.Inline);
                    embed.Fields.RemoveAt(0);
                }
                embeds.Add(tempEmbed);
            }

            embeds.Add(embed.SetDescription("...continued"));

            return embeds;
        }
    }
}
