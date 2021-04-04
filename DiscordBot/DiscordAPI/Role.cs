using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot.DiscordAPI
{
    public class Role
    {
        public string ID { get; set; }
        public string Name { get; set; }

        // Hexadecimal integer representation of Color
        // If 0, it does not affect the final displayed color
        public int Color { get; set; }

        // Display this role separately from other roles
        public bool Hoist { get; set; }

        public int Position { get; set; }

        // Integer bit set of role permissions
        public int Permissions { get; set; }

        public bool Mentionable { get; set; }

        // Whether this Role is managed by an integration
        public bool Managed { get; set; }
    }
}
