using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot.DiscordAPI.Structures
{
    /// <summary>
    /// Payload structure received from Discord Gateway
    /// </summary>
    public class DiscordGatewayPayload
    {
        /// <summary>
        /// Payload type, commonly 0 (DISPATCH) for messages
        /// </summary>
        /// <remarks>
        /// For a list of opcodes:
        /// <see href="https://discord.com/developers/docs/topics/opcodes-and-status-codes#gateway-gateway-opcodes">
        /// Discord API - Gateway Opcodes
        /// </see>
        /// </remarks>
        [JsonProperty("op")]
        public int Opcode { get; set; }

        //TODO: If 'd' can contain arrays then this will need to change to JToken/JContainer
        /// <summary>
        /// Event data ('d') can contain any JSON object.
        /// It should be converted or parsed based on the event type ('t')
        /// </summary>
        [JsonProperty("d")]
        public JObject? EventData { get; set; }

        /// <summary>
        /// Sequence number used for heartbeats session resuming.
        /// Only used for opcode 0
        /// </summary>
        [JsonProperty("s")]
        public int? Sequence { get; set; }

        /// <summary>
        /// Payload event name.
        /// Only used for opcode 0
        /// </summary>
        /// <remarks>
        /// For list of event types:
        /// <see href="https://discord.com/developers/docs/topics/gateway#commands-and-events-gateway-events">
        /// Discord API - Commands and Events
        /// </see>
        /// </remarks>
        [JsonProperty("t")]
        public string? EventType { get; set; }
    }
}
