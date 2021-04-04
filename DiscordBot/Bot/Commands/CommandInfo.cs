using DiscordBot.DiscordAPI;
using DiscordBot.DiscordAPI.Structures;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot.Bot.Commands
{
    public class CommandInfo
    {
        public FullMessage Message { get; set; }
        public string Command { get; set; }
        public string Arguments { get; set; }
        public CommandHandler CommandHandler { get; set; }

        //TODO: Store IDs for User and Channel, then lazy load additional data
        public User Sender { get => Message.Sender; }
        public Channel Channel { get => Message.Channel; }

        public DiscordBotManager DiscordBot { get { return Message.DiscordBot; } }
        public DiscordApiClient ApiClient { get { return DiscordBot.ApiClient; } }
    }
}
