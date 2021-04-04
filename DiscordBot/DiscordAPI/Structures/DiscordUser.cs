using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot.DiscordAPI.Structures
{
    public class DiscordUser : DiscordStructure
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

        /// <summary>
        /// If the user is an Official Discord System user
        /// </summary>
        [JsonProperty("system")]
        public bool? System { get; set; }

        /// <summary>
        /// If the user has two factor authentication
        /// </summary>
        [JsonProperty("mfa_enabled")]
        public bool? MFAEnabled { get; set; }

        [JsonProperty("locale")]
        public string? Locale { get; set; }

        [JsonProperty("verified")]
        public bool? Verified { get; set; }

        [JsonProperty("email")]
        public string? Email { get; set; }

        [JsonProperty("flags")]
        public int? Flags { get; set; }

        /// <summary>
        /// The user's Discord Nitro subscription type
        /// </summary>
        /// <remarks>
        /// <list type="number">
        /// <item>None</item>
        /// <item>Nitro Classic</item>
        /// <item>Nitro</item>
        /// </list>
        /// </remarks>
        [JsonProperty("premium_type")]
        public int? PremiumType { get; set; }

        [JsonProperty("public_flags")]
        public int? PublicFlags { get; set; }
    }
}

/**
 * id	snowflake	the user's id	identify
username	string	the user's username, not unique across the platform	identify
discriminator	string	the user's 4-digit discord-tag	identify
avatar	?string	the user's avatar hash	identify
bot?	boolean	whether the user belongs to an OAuth2 application	identify
system?	boolean	whether the user is an Official Discord System user (part of the urgent message system)	identify
mfa_enabled?	boolean	whether the user has two factor enabled on their account	identify
locale?	string	the user's chosen language option	identify
verified?	boolean	whether the email on this account has been verified	email
email?	?string	the user's email	email
flags?	integer	the flags on a user's account	identify
premium_type?	integer	the type of Nitro subscription on a user's account	identify
public_flags?	integer	the public flags on a user's account	identify
 */