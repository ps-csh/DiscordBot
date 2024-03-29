﻿using System;
using System.Collections.Generic;
using System.Text;
using DiscordBot.DiscordAPI;

namespace DiscordBot.Bot
{
    public class DiscordBotUser : User
    {
        public string Token { get; set; }

        public void FromUser(User user) 
        { 
            this.ID = user.ID;
            this.Roles = user.Roles;
            this.Username = user.Username;
            this.Bot = user.Bot;
            this.Discriminator = user.Discriminator;
            this.Nickname = user.Nickname;
            this.AvatarHash = user.AvatarHash;
        }
    }
}
