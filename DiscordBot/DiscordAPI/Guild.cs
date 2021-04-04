using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot.DiscordAPI
{
    // Also known as a server
    public class Guild
    {
        public Guild()
        {
            Users = new List<User>();
            Roles = new List<Role>();
            TextChannels = new List<Channel>();
            VoiceChannels = new List<Channel>();
            Emojis = new List<Emoji>();
        }

        public string ID { get; set; }
        public string Name { get; set; }
        public string IconHash { get; set; }
        public string OwnerID { get; set; }

        public List<User> Users { get; set; }
        public List<Role> Roles { get; set; }
        public List<Channel> TextChannels { get; set; }
        public List<Channel> VoiceChannels { get; set; }
        public List<Emoji> Emojis { get; set; }
    }
}
