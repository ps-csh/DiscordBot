using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot.DiscordAPI.Structures
{
    public class DiscordMentionChannel
    {
        [JsonProperty("id")]
        public string ID { get; set; }

        [JsonProperty("guild_id")]
        public string GuildID { get; set; }

        [JsonProperty("type")]
        public int Type { get; set; }

        /// <summary>
        /// The name of the channel
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }
    }

    /**
     * id	snowflake	id of the channel
        guild_id	snowflake	id of the guild containing the channel
        type	integer	the type of channel
        name	string	the name of the channel

        GUILD_TEXT	0	a text channel within a server
        DM	1	a direct message between users
        GUILD_VOICE	2	a voice channel within a server
        GROUP_DM	3	a direct message between multiple users
        GUILD_CATEGORY	4	an organizational category that contains up to 50 channels
        GUILD_NEWS	5	a channel that users can follow and crosspost into their own server
        GUILD_STORE	6	a channel in which game developers can sell their game on Discord
     */
}
