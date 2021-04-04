using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot.Configuration
{
    public class BotSettingsConfiguration
    {
        /// <summary>
        /// The user ID of the bot owner. Admin bypasses command permissions.
        /// </summary>
        public string AdminID { get; set; }

        /// <summary>
        /// Users should start their message with one of these identifiers
        /// when sending commands to the bot.
        /// </summary>
        public List<string> CommandIdentifiers { get; set; }

        public string UserAgent { get; set; }
    }
}
