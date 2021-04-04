using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Navigation;

namespace DiscordBot.DiscordAPI
{
    public class User
    {
        public string ID { get; set; }
        public string Username { get; set; }

        // The four-digit number after a user name (ex. UserName#2000)
        public string Discriminator { get; set; }

        // Nicknames are server-specific
        public string Nickname { get; set; }

        public string AvatarHash { get; set; }

        public List<Role> Roles { get; set; }

        // Is this user a bot
        public bool Bot { get; set; }

        public string FullUsername { get => Username + "#" + Discriminator; }

        //If the user has a nickname, there will be an '!' after the '@'
        public string Mention { get => string.IsNullOrEmpty(Nickname) ? $"<@{ID}>" : $"<@!{ID}>"; }

        public string AvatarUrl
        {
            get 
            {
                if (AvatarHash == null) { return null; }
                string imageExtension = AvatarHash.StartsWith("a_") ? ".gif" : ".png";
                return string.Format(DiscordEndpoints.USER_AVATAR, ID, AvatarHash + imageExtension);
            }
        }
    }
}
