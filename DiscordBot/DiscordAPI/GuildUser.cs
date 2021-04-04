using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiscordBot.DiscordAPI
{
    public class GuildUser
    {
        public User User { get; set; }
        public string? Nickname { get; set; }

        public List<Role> Roles { get; set; }
        public Role HoistedRole 
        { 
            get { return Roles?.OrderByDescending(r => r.Position).FirstOrDefault(r => r.Hoist); }
        }

        //TODO: Calculate color based on hoisted roles
        public int RoleColor { get; set; }
    }
}
