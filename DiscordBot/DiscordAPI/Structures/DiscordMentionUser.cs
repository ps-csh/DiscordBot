using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot.DiscordAPI.Structures
{
    public class DiscordMentionUser : DiscordStructure
    {
        [JsonProperty("id")]
        public string ID { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("discriminator")]
        public string Discriminator { get; set; }

        /// <summary>
        /// The user's avatar hash
        /// </summary>
        [JsonProperty("avatar")]
        public string? Avatar { get; set; }

        [JsonProperty("bot")]
        public bool? Bot { get; set; }

        [JsonProperty("public_flags")]
        public int? PublicFlags { get; set; }

        [JsonProperty("member")]
        public DiscordGuildMember Member { get; set; }
    }
}
