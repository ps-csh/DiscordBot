using DiscordBot.Bot;
using DiscordBot.Bot.Commands;
using DiscordBot.DiscordAPI;
using DiscordBot.DiscordAPI.Structures;
using DiscordBot.Models;
using DiscordBot.Utility;
using DiscordBot.Utility.Web;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Modules
{
    public class DefaultCommands : CommandModule
    {
        private readonly DatabaseContext dbContext;
        private readonly DiscordApiClient apiClient;
        private readonly ILogger logger;
        private readonly DiscordApiRequestHandler requestHandler;

        /// <summary>
        /// Loads services from ServiceProvider.
        /// Will throw an exception if a required service isn't found.
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public DefaultCommands(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            dbContext = serviceProvider.GetRequiredService<DatabaseContext>();
            apiClient = serviceProvider.GetRequiredService<DiscordApiClient>();
            logger = serviceProvider.GetRequiredService<ILogger>();
            requestHandler = serviceProvider.GetRequiredService<DiscordApiRequestHandler>();
        }

        /// <summary>
        /// Commands take the form:
        /// <para>
        /// <c>Task Method(CommandInfo info)</c>
        /// </para>
        /// <see cref="BotCommand.CommandHandler"/>
        /// </summary>
        //DELETE: Commands are now loaded by reflection instead
        public void LoadCommands(CommandHandler commandHandler)
        {
            //commandHandler.BotCommands.Add("addquote", new BotCommand(AddQuote));
            //commandHandler.BotCommands.Add("null", new BotCommand(null));
            //commandHandler.BotCommands.Add("quote", new BotCommand(RandomQuote));
            //commandHandler.BotCommands.Add("deletequote", new BotCommand(DeleteQuote, 
            //    new Permissions().SetUsers(commandHandler.DiscordBot.OwnerID)));
        }

        [Command("addquote")]
        private async Task AddQuote(CommandInfo commandInfo)
        {
            Quote quote = new Quote() { Message = commandInfo.Arguments };

            try
            {
                dbContext.Quotes.Add(quote);
                await dbContext.SaveChangesAsync();

                await apiClient.PostMessage("Quote added.", commandInfo.Channel.ID);

                //Logger.LogInfo("Quote added:\n" + quote.Message);
            }
            catch (Exception ex)
            {
                await apiClient.PostMessage("Something went wrong.", commandInfo.Channel.ID);

                logger.LogException(ex);
            }
        }

        [Command("quote")]
        private async Task RandomQuote(CommandInfo commandInfo)
        {
            try
            {
                var quotes = await dbContext.Quotes.ToListAsync();
                int index = new Random().Next(0, quotes.Count);
                var quote = quotes.ElementAtOrDefault(index);
                if (quote != null)
                {
                    await apiClient.PostMessage(quote.Message, commandInfo.Channel.ID);
                }
                else
                {
                    await apiClient.PostMessage("Sorry, I couldn't find any quotes.", commandInfo.Channel.ID);

                    logger.LogWarning($"Quote was null. Index: {index}");
                }
            }
            catch (Exception ex)
            {
                await apiClient.PostMessage("Something went wrong.", commandInfo.Channel.ID);

                logger.LogException(ex);
            }
        }

        [Command("deletequote")]
        private async Task DeleteQuote(CommandInfo commandInfo)
        {
            try
            {
                if (int.TryParse(commandInfo.Arguments, out int id))
                {
                    Quote quote = await dbContext.Quotes.FirstOrDefaultAsync(q => q.Id == id);
                    if (quote != null)
                    {
                        dbContext.Quotes.Remove(quote);
                        await dbContext.SaveChangesAsync();

                        await apiClient.PostMessage("Quote removed.", commandInfo.Channel.ID);

                        logger.LogInfo($"Quote removed:\n{quote.Id} {quote.Message}");
                    }
                    else
                    {
                        await apiClient.PostMessage($"Quote with ID {id} not found.", commandInfo.Channel.ID);
                    }
                }
                else
                {
                    await apiClient.PostMessage("Please enter a valid ID.", commandInfo.Channel.ID);
                }
            }
            catch (Exception ex)
            {
                await apiClient.PostMessage("Something went wrong.", commandInfo.Channel.ID);

                logger.LogException(ex);
            }
        }

        [Command("showratebuckets")]
        private async Task ShowRateBuckets(CommandInfo commandInfo)
        {
            try
            {
                var buckets = requestHandler.RateBuckets;
                var embed = new Embed("Rates", description: $"Rate Limit: {requestHandler.RateLimit}\nRate Reset: {requestHandler.RateReset}");
                foreach(var bucket in requestHandler.RateBuckets)
                {
                    embed.AddField($"{bucket.Id} {bucket.Endpoint}", $"Limit: {bucket.RequestLimit}\n" +
                        $"Remaining: {bucket.RequestsRemaining}\n{bucket.Semaphore?.ToString() ?? "Semaphore is null"}");
                }
                await apiClient.PostMessage(embed, commandInfo.Channel.ID);
            }
            catch (Exception ex)
            {
                await apiClient.PostMessage("Something went wrong.", commandInfo.Channel.ID);

                logger.LogException(ex);
            }
        }
    }
}
