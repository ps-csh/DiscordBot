using DiscordBot.DiscordAPI;
using System;
using System.Collections.Generic;
using System.Text;
using static DiscordBot.Bot.BotCommand;

namespace DiscordBot.Bot
{
    public class ChannelHandler
    {
        public CommandHandler CommandHandler { get; protected set; }

        /// <summary>
        /// Handles temporary add-on commands from a previous command
        /// </summary>
        public CommandHandler TemporaryCommandHandler { get; set; }

        public ChannelHandler(CommandHandler commandHandler)
        {
            this.CommandHandler = commandHandler;
        }
    }
}
