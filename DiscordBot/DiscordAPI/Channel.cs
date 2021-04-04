using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot.DiscordAPI
{
    public class Channel
    {
        public enum ChannelType: int
        {
            GuildText = 0,
            DM,
            GuildVoice,
            GuildDM,
            GuildCategory,
            GuildNews,
            GuildStore = 6
        }

        public string ID { get; set; }
        public ChannelType Type { get; set; }
        public string Name { get; set; }
        public string Topic { get; set; }
        public int Position { get; set; }
        public bool Nsfw { get; set; }
    }

    /*
     * {
  "id": "41771983423143937",
  "guild_id": "41771983423143937",
  "name": "general",
  "type": 0,
  "position": 6,
  "permission_overwrites": [],
  "rate_limit_per_user": 2,
  "nsfw": true,
  "topic": "24/7 chat about how to gank Mike #2",
  "last_message_id": "155117677105512449",
  "parent_id": "399942396007890945"
}

         * GUILD_TEXT	0	a text channel within a server
DM	1	a direct message between users
GUILD_VOICE	2	a voice channel within a server
GROUP_DM	3	a direct message between multiple users
GUILD_CATEGORY	4	an organizational category that contains up to 50 channels
GUILD_NEWS	5	a channel that users can follow and crosspost into their own server
GUILD_STORE	6	a channel in which game developers can sell their game on Discord
     */
}
