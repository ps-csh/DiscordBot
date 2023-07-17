using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot.DiscordAPI.Structures.Voice
{
    public class DiscordVoiceReadyPayload : DiscordStructure
    {
        [JsonProperty("ssrc")]
        //synchronization source?
        public string SSRC { get; set; }

        [JsonProperty("ip")]
        public string IP { get; set; }

        [JsonProperty("port")]
        public int Port { get; set; }

        [JsonProperty("modes")]
        public string[] Modes { get; set; }

        //Heartbeat_Interval may be present in the payload, but it is an erroneous field
    }
}
