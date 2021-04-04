using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot.DiscordAPI.Structures
{
    public class DiscordGuildMember : DiscordStructure
    {
        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// User is not included in MESSAGE_CREATE and MESSAGE_UPDATE events
        /// </remarks>
        [JsonProperty("user")]
        public DiscordUser? User { get; set; }

        [JsonProperty("nick")]
        public string? Nickname { get; set; }

        [JsonProperty("roles")]
        public List<string> Roles { get; set; }

        /// <summary>
        /// ISO8601 timestamp of when the user joined the guild
        /// </summary>
        [JsonProperty("joined_at")]
        public string JoinedAt { get; set; }

        /// <summary>
        /// ISO8601 timestamp of when the user joined the guild
        /// </summary>
        [JsonProperty("premium_since")]
        public string? PremiumSince { get; set; }

        /// <summary>
        /// If the user is deafened in voice channels
        /// </summary>
        [JsonProperty("deaf")]
        public string Deaf { get; set; }

        /// <summary>
        /// If the user is muted in voice channels
        /// </summary>
        [JsonProperty("mute")]
        public string Mute { get; set; }

        /// <summary>
        /// If the user has not yet passed the guild's Membership Screening requirements
        /// </summary>
        [JsonProperty("pending")]
        public bool? Pending { get; set; }

        [JsonProperty("permissions")]
        public string? Permissions { get; set; }
    }

    /**
     * user?	user object	the user this guild member represents
nick?	?string	this users guild nickname
roles	array of snowflakes	array of role object ids
joined_at	ISO8601 timestamp	when the user joined the guild
premium_since	?ISO8601 timestamp	when the user started boosting the guild
deaf	boolean	whether the user is deafened in voice channels
mute	boolean	whether the user is muted in voice channels
pending?	boolean	whether the user has not yet passed the guild's Membership Screening requirements
permissions?	string	total permissions of the member in the channel, including overrides, returned when in the interaction object
     */
}
