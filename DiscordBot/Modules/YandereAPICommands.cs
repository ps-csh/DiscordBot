using DiscordBot.Bot.Commands;
using DiscordBot.Configuration;
using DiscordBot.DiscordAPI;
using DiscordBot.DiscordAPI.Structures;
using DiscordBot.Utility;
using DiscordBot.Utility.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Modules
{
    public class YandereAPICommands : CommandModule
    {
        private readonly WebHeaderCollection defaultHeaderCollection = new WebHeaderCollection();

        private readonly YandereAPIClient yandereAPIClient;
        private readonly DiscordApiClient apiClient;
        private readonly ILogger logger;

        public YandereAPICommands(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            yandereAPIClient = serviceProvider.GetRequiredService<YandereAPIClient>();
            apiClient = serviceProvider.GetRequiredService<DiscordApiClient>();
            logger = serviceProvider.GetRequiredService<ILogger>();
            //var options = serviceProvider.GetRequiredService<IOptions<BotSecretsConfiguration>>();
            defaultHeaderCollection.Add("User-Agent", "DiscordBot(null, 0.1)");
        }

        [Command("yanderelistpools")]
        private async Task QueryPools(CommandInfo commandInfo)
        {
            if (string.IsNullOrEmpty(commandInfo.Arguments))
            {
                await apiClient.PostMessage("Please specify a query", commandInfo.Channel.ID);
                return;
            }

            var results = await yandereAPIClient.GetPools(commandInfo.Arguments, "created");
            if (results != null)
            {
                Embed embed = new Embed(commandInfo.Arguments, description: $"{results.Count} pools found.")
                    .SetFooter("Additional commands:\n!show {ID}\n!showall {ID}\n\nex. !show 97728");
                foreach (var pool in results)
                {
                    embed.AddField(pool.ID, $"[{pool.Name}]({pool.Url}{pool.ID})");
                }

                await apiClient.PostMessage(embed, commandInfo.Channel.ID);      

                commandInfo.CommandHandler.AddTemporaryCommandHandler(CreateTemporaryPoolsHandler(results, commandInfo), commandInfo.Channel.ID);
            }
            else
            {
                await apiClient.PostMessage("Something went wrong", commandInfo.Channel.ID);
            }
        }

        [Command("yanderepool")]
        private async Task ShowPool(CommandInfo commandInfo)
        {
            if (string.IsNullOrEmpty(commandInfo.Arguments))
            {
                await apiClient.PostMessage("Please specify pool ID", commandInfo.Channel.ID);
                return;
            }

            var result = await yandereAPIClient.GetPool(commandInfo.Arguments);
            if (result != null)
            {
                Embed embed = new Embed($"{result.ID}: {result.Name}", description: $"{result.PostCount} posts in pool.")
                    .SetFooter("Additional commands:\n!show {ID}\n\nex. !show 749488");
                foreach (var image in result.Posts)
                {
                    embed.AddField(image.ID, $"[Link]({image.Url})\nRating: {image.Rating}");
                }

                await apiClient.PostMessage(embed, commandInfo.Channel.ID);

                commandInfo.CommandHandler.AddTemporaryCommandHandler(CreateTemporaryPostsHandler(result, commandInfo), commandInfo.Channel.ID);
            }
            else
            {
                await apiClient.PostMessage("Something went wrong", commandInfo.Channel.ID);
            }
        }

        [Command("yanderepoolall")]
        private async Task ShowPoolImages(CommandInfo commandInfo)
        {
            if (string.IsNullOrEmpty(commandInfo.Arguments))
            {
                await apiClient.PostMessage("Please specify pool ID", commandInfo.Channel.ID);
                return;
            }

            var result = await yandereAPIClient.GetPool(commandInfo.Arguments);
            if (result != null)
            {
                //Embed embed = new Embed($"{result.ID}: {result.Name}", description: $"{result.PostCount} posts in pool.");
                //foreach (var image in result.Posts)
                //{
                //    embed.AddField(image.ID, $"{image.FileUrl}");
                //}
                string message = "";
                foreach (var image in result.Posts)
                {
                    message += $"{image.FileUrl}\n";
                }

                await apiClient.PostMessage(message, commandInfo.Channel.ID);

                commandInfo.CommandHandler.AddTemporaryCommandHandler(CreateTemporaryPostsHandler(result, commandInfo), commandInfo.Channel.ID);
            }
            else
            {
                await apiClient.PostMessage("Something went wrong", commandInfo.Channel.ID);
            }
        }

        private TemporaryCommandHandler CreateTemporaryPoolsHandler(List<YandereAPIClient.Pool> pools, CommandInfo commandInfo)
        {
            var tempHandler = new TemporaryCommandHandler(commandInfo.DiscordBot, commandInfo.CommandHandler, commandInfo.Sender, "!")
                    .AddCommand("!show", async (commandInfo) => 
                    {
                        try
                        {
                            var pool = pools.FirstOrDefault(p => p.ID == commandInfo.Arguments);
                            if (pool != null)
                            {
                                //TODO: set additional commandInfo
                                commandInfo.Arguments = pool.ID;
                                await ShowPool(commandInfo);
                            }
                            else
                            {
                                await apiClient.PostMessage($"There is no pool with ID: {commandInfo.Arguments} in previous query",
                                    commandInfo.Channel.ID);
                            }
                        }
                        catch (Exception ex)
                        {
                            await apiClient.PostMessage($"Something went wrong", commandInfo.Channel.ID);
                            logger.LogException(ex);
                        }
                    })
                    .AddCommand("!showall", async (commandInfo) =>
                    {
                        try
                        {
                            var pool = pools.FirstOrDefault(p => p.ID == commandInfo.Arguments);
                            if (pool != null)
                            {
                                //TODO: set additional commandInfo
                                commandInfo.Arguments = pool.ID;
                                await ShowPoolImages(commandInfo);
                            }
                            else
                            {
                                await apiClient.PostMessage($"There is no pool with ID: {commandInfo.Arguments} in previous query",
                                    commandInfo.Channel.ID);
                            }
                        }
                        catch (Exception ex)
                        {
                            await apiClient.PostMessage($"Something went wrong", commandInfo.Channel.ID);
                            logger.LogException(ex);
                        }
                    });

            return tempHandler;
        }

        private TemporaryCommandHandler CreateTemporaryPostsHandler(YandereAPIClient.Pool pool, CommandInfo commandInfo)
        {
            var tempHandler = new TemporaryCommandHandler(commandInfo.DiscordBot, commandInfo.CommandHandler, commandInfo.Sender, "!")
                    .AddCommand("!show", async (commandInfo) =>
                    {
                        try
                        {
                            var post = pool.Posts.FirstOrDefault(p => p.ID == commandInfo.Arguments);
                            if (post != null)
                            {
                                await apiClient.PostMessage($"{post.Url}", commandInfo.Channel.ID);
                            }
                            else
                            {
                                await apiClient.PostMessage($"There is no post with ID: {commandInfo.Arguments} in previous pool", 
                                    commandInfo.Channel.ID);
                            }
                        }
                        catch (Exception ex)
                        {
                            logger.LogException(ex);
                        }
                    });

            return tempHandler;
        }
    }
}
