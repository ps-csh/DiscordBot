using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot.DiscordAPI
{
    public static class DiscordEndpoints
    {
        public const string BASE = "https://discordapp.com/api/";
        public const string CHANNEL = "https://discordapp.com/api/channels/";
        public const string GUILD = "https://discordapp.com/api/guild/";
        public const string WEBHOOK = "https://discordapp.com/api/webhook/";
        public const string USER = "https://discordapp.com/api/users/";

        /// <summary>
        /// Format: 
        /// <para>{0} = user_id</para>
        /// <para>{1} = user_avatar.png*</para>
        /// *can be one of .png, .jpg, .webp, .gif<br />
        /// for animated gifs, the avatar hash will begin with 'a_'
        /// </summary>
        public const string USER_AVATAR = "https://cdn.discordapp.com/avatars/{0}/{1}";

        /// <summary>
        /// Format: 
        /// <para>{0} = emoji_id.png*</para>
        /// *can be one of .png, .gif<br />
        /// </summary>
        public const string EMOJI = "https://cdn.discordapp.com/emojis/{0}";
    }
}
