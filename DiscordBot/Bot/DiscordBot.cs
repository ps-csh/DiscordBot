using DiscordBot.DiscordAPI;
using DiscordBot.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using DiscordBot.DiscordAPI.Utility;
using DiscordBot.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Principal;
using DiscordBot.DiscordAPI.Structures;
using DiscordBot.Bot.Commands;
using Microsoft.Extensions.Options;
using DiscordBot.Configuration;

namespace DiscordBot.Bot
{
    public class DiscordBotManager : INotifyPropertyChanged
    {
        public DiscordGatewayClient GatewayClient { get; private set; }
        public DiscordApiClient ApiClient { get; private set; }
        //TODO: Create VOIP client

        public DiscordBotUser BotUser { get; private set; }

        public DatabaseContext DbContext { get; private set; }

        // Only track own guild for now. No plans to make the bot public.
        public Guild ActiveGuild { get; private set; }

        public List<Channel> DMChannels { get; private set; }

        public string OwnerID { get; private set; }
        public string OwnerDMChannel { get; private set; }

        public CommandHandler CommandHandler { get; private set; }

        private readonly ILogger Logger;

        public DiscordBotManager(DiscordApiClient apiClient, DiscordGatewayClient gatewayClient, DatabaseContext databaseContext,
            CommandHandler commandHandler, ILogger logger, IOptions<BotSettingsConfiguration> options)
        {
            ApiClient = apiClient; 
            GatewayClient = gatewayClient;

            DbContext = databaseContext;
            BotUser = new DiscordBotUser();
            CommandHandler = commandHandler;
            Logger = logger;
            OwnerID = options.Value.AdminID;

            GatewayClient.AddMessageCallback(HandleGatewayEvent);
        }

        void HandleGatewayEvent(DiscordGatewayPayload payload)
        {
            switch (payload.EventType)
            {
                case "MESSAGE_CREATE":
                    OnMessageCreate(payload);
                    break;
                case "GUILD_CREATE":
                    CreateGuild(payload.EventData);
                    break;
                default:
                    break;
            }
        }

        void OnMessageCreate(DiscordGatewayPayload payload)
        {
            try
            {
                Message message = payload.EventData.ToObject<Message>();

                FullMessage fullMessage = new FullMessage(this, message);

                CommandHandler?.HandleMessage(fullMessage);
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }

        void CreateGuild(JObject payload)
        {
            //TODO: Deserialize Json normally instead of manual parsing
            Guild guild = DiscordJsonParser.ParseGuildCreate(payload);

            if (guild != null)
            {
                ActiveGuild = guild;
                ActiveGuild.TextChannels.Sort((x, y) => x.Position.CompareTo(y.Position));
                NotifyPropertyChanged("ActiveGuild");
            }
            else
            {
                Logger.LogWarning("Guild could not be created");
            }
        }

        #region INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
