using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DiscordBot.DiscordAPI;
using DiscordBot.Utility;
using Newtonsoft.Json.Linq;

namespace DiscordBot.DiscordAPI.Utility
{
    //DELETE: after changing GuildCreate to deserialize directly
    public static class DiscordJsonParser
    {
        public static Guild ParseGuildCreate(JObject guildCreateEventData)
        {
            //"roles"
            Guild guild = GuildFromJson(guildCreateEventData);
            if (guild == null)
            {
                return null;
            }

            JArray roles = (JArray)guildCreateEventData["roles"];
            foreach (JObject roleData in roles.Children<JObject>())
            {
                Role role = RoleFromJson(roleData);
                if (role != null)
                {
                    guild.Roles.Add(role);
                }
            }

            //"members"
            JArray users = (JArray)guildCreateEventData["members"];
            foreach (JObject userData in users.Children<JObject>())
            {
                User user = GuildUserFromJson(userData, guild.Roles);
                if (user != null)
                {
                    guild.Users.Add(user);
                }
            }

            //"channels"
            JArray channels = (JArray)guildCreateEventData["channels"];
            foreach (JObject channelData in channels.Children<JObject>())
            {
                Channel channel = ChannelFromJson(channelData);
                if (channel != null)
                {
                    if (channel.Type == Channel.ChannelType.GuildText)
                    {
                        guild.TextChannels.Add(channel);
                    }
                    else if (channel.Type == Channel.ChannelType.GuildVoice)
                    {
                        guild.VoiceChannels.Add(channel);
                    }
                }
            }

            //Emojis
            JArray emojis = (JArray)guildCreateEventData["emojis"];
            foreach (JObject emojiData in emojis.Children<JObject>())
            {
                //TODO: Pass Roles to method to set them for Emoji data
                Emoji emoji = EmojiFromJson(emojiData);
                if (emoji != null)
                {
                    guild.Emojis.Add(emoji);
                }
            }

            return guild;
        }

        public static Guild GuildFromJson(JObject guildJson)
        {
            try
            {
                string id = guildJson["id"].ToString();
                string name = guildJson["name"].ToString();
                string iconHash = guildJson["icon"].ToString();
                string ownerId = guildJson["owner_id"].ToString();

                Guild guild = new Guild()
                {
                    ID = id,
                    Name = name,
                    IconHash = iconHash,
                    OwnerID = ownerId
                };

                return guild;
            }
            catch(Exception ex)
            {
                //Logger.LogWarning("Exception thrown when creating guild: " + ex.Message);
            }

            return null;
        }

        public static User GuildUserFromJson(JObject userJson, List<Role> roles)
        {
            try
            {
                JObject userObject = (JObject)userJson["user"];

                string username = userObject["username"].ToString();
                string id = userObject["id"].ToString();
                string discriminator = userObject["discriminator"].ToString();
                string avatarHash = userObject["avatar"].ToString(); //avatar is guaranteed to exist in Json, but may have a value of null
                bool isBot = userObject["bot"]?.ToObject<bool>() ?? false;

                string nickname = userJson["nick"]?.ToString();

                JArray userRoles = (JArray)userJson["roles"];
                string[] roleIDs = userRoles.ToObject<string[]>();

                // TODO: Set full Roles
                // Create a list of Role IDs to set full roles after
                List<Role> fullRoles = new List<Role>();
                foreach (string roleId in roleIDs)
                {
                    var fullRole = roles.FirstOrDefault(r => r.ID == roleId);
                    if (fullRole != null) { fullRoles.Add(fullRole); }                  
                }

                User user = new User()
                {
                    ID = id,
                    Username = username,
                    Discriminator = discriminator,
                    Nickname = nickname,
                    AvatarHash = avatarHash,
                    Bot = isBot,
                    Roles = fullRoles
                };

                return user;
            }
            catch(Exception ex)
            {
                //Logger.LogException(ex);
            }

            return null;
        }

        public static Channel ChannelFromJson(JObject channelJson)
        {
            /*
             * public string ID { get; set; }
        public ChannelType Type { get; set; }
        public string Name { get; set; }
        public string Topic { get; set; }
        public bool Nsfw { get; set; }
             */
            try
            {
                string id = channelJson["id"].ToString();
                int type = channelJson["type"].ToObject<int>();
                string name = channelJson["name"]?.ToString();
                string topic = channelJson["topic"]?.ToString();
                int position = channelJson["position"]?.ToObject<int>() ?? 999;
                bool nsfw = channelJson["nsfw"]?.ToObject<bool>() ?? false;

                Channel channel = new Channel()
                {
                    ID = id,
                    Type = (Channel.ChannelType)type,
                    Name = name,
                    Topic = topic,
                    Position = position,
                    Nsfw = nsfw
                };

                return channel;
            }
            catch(Exception ex)
            {
                //Logger.LogWarning("Exception thrown when creating channel: " + ex.Message);
            }
            
            return null;
        }

        public static Role RoleFromJson(JObject roleJson)
        {
            try
            {
                string id = roleJson["id"].ToString();
                string name = roleJson["name"].ToString();
                int color = roleJson["color"].ToObject<int>();
                bool hoist = roleJson["hoist"].ToObject<bool>();
                int position = roleJson["position"].ToObject<int>();
                int permissions = roleJson["permissions"].ToObject<int>();
                bool mentionable = roleJson["mentionable"].ToObject<bool>();
                bool managed = roleJson["managed"].ToObject<bool>();

                Role role = new Role()
                {
                    ID = id,
                    Name = name,
                    Color = color,
                    Hoist = hoist,
                    Position = position,
                    Permissions = permissions,
                    Mentionable = mentionable,
                    Managed = managed
                };

                return role;
            }
            catch(Exception ex)
            {
                //Logger.LogWarning("Exception thrown when creating role: " + ex.Message);
            }

            return null;
        }

        public static Emoji EmojiFromJson(JObject channelJson)
        {
            try
            {
                string id = channelJson["id"].ToString();
                string name = channelJson["name"].ToString();
                //TODO: roles
                string userID = channelJson["user"]?["id"].ToString();
                bool requireColons = channelJson["require_colons"]?.ToObject<bool>() ?? true;
                bool managed = channelJson["managed"]?.ToObject<bool>() ?? false;
                bool animated = channelJson["animated"]?.ToObject<bool>() ?? false;
                //"available": true - this is not listed in the docs, possibly for cross-server emotes?

                Emoji emoji = new Emoji()
                {
                    ID = id,
                    Name = name,
                    User = new User() { ID = userID },
                    RequireColons = requireColons,
                    Managed = managed,
                    Animated = animated
                };

                return emoji;
            }
            catch (Exception ex)
            {
                //Logger.LogWarning("Exception thrown when creating emoji: " + ex.Message);
            }

            return null;
        }
    }
}
