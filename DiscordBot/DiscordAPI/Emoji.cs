using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot.DiscordAPI
{
    public class Emoji
    {
        public string ID { get; set; }
        public string Name { get; set; }

        // The Roles that can use this emoji (Nitro users only?)
        public List<string> Roles { get; set; } = new List<string>();

        // The User that created this emoji
        public User User { get; set; }

        public bool RequireColons { get; set; }
        public bool Managed { get; set; }
        public bool Animated { get; set; }

        public string FormattedString
        {
            //TODO: If Guild emoji can have RequireColons == false, check when formatting string
            get { return $"<:{Name}:{ID}>"; }
        }

        public string IconUrl
        {
            get { return string.Format(DiscordEndpoints.EMOJI, ID + (Animated ? ".gif" : ".png")); }
        }
        /*
         * {
  "id": "41771983429993937",
  "name": "LUL",
  "roles": ["41771983429993000", "41771983429993111"],
  "user": {
    "username": "Luigi",
    "discriminator": "0002",
    "id": "96008815106887111",
    "avatar": "5500909a3274e1812beb4e8de6631111"
  },
  "require_colons": true,
  "managed": false,
  "animated": false
}
         */
    }
}
