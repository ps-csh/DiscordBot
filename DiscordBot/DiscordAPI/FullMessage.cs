using DiscordBot.Bot;
using DiscordBot.DiscordAPI.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiscordBot.DiscordAPI
{
    /// <summary>
    /// Gets full data (User, Guild, etc.) from a message using Lazy Loading
    /// </summary>
    public class FullMessage
    {
        public FullMessage(DiscordBotManager discordBot, Message message)
        {
            DiscordBot = discordBot;

            ID = message.ID;
            Sender = discordBot.ActiveGuild.Users.FirstOrDefault(u => u.ID == message.User.ID);
            Content = message.Content;
            Channel = message.GuildID != null ?
                    discordBot.ActiveGuild.TextChannels.FirstOrDefault(c => c.ID == message.ChannelID) :
                    discordBot.DMChannels.FirstOrDefault(c => c.ID == message.ChannelID);
            guild = new Lazy<Guild?>(() => discordBot.ActiveGuild);
            mentions = new Lazy<IEnumerable<User>>(() => discordBot.ActiveGuild.Users.Where(u => message.Mentions.Any(m => m.ID == u.ID)));
            mentionRoles = new Lazy<IEnumerable<Role>>(() => discordBot.ActiveGuild.Roles.Where(r => message.MentionRoles.Contains(r.ID)));
            //TODO: Get list of channels from DMs or other based on channel type
            mentionChannels = new Lazy<IEnumerable<Channel>>(() => 
                discordBot.ActiveGuild.TextChannels.Where(u => message.MentionChannels.Any(m => m.ID == u.ID)));
        }

        public DiscordBotManager DiscordBot { get; set; }

        /// <summary>
        /// Message ID
        /// </summary>
        public string ID { get; set; }
        public User Sender { get; set; }
        public string Content { get; set; }
        public Channel Channel { get; set; }

        private Lazy<Guild?> guild;
        public Guild? Guild { get => guild.Value; }

        private Lazy<IEnumerable<User>> mentions;
        public IEnumerable<User> Mentions { get => mentions.Value; }

        private Lazy<IEnumerable<Role>> mentionRoles;
        public IEnumerable<Role> MentionRoles { get => mentionRoles.Value; }

        private Lazy<IEnumerable<Channel>> mentionChannels;
        public IEnumerable<Channel> MentionChannels { get => mentionChannels.Value; }


        // Text-to-speech
        public bool Tts { get; set; }
        public string File { get; set; }
        public List<Embed> Embeds { get; set; }
        // Nonce can accept an integer value
        public string Nonce { get; set; }
    }
}
