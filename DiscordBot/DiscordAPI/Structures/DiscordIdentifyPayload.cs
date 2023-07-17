using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.DiscordAPI.Structures
{
    /// <summary>
    /// Payload associated with OpCode 2: Identify
    /// </summary>
    internal class DiscordIdentifyPayload : DiscordStructure
    {
        public DiscordIdentifyPayload(string token, ConnectionProperties properties, 
            int intents, int largeThreshold = 50, bool compress = false) 
        {
            Token = token;
            Properties = properties;
            Intents = intents;
            LargeThreshold = largeThreshold;
            Compress = compress;
        }

        [Flags]
        public enum GatewayIntents
        {
            None = 0,
            Guilds = 1 << 0,
            GuildMembers = 1 << 1,
            GuildModeration = 1 << 2,
            GuildEmojisAndStickers = 1 << 3,
            GuildIntegrations = 1 << 4,
            GuildWebhooks = 1 << 5,
            GuildInvites = 1 << 6,
            GuildVoiceStates = 1 << 7,
            GuildPresences = 1 << 8,
            GuildMessages = 1 << 9,
            GuildMessageReactions = 1 << 10,
            GuildMessageTyping = 1 << 11,
            DirectMessages = 1 << 12,
            DirectMessageReactions = 1 << 13,
            DirectMessageTyping = 1 << 14,
            MessageContent = 1 << 15,
            GuildScheduledEvents = 1 << 16,
            AutoModerationConfiguration = 1 << 20,
            AutoModerationExecutions = 1 << 21
        }

        /// <summary>
        /// Authentication token
        /// </summary>
        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("properties")]
        public ConnectionProperties Properties { get; set; }

        [JsonProperty("large_threshold", NullValueHandling = NullValueHandling.Ignore)]
        public int? LargeThreshold { get; set; }

        [JsonProperty("compress", NullValueHandling = NullValueHandling.Ignore)] //false by default
        public bool? Compress { get; set; }

        //TODO: Implement these payloads
        //Shard
        //Presence

        /// <summary>
        /// Gateway Intents you wish to receive 
        /// </summary>
        [JsonProperty("intents")]
        public int Intents { get; set; }

        public struct ConnectionProperties
        {
            [JsonProperty("os", NullValueHandling = NullValueHandling.Ignore)]
            public string? OS { get; set; }

            [JsonProperty("browser", NullValueHandling = NullValueHandling.Ignore)]
            public string? Browser { get; set; }

            [JsonProperty("device", NullValueHandling = NullValueHandling.Ignore)]
            public string? Device { get; set; }
        }

        /**
         * //TODO: Create an Identify class to send this data instead of manual JSON LINQ
            JObject sendPacket = new JObject();
            sendPacket["op"] = 2;
            sendPacket["d"] = new JObject();
            sendPacket["d"]["token"] = token;
            sendPacket["d"]["v"] = 1;
            sendPacket["d"]["properties"] = new JObject();
            sendPacket["d"]["properties"]["device"] = ".Net";
            sendPacket["large_threshold"] = 50; // Number of offline members this client will receive
            sendPacket["compress"] = false;
            //TODO: send Gateway Intents

         */

        /**
         * GUILDS (1 << 0)
  - GUILD_CREATE
  - GUILD_UPDATE
  - GUILD_DELETE
  - GUILD_ROLE_CREATE
  - GUILD_ROLE_UPDATE
  - GUILD_ROLE_DELETE
  - CHANNEL_CREATE
  - CHANNEL_UPDATE
  - CHANNEL_DELETE
  - CHANNEL_PINS_UPDATE
  - THREAD_CREATE
  - THREAD_UPDATE
  - THREAD_DELETE
  - THREAD_LIST_SYNC
  - THREAD_MEMBER_UPDATE
  - THREAD_MEMBERS_UPDATE *
  - STAGE_INSTANCE_CREATE
  - STAGE_INSTANCE_UPDATE
  - STAGE_INSTANCE_DELETE

GUILD_MEMBERS (1 << 1) **
  - GUILD_MEMBER_ADD
  - GUILD_MEMBER_UPDATE
  - GUILD_MEMBER_REMOVE
  - THREAD_MEMBERS_UPDATE *

GUILD_MODERATION (1 << 2)
  - GUILD_AUDIT_LOG_ENTRY_CREATE
  - GUILD_BAN_ADD
  - GUILD_BAN_REMOVE

GUILD_EMOJIS_AND_STICKERS (1 << 3)
  - GUILD_EMOJIS_UPDATE
  - GUILD_STICKERS_UPDATE

GUILD_INTEGRATIONS (1 << 4)
  - GUILD_INTEGRATIONS_UPDATE
  - INTEGRATION_CREATE
  - INTEGRATION_UPDATE
  - INTEGRATION_DELETE

GUILD_WEBHOOKS (1 << 5)
  - WEBHOOKS_UPDATE

GUILD_INVITES (1 << 6)
  - INVITE_CREATE
  - INVITE_DELETE

GUILD_VOICE_STATES (1 << 7)
  - VOICE_STATE_UPDATE

GUILD_PRESENCES (1 << 8) **
  - PRESENCE_UPDATE

GUILD_MESSAGES (1 << 9)
  - MESSAGE_CREATE
  - MESSAGE_UPDATE
  - MESSAGE_DELETE
  - MESSAGE_DELETE_BULK

GUILD_MESSAGE_REACTIONS (1 << 10)
  - MESSAGE_REACTION_ADD
  - MESSAGE_REACTION_REMOVE
  - MESSAGE_REACTION_REMOVE_ALL
  - MESSAGE_REACTION_REMOVE_EMOJI

GUILD_MESSAGE_TYPING (1 << 11)
  - TYPING_START

DIRECT_MESSAGES (1 << 12)
  - MESSAGE_CREATE
  - MESSAGE_UPDATE
  - MESSAGE_DELETE
  - CHANNEL_PINS_UPDATE

DIRECT_MESSAGE_REACTIONS (1 << 13)
  - MESSAGE_REACTION_ADD
  - MESSAGE_REACTION_REMOVE
  - MESSAGE_REACTION_REMOVE_ALL
  - MESSAGE_REACTION_REMOVE_EMOJI

DIRECT_MESSAGE_TYPING (1 << 14)
  - TYPING_START

MESSAGE_CONTENT (1 << 15) ***

GUILD_SCHEDULED_EVENTS (1 << 16)
  - GUILD_SCHEDULED_EVENT_CREATE
  - GUILD_SCHEDULED_EVENT_UPDATE
  - GUILD_SCHEDULED_EVENT_DELETE
  - GUILD_SCHEDULED_EVENT_USER_ADD
  - GUILD_SCHEDULED_EVENT_USER_REMOVE

AUTO_MODERATION_CONFIGURATION (1 << 20)
  - AUTO_MODERATION_RULE_CREATE
  - AUTO_MODERATION_RULE_UPDATE
  - AUTO_MODERATION_RULE_DELETE

AUTO_MODERATION_EXECUTION (1 << 21)
  - AUTO_MODERATION_ACTION_EXECUTION
         */
    }
}
