using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot.Bot
{
    class BotResponse
    {
        // Messages to listen for
        public List<string> Messages { get; set; }

        // Possible responses
        public List<string> Responses { get; set; }

        public bool IgnoreCase { get; set; }

        public BotResponse(IEnumerable<string> messages, IEnumerable<string> responses, bool ignoreCase = false)
        {
            Messages = new List<string>(messages);
            Responses = new List<string>(responses);
            IgnoreCase = ignoreCase;
        }
    }
}
